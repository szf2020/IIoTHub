using IIoTHub.Domain.Models.DeviceSettings;

namespace IIoTHub.App.Wpf.Interfaces
{
    /// <summary>
    /// 提供顯示各種對話框的服務介面
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// 顯示設備設定精靈對話框
        /// </summary>
        /// <param name="deviceSetting"></param>
        void ShowDeviceSettingWizardDialog(DeviceSetting deviceSetting = null);

        /// <summary>
        /// 顯示確認對話框，回傳使用者是否點擊「是」
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        bool ShowConfirmationDialog(string message, string title = "確認");
    }
}
