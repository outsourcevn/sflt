﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SearchFilter.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class xe63Entities : DbContext
    {
        public xe63Entities()
            : base("name=xe63Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<listprovince> listprovinces { get; set; }
        public DbSet<list> lists { get; set; }
        public DbSet<log> logs { get; set; }
        public DbSet<list_online> list_online { get; set; }
        public DbSet<ticket> tickets { get; set; }
    }
}
