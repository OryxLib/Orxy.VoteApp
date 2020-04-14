using Oryx.WebSocket.Infrastructure;
using Oryx.WebSocket.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oryx.WxDisplayWall.WsHandler
{
    public class ProcessWebsocketHandler : IOryxHandler
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
                Console.WriteLine("connection close!");
            });
        }
    }
}
