namespace Unity.IL2CPP.StringLiterals
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP.IoC;

    public class StringLiteralCollector
    {
        [Inject]
        public static IStringLiteralProvider StringLiteralProvider;

        public void Process(AssemblyDefinition usedAssembly)
        {
            foreach (TypeDefinition definition in usedAssembly.MainModule.Types)
            {
                ProcessType(definition);
            }
        }

        private static void ProcessMethods(IEnumerable<MethodDefinition> methods)
        {
            foreach (MethodDefinition definition in methods)
            {
                if (definition.HasBody)
                {
                    foreach (Instruction instruction in definition.Body.Instructions)
                    {
                        if (instruction.OpCode == OpCodes.Ldstr)
                        {
                            StringLiteralProvider.Add((string) instruction.Operand);
                        }
                    }
                }
            }
        }

        private static void ProcessType(TypeDefinition type)
        {
            ProcessMethods(type.Methods);
            foreach (TypeDefinition definition in type.NestedTypes)
            {
                ProcessType(definition);
            }
        }
    }
}

