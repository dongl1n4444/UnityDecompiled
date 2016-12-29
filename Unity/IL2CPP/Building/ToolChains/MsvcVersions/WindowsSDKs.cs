namespace Unity.IL2CPP.Building.ToolChains.MsvcVersions
{
    using Microsoft.Win32;
    using NiceIO;
    using System;
    using System.Runtime.InteropServices;

    public static class WindowsSDKs
    {
        public static NPath GetDotNetFrameworkSDKDirectory()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Microsoft SDKs\NETFXSDK\4.6.1");
            if (key == null)
            {
                key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Microsoft SDKs\NETFXSDK\4.6");
            }
            if (key == null)
            {
                return null;
            }
            string str = (string) key.GetValue("KitsInstallationFolder");
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            return str.ToNPath();
        }

        public static NPath GetWindows10SDKDirectory(out string version)
        {
            version = null;
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Microsoft SDKs\Windows\v10.0");
            if (key == null)
            {
                return null;
            }
            string str = (string) key.GetValue("InstallationFolder");
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            string str2 = (string) key.GetValue("ProductVersion");
            Version version2 = string.IsNullOrEmpty(str2) ? new Version(10, 0, 0x2800) : Version.Parse(str2);
            if (version2.Build == -1)
            {
                version2 = new Version(version2.Major, version2.Minor, 0, 0);
            }
            else if (version2.Revision == -1)
            {
                version2 = new Version(version2.Major, version2.Minor, version2.Build, 0);
            }
            version = version2.ToString();
            return str.ToNPath();
        }

        public static NPath GetWindows7SDKDirectory()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Microsoft SDKs\Windows\v7.0A");
            if (key == null)
            {
                return null;
            }
            string str = (string) key.GetValue("InstallationFolder");
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            return str.ToNPath();
        }

        public static NPath GetWindows81SDKDirectory()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Microsoft SDKs\Windows\v8.1");
            if (key == null)
            {
                return null;
            }
            string str = (string) key.GetValue("InstallationFolder");
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            return str.ToNPath();
        }
    }
}

