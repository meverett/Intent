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
            this.console = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // console
            // 
            this.console.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(28)))));
            this.console.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.console.Dock = System.Windows.Forms.DockStyle.Fill;
            this.console.Font = new System.Drawing.Font("Verdana", 8F);
            this.console.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(151)))), ((int)(((byte)(251)))));
            this.console.Location = new System.Drawing.Point(0, 0);
            this.console.Name = "console";
            this.console.ReadOnly = true;
            this.console.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.console.Size = new System.Drawing.Size(633, 443);
            this.console.TabIndex = 0;
            this.console.Text = "";
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

        private System.Windows.Forms.RichTextBox console;
    }
}
