namespace UnityEngine
{
    using System;
    using UnityEngine.Scripting;

    [Obsolete("Use SerializeField on the private variables that you want to be serialized instead"), RequiredByNativeCode]
    public sealed class SerializePrivateVariables : Attribute
    {
    }
}

