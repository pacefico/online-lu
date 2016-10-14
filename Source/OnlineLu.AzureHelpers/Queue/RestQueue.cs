using OnlineLU.AzureHelpers.Blob;
using OnlineLU.HelperUtils.ZipManager;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace OnlineLU.AzureHelpers.Queue
{
    public static class AzureStorageConstants
    {
        public static string Account = "";
        public static string BlobEndPoint = "";
        public static string QueueEndPoint = "";
        public static string SharedKeyAuthorizationScheme = "";
        public static string StorageAccountKey = "";
    }

    public class RestQueue
    {
        public static bool PutMessage(QueueInfoMessage queueInfo)
        {
            try
            {
                String requestMethod = "POST";

                String urlPath = String.Format("{0}/messages", queueInfo.QueueName);

                String storageServiceVersion = "2012-02-12";
                String dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

                String messageText = String.Format(
                        "<QueueMessage><MessageText>{0}</MessageText></QueueMessage>", queueInfo.SourceMessage);
                UTF8Encoding utf8Encoding = new UTF8Encoding();
                Byte[] messageContent = ZipManager.CompressStringToByte(messageText);  // utf8Encoding.GetBytes(messageText);
                Int32 messageLength = messageContent.Length;

                String canonicalizedHeaders = String.Format(
                        "x-ms-date:{0}\nx-ms-version:{1}",
                        dateInRfc1123Format,
                        storageServiceVersion);
                String canonicalizedResource = String.Format("/{0}/{1}", AzureStorageConstants.Account, urlPath);
                String stringToSign = String.Format(
                        "{0}\n\n\n{1}\n\n\n\n\n\n\n\n\n{2}\n{3}",
                        requestMethod,
                        messageLength,
                        canonicalizedHeaders,
                        canonicalizedResource);
                String authorizationHeader = CreateAuthorizationHeader(stringToSign);

                Uri uri = new Uri(AzureStorageConstants.QueueEndPoint + urlPath);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = requestMethod;
                request.Headers.Add("x-ms-date", dateInRfc1123Format);
                request.Headers.Add("x-ms-version", storageServiceVersion);
                request.Headers.Add("Authorization", authorizationHeader);
                request.ContentLength = messageLength;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(messageContent, 0, messageLength);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    String requestId = response.Headers["x-ms-request-id"];
                    if (response.StatusCode == HttpStatusCode.Created)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
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


        public static bool PutQueue(BlobInfo blobInfo)
        {
            try
            {
                //var _queueUrl = ConfigurationManager.AppSettings["AzureQueueAddress"];
                var _queueName = "processing";
                var _azureStorageAccount = AzureStorageConstants.Account;

                var _storageServiceVersion = "2011-08-18";

                var _urlPath = _queueName; //+ "/messages";

                var _dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

                //var _messageInBytes = Encoding.UTF8.GetBytes(message);
                //var _base64Message = Convert.ToBase64String(_messageInBytes.ToArray());
                //var _messageText = String.Format("<QueueMessage><MessageText>{0}</MessageText></QueueMessage>", _base64Message);
                //var _byteMessage = Encoding.UTF8.GetBytes(_messageText);

                var _messageLength = blobInfo.BlobByteSource.Length;

                var _canonicalizedHeaders = String.Format(
                      "x-ms-date:{0}\nx-ms-version:{1}",
                      _dateInRfc1123Format,
                      _storageServiceVersion);

                var _canonicalizedResource = String.Format("/{0}/{1}", _azureStorageAccount, _urlPath);

                var _stringToSign = String.Format(
                      "{0}\n\n\n{1}\n\n\n\n\n\n\n\n\n{2}\n{3}",
                      "POST",
                      _messageLength,
                      _canonicalizedHeaders,
                      _canonicalizedResource);

                var _authorizationHeader = "";//createAuthorizationHeader(_stringToSign);

                var _uri = new Uri(AzureStorageConstants.QueueEndPoint + "/" + _urlPath);
                var _httpWebRequest = (HttpWebRequest)WebRequest.Create(_uri);
                _httpWebRequest.Method = "POST";
                _httpWebRequest.Headers.Add("x-ms-date", _dateInRfc1123Format);
                _httpWebRequest.Headers.Add("x-ms-version", _storageServiceVersion);
                _httpWebRequest.Headers.Add("Authorization", _authorizationHeader);
                _httpWebRequest.ContentLength = _messageLength;

                using (var _requestStream = _httpWebRequest.GetRequestStream())
                {
                    _requestStream.Write(blobInfo.BlobByteSource, 0, blobInfo.BlobByteSource.Length);
                    _requestStream.Flush();
                    _requestStream.Dispose();
                }

                using (var _response = (HttpWebResponse)_httpWebRequest.GetResponse())
                {
                    if (_response.StatusCode == HttpStatusCode.Created)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        

    }
}
