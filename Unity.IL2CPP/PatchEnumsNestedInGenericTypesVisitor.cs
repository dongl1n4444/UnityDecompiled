using Mono.Cecil;
using System;
using Unity.Cecil.Visitor;

namespace Unity.IL2CPP
{
	public sealed class PatchEnumsNestedInGenericTypesVisitor : Visitor
	{
		protected override void Visit(GenericInstanceType genericInstanceType, Context context)
		{
			if (genericInstanceType.IsEnum())
			{
				ModuleDefinition module = genericInstanceType.Module;
				for (int i = 0; i < genericInstanceType.GenericArguments.Count; i++)
				{
					GenericParameter genericParameter = genericInstanceType.GenericArguments[i] as GenericParameter;
					if (genericParameter != null && genericParameter.Owner == null)
					{
						genericInstanceType.GenericArguments[i] = module.ImportReference(module.TypeSystem.Object);
					}
				}
			}
			base.Visit(genericInstanceType, context);
		}
	}
}
