namespace Intent.Gui
{
    partial class AppForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeButton = new System.Windows.Forms.Button();
            this.minimizeButton = new System.Windows.Forms.Button();
            this.editorButton = new System.Windows.Forms.Button();
            this.consoleButton = new System.Windows.Forms.Button();
            this.activePanel = new System.Windows.Forms.Panel();
            this.formTitle = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.status = new System.Windows.Forms.ToolStripStatusLabel();
            this.maximizeButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.clearConsoleButton = new System.Windows.Forms.Button();
            this.buildScriptsButton = new System.Windows.Forms.Button();
            this.newButton = new System.Windows.Forms.Button();
            this.openButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.saveAsButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.Font = new System.Drawing.Font("Verdana", 10F);
            this.fileToolStripMenuItem.ForeColor = System.Drawing.Color.Gray;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(42, 21);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(28)))));
            this.newToolStripMenuItem.ForeColor = System.Drawing.Color.Gray;
            this.newToolStripMenuItem.Image = global::Intent.Gui.Resources.File_New;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(28)))));
            this.openToolStripMenuItem.ForeColor = System.Drawing.Color.Gray;
            this.openToolStripMenuItem.Image = global::Intent.Gui.Resources.File_Open;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Font = new System.Drawing.Font("Verdana", 10F);
            this.editToolStripMenuItem.ForeColor = System.Drawing.Color.Gray;
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(47, 21);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.BackColor = System.Drawing.Color.Transparent;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.Font = new System.Drawing.Font("Verdana", 7F);
            this.closeButton.ForeColor = System.Drawing.Color.Gray;
            this.closeButton.Location = new System.Drawing.Point(877, 0);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(23, 23);
            this.closeButton.TabIndex = 2;
            this.closeButton.Text = "X";
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // minimizeButton
            // 
            this.minimizeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.minimizeButton.BackColor = System.Drawing.Color.Transparent;
            this.minimizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimizeButton.Font = new System.Drawing.Font("Verdana", 7F);
            this.minimizeButton.ForeColor = System.Drawing.Color.Gray;
            this.minimizeButton.Location = new System.Drawing.Point(819, 0);
            this.minimizeButton.Name = "minimizeButton";
            this.minimizeButton.Size = new System.Drawing.Size(23, 23);
            this.minimizeButton.TabIndex = 0;
            this.minimizeButton.Text = "_";
            this.minimizeButton.UseVisualStyleBackColor = false;
            this.minimizeButton.Click += new System.EventHandler(this.minimizeButton_Click);
            // 
            // editorButton
            // 
            this.editorButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.editorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.editorButton.Font = new System.Drawing.Font("Verdana", 7F);
            this.editorButton.ForeColor = System.Drawing.Color.Gray;
            this.editorButton.Location = new System.Drawing.Point(4, 23);
            this.editorButton.Name = "editorButton";
            this.editorButton.Size = new System.Drawing.Size(79, 23);
            this.editorButton.TabIndex = 4;
            this.editorButton.Text = "Editor";
            this.editorButton.UseVisualStyleBackColor = false;
            this.editorButton.Click += new System.EventHandler(this.editorButton_Click);
            this.editorButton.MouseEnter += new System.EventHandler(this.menuButton_MouseEnter);
            this.editorButton.MouseLeave += new System.EventHandler(this.menuButton_MouseLeave);
            // 
            // consoleButton
            // 
            this.consoleButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.consoleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.consoleButton.Font = new System.Drawing.Font("Verdana", 7F);
            this.consoleButton.ForeColor = System.Drawing.Color.Gray;
            this.consoleButton.Location = new System.Drawing.Point(82, 23);
            this.consoleButton.Name = "consoleButton";
            this.consoleButton.Size = new System.Drawing.Size(79, 23);
            this.consoleButton.TabIndex = 5;
            this.consoleButton.Text = "Console";
            this.consoleButton.UseVisualStyleBackColor = false;
            this.consoleButton.Click += new System.EventHandler(this.consoleButton_Click);
            this.consoleButton.MouseEnter += new System.EventHandler(this.menuButton_MouseEnter);
            this.consoleButton.MouseLeave += new System.EventHandler(this.menuButton_MouseLeave);
            // 
            // activePanel
            // 
            this.activePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.activePanel.Location = new System.Drawing.Point(0, 50);
            this.activePanel.Name = "activePanel";
            this.activePanel.Size = new System.Drawing.Size(900, 525);
            this.activePanel.TabIndex = 4;
            // 
            // formTitle
            // 
            this.formTitle.AutoSize = true;
            this.formTitle.Font = new System.Drawing.Font("Verdana", 8F);
            this.formTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.formTitle.Location = new System.Drawing.Point(4, 4);
            this.formTitle.Name = "formTitle";
            this.formTitle.Size = new System.Drawing.Size(106, 13);
            this.formTitle.TabIndex = 3;
            this.formTitle.Text = "// Intent Editor //";
            this.formTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status});
            this.statusStrip.Location = new System.Drawing.Point(0, 578);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(900, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 6;
            this.statusStrip.Text = "statusStrip";
            // 
            // status
            // 
            this.status.BackColor = System.Drawing.Color.Transparent;
            this.status.Font = new System.Drawing.Font("Verdana", 8F);
            this.status.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(151)))), ((int)(((byte)(251)))));
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(51, 17);
            this.status.Text = "           ";
            // 
            // maximizeButton
            // 
            this.maximizeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.maximizeButton.BackColor = System.Drawing.Color.Transparent;
            this.maximizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.maximizeButton.Font = new System.Drawing.Font("Verdana", 7F);
            this.maximizeButton.ForeColor = System.Drawing.Color.Gray;
            this.maximizeButton.Image = global::Intent.Gui.Resources.Window_Maximize;
            this.maximizeButton.Location = new System.Drawing.Point(848, 0);
            this.maximizeButton.Name = "maximizeButton";
            this.maximizeButton.Size = new System.Drawing.Size(23, 23);
            this.maximizeButton.TabIndex = 1;
            this.maximizeButton.UseVisualStyleBackColor = false;
            this.maximizeButton.Click += new System.EventHandler(this.maximizeButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.BackColor = System.Drawing.Color.Transparent;
            this.stopButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.stopButton.Enabled = false;
            this.stopButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stopButton.Font = new System.Drawing.Font("Verdana", 7F);
            this.stopButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.stopButton.Image = global::Intent.Gui.Resources.Controls_Stop;
            this.stopButton.Location = new System.Drawing.Point(335, 21);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(23, 23);
            this.stopButton.TabIndex = 7;
            this.stopButton.UseVisualStyleBackColor = false;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            this.stopButton.MouseEnter += new System.EventHandler(this.menuButton_MouseEnter);
            this.stopButton.MouseLeave += new System.EventHandler(this.menuButton_MouseLeave);
            // 
            // startButton
            // 
            this.startButton.BackColor = System.Drawing.Color.Transparent;
            this.startButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.startButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.startButton.Font = new System.Drawing.Font("Verdana", 7F);
            this.startButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.startButton.Image = global::Intent.Gui.Resources.Controls_Play;
            this.startButton.Location = new System.Drawing.Point(306, 21);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(23, 23);
            this.startButton.TabIndex = 6;
            this.startButton.UseVisualStyleBackColor = false;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            this.startButton.MouseEnter += new System.EventHandler(this.menuButton_MouseEnter);
            this.startButton.MouseLeave += new System.EventHandler(this.menuButton_MouseLeave);
            // 
            // clearConsoleButton
            // 
            this.clearConsoleButton.BackColor = System.Drawing.Color.Transparent;
            this.clearConsoleButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.clearConsoleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearConsoleButton.Font = new System.Drawing.Font("Verdana", 7F);
            this.clearConsoleButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.clearConsoleButton.Image = global::Intent.Gui.Resources.Console_Clear;
            this.clearConsoleButton.Location = new System.Drawing.Point(393, 21);
            this.clearConsoleButton.Name = "clearConsoleButton";
            this.clearConsoleButton.Size = new System.Drawing.Size(23, 23);
            this.clearConsoleButton.TabIndex = 7;
            this.clearConsoleButton.UseVisualStyleBackColor = false;
            this.clearConsoleButton.Click += new System.EventHandler(this.clearConsoleButton_Click);
            this.clearConsoleButton.MouseEnter += new System.EventHandler(this.menuButton_MouseEnter);
            this.clearConsoleButton.MouseLeave += new System.EventHandler(this.menuButton_MouseLeave);
            // 
            // buildScriptsButton
            // 
            this.buildScriptsButton.BackColor = System.Drawing.Color.Transparent;
            this.buildScriptsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buildScriptsButton.Enabled = false;
            this.buildScriptsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buildScriptsButton.Font = new System.Drawing.Font("Verdana", 7F);
            this.buildScriptsButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.buildScriptsButton.Image = global::Intent.Gui.Resources.Build_All;
            this.buildScriptsButton.Location = new System.Drawing.Point(364, 21);
            this.buildScriptsButton.Name = "buildScriptsButton";
            this.buildScriptsButton.Size = new System.Drawing.Size(23, 23);
            this.buildScriptsButton.TabIndex = 7;
            this.buildScriptsButton.UseVisualStyleBackColor = false;
            this.buildScriptsButton.Click += new System.EventHandler(this.buildScriptsButton_Click);
            this.buildScriptsButton.MouseEnter += new System.EventHandler(this.menuButton_MouseEnter);
            this.buildScriptsButton.MouseLeave += new System.EventHandler(this.menuButton_MouseLeave);
            // 
            // newButton
            // 
            this.newButton.BackColor = System.Drawing.Color.Transparent;
            this.newButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.newButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.newButton.Font = new System.Drawing.Font("Verdana", 7F);
            this.newButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.newButton.Image = global::Intent.Gui.Resources.File_New;
            this.newButton.Location = new System.Drawing.Point(167, 23);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(23, 23);
            this.newButton.TabIndex = 6;
            this.newButton.UseVisualStyleBackColor = false;
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            this.newButton.MouseEnter += new System.EventHandler(this.menuButton_MouseEnter);
            this.newButton.MouseLeave += new System.EventHandler(this.menuButton_MouseLeave);
            // 
            // openButton
            // 
            this.openButton.BackColor = System.Drawing.Color.Transparent;
            this.openButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.openButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.openButton.Font = new System.Drawing.Font("Verdana", 7F);
            this.openButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.openButton.Image = global::Intent.Gui.Resources.File_Open;
            this.openButton.Location = new System.Drawing.Point(196, 23);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(23, 23);
            this.openButton.TabIndex = 6;
            this.openButton.UseVisualStyleBackColor = false;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            this.openButton.MouseEnter += new System.EventHandler(this.menuButton_MouseEnter);
            this.openButton.MouseLeave += new System.EventHandler(this.menuButton_MouseLeave);
            // 
            // saveButton
            // 
            this.saveButton.BackColor = System.Drawing.Color.Transparent;
            this.saveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveButton.Font = new System.Drawing.Font("Verdana", 7F);
            this.saveButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.saveButton.Image = global::Intent.Gui.Resources.File_Save;
            this.saveButton.Location = new System.Drawing.Point(225, 23);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(23, 23);
            this.saveButton.TabIndex = 6;
            this.saveButton.UseVisualStyleBackColor = false;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            this.saveButton.MouseEnter += new System.EventHandler(this.menuButton_MouseEnter);
            this.saveButton.MouseLeave += new System.EventHandler(this.menuButton_MouseLeave);
            // 
            // saveAsButton
            // 
            this.saveAsButton.BackColor = System.Drawing.Color.Transparent;
            this.saveAsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.saveAsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveAsButton.Font = new System.Drawing.Font("Verdana", 7F);
            this.saveAsButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.saveAsButton.Image = global::Intent.Gui.Resources.File_SaveAs;
            this.saveAsButton.Location = new System.Drawing.Point(254, 23);
            this.saveAsButton.Name = "saveAsButton";
            this.saveAsButton.Size = new System.Drawing.Size(23, 23);
            this.saveAsButton.TabIndex = 6;
            this.saveAsButton.UseVisualStyleBackColor = false;
            this.saveAsButton.Click += new System.EventHandler(this.saveAsButton_Click);
            this.saveAsButton.MouseEnter += new System.EventHandler(this.menuButton_MouseEnter);
            this.saveAsButton.MouseLeave += new System.EventHandler(this.menuButton_MouseLeave);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "Intent.xml";
            this.openFileDialog.Filter = "XML files|*.xml|All files|*.*";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "XML files|*.xml|All files|*.*";
            // 
            // AppForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.BackgroundImage = global::Intent.Gui.Resources.Crosshatch;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.minimizeButton);
            this.Controls.Add(this.formTitle);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.maximizeButton);
            this.Controls.Add(this.activePanel);
            this.Controls.Add(this.buildScriptsButton);
            this.Controls.Add(this.clearConsoleButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.saveAsButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.openButton);
            this.Controls.Add(this.newButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.consoleButton);
            this.Controls.Add(this.editorButton);
            this.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "AppForm";
            this.Text = "Intent Editor";
            this.Load += new System.EventHandler(this.Form_Load);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form_MouseUp);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button maximizeButton;
        private System.Windows.Forms.Button minimizeButton;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.Button editorButton;
        private System.Windows.Forms.Button consoleButton;
        private System.Windows.Forms.Panel activePanel;
        private System.Windows.Forms.Label formTitle;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button clearConsoleButton;
        private System.Windows.Forms.Button buildScriptsButton;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button saveAsButton;
        private System.Windows.Forms.ToolStripStatusLabel status;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}

