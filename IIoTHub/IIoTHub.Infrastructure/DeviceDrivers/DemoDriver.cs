using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Interfaces.DeviceDrivers;
using IIoTHub.Domain.Models;
using IIoTHub.Domain.Models.DeviceSettings;
using IIoTHub.Infrastructure.DeviceDrivers.Attributes;

namespace IIoTHub.Infrastructure.DeviceDrivers
{
    public class DemoDriver : IMachineDriver, IMagazineDriver, IRobotDriver
    {
        public string DisplayName => "DEMO 專用驅動器";

        #region 連線設置

        [ConnectionSetting("設置一", "測試顯示用，無需設置", "-")]
        public string ConnectionSettingTest1 { get; set; }

        [ConnectionSetting("設置二", "測試顯示用，無需設置", "-")]
        public string ConnectionSettingTest2 { get; set; }

        [ConnectionSetting("設置三", "測試顯示用，無需設置", "-")]
        public string ConnectionSettingTest3 { get; set; }

        [ConnectionSetting("設置四", "測試顯示用，無需設置", "-")]
        public string ConnectionSettingTest4 { get; set; }

        [ConnectionSetting("設置五", "測試顯示用，無需設置", "-")]
        public string ConnectionSettingTest5 { get; set; }

        [ConnectionSetting("設置六", "測試顯示用，無需設置", "-")]
        public string ConnectionSettingTest6 { get; set; }

        [ConnectionSetting("設置七", "測試顯示用，無需設置", "-")]
        public string ConnectionSettingTest7 { get; set; }

        [ConnectionSetting("設置八", "測試顯示用，無需設置", "-")]
        public string ConnectionSettingTest8 { get; set; }

        [ConnectionSetting("設置九", "測試顯示用，無需設置", "-")]
        public string ConnectionSettingTest9 { get; set; }

        [ConnectionSetting("設置十", "測試顯示用，無需設置", "-")]
        public string ConnectionSettingTest10 { get; set; }

        #endregion

        #region 變數設置

        [VariableSettingAttribute("快照變更間隔", "單位: 秒", "60")]
        public int SnapshotChangeInterval { get; set; } = 60;

        #endregion

        #region 實作

        private readonly Random _random = new Random();
        private readonly Dictionary<Guid, DeviceRunStatus> _currentStatus = new();
        private readonly Dictionary<Guid, DateTime> _lastUpdate = new();

        /// <summary>
        /// 取得指定設備的快照
        /// </summary>
        /// <param name="deviceSetting"></param>
        /// <returns></returns>
        public DeviceSnapshot GetSnapshot(DeviceSetting deviceSetting)
        {
            var variableSetting =
                deviceSetting.DriverSetting.VariableSettings.FirstOrDefault(v => v.Key == nameof(SnapshotChangeInterval));
            var interval = int.TryParse(variableSetting?.Value, out int parsed) ? parsed : SnapshotChangeInterval;
            var now = DateTime.Now;
            if (!_lastUpdate.ContainsKey(deviceSetting.Id))
            {
                _lastUpdate[deviceSetting.Id] = now;
                _currentStatus[deviceSetting.Id] = RandomStatus();
            }
            else
            {
                if ((now - _lastUpdate[deviceSetting.Id]).TotalSeconds >= interval)
                {
                    _currentStatus[deviceSetting.Id] = RandomStatus();
                    _lastUpdate[deviceSetting.Id] = now;
                }
            }
            var status = _currentStatus[deviceSetting.Id];
            return new DeviceSnapshot
            {
                DeviceId = deviceSetting.Id,
                RunStatus = status
            };
        }

        /// <summary>
        /// 隨機產生一個設備狀態
        /// </summary>
        /// <returns></returns>
        private DeviceRunStatus RandomStatus()
        {
            var values = Enum.GetValues(typeof(DeviceRunStatus));
            return (DeviceRunStatus)values.GetValue(_random.Next(values.Length));
        }

        #endregion
    }
}
