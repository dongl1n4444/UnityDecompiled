using Mono.Cecil;
using System;

namespace Unity.Cecil.Visitor
{
	public class Context
	{
		private readonly Role _role;

		private readonly object _data;

		private readonly Context _parent;

		public static Context None
		{
			get
			{
				return new Context(Role.None, null, null);
			}
		}

		public Role Role
		{
			get
			{
				return this._role;
			}
		}

		public object Data
		{
			get
			{
				return this._data;
			}
		}

		public Context Parent
		{
			get
			{
				return this._parent;
			}
		}

		private Context(Role role, object data, Context parent = null)
		{
			this._role = role;
			this._data = data;
			this._parent = parent;
		}

		public Context Member(object data)
		{
			return new Context(Role.Member, data, this);
		}

		public Context NestedType(TypeDefinition data)
		{
			return new Context(Role.NestedType, data, this);
		}

		public Context BaseType(TypeDefinition data)
		{
			return new Context(Role.BaseType, data, this);
		}

		public Context Interface(TypeDefinition data)
		{
			return new Context(Role.Interface, data, this);
		}

		public Context InterfaceType(InterfaceImplementation data)
		{
			return new Context(Role.InterfaceType, data, this);
		}

		public Context ReturnType(object data)
		{
			return new Context(Role.ReturnType, data, this);
		}

		public Context GenericParameter(object data)
		{
			return new Context(Role.GenericParameter, data, this);
		}

		public Context Getter(object data)
		{
			return new Context(Role.Getter, data, this);
		}

		public Context Setter(object data)
		{
			return new Context(Role.Setter, data, this);
		}

		public Context EventAdder(object data)
		{
			return new Context(Role.EventAdder, data, this);
		}

		public Context EventRemover(object data)
		{
			return new Context(Role.EventRemover, data, this);
		}

		public Context ElementType(object data)
		{
			return new Context(Role.ElementType, data, this);
		}

		public Context GenericArgument(object data)
		{
			return new Context(Role.GenericArgument, data, this);
		}

		public Context Parameter(object data)
		{
			return new Context(Role.Parameter, data, this);
		}

		public Context MethodBody(object data)
		{
			return new Context(Role.MethodBody, data, this);
		}

		public Context DeclaringType(object data)
		{
			return new Context(Role.DeclaringType, data, this);
		}

		public Context Attribute(object data)
		{
			return new Context(Role.Attribute, data, this);
		}

		public Context AttributeConstructor(object data)
		{
			return new Context(Role.AttributeConstructor, data, this);
		}

		public Context AttributeType(object data)
		{
			return new Context(Role.AttributeType, data, this);
		}

		public Context AttributeArgument(object data)
		{
			return new Context(Role.AttributeArgument, data, this);
		}

		public Context AttributeArgumentType(object data)
		{
			return new Context(Role.AttributeArgumentType, data, this);
		}

		public Context LocalVariable(object data)
		{
			return new Context(Role.LocalVariable, data, this);
		}

		public Context Operand(object data)
		{
			return new Context(Role.Operand, data, this);
		}
	}
}
