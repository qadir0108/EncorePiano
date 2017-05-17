using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Amazon;
using Amazon.S3;
using Amazon.S3.IO;
using Amazon.S3.Model;
using Amazon.Util;

namespace WFP.ICT.S3
{
    public class S3FileManager
    {
        static string bucket = "adsdatadirect";

        public static void Upload(string fileKey, string localFilePath)
        {
            using (IAmazonS3 client = new AmazonS3Client(RegionEndpoint.USWest2))
            {
                PutObjectRequest request = new PutObjectRequest();
                request.BucketName = bucket;
                request.Key = fileKey;
                request.FilePath = localFilePath;
                client.PutObject(request);
            }
        }

        public static void Download(string fileKey, string localFilePath)
        {
            using (IAmazonS3 client = new AmazonS3Client(RegionEndpoint.USWest2))
            {
                GetObjectRequest request = new GetObjectRequest();
                request.BucketName = bucket;
                request.Key = fileKey;
                GetObjectResponse response = client.GetObject(request);
                response.WriteResponseStreamToFile(localFilePath);
            }
        }

        public static void Delete(string fileKey)
        {
            using (IAmazonS3 client = new AmazonS3Client(RegionEndpoint.USWest2))
            {
                DeleteObjectRequest request = new DeleteObjectRequest();
                request.BucketName = bucket;
                request.Key = fileKey;
                client.DeleteObject(request);
            }
        }

        public static void Move(string oldFileKey,string newFileKey, string folderName)
        {
            if(string.IsNullOrEmpty(oldFileKey)) return;

            using (IAmazonS3 client = new AmazonS3Client(RegionEndpoint.USWest2))
            {
                S3FileInfo currentObject = new S3FileInfo(client, bucket, oldFileKey);
                S3DirectoryInfo destination = new S3DirectoryInfo(client, bucket, folderName);
                if(!destination.Exists) destination.Create();
                S3FileInfo movedObject = currentObject.MoveTo(bucket, newFileKey);
            }
        }
    }
}
