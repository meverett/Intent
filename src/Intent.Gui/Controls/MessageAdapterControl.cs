using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
        bool inputActivity = false;

        // Whether or not there is any adapter output activity
        bool outputActivity = false;

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
            get { return inputActivity; }

            set
            {
                if (inputActivity != value)
                {
                    inputActivity = value;
                    inActivity.BackgroundImage = value ? Resources.ActivityIndicator_Receive : Resources.ActivityIndicator_NoActivity;
                }
            }
        }

        /// <summary>
        /// Gets whether or not there is any adapter output activity.
        /// </summary>
        public bool HasOutput
        {
            get { return outputActivity; }

            set
            {
                if (outputActivity != value)
                {
                    outputActivity = value;
                    outActivity.BackgroundImage = value ? Resources.ActivityIndicator_Send : Resources.ActivityIndicator_NoActivity;
                }
            }
        }

        /// <summary>
        /// Gets whether or not there is any adapter output activity.
        /// </summary>
        public bool HasErrors
        {
            get { return MessageAdapter != null ? MessageAdapter.HasErrors : false; }

            set
            {
                errorActivity.BackgroundImage = value ? Resources.ActivityIndicator_Error : Resources.ActivityIndicator_NoActivity;
            }
        }

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
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
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

        #endregion Event Handlers

        /// <summary>
        /// Unbinds the control from the <see cref="MessageAdapter"/> it is currently bound to.
        /// </summary>
        public void UnbindAdapter()
        {
            MessageAdapter.MessageReceived -= adapter_MessageReceived;
            MessageAdapter.MessageSent -= adapter_MessageSent;
        }

        #endregion Methods
    }
}
