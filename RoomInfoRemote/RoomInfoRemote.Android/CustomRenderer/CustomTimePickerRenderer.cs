using Android.Content;
using Android.Views;
using RoomInfoRemote.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TimePicker), typeof(CustomTimePickerRenderer))]
namespace RoomInfoRemote.Droid.CustomRenderer
{
    public class CustomTimePickerRenderer : TimePickerRenderer
    {
        public CustomTimePickerRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
        {
            base.OnElementChanged(e);
            if (Control!=null)
            {
                Control.Gravity = GravityFlags.CenterHorizontal;
            }
        }
    }
}