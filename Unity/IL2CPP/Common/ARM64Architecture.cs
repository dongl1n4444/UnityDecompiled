namespace Unity.IL2CPP.Common
{
    using System;
    using System.Runtime.CompilerServices;

    public class ARM64Architecture : Unity.IL2CPP.Common.Architecture
    {
        public override int Bits =>
            0x40;

        public override string Name =>
            "ARM64";
    }
}

