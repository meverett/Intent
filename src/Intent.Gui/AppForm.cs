using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using Intent;
using Intent.Midi;
using Intent.Osc;

namespace Intent.Gui
{
    #region Enumerations

    /// <summary>
    /// Different types of status text states.
    /// </summary>
    public enum StatusTypes
    {
        /// <summary>
        /// The status content is normal.
        /// </summary>
        Normal,

        /// <summary>
        /// The status indicates an error has occured.
        /// </summary>
        Error,

        /// <summary>
        /// The status indicates a warning message.
        /// </summary>
        Warning,

        /// <summary>
        /// The status indicates an application hint/helper.
        /// </summary>
        Hint,

        /// <summary>
        /// A temporary status message that should be removed after a period of time.
        /// </summary>
        Temporary,
    }

    #endregion Enumerations

    /// <summary>
    /// Main Intent GUI application form.
    /// </summary>
    public partial class AppForm : Form
    {
        #region Fields

        // Whether or not the left mouse button is currently down
        bool isLeftMouseDown = false;

        // Whether or not the current document has any unsaved changes
        bool isDirty = false;

        // The point where the mouse was last left-clicked
        Point mouseOffset;

        // The console control
        IntentConsole console;

        // The editor control
        IntentEditor editor;

        // Holds a list of application status updates
        List<StatusUpdate> statusList;
        int statusIndex = 0; // current status update index in list

        // Current script compliation error, if any
        StatusUpdate scriptError;

        // The current hint/mouse over status, if any
        StatusUpdate hintStatus;

        // The current timeout status, if any
        StatusUpdate timeoutStatus;
        Timer statusTimer; // used for timeout status

        #endregion Fields

        #region Properties

        #endregion Properties

        #region Constructors

        public AppForm()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
        }

        #endregion Constructors

        #region Methods

        #region Event Handlers

        #region Form

        // Application load/init
        private void Form_Load(object sender, EventArgs e)
        {
            Icon = Icon.FromHandle(Resources.AppIconDark.GetHicon());

            // Add controls
            console = new IntentConsole();
            activePanel.Controls.Add(console);
            console.Dock = DockStyle.Fill;
            console.Visible = false;

            editor = new IntentEditor();
            activePanel.Controls.Add(editor);
            editor.Dock = DockStyle.Fill;
            editor.Visible = true;

            // Register to messaging events that would create a dirty document
            IntentMessaging.AdaptersUpdated += (s, eArgs) => { isDirty = true; };
            editor.ScriptChanged += (s, eArgs) => { isDirty = true; buildScriptsButton.Enabled = true; };

            // Initialize application status list
            statusList = new List<StatusUpdate>();
            statusTimer = new Timer();
            statusTimer.Interval = 3000;
            statusTimer.Tick += statusTimer_Tick;

            // Create a new project file/session
            FileNew();
        }

