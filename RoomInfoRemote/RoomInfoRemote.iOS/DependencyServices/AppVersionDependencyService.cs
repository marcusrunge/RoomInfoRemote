
using Foundation;
using RoomInfoRemote.Interfaces;
using RoomInfoRemote.iOS.DependencyServices;
[assembly: Xamarin.Forms.Dependency(typeof(AppVersionDependencyService))]
namespace RoomInfoRemote.iOS.DependencyServices
{
    public class AppVersionDependencyService : IAppVersion
    {
        public string VersionInfo()
        {
            return NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleVersion")].ToString();
        }
    }
}