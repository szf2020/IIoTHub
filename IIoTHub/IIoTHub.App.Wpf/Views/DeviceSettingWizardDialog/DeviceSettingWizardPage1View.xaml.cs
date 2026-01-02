using IIoTHub.App.Wpf.ViewModels.DeviceSettingWizardDialog;
using System.Windows.Controls;

namespace IIoTHub.App.Wpf.Views.DeviceSettingWizardDialog
{
    public partial class DeviceSettingWizardPage1View : UserControl
    {
        public DeviceSettingWizardPage1View(DeviceSettingWizardPage1ViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
