namespace SynchronizationPrimitives.Mutex
{
    public class MutexExample
    {
        private System.Threading.Mutex _mutex = new System.Threading.Mutex();
        private int _counter = 0;

        public void Start()
        {
            Thread[] threads = new Thread[100];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(Increment);
                threads[i].Start();
            }

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }

            Console.WriteLine($"counter:{_counter}");
        }

        private void Increment()
        {
            // Mutexの待機処理をコメントアウトすると､合計値が合わなくなる
            _mutex.WaitOne();

            for (int i = 0; i < 10000; i++)
            {
                _counter++;
            }

            _mutex.ReleaseMutex();
        }
    }
}

