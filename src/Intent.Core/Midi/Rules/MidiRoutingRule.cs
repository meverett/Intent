using System;
using System.Collections.Generic;
using System.Linq;

using Midi;
using IronJS;

namespace Intent.Midi
{
    /// <summary>
    /// Determines whether or not a MIDI message matches its rules.
    /// </summary>
    public class MidiRoutingRule
    {
        #region Fields

        // The internal list rule for matching whitelist/blacklist for MIDI message types.
        ListRule<MidiMessageTypes> typeRule;

        // Whether or not the rule matches for all messages or specific ones
        bool matchAnyMessage = false;

        // The internal rule to use to evaluate message channel number
        MidiValueRule channelRule;

        // The internal rule to use to evaluate message value byte 1
        MidiValueRule value1Rule;

        // The internal rule to use to evaluate message value byte 2
        MidiValueRule value2Rule;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the OSC message address the MIDI route will map to if it matches
        /// </summary>
        public string OscAddress { get; private set; }

        /// <summary>
        /// Gets the OSC message arguments the MIDI route will map to if it matches.
        /// </summary>
        public string OscArguments { get; private set; }

        /// <summary>
        /// Gets the JavaScript object that generated the routing rule.
        /// </summary>
        public CommonObject ScriptObject { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a MIDI routing rule for the given whitelisted or blacklisted
        /// MIDI message types, and the given value rules.
        /// </summary>
        /// <param name="name">The name of the rule.</param>
        /// <param name="oscAddress">The OSC message address the MIDI route will map to if it matches.</param>
        /// <param name="oscArguments">The OSC message arguments the MIDI route will map to if it matches.</param>
        /// <param name="whitelist">The message whitelist to use.</param>
        /// <param name="blacklist">The message blacklist to use.</param>
        /// <param name="channelRule">The rule to use for the channel number.</param>
        /// <param name="value1Rule">The rule to use for value byte 1.</param>
        /// <param name="value2Rule">The rule to use for value byte 2.</param>
        public MidiRoutingRule( string name, string oscAddress, string oscArguments,
                                ICollection<MidiMessageTypes> whitelist, ICollection<MidiMessageTypes> blacklist, 
                                MidiValueRule channelRule, MidiValueRule value1Rule, MidiValueRule value2Rule)
        {
            Name = name;
            OscAddress = oscAddress;
            OscArguments = oscArguments;

            this.typeRule = new ListRule<MidiMessageTypes>(whitelist, blacklist);
            this.matchAnyMessage = whitelist == null && blacklist == null;
            this.channelRule = channelRule;
            this.value1Rule = value1Rule;
            this.value2Rule = value2Rule;
        }

        /// <summary>
        /// Creates a new MIDI routing rule from the provided JavaScript object.
        /// </summary>
        /// <param name="js">The JavaScript object that contains the routing rule data.</param>
        public MidiRoutingRule(CommonObject js)
        {
            if (js == null) throw new ArgumentNullException("js");

            #region Prepare the rule's components

            string name = js.Members.ContainsKey("name") ? (string)js.Members["name"] : null;
            string oscAddress = js.Members.ContainsKey("address") ? (string)js.Members["address"] : null;
            string oscArguments = js.Members.ContainsKey("args") && js.Members["args"] is string ? (string)js.Members["args"] : null;
            List<MidiMessageTypes> typeWhitelist = null; // whitelisted MIDI message types
            List<MidiMessageTypes> typeBlacklist = null; // blacklisted MIDI message types
            MidiListRule channelRule = null; // rule used to match against the MIDI channel
            MidiListRule value1Rule = null; // rule used to match against MIDI value byte 1
            MidiListRule value2Rule = null; // rule used to match against MIDI value byte 2

            #endregion Prepare the rule's components

            #region Create MIDI message type rule

            // Filter by message type(s) if they were supplied
            if (js.Members.ContainsKey("message"))
            {
                // Get the raw value
                var parsedMessage = js.Members["message"];

                // Is this a simple/single whitelist?
                if (parsedMessage is string)
                {
                    // Add it as the single and only value
                    typeWhitelist = new List<MidiMessageTypes>();
                    typeWhitelist.Add(ParseMidiMessageType((string)parsedMessage));
                }
                // Is this an array of whitelist types?
                else if (parsedMessage is List<object>)
                {
                    typeWhitelist = new List<MidiMessageTypes>();

                    // Add each message type to the whitelist
                    foreach (object o in (List<object>)parsedMessage) typeWhitelist.Add(ParseMidiMessageType(o.ToString()));
                }
                // Otherwise this should be a  whitelist/blacklist object
                else if (parsedMessage is Dictionary<string, object>)
                {
                    var list = (Dictionary<string, object>)parsedMessage;

                    foreach (KeyValuePair<string, object> p in list)
                    {
                        #region Add to whitelist or blacklist

                        // Assign the correct target list for this entry
                        string key = p.Key.ToLower();
                        List<MidiMessageTypes> targetList = null;

                        if (key == "whitelist")
                        {
                            if (typeWhitelist == null) typeWhitelist = new List<MidiMessageTypes>();
                            targetList = typeWhitelist;
                        }
                        else if (key == "blacklist")
                        {
                            if (typeBlacklist == null) typeBlacklist = new List<MidiMessageTypes>();
                            targetList = typeBlacklist;
                        }

                        // A single value
                        if (p.Value is string)
                        {
                            targetList.Add(ParseMidiMessageType((string)p.Value));
                        }
                        // Otherwise a list of values
                        else if (p.Value is List<object>)
                        {
                            foreach (string type in (List<object>)p.Value) targetList.Add(ParseMidiMessageType(type));
                        }

                        #endregion Add to whitelist or blacklist
                    }
                }
            }

            #endregion Create MIDI message type rule

            #region Create MIDI channel rule

            // Filter by channel
            if (js.Members.ContainsKey("channel"))
            {
                // Get the raw value
                var parsedChannel = js.Members["channel"];

                // Is this a simple/single whitelist?
                if (parsedChannel is string || parsedChannel is int || parsedChannel is long)
                {
                    // Add it as the single and only value
                    int channel = parsedChannel is string ? ParseMidiChannel((string)parsedChannel)
                                                          : Convert.ToInt32((long)parsedChannel);

                    channelRule = new MidiListRule(new int[] { channel }, null);
                }
                // Is this an array of whitelist types?
                else if (parsedChannel is List<object>)
                {
                    // Then create the whitelist and then the rule
                    var whitelist = new List<int>();
                    foreach (long channel in (List<object>)parsedChannel) whitelist.Add(Convert.ToInt32(channel));
                    channelRule = new MidiListRule(whitelist, null);
                }
                // Otherwise this should be a  whitelist/blacklist object
                else if (parsedChannel is Dictionary<string, object>)
                {
                    var list = (Dictionary<string, object>)parsedChannel;
                    List<int> whitelist = null;
                    List<int> blacklist = null;

                    // Populate respective lists
                    foreach (KeyValuePair<string, object> p in list)
                    {
                        #region Add to whitelist or blacklist

                        // Assign the correct target list for this entry
                        string key = p.Key.ToLower();
                        List<int> targetList = null;

                        if (key == "whitelist")
                        {
                            if (whitelist == null) whitelist = new List<int>();
                            targetList = whitelist;
                        }
                        else if (key == "blacklist")
                        {
                            if (blacklist == null) blacklist = new List<int>();
                            targetList = blacklist;
                        }

                        // A single channel
                        if (p.Value is string)
                        {
                            targetList.Add(ParseMidiChannel((string)p.Value));
                        }
                        // Otherwise a list of channels
                        else if (p.Value is List<object>)
                        {
                            foreach (long channel in (List<object>)p.Value) targetList.Add(Convert.ToInt32(channel));
                        }

                        #endregion Add to whitelist or blacklist
                    }

                    // Create the rule
                    channelRule = new MidiListRule(whitelist, blacklist);
                }
            }

            #endregion Create MIDI channel rule

            #region Create MIDI value byte 1 rule

            // Filter by value byte 1
            if (js.Members.ContainsKey("value1"))
            {
                // Get the raw value
                var parsedValue = js.Members["value1"];
                GetValueRule(parsedValue, out value1Rule);
            }

            #endregion Create MIDI value byte 1 rule

            #region Create MIDI value byte 2 rule

            // Filter by value byte 1
            if (js.Members.ContainsKey("value2"))
            {
                // Get the raw value
                var parsedValue = js.Members["value2"];
                GetValueRule(parsedValue, out value2Rule);
            }

            #endregion Create MIDI value byte 2 rule

            // Store extracted values
            this.ScriptObject = js;
            this.Name = name;
            this.OscAddress = oscAddress;
            this.OscArguments = oscArguments;
            this.typeRule = new ListRule<MidiMessageTypes>(typeWhitelist, typeBlacklist);
            this.matchAnyMessage = typeWhitelist == null && typeBlacklist == null;
            this.channelRule = channelRule;
            this.value1Rule = value1Rule;
            this.value2Rule = value2Rule;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Determines whether or not the MIDI message is a match for the routing rule.
        /// </summary>
        /// <remarks>
        /// Derived classes should implment their matching logic in the <see cref="IsMatch"/>
        /// method to customize the matching rule logic.
        /// </remarks>
        /// <param name="msg">The MIDI message to match against.</param>
        /// <returns>True if the message matches the routing rule, False if not.</returns>
        public bool IsMatch(Message msg)
        {
            #region Extract MIDI message data

            int channel = -1, value1 = -1, value2 = -1;

            // Get the message type and value bytes
            MidiMessageTypes type = MidiMessageTypes.ControlChange;

            if (msg is ControlChangeMessage)
            {
                ControlChangeMessage m = (ControlChangeMessage)msg;
                channel = (int)m.Channel + 1;
                value1 = m.Value;
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

            // If this rule declares a specific set of message types to match against, do so first
            if (!matchAnyMessage && !typeRule.IsMatch(type)) return false;

            // Match the channel
            if (channelRule != null && channel != -1 && !channelRule.IsMatch(channel)) return false;

            // Match value byte 1
            if (value1Rule != null && value1 != -1 && !value1Rule.IsMatch(value1)) return false;

            // Match value byte 2
            if (value2Rule != null && value2 != -1 && !value2Rule.IsMatch(value2)) return false;

            // We made it; it's a match ladies and gentlemen!
            return true;
        }

        #region Parsing Utilities

        // Parses a MIDI message type enum value from raw text
        MidiMessageTypes ParseMidiMessageType(string text)
        {
            MidiMessageTypes type;

            if (!Enum.TryParse<MidiMessageTypes>(text.Replace(" ", ""), true, out type))
                throw new ApplicationException("MIDI routing rule 'message' value is not formated correctly: " + text);

            return type;
        }

        // Parses a MIDI channel value from raw text
        int ParseMidiChannel(string text)
        {
            int channel;

            if (!int.TryParse(text.Replace(" ", ""), out channel))
                throw new ApplicationException("MIDI routing rule 'channel' value is not formated correctly: " + text);

            return channel;
        }

        // Parses a MIDI data value from raw text
        int ParseMidiValue(string text)
        {
            int value; Pitch pitch = Pitch.A0;
            bool isValue, isPitch = false;
            var raw = text; text = text.Replace(" ", "");

            // Figure out what kind of value this is
            isValue = int.TryParse(text, out value);
            if (!isValue) isPitch = Enum.TryParse<Pitch>(text, true, out pitch);

            // Basic integer/command value
            if (isValue)
            {
                return value;
            }
            // A pitch value
            else if (isPitch)
            {
                return (int)pitch;
            }

            throw new ApplicationException("MIDI routing rule value byte is not formated correctly: " + raw);
        }

        // Gets the value rule for the given parsed value
        void GetValueRule(object parsedValue, out MidiListRule rule)
        {
            rule = null;

            // Is this a simple/single whitelist?
            if (parsedValue is string || parsedValue is int || parsedValue is long)
            {
                // Add it as the single and only value
                int value = parsedValue is string ? ParseMidiValue((string)parsedValue)
                                                      : Convert.ToInt32((long)parsedValue);

                rule = new MidiListRule(new int[] { value }, null);
            }
            // Is this an array of whitelist types?
            else if (parsedValue is List<object>)
            {
                // Then create the whitelist and then the rule
                var whitelist = new List<int>();

                foreach (object rawValue in (List<object>)parsedValue)
                {
                    // Add it as the single and only value
                    int value = rawValue is string ? ParseMidiValue((string)rawValue)
                                                          : Convert.ToInt32((long)rawValue);

                    whitelist.Add(value);
                }

                rule = new MidiListRule(whitelist, null);
            }
            // Otherwise this should be a  whitelist/blacklist object
            else if (parsedValue is Dictionary<string, object>)
            {
                var list = (Dictionary<string, object>)parsedValue;
                List<int> whitelist = null;
                List<int> blacklist = null;

                // Populate respective lists
                foreach (KeyValuePair<string, object> p in list)
                {
                    #region Add to whitelist or blacklist

                    // Assign the correct target list for this entry
                    string key = p.Key.ToLower();
                    List<int> targetList = null;

                    if (key == "whitelist")
                    {
                        if (whitelist == null) whitelist = new List<int>();
                        targetList = whitelist;
                    }
                    else if (key == "blacklist")
                    {
                        if (blacklist == null) blacklist = new List<int>();
                        targetList = blacklist;
                    }

                    // A single value
                    if (p.Value is string)
                    {
                        targetList.Add(ParseMidiValue((string)p.Value));
                    }
                    // Otherwise a list of value
                    else if (p.Value is List<object>)
                    {
                        foreach (object rawValue in (List<object>)p.Value)
                        {
                            // Add it as the single and only value
                            int value = rawValue is string ? ParseMidiValue((string)rawValue)
                                                                  : Convert.ToInt32((long)rawValue);

                            targetList.Add(value);
                        }
                    }

                    #endregion Add to whitelist or blacklist
                }

                // Create the rule
                rule = new MidiListRule(whitelist, blacklist);
            }
        }

        #endregion Parsing Utilities

        #endregion Methods
    }
}
