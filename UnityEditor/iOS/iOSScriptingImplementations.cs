namespace UnityEditor.iOS
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;

    internal class iOSScriptingImplementations : IScriptingImplementations
    {
        public ScriptingImplementation[] Enabled()
        {
            ScriptingImplementation[] implementationArray1 = new ScriptingImplementation[2];
            implementationArray1[1] = ScriptingImplementation.IL2CPP;
            return implementationArray1;
        }

        public ScriptingImplementation[] Supported()
        {
            ScriptingImplementation[] implementationArray1 = new ScriptingImplementation[2];
            implementationArray1[1] = ScriptingImplementation.IL2CPP;
            return implementationArray1;
        }
    }
}

