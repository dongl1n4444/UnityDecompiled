using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.IL2CPP.Marshaling.MarshalInfoWriters;

namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative
{
	internal class PInvokeMethodBodyWriter : ManagedToNativeInteropMethodBodyWriter
	{
		protected readonly MethodDefinition _methodDefinition;

		protected readonly PInvokeInfo _pinvokeInfo;

		private static readonly Dictionary<string, string[]> _internalizedMethods = new Dictionary<string, string[]>
		{
			{
				"MonoPosixHelper",
				new string[]
				{
					"CreateZStream",
					"CloseZStream",
					"Flush",
					"ReadZStream",
					"WriteZStream"
				}
			}
		};

		public PInvokeMethodBodyWriter(MethodReference interopMethod) : base(interopMethod, MarshalType.PInvoke, MarshalingUtils.UseUnicodeAsDefaultMarshalingForStringParameters(interopMethod))
		{
			this._methodDefinition = interopMethod.Resolve();
			this._pinvokeInfo = this._methodDefinition.PInvokeInfo;
		}

		public void WriteExternMethodeDeclarationForInternalPInvoke(CppCodeWriter writer)
		{
			if (this.ShouldInternalizeMethod())
			{
				writer.WriteInternalPInvokeDeclaration(this._pinvokeInfo.EntryPoint, string.Format("extern \"C\" {0} {1} {2}({3});", new object[]
				{
					base.MarshalInfoWriterFor(this.GetMethodReturnType()).MarshaledDecoratedTypeName,
					this.GetCallingConvention(),
					this._pinvokeInfo.EntryPoint,
					this.FormatParametersForTypedef()
				}));
			}
		}

		protected override void WriteMethodPrologue(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine("typedef {0};", new object[]
			{
				this.GetPInvokeMethodVariable()
			});
			if (this.ShouldInternalizeMethod())
			{
				writer.WriteLine();
			}
			else
			{
				writer.WriteLine("static {0} {1};", new object[]
				{
					InteropMethodBodyWriter.Naming.ForPInvokeFunctionPointerTypedef(),
					InteropMethodBodyWriter.Naming.ForPInvokeFunctionPointerVariable()
				});
				writer.WriteLine("if ({0} == NULL)", new object[]
				{
					InteropMethodBodyWriter.Naming.ForPInvokeFunctionPointerVariable()
				});
				writer.BeginBlock();
				string text = "parameterSize";
				writer.WriteLine("int {0} = {1};", new object[]
				{
					text,
					this.CalculateParameterSize()
				});
				string name = this._pinvokeInfo.Module.Name;
				string entryPoint = this._pinvokeInfo.EntryPoint;
				writer.WriteLine("{0} = il2cpp_codegen_resolve_pinvoke<{1}>(IL2CPP_NATIVE_STRING(\"{2}\"), \"{3}\", {4}, {5}, {6}, {7});", new object[]
				{
					InteropMethodBodyWriter.Naming.ForPInvokeFunctionPointerVariable(),
					InteropMethodBodyWriter.Naming.ForPInvokeFunctionPointerTypedef(),
					name,
					entryPoint,
					this.GetIl2CppCallConvention(),
					(!this.UseUnicodeCharSetForPInvokeInWindowsCallResolution()) ? "CHARSET_ANSI" : "CHARSET_UNICODE",
					text,
					(!this._pinvokeInfo.IsNoMangle) ? "false" : "true"
				});
				writer.WriteLine();
				writer.WriteLine("if ({0} == NULL)", new object[]
				{
					InteropMethodBodyWriter.Naming.ForPInvokeFunctionPointerVariable()
				});
				writer.BeginBlock();
				writer.WriteStatement(Emit.RaiseManagedException(string.Format("il2cpp_codegen_get_not_supported_exception(\"Unable to find method for p/invoke: '{0}'\")", base.GetMethodName())));
				writer.EndBlock(false);
				writer.EndBlock(false);
				writer.WriteLine();
			}
		}

		protected override void WriteInteropCallStatement(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
		{
			MethodReturnType methodReturnType = this.GetMethodReturnType();
			if (methodReturnType.ReturnType.MetadataType != MetadataType.Void)
			{
				DefaultMarshalInfoWriter defaultMarshalInfoWriter = base.MarshalInfoWriterFor(this.GetMethodReturnType());
				string marshaledDecoratedTypeName = defaultMarshalInfoWriter.MarshaledDecoratedTypeName;
				writer.WriteLine("{0} {1} = {2};", new object[]
				{
					marshaledDecoratedTypeName,
					InteropMethodBodyWriter.Naming.ForInteropReturnValue(),
					this.GetMethodCallExpression(localVariableNames)
				});
			}
			else
			{
				writer.WriteStatement(this.GetMethodCallExpression(localVariableNames));
			}
			if (this._pinvokeInfo != null && this._pinvokeInfo.SupportsLastError)
			{
				writer.WriteLine("il2cpp_codegen_marshal_store_last_error();");
			}
		}

		private string GetPInvokeMethodVariable()
		{
			return string.Format("{0} ({1} *{2}) ({3})", new object[]
			{
				base.MarshalInfoWriterFor(this.GetMethodReturnType()).MarshaledDecoratedTypeName,
				this.GetCallingConvention(),
				InteropMethodBodyWriter.Naming.ForPInvokeFunctionPointerTypedef(),
				this.FormatParametersForTypedef()
			});
		}

		private string GetCallingConvention()
		{
			string result;
			if (this._pinvokeInfo.IsCallConvStdCall)
			{
				result = "STDCALL";
			}
			else if (this._pinvokeInfo.IsCallConvCdecl)
			{
				result = "CDECL";
			}
			else
			{
				result = "DEFAULT_CALL";
			}
			return result;
		}

		private string GetIl2CppCallConvention()
		{
			string result;
			if (this._pinvokeInfo.IsCallConvStdCall)
			{
				result = "IL2CPP_CALL_STDCALL";
			}
			else if (this._pinvokeInfo.IsCallConvCdecl)
			{
				result = "IL2CPP_CALL_C";
			}
			else
			{
				result = "IL2CPP_CALL_DEFAULT";
			}
			return result;
		}

		private bool UseUnicodeCharSetForPInvokeInWindowsCallResolution()
		{
			return !this._pinvokeInfo.IsCharSetAnsi;
		}

		private string CalculateParameterSize()
		{
			MarshaledParameter[] parameters = this._parameters;
			string result;
			if (parameters.Length == 0)
			{
				result = "0";
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder(this.GetParameterSize(parameters[0]));
				for (int i = 1; i < parameters.Length; i++)
				{
					stringBuilder.AppendFormat(" + {0}", this.GetParameterSize(parameters[i]));
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		private string GetParameterSize(MarshaledParameter parameter)
		{
			DefaultMarshalInfoWriter defaultMarshalInfoWriter = base.MarshalInfoWriterFor(parameter);
			if (defaultMarshalInfoWriter.NativeSize == "-1" && parameter.ParameterType.MetadataType != MetadataType.Array)
			{
				string message = string.Format("Cannot marshal parameter {0} of type {1} for P/Invoke.", parameter.NameInGeneratedCode, parameter.ParameterType.FullName);
				throw new NotSupportedException(message);
			}
			MetadataType metadataType = parameter.ParameterType.MetadataType;
			string result;
			if (metadataType == MetadataType.Class || metadataType == MetadataType.Array)
			{
				result = "sizeof(void*)";
			}
			else
			{
				int num = 4 - defaultMarshalInfoWriter.NativeSizeWithoutPointers % 4;
				if (num != 4)
				{
					result = defaultMarshalInfoWriter.NativeSize + " + " + num;
				}
				else
				{
					result = defaultMarshalInfoWriter.NativeSize;
				}
			}
			return result;
		}

		private string GetMethodCallExpression(string[] localVariableNames)
		{
			string functionCallParametersExpression = base.GetFunctionCallParametersExpression(localVariableNames);
			string result;
			if (this.ShouldInternalizeMethod())
			{
				result = string.Format("reinterpret_cast<{0}>({1})({2})", InteropMethodBodyWriter.Naming.ForPInvokeFunctionPointerTypedef(), this._pinvokeInfo.EntryPoint, functionCallParametersExpression);
			}
			else
			{
				result = string.Format("{0}({1})", InteropMethodBodyWriter.Naming.ForPInvokeFunctionPointerVariable(), functionCallParametersExpression);
			}
			return result;
		}

		protected string FormatParametersForTypedef()
		{
			MarshaledParameter[] parameters = this._parameters;
			string result;
			if (this._parameters.Length == 0)
			{
				result = string.Empty;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder(base.MarshalInfoWriterFor(parameters[0]).MarshaledDecoratedTypeName);
				for (int i = 1; i < this._parameters.Length; i++)
				{
					stringBuilder.AppendFormat(", {0}", base.MarshalInfoWriterFor(parameters[i]).MarshaledDecoratedTypeName);
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		private bool ShouldInternalizeMethod()
		{
			bool result;
			if (this._pinvokeInfo == null)
			{
				result = false;
			}
			else
			{
				string name = this._pinvokeInfo.Module.Name;
				string[] source;
				result = (name == "__Internal" || (PInvokeMethodBodyWriter._internalizedMethods.TryGetValue(name, out source) && source.Any((string a) => a == this._methodDefinition.Name)));
			}
			return result;
		}
	}
}
