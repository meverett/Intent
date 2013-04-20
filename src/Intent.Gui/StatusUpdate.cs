using System;

namespace Intent.Gui
{
    /// <summary>
    /// Represents an application/form status update.
    /// </summary>
    public class StatusUpdate
    {
        /// <summary>
        /// The status message text.
        /// </summary>
        public readonly string Text;

        /// <summary>
        /// The type of status message.
        /// </summary>
        public readonly StatusTypes Type;

        // Constructor.
        public StatusUpdate(string text, StatusTypes type)
        {
            Text = text;
            Type = type;
        }
    }
}
