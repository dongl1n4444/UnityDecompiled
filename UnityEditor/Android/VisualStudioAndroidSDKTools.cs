namespace UnityEditor.Android
{
    using System;
    using System.IO;
    using System.Linq;
    using UnityEditorInternal;

    internal class VisualStudioAndroidSDKTools : AndroidSDKTools
    {
        private Version m_BuildToolsVersion;
        private static VisualStudioAndroidSDKTools s_Instance;

        private VisualStudioAndroidSDKTools(string sdkRoot) : base(sdkRoot)
        {
        }

        protected override Version GetBuildToolsUpdateVersion() => 
            this.m_BuildToolsVersion;

        public static VisualStudioAndroidSDKTools GetInstance()
        {
            string sdkRoot = GetSdkRoot();
            if (!AndroidSdkRoot.IsSdkDir(sdkRoot))
            {
                return null;
            }
            if ((s_Instance == null) || (s_Instance.SDKRootDir != sdkRoot))
            {
                s_Instance = new VisualStudioAndroidSDKTools(sdkRoot);
            }
            return s_Instance;
        }

        protected override string GetSDKBuildToolsDir() => 
            Enumerable.FirstOrDefault<string>(Directory.GetDirectories(Path.Combine(base.SDKRootDir, "build-tools")), (Func<string, bool>) (buildToolsDir => (AndroidComponentVersion.GetComponentVersion(buildToolsDir) == this.m_BuildToolsVersion)));

        private static string GetSdkRoot()
        {
            string str = RegistryUtil.GetRegistryStringValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\SecondaryInstaller\VC", "AndroidHome", null, RegistryView.Default);
            if (!string.IsNullOrEmpty(str))
            {
                return str;
            }
            str = RegistryUtil.GetRegistryStringValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Android SDK Tools", "Path", null, RegistryView.Default);
            if (!string.IsNullOrEmpty(str))
            {
                return str;
            }
            return RegistryUtil.GetRegistryStringValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Android SDK Tools", "Path", null, RegistryView._32);
        }

        public void SetBuildToolsVersion(int version)
        {
            this.m_BuildToolsVersion = new Version(version, 0, 0);
            base.UpdateToolsDirectories();
        }

        public override bool IsVisualStudio =>
            true;
    }
}

