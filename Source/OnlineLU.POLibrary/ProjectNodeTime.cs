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
    
    public partial class ProjectNodeTime
    {
        public ProjectNodeTime()
        {
            this.ProjectSubmit = new HashSet<ProjectSubmit>();
            this.Node = new HashSet<Node>();
        }
    
        public int Id { get; set; }
        public Nullable<System.DateTimeOffset> NodeQueueProcess { get; set; }
        public Nullable<System.DateTimeOffset> NodeDownload { get; set; }
        public Nullable<System.DateTimeOffset> NodeThreadInstance { get; set; }
        public Nullable<System.DateTimeOffset> NodeUploadResult { get; set; }
        public Nullable<System.DateTimeOffset> NodePivot { get; set; }
        public string NodeLocalProcess { get; set; }
    
        public virtual ICollection<ProjectSubmit> ProjectSubmit { get; set; }
        public virtual ICollection<Node> Node { get; set; }
    }
}