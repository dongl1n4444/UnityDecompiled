using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Reflection;

namespace Unity.Cecil.Visitor
{
	public class Visitor
	{
		public void Visit<T>(T node, Context context)
		{
			Type type = node.GetType();
			MethodInfo methodInfo = Visitor.FindVisitMethodFor(type);
			if (methodInfo == null)
			{
				throw new NotImplementedException("Invalid Cecil Definition " + type.Name);
			}
			methodInfo.Invoke(this, new object[]
			{
				node,
				context
			});
		}

		private static MethodInfo FindVisitMethodFor(Type type)
		{
			return typeof(Visitor).GetMethod("Visit", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[]
			{
				type,
				typeof(Context)
			}, new ParameterModifier[0]);
		}

		protected virtual void Visit(AssemblyDefinition assemblyDefinition, Context context)
		{
			foreach (ModuleDefinition current in assemblyDefinition.Modules)
			{
				this.Visit(current, context.Member(assemblyDefinition));
			}
		}

		protected virtual void Visit(ModuleDefinition moduleDefinition, Context context)
		{
			foreach (TypeDefinition current in moduleDefinition.Types)
			{
				this.Visit(current, context.Member(moduleDefinition));
			}
		}

		protected virtual void Visit(TypeDefinition typeDefinition, Context context)
		{
			if (typeDefinition.BaseType != null)
			{
				this.VisitTypeReference(typeDefinition.BaseType, context.BaseType(typeDefinition));
			}
			foreach (CustomAttribute current in typeDefinition.CustomAttributes)
			{
				this.Visit(current, context.Attribute(typeDefinition));
			}
			foreach (InterfaceImplementation current2 in typeDefinition.Interfaces)
			{
				this.Visit(current2, context.Interface(typeDefinition));
			}
			foreach (GenericParameter current3 in typeDefinition.GenericParameters)
			{
				this.Visit(current3, context.GenericParameter(context));
			}
			foreach (PropertyDefinition current4 in typeDefinition.Properties)
			{
				this.Visit(current4, context.Member(typeDefinition));
			}
			foreach (FieldDefinition current5 in typeDefinition.Fields)
			{
				this.Visit(current5, context.Member(typeDefinition));
			}
			foreach (MethodDefinition current6 in typeDefinition.Methods)
			{
				this.Visit(current6, context.Member(typeDefinition));
			}
			foreach (EventDefinition current7 in typeDefinition.Events)
			{
				this.Visit(current7, context.Member(typeDefinition));
			}
			foreach (TypeDefinition current8 in typeDefinition.NestedTypes)
			{
				this.Visit(current8, context.NestedType(typeDefinition));
			}
		}

		protected virtual void Visit(EventDefinition eventDefinition, Context context)
		{
			this.VisitTypeReference(eventDefinition.EventType, context.ReturnType(eventDefinition));
			foreach (CustomAttribute current in eventDefinition.CustomAttributes)
			{
				this.Visit(current, context.Attribute(eventDefinition));
			}
			this.Visit(eventDefinition.AddMethod, context.EventAdder(eventDefinition));
			this.Visit(eventDefinition.RemoveMethod, context.EventRemover(eventDefinition));
		}

		protected virtual void Visit(FieldDefinition fieldDefinition, Context context)
		{
			this.VisitTypeReference(fieldDefinition.FieldType, context.ReturnType(fieldDefinition));
			foreach (CustomAttribute current in fieldDefinition.CustomAttributes)
			{
				this.Visit(current, context.Attribute(fieldDefinition));
			}
		}

		protected virtual void Visit(PropertyDefinition propertyDefinition, Context context)
		{
			this.VisitTypeReference(propertyDefinition.PropertyType, context.ReturnType(propertyDefinition));
			foreach (CustomAttribute current in propertyDefinition.CustomAttributes)
			{
				this.Visit(current, context.Attribute(propertyDefinition));
			}
			if (propertyDefinition.GetMethod != null)
			{
				this.Visit(propertyDefinition.GetMethod, context.Getter(propertyDefinition));
			}
			if (propertyDefinition.SetMethod != null)
			{
				this.Visit(propertyDefinition.SetMethod, context.Setter(propertyDefinition));
			}
		}

