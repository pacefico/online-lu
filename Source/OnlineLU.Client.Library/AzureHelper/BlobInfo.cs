using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLU.Client.Library.AzureHelper
{
    public class MeasuringTime
    {
        public long Id { get; set; }
        
        public long InitialTime { get; set; }
        public long FinalTime { get; set; }

        public long PreparingBytes { get; set; }

        public long QueueInitial { get; set; }
        public long QueueFinal { get; set; }

        public long InitialTransfer { get; set; }
        public long FinalTransfer { get; set; }

        public long InitialProcessing { get; set; }
        public long FinalProcessing { get; set; }

        public long TotalByteLength { get; set; }

        public MeasuringTime(int id)
        {
            this.Id = id;
        }

        public long GetTotalTime()
        {
            return FinalTime - InitialTime;
        }
        public long GetQueueTime()
        {
            return QueueFinal - QueueInitial;
        }
        public double GetRateTransfer()
        {
            //Kb / seg;

            if (TotalByteLength != 0
                && FinalTransfer != 0
                && InitialTransfer != 0)
            {
                //double _totalkbytes = TotalByteLength;
                //double _transferTime = (FinalTransfer - InitialTransfer) / 1000;

                return TotalByteLength / (FinalTransfer - InitialTransfer);
            }
            return 0;
        }

        public long GetProcessingTime()
        {
            return FinalProcessing - InitialProcessing;
        }
        public long GetTransferTime()
        {
            return FinalTransfer - InitialTransfer;
        }
    }

    public class BlobInfo
    {
        public string ContainerName;
        public string BlobName;
        public Byte[] BlobByteSource;
        public Stream BlobStreamSource;
        public int Range;
        public int PrecisionChar;
        public int Id;
        public int Retries;

        public MeasuringTime Times;
        

        public BlobInfo(int id, string containerName, int range, int precisionChar)
        {
            this.Id = id;
            this.ContainerName = containerName;
            this.Range = range;
            this.PrecisionChar = precisionChar;
            this.Times = new MeasuringTime(id);
        }

        public void SetBlobName(long projectid)
        {
            //this.BlobName = string.Format("{0}-{1}-{2}.zip", projectid, Range, Id);
            this.BlobName = string.Format("{0}-{1}.zip", Range, Id);
        }

    }
}
