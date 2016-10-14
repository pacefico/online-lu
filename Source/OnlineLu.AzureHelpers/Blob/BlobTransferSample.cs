using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace StreamingGenerator.AzureServices
{
    public class BlobTransferSample
    {
            const string ACCOUNTNAME = "ENTER ACCOUNT NAME";
            const string ACCOUNTKEY = "ENTER ACCOUNT KEY";
            const string LOCALFILE = @"ENTER LOCAL FILE";
            const string CONTAINER = "temp";

            private static CloudStorageAccount AccountFileTransfer;
            private static CloudBlobClient BlobClientFileTransfer;
            private static CloudBlobContainer ContainerFileTransfer;

            private static bool Transferring;

            public BlobTransferSample(string[] args)
            {
                System.Net.ServicePointManager.DefaultConnectionLimit = 35;

                /*
                AccountFileTransfer = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=" + ACCOUNTNAME + ";AccountKey=" + ACCOUNTKEY);
                if (AccountFileTransfer != null)
                {
                    BlobClientFileTransfer = AccountFileTransfer.CreateCloudBlobClient();
                    ContainerFileTransfer = BlobClientFileTransfer.GetContainerReference(CONTAINER);
                    ContainerFileTransfer.CreateIfNotExist();
                }

                // Upload the file
                ICloudBlob blobUpload = ContainerFileTransfer.GetBlobReference(CONTAINER + "/" + System.IO.Path.GetFileName(LOCALFILE));
                BlobTransfer transferUpload = new BlobTransfer();
                transferUpload.TransferProgressChanged += new EventHandler<BlobTransfer.BlobTransferProgressChangedEventArgs>(transfer_TransferProgressChanged);
                transferUpload.TransferCompleted += new System.ComponentModel.AsyncCompletedEventHandler(transfer_TransferCompleted);
                transferUpload.UploadBlobAsync(blobUpload, LOCALFILE);

                Transferring = true;
                while (Transferring)
                {
                    Console.ReadLine();
                }

                // Download the file
                CloudBlob blobDownload = ContainerFileTransfer.GetBlobReference(CONTAINER + "/" + System.IO.Path.GetFileName(LOCALFILE));
                BlobTransfer transferDownload = new BlobTransfer();
                transferDownload.TransferProgressChanged += new EventHandler<BlobTransfer.BlobTransferProgressChangedEventArgs>(transfer_TransferProgressChanged);
                transferDownload.TransferCompleted += new System.ComponentModel.AsyncCompletedEventHandler(transfer_TransferCompleted);
                transferDownload.DownloadBlobAsync(blobDownload, LOCALFILE + ".copy");

                Transferring = true;
                while (Transferring)
                {
                    Console.ReadLine();
                }
                 * */
            }

            static void transfer_TransferCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
            {
                Transferring = false;
                Console.WriteLine("Transfer completed. Press any key to continue.");
            }

            static void transfer_TransferProgressChanged(object sender, BlobTransferServiceAsync.BlobTransferProgressChangedEventArgs e)
            {
                Console.WriteLine("Transfer progress percentage = " + e.ProgressPercentage + " - " + (e.Speed / 1024).ToString("N2") + "KB/s");
            }
        


    }
}
