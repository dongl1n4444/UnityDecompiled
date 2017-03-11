namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class IntrinsicRemap
    {
        [CompilerGenerated]
        private static Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>> <>f__mg$cache0;
        [CompilerGenerated]
        private static Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>> <>f__mg$cache1;
        [CompilerGenerated]
        private static Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>> <>f__mg$cache2;
        [CompilerGenerated]
        private static Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>> <>f__mg$cache3;
        [CompilerGenerated]
        private static Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>> <>f__mg$cache4;
        private const string GetCurrentMethodSignature = "System.Reflection.MethodBase System.Reflection.MethodBase::GetCurrentMethod()";
        private const string GetExecutingAssemblySignature = "System.Reflection.Assembly System.Reflection.Assembly::GetExecutingAssembly()";
        private const string GetIUnknownForObjectSignatureObject = "System.IntPtr System.Runtime.InteropServices.Marshal::GetIUnknownForObject(System.Object)";
        private const string GetTypeStringSignature = "System.Type System.Type::GetType(System.String)";
        private const string GetTypeStringSignatureBoolean = "System.Type System.Type::GetType(System.String,System.Boolean)";
        private const string GetTypeStringSignatureBooleanBoolean = "System.Type System.Type::GetType(System.String,System.Boolean,System.Boolean)";
        private static readonly Dictionary<string, string> MethodNameMapping;
        private static readonly Dictionary<string, Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>>> MethodNameMappingCustomArguments;
        [Inject]
        public static INamingService Naming;

        static IntrinsicRemap()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string> {
                { 
                    "System.Double System.Math::Asin(System.Double)",
                    "asin"
                },
                { 
                    "System.Double System.Math::Cosh(System.Double)",
                    "cosh"
                },
                { 
                    "System.Double System.Math::Abs(System.Double)",
                    "fabs"
                },
                { 
                    "System.Single System.Math::Abs(System.Single)",
                    "fabsf"
                },
                { 
                    "System.Double System.Math::Log(System.Double)",
                    "log"
                },
                { 
                    "System.Double System.Math::Pow(System.Double,System.Double)",
                    "pow"
                },
                { 
                    "System.Double System.Math::Tan(System.Double)",
                    "tan"
                },
                { 
                    "System.Double System.Math::Exp(System.Double)",
                    "exp"
                },
                { 
                    "System.Int64 System.Math::Abs(System.Int64)",
                    "il2cpp_codegen_abs"
                },
                { 
                    "System.Double System.Math::Ceiling(System.Double)",
                    "ceil"
                },
                { 
                    "System.Double System.Math::Atan(System.Double)",
                    "atan"
                },
                { 
                    "System.Double System.Math::Tanh(System.Double)",
                    "tanh"
                },
                { 
                    "System.Double System.Math::Sqrt(System.Double)",
                    "sqrt"
                },
                { 
                    "System.Double System.Math::Log10(System.Double)",
                    "log10"
                },
                { 
                    "System.Double System.Math::Sinh(System.Double)",
                    "sinh"
                },
                { 
                    "System.Double System.Math::Cos(System.Double)",
                    "cos"
                },
                { 
                    "System.Double System.Math::Atan2(System.Double,System.Double)",
                    "atan2"
                },
                { 
                    "System.Int32 System.Math::Abs(System.Int32)",
                    "il2cpp_codegen_abs"
                },
                { 
                    "System.Double System.Math::Sin(System.Double)",
                    "sin"
                },
                { 
                    "System.Double System.Math::Acos(System.Double)",
                    "acos"
                },
                { 
                    "System.Double System.Math::Floor(System.Double)",
                    "floor"
                },
                { 
                    "System.Double System.Math::Round(System.Double)",
                    "bankers_round"
                },
                { 
                    "System.Reflection.MethodBase System.Reflection.MethodBase::GetCurrentMethod()",
                    "il2cpp_codegen_get_method_object"
                },
                { 
                    "System.Type System.Type::GetType(System.String)",
                    "il2cpp_codegen_get_type"
                },
                { 
                    "System.Type System.Type::GetType(System.String,System.Boolean)",
                    "il2cpp_codegen_get_type"
                },
                { 
                    "System.Type System.Type::GetType(System.String,System.Boolean,System.Boolean)",
                    "il2cpp_codegen_get_type"
                },
                { 
                    "System.IntPtr System.Runtime.InteropServices.Marshal::GetIUnknownForObject(System.Object)",
                    "il2cpp_codegen_com_get_iunknown_for_object"
                },
                { 
                    "System.Reflection.Assembly System.Reflection.Assembly::GetExecutingAssembly()",
                    "il2cpp_codegen_get_executing_assembly"
                },
                { 
                    "System.Void System.Threading.Volatile::Write(System.Byte&,System.Byte)",
                    "VolatileWrite"
                },
                { 
                    "System.Void System.Threading.Volatile::Write(System.Boolean&,System.Boolean)",
                    "VolatileWrite"
                },
                { 
                    "System.Void System.Threading.Volatile::Write(System.Double&,System.Double)",
                    "VolatileWrite"
                },
                { 
                    "System.Void System.Threading.Volatile::Write(System.Int16&,System.Int16)",
                    "VolatileWrite"
                },
                { 
                    "System.Void System.Threading.Volatile::Write(System.Int32&,System.Int32)",
                    "VolatileWrite"
                },
                { 
                    "System.Void System.Threading.Volatile::Write(System.Int64&,System.Int64)",
                    "VolatileWrite"
                },
                { 
                    "System.Void System.Threading.Volatile::Write(System.IntPtr&,System.IntPtr)",
                    "VolatileWrite"
                },
                { 
                    "System.Void System.Threading.Volatile::Write(System.SByte&,System.SByte)",
                    "VolatileWrite"
                },
                { 
                    "System.Void System.Threading.Volatile::Write(System.Single&,System.Single)",
                    "VolatileWrite"
                },
                { 
                    "System.Void System.Threading.Volatile::Write(System.UInt16&,System.UInt16)",
                    "VolatileWrite"
                },
                { 
                    "System.Void System.Threading.Volatile::Write(System.UInt32&,System.UInt32)",
                    "VolatileWrite"
                },
                { 
                    "System.Void System.Threading.Volatile::Write(System.UInt64&,System.UInt64)",
                    "VolatileWrite"
                },
                { 
                    "System.Void System.Threading.Volatile::Write(System.UIntPtr&,System.UIntPtr)",
                    "VolatileWrite"
                },
                { 
                    "System.Void System.Threading.Volatile::Write(T&,T)",
                    "VolatileWrite"
                },
                { 
                    "System.Byte System.Threading.Volatile::Read(System.Byte&)",
                    "VolatileRead"
                },
                { 
                    "System.Boolean System.Threading.Volatile::Read(System.Boolean&)",
                    "VolatileRead"
                },
                { 
                    "System.Double System.Threading.Volatile::Read(System.Double&)",
                    "VolatileRead"
                },
                { 
                    "System.Int16 System.Threading.Volatile::Read(System.Int16&)",
                    "VolatileRead"
                },
                { 
                    "System.Int32 System.Threading.Volatile::Read(System.Int32&)",
                    "VolatileRead"
                },
                { 
                    "System.Int64 System.Threading.Volatile::Read(System.Int64&)",
                    "VolatileRead"
                },
                { 
                    "System.IntPtr System.Threading.Volatile::Read(System.IntPtr&)",
                    "VolatileRead"
                },
                { 
                    "System.SByte System.Threading.Volatile::Read(System.SByte&)",
                    "VolatileRead"
                },
                { 
                    "System.Single System.Threading.Volatile::Read(System.Single&)",
                    "VolatileRead"
                },
                { 
                    "System.UInt16 System.Threading.Volatile::Read(System.UInt16&)",
                    "VolatileRead"
                },
                { 
                    "System.UInt32 System.Threading.Volatile::Read(System.UInt32&)",
                    "VolatileRead"
                },
                { 
                    "System.UInt64 System.Threading.Volatile::Read(System.UInt64&)",
                    "VolatileRead"
                },
                { 
                    "System.UIntPtr System.Threading.Volatile::Read(System.UIntPtr&)",
                    "VolatileRead"
                },
                { 
                    "T System.Threading.Volatile::Read(T&)",
                    "VolatileRead"
                },
                { 
                    "System.Single UnityEngine.Mathf::Sin(System.Single)",
                    "sinf"
                },
                { 
                    "System.Single UnityEngine.Mathf::Cos(System.Single)",
                    "cosf"
                },
                { 
                    "System.Single UnityEngine.Mathf::Tan(System.Single)",
                    "tanf"
                },
                { 
                    "System.Single UnityEngine.Mathf::Asin(System.Single)",
                    "asinf"
                },
                { 
                    "System.Single UnityEngine.Mathf::Acos(System.Single)",
                    "acosf"
                },
                { 
                    "System.Single UnityEngine.Mathf::Atan(System.Single)",
                    "atanf"
                },
                { 
                    "System.Single UnityEngine.Mathf::Atan2(System.Single,System.Single)",
                    "atan2f"
                },
                { 
                    "System.Single UnityEngine.Mathf::Sqrt(System.Single)",
                    "sqrtf"
                },
                { 
                    "System.Single UnityEngine.Mathf::Abs(System.Single)",
                    "fabsf"
                },
                { 
                    "System.Single UnityEngine.Mathf::Pow(System.Single,System.Single)",
                    "powf"
                },
                { 
                    "System.Single UnityEngine.Mathf::Exp(System.Single)",
                    "expf"
                },
                { 
                    "System.Single UnityEngine.Mathf::Log(System.Single)",
                    "logf"
                },
                { 
                    "System.Single UnityEngine.Mathf::Log10(System.Single)",
                    "log10f"
                },
                { 
                    "System.Single UnityEngine.Mathf::Ceil(System.Single)",
                    "ceilf"
                },
                { 
                    "System.Single UnityEngine.Mathf::Floor(System.Single)",
                    "floorf"
                },
                { 
                    "System.Single UnityEngine.Mathf::Round(System.Single)",
                    "bankers_roundf"
                },
                { 
                    "System.Void System.Diagnostics.Debugger::Break()",
                    "IL2CPP_DEBUG_BREAK"
                }
            };
            MethodNameMapping = dictionary;
            Dictionary<string, Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>>> dictionary2 = new Dictionary<string, Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>>>();
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>>(IntrinsicRemap.GetCurrentMethodRemappingCustomArguments);
            }
            dictionary2.Add("System.Reflection.MethodBase System.Reflection.MethodBase::GetCurrentMethod()", <>f__mg$cache0);
            if (<>f__mg$cache1 == null)
            {
                <>f__mg$cache1 = new Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>>(IntrinsicRemap.GetTypeRemappingCustomArguments);
            }
            dictionary2.Add("System.Type System.Type::GetType(System.String)", <>f__mg$cache1);
            if (<>f__mg$cache2 == null)
            {
                <>f__mg$cache2 = new Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>>(IntrinsicRemap.GetTypeRemappingCustomArguments);
            }
            dictionary2.Add("System.Type System.Type::GetType(System.String,System.Boolean)", <>f__mg$cache2);
            if (<>f__mg$cache3 == null)
            {
                <>f__mg$cache3 = new Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>>(IntrinsicRemap.GetTypeRemappingCustomArguments);
            }
            dictionary2.Add("System.Type System.Type::GetType(System.String,System.Boolean,System.Boolean)", <>f__mg$cache3);
            if (<>f__mg$cache4 == null)
            {
                <>f__mg$cache4 = new Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>>(IntrinsicRemap.GetExecutingAssemblyRemappingCustomArguments);
            }
            dictionary2.Add("System.Reflection.Assembly System.Reflection.Assembly::GetExecutingAssembly()", <>f__mg$cache4);
            MethodNameMappingCustomArguments = dictionary2;
        }

        private static IEnumerable<string> GetCurrentMethodRemappingCustomArguments(MethodReference callingMethod, MethodReference methodToCall, IRuntimeMetadataAccess runtimeMetadata, IEnumerable<string> arguments) => 
            new string[] { runtimeMetadata.MethodInfo(callingMethod) };

        public static IEnumerable<string> GetCustomArguments(MethodReference methodToCall, MethodReference callingMethod, IRuntimeMetadataAccess runtimeMetadata, IEnumerable<string> arguments) => 
            MethodNameMappingCustomArguments[methodToCall.FullName](callingMethod, methodToCall, runtimeMetadata, arguments);

        private static IEnumerable<string> GetExecutingAssemblyRemappingCustomArguments(MethodReference callingMethod, MethodReference methodToCall, IRuntimeMetadataAccess runtimeMetadata, IEnumerable<string> arguments) => 
            new string[] { runtimeMetadata.MethodInfo(callingMethod) };

        private static IEnumerable<string> GetTypeRemappingCustomArguments(MethodReference callingMethod, MethodReference methodToCall, IRuntimeMetadataAccess runtimeMetadata, IEnumerable<string> arguments)
        {
            List<string> list = new List<string> {
                $"(Il2CppMethodPointer)&{runtimeMetadata.Method(methodToCall)}"
            };
            list.AddRange(arguments);
            list.Add("\"" + callingMethod.DeclaringType.Module.Assembly.Name + "\"");
            return list;
        }

        public static bool HasCustomArguments(MethodReference methodToCall) => 
            MethodNameMappingCustomArguments.ContainsKey(methodToCall.FullName);

        public static string MappedNameFor(MethodReference methodToCall) => 
            MethodNameMapping[methodToCall.FullName];

        public static bool ShouldRemap(MethodReference methodToCall) => 
            MethodNameMapping.ContainsKey(methodToCall.FullName);
    }
}

