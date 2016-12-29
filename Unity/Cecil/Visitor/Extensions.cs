namespace Unity.Cecil.Visitor
{
    using Mono.Cecil;
    using System;
    using System.Runtime.CompilerServices;

    public static class Extensions
    {
        public static void Accept(this ArrayType arrayType, Unity.Cecil.Visitor.Visitor visitor)
        {
            arrayType.DoAccept<ArrayType>(visitor);
        }

        public static void Accept(this AssemblyDefinition assemblyDefinition, Unity.Cecil.Visitor.Visitor visitor)
        {
            assemblyDefinition.DoAccept<AssemblyDefinition>(visitor);
        }

        public static void Accept(this FieldDefinition fieldDefinition, Unity.Cecil.Visitor.Visitor visitor)
        {
            fieldDefinition.DoAccept<FieldDefinition>(visitor);
        }

        public static void Accept(this GenericInstanceType genericInstanceType, Unity.Cecil.Visitor.Visitor visitor)
        {
            genericInstanceType.DoAccept<GenericInstanceType>(visitor);
        }

        public static void Accept(this MethodDefinition methodDefinition, Unity.Cecil.Visitor.Visitor visitor)
        {
            methodDefinition.DoAccept<MethodDefinition>(visitor);
        }

        public static void Accept(this ModuleDefinition moduleDefinition, Unity.Cecil.Visitor.Visitor visitor)
        {
            moduleDefinition.DoAccept<ModuleDefinition>(visitor);
        }

        public static void Accept(this PointerType pointerType, Unity.Cecil.Visitor.Visitor visitor)
        {
            pointerType.DoAccept<PointerType>(visitor);
        }

        public static void Accept(this PropertyDefinition propertyDefinition, Unity.Cecil.Visitor.Visitor visitor)
        {
            propertyDefinition.DoAccept<PropertyDefinition>(visitor);
        }

        public static void Accept(this TypeDefinition typeDefinition, Unity.Cecil.Visitor.Visitor visitor)
        {
            typeDefinition.DoAccept<TypeDefinition>(visitor);
        }

        private static void DoAccept<T>(this T definition, Unity.Cecil.Visitor.Visitor visitor) where T: class
        {
            if ((visitor != null) && (definition != null))
            {
                visitor.Visit<T>(definition, Context.None);
            }
        }
    }
}

