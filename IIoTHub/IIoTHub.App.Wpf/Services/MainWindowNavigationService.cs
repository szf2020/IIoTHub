using IIoTHub.App.Wpf.Interfaces;
using IIoTHub.App.Wpf.ViewModels.MainWindow;
using IIoTHub.App.Wpf.Views.MainWindow;
using IIoTHub.Application.Interfaces;

namespace IIoTHub.App.Wpf.Services
{
    public class MainWindowNavigationService : IMainWindowNavigationService
    {
        private readonly IDialogService _dialogService;
        private readonly IDeviceSettingService _deviceSettingService;
        private readonly IDeviceMonitorService _deviceMonitorService;

        private object _currentView;
        private DashboardView _dashboardView;

        public MainWindowNavigationService(IDialogService dialogService,
                                           IDeviceSettingService deviceSettingService,
                                           IDeviceMonitorService deviceMonitorService)
        {
            _dialogService = dialogService;
            _deviceSettingService = deviceSettingService;
            _deviceMonitorService = deviceMonitorService;
        }

        /// <summary>
        /// 目前主視窗顯示的內容
        /// </summary>
        public object CurrentView => _currentView;

        /// <summary>
        /// 導覽到 Dashboard 視圖
        /// </summary>
        public async Task NavigateToDashboardAsync()
        {
            if (_dashboardView == null)
            {
                var dashboardViewModel = await DashboardViewModel.CreateAsync(_dialogService, _deviceSettingService, _deviceMonitorService);
                _dashboardView = new DashboardView(dashboardViewModel);
            }

            _currentView = _dashboardView;
        }
    }
}