using Prism.Mvvm;

namespace RoomInfoRemote.Models
{
    public class Room : BindableBase
    {
        string _roomGuid = default;
        public string RoomGuid { get => _roomGuid; set { SetProperty(ref _roomGuid, value); } }

        string _roomName = default;
        public string RoomName { get => _roomName; set { SetProperty(ref _roomName, value); } }

        string _roomNumber = default;
        public string RoomNumber { get => _roomNumber; set { SetProperty(ref _roomNumber, value); } }

        int _occupancy = default;
        public int Occupancy { get => _occupancy; set { SetProperty(ref _occupancy, value); } }

        bool _isIoT = default;
        public bool IsIoT { get => _isIoT; set { SetProperty(ref _isIoT, value); } }
    }
}
