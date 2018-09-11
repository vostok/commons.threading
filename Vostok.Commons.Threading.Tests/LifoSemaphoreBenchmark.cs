using System.Threading;
using BenchmarkDotNet.Attributes;

namespace Vostok.Commons.Threading.Tests
{
    public class LifoSemaphoreBenchmark
    {
        private readonly SemaphoreSlim slimSemaphore;
        private readonly LifoSemaphore lifoSemaphore;

        public LifoSemaphoreBenchmark()
        {
            slimSemaphore = new SemaphoreSlim(50, 50);
            lifoSemaphore = new LifoSemaphore(50);
        }

        [Benchmark]
        public void Slim()
        {
            slimSemaphore.WaitAsync().GetAwaiter().GetResult();
            slimSemaphore.Release();
        }

        [Benchmark]
        public void Lifo()
        {
            lifoSemaphore.WaitAsync().GetAwaiter().GetResult();
            lifoSemaphore.Release();
        }
    }
}