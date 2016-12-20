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

        public override bool developmentMode
        {
            get
            {
                return this.m_IsDevelopmentBuild;
            }
        }

        public override bool enableStackTraces
        {
            get
            {
                return false;
            }
        }

        public override bool loadSymbols
        {
            get
            {
                return this.m_IsDevelopmentBuild;
            }
        }

        public bool platformHasPrecompiledLibIl2Cpp
        {
            get
            {
                return false;
            }
        }

        public override bool supportsEngineStripping
        {
            get
            {
                return true;
            }
        }
    }
}

