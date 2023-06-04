using System.Diagnostics;

namespace SynchronizationPrimitives.WaitHandle
{
    public class EventWaitHandleExample
    {
        // 初期状態(非シグナルorシグナル)とモード(AutoReset or ManualReset)を指定する
        private EventWaitHandle _eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        // private EventWaitHandle _eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        public void Start()
        {
            WaitMilliSeconds(5, 500);
            WaitMilliSeconds(3, 500);
        }

        private void WaitMilliSeconds(int threadCount, int milliSeconds)
        {
            Thread[] threads = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                threads[i] = new Thread(() =>
                {
                    Console.WriteLine($"thread({Thread.CurrentThread.ManagedThreadId}) start.");
                    _eventWaitHandle.WaitOne();
                    Console.WriteLine($"thread({Thread.CurrentThread.ManagedThreadId}) end.");
                });
                threads[i].Start();
            }

            Console.WriteLine($"wait {milliSeconds} milliSeconds.");
            Thread.Sleep(milliSeconds);
            Console.WriteLine($"set eventWaitHandle.");
            _eventWaitHandle.Set();

            for (int i = 0; i < threadCount; i++)
            {
                threads[i].Join();
            }

            _eventWaitHandle.Reset(); // ManualResetなので､自分で非シグナル状態に戻す必要がある
        }
    }
    
    public class AutoResetEventExample
    {
        // AutoResetEventは､シグナル状態での通知後､自動で非シグナル状態に戻る同期プリミティブ
        private AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        public void Start()
        {
            WaitMilliSeconds(5, 500);
            // WaitMilliSeconds(3, 500);
        }

        private void WaitMilliSeconds(int threadCount, int milliSeconds)
        {
            Thread[] threads = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                threads[i] = new Thread(() =>
                {
                    Console.WriteLine($"thread({Thread.CurrentThread.ManagedThreadId}) start.");
                    _autoResetEvent.WaitOne();
                    Console.WriteLine($"thread({Thread.CurrentThread.ManagedThreadId}) end.");
                });
                threads[i].Start();
            }
            
            for (int i = 0; i < threadCount; i++)
            {
                new Thread(() =>
                {
                    Console.WriteLine($"wait {milliSeconds} milliSeconds.");
                    Thread.Sleep(milliSeconds);
                    Console.WriteLine($"set eventWaitHandle.");
                    // ※AutoResetEventの場合､Setで通知がいくのは1スレッドのみの模様
                    // (基底のEventWaitHandleをEventResetMode.ManualResetで利用した場合も同様のようだ)
                    _autoResetEvent.Set();
                }).Start();
            }

            for (int i = 0; i < threadCount; i++)
            {
                threads[i].Join();
            }
        }
    }
    
    public class ManualResetEventExample
    {
        // ManualResetEventは､手動で非シグナル状態に戻す必要がある同期プリミティブ
        private ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

        public void Start()
        {
            WaitMilliSeconds(5, 500);
            WaitMilliSeconds(3, 500);
        }

        private void WaitMilliSeconds(int threadCount, int milliSeconds)
        {
            Thread[] threads = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                threads[i] = new Thread(() =>
                {
                    Console.WriteLine($"thread({Thread.CurrentThread.ManagedThreadId}) start.");
                    _manualResetEvent.WaitOne();
                    Console.WriteLine($"thread({Thread.CurrentThread.ManagedThreadId}) end.");
                });
                threads[i].Start();
            }

            Console.WriteLine($"wait {milliSeconds} milliSeconds.");
            Thread.Sleep(milliSeconds);
            Console.WriteLine($"set eventWaitHandle.");
            _manualResetEvent.Set();

            for (int i = 0; i < threadCount; i++)
            {
                threads[i].Join();
            }

            _manualResetEvent.Reset(); // ManualResetなので､自分で非シグナル状態に戻す必要がある
        }
    }
    
    public class ManualResetEventSlimExample
    {
        private ManualResetEventSlim _manualResetEventSlim = new ManualResetEventSlim(false);

        public void Start()
        {
            WaitMilliSeconds(5, 500);
            WaitMilliSeconds(3, 500);
        }

        private void WaitMilliSeconds(int threadCount, int milliSeconds)
        {
            Thread[] threads = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                threads[i] = new Thread(() =>
                {
                    Console.WriteLine($"thread({Thread.CurrentThread.ManagedThreadId}) start.");
                    _manualResetEventSlim.Wait();
                    Console.WriteLine($"thread({Thread.CurrentThread.ManagedThreadId}) end.");
                });
                threads[i].Start();
            }

            Console.WriteLine($"wait {milliSeconds} milliSeconds.");
            Thread.Sleep(milliSeconds);
            Console.WriteLine($"set eventWaitHandle.");
            _manualResetEventSlim.Set();

            for (int i = 0; i < threadCount; i++)
            {
                threads[i].Join();
            }

            _manualResetEventSlim.Reset(); // ManualResetなので､自分で非シグナル状態に戻す必要がある
        }
    }

    public class CountDownEventExample
    {
        // EventWaitHandleのうち､最も軽量なのがCountDownEvent. 基本的にはこれを使うと良い.
        private CountdownEvent _countdownEvent = new CountdownEvent(10);

        public void Start()
        {
            WaitMilliSeconds(100);
            // ※念の為2回実行してみて､CountDownEventが非シグナル状態に戻っている(再利用可能なこと)を確認
            WaitMilliSeconds(200);
        }

        private void WaitMilliSeconds(int milliSeconds)
        {
            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < _countdownEvent.InitialCount; i++)
            {
                new Thread(() =>
                {
                    Thread.Sleep(milliSeconds);
                    _countdownEvent.Signal(); // 指定回数Signalが呼ばれると､シグナル状態に遷移する
                }).Start();
            }

            _countdownEvent.Wait();
            _countdownEvent.Reset(); // 非シグナル状態に戻す

            sw.Stop();
            Console.WriteLine($"elapsed(ms):{sw.Elapsed.Milliseconds}");
        }
    }
}