using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Commons.Threading.Tests
{
    [TestFixture]
    public class AsyncManualResetEvent_Tests
    {
        private AsyncManualResetEvent @event;

        [SetUp]
        public void TestSetup()
        {
            @event = new AsyncManualResetEvent(false);
        }

        [Test]
        public void WaitAsync_should_return_if_event_was_set_initially()
        {
            @event = new AsyncManualResetEvent(true);

            @event.WaitAsync().IsCompleted.Should().BeTrue();
        }

        [Test]
        public void WaitAsync_should_not_return_if_event_was_not_set_initially()
        {
            @event.WaitAsync().Wait(100).Should().BeFalse();
        }

        [Test]
        public void WaitAsync_should_return_when_event_was_set_before_call()
        {
            @event.Set();

            @event.WaitAsync().IsCompleted.Should().BeTrue();
        }

        [Test]
        public void WaitAsync_should_return_when_event_was_set_after_call()
        {
            var task = @event.WaitAsync();

            @event.Set();

            task.IsCompleted.Should().BeTrue();
        }

        [Test]
        public async Task WaitAsync_should_return_when_event_was_set_after_call_with_cancellation_token()
        {
            var cts = new CancellationTokenSource();
            var task = @event.WaitAsync(cts.Token);

            @event.Set();

            await task;
        }

        [Test]
        public void WaitAsync_should_be_cancelable()
        {
            Func<Task> func = async () =>
            {
                var cts = new CancellationTokenSource();
                var task = @event.WaitAsync(cts.Token);

                cts.Cancel(true);

                await task;
            };
            func.Should().Throw<OperationCanceledException>();
        }

        [Test]
        public void Reset_should_reset_event_that_was_set_initially()
        {
            @event = new AsyncManualResetEvent(true);

            @event.Reset();

            @event.WaitAsync().Wait(100).Should().BeFalse();
        }

        [Test]
        public void Reset_should_reset_event()
        {
            @event.Set();
            @event.Reset();

            @event.WaitAsync().Wait(100).Should().BeFalse();
        }

        [Test]
        public void Reset_should_not_reset_event_if_it_was_not_set()
        {
            var oldWaiter = @event.WaitAsync();

            @event.Reset();
            @event.Set();

            oldWaiter.IsCompleted.Should().BeTrue();
        }

        [Test]
        public void Should_have_correct_state_after_multiple_set_reset_cycles()
        {
            var waiters = new List<Task>();

            @event.Set();
            waiters.Add(@event.WaitAsync());

            @event.Set();
            waiters.Add(@event.WaitAsync());

            @event.Reset();
            waiters.Add(@event.WaitAsync());

            @event.Reset();
            waiters.Add(@event.WaitAsync());

            @event.Set();
            waiters.Add(@event.WaitAsync());

            @event.Reset();
            waiters.Add(@event.WaitAsync());

            @event.Set();
            waiters.Add(@event.WaitAsync());

            @event.Reset();
            waiters.Add(@event.WaitAsync());

            @event.Set();
            waiters.Add(@event.WaitAsync());

            foreach (var waiter in waiters)
                waiter.IsCompleted.Should().BeTrue();
        }
    }
}