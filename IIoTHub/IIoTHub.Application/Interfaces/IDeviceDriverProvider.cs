using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Interfaces.DeviceDrivers;

namespace IIoTHub.Application.Interfaces
{
    /// <summary>
    /// 提供設備驅動器的介面
    /// </summary>
    public interface IDeviceDriverProvider
    {
        /// <summary>
        /// 依照設備類別取得對應的驅動器列表
        /// </summary>
        /// <param name="categoryType"></param>
        /// <returns></returns>
        IEnumerable<IDeviceDriver> GetDrivers(DeviceCategoryType categoryType);

        /// <summary>
        /// 取得指定設備類別及名稱的驅動器
        /// </summary>
        /// <param name="categoryType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        IDeviceDriver GetDriver(DeviceCategoryType categoryType, string name);
    }
}
