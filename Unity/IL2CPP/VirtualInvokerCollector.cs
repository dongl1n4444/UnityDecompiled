namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using Mono.Cecil.Rocks;
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

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

        public VirtualInvokerCollector(bool processGenerics = false)
        {
            this._processGenerics = processGenerics;
        }

        private static string CallParametersFor(InvokerData data)
        {
            string[] first = new string[] { "obj" };
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = (m, i) => $"p{i + 1}";
            }
            string[] second = new string[] { "invokeData.method" };
            return first.Concat<string>(Enumerable.Range(1, data.ParameterCount).Select<int, string>(<>f__am$cache3)).Concat<string>(second).AggregateWithComma();
        }

        private static string FunctionPointerParametersFor(InvokerData data)
        {
            string[] first = new string[] { "void*" };
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = (m, i) => $"T{i + 1}";
            }
            string[] second = new string[] { "const MethodInfo*" };
            return first.Concat<string>(Enumerable.Range(1, data.ParameterCount).Select<int, string>(<>f__am$cache5)).Concat<string>(second).AggregateWithComma();
        }

        private static string InvokeParametersFor(InvokerData data, bool isGeneric)
        {
            string[] first = !isGeneric ? new string[] { "Il2CppMethodSlot slot", "Il2CppObject* obj" } : new string[] { "const MethodInfo* method", "Il2CppObject* obj" };
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = (m, i) => string.Format("T{0} p{0}", i + 1);
            }
            return first.Concat<string>(Enumerable.Range(1, data.ParameterCount).Select<int, string>(<>f__am$cache4)).AggregateWithComma();
        }

        public void Process(AssemblyDefinition assembly)
        {
            foreach (TypeDefinition definition in assembly.MainModule.GetAllTypes())
            {
                foreach (MethodDefinition definition2 in definition.Methods)
                {
                    if (definition2.IsVirtual && (this._processGenerics == definition2.HasGenericParameters))
                    {
                        this._invokerData.Add(new InvokerData(definition2.ReturnType.MetadataType == MetadataType.Void, definition2.Parameters.Count));
                    }
                }
            }
        }

        private static string ReturnTypeFor(InvokerData data) => 
            (!data.VoidReturn ? "R" : "void");

        private static string TemplateParametersFor(InvokerData data)
        {
            if (data.VoidReturn && (data.ParameterCount == 0))
            {
                return string.Empty;
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = i => "T" + i;
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = t => "typename " + t;
            }
            return (!data.VoidReturn ? ((IEnumerable<string>) new string[] { "R" }) : Enumerable.Empty<string>()).Concat<string>(Enumerable.Range(1, data.ParameterCount).Select<int, string>(<>f__am$cache1)).Select<string, string>(<>f__am$cache2).AggregateWithComma();
        }

        public void Write(NPath path)
        {
            using (SourceCodeWriter writer = new SourceCodeWriter(path))
            {
                writer.WriteLine("#pragma once");
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = m => (m.ParameterCount * 10) + (!m.VoidReturn ? 1 : 0);
                }
                foreach (InvokerData data in this._invokerData.OrderBy<InvokerData, int>(<>f__am$cache0))
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
                    if (this._processGenerics)
                    {
                        writer.WriteLine("VirtualInvokeData invokeData;");
                        writer.WriteLine("il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);");
                    }
                    else
                    {
                        writer.WriteLine("const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);");
                    }
                    object[] objArray6 = new object[] { !data.VoidReturn ? "return " : "", str, CallParametersFor(data) };
                    writer.WriteLine("{0}(({1})invokeData.methodPtr)({2});", objArray6);
                    writer.EndBlock(false);
                    writer.EndBlock(true);
                }
            }
        }
    }
}

