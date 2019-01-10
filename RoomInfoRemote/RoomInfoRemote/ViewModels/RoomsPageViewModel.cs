using Prism.Mvvm;
using Prism.Navigation;
using RoomInfoRemote.Models;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace RoomInfoRemote.ViewModels
{
    public class RoomsPageViewModel : ViewModelBase
    {
        ObservableCollection<RoomItem> _roomItems = default(ObservableCollection<RoomItem>);
        public ObservableCollection<RoomItem> RoomItem { get => _roomItems; set { SetProperty(ref _roomItems, value); } }

        public RoomsPageViewModel(INavigationService navigationService) : base(navigationService)
        {

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            
        }
    }
}
