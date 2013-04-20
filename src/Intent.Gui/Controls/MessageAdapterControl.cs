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

        // Mouse over status update
        StatusUpdate status;

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
        public bool HasInput { get; set; }
       
        /// <summary>
        /// Gets whether or not there is any adapter output activity.
        /// </summary>
        public bool HasOutput { get; set; }

        /// <summary>
        /// Gets whether or not there is any adapter output activity.
        /// </summary>
        public bool HasErrors { get; set; }

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
            string text = "remove adapter: " + MessageAdapter.Name;
            status = Program.Current.AddStatus(new StatusUpdate(text, StatusTypes.Hint));
        }

        // Generic -> Mouse Leave
        private void removeButton_MouseLeave(object sender, EventArgs e)
        {
            Program.Current.RemoveStatus(status);
        }

        // Custom painting
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            // Draw the correct activity light based on the current activity states
            g.DrawImage(HasErrors ? activityError : activityNone, Width - 81, 8, activityNone.Width, activityNone.Height);
            g.DrawImage(HasInput ? activityReceive : activityNone, Width - 52, 8, activityNone.Width, activityNone.Height);
            g.DrawImage(HasOutput ? activitySend : activityNone, Width - 23, 8, activityNone.Width, activityNone.Height);

             // Reset activity state now that it's been drawn and wait for more to come in
            HasInput = HasOutput = HasErrors = false;
        }
        

        #endregion Event Handlers

        #region Clean Up

        /// <summary>
        /// Unbinds the control from the <see cref="MessageAdapter"/> it is currently bound to.
        /// </summary>
        public void UnbindAdapter()
        {
            MessageAdapter.MessageReceived -= adapter_MessageReceived;
            MessageAdapter.MessageSent -= adapter_MessageSent;
        }

        #endregion Clean Up

        #endregion Methods
    }
}
