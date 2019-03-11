using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

#pragma warning disable 420

namespace Vostok.Commons.Threading
{
    [PublicAPI]
    internal class AsyncManualResetEvent
    {
        private volatile TaskCompletionSource<bool> state = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

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
                if (Interlocked.CompareExchange(ref state, new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously), currentState) == currentState)
                    return;
            }
        }

        public Task WaitAsync() => state.Task;

        public Task WaitAsync(CancellationToken token) => Task.WhenAny(state.Task, Task.Delay(-1, token));

        public TaskAwaiter GetAwaiter() => WaitAsync().GetAwaiter();

        public static implicit operator Task(AsyncManualResetEvent @event) => @event.WaitAsync();
    }
}
