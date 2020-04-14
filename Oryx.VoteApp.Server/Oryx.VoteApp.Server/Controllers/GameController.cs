using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using Oryx.VoteApp.Server.Models;
using Oryx.VoteApp.Server.Services;
using Oryx.VoteApp.Server.Ultility;
using Oryx.WebSocket.Extension.Utility;
using Oryx.WebSocket.Infrastructure;

namespace Oryx.VoteApp.Server.Controllers
{
    public class GameController : Controller
    {
        private readonly QiniuTool qiniutool;
        private VoteAppDbContext dbContext { get; set; }
        private IMemoryCache memoryCache;
        private OryxWebSocketPool wsPool { get; set; }
        private readonly IDistributedCache distributedCache;
        private RabbitMQClient rabbitMqClient { get; set; }
        public GameController(QiniuTool _qiniutool,
            VoteAppDbContext _dbContext,
            IMemoryCache _memoryCache,
            OryxWebSocketPool _wsPool,
               IDistributedCache _distributedCache,
               RabbitMQClient _rabbitMqClient
            )
        {
            qiniutool = _qiniutool;
            dbContext = _dbContext;
            memoryCache = _memoryCache;
            wsPool = _wsPool;
            distributedCache = _distributedCache;
            rabbitMqClient = _rabbitMqClient;
        }

        public async Task<IActionResult> LoadUserExcel()
        {
            try
            {
                var file = Request.Form.Files["file"];
                if (file == null)
                {
                    return NotFound();
                }

                var xel = new ExcelPackage(file.OpenReadStream());
                var workSheet = xel.Workbook.Worksheets[1];
                for (int rowIndex = 1; rowIndex < workSheet.Dimension.Rows; rowIndex++)
                {
                    var userName = workSheet.Cells[rowIndex, 2].Value.ToString();
                    var description = workSheet.Cells[rowIndex, 1].Value.ToString().ToLower().Replace("cn", "");
                    var exceted = dbContext.ExcelUserInfo.Any(x => x.UserName == userName && x.Description == description);
                    if (!exceted)
                    {
                        await dbContext.ExcelUserInfo.AddAsync(new ExcelUserInfo()
                        {
                            UserName = userName,
                            Description = description
                        });
                    }
                }
                await dbContext.SaveChangesAsync();
            }
            catch (Exception exc)
            {
                return Json(new { success = false, msg = exc.Message });
            }
            return Json(new { success = true, msg = "ok" });
        }

        public async Task<IActionResult> DownloadVoteLogs(int voteId)
        {
            //string sWebRootFolder = _hostingEnvironment.WebRootPath;
            //string sFileName = $"{Guid.NewGuid()}.xlsx";
            //FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var voteLog = await dbContext.VoteLog.Where(x => x.VoteId == voteId).ToListAsync();
            var userInfo = await dbContext.UserInfo.Where(x => voteLog.Select(c => c.UserId).Contains(x.WxOpenId)).ToListAsync();
            var outputStream = new MemoryStream();
            using (ExcelPackage package = new ExcelPackage())
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("vote_game");
                //添加头
                worksheet.Cells[1, 1].Value = "微信名";
                worksheet.Cells[1, 2].Value = "姓名";
                worksheet.Cells[1, 3].Value = "编号";
                worksheet.Cells[1, 4].Value = "投票";
                worksheet.Cells[1, 5].Value = "时间";
                worksheet.Cells[1, 6].Value = "坐标 Latitude";
                worksheet.Cells[1, 7].Value = "坐标 Longtitude";
                worksheet.Cells[1, 8].Value = "OpenId";
                for (int i = 0; i < voteLog.Count; i++)
                {
                    var item = voteLog[i];
                    var user = userInfo.FirstOrDefault(x => x.WxOpenId == item.UserId||x.Password==item.UserKey||x.WxNickName==item.UserKey);
                    //if (user != null)
                    //{
                    JDLYLog jdlyLog = null;
                    string ExcelUserName = string.Empty;
                    if (user == null || user.UserName == null || user.Password == null)
                    {
                        jdlyLog = await dbContext.JDLYLog.FirstOrDefaultAsync(x => x.NickName == item.UserKey);
                        if (jdlyLog != null)
                        { 
                            ExcelUserName = (await dbContext.ExcelUserInfo.FirstOrDefaultAsync(x => x.Description == jdlyLog.UserCode)).UserName;
                        }
                    }
                    worksheet.Cells[i + 2, 1].Value = user?.WxNickName ?? item.UserKey ?? "";
                    worksheet.Cells[i + 2, 2].Value = user?.UserName ?? ExcelUserName;
                    worksheet.Cells[i + 2, 3].Value = user?.Password??jdlyLog?.UserCode;
                    worksheet.Cells[i + 2, 4].Value = item.VoteOption;
                    worksheet.Cells[i + 2, 5].Value = item.CreateTime.ToString("yyyy-MM-dd hh:mm:ss");
                    worksheet.Cells[i + 2, 6].Value = item.Latitude;
                    worksheet.Cells[i + 2, 7].Value = item.Longtitude;
                    worksheet.Cells[i + 2, 8].Value = item.UserId;
                    //}
                }

                package.SaveAs(outputStream);
            }

