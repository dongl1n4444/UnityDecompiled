namespace Unity.IL2CPP.Building.BuildDescriptions
{
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Common;

    public class CppRunnerBuildDescription : IL2CPPOutputBuildDescription
    {
        public CppRunnerBuildDescription(CppRunnerBuildDescription other) : base(other)
        {
        }

        public CppRunnerBuildDescription(NPath sourceDirectory, NPath cacheDirectory, NPath outputFile, DotNetProfile dotnetProfile, CppToolChain cppToolChain, IEnumerable<string> specifiedLinkerFlags, RuntimeBuildType runtimeBuildType, Unity.IL2CPP.Common.Architecture architecture) : base(path, path2, path3, profile, chain, path4, flag, type, architecture2, null, null, null, null, enumerable, null, null)
        {
            NPath path = sourceDirectory;
            NPath path2 = cacheDirectory;
            NPath path3 = outputFile;
            DotNetProfile profile = dotnetProfile;
            CppToolChain chain = cppToolChain;
            NPath path4 = sourceDirectory;
            RuntimeBuildType type = runtimeBuildType;
            Unity.IL2CPP.Common.Architecture architecture2 = architecture;
            IEnumerable<string> enumerable = specifiedLinkerFlags;
        }
    }
}

