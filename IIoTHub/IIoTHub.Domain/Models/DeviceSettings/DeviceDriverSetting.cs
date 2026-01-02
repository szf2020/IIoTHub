namespace IIoTHub.Domain.Models.DeviceSettings
{
    /// <summary>
    /// 設備驅動器設定
    /// </summary>
    public class DeviceDriverSetting
    {
        public DeviceDriverSetting(string displayName,
                                   List<DeviceConnectionSetting> connectionSettings,
                                   List<DeviceVariableSetting> variableSettings)
        {
            DisplayName = displayName;
            ConnectionSettings = connectionSettings ?? [];
            VariableSettings = variableSettings ?? [];
        }

        /// <summary>
        /// 驅動顯示名稱
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// 設備連線設定清單
        /// </summary>
        public IReadOnlyList<DeviceConnectionSetting> ConnectionSettings { get; }

        /// <summary>
        /// 設備變數設定清單
        /// </summary>
        public IReadOnlyList<DeviceVariableSetting> VariableSettings { get; }
    }
}
