namespace UnityEditor.iOS.Il2Cpp
{
    using System;
    using UnityEditor;
    using UnityEditor.BuildReporting;
    using UnityEditorInternal;

    internal class iOSIl2CppPlatformProvider : BaseIl2CppPlatformProvider
    {
        private readonly BuildReport m_BuildReport;
        private readonly bool m_IsDevelopmentBuild;

        public iOSIl2CppPlatformProvider(BuildTarget target, bool isDevelopmentBuild, string dataDirectory, BuildReport report) : base(target, Path.Combine(dataDirectory, "Libraries"))
        {
            this.m_IsDevelopmentBuild = isDevelopmentBuild;
            this.m_BuildReport = report;
        }

        public override BuildReport buildReport =>
            this.m_BuildReport;

        public override bool developmentMode =>
            this.m_IsDevelopmentBuild;

        public override bool enableStackTraces =>
            false;

        public bool platformHasPrecompiledLibIl2Cpp =>
            false;

        public override bool supportsEngineStripping =>
            true;
    }
}

