using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using RoomInfoRemote.Extension;
using RoomInfoRemote.Helpers;
using RoomInfoRemote.Interfaces;
using RoomInfoRemote.Models;
using RoomInfoRemote.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Input;
using Xamarin.Forms;

namespace RoomInfoRemote.ViewModels
{
    public class StandardWeekPageViewModel : ViewModelBase
    {
        ResourceManager _resourceManager;
        readonly CultureInfo _cultureInfo;
        IEventAggregator _eventAggregator;
        INetworkCommunication _networkCommunication;
        string _hostName;

        bool _isTimeSpanContentViewVisible = default;
        public bool IsTimeSpanContentViewVisible { get => _isTimeSpanContentViewVisible; set { SetProperty(ref _isTimeSpanContentViewVisible, value); } }

        string _weekDay = default;
        public string WeekDay { get => _weekDay; set { SetProperty(ref _weekDay, value); } }

        TimeSpanItem _timeSpanItem = default;
        public TimeSpanItem TimeSpanItem { get => _timeSpanItem; set { SetProperty(ref _timeSpanItem, value); } }

        ObservableCollection<TimeSpanItem> _monday = default;
        public ObservableCollection<TimeSpanItem> Monday { get => _monday; set { SetProperty(ref _monday, value); } }

        ObservableCollection<TimeSpanItem> _tuesday = default;
        public ObservableCollection<TimeSpanItem> Tuesday { get => _tuesday; set { SetProperty(ref _tuesday, value); } }

        ObservableCollection<TimeSpanItem> _wednesday = default;
        public ObservableCollection<TimeSpanItem> Wednesday { get => _wednesday; set { SetProperty(ref _wednesday, value); } }

        ObservableCollection<TimeSpanItem> _thursday = default;
        public ObservableCollection<TimeSpanItem> Thursday { get => _thursday; set { SetProperty(ref _thursday, value); } }

        ObservableCollection<TimeSpanItem> _friday = default;
        public ObservableCollection<TimeSpanItem> Friday { get => _friday; set { SetProperty(ref _friday, value); } }

        ObservableCollection<TimeSpanItem> _saturday = default;
        public ObservableCollection<TimeSpanItem> Saturday { get => _saturday; set { SetProperty(ref _saturday, value); } }

        ObservableCollection<TimeSpanItem> _sunday = default;
        public ObservableCollection<TimeSpanItem> Sunday { get => _sunday; set { SetProperty(ref _sunday, value); } }

        bool _isSaveButtonEnabled = default(bool);
        public bool IsSaveButtonEnabled { get => _isSaveButtonEnabled; set { SetProperty(ref _isSaveButtonEnabled, value); } }

        public StandardWeekPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(navigationService)
        {
            _eventAggregator = eventAggregator;
            _networkCommunication = DependencyService.Get<INetworkCommunication>(DependencyFetchTarget.GlobalInstance);
            IsTimeSpanContentViewVisible = false;
            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            {
                _cultureInfo = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            }
            _resourceManager = new ResourceManager("RoomInfoRemote.Resx.AppResources", typeof(TranslateExtension).GetTypeInfo().Assembly);
            _eventAggregator.GetEvent<CurrentPageChangedEvent>().Subscribe(CurrentPageChangedAction);
        }

