using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using RoomInfoRemote.Helpers;
using RoomInfoRemote.Interfaces;
using RoomInfoRemote.Models;
using RoomInfoRemote.Views;
using Syncfusion.XForms.Buttons;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace RoomInfoRemote.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        IEventAggregator _eventAggregator;
        string _tcpPort = default;
        public string TcpPort { get => _tcpPort; set { SetProperty(ref _tcpPort, value); Settings.TcpPort = TcpPort; } }

        string _udpPort = default;
        public string UdpPort { get => _udpPort; set { SetProperty(ref _udpPort, value); Settings.UdpPort = UdpPort; } }

        bool _isLightThemeEnabled = default;
        public bool IsLightThemeEnabled { get => _isLightThemeEnabled; set { SetProperty(ref _isLightThemeEnabled, value); } }

        bool _isDarkThemeEnabled = default;
        public bool IsDarkThemeEnabled { get => _isDarkThemeEnabled; set { SetProperty(ref _isDarkThemeEnabled, value); } }

        string _versionInfo = default;
        public string VersionInfo { get => _versionInfo; set { SetProperty(ref _versionInfo, value); } }

        public SettingsPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(navigationService)
        {
            _eventAggregator = eventAggregator;
            TcpPort = Settings.TcpPort;
            UdpPort = Settings.UdpPort;
            _eventAggregator.GetEvent<CurrentPageChangedEvent>().Subscribe((e) =>
            {
                if (e == typeof(SettingsPage)) { }
            });
            switch (Settings.Theme)
            {
                case Theme.Default:
                    break;
                case Theme.Light:
                    IsLightThemeEnabled = true;
                    break;
                case Theme.Dark:
                    IsDarkThemeEnabled = true;
                    break;
                default:
                    break;
            }
            VersionInfo = DependencyService.Get<IAppVersion>().VersionInfo();
        }

        private ICommand _setThemeCommand;
        public ICommand SetThemeCommand => _setThemeCommand ?? (_setThemeCommand = new DelegateCommand<object>((param) =>
        {
            if (param is SfRadioButton sfRadioButton)
            {
                if (sfRadioButton.StyleId.Equals("lightThemeRadioButton") && sfRadioButton.IsChecked == true)
                {
                    DependencyService.Get<IThemeSelectionDependencyService>().SetTheme(Theme.Light);
                    Settings.Theme = Theme.Light;
                }
                else if (sfRadioButton.StyleId.Equals("darkThemeRadioButton") && sfRadioButton.IsChecked == true)
                {
                    DependencyService.Get<IThemeSelectionDependencyService>().SetTheme(Theme.Dark);
                    Settings.Theme = Theme.Dark;
                }
            }
        }));

        private ICommand _executeHyperLinkCommand;
        public ICommand ExecuteHyperLinkCommand => _executeHyperLinkCommand ?? (_executeHyperLinkCommand = new DelegateCommand<object>((param) =>
        {
            try
            {
                Device.OpenUri(new Uri(param as string));
            }
            catch (Exception)
            {

            }
        }));
    }
}
