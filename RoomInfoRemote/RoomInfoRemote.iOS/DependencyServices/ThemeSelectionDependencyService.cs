using System;
using RoomInfoRemote.Interfaces;
using RoomInfoRemote.iOS.DependencyServices;
using RoomInfoRemote.Models;
using Xamarin.Forms;

[assembly: Dependency(typeof(ThemeSelectionDependencyService))]
namespace RoomInfoRemote.iOS.DependencyServices
{
    public class ThemeSelectionDependencyService : IThemeSelectionDependencyService
    {
        public Theme GetTheme()
        {
            return Theme.Default;
        }

        public void SetTheme(Theme theme)
        {
            switch (theme)
            {
                case Theme.Default:
                    break;
                case Theme.Light:
                    break;
                case Theme.Dark:
                    break;
                default:
                    break;
            }
        }
    }
}