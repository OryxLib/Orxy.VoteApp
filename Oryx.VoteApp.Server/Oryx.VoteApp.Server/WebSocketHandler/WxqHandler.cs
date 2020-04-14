using Newtonsoft.Json;
using Oryx.WebSocket.Extension.Utility;
using Oryx.WebSocket.Infrastructure;
using Oryx.WebSocket.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oryx.VoteApp.Server.WebSocketHandler
{
    public class WxqHandler : IOryxHandler
    { 
        public async Task OnClose(OryxWebSocketMessage msg)
        {

        }

        public async Task OnReciveMessage(OryxWebSocketMessage msg)
        {
      
            //await msg.WebSocket.SendAsync(msg.Message);
        }
    }
}