        // Clean up system on exit
        private void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            IntentMessaging.Stop();
        }

        #region Custom Mouse Handling

        // Form -> Mouse Down
        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            isLeftMouseDown = true;
            int width = SystemInformation.FrameBorderSize.Width;
            int height = SystemInformation.FrameBorderSize.Height;
            mouseOffset = new Point(-(e.X + width), -(e.Y + height));
        }

        // Form -> Mouse Up
        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            isLeftMouseDown = false;
        }

        // Form -> Mouse Move
        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isLeftMouseDown) return;
            Point mouse = Control.MousePosition;
            mouse.Offset(mouseOffset.X, mouseOffset.Y);
            Location = mouse;
        }

        // Form -> Mouse Double Click
        private void Form_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            switch (WindowState)
            {
                case FormWindowState.Normal:
                    WindowState = FormWindowState.Maximized;
                    break;

                case FormWindowState.Maximized:
                    WindowState = FormWindowState.Normal;
                    break;

                case FormWindowState.Minimized:
                    WindowState = FormWindowState.Maximized;
                    break;
            }
        }

        #endregion Custom Mouse Handling

        #endregion Form

        #region Window Buttons

        // Window -> Close
        private void closeButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Window -> Maximize
        private void maximizeButton_Click(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case FormWindowState.Normal:
                    WindowState = FormWindowState.Maximized;
                    break;

                case FormWindowState.Maximized:
                    WindowState = FormWindowState.Normal;
                    break;

                case FormWindowState.Minimized:
                    WindowState = FormWindowState.Maximized;
                    break;
            }
        }

        // Window -> Minimize
        private void minimizeButton_Click(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case FormWindowState.Normal:
                    WindowState = FormWindowState.Minimized;
                    break;

                case FormWindowState.Minimized:
                    WindowState = FormWindowState.Normal;
                    break;

                case FormWindowState.Maximized:
                    WindowState = FormWindowState.Minimized;
                    break;
            }
        }

        #endregion Window Buttons

        #region Active Panel Buttons

        private void editorButton_Click(object sender, EventArgs e)
        {
            editor.Visible = true;
            editor.Start();
            console.Visible = false;
            console.Stop();
        }

        private void consoleButton_Click(object sender, EventArgs e)
        {
            editor.Visible = false;
            editor.Stop();
            console.Visible = true;
            console.Start();
        }

        #endregion Active Panel Buttons

        #region Menu Buttons

        // Intercept form-levle key commands for command shortcut processing
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                // File -> New
                case Keys.Control | Keys.N:
                    FileNew();
                    break;

                // File -> Open
                case Keys.Control | Keys.O:
                    FileOpen();
                    break;

                // File -> Save
                case Keys.Control | Keys.S:
                    FileSave();
                    break;

                // File -> Save As
                case Keys.Control | Keys.S | Keys.Shift:
                    FileSaveAs();
                    break;

                // Scripts -> Build All
                case Keys.Control | Keys.B:
                    BuildScripts();
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #region File

        // File -> New
        private void newButton_Click(object sender, EventArgs e)
        {
            FileNew();
        }

        // File -> Open
        private void openButton_Click(object sender, EventArgs e)
        {
            FileOpen();
        }

        // File -> Save
        private void saveButton_Click(object sender, EventArgs e)
        {
            FileSave();
        }

        // File -> Save As
        private void saveAsButton_Click(object sender, EventArgs e)
        {
            FileSaveAs();
        }

        #endregion File

        #region Controls

        // Start Messaging -> Click
        private void startButton_Click(object sender, EventArgs e)
        {
            if (IntentMessaging.IsRunning) return;

            try
            {
                IntentMessaging.Start();
                startButton.Enabled = false;
                stopButton.Enabled = true;
            }
            catch (Exception ex)
            {
                IntentMessaging.WriteLine(ex);
            }
        }

        // Stop Messaging -> Click
        private void stopButton_Click(object sender, EventArgs e)
        {
            if (!IntentMessaging.IsRunning) return;

            try
            {
                IntentMessaging.Stop();
                startButton.Enabled = true;
                stopButton.Enabled = false;
            }
            catch (Exception ex)
            {
                IntentMessaging.WriteLine(ex);
            }
        }

        #endregion Controls

        #region Script

        // Build All Scripts -> Click
        private void buildScriptsButton_Click(object sender, EventArgs e)
        {
            BuildScripts();
        }

        #endregion Script

        #region Console

        // Clear Console -> Click
        private void clearConsoleButton_Click(object sender, EventArgs e)
        {
            console.Clear();
        }

        #endregion Console

        #region Generic

        // Generic -> Mouse Enter
        private void menuButton_MouseEnter(object sender, EventArgs e)
        {
            var button = (Button)sender;
            string text = null;

            #region Update status by button name

            if (button.Name == editorButton.Name)
            {
                text = "show the editor";
            }
            else if (button.Name == consoleButton.Name)
            {
                text = "show the console";
            }
            else if (button.Name == newButton.Name)
            {
                text = "create a new message routing file";
            }
            else if (button.Name == openButton.Name)
            {
                text = "open an existing message routing file";
            }
            else if (button.Name == saveButton.Name)
            {
                text = "save the current message routing file";
            }
            else if (button.Name == saveAsButton.Name)
            {
                text = "save a copy of the current message routing file";
            }
            else if (button.Name == startButton.Name)
            {
                text = "start message routing";
            }
            else if (button.Name == stopButton.Name)
            {
                text = "stop message routing";
            }
            else if (button.Name == buildScriptsButton.Name)
            {
                text = "build all message adapter scripts";
            }
            else if (button.Name == clearConsoleButton.Name)
            {
                text = "clear the console";
            }

            #endregion Update status by button name

            hintStatus = AddStatus(new StatusUpdate(text, StatusTypes.Hint));
        }

        // Generic -> Mouse Leave
        private void menuButton_MouseLeave(object sender, EventArgs e)
        {
            if (statusList.Count > 0) RemoveStatus(hintStatus);
        }

        #endregion Generic

        #endregion Menu Buttons

        #endregion Event Handlers

        #region File Operations

        // Creates a new session/file
        void FileNew()
        {
            // Don't blow away changes
            if (!ConfirmIsDirty()) return;

            // Remove save file name
            saveFileDialog.FileName = null;

            // Stop and clear current adapter list
            Clear();

            // Notify
            AddStatus(new StatusUpdate("new file created", StatusTypes.Temporary));
        }

        // Saves the current session
        void FileSave()
        {
            // Nothing to save
            if (!isDirty)
            {
                // Notify through status that the file was saved
                AddStatus(new StatusUpdate("no changes to save", StatusTypes.Warning));
                return;
            }

            // If we don't already have a saved file path, get one
            if (string.IsNullOrEmpty(saveFileDialog.FileName) || !File.Exists(saveFileDialog.FileName))
                if (DialogResult.OK != saveFileDialog.ShowDialog()) return;

            XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };

            using (XmlWriter xw = XmlWriter.Create(saveFileDialog.FileName, settings))
            {
                SessionToXml().WriteTo(xw);
            }

            isDirty = false;

            // Notify through status that the file was saved
            AddStatus(new StatusUpdate("file saved: " + saveFileDialog.FileName, StatusTypes.Temporary));
        }

        // Saves the current session as a new file
        void FileSaveAs()
        {
            // Get the save file path
            if (DialogResult.OK != saveFileDialog.ShowDialog()) return;

            XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };

            using (XmlWriter xw = XmlWriter.Create(saveFileDialog.FileName, settings))
            {
                SessionToXml().WriteTo(xw);
            }

            isDirty = false;

            // Notify through status that the file was saved
            AddStatus(new StatusUpdate("file saved: " + saveFileDialog.FileName, StatusTypes.Temporary));
        }

        // Opens a file/session for editing and playback
        void FileOpen()
        {
            // Don't blow away changes
            if (!ConfirmIsDirty()) return;

            if (DialogResult.OK != openFileDialog.ShowDialog()) return;

            // Read in the document
            try
            {
                // Clear the current file/session
                Clear();

                var doc = new XmlDocument();
                doc.LoadXml(File.ReadAllText(openFileDialog.FileName));

                // Get the message adapters node
                var messageAdapters = doc.SelectNodes("//MessageAdapter");

                foreach (XmlNode adapter in messageAdapters)
                {
                    // Make sure we have a valid name
                    var name = adapter.Attributes["name"];
                    if (name == null || string.IsNullOrEmpty(name.Value))
                        throw new ApplicationException("<MessageAdapter> element is missing the 'name' attribute:\n\n" + adapter.OuterXml);

                    // Get the adapters script settings
                    var settingsScript = adapter.InnerText;

                    // Add the adapter
                    IntentMessaging.AddAdapter(name.Value, settingsScript);
                }

                // Update the UI state to the new file data
                editor.RefreshActiveAdapters();

                // Select the first one
                var firstAdapter = IntentMessaging.ActiveAdapters.FirstOrDefault();
                if (firstAdapter != null) editor.SelectMessageAdapter(firstAdapter);

                // Set the save file name to the opened file name
                saveFileDialog.FileName = Path.GetFileName(openFileDialog.FileName);

                // If the message adapter was currently playing, resume
                if (stopButton.Enabled) IntentMessaging.Start();

                isDirty = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "There was an error opening the file:\n" + openFileDialog.FileName, 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error
                );
            }

            // Notify through status that the file was opened
            AddStatus(new StatusUpdate("file opened: " + openFileDialog.FileName, StatusTypes.Temporary));
        }

        // Used to confirm a potentially data destroying action between saved changes
        bool ConfirmIsDirty()
        {
            var question = "You have unsaved changes. Do you wish to continue and lose your changes?";
            if (isDirty && MessageBox.Show(question, "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel) return false;
            return true;
        }

        // Converts the current session's data into an XML document
        XmlDocument SessionToXml()
        {
            // Create the document
            var doc = new XmlDocument();

            // Append the root node
            var root = doc.CreateElement("Intent");
            doc.AppendChild(root);

            // Add nodes for all message adapters
            var messageAdapters = doc.CreateElement("MessageAdapters");
            root.AppendChild(messageAdapters);

            foreach (MessageAdapter adapter in IntentMessaging.ActiveAdapters)
            {
                // Create an element for the message adapter.
                var node = doc.CreateElement("MessageAdapter");
                messageAdapters.AppendChild(node);

                // Add message adapter name
                var name = doc.CreateAttribute("name");
                name.Value = adapter.Name;
                node.Attributes.Append(name);

                // Add the current editor JavaScript for this adapter to the node as CDATA
                var script = doc.CreateCDataSection(editor.GetAdapterScript(adapter));
                node.AppendChild(script);
            }

            return doc;
        }

        // Clears the current document/session
        void Clear()
        {
            IntentMessaging.Stop();
            editor.Clear();
            console.Clear();
            IntentMessaging.ClearAdapters();
            isDirty = false;
        }

        #endregion File Operations

        #region Script

        // Build all project javascript files
        void BuildScripts()
        {
            try
            {
                foreach (MessageAdapter adapter in IntentMessaging.ActiveAdapters)
                {
                    // Get the current script text
                    adapter.ApplySettings(editor.GetAdapterScript(adapter));

                    // Make sure there were no settings errors
                    if (adapter.SettingsException != null)
                    {
                        IntentMessaging.WriteLine("! Compile Error:");
                        IntentMessaging.WriteLine((object)adapter.SettingsException.SourceCode);

                        // Add error status
                        var text = "Script compliation error! See console for details.";
                        scriptError = AddStatus(new StatusUpdate(text, StatusTypes.Error));
                        return;
                    }
                    else
                    {
                        adapter.HasErrors = false;
                    }
                }

                buildScriptsButton.Enabled = false;

                // Remove any prior script error status
                if (scriptError != null)
                {
                    RemoveStatus(scriptError);
                    scriptError = null;
                }

                // Start messaging service to send any new init data
                if (IntentMessaging.IsRunning)
                {
                    IntentMessaging.Stop();
                    IntentMessaging.Start();
                }

                // Add successful compilation message
                AddStatus(new StatusUpdate("all scripts built successfully", StatusTypes.Temporary));
            }
            catch (IronJS.Error.CompileError ce)
            {
                IntentMessaging.WriteLine("! Compile Error:");
                IntentMessaging.WriteLine((object)ce.SourceCode);

                // Add error status
                var text = "Script compliation error! See console for details.";
                scriptError = AddStatus(new StatusUpdate(text, StatusTypes.Error));
            }
            catch (Exception ex)
            {
                IntentMessaging.WriteLine(ex);
            }
        }

        #endregion Script

        #region Utilities

        /// <summary>
        /// Adds a new status update to the application form to be displayed.
        /// </summary>
        /// <param name="update">The status update to add and display.</param>
        internal StatusUpdate AddStatus(StatusUpdate update)
        {
            // Register the status update
            statusList.Add(update);
            int index = statusList.Count - 1;

            switch (update.Type)
            {
                case StatusTypes.Error: status.ForeColor = Color.Red; break;
                case StatusTypes.Warning: status.ForeColor = Color.Orange; break;
                case StatusTypes.Hint:  status.ForeColor = Color.FromArgb(255, 0, 151, 251); break;
                case StatusTypes.Normal: status.ForeColor = Color.FromArgb(255, 0, 151, 251); break;
            }

            // If this update has a lifespan, set a timer on callback to remove it when it's dead
            if (update.Type == StatusTypes.Temporary || update.Type == StatusTypes.Warning)
            {
                lock (statusList)
                {
                    // Stop any current timer
                    statusTimer.Stop();

                    // If there is a current timeout status, clear it immediately
                    if (timeoutStatus != null) RemoveStatus(timeoutStatus);
                    timeoutStatus = update; // always use update, not var current
                    statusTimer.Start();
                }
            }

            // Update the actual status label
            status.Text = update.Text;

            return update;
        }

        // Handler for timeout status updates
        void statusTimer_Tick(object sender, EventArgs e)
        {
            if (timeoutStatus == null) return;
            RemoveStatus(timeoutStatus);
            timeoutStatus = null;
        }

        /// <summary>
        /// Removes an existing status update from the application form.
        /// </summary>
        /// <param name="update">The status update to remove.</param>
        internal void RemoveStatus(StatusUpdate update)
        {
            lock (statusList)
            {
                // Get the index of the update in the status list first
                int index = statusList.IndexOf(update);

                // Remove the status
                statusList.Remove(update);

                // All status entries slide down one
                if (statusIndex > 0) statusIndex--;

                // If this is the current status we need to fall back one status if available
                //if (index == statusIndex) statusIndex -= statusIndex > 0 ? 1 : 0;

                // Update the status to the next available status
                if (statusList.Count <= 0)
                {
                    status.Text = null;
                    return;
                }

                var current = statusList[statusIndex];

                switch (current.Type)
                {
                    case StatusTypes.Error: status.ForeColor = Color.Red; break;
                    case StatusTypes.Warning: status.ForeColor = Color.Orange; break;
                    case StatusTypes.Hint: status.ForeColor = Color.FromArgb(255, 0, 151, 251); break;
                    case StatusTypes.Normal: status.ForeColor = Color.FromArgb(255, 0, 151, 251); break;
                }

                status.Text = current.Text;
            }
        }

        // Clears all currently registered timer.Tick event delegates (useful if anonymous)
        //void ClearTimerDelegates()
        //{
        //    var methodInfo = timer.GetType().GetEvent("Tick").EventHandlerType.GetMethod("GetInvocationList");
        //    Delegate[] handlers = (Delegate[])methodInfo.Invoke(timer, null);
        //    foreach (Delegate d in handlers) timer.Tick -= (EventHandler)d;
        //}

        #endregion Utilities

        #endregion Methods
    }
}
