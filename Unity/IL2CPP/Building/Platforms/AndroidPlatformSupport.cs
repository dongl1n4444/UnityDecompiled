namespace Unity.IL2CPP.Building.Platforms
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Building.BuildDescriptions;
    using Unity.IL2CPP.Building.ToolChains;
    using Unity.IL2CPP.Common;

    internal class AndroidPlatformSupport : PlatformSupport
    {
        public override Unity.IL2CPP.Common.Architecture GetSupportedArchitectureOfSameBitness(Unity.IL2CPP.Common.Architecture architecture)
        {
            if (architecture.Bits != 0x20)
            {
                throw new NotSupportedException($"Android doesn't support {architecture.Bits}-bit architecture.");
            }
            return new ARMv7Architecture();
        }

        public override CppToolChain MakeCppToolChain(BuildingOptions buildingOptions) => 
            new AndroidToolChain(buildingOptions.Architecture, buildingOptions.Configuration, buildingOptions.TreatWarningsAsErrors, buildingOptions.ToolChainPath);

        public override CppToolChain MakeCppToolChain(Unity.IL2CPP.Common.Architecture architecture, BuildConfiguration buildConfiguration, bool treatWarningsAsErrors) => 
            new AndroidToolChain(architecture, buildConfiguration, treatWarningsAsErrors, null);

        public override ProgramBuildDescription PostProcessProgramBuildDescription(ProgramBuildDescription programBuildDescription)
        {
            CppRunnerBuildDescription other = programBuildDescription as CppRunnerBuildDescription;
            return ((other == null) ? base.PostProcessProgramBuildDescription(programBuildDescription) : new AndroidCppRunnerBuildDescription(other));
        }

        public override bool Supports(RuntimePlatform platform) => 
            (platform is AndroidRuntimePlatform);

        private class AndroidCppRunnerBuildDescription : CppRunnerBuildDescription
        {
            public AndroidCppRunnerBuildDescription(CppRunnerBuildDescription other) : base(other)
            {
            }

            public override IEnumerable<string> AdditionalCompilerFlags =>
                base.AdditionalCompilerFlags.Append<string>("-fvisibility=default");

            public override IEnumerable<string> AdditionalLinkerFlags =>
                base.AdditionalLinkerFlags.Append<string>("-rdynamic");
        }
    }
}

