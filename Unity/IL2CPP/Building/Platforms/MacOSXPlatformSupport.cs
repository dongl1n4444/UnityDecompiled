namespace Unity.IL2CPP.Building.Platforms
{
    using System;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Building.ToolChains;
    using Unity.IL2CPP.Common;

    internal class MacOSXPlatformSupport : PlatformSupport
    {
        public override CppToolChain MakeCppToolChain(Unity.IL2CPP.Common.Architecture architecture, BuildConfiguration buildConfiguration, bool treatWarningsAsErrors) => 
            new ClangToolChain(architecture, buildConfiguration, treatWarningsAsErrors);

        public override bool Supports(RuntimePlatform platform) => 
            (platform is MacOSXRuntimePlatform);
    }
}

