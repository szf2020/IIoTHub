using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Models.DeviceSettings;

namespace IIoTHub.Application.Interfaces
{
    /// <summary>
    /// 提供設備驅動器資料的介面
    /// </summary>
    public interface IDeviceDriverMetadataProvider
    {
        /// <summary>
        /// 取得指定設備類別的驅動器資料列表
        /// </summary>
        /// <param name="categoryType"></param>
        /// <returns></returns>
        IEnumerable<DeviceDriverSetting> GetDriverMetadata(DeviceCategoryType categoryType);
    }
}
