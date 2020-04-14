using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

using Oryx.VoteApp.Server.Models;
using Oryx.VoteApp.Server.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Oryx.PressureTest
{
    [TestClass]
    public class UnitTest1
    {
        static int step = 0;
        [TestMethod]
        public void PostTest()
        {
            var errSum = 0;
            var httpClient = new HttpClient();
            var rdm = new Random();
            var voteLog = new VoteLog()
            {
                UserId = "1",
                UserKey = "2" + (rdm.Next(10000)),
                VoteId = 6,
                VoteOption = "啊 "
            };
            var voteLogList = new VoteLog[4];
            voteLogList[0] = new VoteLog
            {
                UserId = "1",
                UserKey = "2" + (rdm.Next(10000)),
                VoteId = 1,
                VoteOption = "这个呢?"
            };
            voteLogList[1] = new VoteLog
            {
                UserId = "1",
                UserKey = "2" + (rdm.Next(10000)),
                VoteId = 1,
                VoteOption = "这是谁"
            };
            voteLogList[2] = new VoteLog
            {
                UserId = "1",
                UserKey = "2" + (rdm.Next(10000)),
                VoteId = 1,
                VoteOption = "这又是谁"
            };
            voteLogList[3] = new VoteLog
            {
                UserId = "1",
                UserKey = "2" + (rdm.Next(10000)),
                VoteId = 1,
                VoteOption = "那个呢?"
            };
            var optRdm = new Random();

            var jsonSetting = new JsonSerializerSettings();
            jsonSetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //var json = JsonConvert.SerializeObject(voteLog, jsonSetting);
            //var content = new StringContent(json, Encoding.UTF8, "application/json");
            var taskList = new List<Task>();
            for (int i = 0; i < 2000; i++)
            {
                var taskResult = Task.Run(async () =>
                  {
                      try
                      {
                          var targetOpt = voteLogList[optRdm.Next(0, 4)];
                          var pvm = new PostViewModel();
                          pvm.VoteLog = new VoteLog
                          {
                              CreateTime = DateTime.Now,
                              UserId = "123",
                              VoteId = 15,
                              VoteOption = "Photo 08"
                          };
                          var json = JsonConvert.SerializeObject(pvm, jsonSetting);
                          //var content = new StringContent(json, Encoding.UTF8, "application/json");
                          var content = new StringContent(json, Encoding.UTF8, "application/json");
                          //var result = await httpClient.PostAsync("http://voteapp.oryxl.com/vote/Post", content);
                          var wc = new WebClient();
                          var result = await httpClient.PostAsync("http://localhost:5000/vote/Post?key=Linengneng", content);
                          Debug.WriteLine("次数: " + ++step + " : " + result.StatusCode);
                          if (result.StatusCode != System.Net.HttpStatusCode.OK)
                          {
                              errSum++;
                              //await Log.WriteLog("Unit Test Status Code : " + result.StatusCode + "\n");
                          }
                      }
                      catch (System.Exception exc)
                      {
                          //await Log.WriteLog("Unit Test Err : " + exc.Message + "\n");
                      }

                  });
                taskList.Add(taskResult);
            }

            while (!taskList.All(x => x.Status == TaskStatus.RanToCompletion))
            {

            }
            Assert.IsTrue(errSum == 0);
        }
    }
}
