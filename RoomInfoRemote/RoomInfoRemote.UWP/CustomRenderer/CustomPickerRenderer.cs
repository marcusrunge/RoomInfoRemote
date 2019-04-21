using RoomInfoRemote.UWP.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(Picker), typeof(CustomPickerRenderer))]
namespace RoomInfoRemote.UWP.CustomRenderer
{
    public class CustomPickerRenderer : PickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            }
        }
    }
}
