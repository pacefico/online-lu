﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class OnlineLUEntities : DbContext
    {
        public OnlineLUEntities()
            : base("name=OnlineLUEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Node> Node { get; set; }
        public DbSet<NodeCpu> NodeCpu { get; set; }
        public DbSet<NodeGpu> NodeGpu { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<ProjectNodeTime> ProjectNodeTime { get; set; }
        public DbSet<ProjectSubmit> ProjectSubmit { get; set; }
        public DbSet<User> User { get; set; }
    }
}
