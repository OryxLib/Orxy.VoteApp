using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
using Oryx.VoteApp.Server.Models;
using Oryx.VoteApp.Server.PostManager;

namespace Oryx.VoteApp.Server.Controllers
{
    public class JdlyController : Controller
    {
        public IMemoryCache memoryCache { get; set; }
        private IDistributedCache _IDistributedCache { get; set; }
        private readonly ILogger<VoteController> _logger;
        private VoteAppDbContext dbContext { get; set; }
        private DbOperationMngr dbOptMng { get; set; }
        private SingletonAppDbContext singletonDbContext { get; set; }


        public JdlyController(
            IMemoryCache _memoryCache,
            IDistributedCache _distributedCache,
            VoteAppDbContext _dbContext,
            DbOperationMngr _dbOptMng,
            ILogger<VoteController> logger,
            SingletonAppDbContext _singletonDbContext
            )
        {
            _IDistributedCache = _distributedCache;
            dbOptMng = _dbOptMng;
            dbContext = _dbContext;
            memoryCache = _memoryCache;
            _logger = logger;
            singletonDbContext = _singletonDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPost(JDLYQuestion question)
        {
            try
            {
                await dbContext.AddAsync(question);
                await dbContext.SaveChangesAsync();
                return Json(new { success = true, msg = true });
            }
            catch (Exception exc)
            {
                return Json(new { success = false, msg = exc.Message });
            }
        }

        public async Task<IActionResult> DownloadLogs(int Id)
        {
            var jdlyLog = await dbContext.JDLYLog.Where(x => x.JdlyId == Id).ToListAsync();
            //var userInfo = await dbContext.UserInfo.Where(x => voteLog.Select(c => c.UserId).Contains(x.WxOpenId)).ToListAsync();
            //jdlyLog.
            var outputStream = new MemoryStream();
            using (ExcelPackage package = new ExcelPackage())
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("jdly_game");
                //添加头
                worksheet.Cells[1, 1].Value = "Staff ID";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Photo 1";
                worksheet.Cells[1, 4].Value = "Photo 2";
                worksheet.Cells[1, 5].Value = "Photo 3";
                worksheet.Cells[1, 6].Value = "Photo 4";
                worksheet.Cells[1, 7].Value = "Photo 5";
                worksheet.Cells[1, 8].Value = "Photo 6";
                worksheet.Cells[1, 9].Value = "Photo 7";
                worksheet.Cells[1, 10].Value = "Photo 8";
                worksheet.Cells[1, 11].Value = "Photo 9";
                worksheet.Cells[1, 12].Value = "Photo 10";
                worksheet.Cells[1, 13].Value = "Photo 11";
                worksheet.Cells[1, 14].Value = "Photo 12";
                worksheet.Cells[1, 15].Value = "Photo 13";
                worksheet.Cells[1, 16].Value = "Photo 14";
                worksheet.Cells[1, 17].Value = "Total Correct";
                worksheet.Cells[1, 18].Value = "NickName";
                worksheet.Cells[1, 19].Value = "Submit DateTime";
                for (int i = 0; i < jdlyLog.Count; i++)
                {
                    var item = jdlyLog[i];
                    //var user = userInfo.FirstOrDefault(x => x.WxOpenId == item.UserId);
                    //if (user != null)
                    //{
                    var voteOption = item.OptionJson;
                    var voteOptionList = voteOption.Split("|", StringSplitOptions.RemoveEmptyEntries);
                    worksheet.Cells[i + 2, 1].Value = item.UserCode;
                    var excelUser = await dbContext.ExcelUserInfo.FirstOrDefaultAsync(x => x.Description == item.UserCode);
                    worksheet.Cells[i + 2, 2].Value = excelUser?.UserName;

                    for (int voteOptionIndex = 0; voteOptionIndex < voteOptionList.Length; voteOptionIndex++)
                    {
                        var voteOptionItem = voteOptionList[voteOptionIndex];
                        var kvArr = voteOptionItem.Split(':');
                        worksheet.Cells[i + 2, 3 + voteOptionIndex].Value = kvArr[1];
                    }
                    worksheet.Cells[i + 2, 17].Value = item.RightNumber;
                    worksheet.Cells[i + 2, 18].Value = item.NickName;
                    worksheet.Cells[i + 2, 19].Value = item.CreateTime.ToString("yyyy-MM-dd hh:mm:ss");
                }

                package.SaveAs(outputStream);
            }

            //return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "投票记录" + DateTime.Now.ToLongDateString() + ".xlsx");            //return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "投票记录" + DateTime.Now.ToLongDateString() + ".xlsx");
            outputStream.Position = 0;
            return File(outputStream, "application/vnd.ms-excel", "金典留影 记录" + ".xlsx");
        }

