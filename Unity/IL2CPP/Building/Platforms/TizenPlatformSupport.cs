namespace Unity.IL2CPP.Building.Platforms
{
    using NiceIO;
    using System;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Building.ToolChains;
    using Unity.IL2CPP.Common;

    internal class TizenPlatformSupport : PlatformSupport
    {
        public override CppToolChain MakeCppToolChain(BuildingOptions buildingOptions) => 
            new TizenToolChain(buildingOptions.Architecture, buildingOptions.Configuration, buildingOptions.TreatWarningsAsErrors, buildingOptions.ToolChainPath);

        public override CppToolChain MakeCppToolChain(Unity.IL2CPP.Building.Architecture architecture, BuildConfiguration buildConfiguration, bool treatWarningsAsErrors) => 
            new TizenToolChain(architecture, buildConfiguration, treatWarningsAsErrors, new NPath(""));

        public override bool Supports(RuntimePlatform platform) => 
            (platform is TizenRuntimePlatform);
    }
}

