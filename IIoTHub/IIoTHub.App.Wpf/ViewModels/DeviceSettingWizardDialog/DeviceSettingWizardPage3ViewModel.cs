using IIoTHub.App.Wpf.Contexts;
using IIoTHub.Application.Interfaces;
using System.Collections.ObjectModel;

namespace IIoTHub.App.Wpf.ViewModels.DeviceSettingWizardDialog
{
    /// <summary>
    /// 設備設定精靈第三頁的 ViewModel
    /// </summary>
    public class DeviceSettingWizardPage3ViewModel : ViewModelBase
    {
        private readonly IDeviceDriverMetadataProvider _deviceDriverMetadataProvider;
        private readonly DeviceSettingWizardDialogContext _context;

        public DeviceSettingWizardPage3ViewModel(IDeviceDriverMetadataProvider deviceDriverMetadataProvider,
                                                 DeviceSettingWizardDialogContext context)
        {
            _deviceDriverMetadataProvider = deviceDriverMetadataProvider;
            _context = context;
            _context.VariableSettings
                ??= _deviceDriverMetadataProvider.GetDriverMetadata(context.DeviceCategoryType)
                                                 .FirstOrDefault(info => info.DisplayName == _context.DeviceDriver)?.VariableSettings
                                                 .Select(setting => new DeviceVariableSettingState(setting))
                                                 .ToList();

            DriverDisplayName = context.DeviceDriver;
            VariableSettings
                = new ObservableCollection<DeviceVariableSettingViewModel>(
                    _context.VariableSettings.Select(setting => new DeviceVariableSettingViewModel(setting)));
        }

        /// <summary>
        /// 驅動器顯示名稱
        /// </summary>
        public string DriverDisplayName { get; }

        /// <summary>
        /// 設備變數設定列表
        /// </summary>
        public ObservableCollection<DeviceVariableSettingViewModel> VariableSettings { get; }
    }

    /// <summary>
    /// 設備變數設定的 ViewModel
    /// </summary>
    public class DeviceVariableSettingViewModel : ViewModelBase
    {
        private readonly DeviceVariableSettingState _variableSetting;

        public DeviceVariableSettingViewModel(DeviceVariableSettingState variableSetting)
        {
            _variableSetting = variableSetting;
        }

        /// <summary>
        /// 設定名稱
        /// </summary>
        public string DisplayName => _variableSetting.DisplayName;

        /// <summary>
        /// 設定說明
        /// </summary>
        public string Note => _variableSetting.Note;

        /// <summary>
        /// 設定值
        /// </summary>
        public string Value
        {
            get => _variableSetting.Value;
            set
            {
                if (_variableSetting.Value == value)
                    return;
                _variableSetting.Value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
    }
}
