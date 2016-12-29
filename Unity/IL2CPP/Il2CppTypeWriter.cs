namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Metadata;

    public class Il2CppTypeWriter : MetadataWriter
    {
        [CompilerGenerated]
        private static Func<Il2CppTypeData, TypeReference> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<KeyValuePair<Il2CppTypeData, int>, int> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<KeyValuePair<Il2CppTypeData, int>, string> <>f__am$cache2;
        [Inject]
        public static IIl2CppTypeCollectorReaderService Il2CppTypeCollectorReader;

        public Il2CppTypeWriter(CppCodeWriter writer) : base(writer)
        {
        }

        private string WriteArrayDataValue(ArrayType arrayType)
        {
            base.Writer.WriteExternForIl2CppType(arrayType.ElementType);
            if (arrayType.Rank == 1)
            {
                return ("(void*)&" + MetadataWriter.Naming.ForIl2CppType(arrayType.ElementType, 0));
            }
            object[] args = new object[] { MetadataWriter.Naming.ForArrayType(arrayType) };
            base.WriteLine("Il2CppArrayType {0} = ", args);
            string[] initializers = new string[] { $"&{MetadataWriter.Naming.ForIl2CppType(arrayType.ElementType, 0)}", arrayType.Rank.ToString(), 0.ToString(), 0.ToString(), MetadataWriter.Naming.Null, MetadataWriter.Naming.Null };
            base.WriteArrayInitializer(initializers, MetadataWriter.ArrayTerminator.None);
            return ("&" + MetadataWriter.Naming.ForArrayType(arrayType));
        }

        private string WriteGenericInstanceTypeDataValue(GenericInstanceType genericInstanceType, IMetadataCollection metadataCollection)
        {
            new GenericClassWriter(base.Writer).WriteDefinition(genericInstanceType, metadataCollection);
            return ("&" + MetadataWriter.Naming.ForGenericClass(genericInstanceType));
        }

        public TableInfo WriteIl2CppTypeDefinitions(IMetadataCollection metadataCollection)
        {
            base.Writer.AddCodeGenIncludes();
            IDictionary<Il2CppTypeData, int> items = Il2CppTypeCollectorReader.Items;
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<Il2CppTypeData, TypeReference>(null, (IntPtr) <WriteIl2CppTypeDefinitions>m__0);
            }
            foreach (IGrouping<TypeReference, Il2CppTypeData> grouping in items.Keys.GroupBy<Il2CppTypeData, TypeReference>(<>f__am$cache0, new Unity.IL2CPP.Common.TypeReferenceEqualityComparer()))
            {
                string str;
                base.Writer.WriteLine();
                TypeReference key = grouping.Key;
                GenericParameter genericParameter = key as GenericParameter;
                GenericInstanceType genericInstanceType = key as GenericInstanceType;
                ArrayType arrayType = key as ArrayType;
                PointerType pointerType = key as PointerType;
                if (genericParameter != null)
                {
                    str = "(void*)" + metadataCollection.GetGenericParameterIndex(genericParameter);
                }
                else if (genericInstanceType != null)
                {
                    str = this.WriteGenericInstanceTypeDataValue(genericInstanceType, metadataCollection);
                }
                else if (arrayType != null)
                {
                    str = this.WriteArrayDataValue(arrayType);
                }
                else if (pointerType != null)
                {
                    str = this.WritePointerDataValue(pointerType);
                }
                else
                {
                    str = "(void*)" + metadataCollection.GetTypeInfoIndex(key.Resolve()).ToString(CultureInfo.InvariantCulture);
                }
                foreach (Il2CppTypeData data in grouping)
                {
                    object[] args = new object[] { MetadataWriter.Naming.ForIl2CppType(data.Type, data.Attrs), str, data.Attrs.ToString(CultureInfo.InvariantCulture), Il2CppTypeSupport.For(data.Type), "0", !data.Type.IsByReference ? "0" : "1", !data.Type.IsPinned ? "0" : "1" };
                    base.Writer.WriteLine("extern const Il2CppType {0} = {{ {1}, {2}, {3}, {4}, {5}, {6} }};", args);
                }
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<KeyValuePair<Il2CppTypeData, int>, int>(null, (IntPtr) <WriteIl2CppTypeDefinitions>m__1);
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<KeyValuePair<Il2CppTypeData, int>, string>(null, (IntPtr) <WriteIl2CppTypeDefinitions>m__2);
            }
            return MetadataWriter.WriteTable<KeyValuePair<Il2CppTypeData, int>>(base.Writer, "extern const Il2CppType* const ", "g_Il2CppTypeTable", items.OrderBy<KeyValuePair<Il2CppTypeData, int>, int>(<>f__am$cache1).ToArray<KeyValuePair<Il2CppTypeData, int>>(), <>f__am$cache2);
        }

        private string WritePointerDataValue(PointerType pointerType)
        {
            base.Writer.WriteExternForIl2CppType(pointerType.ElementType);
            return ("(void*)&" + MetadataWriter.Naming.ForIl2CppType(pointerType.ElementType, 0));
        }
    }
}

