namespace Unity.UNetWeaver
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;

    internal class MessageClassProcessor
    {
        private TypeDefinition m_td;

        public MessageClassProcessor(TypeDefinition td)
        {
            Weaver.DLog(td, "MessageClassProcessor for " + td.Name, new object[0]);
            this.m_td = td;
        }

        private void GenerateDeSerialization()
        {
            Weaver.DLog(this.m_td, "  GenerateDeserialization", new object[0]);
            foreach (MethodDefinition definition in this.m_td.Methods)
            {
                if (definition.Name == "Deserialize")
                {
                    return;
                }
            }
            if (this.m_td.Fields.Count != 0)
            {
                MethodDefinition item = new MethodDefinition("Deserialize", MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual, Weaver.voidType) {
                    Parameters = { new ParameterDefinition("reader", ParameterAttributes.None, Weaver.scriptDef.MainModule.ImportReference(Weaver.NetworkReaderType)) }
                };
                ILProcessor iLProcessor = item.Body.GetILProcessor();
                foreach (FieldDefinition definition3 in this.m_td.Fields)
                {
                    if ((!definition3.IsStatic && !definition3.IsPrivate) && !definition3.IsSpecialName)
                    {
                        MethodReference readFunc = Weaver.GetReadFunc(definition3.FieldType);
                        if (readFunc != null)
                        {
                            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
                            iLProcessor.Append(iLProcessor.Create(OpCodes.Call, readFunc));
                            iLProcessor.Append(iLProcessor.Create(OpCodes.Stfld, definition3));
                        }
                        else
                        {
                            Weaver.fail = true;
                            Log.Error(string.Concat(new object[] { "GenerateDeSerialization for ", this.m_td.Name, " unknown type [", definition3.FieldType, "]. UNet [SyncVar] member variables must be basic types." }));
                            return;
                        }
                    }
                }
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
                this.m_td.Methods.Add(item);
            }
        }

        private void GenerateSerialization()
        {
            Weaver.DLog(this.m_td, "  GenerateSerialization", new object[0]);
            foreach (MethodDefinition definition in this.m_td.Methods)
            {
                if (definition.Name == "Serialize")
                {
                    return;
                }
            }
            if (this.m_td.Fields.Count != 0)
            {
                MethodDefinition item = new MethodDefinition("Serialize", MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual, Weaver.voidType) {
                    Parameters = { new ParameterDefinition("writer", ParameterAttributes.None, Weaver.scriptDef.MainModule.ImportReference(Weaver.NetworkWriterType)) }
                };
                ILProcessor iLProcessor = item.Body.GetILProcessor();
                foreach (FieldDefinition definition3 in this.m_td.Fields)
                {
                    if ((!definition3.IsStatic && !definition3.IsPrivate) && !definition3.IsSpecialName)
                    {
                        if (definition3.FieldType.Resolve().HasGenericParameters)
                        {
                            Weaver.fail = true;
                            Log.Error(string.Concat(new object[] { "GenerateSerialization for ", this.m_td.Name, " [", definition3.FieldType, "/", definition3.FieldType.FullName, "]. UNet [MessageBase] member cannot have generic parameters." }));
                            return;
                        }
                        if (definition3.FieldType.Resolve().IsInterface)
                        {
                            Weaver.fail = true;
                            Log.Error(string.Concat(new object[] { "GenerateSerialization for ", this.m_td.Name, " [", definition3.FieldType, "/", definition3.FieldType.FullName, "]. UNet [MessageBase] member cannot be an interface." }));
                            return;
                        }
                        MethodReference writeFunc = Weaver.GetWriteFunc(definition3.FieldType);
                        if (writeFunc != null)
                        {
                            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
                            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, definition3));
                            iLProcessor.Append(iLProcessor.Create(OpCodes.Call, writeFunc));
                        }
                        else
                        {
                            Weaver.fail = true;
                            Log.Error(string.Concat(new object[] { "GenerateSerialization for ", this.m_td.Name, " unknown type [", definition3.FieldType, "/", definition3.FieldType.FullName, "]. UNet [MessageBase] member variables must be basic types." }));
                            return;
                        }
                    }
                }
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
                this.m_td.Methods.Add(item);
            }
        }

        public void Process()
        {
            Weaver.DLog(this.m_td, "MessageClassProcessor Start", new object[0]);
            Weaver.ResetRecursionCount();
            this.GenerateSerialization();
            if (!Weaver.fail)
            {
                this.GenerateDeSerialization();
                Weaver.DLog(this.m_td, "MessageClassProcessor Done", new object[0]);
            }
        }
    }
}

