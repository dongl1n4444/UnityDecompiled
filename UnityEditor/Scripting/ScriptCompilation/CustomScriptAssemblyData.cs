namespace UnityEditor.Scripting.ScriptCompilation
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class CustomScriptAssemblyData
    {
        public string[] excludePlatforms;
        public string[] includePlatforms;
        public string language;
        public string name;
        public string[] references;

        public static CustomScriptAssemblyData FromJson(string json)
        {
            CustomScriptAssemblyData data = JsonUtility.FromJson<CustomScriptAssemblyData>(json);
            if (data == null)
            {
                throw new Exception("Json file does not contain an assembly definition");
            }
            if (string.IsNullOrEmpty(data.name))
            {
                throw new Exception("Required property 'name' not set");
            }
            if (((data.excludePlatforms != null) && (data.excludePlatforms.Length > 0)) && ((data.includePlatforms != null) && (data.includePlatforms.Length > 0)))
            {
                throw new Exception("Both 'excludePlatforms' and 'includePlatforms' are set.");
            }
            if (string.IsNullOrEmpty(data.language))
            {
                data.language = "CSharp";
            }
            return data;
        }
    }
}

