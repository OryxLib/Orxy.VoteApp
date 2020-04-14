using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oryx.VoteApp.Server.Models;

namespace Oryx.VoteApp.Server.Pages.Client
{
    public class VoteModel : PageModel
    {
        private VoteAppDbContext dbContext;

        public VoteInfo VoteInfo { get; set; }

        public VoteModel(VoteAppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public void OnGet()
        {
            VoteInfo = dbContext.VoteInfo.OrderByDescending(x => x.Id).FirstOrDefault(x => x.IsOpen);
        }
    }
}