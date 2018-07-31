using System.Threading;

namespace Vostok.Commons.Threading.Atomic
{
    public class AtomicBoolean
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

        public static implicit operator bool(AtomicBoolean atomicBoolean) =>
            atomicBoolean.Value;
    }
}