using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Oryx.VoteApp.Server.PostManager
{
    public class DbOperationMngr
    {
        private DbOperationQueue globalQueue { get; set; }

        public DbOperationMngr(DbOperationQueue _queue)
        {
            globalQueue = _queue;
        }


        static Timer _timer;
        static bool timerIsRunning = false;
        static object blockObj = new object();
        static int timerSteps = 0;
        static bool isDbRuning = false;

        static int testAddNum = 0;
        static int testProcessSum = 0;
        //每过200毫秒递归一次, 取出maxBatch项
        //若循环3次未有新的队列入队则退出timer
        //未防止新入队时有新的元素入队, 要给入队操作加锁
        //排队入队, 异步出对
        //Note : 单例情况!
        public void InsertOptAutoRun<T>(T _obj, Func<List<T>, Task> optsFunc, int maxBatchSize = 100)
        {
            lock (blockObj)
            {
                globalQueue.Enqueue(_obj);
                //reset timerStemps
                timerSteps = 0;
                if (!timerIsRunning)
                {
                    timerIsRunning = true;

                    _timer = new Timer(new TimerCallback(timerCbObjMapQueue =>
                    {
                        if (isDbRuning)
                        {
                            return;
                        }
                        isDbRuning = true;

                        var _currentQueue = (DbOperationQueue)timerCbObjMapQueue;

                        //get top maxBatchSize item
                        var topItemList = new List<T>();

                        for (int i = 0; i < (_currentQueue.Count > maxBatchSize ? maxBatchSize : _currentQueue.Count); i++)
                        {
                            Debug.WriteLine("testAddNum==================== : " + ++testAddNum);
                            topItemList.Add((T)_currentQueue.Dequeue());
                        }


                        testProcessSum += topItemList.Count;
                        Debug.WriteLine("testProcessSum=================== : " + testProcessSum);
                        var task = optsFunc(topItemList);
                        Task.WaitAll(task);
                        isDbRuning = false;
                        //if empty queue , plus step one
                        if (_currentQueue.Count == 0)
                        {
                            timerSteps++;
                        }
                        //if empty queue and run 3 times , release resource
                        if (timerSteps == 3)
                        {
                            timerIsRunning = false;
                            _timer.Dispose();
                            _timer = null;
                        }
                    }), globalQueue, 500, 500);
                }
            }
        }
        static bool isRunning = false;
        public void ExcuteWait<T>(T _obj, Func<List<T>, Task> optsFunc, int maxBatchSize = 100)
        {
            globalQueue.Enqueue(_obj);

            if (!isRunning)
            {
                isRunning = true;


            }
        }
    }
}
