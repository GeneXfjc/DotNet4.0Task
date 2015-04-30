using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fjc.AsyncUtility
{
    

    public class UseClass
    {
        public UseClass()
        {
        }
        public string SyncMethod(int i)
        {
            return string.Format("Name{0:D3}", i);
        }
        public Task<string> AsyncMethod(int i)
        {
            return Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(800);
                return SyncMethod(i);
            });
        }
    }
}

