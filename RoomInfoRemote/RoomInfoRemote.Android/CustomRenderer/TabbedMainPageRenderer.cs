using Android.Content;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomNavigation;
using RoomInfoRemote.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(TabbedMainPageRenderer))]
namespace RoomInfoRemote.Droid.CustomRenderer
{
    public class TabbedMainPageRenderer : TabbedPageRenderer
    {
        public TabbedMainPageRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
        {
            base.OnElementChanged(e);
            ViewGroup layout = (ViewGroup)this.GetChildAt(0);
            BottomNavigationView bottomNavigationView = (BottomNavigationView)layout.GetChildAt(1);
            Android.Views.View topShadow = LayoutInflater.From(Context).Inflate(Resource.Layout.top_shadow, null);

            Android.Widget.RelativeLayout.LayoutParams layoutParams =
                new Android.Widget.RelativeLayout.LayoutParams(LayoutParams.MatchParent, 15);
            layoutParams.AddRule(LayoutRules.Above, bottomNavigationView.Id);

            layout.AddView(topShadow, 2, layoutParams);
        }
    }
}