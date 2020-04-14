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
    public class BossVoteModel : PageModel
    {
        private VoteAppDbContext dbContext { get; set; }
        public VoteInfo VoteInfo { get; set; }
        public string BossId { get; set; }
        public BossVoteModel(VoteAppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public void OnGet(int? voteId)
        {
            VoteInfo = dbContext.VoteInfo.Include("VoteOptions").FirstOrDefault(x => x.Id == voteId);
        }
    }
}