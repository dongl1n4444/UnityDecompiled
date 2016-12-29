namespace UnityEditor.WSA
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;

    internal sealed class ScriptingImplementations : IScriptingImplementations
    {
        private static ScriptingImplementation[] DotNETAndIL2CPP = new ScriptingImplementation[] { ScriptingImplementation.WinRTDotNET, ScriptingImplementation.IL2CPP };

        public ScriptingImplementation[] Enabled() => 
            DotNETAndIL2CPP;

        public ScriptingImplementation[] Supported() => 
            DotNETAndIL2CPP;
    }
}

