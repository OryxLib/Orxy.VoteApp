using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oryx.VoteApp.Server.Models;
using Oryx.Wx.MiniApp;

namespace Oryx.VoteApp.Server.Pages
{
    public class IndexModel : PageModel
    {
        private VoteAppDbContext dbContext { get; set; }

        private WxaCode wxaCodeFactory { get; set; }

        public IndexModel(VoteAppDbContext dbContext, WxaCode _wxaCodeFactory)
        {
            this.dbContext = dbContext;
            this.wxaCodeFactory = _wxaCodeFactory;
        }

        public List<VoteInfo> VoteInfoList { get; set; }

        private static object blockObj = new object();
        public void OnGet()
        {
            lock (blockObj)
            {
                VoteInfoList = dbContext.VoteInfo.OrderByDescending(x => x.Id).ToList();
                //wxaCodeFactory.GetWxaCode(VoteInfoList[0].VoteType);
            }
        }
    }
}
