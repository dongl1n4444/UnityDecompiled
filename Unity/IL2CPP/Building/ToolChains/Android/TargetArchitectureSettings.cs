namespace Unity.IL2CPP.Building.ToolChains.Android
{
    using System;
    using System.Collections.Generic;

    internal abstract class TargetArchitectureSettings
    {
        protected TargetArchitectureSettings()
        {
        }

        public abstract string ABI { get; }

        public abstract string Arch { get; }

        public abstract string BinPrefix { get; }

        public abstract IEnumerable<string> CxxFlags { get; }

        public abstract IEnumerable<string> LDFlags { get; }

        public abstract string Platform { get; }

        public abstract string TCPrefix { get; }
    }
}

