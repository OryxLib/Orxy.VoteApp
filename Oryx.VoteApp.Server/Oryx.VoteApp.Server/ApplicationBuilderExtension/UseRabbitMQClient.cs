using Microsoft.AspNetCore.Builder;
using Oryx.VoteApp.Server.Services;
using Oryx.WebSocket.Extension.Utility;
using Oryx.WebSocket.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oryx.VoteApp.Server.ApplicationBuilderExtension
{
    public static class RabbitMQClientExtension
    {
        private static OryxWebSocketPool wsPool { get; set; }

        public static IApplicationBuilder UseRabbitMQClient(this IApplicationBuilder app)
        {
            var rabbitMQClient = (RabbitMQClient)app.ApplicationServices.GetService(typeof(RabbitMQClient));
            wsPool = (OryxWebSocketPool)app.ApplicationServices.GetService(typeof(OryxWebSocketPool));
            rabbitMQClient.RegisterReciverQueue("Broadcast", "FanoutExchange");
            rabbitMQClient.RegisterQueueConsumer("Broadcast");
            rabbitMQClient.RegisterReciverQueue("BroadcastUserInfo", "FanoutUserInfoExchange");
            rabbitMQClient.RegisterQueueConsumer("BroadcastUserInfo");
            rabbitMQClient.RegisterReciverQueue("BroadcastWxq", "FanoutWxqExchange");
            rabbitMQClient.RegisterQueueConsumer("BroadcastWxq");
            rabbitMQClient.RegisterReciverQueue("BroadcastWxqPic", "FanoutWxqPicExchange");
            rabbitMQClient.RegisterQueueConsumer("BroadcastWxqPic");
            rabbitMQClient["Broadcast"].Consumer.Received += Consumer_Received;
            rabbitMQClient["BroadcastUserInfo"].Consumer.Received += BroadcastUserInfoConsumer_Received;
            rabbitMQClient["BroadcastWxq"].Consumer.Received += BroadcastWxqConsumer_Received; ;
            rabbitMQClient["BroadcastWxqPic"].Consumer.Received += BroadcastWxqPicPicConsumer_Received;
            return app;
        }
        private static void Consumer_Received(object sender, RabbitMQ.Client.Events.BasicDeliverEventArgs e)
        {
            var result = Encoding.UTF8.GetString(e.Body);
            
            var wsList = wsPool.WebSocketList.Where(x => x.QueryString.Any(c => c.Value == "voteResult"));

            if (wsList != null && wsList.Count() > 0)
            {
                foreach (var wsItem in wsList)
                {
                    wsItem.OryxWebSocket.SendAsync(result).Wait();
                }
            }
        }

        private static void BroadcastUserInfoConsumer_Received(object sender, RabbitMQ.Client.Events.BasicDeliverEventArgs e)
        {
            if (wsPool != null)
            {
                var result = Encoding.UTF8.GetString(e.Body);
                foreach (var item in wsPool.WebSocketList.Where(x => x.QueryString["key"] == "userSocket"))
                {
                    item.OryxWebSocket.SendAsync(result).Wait();
                }
            }
        }

        private static void BroadcastWxqConsumer_Received(object sender, RabbitMQ.Client.Events.BasicDeliverEventArgs e)
        {
            var result = Encoding.UTF8.GetString(e.Body);
            var wsList = wsPool.WebSocketList.Where(x => x.QueryString.Any(c => c.Value == "wxq"));
            if (wsList != null && wsList.Count() > 0)
            {
                foreach (var wsItem in wsList)
                {
                    wsItem.OryxWebSocket.SendAsync(result).Wait();
                }
            }
        }

        private static void BroadcastWxqPicPicConsumer_Received(object sender, RabbitMQ.Client.Events.BasicDeliverEventArgs e)
        {
            var result = Encoding.UTF8.GetString(e.Body);
            var wsList = wsPool.WebSocketList.Where(x => x.QueryString.Any(c => c.Value == "wxqpic"));

            if (wsList != null && wsList.Count() > 0)
            {
                foreach (var wsItem in wsList)
                {
                    wsItem.OryxWebSocket.SendAsync(result).Wait();
                }
            }
        }
    }
}
