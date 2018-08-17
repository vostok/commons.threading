using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

#pragma warning disable 420

namespace Vostok.Commons.Threading
{
    [PublicAPI]
#if MAKE_CLASSES_PUBLIC
    public
#else
    internal
#endif
    class AsyncManualResetEvent
    {
        private volatile TaskCompletionSource<bool> state = new TaskCompletionSource<bool>();

        public AsyncManualResetEvent(bool isSetInitially)
        {
            if (isSetInitially)
                Set();
        }

        public void Set() => state.TrySetResult(true);

        public void Reset()
        {
            while (true)
            {
                var currentState = state;
                if (!currentState.Task.IsCompleted)
                    return;
                if (Interlocked.CompareExchange(ref state, new TaskCompletionSource<bool>(), currentState) == currentState)
                    return;
            }
        }

        public Task WaitAsync() => state.Task;
    }
}