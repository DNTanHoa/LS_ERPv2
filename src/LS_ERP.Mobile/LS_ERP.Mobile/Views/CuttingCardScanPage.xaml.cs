
using DevExpress.XamarinForms.Editors;
using DevExpress.XamarinForms.Editors.Themes;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace LS_ERP.Mobile.Views
{
    public partial class CuttingCardScanPage : ContentPage
    {
        public CuttingCardScanPage()
        {
            InitializeComponent();
            
            scanner.OnScanResult += (result) => Device.BeginInvokeOnMainThread(() => {              
                qrCodeEntry.Text = result.Text;            
            });
            
        }

        

        protected override void OnAppearing()
        {
            base.OnAppearing();         
            scanner.IsScanning = true;         
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
           
        }
        
    }
}
