using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Windows.Input;

namespace RoomInfoRemote.Models
{
    public class TimeSpanItem : BindableBase
    {
        IEventAggregator _eventAggregator = default;
        [JsonIgnore]
        public IEventAggregator EventAggregator { get => _eventAggregator; set { SetProperty(ref _eventAggregator, value); } }
        int _id = default;
        public int Id { get => _id; set { SetProperty(ref _id, value); } }

        int _dayOfWeek = default;
        public int DayOfWeek { get => _dayOfWeek; set { SetProperty(ref _dayOfWeek, value); } }

        TimeSpan _start = default;
        public TimeSpan Start { get => _start; set { SetProperty(ref _start, value); } }

        TimeSpan _end = default;
        public TimeSpan End { get => _end; set { SetProperty(ref _end, value); } }

        int _occupancy = default;
        public int Occupancy { get => _occupancy; set { SetProperty(ref _occupancy, value); } }

        public long TimeStamp { get; set; }

        public bool IsDeleted { get; set; }

        double _width = default;
        [JsonIgnore]
        public double Width { get => _width; set { SetProperty(ref _width, value); } }

        private ICommand _editTimeSpanItemCommand;
        [JsonIgnore]
        public ICommand EditTimeSpanItemCommand => _editTimeSpanItemCommand ?? (_editTimeSpanItemCommand = new DelegateCommand<object>((param) =>
        {
            EventAggregator.GetEvent<EditTimeSpanItemEvent>().Publish(this);
        }));

        private ICommand _deleteTimeSpanItemCommand;
        [JsonIgnore]
        public ICommand DeleteTimeSpanItemCommand => _deleteTimeSpanItemCommand ?? (_deleteTimeSpanItemCommand = new DelegateCommand<object>((param) =>
        {
            EventAggregator.GetEvent<DeleteTimeSpanItemEvent>().Publish(this);
        }));

        public int CompareTo(object obj) => ((IComparable)Start).CompareTo(((TimeSpanItem)obj).Start);
    }
}
