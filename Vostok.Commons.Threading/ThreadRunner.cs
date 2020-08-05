using System;
using System.Threading;
using JetBrains.Annotations;

namespace Vostok.Commons.Threading
{
    /// <summary>
    /// Runs action in separate thread. Rarely needed in code. Use Task for most cases
    /// </summary>
    [PublicAPI]
    internal class ThreadRunner
    {
        public static Thread Run(Action<object> threadRoutine, Action<Exception> errorHandler = null, [CanBeNull] Action<Thread> tuneThreadBeforeStart = null)
        {
            var t = new Thread(Wrap(threadRoutine, errorHandler))
            {
                IsBackground = true
            };
            tuneThreadBeforeStart?.Invoke(t);
            t.Start();
            return t;
        }

        public static Thread Run(Action threadRoutine, Action<Exception> errorHandler = null, [CanBeNull] Action<Thread> tuneThreadBeforeStart = null)
        {
            return Run(o => threadRoutine(), errorHandler, tuneThreadBeforeStart);
        }

        public static ParameterizedThreadStart Wrap(Action<object> threadRoutine, Action<Exception> errorHandler)
        {
            return parameter =>
            {
                try
                {
                    threadRoutine(parameter);
                }
                catch (ThreadAbortException)
                {
                    Thread.ResetAbort();
                }
                catch (Exception e)
                {
                    errorHandler?.Invoke(e);
                }
            };
        }
    }
}