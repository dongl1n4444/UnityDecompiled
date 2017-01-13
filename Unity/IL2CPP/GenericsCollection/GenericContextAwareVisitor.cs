namespace Unity.IL2CPP.GenericsCollection
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Rocks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.Cecil.Visitor;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common;
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

        private static void AddGenericComparerIfNeeded(GenericInstanceType type, InflatedCollectionCollector generics, GenericInstanceMethod contextMethod, TypeDefinition comparerDefinition, TypeDefinition genericElementComparisonInterfaceDefinition, TypeDefinition genericComparerDefinition)
        {
            <AddGenericComparerIfNeeded>c__AnonStorey1 storey = new <AddGenericComparerIfNeeded>c__AnonStorey1();
            if (type.Resolve() == comparerDefinition)
            {
                TypeReference reference = type.GenericArguments[0];
                TypeReference[] arguments = new TypeReference[] { reference };
                storey.genericElementComparisonInterface = genericElementComparisonInterfaceDefinition.MakeGenericInstanceType(arguments);
                if (reference.GetInterfaces().Any<TypeReference>(new Func<TypeReference, bool>(storey.<>m__0)))
                {
                    TypeReference[] referenceArray2 = new TypeReference[] { reference };
                    ProcessGenericType(genericComparerDefinition.MakeGenericInstanceType(referenceArray2), generics, contextMethod);
                }
            }
        }

        private static void AddNullableComparerIfNeeded(GenericInstanceType type, InflatedCollectionCollector generics, GenericInstanceMethod contextMethod, ModuleDefinition corlib)
        {
            <AddNullableComparerIfNeeded>c__AnonStorey0 storey = new <AddNullableComparerIfNeeded>c__AnonStorey0();
            TypeDefinition definition = corlib.GetType("System.Collections.Generic", "ObjectComparer`1");
            TypeDefinition definition2 = corlib.GetType("System", "Nullable`1");
            TypeDefinition self = corlib.GetType("System", "IComparable`1");
            TypeDefinition definition4 = corlib.GetType("System.Collections.Generic", "NullableComparer`1");
            if ((type.Resolve() == definition) && (type.GenericArguments[0].Resolve() == definition2))
            {
                GenericInstanceType type2 = (GenericInstanceType) type.GenericArguments[0];
                TypeReference reference = type2.GenericArguments[0];
                TypeReference[] arguments = new TypeReference[] { reference };
                storey.genericElementComparisonInterface = self.MakeGenericInstanceType(arguments);
                if (reference.GetInterfaces().Any<TypeReference>(new Func<TypeReference, bool>(storey.<>m__0)))
                {
                    TypeReference[] referenceArray2 = new TypeReference[] { reference };
                    ProcessGenericType(definition4.MakeGenericInstanceType(referenceArray2), generics, contextMethod);
                }
            }
        }

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
                TypeDefinition definition3 = TypeProvider.Corlib.MainModule.GetType("System.Collections.Generic.ICollection`1");
                TypeDefinition definition4 = TypeProvider.Corlib.MainModule.GetType("System.Collections.Generic.IList`1");
                TypeDefinition definition5 = TypeProvider.Corlib.MainModule.GetType("System.Collections.Generic.IEnumerable`1");
                IEnumerable<TypeReference> enumerable = ArrayTypeInfoWriter.TypeAndAllBaseAndInterfaceTypesFor(inflatedType.ElementType);
                foreach (TypeReference reference in enumerable)
                {
                    GenericInstanceType type = new GenericInstanceType(definition3) {
                        GenericArguments = { reference }
                    };
                    ProcessGenericType(type, generics, currentContext.Method);
                    type = new GenericInstanceType(definition4) {
                        GenericArguments = { reference }
                    };
                    ProcessGenericType(type, generics, currentContext.Method);
                    type = new GenericInstanceType(definition5) {
                        GenericArguments = { reference }
                    };
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
                ProcessGenericType(sharedType, generics, contextMethod);
                GenericContext genericContext = new GenericContext(type, contextMethod);
                type.ElementType.Resolve().Accept(new GenericContextAwareDeclarationOnlyVisitor(generics, genericContext, CollectionMode.MethodsAndTypes));
            }
            else if (generics.Types.Add(type))
            {
                ProcessHardcodedDependencies(type, generics, contextMethod);
                GenericContext context2 = new GenericContext(type, contextMethod);
                type.ElementType.Resolve().Accept(new GenericContextAwareVisitor(generics, context2));
            }
        }

        private static void ProcessHardcodedDependencies(GenericInstanceType type, InflatedCollectionCollector generics, GenericInstanceMethod contextMethod)
        {
            ModuleDefinition mainModule = TypeProvider.Corlib.MainModule;
            AddArrayIfNeeded(type, generics, contextMethod, mainModule.GetType("System.Collections.Generic", "IEnumerable`1"), mainModule.GetType("System.Collections.Generic", "ICollection`1"), mainModule.GetType("System.Collections.Generic", "IList`1"));
            AddGenericComparerIfNeeded(type, generics, contextMethod, mainModule.GetType("System.Collections.Generic", "EqualityComparer`1"), mainModule.GetType("System", "IEquatable`1"), mainModule.GetType("System.Collections.Generic", "GenericEqualityComparer`1"));
            AddGenericComparerIfNeeded(type, generics, contextMethod, mainModule.GetType("System.Collections.Generic", "Comparer`1"), mainModule.GetType("System", "IComparable`1"), mainModule.GetType("System.Collections.Generic", "GenericComparer`1"));
            AddNullableComparerIfNeeded(type, generics, contextMethod, mainModule);
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
        private sealed class <AddNullableComparerIfNeeded>c__AnonStorey0
        {
            internal GenericInstanceType genericElementComparisonInterface;

            internal bool <>m__0(TypeReference i) => 
                Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(i, this.genericElementComparisonInterface, TypeComparisonMode.Exact);
        }
    }
}

