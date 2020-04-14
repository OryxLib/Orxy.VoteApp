using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oryx.WebSocket;
//using Oryx.WebSocket.Interface;

namespace Oryx.VoteApp.Server.Pages
{
    public class WebSocketReciverModel : PageModel
    {
        //IOryxWebSocket websocket { get; set; }

        //public WebSocketReciverModel(IOryxWebSocket _websocket)
        //{
        //    websocket = _websocket;
        //}

        public void OnGet()
        {
            //var timer = new Timer(async cb =>
            //{
            //    await websocket.SendMsg("testKey", "哈哈哈");
            //}, null, 3000, 5000);
        }
    }
}