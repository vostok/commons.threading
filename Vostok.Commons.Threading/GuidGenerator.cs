using System;
using JetBrains.Annotations;

namespace Vostok.Commons.Threading
{
    [PublicAPI]
    internal static class GuidGenerator
    {
        public static unsafe Guid GenerateNotCryptoQualityGuid()
        {
            var bytes = stackalloc byte[16];
            var dst = bytes;

#if NET6_0_OR_GREATER
            var random = Random.Shared;
#else
            var random = ThreadSafeRandom.ObtainThreadStaticRandom();
#endif
            for (var i = 0; i < 4; i++)
            {
                *(int*)dst = random.Next();
                dst += 4;
            }

            return *(Guid*)bytes;
        }
    }
}
