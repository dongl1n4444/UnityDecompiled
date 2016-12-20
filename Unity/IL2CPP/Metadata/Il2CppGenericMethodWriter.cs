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

    public class Il2CppGenericMethodWriter : MetadataWriter
    {
        [CompilerGenerated]
        private static Func<KeyValuePair<MethodReference, uint>, uint> <>f__am$cache0;
        [Inject]
        public static IIl2CppGenericInstCollectorReaderService IIl2CppGenericInstCollector;
        [Inject]
        public static IIl2CppGenericMethodCollectorReaderService Il2CppGenericMethodCollector;
        public const string TableSizeVariableName = "g_Il2CppMethodSpecTableSize";
        public const string TableVariableName = "g_Il2CppMethodSpecTable";

        public Il2CppGenericMethodWriter(CppCodeWriter writer) : base(writer)
        {
        }

        private string FormatGenericMethod(MethodReference method, IMetadataCollection metadataCollection)
        {
            return string.Format("{{ {0}, {1}, {2} }}", metadataCollection.GetMethodIndex(method.Resolve()), !method.DeclaringType.IsGenericInstance ? -1 : ((int) IIl2CppGenericInstCollector.Items[((GenericInstanceType) method.DeclaringType).GenericArguments.ToArray()]), !method.IsGenericInstance ? -1 : ((int) IIl2CppGenericInstCollector.Items[((GenericInstanceMethod) method).GenericArguments.ToArray()]));
        }

        public TableInfo WriteIl2CppGenericMethodDefinitions(IMetadataCollection metadataCollection)
        {
            <WriteIl2CppGenericMethodDefinitions>c__AnonStorey0 storey = new <WriteIl2CppGenericMethodDefinitions>c__AnonStorey0 {
                metadataCollection = metadataCollection,
                $this = this
            };
            base.Writer.AddCodeGenIncludes();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<KeyValuePair<MethodReference, uint>, uint>(null, (IntPtr) <WriteIl2CppGenericMethodDefinitions>m__0);
            }
            return MetadataWriter.WriteTable<KeyValuePair<MethodReference, uint>>(base.Writer, "extern const Il2CppMethodSpec", "g_Il2CppMethodSpecTable", Enumerable.ToArray<KeyValuePair<MethodReference, uint>>(Enumerable.OrderBy<KeyValuePair<MethodReference, uint>, uint>(Il2CppGenericMethodCollector.Items, <>f__am$cache0)), new Func<KeyValuePair<MethodReference, uint>, int, string>(storey, (IntPtr) this.<>m__0));
        }

        [CompilerGenerated]
        private sealed class <WriteIl2CppGenericMethodDefinitions>c__AnonStorey0
        {
            internal Il2CppGenericMethodWriter $this;
            internal IMetadataCollection metadataCollection;

            internal string <>m__0(KeyValuePair<MethodReference, uint> item, int index)
            {
                return this.$this.FormatGenericMethod(item.Key, this.metadataCollection);
            }
        }
    }
}

