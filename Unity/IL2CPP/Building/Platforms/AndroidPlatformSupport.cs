namespace Unity.IL2CPP.Building.Platforms
{
    using NiceIO;
    using System;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Building.ToolChains;
    using Unity.IL2CPP.Common;

    internal class AndroidPlatformSupport : PlatformSupport
    {
        public override CppToolChain MakeCppToolChain(BuildingOptions buildingOptions)
        {
            return new AndroidToolChain(buildingOptions.Architecture, buildingOptions.Configuration, buildingOptions.TreatWarningsAsErrors, buildingOptions.ToolChainPath);
        }

        public override CppToolChain MakeCppToolChain(Unity.IL2CPP.Building.Architecture architecture, BuildConfiguration buildConfiguration, bool treatWarningsAsErrors)
        {
            return new AndroidToolChain(architecture, buildConfiguration, treatWarningsAsErrors, new NPath(""));
        }

        public override bool Supports(RuntimePlatform platform)
        {
            return (platform is AndroidRuntimePlatform);
        }
    }
}

