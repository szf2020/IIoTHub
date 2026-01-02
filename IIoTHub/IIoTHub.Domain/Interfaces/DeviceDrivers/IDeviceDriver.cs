using IIoTHub.Domain.Models;
using IIoTHub.Domain.Models.DeviceSettings;

namespace IIoTHub.Domain.Interfaces.DeviceDrivers
{
    /// <summary>
    /// 驅動器介面
    /// </summary>
    public interface IDeviceDriver
    {
        /// <summary>
        /// 驅動器名稱
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// 取得指定設備的快照
        /// </summary>
        /// <param name="deviceSetting"></param>
        /// <returns></returns>
        DeviceSnapshot GetSnapshot(DeviceSetting deviceSetting);
    }
}
