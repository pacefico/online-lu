using OnlineLU.Client.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TypeLite;

namespace OnlineLU.Client.Web.Models
{
    [TsClass(Module = "HistoryModels", Name = "HistoryModelVM")]
    public class HistoryModelVM
    {
        public long ID { get; set; }
        public DateTime ExecutionDate { get; set; }
        public int Range { get; set; }
        public long TimeInitialUploadMs { get; set; }
        public long TimeDonwloadMs { get; set; }
        public long CudaTimeMs { get; set; }
        public long TimeUploadMs { get; set; }
        public long TotalTimeMs { get; set; }
        public bool Success { get; set; }
        public HistoryDetailVM Details { get; set; }

    }

    [TsClass(Module = "HistoryModels", Name = "HistoryDetailVM")]
    public class HistoryDetailVM
    {
        public string BytesInitialUpload { get; set; }
        public string RateKbsInitialUpload { get; set; }

        public string BytesUpload { get; set; }
        public string RateKbsUpload { get; set; }

        public string BytesDownload { get; set; }
        public string RateKbsDownload { get; set; }

    }

    
}