using RoomInfoRemote.Extension;
using RoomInfoRemote.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using Xamarin.Forms;

namespace RoomInfoRemote.Helpers
{
    public class PropertyValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ResourceManager resourceManager = new ResourceManager("RoomInfoRemote.Resx.AppResources", typeof(TranslateExtension).GetTypeInfo().Assembly);
            if (value.GetType() == typeof(int))
            {
                switch ((OccupancyVisualState)((int)value))
                {
                    case OccupancyVisualState.FreeVisualState: return resourceManager.GetString("RoomsPage_Occupancy_Free");
                    case OccupancyVisualState.PresentVisualState: return resourceManager.GetString("RoomsPage_Occupancy_Present");
                    case OccupancyVisualState.AbsentVisualState: return resourceManager.GetString("RoomsPage_Occupancy_Absent");
                    case OccupancyVisualState.BusyVisualState: return resourceManager.GetString("RoomsPage_Occupancy_Busy");
                    case OccupancyVisualState.OccupiedVisualState: return resourceManager.GetString("RoomsPage_Occupancy_Occupied");
                    case OccupancyVisualState.LockedVisualState: return resourceManager.GetString("RoomsPage_Occupancy_Locked");
                    case OccupancyVisualState.HomeVisualState: return resourceManager.GetString("RoomsPage_Occupancy_Home");
                    case OccupancyVisualState.UndefinedVisualState: return null;
                    default: return null;
                }
            }
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
