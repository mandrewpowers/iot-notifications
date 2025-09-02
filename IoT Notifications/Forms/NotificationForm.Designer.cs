namespace IoT_Notifications
{
    partial class NotificationForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.tableLayoutPanel1 = new TableLayoutPanel();
            this.labelCaption = new Label();
            this.labelUpdatedAt = new Label();
            this.pictureBox = new PictureBox();
            this.progressBar = new SimpleProgressBar();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.pictureBox).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.labelCaption, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelUpdatedAt, 1, 0);
            this.tableLayoutPanel1.Dock = DockStyle.Bottom;
            this.tableLayoutPanel1.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel1.Location = new Point(0, 288);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new Size(512, 39);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // labelCaption
            // 
            this.labelCaption.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            this.labelCaption.AutoSize = true;
            this.labelCaption.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.labelCaption.ForeColor = Color.White;
            this.labelCaption.Location = new Point(3, 0);
            this.labelCaption.Name = "labelCaption";
            this.labelCaption.Padding = new Padding(3, 6, 0, 8);
            this.labelCaption.Size = new Size(158, 39);
            this.labelCaption.TabIndex = 0;
            this.labelCaption.Text = "Motion detected";
            this.labelCaption.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labelUpdatedAt
            // 
            this.labelUpdatedAt.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            this.labelUpdatedAt.AutoSize = true;
            this.labelUpdatedAt.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelUpdatedAt.ForeColor = Color.WhiteSmoke;
            this.labelUpdatedAt.Location = new Point(380, 0);
            this.labelUpdatedAt.Name = "labelUpdatedAt";
            this.labelUpdatedAt.Padding = new Padding(0, 0, 8, 0);
            this.labelUpdatedAt.Size = new Size(129, 39);
            this.labelUpdatedAt.TabIndex = 1;
            this.labelUpdatedAt.Text = "a few seconds ago";
            this.labelUpdatedAt.TextAlign = ContentAlignment.MiddleRight;
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = Color.Transparent;
            this.pictureBox.BackgroundImageLayout = ImageLayout.Stretch;
            this.pictureBox.Dock = DockStyle.Fill;
            this.pictureBox.Location = new Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new Size(512, 288);
            this.pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            // 
            // progressBar
            // 
            this.progressBar.BackColor = Color.DarkGray;
            this.progressBar.BufferedColor = Color.LightGray;
            this.progressBar.Dock = DockStyle.Bottom;
            this.progressBar.ForeColor = Color.White;
            this.progressBar.Location = new Point(0, 284);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new Size(512, 4);
            this.progressBar.TabIndex = 3;
            this.progressBar.Value = 25;
            // 
            // NotificationForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.SlateGray;
            this.ClientSize = new Size(512, 327);
            this.ControlBox = false;
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Name = "NotificationForm";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.FormClosed += this.OnClose;
            this.Shown += this.OnShown;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.pictureBox).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
        private TableLayoutPanel tableLayoutPanel1;
        private Label labelCaption;
        private Label labelUpdatedAt;
        private PictureBox pictureBox;
        private SimpleProgressBar progressBar;
    }
}
