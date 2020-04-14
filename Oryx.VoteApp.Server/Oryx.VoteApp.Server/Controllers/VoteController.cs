using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Oryx.VoteApp.Server.DbConextPool;
//using Orxy.Log;
using Oryx.VoteApp.Server.Models;
using Oryx.VoteApp.Server.PostManager;
using Oryx.VoteApp.Server.Ultility;
using Oryx.VoteApp.Server.ViewModel;
using Oryx.WebSocket.Extension.Utility;
using Oryx.WebSocket.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Abstractions;
using Oryx.VoteApp.Server.Services;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Caching.Distributed;

namespace Oryx.VoteApp.Server.Controllers
{
    public class VoteController : Controller
    {
        private readonly ILogger<VoteController> _logger;
        private VoteAppDbContext dbContext { get; set; }

        private OryxWebSocketPool wsPool { get; set; }
        private DbOperationMngr dbOptMng { get; set; }
        private GlobalVoteResultDic globalVoteResultDic { get; set; }
        private SingletonAppDbContext singletonDbContext { get; set; }
        private RabbitMQClient rabbitMqClient { get; set; }
        public IDistributedCache _cache;

        static int stepNum = 0;
        //private IMemoryCache _cache;
        public VoteController(
            VoteAppDbContext _dbContext,
            OryxWebSocketPool _wsPool,
            GlobalVoteResultDic _globalVoteResultDic,
            DbOperationMngr _dbOptMng,
            IDistributedCache memoryCache,
            ILogger<VoteController> logger,
            SingletonAppDbContext _singletonDbContext,
            RabbitMQClient _rabbitMqClient
            )
        {
            dbOptMng = _dbOptMng;
            dbContext = _dbContext;
            wsPool = _wsPool;
            globalVoteResultDic = _globalVoteResultDic;
            _cache = memoryCache;
            _logger = logger;
            singletonDbContext = _singletonDbContext;
            rabbitMqClient = _rabbitMqClient;
        }

        public async Task<IActionResult> GetInfo()
        {
            try
            {
                VoteInfo _voteInfo;
                var info = await _cache.GetStringAsync("getInfo");
                var jsonSetting = new JsonSerializerSettings();
                jsonSetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                if (string.IsNullOrEmpty(info))
                {
                    _voteInfo = await dbContext.VoteInfo.Where(x => x.IsOpen).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                    dbContext.Database.CloseConnection();
                    var cacheEntryOptions = new DistributedCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(5));
                    var dataJson = JsonConvert.SerializeObject(_voteInfo, jsonSetting);
                    var dataBytes = Encoding.UTF8.GetBytes(dataJson);
                    _cache.Set("getInfo", dataBytes, cacheEntryOptions);
                }
                else
                {
                    _voteInfo = JsonConvert.DeserializeObject<VoteInfo>(info);
                }

                return Json(_voteInfo, jsonSetting);
            }
            catch (Exception exc)
            {
                return Json(exc.Message);
            }
        }

