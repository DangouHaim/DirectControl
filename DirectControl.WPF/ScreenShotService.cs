using BLL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DirectControl.WPF
{
    class ScreenShotService : IScreenShotService
    {
        public byte[] Capture()
        {
            double screenLeft = SystemParameters.VirtualScreenLeft;
            double screenTop = SystemParameters.VirtualScreenTop;
            double screenWidth = SystemParameters.VirtualScreenWidth;
            double screenHeight = SystemParameters.VirtualScreenHeight;

            using (Bitmap bmp = new Bitmap((int)screenWidth,
                (int)screenHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen((int)screenLeft, (int)screenTop, 0, 0, bmp.Size);
                    using(MemoryStream stream = new MemoryStream())
                    {
                        var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                        var encParams = new EncoderParameters() { Param = new[] { new EncoderParameter(Encoder.Quality, 25L) } };

                        bmp.Save(stream, encoder, encParams);

                        return stream.GetBuffer();
                    }
                }

            }
        }
    }
}
