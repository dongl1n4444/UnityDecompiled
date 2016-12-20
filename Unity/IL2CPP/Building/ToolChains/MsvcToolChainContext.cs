namespace Unity.IL2CPP.Building.ToolChains
{
    using NiceIO;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal class MsvcToolChainContext : CppToolChainContext
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NPath <ManifestPath>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NPath <ModuleDefinitionPath>k__BackingField;

        public NPath ManifestPath { get; set; }

        public NPath ModuleDefinitionPath { get; set; }
    }
}

