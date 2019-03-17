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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace RoomInfoRemote.ViewModels
{
    public class RoomPageViewModel : ViewModelBase
    {
        INetworkCommunication _networkCommunication;
        IEventAggregator _eventAggregator;
        List<AgendaItem> _agendaItems;
        CalendarInlineEvent _calendarInlineEvent;
        AgendaItem _currentAgendaItem;
        TimeSpan _maximumExtensionTimeSpan;

        RoomItem _roomItem = default(RoomItem);
        public RoomItem RoomItem { get => _roomItem; set { SetProperty(ref _roomItem, value); } }

        CultureInfo _cultureInfo = default(CultureInfo);
        public CultureInfo CultureInfo { get => _cultureInfo; set { SetProperty(ref _cultureInfo, value); } }

        CalendarEventCollection _calendarInlineEvents = default(CalendarEventCollection);
        public CalendarEventCollection CalendarInlineEvents { get => _calendarInlineEvents; set { SetProperty(ref _calendarInlineEvents, value); } }

        AgendaItem _agendaItem = default(AgendaItem);
        public AgendaItem AgendaItem { get => _agendaItem; set { SetProperty(ref _agendaItem, value); } }

        bool _isReservationContentViewVisible = default(bool);
        public bool IsReservationContentViewVisible { get => _isReservationContentViewVisible; set { SetProperty(ref _isReservationContentViewVisible, value); } }

        bool _isExtensionButtonVisible = default(bool);
        public bool IsExtensionButtonVisible { get => _isExtensionButtonVisible; set { SetProperty(ref _isExtensionButtonVisible, value); } }

        public RoomPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(navigationService)
        {
            _networkCommunication = DependencyService.Get<INetworkCommunication>(DependencyFetchTarget.GlobalInstance);
            _eventAggregator = eventAggregator;
            CultureInfo = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            RoomItem = parameters.GetValue<RoomItem>("RoomItem");
            Title = RoomItem.Room.RoomName + " " + RoomItem.Room.RoomNumber;
            var package = new Package() { PayloadType = (int)PayloadType.RequestSchedule };
            _networkCommunication.PayloadReceived += async (s, e) => await ProcessPackage(JsonConvert.DeserializeObject<Package>(e.Package), e.HostName);
            await _networkCommunication.SendPayload(JsonConvert.SerializeObject(package), RoomItem.HostName, Settings.TcpPort, NetworkProtocol.TransmissionControl);
            IsReservationContentViewVisible = false;
            IsExtensionButtonVisible = false;
        }

        private ICommand _openReservationPopupCommand;
        public ICommand OpenReservationPopupCommand => _openReservationPopupCommand ?? (_openReservationPopupCommand = new DelegateCommand<object>((param) =>
        {
            if (param != null && param is InlineItemTappedEventArgs inlineItemTappedEventArgs)
            {
                if (inlineItemTappedEventArgs.InlineEvent != null)
                {
                    AgendaItem = _agendaItems.Where(x => x.Start.DateTime == inlineItemTappedEventArgs.InlineEvent.StartTime).Where(x => x.End.DateTime == inlineItemTappedEventArgs.InlineEvent.EndTime).Select(x => x).FirstOrDefault();
                    _calendarInlineEvent = inlineItemTappedEventArgs.InlineEvent;
                }
                else
                {
                    AgendaItem = _agendaItems.Where(x => x.Start.DateTime.Date == inlineItemTappedEventArgs.SelectedDate.Date).Select(x => x).FirstOrDefault();
                    if (AgendaItem != null) _calendarInlineEvent = CalendarInlineEvents.Where(x => x.StartTime == AgendaItem.Start.DateTime).Where(x => x.EndTime == AgendaItem.End.DateTime).Select(x => x).FirstOrDefault();
                }
            }
            else if (param != null && param is string buttonName && buttonName.Equals("addReservationButton"))
            {
                var now = DateTime.Now;
                AgendaItem = new AgendaItem() { Start = now, End = now, IsAllDayEvent = false };
            }
            if (AgendaItem != null) IsReservationContentViewVisible = true;
        }));

        private ICommand _addOrSaveAgendaItemCommand;
        public ICommand AddOrSaveAgendaItemCommand => _addOrSaveAgendaItemCommand ?? (_addOrSaveAgendaItemCommand = new DelegateCommand<object>(async (param) =>
        {
            if (!(AgendaItem.End >= AgendaItem.Start ? _agendaItems.Where(x => x.Id != AgendaItem.Id && ((AgendaItem.Start >= x.Start && AgendaItem.Start <= x.End) || (AgendaItem.End >= x.Start && AgendaItem.End <= x.End))).FirstOrDefault() == null : false)) return;
            if (AgendaItem.Id < 1)
            {
                _agendaItems.Add(AgendaItem);
                CalendarInlineEvents.Add(new CalendarInlineEvent()
                {
                    StartTime = AgendaItem.Start.DateTime,
                    EndTime = AgendaItem.End.DateTime,
                    Subject = AgendaItem.Title,
                    IsAllDay = AgendaItem.IsAllDayEvent,
                    Color = Color.FromHex(HexColorFromOccupancy((OccupancyVisualState)AgendaItem.Occupancy))
                });
            }
            else
            {
                for (int i = 0; i < _agendaItems.Count; i++)
                {
                    if (_agendaItems[i].Id == AgendaItem.Id)
                    {
                        _agendaItems[i] = AgendaItem;
                        break;
                    }
                }
                if (CalendarInlineEvents == null) CalendarInlineEvents = new CalendarEventCollection();
                else CalendarInlineEvents.Clear();
                for (int i = 0; i < _agendaItems.Count; i++)
                {
                    CalendarInlineEvents.Add(new CalendarInlineEvent()
                    {
                        StartTime = _agendaItems[i].Start.DateTime,
                        EndTime = _agendaItems[i].End.DateTime,
                        Subject = _agendaItems[i].Title,
                        IsAllDay = _agendaItems[i].IsAllDayEvent,
                        Color = Color.FromHex(HexColorFromOccupancy((OccupancyVisualState)_agendaItems[i].Occupancy))
                    });
                }
            }

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
                for (int i = 0; i < CalendarInlineEvents.Count; i++)
                {
                    if (CalendarInlineEvents[i].StartTime == AgendaItem.Start.DateTime && CalendarInlineEvents[i].EndTime == AgendaItem.End.DateTime)
                    {
                        CalendarInlineEvents.RemoveAt(i);
                        break;
                    }
                }
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

        private async Task ProcessPackage(Package package, string hostName)
        {
            if (package != null)
            {
                switch ((PayloadType)package.PayloadType)
                {
                    case PayloadType.Occupancy:
                        break;
                    case PayloadType.Room:
                        break;
                    case PayloadType.Schedule:
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            if (CalendarInlineEvents == null) CalendarInlineEvents = new CalendarEventCollection();
                            else CalendarInlineEvents.Clear();
                            _agendaItems = new List<AgendaItem>(JsonConvert.DeserializeObject<AgendaItem[]>(package.Payload.ToString()));
                            for (int i = 0; i < _agendaItems.Count; i++)
                            {
                                CalendarInlineEvents.Add(new CalendarInlineEvent()
                                {
                                    StartTime = _agendaItems[i].Start.DateTime,
                                    EndTime = _agendaItems[i].End.DateTime,
                                    Subject = _agendaItems[i].Title,
                                    IsAllDay = _agendaItems[i].IsAllDayEvent,
                                    Color = Color.FromHex(HexColorFromOccupancy((OccupancyVisualState)_agendaItems[i].Occupancy))
                                });
                            }
                            AgendaItem currentAgendaItem = FindCurrentAgendaItem(_agendaItems);
                            AgendaItem nextAgendaItem = FindNextAgendaItem(_agendaItems);
                            if (currentAgendaItem != null)
                            {
                                _currentAgendaItem = currentAgendaItem;
                                _maximumExtensionTimeSpan = CalculateMaximumExtensionTimeSpan(currentAgendaItem, _agendaItems);
                                IsExtensionButtonVisible = _maximumExtensionTimeSpan >= TimeSpan.FromMinutes(15) ? true : false;
                            }
                            else if (nextAgendaItem != null) ScheduleExtensionButtonVisibility(nextAgendaItem);                            
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
                    case PayloadType.Discovery:
                        break;
                    case PayloadType.PropertyChanged:
                        package = new Package() { PayloadType = (int)PayloadType.RequestSchedule };
                        await _networkCommunication.SendPayload(JsonConvert.SerializeObject(package), RoomItem.HostName, Settings.TcpPort, NetworkProtocol.TransmissionControl);
                        break;
                    default:
                        break;
                }
            }
        }

        private TimeSpan CalculateMaximumExtensionTimeSpan(AgendaItem currentAgendaItem, List<AgendaItem> agendaItems)
        {
            agendaItems.Sort();
            var nextAgendaItem = agendaItems.Where(x => x.Start > currentAgendaItem.End).Select(x => x).FirstOrDefault();
            return nextAgendaItem != null ? nextAgendaItem.Start - currentAgendaItem.End : TimeSpan.MaxValue;
        }

        private void ScheduleExtensionButtonVisibility(AgendaItem nextAgendaItem)
        {
            TimeSpan interval = nextAgendaItem.Start - DateTimeOffset.Now;
            Device.StartTimer(interval, () => 
            {
                IsExtensionButtonVisible = true;
                interval = nextAgendaItem.End - nextAgendaItem.Start;
                Device.StartTimer(interval, () =>
                {
                    IsExtensionButtonVisible = false;
                    return false;
                });
                return false;
            });
        }

        private AgendaItem FindCurrentAgendaItem(List<AgendaItem> agendaItems)
        {
            DateTimeOffset dateTimeNowOffset = new DateTimeOffset(DateTime.Now);
            return agendaItems.Where(x => x.Start <= dateTimeNowOffset && x.End >= dateTimeNowOffset).Select(x => x).FirstOrDefault();
        }

        private AgendaItem FindNextAgendaItem(List<AgendaItem> agendaItems)
        {
            DateTimeOffset dateTimeNowOffset = new DateTimeOffset(DateTime.Now);
            return agendaItems.Where(predicate: x => x.Start > dateTimeNowOffset).Select(x => x).FirstOrDefault();
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
                        if (AgendaItem.End < AgendaItem.Start) AgendaItem.End = AgendaItem.Start.Date + AgendaItem.End.TimeOfDay;
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

        private ICommand _changeStateCommand;
        public ICommand ChangeStateCommand => _changeStateCommand ?? (_changeStateCommand = new DelegateCommand<object>((param) =>
        {
            if (AgendaItem == null) return;
            if (!(bool)param)
            {
                AgendaItem.Start = AgendaItem.Start.Date + TimeSpan.FromHours(0);
                AgendaItem.End = AgendaItem.End.Date + TimeSpan.FromHours(23).Add(TimeSpan.FromMinutes(59));
            }
            else
            {
                var now = DateTime.Now;
                AgendaItem.Start = AgendaItem.Start.Date + now.TimeOfDay;
                AgendaItem.End = AgendaItem.End.Date + now.TimeOfDay;
            }
        }));

        private string HexColorFromOccupancy(OccupancyVisualState occupancyVisualState)
        {
            switch (occupancyVisualState)
            {
                case OccupancyVisualState.FreeVisualState: return "#FF00F200";
                case OccupancyVisualState.PresentVisualState: return "#FF01A9F4";
                case OccupancyVisualState.AbsentVisualState: return "#FF797979";
                case OccupancyVisualState.BusyVisualState: return "#FFF2F200";
                case OccupancyVisualState.OccupiedVisualState: return "#FFCC00CC";
                case OccupancyVisualState.LockedVisualState: return "#FFF20000";
                default: return "#000000";
            }
        }

        private ICommand _extendCurrentReservationCommand;
        public ICommand ExtendCurrentReservationCommand => _extendCurrentReservationCommand ?? (_extendCurrentReservationCommand = new DelegateCommand<object>((param) =>
        {
            if (_currentAgendaItem != null)
            {
                _currentAgendaItem.End.Add(TimeSpan.FromMinutes(15));
                _maximumExtensionTimeSpan = CalculateMaximumExtensionTimeSpan(_currentAgendaItem, _agendaItems);
                IsExtensionButtonVisible = _maximumExtensionTimeSpan >= TimeSpan.FromMinutes(15) ? true : false;
                //Change AgendaItem in Calendar
                //Send Update to Room
            }
        }));
    }
}
