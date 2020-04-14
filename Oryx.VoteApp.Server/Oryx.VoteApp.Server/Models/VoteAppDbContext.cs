using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oryx.VoteApp.Server.Models
{
    public class VoteAppDbContext : DbContext
    {
        public VoteAppDbContext(DbContextOptions<VoteAppDbContext> options) : base(options)
        {
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

        public DbSet<AdminUser> AdminUser { get; set; }

        public DbSet<VoteBuff> VoteBuff { get; set; }

        public DbSet<WxqLog> WxqLog { get; set; }

        public DbSet<JDLYQuestion> JDLYQuestion { get; set; }

        public DbSet<JDLYOption> JDLYOption { get; set; }

        public DbSet<JDLYLog> JDLYLog { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }

    public class SingletonAppDbContext : DbContext
    {
        public SingletonAppDbContext(DbContextOptions<SingletonAppDbContext> options) : base(options)
        {

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

        public DbSet<AdminUser> AdminUser { get; set; }

        public DbSet<VoteBuff> VoteBuff { get; set; }

        public DbSet<WxqLog> WxqLog { get; set; }

        public DbSet<JDLYQuestion> JDLYQuestion { get; set; }

        public DbSet<JDLYOption> JDLYOption { get; set; }

        public DbSet<JDLYLog> JDLYLog { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=101.132.130.133;database=VoteApp;user=root;password=Linengneng123#;Character Set=utf8;", opts =>
              {
                  opts.CommandTimeout(30000);
                  opts.EnableRetryOnFailure(3);
                  opts.MaxBatchSize(1000);
              });
            base.OnConfiguring(optionsBuilder);
        }
    }
}