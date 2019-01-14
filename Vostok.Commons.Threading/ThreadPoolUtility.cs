﻿using System;
using System.Threading;
using JetBrains.Annotations;

namespace Vostok.Commons.Threading
{
    [PublicAPI]
    internal static class ThreadPoolUtility
    {
        public const int MaximumThreads = short.MaxValue;

        public static void Setup(int multiplier = 128)
        {
            if (multiplier <= 0)
                return;

            var minimumThreads = Math.Min(Environment.ProcessorCount * multiplier, MaximumThreads);

            ThreadPool.SetMaxThreads(MaximumThreads, MaximumThreads);
            ThreadPool.SetMinThreads(minimumThreads, minimumThreads);
        }

        public static ThreadPoolState GetPoolState()
        {
            ThreadPool.GetMinThreads(out var minWorkerThreads, out var minIocpThreads);
            ThreadPool.GetMaxThreads(out var maxWorkerThreads, out var maxIocpThreads);
            ThreadPool.GetAvailableThreads(out var availableWorkerThreads, out var availableIocpThreads);

            return new ThreadPoolState(
                minWorkerThreads,
                maxWorkerThreads - availableWorkerThreads,
                minIocpThreads,
                maxIocpThreads - availableIocpThreads);
        }
    }
}