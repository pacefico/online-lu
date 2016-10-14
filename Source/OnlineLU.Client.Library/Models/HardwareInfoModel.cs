using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineLU.Client.Library.Models
{
    public class SystemInfo
    {
        public string SystemName { get; set; }
        public string Name { get; set; }
        public int Cores { get; set; }
        public int Memory { get; set; }
        public string CudaCapable { get; set; }
    }
    public class VideoInfo
    {
        public string Name { get; set; }
        public double Memory { get; set; }
    }
    public class HardwareInfoModel
    {
        public string HardwareKey {get; set;}
        public SystemInfo System { get; set; }
        public List<VideoInfo> Video { get; set; }
    }
}
