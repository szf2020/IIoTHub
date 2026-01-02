using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Interfaces.DeviceDrivers;
using IIoTHub.Domain.Models;
using IIoTHub.Domain.Models.DeviceSettings;
using IIoTHub.Infrastructure.DeviceDrivers.Attributes;

namespace IIoTHub.Infrastructure.DeviceDrivers
{
    public class FanucFocasLibraryDriver : IMachineDriver
    {
        public string DisplayName => "FANUC FOCAS LIBRARY 驅動器";

        #region 連線設置

        [ConnectionSetting("IP 位址")]
        public string IpAddress { get; set; }

        [ConnectionSetting("通訊埠", "預設: 8193", "8193")]
        public int Port { get; set; } = 8193;

        [ConnectionSetting("超時時間", "單位: 毫秒", "3000")]
        public int Timeout { get; set; } = 3000;

        #endregion

        #region 變數設置

        #endregion

        #region 實作

        /// <summary>
        /// 取得指定設備的快照
        /// </summary>
        /// <param name="deviceSetting"></param>
        /// <returns></returns>
        public DeviceSnapshot GetSnapshot(DeviceSetting deviceSetting)
        {
            return new DeviceSnapshot { DeviceId = deviceSetting.Id, RunStatus = DeviceRunStatus.Running };
        }

        #endregion
    }
}
