namespace Unity.IL2CPP.Common
{
    using System;

    public class UnsupportedRuntimePlatform : RuntimePlatform
    {
        public override string Name
        {
            get
            {
                return "Unsupported";
            }
        }
    }
}