        private static object lockInfo = new object();
        public async Task<IActionResult> Info(string openId)
        {
            VoteInfo _voteInfo;
            var info = await _cache.GetStringAsync("InfoByOpenId");
            var jsonSetting = new JsonSerializerSettings();
            jsonSetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            if (string.IsNullOrEmpty(info))
            {
                _voteInfo = await dbContext.VoteInfo.Where(x => x.IsOpen).Include("VoteOptions").OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                var cacheEntryOptions = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(5));
                var dataJson = JsonConvert.SerializeObject(_voteInfo, jsonSetting);
                var dataBytes = Encoding.UTF8.GetBytes(dataJson);
                _cache.Set("InfoByOpenId", dataBytes, cacheEntryOptions);
            }
            else
            {
                _voteInfo = JsonConvert.DeserializeObject<VoteInfo>(info);
            }
            return Json(_voteInfo, jsonSetting);
        }

        public async Task<IActionResult> Detail(int voteId)
        {
            return View();
        }
        static object lockCehckVoted = new object();
        public async Task<IActionResult> CheckVoted(string userId)
        {
            var currentModel = await dbContext.VoteInfo.Where(x => x.IsOpen).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            var currentVoteId = currentModel.Id;
            var voted = dbContext.VoteLog.Any(x => x.UserId == userId && x.VoteId == currentVoteId);
            return Json(new
            {
                success = true,
                voted = voted,
                isEnable = currentModel.IsEnable,
                enableTime = currentModel.EnableStartTime,
                ShouldLogin = currentModel.ShouldLogin
            });
        }

        static object lockCheckEnable = new object();
        public async Task<IActionResult> CheckEnable()
        {
            var currentVoteInfo = await dbContext.VoteInfo.Where(x => x.IsOpen).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            return Json(new
            {
                success = true,
                isEnable = currentVoteInfo.IsEnable,
                isExpired = currentVoteInfo.Expired,
                enableTime = currentVoteInfo.EnableStartTime
            });
        }

        public async Task<IActionResult> PostTest2([FromForm] VoteLog voteLog)
        {
            await dbContext.AddAsync(voteLog);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        public async Task<IActionResult> PostTest4([FromForm] VoteLog voteLog)
        {
            dbOptMng.InsertOptAutoRun(voteLog, async _localModel =>
            {
                await dbContext.VoteLog.AddRangeAsync(_localModel);
                await dbContext.SaveChangesAsync();
            }, 1500);
            return Ok();
        }

        public async Task<IActionResult> PostTest3([FromBody]PostViewModel postViewModel)
        {
            //try
            //{
            var voteLog = postViewModel.VoteLog;
            var result = await PorcessResult(postViewModel);
            var bytesData = Encoding.UTF8.GetBytes(result);
            rabbitMqClient["Broadcast"].Queue.BasicPublish("FanoutExchange", string.Empty, false, null, bytesData);

            dbOptMng.InsertOptAutoRun(voteLog, async _localModel =>
            {
                await singletonDbContext.AddRangeAsync(_localModel);
                await singletonDbContext.SaveChangesAsync();
            }, 500);
            return Ok();
        }

        private static int wsstep = 0;
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]PostViewModel postViewModel)
        {
            try
            {
                if (postViewModel.VoteLog == null)
                {
                    return BadRequest("No model value");
                }
                postViewModel.VoteLog.UserKey = postViewModel.NickName;

                dbOptMng.InsertOptAutoRun(postViewModel.VoteLog, async _localModel =>
                {
                    await singletonDbContext.AddRangeAsync(_localModel);
                    await singletonDbContext.SaveChangesAsync();
                }, 500);
                //await dbContext.VoteLog.AddAsync(postViewModel.VoteLog);
                //await dbContext.SaveChangesAsync();

                if (postViewModel.VoteLog.VoteOption != "null")
                {
                    //var result = await PorcessResult(postViewModel);
                    var result = postViewModel.VoteLog.VoteOption;// JsonConvert.SerializeObject(new { option=postViewModel.VoteLog});
                    var bytesData = Encoding.UTF8.GetBytes(result);
                    rabbitMqClient["Broadcast"].Queue.BasicPublish("FanoutExchange", string.Empty, false, null, bytesData);
                    //var wsList = wsPool.WebSocketList.Where(x => x.QueryString.Any(c => c.Value == "voteResult"));

                    //if (wsList != null && wsList.Count() > 0)
                    //{
                    //    foreach (var wsItem in wsList)
                    //    {
                    //        Console.WriteLine("ws" + wsstep);
                    //        await wsItem.OryxWebSocket.SendAsync(result).ConfigureAwait(false);
                    //    }
                    //}
                }


                return Json(new { result = "success", msg = "ok" });
            }
            catch (Exception exc)
            {
                //await Log.WriteLog(exc.Message);
                _logger.LogError(exc.Message);
                return new BadRequestResult();
            }
        }

        public async Task<IActionResult> AddBossBuff(int voteId, string option)
        {
            var buffCount = dbContext.VoteBuff.Count(x => x.VoteId == voteId);
            if (buffCount > 0)
            {
                return Json(new { success = false, message = "exited" });
            }
            var bytesData = Encoding.UTF8.GetBytes(option);
            rabbitMqClient["Broadcast"].Queue.BasicPublish("FanoutExchange", string.Empty, false, null, bytesData);
            //var wsList = wsPool.WebSocketList.Where(x => x.QueryString.Any(c => c.Value == "voteResult"));
            //if (wsList != null && wsList.Count() > 0)
            //{
            //    foreach (var wsItem in wsList)
            //    {
            //        Console.WriteLine("ws" + wsstep);

            //        await wsItem.OryxWebSocket.SendAsync(option).ConfigureAwait(false);
            //    }
            //}
            await dbContext.VoteBuff.AddAsync(new VoteBuff
            {
                VoteId = voteId,
                VoteOption = option
            });
            await dbContext.SaveChangesAsync();
            return Json(new { });
        }
        public async Task<IActionResult> AddBuff(int voteId, string option, string bossId)
        {
            var buffCount = dbContext.VoteBuff.Count(x => x.VoteId == voteId && x.BossId == bossId);
            if (buffCount < 1)
            {
                //var wsList = wsPool.WebSocketList.Where(x => x.QueryString.Any(c => c.Value == "voteResult"));
                var addBuffResult = new
                {
                    voteId = voteId,
                    option = option,
                    type = "addBuff"
                };
                var addBuffResultJson = JsonConvert.SerializeObject(addBuffResult);
                var bytesData = Encoding.UTF8.GetBytes(addBuffResultJson);
                rabbitMqClient["Broadcast"].Queue.BasicPublish("FanoutExchange", string.Empty, false, null, bytesData);
                //if (wsList != null && wsList.Count() > 0)
                //{
                //    foreach (var wsItem in wsList)
                //    {
                //        Console.WriteLine("ws" + wsstep);

                //        await wsItem.OryxWebSocket.SendAsync(addBuffResultJson).ConfigureAwait(false);
                //    }
                //}
                await dbContext.VoteBuff.AddAsync(new VoteBuff
                {
                    VoteId = voteId,
                    VoteOption = option,
                    BossId = bossId
                });
                await dbContext.SaveChangesAsync();
            }
            return Json(new { });
        }

        [HttpPost]
        public async Task<IActionResult> StartVote(int Id, int closeSecondes)
        {
            try
            {
                var timeStamp = (DateTime.Now - DateTime.Parse("1970-01-01").AddHours(8)).TotalMilliseconds;

                var voteInfo = dbContext.VoteInfo.FirstOrDefault(x => x.Id == Id);
                voteInfo.CloseSeconds = closeSecondes;

                if (!voteInfo.IsEnable)
                {
                    voteInfo.IsEnable = true;
                    voteInfo.EnableStartTime = timeStamp;
                }
                //else
                //{
                //    timeStamp = voteInfo.EnableStartTime;
                //}

                await dbContext.SaveChangesAsync();


                //if (wsPool != null)
                //{
                //    var userSockets = wsPool.WebSocketList.Where(x => x.QueryString["key"] == "userSocket");

                //    var cmdMsg = JsonConvert.SerializeObject(new { cmd = "startVote", timestamp = timeStamp });
                //    foreach (var _userSocket in userSockets)
                //    {
                //        await _userSocket.OryxWebSocket.SendAsync(cmdMsg);
                //    }
                //}
                var cmdMsg = JsonConvert.SerializeObject(new { cmd = "startVote", timestamp = timeStamp });
                var bytesData = Encoding.UTF8.GetBytes(cmdMsg);
                rabbitMqClient["BroadcastUserInfo"].Queue.BasicPublish("FanoutUserInfoExchange", string.Empty, false, null, bytesData);
                return Json(new { result = "success", msg = "ok" });
            }
            catch (Exception exc)
            {
                //await Log.WriteLog(exc.Message);
                return new BadRequestResult();
            }
        }

        [HttpPost]
        public async Task<IActionResult> EndVote(int Id)
        {
            try
            {
                var voteInfo = dbContext.VoteInfo.FirstOrDefault(x => x.Id == Id);
                voteInfo.IsEnable = false;
                voteInfo.Expired = true;
                await dbContext.SaveChangesAsync();

                //var userSockets = wsPool.WebSocketList.Where(x => x.QueryString["key"] == "userSocket");
                var cmdMsg = JsonConvert.SerializeObject(new { cmd = "endVote" });
                var bytesData = Encoding.UTF8.GetBytes(cmdMsg);
                rabbitMqClient["BroadcastUserInfo"].Queue.BasicPublish("FanoutUserInfoExchange", string.Empty, false, null, bytesData);
                //foreach (var _userSocket in userSockets)
                //{
                //    await _userSocket.OryxWebSocket.SendAsync(cmdMsg);
                //}

                return Json(new { result = "success", msg = "ok" });
            }
            catch (Exception exc)
            {
                //await Log.WriteLog(exc.Message);
                return new BadRequestResult();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetVote(int Id)
        {
            try
            {
                var voteInfo = dbContext.VoteInfo.FirstOrDefault(x => x.Id == Id);
                voteInfo.IsEnable = false;
                voteInfo.Expired = false;
                voteInfo.EnableTime = DateTime.Now.AddSeconds(20);
                var voteLogs = dbContext.VoteLog.Where(x => x.VoteId == Id);
                BlockingCollection<GlobalVoteResultDicItem> bcItem;
                var voteBuffer = dbContext.VoteBuff.Where(x => x.VoteId == Id);
                if (globalVoteResultDic.ContainsKey(Id))
                {
                    globalVoteResultDic.Remove(Id, out bcItem);
                }
                bool resultLoaded;
                if (!voteResultLoaded.TryRemove(Id, out resultLoaded))
                {
                    voteResultLoaded = new ConcurrentDictionary<int, bool>();
                }

                if (voteLogs != null && voteLogs.Count() > 0)
                {
                    dbContext.VoteLog.RemoveRange(voteLogs);
                }

                if (voteBuffer != null && voteBuffer.Count() > 0)
                {
                    dbContext.VoteBuff.RemoveRange(voteBuffer);
                }

                await dbContext.SaveChangesAsync();


                return Json(new { success = true, msg = "ok" });
            }
            catch (Exception exc)
            {
                //await Log.WriteLog(exc.Message);
                return new BadRequestResult();
            }
        }

        static object lockObj = new object();
        static object lockDicObj = new object();
        static ConcurrentDictionary<int, bool> voteResultLoaded = new ConcurrentDictionary<int, bool>();
        public async Task<string> PorcessResult(PostViewModel _postViewModel)
        {
            VoteLog _voteLog = _postViewModel.VoteLog;
            lock (lockDicObj)
            {
                if (!voteResultLoaded.ContainsKey(_voteLog.VoteId))
                {
                    voteResultLoaded.TryAdd(_voteLog.VoteId, false);
                }
                //load history data
                if (!voteResultLoaded[_voteLog.VoteId])
                {
                    //calculate history sum 
                    var _voteLogHistorey = dbContext.VoteLog.Where(x => x.VoteId == _voteLog.VoteId).GroupBy(x => x.VoteOption).Select(x => new
                    {
                        _key = x.Select(c => c.VoteOption).First(),
                        _count = x.Count()
                    }).ToDictionary(x => x._key, y => y._count);

                    //mapping to globalVoteResult 
                    var logList = new BlockingCollection<GlobalVoteResultDicItem>();
                    foreach (var item in _voteLogHistorey)
                    {
                        logList.Add(new GlobalVoteResultDicItem
                        {
                            voteOption = item.Key,
                            voteSum = item.Value
                        });
                    }
                    // load history data
                    globalVoteResultDic.TryAdd(_voteLog.VoteId, logList);

                    voteResultLoaded[_voteLog.VoteId] = true;
                }
            }
            //check is loaded history data 
            if (!globalVoteResultDic.VoteLogList.Contains(_voteLog))
            {
                globalVoteResultDic.VoteLogList.Add(_voteLog);
            }
            //else
            //{
            //    return "exist";
            //}
            return await Task.Run(() =>
            {

                if (!globalVoteResultDic.ContainsKey(_voteLog.VoteId))
                {
                    globalVoteResultDic.TryAdd(_voteLog.VoteId, new BlockingCollection<GlobalVoteResultDicItem>
                        {
                            new GlobalVoteResultDicItem {
                                  voteOption = _voteLog.VoteOption,
                                   voteSum =1
                             }
                        });
                }
                else
                {
                    var itemList = globalVoteResultDic[_voteLog.VoteId];
                    if (!itemList.Any(x => x.voteOption == _voteLog.VoteOption))
                    {
                        itemList.Add(new GlobalVoteResultDicItem
                        {
                            voteOption = _voteLog.VoteOption,
                            voteSum = 1
                        });
                    }
                    else
                    {
                        var item = itemList.FirstOrDefault(x => x.voteOption == _voteLog.VoteOption);
                        if (item != null)
                        {
                            item.voteSum += 1;
                        }
                    }
                }
                var voteResultWrapper = new
                {
                    globalVoteResultDic,
                    nickName = _postViewModel.NickName,
                    avatar = _postViewModel
                };
                return JsonConvert.SerializeObject(voteResultWrapper);

            });
        }
    }
}