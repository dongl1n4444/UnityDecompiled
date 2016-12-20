namespace Unity.IL2CPP.GenericsCollection
{
    using Mono.Cecil;
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

        public GenericContextAwareDeclarationOnlyVisitor(InflatedCollectionCollector generics, GenericContext genericContext, [Optional, DefaultParameterValue(0)] CollectionMode mode)
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
            generics.Arrays.Add(inflatedType);
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
                GenericContext genericContext = new GenericContext(type, contextMethod);
                Unity.Cecil.Visitor.Extensions.Accept(type.ElementType.Resolve(), new GenericContextAwareDeclarationOnlyVisitor(generics, genericContext, CollectionMode.Types));
                foreach (GenericInstanceType type2 in Enumerable.OfType<GenericInstanceType>(type.GenericArguments))
                {
                    ProcessGenericType(Inflater.InflateType(genericContext, type2), generics, null, mode);
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
    }
}

