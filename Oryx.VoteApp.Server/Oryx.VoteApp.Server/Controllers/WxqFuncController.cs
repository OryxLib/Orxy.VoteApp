using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Oryx.VoteApp.Server.Models;
using Oryx.VoteApp.Server.Services;
using Oryx.VoteApp.Server.ViewModel;
using Oryx.WebSocket.Extension.Utility;
using Oryx.WebSocket.Infrastructure;
using RabbitMQ.Client.Events;

namespace Oryx.VoteApp.Server.Controllers
{
    public class WxqFuncController : Controller
    {
        private OryxWebSocketPool wsPool;
        private VoteAppDbContext dbContext { get; set; }
        private RabbitMQClient rabbitMqClient { get; set; }
        public WxqFuncController(OryxWebSocketPool _wsPool,
            VoteAppDbContext _dbContext,
             RabbitMQClient _rabbitMqClient
            )
        {
            wsPool = _wsPool;
            dbContext = _dbContext;
            rabbitMqClient = _rabbitMqClient;
      
        }

     
        public IActionResult Index()
        {
            return View();
        }

        public async Task PostCheckingData([FromBody]WxqViewModel wxqModel)
        {
            var wxqData = JsonConvert.DeserializeObject<WxqViewModel>(wxqModel.msg);

            await dbContext.WxqLog.AddAsync(new WxqLog
            {
                Avarta = wxqData.avarta,
                Msg = wxqData.msg,
                MsgStatus = MsgStatus.Created,
                MsgType = wxqData.msgType,
                NickName = wxqData.nickName
            });
            await dbContext.SaveChangesAsync();
            //var websocketList = wsPool.WebSocketList.Where(x => x.QueryString["key"] == "wxqcheck");
            //if (websocketList != null)
            //{
            //    foreach (var ws in websocketList)
            //    {
            //        await ws.OryxWebSocket.SendAsync(wxqModel.msg);
            //    }
            //}
        }

        public async Task<IActionResult> PullWxqLog()
        {
            var wxqLogList = await dbContext.WxqLog.OrderByDescending(x => x.Id).Where(x => x.MsgStatus == MsgStatus.Created).Take(10).ToListAsync();
            foreach (var item in wxqLogList)
            {
                item.MsgStatus = MsgStatus.Proccessed;
            }
            await dbContext.SaveChangesAsync();
            return Json(wxqLogList);
        }

        public async Task<IActionResult> PostCheckedMsg(int Id)
        {
            try
            {
                var wxqLogEntity = await dbContext.WxqLog.FirstOrDefaultAsync(x => x.Id == Id);
                wxqLogEntity.MsgStatus = MsgStatus.Checked;
                await dbContext.SaveChangesAsync();

                var msg = JsonConvert.SerializeObject(wxqLogEntity);
                var bytesData = Encoding.UTF8.GetBytes(msg);
                rabbitMqClient["BroadcastWxq"].Queue.BasicPublish("FanoutWxqExchange", string.Empty, false, null, bytesData);
                //var websocketList = wsPool.WebSocketList.Where(x => x.QueryString["key"] == "wxq");
                //if (websocketList != null)
                //{
                //    foreach (var ws in websocketList)
                //    {
                //        await ws.OryxWebSocket.SendAsync(msg);
                //    }
                //}
            }
            catch (Exception exc)
            {
                return Json(new { success = false, msg = exc.Message });
            }
            return Json(new { success = true, msg = "success" });
        }

        public async Task<IActionResult> PostCheckedPic(int Id)
        {
            try
            {
                var wxqLogEntity = await dbContext.WxqLog.FirstOrDefaultAsync(x => x.Id == Id);
                wxqLogEntity.MsgStatus = MsgStatus.Checked;
                await dbContext.SaveChangesAsync();

                var msg = JsonConvert.SerializeObject(wxqLogEntity);
                var bytesData = Encoding.UTF8.GetBytes(msg);
                rabbitMqClient["BroadcastWxqPic"].Queue.BasicPublish("FanoutWxqPicExchange", string.Empty, false, null, bytesData);
                //var websocketList = wsPool.WebSocketList.Where(x => x.QueryString["key"] == "wxqpic");
                //if (websocketList != null)
                //{
                //    var msg = JsonConvert.SerializeObject(wxqLogEntity);
                //    foreach (var ws in websocketList)
                //    {
                //        await ws.OryxWebSocket.SendAsync(msg);
                //    }
                //}
            }
            catch (Exception exc)
            {
                return Json(new { success = false, msg = exc.Message });
            }
            return Json(new { success = true, msg = "success" });
        }

        public async Task<IActionResult> DeleteMsg(int Id)
        {
            try
            {
                var wxqLogEntity = await dbContext.WxqLog.FirstOrDefaultAsync(x => x.Id == Id);
                wxqLogEntity.MsgStatus = MsgStatus.Deleted;
                await dbContext.SaveChangesAsync();
            }
            catch (Exception exc)
            {
                return Json(new { success = false, msg = exc.Message });
            }
            return Json(new { success = true, msg = "" });
        }

        public async Task<IActionResult> WxqData(int skipCount, int pageSize = 20)
        {
            var data = await dbContext.WxqLog.Where(x => !x.Msg.Contains("undefined")).OrderBy(x => x.Id).Skip(skipCount).Take(20).ToListAsync(); ;
            return Json(data);
        }

        public async Task<IActionResult> WxqDataCount()
        {
            var count = await dbContext.WxqLog.Where(x => !x.Msg.Contains("undefined")).OrderBy(x => x.Id).CountAsync();
            return Json(new { success = true, count = count });
        }
    }
}