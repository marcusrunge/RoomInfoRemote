using Android.Content;
using Android.Content.PM;
using RoomInfoRemote.Droid.DependencyServices;
using RoomInfoRemote.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(AppVersionDependencyService))]
namespace RoomInfoRemote.Droid.DependencyServices
{
    public class AppVersionDependencyService : IAppVersion
    {
        public string VersionInfo()
        {
            Context context = Android.App.Application.Context;
            PackageManager packageManager = context.PackageManager;
            PackageInfo packageInfo = packageManager.GetPackageInfo(context.PackageName, 0);
            return packageInfo.VersionName;
        }
    }
}