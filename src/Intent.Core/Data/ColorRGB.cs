using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent
{
    /// <summary>
    /// Represents a 24bit red, blue, green color value.
    /// </summary>
    struct ColorRGB
    {
        /// <summary>
        /// The red channel value 0-255.
        /// </summary>
        public int R;

        /// <summary>
        /// The green channel value 0-255.
        /// </summary>
        public int G;

        /// <summary>
        /// The blue channel value 0-255.
        /// </summary>
        public int B;

        public ColorRGB(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }
    }
}
