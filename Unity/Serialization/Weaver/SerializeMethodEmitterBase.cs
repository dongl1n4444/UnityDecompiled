namespace Unity.Serialization.Weaver
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Runtime.CompilerServices;
    using Unity.CecilTools;
    using Unity.SerializationLogic;

    internal abstract class SerializeMethodEmitterBase : MethodEmitterBase
    {
        protected SerializeMethodEmitterBase(TypeDefinition typeDef, SerializationBridgeProvider serializationBridgeProvider) : base(typeDef, serializationBridgeProvider)
        {
        }

        private void CallSerializationMethod(FieldReference fieldDef)
        {
            if (MethodEmitterBase.IsIUnitySerializable(base.TypeOf(fieldDef)))
            {
                base.EmitWithDepthCheck<FieldReference>(new Action<FieldReference>(this.InlinedSerializeInvocationFor), fieldDef);
            }
            else
            {
                this.EmitSerializationFor(fieldDef);
            }
        }

        protected virtual void CallSerializeMethodFor(string fieldName, TypeReference typeReference)
        {
            base.Ldarg_1();
            base.Ldc_I4(1);
            base.Add();
            base.CallMethodOn(this.SerializeMethodRefFor(typeReference), typeReference);
        }

        protected abstract void CallWriterMethod(string methodName, string fieldName);
        private static TypeReference ElementTypeFor(TypeReference typeRef) => 
            (!UnitySerializationLogic.IsSupportedCollection(typeRef) ? typeRef : CecilUtils.ElementTypeOfCollection(typeRef));

        protected void EmitAlign()
        {
            base.LoadStateInstance(this.SerializedStateWriterInterface);
            base.Callvirt(this.SerializedStateWriterInterface, "Align");
        }

        private void EmitInlineSerializationFor(FieldReference fieldRef)
        {
            base.LoadStateInstance(this.SerializedStateWriterInterface);
            base.EmitLoadField(fieldRef);
            base.EmitTypeOfIfNeeded(ElementTypeFor(base.TypeOf(fieldRef)));
            this.CallWriterMethod(this.WriteMethodNameFor(fieldRef), fieldRef.Name);
        }

        private void EmitInlineSerializationLoop(FieldReference fieldDef)
        {
            base.EmitWithNullCheckOnField(fieldDef, new Action<FieldReference>(this.EmitWriteEmptyCollection), new Action<FieldReference>(this.EmitInlineSerializationLoopUnchecked));
        }

        private void EmitInlineSerializationLoopUnchecked(FieldReference fieldDef)
        {
            <EmitInlineSerializationLoopUnchecked>c__AnonStorey0 storey = new <EmitInlineSerializationLoopUnchecked>c__AnonStorey0 {
                fieldDef = fieldDef,
                $this = this
            };
            this.WriteSequenceLength(storey.fieldDef.Name, new Action(storey.<>m__0));
            base.EmitLoopOnFieldElements(storey.fieldDef, new Action<FieldReference, TypeReference>(this.EmitSerializeItem));
            this.FinishWritingSequence();
        }

        protected override void EmitInstructionsFor(FieldReference fieldDef)
        {
            this.CallSerializationMethod(fieldDef);
            TypeReference typeRef = base.TypeOf(fieldDef);
            if ((!MethodEmitterBase.IsStruct(base.TypeDef) || !UnityEngineTypePredicates.IsUnityEngineValueType(base.TypeDef)) && (MethodEmitterBase.IsStruct(typeRef) || MethodEmitterBase.RequiresAlignment(typeRef)))
            {
                this.EmitAlign();
            }
        }

        private void EmitSerializationFor(FieldReference fieldDef)
        {
            TypeReference typeReference = base.TypeOf(fieldDef);
            if (!MethodEmitterBase.CanInlineLoopOn(typeReference))
            {
                if (base.NeedsDepthCheck(typeReference))
                {
                    base.EmitWithDepthCheck<FieldReference>(new Action<FieldReference>(this.EmitInlineSerializationFor), fieldDef);
                }
                else
                {
                    this.EmitInlineSerializationFor(fieldDef);
                }
            }
            else
            {
                base.EmitWithDepthCheck<FieldReference>(new Action<FieldReference>(this.EmitInlineSerializationLoop), fieldDef);
            }
        }

        private void EmitSerializeArrayItemForIUnitySerializable(FieldReference fieldDef, TypeReference elementTypeRef)
        {
            Instruction ifNotNullLbl = base.DefineLabel();
            Instruction endLbl = base.DefineLabel();
            base.EmitLoadFieldArrayItemInLoop(fieldDef);
            base.Brtrue_S(ifNotNullLbl);
            base.ConstructSerializableObject(elementTypeRef);
            base.Br_S(endLbl);
            base.MarkLabel(ifNotNullLbl);
            base.EmitLoadFieldArrayItemInLoop(fieldDef);
            base.MarkLabel(endLbl);
            this.CallSerializeMethodFor(null, elementTypeRef);
        }

        private void EmitSerializeArrayItemForStruct(FieldReference fieldDef, TypeReference elementTypeRef)
        {
            base.EmitLoadFieldArrayItemInLoop(fieldDef);
            this.CallSerializeMethodFor(null, elementTypeRef);
        }

        private void EmitSerializeInvocation(FieldReference fieldDef)
        {
            base.EmitLoadField(fieldDef);
            this.CallSerializeMethodFor(fieldDef.Name, base.TypeOf(fieldDef));
        }

        private void EmitSerializeItem(FieldReference fieldDef, TypeReference elementTypeRef)
        {
            if (!MethodEmitterBase.IsIUnitySerializable(elementTypeRef))
            {
                if (base.TypeOf(fieldDef).IsArray)
                {
                    base.LoadStateInstance(this.SerializedStateWriterInterface);
                    base.EmitLoadFieldArrayItemInLoop(fieldDef);
                    this.CallWriterMethod(this.WriteMethodNameFor(elementTypeRef), null);
                }
                else
                {
                    base.LoadStateInstance(this.SerializedStateWriterInterface);
                    base.EmitLoadField(fieldDef);
                    base.EmitLoadItemIndex();
                    base.Call(base.GetItemMethodRefFor(base.TypeOf(fieldDef)));
                    this.CallWriterMethod(this.WriteMethodNameFor(elementTypeRef), null);
                }
            }
            else
            {
                this.EmitSerializeItemForIDeserializable(fieldDef, elementTypeRef);
            }
        }

        private void EmitSerializeItemForIDeserializable(FieldReference fieldDef, TypeReference elementTypeRef)
        {
            if (MethodEmitterBase.IsStruct(elementTypeRef))
            {
                this.EmitSerializeItemForStruct(fieldDef, elementTypeRef);
            }
            else
            {
                this.EmitSerializeItemForIUnitySerializable(fieldDef, elementTypeRef);
            }
        }

        private void EmitSerializeItemForIUnitySerializable(FieldReference fieldDef, TypeReference elementTypeRef)
        {
            if (base.TypeOf(fieldDef).IsArray)
            {
                this.EmitSerializeArrayItemForIUnitySerializable(fieldDef, elementTypeRef);
            }
            else
            {
                this.EmitSerializeListItemForIUnitySerializable(fieldDef, elementTypeRef);
            }
        }

        private void EmitSerializeItemForStruct(FieldReference fieldDef, TypeReference elementTypeRef)
        {
            if (base.TypeOf(fieldDef).IsArray)
            {
                this.EmitSerializeArrayItemForStruct(fieldDef, elementTypeRef);
            }
            else
            {
                this.EmitSerializeListItemForStruct(fieldDef, elementTypeRef);
            }
        }

        private void EmitSerializeListItemForIUnitySerializable(FieldReference fieldDef, TypeReference elementTypeRef)
        {
            Instruction ifNotNullLbl = base.DefineLabel();
            Instruction endLbl = base.DefineLabel();
            base.EmitLoadField(fieldDef);
            base.EmitLoadItemIndex();
            base.Call(base.GetItemMethodRefFor(base.TypeOf(fieldDef)));
            base.Brtrue_S(ifNotNullLbl);
            base.ConstructSerializableObject(elementTypeRef);
            base.Br_S(endLbl);
            base.MarkLabel(ifNotNullLbl);
            base.EmitLoadField(fieldDef);
            base.EmitLoadItemIndex();
            base.Call(base.GetItemMethodRefFor(base.TypeOf(fieldDef)));
            base.MarkLabel(endLbl);
            this.CallSerializeMethodFor(null, elementTypeRef);
        }

        private void EmitSerializeListItemForStruct(FieldReference fieldDef, TypeReference elementTypeRef)
        {
            base.EmitLoadField(fieldDef);
            base.EmitLoadItemIndex();
            base.Call(base.GetItemMethodRefFor(base.TypeOf(fieldDef)));
            if (MethodEmitterBase.IsStruct(elementTypeRef))
            {
                LocalVariable variable = base.DefineLocal(elementTypeRef);
                variable.EmitStore();
                base.Processor.Emit(OpCodes.Ldloca, variable.Index);
            }
            this.CallSerializeMethodFor(null, elementTypeRef);
        }

        private void EmitWriteEmptyCollection(FieldReference fieldDef)
        {
            this.WriteSequenceLength(fieldDef.Name, () => base.Ldc_I4(0));
            this.FinishWritingSequence();
        }

        protected abstract void FinishWritingSequence();
        protected override void InjectAfterDeserialize()
        {
        }

        protected override void InjectBeforeSerialize()
        {
            base.InvokeMethodIfTypeImplementsInterface("UnityEngine.ISerializationCallbackReceiver", "OnBeforeSerialize");
        }

        private void InlinedSerializeInvocationFor(FieldReference fieldDef)
        {
            if (MethodEmitterBase.IsStruct(base.TypeOf(fieldDef)))
            {
                this.InlinedSerializeInvocationForStruct(fieldDef);
            }
            else
            {
                this.InlinedSerializeInvocationForClass(fieldDef);
            }
        }

        private void InlinedSerializeInvocationForClass(FieldReference fieldDef)
        {
            base.EmitWithNullCheckOnField(fieldDef, new Action<FieldReference>(this.ConstructAndStoreSerializableObject));
            this.EmitSerializeInvocation(fieldDef);
        }

        private void InlinedSerializeInvocationForStruct(FieldReference fieldDef)
        {
            base.EmitLoadField(fieldDef);
            this.CallSerializeMethodFor(fieldDef.Name, base.TypeOf(fieldDef));
        }

        private MethodReference SerializeMethodRefFor(TypeReference typeReference)
        {
            MethodReference reference = new MethodReference(this.SerializeMethodName, base.Import(typeof(void)), MethodEmitterBase.TypeReferenceFor(typeReference)) {
                HasThis = true
            };
            reference.Parameters.Add(new ParameterDefinition("depth", ParameterAttributes.None, base.Import(typeof(int))));
            return reference;
        }

        protected override bool ShouldProcess(FieldDefinition fieldDefinition) => 
            base.WillUnitySerialize(fieldDefinition);

        private string WriteMethodNameFor(FieldReference fieldDef) => 
            this.WriteMethodNameFor(base.TypeOf(fieldDef));

        protected string WriteMethodNameFor(TypeReference typeRef) => 
            ("Write" + base.MethodSuffixFor(typeRef));

        protected abstract void WriteSequenceLength(string fieldName, Action emitSequenceLength);

        protected abstract TypeDefinition SerializedStateWriterInterface { get; }

        protected abstract string SerializeMethodName { get; }

        [CompilerGenerated]
        private sealed class <EmitInlineSerializationLoopUnchecked>c__AnonStorey0
        {
            internal SerializeMethodEmitterBase $this;
            internal FieldReference fieldDef;

            internal void <>m__0()
            {
                this.$this.EmitLengthOf(this.fieldDef);
            }
        }
    }
}

