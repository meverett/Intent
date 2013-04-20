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
    /// A custom list view control that displays a list of
    /// <see cref="MessageAdapter">message adapters</see>.
    /// </summary>
    public partial class MessageAdapterListView : UserControl
    {
        #region Fields

        // Activity timer to animate adapter message input/output activity indicators
        Timer timer;

        // Used to create a delta time between frames for animation effects
        DateTime lastTime;
        double elapsedMs;
        bool showErrors;

        // Mouse over status update
        StatusUpdate status;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the currently selected message adapter.
        /// </summary>
        public MessageAdapter SelectedAdapter { get; private set; }

        #endregion Properties

        #region Events

        /// <summary>
        /// Occurs when a specific message adapter is selected in the UI.
        /// </summary>
        public event EventHandler AdapterSelected;

        #endregion Events

        #region Constructors

        public MessageAdapterListView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        #region Event Handlers

        #region Control

        // Control -> Load
        private void Control_Load(object sender, EventArgs e)
        {
            timer = new Timer();
            timer.Interval = 66;
            lastTime = DateTime.Now;

            // Tick handler, update the activity indicators of the adapter controls
            timer.Tick += (s, eArg) =>
            {
                if (elapsedMs >= 500)
                {
                    elapsedMs = 0;
                    showErrors = !showErrors;                   
                }
                else
                {
                    var now = DateTime.Now;
                    elapsedMs += (now - lastTime).TotalMilliseconds;
                    lastTime = now;
                }

                foreach (MessageAdapterControl control in adaptersPanel.Controls)
                {
                    // Pulse errors if there are any
                    control.HasErrors = IntentMessaging.IsRunning && showErrors && control.MessageAdapter.HasErrors;

                    // Force the control to redraw
                    control.Invalidate();
                }
            };

            timer.Start();
        }

        // Prevent second background drawing pass we won't use
        protected override void OnPaintBackground(PaintEventArgs e) { }

        #endregion Control

        #region Add/Remove Buttons

        // Add Adapter -> Click
        private void addAdapterButton_Click(object sender, EventArgs e)
        {
            if (availableAdapters.Items.Count == 0) return;

            // Retrieve the currently selected adapter info
            var selected = (MessageAdapterInfo)availableAdapters.SelectedItem;

            // Add the message adapter to the messaging system
            var adapter = IntentMessaging.AddAdapter(selected.Type);
            AddMessageAdapterControl(adapter);            
        }

        // Generic -> Mouse Enter
        private void button_MouseEnter(object sender, EventArgs e)
        {
            var text = "add adapter: " + availableAdapters.SelectedItem.ToString();
            status = Program.Current.AddStatus(new StatusUpdate(text, StatusTypes.Hint));
        }

        // Generic -> Mouse Leave
        private void button_MouseLeave(object sender, EventArgs e)
        {
            Program.Current.RemoveStatus(status);
        }

        #endregion Add/Remove Buttons

        #region Message Adapter Control Handlers

        // Message Adapter -> Removed
        void control_Removed(object sender, EventArgs e)
        {
            RemoveControl((MessageAdapterControl)sender);
        }

        // Message Adapter Control -> Clicked
        void control_MouseClick(object sender, MouseEventArgs e)
        {
            SelectMessageAdapter((MessageAdapterControl)sender);
        }

        #endregion Message Adapter Control Handlers

        #endregion Event Handlers

        #region Operation

        /// <summary>
        /// Selects the specified message adapter in the list view.
        /// </summary>
        /// <param name="adapter">The message adapter to select.</param>
        public void SelectMessageAdapter(MessageAdapter adapter)
        {
            var controls = new Control[adaptersPanel.Controls.Count];
            adaptersPanel.Controls.CopyTo(controls, 0);

            // Get the contol that is bound to the given adapter
            var selected = (MessageAdapterControl)controls.First(c => ((MessageAdapterControl)c).MessageAdapter == adapter);
            SelectMessageAdapter(selected);
        }

        // Sets the selected state of the current message adapter control
        void SelectMessageAdapter(MessageAdapterControl selected)
        {
            foreach (MessageAdapterControl control in adaptersPanel.Controls)
            {
                control.BackColor = selected == control ? Color.FromArgb(255, 0, 151, 251) : Color.FromArgb(255, 27, 27, 28);
                control.ForeColor = selected == control ? Color.White : Color.FromArgb(255, 0, 151, 251);
            }

            SelectedAdapter = selected.MessageAdapter;
            if (AdapterSelected != null) AdapterSelected(this, EventArgs.Empty);
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

        /// <summary>
        /// Refreshes the list of available 
        /// <see cref="MessageAdapter">message adapters</see>.
        /// </summary>
        public void RefreshAvailableList()
        {
            availableAdapters.Items.Clear();
            availableAdapters.BeginUpdate();

            foreach (var type in IntentMessaging.GetAdapterTypes())
            {
                var info = new MessageAdapterInfo(IntentMessaging.GetName(type), type);
                availableAdapters.Items.Add(info);
            }

            availableAdapters.SelectedIndex = 0;
            availableAdapters.EndUpdate();
        }

        /// <summary>
        /// Clears the control of its current data set.
        /// </summary>
        public void Clear()
        {
            SuspendLayout();

            // Make a copy of the controls so that we don't throw an error of the collection being
            // modified while also being enumerated.
            var controls = new List<MessageAdapterControl>();
            foreach (MessageAdapterControl control in adaptersPanel.Controls) controls.Add(control);
            foreach (var control in controls) RemoveControl(control);

            ResumeLayout();
        }

        /// <summary>
        /// Refreshes the current list of adaptes to the latest active list of adapters.
        /// </summary>
        public void RefreshActiveAdapters()
        {
            SuspendLayout();

            // Clear current data set
            Clear();
            foreach (MessageAdapter adapter in IntentMessaging.ActiveAdapters) AddMessageAdapterControl(adapter);
            ResumeLayout();
        }

        // Removes the specified control from the list
        void RemoveControl(MessageAdapterControl control)
        {
            control.MouseClick -= control_MouseClick;
            control.Removed -= control_Removed;
            control.UnbindAdapter();
            IntentMessaging.RemoveAdapter(control.MessageAdapter);
            adaptersPanel.Controls.Remove(control);
            
            // Select another control
            if (control.MessageAdapter == SelectedAdapter)
            {
                // If there's at least one adapter, selcect the first in the list
                if (adaptersPanel.Controls.Count > 0)
                {
                    var firstControl = (MessageAdapterControl)adaptersPanel.Controls[0];
                    SelectMessageAdapter(firstControl);
                }
                // Otherwise do a NULL selection
                else
                {
                    SelectedAdapter = null;
                    if (AdapterSelected != null) AdapterSelected(this, EventArgs.Empty);
                }
            }
        }

        // Adds a new control to represent an active message adapter and binds to it
        void AddMessageAdapterControl(MessageAdapter adapter)
        {
            // Create the UI control to represent the adapter
            var control = new MessageAdapterControl(adapter);

            // Bind to events
            control.Removed += control_Removed;
            control.MouseClick += control_MouseClick;

            // Size and add to layout
            control.Width = adaptersPanel.Width - 8;
            //control.Width = bottomPanel.Width - 24; // for scroll bars
            adaptersPanel.Controls.Add(control);

            // Select the newly added control
            SelectMessageAdapter(control);
        }

        #endregion Operation

        #endregion Methods
    }
}
