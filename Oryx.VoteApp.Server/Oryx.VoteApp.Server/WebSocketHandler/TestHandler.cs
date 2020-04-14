using Oryx.WebSocket.Extension.Utility;
using Oryx.WebSocket.Infrastructure;
using Oryx.WebSocket.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Oryx.VoteApp.Server.WebSocketHandler
{
    public class TestHandler : IOryxHandler
    {
        public async Task OnClose(OryxWebSocketMessage msg)
        {
            await Task.Run(() =>
            {
                Console.WriteLine("connection close!");
            });
        }

        public async Task OnReciveMessage(OryxWebSocketMessage msg)
        {
            await Task.Run(() =>
            {
                Console.WriteLine(msg.Message.Trim());
                //msg.WebSocket.SendAsync("我收到了请求, 为什么发不出去?");
            });
        }
    }
}
