namespace UnityEngine.PlaymodeTestsRunner
{
    using System;
    using System.Runtime.CompilerServices;

    [Extension]
    internal static class TestPlatformEnumExtensions
    {
        [Extension]
        public static bool IsFlagIncluded(TestPlatform flags, TestPlatform flag)
        {
            return ((flags & flag) == flag);
        }
    }
}

