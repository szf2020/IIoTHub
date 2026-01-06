namespace IIoTHub.App.Wpf.Services
{
    /// <summary>
    /// 主視窗導覽服務介面
    /// </summary>
    public interface IMainWindowNavigationService
    {
        /// <summary>
        /// 目前主視窗顯示的內容
        /// </summary>
        object CurrentView { get; }

        /// <summary>
        /// 導覽到 Dashboard 視圖
        /// </summary>
        Task NavigateToDashboardAsync();
    }
}
