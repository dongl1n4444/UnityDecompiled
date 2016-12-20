namespace Unity.UNetWeaver
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;

    internal class SyncListStructProcessor
    {
        private TypeReference m_ItemType;
        private TypeDefinition m_TypeDef;

        public SyncListStructProcessor(TypeDefinition typeDef)
        {
            Weaver.DLog(typeDef, "SyncListStructProcessor for " + typeDef.Name, new object[0]);
            this.m_TypeDef = typeDef;
        }

        private MethodReference GenerateDeserialization()
        {
            Weaver.DLog(this.m_TypeDef, "  GenerateDeserialization", new object[0]);
            foreach (MethodDefinition definition in this.m_TypeDef.Methods)
            {
                if (definition.Name == "DeserializeItem")
                {
                    return definition;
                }
            }
            MethodDefinition item = new MethodDefinition("DeserializeItem", MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual, this.m_ItemType) {
                Parameters = { new ParameterDefinition("reader", ParameterAttributes.None, Weaver.scriptDef.MainModule.ImportReference(Weaver.NetworkReaderType)) }
            };
            ILProcessor iLProcessor = item.Body.GetILProcessor();
            iLProcessor.Body.InitLocals = true;
            iLProcessor.Body.Variables.Add(new VariableDefinition("result", this.m_ItemType));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloca, 0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Initobj, this.m_ItemType));
            foreach (FieldDefinition definition3 in this.m_ItemType.Resolve().Fields)
            {
                if ((!definition3.IsStatic && !definition3.IsPrivate) && !definition3.IsSpecialName)
                {
                    FieldReference field = Weaver.scriptDef.MainModule.ImportReference(definition3);
                    TypeDefinition definition4 = field.FieldType.Resolve();
                    MethodReference readFunc = Weaver.GetReadFunc(definition3.FieldType);
                    if (readFunc != null)
                    {
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloca, 0));
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Call, readFunc));
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Stfld, field));
                    }
                    else
                    {
                        Weaver.fail = true;
                        Log.Error(string.Concat(new object[] { "GenerateDeserialization for ", this.m_TypeDef.Name, " unknown type [", definition4, "]. UNet [SyncVar] member variables must be basic types." }));
                        return null;
                    }
                }
            }
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            this.m_TypeDef.Methods.Add(item);
            return item;
        }

        private void GenerateReadFunc(MethodReference readItemFunc)
        {
            string name = "_ReadStruct" + this.m_TypeDef.Name + "_";
            if (this.m_TypeDef.DeclaringType != null)
            {
                name = name + this.m_TypeDef.DeclaringType.Name;
            }
            else
            {
                name = name + "None";
            }
            MethodDefinition newReaderFunc = new MethodDefinition(name, MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Static, Weaver.voidType) {
                Parameters = { 
                    new ParameterDefinition("reader", ParameterAttributes.None, Weaver.scriptDef.MainModule.ImportReference(Weaver.NetworkReaderType)),
                    new ParameterDefinition("instance", ParameterAttributes.None, this.m_TypeDef)
                },
                Body = { 
                    Variables = { 
                        new VariableDefinition("v0", Weaver.uint16Type),
                        new VariableDefinition("v1", Weaver.uint16Type)
                    },
                    InitLocals = true
                }
            };
            ILProcessor iLProcessor = newReaderFunc.Body.GetILProcessor();
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkReadUInt16));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
            TypeReference[] arguments = new TypeReference[] { this.m_ItemType };
            MethodReference method = Helpers.MakeHostInstanceGeneric(Weaver.SyncListClear, arguments);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, method));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_1));
            Instruction target = iLProcessor.Create(OpCodes.Nop);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Br, target));
            Instruction instruction = iLProcessor.Create(OpCodes.Nop);
            iLProcessor.Append(instruction);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, readItemFunc));
            TypeReference[] referenceArray2 = new TypeReference[] { this.m_ItemType };
            MethodReference reference3 = Helpers.MakeHostInstanceGeneric(Weaver.ResolveMethod(Weaver.SyncListStructType, "AddInternal"), referenceArray2);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, reference3));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Add));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Conv_U2));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_1));
            iLProcessor.Append(target);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Blt, instruction));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            Weaver.RegisterReadByReferenceFunc(this.m_TypeDef.FullName, newReaderFunc);
        }

        private MethodReference GenerateSerialization()
        {
            Weaver.DLog(this.m_TypeDef, "  GenerateSerialization", new object[0]);
            foreach (MethodDefinition definition in this.m_TypeDef.Methods)
            {
                if (definition.Name == "SerializeItem")
                {
                    return definition;
                }
            }
            MethodDefinition item = new MethodDefinition("SerializeItem", MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual, Weaver.voidType) {
                Parameters = { 
                    new ParameterDefinition("writer", ParameterAttributes.None, Weaver.scriptDef.MainModule.ImportReference(Weaver.NetworkWriterType)),
                    new ParameterDefinition("item", ParameterAttributes.None, this.m_ItemType)
                }
            };
            ILProcessor iLProcessor = item.Body.GetILProcessor();
            foreach (FieldDefinition definition3 in this.m_ItemType.Resolve().Fields)
            {
                if ((!definition3.IsStatic && !definition3.IsPrivate) && !definition3.IsSpecialName)
                {
                    FieldReference field = Weaver.scriptDef.MainModule.ImportReference(definition3);
                    TypeDefinition definition4 = field.FieldType.Resolve();
                    if (definition4.HasGenericParameters)
                    {
                        Weaver.fail = true;
                        Log.Error(string.Concat(new object[] { "GenerateSerialization for ", this.m_TypeDef.Name, " [", definition4, "/", definition4.FullName, "]. UNet [MessageBase] member cannot have generic parameters." }));
                        return null;
                    }
                    if (definition4.IsInterface)
                    {
                        Weaver.fail = true;
                        Log.Error(string.Concat(new object[] { "GenerateSerialization for ", this.m_TypeDef.Name, " [", definition4, "/", definition4.FullName, "]. UNet [MessageBase] member cannot be an interface." }));
                        return null;
                    }
                    MethodReference writeFunc = Weaver.GetWriteFunc(definition3.FieldType);
                    if (writeFunc != null)
                    {
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_2));
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, field));
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Call, writeFunc));
                    }
                    else
                    {
                        Weaver.fail = true;
                        Log.Error(string.Concat(new object[] { "GenerateSerialization for ", this.m_TypeDef.Name, " unknown type [", definition4, "/", definition4.FullName, "]. UNet [MessageBase] member variables must be basic types." }));
                        return null;
                    }
                }
            }
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            this.m_TypeDef.Methods.Add(item);
            return item;
        }

        private void GenerateWriteFunc(MethodReference writeItemFunc)
        {
            string name = "_WriteStruct" + this.m_TypeDef.GetElementType().Name + "_";
            if (this.m_TypeDef.DeclaringType != null)
            {
                name = name + this.m_TypeDef.DeclaringType.Name;
            }
            else
            {
                name = name + "None";
            }
            MethodDefinition newWriterFunc = new MethodDefinition(name, MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Static, Weaver.voidType) {
                Parameters = { 
                    new ParameterDefinition("writer", ParameterAttributes.None, Weaver.scriptDef.MainModule.ImportReference(Weaver.NetworkWriterType)),
                    new ParameterDefinition("value", ParameterAttributes.None, Weaver.scriptDef.MainModule.ImportReference(this.m_TypeDef))
                },
                Body = { 
                    Variables = { 
                        new VariableDefinition("v0", Weaver.uint16Type),
                        new VariableDefinition("v1", Weaver.uint16Type)
                    },
                    InitLocals = true
                }
            };
            ILProcessor iLProcessor = newWriterFunc.Body.GetILProcessor();
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
            TypeReference[] arguments = new TypeReference[] { this.m_ItemType };
            MethodReference method = Helpers.MakeHostInstanceGeneric(Weaver.ResolveMethod(Weaver.SyncListStructType, "get_Count"), arguments);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, method));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkWriteUInt16));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_1));
            Instruction target = iLProcessor.Create(OpCodes.Nop);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Br, target));
            Instruction instruction = iLProcessor.Create(OpCodes.Nop);
            iLProcessor.Append(instruction);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
            TypeReference[] referenceArray2 = new TypeReference[] { this.m_ItemType };
            MethodReference reference4 = Helpers.MakeHostInstanceGeneric(Weaver.ResolveMethod(Weaver.SyncListStructType, "GetItem"), referenceArray2);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, reference4));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, writeItemFunc));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Add));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Conv_U2));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_1));
            iLProcessor.Append(target);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Blt, instruction));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            Weaver.RegisterWriteFunc(this.m_TypeDef.FullName, newWriterFunc);
        }

        public void Process()
        {
            GenericInstanceType baseType = (GenericInstanceType) this.m_TypeDef.BaseType;
            if (baseType.GenericArguments.Count == 0)
            {
                Weaver.fail = true;
                Log.Error("SyncListStructProcessor no generic args");
            }
            else
            {
                this.m_ItemType = Weaver.scriptDef.MainModule.ImportReference(baseType.GenericArguments[0]);
                Weaver.DLog(this.m_TypeDef, "SyncListStructProcessor Start item:" + this.m_ItemType.FullName, new object[0]);
                Weaver.ResetRecursionCount();
                MethodReference writeItemFunc = this.GenerateSerialization();
                if (!Weaver.fail)
                {
                    MethodReference readItemFunc = this.GenerateDeserialization();
                    if ((readItemFunc != null) && (writeItemFunc != null))
                    {
                        this.GenerateReadFunc(readItemFunc);
                        this.GenerateWriteFunc(writeItemFunc);
                        Weaver.DLog(this.m_TypeDef, "SyncListStructProcessor Done", new object[0]);
                    }
                }
            }
        }
    }
}

