using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValueTestMysql.Models
{
    public class VoteAppDbContext : DbContext
    {
        public VoteAppDbContext(DbContextOptions options) : base(options)
        {
            this.Database.EnsureCreated();
            this.Database.Migrate();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<VoteInfo> VoteInfo { get; set; }

        public DbSet<VoteOptions> VoteOption { get; set; }

        public DbSet<VoteLog> VoteLog { get; set; }

        public DbSet<UserInfo> UserInfo { get; set; }

        public DbSet<ExcelUserInfo> ExcelUserInfo { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //#warning To protect potentially sensitive information in your connection string, 
            //            you should move it out of source code.See http://go.microsoft.com/fwlink/?LinkId=723263 
            //     for guidance on storing connection strings.

            //optionsBuilder.UseMySQL("server=localhost;database=library;user=user;password=password");
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //}
    }

}