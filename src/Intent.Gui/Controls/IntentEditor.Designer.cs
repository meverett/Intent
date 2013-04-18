namespace Intent.Gui
{
    partial class IntentEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.leftPanel = new System.Windows.Forms.Panel();
            this.textEditor = new FastColoredTextBoxNS.FastColoredTextBox();
            this.listOfAdapters = new Intent.Gui.MessageAdapterListView();
            this.leftPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // leftPanel
            // 
            this.leftPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.leftPanel.Controls.Add(this.listOfAdapters);
            this.leftPanel.Location = new System.Drawing.Point(0, 0);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(300, 476);
            this.leftPanel.TabIndex = 1;
            // 
            // textEditor
            // 
            this.textEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textEditor.AutoScrollMinSize = new System.Drawing.Size(27, 15);
            this.textEditor.BackBrush = null;
            this.textEditor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(28)))));
            this.textEditor.CaretColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textEditor.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textEditor.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.textEditor.FoldingIndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(196)))));
            this.textEditor.Font = new System.Drawing.Font("Courier New", 10.5F);
            this.textEditor.ForeColor = System.Drawing.Color.Silver;
            this.textEditor.IndentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(37)))));
            this.textEditor.LineNumberColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(151)))), ((int)(((byte)(251)))));
            this.textEditor.Location = new System.Drawing.Point(306, 3);
            this.textEditor.Name = "textEditor";
            this.textEditor.Paddings = new System.Windows.Forms.Padding(0);
            this.textEditor.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(255)))), ((int)(((byte)(165)))), ((int)(((byte)(0)))));
            this.textEditor.ServiceLinesColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.textEditor.Size = new System.Drawing.Size(565, 470);
            this.textEditor.TabIndex = 9;
            this.textEditor.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.textEditor_TextChanged);
            // 
            // listOfAdapters
            // 
            this.listOfAdapters.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.listOfAdapters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listOfAdapters.Font = new System.Drawing.Font("Verdana", 8F);
            this.listOfAdapters.ForeColor = System.Drawing.Color.Gray;
            this.listOfAdapters.Location = new System.Drawing.Point(0, 0);
            this.listOfAdapters.Name = "listOfAdapters";
            this.listOfAdapters.Size = new System.Drawing.Size(300, 476);
            this.listOfAdapters.TabIndex = 0;
            // 
            // IntentEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.textEditor);
            this.Controls.Add(this.leftPanel);
            this.Font = new System.Drawing.Font("Verdana", 8F);
            this.ForeColor = System.Drawing.Color.Gray;
            this.Name = "IntentEditor";
            this.Size = new System.Drawing.Size(874, 476);
            this.Load += new System.EventHandler(this.Editor_Load);
            this.leftPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel leftPanel;
        private MessageAdapterListView listOfAdapters;
        private FastColoredTextBoxNS.FastColoredTextBox textEditor;
    }
}
