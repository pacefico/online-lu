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
    
    public partial class ProjectSubmit
    {
        public ProjectSubmit()
        {
            this.Project = new HashSet<Project>();
            this.Node = new HashSet<Node>();
        }
    
        public int Id { get; set; }
        public string SubmissionDate { get; set; }
        public string EndDate { get; set; }
        public System.Guid Guid { get; set; }
        public int ProjectNodeTime_Id { get; set; }
    
        public virtual ProjectNodeTime ProjectNodeTime { get; set; }
        public virtual ICollection<Project> Project { get; set; }
        public virtual ICollection<Node> Node { get; set; }
    }
}
