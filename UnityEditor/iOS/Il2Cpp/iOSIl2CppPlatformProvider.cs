namespace UnityEditor.iOS.Il2Cpp
{
    using System;
    using UnityEditor;
    using UnityEditorInternal;

    internal class iOSIl2CppPlatformProvider : BaseIl2CppPlatformProvider
    {
        private readonly bool m_IsDevelopmentBuild;

        public iOSIl2CppPlatformProvider(BuildTarget target, bool isDevelopmentBuild, string dataDirectory) : base(target, Path.Combine(dataDirectory, "Libraries"))
        {
            this.m_IsDevelopmentBuild = isDevelopmentBuild;
        }

        public override bool developmentMode =>
            this.m_IsDevelopmentBuild;

        public override bool enableStackTraces =>
            false;

        public override bool loadSymbols =>
            this.m_IsDevelopmentBuild;

        public bool platformHasPrecompiledLibIl2Cpp =>
            false;

        public override bool supportsEngineStripping =>
            true;
    }
}

