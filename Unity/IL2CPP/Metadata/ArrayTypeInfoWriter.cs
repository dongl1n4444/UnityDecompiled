namespace Unity.IL2CPP.Metadata
{
    using Mono.Cecil;
    using Mono.Collections.Generic;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class ArrayTypeInfoWriter
    {
        [Inject]
        public static ITypeProviderService TypeProvider;

        [DebuggerHidden]
        internal static IEnumerable<MethodDefinition> GetArrayInterfaceMethods(TypeDefinition arrayType, TypeDefinition interfaceType, string arrayMethodPrefix) => 
            new <GetArrayInterfaceMethods>c__Iterator0 { 
                interfaceType = interfaceType,
                arrayType = arrayType,
                arrayMethodPrefix = arrayMethodPrefix,
                $PC = -2
            };

        internal static MethodReference InflateArrayMethod(MethodDefinition method, TypeReference elementType)
        {
            if (!method.HasGenericParameters)
            {
                return method;
            }
            return new GenericInstanceMethod(method) { GenericArguments = { elementType } };
        }

        internal static IEnumerable<MethodReference> InflateArrayMethods(ArrayType arrayType)
        {
            <InflateArrayMethods>c__AnonStorey1 storey = new <InflateArrayMethods>c__AnonStorey1 {
                arrayType = arrayType
            };
            ModuleDefinition mainModule = TypeProvider.Corlib.MainModule;
            TypeDefinition type = mainModule.GetType("System.Array");
            TypeDefinition interfaceType = mainModule.GetType("System.Collections.Generic.ICollection`1");
            TypeDefinition definition4 = mainModule.GetType("System.Collections.Generic.IList`1");
            TypeDefinition definition5 = mainModule.GetType("System.Collections.Generic.IEnumerable`1");
            return Enumerable.Empty<MethodReference>().Concat<MethodReference>(GetArrayInterfaceMethods(type, interfaceType, "InternalArray__ICollection_").Select<MethodDefinition, MethodReference>(new Func<MethodDefinition, MethodReference>(storey.<>m__0))).Concat<MethodReference>(GetArrayInterfaceMethods(type, definition4, "InternalArray__").Select<MethodDefinition, MethodReference>(new Func<MethodDefinition, MethodReference>(storey.<>m__1))).Concat<MethodReference>(GetArrayInterfaceMethods(type, definition5, "InternalArray__IEnumerable_").Select<MethodDefinition, MethodReference>(new Func<MethodDefinition, MethodReference>(storey.<>m__2)));
        }

        private static bool IsGenericInstanceWithMoreThanOneGenericArgument(TypeReference type)
        {
            if (type.IsGenericInstance)
            {
                GenericInstanceType type2 = type as GenericInstanceType;
                if (((type2 != null) && type2.HasGenericArguments) && (type2.GenericArguments.Count > 1))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsSpecialCollectionGenericInterface(string typeFullName) => 
            ((typeFullName.Contains("System.Collections.Generic.ICollection`1") || typeFullName.Contains("System.Collections.Generic.IEnumerable`1")) || typeFullName.Contains("System.Collections.Generic.IList`1"));

        internal static IEnumerable<TypeReference> TypeAndAllBaseAndInterfaceTypesFor(TypeReference type)
        {
            List<TypeReference> list = new List<TypeReference>();
            while (type != null)
            {
                list.Add(type);
                foreach (TypeReference reference in type.GetInterfaces())
                {
                    if (!IsGenericInstanceWithMoreThanOneGenericArgument(reference) && !IsSpecialCollectionGenericInterface(reference.FullName))
                    {
                        list.Add(reference);
                    }
                }
                type = type.GetBaseType();
            }
            return list;
        }

        [CompilerGenerated]
        private sealed class <GetArrayInterfaceMethods>c__Iterator0 : IEnumerable, IEnumerable<MethodDefinition>, IEnumerator, IDisposable, IEnumerator<MethodDefinition>
        {
            internal MethodDefinition $current;
            internal bool $disposing;
            internal Collection<MethodDefinition>.Enumerator $locvar0;
            private <GetArrayInterfaceMethods>c__AnonStorey2 $locvar1;
            private <GetArrayInterfaceMethods>c__AnonStorey3 $locvar2;
            internal int $PC;
            internal MethodDefinition <arrayMethod>__2;
            internal MethodDefinition <method>__1;
            internal string arrayMethodPrefix;
            internal TypeDefinition arrayType;
            internal TypeDefinition interfaceType;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                        try
                        {
                        }
                        finally
                        {
                            this.$locvar0.Dispose();
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
                        this.$locvar1 = new <GetArrayInterfaceMethods>c__AnonStorey2();
                        this.$locvar1.arrayMethodPrefix = this.arrayMethodPrefix;
                        this.$locvar0 = this.interfaceType.Methods.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_0147;
                }
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<method>__1 = this.$locvar0.Current;
                        this.$locvar2 = new <GetArrayInterfaceMethods>c__AnonStorey3();
                        this.$locvar2.<>f__ref$0 = this;
                        this.$locvar2.<>f__ref$2 = this.$locvar1;
                        this.$locvar2.methodName = this.<method>__1.Name;
                        this.<arrayMethod>__2 = this.arrayType.Methods.SingleOrDefault<MethodDefinition>(new Func<MethodDefinition, bool>(this.$locvar2.<>m__0));
                        if (this.<arrayMethod>__2 != null)
                        {
                            this.$current = this.<arrayMethod>__2;
                            if (!this.$disposing)
                            {
                                this.$PC = 1;
                            }
                            flag = true;
                            return true;
                        }
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    this.$locvar0.Dispose();
                }
                this.$PC = -1;
            Label_0147:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<MethodDefinition> IEnumerable<MethodDefinition>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new ArrayTypeInfoWriter.<GetArrayInterfaceMethods>c__Iterator0 { 
                    interfaceType = this.interfaceType,
                    arrayType = this.arrayType,
                    arrayMethodPrefix = this.arrayMethodPrefix
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<Mono.Cecil.MethodDefinition>.GetEnumerator();

            MethodDefinition IEnumerator<MethodDefinition>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;

            private sealed class <GetArrayInterfaceMethods>c__AnonStorey2
            {
                internal string arrayMethodPrefix;
            }

            private sealed class <GetArrayInterfaceMethods>c__AnonStorey3
            {
                internal ArrayTypeInfoWriter.<GetArrayInterfaceMethods>c__Iterator0 <>f__ref$0;
                internal ArrayTypeInfoWriter.<GetArrayInterfaceMethods>c__Iterator0.<GetArrayInterfaceMethods>c__AnonStorey2 <>f__ref$2;
                internal string methodName;

                internal bool <>m__0(MethodDefinition m) => 
                    (((m.Name.Length == (this.<>f__ref$2.arrayMethodPrefix.Length + this.methodName.Length)) && m.Name.StartsWith(this.<>f__ref$2.arrayMethodPrefix)) && m.Name.EndsWith(this.methodName));
            }
        }

        [CompilerGenerated]
        private sealed class <InflateArrayMethods>c__AnonStorey1
        {
            internal ArrayType arrayType;

            internal MethodReference <>m__0(MethodDefinition m) => 
                ArrayTypeInfoWriter.InflateArrayMethod(m, this.arrayType.ElementType);

            internal MethodReference <>m__1(MethodDefinition m) => 
                ArrayTypeInfoWriter.InflateArrayMethod(m, this.arrayType.ElementType);

            internal MethodReference <>m__2(MethodDefinition m) => 
                ArrayTypeInfoWriter.InflateArrayMethod(m, this.arrayType.ElementType);
        }
    }
}

