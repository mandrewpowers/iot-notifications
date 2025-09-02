using System.ComponentModel;

namespace IoT_Notifications {
    [ToolboxItem(true)]
    public partial class SimpleProgressBar : UserControl {
        //static readonly int ANIM_INTERVAL_MS = 20;

        int ExpectedBarWidth = 0;
        int AnimatedBarWidth = 0;

        System.Windows.Forms.Timer AnimationTimer = new System.Windows.Forms.Timer() {
            Interval = 10,
            Enabled = false
        };

        public Color BufferedColor { get; set; }

        [DefaultValue(0)]
        public int Value {
            get;
            set {
                field = value;
                this.Invalidate();
            }
        } = 0;

        [DefaultValue(0)]
        public int Min {
            get;
            set {
                field = value;
                this.Invalidate();
            }
        } = 0;

        [DefaultValue(100)]
        public int Max {
            get;
            set {
                field = value;
                this.Invalidate();
            }
        } = 100;

        public SimpleProgressBar() {
            this.SetStyle(
                ControlStyles.UserPaint |
                //ControlStyles.AllPaintingInWmPaint |
                //ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor
            , true);

            this.ForeColor = Color.White;
            this.BackColor = Color.DarkGray;
            this.BufferedColor = Color.LightGray;
            this.DoubleBuffered = true;

            InitializeComponent();

            this.ExpectedBarWidth = 0;
            this.AnimatedBarWidth = 0;
            this.AnimationTimer.Enabled = true;
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            var barScale = (float)(this.Value - this.Min) / (this.Max - this.Min);
            var expectedBarWidth = this.ExpectedBarWidth = (int)MathF.Round(this.ClientRectangle.Width * barScale);
            var deltaBarWidth = expectedBarWidth - this.AnimatedBarWidth;

            var animatedBarWidth = expectedBarWidth;

            //var animatedBarWidth = this.AnimatedBarWidth;
            //animatedBarWidth = this.AnimatedBarWidth = Math.Min(expectedBarWidth, this.AnimatedBarWidth + (deltaBarWidth / 4));

            //if (deltaBarWidth > 0) {
            //    animatedBarWidth = this.AnimatedBarWidth = Math.Min(expectedBarWidth, this.AnimatedBarWidth + Math.Max(1, deltaBarWidth / 4));
            //} else if (deltaBarWidth < 0) {
            //    animatedBarWidth = this.AnimatedBarWidth = Math.Max(expectedBarWidth, this.AnimatedBarWidth + Math.Min(-1, deltaBarWidth / 4));
            //}

            //Trace.WriteLine($"exp={expectedBarWidth} anim={animatedBarWidth} (delta={deltaBarWidth})");

            using (SolidBrush foreBrush = new SolidBrush(this.ForeColor), backBrush = new SolidBrush(this.BackColor)) {
                e.Graphics.FillRectangle(backBrush, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height);
                e.Graphics.FillRectangle(foreBrush, 0, 0, animatedBarWidth, this.ClientRectangle.Height);
            }

            //using (var brush = new SolidBrush(this.BufferedColor)) {
            //    e.Graphics.FillRectangle(brush, animatedBarWidth, 0, expectedBarWidth - animatedBarWidth, this.ClientRectangle.Height);
            //}
        }
        private void OnAnimationTick(object? sender, EventArgs e) {
            /*if (this.AnimatedBarWidth == this.ExpectedBarWidth) {
                this.AnimationTimer.Enabled = false;
                return;
            }*/
            this.Invalidate();
        }
    }
}
