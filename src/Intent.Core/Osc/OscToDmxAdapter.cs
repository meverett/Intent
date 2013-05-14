using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Ports;

using Intent.Osc;
using Intent.Dmx;

using Bespoke.Common.Osc;
using IronJS;

namespace Intent.Osc
{
    /// <summary>
    /// Adapts incoming OSC messages to DMX message over usb/serial.
    /// </summary>
    [MessageAdapter("OSC to DMX")]
    public class OscToDmxAdapter : OscAdapter
    {
        #region Fields

        // Internal serial port used to communicate with usb/serial connection
        SerialPort port;

        // The serial port name to use for the DMX port
        string dmxPort;

        // The max DMX channel being used
        int maxChannel = 0;

        /// <summary>
        /// A list of setup messages to send once upon contact with the DMX controller.
        /// </summary>
        List<DmxMessage> setupMessages;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the default settings script for the OSC to DMX adapter.
        /// </summary>
        public override string DefaultSettingsScript
        {
            get { return "{ maxChannel: 512, setup: [] }"; }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates an OSC to DMX message adapter using localhost/loopback to listen
        /// for OSC messages on and sends them to the first available serial port as
        /// DMX command requests.
        /// </summary>
        public OscToDmxAdapter() : this (null, null) { }

        /// <summary>
        /// Creates an OSC to DMX message adapter using the provided IP endpoint to
        /// listen for OSC messages on and sends them to the first avilable serial port
        /// as DMX command requests.
        /// </summary>
        /// <param name="oscEndPoint">The IP endpoint to listen for incoming OSC messages on.</param>
        public OscToDmxAdapter(IPEndPoint oscEndPoint) : this(oscEndPoint, null) { }

        /// <summary>
        /// Creates an OSC to DMX message adapter using localhost/loopback to listen
        /// for OSC messages on and sends them to the specified serial port as DMX requests.
        /// </summary>
        /// <param name="oscEndPoint">The IP endpoint to listen for incoming OSC messages on.</param>
        public OscToDmxAdapter(string dmxPort) : this(null, dmxPort) { }

        /// <summary>
        /// Creates an OSC to DMX message adapter.
        /// </summary>
        /// <param name="oscEndPoint">The IP endpoint to listen for incoming OSC messages on.</param>
        /// <param name="dmxPort">The name of the serial port to send packaged DMX commands to.</param>
        public OscToDmxAdapter(IPEndPoint oscEndPoint, string dmxPort)
            : base(oscEndPoint)
        {
            // Create the internal serial port
            this.dmxPort = dmxPort;
            port = new SerialPort();
            port.BaudRate = 250000;
            port.DataReceived += port_DataReceived;
        }

        #endregion Constructors

        #region Methods

        #region Event Handlers

        // Handles data received by the DMX serial port
        void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string value = port.ReadLine().Trim();

            //Intent.WriteLine(value);

            switch (value)
            {
                // The other side was reset, so send setup info
                case "setup":
                    // Write out the max DMX Channel
                    byte[] buffer = GetMsbLsb(54);
                    port.Write(buffer, 0, buffer.Length);

                    // Notify
                    TriggerMessageSent();

                    // Send out any DMX-specific setup requests
                    ApplySetupMessages();

                    break;
            }
        }

