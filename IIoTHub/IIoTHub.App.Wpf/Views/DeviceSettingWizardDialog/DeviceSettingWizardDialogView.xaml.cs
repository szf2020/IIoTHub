using IIoTHub.App.Wpf.ViewModels.DeviceSettingWizardDialog;
using System.Windows;

namespace IIoTHub.App.Wpf.Views.DeviceSettingWizardDialog
{
    public partial class DeviceSettingWizardDialogView : Window
    {
        public DeviceSettingWizardDialogView(DeviceSettingWizardDialogViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
