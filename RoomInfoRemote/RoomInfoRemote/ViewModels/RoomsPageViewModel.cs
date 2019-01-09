using Prism.Commands;
using Prism.Mvvm;
using RoomInfoRemote.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
