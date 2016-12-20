namespace Unity.IL2CPP.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class TypeNameParseInfo
    {
        [CompilerGenerated]
        private static Func<string, string, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<int, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<int, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<int, bool> <>f__am$cache3;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<int> <Arities>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private AssemblyNameParseInfo <Assembly>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<int> <Modifiers>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Name>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Namespace>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<string> <Nested>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<TypeNameParseInfo> <TypeArguments>k__BackingField;
        public const int BoundedModifierMarker = -2;
        public const int ByRefModifierMarker = 0;
        public const int PointerModifierMarker = -1;

        public TypeNameParseInfo()
        {
            this.Modifiers = new List<int>();
            this.Nested = new List<string>();
            this.Arities = new List<int>();
            this.Assembly = new AssemblyNameParseInfo();
            this.TypeArguments = new List<TypeNameParseInfo>();
        }

        public List<int> Arities { get; internal set; }

        public int ArrayDimension
        {
            get
            {
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = new Func<int, bool>(null, (IntPtr) <get_ArrayDimension>m__2);
                }
                return Enumerable.Count<int>(this.Modifiers, <>f__am$cache2);
            }
        }

        public AssemblyNameParseInfo Assembly { get; internal set; }

        public string ElementTypeName
        {
            get
            {
                string name = this.Name;
                if (!string.IsNullOrEmpty(this.Namespace))
                {
                    name = this.Namespace + "." + this.Name;
                }
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<string, string, string>(null, (IntPtr) <get_ElementTypeName>m__0);
                }
                return Enumerable.Aggregate<string, string>(this.Nested, name, <>f__am$cache0);
            }
        }

        public bool HasGenericArguments
        {
            get
            {
                return (this.TypeArguments.Count > 0);
            }
        }

        public bool IsArray
        {
            get
            {
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = new Func<int, bool>(null, (IntPtr) <get_IsArray>m__3);
                }
                return Enumerable.Any<int>(this.Modifiers, <>f__am$cache3);
            }
        }

        public bool IsBounded
        {
            get
            {
                return (this.Modifiers.IndexOf(-2) >= 0);
            }
        }

        public bool IsByRef
        {
            get
            {
                return (this.Modifiers.IndexOf(0) >= 0);
            }
        }

        public bool IsNested
        {
            get
            {
                return (this.Nested.Count > 0);
            }
        }

        public bool IsPointer
        {
            get
            {
                return (this.Modifiers.IndexOf(-1) >= 0);
            }
        }

        public List<int> Modifiers { get; internal set; }

        public string Name { get; internal set; }

        public string Namespace { get; internal set; }

        public List<string> Nested { get; internal set; }

        public int[] Ranks
        {
            get
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = new Func<int, bool>(null, (IntPtr) <get_Ranks>m__1);
                }
                return Enumerable.ToArray<int>(Enumerable.Where<int>(this.Modifiers, <>f__am$cache1));
            }
        }

        public List<TypeNameParseInfo> TypeArguments { get; internal set; }
    }
}

