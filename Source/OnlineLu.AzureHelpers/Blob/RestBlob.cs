using OnlineLU.HelperUtils.MessageResponse;
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

namespace OnlineLU.AzureHelpers.Blob
{
    public static class AzureStorageConstants
    {
        public static string Account = "";
        public static string BlobEndPoint = "";
        public static string QueueEndPoint = "";
        public static string SharedKeyAuthorizationScheme = "SharedKey";
        //public static string StorageAccountKey = "";
        public static string StorageAccountKey = "";
        
    }

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
                String authorizationHeader = CreateAuthorizationHeader(stringToSign);

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

                response = CreateRESTRequestNew("PUT", blobInfo.ContainerName + "/" + blobInfo.BlobName, blobInfo.BlobByteSource, headers).GetResponse() as HttpWebResponse;
                
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
                response = CreateRESTRequest("GET", blobInfo.ContainerName + "/" + blobInfo.BlobName).GetResponse() as HttpWebResponse;

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

        public static String CreateAuthorizationHeader(String canonicalizedString)
        {
            String signature = String.Empty;

            using (HMACSHA256 hmacSha256 = new HMACSHA256(Convert.FromBase64String(AzureStorageConstants.StorageAccountKey)))
            {
                Byte[] dataToHmac = System.Text.Encoding.UTF8.GetBytes(canonicalizedString);
                signature = Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
            }

            String authorizationHeader = String.Format(
                CultureInfo.InvariantCulture,
                "{0} {1}:{2}",
                AzureStorageConstants.SharedKeyAuthorizationScheme,
                AzureStorageConstants.Account,
                signature
            );

            return authorizationHeader;
        }

