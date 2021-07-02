using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Commons.Threading.Tests
{
    [TestFixture]
    internal class GuidGenerator_Tests
    {
        [Test]
        [Explicit]
        public void Print_Generated_Guids()
        {
            const int count = 100;
            for (var i = 0; i < count; i++)
                Console.WriteLine(GuidGenerator.GenerateNotCryptoQualityGuid());
        }

        [Test]
        public void Generated_Guids_Should_Be_Unique()
        {
            const int count = 100_000;
            var guids = new HashSet<Guid>();
            for (var i = 0; i < count; i++)
                guids.Add(GuidGenerator.GenerateNotCryptoQualityGuid());

            guids.Count.Should().Be(count);
        }

        [Test]
        public void SmokeTest_Multithreading_Safety()
        {
            const int tasksCount = 10;
            const int guidsInTask = 100_000;

            var barrier = new Barrier(tasksCount);
            var tasks = new Task<HashSet<Guid>>[tasksCount];
            for (var i = 0; i < tasksCount; i++)
                tasks[i] = Task.Run(() =>
                {
                    barrier.SignalAndWait();
                    var guids = new HashSet<Guid>();
                    for (var j = 0; j < guidsInTask; j++)
                        guids.Add(GuidGenerator.GenerateNotCryptoQualityGuid());

                    return guids;
                });

            Task.WhenAll(tasks).GetAwaiter().GetResult();

            var allGuids = tasks.Aggregate(new HashSet<Guid>(),
                (s, t) =>
                {
                    foreach (var guid in t.Result)
                    {
                        s.Add(guid);
                    }

                    return s;
                });

            allGuids.Count.Should().Be(tasksCount * guidsInTask);
        }
    }
}
