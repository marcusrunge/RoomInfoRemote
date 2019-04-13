using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using RoomInfoRemote.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace RoomInfoRemote.ViewModels
{
    public class RoomPageViewModel : ViewModelBase
    {
        IEventAggregator _eventAggregator;

        RoomItem _roomItem = default(RoomItem);
        public RoomItem RoomItem { get => _roomItem; set { SetProperty(ref _roomItem, value); } }

        public RoomPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(navigationService)
        {
            _eventAggregator = eventAggregator;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            RoomItem = parameters.GetValue<RoomItem>("RoomItem");
            Title = RoomItem.Room.RoomName + " " + RoomItem.Room.RoomNumber;
        }

        private ICommand _notifyCurrentPageChangedCommand;
        public ICommand NotifyCurrentPageChangedCommand => _notifyCurrentPageChangedCommand ?? (_notifyCurrentPageChangedCommand = new DelegateCommand<object>((param) =>
        {

        }));

        private ICommand _openReservationPopupCommand;
        public ICommand OpenReservationPopupCommand => _openReservationPopupCommand ?? (_openReservationPopupCommand = new DelegateCommand<object>((param) =>
        {
            
        }));
    }
}
