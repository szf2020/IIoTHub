using IIoTHub.Domain.Enums;

namespace IIoTHub.Domain.Models
{
    /// <summary>
    /// 設備運行狀態歷史紀錄
    /// </summary>
    public class DeviceRuntimeRecord
    {
        public DeviceRuntimeRecord(Guid deviceId,
                                   DeviceRunStatus runStatus,
                                   DateTime startTime)
        {
            DeviceId = deviceId;
            RunStatus = runStatus;
            StartTime = startTime;
        }

        /// <summary>
        /// 設備識別碼
        /// </summary>
        public Guid DeviceId { get; }

        /// <summary>
        /// 設備運行狀態
        /// </summary>
        public DeviceRunStatus RunStatus { get; }

        /// <summary>
        /// 狀態開始時間
        /// </summary>
        public DateTime StartTime { get; }

        /// <summary>
        /// 狀態結束時間
        /// </summary>
        public DateTime? EndTime { get; private set; }

        /// <summary>
        /// 結束目前的運行狀態紀錄 (通常在設備狀態發生變更時呼叫)
        /// </summary>
        /// <param name="endTime"></param>
        public void Close(DateTime endTime)
        {
            EndTime = endTime;
        }
    }
}
