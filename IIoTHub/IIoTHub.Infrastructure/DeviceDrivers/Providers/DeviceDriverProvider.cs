using IIoTHub.Application.Interfaces;
using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Interfaces.DeviceDrivers;

namespace IIoTHub.Infrastructure.DeviceDrivers.Providers
{
    public class DeviceDriverProvider : IDeviceDriverProvider
    {
        private readonly IEnumerable<IMachineDriver> _machineDrivers;
        private readonly IEnumerable<IMagazineDriver> _magazineDrivers;
        private readonly IEnumerable<IRobotDriver> _robotDrivers;

        public DeviceDriverProvider(IEnumerable<IMachineDriver> machineDrivers,
                                    IEnumerable<IMagazineDriver> magazineDrivers,
                                    IEnumerable<IRobotDriver> robotDrivers)
        {
            _machineDrivers = machineDrivers;
            _magazineDrivers = magazineDrivers;
            _robotDrivers = robotDrivers;
        }

        /// <summary>
        /// 依照設備類別取得對應的驅動器列表
        /// </summary>
        /// <param name="categoryType"></param>
        /// <returns></returns>
        public IEnumerable<IDeviceDriver> GetDrivers(DeviceCategoryType categoryType) => categoryType switch
        {
            DeviceCategoryType.Machine => _machineDrivers,
            DeviceCategoryType.Magazine => _magazineDrivers,
            DeviceCategoryType.Robot => _robotDrivers,
            _ => []
        };

        /// <summary>
        /// 取得指定設備類別及名稱的驅動器
        /// </summary>
        /// <param name="categoryType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IDeviceDriver GetDriver(DeviceCategoryType categoryType, string name) =>
            GetDrivers(categoryType).FirstOrDefault(driver => driver.DisplayName == name);
    }
}
