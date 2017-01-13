namespace Unity.Serialization.Weaver
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class SerializationWeaver
    {
        private readonly SerializationBridgeProvider _serializationBridgeProvider;
        private readonly TypeDefinition _typeDef;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__mg$cache0;

        private SerializationWeaver(TypeDefinition typeDef, AssemblyDefinition unityEngineAssembly)
        {
            this._typeDef = typeDef;
            this._serializationBridgeProvider = new SerializationBridgeProvider(unityEngineAssembly);
        }

        private void AddDeserializeMethod()
        {
            this._typeDef.Methods.Add(DeserializeMethodEmitter.DeserializeMethodDefinitionFor(this._typeDef, this._serializationBridgeProvider));
        }

        private void AddNamedDeserializeMethod()
        {
            this._typeDef.Methods.Add(NamedDeserializeMethodEmitter.DeserializeMethodDefinitionFor(this._typeDef, this._serializationBridgeProvider));
        }

        private void AddNamedSerializeMethod()
        {
            this._typeDef.Methods.Add(NamedSerializeMethodEmitter.SerializeMethodDefinitionFor(this._typeDef, this._serializationBridgeProvider));
        }

        private void AddRemapPPtrsMethod()
        {
            this._typeDef.Methods.Add(RemapPPtrsMethodEmitter.RemapPPtrsMethodDefinitionFor(this._typeDef, this._serializationBridgeProvider));
        }

        private void AddSerializationWeaverConstructor()
        {
            MethodDefinition item = new MethodDefinition(".ctor", DefaultConstructorAttributes(), this.Module.TypeSystem.Void) {
                Parameters = { this.SelfUnitySerializableParam() }
            };
            ILProcessor iLProcessor = item.Body.GetILProcessor();
            if (!this._typeDef.IsValueType)
            {
                if (this._typeDef.BaseType != null)
                {
                    MethodReference method = this.SerializationWeaverInjectedConstructorFor(this._typeDef.BaseType);
                    if (method != null)
                    {
                        iLProcessor.Emit(OpCodes.Ldarg_0);
                        iLProcessor.Emit(OpCodes.Ldarg_1);
                        iLProcessor.Emit(OpCodes.Call, method);
                    }
                    else
                    {
                        MethodReference reference2 = this.EmptyConstructorFor(this._typeDef);
                        if (reference2 != null)
                        {
                            iLProcessor.Emit(OpCodes.Ldarg_0);
                            iLProcessor.Emit(OpCodes.Call, reference2);
                        }
                        else
                        {
                            MethodReference reference3 = this.EmptyConstructorFor(this._typeDef.BaseType);
                            if (reference3 != null)
                            {
                                iLProcessor.Emit(OpCodes.Ldarg_0);
                                iLProcessor.Emit(OpCodes.Call, reference3);
                            }
                        }
                    }
                }
                else
                {
                    MethodReference reference4 = this.EmptyConstructorFor(this._typeDef);
                    if ((reference4 != null) && !reference4.Resolve().IsPrivate)
                    {
                        iLProcessor.Emit(OpCodes.Ldarg_0);
                        iLProcessor.Emit(OpCodes.Call, reference4);
                    }
                }
            }
            iLProcessor.Emit(OpCodes.Ret);
            this._typeDef.Methods.Add(item);
        }

        private void AddSerializeMethod()
        {
            this._typeDef.Methods.Add(SerializeMethodEmitter.SerializeMethodDefinitionFor(this._typeDef, this._serializationBridgeProvider));
        }

        private static MethodAttributes DefaultConstructorAttributes() => 
            (MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName);

        private MethodReference EmptyConstructorFor(TypeReference typeReference)
        {
            TypeDefinition definition = typeReference.Resolve();
            if (definition == null)
            {
                return null;
            }
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Func<MethodDefinition, bool>(SerializationWeaver.IsEmptyConstructor);
            }
            if (definition.Methods.SingleOrDefault<MethodDefinition>(<>f__mg$cache0) == null)
            {
                return null;
            }
            return new MethodReference(".ctor", this.Module.TypeSystem.Void, typeReference) { HasThis = true };
        }

        public static void FinalizeModuleWeaving(ModuleDefinition module)
        {
            CStringStore.SaveCStringStore(module);
        }

        private static bool IsEmptyConstructor(MethodDefinition methodDefinition) => 
            ((methodDefinition.Name == ".ctor") && !methodDefinition.HasParameters);

        private bool IsSerializationWeaverInjectedConstructor(MethodDefinition methodDefinition)
        {
            if ((methodDefinition.Name != ".ctor") || (methodDefinition.Parameters.Count != 1))
            {
                return false;
            }
            return (methodDefinition.Parameters[0].ParameterType.FullName == this._serializationBridgeProvider.UnitySerializableInterface.FullName);
        }

        private bool MakeImplement(TypeDefinition typeDefinition)
        {
            <MakeImplement>c__AnonStorey0 storey = new <MakeImplement>c__AnonStorey0 {
                typeDefinition = typeDefinition
            };
            if (this._typeDef.Interfaces.Any<InterfaceImplementation>(new Func<InterfaceImplementation, bool>(storey.<>m__0)))
            {
                return false;
            }
            this._typeDef.Interfaces.Add(new InterfaceImplementation(this.Module.ImportReference(storey.typeDefinition)));
            return true;
        }

        private void MakeSureUsableCtorExists()
        {
            bool flag = false;
            MethodReference reference = this.EmptyConstructorFor(this._typeDef);
            if (reference != null)
            {
                MethodDefinition definition = reference.Resolve();
                flag = true;
                if (!definition.IsPublic)
                {
                    definition.IsPublic = true;
                }
            }
            if (!flag)
            {
                this.AddSerializationWeaverConstructor();
            }
        }

        private void PromoteToPublicIfNeeded()
        {
            if (this._typeDef.IsNotPublic)
            {
                this._typeDef.IsNotPublic = false;
                this._typeDef.IsPublic = true;
            }
        }

        private ParameterDefinition SelfUnitySerializableParam() => 
            new ParameterDefinition("self", ParameterAttributes.None, this.Module.ImportReference(this._serializationBridgeProvider.UnitySerializableInterface));

        private MethodReference SerializationWeaverInjectedConstructorFor(TypeReference typeReference)
        {
            TypeDefinition definition = typeReference.Resolve();
            if (definition == null)
            {
                return null;
            }
            if (definition.Methods.SingleOrDefault<MethodDefinition>(new Func<MethodDefinition, bool>(this.IsSerializationWeaverInjectedConstructor)) == null)
            {
                return null;
            }
            MethodReference reference2 = new MethodReference(".ctor", this.Module.TypeSystem.Void, typeReference) {
                HasThis = true
            };
            reference2.Parameters.Add(this.SelfUnitySerializableParam());
            return reference2;
        }

        private void Weave()
        {
            if (this.MakeImplement(this._serializationBridgeProvider.UnitySerializableInterface))
            {
                this.MakeSureUsableCtorExists();
                this.PromoteToPublicIfNeeded();
                this.AddSerializeMethod();
                this.AddDeserializeMethod();
                this.AddRemapPPtrsMethod();
                this.AddNamedSerializeMethod();
                this.AddNamedDeserializeMethod();
            }
        }

        public static void Weave(TypeDefinition typeDef, AssemblyDefinition serializationInferfaceProvider)
        {
            new SerializationWeaver(typeDef, serializationInferfaceProvider).Weave();
        }

        private ModuleDefinition Module =>
            this._typeDef.Module;

        [CompilerGenerated]
        private sealed class <MakeImplement>c__AnonStorey0
        {
            internal TypeDefinition typeDefinition;

            internal bool <>m__0(InterfaceImplementation i) => 
                (i.InterfaceType.FullName == this.typeDefinition.FullName);
        }
    }
}

