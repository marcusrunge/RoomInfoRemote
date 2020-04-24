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
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RoomInfoRemote.ViewModels
{
    public class RoomsPageViewModel : ViewModelBase
    {
        INetworkCommunication _networkCommunication;
        IEventAggregator _eventAggregator;
        Package _discoveryPackage;

        ObservableCollection<RoomItem> _roomItems = default;
        public ObservableCollection<RoomItem> RoomItems { get => _roomItems; set { SetProperty(ref _roomItems, value); } }

        bool _isRefreshing = default;
        public bool IsRefreshing { get => _isRefreshing; set { SetProperty(ref _isRefreshing, value); } }

        public RoomsPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(navigationService)
        {
            if (RoomItems == null) RoomItems = new ObservableCollection<RoomItem>();
            _networkCommunication = DependencyService.Get<INetworkCommunication>(DependencyFetchTarget.GlobalInstance);
            _eventAggregator = eventAggregator;
            _networkCommunication.PayloadReceived += (s, e) => { if (e.Package != null) ProcessPackage(JsonConvert.DeserializeObject<Package>(e.Package), e.HostName); };

            _discoveryPackage = new Package() { PayloadType = (int)PayloadType.Discovery };
            _networkCommunication.SendPayload(JsonConvert.SerializeObject(_discoveryPackage), null, Settings.UdpPort, NetworkProtocol.UserDatagram, true);

            _eventAggregator.GetEvent<CurrentPageChangedEvent>().Subscribe(e =>
            {
                if (e.PageType == typeof(RoomsPage))
                {
                    if (RoomItems != null) RoomItems.Clear();
                    _networkCommunication.SendPayload(JsonConvert.SerializeObject(_discoveryPackage), null, Settings.UdpPort, NetworkProtocol.UserDatagram, true);
                }
            });
            _eventAggregator.GetEvent<ButtonPressedEvent>().Subscribe(e =>
            {
                if (e == ButtenType.Refresh)
                {
                    if (RoomItems != null) RoomItems.Clear();
                    _networkCommunication.SendPayload(JsonConvert.SerializeObject(_discoveryPackage), null, Settings.UdpPort, NetworkProtocol.UserDatagram, true);
                }
            });
            Connectivity.ConnectivityChanged += (s, e) => RefreshRooms();
        }

        private void RefreshRooms()
        {
            IsRefreshing = true;
            if (RoomItems != null) RoomItems.Clear();
            _networkCommunication.SendPayload(JsonConvert.SerializeObject(_discoveryPackage), null, Settings.UdpPort, NetworkProtocol.UserDatagram, true);
            IsRefreshing = false;
        }

        private void ProcessPackage(Package package, string hostName)
        {
            if (package != null)
            {
                switch ((PayloadType)package.PayloadType)
                {
                    case PayloadType.Occupancy:
                        break;
                    case PayloadType.Room:
                        if (RoomItems == null) RoomItems = new ObservableCollection<RoomItem>();
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            var room = JsonConvert.DeserializeObject<Room>(package.Payload.ToString());
                            bool roomUpdate = false;
                            for (int i = 0; i < RoomItems.Count; i++)
                            {
                                if (RoomItems[i].Room.RoomGuid.Equals(room.RoomGuid))
                                {
                                    RoomItems[i].HostName = hostName;
                                    RoomItems[i].Room.Occupancy = room.Occupancy;
                                    RoomItems[i].Room.RoomName = room.RoomName;
                                    RoomItems[i].Room.RoomNumber = room.RoomNumber;
                                    roomUpdate = true;
                                    break;
                                }
                            }
                            if (!roomUpdate) RoomItems.Add(new RoomItem(_networkCommunication) { Room = room, HostName = hostName });
                        });
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
                    case PayloadType.AgendaItem:
                        break;
                    case PayloadType.AgendaItemId:
                        break;
                    case PayloadType.Discovery:
                        break;
                    case PayloadType.PropertyChanged:
                        _networkCommunication.SendPayload(JsonConvert.SerializeObject(_discoveryPackage), null, Settings.UdpPort, NetworkProtocol.UserDatagram, true);
                        break;
                    default:
                        break;
                }
            }
        }

        private ICommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new DelegateCommand<object>((param) => RefreshRooms()));

        private ICommand _navigateToSelectedRoomPageCommand;
        public ICommand NavigateToSelectedRoomPageCommand => _navigateToSelectedRoomPageCommand ?? (_navigateToSelectedRoomPageCommand = new DelegateCommand<object>((param) =>
        {
            var navigationParameters = new NavigationParameters
            {
                { "RoomItem", param }
            };
            NavigationService.NavigateAsync("RoomPage", navigationParameters);
        }));
    }
}
