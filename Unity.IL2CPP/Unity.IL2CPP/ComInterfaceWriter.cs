using Mono.Cecil;
using System;
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
					this._writer.Write(ComInterfaceWriter.GetSignature(typeResolver.Resolve(current), typeResolver, null));
					this._writer.WriteLine(" = 0;");
				}
			}
		}

		public static string GetSignature(MethodReference method, TypeResolver typeResolver, string typeName = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			MarshalType marshalType = (!method.DeclaringType.Resolve().IsWindowsRuntime) ? MarshalType.COM : MarshalType.WindowsRuntime;
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
			stringBuilder.Append(ComInterfaceWriter.Naming.ForMethodNameOnly(method));
			stringBuilder.Append('(');
			string value = string.Empty;
			foreach (ParameterDefinition current in method.Parameters)
			{
				stringBuilder.Append(value);
				value = ", ";
				stringBuilder.Append(MarshalingUtils.NativeParameterTypeFor(current, marshalType, typeResolver, false));
				stringBuilder.Append(' ');
				stringBuilder.Append(ComInterfaceWriter.Naming.ForParameterName(current));
			}
			if (method.ReturnType.MetadataType != MetadataType.Void)
			{
				stringBuilder.Append(value);
				stringBuilder.Append(MarshalingUtils.NativeReturnTypeFor(method.MethodReturnType, marshalType, typeResolver, false));
				stringBuilder.AppendFormat("* {0}", ComInterfaceWriter.Naming.ForComInterfaceReturnParameterName());
			}
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
					DefaultMarshalInfoWriter defaultMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(typeResolver.Resolve(current2.ParameterType), marshalType, current2.MarshalInfo, true, false);
					defaultMarshalInfoWriter.WriteMarshaledTypeForwardDeclaration(this._writer);
				}
				if (current.ReturnType.MetadataType != MetadataType.Void)
				{
					DefaultMarshalInfoWriter defaultMarshalInfoWriter2 = MarshalDataCollector.MarshalInfoWriterFor(typeResolver.Resolve(current.ReturnType), marshalType, current.MethodReturnType.MarshalInfo, true, false);
					defaultMarshalInfoWriter2.WriteMarshaledTypeForwardDeclaration(this._writer);
				}
			}
		}
	}
}
