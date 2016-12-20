namespace Unity.IL2CPP.Building
{
    using System;

    public class x86Architecture : Architecture
    {
        public override int Bits
        {
            get
            {
                return 0x20;
            }
        }

        public override string Name
        {
            get
            {
                return "x86";
            }
        }
    }
}

