using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using RoomInfoRemote.Helpers;
using RoomInfoRemote.Interfaces;
using RoomInfoRemote.Models;
using Syncfusion.SfCalendar.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace RoomInfoRemote.ViewModels
{
    public class RoomPageViewModel : ViewModelBase
    {
        INetworkCommunication _networkCommunication;
        IEventAggregator _eventAggregator;

        RoomItem _roomItem = default(RoomItem);
        public RoomItem RoomItem { get => _roomItem; set { SetProperty(ref _roomItem, value); } }

        //ObservableCollection<AgendaItem> _agendaItems = default(ObservableCollection<AgendaItem>);
        //public ObservableCollection<AgendaItem> AgendaItems { get => _agendaItems; set { SetProperty(ref _agendaItems, value); } }
        
        CalendarEventCollection _calendarInlineEvents = default(CalendarEventCollection);
        public CalendarEventCollection CalendarInlineEvents { get => _calendarInlineEvents; set { SetProperty(ref _calendarInlineEvents, value); } }

        public RoomPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(navigationService)
        {
            _networkCommunication = DependencyService.Get<INetworkCommunication>(DependencyFetchTarget.GlobalInstance);
            _eventAggregator = eventAggregator;            
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var RoomItem = parameters.GetValue<RoomItem>("RoomItem");
            Title = RoomItem.Room.RoomName + " " + RoomItem.Room.RoomNumber;
            var package = new Package() { PayloadType = (int)PayloadType.RequestSchedule };
            _networkCommunication.PayloadReceived += (s, e) => ProcessPackage(JsonConvert.DeserializeObject<Package>(e.Package), e.HostName);
            await _networkCommunication.SendPayload(JsonConvert.SerializeObject(package), RoomItem.HostName, Settings.TcpPort, NetworkProtocol.TransmissionControl);
        }

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
                    var agendaItems = JsonConvert.DeserializeObject<AgendaItem[]>(package.Payload.ToString());
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        for (int i = 0; i < agendaItems.Length; i++)
                        {                            
                            CalendarInlineEvents.Add(new CalendarInlineEvent()
                            {
                                StartTime = new DateTime(agendaItems[i].Start.Ticks),
                                EndTime = new DateTime(agendaItems[i].End.Ticks),
                                Subject = agendaItems[i].Title,
                                Color = Color.Fuchsia,
                                IsAllDay = agendaItems[i].IsAllDayEvent
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
                default:
                    break;
            }
        }
    }
}
