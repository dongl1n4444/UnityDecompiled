namespace UnityEditor.Scripting.ScriptCompilation
{
    using System;

    [Flags]
    internal enum BuildFlags
    {
        None,
        BuildingDevelopmentBuild,
        BuildingForEditor
    }
}

