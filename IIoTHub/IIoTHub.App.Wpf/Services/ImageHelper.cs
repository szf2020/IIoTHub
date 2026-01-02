using System.IO;
using System.Windows.Media.Imaging;

namespace IIoTHub.App.Wpf.Services
{
    public static class ImageHelper
    {
        /// <summary>
        /// 將 BitmapSource 轉換為 Base64 字串
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <returns></returns>
        public static string BitmapSourceToBase64(BitmapSource bitmapSource)
        {
            if (bitmapSource == null)
                return null;

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using var ms = new MemoryStream();
            encoder.Save(ms);
            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// 將 Base64 字串轉換為 BitmapSource
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static BitmapSource Base64ToBitmapSource(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
                return null;

            var bytes = Convert.FromBase64String(base64);

            using var ms = new MemoryStream(bytes);
            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = ms;
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }
    }
}
