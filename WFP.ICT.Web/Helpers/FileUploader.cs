using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WFP.ICT.Web.Helpers
{
    public class FileUploader
    {
        private bool _uploadCompleted;
        private bool _status;

        private string _ftpServer;
        private string _ftpUsername;
        private string _ftpPassword;
        private string _baseURL;

        public FileUploader()
        {
            if (ConfigurationManager.AppSettings["FTPServer"] == null)
                throw new Exception("FTPServer not configured");
            if (ConfigurationManager.AppSettings["FTPUsername"] == null)
                throw new Exception("FTPUsername not configured");
            if (ConfigurationManager.AppSettings["FTPPassword"] == null)
                throw new Exception("FTPPassword not configured");

            _ftpServer = ConfigurationManager.AppSettings["FTPServer"];
            _ftpUsername = ConfigurationManager.AppSettings["FTPUsername"];
            _ftpPassword = ConfigurationManager.AppSettings["FTPPassword"];
            _baseURL = string.Format("ftp://{0}/public_html/ep2/", _ftpServer);


        }

        private void FileUploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {

            _status = (e.Cancelled || e.Error == null) ? false : true;
            _uploadCompleted = true;

            if (e.Error == null)
            {
                //_InputFile.Status = FileStatus.Uploaded;
            }
            else
            {
                //_InputFile.Status = FileStatus.Failed;
                //_InputFile.Error = e.Error;
            }
        }

        private void FileUploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            //if (e.ProgressPercentage % 10 == 0)
            {
                //This writes the pecentage data uploaded and downloaded
                Console.WriteLine("Send: {0}, Received: {1}", e.BytesSent, e.BytesReceived);
                //You can have a delegate or a call back to update your UI about the percentage uploaded
                //If you don't have the condition (i.e e.ProgressPercentage % 10 == 0 )for the pecentage of the process 
                //the callback will slow you upload process down
            }
        }

        public bool DirectoryExists(string directoryPath)
        {
            bool directoryExists;
            var request = (FtpWebRequest)WebRequest.Create(directoryPath + "/");
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
            try
            {
                using (request.GetResponse())
                {
                    directoryExists = true;
                }
            }
            catch (WebException ex)
            {
                directoryExists = false;
            }
            return directoryExists;
        }

        public void CreateDirectory(string directoryName)
        {
            string directoryPath = _baseURL + directoryName;

            if(DirectoryExists(directoryPath)) return;

            WebRequest request = WebRequest.Create(directoryPath);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
            try
            {
                var resp = (FtpWebResponse) request.GetResponse();
            }
            catch (Exception ex)
            {
            }
        }

        public string Upload(string directoryName, string fileName, string filePath)
        {
            using (WebClient _client = new WebClient())
            {
                _client.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
                _client.UploadProgressChanged += FileUploadProgressChanged;
                _client.UploadFileCompleted += FileUploadFileCompleted;
                string filePathLive = _baseURL + directoryName + "/" + fileName;
                Uri address = new Uri(filePathLive);
                _client.UploadFile(address, filePath);
                return filePathLive;
            }
        }
        
    }
}
