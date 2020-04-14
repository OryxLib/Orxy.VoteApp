using Microsoft.EntityFrameworkCore;
using Oryx.VoteApp.Server.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oryx.VoteApp.Server.DbConextPool
{
    //public class DbContextPool : ConcurrentBag<VoteAppDbContext>
    //{
    //    public DbContextPool()
    //    {
    //        var dbOption = new DbContextOptions<VoteAppDbContext>();
    //        var optionsBuilder = new DbContextOptionsBuilder<VoteAppDbContext>();
    //        optionsBuilder.UseMySql("server=101.132.130.133;database=VoteApp;user=root;password=Linengneng123#;Character Set=utf8;");
    //        //for (int i = 0; i < 10000; i++)
    //        //{
    //        //    var dbcontext = new VoteAppDbContext(optionsBuilder.Options);
    //        //    try
    //        //    {
    //        //        dbcontext.Database.GetDbConnection().Open();

    //        //    }
    //        //    catch (Exception exc)
    //        //    {

    //        //        throw;
    //        //    }

    //        //    Add(dbcontext);
    //        //}
    //    }
    //}
}
