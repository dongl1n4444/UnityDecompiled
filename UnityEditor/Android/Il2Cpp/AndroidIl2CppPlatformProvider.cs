namespace UnityEditor.Android.Il2Cpp
{
    using System;
    using UnityEditor;
    using UnityEditor.Android;
    using UnityEditorInternal;

    internal class AndroidIl2CppPlatformProvider : BaseIl2CppPlatformProvider
    {
        private AndroidTargetDeviceType m_DeviceType;
        private readonly bool m_IsDevelopmentBuild;

        public AndroidIl2CppPlatformProvider(BuildTarget target, AndroidTargetDeviceType deviceType, bool isDevelopmentBuild) : base(target, BuildPipeline.GetBuildToolsDirectory(BuildTarget.Android))
        {
            this.m_IsDevelopmentBuild = isDevelopmentBuild;
            this.m_DeviceType = deviceType;
        }

        public override Il2CppNativeCodeBuilder CreateIl2CppNativeCodeBuilder()
        {
            return new AndroidIl2CppNativeCodeBuilder(this.m_DeviceType);
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

        public override string[] libraryPaths
        {
            get
            {
                return new string[0];
            }
        }

        public override bool loadSymbols
        {
            get
            {
                return this.m_IsDevelopmentBuild;
            }
        }

        public override string nativeLibraryFileName
        {
            get
            {
                return "libil2cpp.so";
            }
        }
    }
}

