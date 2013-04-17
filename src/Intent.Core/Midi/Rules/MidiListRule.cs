using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Midi
{
    /// <summary>
    /// Matches specified integer values against a known whitelist
    /// and/or blacklist.
    /// </summary>
    public class MidiListRule : MidiValueRule
    {
        #region Fields

        // The internal list rule
        ListRule<int> rule;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the number of items in the whitelist.
        /// </summary>
        public int WhitelistCount { get { return rule.WhitelistCount; } }

        /// <summary>
        /// Gets the number of items in the blacklist.
        /// </summary>
        public int BlacklistCount { get { return rule.BlacklistCount; } }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a rule that matches integer values against a known
        /// whitelist and/or blacklist.
        /// </summary>
        /// <param name="whitelist">The whitelist of values to use.</param>
        /// <param name="blacklist">The blacklist of values to use.</param>
        public MidiListRule(ICollection<int> whitelist, ICollection<int> blacklist)
        {
            rule = new ListRule<int>(whitelist, blacklist);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Determines whether or not the given value matches the whitelist
        /// and/or blacklist rules.
        /// </summary>
        /// <param name="value">The value to check for.</param>
        /// <returns>True if the value matches, False if it does not match.</returns>
        public override bool IsMatch(int value)
        {
            return rule.IsMatch(value);
        }

        #endregion Methods
    }
}
