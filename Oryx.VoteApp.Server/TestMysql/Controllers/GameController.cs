using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using ValueTestMysql.Models;
//using ValueTestMysql.Ultility;

namespace ValueTestMysql.Controllers
{
    public class GameController : Controller
    {
        //private readonly QiniuTool qiniutool;
        private VoteAppDbContext dbContext { get; set; }
        public GameController(/*QiniuTool _qiniutool, */VoteAppDbContext _dbContext)
        {
            //qiniutool = _qiniutool;
            dbContext = _dbContext;
        }

        public async Task<IActionResult> LoadUserExcel()
        {
            var file = Request.Form.Files["file"];

            if (file == null)
            {
                return NotFound();
            }

            var xel = new ExcelPackage(file.OpenReadStream());
            var workSheet = xel.Workbook.Worksheets[0];
            for (int rowIndex = 0; rowIndex < workSheet.Dimension.Rows; rowIndex++)
            {
                var userName = workSheet.Cells[rowIndex, 0].ToString();
                var description = workSheet.Cells[rowIndex, 1].ToString();
                var exceted = dbContext.ExcelUserInfo.Any(x => x.UserName == userName && x.Description == description);
                if (exceted)
                {
                    dbContext.ExcelUserInfo.Add(new ExcelUserInfo()
                    {
                        UserName = userName,
                        Description = description
                    });
                }
            }
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        public async Task<IActionResult> DownloadVoteLogs(int voteId)
        {
            //string sWebRootFolder = _hostingEnvironment.WebRootPath;
            //string sFileName = $"{Guid.NewGuid()}.xlsx";
            //FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var voteLog = dbContext.VoteLog.Where(x => x.VoteId == voteId).ToList();
            var userInfo = dbContext.UserInfo.Where(x => voteLog.Select(c => c.UserId).Contains(x.WxOpenId));
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
                for (int i = 0; i < voteLog.Count; i++)
                {
                    var item = voteLog[i];
                    var user = userInfo.FirstOrDefault(x => x.WxOpenId == item.UserId);
                    worksheet.Cells[i + 2, 1].Value = user.WxNickName;
                    worksheet.Cells[i + 2, 2].Value = user.UserName;
                    worksheet.Cells[i + 2, 3].Value = user.Password;
                    worksheet.Cells[i + 2, 4].Value = item.VoteOption;
                }

                package.SaveAs(outputStream);
            }
            return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "投票记录" + DateTime.Now.ToLongDateString() + ".xlsx");
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        //public async Task<IActionResult> GetToken()
        //{
        //    //var token = qiniutool.GenerateToken();
        //    return Json(new { token });
        //}

        public async Task<IActionResult> CreateVotingPost(VoteInfo voteInfo)
        {
            dbContext.VoteInfo.Add(voteInfo);
            await dbContext.SaveChangesAsync();
            return Json(new { result = true, msg = "success" });
        }

        public async Task<IActionResult> UploadToken()
        {
            return Json(new { });
        }
    }
}