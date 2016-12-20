namespace UnityEditor.Modules
{
    using System;
    using System.Collections.Generic;

    internal static class ModuleUtils
    {
        internal static string[] GetAdditionalReferencesForEditorCsharpProject()
        {
            List<string> list = new List<string>();
            foreach (IPlatformSupportModule module in ModuleManager.platformSupportModules)
            {
                list.AddRange(module.AssemblyReferencesForEditorCsharpProject);
            }
            return list.ToArray();
        }

        internal static string[] GetAdditionalReferencesForUserScripts()
        {
            List<string> list = new List<string>();
            foreach (IPlatformSupportModule module in ModuleManager.platformSupportModules)
            {
                list.AddRange(module.AssemblyReferencesForUserScripts);
            }
            return list.ToArray();
        }
    }
}

