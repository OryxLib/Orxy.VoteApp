using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ValueTestMysql.Models;

namespace ValueTestMysql.Controllers
{
    public class HomeController : Controller
    {
        private VoteAppDbContext dbContext { get; set; }

        public HomeController(VoteAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var voteList = dbContext.VoteInfo.OrderByDescending(x => x.Id).ToList();
            return View(voteList);
        }
    }
}