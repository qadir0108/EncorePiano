using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NSERReceipts;
using WFP.ICT.Data.Entities;
using WFP.ICT.Web.Models;

namespace WFP.ICT.Web.Async
{
    public class QRCodeGenerator
    {
        public static string Generate(string ImagesPath, string UploadPath, List<OrderVm> orderVms)
        {
            string _directoryPath = UploadPath + "\\codes"; 
            foreach (var orderVm in orderVms)
            {
                string code = orderVm.PickupTicket;
                ZingCoder.GenerateQrCode(_directoryPath, code);
            }
            //codes.Add("0526304465172411000");

            var pdf = new PdfSharpp();
            return pdf.Generate(ImagesPath, _directoryPath, orderVms);

            // Batching
            //double batchSize = 25000.0;
            //int batchSizeI = (int)batchSize;
            //int batches = (int)Math.Ceiling(orderVms.Count / batchSize);
            //for (int i = 0; i < batches; i++)
            //{
            //    int skip = batchSizeI * i;
            //    List<OrderVm> batch = orderVms.Skip(skip).Take(batchSizeI).ToList();
            //    pdf.Generate(ImagesPath, _directoryPath, batch);
            //}

        }
    }
}