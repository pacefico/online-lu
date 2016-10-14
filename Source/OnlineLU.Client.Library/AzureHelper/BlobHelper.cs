using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace OnlineLU.Client.Library.AzureHelper
{
    public class BlobHelper : RESTHelper
    {
        public BlobHelper(string storageAccount, string storageKey)
            : base("http://" + storageAccount + ".blob.core.windows.net/", storageAccount, storageKey)
        {
        }

        public bool PutBlob(BlobInfo blobInfo)
        {
            HttpWebResponse response;

            try
            {
                if (blobInfo.BlobByteSource != null && blobInfo.BlobByteSource.Length > 0)
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
                }
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

        public bool GetBlob(BlobInfo blobInfo)
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

    }
}
