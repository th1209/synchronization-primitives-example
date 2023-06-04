using System.Threading;

namespace SynchronizationPrimitives.RWLock
{
    public class ReaderWriterLockExample
    {
        private ReaderWriterLock _rwLock = new ReaderWriterLock();
        private int _value = 0;
        
        public void Start()
        {
            Thread[] threads = new Thread[20];
            for (int i = 0; i < threads.Length / 2; i++)
            {
                Thread reader = new Thread(new ThreadStart(Read));
                Thread writer = new Thread(new ThreadStart(Write));
                reader.Start();
                writer.Start();
                threads[i] = reader;
                threads[i + 1] = writer;
            }
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }
        }

        private void Read()
        {
            // 試しにReaderLockをコメントアウトしてみると､Readを考慮せず値の書き込みが行われることが分かる
            _rwLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                Console.WriteLine($"value:{_value}");
            }
            finally
            {
                _rwLock.ReleaseReaderLock();
            }
        }

        private void Write()
        {
            _rwLock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                _value += 100;
                Thread.Sleep(100);
                _value -= 100;
            }
            finally
            {
                _rwLock.ReleaseWriterLock();
            }
        }
    }
    
    public class ReaderWriterLockSlimExample
    {
        // ReaderWriterLockSlimは軽量版. 再帰などのサポートが失われるが､ReaderWriterLockよりずっと高速で､こちらを使うほうが良い.
        private ReaderWriterLockSlim _rwLockSlim = new ReaderWriterLockSlim();
        private int _value = 0;
        
        public void Start()
        {
            Thread[] threads = new Thread[20];
            for (int i = 0; i < threads.Length / 2; i++)
            {
                Thread reader = new Thread(new ThreadStart(Read));
                Thread writer = new Thread(new ThreadStart(Write));
                reader.Start();
                writer.Start();
                threads[i] = reader;
                threads[i + 1] = writer;
            }
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }
        }

        private void Read()
        {
            _rwLockSlim.EnterReadLock();
            try
            {
                Console.WriteLine($"value:{_value}");
            }
            finally
            {
                _rwLockSlim.ExitReadLock();
            }
        }

        private void Write()
        {
            _rwLockSlim.EnterWriteLock();
            try
            {
                _value += 100;
                Thread.Sleep(100);
                _value -= 100;
            }
            finally
            {
                _rwLockSlim.ExitWriteLock();
            }
        }
    }
}