### 概要

- C#の同期プリミティブ/スレッドのシグナリングに関するサンプルコード集.
- source/Program.cs以下のサンプルをコメントイン/アウトして使ってください.

### 排他制御に関するクラス群

#### Semaphore/SemaphoreSlim

- https://learn.microsoft.com/ja-jp/dotnet/api/system.threading.semaphore?view=net-8.0
- https://learn.microsoft.com/ja-jp/dotnet/api/system.threading.semaphoreslim?view=net-8.0
- 最も原始的な同期プリミティブ.
- 共有リソースに対するアクセス数を制限する.
- 共有リソースに対し､N個のスレッドにアクセス制限を許すのがカウンティングセマフォ. 2値で表現し単一のスレッドにしかアクセスを許さないのがバイナリセマフォ.
- C#の場合はカウンティングセマフォ.
- SemaphoreSlimは､名前付きセマフォなどの機能が使えない軽量版. セマフォを使うことがあれば､まずこちらの使用を検討すると良いだろう.

#### Mutex

- https://learn.microsoft.com/ja-jp/dotnet/api/system.threading.mutex?view=net-8.0
- 共有リソースへの排他制御を実現するクラス.
- セマフォとの違いは､該当スレッドがロック所有権を持っているかのチェック機構があること.
- なので､セマフォより遅いが､誤り検知・安全性という意味ではミューテックスの方が好ましい.
- 共有リソースへのアクセスを制御するためのものであり､デッドロックを避ける観点で､ロックが必要な変数ごとにミューテックスを用意するべき. Rustでは､ミューテックスクラス自体が､保護対象の変数を指定する設計になっている.

#### Interlocked

- https://learn.microsoft.com/ja-jp/dotnet/api/system.threading.interlocked?view=net-8.0
- 少し特殊で､以下のようなプリミティブなケースの場合にのみ使えるクラス
    - 数値のインクリメント・デクリメント
    - 値の代入
    - 値を比較しつつ代入する
- 排他制御の中ではかなり高速な部類なので､Interlockedクラスで済むケースの場合は､積極的に使っていくべき.

#### ReadWriterLock/ReadWriterLockSlim

- https://learn.microsoft.com/ja-jp/dotnet/api/system.threading.readerwriterlock?view=net-8.0
- https://learn.microsoft.com/ja-jp/dotnet/api/system.threading.readerwriterlockslim?view=net-8.0
- ReadによるロックとWriteによるロックを区別できるのなら､この同期プリミティブを使うと良い.
- マルチスレッドプログラミングの制約に基づき､以下のような挙動になる.
    - Readロック取得取得中: Read処理はブロックされない/Write処理はブロックされる
    - Writeロック取得中: Read処理はブロックされる/Write処理はブロックされる
- ReadWriterLockSlimは､再帰などのサポートが失われた軽量版. 再帰しないならこちらを使おう.

#### SpinLock

- https://learn.microsoft.com/ja-jp/dotnet/api/system.threading.spinlock?view=net-8.0
- これも排他制御を表すクラスだが､待機中の動作が特徴的な同期プリミティブ.
- 通常の排他制御では､待機中はスレッドを休眠させてOSに制御を委ねる.スピンロックの場合は､スレッドの処理を継続させたままにして待機する.
  - 短い期間で待機状態が戻ってくる場合､OSに制御を戻さなくて良いので高速なのだそう.
  - 『並行プログラミング入門 ― Rust、C、アセンブリによる実装からのアプローチ』では､単純に待機中にwhileループで待ち続ける実装になっていた.
- ここまで最適化を考えて使うのは難しく､扱う際は相応の知識・最適化が必要になるクラスだろう.


### スレッドのシグナリングに関するクラス群

#### EventWaitHandle

- https://learn.microsoft.com/ja-jp/dotnet/api/system.threading.eventwaithandle?view=net-8.0
- ~Eventクラス群の基底クラス.
- WaitOneでスレッドをブロックして待機し､Setで待機していたスレッドを起こす.
- EventResetModeをコンストラクタの引数に取る. AutoResetとManualResetの2モードがあり､前者はSet後自動で非シグナル状態になり､後者はResetで手動で非シグナル状態に戻す必要がある.
- 継承クラス群がAutoReset､ManualResetのいずれかの特徴を引き継いでいる.敢えてこの基底クラスを使う必要はないのかもしれない.

#### AutoResetEvent

- https://learn.microsoft.com/ja-jp/dotnet/api/system.threading.autoresetevent?view=net-8.0
- Setメソッドでシグナル状態にした後､自動で非シグナル状態に遷移するクラス.
- 実際に触ってみた所､Setでシグナル状態として通知されたスレッドは1個だけだった.

