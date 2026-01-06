using IIoTHub.App.Wpf.Interfaces;
using IIoTHub.App.Wpf.Services;
using IIoTHub.Application.Enums;
using IIoTHub.Application.Interfaces;
using IIoTHub.Application.Models;
using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Models.DeviceSettings;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace IIoTHub.App.Wpf.ViewModels.MainWindow
{
    /// <summary>
    /// 儀表板的 ViewModel
    /// </summary>
    public class DashboardViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IDeviceSettingService _deviceSettingService;
        private readonly IDeviceMonitorService _deviceMonitorService;

        private readonly SemaphoreSlim _deviceSettingChangedLock = new(1, 1);

        private DashboardViewModel(IDialogService dialogService,
                                   IDeviceSettingService deviceSettingService,
                                   IDeviceMonitorService deviceMonitorService)
        {
            _dialogService = dialogService;
            _deviceSettingService = deviceSettingService;
            _deviceMonitorService = deviceMonitorService;
            _deviceSettingService.DeviceSettingChanged += (s, e) => _ = OnDeviceSettingChangedAsync(s, e);
        }

        /// <summary>
        /// 建立 DashboardViewModel
        /// </summary>
        /// <param name="dialogService"></param>
        /// <param name="deviceSettingService"></param>
        /// <param name="deviceMonitorService"></param>
        /// <returns></returns>
        public static async Task<DashboardViewModel> CreateAsync(IDialogService dialogService,
                                                                 IDeviceSettingService deviceSettingService,
                                                                 IDeviceMonitorService deviceMonitorService)
        {
            var dashboardViewModel = new DashboardViewModel(dialogService,
                                                            deviceSettingService,
                                                            deviceMonitorService);
            await dashboardViewModel.InitializeAsync();
            return dashboardViewModel;
        }

        /// <summary>
        /// 包含新增按鈕的設備列表
        /// </summary>
        public ObservableCollection<object> DevicesWithAddButton { get; } = [];

        /// <summary>
        /// 平均稼動率，只計算正在監控的設備
        /// </summary>
        public double AverageUtilization
            => DevicesWithAddButton.OfType<DeviceViewModel>()
                .Where(device => device.IsMonitoring)
                .Select(device => device.Utilization)
                .DefaultIfEmpty(0)
                .Average();

        /// <summary>
        /// 設備數量
        /// </summary>
        public int DeviceCount
            => DevicesWithAddButton.OfType<DeviceViewModel>().Count();

        /// <summary>
        /// 待機設備數量
        /// </summary>
        public int StandbyDeviceCount
            => DevicesWithAddButton.OfType<DeviceViewModel>()
                .Count(device => device.Status == DeviceRunStatus.Standby);

        /// <summary>
        /// 運轉設備數量
        /// </summary>
        public int RunDeviceCount
            => DevicesWithAddButton.OfType<DeviceViewModel>()
                .Count(device => device.Status == DeviceRunStatus.Running);

        /// <summary>
        /// 警報設備數量
        /// </summary>
        public int AlarmDeviceCount
            => DevicesWithAddButton.OfType<DeviceViewModel>()
                .Count(device => device.Status == DeviceRunStatus.Alarm);

        /// <summary>
        /// 離線設備數量
        /// </summary>
        public int OfflineDeviceCount
            => DevicesWithAddButton.OfType<DeviceViewModel>()
                .Count(device => device.Status == DeviceRunStatus.Offline);

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        private async Task InitializeAsync()
        {
            var deviceSettings = await _deviceSettingService.GetAllAsync();
            var deviceTasks = deviceSettings.Select(
                async setting => await DeviceViewModel.CreateAsync(
                    _dialogService,
                    _deviceSettingService,
                    _deviceMonitorService,
                    setting,
                    () => RefreshSummary()));
            var devices = await Task.WhenAll(deviceTasks);

            DevicesWithAddButton.Clear();
            foreach (var device in devices)
            {
                DevicesWithAddButton.Add(device);
            }
            DevicesWithAddButton.Add(new DevicePlaceholderViewModel(_dialogService));

            RefreshSummary();
        }

        /// <summary>
        /// 處理設備設定變更事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task OnDeviceSettingChangedAsync(object sender, DeviceSettingChangedEventArgs e)
        {
            await _deviceSettingChangedLock.WaitAsync();
            try
            {
                var device = DevicesWithAddButton
                    .OfType<DeviceViewModel>()
                    .FirstOrDefault(device => device.DeviceSetting.Id == e.DeviceSetting.Id);

                switch (e.ChangeType)
                {
                    case DeviceSettingChangeType.Added:
                        if (device == null)
                        {
                            var index = DevicesWithAddButton.Count - 1;
                            var deviceViewModel = await DeviceViewModel.CreateAsync(
                                _dialogService,
                                _deviceSettingService,
                                _deviceMonitorService,
                                e.DeviceSetting,
                                () => RefreshSummary());
                            DevicesWithAddButton.Insert(index, deviceViewModel);
                        }
                        break;
                    case DeviceSettingChangeType.Updated:
                        device?.UpdateDeviceSetting(e.DeviceSetting);
                        break;
                    case DeviceSettingChangeType.Deleted:
                        if (device != null)
                        {
                            var index = DevicesWithAddButton.IndexOf(device);
                            DevicesWithAddButton.RemoveAt(index);
                        }
                        break;
                }

                RefreshSummary();
            }
            finally
            {
                _deviceSettingChangedLock.Release();
            }
        }

        /// <summary>
        /// 觸發統計數據屬性變更通知
        /// </summary>
        private void RefreshSummary()
        {
            OnPropertyChanged(nameof(AverageUtilization));
            OnPropertyChanged(nameof(DeviceCount));
            OnPropertyChanged(nameof(StandbyDeviceCount));
            OnPropertyChanged(nameof(RunDeviceCount));
            OnPropertyChanged(nameof(AlarmDeviceCount));
            OnPropertyChanged(nameof(OfflineDeviceCount));
        }
    }

    /// <summary>
    /// 設備的 ViewModel
    /// </summary>
    public class DeviceViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IDeviceSettingService _deviceService;
        private readonly IDeviceMonitorService _deviceMonitorService;
        private readonly Action _snapshotUpdatedAction;

        private DeviceSetting _deviceSetting;

        private IDisposable _subscriptionDisposable;
        private bool _isMonitoring = false;
        private DeviceRunStatus _status = DeviceRunStatus.Offline;
        private double _utilization = 0;

        private DeviceViewModel(IDialogService dialogService,
                                IDeviceSettingService deviceService,
                                IDeviceMonitorService deviceMonitorService,
                                DeviceSetting deviceSetting,
                                Action snapshotUpdatedAction)
        {
            _dialogService = dialogService;
            _deviceService = deviceService;
            _deviceMonitorService = deviceMonitorService;
            _snapshotUpdatedAction = snapshotUpdatedAction;

            _deviceSetting = deviceSetting;

            MonitorCommand = new RelayCommand(async _ => await MonitorDevice());
            ViewCommand = new RelayCommand(_ => ShowDevice());
            EditCommand = new RelayCommand(_ => EditDevice());
            CopyCommand = new RelayCommand(async _ => await CopyDevice());
            DeleteCommand = new RelayCommand(async _ => await DeleteDevice());
        }

        /// <summary>
        /// 建立 DeviceViewModel
        /// </summary>
        /// <param name="dialogService"></param>
        /// <param name="deviceService"></param>
        /// <param name="deviceMonitorService"></param>
        /// <param name="deviceSetting"></param>
        /// <param name="snapshotUpdatedAction"></param>
        /// <returns></returns>
        public static async Task<DeviceViewModel> CreateAsync(IDialogService dialogService,
                                                              IDeviceSettingService deviceService,
                                                              IDeviceMonitorService deviceMonitorService,
                                                              DeviceSetting deviceSetting,
                                                              Action snapshotUpdatedAction)
        {
            var deviceViewModel = new DeviceViewModel(dialogService,
                                                      deviceService,
                                                      deviceMonitorService,
                                                      deviceSetting,
                                                      snapshotUpdatedAction);
            await deviceViewModel.InitializeAsync();
            return deviceViewModel;
        }

        /// <summary>
        /// 對應的設備設定資料
        /// </summary>
        public DeviceSetting DeviceSetting
        {
            get => _deviceSetting;
            private set
            {
                if (_deviceSetting == value)
                    return;

                _deviceSetting = value;
                OnPropertyChanged(nameof(DeviceSetting));
                RefreshDeviceSettingDerivedProperties();
            }
        }

        /// <summary>
        /// 更新設備設定資料
        /// </summary>
        /// <param name="newSetting"></param>
        public void UpdateDeviceSetting(DeviceSetting newSetting)
        {
            DeviceSetting = newSetting;
        }

        /// <summary>
        /// 刷新所有依賴 DeviceSetting 的衍生屬性
        /// </summary>
        private void RefreshDeviceSettingDerivedProperties()
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(ImageSource));
        }

        /// <summary>
        /// 設備名稱
        /// </summary>
        public string Name => _deviceSetting.Name;

        /// <summary>
        /// 設備圖示
        /// </summary>
        public BitmapSource ImageSource
            => ImageHelper.Base64ToBitmapSource(_deviceSetting.ImageBase64String);

        /// <summary>
        /// 是否正在監控
        /// </summary>
        public bool IsMonitoring
        {
            get => _isMonitoring;
            set
            {
                if (_isMonitoring == value)
                    return;

                _isMonitoring = value;
                OnPropertyChanged(nameof(IsMonitoring));
            }
        }

        /// <summary>
        /// 設備運行狀態
        /// </summary>
        public DeviceRunStatus Status
        {
            get => _status;
            set
            {
                if (_status == value)
                    return;

                _status = value;

                OnPropertyChanged(nameof(Status));
            }
        }

        /// <summary>
        /// 設備稼動率
        /// </summary>
        public double Utilization
        {
            get => _utilization;
            set
            {
                if (_utilization == value)
                    return;

                _utilization = value;

                OnPropertyChanged(nameof(Utilization));
            }
        }

        /// <summary>
        /// 開關監控命令
        /// </summary>
        public ICommand MonitorCommand { get; }

        /// <summary>
        /// 查看設備詳細資訊命令
        /// </summary>
        public ICommand ViewCommand { get; }

        /// <summary>
        /// 編輯設備命令
        /// </summary>
        public ICommand EditCommand { get; }

        /// <summary>
        /// 複製設備命令
        /// </summary>
        public ICommand CopyCommand { get; }

        /// <summary>
        /// 刪除設備命令
        /// </summary>
        public ICommand DeleteCommand { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        private async Task InitializeAsync()
        {
            // 訂閱設備快照更新
            _subscriptionDisposable = _deviceMonitorService.Subscribe(_deviceSetting.Id, snapshot =>
            {
                Status = snapshot.RunStatus;
                Utilization = snapshot.Utilization;

                _snapshotUpdatedAction();
            });

            // 同步監控狀態
            IsMonitoring = await _deviceMonitorService.IsMonitoringAsync(DeviceSetting.Id);

            // 若本來已監控，則啟動監控
            if (IsMonitoring)
            {
                await _deviceMonitorService.StartMonitorAsync(DeviceSetting.Id);
            }
        }

        /// <summary>
        /// 開始或停止監控設備
        /// </summary>
        /// <returns></returns>
        private async Task MonitorDevice()
        {
            if (IsMonitoring)
            {
                await _deviceMonitorService.StartMonitorAsync(DeviceSetting.Id);
            }
            else
            {
                await _deviceMonitorService.StopMonitorAsync(DeviceSetting.Id, StopMonitorReason.Temporary);
            }
        }

        /// <summary>
        /// 顯示設備詳細資訊
        /// </summary>
        private void ShowDevice()
        {
            // 顯示對話框
        }

        /// <summary>
        /// 編輯設備設定
        /// </summary>
        private void EditDevice()
        {
            _dialogService.ShowDeviceSettingWizardDialog(DeviceSetting);
        }

        /// <summary>
        /// 複製設備設定
        /// </summary>
        /// <returns></returns>
        private async Task CopyDevice()
        {
            var devices = await _deviceService.GetAllAsync();
            var newId = Guid.NewGuid();
            var newName = GenerateCopyName(DeviceSetting.Name,
                                           devices.Select(device => device.Name));
            await _deviceService.AddAsync(
                new DeviceSetting(newId,
                                  newName,
                                  DeviceSetting.ImageBase64String,
                                  DeviceSetting.CategoryType,
                                  DeviceSetting.DriverSetting));
        }

        /// <summary>
        /// 產生複製名稱
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="existingNames"></param>
        /// <returns></returns>
        private static string GenerateCopyName(string originalName, IEnumerable<string> existingNames)
        {
            int maxIndex = existingNames
                .Select(name =>
                {
                    if (name.StartsWith(originalName + "(") && name.EndsWith(")"))
                    {
                        var inner = name.Substring(originalName.Length + 1, name.Length - originalName.Length - 2);
                        return int.TryParse(inner, out int index) ? index : 0;
                    }
                    else
                    {
                        return 0;
                    }
                })
                .Max();
            return $"{originalName}({maxIndex + 1})";
        }

        /// <summary>
        /// 刪除設備與停止監控
        /// </summary>
        /// <returns></returns>
        private async Task DeleteDevice()
        {
            // 顯示確認對話框
            bool confirm = _dialogService.ShowConfirmationDialog(
                $"是否確定要刪除設備「{DeviceSetting.Name}」？",
                "刪除設備");

            if (!confirm)
                return; // 使用者取消刪除

            // 停止監控
            IsMonitoring = false;
            await _deviceMonitorService.StopMonitorAsync(DeviceSetting.Id, StopMonitorReason.Removed);

            // 刪除設備設定
            await _deviceService.DeleteAsync(DeviceSetting.Id);

            // 取消快照訂閱
            _subscriptionDisposable?.Dispose();
        }
    }

    /// <summary>
    /// 新增設備按鈕的 ViewModel
    /// </summary>
    public class DevicePlaceholderViewModel
    {
        private readonly IDialogService _dialogService;

        public DevicePlaceholderViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            AddCommand = new RelayCommand(_ => AddDevice());
        }

        /// <summary>
        /// 新增設備命令
        /// </summary>
        public ICommand AddCommand { get; }

        /// <summary>
        /// 顯示新增設備對話框
        /// </summary>
        private void AddDevice()
        {
            _dialogService.ShowDeviceSettingWizardDialog();
        }
    }
}
