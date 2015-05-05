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
    public static class IEnumerableTaskExtention
    {
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

