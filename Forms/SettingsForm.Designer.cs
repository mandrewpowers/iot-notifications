namespace IoT_Notifications {
    partial class SettingsForm {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.settingsNavigation = new TreeView();
            this.splitContainer = new SplitContainer();
            this.settingsContainer = new Panel();
            this.labelPlaceholder = new Label();
            this.imageList = new ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)this.splitContainer).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.settingsContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // settingsNavigation
            // 
            this.settingsNavigation.Dock = DockStyle.Fill;
            this.settingsNavigation.FullRowSelect = true;
            this.settingsNavigation.HideSelection = false;
            this.settingsNavigation.Indent = 38;
            this.settingsNavigation.ItemHeight = 36;
            this.settingsNavigation.Location = new Point(0, 0);
            this.settingsNavigation.Name = "settingsNavigation";
            this.settingsNavigation.ShowLines = false;
            this.settingsNavigation.Size = new Size(200, 450);
            this.settingsNavigation.TabIndex = 0;
            this.settingsNavigation.AfterSelect += this.settingsNavigation_AfterSelect;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = DockStyle.Fill;
            this.splitContainer.FixedPanel = FixedPanel.Panel1;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.settingsNavigation);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.settingsContainer);
            this.splitContainer.Size = new Size(804, 450);
            this.splitContainer.SplitterDistance = 200;
            this.splitContainer.TabIndex = 1;
            // 
            // settingsContainer
            // 
            this.settingsContainer.Controls.Add(this.labelPlaceholder);
            this.settingsContainer.Dock = DockStyle.Fill;
            this.settingsContainer.Location = new Point(0, 0);
            this.settingsContainer.Name = "settingsContainer";
            this.settingsContainer.Size = new Size(600, 450);
            this.settingsContainer.TabIndex = 0;
            // 
            // labelPlaceholder
            // 
            this.labelPlaceholder.Dock = DockStyle.Fill;
            this.labelPlaceholder.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            this.labelPlaceholder.Location = new Point(0, 0);
            this.labelPlaceholder.Name = "labelPlaceholder";
            this.labelPlaceholder.Size = new Size(600, 450);
            this.labelPlaceholder.TabIndex = 0;
            this.labelPlaceholder.Text = "Select a section on the left";
            this.labelPlaceholder.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // imageList
            // 
            this.imageList.ColorDepth = ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new Size(16, 16);
            this.imageList.TransparentColor = Color.Transparent;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(804, 450);
            this.Controls.Add(this.splitContainer);
            this.Name = "SettingsForm";
            this.Text = "IoT Notifications - Settings";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.splitContainer).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.settingsContainer.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private TreeView settingsNavigation;
        private SplitContainer splitContainer;
        private ImageList imageList;
        private Panel settingsContainer;
        private Label labelPlaceholder;
    }
}