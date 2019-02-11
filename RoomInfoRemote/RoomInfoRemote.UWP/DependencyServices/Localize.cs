using RoomInfoRemote.Interfaces;
using RoomInfoRemote.UWP.DependencyServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Localize))]
namespace RoomInfoRemote.UWP.DependencyServices
{
    public class Localize : ILocalize
    {
        public CultureInfo GetCurrentCultureInfo()
        {
            return CultureInfo.CurrentCulture;
        }

        public void SetLocale(CultureInfo ci)
        {
            CultureInfo.CurrentCulture = ci;
            CultureInfo.CurrentUICulture = ci;
        }
    }
}
