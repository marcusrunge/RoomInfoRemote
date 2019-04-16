using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using RoomInfoRemote.Extension;
using RoomInfoRemote.Interfaces;
using System;
using System.Collections.Generic;
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

        bool _isTimeSpanContentViewVisible = default;
        public bool IsTimeSpanContentViewVisible { get => _isTimeSpanContentViewVisible; set { SetProperty(ref _isTimeSpanContentViewVisible, value); } }

        string _weekDay = default;
        public string WeekDay { get => _weekDay; set { SetProperty(ref _weekDay, value); } }

        public StandardWeekPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            {
                _cultureInfo = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            }
            _resourceManager = new ResourceManager("RoomInfoRemote.Resx.AppResources", typeof(TranslateExtension).GetTypeInfo().Assembly);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
            IsTimeSpanContentViewVisible = false;
        }

        private ICommand _addTimespanItemCommand;
        public ICommand AddTimespanItemCommand => _addTimespanItemCommand ?? (_addTimespanItemCommand = new DelegateCommand<object>((param) =>
        {
            IsTimeSpanContentViewVisible = true;
            switch ((string)param)
            {
                case "Monday":
                    WeekDay = _resourceManager.GetString("StandardWeekPage_Monday", _cultureInfo);
                    break;
                case "Tuesday":
                    WeekDay = _resourceManager.GetString("StandardWeekPage_Tuesday", _cultureInfo);
                    break;
                case "Wednesday":
                    WeekDay = _resourceManager.GetString("StandardWeekPage_Wednesday", _cultureInfo);
                    break;
                case "Thursday":
                    WeekDay = _resourceManager.GetString("StandardWeekPage_Thursday", _cultureInfo);
                    break;
                case "Friday":
                    WeekDay = _resourceManager.GetString("StandardWeekPage_Friday", _cultureInfo);
                    break;
                case "Saturday":
                    WeekDay = _resourceManager.GetString("StandardWeekPage_Saturday", _cultureInfo);
                    break;
                case "Sunday":
                    WeekDay = _resourceManager.GetString("StandardWeekPage_Sunday", _cultureInfo);
                    break;
                default:
                    WeekDay = "End of Days";
                    break;
            }
        }));

        private ICommand _saveTimespanItemCommand;
        public ICommand SaveTimespanItemCommand => _saveTimespanItemCommand ?? (_saveTimespanItemCommand = new DelegateCommand<object>(async (param) =>
        {
            IsTimeSpanContentViewVisible = false;
        }));

        private ICommand _hideTimeSpanContentCommand;
        public ICommand HideTimeSpanContentCommand => _hideTimeSpanContentCommand ?? (_hideTimeSpanContentCommand = new DelegateCommand<object>((param) =>
        {
            IsTimeSpanContentViewVisible = false;
        }));
    }
}
