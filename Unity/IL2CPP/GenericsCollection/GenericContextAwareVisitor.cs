namespace Unity.IL2CPP.GenericsCollection
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Rocks;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Unity.Cecil.Visitor;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Metadata;

    public class GenericContextAwareVisitor : Unity.Cecil.Visitor.Visitor
    {
        private readonly GenericContext _genericContext;
        private readonly InflatedCollectionCollector _generics;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        [Inject]
        public static IGenericSharingAnalysisService GenericSharingAnalysis;
        [Inject]
        public static ITypeProviderService TypeProvider;
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        public GenericContextAwareVisitor(InflatedCollectionCollector generics, GenericContext genericContext)
        {
            this._generics = generics;
            this._genericContext = genericContext;
        }

        private static void AddArrayIfNeeded(GenericInstanceType type, InflatedCollectionCollector generics, GenericInstanceMethod contextMethod, TypeDefinition ienumerableDefinition, TypeDefinition icollectionDefinition, TypeDefinition ilistDefinition)
        {
            TypeDefinition definition = type.Resolve();
            if (((definition == ienumerableDefinition) || (definition == icollectionDefinition)) || (definition == ilistDefinition))
            {
                ProcessArray(new ArrayType(type.GenericArguments[0]), generics, new GenericContext(type, contextMethod));
            }
        }

        private void AddEmptyTypeIfNecessary(TypeReference type)
        {
            GenericInstanceType item = Inflater.InflateTypeWithoutException(this._genericContext, type) as GenericInstanceType;
            if (item != null)
            {
                this._generics.EmptyTypes.Add(item);
            }
        }

        private static void AddEnumEqualityComparerIfNeeded(TypeReference keyType, InflatedCollectionCollector generics, GenericInstanceMethod contextMethod)
        {
            if ((CodeGenOptions.Dotnetprofile == DotNetProfile.Net45) && keyType.IsEnum())
            {
                TypeDefinition self = null;
                MetadataType metadataType = keyType.GetUnderlyingEnumType().MetadataType;
                switch (metadataType)
                {
                    case MetadataType.SByte:
                        self = TypeProvider.Corlib.MainModule.GetType("System.Collections.Generic.SByteEnumEqualityComparer`1");
                        break;

                    case MetadataType.Int16:
                        self = TypeProvider.Corlib.MainModule.GetType("System.Collections.Generic.ShortEnumEqualityComparer`1");
                        break;

                    default:
                        switch (metadataType)
                        {
                            case MetadataType.Int64:
                            case MetadataType.UInt64:
                                self = TypeProvider.Corlib.MainModule.GetType("System.Collections.Generic.LongEnumEqualityComparer`1");
                                break;

                            default:
                                self = TypeProvider.Corlib.MainModule.GetType("System.Collections.Generic.EnumEqualityComparer`1");
                                break;
                        }
                        break;
                }
                TypeReference[] arguments = new TypeReference[] { keyType };
                ProcessGenericType(self.MakeGenericInstanceType(arguments), generics, contextMethod);
            }
        }

        private static void AddGenericComparerIfNeeded(TypeReference genericArgument, InflatedCollectionCollector generics, GenericInstanceMethod contextMethod, TypeDefinition genericElementComparisonInterfaceDefinition, TypeDefinition genericComparerDefinition)
        {
            <AddGenericComparerIfNeeded>c__AnonStorey1 storey = new <AddGenericComparerIfNeeded>c__AnonStorey1();
            TypeReference[] arguments = new TypeReference[] { genericArgument };
            storey.genericElementComparisonInterface = genericElementComparisonInterfaceDefinition.MakeGenericInstanceType(arguments);
            if (genericArgument.GetInterfaces().Any<TypeReference>(new Func<TypeReference, bool>(storey.<>m__0)))
            {
                TypeReference[] referenceArray2 = new TypeReference[] { genericArgument };
                ProcessGenericType(genericComparerDefinition.MakeGenericInstanceType(referenceArray2), generics, contextMethod);
            }
        }

        private static IEnumerable<GenericInstanceType> GetArrayExtraTypes(ArrayType type)
        {
            if (type.Rank != 1)
            {
                return new GenericInstanceType[0];
            }
            List<TypeReference> types = new List<TypeReference>();
            if (!type.ElementType.IsValueType)
            {
                types.AddRange(ArrayTypeInfoWriter.TypeAndAllBaseAndInterfaceTypesFor(type.ElementType));
                if (type.ElementType.IsArray)
                {
                    types.AddRange(GetArrayExtraTypes((ArrayType) type.ElementType));
                }
            }
            else
            {
                types.Add(type.ElementType);
            }
            return GetArrayExtraTypes(types);
        }

        [DebuggerHidden]
        private static IEnumerable<GenericInstanceType> GetArrayExtraTypes(IEnumerable<TypeReference> types) => 
            new <GetArrayExtraTypes>c__Iterator0 { 
                types = types,
                $PC = -2
            };

        private void ProcessArray(TypeReference elementType, int rank)
        {
            ArrayType inflatedType = new ArrayType(Inflater.InflateType(this._genericContext, elementType), rank);
            ProcessArray(inflatedType, this._generics, this._genericContext);
        }

        internal static void ProcessArray(ArrayType inflatedType, InflatedCollectionCollector generics, GenericContext currentContext)
        {
            if (generics.Arrays.Add(inflatedType))
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = m => m.Name == "InternalArray__IEnumerable_GetEnumerator";
                }
                MethodDefinition definition2 = TypeProvider.Corlib.MainModule.GetType("System", "Array").Methods.Single<MethodDefinition>(<>f__am$cache0);
                if (definition2 != null)
                {
                    GenericInstanceMethod genericInstanceMethod = new GenericInstanceMethod(definition2) {
                        GenericArguments = { inflatedType.ElementType }
                    };
                    GenericInstanceMethod method = Inflater.InflateMethod(currentContext, genericInstanceMethod);
                    GenericContextAwareVisitor visitor = new GenericContextAwareVisitor(generics, new GenericContext(currentContext.Type, method));
                    definition2.Accept(visitor);
                }
                foreach (GenericInstanceMethod method3 in ArrayTypeInfoWriter.InflateArrayMethods(inflatedType).OfType<GenericInstanceMethod>())
                {
                    ProcessGenericMethod(method3, generics);
                }
                foreach (GenericInstanceType type in GetArrayExtraTypes(inflatedType))
                {
                    ProcessGenericType(type, generics, currentContext.Method);
                }
            }
        }

        private static void ProcessGenericArguments(IEnumerable<TypeReference> genericArguments, InflatedCollectionCollector generics)
        {
            foreach (GenericInstanceType type in genericArguments.OfType<GenericInstanceType>())
            {
                if (generics.TypeDeclarations.Add(type))
                {
                    type.Resolve().Accept(new GenericContextAwareDeclarationOnlyVisitor(generics, new GenericContext(type, null), CollectionMode.MethodsAndTypes));
                }
            }
        }

        internal static void ProcessGenericMethod(GenericInstanceMethod method, InflatedCollectionCollector generics)
        {
            if (method.DeclaringType.IsGenericInstance)
            {
                ProcessGenericType((GenericInstanceType) method.DeclaringType, generics, method);
            }
            ProcessGenericArguments(method.GenericArguments, generics);
            MethodReference sharedMethod = GenericSharingAnalysis.GetSharedMethod(method);
            if (GenericSharingAnalysis.CanShareMethod(method) && !Unity.IL2CPP.Common.MethodReferenceComparer.AreEqual(sharedMethod, method))
            {
                ProcessGenericMethod((GenericInstanceMethod) sharedMethod, generics);
                GenericContext genericContext = new GenericContext(method.DeclaringType as GenericInstanceType, method);
                method.Resolve().Accept(new GenericContextAwareDeclarationOnlyVisitor(generics, genericContext, CollectionMode.MethodsAndTypes));
            }
            else if (generics.Methods.Add(method))
            {
                GenericContext context2 = new GenericContext(method.DeclaringType as GenericInstanceType, method);
                method.Resolve().Accept(new GenericContextAwareVisitor(generics, context2));
            }
        }

        private void ProcessGenericType(GenericInstanceType inflatedType)
        {
            ProcessGenericType(inflatedType, this._generics, this._genericContext.Method);
        }

        internal static void ProcessGenericType(GenericInstanceType type, InflatedCollectionCollector generics, GenericInstanceMethod contextMethod)
        {
            generics.TypeDeclarations.Add(type);
            generics.TypeMethodDeclarations.Add(type);
            ProcessGenericArguments(type.GenericArguments, generics);
            GenericInstanceType sharedType = GenericSharingAnalysis.GetSharedType(type);
            if (GenericSharingAnalysis.CanShareType(type) && !Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual((TypeReference) sharedType, (TypeReference) type, TypeComparisonMode.Exact))
            {
                ProcessHardcodedDependencies(type, generics, contextMethod);
                ProcessWindowsRuntimeTypesFull(type, generics, contextMethod);
                ProcessGenericType(sharedType, generics, contextMethod);
                GenericContext genericContext = new GenericContext(type, contextMethod);
                type.ElementType.Resolve().Accept(new GenericContextAwareDeclarationOnlyVisitor(generics, genericContext, CollectionMode.MethodsAndTypes));
            }
            else if (generics.Types.Add(type))
            {
                ProcessHardcodedDependencies(type, generics, contextMethod);
                ProcessWindowsRuntimeTypes(type, generics, contextMethod);
                GenericContext context2 = new GenericContext(type, contextMethod);
                type.ElementType.Resolve().Accept(new GenericContextAwareVisitor(generics, context2));
            }
        }

        private static void ProcessHardcodedDependencies(GenericInstanceType type, InflatedCollectionCollector generics, GenericInstanceMethod contextMethod)
        {
            ModuleDefinition mainModule = TypeProvider.Corlib.MainModule;
            AddArrayIfNeeded(type, generics, contextMethod, mainModule.GetType("System.Collections.Generic", "IEnumerable`1"), mainModule.GetType("System.Collections.Generic", "ICollection`1"), mainModule.GetType("System.Collections.Generic", "IList`1"));
            if (type.GenericArguments.Count > 0)
            {
                TypeDefinition definition2 = type.Resolve();
                TypeReference reference = type.GenericArguments[0];
                if (definition2 == mainModule.GetType("System.Collections.Generic", "EqualityComparer`1"))
                {
                    if (!reference.IsNullable())
                    {
                        AddGenericComparerIfNeeded(reference, generics, contextMethod, mainModule.GetType("System", "IEquatable`1"), mainModule.GetType("System.Collections.Generic", "GenericEqualityComparer`1"));
                        AddEnumEqualityComparerIfNeeded(reference, generics, contextMethod);
                    }
                    else if (CodeGenOptions.Dotnetprofile == DotNetProfile.Net45)
                    {
                        TypeReference genericArgument = ((GenericInstanceType) reference).GenericArguments[0];
                        AddGenericComparerIfNeeded(genericArgument, generics, contextMethod, mainModule.GetType("System", "IEquatable`1"), mainModule.GetType("System.Collections.Generic", "NullableEqualityComparer`1"));
                    }
                }
                else if (definition2 == mainModule.GetType("System.Collections.Generic", "Comparer`1"))
                {
                    AddGenericComparerIfNeeded(reference, generics, contextMethod, mainModule.GetType("System", "IComparable`1"), mainModule.GetType("System.Collections.Generic", "GenericComparer`1"));
                }
                else if ((definition2 == mainModule.GetType("System.Collections.Generic", "ObjectComparer`1")) && reference.IsNullable())
                {
                    TypeReference reference3 = ((GenericInstanceType) reference).GenericArguments[0];
                    AddGenericComparerIfNeeded(reference3, generics, contextMethod, mainModule.GetType("System", "IComparable`1"), mainModule.GetType("System.Collections.Generic", "NullableComparer`1"));
                }
            }
        }

        private static void ProcessWindowsRuntimeTypes(GenericInstanceType type, InflatedCollectionCollector generics, GenericInstanceMethod contextMethod)
        {
            TypeDefinition definition = type.Resolve();
            if (definition.IsWindowsRuntime)
            {
                if ((WindowsRuntimeProjections.ProjectToCLR(definition) != definition) && type.IsValidForWindowsRuntimeType())
                {
                    generics.WindowsRuntimeCCWs.Add(type);
                }
            }
            else
            {
                foreach (GenericInstanceType type2 in type.GetAllAssignableWindowsRuntimeTypes())
                {
                    ProcessGenericType(type2, generics, contextMethod);
                }
            }
        }

        private static void ProcessWindowsRuntimeTypesFull(TypeReference type, InflatedCollectionCollector generics, GenericInstanceMethod contextMethod)
        {
            if (type.MetadataType == MetadataType.GenericInstance)
            {
                ProcessWindowsRuntimeTypes((GenericInstanceType) type, generics, contextMethod);
            }
            TypeDefinition definition = type.Resolve();
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(type);
            if (definition.BaseType != null)
            {
                ProcessWindowsRuntimeTypesFull(resolver.Resolve(definition.BaseType), generics, contextMethod);
            }
            foreach (InterfaceImplementation implementation in definition.Interfaces)
            {
                ProcessWindowsRuntimeTypesFull(resolver.Resolve(implementation.InterfaceType), generics, contextMethod);
            }
        }

        protected override void Visit(ArrayType arrayType, Unity.Cecil.Visitor.Context context)
        {
            this.ProcessArray(arrayType.ElementType, arrayType.Rank);
            base.Visit(arrayType, context);
        }

        protected override void Visit(Instruction instruction, Unity.Cecil.Visitor.Context context)
        {
            if (instruction.OpCode.Code == Code.Newarr)
            {
                this.ProcessArray((TypeReference) instruction.Operand, 1);
            }
            base.Visit(instruction, context);
        }

        protected override void Visit(FieldDefinition fieldDefinition, Unity.Cecil.Visitor.Context context)
        {
            if (GenericsUtilities.CheckForMaximumRecursion(this._genericContext.Type))
            {
                this.AddEmptyTypeIfNecessary(fieldDefinition.FieldType);
            }
            else
            {
                base.Visit(fieldDefinition, context);
            }
        }

        protected override void Visit(FieldReference fieldReference, Unity.Cecil.Visitor.Context context)
        {
            GenericInstanceType declaringType = fieldReference.DeclaringType as GenericInstanceType;
            if (declaringType != null)
            {
                GenericInstanceType inflatedType = Inflater.InflateType(this._genericContext, declaringType);
                this.ProcessGenericType(inflatedType);
                GenericContextAwareVisitor visitor = new GenericContextAwareVisitor(this._generics, new GenericContext(inflatedType, this._genericContext.Method));
                fieldReference.Resolve().Accept(visitor);
            }
            else
            {
                base.Visit(fieldReference, context);
            }
        }

        protected override void Visit(GenericInstanceType genericInstanceType, Unity.Cecil.Visitor.Context context)
        {
            GenericInstanceType inflatedType = Inflater.InflateType(this._genericContext, genericInstanceType);
            this.ProcessGenericType(inflatedType);
            base.Visit(genericInstanceType, context);
        }

        protected override void Visit(MethodDefinition methodDefinition, Unity.Cecil.Visitor.Context context)
        {
            if (!methodDefinition.HasGenericParameters || ((this._genericContext.Method != null) && (this._genericContext.Method.Resolve() == methodDefinition)))
            {
                if (GenericsUtilities.CheckForMaximumRecursion(this._genericContext.Type))
                {
                    this.AddEmptyTypeIfNecessary(methodDefinition.ReturnType);
                    foreach (ParameterDefinition definition in methodDefinition.Parameters)
                    {
                        this.AddEmptyTypeIfNecessary(definition.ParameterType);
                    }
                }
                else
                {
                    base.Visit(methodDefinition, context);
                }
            }
        }

        protected override void Visit(MethodReference methodReference, Unity.Cecil.Visitor.Context context)
        {
            MethodDefinition definition = methodReference.Resolve();
            GenericInstanceType declaringType = methodReference.DeclaringType as GenericInstanceType;
            GenericInstanceMethod genericInstanceMethod = methodReference as GenericInstanceMethod;
            if (genericInstanceMethod != null)
            {
                ProcessGenericMethod(Inflater.InflateMethod(this._genericContext, genericInstanceMethod), this._generics);
            }
            else if ((definition != null) && (declaringType != null))
            {
                this.ProcessGenericType(Inflater.InflateType(this._genericContext, declaringType));
            }
            else
            {
                base.Visit(methodReference, context);
            }
        }

        protected override void Visit(PropertyDefinition propertyDefinition, Unity.Cecil.Visitor.Context context)
        {
            if (GenericsUtilities.CheckForMaximumRecursion(this._genericContext.Type))
            {
                this.AddEmptyTypeIfNecessary(propertyDefinition.PropertyType);
            }
            else
            {
                base.Visit(propertyDefinition, context);
            }
        }

        protected override void Visit(TypeDefinition typeDefinition, Unity.Cecil.Visitor.Context context)
        {
            if (context.Role != Role.NestedType)
            {
                base.Visit(typeDefinition, context);
            }
        }

        protected override void Visit(TypeReference typeReference, Unity.Cecil.Visitor.Context context)
        {
            if (context.Role == Role.Operand)
            {
                Instruction data = (Instruction) context.Data;
                if (data.OpCode.Code == Code.Ldtoken)
                {
                    if (typeReference.IsGenericInstance)
                    {
                        throw new Exception();
                    }
                }
                else if (data.OpCode.Code == Code.Newarr)
                {
                    this.ProcessArray(typeReference, 1);
                }
            }
            base.Visit(typeReference, context);
        }

        [CompilerGenerated]
        private sealed class <AddGenericComparerIfNeeded>c__AnonStorey1
        {
            internal GenericInstanceType genericElementComparisonInterface;

            internal bool <>m__0(TypeReference i) => 
                Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(i, this.genericElementComparisonInterface, TypeComparisonMode.Exact);
        }

        [CompilerGenerated]
        private sealed class <GetArrayExtraTypes>c__Iterator0 : IEnumerable, IEnumerable<GenericInstanceType>, IEnumerator, IDisposable, IEnumerator<GenericInstanceType>
        {
            internal GenericInstanceType $current;
            internal bool $disposing;
            internal IEnumerator<TypeReference> $locvar0;
            internal int $PC;
            internal TypeDefinition <iCollectionType>__1;
            internal GenericInstanceType <iCollectionTypeGenericInstanceType>__3;
            internal TypeDefinition <iEnumerableType>__1;
            internal GenericInstanceType <iEnumerableTypeTypeGenericInstanceType>__3;
            internal TypeDefinition <iListType>__1;
            internal GenericInstanceType <iListTypeGenericInstanceType>__3;
            internal TypeReference <type>__2;
            internal IEnumerable<TypeReference> types;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                    case 2:
                    case 3:
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
                        this.<iListType>__1 = GenericContextAwareVisitor.TypeProvider.Corlib.MainModule.GetType("System.Collections.Generic.IList`1");
                        this.<iCollectionType>__1 = GenericContextAwareVisitor.TypeProvider.Corlib.MainModule.GetType("System.Collections.Generic.ICollection`1");
                        this.<iEnumerableType>__1 = GenericContextAwareVisitor.TypeProvider.Corlib.MainModule.GetType("System.Collections.Generic.IEnumerable`1");
                        this.$locvar0 = this.types.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                    case 2:
                    case 3:
                        break;

                    default:
                        goto Label_01DC;
                }
                try
                {
                    switch (num)
                    {
                        case 1:
                            goto Label_0112;

                        case 2:
                            goto Label_015B;
                    }
                    while (this.$locvar0.MoveNext())
                    {
                        this.<type>__2 = this.$locvar0.Current;
                        this.<iListTypeGenericInstanceType>__3 = new GenericInstanceType(this.<iListType>__1);
                        this.<iListTypeGenericInstanceType>__3.GenericArguments.Add(this.<type>__2);
                        this.$current = this.<iListTypeGenericInstanceType>__3;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        flag = true;
                        goto Label_01DE;
                    Label_0112:
                        this.<iCollectionTypeGenericInstanceType>__3 = new GenericInstanceType(this.<iCollectionType>__1);
                        this.<iCollectionTypeGenericInstanceType>__3.GenericArguments.Add(this.<type>__2);
                        this.$current = this.<iCollectionTypeGenericInstanceType>__3;
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        flag = true;
                        goto Label_01DE;
                    Label_015B:
                        this.<iEnumerableTypeTypeGenericInstanceType>__3 = new GenericInstanceType(this.<iEnumerableType>__1);
                        this.<iEnumerableTypeTypeGenericInstanceType>__3.GenericArguments.Add(this.<type>__2);
                        this.$current = this.<iEnumerableTypeTypeGenericInstanceType>__3;
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        flag = true;
                        goto Label_01DE;
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
                this.$PC = -1;
            Label_01DC:
                return false;
            Label_01DE:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<GenericInstanceType> IEnumerable<GenericInstanceType>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new GenericContextAwareVisitor.<GetArrayExtraTypes>c__Iterator0 { types = this.types };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<Mono.Cecil.GenericInstanceType>.GetEnumerator();

            GenericInstanceType IEnumerator<GenericInstanceType>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

