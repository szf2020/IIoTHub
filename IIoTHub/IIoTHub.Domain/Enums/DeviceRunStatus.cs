namespace IIoTHub.Domain.Enums
{
    /// <summary>
    /// 設備運行狀態
    /// </summary>
    public enum DeviceRunStatus
    {
        /// <summary>
        /// 離線狀態 (設備未連線或無法與系統通訊)
        /// </summary>
        Offline = 0,

        /// <summary>
        /// 待機狀態 (設備已連線但尚未執行任何作業)
        /// </summary>
        Standby = 1,

        /// <summary>
        /// 運行中狀態 (設備正在執行生產或作業流程)
        /// </summary>
        Running = 2,

        /// <summary>
        /// 警報狀態 (設備發生異常或錯誤，需要人工介入處理)
        /// </summary>
        Alarm = 3
    }
}
