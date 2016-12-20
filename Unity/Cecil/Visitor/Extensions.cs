namespace Unity.Cecil.Visitor
{
    using Mono.Cecil;
    using System;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class Extensions
    {
        [Extension]
        public static void Accept(ArrayType arrayType, Unity.Cecil.Visitor.Visitor visitor)
        {
            DoAccept<ArrayType>(arrayType, visitor);
        }

        [Extension]
        public static void Accept(AssemblyDefinition assemblyDefinition, Unity.Cecil.Visitor.Visitor visitor)
        {
            DoAccept<AssemblyDefinition>(assemblyDefinition, visitor);
        }

        [Extension]
        public static void Accept(FieldDefinition fieldDefinition, Unity.Cecil.Visitor.Visitor visitor)
        {
            DoAccept<FieldDefinition>(fieldDefinition, visitor);
        }

        [Extension]
        public static void Accept(GenericInstanceType genericInstanceType, Unity.Cecil.Visitor.Visitor visitor)
        {
            DoAccept<GenericInstanceType>(genericInstanceType, visitor);
        }

        [Extension]
        public static void Accept(MethodDefinition methodDefinition, Unity.Cecil.Visitor.Visitor visitor)
        {
            DoAccept<MethodDefinition>(methodDefinition, visitor);
        }

        [Extension]
        public static void Accept(ModuleDefinition moduleDefinition, Unity.Cecil.Visitor.Visitor visitor)
        {
            DoAccept<ModuleDefinition>(moduleDefinition, visitor);
        }

        [Extension]
        public static void Accept(PointerType pointerType, Unity.Cecil.Visitor.Visitor visitor)
        {
            DoAccept<PointerType>(pointerType, visitor);
        }

        [Extension]
        public static void Accept(PropertyDefinition propertyDefinition, Unity.Cecil.Visitor.Visitor visitor)
        {
            DoAccept<PropertyDefinition>(propertyDefinition, visitor);
        }

        [Extension]
        public static void Accept(TypeDefinition typeDefinition, Unity.Cecil.Visitor.Visitor visitor)
        {
            DoAccept<TypeDefinition>(typeDefinition, visitor);
        }

        [Extension]
        private static void DoAccept<T>(T definition, Unity.Cecil.Visitor.Visitor visitor) where T: class
        {
            if ((visitor != null) && (definition != null))
            {
                visitor.Visit<T>(definition, Context.None);
            }
        }
    }
}

