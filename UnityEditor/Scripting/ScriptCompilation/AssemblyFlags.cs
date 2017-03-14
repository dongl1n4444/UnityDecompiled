namespace UnityEditor.Scripting.ScriptCompilation
{
    using System;

    [Flags]
    internal enum AssemblyFlags
    {
        EditorOnly = 1,
        None = 0,
        UseForDotNet = 4,
        UseForMono = 2
    }
}

