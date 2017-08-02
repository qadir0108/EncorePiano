using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace NSERReceipts
{
    public class ZingCoder
    {
        public static void GenerateQrCode(string _directoryPath, string code)
        {
            if (!Directory.Exists(_directoryPath))
                Directory.CreateDirectory(_directoryPath);

            var filePathFinal = string.Format("{0}\\{1}.png", _directoryPath, code);

            var qrcode = new QRCodeWriter();

            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 200,
                    Width = 200,
                    Margin = 1
                }
            };

            using (var bitmap = barcodeWriter.Write(code))
            {
                bitmap.Save(filePathFinal);
            }
        }

        public static string GenerateForInvoice(string _directoryPath, string code)
        {
            if (!Directory.Exists(_directoryPath))
                Directory.CreateDirectory(_directoryPath);

            var filePathFinal = string.Format("{0}\\{1}.png", _directoryPath, code);

            var qrcode = new QRCodeWriter();

            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 100,
                    Width = 100,
                    Margin = 0
                }
            };

            using (var bitmap = barcodeWriter.Write(code))
            {
                bitmap.Save(filePathFinal);
            }
            return filePathFinal;
        }
    }
}
