using OnlineLU.Client.Library.AzureHelper;
using OnlineLU.Client.Library.ZipHelper;
using OnlineLU.TOLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace OnlineLU.Client.Web.Models
{
    public class CloudFile
    {
        public string FileName { get; set; }
        public string URL { get; set; }
        public long Size { get; set; }
        public long BlockCount { get; set; }
        public DateTime StartTime { get; set; }
        public string UploadStatusMessage { get; set; }
        public bool IsUploadCompleted { get; set; }
        
        public ProjectTO ProjectData { get; set; }

        public long CompressedBytesSent { get; set; }
        private PutBlobAsync m_queue;

        private int m_precisionChar = OnlineLUConstants.PrecisionChar;
        
        private bool m_showDetails = false;

        public CloudFile(int range)
        {
            m_queue = new PutBlobAsync(OnlineLUConstants.ThreadUpload, range, m_showDetails);
        }

        public void SendFile(int id, byte[] bytesReaded)
        {
            var _blobFile = new BlobInfo(id, ProjectData.ContainerName, ProjectData.Range, m_precisionChar);
            _blobFile.SetBlobName(ProjectData.ID);
            _blobFile.BlobByteSource = ZipHelperManager.ZipByteToByte(ref bytesReaded, id);
            
            this.CompressedBytesSent += _blobFile.BlobByteSource.LongLength;
            m_queue.EnqueueTask(_blobFile);
        }

        public void DisposeQueue()
        {
            this.m_queue.Dispose();
        }
    }
}