using System;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;

namespace Vostok.Commons.Threading.Tests
{
    [TestFixture]
    internal class AsyncReaderWriterLock_Tests
    {
        private AsyncReaderWriterLock locker;
        private TimeSpan shortWaitTimeout;
        private TimeSpan longWaitTimeout;
        private IDisposable releaser;

        [SetUp]
        public void TestSetup()
        {
            locker = new AsyncReaderWriterLock();
            shortWaitTimeout = 50.Milliseconds();
            longWaitTimeout = 5.Seconds();
        }

        [Test]
        public void Should_allow_write_lock_immediately_when_unlocked()
        {
            var writerTask = locker.WriteLockAsync();

            writerTask.IsCompleted.Should().BeTrue();
        }

        [Test]
        public void Should_allow_multiple_read_locks_immediately_when_unlocked()
        {
            var readerTask1 = locker.ReadLockAsync();
            var readerTask2 = locker.ReadLockAsync();
            var readerTask3 = locker.ReadLockAsync();

            readerTask1.IsCompleted.Should().BeTrue();
            readerTask2.IsCompleted.Should().BeTrue();
            readerTask3.IsCompleted.Should().BeTrue();
        }

        [Test]
        public void Should_not_allow_multiple_writers_when_unlocked()
        {
            locker.WriteLockAsync().Wait();
            var writerTask1 = locker.WriteLockAsync();
            var writerTask2 = locker.WriteLockAsync();

            writerTask1.Wait(shortWaitTimeout).Should().BeFalse();
            writerTask2.Wait(shortWaitTimeout).Should().BeFalse();
        }

        [Test]
        public void Should_not_allow_read_locks_when_locked_by_writer()
        {
            locker.WriteLockAsync().Wait();

            var readerTask1 = locker.ReadLockAsync();
            var readerTask2 = locker.ReadLockAsync();

            readerTask1.Wait(shortWaitTimeout).Should().BeFalse();
            readerTask2.Wait(shortWaitTimeout).Should().BeFalse();
        }

        [Test]
        public void Should_not_allow_write_locks_when_locked_by_reader()
        {
            locker.ReadLockAsync().Wait();

            var writerTask = locker.WriteLockAsync();

            writerTask.Wait(shortWaitTimeout).Should().BeFalse();
        }

        [Test]
        public void Should_allow_waiting_writer_to_proceed_before_readers_when_unlocked_by_writer()
        {
            releaser = locker.WriteLockAsync().Result;

            var writerTask = locker.WriteLockAsync();
            var readerTask = locker.ReadLockAsync();

            releaser.Dispose();

            writerTask.Wait(longWaitTimeout).Should().BeTrue();
            readerTask.Wait(shortWaitTimeout).Should().BeFalse();

            writerTask.Result.Dispose();

            readerTask.Wait(longWaitTimeout).Should().BeTrue();
        }

        [Test]
        public void Should_allow_one_waiting_writer_to_proceed_when_unlocked_by_all_readers()
        {
            var releaser1 = locker.ReadLockAsync().Result;
            var releaser2 = locker.ReadLockAsync().Result;

            var writerTask1 = locker.WriteLockAsync();
            var writerTask2 = locker.WriteLockAsync();

            releaser1.Dispose();
            writerTask1.Wait(shortWaitTimeout).Should().BeFalse();
            writerTask2.Wait(shortWaitTimeout).Should().BeFalse();

            releaser2.Dispose();
            writerTask1.Wait(longWaitTimeout).Should().BeTrue();
            writerTask2.Wait(shortWaitTimeout).Should().BeFalse();
        }

        [Test]
        public void Should_allow_all_waiting_readers_to_proceed_when_unlocked_by_writer()
        {
            releaser = locker.WriteLockAsync().Result;

            var readerTask1 = locker.ReadLockAsync();
            var readerTask2 = locker.ReadLockAsync();

            releaser.Dispose();

            readerTask1.Wait(longWaitTimeout).Should().BeTrue();
            readerTask2.Wait(longWaitTimeout).Should().BeTrue();
        }

