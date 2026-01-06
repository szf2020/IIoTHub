using IIoTHub.App.Wpf.Services;
using System.Windows.Input;

namespace IIoTHub.App.Wpf.ViewModels.MainWindow
{
    /// <summary>
    /// 主視窗的 ViewModel
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IMainWindowNavigationService _mainWindowNavigationService;

        public MainWindowViewModel(IMainWindowNavigationService mainWindowNavigationService)
        {
            _mainWindowNavigationService = mainWindowNavigationService;
            ShowDashboardCommand.Execute(null);
        }

        /// <summary>
        /// 當前顯示的內容頁面
        /// </summary>
        public object CurrentView => _mainWindowNavigationService.CurrentView;

        /// <summary>
        /// 導航到 Dashboard 的命令
        /// </summary>
        public ICommand ShowDashboardCommand => new RelayCommand(async _ =>
        {
            await _mainWindowNavigationService.NavigateToDashboardAsync();
            OnPropertyChanged(nameof(CurrentView));
        });
    }
}
