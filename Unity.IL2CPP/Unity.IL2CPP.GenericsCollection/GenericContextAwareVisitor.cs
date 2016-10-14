using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cecil.Visitor;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Metadata;

namespace Unity.IL2CPP.GenericsCollection
{
	public class GenericContextAwareVisitor : Visitor
	{
		private readonly InflatedCollectionCollector _generics;

		private readonly GenericContext _genericContext;

		[Inject]
		public static IGenericSharingAnalysisService GenericSharingAnalysis;

		[Inject]
		public static ITypeProviderService TypeProvider;

		public GenericContextAwareVisitor(InflatedCollectionCollector generics, GenericContext genericContext)
		{
			this._generics = generics;
			this._genericContext = genericContext;
		}

		protected override void Visit(TypeDefinition typeDefinition, Context context)
		{
			if (context.Role != Role.NestedType)
			{
				base.Visit(typeDefinition, context);
			}
		}

		protected override void Visit(MethodDefinition methodDefinition, Context context)
		{
			if (!methodDefinition.HasGenericParameters || (this._genericContext.Method != null && this._genericContext.Method.Resolve() == methodDefinition))
			{
				if (GenericsUtilities.CheckForMaximumRecursion(this._genericContext.Type))
				{
					this.AddEmptyTypeIfNecessary(methodDefinition.ReturnType);
					foreach (ParameterDefinition current in methodDefinition.Parameters)
					{
						this.AddEmptyTypeIfNecessary(current.ParameterType);
					}
				}
				else
				{
					base.Visit(methodDefinition, context);
				}
			}
		}

		protected override void Visit(PropertyDefinition propertyDefinition, Context context)
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

		protected override void Visit(FieldDefinition fieldDefinition, Context context)
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

		protected override void Visit(FieldReference fieldReference, Context context)
		{
			GenericInstanceType genericInstanceType = fieldReference.DeclaringType as GenericInstanceType;
			if (genericInstanceType != null)
			{
				GenericInstanceType genericInstanceType2 = Inflater.InflateType(this._genericContext, genericInstanceType);
				this.ProcessGenericType(genericInstanceType2);
				GenericContextAwareVisitor visitor = new GenericContextAwareVisitor(this._generics, new GenericContext(genericInstanceType2, this._genericContext.Method));
				fieldReference.Resolve().Accept(visitor);
			}
			else
			{
				base.Visit(fieldReference, context);
			}
		}

