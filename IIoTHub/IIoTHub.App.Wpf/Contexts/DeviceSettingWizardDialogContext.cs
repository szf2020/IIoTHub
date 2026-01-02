using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Models.DeviceSettings;

namespace IIoTHub.App.Wpf.Contexts
{
    /// <summary>
    /// 用於 Device Setting 精靈對話框的上下文資料
    /// </summary>
    public class DeviceSettingWizardDialogContext
    {
        /// <summary>
        /// 設備唯一識別 ID
        /// </summary>
        public Guid DeviceId { get; private set; }

        /// <summary>
        /// 設備名稱
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 設備圖片的 Base64 編碼字串
        /// </summary>
        public string DeviceImageBase64String { get; set; }

        /// <summary>
        /// 設備類別
        /// </summary>
        public DeviceCategoryType DeviceCategoryType { get; set; }

        /// <summary>
        /// 設備所使用的驅動器名稱
        /// </summary>
        public string DeviceDriver { get; set; }

        /// <summary>
        /// 設備連線設定狀態列表
        /// </summary>
        public List<DeviceConnectionSettingState> ConnectionSettings { get; set; }

        /// <summary>
        /// 設備變數設定狀態列表
        /// </summary>
        public List<DeviceVariableSettingState> VariableSettings { get; set; }

        /// <summary>
        /// 建立一個新的上下文 (新增設備)
        /// </summary>
        public static DeviceSettingWizardDialogContext CreateNew()
        {
            return new DeviceSettingWizardDialogContext
            {
                DeviceId = Guid.NewGuid()
            };
        }

        /// <summary>
        /// 從現有的 DeviceSetting 建立上下文 (編輯設備)
        /// </summary>
        public static DeviceSettingWizardDialogContext FromDeviceSetting(DeviceSetting setting)
        {
            return new DeviceSettingWizardDialogContext
            {
                DeviceId = setting.Id,
                DeviceName = setting.Name,
                DeviceImageBase64String = setting.ImageBase64String,
                DeviceCategoryType = setting.CategoryType,
                DeviceDriver = setting.DriverSetting.DisplayName,
                ConnectionSettings = setting.DriverSetting.ConnectionSettings.Select(connectionSetting => new DeviceConnectionSettingState(connectionSetting)).ToList(),
                VariableSettings = setting.DriverSetting.VariableSettings.Select(variableSetting => new DeviceVariableSettingState(variableSetting)).ToList()
            };
        }

        /// <summary>
        /// 將上下文轉換回 DeviceSetting
        /// </summary>
        /// <returns></returns>
        public DeviceSetting ConvertToDeviceSetting()
            => new DeviceSetting(DeviceId,
                                 DeviceName,
                                 DeviceImageBase64String,
                                 DeviceCategoryType,
                                 new DeviceDriverSetting(
                                     DeviceDriver,
                                     ConnectionSettings.Select(setting => setting.ConvertToDeviceConnectionSetting()).ToList(),
                                     VariableSettings.Select(setting => setting.ConvertToDeviceVariableSetting()).ToList()));
    }

    /// <summary>
    /// DeviceConnectionSetting 在 Wizard 中的狀態封裝 (支援編輯時的暫存與轉換)
    /// </summary>
    public class DeviceConnectionSettingState
    {
        public DeviceConnectionSettingState(DeviceConnectionSetting connectionSetting)
        {
            Key = connectionSetting.Key;
            DisplayName = connectionSetting.DisplayName;
            Note = connectionSetting.Note;
            Value = connectionSetting.Value;
        }

        /// <summary>
        /// 連線設定的唯一 Key
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// 連線設定顯示名稱
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// 連線設定說明
        /// </summary>
        public string Note { get; }

        /// <summary>
        /// 連線設定的值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 轉換回 DeviceConnectionSetting
        /// </summary>
        /// <returns></returns>
        public DeviceConnectionSetting ConvertToDeviceConnectionSetting()
            => new DeviceConnectionSetting(Key, DisplayName, Note, Value);
    }

    /// <summary>
    /// DeviceVariableSetting 在 Wizard 中的狀態封裝 (支援編輯時的暫存與轉換)
    /// </summary>
    public class DeviceVariableSettingState
    {
        public DeviceVariableSettingState(DeviceVariableSetting variableSetting)
        {
            Key = variableSetting.Key;
            DisplayName = variableSetting.DisplayName;
            Note = variableSetting.Note;
            Value = variableSetting.Value;
        }

        /// <summary>
        /// 變數設定的唯一 Key
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// 變數設定顯示名稱
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// 變數設定說明
        /// </summary>
        public string Note { get; }

        /// <summary>
        /// 變數設定的值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 轉換回 DeviceVariableSetting
        /// </summary>
        /// <returns></returns>
        public DeviceVariableSetting ConvertToDeviceVariableSetting()
            => new DeviceVariableSetting(Key, DisplayName, Note, Value);
    }
}
