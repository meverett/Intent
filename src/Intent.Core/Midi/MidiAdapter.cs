using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Midi;
using Bespoke.Common.Osc;
using IronJS;

namespace Intent.Midi
{
    /// <summary>
    /// Base class for classes that will receive and adapts MIDI messages to various outputs.
    /// </summary>
    public abstract class MidiAdapter : MessageAdapter
    {
        #region Fields

        #region Routing

        // Inernal list of MIDI message routing rules.
        List<MidiRoutingRule> routingRules;

        #endregion Routing

        #region MIDI

        // The MIDI device that is being used to capture MIDI input events.
        InputDevice midi;

        #endregion MIDI

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets he name of the MIDI device to use to capture MIDI events.
        /// </summary>
        public string DeviceName { get; private set; }

        /// <summary>
        /// Gets the default settings script for the MIDI to Console adapter.
        /// </summary>
        public override string DefaultSettingsScript
        {
            get
            {
                return "{ routing: { \"All\": { } } }";
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a MIDI adapter that listens for MIDI events.
        /// </summary>
        public MidiAdapter()
        {
            // Initialize routing rules
            routingRules = new List<MidiRoutingRule>();
        }

        #endregion Constructors

        #region Methods

        #region Event Handlers

        // Message-specific MIDI message received event handlers
        void midiDevice_NoteOn(NoteOnMessage msg)               { midiDevice_MessageReceived(msg); }
        void midiDevice_NoteOff(NoteOffMessage msg)             { midiDevice_MessageReceived(msg); }
        void midiDevice_ControlChange(ControlChangeMessage msg) { midiDevice_MessageReceived(msg); }
        void midiDevice_PitchBend(PitchBendMessage msg)         { midiDevice_MessageReceived(msg); }
        void midiDevice_ProgramChange(ProgramChangeMessage msg) { midiDevice_MessageReceived(msg); }
        
        // Catch all MIDI message received event handler
        void midiDevice_MessageReceived(Message msg)
        {
            //Intent.WriteLine("MIDI message received: {0}", msg);

            #region Extract MIDI message data

            int channel = -1, value1 = -1, value2 = -1;

            // Get the message type and value bytes
            MidiMessageTypes type = MidiMessageTypes.ControlChange;

            if (msg is ControlChangeMessage)
            {
                ControlChangeMessage m = (ControlChangeMessage)msg;
                channel = (int)m.Channel + 1;
                value1 = (int)m.Control;
                value2 = m.Value;
            }
            else if (msg is NoteOnMessage)
            {
                type = MidiMessageTypes.NoteOn;
                NoteOnMessage m = (NoteOnMessage)msg;
                channel = (int)m.Channel + 1;
                value1 = (int)m.Pitch;
                value2 = m.Velocity;
            }
            else if (msg is NoteOffMessage)
            {
                type = MidiMessageTypes.NoteOff;
                NoteOffMessage m = (NoteOffMessage)msg;
                channel = (int)m.Channel + 1;
                value1 = (int)m.Pitch;
                value2 = m.Velocity;
            }
            else if (msg is PitchBendMessage)
            {
                type = MidiMessageTypes.PitchBend;
                PitchBendMessage m = (PitchBendMessage)msg;
                channel = (int)m.Channel + 1;
                value1 = (int)m.Value;
            }
            else if (msg is ProgramChangeMessage)
            {
                type = MidiMessageTypes.ProgramChange;
                ProgramChangeMessage m = (ProgramChangeMessage)msg;
                channel = (int)m.Channel + 1;
                value1 = (int)m.Instrument;
            }

            #endregion Extract MIDI message data

            // Let subclasses know a MIDI message was received
            OnMidiMessageReceived(msg, type, channel, value1, value2);

            // Notify
            TriggerMessageReceived();

            // Match against any loaded routing rules...
            for (int i = 0; i < routingRules.Count; i++)
            {
                var rule = routingRules[i];
                
                // If the rule is disabled, skip it
                if (!rule.Enabled) continue;

                // But if it's a match, pass the message on
                if (rule.IsMatch(msg))
                {
                    // Let subclasses know a MIDI message routing rule matched
                    OnMidiMessageRouted(rule, msg, type, channel, value1, value2);
                }
            }
        }

        /// <summary>
        /// When overriden in derived classes, handles the reception of a MIDI message.
        /// </summary>
        /// <param name="msg">The MIDI message that was received.</param>
        /// <param name="type">The MIDI message type.</param>
        /// <param name="channel">The MIDI channel the message was received on.</param>
        /// <param name="value1">The MIDI message data byte 1 value.</param>
        /// <param name="value1">The MIDI message data byte 2 value.</param>
        protected virtual void OnMidiMessageReceived(Message msg, MidiMessageTypes type,
                                                        int channel, int value1, int value2) { }
        
        /// <summary>
        /// When overriden in derived classes, handles the reception of a MIDI message
        /// that satisfied a routing rule.
        /// </summary>
        /// <param name="msg">The MIDI message that was received.</param>
        /// <param name="type">The MIDI message type.</param>
        /// <param name="channel">The MIDI channel the message was received on.</param>
        /// <param name="value1">The MIDI message data byte 1 value.</param>
        /// <param name="value1">The MIDI message data byte 2 value.</param>
        protected virtual void OnMidiMessageRouted(MidiRoutingRule rule, Message msg, MidiMessageTypes type, 
                                                    int channel, int value1, int value2) { }
        
        #endregion Event Handlers

        #region Operation

        /// <summary>
        /// Starts the MIDI relay.
        /// </summary>
        protected override void OnStart()
        {
            // Initialize MIDI device as needed - attempt to pull device name from script settings object
            if (midi == null)
            {
                var members = CurrentSettings != null ? CurrentSettings.Members : null;
                DeviceName = members != null && members.ContainsKey("device") ? (string)members["device"] : "LoopBe";

                // Construct the MIDI input device
                string deviceNameLower = DeviceName.ToLower();
                midi = InputDevice.InstalledDevices.FirstOrDefault(d => d.Name.ToLower().Contains(deviceNameLower));

                // Make sure the device was found
                if (midi == null)
                    throw new ArgumentException("MIDI input device not found: " + DeviceName);
            }

            if (midi.IsOpen) return;

            // Hook up MIDI handlers
            midi.NoteOn         += midiDevice_NoteOn;
            midi.NoteOff        += midiDevice_NoteOff;
            midi.ControlChange  += midiDevice_ControlChange;
            midi.PitchBend      += midiDevice_PitchBend;
            midi.ProgramChange  += midiDevice_ProgramChange;

            //midi.MessageReceived += midi_MessageReceived;
            midi.Open();
            midi.StartReceiving(null);
            IntentRuntime.WriteLine("Started listening for MIDI input events on: {0}", midi.Name);
        }

        /// <summary>
        /// Stops the MIDI relay.
        /// </summary>
        protected override void OnStop()
        {
            if (!midi.IsOpen) return;
            midi.StopReceiving();
            midi.Close();

            // Hook up MIDI handlers
            midi.NoteOn         -= midiDevice_NoteOn;
            midi.NoteOff        -= midiDevice_NoteOff;
            midi.ControlChange  -= midiDevice_ControlChange;
            midi.PitchBend      -= midiDevice_PitchBend;
            midi.ProgramChange  -= midiDevice_ProgramChange;

            IntentRuntime.WriteLine("Stopped listening for MIDI input events on: {0}", midi.Name);
        }

        #endregion Operation

        #region Settings

        /// <summary>
        /// Applies the custom MIDI routing settings.
        /// </summary>
        /// <param name="settings">The settings to apply.</param>
        protected override void OnApplySettings(CommonObject settings)
        {
            // Clear any current routing rules
            this.routingRules.Clear();
            var members = settings.Members;

            // Get the routing section if it exists
            if (members.ContainsKey("routing") && members["routing"] is CommonObject)
            {
                var routes = (CommonObject)members["routing"];

                foreach (KeyValuePair<string, object> pair in routes.Members)
                {
                    if (!(pair.Value is CommonObject)) continue; // skip non objects
                    var js = (CommonObject)pair.Value;
                    var route = new MidiRoutingRule(pair.Key, js);
                    routingRules.Add(route);
                }
            }

            IntentRuntime.WriteLine("MIDI routing rules loaded: {0}", routingRules.Count);
        }

        #endregion Settings

        #endregion Methods
    }
}
