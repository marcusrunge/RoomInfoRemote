using Newtonsoft.Json;
using Prism.Events;
using Prism.Navigation;
using RoomInfoRemote.Helpers;
using RoomInfoRemote.Interfaces;
using RoomInfoRemote.Models;
using RoomInfoRemote.Views;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace RoomInfoRemote.ViewModels
{
    public class RoomsPageViewModel : ViewModelBase
    {
        INetworkCommunication _networkCommunication;
        IEventAggregator _eventAggregator;

        ObservableCollection<RoomItem> _roomItems = default(ObservableCollection<RoomItem>);
        public ObservableCollection<RoomItem> RoomItems { get => _roomItems; set { SetProperty(ref _roomItems, value); } }

        public RoomsPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(navigationService)
        {
            RoomItems = new ObservableCollection<RoomItem>();
            _networkCommunication = DependencyService.Get<INetworkCommunication>(DependencyFetchTarget.GlobalInstance);
            _eventAggregator = eventAggregator;
            _networkCommunication.ConnectionReceived += (s, e) => ProcessPackage(JsonConvert.DeserializeObject<Package>(e.Package), e.HostName);
            _networkCommunication.SendPayload(null, null, Settings.UdpPort, NetworkProtocol.UserDatagram, true);
            _eventAggregator.GetEvent<CurrentPageChangedEvent>().Subscribe((e) =>
            {
                if (e == typeof(RoomsPage)) _networkCommunication.SendPayload(null, null, Settings.UdpPort, NetworkProtocol.UserDatagram, true);
            });
        }

        private void ProcessPackage(Package package, string hostName)
        {
            System.Diagnostics.Debug.WriteLine("ProcessPackage");
            switch ((PayloadType)package.PayloadType)
            {
                case PayloadType.Occupancy:
                    break;
                case PayloadType.Room:
                    RoomItems.Add(new RoomItem() { Room = (Room)package.Payload, HostName = hostName });
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
    }
}
