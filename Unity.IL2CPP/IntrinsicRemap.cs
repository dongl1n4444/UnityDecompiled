using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	public class IntrinsicRemap
	{
		private const string GetCurrentMethodSignature = "System.Reflection.MethodBase System.Reflection.MethodBase::GetCurrentMethod()";

		private const string GetTypeStringSignature = "System.Type System.Type::GetType(System.String)";

		private const string GetTypeStringSignatureBoolean = "System.Type System.Type::GetType(System.String,System.Boolean)";

		private const string GetTypeStringSignatureBooleanBoolean = "System.Type System.Type::GetType(System.String,System.Boolean,System.Boolean)";

		private const string GetIUnknownForObjectSignatureObject = "System.IntPtr System.Runtime.InteropServices.Marshal::GetIUnknownForObject(System.Object)";

		[Inject]
		public static INamingService Naming;

		private static readonly Dictionary<string, string> MethodNameMapping = new Dictionary<string, string>
		{
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

		private static readonly Dictionary<string, Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>>> MethodNameMappingCustomArguments;

		[CompilerGenerated]
		private static Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>> <>f__mg$cache0;

		[CompilerGenerated]
		private static Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>> <>f__mg$cache1;

		[CompilerGenerated]
		private static Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>> <>f__mg$cache2;

		[CompilerGenerated]
		private static Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>> <>f__mg$cache3;

		public static bool ShouldRemap(MethodReference methodToCall)
		{
			return IntrinsicRemap.MethodNameMapping.ContainsKey(methodToCall.FullName);
		}

		public static string MappedNameFor(MethodReference methodToCall)
		{
			return IntrinsicRemap.MethodNameMapping[methodToCall.FullName];
		}

		public static bool HasCustomArguments(MethodReference methodToCall)
		{
			return IntrinsicRemap.MethodNameMappingCustomArguments.ContainsKey(methodToCall.FullName);
		}

		public static IEnumerable<string> GetCustomArguments(MethodReference methodToCall, MethodReference callingMethod, IRuntimeMetadataAccess runtimeMetadata, IEnumerable<string> arguments)
		{
			return IntrinsicRemap.MethodNameMappingCustomArguments[methodToCall.FullName](callingMethod, methodToCall, runtimeMetadata, arguments);
		}

		private static IEnumerable<string> GetCurrentMethodRemappingCustomArguments(MethodReference callingMethod, MethodReference methodToCall, IRuntimeMetadataAccess runtimeMetadata, IEnumerable<string> arguments)
		{
			return new string[]
			{
				runtimeMetadata.MethodInfo(callingMethod)
			};
		}

		private static IEnumerable<string> GetTypeRemappingCustomArguments(MethodReference callingMethod, MethodReference methodToCall, IRuntimeMetadataAccess runtimeMetadata, IEnumerable<string> arguments)
		{
			List<string> list = new List<string>();
			list.Add(string.Format("(Il2CppMethodPointer)&{0}", IntrinsicRemap.Naming.ForMethod(methodToCall)));
			list.AddRange(arguments);
			list.Add("\"" + callingMethod.DeclaringType.Module.Assembly.Name + "\"");
			return list;
		}

		static IntrinsicRemap()
		{
			// Note: this type is marked as 'beforefieldinit'.
			Dictionary<string, Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>>> dictionary = new Dictionary<string, Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>>>();
			Dictionary<string, Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>>> arg_4B5_0 = dictionary;
			string arg_4B5_1 = "System.Reflection.MethodBase System.Reflection.MethodBase::GetCurrentMethod()";
			if (IntrinsicRemap.<>f__mg$cache0 == null)
			{
				IntrinsicRemap.<>f__mg$cache0 = new Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>>(IntrinsicRemap.GetCurrentMethodRemappingCustomArguments);
			}
			arg_4B5_0.Add(arg_4B5_1, IntrinsicRemap.<>f__mg$cache0);
			Dictionary<string, Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>>> arg_4DD_0 = dictionary;
			string arg_4DD_1 = "System.Type System.Type::GetType(System.String)";
			if (IntrinsicRemap.<>f__mg$cache1 == null)
			{
				IntrinsicRemap.<>f__mg$cache1 = new Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>>(IntrinsicRemap.GetTypeRemappingCustomArguments);
			}
			arg_4DD_0.Add(arg_4DD_1, IntrinsicRemap.<>f__mg$cache1);
			Dictionary<string, Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>>> arg_505_0 = dictionary;
			string arg_505_1 = "System.Type System.Type::GetType(System.String,System.Boolean)";
			if (IntrinsicRemap.<>f__mg$cache2 == null)
			{
				IntrinsicRemap.<>f__mg$cache2 = new Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>>(IntrinsicRemap.GetTypeRemappingCustomArguments);
			}
			arg_505_0.Add(arg_505_1, IntrinsicRemap.<>f__mg$cache2);
			Dictionary<string, Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>>> arg_52D_0 = dictionary;
			string arg_52D_1 = "System.Type System.Type::GetType(System.String,System.Boolean,System.Boolean)";
			if (IntrinsicRemap.<>f__mg$cache3 == null)
			{
				IntrinsicRemap.<>f__mg$cache3 = new Func<MethodReference, MethodReference, IRuntimeMetadataAccess, IEnumerable<string>, IEnumerable<string>>(IntrinsicRemap.GetTypeRemappingCustomArguments);
			}
			arg_52D_0.Add(arg_52D_1, IntrinsicRemap.<>f__mg$cache3);
			IntrinsicRemap.MethodNameMappingCustomArguments = dictionary;
		}
	}
}
