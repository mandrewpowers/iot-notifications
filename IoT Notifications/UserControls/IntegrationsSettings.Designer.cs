namespace IoT_Notifications.UserControls {
    partial class IntegrationsSettings {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.groupBox1 = new GroupBox();
            this.cbSilent = new CheckBox();
            this.txtName = new TextBox();
            this.label2 = new Label();
            this.txtGuid = new TextBox();
            this.label1 = new Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.cbSilent);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtGuid);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = DockStyle.Fill;
            this.groupBox1.Location = new Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(631, 121);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // cbSilent
            // 
            this.cbSilent.AutoSize = true;
            this.cbSilent.Location = new Point(54, 80);
            this.cbSilent.Name = "cbSilent";
            this.cbSilent.Size = new Size(55, 19);
            this.cbSilent.TabIndex = 6;
            this.cbSilent.Text = "Silent";
            this.cbSilent.UseVisualStyleBackColor = true;
            // 
            // txtName
            // 
            this.txtName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.txtName.Location = new Point(54, 51);
            this.txtName.Name = "txtName";
            this.txtName.Size = new Size(571, 23);
            this.txtName.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new Point(6, 54);
            this.label2.Name = "label2";
            this.label2.Size = new Size(42, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Name:";
            // 
            // txtGuid
            // 
            this.txtGuid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.txtGuid.Location = new Point(54, 22);
            this.txtGuid.Name = "txtGuid";
            this.txtGuid.ReadOnly = true;
            this.txtGuid.Size = new Size(571, 23);
            this.txtGuid.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new Size(37, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "GUID:";
            // 
            // IntegrationsSettings
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.groupBox1);
            this.Name = "IntegrationsSettings";
            this.Size = new Size(631, 121);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private TextBox txtName;
        private Label label2;
        private TextBox txtGuid;
        private Label label1;
        private CheckBox cbSilent;
    }
}
