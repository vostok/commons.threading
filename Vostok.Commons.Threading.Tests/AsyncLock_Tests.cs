using System;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;

namespace Vostok.Commons.Threading.Tests
{
    [TestFixture]
    internal class AsyncLock_Tests
    {
        private AsyncLock locker;
        private TimeSpan shortWaitTimeout;
        private TimeSpan longWaitTimeout;
        private IDisposable releaser;

        [SetUp]
        public void TestSetup()
        {
            locker = new AsyncLock();
            shortWaitTimeout = 50.Milliseconds();
            longWaitTimeout = 5.Seconds();
        }

        [Test]
        public void Should_be_acquired_synchronously_when_unlocked()
        {
            var acquireTask = locker.LockAsync();

            acquireTask.IsCompleted.Should().BeTrue();
        }

        [Test]
        public void Should_not_allow_to_acquire_when_locked()
        {
            locker.LockAsync().Wait();

            var acquireTask = locker.LockAsync();

            acquireTask.Wait(shortWaitTimeout).Should().BeFalse();
        }

        [Test]
        public void Should_allow_one_waiter_to_proceed_when_unlocked_by_holder()
        {
            releaser = locker.LockAsync().Result;

            var waiterTask1 = locker.LockAsync();
            var waiterTask2 = locker.LockAsync();

            releaser.Dispose();

            waiterTask1.Wait(longWaitTimeout).Should().BeTrue();
            waiterTask2.Wait(shortWaitTimeout).Should().BeFalse();
        }

        [Test]
        public void TryLockImmediately_should_return_true_when_lock_is_not_acquired()
        {
            locker.TryLockImmediately(out releaser).Should().BeTrue();

            releaser.Should().NotBeNull();
        }

        [Test]
        public void TryLockImmediately_should_return_false_when_lock_is_acquired()
        {
            locker.TryLockImmediately(out releaser);

            locker.TryLockImmediately(out releaser).Should().BeFalse();

            releaser.Should().BeNull();
        }

        [Test]
        public void TryLockImmediately_should_not_have_delayed_side_effects()
        {
            using (locker.LockAsync().Result)
            {
                locker.TryLockImmediately(out releaser).Should().BeFalse();
            }

            locker.TryLockImmediately(out releaser).Should().BeTrue();
        }
    }
}