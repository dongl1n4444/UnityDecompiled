namespace Unity.IL2CPP.Common
{
    using NiceIO;
    using System;

    public static class UnitySourceCode
    {
        public static bool Available =>
            (Paths.UnityRoot != null);

        public static class Paths
        {
            private static readonly NPath _unityRoot;

            static Paths()
            {
                string environmentVariable = Environment.GetEnvironmentVariable("IL2CPP_UNITY_ROOT");
                if (!string.IsNullOrEmpty(environmentVariable))
                {
                    _unityRoot = environmentVariable.ToNPath();
                }
                else
                {
                    _unityRoot = CommonPaths.Il2CppRoot?.ParentContaining("build.pl");
                }
            }

            private static NPath EditorToolsPath
            {
                get
                {
                    if (PlatformUtils.IsWindows())
                    {
                        string[] textArray1 = new string[] { "build/WindowsEditor/Data/Tools" };
                        return UnityRoot.Combine(textArray1);
                    }
                    if (PlatformUtils.IsLinux())
                    {
                        string[] textArray2 = new string[] { "build/LinuxEditor/Data/Tools/" };
                        return UnityRoot.Combine(textArray2);
                    }
                    string[] append = new string[] { "build/MacEditor/Unity.app/Contents/Tools" };
                    return UnityRoot.Combine(append);
                }
            }

            public static NPath UnityRoot =>
                _unityRoot;

            public static NPath UnusedBytecodeStripper
            {
                get
                {
                    string[] append = new string[] { "UnusedByteCodeStripper2/UnusedBytecodeStripper2.exe" };
                    return EditorToolsPath.Combine(append);
                }
            }
        }
    }
}

