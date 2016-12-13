using Mono.Cecil;
using NiceIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Portability;

namespace Unity.IL2CPP
{
	public class VirtualCallCollector : IVirtualCallCollectorService, IDisposable
	{
		private readonly Dictionary<TypeReference[], int> _signatures = new Dictionary<TypeReference[], int>(new RuntimeInvokerComparer());

		[Inject]
		public static IGenericSharingAnalysisService GenericSharingAnalysis;

		[Inject]
		public static INamingService Naming;

		[Inject]
		public static ITypeProviderService TypeProvider;

		[Inject]
		public static IIl2CppTypeCollectorWriterService TypeCollector;

		[CompilerGenerated]
		private static Func<KeyValuePair<TypeReference[], int>, string> <>f__mg$cache0;

		public int Count
		{
			get
			{
				return this._signatures.Count;
			}
		}

		public void AddMethod(MethodReference method)
		{
			object signatures = this._signatures;
			lock (signatures)
			{
				TypeReference[] key = VirtualCallCollector.MethodToSignature(method);
				if (!this._signatures.ContainsKey(key))
				{
					this._signatures.Add(key, this._signatures.Count);
				}
			}
		}

		public void Dispose()
		{
			this._signatures.Clear();
		}

		private static TypeReference[] MethodToSignature(MethodReference method)
		{
			if (VirtualCallCollector.GenericSharingAnalysis.CanShareMethod(method))
			{
				method = VirtualCallCollector.GenericSharingAnalysis.GetSharedMethod(method);
			}
			TypeResolver typeResolver = new TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);
			TypeReference[] array = new TypeReference[method.Parameters.Count + 1];
			array[0] = VirtualCallCollector.TypeFor(typeResolver.ResolveReturnType(method));
			for (int i = 0; i < method.Parameters.Count; i++)
			{
				array[i + 1] = VirtualCallCollector.TypeFor(typeResolver.ResolveParameterType(method, method.Parameters[i]));
			}
			return array;
		}

		private static TypeReference TypeFor(TypeReference type)
		{
			TypeReference result;
			if (type.IsByReference || !type.IsValueType())
			{
				result = type.Module.TypeSystem.Object;
			}
			else if (type.MetadataType == MetadataType.Boolean)
			{
				result = type.Module.TypeSystem.SByte;
			}
			else if (type.MetadataType == MetadataType.Char)
			{
				result = type.Module.TypeSystem.Int16;
			}
			else if (type.IsEnum())
			{
				result = type.GetUnderlyingEnumType();
			}
			else
			{
				result = type;
			}
			return result;
		}

		private static void RecordIncludes(CppCodeWriter writer, TypeReference[] signature)
		{
			if (signature[0].MetadataType != MetadataType.Void)
			{
				writer.AddIncludesForTypeReference(signature[0], false);
			}
			foreach (TypeReference current in signature.Skip(1))
			{
				writer.AddIncludesForTypeReference(current, true);
			}
		}

		private static string GetMethodSignature(CppCodeWriter writer, TypeReference[] signature, int index)
		{
			VirtualCallCollector.RecordIncludes(writer, signature);
			return MethodSignatureWriter.GetMethodSignature(string.Format("UnresolvedVirtualCall_{0}", index), VirtualCallCollector.Naming.ForVariable(signature[0]), VirtualCallCollector.FormatParameters(signature.Skip(1)), "static", "");
		}

		private static string FormatParameters(IEnumerable<TypeReference> inputParams)
		{
			List<string> elements = VirtualCallCollector.ParametersFor(inputParams).ToList<string>();
			return elements.AggregateWithComma();
		}

		[DebuggerHidden]
		private static IEnumerable<string> ParametersFor(IEnumerable<TypeReference> inputParams)
		{
			VirtualCallCollector.<ParametersFor>c__Iterator0 <ParametersFor>c__Iterator = new VirtualCallCollector.<ParametersFor>c__Iterator0();
			<ParametersFor>c__Iterator.inputParams = inputParams;
			VirtualCallCollector.<ParametersFor>c__Iterator0 expr_0E = <ParametersFor>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		private static string FormatParameterName(TypeReference parameterType, string parameterName)
		{
			string str = string.Empty;
			str += VirtualCallCollector.Naming.ForVariable(parameterType);
			str += " ";
			return str + parameterName;
		}

		private static string MethodNameFor(KeyValuePair<TypeReference[], int> kvp)
		{
			return string.Format("(const Il2CppMethodPointer) UnresolvedVirtualCall_{0}", kvp.Value);
		}

		public UnresolvedVirtualsTablesInfo WriteUnresolvedStubs(NPath outputDir)
		{
			UnresolvedVirtualsTablesInfo result;
			using (SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(outputDir.Combine(new string[]
			{
				"UnresolvedVirtualCallStubs"
			}).ChangeExtension("cpp")))
			{
				sourceCodeWriter.AddCodeGenIncludes();
				KeyValuePair<TypeReference[], int>[] array = (from item in this._signatures
				orderby item.Value
				select item).ToArray<KeyValuePair<TypeReference[], int>>();
				KeyValuePair<TypeReference[], int>[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					KeyValuePair<TypeReference[], int> keyValuePair = array2[i];
					sourceCodeWriter.WriteLine(VirtualCallCollector.GetMethodSignature(sourceCodeWriter, keyValuePair.Key, keyValuePair.Value));
					sourceCodeWriter.WriteLine("{");
					sourceCodeWriter.Indent(1);
					sourceCodeWriter.WriteLine("il2cpp_codegen_raise_execution_engine_exception(method);");
					sourceCodeWriter.WriteLine("il2cpp_codegen_no_return();");
					sourceCodeWriter.Dedent(1);
					sourceCodeWriter.WriteLine("}");
					sourceCodeWriter.WriteLine();
				}
				CppCodeWriter arg_110_0 = sourceCodeWriter;
				string arg_110_1 = "extern const Il2CppMethodPointer";
				string arg_110_2 = "g_UnresolvedVirtualMethodPointers";
				IEnumerable<KeyValuePair<TypeReference[], int>> arg_10A_0 = array;
				if (VirtualCallCollector.<>f__mg$cache0 == null)
				{
					VirtualCallCollector.<>f__mg$cache0 = new Func<KeyValuePair<TypeReference[], int>, string>(VirtualCallCollector.MethodNameFor);
				}
				UnresolvedVirtualsTablesInfo unresolvedVirtualsTablesInfo;
				unresolvedVirtualsTablesInfo.MethodPointersInfo = arg_110_0.WriteArrayInitializer(arg_110_1, arg_110_2, arg_10A_0.Select(VirtualCallCollector.<>f__mg$cache0), false);
				List<Range> list = new List<Range>();
				int num = 0;
				foreach (TypeReference[] current in from item in array
				select item.Key)
				{
					int num2 = current.Length;
					list.Add(new Range(num, num2));
					num += num2;
				}
				unresolvedVirtualsTablesInfo.SignatureRangesInfo = list.AsReadOnly();
				unresolvedVirtualsTablesInfo.SignatureTypesInfo = (from type in array.SelectMany((KeyValuePair<TypeReference[], int> item) => item.Key)
				select VirtualCallCollector.TypeCollector.GetIndex(type, 0)).ToArray<int>().AsReadOnlyPortable<int>();
				result = unresolvedVirtualsTablesInfo;
			}
			return result;
		}
	}
}