		protected override void Visit(MethodReference methodReference, Context context)
		{
			MethodDefinition methodDefinition = methodReference.Resolve();
			GenericInstanceType genericInstanceType = methodReference.DeclaringType as GenericInstanceType;
			GenericInstanceMethod genericInstanceMethod = methodReference as GenericInstanceMethod;
			if (genericInstanceMethod != null)
			{
				GenericContextAwareVisitor.ProcessGenericMethod(Inflater.InflateMethod(this._genericContext, genericInstanceMethod), this._generics);
			}
			else if (methodDefinition != null && genericInstanceType != null)
			{
				this.ProcessGenericType(Inflater.InflateType(this._genericContext, genericInstanceType));
			}
			else
			{
				base.Visit(methodReference, context);
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

		protected override void Visit(Instruction instruction, Context context)
		{
			if (instruction.OpCode.Code == Code.Newarr)
			{
				this.ProcessArray((TypeReference)instruction.Operand, 1);
			}
			base.Visit(instruction, context);
		}

		private void ProcessGenericType(GenericInstanceType inflatedType)
		{
			GenericContextAwareVisitor.ProcessGenericType(inflatedType, this._generics, this._genericContext.Method);
		}

		internal static void ProcessGenericMethod(GenericInstanceMethod method, InflatedCollectionCollector generics)
		{
			if (method.DeclaringType.IsGenericInstance)
			{
				GenericContextAwareVisitor.ProcessGenericType((GenericInstanceType)method.DeclaringType, generics, method);
			}
			GenericContextAwareVisitor.ProcessGenericArguments(method.GenericArguments, generics);
			MethodReference sharedMethod = GenericContextAwareVisitor.GenericSharingAnalysis.GetSharedMethod(method);
			if (GenericContextAwareVisitor.GenericSharingAnalysis.CanShareMethod(method) && !MethodReferenceComparer.AreEqual(sharedMethod, method))
			{
				GenericContextAwareVisitor.ProcessGenericMethod((GenericInstanceMethod)sharedMethod, generics);
				GenericContext genericContext = new GenericContext(method.DeclaringType as GenericInstanceType, method);
				method.Resolve().Accept(new GenericContextAwareDeclarationOnlyVisitor(generics, genericContext, CollectionMode.MethodsAndTypes));
			}
			else if (generics.Methods.Add(method))
			{
				GenericContext genericContext2 = new GenericContext(method.DeclaringType as GenericInstanceType, method);
				method.Resolve().Accept(new GenericContextAwareVisitor(generics, genericContext2));
			}
		}

		internal static void ProcessGenericType(GenericInstanceType type, InflatedCollectionCollector generics, GenericInstanceMethod contextMethod)
		{
			generics.TypeDeclarations.Add(type);
			generics.TypeMethodDeclarations.Add(type);
			GenericContextAwareVisitor.ProcessGenericArguments(type.GenericArguments, generics);
			GenericInstanceType sharedType = GenericContextAwareVisitor.GenericSharingAnalysis.GetSharedType(type);
			if (GenericContextAwareVisitor.GenericSharingAnalysis.CanShareType(type) && !TypeReferenceEqualityComparer.AreEqual(sharedType, type, TypeComparisonMode.Exact))
			{
				GenericContextAwareVisitor.ProcessGenericType(sharedType, generics, contextMethod);
				GenericContext genericContext = new GenericContext(type, contextMethod);
				type.ElementType.Resolve().Accept(new GenericContextAwareDeclarationOnlyVisitor(generics, genericContext, CollectionMode.MethodsAndTypes));
			}
			else if (generics.Types.Add(type))
			{
				GenericContextAwareVisitor.ProcessHardcodedDependencies(type, generics, contextMethod);
				GenericContext genericContext2 = new GenericContext(type, contextMethod);
				type.ElementType.Resolve().Accept(new GenericContextAwareVisitor(generics, genericContext2));
			}
		}

		private static void ProcessGenericArguments(IEnumerable<TypeReference> genericArguments, InflatedCollectionCollector generics)
		{
			foreach (GenericInstanceType current in genericArguments.OfType<GenericInstanceType>())
			{
				if (generics.TypeDeclarations.Add(current))
				{
					current.Resolve().Accept(new GenericContextAwareDeclarationOnlyVisitor(generics, new GenericContext(current, null), CollectionMode.MethodsAndTypes));
				}
			}
		}

		private static void ProcessHardcodedDependencies(GenericInstanceType type, InflatedCollectionCollector generics, GenericInstanceMethod contextMethod)
		{
			ModuleDefinition mainModule = GenericContextAwareVisitor.TypeProvider.Corlib.MainModule;
			GenericContextAwareVisitor.AddArrayIfNeeded(type, generics, contextMethod, mainModule.GetType("System.Collections.Generic", "IEnumerable`1"), mainModule.GetType("System.Collections.Generic", "ICollection`1"), mainModule.GetType("System.Collections.Generic", "IList`1"));
			GenericContextAwareVisitor.AddGenericComparerIfNeeded(type, generics, contextMethod, mainModule.GetType("System.Collections.Generic", "EqualityComparer`1"), mainModule.GetType("System", "IEquatable`1"), mainModule.GetType("System.Collections.Generic", "GenericEqualityComparer`1"));
			GenericContextAwareVisitor.AddGenericComparerIfNeeded(type, generics, contextMethod, mainModule.GetType("System.Collections.Generic", "Comparer`1"), mainModule.GetType("System", "IComparable`1"), mainModule.GetType("System.Collections.Generic", "GenericComparer`1"));
		}

		private static void AddArrayIfNeeded(GenericInstanceType type, InflatedCollectionCollector generics, GenericInstanceMethod contextMethod, TypeDefinition ienumerableDefinition, TypeDefinition icollectionDefinition, TypeDefinition ilistDefinition)
		{
			TypeDefinition typeDefinition = type.Resolve();
			if (typeDefinition == ienumerableDefinition || typeDefinition == icollectionDefinition || typeDefinition == ilistDefinition)
			{
				GenericContextAwareVisitor.ProcessArray(new ArrayType(type.GenericArguments[0]), generics, new GenericContext(type, contextMethod));
			}
		}

		private static void AddGenericComparerIfNeeded(GenericInstanceType type, InflatedCollectionCollector generics, GenericInstanceMethod contextMethod, TypeDefinition comparerDefinition, TypeDefinition genericElementComparisonInterfaceDefinition, TypeDefinition genericComparerDefinition)
		{
			TypeDefinition typeDefinition = type.Resolve();
			if (typeDefinition == comparerDefinition)
			{
				TypeReference typeReference = type.GenericArguments[0];
				GenericInstanceType genericElementComparisonInterface = genericElementComparisonInterfaceDefinition.MakeGenericInstanceType(new TypeReference[]
				{
					typeReference
				});
				if (typeReference.GetInterfaces().Any((TypeReference i) => TypeReferenceEqualityComparer.AreEqual(i, genericElementComparisonInterface, TypeComparisonMode.Exact)))
				{
					GenericInstanceType type2 = genericComparerDefinition.MakeGenericInstanceType(new TypeReference[]
					{
						typeReference
					});
					GenericContextAwareVisitor.ProcessGenericType(type2, generics, contextMethod);
				}
			}
		}

		internal static void ProcessArray(ArrayType inflatedType, InflatedCollectionCollector generics, GenericContext currentContext)
		{
			if (generics.Arrays.Add(inflatedType))
			{
				TypeDefinition type = GenericContextAwareVisitor.TypeProvider.Corlib.MainModule.GetType("System", "Array");
				MethodDefinition methodDefinition = type.Methods.Single((MethodDefinition m) => m.Name == "InternalArray__IEnumerable_GetEnumerator");
				if (methodDefinition != null)
				{
					GenericInstanceMethod method = Inflater.InflateMethod(currentContext, new GenericInstanceMethod(methodDefinition)
					{
						GenericArguments = 
						{
							inflatedType.ElementType
						}
					});
					GenericContextAwareVisitor visitor = new GenericContextAwareVisitor(generics, new GenericContext(currentContext.Type, method));
					methodDefinition.Accept(visitor);
				}
				foreach (GenericInstanceMethod current in ArrayTypeInfoWriter.InflateArrayMethods(inflatedType).OfType<GenericInstanceMethod>())
				{
					GenericContextAwareVisitor.ProcessGenericMethod(current, generics);
				}
				TypeDefinition type2 = GenericContextAwareVisitor.TypeProvider.Corlib.MainModule.GetType("System.Collections.Generic.ICollection`1");
				TypeDefinition type3 = GenericContextAwareVisitor.TypeProvider.Corlib.MainModule.GetType("System.Collections.Generic.IList`1");
				TypeDefinition type4 = GenericContextAwareVisitor.TypeProvider.Corlib.MainModule.GetType("System.Collections.Generic.IEnumerable`1");
				IEnumerable<TypeReference> enumerable = ArrayTypeInfoWriter.TypeAndAllBaseAndInterfaceTypesFor(inflatedType.ElementType);
				foreach (TypeReference current2 in enumerable)
				{
					GenericContextAwareVisitor.ProcessGenericType(new GenericInstanceType(type2)
					{
						GenericArguments = 
						{
							current2
						}
					}, generics, currentContext.Method);
					GenericContextAwareVisitor.ProcessGenericType(new GenericInstanceType(type3)
					{
						GenericArguments = 
						{
							current2
						}
					}, generics, currentContext.Method);
					GenericContextAwareVisitor.ProcessGenericType(new GenericInstanceType(type4)
					{
						GenericArguments = 
						{
							current2
						}
					}, generics, currentContext.Method);
				}
			}
		}

		private void ProcessArray(TypeReference elementType, int rank)
		{
			ArrayType inflatedType = new ArrayType(Inflater.InflateType(this._genericContext, elementType), rank);
			GenericContextAwareVisitor.ProcessArray(inflatedType, this._generics, this._genericContext);
		}

		private void AddEmptyTypeIfNecessary(TypeReference type)
		{
			GenericInstanceType genericInstanceType = Inflater.InflateTypeWithoutException(this._genericContext, type) as GenericInstanceType;
			if (genericInstanceType != null)
			{
				this._generics.EmptyTypes.Add(genericInstanceType);
			}
		}
	}
}
