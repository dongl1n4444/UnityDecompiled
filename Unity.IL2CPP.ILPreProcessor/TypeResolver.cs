using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace Unity.IL2CPP.ILPreProcessor
{
	public class TypeResolver
	{
		private readonly IGenericInstance _typeDefinitionContext;

		private readonly IGenericInstance _methodDefinitionContext;

		public TypeResolver()
		{
		}

		public TypeResolver(GenericInstanceType typeDefinitionContext)
		{
			this._typeDefinitionContext = typeDefinitionContext;
		}

		public TypeResolver(GenericInstanceMethod methodDefinitionContext)
		{
			this._methodDefinitionContext = methodDefinitionContext;
		}

		public TypeResolver(GenericInstanceType typeDefinitionContext, GenericInstanceMethod methodDefinitionContext)
		{
			this._typeDefinitionContext = typeDefinitionContext;
			this._methodDefinitionContext = methodDefinitionContext;
		}

		public static TypeResolver For(TypeReference typeReference)
		{
			return (!typeReference.IsGenericInstance) ? new TypeResolver() : new TypeResolver((GenericInstanceType)typeReference);
		}

		public static TypeResolver For(TypeReference typeReference, MethodReference methodReference)
		{
			return new TypeResolver(typeReference as GenericInstanceType, methodReference as GenericInstanceMethod);
		}

		public MethodReference Resolve(MethodReference method)
		{
			MethodReference methodReference = method;
			MethodReference result;
			if (this.IsDummy())
			{
				result = methodReference;
			}
			else
			{
				TypeReference declaringType = this.Resolve(method.DeclaringType);
				GenericInstanceMethod genericInstanceMethod = method as GenericInstanceMethod;
				if (genericInstanceMethod != null)
				{
					methodReference = new MethodReference(method.Name, method.ReturnType, declaringType);
					foreach (ParameterDefinition current in method.Parameters)
					{
						methodReference.Parameters.Add(new ParameterDefinition(current.Name, current.Attributes, current.ParameterType));
					}
					foreach (GenericParameter current2 in genericInstanceMethod.ElementMethod.GenericParameters)
					{
						methodReference.GenericParameters.Add(new GenericParameter(current2.Name, methodReference));
					}
					methodReference.HasThis = method.HasThis;
					GenericInstanceMethod genericInstanceMethod2 = new GenericInstanceMethod(methodReference);
					foreach (TypeReference current3 in genericInstanceMethod.GenericArguments)
					{
						genericInstanceMethod2.GenericArguments.Add(this.Resolve(current3));
					}
					methodReference = genericInstanceMethod2;
				}
				else
				{
					methodReference = new MethodReference(method.Name, method.ReturnType, declaringType);
					foreach (GenericParameter current4 in method.GenericParameters)
					{
						methodReference.GenericParameters.Add(new GenericParameter(current4.Name, methodReference));
					}
					foreach (ParameterDefinition current5 in method.Parameters)
					{
						methodReference.Parameters.Add(new ParameterDefinition(current5.Name, current5.Attributes, current5.ParameterType));
					}
					methodReference.HasThis = method.HasThis;
				}
				result = methodReference;
			}
			return result;
		}

		public FieldReference Resolve(FieldReference field)
		{
			TypeReference typeReference = this.Resolve(field.DeclaringType);
			FieldReference result;
			if (typeReference == field.DeclaringType)
			{
				result = field;
			}
			else
			{
				result = new FieldReference(field.Name, field.FieldType, typeReference);
			}
			return result;
		}

		public TypeReference ResolveReturnType(MethodReference method)
		{
			return this.Resolve(GenericParameterResolver.ResolveReturnTypeIfNeeded(method));
		}

		public TypeReference ResolveParameterType(MethodReference method, ParameterReference parameter)
		{
			return this.Resolve(GenericParameterResolver.ResolveParameterTypeIfNeeded(method, parameter));
		}

		public TypeReference ResolveVariableType(MethodReference method, VariableReference variable)
		{
			return this.Resolve(GenericParameterResolver.ResolveVariableTypeIfNeeded(method, variable));
		}

		public TypeReference ResolveFieldType(FieldReference field)
		{
			return this.Resolve(GenericParameterResolver.ResolveFieldTypeIfNeeded(field));
		}

		public TypeReference Resolve(TypeReference typeReference)
		{
			return this.Resolve(typeReference, true);
		}

		public TypeReference Resolve(TypeReference typeReference, bool includeTypeDefinitions)
		{
			TypeReference result;
			if (this.IsDummy())
			{
				result = typeReference;
			}
			else if (this._typeDefinitionContext != null && this._typeDefinitionContext.GenericArguments.Contains(typeReference))
			{
				result = typeReference;
			}
			else if (this._methodDefinitionContext != null && this._methodDefinitionContext.GenericArguments.Contains(typeReference))
			{
				result = typeReference;
			}
			else
			{
				GenericParameter genericParameter = typeReference as GenericParameter;
				if (genericParameter != null)
				{
					if (this._typeDefinitionContext != null && this._typeDefinitionContext.GenericArguments.Contains(genericParameter))
					{
						result = genericParameter;
					}
					else if (this._methodDefinitionContext != null && this._methodDefinitionContext.GenericArguments.Contains(genericParameter))
					{
						result = genericParameter;
					}
					else
					{
						result = this.ResolveGenericParameter(genericParameter);
					}
				}
				else
				{
					ArrayType arrayType = typeReference as ArrayType;
					if (arrayType != null)
					{
						result = new ArrayType(this.Resolve(arrayType.ElementType), arrayType.Rank);
					}
					else
					{
						PointerType pointerType = typeReference as PointerType;
						if (pointerType != null)
						{
							result = new PointerType(this.Resolve(pointerType.ElementType));
						}
						else
						{
							ByReferenceType byReferenceType = typeReference as ByReferenceType;
							if (byReferenceType != null)
							{
								result = new ByReferenceType(this.Resolve(byReferenceType.ElementType));
							}
							else
							{
								PinnedType pinnedType = typeReference as PinnedType;
								if (pinnedType != null)
								{
									result = new PinnedType(this.Resolve(pinnedType.ElementType));
								}
								else
								{
									GenericInstanceType genericInstanceType = typeReference as GenericInstanceType;
									if (genericInstanceType != null)
									{
										GenericInstanceType genericInstanceType2 = new GenericInstanceType(genericInstanceType.ElementType);
										foreach (TypeReference current in genericInstanceType.GenericArguments)
										{
											genericInstanceType2.GenericArguments.Add(this.Resolve(current));
										}
										result = genericInstanceType2;
									}
									else
									{
										RequiredModifierType requiredModifierType = typeReference as RequiredModifierType;
										if (requiredModifierType != null)
										{
											result = this.Resolve(requiredModifierType.ElementType, includeTypeDefinitions);
										}
										else
										{
											if (includeTypeDefinitions)
											{
												TypeDefinition typeDefinition = typeReference as TypeDefinition;
												if (typeDefinition != null && typeDefinition.HasGenericParameters)
												{
													GenericInstanceType genericInstanceType3 = new GenericInstanceType(typeDefinition);
													foreach (GenericParameter current2 in typeDefinition.GenericParameters)
													{
														genericInstanceType3.GenericArguments.Add(this.Resolve(current2));
													}
													result = genericInstanceType3;
													return result;
												}
											}
											if (typeReference is TypeSpecification)
											{
												throw new NotSupportedException(string.Format("The type {0} cannot be resolved correctly.", typeReference.FullName));
											}
											result = typeReference;
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		internal TypeResolver Nested(GenericInstanceMethod genericInstanceMethod)
		{
			return new TypeResolver(this._typeDefinitionContext as GenericInstanceType, genericInstanceMethod);
		}

		private TypeReference ResolveGenericParameter(GenericParameter genericParameter)
		{
			TypeReference result;
			if (genericParameter.Owner == null)
			{
				result = this.HandleOwnerlessInvalidILCode(genericParameter);
			}
			else
			{
				if (!(genericParameter.Owner is MemberReference))
				{
					throw new NotSupportedException();
				}
				result = ((genericParameter.Type != GenericParameterType.Type) ? ((this._methodDefinitionContext == null) ? genericParameter : this._methodDefinitionContext.GenericArguments[genericParameter.Position]) : this._typeDefinitionContext.GenericArguments[genericParameter.Position]);
			}
			return result;
		}

		private TypeReference HandleOwnerlessInvalidILCode(GenericParameter genericParameter)
		{
			TypeReference result;
			if (genericParameter.Type == GenericParameterType.Method && this._typeDefinitionContext != null && genericParameter.Position < this._typeDefinitionContext.GenericArguments.Count)
			{
				result = this._typeDefinitionContext.GenericArguments[genericParameter.Position];
			}
			else
			{
				result = genericParameter.Module.TypeSystem.Object;
			}
			return result;
		}

		private bool IsDummy()
		{
			return this._typeDefinitionContext == null && this._methodDefinitionContext == null;
		}
	}
}
