namespace UnityEditor.LinuxStandalone
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Utils;
    using UnityEditorInternal;
    using UnityEngine;

    public class LinuxStandaloneIL2CppNativeCodeBuilder : Il2CppNativeCodeBuilder
    {
        private readonly string _architecture;

        public LinuxStandaloneIL2CppNativeCodeBuilder(BuildTarget target)
        {
            if (target != BuildTarget.StandaloneLinux)
            {
                if (target != BuildTarget.StandaloneLinux64)
                {
                    throw new ArgumentException("Unexpected target: " + target);
                }
            }
            else
            {
                this._architecture = "x86";
                return;
            }
            this._architecture = "x64";
        }

        public override string CacheDirectory
        {
            get
            {
                string[] components = new string[] { Path.GetFullPath(Application.dataPath), "..", "Library" };
                return Paths.Combine(components);
            }
        }

        public override string CompilerArchitecture =>
            this._architecture;

        public override string CompilerPlatform =>
            "Linux";
    }
}

