﻿
using Newtonsoft.Json;
using Prism.Mvvm;
using System;

namespace RoomInfoRemote.Models
{
    public class AgendaItem : BindableBase
    {
        int _id = default(int);        
        public int Id { get => _id; set { SetProperty(ref _id, value); } }

        string _title = default(string);
        public string Title { get => _title; set { SetProperty(ref _title, value); } }

        DateTimeOffset _start = default(DateTimeOffset);
        public DateTimeOffset Start { get => _start; set { SetProperty(ref _start, value); } }

        DateTimeOffset _end = default(DateTimeOffset);
        public DateTimeOffset End { get => _end; set { SetProperty(ref _end, value); } }

        bool _isAllDayEvent = default(bool);
        public bool IsAllDayEvent { get => _isAllDayEvent; set { SetProperty(ref _isAllDayEvent, value); } }

        bool _isOverridden = default(bool);
        public bool IsOverridden { get => _isOverridden; set { SetProperty(ref _isOverridden, value); } }

        string _description = default(string);
        public string Description { get => _description; set { SetProperty(ref _description, value); } }

        int _occupancy = default(int);
        public int Occupancy { get => _occupancy; set { SetProperty(ref _occupancy, value); } }

        public long TimeStamp { get; set; }

        public bool IsDeleted { get; set; }
                
        double _width = default(double);
        [JsonIgnore]
        public double Width { get => _width; set { SetProperty(ref _width, value); } }
                
        double _mediumFontSize = default(double);
        [JsonIgnore]
        public double MediumFontSize { get => _mediumFontSize; set { SetProperty(ref _mediumFontSize, value); } }

        double _largeFontSize = default(double);
        [JsonIgnore]
        public double LargeFontSize { get => _largeFontSize; set { SetProperty(ref _largeFontSize, value); } }
    }
}
