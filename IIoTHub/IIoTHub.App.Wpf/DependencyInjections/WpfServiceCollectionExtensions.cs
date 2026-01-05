using IIoTHub.App.Wpf.Interfaces;
using IIoTHub.App.Wpf.Services;
using IIoTHub.App.Wpf.ViewModels.MainWindow;
using Microsoft.Extensions.DependencyInjection;

namespace IIoTHub.App.Wpf.DependencyInjections
{
    /// <summary>
    /// 擴充方法，提供將WPF層服務註冊到依賴注入容器的功能
    /// </summary>
    public static class WpfServiceCollectionExtensions
    {
        /// <summary>
        /// 註冊WPF層服務
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddWpf(this IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<IMainWindowNavigationService, MainWindowNavigationService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IImagePickerService, ImagePickerService>();
            return services;
        }
    }
}
