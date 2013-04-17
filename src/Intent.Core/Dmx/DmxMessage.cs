using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Dmx
{
    /// <summary>
    /// A basic DMX message specifying the channel and value.
    /// </summary>
    public struct DmxMessage
    {
        /// <summary>
        /// Gets the DMX channel the message is for.
        /// </summary>
        public readonly int Channel;

        /// <summary>
        /// Gets the DMX value to set the channel to.
        /// </summary>
        public readonly int Value;

        /// <summary>
        /// Creates a DMX message for the given channel and value.
        /// </summary>
        /// <param name="channel">The DMX channel.</param>
        /// <param name="value">The channel value.</param>
        public DmxMessage(int channel, int value)
        {
            Channel = channel;
            Value = value;
        }
    }
}
