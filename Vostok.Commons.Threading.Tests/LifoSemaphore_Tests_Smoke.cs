using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;

namespace Vostok.Commons.Threading.Tests
{
    [TestFixture]
    [Explicit]
    internal class LifoSemaphore_Tests_Smoke
    {
        [SetUp]
        public void TestSetup()
        {
            ThreadPool.SetMinThreads(2048, 2048);
        }

        [TestCase(1, 1, 5)]
        [TestCase(1, 2, 5)]
        [TestCase(1, 4, 5)]
        [TestCase(1, 8, 5)]
        [TestCase(1, 16, 5)]
        [TestCase(1, 32, 5)]
        [TestCase(2, 2, 5)]
        [TestCase(2, 4, 5)]
        [TestCase(2, 8, 5)]
        [TestCase(2, 16, 5)]
        [TestCase(2, 32, 5)]
        [TestCase(4, 4, 5)]
        [TestCase(4, 8, 5)]
        [TestCase(4, 16, 5)]
        [TestCase(4, 32, 5)]
        [TestCase(4, 64, 5)]
        [TestCase(8, 8, 5)]
        [TestCase(8, 16, 5)]
        [TestCase(8, 32, 5)]
        [TestCase(8, 64, 5)]
        [TestCase(8, 128, 5)]
        [TestCase(16, 16, 5)]
        [TestCase(16, 32, 5)]
        [TestCase(16, 64, 5)]
        [TestCase(16, 128, 5)]
        [TestCase(16, 256, 5)]
        [TestCase(32, 32, 5)]
        [TestCase(32, 64, 5)]
        [TestCase(32, 128, 5)]
        [TestCase(32, 256, 5)]
        [TestCase(64, 64, 5)]
        [TestCase(64, 128, 5)]
        [TestCase(64, 256, 5)]
        [TestCase(128, 128, 5)]
        [TestCase(128, 256, 5)]
        [TestCase(256, 256, 5)]
        [TestCase(256, 128, 5)]
        [TestCase(256, 64, 5)]
        [TestCase(256, 32, 5)]
        [TestCase(256, 16, 5)]
        [TestCase(256, 8, 5)]
        [TestCase(256, 4, 5)]
        [TestCase(256, 2, 5)]
        [TestCase(256, 1, 5)]
        [Explicit]
        [MaxTime(15000)]
        public void Should_not_leak_or_violate_allowed_parallelism_without_payload(int capacity, int parallelism, int durationSeconds)
        {
            PerformSmokeTest(capacity, parallelism, durationSeconds.Seconds(), () => {});
        }

        [TestCase(1, 1, 5)]
        [TestCase(1, 2, 5)]
        [TestCase(1, 4, 5)]
        [TestCase(1, 8, 5)]
        [TestCase(1, 16, 5)]
        [TestCase(1, 32, 5)]
        [TestCase(2, 2, 5)]
        [TestCase(2, 4, 5)]
        [TestCase(2, 8, 5)]
        [TestCase(2, 16, 5)]
        [TestCase(2, 32, 5)]
        [TestCase(4, 4, 5)]
        [TestCase(4, 8, 5)]
        [TestCase(4, 16, 5)]
        [TestCase(4, 32, 5)]
        [TestCase(4, 64, 5)]
        [TestCase(8, 8, 5)]
        [TestCase(8, 16, 5)]
        [TestCase(8, 32, 5)]
        [TestCase(8, 64, 5)]
        [TestCase(8, 128, 5)]
        [TestCase(16, 16, 5)]
        [TestCase(16, 32, 5)]
        [TestCase(16, 64, 5)]
        [TestCase(16, 128, 5)]
        [TestCase(16, 256, 5)]
        [TestCase(32, 32, 5)]
        [TestCase(32, 64, 5)]
        [TestCase(32, 128, 5)]
        [TestCase(32, 256, 5)]
        [TestCase(64, 64, 5)]
        [TestCase(64, 128, 5)]
        [TestCase(64, 256, 5)]
        [TestCase(128, 128, 5)]
        [TestCase(128, 256, 5)]
        [TestCase(256, 256, 5)]
        [TestCase(256, 128, 5)]
        [TestCase(256, 64, 5)]
        [TestCase(256, 32, 5)]
        [TestCase(256, 16, 5)]
        [TestCase(256, 8, 5)]
        [TestCase(256, 4, 5)]
        [TestCase(256, 2, 5)]
        [TestCase(256, 1, 5)]
        [Explicit]
        [MaxTime(15000)]
        public void Should_not_leak_or_violate_allowed_parallelism_with_small_sleep_payload(int capacity, int parallelism, int durationSeconds)
        {
            PerformSmokeTest(capacity, parallelism, durationSeconds.Seconds(), () => Thread.Sleep(1));
        }

