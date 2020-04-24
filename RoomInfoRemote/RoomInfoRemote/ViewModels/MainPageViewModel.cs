using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using RoomInfoRemote.Extension;
using RoomInfoRemote.Helpers;
using RoomInfoRemote.Interfaces;
using RoomInfoRemote.Models;
using RoomInfoRemote.Views;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows.Input;
using Xamarin.Forms;

namespace RoomInfoRemote.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        IEventAggregator _eventAggregator;
        ResourceManager _resourceManager;
        readonly CultureInfo _cultureInfo;

        bool _isRefreshButtonVisible = default;
        public bool IsRefreshButtonVisible { get => _isRefreshButtonVisible; set { SetProperty(ref _isRefreshButtonVisible, value); } }

        public MainPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(navigationService)
        {
            _eventAggregator = eventAggregator;
            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            {
                _cultureInfo = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            }
            _resourceManager = new ResourceManager("RoomInfoRemote.Resx.AppResources", typeof(TranslateExtension).GetTypeInfo().Assembly);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (string.IsNullOrEmpty(Settings.TcpPort)) Settings.TcpPort = "8273";
            if (string.IsNullOrEmpty(Settings.UdpPort)) Settings.UdpPort = "8274";
            INetworkCommunication networkCommunication = DependencyService.Get<INetworkCommunication>(DependencyFetchTarget.GlobalInstance);
            networkCommunication.StartConnectionListener(Settings.TcpPort, NetworkProtocol.TransmissionControl);
            networkCommunication.StartConnectionListener(Settings.UdpPort, NetworkProtocol.UserDatagram);
            IsRefreshButtonVisible = true;
            Title = _resourceManager.GetString("MainPage_Title", _cultureInfo);
        }

        private ICommand _notifyCurrentPageChangedCommand;
        public ICommand NotifyCurrentPageChangedCommand => _notifyCurrentPageChangedCommand ?? (_notifyCurrentPageChangedCommand = new DelegateCommand<object>((param) =>
        {


            var currentPageType = (param as TabbedPage).CurrentPage.GetType();
            _eventAggregator.GetEvent<CurrentPageChangedEvent>().Publish(new CurrentPageChangedEventArgs(currentPageType, null));
            IsRefreshButtonVisible = currentPageType == typeof(RoomsPage);
            if (currentPageType == typeof(RoomsPage))
            {
                Title = _resourceManager.GetString("MainPage_Title", _cultureInfo);
            }
            else if (currentPageType == typeof(SettingsPage))
            {
                Title = _resourceManager.GetString("SettingsTabTitle", _cultureInfo);
            }
        }));

        private ICommand _notifyButtonPressedCommand;
        public ICommand NotifyButtonPressedCommand => _notifyButtonPressedCommand ?? (_notifyButtonPressedCommand = new DelegateCommand<object>((param) =>
        {
            _eventAggregator.GetEvent<ButtonPressedEvent>().Publish((ButtenType)(int.Parse((string)param)));
        }));
    }
}
