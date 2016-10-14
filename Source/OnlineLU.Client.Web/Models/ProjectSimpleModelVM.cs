using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TypeLite;

namespace OnlineLU.Client.Web.Models
{
    [TsClass(Module = "ProjectModels", Name = "ProjectSimpleModelVM")]
    public class ProjectSimpleModelVM
    {
        public long ID { get; set; }
        public DateTime ExecutionDate { get; set; }
        public int Range { get; set; }
        
        [TsProperty(IsOptional=true)]
        public bool Status { get; set; }
        public HistoryModelVM History { get; set; }
    }
}