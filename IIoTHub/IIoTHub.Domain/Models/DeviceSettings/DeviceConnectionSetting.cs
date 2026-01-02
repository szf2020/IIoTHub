namespace IIoTHub.Domain.Models.DeviceSettings
{
    /// <summary>
    /// 設備連線設定項目
    /// </summary>
    public class DeviceConnectionSetting
    {
        public DeviceConnectionSetting(string key,
                                       string displayName,
                                       string note,
                                       string value)
        {
            Key = key;
            DisplayName = displayName;
            Note = note;
            Value = value;
        }

        /// <summary>
        /// 連線設定鍵值
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
        /// 連線設定值
        /// </summary>
        public string Value { get; set; }
    }
}
