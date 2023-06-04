namespace SynchronizationPrimitives.ExclusiveControl.SpinLock
{
    public class SpinLockExample
    {
        private int _counter = 0;
        private System.Threading.SpinLock _spinLock = new System.Threading.SpinLock();

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
            bool gotLock = false;
            try
            {
                _spinLock.Enter(ref gotLock);
                for (int i = 0; i < 10000; i++)
                {
                    _counter++;
                }
            }
            finally
            {
                if (gotLock)
                {
                    _spinLock.Exit();
                }
            }
        }
    }
}