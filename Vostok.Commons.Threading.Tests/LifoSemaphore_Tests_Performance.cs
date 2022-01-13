using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;
using NUnit.Framework;

namespace Vostok.Commons.Threading.Tests
{
    [TestFixture]
    [Explicit]
    internal class LifoSemaphore_Tests_Performance
    {
        [SetUp]
        public void TestSetup()
        {
            ThreadPool.SetMinThreads(2048, 2048);
        }

        [Test]
        [Explicit]
        public void Benchmark_single_threaded_speed_vs_SemaphoreSlim()
        {
            BenchmarkRunner.Run<LifoSemaphoreBenchmark>(
                DefaultConfig.Instance
                    .AddDiagnoser(MemoryDiagnoser.Default)
                    .WithOption(ConfigOptions.DisableOptimizationsValidator, true));
        }

        [TestCase(1, 1, 500 * 1000)]
        [TestCase(1, 2, 75 * 1000)]
        [TestCase(1, 4, 50 * 1000)]
        [TestCase(1, 8, 30 * 1000)]
        [TestCase(1, 16, 15 * 1000)]
        [TestCase(1, 32, 10 * 1000)]
        [TestCase(2, 2, 250 * 1000)]
        [TestCase(2, 3, 45 * 1000)]
        [TestCase(2, 4, 40 * 1000)]
        [TestCase(2, 8, 25 * 1000)]
        [TestCase(2, 16, 12 * 1000)]
        [TestCase(2, 32, 10 * 1000)]
        [TestCase(3, 3, 45 * 1000)]
        [TestCase(3, 4, 40 * 1000)]
        [TestCase(3, 8, 25 * 1000)]
        [TestCase(3, 16, 12 * 1000)]
        [TestCase(3, 32, 10 * 1000)]
        [TestCase(4, 4, 40 * 1000)]
        [TestCase(4, 5, 25 * 1000)]
        [TestCase(4, 8, 20 * 1000)]
        [TestCase(4, 16, 12 * 1000)]
        [TestCase(4, 32, 10 * 1000)]
        [TestCase(4, 64, 3 * 1000)]
        [TestCase(8, 8, 250 * 1000)]
        [TestCase(8, 16, 25 * 1000)]
        [TestCase(8, 32, 20 * 1000)]
        [TestCase(8, 64, 3 * 1000)]
        [TestCase(8, 128, 3 * 1000)]
        [TestCase(16, 16, 250 * 1000)]
        [TestCase(16, 20, 50 * 1000)]
        [TestCase(16, 32, 40 * 1000)]
        [TestCase(16, 64, 15 * 1000)]
        [TestCase(16, 128, 10 * 1000)]
        [TestCase(32, 32, 250 * 1000)]
        [TestCase(32, 45, 150 * 1000)]
        [TestCase(32, 64, 75 * 1000)]
        [TestCase(32, 128, 25 * 1000)]
        [TestCase(32, 256, 7 * 1000)]
        [TestCase(64, 64, 75 * 1000)]
        [TestCase(64, 65, 75 * 1000)]
        [TestCase(64, 100, 45 * 1000)]
        [TestCase(64, 128, 20 * 1000)]
        [TestCase(64, 256, 10 * 1000)]
        [TestCase(250, 32, 100 * 1000)]
        [TestCase(250, 64, 100 * 1000)]
        [TestCase(250, 128, 75 * 1000)]
        [TestCase(250, 192, 75 * 1000)]
        [TestCase(250, 249, 50 * 1000)]
        [TestCase(250, 251, 40 * 1000)]
        [TestCase(250, 260, 40 * 1000)]
        [TestCase(250, 300, 25 * 1000)]
        [TestCase(250, 1001, 5 * 1000)]
        [Explicit]
        public void Benchmark_multi_threaded_speed_vs_SemaphoreSlim(int capacity, int parallelism, int iterations)
        {
            var slimTimes = new List<double>();
            var lifoTimes = new List<double>();

            var slimSemaphore = new SemaphoreSlim(capacity, capacity);
            var lifoSemaphore = new LifoSemaphore(capacity);

            for (var i = 0; i < 5; i++)
            {
                var watch = Stopwatch.StartNew();

                var tasks = new List<Task>();

                for (var j = 0; j < parallelism; j++)
                {
                    tasks.Add(
                        Task.Run(
                            async () =>
                            {
                                for (var k = 0; k < iterations; k++)
                                {
                                    await slimSemaphore.WaitAsync();

                                    slimSemaphore.Release();
                                }
                            }));
                }

                Task.WhenAll(tasks).GetAwaiter().GetResult();

                slimTimes.Add(watch.Elapsed.TotalMilliseconds);

                Console.Out.WriteLine($"Slim: {watch.Elapsed.TotalMilliseconds:F2} ms");
            }

            var avgSlimTime = slimTimes.Average();

            Console.Out.WriteLine();
            Console.Out.WriteLine($"Slim avg: {avgSlimTime:F2} ms");
            Console.Out.WriteLine();

            for (var i = 0; i < 5; i++)
            {
                var watch = Stopwatch.StartNew();

                var tasks = new List<Task>();

                for (var j = 0; j < parallelism; j++)
                {
                    tasks.Add(
                        Task.Run(
                            async () =>
                            {
                                for (var k = 0; k < iterations; k++)
                                {
                                    await lifoSemaphore.WaitAsync();

                                    lifoSemaphore.Release();
                                }
                            }));
                }

                Task.WhenAll(tasks).GetAwaiter().GetResult();

                lifoTimes.Add(watch.Elapsed.TotalMilliseconds);

                Console.Out.WriteLine($"Lifo: {watch.Elapsed.TotalMilliseconds:F2} ms");
            }

            var avgLifoTime = lifoTimes.Average();

            Console.Out.WriteLine();
            Console.Out.WriteLine($"Lifo avg: {avgLifoTime:F2} ms");
            Console.Out.WriteLine();

            if (avgLifoTime < avgSlimTime)
            {
                Console.Out.WriteLine($"Winner is LifoSemaphore ({avgSlimTime / avgLifoTime:F2}x)!");
                Assert.Pass();
            }
            else
            {
                Console.Out.WriteLine($"Winner is SemaphoreSlim ({avgLifoTime / avgSlimTime:F2}x)!");
                Assert.Inconclusive();
            }
        }
    }
}