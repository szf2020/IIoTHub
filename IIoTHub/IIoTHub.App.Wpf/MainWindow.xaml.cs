using IIoTHub.App.Wpf.ViewModels.MainWindow;
using System.ComponentModel;
using System.Windows;

namespace IIoTHub.App.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // 不關閉，只隱藏
            e.Cancel = true;
            TaskbarIconViewModel.HideWindowCommand.Execute(null);
        }
    }
}