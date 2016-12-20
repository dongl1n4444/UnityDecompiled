namespace UnityEngine
{
    using System;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Prefer ScriptableObject derived type to use binary serialization regardless of project's asset serialization mode.</para>
    /// </summary>
    [RequiredByNativeCode, AttributeUsage(AttributeTargets.Class)]
    public sealed class PreferBinarySerialization : Attribute
    {
    }
}

