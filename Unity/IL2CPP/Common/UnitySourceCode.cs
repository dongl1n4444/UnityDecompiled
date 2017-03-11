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
                    _unityRoot = CommonPaths.Il2CppRoot?.Parent.ParentContaining("build.pl");
                }
            }

            public static NPath UnityRoot =>
                _unityRoot;
        }
    }
}

