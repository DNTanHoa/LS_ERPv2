using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ZXing.Mobile;



namespace LS_ERP.Mobile.Services
{
    public class QrScanningService : IQrScanningService
    {
        public async Task<string> ScanAsync()
        {
            var optionsDefault = new MobileBarcodeScanningOptions();
            var optionsCustom = new MobileBarcodeScanningOptions();
            //optionsCustom.UseFrontCameraIfAvailable = true;
            
            var scanner = new MobileBarcodeScanner()
            {
                TopText = "Scan the QR Code",
                BottomText = "Please Wait",
                CancelButtonText = "Cancel"
               
            };

            var scanResult = await scanner.Scan(optionsCustom);
            if(scanResult != null)
            {
                return scanResult.Text;
            }   
            else
            {
                return null;
            }    
            
        }
    }
}
