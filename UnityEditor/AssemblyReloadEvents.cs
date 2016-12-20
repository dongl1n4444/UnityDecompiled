namespace UnityEditor
{
    using System;
    using UnityEditorInternal;

    internal class AssemblyReloadEvents
    {
        public static void OnAfterAssemblyReload()
        {
            foreach (ProjectBrowser browser in ProjectBrowser.GetAllProjectBrowsers())
            {
                browser.Repaint();
            }
        }

        public static void OnBeforeAssemblyReload()
        {
            InternalEditorUtility.AuxWindowManager_OnAssemblyReload();
        }
    }
}

