﻿using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
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

        private ICommand _updateTimespanItemCommand;
        [JsonIgnore]
        public ICommand UpdateTimespanItemCommand => _updateTimespanItemCommand ?? (_updateTimespanItemCommand = new DelegateCommand<object>((param) =>
        {
            EventAggregator.GetEvent<UpdateTimespanItemEvent>().Publish(this);
        }));

        private ICommand _deleteTimespanItemCommand;
        [JsonIgnore]
        public ICommand DeleteTimespanItemCommand => _deleteTimespanItemCommand ?? (_deleteTimespanItemCommand = new DelegateCommand<object>((param) =>
        {
            EventAggregator.GetEvent<DeleteTimespanItemEvent>().Publish(param);
        }));        

        public int CompareTo(object obj)
        {
            return ((IComparable)Start).CompareTo(((TimeSpanItem)obj).Start);
        }        
    }
}