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
            this.thisParameter = InteropMethodInfo.Naming.ThisParameterName;
            this.identityField = InteropMethodInfo.Naming.ForIl2CppComObjectIdentityField();
            this.constructedObjectType = constructor.DeclaringType;
            TypeReference[] activationFactoryTypes = this.constructedObjectType.GetActivationFactoryTypes().ToArray<TypeReference>();
            if ((constructor.Parameters.Count != 0) || (activationFactoryTypes.Length == 0))
            {
                this.factoryMethod = GetFactoryMethod(constructor, activationFactoryTypes, false);
                if (this.factoryMethod == null)
                {
                    TypeReference[] referenceArray2 = this.constructedObjectType.GetComposableFactoryTypes().ToArray<TypeReference>();
                    this.factoryMethod = GetFactoryMethod(constructor, referenceArray2, true);
                    this.isComposingConstructor = true;
                }
                if (this.factoryMethod == null)
                {
                    throw new InvalidOperationException(string.Format($"Could not find factory method for Windows Runtime constructor {constructor.FullName}!", new object[0]));
                }
            }
        }

        private void ActivateThroughCompositionFactory(CppCodeWriter writer, string staticFieldsAccess, string parameters, IRuntimeMetadataAccess metadataAccess)
        {
            string str = metadataAccess.TypeInfoFor(this.constructedObjectType);
            string str2 = InteropMethodInfo.Naming.ForMethod(this.factoryMethod);
            TypeReference interfaceType = this.constructedObjectType.Resolve().ExtractDefaultInterface();
            string str3 = InteropMethodInfo.Naming.ForComTypeInterfaceFieldName(interfaceType);
            writer.WriteLine($"Il2CppIInspectable* outerInstance = {InteropMethodInfo.Naming.Null};");
            writer.WriteLine($"Il2CppIInspectable** innerInstance = {InteropMethodInfo.Naming.Null};");
            writer.WriteLine($"bool isComposedConstruction = {this.thisParameter}->klass != {str};");
            WriteDeclareActivationFactory(writer, this.factoryMethod.DeclaringType, staticFieldsAccess);
            writer.WriteLine();
            writer.WriteLine("if (isComposedConstruction)");
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine($"outerInstance = il2cpp_codegen_com_get_or_create_ccw<Il2CppIInspectable>({this.thisParameter});");
                writer.WriteLine($"innerInstance = reinterpret_cast<Il2CppIInspectable**>(&{this.thisParameter}->{this.identityField});");
            }
            writer.WriteLine();
            writer.WriteLine($"il2cpp_hresult_t hr = activationFactory->{str2}({parameters}outerInstance, innerInstance, &{this.thisParameter}->{str3});");
            writer.WriteLine("il2cpp_codegen_com_raise_exception_if_failed(hr, false);");
            writer.WriteLine();
            writer.WriteLine("if (isComposedConstruction)");
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine("outerInstance->Release();");
                writer.WriteLine($"{this.thisParameter}->{str3}->Release();");
            }
            writer.WriteLine("else");
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine($"hr = {this.thisParameter}->{str3}->QueryInterface(Il2CppIUnknown::IID, reinterpret_cast<void**>(&{this.thisParameter}->{this.identityField}));");
                writer.WriteLine("il2cpp_codegen_com_raise_exception_if_failed(hr, false);");
                writer.WriteLine();
                writer.WriteLine($"il2cpp_codegen_com_register_rcw({this.thisParameter});");
            }
        }

        private void ActivateThroughCustomActivationFactory(CppCodeWriter writer, string staticFieldsAccess, string parameters)
        {
            string str = InteropMethodInfo.Naming.ForMethod(this.factoryMethod);
            string str2 = InteropMethodInfo.Naming.ForComTypeInterfaceFieldName(this.constructedObjectType.Resolve().ExtractDefaultInterface());
            WriteDeclareActivationFactory(writer, this.factoryMethod.DeclaringType, staticFieldsAccess);
            writer.WriteLine($"il2cpp_hresult_t hr = activationFactory->{str}({parameters}&{this.thisParameter}->{str2});");
            writer.WriteLine("il2cpp_codegen_com_raise_exception_if_failed(hr, false);");
            writer.WriteLine();
            writer.WriteLine($"hr = {this.thisParameter}->{str2}->QueryInterface(Il2CppIUnknown::IID, reinterpret_cast<void**>(&{this.thisParameter}->{this.identityField}));");
            writer.WriteLine("il2cpp_codegen_com_raise_exception_if_failed(hr, false);");
            writer.WriteLine();
            writer.WriteLine($"il2cpp_codegen_com_register_rcw({this.thisParameter});");
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
            writer.WriteLine($"il2cpp_hresult_t hr = activationFactory->ActivateInstance({parameters}reinterpret_cast<Il2CppIInspectable**>(&{this.thisParameter}->{this.identityField}));");
            writer.WriteLine("il2cpp_codegen_com_raise_exception_if_failed(hr, false);");
            writer.WriteLine();
            writer.WriteLine($"il2cpp_codegen_com_register_rcw({this.thisParameter});");
        }

        private static void WriteDeclareActivationFactory(CppCodeWriter writer, TypeReference factoryType, string staticFieldsAccess)
        {
            string str = InteropMethodInfo.Naming.ForTypeNameOnly(factoryType);
            string str2 = InteropMethodInfo.Naming.ForComTypeInterfaceFieldGetter(factoryType);
            writer.WriteLine($"{str}* activationFactory = {staticFieldsAccess}->{str2}();");
        }

        protected override void WriteInteropCallStatement(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
        {
            string str = InteropMethodInfo.Naming.ForStaticFieldsStruct(this.constructedObjectType);
            string str2 = metadataAccess.TypeInfoFor(this.constructedObjectType);
            string staticFieldsAccess = string.Format($"(({str}*){str2}->static_fields)", new object[0]);
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

