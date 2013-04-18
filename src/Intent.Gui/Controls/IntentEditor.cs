using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using Intent;
using Intent.Json;
using FastColoredTextBoxNS;

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

        #region Text Editor Styles & Regex

        TextStyle functionStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
        TextStyle BlueBoldStyle = new TextStyle(Brushes.Pink, null, FontStyle.Bold);
        TextStyle BoldStyle = new TextStyle(null, null, FontStyle.Underline | FontStyle.Bold);
        TextStyle GrayStyle = new TextStyle(Brushes.Pink, null, FontStyle.Regular);
        TextStyle constantStyle = new TextStyle(Brushes.Lime, null, FontStyle.Regular);
        TextStyle commentsStyle = new TextStyle(Brushes.Orange, null, FontStyle.Italic);
        TextStyle stringStyle = new TextStyle(new SolidBrush(Color.FromArgb(255, 0, 151, 251)), null, FontStyle.Regular);
        TextStyle RedStyle = new TextStyle(Brushes.Pink, null, FontStyle.Regular);
        TextStyle MaroonStyle = new TextStyle(Brushes.Pink, null, FontStyle.Regular);

        Regex JScriptStringRegex;
        Regex JScriptCommentRegex1;
        Regex JScriptCommentRegex2;
        Regex JScriptCommentRegex3;
        Regex JScriptNumberRegex;
        Regex JScriptKeywordRegex;

        #endregion Text Editor Styles & Regex

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

                // If no adapter is selected, clear the editor
                if (adapter == null)
                {
                    textEditor.Text = "";
                    return;
                }

                if (!settingsByAdapter.ContainsKey(adapter))
                {
                    // Convert the current adapter settings to their JSON editable string format
                    settingsByAdapter.Add(adapter, adapter.CurrentSettingScript);
                }
                
                // Apply the current content to the editor
                textEditor.Text = settingsByAdapter[adapter];
            };

            // Initialize javascript regexes
            var option = PlatformType.GetOperationSystemPlatform() == Platform.X86 ? RegexOptions.Compiled : RegexOptions.None;
            JScriptStringRegex = new Regex("\"\"|''|\".*?[^\\\\]\"|'.*?[^\\\\]'", option);
            JScriptCommentRegex1 = new Regex("//.*$", RegexOptions.Multiline | option);
            JScriptCommentRegex2 = new Regex(@"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline | option);
            JScriptCommentRegex3 = new Regex(@"(/\*.*?\*/)|(.*\*/)", (RegexOptions.RightToLeft | RegexOptions.Singleline) | option);
            JScriptNumberRegex = new Regex(@"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b", option);
            JScriptKeywordRegex = new Regex(@"\b(true|false|break|case|catch|const|continue|default|delete|do|else|export|for|function|if|in|instanceof|new|null|return|switch|this|throw|try|var|void|while|with|typeof)\b", option);
        }

        // Text Editor -> text changed/edited
        private void textEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (listOfAdapters.SelectedAdapter == null) return;
            var currentText = settingsByAdapter[listOfAdapters.SelectedAdapter];
            bool hasChanged = !string.IsNullOrEmpty(currentText) && !string.IsNullOrEmpty(textEditor.Text) && currentText != textEditor.Text;
            settingsByAdapter[listOfAdapters.SelectedAdapter] = textEditor.Text;

            var range = e.ChangedRange;

            // Highlight the text
            range.tb.CommentPrefix = "//";
            range.tb.LeftBracket = '{';
            range.tb.RightBracket = '}';
            range.tb.LeftBracket2 = '[';
            range.tb.RightBracket2 = ']';

            range.ClearStyle(new Style[] { functionStyle, BoldStyle, GrayStyle, constantStyle, commentsStyle, stringStyle });

            range.SetStyle(stringStyle, JScriptStringRegex);
            range.SetStyle(commentsStyle, JScriptCommentRegex1);
            range.SetStyle(commentsStyle, JScriptCommentRegex2);
            range.SetStyle(commentsStyle, JScriptCommentRegex3);
            range.SetStyle(constantStyle, JScriptNumberRegex);
            range.SetStyle(functionStyle, JScriptKeywordRegex);
            range.ClearFoldingMarkers();
            range.SetFoldingMarkers("{", "}");
            range.SetFoldingMarkers(@"/\*", @"\*/");

            // Bubble
            if (ScriptChanged != null && hasChanged) ScriptChanged(this, EventArgs.Empty);
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
            textEditor.Text = "";
            listOfAdapters.Clear();
            ResumeLayout();
        }

        #endregion Adapter List

        #endregion Methods
    }
}
