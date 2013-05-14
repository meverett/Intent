using System;
using System.Collections.Generic;

using Bespoke.Common.Osc;

namespace Intent.Osc
{
    /// <summary>
    /// Test class used to display incoming MIDI events in the console.
    /// </summary>
    [MessageAdapter("OSC to Console")]
    public class OscToConsoleAdapter : OscAdapter
    {
        /// <summary>
        /// Writes a received OSC message to the console.
        /// </summary>
        /// <param name="msg">The message that was received.</param>
        /// <param name="data">The message data.</param>
        protected override void OnOscMessageReceived(OscMessage msg, Dictionary<string, string> data)
        {
            IntentRuntime.WriteLine("OSC Received @ {0} => {1}", ipEndPoint, msg.Address);
            foreach (KeyValuePair<string, string> pair in data) IntentRuntime.WriteLine(" ° {0,-16} = {1}", pair.Key, pair.Value);
            TriggerMessageSent();
        }
    }
}
