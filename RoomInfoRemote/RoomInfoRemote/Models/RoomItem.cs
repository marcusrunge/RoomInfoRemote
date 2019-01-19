using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using RoomInfoRemote.Helpers;
using RoomInfoRemote.Interfaces;
using System.Collections.Generic;
using System.Windows.Input;

namespace RoomInfoRemote.Models
{
    public class RoomItem : BindableBase
    {
        bool _isCommandExecutionAllowed;
        INetworkCommunication _networkCommunication;
        public string HostName { get; set; }
        public List<AgendaItem> AgendaItems { get; set; }
        public StandardWeek StandardWeek { get; set; }

        Room _room = default(Room);
        public Room Room { get => _room; set { SetProperty(ref _room, value); } }

        //object _reserved = default(object);
        //public object Reserved { get => _reserved; set { SetProperty(ref _reserved, value); } }
        public RoomItem(INetworkCommunication networkCommunication)
        {
            _networkCommunication = networkCommunication;
            _isCommandExecutionAllowed = true;
        }

        private ICommand _updateRemoteOccupancyCommand;
        public ICommand UpdateRemoteOccupancyCommand => _updateRemoteOccupancyCommand ?? (_updateRemoteOccupancyCommand = new DelegateCommand<object>(async (param) =>
        {
            if (_isCommandExecutionAllowed)
            {
                _isCommandExecutionAllowed = false;
                var package = new Package() { PayloadType = (int)PayloadType.Occupancy, Payload = Room.Occupancy };
                await _networkCommunication.SendPayload(JsonConvert.SerializeObject(package), HostName, Settings.TcpPort, NetworkProtocol.TransmissionControl);
                System.Diagnostics.Debug.WriteLine("UpdateRemoteOccupancyCommand executed");
                System.Diagnostics.Debug.WriteLine("UpdateRemoteOccupancyCommand occupancy: " + Room.Occupancy);
            }
            else _isCommandExecutionAllowed = true;
        }));
    }
}
