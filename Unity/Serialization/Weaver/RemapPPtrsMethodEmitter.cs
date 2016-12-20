namespace Unity.Serialization.Weaver
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Runtime.CompilerServices;
    using Unity.SerializationLogic;

    public class RemapPPtrsMethodEmitter : MethodEmitterBase
    {
        private const string CanExecuteSerializationCallbacks = "CanExecuteSerializationCallbacks";
        private const string GetNewInstanceToReplaceOldInstanceMethodName = "GetNewInstanceToReplaceOldInstance";
        private const string RemapPPtrsTypeName = "Unity_RemapPPtrs";

        private RemapPPtrsMethodEmitter(TypeDefinition typeDef, SerializationBridgeProvider serializationBridgeProvider) : base(typeDef, serializationBridgeProvider)
        {
            base.CreateMethodDef("Unity_RemapPPtrs");
        }

        private void CallRemapPPtrsMethodFor(TypeReference typeReference)
        {
            base.Ldarg_1();
            base.Ldc_I4(1);
            base.Add();
            base.CallMethodOn(this.RemapPPtrsMethodRefFor(typeReference), typeReference);
        }

        private void EmitInlinedRemapPPtrsInvocationForCollection(FieldReference fieldDef)
        {
            base.EmitLoopOnFieldElements(fieldDef, new Action<FieldReference, TypeReference>(this, (IntPtr) this.EmitRemapPPtrsInvocationLoopBody));
        }

        protected override void EmitInstructionsFor(FieldReference field)
        {
            if (UnitySerializationLogic.IsSupportedCollection(base.TypeOf(field)))
            {
                base.EmitWithDepthCheck<FieldReference>(new Action<FieldReference>(this.EmitRemappingForCollectionField), field);
            }
            else
            {
                this.EmitRemappingForNonCollectionField(field, new Action<FieldReference>(this.EmitLoadField));
            }
        }

        private void EmitRemappingForCollectionField(FieldReference fieldDef)
        {
            base.EmitWithNotNullCheckOnField(fieldDef, new Action<FieldReference>(this.EmitInlinedRemapPPtrsInvocationForCollection));
        }

        private void EmitRemappingForIDeserializable(FieldReference fieldDef, Action<FieldReference> fieldLoader)
        {
            <EmitRemappingForIDeserializable>c__AnonStorey0 storey = new <EmitRemappingForIDeserializable>c__AnonStorey0 {
                fieldLoader = fieldLoader,
                $this = this
            };
            if (MethodEmitterBase.IsStruct(base.TypeOf(fieldDef)))
            {
                this.InvokeRemapPPtrsOn(fieldDef, storey.fieldLoader);
            }
            else
            {
                base.EmitWithNotNullCheckOnField(fieldDef, new Action<FieldReference>(storey.<>m__0));
            }
        }

        private void EmitRemappingForNonCollectionField(FieldReference field, Action<FieldReference> fieldLoader)
        {
            TypeReference typeDeclaration = base.TypeOf(field);
            if (UnitySerializationLogic.ShouldImplementIDeserializable(typeDeclaration) && !MethodEmitterBase.IsUnityEngineObject(typeDeclaration))
            {
                base.EmitWithDepthCheck<FieldReference, Action<FieldReference>>(new Action<FieldReference, Action<FieldReference>>(this, (IntPtr) this.EmitRemappingForIDeserializable), field, fieldLoader);
            }
            else
            {
                this.EmitRemappingForSimpleField(field, fieldLoader);
            }
        }

        private void EmitRemappingForSimpleField(FieldReference fieldDef, Action<FieldReference> fieldLoader)
        {
            if (MethodEmitterBase.IsUnityEngineObject(base.TypeOf(fieldDef)))
            {
                Instruction endLbl = base.DefineLabel();
                fieldLoader(fieldDef);
                base.Brfalse_S(endLbl);
                this.EmitRemappingForSimpleFieldUnchecked(fieldDef, fieldLoader);
                base.MarkLabel(endLbl);
            }
            else
            {
                this.EmitRemappingForSimpleFieldUnchecked(fieldDef, fieldLoader);
            }
        }

        private void EmitRemappingForSimpleFieldUnchecked(FieldReference fieldDef, Action<FieldReference> fieldLoader)
        {
            base.Ldarg_0();
            base.LoadStateInstance(base._serializationBridgeProvider.PPtrRemapperInterface);
            fieldLoader(fieldDef);
            base.Callvirt(this.PPtrRemapperInterface, "GetNewInstanceToReplaceOldInstance");
            base.Isinst(base.TypeOf(fieldDef));
            base.EmitStoreField(fieldDef);
        }

        private void EmitRemapPPtrsArrayItemInvocation(FieldReference fieldDef, TypeReference elementType)
        {
            base.EmitLoadField(fieldDef);
            base.EmitLoadItemIndex();
            base.LoadStateInstance(this.PPtrRemapperInterface);
            base.EmitLoadFieldArrayItemInLoop(fieldDef);
            base.Callvirt(this.PPtrRemapperInterface, "GetNewInstanceToReplaceOldInstance");
            base.Isinst(elementType);
            base.Stelem_Ref();
        }

        private void EmitRemapPPtrsInvocationLoopBody(FieldReference fieldDef, TypeReference elementType)
        {
            if (UnitySerializationLogic.ShouldImplementIDeserializable(elementType) && !MethodEmitterBase.IsUnityEngineObject(elementType))
            {
                if (base.TypeOf(fieldDef).IsArray)
                {
                    Instruction endLbl = base.DefineLabel();
                    base.EmitLoadFieldArrayItemInLoop(fieldDef);
                    base.Brfalse_S(endLbl);
                    base.EmitLoadFieldArrayItemInLoop(fieldDef);
                    this.CallRemapPPtrsMethodFor(elementType);
                    base.MarkLabel(endLbl);
                }
                else
                {
                    Instruction instruction2 = base.DefineLabel();
                    base.EmitLoadField(fieldDef);
                    base.EmitLoadItemIndex();
                    base.Callvirt(base.GetItemMethodRefFor(base.TypeOf(fieldDef)));
                    LocalVariable variable = base.DefineLocal(elementType);
                    variable.EmitStore();
                    if (MethodEmitterBase.IsStruct(elementType))
                    {
                        base.Processor.Emit(OpCodes.Ldloca, variable.Index);
                    }
                    else
                    {
                        variable.EmitLoad();
                        base.Brfalse_S(instruction2);
                        variable.EmitLoad();
                    }
                    this.CallRemapPPtrsMethodFor(elementType);
                    if (MethodEmitterBase.IsStruct(elementType))
                    {
                        base.EmitLoadField(fieldDef);
                        base.EmitLoadItemIndex();
                        base.Processor.Emit(OpCodes.Ldloc, variable.Index);
                        base.Callvirt(base.SetItemMethodRefFor(base.TypeOf(fieldDef)));
                    }
                    base.MarkLabel(instruction2);
                }
            }
            else if (base.TypeOf(fieldDef).IsArray)
            {
                this.EmitRemapPPtrsArrayItemInvocation(fieldDef, elementType);
            }
            else
            {
                this.EmitRemapPPtrsListItemInvocation(fieldDef, elementType);
            }
        }

        private void EmitRemapPPtrsListItemInvocation(FieldReference fieldDef, TypeReference elementType)
        {
            base.EmitLoadField(fieldDef);
            base.EmitLoadItemIndex();
            base.LoadStateInstance(this.PPtrRemapperInterface);
            base.EmitLoadField(fieldDef);
            base.EmitLoadItemIndex();
            base.Callvirt(base.GetItemMethodRefFor(base.TypeOf(fieldDef)));
            base.Callvirt(this.PPtrRemapperInterface, "GetNewInstanceToReplaceOldInstance");
            base.Isinst(elementType);
            base.Callvirt(base.SetItemMethodRefFor(base.TypeOf(fieldDef)));
        }

        protected override void InjectAfterDeserialize()
        {
            this.InjectSerializationCallbackMethod("OnAfterDeserialize");
        }

        protected override void InjectBeforeSerialize()
        {
            this.InjectSerializationCallbackMethod("OnBeforeSerialize");
        }

        private void InjectSerializationCallbackMethod(string methodName)
        {
            string interfaceName = "UnityEngine.ISerializationCallbackReceiver";
            if (base.IfTypeImplementsInterface(interfaceName))
            {
                Instruction endLbl = base.DefineLabel();
                base.LoadStateInstance(base._serializationBridgeProvider.PPtrRemapperInterface);
                base.Callvirt(this.PPtrRemapperInterface, "CanExecuteSerializationCallbacks");
                base.Brfalse_S(endLbl);
                base.InvokeMethodIfTypeImplementsInterface(interfaceName, methodName);
                base.MarkLabel(endLbl);
            }
        }

        private void InvokeRemapPPtrsOn(FieldReference fieldDef, Action<FieldReference> fieldLoader)
        {
            fieldLoader(fieldDef);
            this.CallRemapPPtrsMethodFor(base.TypeOf(fieldDef));
        }

        public static MethodDefinition RemapPPtrsMethodDefinitionFor(TypeDefinition typeDef, SerializationBridgeProvider serializationBridgeProvider)
        {
            return new RemapPPtrsMethodEmitter(typeDef, serializationBridgeProvider).MethodDefinition;
        }

        private MethodReference RemapPPtrsMethodRefFor(TypeReference typeReference)
        {
            MethodReference reference = new MethodReference("Unity_RemapPPtrs", base.Import(typeof(void)), MethodEmitterBase.TypeReferenceFor(typeReference)) {
                HasThis = true
            };
            reference.Parameters.Add(new ParameterDefinition("depth", ParameterAttributes.None, base.Import(typeof(int))));
            return reference;
        }

        protected override bool ShouldProcess(FieldDefinition fieldDefinition)
        {
            return UnitySerializationLogic.ShouldFieldBePPtrRemapped(fieldDefinition, base.TypeResolver);
        }

        private TypeDefinition PPtrRemapperInterface
        {
            get
            {
                return base._serializationBridgeProvider.PPtrRemapperInterface;
            }
        }

        [CompilerGenerated]
        private sealed class <EmitRemappingForIDeserializable>c__AnonStorey0
        {
            internal RemapPPtrsMethodEmitter $this;
            internal Action<FieldReference> fieldLoader;

            internal void <>m__0(FieldReference _)
            {
                this.$this.InvokeRemapPPtrsOn(_, this.fieldLoader);
            }
        }
    }
}

