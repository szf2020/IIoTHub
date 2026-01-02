using IIoTHub.Domain.Interfaces.Repositories;
using IIoTHub.Domain.Models.DeviceSettings;
using Newtonsoft.Json;
using System.IO;

namespace IIoTHub.Infrastructure.Repositories
{
    public class JsonDeviceSettingRepository : IDeviceSettingRepository
    {
        private readonly string _filePath = "deviceSettings.json";
        private static readonly SemaphoreSlim _fileLock = new(1, 1);

        /// <summary>
        /// 新增一筆設備設定
        /// </summary>
        /// <param name="deviceSetting"></param>
        /// <returns></returns>
        public async Task AddAsync(DeviceSetting deviceSetting)
        {
            await _fileLock.WaitAsync();
            try
            {
                var deviceSettings = new List<DeviceSetting>();
                if (File.Exists(_filePath))
                {
                    var json = await File.ReadAllTextAsync(_filePath);
                    deviceSettings = JsonConvert.DeserializeObject<List<DeviceSetting>>(json) ?? [];
                }

                deviceSettings.Add(deviceSetting);
                var output = JsonConvert.SerializeObject(deviceSettings, Formatting.Indented);
                await File.WriteAllTextAsync(_filePath, output);
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// 更新指定的設備設定
        /// </summary>
        /// <param name="deviceSetting"></param>
        /// <returns></returns>
        public async Task UpdateAsync(DeviceSetting deviceSetting)
        {
            await _fileLock.WaitAsync();
            try
            {
                var deviceSettings = new List<DeviceSetting>();
                if (File.Exists(_filePath))
                {
                    var json = await File.ReadAllTextAsync(_filePath);
                    deviceSettings = JsonConvert.DeserializeObject<List<DeviceSetting>>(json) ?? [];
                }

                var updateIndex = deviceSettings.FindIndex(e => e.Id == deviceSetting.Id);
                if (updateIndex >= 0)
                {
                    deviceSettings[updateIndex] = deviceSetting;
                }
                else
                {
                    deviceSettings.Add(deviceSetting);
                }
                var output = JsonConvert.SerializeObject(deviceSettings, Formatting.Indented);
                await File.WriteAllTextAsync(_filePath, output);
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// 刪除指定的設備設定
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(Guid id)
        {
            await _fileLock.WaitAsync();
            try
            {
                var deviceSettings = new List<DeviceSetting>();
                if (File.Exists(_filePath))
                {
                    var json = await File.ReadAllTextAsync(_filePath);
                    deviceSettings = JsonConvert.DeserializeObject<List<DeviceSetting>>(json) ?? [];
                }

                deviceSettings.RemoveAll(e => e.Id == id);
                var output = JsonConvert.SerializeObject(deviceSettings, Formatting.Indented);
                await File.WriteAllTextAsync(_filePath, output);
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// 取得所有的設備設定
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DeviceSetting>> GetAllAsync()
        {
            await _fileLock.WaitAsync();
            try
            {
                if (!File.Exists(_filePath)) return [];
                var json = await File.ReadAllTextAsync(_filePath);
                return JsonConvert.DeserializeObject<List<DeviceSetting>>(json) ?? [];
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// 取得指定的設備設定
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DeviceSetting> GetByIdAsync(Guid id)
        {
            var devices = await GetAllAsync();
            return devices.FirstOrDefault(device => device.Id == id);
        }
    }
}
