using Prism;
using Prism.Ioc;
using RoomInfoRemote.Interfaces;
using RoomInfoRemote.ViewModels;
using RoomInfoRemote.Views;
using Syncfusion.Licensing;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace RoomInfoRemote
{
    public partial class App
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            SyncfusionLicenseProvider.RegisterLicense("MTk4NjYyQDMxMzcyZTM0MmUzMEdjdGlBeVVnWTIvSFdRenV3K2FjOHlkbGlucHJOMXVVWGIxMTRHYSt5R289");
            InitializeComponent();            
            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            {
                var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
                Resx.AppResources.Culture = ci;
                DependencyService.Get<ILocalize>().SetLocale(ci);
            }

            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<RoomsPage, RoomsPageViewModel>();
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();
            containerRegistry.RegisterForNavigation<CalendarPage, CalendarPageViewModel>();
            containerRegistry.RegisterForNavigation<StandardWeekPage, StandardWeekPageViewModel>();
            containerRegistry.RegisterForNavigation<RoomPage, RoomPageViewModel>();
        }
    }
}
