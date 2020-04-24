using RoomInfoRemote.Customs;
using RoomInfoRemote.iOS.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TransparentButton), typeof(TransparentButtonRenderer))]
namespace RoomInfoRemote.iOS.Renderer
{
    public class TransparentButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            Control.BackgroundColor = UIKit.UIColor.Clear;
            Control.TintColor = UIKit.UIColor.Clear;
        }
    }
}
