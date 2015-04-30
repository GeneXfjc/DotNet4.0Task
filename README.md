# DotNet4.0Task
C#における.Net4.0 Task非同期の使い方

### 使い方
* 直前までTask.Resultを使用しないData Classを活用する。
### 便利なUtility
* 拡張Method
   Extentions
* Generic
   各種Data Classを使い易いIEnumerable<T>にコンバート
* IEnumerable<T>
   Linq To Task で非同期処理するとIEnumerable<Task<T>>となるので、扱い易いIEnumerable<T>
   にするにはyield return を活用
