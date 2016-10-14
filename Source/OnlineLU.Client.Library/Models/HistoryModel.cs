using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineLU.Client.Library.Models
{
    public class HistoryTransfer
    {
        public string Bytes { get; set; }
        public string RateKbs { get; set; }
        public long TimeMs { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
    
    public class HistoryModel
    {
        public long ProjectID { get; set; }
        public DateTime ExecutionDate { get; set; }
        public HistoryTransfer Download { get; set; } 
        public long TimeCudaMs { get; set; }
        public HistoryTransfer Upload { get; set; }
        public QueueMessage queueMessage { get; set; }
        public long TotalTime { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public HardwareInfoModel HardwareInfo { get; set; }
        
    }
}
