using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Midi
{
    /// <summary>
    /// Base class for creating rules that can be evaluated against a MIDI value to determine
    /// whether or not the value satisfies the rule.
    /// </summary>
    public abstract class MidiValueRule
    {
        /// <summary>
        /// Determines whether or not the particular MIDI value matches the rule.
        /// </summary>
        /// <param name="value">The MIDI value to evaluate.</param>
        /// <returns>True if the value matches the rule, False if not.</returns>
        public abstract bool IsMatch(int value);
    }
}
