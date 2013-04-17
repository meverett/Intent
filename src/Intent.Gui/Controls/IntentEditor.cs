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
using Intent.Json;

namespace Intent.Gui
{
    /// <summary>
    /// Control that lets you create and edit Intent messaging configurations.
    /// </summary>
    public partial class IntentEditor : UserControl
    {
        #region Fields

        // Stores the editable settings/configure text by the adapter that it is for
        Dictionary<MessageAdapter, string> settingsByAdapter;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the message adapter that is currently selected in the editor.
        /// </summary>
        public MessageAdapter SelectedAdapter
        {
            get { return listOfAdapters.SelectedAdapter; }
        }

        /// <summary>
        /// Gets the script currently in the text editor.
        /// </summary>
        public string Script
        {
            get { return textEditor.Text; }
        }

        #endregion Properties

        #region Events

        /// <summary>
        /// Occurs when the script in the editor has been changed.
        /// </summary>
        public event EventHandler ScriptChanged;

        #endregion Events

        #region Constructors

        public IntentEditor()
        {
            settingsByAdapter = new Dictionary<MessageAdapter, string>();
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        #region Event Handlers

        private void Editor_Load(object sender, EventArgs e)
        {
            // Load in the list of available message adapters into the view control
            listOfAdapters.RefreshAvailableList();

            // Listen for when an active message adapter is selected in the UI
            listOfAdapters.AdapterSelected += (s, eArgs) =>
            {
                
                var adapter = listOfAdapters.SelectedAdapter;

                if (!settingsByAdapter.ContainsKey(adapter))
                {
                    // Convert the current adapter settings to their JSON editable string format
                    settingsByAdapter.Add(adapter, adapter.CurrentSettingScript);
                }
                
                // Apply the current content to the editor
                textEditor.Text = settingsByAdapter[adapter];
            };

            // Listen for changes in the text editor and update the adapter text
            textEditor.TextChanged += (s, eArgs) =>
            {
                if (listOfAdapters.SelectedAdapter == null) return;
                settingsByAdapter[listOfAdapters.SelectedAdapter] = textEditor.Text;

                // Bubble
                if (ScriptChanged != null) ScriptChanged(this, EventArgs.Empty);
            };

            textEditor.KeyUp += (s, eArgs) =>
            {
                
            };
        }

        #endregion Event Handlers

        #region Operation

        /// <summary>
        /// Starts the animation timer.
        /// </summary>
        public void Start()
        {
            listOfAdapters.Start();
        }

        /// <summary>
        /// Stops the animation timer.
        /// </summary>
        public void Stop()
        {
            listOfAdapters.Stop();
        }

        /// <summary>
        /// Gets the editor's current settings script for a given message adapter.
        /// </summary>
        /// <param name="adapter">The message adapter who's script will be returned.</param>
        /// <returns>The script if the adapter has one, otherwise NULL.</returns>
        public string GetAdapterScript(MessageAdapter adapter)
        {
            if (!settingsByAdapter.ContainsKey(adapter)) return null;
            return settingsByAdapter[adapter];
        }

        /// <summary>
        /// Selects the specified message adapter in the list view.
        /// </summary>
        /// <param name="adapter">The message adapter to select.</param>
        public void SelectMessageAdapter(MessageAdapter adapter)
        {
            listOfAdapters.SelectMessageAdapter(adapter);
        }

        /// <summary>
        /// Refreshes the current list of adaptes to the latest active list of adapters.
        /// </summary>
        public void RefreshActiveAdapters()
        {
            listOfAdapters.RefreshActiveAdapters();
        }

        #endregion Operation

        #region Adapter List

        /// <summary>
        /// Clears the control of its current data.
        /// </summary>
        public void Clear()
        {
            SuspendLayout();
            textEditor.Text = null;
            listOfAdapters.Clear();
            ResumeLayout();
        }

        #endregion Adapter List

        #endregion Methods
    }
}