        public async Task<IActionResult> ChekcUserCode(string userCode)
        {
            var jsonSetting = new JsonSerializerSettings();
            jsonSetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            var cacheEntryOptions = new DistributedCacheEntryOptions()
                   .SetSlidingExpiration(TimeSpan.FromSeconds(10));
            try
            {
                //check is employee
                var isEmployee = false;
                var isEmployeeResult = await _IDistributedCache.GetStringAsync("isEmployee" + userCode);
                if (string.IsNullOrEmpty(isEmployeeResult))
                {
                    var excelUserInfo = await dbContext.ExcelUserInfo.FirstOrDefaultAsync(x => x.Description == userCode);
                    if (excelUserInfo != null)
                    {
                        isEmployee = true;
                    }
                    //await dbContext.UserInfo.AddAsync(new UserInfo
                    //{
                    //    Password = excelUserInfo.Description,
                    //    UserName = excelUserInfo.UserName
                    //});

                    //await dbContext.SaveChangesAsync();
                    await _IDistributedCache.SetStringAsync("isEmployee" + userCode, isEmployee.ToString(), cacheEntryOptions);
                }
                else
                {
                    bool.TryParse(isEmployeeResult, out isEmployee);
                }

                JDLYQuestion jsylQuestion = await dbContext.JDLYQuestion.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                var isVoted = await dbContext.JDLYLog.AnyAsync(x => x.UserCode == userCode);
                return Json(new { success = true, msg = isEmployee, isEnable = jsylQuestion.IsEnable, isVoted = isVoted });
            }
            catch (Exception exc)
            {
                return Json(new { success = false, msg = exc.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> StartVote(int Id, int closeSecondes)
        {
            try
            {
                var timeStamp = (DateTime.Now - DateTime.Parse("1970-01-01").AddHours(8)).TotalMilliseconds;

                var jklyInfo = dbContext.JDLYQuestion.FirstOrDefault(x => x.Id == Id);
                jklyInfo.CloseSeconds = closeSecondes;


                jklyInfo.IsEnable = true;
                jklyInfo.EnableStartTime = timeStamp;
                jklyInfo.CloseSeconds = closeSecondes;
                await dbContext.SaveChangesAsync();
                return Json(new { success = true, msg = true });
            }
            catch (Exception exc)
            {
                //await Log.WriteLog(exc.Message);
                return new BadRequestResult();
            }
        }

        public async Task<IActionResult> ClearUp(int Id)
        {
            try
            {
                var jdly = await dbContext.JDLYQuestion.FirstOrDefaultAsync(x => x.Id == Id);
                jdly.IsEnable = false;
                jdly.CloseSeconds = 0;
                jdly.EnableStartTime = 0;
                var readyToRemove = dbContext.JDLYLog.Where(x => x.JdlyId == Id);
                dbContext.JDLYLog.RemoveRange(readyToRemove);
                await dbContext.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception exc)
            {
                return Json(new { success = false, msg = exc.Message });
            }
        }

        public async Task<IActionResult> GetVote()
        {

            var jsonSetting = new JsonSerializerSettings();
            jsonSetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            try
            {
                JDLYQuestion question;
                if (!memoryCache.TryGetValue<JDLYQuestion>("jdlyQuestion", out question))
                {
                    question = await dbContext.JDLYQuestion.Include("Options").OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(30));
                    memoryCache.Set("jdlyQuestion", question, cacheEntryOptions);
                }
                return Json(question, jsonSetting);
            }
            catch (Exception exc)
            {
                return Json(exc.Message);
            }
        }

        public async Task<IActionResult> Post(JDLYLog jdlyLog)
        {
            try
            {
                dbOptMng.InsertOptAutoRun(jdlyLog, async _localModel =>
                {
                    await singletonDbContext.AddRangeAsync(_localModel);
                    await singletonDbContext.SaveChangesAsync();
                }, 500);
                return Json(new { success = true, msg = true });
            }
            catch (Exception exc)
            {
                return Json(new { success = false, msg = exc.Message });
            }
        }
    }
}