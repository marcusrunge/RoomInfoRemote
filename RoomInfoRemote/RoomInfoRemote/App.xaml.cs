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
            SyncfusionLicenseProvider.RegisterLicense("ODMwMjBAMzEzNzJlMzEyZTMwV2JmdHRUdkFhdzRFKzRKd1NOL3RGM2phT1NhRDUrcDJVL1FqRzE0ak0rQT0=;ODMwMjFAMzEzNzJlMzEyZTMwVmJIcEdHNGs5TUhkQnJ1aExwZ0s3RnF4U2ZvbVlsZWtjenBjb0ZmODZ2UT0=;ODMwMjJAMzEzNzJlMzEyZTMwb2l1SHFtY1pvUm1Nb0hoZ3RqYnRYY2EwKzgwQU1mVXB5V0JZWmxhZEZBZz0=;ODMwMjNAMzEzNzJlMzEyZTMwV2Z1REs3WGI1YnNqRGVZQTBmMllvZVdZZ0t0V0ZRTXV2aEVzVHFnSkNQMD0=;ODMwMjRAMzEzNzJlMzEyZTMwSTV3N1RyS3QyZU11QlV0OXF5YjN3WTFtOWhKOENJc2NpM1U5YmkvYjAvbz0=;ODMwMjVAMzEzNzJlMzEyZTMwQ2hxWUVidXh1eTFsNExSQzZXbURZTUhwUElmMHZKNjV3eklDVE16WTB1Zz0=;ODMwMjZAMzEzNzJlMzEyZTMwQk9nNmRSUFFGaW5sVU1nbVFXYy9FZjNXL3A0T25LczViYjc4UU9FODZNWT0=;ODMwMjdAMzEzNzJlMzEyZTMwQ3ptZVpVc0Z1MGhEamltYkdYRHZOcjNINEd0M1VTT0tzdWd1VHByRjJ2cz0=;ODMwMjhAMzEzNzJlMzEyZTMwRzFYQ3R2NEhaWU5oYUVLeGRuTFNXTVY1Z1Z3aXNFRVpWTGJqdnFDOGpIbz0=;ODMwMjlAMzEzNzJlMzEyZTMwSTV3N1RyS3QyZU11QlV0OXF5YjN3WTFtOWhKOENJc2NpM1U5YmkvYjAvbz0=");
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
            containerRegistry.RegisterForNavigation<RoomPage, RoomPageViewModel>();
        }
    }
}
