using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oryx.VoteApp.Server.Models;

namespace Oryx.VoteApp.Server.Controllers
{
    public class HomeController : Controller
    {
        private VoteAppDbContext dbContext { get; set; }

        public HomeController(VoteAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var voteList = await dbContext.VoteInfo.OrderByDescending(x => x.Id).ToListAsync();
            return View(voteList);
        }
    }
}