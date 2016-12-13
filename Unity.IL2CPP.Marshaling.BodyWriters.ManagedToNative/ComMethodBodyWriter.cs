using Mono.Cecil;
using System;
using Unity.IL2CPP.Marshaling.MarshalInfoWriters;

namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative
{
	internal abstract class ComMethodBodyWriter : ManagedToNativeInteropMethodBodyWriter
	{
		protected readonly MethodReference _actualMethod;

		protected readonly TypeReference _interfaceType;

		protected virtual bool UseQueryInterfaceToObtainInterfacePointer
		{
			get
			{
				return this._actualMethod.DeclaringType.IsInterface();
			}
		}

		public ComMethodBodyWriter(MethodReference actualMethod, MethodReference interfaceMethod) : base(interfaceMethod, actualMethod, ComMethodBodyWriter.GetMarshalType(interfaceMethod), true)
		{
			this._actualMethod = actualMethod;
			this._interfaceType = interfaceMethod.DeclaringType;
		}

		private static MarshalType GetMarshalType(MethodReference interfaceMethod)
		{
			return (!interfaceMethod.DeclaringType.IsComInterface()) ? MarshalType.WindowsRuntime : MarshalType.COM;
		}

		protected override void WriteMethodPrologue(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
			string text = InteropMethodBodyWriter.Naming.ForTypeNameOnly(this._interfaceType);
			string text2 = InteropMethodBodyWriter.Naming.ForInteropInterfaceVariable(this._interfaceType);
			if (this.UseQueryInterfaceToObtainInterfacePointer)
			{
				writer.WriteLine("{0}* {1} = {2};", new object[]
				{
					text,
					text2,
					InteropMethodBodyWriter.Naming.Null
				});
				writer.WriteLine("il2cpp_hresult_t {0} = static_cast<{1}>({2})->{3}->QueryInterface({4}::IID, reinterpret_cast<void**>(&{5}));", new object[]
				{
					InteropMethodBodyWriter.Naming.ForInteropHResultVariable(),
					InteropMethodBodyWriter.Naming.ForVariable(InteropMethodBodyWriter.TypeProvider.Il2CppComObjectTypeReference),
					InteropMethodBodyWriter.Naming.ThisParameterName,
					InteropMethodBodyWriter.Naming.ForIl2CppComObjectIdentityField(),
					text,
					text2
				});
				writer.WriteStatement(Emit.Call("il2cpp_codegen_com_raise_exception_if_failed", InteropMethodBodyWriter.Naming.ForInteropHResultVariable()));
			}
			else
			{
				string text3 = (!this._actualMethod.HasThis) ? string.Format("(({0}*){1}->static_fields)", InteropMethodBodyWriter.Naming.ForStaticFieldsStruct(this._actualMethod.DeclaringType), metadataAccess.TypeInfoFor(this._actualMethod.DeclaringType)) : string.Format("{0}", InteropMethodBodyWriter.Naming.ThisParameterName);
				writer.WriteLine("{0}* {1} = {2}->{3}();", new object[]
				{
					InteropMethodBodyWriter.Naming.ForTypeNameOnly(this._interfaceType),
					InteropMethodBodyWriter.Naming.ForInteropInterfaceVariable(this._interfaceType),
					text3,
					InteropMethodBodyWriter.Naming.ForComTypeInterfaceFieldGetter(this._interfaceType)
				});
			}
			writer.WriteLine();
		}

		protected override void WriteInteropCallStatement(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
		{
			MethodReturnType methodReturnType = this.GetMethodReturnType();
			if (methodReturnType.ReturnType.MetadataType != MetadataType.Void)
			{
				DefaultMarshalInfoWriter defaultMarshalInfoWriter = base.MarshalInfoWriterFor(methodReturnType);
				defaultMarshalInfoWriter.WriteNativeVariableDeclarationOfType(writer, InteropMethodBodyWriter.Naming.ForInteropReturnValue());
			}
			writer.WriteStatement(this.GetMethodCallExpression(localVariableNames));
			this.OnBeforeHResultCheck(writer);
			writer.WriteLine();
			writer.WriteStatement(Emit.Call("il2cpp_codegen_com_raise_exception_if_failed", InteropMethodBodyWriter.Naming.ForInteropHResultVariable()));
		}

		protected virtual void OnBeforeHResultCheck(CppCodeWriter writer)
		{
			if (this.UseQueryInterfaceToObtainInterfacePointer)
			{
				writer.WriteStatement(InteropMethodBodyWriter.Naming.ForInteropInterfaceVariable(this._interfaceType) + "->Release()");
			}
		}

		private string GetMethodCallExpression(string[] localVariableNames)
		{
			string text = string.Empty;
			MethodReturnType methodReturnType = this.GetMethodReturnType();
			string functionCallParametersExpression = base.GetFunctionCallParametersExpression(localVariableNames);
			if (methodReturnType.ReturnType.MetadataType != MetadataType.Void)
			{
				if (functionCallParametersExpression.Length > 0)
				{
					text = ", ";
				}
				text = text + '&' + InteropMethodBodyWriter.Naming.ForInteropReturnValue();
			}
			string text2 = string.Format("{0} = {1}->{2}({3}{4})", new object[]
			{
				InteropMethodBodyWriter.Naming.ForInteropHResultVariable(),
				InteropMethodBodyWriter.Naming.ForInteropInterfaceVariable(this._interfaceType),
				this.GetMethodNameInGeneratedCode(),
				functionCallParametersExpression,
				text
			});
			if (!this.UseQueryInterfaceToObtainInterfacePointer)
			{
				text2 = "const il2cpp_hresult_t " + text2;
			}
			return text2;
		}
	}
}
