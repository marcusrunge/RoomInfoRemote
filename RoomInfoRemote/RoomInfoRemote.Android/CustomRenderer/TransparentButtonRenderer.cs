﻿using Android.Content;
using Android.Views;
using RoomInfoRemote.Customs;
using RoomInfoRemote.Droid.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TransparentButton), typeof(TransparentButtonRenderer))]
namespace RoomInfoRemote.Droid.Renderer
{
    //Renderer zum Erstellen eines Transparenten Buttons
    public class TransparentButtonRenderer : ButtonRenderer
    {
        public TransparentButtonRenderer(Context context) : base(context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
            Control.SetHighlightColor(Android.Graphics.Color.Transparent);
            if (e.NewElement.StyleId.Equals("refreshButton")) e.NewElement.Clicked += (sender, eventArgs) =>
            {
                MainActivity.DecorView.PerformHapticFeedback(FeedbackConstants.VirtualKey, FeedbackFlags.IgnoreGlobalSetting);
            };
        }
    }
}