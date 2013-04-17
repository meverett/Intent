using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent
{
    /// <summary>
    /// Marks a class as a <see cref="MessageAdapter">message adapter</see> module.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MessageAdapterAttribute : Attribute
    {
        #region Fields

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the name of the message adapter.
        /// </summary>
        public string Name { get; private set; }

        #endregion Properties

        #region Constructors

        public MessageAdapterAttribute(string name)
        {
            Name = name;
        }

        #endregion Constructors

        #region Methods

        #endregion Methods
    }
}