		protected virtual void Visit(MethodDefinition methodDefinition, Context context)
		{
			this.VisitTypeReference(methodDefinition.ReturnType, context.ReturnType(methodDefinition));
			foreach (CustomAttribute current in methodDefinition.CustomAttributes)
			{
				this.Visit(current, context.Attribute(methodDefinition));
			}
			foreach (GenericParameter current2 in methodDefinition.GenericParameters)
			{
				this.Visit(current2, context.GenericParameter(methodDefinition));
			}
			foreach (ParameterDefinition current3 in methodDefinition.Parameters)
			{
				this.Visit(current3, context.Parameter(methodDefinition));
			}
			if (methodDefinition.HasBody)
			{
				this.Visit(methodDefinition.Body, context.MethodBody(methodDefinition));
			}
		}

		protected virtual void Visit(CustomAttribute customAttribute, Context context)
		{
			if (customAttribute.Constructor != null)
			{
				this.Visit(customAttribute.Constructor, context.AttributeConstructor(customAttribute));
			}
			if (customAttribute.AttributeType != null)
			{
				this.VisitTypeReference(customAttribute.AttributeType, context.AttributeType(customAttribute));
			}
			foreach (CustomAttributeArgument current in customAttribute.ConstructorArguments)
			{
				this.Visit(current, context.AttributeArgument(customAttribute));
			}
			foreach (Mono.Cecil.CustomAttributeNamedArgument current2 in customAttribute.Fields)
			{
				this.Visit(current2, context.AttributeArgument(customAttribute));
			}
			foreach (Mono.Cecil.CustomAttributeNamedArgument current3 in customAttribute.Properties)
			{
				this.Visit(current3, context.AttributeArgument(customAttribute));
			}
		}

		protected virtual void Visit(CustomAttributeArgument customAttributeArgument, Context context)
		{
			this.VisitTypeReference(customAttributeArgument.Type, context.AttributeArgumentType(customAttributeArgument));
		}

		protected virtual void Visit(Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument, Context context)
		{
			this.Visit(customAttributeNamedArgument.Argument, context);
		}

		protected virtual void Visit(FieldReference fieldReference, Context context)
		{
			this.VisitTypeReference(fieldReference.FieldType, context.ReturnType(fieldReference));
			this.VisitTypeReference(fieldReference.DeclaringType, context.DeclaringType(fieldReference));
		}

		protected virtual void Visit(MethodReference methodReference, Context context)
		{
			this.VisitTypeReference(methodReference.ReturnType, context.ReturnType(methodReference));
			this.VisitTypeReference(methodReference.DeclaringType, context.DeclaringType(methodReference));
			foreach (GenericParameter current in methodReference.GenericParameters)
			{
				this.VisitTypeReference(current, context.GenericParameter(methodReference));
			}
			foreach (ParameterDefinition current2 in methodReference.Parameters)
			{
				this.Visit(current2, context.Parameter(methodReference));
			}
			GenericInstanceMethod genericInstanceMethod = methodReference as GenericInstanceMethod;
			if (genericInstanceMethod != null)
			{
				foreach (TypeReference current3 in genericInstanceMethod.GenericArguments)
				{
					this.VisitTypeReference(current3, context.GenericArgument(genericInstanceMethod));
				}
			}
		}

		protected virtual void Visit(TypeReference typeReference, Context context)
		{
			if (typeReference.DeclaringType != null)
			{
				this.VisitTypeReference(typeReference.DeclaringType, context.DeclaringType(typeReference));
			}
		}

		protected virtual void Visit(ParameterDefinition parameterDefinition, Context context)
		{
			this.VisitTypeReference(parameterDefinition.ParameterType, context.ReturnType(parameterDefinition));
		}

		protected virtual void Visit(Mono.Cecil.Cil.MethodBody methodBody, Context context)
		{
			foreach (ExceptionHandler current in methodBody.ExceptionHandlers)
			{
				this.Visit(current, context.Member(current));
			}
			foreach (VariableDefinition current2 in methodBody.Variables)
			{
				this.Visit(current2, context.LocalVariable(methodBody));
			}
			foreach (Instruction current3 in methodBody.Instructions)
			{
				this.Visit(current3, context);
			}
		}

		protected virtual void Visit(VariableDefinition variableDefinition, Context context)
		{
			this.VisitTypeReference(variableDefinition.VariableType, context.ReturnType(variableDefinition));
		}

