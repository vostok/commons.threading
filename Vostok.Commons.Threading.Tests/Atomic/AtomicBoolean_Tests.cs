using FluentAssertions;
using NUnit.Framework;
using Vostok.Commons.Threading.Atomic;

namespace Vostok.Commons.Threading.Tests.Atomic
{
    [TestFixture]
    public class AtomicBoolean_Tests
    {
        private AtomicBoolean boolean;

        [SetUp]
        public void TestSetup()
        {
            boolean = new AtomicBoolean(false);
        }

        [Test]
        public void TrySetTrue_should_return_true_if_value_was_false()
        {
            boolean.TrySetTrue().Should().BeTrue();
        }

        [Test]
        public void TrySetTrue_should_set_value_to_true()
        {
            boolean.TrySetTrue();

            boolean.Value.Should().BeTrue();
        }

        [Test]
        public void TrySetTrue_should_return_false_if_value_was_true()
        {
            boolean.TrySetTrue();

            boolean.TrySetTrue().Should().BeFalse();
        }

        [Test]
        public void TrySetFalse_should_return_true_if_value_was_true()
        {
            boolean.Value = true;

            boolean.TrySetFalse().Should().BeTrue();
        }

        [Test]
        public void TrySetFalse_should_set_value_to_false()
        {
            boolean.Value = true;

            boolean.TrySetFalse();

            boolean.Value.Should().BeFalse();
        }

        [Test]
        public void TrySetFalse_should_return_false_if_value_was_false()
        {
            boolean.TrySetFalse();

            boolean.TrySetFalse().Should().BeFalse();
        }
    }
}