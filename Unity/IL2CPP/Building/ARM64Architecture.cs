namespace Unity.IL2CPP.Building
{
    using System;

    public class ARM64Architecture : Architecture
    {
        public override int Bits
        {
            get
            {
                return 0x40;
            }
        }

        public override string Name
        {
            get
            {
                return "ARM64";
            }
        }
    }
}

