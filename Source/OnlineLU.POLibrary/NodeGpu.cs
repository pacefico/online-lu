//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OnlineLU.POLibrary
{
    using System;
    using System.Collections.Generic;
    
    public partial class NodeGpu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Capability { get; set; }
        public int Cores { get; set; }
        public string Processor { get; set; }
        public int Memory { get; set; }
        public System.DateTime Created { get; set; }
        public int Node_Id { get; set; }
    
        public virtual Node Node { get; set; }
    }
}
