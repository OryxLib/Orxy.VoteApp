using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Oryx.VoteApp.Server.Models;

namespace Oryx.VoteApp.Server.Pages.JDLY
{
    public class ResultModel : PageModel
    {
        private VoteAppDbContext dbContext { get; set; }
        public ResultModel(VoteAppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public List<JDLYLog> JdlyLogs { get; set; }
        public int JdlyId { get; set; }
        public async Task OnGet()
        {
            var jdly = await dbContext.JDLYQuestion.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            JdlyId = jdly.Id;

            JdlyLogs = await dbContext.JDLYLog
                .OrderByDescending(x => x.RightNumber)
                .OrderBy(x => x.Id)
                .Take(50)
                .Where(x => x.JdlyId == JdlyId)
                .ToListAsync();
        }
    }
}