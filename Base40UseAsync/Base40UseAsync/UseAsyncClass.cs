using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Base40UseAsync
{
    /// <summary>
    /// よくある非同期処理を含むClass
    /// </summary>
    public class UseAsyncClass
    {
        const int TIME_SP = 600;
        const int TiMe_MX = 3000;
        object LockObj = new object();
        
        int WAIT = 0;
        Random rnd = new Random(DateTime.Now.Second);

        int GetWaitTime()
        {
            return WAIT == 0 ? rnd.Next(TIME_SP, TiMe_MX) : WAIT;
        }
        //同期メソッド
        public UseAsyncClass(string wait)
        {
            if (wait == "Randm") WAIT = 0;

            int.TryParse(wait, out WAIT);
        }
        public string SyncMethod(int num)
        {
                int count = 0;
                string resMark = string.Empty;
                //本来ならSocket等でブロッキングされる。
                var wAit = GetWaitTime();
                for (int i = wAit; i > TIME_SP; i -= TIME_SP)
                {
                    System.Threading.Thread.Sleep(TIME_SP );
                    resMark += "*";
                    count++;
                }

                return CreateName(wAit, num, resMark);
        }
        //非同期メソッド
        public Task<string> AsyncMethod(int i)
        {
            return Task.Factory.StartNew(() =>
            {
                return SyncMethod(i);
            });
        }
        //非同期メソッド　本来はこれで実施
        public Task<string> AsyncMethod40(int i)
        {
            var wai = GetWaitTime();
            return Task.Delay(wai)
                .ContinueWith(_ => CreateName(wai, i,new string('*',wai/TIME_SP )));
                
        }
        string CreateName(int waite, int num)
        {
            return string.Format("Name{0:D3}({1:D6})",num,waite);
        }
        string CreateName(int waite, int num ,string jobMark)
        {
            return string.Format("Name{0:D3}({1:D6}) {3:D4} {2}", num, waite,jobMark,waite/TIME_SP );
        }
    }
}

