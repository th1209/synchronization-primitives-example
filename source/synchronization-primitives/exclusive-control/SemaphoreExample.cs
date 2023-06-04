namespace SynchronizationPrimitives.ExclusiveControl.Semaphore
{
    public class SemaphoreExample
    {
        private const int SemaphoreMaxCount = 3;
        
        private System.Threading.Semaphore
            _semaphore = new System.Threading.Semaphore(initialCount: 0, maximumCount: SemaphoreMaxCount);

        public void Start()
        {
            Thread[] threads = new Thread[5];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(ThreadProc);
                threads[i].Start();
            }
            
            int milliSeconds = 1000;
            Console.WriteLine($"sleep {milliSeconds} milliSeconds.");
            Thread.Sleep(milliSeconds);
            // ここでセマフォをインクリメントし､指定数分のスレッドが再開する
            _semaphore.Release(releaseCount:3);

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }
        }

        private void ThreadProc()
        {
            Console.WriteLine($"thread({Thread.CurrentThread.ManagedThreadId}) start.");
            _semaphore.WaitOne();
            Thread.Sleep(300);
            // スレッド内でもセマフォをインクリメントし､残りのスレッドを実行する
            int prevCount = _semaphore.Release();
            Console.WriteLine($"thread({Thread.CurrentThread.ManagedThreadId}) end. prevCount:{prevCount}");
        }
    }

    public class SemaphoreSlimExample
    {
        private const int SemaphoreMaxCount = 3;

        // SemaphoreSlimは軽量版で､名前付きセマフォ(名前を指定してセマフォの状態を取得する機能)が使えないなどの制約がある模様.
        private System.Threading.SemaphoreSlim
            _semaphoreSlim = new System.Threading.SemaphoreSlim(initialCount: 0, maxCount: SemaphoreMaxCount);

        public void Start()
        {
            Thread[] threads = new Thread[5];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(ThreadProc);
                threads[i].Start();
            }
            
            int milliSeconds = 1000;
            Console.WriteLine($"sleep {milliSeconds} milliSeconds.");
            Thread.Sleep(milliSeconds);
            _semaphoreSlim.Release(releaseCount:3);

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }
        }

        private void ThreadProc()
        {
            Console.WriteLine($"thread({Thread.CurrentThread.ManagedThreadId}) start.");
            _semaphoreSlim.Wait();
            Thread.Sleep(300);
            int prevCount = _semaphoreSlim.Release();
            Console.WriteLine($"thread({Thread.CurrentThread.ManagedThreadId}) end. prevCount:{prevCount}");
        }
    }
}

