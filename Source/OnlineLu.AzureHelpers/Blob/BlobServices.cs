using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLU.AzureHelpers.Blob
{
    public class BlobServices
    {
        #region members

        private CloudBlobClient m_BlobClient = null;
        private CloudBlobContainer m_BlobContainer = null;
        private BlobContainerPermissions m_BlobPermissions = null;

        private OperationContext m_OperationContext = null;
        private BlobRequestOptions m_RequestOptions = null;

        #endregion members

        #region Singleton Instance Constructor
        private static BlobServices m_instance;
        private static bool m_Initialized;

        private BlobServices()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            m_BlobClient = storageAccount.CreateCloudBlobClient();
            m_Initialized = false;
        }

        public static BlobServices Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new BlobServices();
                }
                return m_instance;
            }
        }

        #endregion Singleton Instance Constructor

        #region public methods

        public void Initialize(string ContainerName)
        {
            if (!m_Initialized)
            {
                if (m_BlobContainer == null)
                {
                    m_BlobContainer = m_BlobClient.GetContainerReference(ContainerName);
                }

                if (m_BlobPermissions == null)
                {
                    m_BlobPermissions = new BlobContainerPermissions();
                    m_BlobPermissions.PublicAccess = BlobContainerPublicAccessType.Off;
                }

                if (m_BlobContainer.CreateIfNotExists())
                {
                    m_BlobContainer.SetPermissions(m_BlobPermissions);
                }

                //m_RequestOptions = new BlobRequestOptions();
                //m_RequestOptions.ParallelOperationThreadCount = 20;
                
                m_Initialized = true;
            }
            
        }

        public bool SendFile(BlobInfo blobInfo)
        {
            bool _globalSuccess = true;

            //Creating Container
            int _retriesContainer = 3;
            bool _successContainer = false;

            do
            {
                try
                {
                    if (m_BlobContainer == null)
                    {
                        m_BlobContainer = m_BlobClient.GetContainerReference(blobInfo.ContainerName);
                    }

                    if (m_BlobPermissions == null)
                    {
                        m_BlobPermissions = new BlobContainerPermissions();
                        m_BlobPermissions.PublicAccess = BlobContainerPublicAccessType.Off;
                    }

                    if (m_BlobContainer.CreateIfNotExists())
                    {
                        m_BlobContainer.SetPermissions(m_BlobPermissions);
                    }

                    _successContainer = true;
                }
                catch (Exception ex)
                {
                    //Logger.WriteError("", ex);
                }

            } while (!_successContainer && --_retriesContainer > 0);

            _globalSuccess = _successContainer;

            if (_successContainer)
            {
                bool _successBlob = false;
                int _retriesBlob = 3;

                CloudBlockBlob _blockBlob = m_BlobContainer.GetBlockBlobReference(blobInfo.BlobName);
                
                do
                {
                    try
                    {
                        //_blockBlob.UploadFromByteArray(blobInfo.BlobByteSource, 0, blobInfo.BlobByteSource.Count(), null, null, null);
                        _blockBlob.UploadFromStream(blobInfo.BlobStreamSource);
                        //_blockBlob.UploadByteArray(file);
                        _successBlob = true;
                    }
                    catch (Exception ex)
                    {
                        // Logger.WriteError("", ex);
                    }

                } while (!_successBlob && --_retriesBlob > 0);

                _globalSuccess = _successBlob;
            }
            Console.WriteLine("Finished: " + blobInfo.BlobName);
            return _globalSuccess;
        }

        public bool SendFileAsync(BlobInfo blobInfo)
        {
            if (!m_Initialized)
            {
                this.Initialize(blobInfo.ContainerName);
            }

            CloudBlockBlob _blockBlob = m_BlobContainer.GetBlockBlobReference(blobInfo.BlobName);

            _blockBlob.UploadFromByteArrayAsync(blobInfo.BlobByteSource, 0, blobInfo.BlobByteSource.Count());
            //_blockBlob.UploadFromByteArray(blobInfo.BlobByteSource, 0, blobInfo.BlobByteSource.Count(), null, null, null);
            return true;
        }

        #endregion public methods
    }
}
