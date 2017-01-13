namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using NiceIO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Portability;

    public class VirtualCallCollector : IVirtualCallCollectorService, IDisposable
    {
        private readonly Dictionary<TypeReference[], int> _signatures = new Dictionary<TypeReference[], int>(new RuntimeInvokerComparer());
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeReference[], int>, int> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeReference[], int>, TypeReference[]> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeReference[], int>, IEnumerable<TypeReference>> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<TypeReference, int> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeReference[], int>, string> <>f__mg$cache0;
        [Inject]
        public static IGenericSharingAnalysisService GenericSharingAnalysis;
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static IIl2CppTypeCollectorWriterService TypeCollector;
        [Inject]
        public static ITypeProviderService TypeProvider;

        public void AddMethod(MethodReference method)
        {
            object obj2 = this._signatures;
            lock (obj2)
            {
                TypeReference[] key = MethodToSignature(method);
                if (!this._signatures.ContainsKey(key))
                {
                    this._signatures.Add(key, this._signatures.Count);
                }
            }
        }

        public void Dispose()
        {
            this._signatures.Clear();
        }

        private static string FormatParameterName(TypeReference parameterType, string parameterName) => 
            ((string.Empty + Naming.ForVariable(parameterType)) + " " + parameterName);

        private static string FormatParameters(IEnumerable<TypeReference> inputParams) => 
            ParametersFor(inputParams).ToList<string>().AggregateWithComma();

        private static string GetMethodSignature(CppCodeWriter writer, TypeReference[] signature, int index)
        {
            RecordIncludes(writer, signature);
            return MethodSignatureWriter.GetMethodSignature($"UnresolvedVirtualCall_{index}", Naming.ForVariable(signature[0]), FormatParameters(signature.Skip<TypeReference>(1)), "static", "");
        }

        private static string MethodNameFor(KeyValuePair<TypeReference[], int> kvp) => 
            $"(const Il2CppMethodPointer) UnresolvedVirtualCall_{kvp.Value}";

        private static TypeReference[] MethodToSignature(MethodReference method)
        {
            if (GenericSharingAnalysis.CanShareMethod(method))
            {
                method = GenericSharingAnalysis.GetSharedMethod(method);
            }
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = new Unity.IL2CPP.ILPreProcessor.TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);
            TypeReference[] referenceArray = new TypeReference[method.Parameters.Count + 1];
            referenceArray[0] = TypeFor(resolver.ResolveReturnType(method));
            for (int i = 0; i < method.Parameters.Count; i++)
            {
                referenceArray[i + 1] = TypeFor(resolver.ResolveParameterType(method, method.Parameters[i]));
            }
            return referenceArray;
        }

        [DebuggerHidden]
        private static IEnumerable<string> ParametersFor(IEnumerable<TypeReference> inputParams) => 
            new <ParametersFor>c__Iterator0 { 
                inputParams = inputParams,
                $PC = -2
            };

        private static void RecordIncludes(CppCodeWriter writer, TypeReference[] signature)
        {
            if (signature[0].MetadataType != MetadataType.Void)
            {
                writer.AddIncludesForTypeReference(signature[0], false);
            }
            foreach (TypeReference reference in signature.Skip<TypeReference>(1))
            {
                writer.AddIncludesForTypeReference(reference, true);
            }
        }

        private static TypeReference TypeFor(TypeReference type)
        {
            if (type.IsByReference || !type.IsValueType())
            {
                return type.Module.TypeSystem.Object;
            }
            if (type.MetadataType == MetadataType.Boolean)
            {
                return type.Module.TypeSystem.SByte;
            }
            if (type.MetadataType == MetadataType.Char)
            {
                return type.Module.TypeSystem.Int16;
            }
            if (type.IsEnum())
            {
                return type.GetUnderlyingEnumType();
            }
            return type;
        }

        public UnresolvedVirtualsTablesInfo WriteUnresolvedStubs(NPath outputDir)
        {
            string[] append = new string[] { "UnresolvedVirtualCallStubs" };
            using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append).ChangeExtension("cpp")))
            {
                UnresolvedVirtualsTablesInfo info;
                writer.AddCodeGenIncludes();
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = item => item.Value;
                }
                KeyValuePair<TypeReference[], int>[] source = this._signatures.OrderBy<KeyValuePair<TypeReference[], int>, int>(<>f__am$cache0).ToArray<KeyValuePair<TypeReference[], int>>();
                foreach (KeyValuePair<TypeReference[], int> pair in source)
                {
                    writer.WriteLine(GetMethodSignature(writer, pair.Key, pair.Value));
                    writer.WriteLine("{");
                    writer.Indent(1);
                    writer.WriteLine("il2cpp_codegen_raise_execution_engine_exception(method);");
                    writer.WriteLine("il2cpp_codegen_no_return();");
                    writer.Dedent(1);
                    writer.WriteLine("}");
                    writer.WriteLine();
                }
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new Func<KeyValuePair<TypeReference[], int>, string>(VirtualCallCollector.MethodNameFor);
                }
                info.MethodPointersInfo = writer.WriteArrayInitializer("extern const Il2CppMethodPointer", "g_UnresolvedVirtualMethodPointers", source.Select<KeyValuePair<TypeReference[], int>, string>(<>f__mg$cache0), false);
                List<Unity.IL2CPP.IoCServices.Range> list = new List<Unity.IL2CPP.IoCServices.Range>();
                int num2 = 0;
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = item => item.Key;
                }
                foreach (TypeReference[] referenceArray in source.Select<KeyValuePair<TypeReference[], int>, TypeReference[]>(<>f__am$cache1))
                {
                    int length = referenceArray.Length;
                    list.Add(new Unity.IL2CPP.IoCServices.Range(num2, length));
                    num2 += length;
                }
                info.SignatureRangesInfo = list.AsReadOnly();
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = item => item.Key;
                }
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = type => TypeCollector.GetIndex(type, 0);
                }
                info.SignatureTypesInfo = source.SelectMany<KeyValuePair<TypeReference[], int>, TypeReference>(<>f__am$cache2).Select<TypeReference, int>(<>f__am$cache3).ToArray<int>().AsReadOnlyPortable<int>();
                return info;
            }
        }

        public int Count =>
            this._signatures.Count;

        [CompilerGenerated]
        private sealed class <ParametersFor>c__Iterator0 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<TypeReference> $locvar0;
            internal int $PC;
            internal int <i>__0;
            internal TypeReference <type>__1;
            internal IEnumerable<TypeReference> inputParams;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 2:
                        try
                        {
                        }
                        finally
                        {
                            if (this.$locvar0 != null)
                            {
                                this.$locvar0.Dispose();
                            }
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        this.$current = VirtualCallCollector.FormatParameterName(VirtualCallCollector.TypeProvider.SystemObject, VirtualCallCollector.Naming.ThisParameterName);
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_013D;

                    case 1:
                        this.<i>__0 = 1;
                        this.$locvar0 = this.inputParams.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 2:
                        break;

                    case 3:
                        this.$PC = -1;
                        goto Label_013B;

                    default:
                        goto Label_013B;
                }
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<type>__1 = this.$locvar0.Current;
                        this.$current = VirtualCallCollector.FormatParameterName(this.<type>__1, VirtualCallCollector.Naming.ForParameterName(this.<type>__1, this.<i>__0++));
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        flag = true;
                        goto Label_013D;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    if (this.$locvar0 != null)
                    {
                        this.$locvar0.Dispose();
                    }
                }
                this.$current = "const MethodInfo* method";
                if (!this.$disposing)
                {
                    this.$PC = 3;
                }
                goto Label_013D;
            Label_013B:
                return false;
            Label_013D:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new VirtualCallCollector.<ParametersFor>c__Iterator0 { inputParams = this.inputParams };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();

            string IEnumerator<string>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

