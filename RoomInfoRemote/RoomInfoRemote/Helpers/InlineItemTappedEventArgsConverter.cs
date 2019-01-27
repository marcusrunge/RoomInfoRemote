using Syncfusion.SfCalendar.XForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace RoomInfoRemote.Helpers
{
    public class InlineItemTappedEventArgsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is InlineItemTappedEventArgs inlineItemTappedEventArgs))
            {
                throw new ArgumentException("Expected value to be of type InlineItemTappedEventArgs", nameof(value));
            }
            return value as InlineItemTappedEventArgs;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
