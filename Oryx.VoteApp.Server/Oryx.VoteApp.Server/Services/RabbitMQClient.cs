using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oryx.VoteApp.Server.Services
{
    public class RabbitMQClient
    {
        private Guid EnvID = Guid.NewGuid();
        private Dictionary<string, IModel> QueueDictionary = new Dictionary<string, IModel>();
        private Dictionary<string, EventingBasicConsumer> ConsumerDictionary = new Dictionary<string, EventingBasicConsumer>();
        //private ConnectionFactory factory;

        public RabbitMQClient()
        {
            //创建连接工厂
         
        }

        public RabbitMInstance this[string queueName]
        {
            get
            {
                var rabbitMInstance = new RabbitMInstance();
                rabbitMInstance.Queue = QueueDictionary[queueName + EnvID];
                rabbitMInstance.Consumer = ConsumerDictionary[queueName + EnvID];
                return rabbitMInstance;
            }
        }

        public void RegisterReciverQueue(string queueName, string exchangeName)
        {
           var factory = new ConnectionFactory
            {
                UserName = "admin",//用户名
                Password = "admin",//密码
                HostName = "101.132.130.133"//rabbitmq ip
            };
            var queueKey = queueName + EnvID.ToString();
            if (QueueDictionary.ContainsKey(queueKey))
            {
                return;
            }
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, false, true, null);
            channel.QueueDeclare(queueKey, false, false, false, null);
            channel.QueueBind(queueKey, exchangeName, string.Empty, null);
            QueueDictionary.Add(queueKey, channel);
        }

        public void RegisterQueueConsumer(string queueName)
        {
          var  factory = new ConnectionFactory
            {
                UserName = "admin",//用户名
                Password = "admin",//密码
                HostName = "101.132.130.133"//rabbitmq ip
            };
            var queueKey = queueName + EnvID;
            if (ConsumerDictionary.ContainsKey(queueKey))
            {
                return;
            }
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queueKey, false, consumer);
            ConsumerDictionary.Add(queueKey, consumer);
        }
    }

    public class RabbitMInstance
    {
        public IModel Queue { get; set; }

        public EventingBasicConsumer Consumer { get; set; }
    }
}
