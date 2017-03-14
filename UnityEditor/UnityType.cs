namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    internal sealed class UnityType
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private UnityType <baseClass>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private UnityTypeFlags <flags>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <name>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <nativeNamespace>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <persistentTypeID>k__BackingField;
        private uint descendantCount;
        private static Dictionary<int, UnityType> ms_idToTypeInfo;
        private static Dictionary<string, UnityType> ms_nameToTypeInfo;
        private static UnityType[] ms_types;
        private static ReadOnlyCollection<UnityType> ms_typesReadOnly;
        private uint runtimeTypeIndex;

        static UnityType()
        {
            UnityTypeTransport[] transportArray = Internal_GetAllTypes();
            ms_types = new UnityType[transportArray.Length];
            ms_idToTypeInfo = new Dictionary<int, UnityType>();
            ms_nameToTypeInfo = new Dictionary<string, UnityType>();
            for (int i = 0; i < transportArray.Length; i++)
            {
                UnityType type = null;
                if (transportArray[i].baseClassIndex < transportArray.Length)
                {
                    type = ms_types[transportArray[i].baseClassIndex];
                }
                UnityType type2 = new UnityType {
                    runtimeTypeIndex = transportArray[i].runtimeTypeIndex,
                    descendantCount = transportArray[i].descendantCount,
                    name = transportArray[i].className,
                    nativeNamespace = transportArray[i].classNamespace,
                    persistentTypeID = transportArray[i].persistentTypeID,
                    baseClass = type,
                    flags = (UnityTypeFlags) transportArray[i].flags
                };
                ms_types[i] = type2;
                ms_typesReadOnly = new ReadOnlyCollection<UnityType>(ms_types);
                ms_idToTypeInfo[type2.persistentTypeID] = type2;
                ms_nameToTypeInfo[type2.name] = type2;
            }
        }

        public static UnityType FindTypeByName(string name)
        {
            UnityType type = null;
            ms_nameToTypeInfo.TryGetValue(name, out type);
            return type;
        }

        public static UnityType FindTypeByNameCaseInsensitive(string name)
        {
            <FindTypeByNameCaseInsensitive>c__AnonStorey0 storey = new <FindTypeByNameCaseInsensitive>c__AnonStorey0 {
                name = name
            };
            return Enumerable.FirstOrDefault<UnityType>(ms_types, new Func<UnityType, bool>(storey.<>m__0));
        }

        public static UnityType FindTypeByPersistentTypeID(int id)
        {
            UnityType type = null;
            ms_idToTypeInfo.TryGetValue(id, out type);
            return type;
        }

        public static ReadOnlyCollection<UnityType> GetTypes() => 
            ms_typesReadOnly;

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern UnityTypeTransport[] Internal_GetAllTypes();
        public bool IsDerivedFrom(UnityType baseClass) => 
            ((this.runtimeTypeIndex - baseClass.runtimeTypeIndex) < baseClass.descendantCount);

        public UnityType baseClass { get; private set; }

        public UnityTypeFlags flags { get; private set; }

        public bool hasNativeNamespace =>
            (this.nativeNamespace.Length > 0);

        public bool isAbstract =>
            ((this.flags & UnityTypeFlags.Abstract) != 0);

        public bool isDeprecated =>
            ((this.flags & UnityTypeFlags.Deprecated) != 0);

        public bool isEditorOnly =>
            ((this.flags & UnityTypeFlags.EditorOnly) != 0);

        public bool isSealed =>
            ((this.flags & UnityTypeFlags.Sealed) != 0);

        public string name { get; private set; }

        public string nativeNamespace { get; private set; }

        public int persistentTypeID { get; private set; }

        public string qualifiedName =>
            (!this.hasNativeNamespace ? this.name : (this.nativeNamespace + "::" + this.name));

        [CompilerGenerated]
        private sealed class <FindTypeByNameCaseInsensitive>c__AnonStorey0
        {
            internal string name;

            internal bool <>m__0(UnityType t) => 
                string.Equals(this.name, t.name, StringComparison.OrdinalIgnoreCase);
        }

        [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
        private struct UnityTypeTransport
        {
            public uint runtimeTypeIndex;
            public uint descendantCount;
            public uint baseClassIndex;
            public string className;
            public string classNamespace;
            public int persistentTypeID;
            public uint flags;
        }
    }
}

