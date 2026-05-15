using System.Drawing;
using System.Drawing.Imaging;

namespace highspeed.framework
{
    /// <summary>
    /// Bitmap和Base64互转扩展方法类
    /// </summary>
    public static class BinBitmapExtension
    {
        /// <summary>
        /// Base64String转Bitmap
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static Bitmap? FromBase64(this string base64String)
        {
            Bitmap bmpReturn = null;
            try
            {
                byte[] arr = Convert.FromBase64String(base64String);
                using (MemoryStream ms = new MemoryStream(arr))
                {
                    ms.Position = 0;
                    bmpReturn = new Bitmap(ms);
                }
                arr = null;
            }
            catch { }
            return bmpReturn;
        }

        /// <summary>
        /// Bitmap转Base64String
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static string? ToBase64(this Bitmap bitmap, ImageFormat format = null)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                using (Bitmap img = new Bitmap(bitmap))
                {
                    if (format == null) format = bitmap.RawFormat;
                    ms.Position = 0;
                    img.Save(ms,format);
                    byte[] arr = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(arr, 0, (int)ms.Length);
                    String strbaser64 = Convert.ToBase64String(arr);
                    arr = null;
                    return strbaser64;
                }
            }
            catch
            {
                return null;
            }
        }

        public static byte[] ToByteArray(this Bitmap bitmap)
        {
            return bitmap.ToByteArray(ImageFormat.Bmp);
        }

        public static byte[] ToByteArray(this Bitmap bitmap, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, format ?? bitmap.RawFormat);
                byte[] bytes = ms.GetBuffer();
                return bytes;
            }
        }

        public static Bitmap ToBitmap(this byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                var bitmap = (Bitmap)Image.FromStream(ms);
                return bitmap;
            }
        }
    }
}
