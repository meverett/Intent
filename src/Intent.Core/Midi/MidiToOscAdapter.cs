﻿using System;
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
                return "{ routing: [ { name: \"Note On\", address: \"/midi\", args: \"channel={c}&value={v2}\" } ] }";
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a MIDI relay using the local loopback/localhost as the IP endpoint and
        /// the LoopBe software MIDI interface for MIDI event capture.
        /// </summary>
        public MidiToOscAdapter() : this(null, localEndPoint) { }

        /// <summary>
        /// Creates a MIDI relay using the specified MIDI device and the
        /// local loopback/localhost as the IP endpoint.
        /// </summary>
        /// <param name="deviceName">The name of the MIDI device to use to capture MIDI events for relay.</param>
        public MidiToOscAdapter(string deviceName) : this(deviceName, localEndPoint) { }

        /// <summary>
        /// Creates a MIDI relay using the specified IP endpoint and the LoopBe software MIDI device
        /// for MIDI event capture.
        /// </summary>
        /// <param name="ipEndPoint">The IP endpoint to use.</param>
        public MidiToOscAdapter(IPEndPoint ipEndPoint) : this(null, ipEndPoint) { }

        /// <summary>
        /// Creates a MIDI relay using the supplied <see cref="IPEndPoint">IP endpoint</see>.
        /// </summary>
        /// <remarks>
        /// Supplying a NULL device name defaults to using the LoopBe software MIDI driver.
        /// </remarks>
        /// <param name="deviceName">The name of the MIDI device to use to capture MIDI events for relay.</param>
        /// <param name="ipEndPoint">The IP endpoint to use.</param>
        public MidiToOscAdapter(string deviceName, IPEndPoint ipEndPoint) : base(deviceName)
        {
            // Assign paramters
            this.ipEndPoint = ipEndPoint;
            sb = new StringBuilder();
        }

        #endregion Constructors

        #region Methods

        #region Event Handlers

        protected override void OnStart()
        {
            base.OnStart();
            IntentMessaging.WriteLine("Sending MIDI => OSC out on: {0}", ipEndPoint);
        }

        /// <summary>
        /// Forwards the mapped message on as an OSC message.
        /// </summary>
        /// <param name="msg">The MIDI message that was received.</param>
        /// <param name="type">The MIDI message type.</param>
        /// <param name="channel">The MIDI channel the message was received on.</param>
        /// <param name="value1">The MIDI message data byte 1 value.</param>
        /// <param name="value1">The MIDI message data byte 2 value.</param>
        protected override void OnMidiMessageRouted(MidiRoutingRule rule, Message msg, MidiMessageTypes type, 
                                                    int channel, int value1, int value2)
        {
            // Setup the mapped outgoing message with the raw MIDI data and the OSC mapped arguments
            var oscMsg = new OscMessage(ipEndPoint, rule.OscAddress);

            // Prepare the outgoing arguments
            string args = null;

            // If args is NULL, see if the original settings object defines it as a function
            if (string.IsNullOrEmpty(rule.OscArguments) && rule.JavaScript != null 
                && rule.JavaScript.Members.ContainsKey("args") && rule.JavaScript.Members["args"] is FunctionObject)
            {
                // Get the JavaScript args function
                var argsFunc = (FunctionObject)rule.JavaScript.Members["args"];

                // Box the MIDI message values
                var boxedType = BoxedValue.Box(type);
                var boxedChannel = BoxedValue.Box(channel);
                var boxedValue1 = BoxedValue.Box(value1);
                var boxedValue2 = BoxedValue.Box(value1);

                // Call the function
                var result = argsFunc.Call(rule.JavaScript, new BoxedValue[] { boxedType, boxedChannel, boxedValue1, boxedValue2 } );

                // If the function returned a string, just pass it on
                if (result.IsString)
                {
                    args = (string)result.ClrBoxed;
                }
                else if (result.IsObject)
                {
                    // Build the outgoing message form the returned object
                    var argsObj = result.Object;
                    sb.Clear();
                    int count = 1;

                    foreach (KeyValuePair<string, object> pair in argsObj.Members)
                    {
                        sb.Append(pair.Key + "=" + pair.Value.ToString());
                        if (count < argsObj.Members.Count) sb.Append("&");
                        count++;
                    }

                    args = sb.ToString();
                }
            }
            // Otherwise value inject into the OSC arguments; substitute dynamic MIDI values: channel, value 1, value 2 bytes
            else
            {
                args = rule.OscArguments.
                    Replace("{n}", type.ToString()).
                    Replace("{c}", channel.ToString()).
                    Replace("{v1}", value1.ToString()).
                    Replace("{v2}", value2.ToString());
            }

            // Append the message arguments
            oscMsg.Append(args);

            //Intent.WriteLine("{0} => {1}:{2}", rule.Name, rule.OscAddress, args);

            // Relay the MIDI message over OSC
            oscMsg.Send(ipEndPoint);

            // Notify
            TriggerMessageSent();
        }

        #endregion Event Handlers

        #endregion Methods
    }
}