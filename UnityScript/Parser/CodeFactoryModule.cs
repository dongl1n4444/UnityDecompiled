namespace UnityScript.Parser
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    [CompilerGlobalScope]
    public sealed class CodeFactoryModule
    {
        private CodeFactoryModule()
        {
        }

        public static string ModuleNameFromFileName(string fname) => 
            Path.GetFileNameWithoutExtension(fname);
    }
}

