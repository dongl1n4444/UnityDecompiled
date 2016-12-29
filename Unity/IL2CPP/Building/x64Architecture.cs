namespace Unity.IL2CPP.Building
{
    using System;

    public class x64Architecture : Architecture
    {
        public override int Bits =>
            0x40;

        public override string Name =>
            "x64";
    }
}

