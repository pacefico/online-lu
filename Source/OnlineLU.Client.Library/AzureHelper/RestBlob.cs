using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace OnlineLU.Client.Library.AzureHelper
{
    public static class RestBlob
    {
        public static bool PutBlobOLD(BlobInfo blobInfo)
        {
            try
            {
                String requestMethod = "PUT";
                String urlPath = String.Format("{0}/{1}", blobInfo.ContainerName, blobInfo.BlobName);
                String storageServiceVersion = "2012-02-12";
                String dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
                Int32 blobLength = blobInfo.BlobByteSource.Length;
                const String blobType = "BlockBlob";

                String canonicalizedHeaders = String.Format(
                        "x-ms-blob-type:{0}\nx-ms-date:{1}\nx-ms-version:{2}",
                        blobType,
                        dateInRfc1123Format,
                        storageServiceVersion);
                String canonicalizedResource = String.Format("/{0}/{1}", AzureStorageConstants.Account, urlPath);
                String stringToSign = String.Format(
                        "{0}\n\n\n{1}\n\n\n\n\n\n\n\n\n{2}\n{3}",
                        requestMethod,
                        blobLength,
                        canonicalizedHeaders,
                        canonicalizedResource);
                String authorizationHeader = "";// RestHelper.CreateAuthorizationHeader(stringToSign);

                Uri uri = new Uri(AzureStorageConstants.BlobEndPoint + urlPath);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Timeout = 400000;
                request.Method = requestMethod;
                request.Headers.Add("x-ms-blob-type", blobType);
                request.Headers.Add("x-ms-date", dateInRfc1123Format);
                request.Headers.Add("x-ms-version", storageServiceVersion);
                request.Headers.Add("Authorization", authorizationHeader);
                request.ContentLength = blobLength;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(blobInfo.BlobByteSource, 0, blobLength);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    String ETag = response.Headers["ETag"];
                    if (response.StatusCode == HttpStatusCode.Created)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                
                return false;
            }
        }

        public static bool PutBlob(BlobInfo blobInfo)
        {
            HttpWebResponse response;

            try
            {
                SortedList<string, string> headers = new SortedList<string, string>();
                headers.Add("x-ms-blob-type", "BlockBlob");

                response = null;// RestHelper.CreateRESTRequestNew("PUT", blobInfo.ContainerName + "/" + blobInfo.BlobName, blobInfo.BlobByteSource, headers).GetResponse() as HttpWebResponse;
                
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    response.Close();
                    return true;
                }
                response.Close();
                return false;
            }
            catch (WebException ex)
            {
                return false;
                //if (ex.Status == WebExceptionStatus.ProtocolError &&
                //    ex.Response != null &&
                //    (int)(ex.Response as HttpWebResponse).StatusCode == 409)
                //    return false;

                //throw;
            }
        
        }

        public static bool GetBlob(BlobInfo blobInfo)
        {
            HttpWebResponse response;

            try
            {
                response = null; //RestHelper.CreateRESTRequest("GET", blobInfo.ContainerName + "/" + blobInfo.BlobName).GetResponse() as HttpWebResponse;

                using (var reader = response.GetResponseStream())
                {
                    blobInfo.BlobStreamSource = new MemoryStream();
                    reader.CopyTo(blobInfo.BlobStreamSource);
                }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    response.Close();
                    return true;
                }
                response.Close();
                return false;
            }
            catch (WebException ex)
            {
                return false;
                //if (ex.Status == WebExceptionStatus.ProtocolError &&
                //    ex.Response != null &&
                //    (int)(ex.Response as HttpWebResponse).StatusCode == 409)
                //    //return null;

                //throw;
            }

            return true;
        }

       

        
    }
}
