using Prism.Mvvm;
using RoomInfoRemote.Models;
using System.Collections.ObjectModel;

namespace RoomInfoRemote.ViewModels
{
    public class RoomsPageViewModel : BindableBase
	{
        ObservableCollection<RoomItem> _roomItems = default(ObservableCollection<RoomItem>);
        public ObservableCollection<RoomItem> RoomItem { get => _roomItems; set { SetProperty(ref _roomItems, value); } }

        public RoomsPageViewModel()
        {

        }
	}
}
