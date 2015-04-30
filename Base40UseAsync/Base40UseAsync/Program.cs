using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Fjc.DataSrc;
using Fjc.AsyncUtility;

namespace Base40UseAsync
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            var IDs = Enumerable.Range(11, 10);
            var used = new UseAsyncClass();
            //Async
            var tasks = IDs.Select(used.AsyncMethod);
            var taskEnume = new EnumeTaskCreator<string>(tasks);
            StopWatchWrite(sw, "StartSync");
            //Sync
            foreach (var item in IDs.Select(i=>used.AsyncMethod(i)))
            {
                    Console.WriteLine (item.Result);
            }
            StopWatchWrite(sw, "EndSync");


            //Async01
            //非同期処理結果を表示
            StopWatchWrite(sw, "StartAsync");
            foreach (var item in taskEnume) Console.WriteLine(item);
            StopWatchWrite(sw, "EndAsync");

            //非同期処理後になんらかの加工を実施するLinqここでは処理されない。
            var ProcessedTask = IDs.Select(i =>
            {
                var resTask =used.AsyncMethod(i);
                //非同期処理後に戻り値データをTaskをContinueWith内で2次処理する。
                //終了タスクから戻り値を取り出すresTask.ResultはNGここでWait()しない
                return  resTask.ContinueWith(
                            tRes=> new NameInfo(i,tRes)
                        );
            });
            //Linqで記述したProcessdTaskはここではじめて遅延処理される。
            var TaskList = new EnumeTaskCreator<NameInfo>(ProcessedTask);
            //ProcessedTaskで2次処理されたTaskのIEnumerable<>を処理する
            foreach (var item in TaskList) Console.WriteLine(item);
            
        }
        public static void StopWatchWrite(Stopwatch sw,string msg)
        {
            Console.WriteLine("{0}:{1}",msg,sw.ElapsedMilliseconds);
        }

    }
}
