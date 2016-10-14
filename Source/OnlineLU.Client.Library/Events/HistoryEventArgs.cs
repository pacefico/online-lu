using OnlineLU.Client.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineLU.Client.Library.Events
{
    public class HistoryEventArgs : EventArgs
    {
        public HistoryModel History { get; set; }
    }
}
