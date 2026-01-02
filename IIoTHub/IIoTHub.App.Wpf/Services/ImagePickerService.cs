using IIoTHub.App.Wpf.Interfaces;
using Microsoft.Win32;

namespace IIoTHub.App.Wpf.Services
{
    public class ImagePickerService : IImagePickerService
    {
        /// <summary>
        /// 顯示圖片選擇對話框，讓使用者選擇檔案
        /// </summary>
        /// <returns></returns>
        public Task<string> PickImageAsync()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp",
                Title = "選擇圖片"
            };
            return Task.FromResult(dialog.ShowDialog() == true ? dialog.FileName : null);
        }
    }
}
