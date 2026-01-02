using IIoTHub.Domain.Enums;

namespace IIoTHub.Domain.Models
{
    /// <summary>
    /// 設備狀態快照
    /// </summary>
    public class DeviceSnapshot
    {
        /// <summary>
        /// 設備識別碼
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 設備當下的運行狀態
        /// </summary>
        public DeviceRunStatus RunStatus { get; set; }

        /// <summary>
        /// 快照產生時間
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
