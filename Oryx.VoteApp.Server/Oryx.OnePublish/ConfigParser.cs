using Oryx.OnePublish.ConfigArchticle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oryx.OnePublish
{
    public class ConfigParser
    {
        private readonly string configdir = AppContext.BaseDirectory + "/config_sample";

        public Dictionary<string, ConfigStruct> LoadConfig()
        {
            var confStructList = new Dictionary<string, ConfigStruct>();

            var configFIleList = LoadFile();

            foreach (var confile in configFIleList)
            {
                var configStruct = ParseConfContent(confile.Value);
            }

            return confStructList;
        }

        private ConfigStruct ParseConfContent(Queue<string> confContentQueue)
        {
            var clientStruct = new ClientStruct();
            var serverList = new List<ServerStruct>();

            while (confContentQueue.Count > 0)
            {
                switch (confContentQueue.Peek())
                {
                    case "[client]":
                        clientStruct = parseClient(confContentQueue);
                        break;
                    case "[server]":
                        break;
                }
            }
            return new ConfigStruct { Client = clientStruct, ServerList = serverList };
        }

        private ClientStruct parseClient(Queue<string> confContentQueue)
        {
            var clientStruct = new ClientStruct();
            while (true)
            {
                var willClose = false;
                switch (confContentQueue.Peek())
                {
                    default:
                        var kvp = confContentQueue.Dequeue().Split(' ');
                        break;
                    case "[client]":
                        break;
                    case "[server]":
                        willClose = true;
                        break;
                }
                if (willClose)
                {
                    return clientStruct;
                }
            }
        }

        //get all conf files
        private Dictionary<string, Queue<string>> LoadFile()
        {
            var fileConfQueue = new Dictionary<string, Queue<string>>();
            var confs = Directory.GetFileSystemEntries(configdir);
            var taskList = new List<Task>();
            foreach (var confPath in confs)
            {
                var _taskHandler = Task.Run(async () =>
                   {
                       var fileContent = await File.ReadAllLinesAsync(confPath);
                       var queue = new Queue<string>();
                       foreach (var queueitem in fileContent)
                       {
                           queue.Enqueue(queueitem);
                       }

                       fileConfQueue.Add(confPath, queue);
                   });
                taskList.Add(_taskHandler);
            }
            Task.WaitAll(taskList.ToArray());
            return fileConfQueue;
        }
    }
}
