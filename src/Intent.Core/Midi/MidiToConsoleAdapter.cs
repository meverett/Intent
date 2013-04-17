using System;
using System.Collections.Generic;

using Midi;

namespace Intent.Midi
{
    /// <summary>
    /// Test class used to display incoming MIDI events in the console.
    /// </summary>
    [MessageAdapter("MIDI to Console")]
    public class MidiToConsoleAdapter : MidiAdapter
    {
        /// <summary>
        /// Writes the received MIDI message out to the console.
        /// </summary>
        /// <param name="msg">The MIDI message that was received.</param>
        /// <param name="type">The MIDI message type.</param>
        /// <param name="channel">The MIDI channel the message was received on.</param>
        /// <param name="value1">The MIDI message data byte 1 value.</param>
        /// <param name="value1">The MIDI message data byte 2 value.</param>
        protected override void OnMidiMessageReceived(Message msg, MidiMessageTypes type, int channel, int value1, int value2)
        {
            IntentMessaging.WriteLine("{0,-14} channel:{1:###}\tvalue1: {2:###}\tvalue2 {3:###}", type, channel, value1, value2);
        }

        /// <summary>
        /// Writes information about routing rule matches to the console.
        /// </summary>
        /// <param name="msg">The MIDI message that was received.</param>
        /// <param name="type">The MIDI message type.</param>
        /// <param name="channel">The MIDI channel the message was received on.</param>
        /// <param name="value1">The MIDI message data byte 1 value.</param>
        /// <param name="value1">The MIDI message data byte 2 value.</param>
        protected override void OnMidiMessageRouted(MidiRoutingRule rule, Message msg, MidiMessageTypes type, 
                                                    int channel, int value1, int value2)
        {
            IntentMessaging.WriteLine("=> {0}", rule.Name);
            TriggerMessageSent();
        }
    }
}
