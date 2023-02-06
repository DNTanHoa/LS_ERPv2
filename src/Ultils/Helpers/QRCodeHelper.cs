

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.QrCode;

namespace Ultils.Helpers
{
    public static class QRCodeHelper
    {
        
        public static string GenerateQRCodeBase64WithMargin(string value)
        {
           
            var writer = new QRCodeWriter();          
            var resultBit = writer.encode(value, BarcodeFormat.QR_CODE, 100, 100);            
            var matrix = resultBit;        
            int scale = 2;
            Bitmap qrcode = new Bitmap(matrix.Width * scale, matrix.Height * scale);
            for (int x = 0; x < matrix.Height; x++)
                for (int y = 0; y < matrix.Width; y++)
                {
                    Color pixel = matrix[x, y] ? Color.Black : Color.White;
                    for (int i = 0; i < scale; i++)
                        for (int j = 0; j < scale; j++)
                            qrcode.SetPixel(x * scale + i, y * scale + j, pixel);
                }

            string base64String = "";
            using (MemoryStream ms = new MemoryStream())
            {                  
                qrcode.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                byte[] imageBytes = ms.ToArray();               
                base64String = Convert.ToBase64String(imageBytes);
            }
            return base64String;
        }
        public static string GenerateQRCodeBase64(string value)
        {
            var base64 = "";
            var QrcodeContent = value;           
            var width = 200; // width of the Qr Code   
            var height = 200; // height of the Qr Code   
            var margin = 0;
            var qrCodeWriter = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = margin
                }
            };
            var pixelData = qrCodeWriter.Write(QrcodeContent);           
            using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height
                , System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            using (var ms = new MemoryStream())
            {
                var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height)
                    , System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                try
                {                       
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }               
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                base64 = Convert.ToBase64String(ms.ToArray());
            }
            return base64;
        }
    }
}
