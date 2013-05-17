using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;

using Midi;
using IronJS;
using Bespoke.Common.Osc;

namespace Intent.Midi
{
    /// <summary>
    /// Routes and maps incoming MIDI messages as outgoing OSC messages.
    /// </summary>
    [MessageAdapter("MIDI to OSC")]
    public class MidiToOscAdapter : MidiAdapter
    {
        #region Fields

        // Used for efficient string building
        StringBuilder sb;

        #region IP/Networking

        // The default IP endpoint for localhost.
        static IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Loopback, OscHelper.DefaultPort);
        
        // The IP address/endpoint to use for relaying OSC messages.
        IPEndPoint ipEndPoint;

        #endregion IP/Networking

        #region OSC

        #endregion OSC

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the default settings script for the MIDI to Console adapter.
        /// </summary>
        public override string DefaultSettingsScript
        {
            get
            {
                return "{ routing: { \"All\": { address: \"/midi\", args: \"channel={c}&value={v2}\" } } }";
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a MIDI relay using the MIDI device and <see cref="IPEndPoint">IP endpoint</see>
        /// specified from the script settings object.
        /// </summary>
        public MidiToOscAdapter()
        {
            sb = new StringBuilder();
        }

        #endregion Constructors

        #region Methods

        #region Event Handlers

        protected override void OnStart()
        {
            // Get IP address from script settings object or use default
            var members = CurrentSettings != null ? CurrentSettings.Members : null;
            if (members != null && (members.ContainsKey("ip") || members.ContainsKey("port")))
            {
                var address = members.ContainsKey("ip") ? (string)members["ip"] : localEndPoint.Address.ToString();
                int port = members.ContainsKey("port") ? Convert.ToInt32(members["port"]) : localEndPoint.Port;
                var ipAddress = IPAddress.Parse(address);
                this.ipEndPoint = new IPEndPoint(ipAddress, port);
            }
            // Otherwise use local loopback defaults
            else
            {
                this.ipEndPoint = localEndPoint;
            }

            base.OnStart();
            IntentRuntime.WriteLine("Sending MIDI => OSC out on: {0}", ipEndPoint);
        }

        /// <summary>
        /// Forwards the mapped message on as an OSC message.
        /// </summary>
        /// <param name="type">The MIDI message type.</param>
        /// <param name="channel">The MIDI channel the message was received on.</param>
        /// <param name="value1">The MIDI message data byte 1 value.</param>
        /// <param name="value1">The MIDI message data byte 2 value.</param>
        protected override void OnMidiMessageRouted(MidiRoutingRule rule, MidiMessageTypes type, int channel, int value1, int value2)
        {
            try
            {
                // Create the outgoing OSC message data from the rule
                string data = null;
                var members = rule.ScriptObject != null ? rule.ScriptObject.Members : null;

                // If the outgoing data is a script object, process it
                if (string.IsNullOrEmpty(rule.OutData) && members != null && members.ContainsKey("data") && members["data"] is FunctionObject)
                {
                    data = GenerateDataString(rule.ScriptObject, type, channel, value1, value2);
                }
                // Otherwise process it as a string with value substitution
                else
                {
                    data = GenerateDataString(rule.OutData, type, channel, value1, value2);
                }

                // If there is no data to return, don't send a message
                if (data == null) return;

                // Otherwise send it
                SendOscMessage(rule.OutAddress, data);
            }
            catch (Exception ex)
            {
                HasErrors = true;
                IntentRuntime.WriteLine(ex);
            }
        }

        /// <summary>
        /// Fulfills a script-generated call to send an outbound OSC message from the adapter instance.
        /// </summary>
        /// <param name="message">The OSC message object to use for sending the message.</param>
        protected override void OnSendMessage(CommonObject message)
        {
            #region Validate

            var members = message.Members;

            if (!members.ContainsKey("address"))
            {
                IntentRuntime.WriteLine("{0}:{1} -> sendMessage(id, message): message.address cannot be NULL", Name, Id);
                return;
            }
            else if (!members.ContainsKey("data"))
            {
                IntentRuntime.WriteLine("{0}:{1} -> sendMessage(id, message): message.data cannot be NULL", Name, Id);
                return;
            }

            #endregion Validate

            try
            {
                var address = (string)members["address"];
                var messageData = members["data"];

                // If this is a data function, call it and expect arguments to be attached to the message
                if (messageData is FunctionObject)
                {
                    // Extract MIDI values for function from the message itself
                    MidiMessageTypes type = MidiRoutingRule.ParseMidiMessageType((string)members["type"]);
                    int channel = TypeConverter.ToInt32((double)members["channel"]);
                    int value1 = members["value1"] is string ? MidiRoutingRule.ParseMidiValue((string)members["value1"]) : TypeConverter.ToInt32((double)members["value1"]);
                    int value2 = members["value2"] is string ? MidiRoutingRule.ParseMidiValue((string)members["value1"]) : TypeConverter.ToInt32((double)members["value2"]);

                    var msgDataString = GenerateDataString(message, type, channel, value1, value2);
                    SendOscMessage(address, msgDataString);
                    
                }
                // If this is a data object, pack it for sending
                else if (messageData is CommonObject)
                {
                    var dataObject = (CommonObject)messageData;
                    SendOscMessage(address, ConvertMessageDataToOsc(dataObject));
                }
                // Otherwise if this is just a constant string value, send it
                else if (messageData is string)
                {
                    SendOscMessage(address, (string)messageData);
                }
            }
            catch (Exception ex)
            {
                HasErrors = true;
                IntentRuntime.WriteLine(ex);
            }
        }

        #endregion Event Handlers

        #region Messaging

        /// <summary>
        /// Sends an outbound OSC message with the specified OSC address and data.
        /// </summary>
        /// <param name="address">The OSC address to send to.</param>
        /// <param name="data">The data to send in the message.</param>
        public void SendOscMessage(string address, string data)
        {
            #region Validate

            if (string.IsNullOrEmpty(address))
            {
                IntentRuntime.WriteLine("OSC message address cannot be NULL. {0} => {1}", address, data);
                return;
            }
            else if (string.IsNullOrEmpty(data))
            {
                IntentRuntime.WriteLine("OSC message data cannot be NULL. {0} => {1}", address, data);
                return;
            }

            #endregion Validate

            // Setup the mapped outgoing message with the raw MIDI data and the OSC mapped arguments
            var oscMsg = new OscMessage(ipEndPoint, address);

            // Append the message arguments
            oscMsg.Append(data);

            //Intent.WriteLine("{0} => {1}:{2}", rule.Name, rule.OscAddress, args);

            // Relay the MIDI message over OSC
            oscMsg.Send(ipEndPoint);

            // Notify
            TriggerMessageSent();
        }

        #endregion Messaging

        #region Utilities

        // Given an outgoing OSC message's script data object - convert into an OSC data payload string
        string GenerateDataString(CommonObject dataObject, MidiMessageTypes type, int channel, int value1, int value2)
        {
            // Prepare the outgoing arguments
            string data = null;

            // Get the JavaScript args function
            var dataFunc = (FunctionObject)dataObject.Members["data"];

            // Box the MIDI message values
            var boxedType = BoxedValue.Box(type);
            var boxedChannel = BoxedValue.Box(channel);
            var boxedValue1 = BoxedValue.Box(value1);
            var boxedValue2 = BoxedValue.Box(value2);

            // Call the function
            var result = dataFunc.Call(dataObject, new BoxedValue[] { boxedType, boxedChannel, boxedValue1, boxedValue2 });

            // NULL - don't do anything
            if (result.IsNull)
            {
                return null;
            }
            // If the function returned a string, just pass it on
            if (result.IsString)
            {
                data = (string)result.ClrBoxed;
            }
            else if (result.IsObject)
            {
                data = ConvertMessageDataToOsc(result.Object); 
            }

            return data;
        }

        // Given an outgoing OSC message's script data object - convert into an OSC data payload string
        string GenerateDataString(string dataString, MidiMessageTypes type, int channel, int value1, int value2)
        {
            // Prepare the outgoing arguments
            return dataString.
                    Replace("{t}", type.ToString()).
                    Replace("{c}", channel.ToString()).
                    Replace("{v1}", value1.ToString()).
                    Replace("{v2}", value2.ToString());
        }

        // Takes a script OSC data object and packs its values into an OSC data string
        string ConvertMessageDataToOsc(CommonObject dataObject)
        {
            // Build the outgoing message form the returned object
            sb.Clear();     // clear string builder for the new message
            int count = 1;  // count the number of name/value pairs

            foreach (KeyValuePair<string, object> pair in dataObject.Members)
            {
                // If this is not an array of values just add the direct key/value
                if (!(pair.Value is ArrayObject))
                {
                    sb.Append(pair.Key + "=" + pair.Value.ToString());

                }
                // Otherwise this is an array of values
                else
                {
                    sb.Append(pair.Key + "=");
                    var array = (ArrayObject)pair.Value;

                    for (int i = 0; i < array.Length; i++)
                    {
                        sb.Append(array.Get(i).ClrBoxed.ToString());
                        if (i < array.Length - 1) sb.Append(",");
                    }
                }

                // Properly encode any future values
                if (count < dataObject.Members.Count) sb.Append("&");
                count++;
            }

            return sb.ToString();
        }

        #endregion Utilities

        #endregion Methods
    }
}
