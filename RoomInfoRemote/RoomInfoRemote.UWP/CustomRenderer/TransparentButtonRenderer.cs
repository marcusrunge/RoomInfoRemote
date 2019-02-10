using RoomInfoRemote.Customs;
using RoomInfoRemote.UWP.CustomRenderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(TransparentButton), typeof(TransparentButtonRenderer))]
namespace RoomInfoRemote.UWP.CustomRenderer
{
    public class TransparentButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.Resources.Source = new Uri("ms-appx:///Dictionaries/UIElementsDictionary.xaml");
                Control.Style = Control.Resources["TransparentButtonStyle"] as Windows.UI.Xaml.Style;
            }
        }
    }
}
