namespace Unity.Serialization.Weaver
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using Unity.CecilTools;
    using Unity.SerializationLogic;

    internal abstract class DeserializeMethodEmitterBase : MethodEmitterBase
    {
        private LocalVariable _countVar;

        protected DeserializeMethodEmitterBase(TypeDefinition typeDef, SerializationBridgeProvider serializationBridgeProvider) : base(typeDef, serializationBridgeProvider)
        {
        }

        private MethodReference AddMethodRefFor(TypeReference typeReference)
        {
            GenericParameter parameterType = ((GenericInstanceType) typeReference).ElementType.GenericParameters[0];
            MethodReference method = new MethodReference("Add", base.Import(typeof(void)), MethodEmitterBase.TypeReferenceFor(typeReference)) {
                Parameters = { new ParameterDefinition("item", ParameterAttributes.None, parameterType) },
                HasThis = true
            };
            return base.Module.ImportReference(method);
        }

        private void CallDeserializationMethod(FieldReference fieldDef)
        {
            if (MethodEmitterBase.IsIUnitySerializable(base.TypeOf(fieldDef)))
            {
                base.EmitWithDepthCheck<FieldReference>(new Action<FieldReference>(this.InlinedDeserializeInvocationFor), fieldDef);
            }
            else
            {
                this.EmitDeserializationFor(fieldDef);
            }
        }

        protected virtual void CallDeserializeMethodFor(string fieldName, TypeReference typeReference)
        {
            base.Ldarg_1();
            base.Ldc_I4(1);
            base.Add();
            base.CallMethodOn(this.DeserializeMethodRefFor(typeReference), typeReference);
        }

        protected abstract void CallReaderMethod(string methodName, string fieldName);
        private MethodReference DeserializeMethodRefFor(TypeReference typeReference)
        {
            MethodReference reference = new MethodReference(this.DeserializeMethodName, base.Import(typeof(void)), MethodEmitterBase.TypeReferenceFor(typeReference)) {
                HasThis = true
            };
            reference.Parameters.Add(new ParameterDefinition("depth", ParameterAttributes.None, base.Import(typeof(int))));
            return reference;
        }

        private void EmitAlign()
        {
            base.LoadStateInstance(this.SerializedStateReaderInterface);
            base.Callvirt(this.SerializedStateReaderInterface, "Align");
        }

        private void EmitDeserializationFor(FieldReference fieldDef)
        {
            TypeReference typeReference = base.TypeOf(fieldDef);
            if (!MethodEmitterBase.CanInlineLoopOn(typeReference))
            {
                if (base.NeedsDepthCheck(typeReference))
                {
                    base.EmitWithDepthCheck<FieldReference, TypeReference>(new Action<FieldReference, TypeReference>(this, (IntPtr) this.EmitInlineDeserializationFor), fieldDef, typeReference);
                }
                else
                {
                    this.EmitInlineDeserializationFor(fieldDef, typeReference);
                }
            }
            else
            {
                base.EmitWithDepthCheck<FieldReference, TypeReference>(new Action<FieldReference, TypeReference>(this, (IntPtr) this.EmitDeserializationLoopFor), fieldDef, typeReference);
            }
        }

        private void EmitDeserializationLoopFor(FieldReference fieldDef, TypeReference typeRef)
        {
            base.Ldarg_0();
            this.ReadSequenceLength(fieldDef.Name);
            if (CecilUtils.IsGenericList(base.TypeOf(fieldDef)))
            {
                this.CountVar.EmitStore();
                this.CountVar.EmitLoad();
                this.EmitNewCollectionOf(typeRef);
                base.EmitStoreField(fieldDef);
                base.EmitLoopOnFieldElements(fieldDef, _ => this.CountVar.EmitLoad(), new Action<FieldReference, TypeReference>(this, (IntPtr) this.EmitDeserializeOfItem));
            }
            else
            {
                this.EmitNewCollectionOf(typeRef);
                base.EmitStoreField(fieldDef);
                base.EmitLoopOnFieldElements(fieldDef, new Action<FieldReference, TypeReference>(this, (IntPtr) this.EmitDeserializeOfItem));
            }
            this.FinishReadingSequence();
        }

        private void EmitDeserializeInvocationFor(FieldReference fieldDef)
        {
            base.LoadStateInstance(this.SerializedStateReaderInterface);
            this.CallReaderMethod(this.ReadMethodNameFor(fieldDef), fieldDef.Name);
        }

        private void EmitDeserializeOfItem(FieldReference fieldDef, TypeReference elementTypeRef)
        {
            if (!MethodEmitterBase.IsIUnitySerializable(elementTypeRef))
            {
                TypeReference type = base.TypeOf(fieldDef);
                if (CecilUtils.IsGenericList(type))
                {
                    base.EmitLoadField(fieldDef);
                    base.LoadStateInstance(this.SerializedStateReaderInterface);
                    this.CallReaderMethod(this.ReadMethodNameFor(elementTypeRef), null);
                    if (!elementTypeRef.IsValueType)
                    {
                        base.Isinst(elementTypeRef);
                        base.Callvirt(this.AddMethodRefFor(type));
                    }
                    else
                    {
                        base.Callvirt(this.AddMethodRefFor(type));
                    }
                }
                else
                {
                    base.EmitLoadField(fieldDef);
                    base.EmitLoadItemIndex();
                    base.LoadStateInstance(this.SerializedStateReaderInterface);
                    this.CallReaderMethod(this.ReadMethodNameFor(elementTypeRef), null);
                    if (!elementTypeRef.IsValueType)
                    {
                        base.Isinst(elementTypeRef);
                        base.Stelem_Ref();
                    }
                    else
                    {
                        this.Stelem_Any(elementTypeRef);
                    }
                }
            }
            else
            {
                this.EmitDeserializeOfIUnitySerializableItem(fieldDef, elementTypeRef);
            }
        }

        protected void EmitDeserializeOfIUnitySerializableClassItem(FieldReference fieldDef, TypeReference elementTypeRef)
        {
            if (CecilUtils.IsGenericList(base.TypeOf(fieldDef)))
            {
                LocalVariable variable = base.DefineLocal(elementTypeRef);
                base.ConstructSerializableObject(elementTypeRef);
                variable.EmitStore();
                variable.EmitLoad();
                this.CallDeserializeMethodFor(null, elementTypeRef);
                base.EmitLoadField(fieldDef);
                variable.EmitLoad();
                base.Callvirt(this.AddMethodRefFor(base.TypeOf(fieldDef)));
            }
            else
            {
                base.EmitLoadField(fieldDef);
                base.EmitLoadItemIndex();
                base.ConstructSerializableObject(elementTypeRef);
                base.Stelem_Ref();
                base.EmitLoadFieldArrayItemInLoop(fieldDef);
                this.CallDeserializeMethodFor(null, elementTypeRef);
            }
        }

        private void EmitDeserializeOfIUnitySerializableItem(FieldReference fieldDef, TypeReference elementTypeRef)
        {
            if (MethodEmitterBase.IsStruct(elementTypeRef))
            {
                this.EmitDeserializeOfStructItem(fieldDef, elementTypeRef);
            }
            else
            {
                this.EmitDeserializeOfIUnitySerializableClassItem(fieldDef, elementTypeRef);
            }
        }

        private void EmitDeserializeOfStructItem(FieldReference fieldDef, TypeReference elementTypeRef)
        {
            if (CecilUtils.IsGenericList(base.TypeOf(fieldDef)))
            {
                LocalVariable variable = base.DefineLocal(elementTypeRef);
                base.Processor.Emit(OpCodes.Ldloca, variable.Index);
                base.Processor.Emit(OpCodes.Initobj, elementTypeRef);
                base.Processor.Emit(OpCodes.Ldloca, variable.Index);
                this.CallDeserializeMethodFor(null, elementTypeRef);
                base.EmitLoadField(fieldDef);
                variable.EmitLoad();
                base.Callvirt(this.AddMethodRefFor(base.TypeOf(fieldDef)));
            }
            else
            {
                LocalVariable variable2 = base.DefineLocal(elementTypeRef);
                base.Processor.Emit(OpCodes.Ldloca, variable2.Index);
                base.Processor.Emit(OpCodes.Initobj, elementTypeRef);
                base.Processor.Emit(OpCodes.Ldloca, variable2.Index);
                this.CallDeserializeMethodFor(null, elementTypeRef);
                base.EmitLoadField(fieldDef);
                base.EmitLoadItemIndex();
                variable2.EmitLoad();
                this.Stelem_Any(elementTypeRef);
            }
        }

        private void EmitInlineDeserializationFor(FieldReference fieldRef, TypeReference typeRef)
        {
            base.Ldarg_0();
            this.EmitDeserializeInvocationFor(fieldRef);
            if ((!typeRef.IsPrimitive && !MethodEmitterBase.IsEnum(typeRef)) && (!UnityEngineTypePredicates.IsMatrix4x4(typeRef) && !UnityEngineTypePredicates.IsColor32(typeRef)))
            {
                base.Isinst(typeRef);
            }
            base.EmitStoreField(fieldRef);
        }

        protected override void EmitInstructionsFor(FieldReference fieldDef)
        {
            this.CallDeserializationMethod(fieldDef);
            TypeReference typeRef = base.TypeOf(fieldDef);
            if ((!MethodEmitterBase.IsStruct(base.TypeDef) || !UnityEngineTypePredicates.IsUnityEngineValueType(base.TypeDef)) && (MethodEmitterBase.IsStruct(typeRef) || MethodEmitterBase.RequiresAlignment(typeRef)))
            {
                this.EmitAlign();
            }
        }

        private void EmitNewCollectionOf(TypeReference typeRef)
        {
            TypeReference elementType = CecilUtils.ElementTypeOfCollection(typeRef);
            if (typeRef.IsArray)
            {
                this.Newarr(elementType);
            }
            else
            {
                base.Newobj(this.ListConstructorRefFor(typeRef));
            }
        }

        protected abstract void FinishReadingSequence();
        protected override void InjectAfterDeserialize()
        {
            base.InvokeMethodIfTypeImplementsInterface("UnityEngine.ISerializationCallbackReceiver", "OnAfterDeserialize");
        }

        protected override void InjectBeforeSerialize()
        {
        }

        private void InlinedDeserializeInvocationFor(FieldReference fieldDef)
        {
            TypeReference typeRef = base.TypeOf(fieldDef);
            if (!MethodEmitterBase.IsStruct(typeRef))
            {
                base.EmitWithNullCheckOnField(fieldDef, new Action<FieldReference>(this.ConstructAndStoreSerializableObject));
                base.EmitLoadField(fieldDef);
                this.CallDeserializeMethodFor(fieldDef.Name, typeRef);
            }
            else
            {
                base.EmitLoadField(fieldDef);
                this.CallDeserializeMethodFor(fieldDef.Name, typeRef);
            }
        }

        private MethodReference ListConstructorRefFor(TypeReference typeReference)
        {
            return new MethodReference(".ctor", base.Import(typeof(void)), MethodEmitterBase.TypeReferenceFor(typeReference)) { 
                HasThis = true,
                Parameters = { base.ParamDef("count", typeof(int)) }
            };
        }

        private void Newarr(TypeReference elementType)
        {
            base.Processor.Emit(OpCodes.Newarr, elementType);
        }

        private string ReadMethodNameFor(FieldReference fieldDef)
        {
            return this.ReadMethodNameFor(base.TypeOf(fieldDef));
        }

        protected string ReadMethodNameFor(TypeReference typeRef)
        {
            return ("Read" + base.MethodSuffixFor(typeRef));
        }

        protected abstract void ReadSequenceLength(string fieldName);
        protected override bool ShouldProcess(FieldDefinition fieldDefinition)
        {
            return base.WillUnitySerialize(fieldDefinition);
        }

        private void Stelem_Any(TypeReference elementTypeRef)
        {
            base.Processor.Emit(OpCodes.Stelem_Any, elementTypeRef);
        }

        private LocalVariable CountVar
        {
            get
            {
                LocalVariable variable2;
                if (this._countVar != null)
                {
                    variable2 = this._countVar;
                }
                else
                {
                    variable2 = this._countVar = base.DefineLocal(base.Import(typeof(int)));
                }
                return variable2;
            }
        }

        protected abstract string DeserializeMethodName { get; }

        protected abstract TypeDefinition SerializedStateReaderInterface { get; }
    }
}

