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
using System.Threading.Tasks;
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
                        for (int i = Friday.Count; i > 0; i--)
                        {
                            if (Friday.ElementAt(i-1).Id == e.Id)
                            {
                                Friday.RemoveAt(i - 1);
                                break;
                            }
                        }
                        break;
                    case DayOfWeek.Monday:
                        for (int i = Monday.Count; i > 0; i--)
                        {
                            if (Monday.ElementAt(i - 1).Id == e.Id)
                            {
                                Monday.RemoveAt(i - 1);
                                break;
                            }
                        }
                        break;
                    case DayOfWeek.Saturday:
                        for (int i = Saturday.Count; i > 0; i--)
                        {
                            if (Saturday.ElementAt(i - 1).Id == e.Id)
                            {
                                Saturday.RemoveAt(i - 1);
                                break;
                            }
                        }
                        break;
                    case DayOfWeek.Sunday:
                        for (int i = Sunday.Count; i > 0; i--)
                        {
                            if (Sunday.ElementAt(i - 1).Id == e.Id)
                            {
                                Sunday.RemoveAt(i - 1);
                                break;
                            }
                        }
                        break;
                    case DayOfWeek.Thursday:
                        for (int i = Thursday.Count; i > 0; i--)
                        {
                            if (Thursday.ElementAt(i - 1).Id == e.Id)
                            {
                                Thursday.RemoveAt(i - 1);
                                break;
                            }
                        }
                        break;
                    case DayOfWeek.Tuesday:
                        for (int i = Tuesday.Count; i > 0; i--)
                        {
                            if (Tuesday.ElementAt(i - 1).Id == e.Id)
                            {
                                Tuesday.RemoveAt(i - 1);
                                break;
                            }
                        }
                        break;
                    case DayOfWeek.Wednesday:
                        for (int i = Wednesday.Count; i > 0; i--)
                        {
                            if (Wednesday.ElementAt(i - 1).Id == e.Id)
                            {
                                Wednesday.RemoveAt(i - 1);
                                break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            });
            _networkCommunication.PayloadReceived += async (s, e) => { if (e.Package != null) await ProcessPackage(JsonConvert.DeserializeObject<Package>(e.Package), e.HostName); };
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
                var package = new Package() { PayloadType = (int)PayloadType.RequestStandardWeek };
                await _networkCommunication.SendPayload(JsonConvert.SerializeObject(package), _hostName, Settings.TcpPort, NetworkProtocol.TransmissionControl);
            }
        }

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
                        break;
                    case PayloadType.StandardWeek:
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Monday.Clear();
                            Tuesday.Clear();
                            Wednesday.Clear();
                            Thursday.Clear();
                            Friday.Clear();
                            Saturday.Clear();
                            Sunday.Clear();
                            var timeSpanItems = JsonConvert.DeserializeObject<TimeSpanItem[]>(package.Payload.ToString());
                            foreach (var timeSpanItem in timeSpanItems)
                            {
                                timeSpanItem.EventAggregator = _eventAggregator;
                                switch ((DayOfWeek)timeSpanItem.DayOfWeek)
                                {
                                    case DayOfWeek.Friday:
                                        Friday.Add(timeSpanItem);
                                        break;
                                    case DayOfWeek.Monday:
                                        Monday.Add(timeSpanItem);
                                        break;
                                    case DayOfWeek.Saturday:
                                        Saturday.Add(timeSpanItem);
                                        break;
                                    case DayOfWeek.Sunday:
                                        Sunday.Add(timeSpanItem);
                                        break;
                                    case DayOfWeek.Thursday:
                                        Thursday.Add(timeSpanItem);
                                        break;
                                    case DayOfWeek.Tuesday:
                                        Tuesday.Add(timeSpanItem);
                                        break;
                                    case DayOfWeek.Wednesday:
                                        Wednesday.Add(timeSpanItem);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            Friday.OrderByDescending(x => x.Start);
                            Monday.OrderByDescending(x => x.Start);
                            Saturday.OrderByDescending(x => x.Start);
                            Sunday.OrderByDescending(x => x.Start);
                            Thursday.OrderByDescending(x => x.Start);
                            Tuesday.OrderByDescending(x => x.Start);
                            Wednesday.OrderByDescending(x => x.Start);
                        });
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
                        var requestStandardWeekPackage = new Package() { PayloadType = (int)PayloadType.RequestStandardWeek };
                        await _networkCommunication.SendPayload(JsonConvert.SerializeObject(requestStandardWeekPackage), _hostName, Settings.TcpPort, NetworkProtocol.TransmissionControl);
                        break;
                    case PayloadType.TimeSpanItem:
                        break;
                    case PayloadType.TimeSpanItemId:
                        TimeSpanItem.Id = (int)Convert.ChangeType(package.Payload, typeof(int));
                        switch ((DayOfWeek)TimeSpanItem.DayOfWeek)
                        {
                            case DayOfWeek.Friday:
                                Friday.Add(new TimeSpanItem() { DayOfWeek = TimeSpanItem.DayOfWeek, End = TimeSpanItem.End, Id = TimeSpanItem.Id, Occupancy = TimeSpanItem.Occupancy, Start = TimeSpanItem.Start, TimeStamp = TimeSpanItem.TimeStamp, Width = TimeSpanItem.Width, EventAggregator = _eventAggregator });
                                Friday.OrderByDescending(x => x.Start);
                                break;
                            case DayOfWeek.Monday:
                                Monday.Add(new TimeSpanItem() { DayOfWeek = TimeSpanItem.DayOfWeek, End = TimeSpanItem.End, Id = TimeSpanItem.Id, Occupancy = TimeSpanItem.Occupancy, Start = TimeSpanItem.Start, TimeStamp = TimeSpanItem.TimeStamp, Width = TimeSpanItem.Width, EventAggregator = _eventAggregator });
                                Monday.OrderByDescending(x => x.Start);
                                break;
                            case DayOfWeek.Saturday:
                                Saturday.Add(new TimeSpanItem() { DayOfWeek = TimeSpanItem.DayOfWeek, End = TimeSpanItem.End, Id = TimeSpanItem.Id, Occupancy = TimeSpanItem.Occupancy, Start = TimeSpanItem.Start, TimeStamp = TimeSpanItem.TimeStamp, Width = TimeSpanItem.Width, EventAggregator = _eventAggregator });
                                Saturday.OrderByDescending(x => x.Start);
                                break;
                            case DayOfWeek.Sunday:
                                Sunday.Add(new TimeSpanItem() { DayOfWeek = TimeSpanItem.DayOfWeek, End = TimeSpanItem.End, Id = TimeSpanItem.Id, Occupancy = TimeSpanItem.Occupancy, Start = TimeSpanItem.Start, TimeStamp = TimeSpanItem.TimeStamp, Width = TimeSpanItem.Width, EventAggregator = _eventAggregator });
                                Sunday.OrderByDescending(x => x.Start);
                                break;
                            case DayOfWeek.Thursday:
                                Thursday.Add(new TimeSpanItem() { DayOfWeek = TimeSpanItem.DayOfWeek, End = TimeSpanItem.End, Id = TimeSpanItem.Id, Occupancy = TimeSpanItem.Occupancy, Start = TimeSpanItem.Start, TimeStamp = TimeSpanItem.TimeStamp, Width = TimeSpanItem.Width, EventAggregator = _eventAggregator });
                                Thursday.OrderByDescending(x => x.Start);
                                break;
                            case DayOfWeek.Tuesday:
                                Tuesday.Add(new TimeSpanItem() { DayOfWeek = TimeSpanItem.DayOfWeek, End = TimeSpanItem.End, Id = TimeSpanItem.Id, Occupancy = TimeSpanItem.Occupancy, Start = TimeSpanItem.Start, TimeStamp = TimeSpanItem.TimeStamp, Width = TimeSpanItem.Width, EventAggregator = _eventAggregator });
                                Tuesday.OrderByDescending(x => x.Start);
                                break;
                            case DayOfWeek.Wednesday:
                                Wednesday.Add(new TimeSpanItem() { DayOfWeek = TimeSpanItem.DayOfWeek, End = TimeSpanItem.End, Id = TimeSpanItem.Id, Occupancy = TimeSpanItem.Occupancy, Start = TimeSpanItem.Start, TimeStamp = TimeSpanItem.TimeStamp, Width = TimeSpanItem.Width, EventAggregator = _eventAggregator });
                                Wednesday.OrderByDescending(x => x.Start);
                                break;
                            default:
                                break;
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
            switch ((string)param)
            {
                case "Monday":
                    WeekDay = _resourceManager.GetString("StandardWeekPage_Monday", _cultureInfo);
                    TimeSpanItem.DayOfWeek = (int)DayOfWeek.Monday;
                    break;
                case "Tuesday":
                    WeekDay = _resourceManager.GetString("StandardWeekPage_Tuesday", _cultureInfo);
                    TimeSpanItem.DayOfWeek = (int)DayOfWeek.Tuesday;
                    break;
                case "Wednesday":
                    WeekDay = _resourceManager.GetString("StandardWeekPage_Wednesday", _cultureInfo);
                    TimeSpanItem.DayOfWeek = (int)DayOfWeek.Wednesday;
                    break;
                case "Thursday":
                    WeekDay = _resourceManager.GetString("StandardWeekPage_Thursday", _cultureInfo);
                    TimeSpanItem.DayOfWeek = (int)DayOfWeek.Thursday;
                    break;
                case "Friday":
                    WeekDay = _resourceManager.GetString("StandardWeekPage_Friday", _cultureInfo);
                    TimeSpanItem.DayOfWeek = (int)DayOfWeek.Friday;
                    break;
                case "Saturday":
                    WeekDay = _resourceManager.GetString("StandardWeekPage_Saturday", _cultureInfo);
                    TimeSpanItem.DayOfWeek = (int)DayOfWeek.Saturday;
                    break;
                case "Sunday":
                    WeekDay = _resourceManager.GetString("StandardWeekPage_Sunday", _cultureInfo);
                    TimeSpanItem.DayOfWeek = (int)DayOfWeek.Sunday;
                    break;
                default:
                    WeekDay = "End of Days";
                    break;
            }
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
            TimeSpanItem = null;
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
                    List<TimeSpanItem> TimeSpanItems;
                    switch ((DayOfWeek)TimeSpanItem.DayOfWeek)
                    {
                        case DayOfWeek.Friday:
                            TimeSpanItems = Friday.ToList();
                            break;
                        case DayOfWeek.Monday:
                            TimeSpanItems = Monday.ToList();
                            break;
                        case DayOfWeek.Saturday:
                            TimeSpanItems = Saturday.ToList();
                            break;
                        case DayOfWeek.Sunday:
                            TimeSpanItems = Sunday.ToList();
                            break;
                        case DayOfWeek.Thursday:
                            TimeSpanItems = Thursday.ToList();
                            break;
                        case DayOfWeek.Tuesday:
                            TimeSpanItems = Tuesday.ToList();
                            break;
                        case DayOfWeek.Wednesday:
                            TimeSpanItems = Wednesday.ToList();
                            break;
                        default:
                            TimeSpanItems = new List<TimeSpanItem>();
                            break;
                    }
                    if (timePicker.StyleId.Equals("startTime"))
                    {
                        var timeSpanStart = TimeValidator.ValidateStartTime(TimeSpanItem, TimeSpanItems);
                        if (timeSpanStart != TimeSpan.Zero) TimeSpanItem.Start = timeSpanStart;
                    }
                    else if (timePicker.StyleId.Equals("endTime"))
                    {
                        var timeSpanStart = TimeValidator.ValidateStartTime(TimeSpanItem, TimeSpanItems);
                        if (timeSpanStart != TimeSpan.Zero) TimeSpanItem.Start = timeSpanStart;
                        var timeSpanEnd = TimeValidator.ValidateEndTime(TimeSpanItem, TimeSpanItems);
                        if (timeSpanEnd != TimeSpan.Zero) TimeSpanItem.End = timeSpanEnd;
                    }
                    IsSaveButtonEnabled = TimeSpanItem.End <= TimeSpanItem.Start ? false : true;
                }
            }
        }));
    }
}