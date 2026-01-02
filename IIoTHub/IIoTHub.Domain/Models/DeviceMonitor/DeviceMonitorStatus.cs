namespace IIoTHub.Domain.Models.DeviceMonitor
{
    /// <summary>
    /// 設備監控狀態
    /// </summary>
    public class DeviceMonitorStatus
    {
        public DeviceMonitorStatus(Guid id)
        {
            Id = id;
        }

        /// <summary>
        /// 設備識別碼
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// 是否啟用監控
        /// </summary>
        public bool IsMonitoring { get; set; }
    }
}
