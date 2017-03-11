namespace UnityEngine.TestTools
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class TestPlatformEnumExtensions
    {
        public static bool IsFlagIncluded(this TestPlatform flags, TestPlatform flag) => 
            ((flags & flag) == flag);
    }
}

