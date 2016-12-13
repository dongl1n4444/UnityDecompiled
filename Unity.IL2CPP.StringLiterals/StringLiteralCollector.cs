using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.IoC;

namespace Unity.IL2CPP.StringLiterals
{
	public class StringLiteralCollector
	{
		[Inject]
		public static IStringLiteralProvider StringLiteralProvider;

		public void Process(AssemblyDefinition usedAssembly)
		{
			foreach (TypeDefinition current in usedAssembly.MainModule.Types)
			{
				StringLiteralCollector.ProcessType(current);
			}
		}

		private static void ProcessType(TypeDefinition type)
		{
			StringLiteralCollector.ProcessMethods(type.Methods);
			foreach (TypeDefinition current in type.NestedTypes)
			{
				StringLiteralCollector.ProcessType(current);
			}
		}

		private static void ProcessMethods(IEnumerable<MethodDefinition> methods)
		{
			foreach (MethodDefinition current in methods)
			{
				if (current.HasBody)
				{
					foreach (Instruction current2 in current.Body.Instructions)
					{
						if (current2.OpCode == OpCodes.Ldstr)
						{
							StringLiteralCollector.StringLiteralProvider.Add((string)current2.Operand);
						}
					}
				}
			}
		}
	}
}
