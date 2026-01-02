using IIoTHub.Application.Interfaces;
using IIoTHub.Domain.Interfaces.Repositories;
using IIoTHub.Domain.Models.DeviceSettings;

namespace IIoTHub.Application.Services
{
    public class DeviceSettingService : IDeviceSettingService
    {
        private readonly IDeviceSettingRepository _deviceSettingRepository;

        public DeviceSettingService(IDeviceSettingRepository deviceSettingRepository)
        {
            _deviceSettingRepository = deviceSettingRepository;
        }

        /// <summary>
        /// 設備設定變更事件
        /// </summary>
        public event EventHandler DeviceSettingChanged;

        /// <summary>
        /// 新增設備設定
        /// </summary>
        /// <param name="deviceSetting"></param>
        public async Task AddAsync(DeviceSetting deviceSetting)
        {
            await _deviceSettingRepository.AddAsync(deviceSetting);
            DeviceSettingChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// 更新設備設定
        /// </summary>
        /// <param name="deviceSetting"></param>
        public async Task UpdateAsync(DeviceSetting deviceSetting)
        {
            await _deviceSettingRepository.UpdateAsync(deviceSetting);
            DeviceSettingChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// 刪除指定的設備設定
        /// </summary>
        /// <param name="id"></param>
        public async Task DeleteAsync(Guid id)
        {
            await _deviceSettingRepository.DeleteAsync(id);
            DeviceSettingChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// 取得所有設備設定
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DeviceSetting>> GetAllAsync()
        {
            return await _deviceSettingRepository.GetAllAsync();
        }

        /// <summary>
        /// 取得指定的設備設定
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DeviceSetting> GetByIdAsync(Guid id)
        {
            return await _deviceSettingRepository.GetByIdAsync(id);
        }
    }
}
