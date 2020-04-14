using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Oryx.VoteApp.Server.Models;

namespace Oryx.VoteApp.Server.Pages
{
    public class WxqPicModel : PageModel
    {
        private VoteAppDbContext dbContext { get; set; }
        //public VoteInfo VoteInfo { get; set; }
        //public Dictionary<string, int> VoteLog { get; set; }
        //public int PeopleNum { get; set; }
        public List<WxqLog> WxqLogList = new List<WxqLog>();
        public WxqPicModel(VoteAppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public async Task<IActionResult> OnGet()
        {

            WxqLogList = await dbContext.WxqLog.OrderByDescending(x => x.Id).Where(x => x.MsgStatus == MsgStatus.Checked && x.MsgType == "img").Take(100).ToListAsync();
            //WxqLogList = WxqLogList.Where(x => x.MsgStatus == MsgStatus.Checked).ToList();
            //foreach (var item in WxqLogList)
            //{
            //    item.MsgStatus = MsgStatus.Proccessed;
            //}
            await dbContext.SaveChangesAsync();
            //WxqLogList.Reverse();
            //if (voteId == null)
            //{
            //    return NotFound();
            //}

            //VoteInfo = dbContext.VoteInfo.Include("VoteOptions").FirstOrDefault(x => x.Id == voteId.Value);
            //VoteLog = dbContext.VoteLog.Where(x => x.VoteId == voteId.Value && x.VoteOption != "null").GroupBy(x => x.VoteOption).Select(x => new
            //{
            //    _key = x.Select(c => c.VoteOption).First() ?? "",
            //    _count = x.Count()
            //})?.ToDictionary(x => x._key, y => y._count);
            //PeopleNum = dbContext.ExcelUserInfo.Count();
            //ViewData["voteType"] = VoteInfo.VoteType;
            //ViewData["voteName"] = VoteInfo.Name;
            return Page();
        }
    }
}