        [TestCase(1, 1, 5)]
        [TestCase(1, 2, 5)]
        [TestCase(1, 4, 5)]
        [TestCase(1, 8, 5)]
        [TestCase(1, 16, 5)]
        [TestCase(1, 32, 5)]
        [TestCase(2, 2, 5)]
        [TestCase(2, 4, 5)]
        [TestCase(2, 8, 5)]
        [TestCase(2, 16, 5)]
        [TestCase(2, 32, 5)]
        [TestCase(4, 4, 5)]
        [TestCase(4, 8, 5)]
        [TestCase(4, 16, 5)]
        [TestCase(4, 32, 5)]
        [TestCase(4, 64, 5)]
        [TestCase(8, 8, 5)]
        [TestCase(8, 16, 5)]
        [TestCase(8, 32, 5)]
        [TestCase(8, 64, 5)]
        [TestCase(8, 128, 5)]
        [TestCase(16, 16, 5)]
        [TestCase(16, 32, 5)]
        [TestCase(16, 64, 5)]
        [TestCase(16, 128, 5)]
        [TestCase(16, 256, 5)]
        [TestCase(32, 32, 5)]
        [TestCase(32, 64, 5)]
        [TestCase(32, 128, 5)]
        [TestCase(32, 256, 5)]
        [TestCase(64, 64, 5)]
        [TestCase(64, 128, 5)]
        [TestCase(64, 256, 5)]
        [TestCase(128, 128, 5)]
        [TestCase(128, 256, 5)]
        [TestCase(256, 256, 5)]
        [TestCase(256, 128, 5)]
        [TestCase(256, 64, 5)]
        [TestCase(256, 32, 5)]
        [TestCase(256, 16, 5)]
        [TestCase(256, 8, 5)]
        [TestCase(256, 4, 5)]
        [TestCase(256, 2, 5)]
        [TestCase(256, 1, 5)]
        [Explicit]
        [MaxTime(15000)]
        public void Should_not_leak_or_violate_allowed_parallelism_with_large_random_sleep_payload(int capacity, int parallelism, int durationSeconds)
        {
            PerformSmokeTest(capacity, parallelism, durationSeconds.Seconds(), () => Thread.Sleep(ThreadSafeRandom.Next(20)));
        }

        private static void PerformSmokeTest(int capacity, int parallelism, TimeSpan duration, Action payload)
        {
            var semaphore = new LifoSemaphore(capacity);

            var tasks = new List<Task>();

            var observedParallelism = 0;

            for (var i = 0; i < parallelism; i++)
            {
                tasks.Add(
                    Task.Run(
                        async () =>
                        {
                            var watch = Stopwatch.StartNew();

                            while (watch.Elapsed < duration)
                            {
                                await semaphore.WaitAsync();

                                if (Interlocked.Increment(ref observedParallelism) > capacity)
                                    throw new Exception($"Violated max allowed parallelism of {parallelism} (observed {observedParallelism})");

                                payload();

                                Interlocked.Decrement(ref observedParallelism);

                                semaphore.Release();
                            }
                        }));
            }

            Task.WhenAll(tasks).GetAwaiter().GetResult();

            semaphore.CurrentCount.Should().Be(capacity);
            semaphore.CurrentQueue.Should().Be(0);

            Console.Out.WriteLine($"Success: capacity = {capacity}, parallelism = {parallelism}, duration = {duration}.");
        }
    }
}