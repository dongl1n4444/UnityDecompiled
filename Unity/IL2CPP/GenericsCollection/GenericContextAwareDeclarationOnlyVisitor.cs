namespace Unity.IL2CPP.GenericsCollection
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Unity.Cecil.Visitor;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class GenericContextAwareDeclarationOnlyVisitor : Unity.Cecil.Visitor.Visitor
    {
        private readonly GenericContext _genericContext;
        private readonly InflatedCollectionCollector _generics;
        private readonly CollectionMode _mode;
        [Inject]
        public static ITypeProviderService TypeProvider;

        public GenericContextAwareDeclarationOnlyVisitor(InflatedCollectionCollector generics, GenericContext genericContext, CollectionMode mode = 0)
        {
            this._generics = generics;
            this._genericContext = genericContext;
            this._mode = mode;
        }

        private void ProcessArray(TypeReference elementType, int rank)
        {
            ArrayType inflatedType = new ArrayType(Inflater.InflateType(this._genericContext, elementType), rank);
            ProcessArray(inflatedType, this._generics, this._genericContext);
        }

        internal static void ProcessArray(ArrayType inflatedType, InflatedCollectionCollector generics, GenericContext currentContext)
        {
            if (inflatedType.NeedsComCallableWrapper())
            {
                GenericContextAwareVisitor.ProcessArray(inflatedType, generics, currentContext);
            }
            else
            {
                generics.Arrays.Add(inflatedType);
            }
        }

        private void ProcessGenericType(GenericInstanceType inflatedType)
        {
            ProcessGenericType(inflatedType, this._generics, this._genericContext.Method, this._mode);
        }

        internal static void ProcessGenericType(GenericInstanceType type, InflatedCollectionCollector generics, GenericInstanceMethod contextMethod, CollectionMode mode)
        {
            bool flag = generics.TypeDeclarations.Add(type);
            if (((mode != CollectionMode.Types) || flag) && ((mode != CollectionMode.MethodsAndTypes) || generics.TypeMethodDeclarations.Add(type)))
            {
                if (type.NeedsComCallableWrapper())
                {
                    GenericContextAwareVisitor.ProcessGenericType(type, generics, contextMethod);
                }
                else
                {
                    GenericContext genericContext = new GenericContext(type, contextMethod);
                    type.ElementType.Resolve().Accept(new GenericContextAwareDeclarationOnlyVisitor(generics, genericContext, CollectionMode.Types));
                    foreach (GenericInstanceType type2 in type.GenericArguments.OfType<GenericInstanceType>())
                    {
                        ProcessGenericType(Inflater.InflateType(genericContext, type2), generics, null, mode);
                    }
                }
            }
        }

        protected override void Visit(ArrayType arrayType, Unity.Cecil.Visitor.Context context)
        {
            this.ProcessArray(arrayType.ElementType, arrayType.Rank);
            base.Visit(arrayType, context);
        }

        protected override void Visit(FieldDefinition fieldDefinition, Unity.Cecil.Visitor.Context context)
        {
            if (!GenericsUtilities.CheckForMaximumRecursion(this._genericContext.Type))
            {
                base.Visit(fieldDefinition, context);
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
            if ((!methodDefinition.HasGenericParameters || ((this._genericContext.Method != null) && (this._genericContext.Method.Resolve() == methodDefinition))) && !GenericsUtilities.CheckForMaximumRecursion(this._genericContext.Type))
            {
                base.VisitTypeReference(methodDefinition.ReturnType, context.ReturnType(methodDefinition));
                foreach (ParameterDefinition definition in methodDefinition.Parameters)
                {
                    this.Visit(definition, context.Parameter(methodDefinition));
                }
                VisitMethodBody(methodDefinition, this._genericContext, this._generics);
            }
        }

        protected override void Visit(TypeDefinition typeDefinition, Unity.Cecil.Visitor.Context context)
        {
            if (context.Role != Role.NestedType)
            {
                if (typeDefinition.BaseType != null)
                {
                    base.VisitTypeReference(typeDefinition.BaseType, context.BaseType(typeDefinition));
                }
                foreach (FieldDefinition definition in typeDefinition.Fields)
                {
                    this.Visit(definition, context.Member(typeDefinition));
                }
                if (this._mode != CollectionMode.Types)
                {
                    foreach (MethodDefinition definition2 in typeDefinition.Methods)
                    {
                        this.Visit(definition2, context.Member(typeDefinition));
                    }
                }
            }
        }

        private static void VisitMethodBody(MethodDefinition methodDefinition, GenericContext genericContext, InflatedCollectionCollector generics)
        {
            if (methodDefinition.HasBody)
            {
                foreach (Instruction instruction in methodDefinition.Body.Instructions)
                {
                    MethodReference operand = instruction.Operand as MethodReference;
                    if (operand != null)
                    {
                        GenericInstanceType declaringType = operand.DeclaringType as GenericInstanceType;
                        GenericInstanceMethod genericInstanceMethod = operand as GenericInstanceMethod;
                        if ((declaringType != null) || (genericInstanceMethod != null))
                        {
                            if (instruction.OpCode.Code == Code.Newobj)
                            {
                                if (declaringType != null)
                                {
                                    GenericInstanceType item = Inflater.InflateType(genericContext, declaringType);
                                    if (generics.TypeDeclarations.Add(item))
                                    {
                                        GenericContextAwareVisitor.ProcessGenericType(item, generics, genericContext.Method);
                                    }
                                }
                            }
                            else
                            {
                                GenericInstanceMethod method2 = (genericInstanceMethod == null) ? null : Inflater.InflateMethod(genericContext, genericInstanceMethod);
                                if (generics.VisitedMethodBodies.Add(method2))
                                {
                                    GenericInstanceType genericInstance = (declaringType == null) ? null : Inflater.InflateType(genericContext, declaringType);
                                    if (!GenericsUtilities.CheckForMaximumRecursion(genericInstance) && !GenericsUtilities.CheckForMaximumRecursion(method2))
                                    {
                                        GenericContext context = new GenericContext(genericInstance, method2);
                                        VisitMethodBody(operand.Resolve(), context, generics);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