#### ManualResetEvent/ManualResetEventSlim

- https://learn.microsoft.com/ja-jp/dotnet/api/system.threading.manualresetevent?view=net-8.0
- https://learn.microsoft.com/ja-jp/dotnet/api/system.threading.manualreseteventslim?view=net-8.0
- こちらはSetメソッドでシグナル状態にした後､Resetメソッドで手動で非シグナル状態に戻す必要があるクラス.
- ManualResetEventSlimは､内部でスピニングループを利用した高速版だそう(なので､スレッドの待機時にOSに制御を返さないのだろう). 短く処理が終わる場合はこちらが良いとのこと.

#### CountDownEvent

- https://learn.microsoft.com/ja-jp/dotnet/api/system.threading.countdownevent?view=net-8.0
- こちらは少し特殊で､指定カウントに達しないとシグナル状態に遷移しないクラス.
- また､ManualResetEventクラスと同じで､シグナル状態は手動で戻す必要がある.
- 全てのEventWaitHandleの中で最も高速だそうなので､基本的にはこのクラスを使うと良い.

#### Monitor

- https://learn.microsoft.com/ja-jp/dotnet/api/system.threading.monitor?view=net-8.0
- 高レイヤに位置づけられる同期プリミティブで､セマフォやミューテックスや条件変数､待機と通知機能をラップしたオブジェクトを指すのだそう.
- C#の場合は静的クラスとして用意されている.
- 排他制御にも､シグナリングにも使うことができる.
- 排他制御に渡す値はObject型なので､値型を渡すとBox化されるから注意.

#### Barrier

- https://learn.microsoft.com/ja-jp/dotnet/api/system.threading.barrier?view=net-8.0
- いわゆるバリア同期と呼ばれるものを実現するクラス.
- バリア同期とは､｢複数スレッドで並列で実行するようなある処理があって､その処理が終わるまで待つ｣こと.
- C#のBarrierクラスは､特に｢N段階(フェーズ)処理があって､各段階(フェーズ)では全てのスレッドの処理が終わるまで待ちたい｣というケースで使えるような実装になっている.
- いわゆる､並行処理(複数スレッドで同時に処理したいような大きなタスクがある時)で特に使えるようなクラスだろう.


### 参考

- [マルチスレッド・プログラミングの道具箱](https://zenn.dev/yohhoy/articles/multithreading-toolbox)
    - 各同期プリミティブ機構がどういうものかを､大変分かりやすく説明してくれている記事.
    - 具体的な言語・実装は出てこず､概念としてどういうものなのかを教えてくれる.
    - 非常に素晴らしい記事だと思う.マルチスレッドプログラミング・排他制御・シグナリングでどういう概念があるかを最初に把握するのによい.
- [Microsoft 同期プリミティブの概要](https://learn.microsoft.com/ja-jp/dotnet/standard/threading/overview-of-synchronization-primitives)
    - どういう同期プリミティブクラスがあるかについては､ここに書いてある.
    - C#で最初に同期プリミティブの概要を把握したい場合は､まずここを読むのが良いと思う.
- [@IT 第3回　マルチスレッドでデータの不整合を防ぐための排他制御 ― マルチスレッド・プログラミングにおける排他制御と同期制御（前編） ―](https://atmarkit.itmedia.co.jp/ait/articles/0505/25/news113_2.html)
    - InterlockedやReadWriterLockの使い方はこちらを参考にした.
- [@IT 第4回　デッドロックの回避とスレッド間での同期制御 ― マルチスレッド・プログラミングにおける排他制御と同期制御（後編） ―](https://atmarkit.itmedia.co.jp/ait/articles/0506/15/news114.html)
    - Monitorクラスの参考例として見返してみた.
    - 同期プリミティブというよりは､デッドロックの具体例・詳細な解説として素晴らしい記事.
- [.NET クラスライブラリ探訪-047 (System.Threading.Barrier(1))(バリア, SignalAndWait, フェーズ, 協調動作, .NET 4.0)](https://devlights.hatenablog.com/entry/20110329/p1)
    - Barrierクラスの使い方は､ここを参考にさせてもらった.
- [マルチスレッドで高速なC#を書くためのロック戦略](https://qiita.com/tadokoro/items/28b3623a5ec58517d431)
  - 各ロックの速度比較について.
  - Interlocked > lock > SemaphoreSlim > Semaphore > Mutexの順に高速だそう.