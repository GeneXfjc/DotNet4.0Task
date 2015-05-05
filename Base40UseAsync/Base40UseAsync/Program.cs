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

            var IDs = Enumerable.Range(0, 10);
            var used = new UseAsyncClass("Ramdm");
            sw.Start();
            #region 同期で実施
            //StopWatchWrite(sw, "StartSync");
            ////Sync
            //foreach (var item in IDs.Select(used.AsyncMethod))
            //{
            //    //結局ここで待機Resultするので同期処理となります。
            //    Console.WriteLine(item.Result);
            //}
            //StopWatchWrite(sw, "EndSync");
            #endregion

            ////Async
            //StopWatchWrite(sw, "InitAsyncStart");
            //IEnumerable<Task<string>>が帰っていきます。
            //var tasks = IDs.Select(new UseAsyncClass("Ramdm").AsyncMethod40);
                
            //StopWatchWrite(sw, "InitAsyncEnd--");
            
            //StopWatchWrite(sw, "StartTasks");
            ////ここで一気に開始
            //var taskEnume = new EnumeTaskCreator<string>(tasks);
            //StopWatchWrite(sw, "End  Tasks");
            ////Async01　非同期処理は最初から最後まで非同期でロジックを組むこと
            ////非同期処理結果を表示
            //StopWatchWrite(sw, "StartAsync");
            //foreach (var item in taskEnume) Console.WriteLine(item);

            //StopWatchWrite(sw, "End Async");

            //かなり見通しの良いラムダ式
//            var res = IDs.Select(new UseAsyncClass("Ramdm").AsyncMethod40)
//                .MultiProcessing(t => Console.WriteLine(t));

            //Async 02 
            //非同期処理後になんらかの加工を実施するLinqここでは処理されない。
            var user02 = new UseAsyncClass("Ramdm");


            //非同期内のラムダ式が美しくない
//            var ProcessedTask = IDs.Select(i =>
//            {
//                //非同期処理後に戻り値データをTaskをContinueWith内で2次処理する。
//                //終了タスクから戻り値を取り出すresTask.ResultはNGここでWait()しない
//                return user02.AsyncMethod40(i)
//                    .ContinueWith( tRes=> new NameInfo(i,tRes)  );
//            });
//
//            //上記をもう少しスマートに書き直すと
//            var ProcessedTask01 = IDs.Select(i => user02.AsyncMethod40(i))
//                .Select(t => t.ContinueWith(tRes => new NameInfo(tRes.Id, tRes)));

            //Linqで記述したProcessdTaskはここではじめて遅延処理される。
            //var TaskList = new EnumeTaskCreator<NameInfo>(ProcessedTask);
            //ProcessedTaskで2次処理されたTaskのIEnumerable<>を処理する
            //foreach (var item in TaskList) Console.WriteLine(item);

            //拡張メソッドで対応してる
//            ProcessedTask01.SelialProcessing( t => Console.WriteLine(t));

            //全面的に拡張メソッドUtility.Linq.Taskを活用
            IDs.Select(i => used.AsyncMethod40(i))
                .SelialProcessing(t => 
                {
                        Console.WriteLine (t);
                        return 1;
                });

            Console.ReadKey();
        }
        public static void StopWatchWrite(Stopwatch sw,string msg)
        {
            Console.WriteLine("{0}:{1}",msg,sw.Elapsed.ToString());
        }

    }
}
