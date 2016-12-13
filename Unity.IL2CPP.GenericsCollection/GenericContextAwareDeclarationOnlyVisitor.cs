using Mono.Cecil;
using System;
using System.Linq;
using Unity.Cecil.Visitor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.GenericsCollection
{
	public class GenericContextAwareDeclarationOnlyVisitor : Visitor
	{
		[Inject]
		public static ITypeProviderService TypeProvider;

		private readonly InflatedCollectionCollector _generics;

		private readonly GenericContext _genericContext;

		private readonly CollectionMode _mode;

		public GenericContextAwareDeclarationOnlyVisitor(InflatedCollectionCollector generics, GenericContext genericContext, CollectionMode mode = CollectionMode.MethodsAndTypes)
		{
			this._generics = generics;
			this._genericContext = genericContext;
			this._mode = mode;
		}

		protected override void Visit(TypeDefinition typeDefinition, Context context)
		{
			if (context.Role != Role.NestedType)
			{
				if (typeDefinition.BaseType != null)
				{
					base.VisitTypeReference(typeDefinition.BaseType, context.BaseType(typeDefinition));
				}
				foreach (FieldDefinition current in typeDefinition.Fields)
				{
					this.Visit(current, context.Member(typeDefinition));
				}
				if (this._mode != CollectionMode.Types)
				{
					foreach (MethodDefinition current2 in typeDefinition.Methods)
					{
						this.Visit(current2, context.Member(typeDefinition));
					}
				}
			}
		}

		protected override void Visit(MethodDefinition methodDefinition, Context context)
		{
			if (!methodDefinition.HasGenericParameters || (this._genericContext.Method != null && this._genericContext.Method.Resolve() == methodDefinition))
			{
				if (!GenericsUtilities.CheckForMaximumRecursion(this._genericContext.Type))
				{
					base.VisitTypeReference(methodDefinition.ReturnType, context.ReturnType(methodDefinition));
					foreach (ParameterDefinition current in methodDefinition.Parameters)
					{
						this.Visit(current, context.Parameter(methodDefinition));
					}
				}
			}
		}

		protected override void Visit(FieldDefinition fieldDefinition, Context context)
		{
			if (!GenericsUtilities.CheckForMaximumRecursion(this._genericContext.Type))
			{
				base.Visit(fieldDefinition, context);
			}
		}

		protected override void Visit(ArrayType arrayType, Context context)
		{
			this.ProcessArray(arrayType.ElementType, arrayType.Rank);
			base.Visit(arrayType, context);
		}

		protected override void Visit(GenericInstanceType genericInstanceType, Context context)
		{
			GenericInstanceType inflatedType = Inflater.InflateType(this._genericContext, genericInstanceType);
			this.ProcessGenericType(inflatedType);
			base.Visit(genericInstanceType, context);
		}

		private void ProcessGenericType(GenericInstanceType inflatedType)
		{
			GenericContextAwareDeclarationOnlyVisitor.ProcessGenericType(inflatedType, this._generics, this._genericContext.Method, this._mode);
		}

		internal static void ProcessGenericType(GenericInstanceType type, InflatedCollectionCollector generics, GenericInstanceMethod contextMethod, CollectionMode mode)
		{
			bool flag = generics.TypeDeclarations.Add(type);
			if (mode != CollectionMode.Types || flag)
			{
				if (mode != CollectionMode.MethodsAndTypes || generics.TypeMethodDeclarations.Add(type))
				{
					GenericContext genericContext = new GenericContext(type, contextMethod);
					type.ElementType.Resolve().Accept(new GenericContextAwareDeclarationOnlyVisitor(generics, genericContext, CollectionMode.Types));
					foreach (GenericInstanceType current in type.GenericArguments.OfType<GenericInstanceType>())
					{
						GenericContextAwareDeclarationOnlyVisitor.ProcessGenericType(Inflater.InflateType(genericContext, current), generics, null, mode);
					}
				}
			}
		}

		internal static void ProcessArray(ArrayType inflatedType, InflatedCollectionCollector generics, GenericContext currentContext)
		{
			generics.Arrays.Add(inflatedType);
		}

		private void ProcessArray(TypeReference elementType, int rank)
		{
			ArrayType inflatedType = new ArrayType(Inflater.InflateType(this._genericContext, elementType), rank);
			GenericContextAwareDeclarationOnlyVisitor.ProcessArray(inflatedType, this._generics, this._genericContext);
		}
	}
}
