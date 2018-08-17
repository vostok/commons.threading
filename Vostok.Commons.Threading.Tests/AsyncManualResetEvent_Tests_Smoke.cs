using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions.Extensions;
using NUnit.Framework;

namespace Vostok.Commons.Threading.Tests
{
    [Explicit]
    [TestFixture]
    public class AsyncManualResetEvent_Tests_Smoke
    {
        [Explicit]
        [TestCase(1, 1, 0)]
        [TestCase(4, 4, 4)]
        [TestCase(2, 1, 6)]
        public void Should_not_leak_waiters_in_concurrent_environment(int waitersCount, int settersCount, int resettersCount)
        {
            var @event = new AsyncManualResetEvent(false);

            var stop = false;
            var trigger = new CountdownEvent(waitersCount + settersCount + resettersCount);
            var setters = Enumerable.Range(0, settersCount)
                .Select(_ => Task.Run(() =>
                {
                    trigger.Signal();
                    trigger.Wait();
                    while (!stop)
                        @event.Set();
                })).ToList();
            var resetters = Enumerable.Range(0, resettersCount)
                .Select(_ => Task.Run(() =>
                {
                    trigger.Signal();
                    trigger.Wait();
                    while (!stop)
                        @event.Reset();
                })).ToList();
            var waiters = Enumerable.Range(0, waitersCount)
                .Select(_ => Task.Run(() =>
                {
                    trigger.Signal();
                    trigger.Wait();
                    while (!stop)
                        @event.WaitAsync().GetAwaiter().GetResult();
                })).ToList();

            trigger.Wait();
            Thread.Sleep(10.Seconds());
            stop = true;

            Task.WhenAll(setters.Concat(resetters)).GetAwaiter().GetResult();
            @event.Set();
            Task.WhenAll(waiters).GetAwaiter().GetResult();
        }
    }
}