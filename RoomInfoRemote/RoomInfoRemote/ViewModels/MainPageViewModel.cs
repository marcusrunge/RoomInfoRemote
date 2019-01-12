using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using RoomInfoRemote.Helpers;
using RoomInfoRemote.Interfaces;
using RoomInfoRemote.Models;
using System.Windows.Input;
using Xamarin.Forms;

namespace RoomInfoRemote.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        IEventAggregator _eventAggregator;
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
        }

        private ICommand _notifyCurrentPageChangedCommand;
        public ICommand NotifyCurrentPageChangedCommand => _notifyCurrentPageChangedCommand ?? (_notifyCurrentPageChangedCommand = new DelegateCommand<object>((param) =>
        {
            _eventAggregator.GetEvent<CurrentPageChangedEvent>().Publish((param as TabbedPage).CurrentPage.GetType());
        }));
    }
}
