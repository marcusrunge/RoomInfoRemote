using Prism;
using Prism.Ioc;

namespace RoomInfoRemote.UWP
{
    public sealed partial class MainPage
    {
        public static MainPage MainPageInstance;
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new RoomInfoRemote.App(new UwpInitializer()));
            MainPageInstance = this;
        }
    }

    public class UwpInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
        }
    }
}
