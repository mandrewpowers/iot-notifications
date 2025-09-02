using Windows.UI.ViewManagement;


namespace IoT_Notifications {
    public partial class NotificationForm : Form {
        static readonly int EXPIRATION_TIME_MS = 10_000;

        static readonly int FADE_INTERVAL_MS = 20;
        static readonly int FADE_IN_TIME_MS = 80;
        static readonly int FADE_OUT_TIME_MS = 200;

        static readonly double FADE_OPACITY_MAX = 0.98;

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams {
            get {
                var createParams = base.CreateParams;
                createParams.ExStyle |= 0x00000008; // WS_EX_TOPMOST
                return createParams;
            }
        }

        UISettings UISettings { get; init; }
        Color AccentColor { get; init; }
        public bool Silent { get; set; }

        Queue<(Image image, TimeSpan lifetime)> ImageQueue { get; init; }
        int ImagesShown { get; set; }
        int ImagesQueued { get; set; }
        DateTime ImageExpiration { get; set; }
        DateTime ShownAt { get; set; }
        public bool Expired { get; private set; }

        System.Windows.Forms.Timer ExpirationTimer = new System.Windows.Forms.Timer() {
            Interval = EXPIRATION_TIME_MS,
            Enabled = false,
        };

        System.Windows.Forms.Timer FadeTimer = new System.Windows.Forms.Timer() {
            Interval = FADE_INTERVAL_MS,
            Enabled = false,
        };

        System.Windows.Forms.Timer ContentUpdateTimer = new System.Windows.Forms.Timer() {
            Interval = 100,
            Enabled = true,
        };

        public NotificationForm() {
            InitializeComponent();

            this.UISettings = new UISettings();

            var accentColor = this.UISettings.GetColorValue(UIColorType.Accent);
            this.AccentColor = Color.FromArgb(accentColor.R, accentColor.G, accentColor.B);
            this.Silent = false;

            this.BackColor = this.AccentColor;
            this.Opacity = 0.0;

            this.ImageQueue = new Queue<(Image image, TimeSpan lifetime)>();
            this.ImagesShown = 0;
            this.ImagesQueued = 0;
            this.ImageExpiration = DateTime.MinValue;
            this.ShownAt = DateTime.MinValue;
            this.Expired = false;

            this.ExpirationTimer.Tick += this.OnExpirationTick;
            this.FadeTimer.Tick += this.OnFadeTick;
            this.ContentUpdateTimer.Tick += this.OnContentUpdateTick;
        }

        public new void Show() {
            this.Show(null);
        }

        public void Show(Image? image, TimeSpan? imageLifetime) {
            // TODO: Style for notification without image
            this.ReplaceImage(image, imageLifetime);

            this.Visible = true;
        }

        private void OnShown(object sender, EventArgs e) {
            if (!this.Silent) {
                System.Media.SystemSounds.Asterisk.Play();
            }

            Screen? screen = Screen.AllScreens[1];
            if (screen != null) {
                this.Location = new Point(
                     screen.WorkingArea.Location.X + screen.WorkingArea.Size.Width - this.Size.Width - 200,
                     screen.WorkingArea.Location.Y + 10
                );
            }

            this.Opacity = 0.0;
            this.ShownAt = DateTime.Now;
            this.Expired = false;

            this.UpdateDetails();

            this.FadeTimer.Start();
        }

        public void Touch() {
            this.ShownAt = DateTime.Now;

            this.UpdateDetails();

            this.ExpirationTimer.Stop();
            this.ExpirationTimer.Start();
        }

        /// <summary>
        /// Replaces current hero image and clears image queue.
        /// Also: Resets shown timestamp, updates details, and restarts expiration timer.
        /// Note: Image is treated as first in queue optionally with specified lifetime.
        /// </summary>
        /// <param name="image">Image to show</param>
        public void ReplaceImage(Image? image, TimeSpan? imageLifetime) {
            this.pictureBox.Image = image;
            this.pictureBox.BackgroundImage = image;

            this.ImageQueue.Clear();
            this.ImagesQueued = 1;
            this.ImagesShown = 1;

            this.ImageExpiration = DateTime.Now;
            if (imageLifetime.HasValue) {
                this.ImageExpiration += imageLifetime.Value;
            }

            this.Touch();
        }

        /// <summary>
        /// Queues future hero image.
        /// Also: Updates details.
        /// </summary>
        /// <param name="image">Image to show</param>
        /// <param name="imageLifetime">Image lifetime once shown</param>
        public void QueueImage(Image image, TimeSpan imageLifetime) {
            this.ImageQueue.Enqueue((image, imageLifetime));
            this.ImagesQueued++;

            this.Touch();
        }

        private void UpdateImage() {
            var now = DateTime.Now;
            if (this.ImageQueue.Count == 0 || this.ImageExpiration > now) {
                return;
            }
            
            var entry = this.ImageQueue.Dequeue();

            this.pictureBox.Image = entry.image;
            this.pictureBox.BackgroundImage = entry.image;

            this.ImagesShown++;
            this.ImageExpiration = now + entry.lifetime;
        }

        private void UpdateDetails() {
            // Last updated timestamp
            var deltaTime = DateTime.Now.Subtract(this.ShownAt);
            if (deltaTime.TotalSeconds < 1) {
                this.labelUpdatedAt.Text = "now";
            } else if (deltaTime.TotalSeconds < 3) {
                this.labelUpdatedAt.Text = "few seconds ago";
            } else if (deltaTime.TotalSeconds < 60) {
                this.labelUpdatedAt.Text = $"{deltaTime.TotalSeconds:F0} seconds ago";
            } else {
                this.labelUpdatedAt.Text = $"{deltaTime.ToString(@"mm\:ss")} ago";
            }

            // Image queue progress
            this.progressBar.Max = this.ImagesQueued;
            this.progressBar.Value = this.ImagesShown;
            this.progressBar.Visible = this.ImagesQueued > 1;
        }

        private void OnClose(object sender, FormClosedEventArgs e) {
            this.ImageQueue.Clear();
            this.ExpirationTimer.Enabled = false;
            this.FadeTimer.Enabled = false;
            this.ContentUpdateTimer.Enabled = false;
            this.Dispose();
        }

        private void OnExpirationTick(object? sender, EventArgs e) {
            this.Expired = true;
            this.ExpirationTimer.Stop();
            this.FadeTimer.Start();
        }

        private void OnFadeTick(object? sender, EventArgs e) {
            if (this.Expired) {
                // Fade out and close

                var opacityDelta = FADE_OPACITY_MAX / (FADE_OUT_TIME_MS / this.FadeTimer.Interval);

                this.Opacity -= opacityDelta;
                if (this.Opacity <= 0.0) {
                    this.Opacity = 0.0;
                    this.FadeTimer.Stop();
                    this.Close();
                }
            } else {
                // Fade in and start expiration timer

                var opacityDelta = FADE_OPACITY_MAX / (FADE_IN_TIME_MS / this.FadeTimer.Interval);

                this.Opacity += opacityDelta;
                if (this.Opacity >= FADE_OPACITY_MAX) {
                    this.Opacity = FADE_OPACITY_MAX;
                    this.FadeTimer.Stop();
                    this.ExpirationTimer.Start();
                }
            }
        }

        private void OnContentUpdateTick(object? sender, EventArgs e) {
            if (!this.Expired) {
                this.UpdateImage();
                this.UpdateDetails();
            }
        }
    }
}
