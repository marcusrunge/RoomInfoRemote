using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace RoomInfoRemote.ViewModels
{
    public class StandardWeekPageViewModel : BindableBase
    {
        public StandardWeekPageViewModel()
        {

        }

        private ICommand _addTimespanItemCommand;
        public ICommand AddTimespanItemCommand => _addTimespanItemCommand ?? (_addTimespanItemCommand = new DelegateCommand<object>((param) =>
        {
            switch ((string)param)
            {
                case "Monday":
                    break;
                case "Tuesday":
                    break;
                case "Wednesday":
                    break;
                case "Thursday":
                    break;
                case "Friday":
                    break;
                case "Saturday":
                    break;
                case "Sunday":
                    break;
                default:
                    break;
            }
        }));
    }
}
