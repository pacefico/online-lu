using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TypeLite;

namespace OnlineLU.Client.Web.Models
{
    [TsClass(Module = "ProjectModels", Name = "UploadFileModelParamVM")]
    public class UploadFileModelParamVM
    {
        public int ID { get; set; }
        public string Line { get; set; }
    }
}