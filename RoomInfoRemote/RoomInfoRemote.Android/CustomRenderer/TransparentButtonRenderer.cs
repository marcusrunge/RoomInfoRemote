using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Content;
using RoomInfoRemote.Customs;
using RoomInfoRemote.Droid.Renderer;

[assembly: ExportRenderer(typeof(TransparentButton), typeof(TransparentButtonRenderer))]
namespace RoomInfoRemote.Droid.Renderer
{
    //Renderer zum Erstellen eines Transparenten Buttons
    public class TransparentButtonRenderer : ButtonRenderer
    {
        public TransparentButtonRenderer(Context context) : base(context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
            Control.SetHighlightColor(Android.Graphics.Color.Transparent);
        }
    }
}