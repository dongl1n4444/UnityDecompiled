namespace Unity.IL2CPP.Common
{
    using System;

    public class PSP2RuntimePlatform : RuntimePlatform
    {
        public override bool ExecutesOnHostMachine
        {
            get
            {
                return false;
            }
        }

        public override string Name
        {
            get
            {
                return "PSP2";
            }
        }
    }
}

