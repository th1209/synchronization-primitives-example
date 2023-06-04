namespace SynchronizationPrimitives.ExclusiveControl.InterLocked
{
    public class InterlockedExample
    {
        private int _counter = 0;

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

            // 安全に代入したい時は､ExchangeやCompareExchangeを使う
            string str1 = "apple";
            string str2 = "banana";
            Interlocked.Exchange(ref str1, str2);
            Console.WriteLine($"result:{str1}");

            str1 = null;
            str2 = "hello";
            Interlocked.CompareExchange(ref str1, str2, null);
            Console.WriteLine($"result:{str1}");
        }

        private void Increment()
        {
            for (int i = 0; i < 10000; i++)
            {
                Interlocked.Increment(ref _counter);
                // 試しに↓の単純なインクリメントに置き換えると､合計値が合わなくなるはず
                // _counter++;
            }
        }
    }
}