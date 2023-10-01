using System.Drawing.Imaging;

namespace VCE.Utility
{
    public class ImageCompression
    {
        /// <summary>
        /// Encode image
        /// </summary>
        /// <returns></returns>
        public static Test CompressedImage()
        {
            //Image compression code
            //ImageCodecInfo codec = GetEncoder(ImageFormat.Jpeg);
            return "";
        }
        /// <summary>
        /// get image ecoded format
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)                
                    return codec;               
            }
            return null;
        }
    }
}
