using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT_Notifications {
    internal static class Helpers {
        public static Bitmap ResizeImage(Image image, int width, int height) {
            Bitmap resized = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(resized)) {
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.SmoothingMode = SmoothingMode.HighQuality;
                gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gfx.CompositingQuality = CompositingQuality.HighQuality;
                gfx.DrawImage(image, 0, 0, width, height);
            }
            return resized;
        }
    }
}
