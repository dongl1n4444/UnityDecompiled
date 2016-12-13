using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IL2CPP.Common;

namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative
{
	internal class WindowsRuntimeConstructorMethodBodyWriter : ManagedToNativeInteropMethodBodyWriter
	{
		private readonly TypeReference constructedObjectType;

		private readonly MethodReference factoryMethod;

		private readonly bool isComposingConstructor;

		private readonly string thisParameter = InteropMethodBodyWriter.Naming.ThisParameterName;

		private readonly string identityField = InteropMethodBodyWriter.Naming.ForIl2CppComObjectIdentityField();

		public WindowsRuntimeConstructorMethodBodyWriter(MethodReference constructor) : base(constructor, constructor, MarshalType.WindowsRuntime, true)
		{
			this.constructedObjectType = constructor.DeclaringType;
			TypeReference[] array = this.constructedObjectType.GetActivationFactoryTypes().ToArray<TypeReference>();
			if (constructor.Parameters.Count != 0 || array.Length == 0)
			{
				this.factoryMethod = WindowsRuntimeConstructorMethodBodyWriter.GetFactoryMethod(constructor, array, false);
				if (this.factoryMethod == null)
				{
					TypeReference[] activationFactoryTypes = this.constructedObjectType.GetComposableFactoryTypes().ToArray<TypeReference>();
					this.factoryMethod = WindowsRuntimeConstructorMethodBodyWriter.GetFactoryMethod(constructor, activationFactoryTypes, true);
					this.isComposingConstructor = true;
				}
				if (this.factoryMethod == null)
				{
					throw new InvalidOperationException(string.Format(string.Format("Could not find factory method for Windows Runtime constructor {0}!", constructor.FullName), new object[0]));
				}
			}
		}

		private static MethodReference GetFactoryMethod(MethodReference constructor, IEnumerable<TypeReference> activationFactoryTypes, bool isComposing)
		{
			int num = (!isComposing) ? 0 : 2;
			MethodReference result;
			foreach (TypeReference current in activationFactoryTypes)
			{
				foreach (MethodDefinition current2 in current.Resolve().Methods)
				{
					if (current2.Parameters.Count - num == constructor.Parameters.Count)
					{
						bool flag = true;
						for (int i = 0; i < constructor.Parameters.Count; i++)
						{
							if (!TypeReferenceEqualityComparer.AreEqual(current2.Parameters[i].ParameterType, constructor.Parameters[i].ParameterType, TypeComparisonMode.Exact))
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							result = current2;
							return result;
						}
					}
				}
			}
			result = null;
			return result;
		}

		protected override void WriteInteropCallStatement(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
		{
			string arg = InteropMethodBodyWriter.Naming.ForStaticFieldsStruct(this.constructedObjectType);
			string arg2 = metadataAccess.TypeInfoFor(this.constructedObjectType);
			string staticFieldsAccess = string.Format(string.Format("(({0}*){1}->static_fields)", arg, arg2), new object[0]);
			string text = base.GetFunctionCallParametersExpression(localVariableNames);
			if (text.Length > 0)
			{
				text += ", ";
			}
			if (this.factoryMethod == null)
			{
				this.WriteActivateThroughIActivationFactory(writer, staticFieldsAccess, text);
			}
			else if (!this.isComposingConstructor)
			{
				this.ActivateThroughCustomActivationFactory(writer, staticFieldsAccess, text);
			}
			else
			{
				this.ActivateThroughCompositionFactory(writer, staticFieldsAccess, text, metadataAccess);
			}
		}

		private void WriteActivateThroughIActivationFactory(CppCodeWriter writer, string staticFieldsAccess, string parameters)
		{
			WindowsRuntimeConstructorMethodBodyWriter.WriteDeclareActivationFactory(writer, InteropMethodBodyWriter.TypeProvider.IActivationFactoryTypeReference, staticFieldsAccess);
			writer.WriteLine(string.Format("il2cpp_hresult_t hr = activationFactory->ActivateInstance({0}reinterpret_cast<Il2CppIInspectable**>(&{1}->{2}));", parameters, this.thisParameter, this.identityField));
			writer.WriteLine("il2cpp_codegen_com_raise_exception_if_failed(hr);");
			writer.WriteLine();
			writer.WriteLine(string.Format("il2cpp_codegen_com_register_rcw({0});", this.thisParameter));
		}

		private void ActivateThroughCustomActivationFactory(CppCodeWriter writer, string staticFieldsAccess, string parameters)
		{
			string text = InteropMethodBodyWriter.Naming.ForMethod(this.factoryMethod);
			string text2 = InteropMethodBodyWriter.Naming.ForComTypeInterfaceFieldName(this.constructedObjectType.Resolve().ExtractDefaultInterface());
			WindowsRuntimeConstructorMethodBodyWriter.WriteDeclareActivationFactory(writer, this.factoryMethod.DeclaringType, staticFieldsAccess);
			writer.WriteLine(string.Format("il2cpp_hresult_t hr = activationFactory->{0}({1}&{2}->{3});", new object[]
			{
				text,
				parameters,
				this.thisParameter,
				text2
			}));
			writer.WriteLine("il2cpp_codegen_com_raise_exception_if_failed(hr);");
			writer.WriteLine();
			writer.WriteLine(string.Format("hr = {0}->{1}->QueryInterface(Il2CppIUnknown::IID, reinterpret_cast<void**>(&{2}->{3}));", new object[]
			{
				this.thisParameter,
				text2,
				this.thisParameter,
				this.identityField
			}));
			writer.WriteLine("il2cpp_codegen_com_raise_exception_if_failed(hr);");
			writer.WriteLine();
			writer.WriteLine(string.Format("il2cpp_codegen_com_register_rcw({0});", this.thisParameter));
		}

		private void ActivateThroughCompositionFactory(CppCodeWriter writer, string staticFieldsAccess, string parameters, IRuntimeMetadataAccess metadataAccess)
		{
			string arg = metadataAccess.TypeInfoFor(this.constructedObjectType);
			string text = InteropMethodBodyWriter.Naming.ForMethod(this.factoryMethod);
			TypeReference interfaceType = this.constructedObjectType.Resolve().ExtractDefaultInterface();
			string text2 = InteropMethodBodyWriter.Naming.ForComTypeInterfaceFieldName(interfaceType);
			writer.WriteLine(string.Format("Il2CppIInspectable* outerInstance = {0};", InteropMethodBodyWriter.Naming.Null));
			writer.WriteLine(string.Format("Il2CppIInspectable** innerInstance = {0};", InteropMethodBodyWriter.Naming.Null));
			writer.WriteLine(string.Format("bool isComposedConstruction = {0}->klass != {1};", this.thisParameter, arg));
			WindowsRuntimeConstructorMethodBodyWriter.WriteDeclareActivationFactory(writer, this.factoryMethod.DeclaringType, staticFieldsAccess);
			writer.WriteLine();
			writer.WriteLine("if (isComposedConstruction)");
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine(string.Format("outerInstance = il2cpp_codegen_com_get_or_create_ccw<Il2CppIInspectable>({0});", this.thisParameter));
				writer.WriteLine(string.Format("innerInstance = reinterpret_cast<Il2CppIInspectable**>(&{0}->{1});", this.thisParameter, this.identityField));
			}
			writer.WriteLine();
			writer.WriteLine(string.Format("il2cpp_hresult_t hr = activationFactory->{0}({1}outerInstance, innerInstance, &{2}->{3});", new object[]
			{
				text,
				parameters,
				this.thisParameter,
				text2
			}));
			writer.WriteLine("il2cpp_codegen_com_raise_exception_if_failed(hr);");
			writer.WriteLine();
			writer.WriteLine("if (isComposedConstruction)");
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine("outerInstance->Release();");
				writer.WriteLine(string.Format("{0}->{1}->Release();", this.thisParameter, text2));
			}
			writer.WriteLine("else");
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine(string.Format("hr = {0}->{1}->QueryInterface(Il2CppIUnknown::IID, reinterpret_cast<void**>(&{2}->{3}));", new object[]
				{
					this.thisParameter,
					text2,
					this.thisParameter,
					this.identityField
				}));
				writer.WriteLine("il2cpp_codegen_com_raise_exception_if_failed(hr);");
				writer.WriteLine();
				writer.WriteLine(string.Format("il2cpp_codegen_com_register_rcw({0});", this.thisParameter));
			}
		}

		private static void WriteDeclareActivationFactory(CppCodeWriter writer, TypeReference factoryType, string staticFieldsAccess)
		{
			string arg = InteropMethodBodyWriter.Naming.ForTypeNameOnly(factoryType);
			string arg2 = InteropMethodBodyWriter.Naming.ForComTypeInterfaceFieldGetter(factoryType);
			writer.WriteLine(string.Format("{0}* activationFactory = {1}->{2}();", arg, staticFieldsAccess, arg2));
		}
	}
}