		protected virtual void Visit(Instruction instruction, Context context)
		{
			if (instruction.Operand != null)
			{
				if (!(instruction.Operand is Instruction))
				{
					FieldReference fieldReference = instruction.Operand as FieldReference;
					if (fieldReference != null)
					{
						this.Visit(fieldReference, context.Operand(instruction));
					}
					else
					{
						MethodReference methodReference = instruction.Operand as MethodReference;
						if (methodReference != null)
						{
							this.Visit(methodReference, context.Operand(instruction));
						}
						else
						{
							TypeReference typeReference = instruction.Operand as TypeReference;
							if (typeReference != null)
							{
								this.VisitTypeReference(typeReference, context.Operand(instruction));
							}
							else
							{
								ParameterDefinition parameterDefinition = instruction.Operand as ParameterDefinition;
								if (parameterDefinition != null)
								{
									this.Visit(parameterDefinition, context.Operand(instruction));
								}
								else
								{
									VariableDefinition variableDefinition = instruction.Operand as VariableDefinition;
									if (variableDefinition != null)
									{
										this.Visit(variableDefinition, context.Operand(instruction));
									}
								}
							}
						}
					}
				}
			}
		}

		protected virtual void Visit(ExceptionHandler exceptionHandler, Context context)
		{
			if (exceptionHandler.CatchType != null)
			{
				this.VisitTypeReference(exceptionHandler.CatchType, context.ReturnType(exceptionHandler));
			}
		}

		protected virtual void Visit(GenericParameter genericParameter, Context context)
		{
		}

		protected virtual void Visit(ArrayType arrayType, Context context)
		{
			this.VisitTypeReference(arrayType.ElementType, context.ElementType(arrayType));
		}

		protected virtual void Visit(PointerType pointerType, Context context)
		{
			this.VisitTypeReference(pointerType.ElementType, context.ElementType(pointerType));
		}

		protected virtual void Visit(ByReferenceType byReferenceType, Context context)
		{
			this.VisitTypeReference(byReferenceType.ElementType, context.ElementType(byReferenceType));
		}

		protected virtual void Visit(PinnedType pinnedType, Context context)
		{
			this.VisitTypeReference(pinnedType.ElementType, context.ElementType(pinnedType));
		}

		protected virtual void Visit(SentinelType sentinelType, Context context)
		{
			this.VisitTypeReference(sentinelType.ElementType, context.ElementType(sentinelType));
		}

		protected virtual void Visit(FunctionPointerType functionPointerType, Context context)
		{
		}

		protected virtual void Visit(RequiredModifierType requiredModifierType, Context context)
		{
			this.VisitTypeReference(requiredModifierType.ElementType, context.ElementType(requiredModifierType));
		}

		protected virtual void Visit(GenericInstanceType genericInstanceType, Context context)
		{
			this.VisitTypeReference(genericInstanceType.ElementType, context.ElementType(genericInstanceType));
			foreach (TypeReference current in genericInstanceType.GenericArguments)
			{
				this.VisitTypeReference(current, context.GenericArgument(genericInstanceType));
			}
		}

		protected void VisitTypeReference(TypeReference typeReference, Context context)
		{
			GenericParameter genericParameter = typeReference as GenericParameter;
			if (genericParameter != null)
			{
				this.Visit(genericParameter, context);
			}
			else
			{
				ArrayType arrayType = typeReference as ArrayType;
				if (arrayType != null)
				{
					this.Visit(arrayType, context);
				}
				else
				{
					PointerType pointerType = typeReference as PointerType;
					if (pointerType != null)
					{
						this.Visit(pointerType, context);
					}
					else
					{
						ByReferenceType byReferenceType = typeReference as ByReferenceType;
						if (byReferenceType != null)
						{
							this.Visit(byReferenceType, context);
						}
						else
						{
							FunctionPointerType functionPointerType = typeReference as FunctionPointerType;
							if (functionPointerType != null)
							{
								this.Visit(functionPointerType, context);
							}
							else
							{
								PinnedType pinnedType = typeReference as PinnedType;
								if (pinnedType != null)
								{
									this.Visit(pinnedType, context);
								}
								else
								{
									SentinelType sentinelType = typeReference as SentinelType;
									if (sentinelType != null)
									{
										this.Visit(sentinelType, context);
									}
									else
									{
										GenericInstanceType genericInstanceType = typeReference as GenericInstanceType;
										if (genericInstanceType != null)
										{
											this.Visit(genericInstanceType, context);
										}
										else
										{
											RequiredModifierType requiredModifierType = typeReference as RequiredModifierType;
											if (requiredModifierType != null)
											{
												this.Visit(requiredModifierType, context);
											}
											else
											{
												this.Visit(typeReference, context);
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

		protected virtual void Visit(InterfaceImplementation interfaceImpl, Context context)
		{
			this.VisitTypeReference(interfaceImpl.InterfaceType, context.InterfaceType(interfaceImpl));
			foreach (CustomAttribute current in interfaceImpl.CustomAttributes)
			{
				this.Visit(current, context.Attribute(interfaceImpl));
			}
		}
	}
}
