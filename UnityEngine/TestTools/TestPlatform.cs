namespace UnityEngine.TestTools
{
    using System;

    /// <summary>
    /// <para>Platforms the tests can run on.</para>
    /// </summary>
    [Flags]
    public enum TestPlatform : ulong
    {
        All = 18446744073709551615L,
        EditMode = 2L,
        PlayMode = 4L
    }
}

