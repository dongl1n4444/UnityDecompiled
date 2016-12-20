namespace Unity.IL2CPP.Building
{
    using System;

    public class x64Architecture : Architecture
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
                return "x64";
            }
        }
    }
}

