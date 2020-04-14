using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Oryx.VoteApp.Server.Models;

namespace Oryx.VoteApp.Server.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private VoteAppDbContext dbContext { get; set; }

        public AccountController(VoteAppDbContext _dbContext, ILogger<AccountController> logger)
        {
            dbContext = _dbContext;
            _logger = logger;
            _logger.LogDebug("test");
        }

        [HttpPost]
        public async Task<IActionResult> LoginApi(AdminUser _user)
        {
            var isExcited = dbContext.AdminUser.Any(x => x.UserName == _user.UserName && x.Passworkd == _user.Passworkd);
            if (isExcited)
            {
                HttpContext.Session.SetString("loginUser", "true");
                await HttpContext.Session.CommitAsync();
                return Redirect("/index");
            }
            else
            {
                return Redirect("/account/login");
            }
        }
    }
}