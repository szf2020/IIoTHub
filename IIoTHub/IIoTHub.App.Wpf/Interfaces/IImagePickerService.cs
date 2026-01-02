namespace IIoTHub.App.Wpf.Interfaces
{
    /// <summary>
    /// 提供圖片選擇功能的服務介面
    /// </summary>
    public interface IImagePickerService
    {
        /// <summary>
        /// 顯示圖片選擇對話框，讓使用者選擇圖片檔案
        /// </summary>
        /// <returns></returns>
        Task<string> PickImageAsync();
    }
}
