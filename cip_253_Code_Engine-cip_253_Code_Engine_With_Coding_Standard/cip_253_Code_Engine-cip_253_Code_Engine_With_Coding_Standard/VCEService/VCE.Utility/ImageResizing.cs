using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SD = System.Drawing;
using System.Drawing.Drawing2D;

namespace VCE.Utility
{
    public static class ImageResizing
    {
        /// <summary>
        /// Scales an image proportionally.  Returns a bitmap.
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        static public MemoryStream ScaleImage(Stream inputStream, int maxWidth, int maxHeight)
        {
            MemoryStream ms = null;
            // Retrieve the uploaded image
            Image image = System.Drawing.Image.FromStream(inputStream);
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
            // Create an EncoderParameters object. 
            // An EncoderParameters object has an array of EncoderParameter 
            // objects. In this case, there is only one 
            // EncoderParameter object in the array.
            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 75L);
            ImageCodecInfo codec = ImageCompression.CompressedImage();
            ms = new MemoryStream();
            //now verify that the encoder returned from GetEncoderInfo isnt a null value
            if (codec != null)
                newImage.Save(ms, codec, parameters);
            else
                newImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            // Store the thumbnail in the session (Note: this is bad, it will take a lot of memory, but this is just a demo)           

            return ms;
        }
        /// <summary>
        /// For crop image 
        /// </summary>
        /// <param name="Img"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        static public byte[] Crop(string Img, int Width, int Height, int X, int Y)
        {
            try
            {
                using (SD.Image OriginalImage = SD.Image.FromFile(Img))
                {
                    using (SD.Bitmap bmp = new SD.Bitmap(Width, Height))
                    {
                        bmp.SetResolution(OriginalImage.HorizontalResolution, OriginalImage.VerticalResolution);
                        using (SD.Graphics Graphic = SD.Graphics.FromImage(bmp))
                        {
                            Graphic.SmoothingMode = SmoothingMode.AntiAlias;
                            Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            Graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            Graphic.DrawImage(OriginalImage, new SD.Rectangle(0, 0, Width, Height), X, Y, Width, Height, SD.GraphicsUnit.Pixel);
                            MemoryStream ms = new MemoryStream();
                            bmp.Save(ms, OriginalImage.RawFormat);
                            return ms.GetBuffer();
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                throw (Ex);
            }
        }
    }
}