        private async void CurrentPageChangedAction(CurrentPageChangedEventArgs obj)
        {
            if (obj.PageType == typeof(StandardWeekPage))
            {
                _hostName = obj.HostName;
                Monday = Monday ?? new ObservableCollection<TimeSpanItem>();
                Tuesday = Tuesday ?? new ObservableCollection<TimeSpanItem>();
                Wednesday = Wednesday ?? new ObservableCollection<TimeSpanItem>();
                Thursday = Thursday ?? new ObservableCollection<TimeSpanItem>();
                Friday = Friday ?? new ObservableCollection<TimeSpanItem>();
                Saturday = Saturday ?? new ObservableCollection<TimeSpanItem>();
                Sunday = Sunday ?? new ObservableCollection<TimeSpanItem>();

                _eventAggregator.GetEvent<EditTimeSpanItemEvent>().Subscribe(e =>
                {
                    TimeSpanItem = e;
                    IsTimeSpanContentViewVisible = true;
                    IsSaveButtonEnabled = true;
                });
                _eventAggregator.GetEvent<DeleteTimeSpanItemEvent>().Subscribe(async e =>
                {
                    e.IsDeleted = true;
                    var timeSpanItemPackage = new Package() { PayloadType = (int)PayloadType.TimeSpanItem, Payload = e };
                    await _networkCommunication.SendPayload(JsonConvert.SerializeObject(timeSpanItemPackage), _hostName, Settings.TcpPort, NetworkProtocol.TransmissionControl);
                    switch ((DayOfWeek)e.DayOfWeek)
                    {
                        case DayOfWeek.Friday:
                            Friday.Remove(e);
                            break;
                        case DayOfWeek.Monday:
                            Monday.Remove(e);
                            break;
                        case DayOfWeek.Saturday:
                            Saturday.Remove(e);
                            break;
                        case DayOfWeek.Sunday:
                            Sunday.Remove(e);
                            break;
                        case DayOfWeek.Thursday:
                            Thursday.Remove(e);
                            break;
                        case DayOfWeek.Tuesday:
                            Tuesday.Remove(e);
                            break;
                        case DayOfWeek.Wednesday:
                            Wednesday.Remove(e);
                            break;
                        default:
                            break;
                    }
                });
                _networkCommunication.PayloadReceived += (s, e) => { if (e.Package != null) Device.BeginInvokeOnMainThread(() => ProcessPackage(JsonConvert.DeserializeObject<Package>(e.Package), e.HostName)); };
                var package = new Package() { PayloadType = (int)PayloadType.RequestStandardWeek };
                await _networkCommunication.SendPayload(JsonConvert.SerializeObject(package), _hostName, Settings.TcpPort, NetworkProtocol.TransmissionControl);
            }
        }

