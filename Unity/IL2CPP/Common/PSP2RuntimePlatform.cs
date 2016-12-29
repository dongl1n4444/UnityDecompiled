namespace Unity.IL2CPP.Common
{
    using System;

    public class PSP2RuntimePlatform : RuntimePlatform
    {
        public override bool ExecutesOnHostMachine =>
            false;

        public override string Name =>
            "PSP2";
    }
}

