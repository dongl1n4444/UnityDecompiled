namespace UnityEditor.Scripting.ScriptCompilation
{
    using System;

    [Flags]
    internal enum EditorScriptCompilationOptions
    {
        BuildingDevelopmentBuild = 1,
        BuildingEditorOnlyAssembly = 4,
        BuildingEmpty = 0,
        BuildingForEditor = 2,
        BuildingForIl2Cpp = 8,
        BuildingWithAsserts = 0x10
    }
}