        private void ProcessPackage(Package package, string hostName)
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
                        break;
                    case PayloadType.StandardWeek:
                        Monday.Clear();
                        Tuesday.Clear();
                        Wednesday.Clear();
                        Thursday.Clear();
                        Friday.Clear();
                        Saturday.Clear();
                        Sunday.Clear();
                        var timeSpanItems = JsonConvert.DeserializeObject<TimeSpanItem[]>(package.Payload.ToString());
                        var monday = new List<TimeSpanItem>();
                        var tuesday = new List<TimeSpanItem>();
                        var wednesday = new List<TimeSpanItem>();
                        var thursday = new List<TimeSpanItem>();
                        var friday = new List<TimeSpanItem>();
                        var saturday = new List<TimeSpanItem>();
                        var sunday = new List<TimeSpanItem>();
                        foreach (var timeSpanItem in timeSpanItems)
                        {
                            timeSpanItem.EventAggregator = _eventAggregator;
                            switch ((DayOfWeek)timeSpanItem.DayOfWeek)
                            {
                                case DayOfWeek.Friday:
                                    friday.Add(timeSpanItem);
                                    break;
                                case DayOfWeek.Monday:
                                    monday.Add(timeSpanItem);
                                    break;
                                case DayOfWeek.Saturday:
                                    saturday.Add(timeSpanItem);
                                    break;
                                case DayOfWeek.Sunday:
                                    sunday.Add(timeSpanItem);
                                    break;
                                case DayOfWeek.Thursday:
                                    thursday.Add(timeSpanItem);
                                    break;
                                case DayOfWeek.Tuesday:
                                    tuesday.Add(timeSpanItem);
                                    break;
                                case DayOfWeek.Wednesday:
                                    wednesday.Add(timeSpanItem);
                                    break;
                                default:
                                    break;
                            }
                        }
                        Monday = new ObservableCollection<TimeSpanItem>(monday.OrderBy(x => x.Start));
                        Tuesday = new ObservableCollection<TimeSpanItem>(tuesday.OrderBy(x => x.Start));
                        Wednesday = new ObservableCollection<TimeSpanItem>(wednesday.OrderBy(x => x.Start));
                        Thursday = new ObservableCollection<TimeSpanItem>(thursday.OrderBy(x => x.Start));
                        Friday = new ObservableCollection<TimeSpanItem>(friday.OrderBy(x => x.Start));
                        Saturday = new ObservableCollection<TimeSpanItem>(saturday.OrderBy(x => x.Start));
                        Sunday = new ObservableCollection<TimeSpanItem>(sunday.OrderBy(x => x.Start));
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
                        break;
                    case PayloadType.Discovery:
                        break;
                    case PayloadType.PropertyChanged:
                        //var requestStandardWeekPackage = new Package() { PayloadType = (int)PayloadType.RequestStandardWeek };
                        //await _networkCommunication.SendPayload(JsonConvert.SerializeObject(requestStandardWeekPackage), _hostName, Settings.TcpPort, NetworkProtocol.TransmissionControl);
                        break;
                    case PayloadType.TimeSpanItem:
                        break;
                    case PayloadType.TimeSpanItemId:
                        if (TimeSpanItem != null)
                        {
                            TimeSpanItem.Id = (int)Convert.ChangeType(package.Payload, typeof(int));
                            switch ((DayOfWeek)TimeSpanItem.DayOfWeek)
                            {
                                case DayOfWeek.Friday:
                                    var fridayTimespanItem = Friday.Where(x => x.Id == TimeSpanItem.Id).Select(x => x).FirstOrDefault();
                                    if (fridayTimespanItem == null)
                                    {
                                        Friday.Add(new TimeSpanItem() { DayOfWeek = TimeSpanItem.DayOfWeek, End = TimeSpanItem.End, Id = TimeSpanItem.Id, Occupancy = TimeSpanItem.Occupancy, Start = TimeSpanItem.Start, TimeStamp = TimeSpanItem.TimeStamp, Width = TimeSpanItem.Width, EventAggregator = _eventAggregator });
                                        Friday = new ObservableCollection<TimeSpanItem>(Friday.OrderBy(x => x.Start));
                                    }
                                    break;
                                case DayOfWeek.Monday:
                                    var mondayTimespanItem = Monday.Where(x => x.Id == TimeSpanItem.Id).Select(x => x).FirstOrDefault();
                                    if (mondayTimespanItem == null)
                                    {
                                        Monday.Add(new TimeSpanItem() { DayOfWeek = TimeSpanItem.DayOfWeek, End = TimeSpanItem.End, Id = TimeSpanItem.Id, Occupancy = TimeSpanItem.Occupancy, Start = TimeSpanItem.Start, TimeStamp = TimeSpanItem.TimeStamp, Width = TimeSpanItem.Width, EventAggregator = _eventAggregator });
                                        Monday = new ObservableCollection<TimeSpanItem>(Monday.OrderBy(x => x.Start));
                                    }
                                    break;
                                case DayOfWeek.Saturday:
                                    var saturdayTimespanItem = Saturday.Where(x => x.Id == TimeSpanItem.Id).Select(x => x).FirstOrDefault();
                                    if (saturdayTimespanItem == null)
                                    {
                                        Saturday.Add(new TimeSpanItem() { DayOfWeek = TimeSpanItem.DayOfWeek, End = TimeSpanItem.End, Id = TimeSpanItem.Id, Occupancy = TimeSpanItem.Occupancy, Start = TimeSpanItem.Start, TimeStamp = TimeSpanItem.TimeStamp, Width = TimeSpanItem.Width, EventAggregator = _eventAggregator });
                                        Saturday = new ObservableCollection<TimeSpanItem>(Saturday.OrderBy(x => x.Start));
                                    }
                                    break;
                                case DayOfWeek.Sunday:
                                    var sundayTimespanItem = Sunday.Where(x => x.Id == TimeSpanItem.Id).Select(x => x).FirstOrDefault();
                                    if (sundayTimespanItem == null)
                                    {
                                        Sunday.Add(new TimeSpanItem() { DayOfWeek = TimeSpanItem.DayOfWeek, End = TimeSpanItem.End, Id = TimeSpanItem.Id, Occupancy = TimeSpanItem.Occupancy, Start = TimeSpanItem.Start, TimeStamp = TimeSpanItem.TimeStamp, Width = TimeSpanItem.Width, EventAggregator = _eventAggregator });
                                        Sunday = new ObservableCollection<TimeSpanItem>(Sunday.OrderBy(x => x.Start));
                                    }
                                    break;
                                case DayOfWeek.Thursday:
                                    var thursdayTimespanItem = Thursday.Where(x => x.Id == TimeSpanItem.Id).Select(x => x).FirstOrDefault();
                                    if (thursdayTimespanItem == null)
                                    {
                                        Thursday.Add(new TimeSpanItem() { DayOfWeek = TimeSpanItem.DayOfWeek, End = TimeSpanItem.End, Id = TimeSpanItem.Id, Occupancy = TimeSpanItem.Occupancy, Start = TimeSpanItem.Start, TimeStamp = TimeSpanItem.TimeStamp, Width = TimeSpanItem.Width, EventAggregator = _eventAggregator });
                                        Thursday = new ObservableCollection<TimeSpanItem>(Thursday.OrderBy(x => x.Start));
                                    }
                                    break;
                                case DayOfWeek.Tuesday:
                                    var tuesdayTimespanItem = Tuesday.Where(x => x.Id == TimeSpanItem.Id).Select(x => x).FirstOrDefault();
                                    if (tuesdayTimespanItem == null)
                                    {
                                        Tuesday.Add(new TimeSpanItem() { DayOfWeek = TimeSpanItem.DayOfWeek, End = TimeSpanItem.End, Id = TimeSpanItem.Id, Occupancy = TimeSpanItem.Occupancy, Start = TimeSpanItem.Start, TimeStamp = TimeSpanItem.TimeStamp, Width = TimeSpanItem.Width, EventAggregator = _eventAggregator });
                                        Tuesday = new ObservableCollection<TimeSpanItem>(Tuesday.OrderBy(x => x.Start));
                                    }
                                    break;
                                case DayOfWeek.Wednesday:
                                    var wednesdayimespanItem = Wednesday.Where(x => x.Id == TimeSpanItem.Id).Select(x => x).FirstOrDefault();
                                    if (wednesdayimespanItem == null)
                                    {
                                        Wednesday.Add(new TimeSpanItem() { DayOfWeek = TimeSpanItem.DayOfWeek, End = TimeSpanItem.End, Id = TimeSpanItem.Id, Occupancy = TimeSpanItem.Occupancy, Start = TimeSpanItem.Start, TimeStamp = TimeSpanItem.TimeStamp, Width = TimeSpanItem.Width, EventAggregator = _eventAggregator });
                                        Wednesday = new ObservableCollection<TimeSpanItem>(Wednesday.OrderBy(x => x.Start));
                                    }
                                    break;
                                default:
                                    break;
                            }
                            TimeSpanItem = null;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private ICommand _addTimeSpanItemCommand;
        public ICommand AddTimeSpanItemCommand => _addTimeSpanItemCommand ?? (_addTimeSpanItemCommand = new DelegateCommand<object>((param) =>
        {
            TimeSpanItem = new TimeSpanItem() { Occupancy = 0, EventAggregator = _eventAggregator, TimeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds() };
            IsTimeSpanContentViewVisible = true;
            IsSaveButtonEnabled = false;
            List<TimeSpanItem> timeSpanItems = new List<TimeSpanItem>();
            switch ((string)param)
            {
                case "Monday":
                    WeekDay = _resourceManager.GetString("StandardWeekPage_Monday", _cultureInfo);
                    TimeSpanItem.DayOfWeek = (int)DayOfWeek.Monday;
                    timeSpanItems = Monday.ToList();
                    break;
                case "Tuesday":
                    WeekDay = _resourceManager.GetString("StandardWeekPage_Tuesday", _cultureInfo);
                    TimeSpanItem.DayOfWeek = (int)DayOfWeek.Tuesday;
                    timeSpanItems = Tuesday.ToList();
                    break;
                case "Wednesday":
                    WeekDay = _resourceManager.GetString("StandardWeekPage_Wednesday", _cultureInfo);
                    TimeSpanItem.DayOfWeek = (int)DayOfWeek.Wednesday;
                    timeSpanItems = Wednesday.ToList();
                    break;
                case "Thursday":
                    WeekDay = _resourceManager.GetString("StandardWeekPage_Thursday", _cultureInfo);
                    TimeSpanItem.DayOfWeek = (int)DayOfWeek.Thursday;
                    timeSpanItems = Thursday.ToList();
                    break;
                case "Friday":
                    WeekDay = _resourceManager.GetString("StandardWeekPage_Friday", _cultureInfo);
                    TimeSpanItem.DayOfWeek = (int)DayOfWeek.Friday;
                    timeSpanItems = Friday.ToList();
                    break;
                case "Saturday":
                    WeekDay = _resourceManager.GetString("StandardWeekPage_Saturday", _cultureInfo);
                    TimeSpanItem.DayOfWeek = (int)DayOfWeek.Saturday;
                    timeSpanItems = Saturday.ToList();
                    break;
                case "Sunday":
                    WeekDay = _resourceManager.GetString("StandardWeekPage_Sunday", _cultureInfo);
                    TimeSpanItem.DayOfWeek = (int)DayOfWeek.Sunday;
                    timeSpanItems = Sunday.ToList();
                    break;
                default:
                    WeekDay = "End of Days";
                    break;
            }
            var timeSpanStart = TimeValidator.ValidateStartTime(TimeSpanItem, timeSpanItems);
            if (timeSpanStart != TimeSpan.Zero) TimeSpanItem.Start = timeSpanStart;
            var timeSpanEnd = TimeValidator.ValidateEndTime(TimeSpanItem, timeSpanItems);
            if (timeSpanEnd != TimeSpan.Zero) TimeSpanItem.End = timeSpanEnd;
        }));

        private ICommand _saveTimeSpanItemCommand;
        public ICommand SaveTimeSpanItemCommand => _saveTimeSpanItemCommand ?? (_saveTimeSpanItemCommand = new DelegateCommand<object>(async (param) =>
        {
            TimeSpanItem.TimeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (TimeSpanItem.Id < 1)
            {
                var package = new Package() { PayloadType = (int)PayloadType.TimeSpanItem, Payload = TimeSpanItem };
                await _networkCommunication.SendPayload(JsonConvert.SerializeObject(package), _hostName, Settings.TcpPort, NetworkProtocol.TransmissionControl);
            }
            else
            {
                switch ((DayOfWeek)TimeSpanItem.DayOfWeek)
                {
                    case DayOfWeek.Friday:
                        for (int i = 0; i < Friday.Count; i++)
                        {
                            if (Friday[i].Id == TimeSpanItem.Id)
                            {
                                Friday[i].End = TimeSpanItem.End;
                                Friday[i].Occupancy = TimeSpanItem.Occupancy;
                                Friday[i].Start = TimeSpanItem.Start;
                                Friday[i].TimeStamp = TimeSpanItem.TimeStamp;
                            }
                        }
                        break;
                    case DayOfWeek.Monday:
                        for (int i = 0; i < Monday.Count; i++)
                        {
                            if (Monday[i].Id == TimeSpanItem.Id)
                            {
                                Monday[i].End = TimeSpanItem.End;
                                Monday[i].Occupancy = TimeSpanItem.Occupancy;
                                Monday[i].Start = TimeSpanItem.Start;
                                Monday[i].TimeStamp = TimeSpanItem.TimeStamp;
                            }
                        }
                        break;
                    case DayOfWeek.Saturday:
                        for (int i = 0; i < Sunday.Count; i++)
                        {
                            if (Saturday[i].Id == TimeSpanItem.Id)
                            {
                                Saturday[i].End = TimeSpanItem.End;
                                Saturday[i].Occupancy = TimeSpanItem.Occupancy;
                                Saturday[i].Start = TimeSpanItem.Start;
                                Saturday[i].TimeStamp = TimeSpanItem.TimeStamp;
                            }
                        }
                        break;
                    case DayOfWeek.Sunday:
                        for (int i = 0; i < Sunday.Count; i++)
                        {
                            if (Sunday[i].Id == TimeSpanItem.Id)
                            {
                                Sunday[i].End = TimeSpanItem.End;
                                Sunday[i].Occupancy = TimeSpanItem.Occupancy;
                                Sunday[i].Start = TimeSpanItem.Start;
                                Sunday[i].TimeStamp = TimeSpanItem.TimeStamp;
                            }
                        }
                        break;
                    case DayOfWeek.Thursday:
                        for (int i = 0; i < Thursday.Count; i++)
                        {
                            if (Thursday[i].Id == TimeSpanItem.Id)
                            {
                                Thursday[i].End = TimeSpanItem.End;
                                Thursday[i].Occupancy = TimeSpanItem.Occupancy;
                                Thursday[i].Start = TimeSpanItem.Start;
                                Thursday[i].TimeStamp = TimeSpanItem.TimeStamp;
                            }
                        }
                        break;
                    case DayOfWeek.Tuesday:
                        for (int i = 0; i < Tuesday.Count; i++)
                        {
                            if (Tuesday[i].Id == TimeSpanItem.Id)
                            {
                                Tuesday[i].End = TimeSpanItem.End;
                                Tuesday[i].Occupancy = TimeSpanItem.Occupancy;
                                Tuesday[i].Start = TimeSpanItem.Start;
                                Tuesday[i].TimeStamp = TimeSpanItem.TimeStamp;
                            }
                        }
                        break;
                    case DayOfWeek.Wednesday:
                        for (int i = 0; i < Wednesday.Count; i++)
                        {
                            if (Wednesday[i].Id == TimeSpanItem.Id)
                            {
                                Wednesday[i].End = TimeSpanItem.End;
                                Wednesday[i].Occupancy = TimeSpanItem.Occupancy;
                                Wednesday[i].Start = TimeSpanItem.Start;
                                Wednesday[i].TimeStamp = TimeSpanItem.TimeStamp;
                            }
                        }
                        break;
                    default:
                        break;
                }
                var package = new Package() { PayloadType = (int)PayloadType.TimeSpanItem, Payload = TimeSpanItem };
                await _networkCommunication.SendPayload(JsonConvert.SerializeObject(package), _hostName, Settings.TcpPort, NetworkProtocol.TransmissionControl);
            }
            IsTimeSpanContentViewVisible = false;
        }));

        private ICommand _hideTimeSpanContentCommand;
        public ICommand HideTimeSpanContentCommand => _hideTimeSpanContentCommand ?? (_hideTimeSpanContentCommand = new DelegateCommand<object>((param) =>
        {
            IsTimeSpanContentViewVisible = false;
            TimeSpanItem = null;
        }));

        private ICommand _updateValueFromPickerCommand;
        public ICommand UpdateValueFromPickerCommand => _updateValueFromPickerCommand ?? (_updateValueFromPickerCommand = new DelegateCommand<object>((param) =>
        {
            if (TimeSpanItem != null)
            {
                if (param is TimePicker)
                {
                    var timePicker = param as TimePicker;
                    List<TimeSpanItem> timeSpanItems;
                    switch ((DayOfWeek)TimeSpanItem.DayOfWeek)
                    {
                        case DayOfWeek.Friday:
                            timeSpanItems = Friday.ToList();
                            break;
                        case DayOfWeek.Monday:
                            timeSpanItems = Monday.ToList();
                            break;
                        case DayOfWeek.Saturday:
                            timeSpanItems = Saturday.ToList();
                            break;
                        case DayOfWeek.Sunday:
                            timeSpanItems = Sunday.ToList();
                            break;
                        case DayOfWeek.Thursday:
                            timeSpanItems = Thursday.ToList();
                            break;
                        case DayOfWeek.Tuesday:
                            timeSpanItems = Tuesday.ToList();
                            break;
                        case DayOfWeek.Wednesday:
                            timeSpanItems = Wednesday.ToList();
                            break;
                        default:
                            timeSpanItems = new List<TimeSpanItem>();
                            break;
                    }
                    if (timePicker.StyleId.Equals("startTime"))
                    {
                        var timeSpanStart = TimeValidator.ValidateStartTime(TimeSpanItem, timeSpanItems);
                        if (timeSpanStart != TimeSpan.Zero) TimeSpanItem.Start = timeSpanStart;
                    }
                    else if (timePicker.StyleId.Equals("endTime"))
                    {
                        var timeSpanStart = TimeValidator.ValidateStartTime(TimeSpanItem, timeSpanItems);
                        if (timeSpanStart != TimeSpan.Zero) TimeSpanItem.Start = timeSpanStart;
                        var timeSpanEnd = TimeValidator.ValidateEndTime(TimeSpanItem, timeSpanItems);
                        if (timeSpanEnd != TimeSpan.Zero) TimeSpanItem.End = timeSpanEnd;
                    }
                    IsSaveButtonEnabled = TimeSpanItem.End <= TimeSpanItem.Start ? false : true;
                }
            }
        }));
    }
}