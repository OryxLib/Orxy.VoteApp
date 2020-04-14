using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Oryx.VoteApp.Server.Models;

namespace Oryx.VoteApp.Server.Pages
{
    public class VoteEditModel : PageModel
    {
        private VoteAppDbContext dbContext { get; set; }
        public VoteInfo VoteInfo { get; set; }
        public string VoteInfoJson { get; set; }
        public Dictionary<string, int> VoteLog { get; set; }

        public VoteEditModel(VoteAppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public IActionResult OnGet(int? voteId)
        {
            if (voteId == null)
            {
                return NotFound();
            }

            VoteInfo = dbContext.VoteInfo.Include("VoteOptions").FirstOrDefault(x => x.Id == voteId.Value);
            VoteLog = dbContext.VoteLog.Where(x => x.VoteId == voteId.Value).GroupBy(x => x.VoteOption).Select(x => new
            {
                _key = x.Select(c => c.VoteOption).First() ?? "",
                _count = x.Count()
            })?.ToDictionary(x => x._key, y => y._count);
            var jsonSetting = new JsonSerializerSettings();
            jsonSetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            VoteInfoJson = JsonConvert.SerializeObject(VoteInfo, jsonSetting);
            return Page();
        }
    }
}