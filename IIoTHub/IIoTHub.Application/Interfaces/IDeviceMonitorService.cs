using IIoTHub.Application.Models;

namespace IIoTHub.Application.Interfaces
{
    /// <summary>
    /// 設備監控服務介面
    /// </summary>
    public interface IDeviceMonitorService
    {
        /// <summary>
        /// 啟動指定設備的監控
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task StartMonitor(Guid id);

        /// <summary>
        /// 停止指定設備的監控
        /// </summary>
        /// <param name="id"></param>
        Task StopMonitor(Guid id);

        /// <summary>
        /// 檢查指定設備是否正在監控中
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> IsMonitoring(Guid id);

        /// <summary>
        /// 訂閱指定設備的快照更新事件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="onSnapshot"></param>
        /// <returns></returns>
        IDisposable Subscribe(Guid id, Action<DeviceSnapshotExtended> onSnapshot);
    }
}
