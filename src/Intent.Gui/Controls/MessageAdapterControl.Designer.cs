namespace Intent.Gui
{
    partial class MessageAdapterControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageAdapterControl));
            this.adapterName = new System.Windows.Forms.Label();
            this.removeButton = new System.Windows.Forms.Button();
            this.outActivity = new System.Windows.Forms.Panel();
            this.inActivity = new System.Windows.Forms.Panel();
            this.errorActivity = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // adapterName
            // 
            this.adapterName.AutoSize = true;
            this.adapterName.BackColor = System.Drawing.Color.Transparent;
            this.adapterName.Font = new System.Drawing.Font("Verdana", 8F);
            this.adapterName.Location = new System.Drawing.Point(30, 10);
            this.adapterName.Name = "adapterName";
            this.adapterName.Size = new System.Drawing.Size(142, 13);
            this.adapterName.TabIndex = 0;
            this.adapterName.Text = "Message Adapter Name";
            // 
            // removeButton
            // 
            this.removeButton.BackColor = System.Drawing.Color.Transparent;
            this.removeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.removeButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.removeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.removeButton.Font = new System.Drawing.Font("Verdana", 7F);
            this.removeButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(28)))));
            this.removeButton.Image = global::Intent.Gui.Resources.DeleteItem;
            this.removeButton.Location = new System.Drawing.Point(3, 6);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(23, 23);
            this.removeButton.TabIndex = 8;
            this.removeButton.UseVisualStyleBackColor = false;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            this.removeButton.MouseEnter += new System.EventHandler(this.removeButton_MouseEnter);
            this.removeButton.MouseLeave += new System.EventHandler(this.removeButton_MouseLeave);
            // 
            // outActivity
            // 
            this.outActivity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outActivity.BackColor = System.Drawing.Color.Transparent;
            this.outActivity.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("outActivity.BackgroundImage")));
            this.outActivity.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.outActivity.Location = new System.Drawing.Point(286, 6);
            this.outActivity.Name = "outActivity";
            this.outActivity.Size = new System.Drawing.Size(23, 23);
            this.outActivity.TabIndex = 1;
            // 
            // inActivity
            // 
            this.inActivity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.inActivity.BackColor = System.Drawing.Color.Transparent;
            this.inActivity.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("inActivity.BackgroundImage")));
            this.inActivity.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.inActivity.Location = new System.Drawing.Point(257, 6);
            this.inActivity.Name = "inActivity";
            this.inActivity.Size = new System.Drawing.Size(23, 23);
            this.inActivity.TabIndex = 1;
            // 
            // errorActivity
            // 
            this.errorActivity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.errorActivity.BackColor = System.Drawing.Color.Transparent;
            this.errorActivity.BackgroundImage = global::Intent.Gui.Resources.ActivityIndicator_NoActivity;
            this.errorActivity.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.errorActivity.Location = new System.Drawing.Point(228, 6);
            this.errorActivity.Name = "errorActivity";
            this.errorActivity.Size = new System.Drawing.Size(23, 23);
            this.errorActivity.TabIndex = 1;
            // 
            // MessageAdapterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(28)))));
            this.BackgroundImage = global::Intent.Gui.Resources.Crosshatch;
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.outActivity);
            this.Controls.Add(this.errorActivity);
            this.Controls.Add(this.inActivity);
            this.Controls.Add(this.adapterName);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Verdana", 8F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(151)))), ((int)(((byte)(251)))));
            this.Name = "MessageAdapterControl";
            this.Size = new System.Drawing.Size(312, 32);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label adapterName;
        private System.Windows.Forms.Panel inActivity;
        private System.Windows.Forms.Panel outActivity;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Panel errorActivity;
    }
}
