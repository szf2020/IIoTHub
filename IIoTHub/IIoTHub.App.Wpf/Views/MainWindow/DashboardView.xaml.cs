using IIoTHub.App.Wpf.Interfaces;
using IIoTHub.App.Wpf.ViewModels.MainWindow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace IIoTHub.App.Wpf.Views.MainWindow
{
    public partial class DashboardView : UserControl
    {
        public DashboardView(DashboardViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
