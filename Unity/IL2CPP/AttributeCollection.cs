namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class AttributeCollection
    {
        private readonly List<int> _attributeTypeIndices = new List<int>();
        private readonly List<AttributeTypeRange> _attributeTypeRanges = new List<AttributeTypeRange>();
        private readonly Dictionary<string, uint> _indices = new Dictionary<string, uint>();
        [CompilerGenerated]
        private static Func<KeyValuePair<string, uint>, uint> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<KeyValuePair<string, uint>, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<CustomAttribute, int> <>f__am$cache2;
        [Inject]
        public static IIl2CppTypeCollectorWriterService Il2CppTypeCollector;
        [Inject]
        public static INamingService Naming;

        public AttributeCollection()
        {
            this._indices.Add(Naming.Null, 0);
            this._attributeTypeRanges.Add(new AttributeTypeRange(0, 0));
        }

        public void Add(string name, CustomAttribute[] customAttributes)
        {
            this._indices.Add(name, (uint) this._indices.Count);
            this._attributeTypeRanges.Add(new AttributeTypeRange(this._attributeTypeIndices.Count, customAttributes.Length));
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<CustomAttribute, int>(null, (IntPtr) <Add>m__2);
            }
            this._attributeTypeIndices.AddRange(customAttributes.Select<CustomAttribute, int>(<>f__am$cache2));
        }

        public IEnumerable<int> GetAttributeTypeIndices() => 
            this._attributeTypeIndices;

        public IEnumerable<AttributeTypeRange> GetAttributeTypeRanges() => 
            this._attributeTypeRanges;

        public string[] GetEntries()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<KeyValuePair<string, uint>, uint>(null, (IntPtr) <GetEntries>m__0);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<KeyValuePair<string, uint>, string>(null, (IntPtr) <GetEntries>m__1);
            }
            return this._indices.OrderBy<KeyValuePair<string, uint>, uint>(<>f__am$cache0).Select<KeyValuePair<string, uint>, string>(<>f__am$cache1).ToArray<string>();
        }

        public uint GetIndex(AssemblyDefinition assembly) => 
            this.GetIndex(Naming.ForCustomAttributesCacheGenerator(assembly));

        public uint GetIndex(EventDefinition eventDefinition) => 
            this.GetIndex(Naming.ForCustomAttributesCacheGenerator(eventDefinition));

        public uint GetIndex(FieldDefinition fieldDefinition) => 
            this.GetIndex(Naming.ForCustomAttributesCacheGenerator(fieldDefinition));

        public uint GetIndex(MethodDefinition methodDefinition) => 
            this.GetIndex(Naming.ForCustomAttributesCacheGenerator(methodDefinition));

        public uint GetIndex(PropertyDefinition propertyDefinition) => 
            this.GetIndex(Naming.ForCustomAttributesCacheGenerator(propertyDefinition));

        public uint GetIndex(TypeDefinition type) => 
            this.GetIndex(Naming.ForCustomAttributesCacheGenerator(type));

        private uint GetIndex(string name)
        {
            uint num;
            this._indices.TryGetValue(name, out num);
            return num;
        }

        public uint GetIndex(ParameterDefinition parameterDefinition, MethodDefinition methodDefinition) => 
            this.GetIndex(Naming.ForCustomAttributesCacheGenerator(parameterDefinition, methodDefinition));

        public int Count =>
            this._indices.Count;

        [StructLayout(LayoutKind.Sequential)]
        public struct AttributeTypeRange
        {
            public readonly int Start;
            public readonly int Count;
            public AttributeTypeRange(int start, int count)
            {
                this.Start = start;
                this.Count = count;
            }
        }
    }
}

