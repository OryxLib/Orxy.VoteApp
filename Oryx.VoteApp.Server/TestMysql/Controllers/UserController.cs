using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ValueTestMysql.Models;
//using Oryx.Wx.MiniApp;

namespace ValueTestMysql.Controllers
{
    public class UserController : Controller
    {
        public IConfiguration Configuration { get; set; }

        public UserController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        //public IActionResult UseCode(string code)
        //{
        //    var result = Autherize.CheckSession(Configuration["AppId"], Configuration["Secret"], code);

        //    return Content(JObject.Parse(result)["openid"].ToString());
        //}

        public IActionResult InfoUpload(string jsonUserInfo, string openId)
        {
            var userInfo = JsonConvert.DeserializeObject<UserInfo>(jsonUserInfo);
            userInfo.WxOpenId = openId;
            return Json(new { success = true });
        }
    }
}