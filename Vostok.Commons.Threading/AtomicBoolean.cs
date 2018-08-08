using System.Threading;
using JetBrains.Annotations;

namespace Vostok.Commons.Threading
{
    [PublicAPI]
#if MAKE_CLASSES_PUBLIC
    public
#else
    internal
#endif
        class AtomicBoolean
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

        public static implicit operator bool([NotNull] AtomicBoolean atomicBoolean) =>
            atomicBoolean.Value;
    }
}