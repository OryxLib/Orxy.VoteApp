using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Oryx.WebSocket.Interface;

namespace Oryx.WebSocket.Infrastructure
{
    public class OryxWebSocketPool
    {
        public List<OryxWebSocketPoolItem> WebSocketList { get; set; } = new List<OryxWebSocketPoolItem>();

        public void Add(string path, IOryxHandler handler)
        {
            WebSocketList.Add(new OryxWebSocketPoolItem
            {
                Handler = handler,
                Path = path
            });
        }
    }
}