        /// <summary>
        /// Handles a received OSC message.
        /// </summary>
        /// <param name="msg">The received OSC message.</param>
        /// <param name="data">The message's decoded data dictionary.</param>
        protected override void OnOscMessageReceived(OscMessage msg, Dictionary<string, string> data)
        {
            // Serial port is closed so nothing to do
            if (!port.IsOpen)
            {
                IntentRuntime.WriteLine("! Incoming OSC but serial port '{0}' is closed attempting to reconnect...", port.PortName);
                Stop();
                Start();
                return;
            }

            // If this is a multi channel/value message, parse out the individual channel messages
            if (data["channel"].Contains(','))
            {
                int valueIndex = 0;
                string[] values = data["value"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] channels = data["channel"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                // Go through each channel
                for (int c = 0; c < channels.Length; )
                {
                    // No more values available
                    if (valueIndex >= values.Length) break;

                    // Get the current raw value
                    string rawValue = values[valueIndex];

                    // This is an HSV value that wants to be converted to RGB and applied to 3 incoming channels
                    if (rawValue.Contains(';') && c + 2 < channels.Length)
                    {
                        #region Parse and apply HSV -> RGB conversion

                        // Parse out the separate hue, saturation, and value
                        string[] hsv = rawValue.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        if (hsv.Length < 3) continue; // incorrect number of values - we should have 3: H;S;V
                        float h = float.Parse(hsv[0]);
                        float s = float.Parse(hsv[1]);
                        float v = float.Parse(hsv[2]);
                        var color = HSVtoRGB(h, s, v);

                        // Send these to the next 3 channels assuming they are r, g, and b
                        WriteDmx(int.Parse(channels[c]),        color.R);
                        WriteDmx(int.Parse(channels[c + 1]),    color.G);
                        WriteDmx(int.Parse(channels[c + 2]),    color.B);

                        // Increment channel count by 3
                        c += 3;
                        #endregion Parse and apply HSV -> RGB conversion
                    }
                    // This was not a special HSV value that needed to be converted by
                    // this adapter so just pass the direct values through
                    else
                    {
                        #region Parse and apply

                        int value = int.Parse(rawValue);
                        int channel = int.Parse(channels[c]);
                        WriteDmx(channel, value);

                        // Increment the channels by one
                        c++;
                        #endregion Parse and apply
                    }

                    // Increment the value as needed
                    if (values.Length > 1) valueIndex++;
                }
                
            }
            // Otherwise just send on the single message
            else
            {
                int channel = int.Parse(data["channel"]);
                int value = int.Parse(data["value"]);
                WriteDmx(channel, value);
            }
        }

        #endregion Event Handlers

        #region Operation

        /// <summary>
        /// Simultaneously starts the OSC server and the DMX usb/serial connection.
        /// </summary>
        protected override void OnStart()
        {
            base.OnStart();

            // Load configuration
            if (maxChannel == 0 && File.Exists("dmx.js")) ApplySettings(File.ReadAllText("dmx.js"));
                
            if (!port.IsOpen)
            {
                try
                {
                    // Get available port names
                    string[] portNames = SerialPort.GetPortNames();

                    // Use the first available serial port if none was specified
                    if (dmxPort == null && portNames.Length > 0) dmxPort = portNames.First();

                    // Make sure at least one port was found
                    if (dmxPort == null) throw new ApplicationException("No serial ports are currently available.");

                    // Assign the port to open
                    port.PortName = dmxPort;

                    // Make sure the port exists
                    if (!portNames.Contains(port.PortName))
                        throw new ArgumentException("DMX serial port not found: " + port.PortName);

                    port.Open();

                    // Send out any DMX-specific setup requests
                    ApplySetupMessages();

                    HasErrors = false;

                    IntentRuntime.WriteLine("Sending DMX requests to serial port: {0}", port.PortName);
                }
                catch (Exception ex)
                {
                    IntentRuntime.WriteLine(ex);
                    HasErrors = true;
                }
            }
        }

        /// <summary>
        /// Simultaneously stops the OSC server and the DMX usb/serial connection.
        /// </summary>
        protected override void OnStop()
        {
            base.OnStop();

            if (port.IsOpen)
            {
                IntentRuntime.WriteLine("Closing DMX serial port: {0}", port.PortName);
                port.Close();
            }
        }

        // Applies any setup messages to the DMX controller state
        void ApplySetupMessages()
        {
            if (setupMessages == null) return;
            foreach (DmxMessage msg in setupMessages) WriteDmx(msg.Channel, msg.Value);
            IntentRuntime.WriteLine("{0} DMX setup messages applied", setupMessages.Count);
        }

        #endregion Operation

        #region Serial

        // Writes a value to a specified DMX channel
        void WriteDmx(int channel, int value)
        {
            var channelBuffer = GetMsbLsb(channel);
            byte[] buffer = new byte[] { channelBuffer[0], channelBuffer[1], (byte)(value & 0xff) };
            port.Write(buffer, 0, buffer.Length);

            // Notify
            TriggerMessageSent();
        }

        // Splits an INT into a 2 byte MSB + LSB byte buffer
        byte[] GetMsbLsb(int value)
        {
            return new byte[] { (byte)((value >> 8) & 0xff), (byte)(value & 0xff) };
        }

        #endregion Serial

        #region Settings

        protected override void OnApplySettings(CommonObject settings)
        {
            // Make sure the setup messages list is initialized
            if (setupMessages == null) setupMessages = new List<DmxMessage>();
            var members = settings.Members;

            // Set the max DMX channel used for operation
            if (members.ContainsKey("maxChannel"))
            {
                maxChannel = Convert.ToInt32(members["maxChannel"]);
            }

            // Load any setup messages
            if (members.ContainsKey("setup"))
            {
                setupMessages.Clear();

                var setupList = members["setup"] as ArrayObject;

                if (setupList == null)
                    IntentRuntime.WriteLine("DMX setup messages list is not formatted correctly:\n{0}\n", setupList);

                for (int i = 0; i < setupList.Length; i++)
                {
                    var msg = setupList.Get(i).Object;
                    int channel = Convert.ToInt32(msg.Members["channel"]);
                    int value = Convert.ToInt32(msg.Members["value"]);
                    setupMessages.Add(new DmxMessage(channel, value));
                }
            }

            IntentRuntime.WriteLine("DMX max output channel: {0}", maxChannel);
        }

        #endregion Settings

        #region Utilities

        // HSV -> RGB conversion
        ColorRGB HSVtoRGB (float h, float s, float v)
        {
            // hsv values = 0 - 1, rgb values = 0 - 255
            ColorRGB c = new ColorRGB();
            double var_r, var_g, var_b;

            if(s==0)
            {
                c.R = c.G = c.B = (int)Math.Round(v * 255);
            }
            else
            {
                // h must be < 1
                var var_h = h * 6;
                if (var_h==6) var_h = 0;
                //Or ... var_i = floor( var_h )
                var var_i = Math.Floor( var_h );
                var var_1 = v*(1-s);
                var var_2 = v*(1-s*(var_h-var_i));
                var var_3 = v*(1-s*(1-(var_h-var_i)));
                if(var_i==0){ 
                    var_r = v; 
                    var_g = var_3; 
                    var_b = var_1;
                }else if(var_i==1){ 
                    var_r = var_2;
                    var_g = v;
                    var_b = var_1;
                }else if(var_i==2){
                    var_r = var_1;
                    var_g = v;
                    var_b = var_3;
                }else if(var_i==3){
                    var_r = var_1;
                    var_g = var_2;
                    var_b = v;
                }else if (var_i==4){
                    var_r = var_3;
                    var_g = var_1;
                    var_b = v;
                }else{ 
                    var_r = v;
                    var_g = var_1;
                    var_b = var_2;
                }
                //rgb results = 0 ÷ 255  
                c.R = (int)Math.Round(var_r * 255);
                c.G = (int)Math.Round(var_g * 255);
                c.B = (int)Math.Round(var_b * 255);
            }
            return c;  
        }

        #endregion Utilities

        #endregion Methods
    }
}
