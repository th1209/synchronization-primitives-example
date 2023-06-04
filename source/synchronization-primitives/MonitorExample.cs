namespace SynchronizationPrimitives.Monitor;

public class MonitorExample
{
    private int _counter = 0;
    private object _counterLockObj = new object();
    private object _waitObj = new object();

    public void Start()
    {
        // Monitorは､単純な排他制御や､シグナリングにも使うことができる
        LockExample();
        SignalExample(2, 3000);
    }

    private void LockExample()
    {
        void Increment()
        {
            // Enterメソッドで排他制御ができる. 値型を渡すとボックス化されるので注意.
            // ｢排他制御を取得できなかった場合何かする｣というケースではTryEnterが使えるはず.
            System.Threading.Monitor.Enter(_counterLockObj);
            try
            {
                for (int i = 0; i < 10000; i++)
                {
                    _counter++;
                }
            }
            finally
            {
                System.Threading.Monitor.Exit(_counterLockObj);
            }
        }

        Thread[] threads = new Thread[100];
        for (int i = 0; i < threads.Length; i++)
        {
            Thread thread = new Thread(Increment);
            thread.Start();
            threads[i] = thread;
        }
        for (int i = 0; i < threads.Length; i++)
        {
            threads[i].Join();
        }
        Console.WriteLine($"counter:{_counter}");
    }
    
    private void SignalExample(int threadCount, int milliSeconds)
    {
        Thread[] waitThreads = new Thread[threadCount];
        for (int i = 0; i < threadCount; i++)
        {
            waitThreads[i] = new Thread(() =>
            {
                Console.WriteLine($"thread({Thread.CurrentThread.ManagedThreadId}) start.");
                // WaitメソッドやPulseメソッドは､事前に対象のオブジェクトのロックを取得した状態で行わないと例外になる.
                // (System.Threading.SynchronizationLockExceptionがスローさせる)
                // System.Threading.Monitor.Enter(_waitObj);
                bool entered = System.Threading.Monitor.TryEnter(_waitObj);
                try
                {
                    if (entered)
                    {
                        Console.WriteLine($"thread({Thread.CurrentThread.ManagedThreadId}) start waiting.");
                        // Waitメソッドはロックしたオブジェクトのロックを解放しつつ､Pulseが来るまでスレッドをブロックして待機する.
                        System.Threading.Monitor.Wait(_waitObj);
                        Console.WriteLine($"thread({Thread.CurrentThread.ManagedThreadId}) end waiting.");
                    }
                }
                finally
                {
                    if (entered)
                    {
                        System.Threading.Monitor.Exit(_waitObj);
                    }
                    Console.WriteLine($"thread({Thread.CurrentThread.ManagedThreadId}) end. entered:{entered}");
                }
            });
            waitThreads[i].Start();
        }
        
        var pulseThread = new Thread(() =>
        {
            System.Threading.Monitor.Enter(_waitObj);
            try
            {
                Console.WriteLine($"sleep {milliSeconds} milliseconds.");
                Thread.Sleep(milliSeconds);

                Console.WriteLine($"pulse all.");
                // PulseAllメソッドは､Waitで待機中のスレッド群を再開させる(Pulseメソッドの場合はどれか1つのメソッドを解放する)
                System.Threading.Monitor.PulseAll(_waitObj);
            }
            finally
            {
                System.Threading.Monitor.Exit(_waitObj);
            }
        });
        pulseThread.Start();

        for (int i = 0; i < threadCount; i++)
        {
            waitThreads[i].Join();
        }
    }
}