using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace LS_ERP.Mobile.Views
{
    public partial class CuttingCardPage : ContentPage
    {
        public CuttingCardPage()
        {
            DevExpress.XamarinForms.Editors.Initializer.Init();
            InitializeComponent();
            
        }

        public List<string> States { get; }

      
    }
}
