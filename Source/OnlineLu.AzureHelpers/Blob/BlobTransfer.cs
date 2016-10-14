using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using StreamingGenerator.AzureServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineLU.AzureHelpers.Blob
{
    public class BlobTransfer : IDisposable
    {
        private static CloudStorageAccount AccountFileTransfer;
        private static CloudBlobClient BlobClientFileTransfer;
        private static CloudBlobContainer ContainerFileTransfer;
        private BlobTransferServiceAsync transferUpload;

        private static bool Transferring;

        public BlobTransfer(string ContainerName)
        {
            AccountFileTransfer = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            if (AccountFileTransfer != null)
            {
                BlobClientFileTransfer = AccountFileTransfer.CreateCloudBlobClient();
                ContainerFileTransfer = BlobClientFileTransfer.GetContainerReference(ContainerName);
                ContainerFileTransfer.CreateIfNotExists();
            }
            transferUpload = new BlobTransferServiceAsync();
        }


        public void UploadBlobAsync(BlobInfo blobInfo)
        {
            // Upload the file
            ICloudBlob blobUpload = ContainerFileTransfer.GetBlockBlobReference(blobInfo.BlobName);

            transferUpload.TransferProgressChanged += new EventHandler<BlobTransferServiceAsync.BlobTransferProgressChangedEventArgs>(transfer_TransferProgressChanged);
            transferUpload.TransferCompleted += new System.ComponentModel.AsyncCompletedEventHandler(transfer_TransferCompleted);
            transferUpload.UploadBlobAsync(blobUpload, blobInfo.BlobByteSource, blobInfo.BlobName);

            Transferring = true;
            while (Transferring)
            {
                Console.ReadLine();
            }
        }

        private void transfer_TransferCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Transferring = false;
            transferUpload = null;
            AccountFileTransfer = null;
            BlobClientFileTransfer = null;
            ContainerFileTransfer = null;
            Console.WriteLine("Transfer completed. ");
        }

        private void transfer_TransferProgressChanged(object sender, BlobTransferServiceAsync.BlobTransferProgressChangedEventArgs e)
        {
            Console.WriteLine("Transfer progress percentage = " + e.ProgressPercentage + " - " + (e.Speed / 1024).ToString("N2") + "KB/s");
        }

        public void Dispose()
        {
        
        }
    }
}
