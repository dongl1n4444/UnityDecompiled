namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.Marshaling.BodyWriters;

    internal class WindowsRuntimeConstructorMethodBodyWriter : ManagedToNativeInteropMethodBodyWriter
    {
        private readonly TypeReference constructedObjectType;
        private readonly MethodReference factoryMethod;
        private readonly string identityField;
        private readonly bool isComposingConstructor;
        private readonly string thisParameter;

        public WindowsRuntimeConstructorMethodBodyWriter(MethodReference constructor) : base(constructor, constructor, MarshalType.WindowsRuntime, true)
        {
            this.thisParameter = InteropMethodBodyWriter.Naming.ThisParameterName;
            this.identityField = InteropMethodBodyWriter.Naming.ForIl2CppComObjectIdentityField();
            this.constructedObjectType = constructor.DeclaringType;
            TypeReference[] activationFactoryTypes = Enumerable.ToArray<TypeReference>(Extensions.GetActivationFactoryTypes(this.constructedObjectType));
            if ((constructor.Parameters.Count != 0) || (activationFactoryTypes.Length == 0))
            {
                this.factoryMethod = GetFactoryMethod(constructor, activationFactoryTypes, false);
                if (this.factoryMethod == null)
                {
                    TypeReference[] referenceArray2 = Enumerable.ToArray<TypeReference>(Extensions.GetComposableFactoryTypes(this.constructedObjectType));
                    this.factoryMethod = GetFactoryMethod(constructor, referenceArray2, true);
                    this.isComposingConstructor = true;
                }
                if (this.factoryMethod == null)
                {
                    throw new InvalidOperationException(string.Format(string.Format("Could not find factory method for Windows Runtime constructor {0}!", constructor.FullName), new object[0]));
                }
            }
        }

        private void ActivateThroughCompositionFactory(CppCodeWriter writer, string staticFieldsAccess, string parameters, IRuntimeMetadataAccess metadataAccess)
        {
            string str = metadataAccess.TypeInfoFor(this.constructedObjectType);
            string str2 = InteropMethodBodyWriter.Naming.ForMethod(this.factoryMethod);
            TypeReference interfaceType = Extensions.ExtractDefaultInterface(this.constructedObjectType.Resolve());
            string str3 = InteropMethodBodyWriter.Naming.ForComTypeInterfaceFieldName(interfaceType);
            writer.WriteLine(string.Format("Il2CppIInspectable* outerInstance = {0};", InteropMethodBodyWriter.Naming.Null));
            writer.WriteLine(string.Format("Il2CppIInspectable** innerInstance = {0};", InteropMethodBodyWriter.Naming.Null));
            writer.WriteLine(string.Format("bool isComposedConstruction = {0}->klass != {1};", this.thisParameter, str));
            WriteDeclareActivationFactory(writer, this.factoryMethod.DeclaringType, staticFieldsAccess);
            writer.WriteLine();
            writer.WriteLine("if (isComposedConstruction)");
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine(string.Format("outerInstance = il2cpp_codegen_com_get_or_create_ccw<Il2CppIInspectable>({0});", this.thisParameter));
                writer.WriteLine(string.Format("innerInstance = reinterpret_cast<Il2CppIInspectable**>(&{0}->{1});", this.thisParameter, this.identityField));
            }
            writer.WriteLine();
            writer.WriteLine(string.Format("il2cpp_hresult_t hr = activationFactory->{0}({1}outerInstance, innerInstance, &{2}->{3});", new object[] { str2, parameters, this.thisParameter, str3 }));
            writer.WriteLine("il2cpp_codegen_com_raise_exception_if_failed(hr);");
            writer.WriteLine();
            writer.WriteLine("if (isComposedConstruction)");
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine("outerInstance->Release();");
                writer.WriteLine(string.Format("{0}->{1}->Release();", this.thisParameter, str3));
            }
            writer.WriteLine("else");
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine(string.Format("hr = {0}->{1}->QueryInterface(Il2CppIUnknown::IID, reinterpret_cast<void**>(&{2}->{3}));", new object[] { this.thisParameter, str3, this.thisParameter, this.identityField }));
                writer.WriteLine("il2cpp_codegen_com_raise_exception_if_failed(hr);");
                writer.WriteLine();
                writer.WriteLine(string.Format("il2cpp_codegen_com_register_rcw({0});", this.thisParameter));
            }
        }

        private void ActivateThroughCustomActivationFactory(CppCodeWriter writer, string staticFieldsAccess, string parameters)
        {
            string str = InteropMethodBodyWriter.Naming.ForMethod(this.factoryMethod);
            string str2 = InteropMethodBodyWriter.Naming.ForComTypeInterfaceFieldName(Extensions.ExtractDefaultInterface(this.constructedObjectType.Resolve()));
            WriteDeclareActivationFactory(writer, this.factoryMethod.DeclaringType, staticFieldsAccess);
            writer.WriteLine(string.Format("il2cpp_hresult_t hr = activationFactory->{0}({1}&{2}->{3});", new object[] { str, parameters, this.thisParameter, str2 }));
            writer.WriteLine("il2cpp_codegen_com_raise_exception_if_failed(hr);");
            writer.WriteLine();
            writer.WriteLine(string.Format("hr = {0}->{1}->QueryInterface(Il2CppIUnknown::IID, reinterpret_cast<void**>(&{2}->{3}));", new object[] { this.thisParameter, str2, this.thisParameter, this.identityField }));
            writer.WriteLine("il2cpp_codegen_com_raise_exception_if_failed(hr);");
            writer.WriteLine();
            writer.WriteLine(string.Format("il2cpp_codegen_com_register_rcw({0});", this.thisParameter));
        }

        private static MethodReference GetFactoryMethod(MethodReference constructor, IEnumerable<TypeReference> activationFactoryTypes, bool isComposing)
        {
            int num = !isComposing ? 0 : 2;
            foreach (TypeReference reference in activationFactoryTypes)
            {
                foreach (MethodDefinition definition in reference.Resolve().Methods)
                {
                    if ((definition.Parameters.Count - num) != constructor.Parameters.Count)
                    {
                        continue;
                    }
                    bool flag = true;
                    for (int i = 0; i < constructor.Parameters.Count; i++)
                    {
                        if (!Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(definition.Parameters[i].ParameterType, constructor.Parameters[i].ParameterType, TypeComparisonMode.Exact))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        return definition;
                    }
                }
            }
            return null;
        }

        private void WriteActivateThroughIActivationFactory(CppCodeWriter writer, string staticFieldsAccess, string parameters)
        {
            WriteDeclareActivationFactory(writer, InteropMethodBodyWriter.TypeProvider.IActivationFactoryTypeReference, staticFieldsAccess);
            writer.WriteLine(string.Format("il2cpp_hresult_t hr = activationFactory->ActivateInstance({0}reinterpret_cast<Il2CppIInspectable**>(&{1}->{2}));", parameters, this.thisParameter, this.identityField));
            writer.WriteLine("il2cpp_codegen_com_raise_exception_if_failed(hr);");
            writer.WriteLine();
            writer.WriteLine(string.Format("il2cpp_codegen_com_register_rcw({0});", this.thisParameter));
        }

        private static void WriteDeclareActivationFactory(CppCodeWriter writer, TypeReference factoryType, string staticFieldsAccess)
        {
            string str = InteropMethodBodyWriter.Naming.ForTypeNameOnly(factoryType);
            string str2 = InteropMethodBodyWriter.Naming.ForComTypeInterfaceFieldGetter(factoryType);
            writer.WriteLine(string.Format("{0}* activationFactory = {1}->{2}();", str, staticFieldsAccess, str2));
        }

        protected override void WriteInteropCallStatement(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
        {
            string str = InteropMethodBodyWriter.Naming.ForStaticFieldsStruct(this.constructedObjectType);
            string str2 = metadataAccess.TypeInfoFor(this.constructedObjectType);
            string staticFieldsAccess = string.Format(string.Format("(({0}*){1}->static_fields)", str, str2), new object[0]);
            string functionCallParametersExpression = base.GetFunctionCallParametersExpression(localVariableNames);
            if (functionCallParametersExpression.Length > 0)
            {
                functionCallParametersExpression = functionCallParametersExpression + ", ";
            }
            if (this.factoryMethod == null)
            {
                this.WriteActivateThroughIActivationFactory(writer, staticFieldsAccess, functionCallParametersExpression);
            }
            else if (!this.isComposingConstructor)
            {
                this.ActivateThroughCustomActivationFactory(writer, staticFieldsAccess, functionCallParametersExpression);
            }
            else
            {
                this.ActivateThroughCompositionFactory(writer, staticFieldsAccess, functionCallParametersExpression, metadataAccess);
            }
        }
    }
}

