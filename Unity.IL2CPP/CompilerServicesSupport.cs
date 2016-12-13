using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.IL2CPP
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct CompilerServicesSupport
	{
		private const string SetOptionsAttributeFullName = "Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute";

		public static bool HasNullChecksSupportEnabled(MethodDefinition methodDefinition, bool globalValue)
		{
			return CompilerServicesSupport.HasOptionEnabled(methodDefinition, Option.NullChecks, globalValue);
		}

		public static bool HasArrayBoundsChecksSupportEnabled(MethodDefinition methodDefinition, bool globalValue)
		{
			return CompilerServicesSupport.HasOptionEnabled(methodDefinition, Option.ArrayBoundsChecks, globalValue);
		}

		public static bool HasDivideByZeroChecksSupportEnabled(MethodDefinition methodDefinition, bool globalValue)
		{
			return CompilerServicesSupport.HasOptionEnabled(methodDefinition, Option.DivideByZeroChecks, globalValue);
		}

		private static bool HasOptionEnabled(IMemberDefinition methodDefinition, Option option, bool globalValue)
		{
			bool flag = globalValue;
			bool result;
			if (CompilerServicesSupport.GetBooleanOptionValue(methodDefinition.CustomAttributes, option, ref flag))
			{
				result = flag;
			}
			else
			{
				TypeDefinition declaringType = methodDefinition.DeclaringType;
				foreach (PropertyDefinition current in declaringType.Properties)
				{
					if (current.GetMethod == methodDefinition || current.SetMethod == methodDefinition)
					{
						if (CompilerServicesSupport.GetBooleanOptionValue(current.CustomAttributes, option, ref flag))
						{
							result = flag;
							return result;
						}
					}
				}
				if (CompilerServicesSupport.GetBooleanOptionValue(declaringType.CustomAttributes, option, ref flag))
				{
					result = flag;
				}
				else
				{
					result = globalValue;
				}
			}
			return result;
		}

		private static bool GetBooleanOptionValue(IEnumerable<CustomAttribute> attributes, Option option, ref bool result)
		{
			return CompilerServicesSupport.GetOptionValue<bool>(attributes, option, ref result);
		}

		private static bool GetOptionValue<T>(IEnumerable<CustomAttribute> attributes, Option option, ref T result)
		{
			bool result2;
			foreach (CustomAttribute current in attributes)
			{
				if (!(current.AttributeType.FullName != "Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute"))
				{
					Collection<CustomAttributeArgument> constructorArguments = current.ConstructorArguments;
					if ((int)constructorArguments[0].Value == (int)option)
					{
						try
						{
							result = (T)((object)((CustomAttributeArgument)constructorArguments[1].Value).Value);
						}
						catch (InvalidCastException)
						{
							continue;
						}
						result2 = true;
						return result2;
					}
				}
			}
			result2 = false;
			return result2;
		}
	}
}
