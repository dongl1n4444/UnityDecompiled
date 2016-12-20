namespace Unity.IL2CPP.Building.Platforms
{
    using System;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Building.BuildDescriptions;
    using Unity.IL2CPP.Building.ToolChains;
    using Unity.IL2CPP.Common;

    internal class EmscriptenPlatformSupport : PlatformSupport
    {
        public override CppToolChain MakeCppToolChain(Unity.IL2CPP.Building.Architecture architecture, BuildConfiguration buildConfiguration, bool treatWarningsAsErrors)
        {
            return new EmscriptenToolChain(architecture, buildConfiguration, false);
        }

        public override ProgramBuildDescription PostProcessProgramBuildDescription(ProgramBuildDescription programBuildDescription)
        {
            IL2CPPOutputBuildDescription buildDescription = programBuildDescription as IL2CPPOutputBuildDescription;
            if (buildDescription == null)
            {
                return programBuildDescription;
            }
            return new EmscriptenIL2CPPOutputBuildDescription(buildDescription);
        }

        public override bool Supports(RuntimePlatform platform)
        {
            return (platform is WebGLRuntimePlatform);
        }
    }
}

