using IIoTHub.Application.Interfaces;
using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Interfaces;
using IIoTHub.Domain.Interfaces.Repositories;
using IIoTHub.Domain.Models.DeviceMonitor;

namespace IIoTHub.Application.Services
{
    public class DeviceMonitorService : IDeviceMonitorService
    {
        private readonly IDeviceMonitorStatusRepository _deviceMonitorStatusRepository;
        private readonly IDeviceSettingRepository _deviceSettingRepository;
        private readonly IDeviceDriverProvider _deviceDriverProvider;
        private readonly IDeviceRuntimeStatisticsService _deviceRuntimeStatisticsService;

        private readonly Dictionary<Guid, Timer> _timers = [];
        private readonly Dictionary<Guid, List<Action<Application.Models.DeviceSnapshot>>> _subscribers = [];

        public DeviceMonitorService(IDeviceMonitorStatusRepository deviceMonitorStatusRepository,
                                    IDeviceSettingRepository deviceSettingRepository,
                                    IDeviceDriverProvider deviceDriverProvider,
                                    IDeviceRuntimeStatisticsService deviceRuntimeStatisticsService)
        {
            _deviceMonitorStatusRepository = deviceMonitorStatusRepository;
            _deviceSettingRepository = deviceSettingRepository;
            _deviceDriverProvider = deviceDriverProvider;
            _deviceRuntimeStatisticsService = deviceRuntimeStatisticsService;
        }

        /// <summary>
        /// 啟動指定設備的監控
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task StartMonitor(Guid id)
        {
            // 持久化
            var status = await _deviceMonitorStatusRepository.GetByIdAsync(id) ?? new DeviceMonitorStatus(id);
            status.IsMonitoring = true;
            await _deviceMonitorStatusRepository.UpdateAsync(status);

            // 啟動監控
            if (!_timers.ContainsKey(id))
            {
                _timers[id] = new Timer(async _ =>
                {
                    var deviceSetting = await _deviceSettingRepository.GetByIdAsync(id);
                    var deviceDriver = _deviceDriverProvider.GetDriver(deviceSetting.CategoryType, deviceSetting.DriverSetting.DisplayName);
                    var snapshot = deviceDriver.GetSnapshot(deviceSetting);
                    await _deviceRuntimeStatisticsService.OnSnapshotAsync(snapshot);
                    var utilization = await _deviceRuntimeStatisticsService.GetUtilizationAsync(id);

                    // 通知所有訂閱者
                    foreach (var handler in _subscribers[id])
                    {
                        handler(new Application.Models.DeviceSnapshot(snapshot, utilization));
                    }
                }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            }
        }

        /// <summary>
        /// 停止指定設備的監控
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task StopMonitor(Guid id)
        {
            // 持久化
            var status = await _deviceMonitorStatusRepository.GetByIdAsync(id) ?? new DeviceMonitorStatus(id);
            status.IsMonitoring = false;
            await _deviceMonitorStatusRepository.UpdateAsync(status);

            // 關閉監控
            if (_timers.TryGetValue(id, out var timer))
            {
                timer.Dispose();
                _timers.Remove(id);
            }

            // 通知所有訂閱者
            var offlineSnapshotBase = new Domain.Models.DeviceSnapshot
            {
                DeviceId = id,
                RunStatus = DeviceRunStatus.Offline
            };
            var offlineSnapshot = new Application.Models.DeviceSnapshot(offlineSnapshotBase, 0);
            foreach (var handler in _subscribers[id])
            {
                handler(offlineSnapshot);
            }
        }

        /// <summary>
        /// 檢查指定設備是否正在監控中
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> IsMonitoring(Guid id)
            => (await _deviceMonitorStatusRepository.GetByIdAsync(id))?.IsMonitoring ?? false;

        /// <summary>
        /// 訂閱指定設備的快照更新事件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="onSnapshot"></param>
        /// <returns></returns>
        public IDisposable Subscribe(Guid id, Action<Application.Models.DeviceSnapshot> onSnapshot)
        {
            if (!_subscribers.TryGetValue(id, out var subscribers))
            {
                subscribers = new List<Action<Application.Models.DeviceSnapshot>>();
                _subscribers[id] = subscribers;
            }

            subscribers.Add(onSnapshot);

            return new SubscriptionDisposable(() =>
            {
                subscribers.Remove(onSnapshot);
            });
        }

        /// <summary>
        /// 用於管理訂閱取消的 IDisposable 實作
        /// </summary>
        internal sealed class SubscriptionDisposable : IDisposable
        {
            private Action _unsubscribe;

            public SubscriptionDisposable(Action unsubscribe)
            {
                _unsubscribe = unsubscribe;
            }

            public void Dispose()
            {
                _unsubscribe?.Invoke();
                _unsubscribe = null;
            }
        }
    }
}
