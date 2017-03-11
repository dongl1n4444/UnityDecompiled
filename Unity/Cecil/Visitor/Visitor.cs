namespace Unity.Cecil.Visitor
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Reflection;

    public class Visitor
    {
        private static MethodInfo FindVisitMethodFor(System.Type type)
        {
            System.Type[] parameters = new System.Type[] { type, typeof(Context) };
            return GetMethodPortable(typeof(Unity.Cecil.Visitor.Visitor), "Visit", BindingFlags.NonPublic | BindingFlags.Instance, parameters);
        }

        private static MethodInfo GetMethodPortable(System.Type type, string name, BindingFlags flags, System.Type[] parameters) => 
            type.GetMethod(name, flags, null, parameters, new ParameterModifier[0]);

        protected virtual void Visit(ArrayType arrayType, Context context)
        {
            this.VisitTypeReference(arrayType.ElementType, context.ElementType(arrayType));
        }

        protected virtual void Visit(AssemblyDefinition assemblyDefinition, Context context)
        {
            foreach (ModuleDefinition definition in assemblyDefinition.Modules)
            {
                this.Visit(definition, context.Member(assemblyDefinition));
            }
        }

        protected virtual void Visit(ByReferenceType byReferenceType, Context context)
        {
            this.VisitTypeReference(byReferenceType.ElementType, context.ElementType(byReferenceType));
        }

        protected virtual void Visit(ExceptionHandler exceptionHandler, Context context)
        {
            if (exceptionHandler.CatchType != null)
            {
                this.VisitTypeReference(exceptionHandler.CatchType, context.ReturnType(exceptionHandler));
            }
        }

        protected virtual void Visit(Instruction instruction, Context context)
        {
            if ((instruction.Operand != null) && !(instruction.Operand is Instruction))
            {
                FieldReference operand = instruction.Operand as FieldReference;
                if (operand != null)
                {
                    this.Visit(operand, context.Operand(instruction));
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

        protected virtual void Visit(Mono.Cecil.Cil.MethodBody methodBody, Context context)
        {
            foreach (ExceptionHandler handler in methodBody.ExceptionHandlers)
            {
                this.Visit(handler, context.Member(handler));
            }
            foreach (VariableDefinition definition in methodBody.Variables)
            {
                this.Visit(definition, context.LocalVariable(methodBody));
            }
            foreach (Instruction instruction in methodBody.Instructions)
            {
                this.Visit(instruction, context);
            }
        }

        protected virtual void Visit(VariableDefinition variableDefinition, Context context)
        {
            this.VisitTypeReference(variableDefinition.VariableType, context.ReturnType(variableDefinition));
        }

        protected virtual void Visit(Mono.Cecil.CustomAttribute customAttribute, Context context)
        {
            if (customAttribute.Constructor != null)
            {
                this.Visit(customAttribute.Constructor, context.AttributeConstructor(customAttribute));
            }
            if (customAttribute.AttributeType != null)
            {
                this.VisitTypeReference(customAttribute.AttributeType, context.AttributeType(customAttribute));
            }
            foreach (CustomAttributeArgument argument in customAttribute.ConstructorArguments)
            {
                this.Visit(argument, context.AttributeArgument(customAttribute));
            }
            foreach (Mono.Cecil.CustomAttributeNamedArgument argument2 in customAttribute.Fields)
            {
                this.Visit(argument2, context.AttributeArgument(customAttribute));
            }
            foreach (Mono.Cecil.CustomAttributeNamedArgument argument3 in customAttribute.Properties)
            {
                this.Visit(argument3, context.AttributeArgument(customAttribute));
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

        protected virtual void Visit(EventDefinition eventDefinition, Context context)
        {
            this.VisitTypeReference(eventDefinition.EventType, context.ReturnType(eventDefinition));
            foreach (Mono.Cecil.CustomAttribute attribute in eventDefinition.CustomAttributes)
            {
                this.Visit(attribute, context.Attribute(eventDefinition));
            }
            this.Visit(eventDefinition.AddMethod, context.EventAdder(eventDefinition));
            this.Visit(eventDefinition.RemoveMethod, context.EventRemover(eventDefinition));
        }

        protected virtual void Visit(FieldDefinition fieldDefinition, Context context)
        {
            this.VisitTypeReference(fieldDefinition.FieldType, context.ReturnType(fieldDefinition));
            foreach (Mono.Cecil.CustomAttribute attribute in fieldDefinition.CustomAttributes)
            {
                this.Visit(attribute, context.Attribute(fieldDefinition));
            }
        }

        protected virtual void Visit(FieldReference fieldReference, Context context)
        {
            this.VisitTypeReference(fieldReference.FieldType, context.ReturnType(fieldReference));
            this.VisitTypeReference(fieldReference.DeclaringType, context.DeclaringType(fieldReference));
        }

        protected virtual void Visit(FunctionPointerType functionPointerType, Context context)
        {
        }

        protected virtual void Visit(GenericInstanceType genericInstanceType, Context context)
        {
            this.VisitTypeReference(genericInstanceType.ElementType, context.ElementType(genericInstanceType));
            foreach (TypeReference reference in genericInstanceType.GenericArguments)
            {
                this.VisitTypeReference(reference, context.GenericArgument(genericInstanceType));
            }
        }

        protected virtual void Visit(GenericParameter genericParameter, Context context)
        {
        }

        protected virtual void Visit(InterfaceImplementation interfaceImpl, Context context)
        {
            this.VisitTypeReference(interfaceImpl.InterfaceType, context.InterfaceType(interfaceImpl));
            foreach (Mono.Cecil.CustomAttribute attribute in interfaceImpl.CustomAttributes)
            {
                this.Visit(attribute, context.Attribute(interfaceImpl));
            }
        }

        protected virtual void Visit(MethodDefinition methodDefinition, Context context)
        {
            this.Visit(methodDefinition.MethodReturnType, context.ReturnType(methodDefinition));
            foreach (Mono.Cecil.CustomAttribute attribute in methodDefinition.CustomAttributes)
            {
                this.Visit(attribute, context.Attribute(methodDefinition));
            }
            foreach (GenericParameter parameter in methodDefinition.GenericParameters)
            {
                this.Visit(parameter, context.GenericParameter(methodDefinition));
            }
            foreach (ParameterDefinition definition in methodDefinition.Parameters)
            {
                this.Visit(definition, context.Parameter(methodDefinition));
            }
            if (methodDefinition.HasBody)
            {
                this.Visit(methodDefinition.Body, context.MethodBody(methodDefinition));
            }
        }

        protected virtual void Visit(MethodReference methodReference, Context context)
        {
            this.VisitTypeReference(methodReference.ReturnType, context.ReturnType(methodReference));
            this.VisitTypeReference(methodReference.DeclaringType, context.DeclaringType(methodReference));
            foreach (GenericParameter parameter in methodReference.GenericParameters)
            {
                this.VisitTypeReference(parameter, context.GenericParameter(methodReference));
            }
            foreach (ParameterDefinition definition in methodReference.Parameters)
            {
                this.Visit(definition, context.Parameter(methodReference));
            }
            GenericInstanceMethod data = methodReference as GenericInstanceMethod;
            if (data != null)
            {
                foreach (TypeReference reference in data.GenericArguments)
                {
                    this.VisitTypeReference(reference, context.GenericArgument(data));
                }
            }
        }

        protected virtual void Visit(MethodReturnType methodReturnType, Context context)
        {
            this.VisitTypeReference(methodReturnType.ReturnType, context.ReturnType(methodReturnType));
        }

        protected virtual void Visit(ModuleDefinition moduleDefinition, Context context)
        {
            foreach (TypeDefinition definition in moduleDefinition.Types)
            {
                this.Visit(definition, context.Member(moduleDefinition));
            }
        }

        protected virtual void Visit(ParameterDefinition parameterDefinition, Context context)
        {
            this.VisitTypeReference(parameterDefinition.ParameterType, context.ReturnType(parameterDefinition));
        }

        protected virtual void Visit(PinnedType pinnedType, Context context)
        {
            this.VisitTypeReference(pinnedType.ElementType, context.ElementType(pinnedType));
        }

        protected virtual void Visit(PointerType pointerType, Context context)
        {
            this.VisitTypeReference(pointerType.ElementType, context.ElementType(pointerType));
        }

        protected virtual void Visit(PropertyDefinition propertyDefinition, Context context)
        {
            this.VisitTypeReference(propertyDefinition.PropertyType, context.ReturnType(propertyDefinition));
            foreach (Mono.Cecil.CustomAttribute attribute in propertyDefinition.CustomAttributes)
            {
                this.Visit(attribute, context.Attribute(propertyDefinition));
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

        protected virtual void Visit(RequiredModifierType requiredModifierType, Context context)
        {
            this.VisitTypeReference(requiredModifierType.ElementType, context.ElementType(requiredModifierType));
        }

        protected virtual void Visit(SentinelType sentinelType, Context context)
        {
            this.VisitTypeReference(sentinelType.ElementType, context.ElementType(sentinelType));
        }

        protected virtual void Visit(TypeDefinition typeDefinition, Context context)
        {
            if (typeDefinition.BaseType != null)
            {
                this.VisitTypeReference(typeDefinition.BaseType, context.BaseType(typeDefinition));
            }
            foreach (Mono.Cecil.CustomAttribute attribute in typeDefinition.CustomAttributes)
            {
                this.Visit(attribute, context.Attribute(typeDefinition));
            }
            foreach (InterfaceImplementation implementation in typeDefinition.Interfaces)
            {
                this.Visit(implementation, context.Interface(typeDefinition));
            }
            foreach (GenericParameter parameter in typeDefinition.GenericParameters)
            {
                this.Visit(parameter, context.GenericParameter(context));
            }
            foreach (PropertyDefinition definition in typeDefinition.Properties)
            {
                this.Visit(definition, context.Member(typeDefinition));
            }
            foreach (FieldDefinition definition2 in typeDefinition.Fields)
            {
                this.Visit(definition2, context.Member(typeDefinition));
            }
            foreach (MethodDefinition definition3 in typeDefinition.Methods)
            {
                this.Visit(definition3, context.Member(typeDefinition));
            }
            foreach (EventDefinition definition4 in typeDefinition.Events)
            {
                this.Visit(definition4, context.Member(typeDefinition));
            }
            foreach (TypeDefinition definition5 in typeDefinition.NestedTypes)
            {
                this.Visit(definition5, context.NestedType(typeDefinition));
            }
        }

        protected virtual void Visit(TypeReference typeReference, Context context)
        {
            if (typeReference.DeclaringType != null)
            {
                this.VisitTypeReference(typeReference.DeclaringType, context.DeclaringType(typeReference));
            }
        }

        public void Visit<T>(T node, Context context)
        {
            System.Type type = node.GetType();
            MethodInfo info = FindVisitMethodFor(type);
            if (info == null)
            {
                throw new NotImplementedException("Invalid Cecil Definition " + type.Name);
            }
            object[] parameters = new object[] { node, context };
            info.Invoke(this, parameters);
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
    }
}

