using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Marshaling;
using Unity.IL2CPP.Marshaling.MarshalInfoWriters;

namespace Unity.IL2CPP
{
	public class ComInterfaceWriter
	{
		[Inject]
		public static INamingService Naming;

		private readonly CppCodeWriter _writer;

		public ComInterfaceWriter(CppCodeWriter writer)
		{
			this._writer = writer;
		}

		public void WriteComInterfaceFor(TypeReference type)
		{
			this._writer.WriteCommentedLine(type.FullName);
			this.WriteForwardDeclarations(type);
			string text = (!type.Resolve().IsWindowsRuntime) ? "Il2CppIUnknown" : "Il2CppIInspectable";
			this._writer.WriteLine("struct NOVTABLE {0} : {1}", new object[]
			{
				ComInterfaceWriter.Naming.ForTypeNameOnly(type),
				text
			});
			using (new BlockWriter(this._writer, true))
			{
				this._writer.WriteStatement("static const Il2CppGuid IID");
				TypeResolver typeResolver = TypeResolver.For(type);
				foreach (MethodDefinition current in type.Resolve().Methods)
				{
					MethodReference methodReference = typeResolver.Resolve(current);
					this._writer.Write(ComInterfaceWriter.GetSignature(methodReference, methodReference, typeResolver, null));
					this._writer.WriteLine(" = 0;");
				}
			}
		}

		public static string GetSignature(MethodReference method, MethodReference interfaceMethod, TypeResolver typeResolver, string typeName = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			MarshalType marshalType = (!interfaceMethod.DeclaringType.Resolve().IsWindowsRuntime) ? MarshalType.COM : MarshalType.WindowsRuntime;
			if (string.IsNullOrEmpty(typeName))
			{
				stringBuilder.Append("virtual il2cpp_hresult_t STDCALL ");
			}
			else
			{
				stringBuilder.Append("il2cpp_hresult_t ");
				stringBuilder.Append(typeName);
				stringBuilder.Append("::");
			}
			stringBuilder.Append(ComInterfaceWriter.Naming.ForMethodNameOnly(interfaceMethod));
			stringBuilder.Append('(');
			stringBuilder.Append(ComInterfaceWriter.BuildMethodParameterList(method, interfaceMethod, typeResolver, marshalType, true));
			stringBuilder.Append(')');
			return stringBuilder.ToString();
		}

		private void WriteForwardDeclarations(TypeReference type)
		{
			TypeResolver typeResolver = TypeResolver.For(type);
			MarshalType marshalType = (!type.Resolve().IsWindowsRuntime) ? MarshalType.COM : MarshalType.WindowsRuntime;
			foreach (MethodDefinition current in type.Resolve().Methods)
			{
				foreach (ParameterDefinition current2 in current.Parameters)
				{
					DefaultMarshalInfoWriter defaultMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(typeResolver.Resolve(current2.ParameterType), marshalType, current2.MarshalInfo, true, false, false, null);
					defaultMarshalInfoWriter.WriteMarshaledTypeForwardDeclaration(this._writer);
				}
				if (current.ReturnType.MetadataType != MetadataType.Void)
				{
					DefaultMarshalInfoWriter defaultMarshalInfoWriter2 = MarshalDataCollector.MarshalInfoWriterFor(typeResolver.Resolve(current.ReturnType), marshalType, current.MethodReturnType.MarshalInfo, true, false, false, null);
					defaultMarshalInfoWriter2.WriteMarshaledTypeForwardDeclaration(this._writer);
				}
			}
		}

		internal static string BuildMethodParameterList(MethodReference interopMethod, MethodReference interfaceMethod, TypeResolver typeResolver, MarshalType marshalType, bool includeTypeNames)
		{
			List<string> list = new List<string>();
			int num = 0;
			foreach (ParameterDefinition current in interopMethod.Parameters)
			{
				MarshalInfo marshalInfo = interfaceMethod.Parameters[num].MarshalInfo;
				TypeReference type = typeResolver.Resolve(current.ParameterType);
				DefaultMarshalInfoWriter defaultMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(type, marshalType, marshalInfo, true, false, false, null);
				MarshaledType[] marshaledTypes = defaultMarshalInfoWriter.MarshaledTypes;
				for (int i = 0; i < marshaledTypes.Length; i++)
				{
					MarshaledType marshaledType = marshaledTypes[i];
					list.Add(string.Format((!includeTypeNames) ? "{1}" : "{0} {1}", marshaledType.DecoratedName, ComInterfaceWriter.Naming.ForParameterName(current) + marshaledType.VariableName));
				}
				num++;
			}
			TypeReference typeReference = typeResolver.Resolve(interopMethod.ReturnType);
			if (typeReference.MetadataType != MetadataType.Void)
			{
				MarshalInfo marshalInfo2 = interfaceMethod.MethodReturnType.MarshalInfo;
				DefaultMarshalInfoWriter defaultMarshalInfoWriter2 = MarshalDataCollector.MarshalInfoWriterFor(typeReference, marshalType, marshalInfo2, true, false, false, null);
				MarshaledType[] marshaledTypes2 = defaultMarshalInfoWriter2.MarshaledTypes;
				for (int j = 0; j < marshaledTypes2.Length - 1; j++)
				{
					list.Add(string.Format((!includeTypeNames) ? "{1}" : "{0}* {1}", marshaledTypes2[j].DecoratedName, ComInterfaceWriter.Naming.ForComInterfaceReturnParameterName() + marshaledTypes2[j].VariableName));
				}
				list.Add(string.Format((!includeTypeNames) ? "{1}" : "{0}* {1}", marshaledTypes2[marshaledTypes2.Length - 1].DecoratedName, ComInterfaceWriter.Naming.ForComInterfaceReturnParameterName()));
			}
			return list.AggregateWithComma();
		}
	}
}
