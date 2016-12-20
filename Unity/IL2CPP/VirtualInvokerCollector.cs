namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.Cecil.Visitor;

    internal class VirtualInvokerCollector
    {
        private readonly HashSet<InvokerData> _invokerData = new HashSet<InvokerData>();
        private readonly bool _processGenerics;
        [CompilerGenerated]
        private static Func<InvokerData, int> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<int, int, string> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<int, int, string> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<int, int, string> <>f__am$cache5;

        public VirtualInvokerCollector([Optional, DefaultParameterValue(false)] bool processGenerics)
        {
            this._processGenerics = processGenerics;
        }

        private static string CallParametersFor(InvokerData data)
        {
            string[] first = new string[] { "obj" };
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Func<int, int, string>(null, (IntPtr) <CallParametersFor>m__3);
            }
            string[] second = new string[] { "invokeData.method" };
            return EnumerableExtensions.AggregateWithComma(Enumerable.Concat<string>(Enumerable.Concat<string>(first, Enumerable.Select<int, string>(Enumerable.Range(1, data.ParameterCount), <>f__am$cache3)), second));
        }

        private static string FunctionPointerParametersFor(InvokerData data)
        {
            string[] first = new string[] { "void*" };
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = new Func<int, int, string>(null, (IntPtr) <FunctionPointerParametersFor>m__5);
            }
            string[] second = new string[] { "const MethodInfo*" };
            return EnumerableExtensions.AggregateWithComma(Enumerable.Concat<string>(Enumerable.Concat<string>(first, Enumerable.Select<int, string>(Enumerable.Range(1, data.ParameterCount), <>f__am$cache5)), second));
        }

        private static string InvokeParametersFor(InvokerData data, bool isGeneric)
        {
            string[] first = !isGeneric ? new string[] { "Il2CppMethodSlot slot", "void* obj" } : new string[] { "const MethodInfo* method", "void* obj" };
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = new Func<int, int, string>(null, (IntPtr) <InvokeParametersFor>m__4);
            }
            return EnumerableExtensions.AggregateWithComma(Enumerable.Concat<string>(first, Enumerable.Select<int, string>(Enumerable.Range(1, data.ParameterCount), <>f__am$cache4)));
        }

        public void Process(AssemblyDefinition assembly)
        {
            InvokerVisitor visitor = new InvokerVisitor(this._invokerData, this._processGenerics);
            Unity.Cecil.Visitor.Extensions.Accept(assembly, visitor);
        }

        private static string ReturnTypeFor(InvokerData data)
        {
            return (!data.VoidReturn ? "R" : "void");
        }

        private static string TemplateParametersFor(InvokerData data)
        {
            if (data.VoidReturn && (data.ParameterCount == 0))
            {
                return string.Empty;
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<int, string>(null, (IntPtr) <TemplateParametersFor>m__1);
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<string, string>(null, (IntPtr) <TemplateParametersFor>m__2);
            }
            return EnumerableExtensions.AggregateWithComma(Enumerable.Select<string, string>(Enumerable.Concat<string>(!data.VoidReturn ? ((IEnumerable<string>) new string[] { "R" }) : Enumerable.Empty<string>(), Enumerable.Select<int, string>(Enumerable.Range(1, data.ParameterCount), <>f__am$cache1)), <>f__am$cache2));
        }

        public void Write(NPath path)
        {
            using (SourceCodeWriter writer = new SourceCodeWriter(path))
            {
                writer.WriteLine("#pragma once");
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<InvokerData, int>(null, (IntPtr) <Write>m__0);
                }
                foreach (InvokerData data in Enumerable.OrderBy<InvokerData, int>(this._invokerData, <>f__am$cache0))
                {
                    string str = !data.VoidReturn ? "Func" : "Action";
                    string str2 = TemplateParametersFor(data);
                    if (!string.IsNullOrEmpty(str2))
                    {
                        object[] objArray1 = new object[] { str2 };
                        writer.WriteLine("template <{0}>", objArray1);
                    }
                    if (this._processGenerics)
                    {
                        object[] objArray2 = new object[] { str, data.ParameterCount };
                        writer.WriteLine("struct GenericVirt{0}Invoker{1}", objArray2);
                    }
                    else
                    {
                        object[] objArray3 = new object[] { str, data.ParameterCount };
                        writer.WriteLine("struct Virt{0}Invoker{1}", objArray3);
                    }
                    writer.BeginBlock();
                    object[] args = new object[] { ReturnTypeFor(data), str, FunctionPointerParametersFor(data) };
                    writer.WriteLine("typedef {0} (*{1})({2});", args);
                    writer.WriteLine();
                    object[] objArray5 = new object[] { ReturnTypeFor(data), InvokeParametersFor(data, this._processGenerics) };
                    writer.WriteLine("static inline {0} Invoke ({1})", objArray5);
                    writer.BeginBlock();
                    writer.WriteLine("VirtualInvokeData invokeData;");
                    if (this._processGenerics)
                    {
                        writer.WriteLine("il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);");
                    }
                    else
                    {
                        writer.WriteLine("il2cpp_codegen_get_virtual_invoke_data (slot, obj, &invokeData);");
                    }
                    object[] objArray6 = new object[] { !data.VoidReturn ? "return " : "", str, CallParametersFor(data) };
                    writer.WriteLine("{0}(({1})invokeData.methodPtr)({2});", objArray6);
                    writer.EndBlock(false);
                    writer.EndBlock(true);
                }
            }
        }

        private class InvokerVisitor : Unity.Cecil.Visitor.Visitor
        {
            private readonly HashSet<InvokerData> _collection;
            private readonly bool _processGenerics;

            public InvokerVisitor(HashSet<InvokerData> collection, bool processGenerics)
            {
                this._collection = collection;
                this._processGenerics = processGenerics;
            }

            protected override void Visit(MethodDefinition methodDefinition, Unity.Cecil.Visitor.Context context)
            {
                if (methodDefinition.IsVirtual && (this._processGenerics == methodDefinition.HasGenericParameters))
                {
                    this._collection.Add(new InvokerData(methodDefinition.ReturnType.MetadataType == MetadataType.Void, methodDefinition.Parameters.Count, false));
                }
            }
        }
    }
}

