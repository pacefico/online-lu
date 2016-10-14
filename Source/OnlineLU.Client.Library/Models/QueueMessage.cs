using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineLU.Client.Library.Models
{
    public class QueueMessage
    {
        public long projectid { get; set; }
        public int range { get; set; }
        public string containerSource { get; set; }
        public string containerResult { get; set; }
        
        public string messageID { get; set; }
        public string popReceipt { get; set; }
        public string queueName { get; set; }
    }
}
