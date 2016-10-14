using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TypeLite;

namespace OnlineLU.Client.Web.Models
{
    [TsClass(Module = "ProjectModels", Name = "ProjectModelParamVM")]
    public class ProjectModelParamVM
    {
        [TsProperty(IsOptional=true)]
        public DateTime? DateFrom { get; set; }
        [TsProperty(IsOptional = true)]
        public DateTime? DateTo { get; set; }
        public bool? Status { get; set; }
        public int Range { get; set; }
    }
}