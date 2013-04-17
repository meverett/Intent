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

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the form's status bar text.
        /// </summary>
        public string StatusText
        {
            get { return status.Text; }
            set { status.Text = value; }
        }

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

        #region File

        // File -> New
        private void newButton_Click(object sender, EventArgs e)
        {
            // Stop and clear current adapter list
            Clear();
        }

        // File -> Open
        private void openButton_Click(object sender, EventArgs e)
        {
            Open();
        }

        // File -> Save
        private void saveButton_Click(object sender, EventArgs e)
        {
            Save();
        }

        // File -> Save As
        private void saveAsButton_Click(object sender, EventArgs e)
        {
            SaveAs();
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

        // Build Current Script -> Click
        private void buildCurrentButton_Click(object sender, EventArgs e)
        {
            var adapter = editor.SelectedAdapter;
            if (adapter == null) return;

            try
            {
                // Get the current script text
                adapter.ApplySettings(editor.Script);
            }
            catch (Exception ex)
            {
                IntentMessaging.WriteLine(ex);
            }
        }

        // Build All Scripts -> Click
        private void buildAllButton_Click(object sender, EventArgs e)
        {

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

            #region Update status by button name

            if (button.Name == editorButton.Name)
            {
                Console.WriteLine(button.Name);
                status.Text = "show the editor";
            }
            else if (button.Name == consoleButton.Name)
            {
                status.Text = "show the console";
            }
            else if (button.Name == newButton.Name)
            {
                status.Text = "create a new message routing file";
            }
            else if (button.Name == openButton.Name)
            {
                status.Text = "open an existing message routing file";
            }
            else if (button.Name == saveButton.Name)
            {
                status.Text = "save the current message routing file";
            }
            else if (button.Name == saveAsButton.Name)
            {
                status.Text = "save a copy of the current message routing file";
            }
            else if (button.Name == startButton.Name)
            {
                status.Text = "start message routing";
            }
            else if (button.Name == stopButton.Name)
            {
                status.Text = "stop message routing";
            }
            else if (button.Name == buildCurrentButton.Name)
            {
                status.Text = "build current message adapter script";
            }
            else if (button.Name == buildAllButton.Name)
            {
                status.Text = "build all message adapter scripts";
            }
            else if (button.Name == clearConsoleButton.Name)
            {
                status.Text = "clear the console";
            }

            #endregion Update status by button name
        }

        // Generic -> Mouse Leave
        private void menuButton_MouseLeave(object sender, EventArgs e)
        {
            status.Text = null;
        }

        #endregion Generic

        #endregion Menu Buttons

        #endregion Event Handlers

        #region File Operations

        // Saves the current session
        void Save()
        {
            // If we don't already have a saved file path, get one
            if (string.IsNullOrEmpty(saveFileDialog.FileName))
                if (DialogResult.OK != saveFileDialog.ShowDialog()) return;

            XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };

            using (XmlWriter xw = XmlWriter.Create(saveFileDialog.FileName, settings))
            {
                SessionToXml().WriteTo(xw);
            }
        }

        // Saves the current session as a new file
        void SaveAs()
        {
            // Get the save file path
            if (DialogResult.OK != saveFileDialog.ShowDialog()) return;

            XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };

            using (XmlWriter xw = XmlWriter.Create(saveFileDialog.FileName, settings))
            {
                SessionToXml().WriteTo(xw);
            }
        }

        // Opens a file/session for editing and playback
        void Open()
        {
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "There was an error opening the file:\n" + openFileDialog.FileName, 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error
                );
            }
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
        }

        #endregion File Operations

        #endregion Methods
    }
}
