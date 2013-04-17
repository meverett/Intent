namespace Intent.Gui
{
    partial class MessageAdapterListView
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
            this.topPanel = new System.Windows.Forms.Panel();
            this.addAdapterButton = new System.Windows.Forms.Button();
            this.availableAdapters = new System.Windows.Forms.ComboBox();
            this.adaptersPanel = new Intent.Gui.DoubleBufferedFlowPanel();
            this.topPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // topPanel
            // 
            this.topPanel.Controls.Add(this.addAdapterButton);
            this.topPanel.Controls.Add(this.availableAdapters);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(337, 33);
            this.topPanel.TabIndex = 0;
            // 
            // addAdapterButton
            // 
            this.addAdapterButton.BackColor = System.Drawing.Color.Transparent;
            this.addAdapterButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addAdapterButton.Font = new System.Drawing.Font("Verdana", 7F);
            this.addAdapterButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(28)))));
            this.addAdapterButton.Image = global::Intent.Gui.Resources.AddItem;
            this.addAdapterButton.Location = new System.Drawing.Point(4, 6);
            this.addAdapterButton.Name = "addAdapterButton";
            this.addAdapterButton.Size = new System.Drawing.Size(23, 21);
            this.addAdapterButton.TabIndex = 3;
            this.addAdapterButton.UseVisualStyleBackColor = false;
            this.addAdapterButton.Click += new System.EventHandler(this.addAdapterButton_Click);
            this.addAdapterButton.MouseEnter += new System.EventHandler(this.button_MouseEnter);
            this.addAdapterButton.MouseLeave += new System.EventHandler(this.button_MouseLeave);
            // 
            // availableAdapters
            // 
            this.availableAdapters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.availableAdapters.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.availableAdapters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.availableAdapters.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(151)))), ((int)(((byte)(251)))));
            this.availableAdapters.FormattingEnabled = true;
            this.availableAdapters.Location = new System.Drawing.Point(33, 6);
            this.availableAdapters.Name = "availableAdapters";
            this.availableAdapters.Size = new System.Drawing.Size(303, 21);
            this.availableAdapters.TabIndex = 0;
            // 
            // adaptersPanel
            // 
            this.adaptersPanel.AutoScroll = true;
            this.adaptersPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(28)))));
            this.adaptersPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.adaptersPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.adaptersPanel.Location = new System.Drawing.Point(0, 33);
            this.adaptersPanel.Name = "adaptersPanel";
            this.adaptersPanel.Size = new System.Drawing.Size(337, 430);
            this.adaptersPanel.TabIndex = 1;
            this.adaptersPanel.WrapContents = false;
            // 
            // MessageAdapterListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.adaptersPanel);
            this.Controls.Add(this.topPanel);
            this.Font = new System.Drawing.Font("Verdana", 8F);
            this.ForeColor = System.Drawing.Color.Gray;
            this.Name = "MessageAdapterListView";
            this.Size = new System.Drawing.Size(337, 463);
            this.Load += new System.EventHandler(this.Control_Load);
            this.topPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.ComboBox availableAdapters;
        private System.Windows.Forms.Button addAdapterButton;
        private DoubleBufferedFlowPanel adaptersPanel;
    }
}
