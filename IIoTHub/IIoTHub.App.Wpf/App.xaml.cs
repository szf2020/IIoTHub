using Hardcodet.Wpf.TaskbarNotification;
using IIoTHub.App.Wpf.Interfaces;
using IIoTHub.App.Wpf.Services;
using IIoTHub.App.Wpf.ViewModels.MainWindow;
using IIoTHub.Application.Interfaces;
using IIoTHub.Application.Services;
using IIoTHub.Domain.Interfaces;
using IIoTHub.Domain.Interfaces.DeviceDrivers;
using IIoTHub.Domain.Interfaces.Repositories;
using IIoTHub.Domain.Services;
using IIoTHub.Infrastructure.DeviceDrivers;
using IIoTHub.Infrastructure.DeviceDrivers.Providers;
using IIoTHub.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace IIoTHub.App.Wpf
{
    public partial class App : System.Windows.Application
    {
        private IHost _host;
        private TaskbarIcon _taskbarIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            // 檢查是否已有應用程式實例
            if (!AppSingleInstanceManager.CanCreateInstance())
            {
                // 已有實例，通知現有實例顯示主視窗後退出
                AppSingleInstanceManager.NotifyExistingInstanceToShowWindow();
                this.Shutdown();
                return;
            }
            else
            {
                base.OnStartup(e);

                // 啟動監聽來自其他實例的「顯示主視窗」訊息
                AppSingleInstanceManager.StartListeningForShowWindowMessage(ShowMainWindow);

                // 初始化 HostBuilder
                _host = Host.CreateDefaultBuilder()
                            .ConfigureServices((context, services) =>
                            {
                                services.AddSingleton<MainWindow>();
                                services.AddSingleton<MainWindowViewModel>();
                                services.AddSingleton<IMainWindowNavigationService, MainWindowNavigationService>();
                                services.AddSingleton<IDialogService, DialogService>();
                                services.AddSingleton<IImagePickerService, ImagePickerService>();
                                services.AddSingleton<IDeviceSettingService, DeviceSettingService>();
                                services.AddSingleton<IDeviceMonitorService, DeviceMonitorService>();
                                services.AddSingleton<IDeviceDriverMetadataProvider, DeviceDriverMetadataProvider>();
                                services.AddSingleton<IDeviceDriverProvider, DeviceDriverProvider>();
                                services.AddSingleton<IMachineDriver, DemoDriver>();
                                services.AddSingleton<IMachineDriver, FanucFocasLibraryDriver>();
                                services.AddSingleton<IMagazineDriver, DemoDriver>();
                                services.AddSingleton<IRobotDriver, DemoDriver>();
                                services.AddSingleton<IDeviceSettingRepository, JsonDeviceSettingRepository>();
                                services.AddSingleton<IDeviceMonitorStatusRepository, JsonDeviceMonitorStatusRepository>();
                                services.AddSingleton<IDeviceRuntimeRepository, SqliteDeviceRuntimeRepository>();
                                services.AddSingleton<IDeviceRuntimeStatisticsService, DeviceRuntimeStatisticsService>();
                            })
                            .Build();

                // 啟動 Host
                _host.Start();

                // 取得並初始化托盤圖示
                _taskbarIcon = (TaskbarIcon)FindResource("Taskbar");

                // 顯示主視窗
                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
        }

        private void ShowMainWindow()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.Show();
                    mainWindow.Activate();
                    mainWindow.WindowState = WindowState.Maximized;
                }
            });
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            _taskbarIcon?.Dispose();

            await _host?.StopAsync();
            _host?.Dispose();

            AppSingleInstanceManager.Dispose();

            base.OnExit(e);
        }
    }
}
