using IIoTHub.App.Wpf.ViewModels.DeviceSettingWizardDialog;
using System.Windows.Controls;

namespace IIoTHub.App.Wpf.Views.DeviceSettingWizardDialog
{
    public partial class DeviceSettingWizardPage3View : UserControl
    {
        public DeviceSettingWizardPage3View(DeviceSettingWizardPage3ViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
