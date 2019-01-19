using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using RoomInfoRemote.Helpers;
using RoomInfoRemote.Interfaces;
using RoomInfoRemote.Models;
using RoomInfoRemote.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace RoomInfoRemote.ViewModels
{
    public class RoomsPageViewModel : ViewModelBase
    {
        INetworkCommunication _networkCommunication;
        IEventAggregator _eventAggregator;
        INavigationService _navigationService;

        ObservableCollection<RoomItem> _roomItems = default(ObservableCollection<RoomItem>);
        public ObservableCollection<RoomItem> RoomItems { get => _roomItems; set { SetProperty(ref _roomItems, value); } }

        bool _isRefreshing = default(bool);
        public bool IsRefreshing { get => _isRefreshing; set { SetProperty(ref _isRefreshing, value); } }

        public RoomsPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(navigationService)
        {
            if (RoomItems == null) RoomItems = new ObservableCollection<RoomItem>();
            _networkCommunication = DependencyService.Get<INetworkCommunication>(DependencyFetchTarget.GlobalInstance);
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
            _networkCommunication.PayloadReceived += (s, e) => ProcessPackage(JsonConvert.DeserializeObject<Package>(e.Package), e.HostName);
            _networkCommunication.SendPayload("", null, Settings.UdpPort, NetworkProtocol.UserDatagram, true);
            _eventAggregator.GetEvent<CurrentPageChangedEvent>().Subscribe(e =>
            {
                if (e == typeof(RoomsPage))
                {
                    if (RoomItems != null) RoomItems.Clear();
                    _networkCommunication.SendPayload(null, null, Settings.UdpPort, NetworkProtocol.UserDatagram, true);
                }
            });
            _eventAggregator.GetEvent<ButtonPressedEvent>().Subscribe(e =>
            {
                if (e == ButtenType.Refresh)
                {
                    if (RoomItems != null) RoomItems.Clear();
                    _networkCommunication.SendPayload(null, null, Settings.UdpPort, NetworkProtocol.UserDatagram, true);
                }
            });
        }

        private void ProcessPackage(Package package, string hostName)
        {
            switch ((PayloadType)package.PayloadType)
            {
                case PayloadType.Occupancy:
                    break;
                case PayloadType.Room:
                    if (RoomItems == null) RoomItems = new ObservableCollection<RoomItem>();
                    Device.BeginInvokeOnMainThread(() => RoomItems.Add(new RoomItem(_networkCommunication) { Room = JsonConvert.DeserializeObject<Room>(package.Payload.ToString()), HostName = hostName }));
                    break;
                case PayloadType.Schedule:
                    break;
                case PayloadType.StandardWeek:
                    break;
                case PayloadType.RequestOccupancy:
                    break;
                case PayloadType.RequestSchedule:
                    break;
                case PayloadType.RequestStandardWeek:
                    break;
                case PayloadType.IotDim:
                    break;
                default:
                    break;
            }
        }

        private ICommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new DelegateCommand<object>((param) =>
        {
            IsRefreshing = true;
            if (RoomItems != null) RoomItems.Clear();
            _networkCommunication.SendPayload("", null, Settings.UdpPort, NetworkProtocol.UserDatagram, true);
            IsRefreshing = false;
        }));

        private ICommand _navigateToSelectedRoomPageCommand;
        public ICommand NavigateToSelectedRoomPageCommand => _navigateToSelectedRoomPageCommand ?? (_navigateToSelectedRoomPageCommand = new DelegateCommand<object>((param) =>
        {
            _navigationService.NavigateAsync("RoomPage");
            System.Diagnostics.Debug.WriteLine("UpdateRemoteOccupancyCommand executed");
            System.Diagnostics.Debug.WriteLine("UpdateRemoteOccupancyCommand parameter: " + param.ToString());
        }));
    }
}
