using RoomInfoRemote.Interfaces;
using RoomInfoRemote.UWP.DependencyServices;
using Windows.ApplicationModel;
[assembly: Xamarin.Forms.Dependency(typeof(AppVersionDependencyService))]
namespace RoomInfoRemote.UWP.DependencyServices
{
    public class AppVersionDependencyService : IAppVersion
    {
        public string VersionInfo()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;
            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }
    }
}
