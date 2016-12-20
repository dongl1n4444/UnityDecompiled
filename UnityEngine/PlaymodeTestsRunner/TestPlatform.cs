namespace UnityEngine.PlaymodeTestsRunner
{
    using System;

    [Flags]
    public enum TestPlatform : ulong
    {
        All = 18446744073709551615L,
        EditMode = 2L,
        PlayMode = 4L
    }
}

