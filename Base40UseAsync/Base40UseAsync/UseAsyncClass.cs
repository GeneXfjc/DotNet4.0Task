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
        //同期メソッド
        public string SyncMethod(int i)
        {
            //本来ならSocket等でブロッキングされる。
            System.Threading.Thread.Sleep(1000);
            return string.Format("Name{0:D3}", i);
        }
        //非同期メソッド
        public Task<string> AsyncMethod(int i)
        {
            return Task.Factory.StartNew(() =>
            {
                return SyncMethod(i);
            });
        }
    }
}

