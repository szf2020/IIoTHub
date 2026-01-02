using IIoTHub.App.Wpf.Interfaces;
using IIoTHub.App.Wpf.ViewModels.MainWindow;
using IIoTHub.App.Wpf.Views.MainWindow;
using IIoTHub.Application.Interfaces;

namespace IIoTHub.App.Wpf.Services
{
    public class MainWindowNavigationService : IMainWindowNavigationService
    {
        private object _currentView;
        private readonly IDialogService _dialogService;
        private readonly IDeviceSettingService _deviceSettingService;
        private readonly IDeviceMonitorService _deviceMonitorService;

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
        public void NavigateToDashboard()
        {
            _currentView = new DashboardView(new DashboardViewModel(_dialogService, _deviceSettingService, _deviceMonitorService));
        }
    }
}