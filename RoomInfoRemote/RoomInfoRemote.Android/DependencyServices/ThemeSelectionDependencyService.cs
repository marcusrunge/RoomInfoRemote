using RoomInfoRemote.Droid.DependencyServices;
using RoomInfoRemote.Interfaces;
using RoomInfoRemote.Models;
using Xamarin.Forms;

[assembly: Dependency(typeof(ThemeSelectionDependencyService))]
namespace RoomInfoRemote.Droid.DependencyServices
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