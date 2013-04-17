using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Bespoke.Common.Osc;

namespace Intent.Osc
{
    /// <summary>
    /// Base class for classes that will receive and adapts OSC messages to various outputs.
    /// </summary>
    public abstract class OscAdapter : MessageAdapter
    {
        #region Fields

        #region IP/Networking

        // The default IP endpoint for localhost.
        static IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Loopback, OscHelper.DefaultPort);

        // The IP address/endpoint to use for relaying OSC messages.
        protected IPEndPoint ipEndPoint;

        #endregion IP/Networking

        #region OSC

        // The internal OSC server that receives the OSC messages
        OscServer server;

        #endregion OSC

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the underlying OSC server that is receiving OSC messages.
        /// </summary>
        protected OscServer Server { get { return server; } }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates an OSC message adapter listening on the local loopback.
        /// </summary>
        public OscAdapter() : this (null) { }

        /// <summary>
        /// Creates an OSC message adapter listening on the given IP endpoint.
        /// </summary>
        /// <param name="ipEndPoint">The IP endpoint to listen for messages on.</param>
        public OscAdapter(IPEndPoint ipEndPoint)
        {
            if (ipEndPoint == null) ipEndPoint = localEndPoint;
            this.ipEndPoint = ipEndPoint;

            // Create the OSC server to listen for incoming messages
            server = new OscServer(TransportType.Udp, ipEndPoint.Address, ipEndPoint.Port);
            server.FilterRegisteredMethods = false;
            server.ConsumeParsingExceptions = false;

            // Connect event handlers for server
            server.MessageReceived += server_MessageReceived;
        }

        #endregion Constructors

        #region Methods

        #region Event Handlers

        // Incoming OSC message received
        void server_MessageReceived(object sender, OscMessageReceivedEventArgs e)
        {
            //Intent.WriteLine("Message Received: {0}", e.Message.Address);

            // Extract message paramters
            var oscData = e.Message.Data;
            var args = oscData.FirstOrDefault();
            var data = new Dictionary<string, string>();

            // Attempt to extract the one and only first argument as a string that is encoded as name/value pairs
            if (args != null && args is string)
            {
                // Parse the name/value pairs (format: name1=value1&name2=value2...)
                string[] pairs = ((string)args).Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                if (pairs.Length > 0)
                {
                    // Parse the pairs and add to the data dictionary
                    foreach (string pair in pairs)
                    {
                        string[] keyAndValue = pair.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                        if (keyAndValue.Length == 2) data.Add(keyAndValue[0], keyAndValue[1]);
                    }
                }
            }

            //foreach (KeyValuePair<string, string> pair in data) Intent.WriteLine("{0} = {1}", pair.Key, pair.Value);
            //if (data.Count > 0) Intent.WriteLine();

            // Notify
            TriggerMessageReceived();

            // Pass on the data-decoded message
            OnOscMessageReceived(e.Message, data);
        }

        /// <summary>
        /// When overriden in derived classes, processes an incoming OSC message.
        /// </summary>
        /// <param name="msg">The OSC message that was received.</param>
        /// <param name="data">The decoded data dictionary argument.</param>
        protected virtual void OnOscMessageReceived(OscMessage msg, Dictionary<string, string> data) { }

        #endregion Event Handlers

        #region Operation

        /// <summary>
        /// When overriden in derived class, carries out any custom adapter start logic.
        /// </summary>
        protected override void OnStart()
        {
            if (server.IsRunning) return;
            server.Start();
            IntentMessaging.WriteLine("Started listening for OSC input events on: {0}", ipEndPoint);
        }

        /// <summary>
        /// When overriden in derived class, carries out any custom adapter start logic.
        /// </summary>
        protected override void OnStop()
        {
            if (!server.IsRunning) return;
            server.Stop();
            IntentMessaging.WriteLine("Stopped listening for OSC input events on: {0}", ipEndPoint);
        }

        #endregion Operation

        #endregion Methods
    }
}
