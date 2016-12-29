namespace Unity.IL2CPP.Building.Platforms
{
    using System;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Building.ToolChains;
    using Unity.IL2CPP.Common;

    internal class TizenPlatformSupport : PlatformSupport
    {
        public override CppToolChain MakeCppToolChain(Unity.IL2CPP.Building.Architecture architecture, BuildConfiguration buildConfiguration, bool treatWarningsAsErrors) => 
            new TizenToolChain(architecture, buildConfiguration);

        public override bool Supports(RuntimePlatform platform) => 
            (platform is TizenRuntimePlatform);
    }
}

