using IIoTHub.Application.Interfaces;
using IIoTHub.Application.Models;
using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Interfaces.Repositories;
using IIoTHub.Domain.Models.DeviceRecords;
using System.Collections.Concurrent;

namespace IIoTHub.Application.Services
{
    public class DeviceRuntimeStatisticsService : IDeviceRuntimeStatisticsService
    {
        private readonly IDeviceRuntimeRepository _deviceRuntimeRepository;
        private readonly ConcurrentDictionary<Guid, Subscription> _subscriptions = new();

        public DeviceRuntimeStatisticsService(IDeviceRuntimeRepository deviceRuntimeRepository)
        {
            _deviceRuntimeRepository = deviceRuntimeRepository;
        }

        /// <summary>
        /// 通知服務設備的運轉狀態已發生變化。
        /// 此方法由設備監控服務呼叫。
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="runStatus"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public async Task OnRuntimeStatusChangedAsync(Guid deviceId, DeviceRunStatus runStatus, DateTime timestamp)
        {
            // 更新運轉狀態
            var last = await _deviceRuntimeRepository.GetLatestRecordAsync(deviceId);
            if (last == null)
            {
                // 建立第一筆紀錄
                var newRecord = new DeviceRuntimeRecord(deviceId, runStatus, timestamp, timestamp);
                await _deviceRuntimeRepository.AddAsync(newRecord);
            }
            else if (last.RunStatus != runStatus)
            {
                // 狀態變了，先更新上一筆的結束時間，然後再建立新紀錄
                var lastRecord = new DeviceRuntimeRecord(deviceId, last.RunStatus, last.StartTime, timestamp);
                await _deviceRuntimeRepository.UpdateAsync(lastRecord);
                var newRecord = new DeviceRuntimeRecord(deviceId, runStatus, timestamp, timestamp);
                await _deviceRuntimeRepository.AddAsync(newRecord);
            }
            else
            {
                // 狀態沒變 → 延長上一筆的結束時間
                var lastRecord = new DeviceRuntimeRecord(deviceId, last.RunStatus, last.StartTime, timestamp);
                await _deviceRuntimeRepository.UpdateAsync(lastRecord);
            }

            // 發布運轉統計摘要變化
            if (!_subscriptions.TryGetValue(deviceId, out var subscription))
                return;
            var runtimeSummary = await GetDeviceRuntimeSummaryAsync(deviceId, to: timestamp);
            foreach (var handler in subscription.Handlers.ToList())
            {
                handler(runtimeSummary);
            }
        }

        /// <summary>
        /// 獲取設備運轉統計摘要
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private async Task<DeviceRuntimeSummary> GetDeviceRuntimeSummaryAsync(Guid deviceId, DateTime? from = null, DateTime? to = null)
        {
            var now = DateTime.Now;
            from ??= DateTime.Today;
            to = (to == null || to > now) ? now : to;
            if (from >= to)
            {
                return new DeviceRuntimeSummary();
            }

            // 取得並排序原始紀錄
            var runtimeRecords = (await _deviceRuntimeRepository.GetRecordsAsync(deviceId, from.Value, to.Value))
                .OrderBy(record => record.StartTime)
                .ToList();
            if (runtimeRecords.Count == 0)
            {
                return new DeviceRuntimeSummary();
            }

            // 正規化時間軸（補 Offline）
            var normalizedRuntimeRecords = new List<DeviceRuntimeRecord>();
            var cursor = from.Value;
            foreach (var runtimeRecord in runtimeRecords)
            {
                // 前一段與此段之間有間隔 → 補 Offline
                if (runtimeRecord.StartTime > cursor)
                {
                    normalizedRuntimeRecords.Add(new DeviceRuntimeRecord(
                        deviceId,
                        DeviceRunStatus.Offline,
                        cursor,
                        runtimeRecord.StartTime));

                    cursor = runtimeRecord.StartTime;
                }

                // 插入此段
                var start = runtimeRecord.StartTime < cursor ? cursor : runtimeRecord.StartTime;
                var end = runtimeRecord.EndTime > to ? to.Value : runtimeRecord.EndTime;
                if (start < end)
                {
                    normalizedRuntimeRecords.Add(new DeviceRuntimeRecord(
                        deviceId,
                        runtimeRecord.RunStatus,
                        start,
                        end));

                    cursor = end;
                }
            }

            // 尾段補 Offline
            if (cursor < to)
            {
                normalizedRuntimeRecords.Add(new DeviceRuntimeRecord(
                    deviceId,
                    DeviceRunStatus.Offline,
                    cursor,
                    to.Value));
            }

            // 計算占比
            var totalSeconds = (to.Value - from.Value).TotalSeconds;
            var offlineSeconds = normalizedRuntimeRecords
                .Where(r => r.RunStatus == DeviceRunStatus.Offline)
                .Sum(r => r.RunDuration.TotalSeconds);
            var standbySeconds = normalizedRuntimeRecords
                .Where(r => r.RunStatus == DeviceRunStatus.Standby)
                .Sum(r => r.RunDuration.TotalSeconds);
            var runningSeconds = normalizedRuntimeRecords
                .Where(r => r.RunStatus == DeviceRunStatus.Running)
                .Sum(r => r.RunDuration.TotalSeconds);
            var alarmSeconds = normalizedRuntimeRecords
                .Where(r => r.RunStatus == DeviceRunStatus.Alarm)
                .Sum(r => r.RunDuration.TotalSeconds);

            // 回傳結果
            return new DeviceRuntimeSummary
            {
                RuntimeRecords = normalizedRuntimeRecords,
                OfflineUtilization = Quantize(offlineSeconds / totalSeconds),
                StandbyUtilization = Quantize(standbySeconds / totalSeconds),
                RunningUtilization = Quantize(runningSeconds / totalSeconds),
                AlarmUtilization = Quantize(alarmSeconds / totalSeconds)
            };
        }

        /// <summary>
        /// 將數值量化至固定小數位數 (四捨五入至小數點後兩位的數值)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private double Quantize(double value) => Math.Round(value, 2);

        /// <summary>
        /// 訂閱指定設備的運轉統計摘要變化
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public IDisposable SubscribeRuntimeSummary(Guid deviceId, Action<DeviceRuntimeSummary> handler)
        {
            var subscription = _subscriptions.GetOrAdd(deviceId, _ => new Subscription());

            lock (subscription.SyncRoot)
            {
                subscription.Handlers.Add(handler);
            }

            return new SubscriptionDisposable(() =>
            {
                lock (subscription.SyncRoot)
                {
                    subscription.Handlers.Remove(handler);
                }
            });
        }

        /// <summary>
        /// 訂閱訊息
        /// </summary>
        private class Subscription
        {
            /// <summary>
            /// 鎖物件
            /// </summary>
            public readonly object SyncRoot = new();

            /// <summary>
            /// 訂閱者回調列表
            /// </summary>
            public readonly List<Action<DeviceRuntimeSummary>> Handlers = new();
        }

        /// <summary>
        /// 用於管理訂閱取消的 IDisposable 實作
        /// </summary>
        private class SubscriptionDisposable : IDisposable
        {
            private readonly Action _unsubscribe;

            public SubscriptionDisposable(Action unsubscribe)
            {
                _unsubscribe = unsubscribe;
            }

            public void Dispose() => _unsubscribe();
        }
    }
}
