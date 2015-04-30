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
    */

    /// <summary>
    /// IEnumerable<Task<T>>から非同期処理を実行し結果を列挙します。
    /// </summary>
    public class EnumeTaskCreator<T>:IEnumerable<T>
    {
        List<Task<T>> _Tasks;
        public EnumeTaskCreator(IEnumerable<Task<T>> task)
        {
            _Tasks = task.ToList();
        }
        public IEnumerator<T> GetEnumerator()
        {
            //必要に応じてキャッシュを利用することも考えるべき
            //特にキャンセルがこの処理から利用できると嬉しいかも
            var tasks = new List<Task<T>>(_Tasks);
            while (tasks.Count > 0)
            {
                var endTask = Task.WhenAny(tasks);
                var t = endTask.Result;
                tasks.Remove(t);
                yield return t.Result;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() 
        {
            return this.GetEnumerator(); 
        } 
    }
}

