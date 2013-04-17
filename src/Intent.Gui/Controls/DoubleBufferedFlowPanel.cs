using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Intent.Gui
{
    /// <summary>
    /// Custom flow panel that uses double buffering to reduce control flickering.
    /// </summary>
    public class DoubleBufferedFlowPanel : FlowLayoutPanel
    {
        #region Fields

        #endregion Fields

        #region Properties

        #endregion Properties

        #region Constructors

        public DoubleBufferedFlowPanel()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        #endregion Constructors

        #region Methods

        #endregion Methods
    }
}
