using System;

namespace Unity.Cecil.Visitor
{
	public enum Role
	{
		None,
		EventAdder,
		EventRemover,
		Member,
		BaseType,
		NestedType,
		Interface,
		InterfaceType,
		ReturnType,
		GenericParameter,
		Getter,
		Setter,
		ElementType,
		GenericArgument,
		Parameter,
		MethodBody,
		DeclaringType,
		Attribute,
		AttributeConstructor,
		AttributeType,
		AttributeArgument,
		AttributeArgumentType,
		LocalVariable,
		Operand
	}
}
