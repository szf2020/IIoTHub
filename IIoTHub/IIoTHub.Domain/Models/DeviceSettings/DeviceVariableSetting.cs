namespace IIoTHub.Domain.Models.DeviceSettings
{
    /// <summary>
    /// 設備變數設定項目
    /// </summary>
    public class DeviceVariableSetting
    {
        public DeviceVariableSetting(string key,
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
        /// 變數設定鍵值
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
        /// 變數設定值
        /// </summary>
        public string Value { get; }
    }
}
