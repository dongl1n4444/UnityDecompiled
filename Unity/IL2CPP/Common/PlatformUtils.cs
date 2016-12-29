namespace Unity.IL2CPP.Common
{
    using System;
    using System.IO;
    using System.Reflection;
    using Unity.IL2CPP.Portability;

    public static class PlatformUtils
    {
        private static Architecture _nativeCompilerArchitecture;
        private static bool _runningOnIl2Cpp;
        private static bool _runningWithMono;

        static PlatformUtils()
        {
            Type type = Type.GetType("Mono.Runtime");
            if (type != null)
            {
                string str = (string) type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
                if (str.Contains("IL2CPP"))
                {
                    _runningOnIl2Cpp = true;
                }
                else
                {
                    _runningWithMono = true;
                }
            }
        }

        public static bool IsLinux() => 
            (!IsWindows() && Directory.Exists("/proc"));

        public static bool IsOSX() => 
            (!IsWindows() && !IsLinux());

        public static bool IsWindows() => 
            PortabilityUtilities.IsWindows();

        public static bool RunningOnIl2Cpp() => 
            _runningOnIl2Cpp;

        public static bool RunningWithMono() => 
            _runningWithMono;

        public static Architecture ManagedRuntimeArchitecture =>
            ((IntPtr.Size != 4) ? Architecture.x64 : Architecture.x86);

        public static Architecture NativeCompilerArchitecture
        {
            get => 
                ((_nativeCompilerArchitecture != Architecture.UseManagedRuntimeArchitecture) ? _nativeCompilerArchitecture : ManagedRuntimeArchitecture);
            set
            {
                _nativeCompilerArchitecture = value;
            }
        }

        public enum Architecture
        {
            UseManagedRuntimeArchitecture,
            x86,
            x64
        }
    }
}

