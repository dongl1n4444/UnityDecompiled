namespace Unity.IL2CPP.Common
{
    using System;
    using System.Runtime.CompilerServices;

    public class PSP2RuntimePlatform : RuntimePlatform
    {
        public override bool ExecutesOnHostMachine =>
            false;

        public override string Name =>
            "PSP2";
    }
}

