namespace UnityEditor.OSXStandalone
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Utils;
    using UnityEditorInternal;
    using UnityEngine;

    public class OSXStandaloneIL2CppNativeCodeBuilder : Il2CppNativeCodeBuilder
    {
        private readonly string _architecture;

        public OSXStandaloneIL2CppNativeCodeBuilder(BuildTarget target)
        {
            if (target != BuildTarget.StandaloneOSXIntel)
            {
                throw new ArgumentException("Unexpected target: " + target);
            }
            this._architecture = "x86";
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
                return "MacOSX";
            }
        }
    }
}

