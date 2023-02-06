using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LS_ERP.Mobile.Custom;
using LS_ERP.Mobile.Droid.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace LS_ERP.Mobile.Droid.Render
{
    public class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var element = Element as CustomEntry;

                if (element != null)
                {
                    if (!element.HasUnderline)
                    {
                        Control.Background = null;
                        Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
                    }
                }
            }
        }
    }
}