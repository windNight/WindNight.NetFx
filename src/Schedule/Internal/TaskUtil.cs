﻿using System.Threading.Tasks;

namespace Schedule
{
    /// <summary>
    ///     Internal helpers for working with tasks.
    /// </summary>
    internal static class TaskUtil
    {
        public static readonly Task CompletedTask = Task.FromResult(true);
    }
}