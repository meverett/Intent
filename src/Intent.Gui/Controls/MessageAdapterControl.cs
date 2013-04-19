using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Intent.Gui
{
    /// <summary>
    /// Control that represents an active <see cref="MessageAdapter">message adapter</see>.
    /// </summary>
    public partial class MessageAdapterControl : UserControl
    {
        #region Fields

        // Whether or not there is any adapter input activity
        bool hasInput = false;

        // Whether or not there is any adapter output activity
        bool hasOutput = false;

        // Whether or not there is any adapter error activity
        bool hasError = false;

        // Cache image resources locally for faster look up
        static Bitmap activityReceive = Resources.ActivityIndicator_Receive;
        static Bitmap activitySend = Resources.ActivityIndicator_Send;
        static Bitmap activityError = Resources.ActivityIndicator_Error;
        static Bitmap activityNone = Resources.ActivityIndicator_NoActivity;

        // Used to draw the tiled backgroun
        TextureBrush bgBrush;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the message adapter bound to the control.
        /// </summary>
        public MessageAdapter MessageAdapter { get; private set; }

        /// <summary>
        /// Gets whether or not there is any adapter input activity.
        /// </summary>
        public bool HasInput
        {
            get { return hasInput; }

            set
            {
                if (hasInput != value)
                {
                    hasInput = value;
                }
            }
        }

        /// <summary>
        /// Gets whether or not there is any adapter output activity.
        /// </summary>
        public bool HasOutput
        {
            get { return hasOutput; }

            set
            {
                if (hasOutput != value)
                {
                    hasOutput = value;
                }
            }
        }

        /// <summary>
        /// Gets whether or not there is any adapter output activity.
        /// </summary>
        public bool HasErrors
        {
            get
            {
                return hasError;
            }

            set
            {
                if (hasError != value)
                {
                    hasError = value;
                }
            }
        }

        /// <summary>
        /// The last recorded input activity state.
        /// </summary>
        public bool LastInput { get; private set; }

        /// <summary>
        /// The last recorded output activity state.
        /// </summary>
        public bool LastOutput { get; private set; }

        /// <summary>
        /// The last recorded error state.
        /// </summary>
        public bool LastErrors { get; private set; }

        #endregion Properties

        #region Events

        /// <summary>
        /// Occurs when the remove button has been clicked.
        /// </summary>
        public event EventHandler Removed;

        #endregion Events

        #region Constructors

        public MessageAdapterControl()
        {
            InitializeComponent();
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //SetStyle(ControlStyles.UserPaint, true);
        }

        public MessageAdapterControl(MessageAdapter adapter)
        {
            InitializeComponent();

            if (adapter != null)
            {
                adapterName.Text = adapter.Name;
                MessageAdapter = adapter;

                // Monitor adapter events
                adapter.MessageReceived += adapter_MessageReceived;
                adapter.MessageSent += adapter_MessageSent;
            }

            // Setup background drawing brush
            bgBrush = new TextureBrush(BackgroundImage);
        }

        #endregion Constructors

        #region Methods

        #region Event Handlers

        // Message Adapter -> Message Received
        private void adapter_MessageReceived(object sender, EventArgs e)
        {
            HasInput = true;
        }

        // Message Adapter -> Message Sent
        private void adapter_MessageSent(object sender, EventArgs e)
        {
            HasOutput = true;
        }

        // Remove -> Click
        private void removeButton_Click(object sender, EventArgs e)
        {
            if (Removed != null) Removed(this, EventArgs.Empty);
        }

        // Generic -> Mouse Enter
        private void removeButton_MouseEnter(object sender, EventArgs e)
        {
            Program.Current.StatusText = "remove adapter: " + MessageAdapter.Name;
        }

        // Generic -> Mouse Leave
        private void removeButton_MouseLeave(object sender, EventArgs e)
        {
            Program.Current.StatusText = null;
        }

        // Custom painting
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            // Draw the correct activity light based on the current activity states
            g.DrawImage(hasError ? activityError : activityNone, Width - 81, 8, activityNone.Width, activityNone.Height);
            g.DrawImage(hasInput ? activityReceive : activityNone, Width - 52, 8, activityNone.Width, activityNone.Height);
            g.DrawImage(hasOutput ? activitySend : activityNone, Width - 23, 8, activityNone.Width, activityNone.Height);

            // Record the current state
            LastInput = hasInput;
            LastOutput = hasOutput;
            LastErrors = hasError;

            // Reset activity state now that it's been drawn and wait for more to come in
            hasInput = hasOutput = hasError = false;
        }
        

        #endregion Event Handlers

        /// <summary>
        /// Unbinds the control from the <see cref="MessageAdapter"/> it is currently bound to.
        /// </summary>
        public void UnbindAdapter()
        {
            MessageAdapter.MessageReceived -= adapter_MessageReceived;
            MessageAdapter.MessageSent -= adapter_MessageSent;
        }


        public void UpdateViewState()
        {
            // Record the current state
            LastInput = hasInput;
            LastOutput = hasOutput;
            LastErrors = hasError;

            // Clear the states
            hasInput = hasOutput = hasError = false;
        }

        #endregion Methods
    }
}
