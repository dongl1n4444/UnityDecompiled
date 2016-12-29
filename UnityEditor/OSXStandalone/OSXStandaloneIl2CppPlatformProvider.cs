namespace UnityEditor.OSXStandalone
{
    using System;
    using UnityEditor;
    using UnityEditorInternal;

    internal class OSXStandaloneIl2CppPlatformProvider : BaseIl2CppPlatformProvider
    {
        private readonly bool m_DevelopmentBuild;

        public OSXStandaloneIl2CppPlatformProvider(BuildTarget target, string dataFolder, bool developmentBuild) : base(target, Path.Combine(dataFolder, "Libraries"))
        {
            this.m_DevelopmentBuild = developmentBuild;
        }

        public override Il2CppNativeCodeBuilder CreateIl2CppNativeCodeBuilder() => 
            new OSXStandaloneIL2CppNativeCodeBuilder(this.target);

        public override bool developmentMode =>
            this.m_DevelopmentBuild;

        public override bool emitNullChecks =>
            this.m_DevelopmentBuild;

        public override bool enableStackTraces =>
            this.m_DevelopmentBuild;

        public override string[] includePaths =>
            new string[0];

        public override string nativeLibraryFileName =>
            "UserAssembly.dylib";
    }
}

