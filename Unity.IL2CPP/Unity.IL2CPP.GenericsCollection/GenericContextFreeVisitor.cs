using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;
using Unity.Cecil.Visitor;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.GenericsCollection
{
	public class GenericContextFreeVisitor : Visitor
	{
		[Inject]
		public static IIl2CppGenericMethodCollectorWriterService Il2CppGenericMethodCollector;

		[Inject]
		public static IGenericSharingAnalysisService GenericSharingAnalysis;

		[Inject]
		public static ITypeProviderService TypeProvider;

		private readonly InflatedCollectionCollector _generics;

		public GenericContextFreeVisitor(InflatedCollectionCollector generics)
		{
			this._generics = generics;
		}

		protected override void Visit(TypeDefinition typeDefinition, Context context)
		{
			if (ArrayRegistration.ShouldForce2DArrayFor(typeDefinition))
			{
				this.ProcessArray(typeDefinition, 2);
			}
			if (typeDefinition.HasGenericParameters && GenericContextFreeVisitor.GenericSharingAnalysis.AreFullySharableGenericParameters(typeDefinition.GenericParameters))
			{
				this.ProcessGenericType(GenericContextFreeVisitor.GenericSharingAnalysis.GetFullySharedType(typeDefinition));
			}
			base.Visit(typeDefinition, context);
		}

		protected override void Visit(MethodDefinition methodDefinition, Context context)
		{
			if (GenericContextFreeVisitor.MethodHasFullySharableGenericParameters(methodDefinition) && (!methodDefinition.DeclaringType.HasGenericParameters || GenericContextFreeVisitor.TypeHasFullySharableGenericParameters(methodDefinition.DeclaringType)))
			{
				GenericContextAwareVisitor.ProcessGenericMethod((GenericInstanceMethod)GenericContextFreeVisitor.GenericSharingAnalysis.GetFullySharedMethod(methodDefinition), this._generics);
			}
			if (methodDefinition.HasGenericParameters || methodDefinition.DeclaringType.HasGenericParameters)
			{
				if ((!methodDefinition.HasGenericParameters || GenericContextFreeVisitor.GenericSharingAnalysis.AreFullySharableGenericParameters(methodDefinition.GenericParameters)) && (!methodDefinition.DeclaringType.HasGenericParameters || GenericContextFreeVisitor.GenericSharingAnalysis.AreFullySharableGenericParameters(methodDefinition.DeclaringType.GenericParameters)))
				{
					GenericContextFreeVisitor.Il2CppGenericMethodCollector.Add(GenericContextFreeVisitor.GenericSharingAnalysis.GetFullySharedMethod(methodDefinition));
				}
			}
			foreach (GenericParameter current in methodDefinition.GenericParameters)
			{
				foreach (TypeReference current2 in current.Constraints)
				{
					if (current2 is GenericInstanceType && !current2.ContainsGenericParameters())
					{
						this.ProcessGenericType((GenericInstanceType)current2);
					}
				}
			}
			base.Visit(methodDefinition, context);
		}

		private static bool MethodHasFullySharableGenericParameters(MethodDefinition methodDefinition)
		{
			return methodDefinition.HasGenericParameters && GenericContextFreeVisitor.GenericSharingAnalysis.AreFullySharableGenericParameters(methodDefinition.GenericParameters);
		}

		private static bool TypeHasFullySharableGenericParameters(TypeDefinition typeDefinition)
		{
			return typeDefinition.HasGenericParameters && GenericContextFreeVisitor.GenericSharingAnalysis.AreFullySharableGenericParameters(typeDefinition.GenericParameters);
		}

		protected override void Visit(CustomAttributeArgument customAttributeArgument, Context context)
		{
			this.ProcessCustomAttributeArgument(customAttributeArgument);
			base.Visit(customAttributeArgument, context);
		}

		private void ProcessCustomAttributeTypeReferenceRecursive(TypeReference typeReference)
		{
			ArrayType arrayType = typeReference as ArrayType;
			if (arrayType != null)
			{
				this.ProcessCustomAttributeTypeReferenceRecursive(arrayType.ElementType);
			}
			else if (typeReference.IsGenericInstance)
			{
				this.ProcessGenericType((GenericInstanceType)typeReference);
			}
		}

		private void ProcessCustomAttributeArgument(CustomAttributeArgument customAttributeArgument)
		{
			if (TypeReferenceEqualityComparer.AreEqual(customAttributeArgument.Type, GenericContextFreeVisitor.TypeProvider.Corlib.MainModule.GetType("System", "Type"), TypeComparisonMode.Exact))
			{
				TypeReference typeReference = customAttributeArgument.Value as TypeReference;
				if (typeReference != null)
				{
					this.ProcessCustomAttributeTypeReferenceRecursive(typeReference);
				}
			}
			else
			{
				CustomAttributeArgument[] array = customAttributeArgument.Value as CustomAttributeArgument[];
				if (array != null)
				{
					CustomAttributeArgument[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						CustomAttributeArgument customAttributeArgument2 = array2[i];
						this.ProcessCustomAttributeArgument(customAttributeArgument2);
					}
				}
				else if (customAttributeArgument.Value is CustomAttributeArgument)
				{
					this.ProcessCustomAttributeArgument((CustomAttributeArgument)customAttributeArgument.Value);
				}
			}
		}

		protected override void Visit(MethodBody methodBody, Context context)
		{
			if (!methodBody.Method.HasGenericParameters)
			{
				base.Visit(methodBody, context);
			}
		}

		protected override void Visit(TypeReference typeReference, Context context)
		{
			if (context.Role == Role.Operand)
			{
				Instruction instruction = (Instruction)context.Data;
				if (instruction.OpCode.Code == Code.Ldtoken)
				{
					if (typeReference.IsGenericInstance)
					{
						throw new Exception();
					}
				}
				else if (instruction.OpCode.Code == Code.Newarr)
				{
					this.ProcessArray(typeReference, 1);
				}
			}
			base.Visit(typeReference, context);
		}

		protected override void Visit(MethodReference methodReference, Context context)
		{
			GenericInstanceType genericInstanceType = methodReference.DeclaringType as GenericInstanceType;
			GenericInstanceMethod genericInstanceMethod = methodReference as GenericInstanceMethod;
			if (GenericContextFreeVisitor.IsFullyInflated(genericInstanceMethod))
			{
				GenericInstanceType genericInstanceType2 = null;
				if (GenericContextFreeVisitor.IsFullyInflated(genericInstanceType))
				{
					genericInstanceType2 = Inflater.InflateType(null, genericInstanceType);
					this.ProcessGenericType(genericInstanceType2);
				}
				GenericContextAwareVisitor.ProcessGenericMethod(Inflater.InflateMethod(new GenericContext(genericInstanceType2, null), genericInstanceMethod), this._generics);
			}
			else if (GenericContextFreeVisitor.IsFullyInflated(genericInstanceType))
			{
				this.ProcessGenericType(Inflater.InflateType(null, genericInstanceType));
			}
			if (!methodReference.HasGenericParameters && !methodReference.IsGenericInstance)
			{
				base.Visit(methodReference, context);
			}
		}

		protected override void Visit(ArrayType arrayType, Context context)
		{
			if (!arrayType.ContainsGenericParameters())
			{
				this.ProcessArray(arrayType.ElementType, arrayType.Rank);
			}
			base.Visit(arrayType, context);
		}

		protected override void Visit(Instruction instruction, Context context)
		{
			if (instruction.OpCode.Code == Code.Newarr)
			{
				TypeReference typeReference = (TypeReference)instruction.Operand;
				if (!typeReference.ContainsGenericParameters())
				{
					this.ProcessArray(typeReference, 1);
				}
			}
			base.Visit(instruction, context);
		}

		protected override void Visit(GenericInstanceType genericInstanceType, Context context)
		{
			if (!genericInstanceType.ContainsGenericParameters())
			{
				GenericInstanceType inflatedType = Inflater.InflateType(null, genericInstanceType);
				this.ProcessGenericType(inflatedType);
			}
			base.Visit(genericInstanceType, context);
		}

		private void ProcessGenericType(GenericInstanceType inflatedType)
		{
			GenericContextAwareVisitor.ProcessGenericType(inflatedType, this._generics, null);
		}

		private void ProcessArray(TypeReference elementType, int rank)
		{
			ArrayType inflatedType = new ArrayType(Inflater.InflateType(null, elementType), rank);
			GenericContextAwareVisitor.ProcessArray(inflatedType, this._generics, new GenericContext(null, null));
		}

		private static bool IsFullyInflated(GenericInstanceMethod genericInstanceMethod)
		{
			int arg_45_0;
			if (genericInstanceMethod != null)
			{
				if (!genericInstanceMethod.GenericArguments.Any((TypeReference t) => t.ContainsGenericParameters()))
				{
					arg_45_0 = ((!genericInstanceMethod.DeclaringType.ContainsGenericParameters()) ? 1 : 0);
					return arg_45_0 != 0;
				}
			}
			arg_45_0 = 0;
			return arg_45_0 != 0;
		}

		private static bool IsFullyInflated(GenericInstanceType genericInstanceType)
		{
			return genericInstanceType != null && !genericInstanceType.ContainsGenericParameters();
		}
	}
}