            //return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "投票记录" + DateTime.Now.ToLongDateString() + ".xlsx");            //return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "投票记录" + DateTime.Now.ToLongDateString() + ".xlsx");
            outputStream.Position = 0;
            return File(outputStream, "application/vnd.ms-excel", "投票记录" + ".xlsx");
        }

        public async Task<IActionResult> OutputWxqData()
        {
            var dataList = dbContext.WxqLog.Where(x => x.MsgType == "txt").ToList();
            var outputStream = new MemoryStream();
            using (ExcelPackage package = new ExcelPackage())
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("微信墙数据");
                //添加头
                worksheet.Cells[1, 1].Value = "姓名";
                worksheet.Cells[1, 2].Value = "头像";
                worksheet.Cells[1, 3].Value = "消息状态";
                worksheet.Cells[1, 4].Value = "内容";
                for (int i = 0; i < dataList.Count; i++)
                {
                    var item = dataList[i];
                    //var user = userInfo.FirstOrDefault(x => x.WxOpenId == item.UserId);
                    //if (user != null)
                    //{
                    worksheet.Cells[i + 2, 1].Value = item.NickName;
                    worksheet.Cells[i + 2, 2].Value = item.Avarta;
                    worksheet.Cells[i + 2, 3].Value = item.MsgStatus;
                    worksheet.Cells[i + 2, 4].Value = item.Msg;
                    //worksheet.Cells[i + 2, 5].Value = item.CreateTime.ToString("yyyy-MM-dd hh:mm:ss");
                    //worksheet.Cells[i + 2, 6].Value = item.Latitude;
                    //worksheet.Cells[i + 2, 7].Value = item.Longtitude;
                    //worksheet.Cells[i + 2, 8].Value = item.UserId;
                    //}
                }

                package.SaveAs(outputStream);
            }

