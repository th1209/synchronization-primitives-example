namespace SynchronizationPrimitives.Signaling.Barrier
{
    public class BarrierExample
    {
        private int _counter = 0;

        public void Start()
        {
            // バリア同期は､シグナル処理の一種.
            // "全てのスレッドの処理が終わるまで待つ"が基本なのだが､特に"並行処理が複数フェーズあって､各フェーズ毎に結果を待つ"というケースの際に使える.
            using (System.Threading.Barrier barrier = new System.Threading.Barrier(5, BarrierPostPhaseBlock))
            {
                Parallel.Invoke(
        () => ThreePhaseWork(barrier),
                    () => ThreePhaseWork(barrier),
                    () => ThreePhaseWork(barrier),
                    () => ThreePhaseWork(barrier),
                    () => ThreePhaseWork(barrier)
                );
            }

            Console.WriteLine($" sumValue:{_counter}");
        }

        private void ThreePhaseWork(System.Threading.Barrier barrier)
        {
            IncrementLoop(100, barrier);
            IncrementLoop(100, barrier);
            IncrementLoop(100, barrier);
        }

        private void IncrementLoop(int count, System.Threading.Barrier barrier)
        {
            for (int i = 0; i < count; i++)
            {
                Interlocked.Increment(ref _counter);
            }
            Console.WriteLine($"thread({Thread.CurrentThread.ManagedThreadId}) barrierCurrentPhaseNum:{barrier.CurrentPhaseNumber}");

            try
            {
                barrier.SignalAndWait();
            }
            catch (BarrierPostPhaseException postPhaseException)
            {
                Console.WriteLine($"{nameof(BarrierPostPhaseException)} message:{postPhaseException.Message}");
                throw;
            }
        }

        // 各フェーズの終了時に呼ばれるコールバック
        private void BarrierPostPhaseBlock(System.Threading.Barrier barrier)
        {
            // ※(呼び出し元のスレッドで呼ばれるかと思いきや)フェーズの終了時に呼ばれるコールバックは､最後にそのフェーズの処理を終えたスレッド上で呼ばれる点に注意
            Console.WriteLine($"BarrierEachPhaseEndCallback currentValue:{_counter} currentPhaseNum:{barrier.CurrentPhaseNumber} participantsCount:{barrier.ParticipantCount} invokedThread:{Thread.CurrentThread.ManagedThreadId}");
        }
    }
}


