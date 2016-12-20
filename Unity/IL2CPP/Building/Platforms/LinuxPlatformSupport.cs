namespace Unity.IL2CPP.Building.Platforms
{
    using System;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Building.ToolChains;
    using Unity.IL2CPP.Common;

    internal class LinuxPlatformSupport : PlatformSupport
    {
        public override CppToolChain MakeCppToolChain(Unity.IL2CPP.Building.Architecture architecture, BuildConfiguration buildConfiguration, bool treatWarningsAsErrors)
        {
            return new GccToolChain(architecture, buildConfiguration);
        }

        public override bool Supports(RuntimePlatform platform)
        {
            return (platform is LinuxRuntimePlatform);
        }
    }
}

