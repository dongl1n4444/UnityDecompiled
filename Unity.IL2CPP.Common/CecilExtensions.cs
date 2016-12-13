using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Unity.IL2CPP.Common
{
	public static class CecilExtensions
	{
		public static IEnumerable<TypeDefinition> AllDefinedTypes(this AssemblyDefinition assemblyDefinition)
		{
			return assemblyDefinition.Modules.SelectMany((ModuleDefinition m) => m.AllDefinedTypes());
		}

		[DebuggerHidden]
		public static IEnumerable<TypeDefinition> AllDefinedTypes(this ModuleDefinition moduleDefinition)
		{
			CecilExtensions.<AllDefinedTypes>c__Iterator0 <AllDefinedTypes>c__Iterator = new CecilExtensions.<AllDefinedTypes>c__Iterator0();
			<AllDefinedTypes>c__Iterator.moduleDefinition = moduleDefinition;
			CecilExtensions.<AllDefinedTypes>c__Iterator0 expr_0E = <AllDefinedTypes>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		[DebuggerHidden]
		public static IEnumerable<TypeDefinition> AllDefinedTypes(this TypeDefinition typeDefinition)
		{
			CecilExtensions.<AllDefinedTypes>c__Iterator1 <AllDefinedTypes>c__Iterator = new CecilExtensions.<AllDefinedTypes>c__Iterator1();
			<AllDefinedTypes>c__Iterator.typeDefinition = typeDefinition;
			CecilExtensions.<AllDefinedTypes>c__Iterator1 expr_0E = <AllDefinedTypes>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
