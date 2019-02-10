using RoomInfoRemote.Interfaces;
using RoomInfoRemote.Models;
using RoomInfoRemote.UWP.DependencyServices;
using Windows.UI.Xaml;
using Xamarin.Forms;

[assembly: Dependency(typeof(ThemeSelectionDependencyService))]
namespace RoomInfoRemote.UWP.DependencyServices
{
    public class ThemeSelectionDependencyService : IThemeSelectionDependencyService
    {
        public Theme GetTheme()
        {
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                switch (frameworkElement.ActualTheme)
                {
                    case ElementTheme.Light: return Theme.Light;
                    case ElementTheme.Dark: return Theme.Dark;
                    default: return Theme.Default;
                }
            }
            else return Theme.Default;
        }

        public void SetTheme(Theme theme)
        {
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                switch (theme)
                {
                    case Theme.Light:
                        frameworkElement.RequestedTheme = ElementTheme.Light;
                        break;
                    case Theme.Dark:
                        frameworkElement.RequestedTheme = ElementTheme.Dark;
                        break;
                    default:
                        frameworkElement.RequestedTheme = ElementTheme.Default;
                        break;
                }
            }
        }
    }
}