            //return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "投票记录" + DateTime.Now.ToLongDateString() + ".xlsx");            //return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "投票记录" + DateTime.Now.ToLongDateString() + ".xlsx");
            outputStream.Position = 0;
            return File(outputStream, "application/vnd.ms-excel", "微信墙数据" + ".xlsx");
        }

        public async Task<IActionResult> OuputWxqPicData()
        {
            var dataList = dbContext.WxqLog.Where(x => x.MsgType == "img" && !x.NickName.Contains("李能能") && !x.Msg.Contains("undefined")).ToList();
            var outputStream = new MemoryStream();
            var saveImgPath = AppDomain.CurrentDomain.BaseDirectory + "WxqPic";
            if (!Directory.Exists(saveImgPath))
            {
                Directory.CreateDirectory(saveImgPath);
            }
            using (ExcelPackage package = new ExcelPackage())
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("微信墙图片数据");
                var wc = new WebClient();
                //添加头
                worksheet.Cells[1, 1].Value = "姓名";
                worksheet.Cells[1, 2].Value = "头像";
                worksheet.Cells[1, 3].Value = "消息状态";
                worksheet.Cells[1, 4].Value = "图片";
                for (int i = 0; i < dataList.Count; i++)
                {
                    var item = dataList[i];
                    worksheet.Cells[i + 2, 1].Value = item.NickName;
                    worksheet.Cells[i + 2, 2].Value = item.Avarta;
                    worksheet.Cells[i + 2, 3].Value = item.MsgStatus;
                    worksheet.Cells[i + 2, 4].Value = item.Msg;
                    var imgUrl = item.Msg;
                    try
                    {
                        var bytes = await wc.DownloadDataTaskAsync(imgUrl);
                        var fileName = item.Msg.Replace("https://mioto.milbit.com/", "");
                        var file = System.IO.File.Create(saveImgPath + "/" + fileName);
                        await file.WriteAsync(bytes, 0, bytes.Length);
                        ////var fileStream = new MemoryStream(bytes);
                        ////var image = Image.FromStream(fileStream);
                        ////var drawImg = worksheet.Drawings.AddPicture(item.NickName + DateTime.Now.ToString("yyyyMMddhhmmss"), image);
                        ////drawImg.From.Row = i + 2;
                        ////drawImg.From.Column = 4;
                        ////drawImg.SetSize(200, 200);

                    }
                    catch (Exception exc)
                    {

                        //throw;
                    }


                    //worksheet.Cells[i + 2, 3]. = item.Msg; 
                }

                package.SaveAs(outputStream);
            }

            //return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "投票记录" + DateTime.Now.ToLongDateString() + ".xlsx");            //return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "投票记录" + DateTime.Now.ToLongDateString() + ".xlsx");
            outputStream.Position = 0;
            return File(outputStream, "application/vnd.ms-excel", "微信墙图片数据" + ".xlsx");
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> GetToken()
        {
            string token = "";
            //if (!memoryCache.TryGetValue("getToken", out token))
            //{
            //    token = qiniutool.GenerateToken();
            //    var cacheEntryOptions = new MemoryCacheEntryOptions()
            //        .SetSlidingExpiration(TimeSpan.FromSeconds(30));
            //    memoryCache.Set("getToken", token, cacheEntryOptions);
            //}
            token = qiniutool.GenerateToken();
            return Json(new { token });
        }

        public async Task<IActionResult> CreateVotingPost(VoteInfo voteInfo)
        {
            try
            {
                dbContext.VoteInfo.Add(voteInfo);
                var otherOpenVote = await dbContext.VoteInfo.Where(x => x.IsOpen).ToListAsync();
                foreach (var item in otherOpenVote)
                {
                    item.IsOpen = false;
                }
                await dbContext.SaveChangesAsync();
                return Json(new { result = true, msg = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { result = true, msg = exc.Message });
            }
        }

        public async Task<IActionResult> UpdateVotingPost(VoteInfo voteInfo)
        {
            try
            {
                var trackVoteInfo = await dbContext.VoteInfo.Include("VoteOptions").FirstOrDefaultAsync(x => x.Id == voteInfo.Id);
                trackVoteInfo.Name = voteInfo.Name;
                //trackVoteInfo.VoteOptions = voteInfo.VoteOptions;
                //var removeOption = new List<VoteOptions>();
                foreach (var item in trackVoteInfo.VoteOptions)
                {
                    var changedOption = voteInfo.VoteOptions.FirstOrDefault(x => x.Id == item.Id);
                    if (changedOption != null)
                    {
                        item.Key = changedOption.Key;
                        item.Value = changedOption.Value;
                        item.Description = changedOption.Description;
                    }
                }
                var newOptionList = voteInfo.VoteOptions.Where(x => !trackVoteInfo.VoteOptions.Select(c => c.Id).Contains(x.Id));

                if (newOptionList != null && newOptionList.Count() > 0)
                {
                    foreach (var item in newOptionList)
                    {
                        trackVoteInfo.VoteOptions.Add(item);
                    }
                }

                var removeOptionList = trackVoteInfo.VoteOptions.Where(x => !voteInfo.VoteOptions.Select(c => c.Id).Contains(x.Id));
                dbContext.RemoveRange(removeOptionList);
                //if (removeOptionList != null && voteInfo.VoteOptions.Count() > 0)
                //{
                //    foreach (var item in removeOptionList)
                //    {

                //    }
                //}
                trackVoteInfo.VoteType = voteInfo.VoteType;
                trackVoteInfo.EnableTime = voteInfo.EnableTime;
                trackVoteInfo.CloseSeconds = voteInfo.CloseSeconds;
                trackVoteInfo.IsOpen = voteInfo.IsOpen;
                trackVoteInfo.ShouldLogin = voteInfo.ShouldLogin;
                //dbContext.VoteInfo.Add(voteInfo);
                if (voteInfo.IsOpen)
                {
                    var otherOpenVote = dbContext.VoteInfo.Where(x => x.IsOpen).ToList();
                    foreach (var item in otherOpenVote)
                    {
                        item.IsOpen = false;
                    }
                }
                //dbContext.Update(trackVoteInfo);
                await dbContext.SaveChangesAsync();
                //dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                return Json(new { result = true, msg = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { result = false, msg = exc.Message });
            }
        }

        public async Task<IActionResult> DelVote(int voteId)
        {
            try
            {
                var voteOptions = await dbContext.VoteOption.Where(x => x.VoteInfo.Id == voteId).ToListAsync();
                dbContext.VoteOption.RemoveRange(voteOptions);
                await dbContext.SaveChangesAsync();
                var voteInfo = dbContext.VoteInfo.FirstOrDefault(x => x.Id == voteId);
                dbContext.VoteInfo.Remove(voteInfo);
                await dbContext.SaveChangesAsync();
                if (voteInfo.IsOpen)
                {
                    var topVote = await dbContext.VoteInfo.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                    topVote.IsOpen = true;
                }

                await dbContext.SaveChangesAsync();
                return Json(new { result = true, msg = "success" });
            }
            catch (Exception exc)
            {
                return Content(exc.Message);
            }
        }

        public async Task<IActionResult> OpenVote(int voteId)
        {
            try
            {
                var voteInfo = dbContext.VoteInfo.FirstOrDefault(x => x.Id == voteId);
                voteInfo.IsOpen = true;
                var otherOpenVote = await dbContext.VoteInfo.Where(x => x.IsOpen).ToListAsync();
                foreach (var item in otherOpenVote)
                {
                    item.IsOpen = false;
                }
                await dbContext.SaveChangesAsync();
                var cmdMsgNextVote = JsonConvert.SerializeObject(new { cmd = "nextVote", timestamp = voteInfo.EnableStartTime });
                var byteData = Encoding.UTF8.GetBytes(cmdMsgNextVote);
                //var targetWebSocket = wsPool.WebSocketList.Where(x => x.QueryString["key"] == "userSocket");

                rabbitMqClient["BroadcastUserInfo"].Queue.BasicPublish("FanoutUserInfoExchange", string.Empty, false, null, byteData);

                return Json(new { result = true, msg = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { result = true, msg = exc.Message });
            }
        }

        public async Task<IActionResult> UploadToken()
        {
            return Json(new { });
        }
    }
}