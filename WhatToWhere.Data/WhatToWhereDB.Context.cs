//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WhereToWhere.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class WhatToWhereDBEntities : DbContext
    {
        public WhatToWhereDBEntities()
            : base("name=WhatToWhereDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<tbl_follow> tbl_follow { get; set; }
        public DbSet<tbl_post> tbl_post { get; set; }
        public DbSet<vw_dashboard_post> vw_dashboard_post { get; set; }
        public DbSet<tbl_admin> tbl_admin { get; set; }
        public DbSet<tbl_user> tbl_user { get; set; }
        public DbSet<tbl_answer> tbl_answer { get; set; }
        public DbSet<tbl_question> tbl_question { get; set; }
        public DbSet<tbl_directory> tbl_directory { get; set; }
        public DbSet<tbl_useranswer> tbl_useranswer { get; set; }
    }
}
