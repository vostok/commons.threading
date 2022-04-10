using System;
#if NET6_0_OR_GREATER
using System.Runtime.CompilerServices;
#else
#endif
using JetBrains.Annotations;

namespace Vostok.Commons.Threading
{
    [PublicAPI]
    internal static class GuidGenerator
    {
#if NET6_0_OR_GREATER
        [SkipLocalsInit]
#else
#endif
        public static unsafe Guid GenerateNotCryptoQualityGuid()
        {
            var bytes = stackalloc byte[16];
            var dst = bytes;

            var random = ThreadSafeRandom.ObtainThreadStaticRandom();

#if NET6_0_OR_GREATER
            random.NextBytes(new Span<byte>(bytes, 16));
#else
            for (var i = 0; i < 4; i++)
            {
                *(int*)dst = random.Next();
                dst += 4;
            }
#endif

            return *(Guid*)bytes;
        }
    }
}
