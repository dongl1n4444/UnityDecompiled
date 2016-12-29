namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using Mono.Collections.Generic;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP.CompilerServices;

    [StructLayout(LayoutKind.Sequential, Size=1)]
    internal struct CompilerServicesSupport
    {
        private const string SetOptionsAttributeFullName = "Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute";
        public static bool HasNullChecksSupportEnabled(MethodDefinition methodDefinition, bool globalValue) => 
            HasOptionEnabled(methodDefinition, Option.NullChecks, globalValue);

        public static bool HasArrayBoundsChecksSupportEnabled(MethodDefinition methodDefinition, bool globalValue) => 
            HasOptionEnabled(methodDefinition, Option.ArrayBoundsChecks, globalValue);

        public static bool HasDivideByZeroChecksSupportEnabled(MethodDefinition methodDefinition, bool globalValue) => 
            HasOptionEnabled(methodDefinition, Option.DivideByZeroChecks, globalValue);

        private static bool HasOptionEnabled(IMemberDefinition methodDefinition, Option option, bool globalValue)
        {
            bool result = globalValue;
            if (GetBooleanOptionValue(methodDefinition.CustomAttributes, option, ref result))
            {
                return result;
            }
            TypeDefinition declaringType = methodDefinition.DeclaringType;
            foreach (PropertyDefinition definition2 in declaringType.Properties)
            {
                if (((definition2.GetMethod == methodDefinition) || (definition2.SetMethod == methodDefinition)) && GetBooleanOptionValue(definition2.CustomAttributes, option, ref result))
                {
                    return result;
                }
            }
            if (GetBooleanOptionValue(declaringType.CustomAttributes, option, ref result))
            {
                return result;
            }
            return globalValue;
        }

        private static bool GetBooleanOptionValue(IEnumerable<CustomAttribute> attributes, Option option, ref bool result) => 
            GetOptionValue<bool>(attributes, option, ref result);

        private static bool GetOptionValue<T>(IEnumerable<CustomAttribute> attributes, Option option, ref T result)
        {
            foreach (CustomAttribute attribute in attributes)
            {
                if (attribute.AttributeType.FullName == "Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute")
                {
                    Collection<CustomAttributeArgument> constructorArguments = attribute.ConstructorArguments;
                    CustomAttributeArgument argument = constructorArguments[0];
                    if (((int) argument.Value) == option)
                    {
                        try
                        {
                            CustomAttributeArgument argument2 = constructorArguments[1];
                            CustomAttributeArgument argument3 = (CustomAttributeArgument) argument2.Value;
                            result = (T) argument3.Value;
                        }
                        catch (InvalidCastException)
                        {
                            continue;
                        }
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

