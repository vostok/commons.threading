﻿using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Commons.Threading.Tests
{
    [TestFixture]
    internal class AtomicLong_Tests
    {
        private AtomicLong atomicNumber;

        [SetUp]
        public void TestSetup()
        {
            atomicNumber = new AtomicLong(0);
        }

        [Test]
        public void Add_is_working()
        {
            atomicNumber.Add(1).Should().Be(1);
            atomicNumber.Add(2).Should().Be(3);
            atomicNumber.Add(-10).Should().Be(-7);
        }

        [Test]
        public void CompareExchange_Is_Working()
        {
            atomicNumber.Value = 1;
            atomicNumber.CompareExchange(2, 0).Should().Be(1);
            atomicNumber.Value.Should().Be(1);

            atomicNumber.CompareExchange(2, 1).Should().Be(1);
            atomicNumber.Value.Should().Be(2);
        }

        [Test]
        public void Exchange_Is_Working()
        {
            atomicNumber.Value = 1;
            atomicNumber.Exchange(2).Should().Be(1);
            atomicNumber.Value.Should().Be(2);
        }

        [Test]
        public void Increment_should_return_incremented_value()
        {
            atomicNumber.Increment().Should().Be(1);
        }

        [Test]
        public void Increment_should_mutate_value()
        {
            atomicNumber.Increment();

            atomicNumber.Value.Should().Be(1);
        }

        [Test]
        public void Decrement_should_return_decremented_value()
        {
            atomicNumber.Decrement().Should().Be(-1);
        }

        [Test]
        public void Decrement_should_mutate_value()
        {
            atomicNumber.Decrement();

            atomicNumber.Value.Should().Be(-1);
        }

        [Test]
        public void TrySet_should_return_true_when_current_value_matches_expected()
        {
            atomicNumber.TrySet(10, 0).Should().BeTrue();
        }

        [Test]
        public void TrySet_should_update_value_when_current_value_matches_expected()
        {
            atomicNumber.TrySet(10, 0);

            atomicNumber.Value.Should().Be(10);
        }

        [Test]
        public void TrySet_should_return_false_when_current_value_does_not_match_expected()
        {
            atomicNumber.TrySet(10, 1).Should().BeFalse();
        }

        [Test]
        public void TrySet_should_not_update_value_when_current_value_does_not_match_expected()
        {
            atomicNumber.TrySet(10, 1).Should().BeFalse();

            atomicNumber.Value.Should().Be(0);
        }
    }
}