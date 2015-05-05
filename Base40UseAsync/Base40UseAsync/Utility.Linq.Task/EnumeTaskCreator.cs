using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fjc.AsyncUtility
{
    /*--------------------------------------------------------------
    IEnumerable<Task<T>>から非同期処理を実行し結果を列挙します。
    使用方法：  var Linq_Tasks = Enumerable.Range(1,100).Select(CreateTask);
              var AsyncResult = new EnumeTaskCreator<NameInfo>(Linq_taks);
              foreach (var item in Async03)Console.WriteLine(item);
    ToDo:   Task Cancelle機能の追加
    */

    /// <summary>
    /// IEnumerable<Task<T>>から非同期処理を実行し結果を列挙します。
    /// </summary>
    public class EnumeTaskCreator<T>:IEnumerable<T> 
    {
        List<Task<T>> _Tasks;
        public List<Task<T>> Tasks
        {
            get { return _Tasks; }
        }
        public EnumeTaskCreator(IEnumerable<Task<T>> task)
        {
            _Tasks = task.ToList();
        }
        //終了したタスクから結果を戻す。
        public IEnumerator<T> GetEnumerator()
        {
            //必要に応じてキャッシュを利用することも考えるべき
            var tasks = new List<Task<T>>(_Tasks);
            while (tasks.Count > 0)
            {
                var endTask = Task.WhenAny(tasks);
                var t = endTask.Result;
                if (!t.IsCompleted) continue;
                tasks.Remove(t);
                
                yield return t.Result;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() 
        {
            return this.GetEnumerator(); 
        } 
    }
    /*-------------------------------------------------------------------
     *  Linq To Task で非同期(IEnumerable<Task<T>>)を同期(IEnumerable<T>) 
     *  に変換する拡張クラス  
     *  備考: Linq Generic拡張Mtthodは戻り値がvoid型とTResultの二つMethodを定義
     *-------------------------------------------------------------------*/
    public static class IEnumerableTaskExtention
    {
        //課題　発生　Yield Return は　デリゲートの中で使用できないのです。　
        private static IEnumerable<TResult> RecurSerialProcess<T,TResult>(IEnumerator<Task<T>> IEnum,Func<T,TResult> Dlgt)
        {
            if(IEnum.MoveNext())
            {
                var tsk = IEnum.Current.ContinueWith(t => t.Result);
                yield return Dlgt(tsk.Result);
                var ResEnum =RecurSerialProcess(IEnum,Dlgt);
                foreach (var item in ResEnum)
                {
                    yield return item;
                }
            }   
        }
        //
        private static IEnumerable<TResult> LoopSerialProcess<T,TResult>(IEnumerator<Task<T>> IEnum,Func<T,TResult> Dlgt)
        {
            while (IEnum.MoveNext())
            {
                var tsk = IEnum.Current.ContinueWith(t => t.Result);
                yield return Dlgt(tsk.Result);
            }
        }
//        private static IEnumerable<TResult> RecTask<T,TResult>(Func<T,TResult> func,Task<T> tsk){
//            yield return  func(tsk.Result);
//            RecurSerialProcess(IEnum,Dlgt);
//        }
        public static IEnumerable<TResult> SelialProcessing<T,TResult>( this IEnumerable<Task<T>> IEnum,Func<T,TResult> actDlgt)
        {
            Task.Factory.StartNew(() =>
                {
                    var IRator = IEnum.GetEnumerator();
                    while (IRator.MoveNext())
                    {
                        var tsk = IRator.Current.ContinueWith(t => t.Result);
                        yield return actDlgt(tsk.Result);
                    }
                });
//            //var Res = RecurSerialProcess(enumTask.GetEnumerator(), actDlgt);
//            //return Res;
            //return LoopSerialProcess(enumTask.GetEnumerator(), actDlgt);
//            var terator = enumTask.GetEnumerator();
//
//            Action a = null;
            //            //Action a() デリゲートを変換し
//            a = () =>
//                {
//                    if(terator.MoveNext())
//                    {
//                        var tsk = terator.Current;
//                        tsk.ContinueWith(t =>
//                            {
            //ラムダ式の中でyield return は使えないです。なので上記のRecurSerialProcessメソッドで再帰yield return やってますまだ間違ってるかの性あり５月５日深夜
//                                yield return actDlgt(t.Result);
//                                a();
//                            });
//                    }
//                };
//
//            a();
        }
        public static void SelialProcessing<T>( this IEnumerable<Task<T>> enumTask,Action<T> actDlgt)
        {
            var terator = enumTask.GetEnumerator();

            Action a = null;

            a = () =>
            {
                if(terator.MoveNext())
                {
                    var tsk = terator.Current;
                    tsk.ContinueWith(t =>
                    {
                        actDlgt(t.Result);
                        a();
                    });
                }
            };

            a();
        }
        public static object MultiProcessing<T>(this IEnumerable<Task<T>> enumTask, Action<T> actDlgt)
        {
            var tsks = new EnumeTaskCreator<T>(enumTask);
            foreach (var item in tsks)
            {
                actDlgt(item);
            }
            return null;
        }
        public static IEnumerable<TResult> MultiProcessing<T, TResult>(this IEnumerable<Task<T>> enumTask, Func<T, TResult> actDlgt)
        {
            var tsks = new EnumeTaskCreator<T>(enumTask);
            foreach (var item in tsks)
            {
                yield return actDlgt(item);
            }
        }
    }
}

