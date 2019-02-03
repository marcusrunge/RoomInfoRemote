﻿using Newtonsoft.Json;
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
                var now = DateTime.Now;
                AgendaItem = new AgendaItem() { Start = now, End = now, IsAllDayEvent = false };
            }
            IsReservationContentViewVisible = true;
        }));

        private ICommand _addOrSaveAgendaItemCommand;
        public ICommand AddOrSaveAgendaItemCommand => _addOrSaveAgendaItemCommand ?? (_addOrSaveAgendaItemCommand = new DelegateCommand<object>(async (param) =>
        {
            //TODO updating
            if (!(AgendaItem.End >= AgendaItem.Start ? _agendaItems.Where(x => x.Id != AgendaItem.Id && ((AgendaItem.Start >= x.Start && AgendaItem.Start <= x.End) || (AgendaItem.End >= x.Start && AgendaItem.End <= x.End))).FirstOrDefault() == null : false)) return;
            _agendaItems.Add(AgendaItem);
            CalendarInlineEvents.Add(new CalendarInlineEvent()
            {
                StartTime = AgendaItem.Start.DateTime,
                EndTime = AgendaItem.End.DateTime,
                Subject = AgendaItem.Title,
                Color = Color.Fuchsia,
                IsAllDay = AgendaItem.IsAllDayEvent
            });
            var package = new Package() { PayloadType = (int)PayloadType.AgendaItem, Payload = AgendaItem };
            await _networkCommunication.SendPayload(JsonConvert.SerializeObject(package), RoomItem.HostName, Settings.TcpPort, NetworkProtocol.TransmissionControl);
            IsReservationContentViewVisible = false;
            AgendaItem = null;
        }));

        private ICommand _deleteReservationPopupCommand;
        public ICommand DeleteReservationPopupCommand => _deleteReservationPopupCommand ?? (_deleteReservationPopupCommand = new DelegateCommand<object>(async (param) =>
        {
            if (AgendaItem != null)
            {
                _agendaItems.Remove(AgendaItem);
                CalendarInlineEvents.Remove(CalendarInlineEvents.Where(x => x.StartTime == AgendaItem.Start.DateTime).Where(x => x.EndTime == AgendaItem.End.DateTime).FirstOrDefault());
                AgendaItem.IsDeleted = true;
                var package = new Package() { PayloadType = (int)PayloadType.AgendaItem, Payload = AgendaItem };
                await _networkCommunication.SendPayload(JsonConvert.SerializeObject(package), RoomItem.HostName, Settings.TcpPort, NetworkProtocol.TransmissionControl);
                IsReservationContentViewVisible = false;
                AgendaItem = null;
            }
        }));

        private ICommand _closeReservationPopupCommand;
        public ICommand CloseReservationPopupCommand => _closeReservationPopupCommand ?? (_closeReservationPopupCommand = new DelegateCommand<object>((param) =>
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
                                StartTime = _agendaItems[i].Start.DateTime,
                                EndTime = _agendaItems[i].End.DateTime,
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
        private ICommand _updateValueFromPickerCommand;
        public ICommand UpdateValueFromPickerCommand => _updateValueFromPickerCommand ?? (_updateValueFromPickerCommand = new DelegateCommand<object>((param) =>
        {
            if (AgendaItem != null)
            {
                if (param is DatePicker)
                {
                    var datePicker = param as DatePicker;
                    if (datePicker.StyleId.Equals("startDate"))
                    {
                        AgendaItem.Start = new DateTimeOffset(datePicker.Date + AgendaItem.Start.TimeOfDay);
                        if(AgendaItem.End < AgendaItem.Start) AgendaItem.End = AgendaItem.Start;
                    }
                    else if (datePicker.StyleId.Equals("endDate")) AgendaItem.End = new DateTimeOffset(datePicker.Date + AgendaItem.End.TimeOfDay);
                }
                else if (param is TimePicker)
                {
                    var timePicker = param as TimePicker;
                    if (timePicker.StyleId.Equals("startTime")) AgendaItem.Start = new DateTimeOffset(AgendaItem.Start.Date + timePicker.Time);
                    else if (timePicker.StyleId.Equals("endTime")) AgendaItem.End = new DateTimeOffset(AgendaItem.End.Date + timePicker.Time);
                }
            }
        }));
    }
}
