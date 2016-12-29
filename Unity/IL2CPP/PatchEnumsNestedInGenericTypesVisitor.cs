namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using Unity.Cecil.Visitor;

    public sealed class PatchEnumsNestedInGenericTypesVisitor : Unity.Cecil.Visitor.Visitor
    {
        protected override void Visit(GenericInstanceType genericInstanceType, Unity.Cecil.Visitor.Context context)
        {
            if (genericInstanceType.IsEnum())
            {
                ModuleDefinition module = genericInstanceType.Module;
                for (int i = 0; i < genericInstanceType.GenericArguments.Count; i++)
                {
                    GenericParameter parameter = genericInstanceType.GenericArguments[i] as GenericParameter;
                    if ((parameter != null) && (parameter.Owner == null))
                    {
                        genericInstanceType.GenericArguments[i] = module.ImportReference(module.TypeSystem.Object);
                    }
                }
            }
            base.Visit(genericInstanceType, context);
        }
    }
}