        public static HttpWebRequest CreateRESTRequestNew(string method, string resource, byte[] requestBody = null, SortedList<string, string> headers = null,
         string ifMatch = "", string md5 = "")
        {
            byte[] byteArray = null;
            DateTime now = DateTime.UtcNow;
            //string uri = Endpoint + resource;
            string uri = AzureStorageConstants.BlobEndPoint + resource;

            HttpWebRequest request = HttpWebRequest.Create(uri) as HttpWebRequest;
            request.Method = method;
            request.ContentLength = requestBody.Length; //0;
            request.Headers.Add("x-ms-date", now.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
            request.Headers.Add("x-ms-version", "2009-09-19");

            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            request.Headers.Add("Authorization", AuthorizationHeader(method, now, request, ifMatch, md5));

            //if (!String.IsNullOrEmpty(requestBody))
            {
                request.GetRequestStream().Write(requestBody, 0, requestBody.Length);
            }

            return request;
        }

        public static HttpWebRequest CreateRESTRequest(string method, string resource, string requestBody = null, SortedList<string, string> headers = null,
          string ifMatch = "", string md5 = "")
        {
            byte[] byteArray = null;
            DateTime now = DateTime.UtcNow;
            //string uri = Endpoint + resource;
            string uri = AzureStorageConstants.BlobEndPoint + resource;


            HttpWebRequest request = HttpWebRequest.Create(uri) as HttpWebRequest;
            request.Method = method;
            request.ContentLength = 0;
            request.Headers.Add("x-ms-date", now.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
            request.Headers.Add("x-ms-version", "2009-09-19");

            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (!String.IsNullOrEmpty(requestBody))
            {
                request.Headers.Add("Accept-Charset", "UTF-8");

                byteArray = Encoding.UTF8.GetBytes(requestBody);
                request.ContentLength = byteArray.Length;
            }

            request.Headers.Add("Authorization", AuthorizationHeader(method, now, request, ifMatch, md5));

            if (!String.IsNullOrEmpty(requestBody))
            {
                request.GetRequestStream().Write(byteArray, 0, byteArray.Length);
            }

            return request;
        }


        // Generate an authorization header.

        public static string AuthorizationHeader(string method, DateTime now, HttpWebRequest request, string ifMatch = "", string md5 = "")
        {
            string MessageSignature;

            //if (IsTableStorage)
            //{
            //    MessageSignature = String.Format("{0}\n\n{1}\n{2}\n{3}",
            //        method,
            //        "application/atom+xml",
            //        now.ToString("R", System.Globalization.CultureInfo.InvariantCulture),
            //        GetCanonicalizedResource(request.RequestUri, StorageAccount)
            //        );
            //}
            //else
            {
                MessageSignature = String.Format("{0}\n\n\n{1}\n{5}\n\n\n\n{2}\n\n\n\n{3}{4}",
                    method,
                    (method == "GET" || method == "HEAD") ? String.Empty : request.ContentLength.ToString(),
                    ifMatch,
                    GetCanonicalizedHeaders(request),
                    GetCanonicalizedResource(request.RequestUri, AzureStorageConstants.Account),
                    md5
                    );
            }
            byte[] SignatureBytes = System.Text.Encoding.UTF8.GetBytes(MessageSignature);
            System.Security.Cryptography.HMACSHA256 SHA256 = new System.Security.Cryptography.HMACSHA256(Convert.FromBase64String(AzureStorageConstants.StorageAccountKey));
            String AuthorizationHeader = "SharedKey " + AzureStorageConstants.Account + ":" + Convert.ToBase64String(SHA256.ComputeHash(SignatureBytes));
            return AuthorizationHeader;
        }

        // Get canonicalized headers.

        public static string GetCanonicalizedHeaders(HttpWebRequest request)
        {
            ArrayList headerNameList = new ArrayList();
            StringBuilder sb = new StringBuilder();
            foreach (string headerName in request.Headers.Keys)
            {
                if (headerName.ToLowerInvariant().StartsWith("x-ms-", StringComparison.Ordinal))
                {
                    headerNameList.Add(headerName.ToLowerInvariant());
                }
            }
            headerNameList.Sort();
            foreach (string headerName in headerNameList)
            {
                StringBuilder builder = new StringBuilder(headerName);
                string separator = ":";
                foreach (string headerValue in GetHeaderValues(request.Headers, headerName))
                {
                    string trimmedValue = headerValue.Replace("\r\n", String.Empty);
                    builder.Append(separator);
                    builder.Append(trimmedValue);
                    separator = ",";
                }
                sb.Append(builder.ToString());
                sb.Append("\n");
            }
            return sb.ToString();
        }

        // Get header values.

        public static ArrayList GetHeaderValues(NameValueCollection headers, string headerName)
        {
            ArrayList list = new ArrayList();
            string[] values = headers.GetValues(headerName);
            if (values != null)
            {
                foreach (string str in values)
                {
                    list.Add(str.TrimStart(null));
                }
            }
            return list;
        }

        // Get canonicalized resource.

        public static string GetCanonicalizedResource(Uri address, string accountName)
        {
            StringBuilder str = new StringBuilder();
            StringBuilder builder = new StringBuilder("/");
            builder.Append(accountName);
            builder.Append(address.AbsolutePath);
            str.Append(builder.ToString());
            NameValueCollection values2 = new NameValueCollection();
            //if (!IsTableStorage)
            //{
            //    NameValueCollection values = HttpUtility.ParseQueryString(address.Query);
            //    foreach (string str2 in values.Keys)
            //    {
            //        ArrayList list = new ArrayList(values.GetValues(str2));
            //        list.Sort();
            //        StringBuilder builder2 = new StringBuilder();
            //        foreach (object obj2 in list)
            //        {
            //            if (builder2.Length > 0)
            //            {
            //                builder2.Append(",");
            //            }
            //            builder2.Append(obj2.ToString());
            //        }
            //        values2.Add((str2 == null) ? str2 : str2.ToLowerInvariant(), builder2.ToString());
            //    }
            //}
            ArrayList list2 = new ArrayList(values2.AllKeys);
            list2.Sort();
            foreach (string str3 in list2)
            {
                StringBuilder builder3 = new StringBuilder(string.Empty);
                builder3.Append(str3);
                builder3.Append(":");
                builder3.Append(values2[str3]);
                str.Append("\n");
                str.Append(builder3.ToString());
            }
            return str.ToString();
        }


        
    }
}
