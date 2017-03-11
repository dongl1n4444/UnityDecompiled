namespace UnityEngine.Collections
{
    using System;
    using Unity.Bindings;
    using UnityEngine.Scripting;

    [NativeEnum(Name="NativeCollection::Allocator"), UsedByNativeCode]
    public enum Allocator
    {
        Invalid,
        None,
        Temp,
        TempJob,
        Persistent
    }
}

