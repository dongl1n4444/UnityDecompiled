namespace UnityEditor.WindowsStandalone
{
    using System;
    using UnityEditor;
    using UnityEditorInternal;

    internal class WindowsStandaloneIl2CppPlatformProvider : BaseIl2CppPlatformProvider
    {
        private readonly bool m_DevelopmentBuild;

        internal WindowsStandaloneIl2CppPlatformProvider(BuildTarget target, string dataFolder, bool developmentBuild) : base(target, Path.Combine(dataFolder, "Libraries"))
        {
            this.m_DevelopmentBuild = developmentBuild;
        }

        public override Il2CppNativeCodeBuilder CreateIl2CppNativeCodeBuilder() => 
            new WindowsStandaloneIL2CppNativeCodeBuilder(this.target);

        public override bool developmentMode =>
            this.m_DevelopmentBuild;

        public override bool emitNullChecks =>
            this.m_DevelopmentBuild;

        public override bool enableStackTraces =>
            this.m_DevelopmentBuild;

        public override string[] includePaths =>
            new string[0];

        public override string nativeLibraryFileName =>
            "UserAssembly.dll";

        public override string staticLibraryExtension =>
            "lib";
    }
}

