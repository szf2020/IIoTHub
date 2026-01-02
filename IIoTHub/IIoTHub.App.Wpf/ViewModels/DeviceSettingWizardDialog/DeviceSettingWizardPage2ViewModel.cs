using IIoTHub.App.Wpf.Contexts;
using IIoTHub.Application.Interfaces;
using System.Collections.ObjectModel;

namespace IIoTHub.App.Wpf.ViewModels.DeviceSettingWizardDialog
{
    /// <summary>
    /// 設備設定精靈第二頁的 ViewModel
    /// </summary>
    public class DeviceSettingWizardPage2ViewModel : ViewModelBase
    {
        public DeviceSettingWizardPage2ViewModel(IDeviceDriverMetadataProvider deviceDriverMetadataProvider,
                                                 DeviceSettingWizardDialogContext context)
        {
            context.ConnectionSettings
                ??= deviceDriverMetadataProvider.GetDriverMetadata(context.DeviceCategoryType)
                                                .FirstOrDefault(info => info.DisplayName == context.DeviceDriver)?.ConnectionSettings
                                                .Select(setting => new DeviceConnectionSettingState(setting))
                                                .ToList();

            DriverDisplayName = context.DeviceDriver;
            ConnectionSettings
                = new ObservableCollection<DeviceConnectionSettingViewModel>(
                    context.ConnectionSettings.Select(setting => new DeviceConnectionSettingViewModel(setting)));
        }

        /// <summary>
        /// 驅動器顯示名稱
        /// </summary>
        public string DriverDisplayName { get; }

        /// <summary>
        /// 設備連線設定列表
        /// </summary>
        public ObservableCollection<DeviceConnectionSettingViewModel> ConnectionSettings { get; }
    }

    /// <summary>
    /// 設備連線設定的 ViewModel
    /// </summary>
    public class DeviceConnectionSettingViewModel : ViewModelBase
    {
        private readonly DeviceConnectionSettingState _connectionSetting;

        public DeviceConnectionSettingViewModel(DeviceConnectionSettingState connectionSetting)
        {
            _connectionSetting = connectionSetting;
        }

        /// <summary>
        /// 設定名稱
        /// </summary>
        public string DisplayName => _connectionSetting.DisplayName;

        /// <summary>
        /// 設定說明
        /// </summary>
        public string Note => _connectionSetting.Note;

        /// <summary>
        /// 設定值
        /// </summary>
        public string Value
        {
            get => _connectionSetting.Value;
            set
            {
                if (_connectionSetting.Value == value)
                    return;
                _connectionSetting.Value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
    }
}
