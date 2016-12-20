namespace UnityEngine
{
    using System;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Assembly level attribute. Any classes in an assembly with this attribute will be considered to be Editor Classes.</para>
    /// </summary>
    [RequiredByNativeCode, AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyIsEditorAssembly : Attribute
    {
    }
}

