using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using RoomInfoRemote.Models;
using RoomInfoRemote.Views;
using System.Windows.Input;
using Xamarin.Forms;

namespace RoomInfoRemote.ViewModels
{
    public class RoomPageViewModel : ViewModelBase
    {
        IEventAggregator _eventAggregator;

        RoomItem _roomItem = default;
        public RoomItem RoomItem { get => _roomItem; set { SetProperty(ref _roomItem, value); } }

        bool _isAddReservationButtonVisible = default;
        public bool IsAddReservationButtonVisible { get => _isAddReservationButtonVisible; set { SetProperty(ref _isAddReservationButtonVisible, value); } }

        public RoomPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(navigationService)
        {
            _eventAggregator = eventAggregator;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            RoomItem = parameters.GetValue<RoomItem>("RoomItem");
            Title = RoomItem.Room.RoomName + " " + RoomItem.Room.RoomNumber;
            IsAddReservationButtonVisible = true;
        }

        private ICommand _notifyCurrentPageChangedCommand;
        public ICommand NotifyCurrentPageChangedCommand => _notifyCurrentPageChangedCommand ?? (_notifyCurrentPageChangedCommand = new DelegateCommand<object>((param) =>
        {
            var currentPageType = (param as TabbedPage).CurrentPage.GetType();
            var hostName = RoomItem?.HostName;
            _eventAggregator.GetEvent<CurrentPageChangedEvent>().Publish(new CurrentPageChangedEventArgs(currentPageType, hostName));
            IsAddReservationButtonVisible = currentPageType == typeof(CalendarPage);
        }));

        private ICommand _openReservationPopupCommand;
        public ICommand OpenReservationPopupCommand => _openReservationPopupCommand ?? (_openReservationPopupCommand = new DelegateCommand<object>((param) =>
        {
            _eventAggregator.GetEvent<OpenReservationPopupEvent>().Publish(param as string);
        }));
    }
}
