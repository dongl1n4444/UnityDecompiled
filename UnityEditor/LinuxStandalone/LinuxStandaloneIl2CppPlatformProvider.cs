namespace UnityEditor.LinuxStandalone
{
    using System;
    using UnityEditor;
    using UnityEditorInternal;

    internal class LinuxStandaloneIl2CppPlatformProvider : BaseIl2CppPlatformProvider
    {
        private readonly bool m_DevelopmentBuild;

        public LinuxStandaloneIl2CppPlatformProvider(BuildTarget target, string dataFolder, bool developmentBuild) : base(target, Path.Combine(dataFolder, "Libraries"))
        {
            this.m_DevelopmentBuild = developmentBuild;
        }

        public override Il2CppNativeCodeBuilder CreateIl2CppNativeCodeBuilder() => 
            new LinuxStandaloneIL2CppNativeCodeBuilder(this.target);

        public override bool developmentMode =>
            this.m_DevelopmentBuild;

        public override bool emitNullChecks =>
            false;

        public override bool enableStackTraces =>
            false;

        public override string[] includePaths =>
            new string[0];

        public override string nativeLibraryFileName =>
            "libUserAssembly.so";

        public bool platformHasPrecompiledLibIl2Cpp =>
            false;
    }
}

