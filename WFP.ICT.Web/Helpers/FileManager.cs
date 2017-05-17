using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using WFP.ICT.Enum;
using WFP.ICT.Web.Models;

namespace WFP.ICT.Web.Helpers
{
    public class FileManager
    {
        static string OptOut = @"<center><p style='text-align:center;'><br><br><span style='font-size:12px;'>[~Physical_Mailing_Address~]<br>[~Optout style = ""~]Opt Out[~EndOptout~]</span></p></center>";
        static string ViewInBrowser = @"<center>[~ViewInBrowser style=""~]If you cannot view this email  please click here.[~EndViewInBrowser~]</center>";
        private static string _orderNumber;
        private static bool _isAddOptout;
        private static bool _isAddViewinBrowser;

        public static HtmlProcessResult ProcessHtmlZip(string tempFolder, string zipFilePath, string orderNumber, bool isAddOptout, bool isAddViewinBrowser)
        {
            _orderNumber = orderNumber;
            _isAddOptout = isAddOptout;
            _isAddViewinBrowser = isAddViewinBrowser;
            FileUploader uploader = new FileUploader();
            
            // Unzip
            string temp = string.Format("{0}{1}", tempFolder, orderNumber);
            if (Directory.Exists(temp))
            {
                new DirectoryInfo(temp).Delete(true);
            }
            else
            {
                Directory.CreateDirectory(temp);
            }
            ZipFile.ExtractToDirectory(zipFilePath, temp);

            // Change html
            string htmlFileName = string.Format("{0}.htm", orderNumber);
            string htmlFilePath = string.Format("{0}\\{1}", temp, htmlFileName);

            string htmlFile = Directory.EnumerateFiles(temp).FirstOrDefault(x => x.EndsWith("htm") || x.EndsWith("html"));
            if(string.IsNullOrEmpty(htmlFile))
                throw new Exception("Html not found");

            UploadFileStatusEnum status = ProcessHtml(htmlFile, htmlFilePath);

            // Create 2501 directory
            uploader.CreateDirectory(orderNumber);

            // Upload html
            string filePathLive = uploader.Upload(orderNumber, htmlFileName, htmlFilePath);

            // Create images directory
            var imagesLive = string.Format("{0}/{0}img", orderNumber);
            uploader.CreateDirectory(imagesLive);

            // Upload Images
            var images = Directory.EnumerateDirectories(temp).FirstOrDefault();
            foreach (var imgFile in Directory.EnumerateFiles(images))
            {
                var file = new FileInfo(imgFile);
                uploader.Upload(imagesLive, file.Name, imgFile);
            }

            // Delete tmp
            new DirectoryInfo(temp).Delete(true);

            return new HtmlProcessResult() {Status = status, filePathLive = filePathLive};
        }

        public static UploadFileStatusEnum ProcessHtml(string htmlFile, string outputFilePath)
        {
            string imagesPath = string.Format("http://www.digitaldynamixs.net/ep2/{0}/{0}img/", _orderNumber);

            HtmlDocument doc = new HtmlDocument();
            doc.Load(htmlFile);
            var allSrcs = doc.DocumentNode.SelectNodes("//img[@src]");
            bool alreadyHosted = false;
            foreach (HtmlNode node in allSrcs)
            {
                var src = node.Attributes["src"].Value;
                // Already hosted : look for http
                if (src.Contains("http://"))
                {
                    alreadyHosted = true;
                    break;;
                }
                else
                {
                    // imagesPath
                    // images/GregMayHonda4_13_r1_c1.jpg
                    var srcs = src.Split("/".ToCharArray());
                    string imgFileName = srcs.Length > 1 ? srcs[1] : string.Empty;
                    src = string.Format("{0}{1}", imagesPath, imgFileName);
                }
                node.SetAttributeValue("src", src);
            }

            HtmlNode body = doc.DocumentNode.SelectSingleNode("//body");
            if (_isAddViewinBrowser)
            {
                HtmlNode newNode = HtmlNode.CreateNode(ViewInBrowser);
                body.PrependChild(newNode);
            }
            if (_isAddOptout)
            {
                HtmlNode newNode = HtmlNode.CreateNode(OptOut);
                body.AppendChild(newNode);
            }

            doc.Save(outputFilePath);

            return alreadyHosted ? UploadFileStatusEnum.HostedWithOutImages :  UploadFileStatusEnum.HostedWithImages;
        }

        public static string UploadFile(UploadFileTypeEnum uploadFileType, string filePath, string orderNumber)
        {
            FileUploader uploader = new FileUploader();

            // Create 2501 directory
            uploader.CreateDirectory(orderNumber);

            string fileName = "";
            switch (uploadFileType)
            {
                    case UploadFileTypeEnum.ZipFile:
                        fileName = string.Format("{0}zip.csv", orderNumber);
                        break;
                    case UploadFileTypeEnum.TestSeedFile:
                        fileName = string.Format("{0}test.csv", orderNumber);
                    break;
                    case UploadFileTypeEnum.LiveSeedFile:
                        fileName = string.Format("{0}live.csv", orderNumber);
                    break;
                    case UploadFileTypeEnum.SuppressionFile:
                        fileName = string.Format("{0}supp.csv", orderNumber);
                    break;
            }
            return uploader.Upload(orderNumber, fileName, filePath);
        }
    }
}