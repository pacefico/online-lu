using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLU.Client.Library.Events
{
    public class SummaryEventArgs : EventArgs
    {
        public SummaryEventArgs(string message, bool clear = false)
        {
            this.Message = message;
            this.Clear = clear;
        }

        public string Message { get; set; }
        public bool Clear { get; set; }
    }
}
