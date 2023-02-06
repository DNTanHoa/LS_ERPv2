using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LS_ERP.Mobile.Custom
{
    public class CustomEntry : Entry
    {
        public bool HasUnderline { get; set; }
        public string UnderlineColor { get; set; }
        public string CursorColor { get; set; }
    }
}
