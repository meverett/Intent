using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Midi
{
    /// <summary>
    /// Different types of MIDI message events.
    /// </summary>
    public enum MidiMessageTypes
    {
        ControlChange   = 0,
        NoteOn          = 1,
        NoteOff         = 2,
        PitchBend       = 3,
        ProgramChange   = 4,
    }
}
