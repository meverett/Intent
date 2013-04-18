using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Intent;

namespace Intent.Gui
{
    /// <summary>
    /// Provides a GUI version of the intent console.
    /// </summary>
    public partial class IntentConsole : UserControl
    {
        #region Fields

        // List of pending console text updates
        List<string> consoleUpdates;

        // Timer used to update the console with new events
        Timer timer;

        #endregion Fields

        #region Properties

        #endregion Properties

        #region Constructors

        public IntentConsole()
        {
            InitializeComponent();

            // List of pending updates to the console
            consoleUpdates = new List<string>();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Clears the console text.
        /// </summary>
        public void Clear()
        {
            console.Clear();
        }

        /// <summary>
        /// Starts the animation timer.
        /// </summary>
        public void Start()
        {
            timer.Start();
        }

        /// <summary>
        /// Stops the animation timer.
        /// </summary>
        public void Stop()
        {
            timer.Stop();
        }

        // Control -> Load
        private void Control_Load(object sender, EventArgs e)
        {
            // Couple the textbox to the Intent console output
            IntentMessaging.ConsoleUpdated += (s, eArgs) =>
            {
                lock (consoleUpdates) consoleUpdates.Add(IntentMessaging.LastConsoleText);
            };

            // Setup the console udpate timer to display updates in GUI thread
            timer = new Timer();
            timer.Interval = 66;

            timer.Tick += (s, eArgs) =>
            {
                lock (consoleUpdates)
                {
                    if (consoleUpdates.Count > 0)
                    {
                        // Update the console
                        foreach (string update in consoleUpdates) console.AppendText(update);

                        // Clear the updates
                        consoleUpdates.Clear();
                        
                        // Scroll to the latest
                        console.GoEnd();
                    }
                }
            };

            timer.Start();
        }

        #endregion Methods

        
    }
}
