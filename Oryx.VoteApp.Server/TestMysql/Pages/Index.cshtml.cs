using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ValueTestMysql.Models;

namespace ValueTestMysql.Pages
{
    public class IndexModel : PageModel
    {
        private VoteAppDbContext dbContext { get; set; }

        public IndexModel(VoteAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<VoteInfo> VoteInfoList { get; set; }

        private static object blockObj = new object();
        public void OnGet()
        {
            lock (blockObj)
            {
                VoteInfoList = dbContext.VoteInfo.OrderByDescending(x => x.Id).ToList();
            }
        }
    }
}
