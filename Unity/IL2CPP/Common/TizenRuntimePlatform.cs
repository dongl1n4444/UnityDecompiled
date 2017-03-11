namespace Unity.IL2CPP.Common
{
    using System;

    public class TizenRuntimePlatform : RuntimePlatform
    {
        public override bool ExecutesOnHostMachine =>
            false;

        public override string Name =>
            "Tizen";
    }
}

