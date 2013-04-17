using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Midi
{
    /// <summary>
    /// Creates a rule that matches a given value to a known list of whitelist
    /// and/or blacklist values.
    /// </summary>
    /// <remarks>
    /// Values that exist in the whitelist will trigger a match where as
    /// values that exist in the blacklist will reject the match and are
    /// evaluated first if present.
    /// </remarks>
    public class ListRule<T>
    {
        #region Fields

        // Values that are in the whitelist are allowed
        List<T> whitelist;

        // Values tha are in the blacklist are not allowed
        List<T> blacklist;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the number of items in the whitelist.
        /// </summary>
        public int WhitelistCount { get { return whitelist != null ? whitelist.Count : 0; } }

        /// <summary>
        /// Gets the number of items in the blacklist.
        /// </summary>
        public int BlacklistCount { get { return blacklist != null ? blacklist.Count : 0; } }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a value rule that matches the specified value against the
        /// given blacklist and/or whitelist.
        /// </summary>
        /// <param name="whitelist">The white list of </param>
        /// <param name="blacklist"></param>
        public ListRule(ICollection<T> whitelist, ICollection<T> blacklist)
        {
            if (whitelist != null && whitelist.Count > 0) this.whitelist = new List<T>(whitelist);
            if (blacklist != null && blacklist.Count > 0) this.blacklist = new List<T>(blacklist);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Determines whether or not the value is allowed based on wether or not
        /// it appears in the whitelist or blacklist of the rule.
        /// </summary>
        /// <remarks>
        /// Values that exist in the whitelist will trigger a match where as
        /// values that exist in the blacklist will reject the match and are
        /// evaluated first if present.
        /// </remarks>
        /// <param name="value">The value to evaluate.</param>
        /// <returns>True if the rule matches, False if not.</returns>
        public bool IsMatch(T value)
        {
            // Match against blacklist
            if (blacklist != null && blacklist.Contains(value)) return false;

            // If there's only a blacklist - assume it's a match; we only wanted to keep out blacklisted values
            if (blacklist != null && whitelist == null) return true;

            // Otherwise match against white list
            if (whitelist != null && whitelist.Contains(value)) return true;

            // No match either way
            return false;
        }

        #endregion Methods
    }
}
