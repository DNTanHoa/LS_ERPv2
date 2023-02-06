using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LS_ERP.Mobile.Custom;
using LS_ERP.Mobile.Droid.Render;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Java.Util.ResourceBundle;

[assembly: ExportRenderer(typeof(CustomFrame), typeof(CustomFrameRenderer))]
namespace LS_ERP.Mobile.Droid.Render
{
    [Obsolete]
    public class CustomFrameRenderer : FrameRenderer
    {
        public CustomFrameRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(CustomFrame.CornerRadius) ||
                e.PropertyName == nameof(CustomFrame))
            {
                UpdateCornerRadius();
            }
        }

        private void UpdateCornerRadius()
        {
            var cornerRadius = (Element as CustomFrame)?.CornerRadius;
            if (!cornerRadius.HasValue)
            {
                return;
            }

            var topLeftCorner = Context.ToPixels(cornerRadius.Value.TopLeft);
            var topRightCorner = Context.ToPixels(cornerRadius.Value.TopRight);
            var bottomLeftCorner = Context.ToPixels(cornerRadius.Value.BottomLeft);
            var bottomRightCorner = Context.ToPixels(cornerRadius.Value.BottomRight);

            var cornerRadii = new[]
            {
                    topLeftCorner,
                    topLeftCorner,

                    topRightCorner,
                    topRightCorner,

                    bottomRightCorner,
                    bottomRightCorner,

                    bottomLeftCorner,
                    bottomLeftCorner,
                };
        }
    }
}