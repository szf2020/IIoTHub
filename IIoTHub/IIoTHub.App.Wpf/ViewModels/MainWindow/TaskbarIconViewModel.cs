using System.Windows;
using System.Windows.Input;

namespace IIoTHub.App.Wpf.ViewModels.MainWindow
{
    /// <summary>
    /// 任務列圖示 (Taskbar Icon) 的 ViewModel
    /// </summary>
    public class TaskbarIconViewModel : ViewModelBase
    {
        /// <summary>
        /// 顯示主視窗命令
        /// </summary>
        public static ICommand ShowWindowCommand => new RelayCommand(_ =>
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
        });

        /// <summary>
        /// 隱藏主視窗命令
        /// </summary>
        public static ICommand HideWindowCommand => new RelayCommand(_ =>
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.Hide();
                }
            });
        });

        /// <summary>
        /// 關閉應用程式命令
        /// </summary>
        public static ICommand ExitApplicationCommand => new RelayCommand(_ =>
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                var currentApplication = System.Windows.Application.Current;
                if (currentApplication != null)
                {
                    currentApplication.Shutdown();
                }
            });
        });
    }
}
