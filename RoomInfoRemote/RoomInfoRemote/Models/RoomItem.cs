﻿using Newtonsoft.Json;
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
        INetworkCommunication _networkCommunication;
        public string HostName { get; set; }
        public List<AgendaItem> AgendaItems { get; set; }

        Room _room = default;
        public Room Room { get => _room; set { SetProperty(ref _room, value); } }

        //object _reserved = default(object);
        //public object Reserved { get => _reserved; set { SetProperty(ref _reserved, value); } }
        public RoomItem(INetworkCommunication networkCommunication)
        {
            _networkCommunication = networkCommunication;
        }

        private ICommand _updateRemoteOccupancyCommand;
        public ICommand UpdateRemoteOccupancyCommand => _updateRemoteOccupancyCommand ?? (_updateRemoteOccupancyCommand = new DelegateCommand<object>(async (param) =>
        {
            var package = new Package() { PayloadType = (int)PayloadType.Occupancy, Payload = Room.Occupancy };
            await _networkCommunication.SendPayload(JsonConvert.SerializeObject(package), HostName, Settings.TcpPort, NetworkProtocol.TransmissionControl);
        }));

        private ICommand _dimRemoteIoTDeviceCommand;
        public ICommand DimRemoteIoTDeviceCommand => _dimRemoteIoTDeviceCommand ?? (_dimRemoteIoTDeviceCommand = new DelegateCommand<object>(async (param) =>
        {
            var package = new Package() { PayloadType = (int)PayloadType.IotDim, Payload = !(bool)param };
            await _networkCommunication.SendPayload(JsonConvert.SerializeObject(package), HostName, Settings.TcpPort, NetworkProtocol.TransmissionControl);
        }));
    }
}
