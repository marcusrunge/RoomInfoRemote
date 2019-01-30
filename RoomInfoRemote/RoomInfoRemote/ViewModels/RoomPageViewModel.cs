using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using RoomInfoRemote.Helpers;
using RoomInfoRemote.Interfaces;
using RoomInfoRemote.Models;
using Syncfusion.SfCalendar.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace RoomInfoRemote.ViewModels
{
    public class RoomPageViewModel : ViewModelBase
    {
        INetworkCommunication _networkCommunication;
        IEventAggregator _eventAggregator;
        List<AgendaItem> _agendaItems;

        RoomItem _roomItem = default(RoomItem);
        public RoomItem RoomItem { get => _roomItem; set { SetProperty(ref _roomItem, value); } }

        //ObservableCollection<AgendaItem> _agendaItems = default(ObservableCollection<AgendaItem>);
        //public ObservableCollection<AgendaItem> AgendaItems { get => _agendaItems; set { SetProperty(ref _agendaItems, value); } }

        CalendarEventCollection _calendarInlineEvents = default(CalendarEventCollection);
        public CalendarEventCollection CalendarInlineEvents { get => _calendarInlineEvents; set { SetProperty(ref _calendarInlineEvents, value); } }

        AgendaItem _agendaItem = default(AgendaItem);
        public AgendaItem AgendaItem { get => _agendaItem; set { SetProperty(ref _agendaItem, value); } }

        bool _isReservationContentViewVisible = default(bool);
        public bool IsReservationContentViewVisible { get => _isReservationContentViewVisible; set { SetProperty(ref _isReservationContentViewVisible, value); } }

        public RoomPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(navigationService)
        {
            _networkCommunication = DependencyService.Get<INetworkCommunication>(DependencyFetchTarget.GlobalInstance);
            _eventAggregator = eventAggregator;
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            RoomItem = parameters.GetValue<RoomItem>("RoomItem");
            Title = RoomItem.Room.RoomName + " " + RoomItem.Room.RoomNumber;
            var package = new Package() { PayloadType = (int)PayloadType.RequestSchedule };
            _networkCommunication.PayloadReceived += (s, e) => ProcessPackage(JsonConvert.DeserializeObject<Package>(e.Package), e.HostName);
            await _networkCommunication.SendPayload(JsonConvert.SerializeObject(package), RoomItem.HostName, Settings.TcpPort, NetworkProtocol.TransmissionControl);
            IsReservationContentViewVisible = false;
        }

        private ICommand _openReservationPopupCommand;
        public ICommand OpenReservationPopupCommand => _openReservationPopupCommand ?? (_openReservationPopupCommand = new DelegateCommand<object>((param) =>
        {
            if (param != null && param is InlineItemTappedEventArgs inlineItemTappedEventArgs)
            {
                if (inlineItemTappedEventArgs.InlineEvent != null) AgendaItem = _agendaItems.Where(x => x.Start.DateTime == inlineItemTappedEventArgs.InlineEvent.StartTime).Where(x => x.End.DateTime == inlineItemTappedEventArgs.InlineEvent.EndTime).Select(x => x).FirstOrDefault();
                else AgendaItem = _agendaItems.Where(x => x.Start.DateTime.Date == inlineItemTappedEventArgs.SelectedDate.Date).Select(x => x).FirstOrDefault();
            }
            else if (param != null && param is string buttonName && buttonName.Equals("addReservationButton"))
            {
                AgendaItem = new AgendaItem();
            }
            IsReservationContentViewVisible = true;
        }));

        private ICommand _addOrSaveAgendaItemCommand;
        public ICommand AddOrSaveAgendaItemCommand => _addOrSaveAgendaItemCommand ?? (_addOrSaveAgendaItemCommand = new DelegateCommand<object>(async (param) =>
        {
            var package = new Package() { PayloadType = (int)PayloadType.AgendaItem, Payload = AgendaItem };
            await _networkCommunication.SendPayload(JsonConvert.SerializeObject(package), RoomItem.HostName, Settings.TcpPort, NetworkProtocol.TransmissionControl);
            IsReservationContentViewVisible = false;
            AgendaItem = null;
        }));

        private ICommand _deleteReservationPopupCommand;
        public ICommand DeleteReservationPopupCommand => _deleteReservationPopupCommand ?? (_deleteReservationPopupCommand = new DelegateCommand<object>(async (param) =>
        {
            _agendaItems.Remove(AgendaItem);
            CalendarInlineEvents.Remove(CalendarInlineEvents.Where(x => x.StartTime == AgendaItem.Start.DateTime).Where(x => x.EndTime == AgendaItem.End.DateTime).FirstOrDefault());
            AgendaItem.IsDeleted = true;
            var package = new Package() { PayloadType = (int)PayloadType.AgendaItem, Payload = AgendaItem };
            await _networkCommunication.SendPayload(JsonConvert.SerializeObject(package), RoomItem.HostName, Settings.TcpPort, NetworkProtocol.TransmissionControl);
            IsReservationContentViewVisible = false;
            AgendaItem = null;
        }));

        private ICommand _closeReservationPopupCommand;
        public ICommand CloseReservationPopupCommand => _closeReservationPopupCommand ?? (_closeReservationPopupCommand = new DelegateCommand<object>(async (param) =>
        {
            IsReservationContentViewVisible = false;
            AgendaItem = null;
        }));

        private void ProcessPackage(Package package, string hostName)
        {
            switch ((PayloadType)package.PayloadType)
            {
                case PayloadType.Occupancy:
                    break;
                case PayloadType.Room:
                    break;
                case PayloadType.Schedule:
                    if (CalendarInlineEvents == null) CalendarInlineEvents = new CalendarEventCollection();
                    else CalendarInlineEvents.Clear();
                    _agendaItems = new List<AgendaItem>(JsonConvert.DeserializeObject<AgendaItem[]>(package.Payload.ToString()));
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        for (int i = 0; i < _agendaItems.Count; i++)
                        {
                            CalendarInlineEvents.Add(new CalendarInlineEvent()
                            {
                                StartTime = new DateTime(_agendaItems[i].Start.Ticks),
                                EndTime = new DateTime(_agendaItems[i].End.Ticks),
                                Subject = _agendaItems[i].Title,
                                Color = Color.Fuchsia,
                                IsAllDay = _agendaItems[i].IsAllDayEvent
                            });
                        }
                    });
                    break;
                case PayloadType.StandardWeek:
                    break;
                case PayloadType.RequestOccupancy:
                    break;
                case PayloadType.RequestSchedule:
                    break;
                case PayloadType.RequestStandardWeek:
                    break;
                case PayloadType.IotDim:
                    break;
                case PayloadType.AgendaItem:
                    break;
                case PayloadType.AgendaItemId:
                    AgendaItem.Id = (int)Convert.ChangeType(package.Payload, typeof(int));
                    break;
                default:
                    break;
            }
        }
    }
}
