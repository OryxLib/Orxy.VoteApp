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
    public class HugeScreenGDModel : PageModel
    {
        private VoteAppDbContext dbContext { get; set; }
        public VoteInfo VoteInfo { get; set; }
        public Dictionary<string, int> VoteLog { get; set; }
        public Dictionary<string, int> VoteLogDic { get; set; }
        public string VoteLogJson { get; set; }
        public string VoteLogDicJson { get; set; }
        public string BuffResult { get; set; }
        public int PeopleNum { get; set; }

        public HugeScreenGDModel(VoteAppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public async Task<IActionResult> OnGet(int? voteId)
        {
            if (voteId == null)
            {
                return NotFound();
            }
            VoteInfo = dbContext.VoteInfo.Include("VoteOptions").FirstOrDefault(x => x.Id == voteId.Value);
            VoteLog = dbContext.VoteLog.Where(x => x.VoteId == voteId.Value && x.VoteOption != "null").GroupBy(x => x.VoteOption).Select(x => new
            {
                _key = x.Select(c => c.VoteOption).First() ?? "",
                _count = x.Count()
            })?.ToDictionary(x => x._key, y => y._count);
            //初始化的结构数据
            VoteLogDic = VoteInfo.VoteOptions.Select(x => new
            {
                _key = x.Key,
                _count = 0
            }).ToDictionary(x => x._key, y => y._count);
            VoteLogJson = JsonConvert.SerializeObject(VoteLog.Select(x => new { voteOption = x.Key, voteSum = x.Value }).ToList());
            VoteLogDicJson = JsonConvert.SerializeObject(VoteLogDic);
            var currentBuffer = await dbContext.VoteBuff.FirstOrDefaultAsync(x => x.VoteId == voteId);

            var buffResultList = dbContext.VoteBuff.Where(x => x.VoteId == voteId).ToList();
            var buffResultDic = new Dictionary<string, double>();
            foreach (var item in buffResultList)
            {
                if (buffResultDic.ContainsKey(item.VoteOption))
                {
                    buffResultDic[item.VoteOption] += 10;
                }
                else
                {
                    buffResultDic[item.VoteOption] = 10;
                }
            }
            BuffResult = JsonConvert.SerializeObject(buffResultDic);
            //BuffResult = currentBuffer?.VoteOption;
            PeopleNum = dbContext.ExcelUserInfo.Count();
            return Page();
        }
    }
}