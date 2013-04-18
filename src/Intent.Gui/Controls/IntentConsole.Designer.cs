namespace Intent.Gui
{
    partial class IntentConsole
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
            this.console = new FastColoredTextBoxNS.FastColoredTextBox();
            this.SuspendLayout();
            // 
            // console
            // 
            this.console.AutoScrollMinSize = new System.Drawing.Size(2, 15);
            this.console.BackBrush = null;
            this.console.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(28)))));
            this.console.CaretColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(151)))), ((int)(((byte)(251)))));
            this.console.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.console.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.console.Dock = System.Windows.Forms.DockStyle.Fill;
            this.console.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(151)))), ((int)(((byte)(251)))));
            this.console.IndentBackColor = System.Drawing.Color.Transparent;
            this.console.Location = new System.Drawing.Point(0, 0);
            this.console.Name = "console";
            this.console.Paddings = new System.Windows.Forms.Padding(0);
            this.console.ReadOnly = true;
            this.console.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(255)))), ((int)(((byte)(165)))), ((int)(((byte)(0)))));
            this.console.ServiceLinesColor = System.Drawing.Color.Transparent;
            this.console.ShowLineNumbers = false;
            this.console.Size = new System.Drawing.Size(633, 443);
            this.console.TabIndex = 0;
            // 
            // IntentConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.console);
            this.DoubleBuffered = true;
            this.Name = "IntentConsole";
            this.Size = new System.Drawing.Size(633, 443);
            this.Load += new System.EventHandler(this.Control_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private FastColoredTextBoxNS.FastColoredTextBox console;

    }
}
