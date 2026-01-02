using IIoTHub.Application.Interfaces;
using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Interfaces.DeviceDrivers;
using IIoTHub.Domain.Models.DeviceSettings;
using IIoTHub.Infrastructure.DeviceDrivers.Attributes;
using System.Reflection;

namespace IIoTHub.Infrastructure.DeviceDrivers.Providers
{
    public class DeviceDriverMetadataProvider : IDeviceDriverMetadataProvider
    {
        private readonly IDeviceDriverProvider _deviceDriverProvider;

        public DeviceDriverMetadataProvider(IDeviceDriverProvider deviceDriverProvider)
        {
            _deviceDriverProvider = deviceDriverProvider;
        }

        /// <summary>
        /// 取得指定設備類別的驅動器資料列表
        /// </summary>
        /// <param name="categoryType"></param>
        /// <returns></returns>
        public IEnumerable<DeviceDriverSetting> GetDriverMetadata(DeviceCategoryType categoryType)
            => GetDriverMetadata(_deviceDriverProvider.GetDrivers(categoryType));

        /// <summary>
        /// 解析驅動器清單，取得其連線設定與變數設定的資料
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<DeviceDriverSetting> GetDriverMetadata(IEnumerable<IDeviceDriver> drivers)
            => drivers
                .Select(driver => new DeviceDriverSetting(
                    driver.DisplayName,
                    GetSettings<ConnectionSettingAttribute>(driver)
                        .Select(settings => new DeviceConnectionSetting(settings.Property.Name, settings.Attribute.DisplayName, settings.Attribute.Note, settings.Attribute.DefaultValue))
                        .ToList(),
                    GetSettings<VariableSettingAttribute>(driver)
                        .Select(settings => new DeviceVariableSetting(settings.Property.Name, settings.Attribute.DisplayName, settings.Attribute.Note, settings.Attribute.DefaultValue))
                        .ToList()))
                .ToList();

        /// <summary>
        /// 取得驅動器上標記特定 Attribute 的屬性與對應 Attribute
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="driver"></param>
        /// <returns></returns>
        private static IEnumerable<(PropertyInfo Property, TAttribute Attribute)> GetSettings<TAttribute>(IDeviceDriver driver) where TAttribute : Attribute
            => driver.GetType()
                     .GetProperties()
                     .Select(p => (Property: p, Attribute: p.GetCustomAttribute<TAttribute>()))
                     .Where(t => t.Attribute != null);
    }
}
