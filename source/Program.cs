using SynchronizationPrimitives.ExclusiveControl.Semaphore;
using SynchronizationPrimitives.ExclusiveControl.Lock;
using SynchronizationPrimitives.ExclusiveControl.Mutex;
using SynchronizationPrimitives.ExclusiveControl.InterLocked;
using SynchronizationPrimitives.ExclusiveControl.RWLock;
using SynchronizationPrimitives.ExclusiveControl.SpinLock;
using SynchronizationPrimitives.Signaling.WaitHandle;
using SynchronizationPrimitives.Signaling.Monitor;
using SynchronizationPrimitives.Signaling.Barrier;

Main();

void Main()
{
    // 同期プリミティブに関するサンプル
    {
        new SemaphoreExample().Start();

        // new SemaphoreSlimExample().Start();

        // new LockExample().Start();

        // new MutexExample().Start();

        // new InterlockedExample().Start();

        // new ReaderWriterLockSlimExample().Start();

        // new SpinLockExample().Start();
    }

    // シグナリングに関するサンプル
    {
        // new EventWaitHandleExample().Start();

        // new AutoResetEventExample().Start();

        // new ManualResetEventExample().Start();

        // new ManualResetEventSlimExample().Start();

        // new CountDownEventExample().Start();

        // new MonitorExample().Start();

        // new BarrierExample().Start();
    }
}






