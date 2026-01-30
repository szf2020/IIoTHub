using IIoTHub.Application.Models;
using IIoTHub.Domain.Enums;

namespace IIoTHub.Application.Interfaces
{
    /// <summary>
    /// 設備運轉統計服務介面
    /// </summary>
    public interface IDeviceRuntimeStatisticsService
    {
        /// <summary>
        /// 通知服務設備的運轉狀態已發生變化。
        /// 此方法由設備監控服務呼叫。
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="runStatus"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        Task OnRuntimeStatusChangedAsync(Guid deviceId, DeviceRunStatus runStatus, DateTime timestamp);

        /// <summary>
        /// 訂閱指定設備的運轉統計摘要變化
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        IDisposable SubscribeRuntimeSummary(Guid deviceId, Action<DeviceRuntimeSummary> handler);
    }
}
