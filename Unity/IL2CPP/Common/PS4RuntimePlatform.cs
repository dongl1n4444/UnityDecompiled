namespace Unity.IL2CPP.Common
{
    using System;
    using System.Runtime.CompilerServices;

    public class PS4RuntimePlatform : RuntimePlatform
    {
        public override bool ExecutesOnHostMachine =>
            false;

        public override string Name =>
            "PS4";
    }
}

