using ADSDataDirect.Web.Helpers;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WFP.ICT.Data.Entities;
using System.Data.Entity;

namespace WFP.ICT.Web.Async
{
    public class PODFormsGenerater
    {
        public static void GeneratePODForm(WFPICTContext Db, string UploadsFormsPath, string DownloadsFormsPath, string SignPath, Guid Id)
        {
            Proof pod = Db.Proofs
                                            .Include(x => x.Assignment)
                                            .Include(x => x.Assignment.Order)
                                            .Include(x => x.Pictures)
                                            .Include(x => x.Piano)
                                            .Include(x => x.Piano.Statuses)
                                            .FirstOrDefault(x => x.Id == Id)
                                            ;

            string templateFormsPath = Path.Combine(UploadsFormsPath, pod.Assignment.Order.DeliveryForm);
            string podFileName = "POD" + pod.Assignment.Order.OrderNumber + ".pdf";
            string podFilePath = Path.Combine(DownloadsFormsPath, pod.Id.ToString() + ".pdf");

            string signature = Path.Combine(SignPath, pod.Signature);
            string signatureResized = Path.Combine(SignPath, "1" + pod.Signature);

            if (!System.IO.File.Exists(signatureResized))
                ImageResizer.Resize(signature, signatureResized, 150, 150, true);

            GeneratePODForm(templateFormsPath, podFilePath, pod.ReceivedBy, pod.ReceivingTime?.ToString(), signatureResized);

            EmailHelper.SendEmail(pod.Assignment.Order.CallerEmail, "Signed POD Form "+ podFileName, "Please find attached Signed Delivery Form " + podFileName, "", new string[] { podFilePath }.ToList() );
        }

        public static void GeneratePODForm(string templateFormsPath, string podFilePath, string receivedBy, string receivingTime, string signature)
        {
            PdfReader reader = new PdfReader(templateFormsPath);
            //select three pages from the original document
            reader.SelectPages("1-3");
            //create PdfStamper object to write to get the pages from reader 
            PdfStamper stamper = new PdfStamper(reader, new FileStream(podFilePath, FileMode.Create));

            Rectangle pagesize = reader.GetPageSize(1);

            // PdfContentByte from stamper to add content to the pages over the original content
            PdfContentByte pbover = stamper.GetOverContent(1);
            PdfContentByte pbunder = stamper.GetUnderContent(1);

            //add content to the page using ColumnText
            ColumnText.ShowTextAligned(pbover, Element.ALIGN_LEFT, new Phrase(receivedBy), pagesize.Left + 320, pagesize.Bottom + 90, 0);
            ColumnText.ShowTextAligned(pbover, Element.ALIGN_LEFT, new Phrase(receivingTime), pagesize.Left + 400, pagesize.Bottom + 60, 0);

            // PdfContentByte from stamper to add content to the pages under the original content
            //add image from a file 
            iTextSharp.text.Image img = new Jpeg(imageToByteArray(System.Drawing.Image.FromFile(signature)));
            //add the image under the original content
            img.SetAbsolutePosition(pagesize.Left + 380 , pagesize.Bottom + 10);
            pbunder.AddImage(img);

            //pbunder.AddImage(img, img.Width / 2, 0, 0, img.Height / 2, 0, 0);

            //close the stamper
            stamper.Close();

        }

        //this method will be called to convert an image object to byte array
        public static byte[] imageToByteArray(System.Drawing.Image imageori)
        {
            using (var ms = new MemoryStream())
            {
                imageori.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }
    }
}