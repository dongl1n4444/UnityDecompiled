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
        private static bool _runningOnNetCore;
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
            if (!_runningOnIl2Cpp && !_runningWithMono)
            {
                foreach (Assembly assembly in AppDomainPortable.GetAllAssembliesInCurrentAppDomainPortable())
                {
                    if (assembly.FullName.StartsWith("Microsoft.Extensions.PlatformAbstractions"))
                    {
                        object obj2 = assembly.CreateInstance("Microsoft.Extensions.PlatformAbstractions.ApplicationEnvironment");
                        if ((obj2 != null) && obj2.GetType().GetMethod("get_RuntimeFramework").Invoke(obj2, null).ToString().StartsWith(".NETCoreApp"))
                        {
                            _runningOnNetCore = true;
                            break;
                        }
                    }
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

        public static bool RunningOnNetCore() => 
            _runningOnNetCore;

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

