using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using NUnit.Framework;

namespace Vostok.Commons.Threading.Tests
{
    [TestFixture, Explicit]
    public class GuidGenerator_Benchmarks
    {
        [Test]
        public void RunBenchmark()
        {
            BenchmarkRunner.Run<GuidGenerator_Benchmarks>(
                DefaultConfig.Instance
                    .AddJob(Job.Default.WithRuntime(ClrRuntime.Net48).WithId(".Net 4.8"))
                    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core50).WithId(".Net 5.0"))
                    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core60).WithId(".Net 6.0"))
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

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1526 (21H2)
Intel Core i7-4771 CPU 3.50GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.103
  [Host]   : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT
  .Net 4.8 : .NET Framework 4.8 (4.8.4470.0), X64 RyuJIT
  .Net 5.0 : .NET 5.0.15 (5.0.1522.11506), X64 RyuJIT
  .Net 6.0 : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT


|      Method |            Runtime |     Mean |
|------------ |------------------- |---------:|
| GuidNewGuid | .NET Framework 4.8 | 81.36 ns |
|    Generate | .NET Framework 4.8 | 55.60 ns |
|------------ |------------------- |---------:|
| GuidNewGuid |           .NET 5.0 | 80.87 ns |
|    Generate |           .NET 5.0 | 45.42 ns |
|------------ |------------------- |---------:|
| GuidNewGuid |           .NET 6.0 | 80.38 ns |
|    Generate |           .NET 6.0 | 15.40 ns |
         */
    }
}