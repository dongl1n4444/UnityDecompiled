using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.SerializationLogic
{
	public class TypeResolver
	{
		private readonly IGenericInstance _typeDefinitionContext;

		private readonly IGenericInstance _methodDefinitionContext;

		private readonly Dictionary<string, GenericInstanceHolder> _context = new Dictionary<string, GenericInstanceHolder>();

		public TypeResolver()
		{
		}

		public TypeResolver(IGenericInstance typeDefinitionContext)
		{
			this._typeDefinitionContext = typeDefinitionContext;
		}

		public TypeResolver(GenericInstanceMethod methodDefinitionContext)
		{
			this._methodDefinitionContext = methodDefinitionContext;
		}

		public TypeResolver(IGenericInstance typeDefinitionContext, IGenericInstance methodDefinitionContext)
		{
			this._typeDefinitionContext = typeDefinitionContext;
			this._methodDefinitionContext = methodDefinitionContext;
		}

		public void Add(GenericInstanceType genericInstanceType)
		{
			this.Add(TypeResolver.ElementTypeFor(genericInstanceType).FullName, genericInstanceType);
		}

		public void Remove(GenericInstanceType genericInstanceType)
		{
			this.Remove(genericInstanceType.ElementType.FullName, genericInstanceType);
		}

		public void Add(GenericInstanceMethod genericInstanceMethod)
		{
			this.Add(TypeResolver.ElementTypeFor(genericInstanceMethod).FullName, genericInstanceMethod);
		}

		private static MemberReference ElementTypeFor(TypeSpecification genericInstanceType)
		{
			return genericInstanceType.ElementType;
		}

		private static MemberReference ElementTypeFor(MethodSpecification genericInstanceMethod)
		{
			return genericInstanceMethod.ElementMethod;
		}

		public void Remove(GenericInstanceMethod genericInstanceMethod)
		{
			this.Remove(genericInstanceMethod.ElementMethod.FullName, genericInstanceMethod);
		}

		public TypeReference Resolve(TypeReference typeReference)
		{
			GenericParameter genericParameter = typeReference as GenericParameter;
			TypeReference result;
			if (genericParameter != null)
			{
				result = this.Resolve(this.ResolveGenericParameter(genericParameter));
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
							GenericInstanceType genericInstanceType = typeReference as GenericInstanceType;
							if (genericInstanceType != null)
							{
								GenericInstanceType genericInstanceType2 = new GenericInstanceType(this.Resolve(genericInstanceType.ElementType));
								foreach (TypeReference current in genericInstanceType.GenericArguments)
								{
									genericInstanceType2.GenericArguments.Add(this.Resolve(current));
								}
								result = genericInstanceType2;
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
									RequiredModifierType requiredModifierType = typeReference as RequiredModifierType;
									if (requiredModifierType != null)
									{
										result = this.Resolve(requiredModifierType.ElementType);
									}
									else
									{
										OptionalModifierType optionalModifierType = typeReference as OptionalModifierType;
										if (optionalModifierType != null)
										{
											result = new OptionalModifierType(this.Resolve(optionalModifierType.ModifierType), this.Resolve(optionalModifierType.ElementType));
										}
										else
										{
											SentinelType sentinelType = typeReference as SentinelType;
											if (sentinelType != null)
											{
												result = new SentinelType(this.Resolve(sentinelType.ElementType));
											}
											else
											{
												FunctionPointerType functionPointerType = typeReference as FunctionPointerType;
												if (functionPointerType != null)
												{
													throw new NotSupportedException("Function pointer types are not supported by the SerializationWeaver");
												}
												if (typeReference is TypeSpecification)
												{
													throw new NotSupportedException();
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
			}
			return result;
		}

		private TypeReference ResolveGenericParameter(GenericParameter genericParameter)
		{
			if (genericParameter.Owner == null)
			{
				throw new NotSupportedException();
			}
			MemberReference memberReference = genericParameter.Owner as MemberReference;
			if (memberReference == null)
			{
				throw new NotSupportedException();
			}
			string fullName = memberReference.FullName;
			TypeReference result;
			if (!this._context.ContainsKey(fullName))
			{
				result = ((genericParameter.Type != GenericParameterType.Type) ? this._methodDefinitionContext.GenericArguments[genericParameter.Position] : this._typeDefinitionContext.GenericArguments[genericParameter.Position]);
			}
			else
			{
				result = this.GenericArgumentAt(fullName, genericParameter.Position);
			}
			return result;
		}

		private TypeReference GenericArgumentAt(string key, int position)
		{
			return this._context[key].GenericInstance.GenericArguments[position];
		}

		private void Add(string key, IGenericInstance value)
		{
			GenericInstanceHolder genericInstanceHolder;
			if (this._context.TryGetValue(key, out genericInstanceHolder))
			{
				MemberReference memberReference = value as MemberReference;
				if (memberReference == null)
				{
					throw new NotSupportedException();
				}
				MemberReference memberReference2 = (MemberReference)genericInstanceHolder.GenericInstance;
				if (memberReference2.FullName != memberReference.FullName)
				{
					throw new ArgumentException("Duplicate key!", "key");
				}
				genericInstanceHolder.Count++;
			}
			else
			{
				this._context.Add(key, new GenericInstanceHolder
				{
					Count = 1,
					GenericInstance = value
				});
			}
		}

		private void Remove(string key, IGenericInstance value)
		{
			GenericInstanceHolder genericInstanceHolder;
			if (!this._context.TryGetValue(key, out genericInstanceHolder))
			{
				throw new ArgumentException("Invalid key!", "key");
			}
			MemberReference memberReference = value as MemberReference;
			if (memberReference == null)
			{
				throw new NotSupportedException();
			}
			MemberReference memberReference2 = (MemberReference)genericInstanceHolder.GenericInstance;
			if (memberReference2.FullName != memberReference.FullName)
			{
				throw new ArgumentException("Invalid value!", "value");
			}
			genericInstanceHolder.Count--;
			if (genericInstanceHolder.Count == 0)
			{
				this._context.Remove(key);
			}
		}
	}
}
