namespace UnityScript.Parser
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    [CompilerGlobalScope, Extension]
    public sealed class CodeFactoryModule
    {
        private CodeFactoryModule()
        {
        }

        public static string ModuleNameFromFileName(string fname)
        {
            return Path.GetFileNameWithoutExtension(fname);
        }
    }
}

