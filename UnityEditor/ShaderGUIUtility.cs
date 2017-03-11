namespace UnityEditor
{
    using System;
    using System.Reflection;

    internal static class ShaderGUIUtility
    {
        internal static ShaderGUI CreateShaderGUI(string customEditorName) => 
            CreateShaderGUI(ExtractCustomEditorType(customEditorName));

        internal static ShaderGUI CreateShaderGUI(System.Type customEditorType) => 
            ((customEditorType == null) ? null : (Activator.CreateInstance(customEditorType) as ShaderGUI));

        internal static System.Type ExtractCustomEditorType(string customEditorName)
        {
            if (!string.IsNullOrEmpty(customEditorName))
            {
                string str = "UnityEditor." + customEditorName;
                Assembly[] loadedAssemblies = EditorAssemblies.loadedAssemblies;
                for (int i = loadedAssemblies.Length - 1; i >= 0; i--)
                {
                    foreach (System.Type type2 in AssemblyHelper.GetTypesFromAssembly(loadedAssemblies[i]))
                    {
                        if (type2.FullName.Equals(customEditorName, StringComparison.Ordinal) || type2.FullName.Equals(str, StringComparison.Ordinal))
                        {
                            return (!typeof(ShaderGUI).IsAssignableFrom(type2) ? null : type2);
                        }
                    }
                }
            }
            return null;
        }
    }
}

