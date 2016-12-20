namespace Unity.Cecil.Visitor
{
    using Mono.Cecil;
    using System;
    using System.Runtime.InteropServices;

    public class Context
    {
        private readonly object _data;
        private readonly Context _parent;
        private readonly Unity.Cecil.Visitor.Role _role;

        private Context(Unity.Cecil.Visitor.Role role, object data, [Optional, DefaultParameterValue(null)] Context parent)
        {
            this._role = role;
            this._data = data;
            this._parent = parent;
        }

        public Context Attribute(object data)
        {
            return new Context(Unity.Cecil.Visitor.Role.Attribute, data, this);
        }

        public Context AttributeArgument(object data)
        {
            return new Context(Unity.Cecil.Visitor.Role.AttributeArgument, data, this);
        }

        public Context AttributeArgumentType(object data)
        {
            return new Context(Unity.Cecil.Visitor.Role.AttributeArgumentType, data, this);
        }

        public Context AttributeConstructor(object data)
        {
            return new Context(Unity.Cecil.Visitor.Role.AttributeConstructor, data, this);
        }

        public Context AttributeType(object data)
        {
            return new Context(Unity.Cecil.Visitor.Role.AttributeType, data, this);
        }

        public Context BaseType(TypeDefinition data)
        {
            return new Context(Unity.Cecil.Visitor.Role.BaseType, data, this);
        }

        public Context DeclaringType(object data)
        {
            return new Context(Unity.Cecil.Visitor.Role.DeclaringType, data, this);
        }

        public Context ElementType(object data)
        {
            return new Context(Unity.Cecil.Visitor.Role.ElementType, data, this);
        }

        public Context EventAdder(object data)
        {
            return new Context(Unity.Cecil.Visitor.Role.EventAdder, data, this);
        }

        public Context EventRemover(object data)
        {
            return new Context(Unity.Cecil.Visitor.Role.EventRemover, data, this);
        }

        public Context GenericArgument(object data)
        {
            return new Context(Unity.Cecil.Visitor.Role.GenericArgument, data, this);
        }

        public Context GenericParameter(object data)
        {
            return new Context(Unity.Cecil.Visitor.Role.GenericParameter, data, this);
        }

        public Context Getter(object data)
        {
            return new Context(Unity.Cecil.Visitor.Role.Getter, data, this);
        }

        public Context Interface(TypeDefinition data)
        {
            return new Context(Unity.Cecil.Visitor.Role.Interface, data, this);
        }

        public Context InterfaceType(InterfaceImplementation data)
        {
            return new Context(Unity.Cecil.Visitor.Role.InterfaceType, data, this);
        }

        public Context LocalVariable(object data)
        {
            return new Context(Unity.Cecil.Visitor.Role.LocalVariable, data, this);
        }

        public Context Member(object data)
        {
            return new Context(Unity.Cecil.Visitor.Role.Member, data, this);
        }

        public Context MethodBody(object data)
        {
            return new Context(Unity.Cecil.Visitor.Role.MethodBody, data, this);
        }

        public Context NestedType(TypeDefinition data)
        {
            return new Context(Unity.Cecil.Visitor.Role.NestedType, data, this);
        }

        public Context Operand(object data)
        {
            return new Context(Unity.Cecil.Visitor.Role.Operand, data, this);
        }

        public Context Parameter(object data)
        {
            return new Context(Unity.Cecil.Visitor.Role.Parameter, data, this);
        }

        public Context ReturnType(object data)
        {
            return new Context(Unity.Cecil.Visitor.Role.ReturnType, data, this);
        }

        public Context Setter(object data)
        {
            return new Context(Unity.Cecil.Visitor.Role.Setter, data, this);
        }

        public object Data
        {
            get
            {
                return this._data;
            }
        }

        public static Context None
        {
            get
            {
                return new Context(Unity.Cecil.Visitor.Role.None, null, null);
            }
        }

        public Context Parent
        {
            get
            {
                return this._parent;
            }
        }

        public Unity.Cecil.Visitor.Role Role
        {
            get
            {
                return this._role;
            }
        }
    }
}

