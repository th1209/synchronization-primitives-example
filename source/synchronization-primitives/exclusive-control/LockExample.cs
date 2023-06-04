namespace SynchronizationPrimitives.ExclusiveControl.Lock
{
    public class LockExample
    {
        private int _counter = 0;
        private object _lockObj = new object();

        public void Start()
        {
            Thread[] threads = new Thread[100];
            for (int i = 0; i < threads.Length; i++)
            {
                Thread thread = new Thread(new ThreadStart(Increment));
                thread.Start();
                threads[i] = thread;
            }
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }

            Console.WriteLine($"counter:{_counter}");
        }

        private void Increment()
        {
            // 試しにlockブロックを外すと､合計値がおかしくなる
            lock(_lockObj)
            {
                for (int i = 0; i < 10000; i++)
                {
                    _counter++;
                }
            }
        }
    }
}