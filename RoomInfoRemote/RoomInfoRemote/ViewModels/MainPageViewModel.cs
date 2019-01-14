using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using RoomInfoRemote.Helpers;
using RoomInfoRemote.Interfaces;
using RoomInfoRemote.Models;
using RoomInfoRemote.Views;
using System.Windows.Input;
using Xamarin.Forms;

namespace RoomInfoRemote.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        IEventAggregator _eventAggregator;

        bool _isRefreshButtonVisible = default(bool);
        public bool IsRefreshButtonVisible { get => _isRefreshButtonVisible; set { SetProperty(ref _isRefreshButtonVisible, value); } }

        public MainPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(navigationService)
        {
            Title = "Main Page";
            _eventAggregator = eventAggregator;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (string.IsNullOrEmpty(Settings.TcpPort)) Settings.TcpPort = "8273";
            if (string.IsNullOrEmpty(Settings.UdpPort)) Settings.UdpPort = "8274";
            DependencyService.Get<INetworkCommunication>(DependencyFetchTarget.GlobalInstance).StartConnectionListener(Settings.TcpPort, NetworkProtocol.TransmissionControl);
            IsRefreshButtonVisible = true;
        }

        private ICommand _notifyCurrentPageChangedCommand;
        public ICommand NotifyCurrentPageChangedCommand => _notifyCurrentPageChangedCommand ?? (_notifyCurrentPageChangedCommand = new DelegateCommand<object>((param) =>
        {
            var currentPageType = (param as TabbedPage).CurrentPage.GetType();
            _eventAggregator.GetEvent<CurrentPageChangedEvent>().Publish(currentPageType);
            IsRefreshButtonVisible = currentPageType == typeof(RoomsPage);
        }));

        private ICommand _notifyButtonPressedCommand;
        public ICommand NotifyButtonPressedCommand => _notifyButtonPressedCommand ?? (_notifyButtonPressedCommand = new DelegateCommand<object>((param) =>
        {
            _eventAggregator.GetEvent<ButtonPressedEvent>().Publish((ButtenType)(int.Parse((string)param)));
        }));
    }
}
