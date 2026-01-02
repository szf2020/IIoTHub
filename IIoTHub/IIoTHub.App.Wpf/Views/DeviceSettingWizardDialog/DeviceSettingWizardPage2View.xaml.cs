using IIoTHub.App.Wpf.ViewModels.DeviceSettingWizardDialog;
using System.Windows.Controls;

namespace IIoTHub.App.Wpf.Views.DeviceSettingWizardDialog
{
    public partial class DeviceSettingWizardPage2View : UserControl
    {
        public DeviceSettingWizardPage2View(DeviceSettingWizardPage2ViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
