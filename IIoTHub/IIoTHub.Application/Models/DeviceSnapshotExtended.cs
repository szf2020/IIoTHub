using IIoTHub.Domain.Models;

namespace IIoTHub.Application.Models
{
    /// <summary>
    /// 擴充設備快照
    /// </summary>
    public class DeviceSnapshotExtended : DeviceSnapshot
    {
        public DeviceSnapshotExtended(DeviceSnapshot snapshot,
                                      double utilization)
        {
            DeviceId = snapshot.DeviceId;
            RunStatus = snapshot.RunStatus;
            Timestamp = snapshot.Timestamp;
            Utilization = utilization;
        }

        /// <summary>
        /// 設備稼動率
        /// </summary>
        public double Utilization { get; set; }
    }
}
