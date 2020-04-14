using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
//using Orxy.Log;
using ValueTestMysql.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValueTestMysql.Controllers
{
    public class VoteController : Controller
    {
        private VoteAppDbContext dbContext { get; set; }
        //private TransientVoteAppDbContext _transientDbContext { get; set; }

        //private OryxWebSocketPool wsPool { get; set; }
        //private DbOperationMngr dbOptMng22 { get; set; }
        //private GlobalVoteResultDic globalVoteResultDic { get; set; }
        static int stepNum = 0;

        public VoteController(VoteAppDbContext dbContext)
        //TransientVoteAppDbContext transientDbContext,
        //OryxWebSocketPool _wsPool,
        //GlobalVoteResultDic _globalVoteResultDic,
        //DbOperationMngr _dbOptMng2222)
        {
            //_transientDbContext = transientDbContext;
            //dbOptMng22 = _dbOptMng2222;
            this.dbContext = dbContext;
            //this.wsPool = _wsPool;
            //globalVoteResultDic = _globalVoteResultDic;
        }

        public async Task<IActionResult> Login(string openId)
        {
            var voteInfo = dbContext.VoteInfo.Include("VoteOptions").OrderByDescending(x => x.CreateTime).Take(1).FirstOrDefault();
            var result = dbContext.VoteLog.Any(x => x.VoteId == voteInfo.Id && x.UserId == openId);
            return Json(new { voted = result });
        }

        static object voteInfolocker = new object();
        public async Task<IActionResult> Info(string openId)
        {
            lock (voteInfolocker)
            {
                var voteInfo = dbContext.VoteInfo.Include("VoteOptions").OrderByDescending(x => x.Id).Take(1).FirstOrDefault();
                //var voted = dbContext.VoteLog.Any(x => x.UserId == openId && x.VoteId == voteInfo.Id);
                var jsonSetting = new JsonSerializerSettings();
                jsonSetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                return Json(voteInfo, jsonSetting);
            }
        }

        public async Task<IActionResult> Detail(int voteId)
        {
            return View();
        }

        public async Task<IActionResult> CheckVoted(string userId)
        {
            var voted = dbContext.VoteLog.Any(x => x.UserId == userId);
            return Json(new
            {
                success = true,
                voted = voted
            });
        }

        //[HttpPost]
        //[Route("CustomePostForm")]
        //public async Task<IActionResult> CustomePostForm([FromForm]VoteLog voteLog)
        //{
        //    try
        //    {
        //        await dbContext.VoteLog.AddAsync(voteLog);
        //        await dbContext.SaveChangesAsync();
        //    }
        //    catch (Exception exc)
        //    {
        //        Log.WriteLog(exc.Message);
        //    }

        //    return Json(new { result = "success", msg = "ok" });
        //    //try
        //    //{
        //    //    if (voteLog == null)
        //    //    {
        //    //        return NoContent();
        //    //    }
        //    //    var result = await PorcessResult(voteLog);
        //    //    //var ws = wsPool.WebSocketList.Find(x => x.QueryString.Any(c => c.Value == "voteResult"));
        //    //    if (ws != null)
        //    //        await ws.OryxWebSocket.SendAsync(result);
        //    //    dbOptMng22.InsertOptAutoRun(voteLog, async _localModel =>
        //    //    {
        //    //        await dbContext.AddRangeAsync(_localModel);
        //    //        await dbContext.SaveChangesAsync();
        //    //    });
        //    //    return Json(new { result = "success", msg = "ok" });
        //    //}
        //    //catch (Exception exc)
        //    //{
        //    //    await Log.WriteLog(exc.Message);
        //    //    return new BadRequestResult();
        //    //}
        //}
        //static int stepsNumbLog = 0;
        [HttpPost]
        public async Task<IActionResult> Post([FromForm]VoteLog voteLog)
        {
            await dbContext.AddRangeAsync(voteLog);
            await dbContext.SaveChangesAsync();
            return Json(new { result = "success", msg = "ok" });
            //try
            //{
            //    if (voteLog == null)
            //    {
            //        return NoContent();
            //    }
            //    var result = await PorcessResult(voteLog);
            //    //var ws = wsPool.WebSocketList.Find(x => x.QueryString.Any(c => c.Value == "voteResult"));
            //    if (ws != null)
            //        await ws.OryxWebSocket.SendAsync(result);
            //    dbOptMng22.InsertOptAutoRun(voteLog, async _localModel =>
            //    {
            //        await dbContext.AddRangeAsync(_localModel);
            //        await dbContext.SaveChangesAsync();
            //    });
            //    return Json(new { result = "success", msg = "ok" });
            //}
            //catch (Exception exc)
            //{
            //    await Log.WriteLog(exc.Message);
            //    return new BadRequestResult();
            //}
        }
        static object lockObj = new object();
        static object lockDicObj = new object();
        static Dictionary<int, bool> voteResultLoaded = new Dictionary<int, bool>();
        //public async Task<string> PorcessResult(VoteLog _voteLog)
        //{
        //    lock (lockDicObj)
        //    {
        //        if (!voteResultLoaded.ContainsKey(_voteLog.VoteId))
        //        {
        //            voteResultLoaded.Add(_voteLog.VoteId, false);
        //        }
        //        //load history data
        //        if (!voteResultLoaded[_voteLog.VoteId])
        //        {
        //            //calculate history sum 
        //            var _voteLogHistorey = dbContext.VoteLog.Where(x => x.VoteId == _voteLog.VoteId).GroupBy(x => x.VoteOption).Select(x => new
        //            {
        //                _key = x.Select(c => c.VoteOption).First(),
        //                _count = x.Count()
        //            }).ToDictionary(x => x._key, y => y._count);


        //            //mapping to globalVoteResult 
        //            var logList = new List<GlobalVoteResultDicItem>();
        //            foreach (var item in _voteLogHistorey)
        //            {
        //                logList.Add(new GlobalVoteResultDicItem
        //                {
        //                    voteOption = item.Key,
        //                    voteSum = item.Value
        //                });
        //            }
        //            // load history data
        //            globalVoteResultDic.Add(_voteLog.VoteId, logList);

        //            voteResultLoaded[_voteLog.VoteId] = true;
        //        }

        //    }
        //    //check is loaded history data 
        //    if (!globalVoteResultDic.VoteLogList.Contains(_voteLog))
        //    {
        //        globalVoteResultDic.VoteLogList.Add(_voteLog);
        //    }
        //    else
        //    {
        //        return "exist";
        //    }
        //    return await Task.Run(() =>
        //    {

        //        if (!globalVoteResultDic.ContainsKey(_voteLog.VoteId))
        //        {
        //            globalVoteResultDic.Add(_voteLog.VoteId, new List<GlobalVoteResultDicItem>
        //                {
        //                    new GlobalVoteResultDicItem {
        //                          voteOption = _voteLog.VoteOption,
        //                           voteSum =1
        //                     }
        //                });
        //        }
        //        else
        //        {
        //            var itemList = globalVoteResultDic[_voteLog.VoteId];
        //            if (!itemList.Any(x => x.voteOption == _voteLog.VoteOption))
        //            {
        //                itemList.Add(new GlobalVoteResultDicItem
        //                {
        //                    voteOption = _voteLog.VoteOption,
        //                    voteSum = 1
        //                });
        //            }
        //            else
        //            {
        //                itemList.Find(x => x.voteOption == _voteLog.VoteOption).voteSum++;
        //            }
        //        }
        //        return JsonConvert.SerializeObject(globalVoteResultDic);

        //    });
        //}
    }
}