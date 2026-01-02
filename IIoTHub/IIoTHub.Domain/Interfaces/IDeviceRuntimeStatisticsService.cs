using IIoTHub.Domain.Models;

namespace IIoTHub.Domain.Interfaces
{
    /// <summary>
    /// 設備運轉統計服務介面
    /// </summary>
    public interface IDeviceRuntimeStatisticsService
    {
        /// <summary>
        /// 處理設備快照事件，依據狀態變化更新設備運轉紀錄
        /// </summary>
        /// <param name="snapshot"></param>
        Task OnSnapshotAsync(DeviceSnapshot snapshot);

        /// <summary>
        /// 取得指定設備在時間區間內的稼動率 (稼動率定義為 Running 狀態時間佔總時間的比例)
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        Task<double> GetUtilizationAsync(Guid deviceId, DateTime? from = null, DateTime? to = null);
    }
}
