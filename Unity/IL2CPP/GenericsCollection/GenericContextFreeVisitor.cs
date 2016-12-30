namespace Unity.IL2CPP.GenericsCollection
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.Cecil.Visitor;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class GenericContextFreeVisitor : Unity.Cecil.Visitor.Visitor
    {
        private readonly InflatedCollectionCollector _generics;
        private readonly IInteropDataCollector _interopDataCollector;
        [CompilerGenerated]
        private static Func<TypeReference, bool> <>f__am$cache0;
        [Inject]
        public static IGenericSharingAnalysisService GenericSharingAnalysis;
        [Inject]
        public static IIl2CppGenericMethodCollectorWriterService Il2CppGenericMethodCollector;
        [Inject]
        public static IStatsService Stats;
        [Inject]
        public static ITypeProviderService TypeProvider;
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        public GenericContextFreeVisitor(InflatedCollectionCollector generics, IInteropDataCollector interopDataCollector)
        {
            this._generics = generics;
            this._interopDataCollector = interopDataCollector;
        }

        private static bool IsFullyInflated(GenericInstanceMethod genericInstanceMethod)
        {
            if ((genericInstanceMethod != null) && (<>f__am$cache0 == null))
            {
                <>f__am$cache0 = t => t.ContainsGenericParameters();
            }
            return (!genericInstanceMethod.GenericArguments.Any<TypeReference>(<>f__am$cache0) && !genericInstanceMethod.DeclaringType.ContainsGenericParameters());
        }

        private static bool IsFullyInflated(GenericInstanceType genericInstanceType) => 
            ((genericInstanceType != null) && !genericInstanceType.ContainsGenericParameters());

        private static bool MethodHasFullySharableGenericParameters(MethodDefinition methodDefinition) => 
            (methodDefinition.HasGenericParameters && GenericSharingAnalysis.AreFullySharableGenericParameters(methodDefinition.GenericParameters));

        private void ProcessArray(TypeReference elementType, int rank)
        {
            ArrayType inflatedType = new ArrayType(Inflater.InflateType(null, elementType), rank);
            GenericContextAwareVisitor.ProcessArray(inflatedType, this._generics, new GenericContext(null, null));
        }

        private void ProcessCustomAttributeArgument(CustomAttributeArgument customAttributeArgument)
        {
            if (Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(customAttributeArgument.Type, TypeProvider.Corlib.MainModule.GetType("System", "Type"), TypeComparisonMode.Exact))
            {
                TypeReference typeReference = customAttributeArgument.Value as TypeReference;
                if (typeReference != null)
                {
                    this.ProcessCustomAttributeTypeReferenceRecursive(typeReference);
                }
            }
            else
            {
                CustomAttributeArgument[] argumentArray = customAttributeArgument.Value as CustomAttributeArgument[];
                if (argumentArray != null)
                {
                    foreach (CustomAttributeArgument argument in argumentArray)
                    {
                        this.ProcessCustomAttributeArgument(argument);
                    }
                }
                else if (customAttributeArgument.Value is CustomAttributeArgument)
                {
                    this.ProcessCustomAttributeArgument((CustomAttributeArgument) customAttributeArgument.Value);
                }
            }
        }

        private void ProcessCustomAttributeTypeReferenceRecursive(TypeReference typeReference)
        {
            ArrayType type = typeReference as ArrayType;
            if (type != null)
            {
                this.ProcessCustomAttributeTypeReferenceRecursive(type.ElementType);
            }
            else if (typeReference.IsGenericInstance)
            {
                this.ProcessGenericType((GenericInstanceType) typeReference);
            }
        }

        private void ProcessGenericType(GenericInstanceType inflatedType)
        {
            GenericContextAwareVisitor.ProcessGenericType(inflatedType, this._generics, null);
        }

        private void ProcessIReferenceIfNeeded(TypeDefinition typeDefinition)
        {
            if (typeDefinition.CanBoxToWindowsRuntime())
            {
                string fullName;
                GenericInstanceType inflatedType = new GenericInstanceType(TypeProvider.IReferenceType) {
                    GenericArguments = { typeDefinition }
                };
                this.ProcessGenericType(inflatedType);
                string windowsRuntimePrimitiveName = typeDefinition.GetWindowsRuntimePrimitiveName();
                if (windowsRuntimePrimitiveName != null)
                {
                    fullName = windowsRuntimePrimitiveName;
                }
                else
                {
                    fullName = WindowsRuntimeProjections.ProjectToWindowsRuntime(typeDefinition).FullName;
                }
                this._interopDataCollector.AddWindowsRuntimeTypeWithName(inflatedType, $"Windows.Foundation.IReference`1<{fullName}>");
                Stats.RecordWindowsRuntimeBoxedType();
            }
        }

        private static bool TypeHasFullySharableGenericParameters(TypeDefinition typeDefinition) => 
            (typeDefinition.HasGenericParameters && GenericSharingAnalysis.AreFullySharableGenericParameters(typeDefinition.GenericParameters));

        protected override void Visit(ArrayType arrayType, Unity.Cecil.Visitor.Context context)
        {
            if (!arrayType.ContainsGenericParameters())
            {
                this.ProcessArray(arrayType.ElementType, arrayType.Rank);
            }
            base.Visit(arrayType, context);
        }

        protected override void Visit(Instruction instruction, Unity.Cecil.Visitor.Context context)
        {
            if (instruction.OpCode.Code == Code.Newarr)
            {
                TypeReference operand = (TypeReference) instruction.Operand;
                if (!operand.ContainsGenericParameters())
                {
                    this.ProcessArray(operand, 1);
                }
            }
            base.Visit(instruction, context);
        }

        protected override void Visit(MethodBody methodBody, Unity.Cecil.Visitor.Context context)
        {
            if (!methodBody.Method.HasGenericParameters)
            {
                base.Visit(methodBody, context);
            }
        }

        protected override void Visit(CustomAttributeArgument customAttributeArgument, Unity.Cecil.Visitor.Context context)
        {
            this.ProcessCustomAttributeArgument(customAttributeArgument);
            base.Visit(customAttributeArgument, context);
        }

        protected override void Visit(GenericInstanceType genericInstanceType, Unity.Cecil.Visitor.Context context)
        {
            if (!genericInstanceType.ContainsGenericParameters())
            {
                GenericInstanceType inflatedType = Inflater.InflateType(null, genericInstanceType);
                this.ProcessGenericType(inflatedType);
            }
            base.Visit(genericInstanceType, context);
        }

        protected override void Visit(MethodDefinition methodDefinition, Unity.Cecil.Visitor.Context context)
        {
            if (MethodHasFullySharableGenericParameters(methodDefinition) && (!methodDefinition.DeclaringType.HasGenericParameters || TypeHasFullySharableGenericParameters(methodDefinition.DeclaringType)))
            {
                GenericContextAwareVisitor.ProcessGenericMethod((GenericInstanceMethod) GenericSharingAnalysis.GetFullySharedMethod(methodDefinition), this._generics);
            }
            if (((methodDefinition.HasGenericParameters || methodDefinition.DeclaringType.HasGenericParameters) && (!methodDefinition.HasGenericParameters || GenericSharingAnalysis.AreFullySharableGenericParameters(methodDefinition.GenericParameters))) && (!methodDefinition.IsStripped() && (!methodDefinition.DeclaringType.HasGenericParameters || GenericSharingAnalysis.AreFullySharableGenericParameters(methodDefinition.DeclaringType.GenericParameters))))
            {
                Il2CppGenericMethodCollector.Add(GenericSharingAnalysis.GetFullySharedMethod(methodDefinition));
            }
            foreach (GenericParameter parameter in methodDefinition.GenericParameters)
            {
                foreach (TypeReference reference in parameter.Constraints)
                {
                    if ((reference is GenericInstanceType) && !reference.ContainsGenericParameters())
                    {
                        this.ProcessGenericType((GenericInstanceType) reference);
                    }
                }
            }
            base.Visit(methodDefinition, context);
        }

        protected override void Visit(MethodReference methodReference, Unity.Cecil.Visitor.Context context)
        {
            GenericInstanceType declaringType = methodReference.DeclaringType as GenericInstanceType;
            GenericInstanceMethod genericInstanceMethod = methodReference as GenericInstanceMethod;
            if (IsFullyInflated(genericInstanceMethod))
            {
                GenericInstanceType inflatedType = null;
                if (IsFullyInflated(declaringType))
                {
                    inflatedType = Inflater.InflateType(null, declaringType);
                    this.ProcessGenericType(inflatedType);
                }
                GenericContextAwareVisitor.ProcessGenericMethod(Inflater.InflateMethod(new GenericContext(inflatedType, null), genericInstanceMethod), this._generics);
            }
            else if (IsFullyInflated(declaringType))
            {
                this.ProcessGenericType(Inflater.InflateType(null, declaringType));
            }
            if (!methodReference.HasGenericParameters && !methodReference.IsGenericInstance)
            {
                base.Visit(methodReference, context);
            }
        }

        protected override void Visit(TypeDefinition typeDefinition, Unity.Cecil.Visitor.Context context)
        {
            if (ArrayRegistration.ShouldForce2DArrayFor(typeDefinition))
            {
                this.ProcessArray(typeDefinition, 2);
            }
            if (typeDefinition.HasGenericParameters && GenericSharingAnalysis.AreFullySharableGenericParameters(typeDefinition.GenericParameters))
            {
                this.ProcessGenericType(GenericSharingAnalysis.GetFullySharedType(typeDefinition));
            }
            this.ProcessIReferenceIfNeeded(typeDefinition);
            base.Visit(typeDefinition, context);
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
    }
}

