namespace Unity.IL2CPP.Metadata
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class MonoGenericInstMetadataWriter : MetadataWriter
    {
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeReference[], uint>, TypeReference[]> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<TypeReference, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeReference[], uint>, uint> <>f__am$cache2;
        [Inject]
        public static IIl2CppGenericInstCollectorReaderService Il2CppGenericInstCollector;
        public const string TableVariableName = "g_MonoGenericInstMetadataTable";
        [Inject]
        public static IIl2CppTypeCollectorWriterService TypeCollector;

        public MonoGenericInstMetadataWriter(CppCodeWriter writer) : base(writer)
        {
        }

        private string GenericInstName(IList<TypeReference> inst) => 
            ("Mono" + MetadataWriter.Naming.ForGenericInst(inst));

        private List<TypeReference[]> OrderInstances()
        {
            HashSet<TypeReference[]> set = new HashSet<TypeReference[]>(new Il2CppGenericInstCollectorComponent.Il2CppGenericInstComparer());
            List<TypeReference[]> list = new List<TypeReference[]>();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = item => item.Key;
            }
            List<TypeReference[]> list2 = Il2CppGenericInstCollector.Items.Select<KeyValuePair<TypeReference[], uint>, TypeReference[]>(<>f__am$cache0).ToList<TypeReference[]>();
            while (list2.Count > 0)
            {
                int index = 0;
                while (index < list2.Count)
                {
                    TypeReference[] referenceArray = list2[index];
                    bool flag = true;
                    foreach (TypeReference reference in referenceArray)
                    {
                        GenericInstanceType type = reference as GenericInstanceType;
                        if ((type != null) && !set.Contains(type.GenericArguments.ToArray()))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        list.Add(referenceArray);
                        list2.RemoveAt(index);
                        set.Add(referenceArray);
                    }
                    else
                    {
                        index++;
                    }
                }
            }
            return list;
        }

        public TableInfo WriteMonoMetadataForGenericInstances()
        {
            base.Writer.AddInclude("il2cpp-mapping.h");
            foreach (TypeReference[] referenceArray in this.OrderInstances())
            {
                object[] args = new object[2];
                args[0] = this.GenericInstName(referenceArray) + "_Types";
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = typeRef => TypeCollector.GetIndex(typeRef, 0).ToString();
                }
                args[1] = referenceArray.Select<TypeReference, string>(<>f__am$cache1).AggregateWithComma();
                base.WriteLine("static const TypeIndex {0}[] = {{ {1} }};", args);
                object[] objArray2 = new object[] { this.GenericInstName(referenceArray), referenceArray.Length, this.GenericInstName(referenceArray) + "_Types" };
                base.WriteLine("extern const MonoGenericInstMetadata {0} = {{ {1}, {2} }};", objArray2);
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = item => item.Value;
            }
            return MetadataWriter.WriteTable<KeyValuePair<TypeReference[], uint>>(base.Writer, "extern const MonoGenericInstMetadata* const", "g_MonoGenericInstMetadataTable", Il2CppGenericInstCollector.Items.OrderBy<KeyValuePair<TypeReference[], uint>, uint>(<>f__am$cache2).ToArray<KeyValuePair<TypeReference[], uint>>(), (Func<KeyValuePair<TypeReference[], uint>, string>) (item => ("&" + this.GenericInstName(item.Key))));
        }
    }
}

