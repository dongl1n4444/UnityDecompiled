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

    public class Il2CppGenericInstWriter : MetadataWriter
    {
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeReference[], uint>, TypeReference[]> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<TypeReference, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeReference[], uint>, uint> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeReference[], uint>, string> <>f__am$cache3;
        [Inject]
        public static IIl2CppGenericInstCollectorReaderService Il2CppGenericInstCollector;
        public const string TableVariableName = "g_Il2CppGenericInstTable";
        [Inject]
        public static IIl2CppTypeCollectorWriterService TypeCollector;

        public Il2CppGenericInstWriter(CppCodeWriter writer) : base(writer)
        {
        }

        public TableInfo WriteIl2CppGenericInstDefinitions()
        {
            base.Writer.AddCodeGenIncludes();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = item => item.Key;
            }
            foreach (TypeReference[] referenceArray in Il2CppGenericInstCollector.Items.Select<KeyValuePair<TypeReference[], uint>, TypeReference[]>(<>f__am$cache0))
            {
                for (int i = 0; i < referenceArray.Length; i++)
                {
                    base.Writer.WriteExternForIl2CppType(referenceArray[i]);
                }
                object[] args = new object[2];
                args[0] = MetadataWriter.Naming.ForGenericInst(referenceArray) + "_Types";
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = t => MetadataWriter.TypeRepositoryTypeFor(t, 0);
                }
                args[1] = referenceArray.Select<TypeReference, string>(<>f__am$cache1).AggregateWithComma();
                base.WriteLine("static const Il2CppType* {0}[] = {{ {1} }};", args);
                object[] objArray2 = new object[] { MetadataWriter.Naming.ForGenericInst(referenceArray), referenceArray.Length, MetadataWriter.Naming.ForGenericInst(referenceArray) + "_Types" };
                base.WriteLine("extern const Il2CppGenericInst {0} = {{ {1}, {2} }};", objArray2);
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = item => item.Value;
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = item => "&" + MetadataWriter.Naming.ForGenericInst(item.Key);
            }
            return MetadataWriter.WriteTable<KeyValuePair<TypeReference[], uint>>(base.Writer, "extern const Il2CppGenericInst* const", "g_Il2CppGenericInstTable", Il2CppGenericInstCollector.Items.OrderBy<KeyValuePair<TypeReference[], uint>, uint>(<>f__am$cache2).ToArray<KeyValuePair<TypeReference[], uint>>(), <>f__am$cache3);
        }
    }
}

