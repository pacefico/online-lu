using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineLU.Client.Library.AzureHelper
{
    public static class OnlineLUConstants
    {
        public static int PrecisionChar = 10;
        public static string ContainerName = "000-lutoprocess";
        public static string ContainerNameResult = "000-luprocessed";
        public static string QueueName = "lutoprocess";
        public static string QueueNameResult = "luprocessed";
        public static int ThreadUpload = 300;
        public static int ThreadDownload = 300;
    }
}
