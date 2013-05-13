using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Midi
{
    /// <summary>
    /// Represents a MIDI device/driver.
    /// </summary>
    public struct MidiDeviceInfo
    {
        /// <summary>
        /// Gets the device ID for the MIDI device.
        /// </summary>
        public readonly int Id;

        /// <summary>
        /// Gets the name of the MIDI device.
        /// </summary>
        public readonly string Name;

        public MidiDeviceInfo(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
