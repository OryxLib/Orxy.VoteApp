using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oryx.VoteApp.Server.Models;
using Oryx.VoteApp.Server.PostManager;
using Oryx.Wx.MiniApp;

namespace Oryx.VoteApp.Server.Controllers
{
    public class UserController : Controller
    {
        public IConfiguration Configuration { get; set; }

        public VoteAppDbContext dbContext { get; set; }

        private DbOperationMngr dbOptMng { get; set; }

        private ILogger<UserController> _logger;

        public UserController(IConfiguration _configuration, VoteAppDbContext _dbContext, DbOperationMngr _dbOptMng, ILogger<UserController> logger)
        {
            Configuration = _configuration;
            dbContext = _dbContext;
            dbOptMng = _dbOptMng;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Login(string username, string description, string openId, string nickName)
        {
            try
            {
                var employeInfo = await dbContext.ExcelUserInfo.FirstOrDefaultAsync(x => x.Description == description);

                if (employeInfo != null)
                {
                    var hasExceted = dbContext.UserInfo.Any(x => x.WxOpenId == openId || x.Password == description);
                    if (!hasExceted)
                    {
                        var newUser = new UserInfo
                        {
                            Password = description,
                            UserName = employeInfo.UserName,
                            WxOpenId = openId,
                            WxNickName = nickName
                        };
                        await dbContext.AddRangeAsync(newUser);
                        await dbContext.SaveChangesAsync();
                    }
                    var currentModel = await dbContext.VoteInfo.Where(x => x.IsOpen).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                    var currentVoteId = currentModel.Id;
                    var voted = dbContext.VoteLog.Any(x => x.UserId == openId && x.VoteId == currentVoteId);
                    //var currentVoteInfo = dbContext.VoteInfo.OrderByDescending(x => x.Id).Take(1).ToList()?[0];
                    //var timeStamp = (currentModel.EnableStartTime - DateTime.Parse("1970-01-01")).TotalMilliseconds;
                    return Json(new { success = true, msg = "验证信息成功", endTimeStamp = currentModel.EnableStartTime, voteStartTime = currentModel.EnableStartTime, isEnable = currentModel.IsEnable, isVoted = voted, closeSeconds = currentModel.CloseSeconds });
                }
                else
                {
                    return Json(new { success = false, msg = "验证失败,请重试" });
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);

                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, msg = "网络错误请重试" });
            }
        }



        public async Task<IActionResult> UseCode(string code)
        {
            var result = await Autherize.CheckSession(Configuration["AppId"], Configuration["Secret"], code);
            return Content(JObject.Parse(result)["openid"].ToString());
        }

        public async Task<IActionResult> InfoUpload(string jsonUserInfo, string openId)
        {
            var userInfo = JsonConvert.DeserializeObject<UserInfo>(jsonUserInfo);
            userInfo.WxOpenId = openId;
            var voteInfo = await dbContext.VoteInfo.Where(x => x.IsOpen).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            return Json(new
            {
                success = true,
                voteInfo = new
                {
                    name = voteInfo.Name,
                    type = voteInfo.VoteType
                }
            });
        }
    }
}