        [Test]
        public void TryObtainReadLockImmediately_should_return_true_when_lock_is_not_contended()
        {
            locker.TryObtainReadLockImmediately(out releaser).Should().BeTrue();

            releaser.Should().NotBeNull();
        }

        [Test]
        public void TryObtainReadLockImmediately_should_return_true_when_lock_is_contended_by_other_readers()
        {
            locker.ReadLockAsync().Wait();
            locker.ReadLockAsync().Wait();
            locker.ReadLockAsync().Wait();

            locker.TryObtainReadLockImmediately(out releaser).Should().BeTrue();

            releaser.Should().NotBeNull();
        }

        [Test]
        public void TryObtainReadLockImmediately_should_return_false_when_lock_is_contended_by_writers()
        {
            locker.WriteLockAsync().Wait();

            locker.TryObtainReadLockImmediately(out releaser).Should().BeFalse();

            releaser.Should().BeNull();
        }

        [Test]
        public void TryObtainReadLockImmediately_should_prevent_writers_from_acquiring_the_lock()
        {
            locker.TryObtainReadLockImmediately(out releaser);

            var writerTask = locker.WriteLockAsync();

            writerTask.Wait(shortWaitTimeout).Should().BeFalse();
        }

        [Test]
        public void TryObtainReadLockImmediately_should_provide_a_correct_releaser()
        {
            locker.TryObtainReadLockImmediately(out releaser);

            var writerTask = locker.WriteLockAsync();

            releaser.Dispose();

            writerTask.Wait(longWaitTimeout).Should().BeTrue();
        }

        [Test]
        public void TryObtainReadLockImmediately_should_not_have_delayed_side_effects_on_failed_attempt()
        {
            using (locker.WriteLockAsync().Result)
            {
                locker.TryObtainReadLockImmediately(out releaser);
            }

            var writerTask = locker.WriteLockAsync();

            writerTask.Wait(longWaitTimeout).Should().BeTrue();
        }

        [Test]
        public void TryObtainWriteLockImmediately_should_return_true_when_lock_is_not_contended()
        {
            locker.TryObtainWriteLockImmediately(out releaser).Should().BeTrue();

            releaser.Should().NotBeNull();
        }

        [Test]
        public void TryObtainWriteLockImmediately_should_return_false_when_lock_is_contended_by_other_readers()
        {
            locker.ReadLockAsync().Wait();
            locker.ReadLockAsync().Wait();
            locker.ReadLockAsync().Wait();

            locker.TryObtainWriteLockImmediately(out releaser).Should().BeFalse();

            releaser.Should().BeNull();
        }

        [Test]
        public void TryObtainWriteLockImmediately_should_return_false_when_lock_is_contended_by_writers()
        {
            locker.WriteLockAsync().Wait();

            locker.TryObtainWriteLockImmediately(out releaser).Should().BeFalse();

            releaser.Should().BeNull();
        }

        [Test]
        public void TryObtainWriteLockImmediately_should_prevent_writers_from_acquiring_the_lock()
        {
            locker.TryObtainWriteLockImmediately(out releaser);

            var writerTask = locker.WriteLockAsync();

            writerTask.Wait(shortWaitTimeout).Should().BeFalse();
        }

        [Test]
        public void TryObtainWriteLockImmediately_should_prevent_readers_from_acquiring_the_lock()
        {
            locker.TryObtainWriteLockImmediately(out releaser);

            var writerTask = locker.ReadLockAsync();

            writerTask.Wait(shortWaitTimeout).Should().BeFalse();
        }

        [Test]
        public void TryObtainWriteLockImmediately_should_provide_a_correct_releaser()
        {
            locker.TryObtainWriteLockImmediately(out releaser);

            var writerTask = locker.WriteLockAsync();

            releaser.Dispose();

            writerTask.Wait(longWaitTimeout).Should().BeTrue();
        }

        [Test]
        public void TryObtainWriteLockImmediately_should_not_have_delayed_side_effects_on_failed_attempt()
        {
            using (locker.WriteLockAsync().Result)
            {
                locker.TryObtainWriteLockImmediately(out releaser);
            }

            var writerTask = locker.WriteLockAsync();

            writerTask.Wait(longWaitTimeout).Should().BeTrue();
        }
    }
}