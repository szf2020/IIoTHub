using IIoTHub.Domain.Interfaces.Repositories;
using IIoTHub.Domain.Models.DeviceMonitor;
using Newtonsoft.Json;
using System.IO;

namespace IIoTHub.Infrastructure.Repositories
{
    public class JsonDeviceMonitorStatusRepository : IDeviceMonitorStatusRepository
    {
        private readonly string _filePath = "deviceMonitorStatuses.json";
        private static readonly SemaphoreSlim _fileLock = new(1, 1);

        /// <summary>
        /// 新增一筆設備監控狀態
        /// </summary>
        /// <param name="deviceMonitorStatus"></param>
        /// <returns></returns>
        public async Task AddAsync(DeviceMonitorStatus deviceMonitorStatus)
        {
            await _fileLock.WaitAsync();
            try
            {
                var deviceMonitorStatuses = new List<DeviceMonitorStatus>();
                if (File.Exists(_filePath))
                {
                    var json = await File.ReadAllTextAsync(_filePath);
                    deviceMonitorStatuses = JsonConvert.DeserializeObject<List<DeviceMonitorStatus>>(json) ?? [];
                }

                deviceMonitorStatuses.Add(deviceMonitorStatus);
                var output = JsonConvert.SerializeObject(deviceMonitorStatuses, Formatting.Indented);
                await File.WriteAllTextAsync(_filePath, output);
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// 更新指定的設備監控狀態
        /// </summary>
        /// <param name="deviceMonitorStatus"></param>
        /// <returns></returns>
        public async Task UpdateAsync(DeviceMonitorStatus deviceMonitorStatus)
        {
            await _fileLock.WaitAsync();
            try
            {
                var deviceMonitorStatuses = new List<DeviceMonitorStatus>();
                if (File.Exists(_filePath))
                {
                    var json = await File.ReadAllTextAsync(_filePath);
                    deviceMonitorStatuses = JsonConvert.DeserializeObject<List<DeviceMonitorStatus>>(json) ?? [];
                }

                var updateIndex = deviceMonitorStatuses.FindIndex(e => e.Id == deviceMonitorStatus.Id);
                if (updateIndex >= 0)
                {
                    deviceMonitorStatuses[updateIndex] = deviceMonitorStatus;
                }
                else
                {
                    deviceMonitorStatuses.Add(deviceMonitorStatus);
                }
                var output = JsonConvert.SerializeObject(deviceMonitorStatuses, Formatting.Indented);
                await File.WriteAllTextAsync(_filePath, output);
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// 刪除指定的設備監控狀態
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(Guid id)
        {
            await _fileLock.WaitAsync();
            try
            {
                var deviceMonitorStatuses = new List<DeviceMonitorStatus>();
                if (File.Exists(_filePath))
                {
                    var json = await File.ReadAllTextAsync(_filePath);
                    deviceMonitorStatuses = JsonConvert.DeserializeObject<List<DeviceMonitorStatus>>(json) ?? [];
                }

                deviceMonitorStatuses.RemoveAll(e => e.Id == id);
                var output = JsonConvert.SerializeObject(deviceMonitorStatuses, Formatting.Indented);
                await File.WriteAllTextAsync(_filePath, output);
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// 取得所有的設備監控狀態
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DeviceMonitorStatus>> GetAllAsync()
        {
            await _fileLock.WaitAsync();
            try
            {
                if (!File.Exists(_filePath)) return [];
                var json = await File.ReadAllTextAsync(_filePath);
                return JsonConvert.DeserializeObject<List<DeviceMonitorStatus>>(json) ?? [];
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// 取得指定的設備監控狀態
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DeviceMonitorStatus> GetByIdAsync(Guid id)
        {
            var devices = await GetAllAsync();
            return devices.FirstOrDefault(device => device.Id == id);
        }
    }
}
