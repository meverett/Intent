using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;

namespace Intent.Input
{
    /// <summary>
    /// Base class for classes that will receive mouse and keyboard input.
    /// </summary>
    public abstract class InputAdapter : MessageAdapter
    {
        #region Fields

        // Window form that is used to capture keyboard and mouse input.
        Form inputForm;

        #endregion Fields

        #region Properties

        #endregion Properties

        #region Constructors

        public InputAdapter()
        {

        }

        #endregion Constructors

        #region Methods

        #region Operation

        /// <summary>
        /// Starts listening for device input.
        /// </summary>
        protected override void OnStart()
        {
            //// Setup and create the input form
            //inputForm = new Form();
            //inputForm.Activate();
            //inputForm.Show();
            //inputForm.Enabled = true;
            //inputForm.Focus();
            //inputForm.Visible = true;

            //// Register keyboard event handlers
            //inputForm.KeyDown += inputForm_KeyDown;
            //inputForm.KeyUp += inputForm_KeyUp;
            IntentRuntime.WriteLine(Cursor.Position);
        }

        /// <summary>
        /// Stops listening for device input.
        /// </summary>
        protected override void OnStop()
        {
            //// Release any currently created input form
            //if (inputForm != null) inputForm.Dispose();
        }

        #endregion Operation

        #region Event Handlers

        // Handles keyboard input when a keyboard key is pressed
        void inputForm_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        // Handles keyboard input when a keyboard key is released
        void inputForm_KeyUp(object sender, KeyEventArgs e)
        {

        }

        #endregion Event Handlers

        #endregion Methods
    }
}
