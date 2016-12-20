namespace UnityEditor.WindowsStandalone
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Utils;
    using UnityEditorInternal;
    using UnityEngine;

    public class WindowsStandaloneIL2CppNativeCodeBuilder : Il2CppNativeCodeBuilder
    {
        private readonly string _architecture;

        public WindowsStandaloneIL2CppNativeCodeBuilder(BuildTarget target)
        {
            if (target != BuildTarget.StandaloneWindows)
            {
                if (target != BuildTarget.StandaloneWindows64)
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

        public override string CompilerArchitecture
        {
            get
            {
                return this._architecture;
            }
        }

        public override string CompilerPlatform
        {
            get
            {
                return "WindowsDesktop";
            }
        }
    }
}

