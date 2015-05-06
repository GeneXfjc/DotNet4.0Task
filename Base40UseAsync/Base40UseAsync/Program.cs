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

            var IDs = Enumerable.Range(0, 100);
            var used = new UseAsyncClass("Ramdm");
            sw.Start();
            #region 同期で実施
            StopWatchWrite(sw, "StartSync");
            //Sync
            foreach (var item in IDs.Select(used.AsyncMethod40))
            {
                //結局ここで待機Resultするので同期処理となります。
                Console.WriteLine(item.Result);
            }
            StopWatchWrite(sw, "EndSync");
            #endregion
            //非同期処理後の２時処理は継続タスク内で処理する。
            //＜＜混ぜるな危険＞＞
            var ProcessedTask01 = IDs.Select(used.AsyncMethod)
                .Select(t => t.ContinueWith(tRes => new NameInfo(tRes.Id, tRes)));

            var ProcessedTask02 = IDs.Select(used.AsyncMethod40);
                
            //順次処理順（使用の際は別スレッド）
            Task.Factory.StartNew( () =>
                {
                    ProcessedTask01.SelialProcessing(Console.WriteLine);
                });

            //早期処理順（別スレッドで処理している。）
            ProcessedTask02.MultiProcessing(Console.WriteLine);


 
            for (int i = 0; i < 60; i++)
            {
                Thread.Sleep(1000);
                Console.WriteLine("{0:D3} : {1}",i,new String('*',80));
            }


            Console.ReadKey();
        }
        public static void StopWatchWrite(Stopwatch sw,string msg)
        {
            Console.WriteLine("{0}:{1}",msg,sw.Elapsed.ToString());
        }

    }
}
