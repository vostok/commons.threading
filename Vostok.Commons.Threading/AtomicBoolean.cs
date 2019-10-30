using System.Threading;
using JetBrains.Annotations;

namespace Vostok.Commons.Threading
{
    [PublicAPI]
    internal class AtomicBoolean
    {
        private const int TrueState = 1;
        private const int FalseState = 0;

        private int state;

        public AtomicBoolean(bool initialValue) =>
            state = initialValue ? TrueState : FalseState;

        public bool Value
        {
            get => state == TrueState;
            set => Interlocked.Exchange(ref state, value ? TrueState : FalseState);
        }

        public bool TrySetTrue() =>
            Interlocked.CompareExchange(ref state, TrueState, FalseState) == FalseState;

        public bool TrySetFalse() =>
            Interlocked.CompareExchange(ref state, FalseState, TrueState) == TrueState;

        public bool TrySet(bool value) =>
            value ? TrySetTrue() : TrySetFalse();

        public override string ToString() =>
            $"{nameof(Value)}: {Value}";

        public static implicit operator bool([NotNull] AtomicBoolean atomicBoolean) =>
            atomicBoolean.Value;

        public static implicit operator AtomicBoolean(bool initialValue) =>
            new AtomicBoolean(initialValue);
    }
}