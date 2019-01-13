using Prism.Mvvm;
using System.Collections.Generic;

namespace RoomInfoRemote.Models
{
    public class RoomItem : BindableBase
    {
        public string HostName { get; set; }
        public List<AgendaItem> AgendaItems { get; set; }
        public StandardWeek StandardWeek { get; set; }

        Room _room = default(Room);
        public Room Room { get => _room; set { SetProperty(ref _room, value); } }

        int _occupancy = default(int);
        public int Occupancy { get => _occupancy; set { SetProperty(ref _occupancy, value); } }

        //object _reserved = default(object);
        //public object Reserved { get => _reserved; set { SetProperty(ref _reserved, value); } }
    }
}
