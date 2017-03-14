namespace Unity.IL2CPP.Common
{
    using System;
    using System.Runtime.CompilerServices;

    public class UnsupportedRuntimePlatform : RuntimePlatform
    {
        public override string Name =>
            "Unsupported";
    }
}

