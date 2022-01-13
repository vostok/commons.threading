using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;
using NUnit.Framework;

namespace Vostok.Commons.Threading.Tests
{
    [TestFixture]
    public class GuidGenerator_Benchmarks
    {
        [Test]
        public void RunBenchmark()
        {
            BenchmarkRunner.Run<GuidGenerator_Benchmarks>(
                DefaultConfig.Instance
                    .AddDiagnoser(MemoryDiagnoser.Default)
                    .WithOption(ConfigOptions.DisableOptimizationsValidator, true));
        }

        [Benchmark]
        public Guid GuidNewGuid()
        {
            return Guid.NewGuid();
        }

        [Benchmark]
        public Guid Generate()
        {
            return GuidGenerator.GenerateNotCryptoQualityGuid();
        }

        /*
// * Summary *

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1415 (21H1/May2021Update)
Intel Core i7-4771 CPU 3.50GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  DefaultJob : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT


|      Method |     Mean |    Error |   StdDev | Allocated |
|------------ |---------:|---------:|---------:|----------:|
| GuidNewGuid | 76.10 ns | 0.623 ns | 0.552 ns |         - |
|    Generate | 42.97 ns | 0.150 ns | 0.133 ns |         - |
         */
    }
}