using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent
{
    /// <summary>
    /// Contains information about a particular <see cref="MessageAdapter"/>.
    /// </summary>
    public struct MessageAdapterInfo
    {
        #region Fields

        /// <summary>
        /// Gets the name of the message adapter.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Gets the message adapter's type.
        /// </summary>
        public readonly Type Type;

        #endregion Fields

        #region Constructors

        public MessageAdapterInfo(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Returns the message adapter's name.
        /// </summary>
        public override string ToString()
        {
            return Name;
        }

        #endregion Methods
    }
}
