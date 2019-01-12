using Prism.Events;
using Prism.Navigation;
using RoomInfoRemote.Helpers;
using RoomInfoRemote.Models;
using RoomInfoRemote.Views;

namespace RoomInfoRemote.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        IEventAggregator _eventAggregator;
        string _tcpPort = default(string);
        public string TcpPort { get => _tcpPort; set { SetProperty(ref _tcpPort, value); Settings.TcpPort = TcpPort; } }

        string _udpPort = default(string);
        public string UdpPort { get => _udpPort; set { SetProperty(ref _udpPort, value); Settings.UdpPort = UdpPort; } }

        public SettingsPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(navigationService)
        {
            _eventAggregator = eventAggregator;
            TcpPort = Settings.TcpPort;
            UdpPort = Settings.UdpPort;
            _eventAggregator.GetEvent<CurrentPageChangedEvent>().Subscribe((e) =>
            {
                if (e == typeof(SettingsPage)) { }
            });
        }
    }
}
