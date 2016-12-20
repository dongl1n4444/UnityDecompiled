namespace Unity.UNetWeaver
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class NetworkBehaviourProcessor
    {
        private const string k_CmdPrefix = "InvokeCmd";
        private const string k_RpcPrefix = "InvokeRpc";
        private const int k_SyncVarLimit = 0x20;
        private const string k_TargetRpcPrefix = "InvokeTargetRpc";
        private List<MethodDefinition> m_CmdCallFuncs = new List<MethodDefinition>();
        private List<MethodDefinition> m_CmdInvocationFuncs = new List<MethodDefinition>();
        private List<MethodDefinition> m_Cmds = new List<MethodDefinition>();
        private List<MethodDefinition> m_EventInvocationFuncs = new List<MethodDefinition>();
        private List<EventDefinition> m_Events = new List<EventDefinition>();
        private int m_NetIdFieldCounter;
        private int m_QosChannel;
        private List<MethodDefinition> m_RpcCallFuncs = new List<MethodDefinition>();
        private List<MethodDefinition> m_RpcInvocationFuncs = new List<MethodDefinition>();
        private List<MethodDefinition> m_Rpcs = new List<MethodDefinition>();
        private List<MethodDefinition> m_SyncListInvocationFuncs = new List<MethodDefinition>();
        private List<FieldDefinition> m_SyncLists = new List<FieldDefinition>();
        private List<FieldDefinition> m_SyncListStaticFields = new List<FieldDefinition>();
        private List<FieldDefinition> m_SyncVarNetIds = new List<FieldDefinition>();
        private List<FieldDefinition> m_SyncVars = new List<FieldDefinition>();
        private List<MethodDefinition> m_TargetRpcCallFuncs = new List<MethodDefinition>();
        private List<MethodDefinition> m_TargetRpcInvocationFuncs = new List<MethodDefinition>();
        private List<MethodDefinition> m_TargetRpcs = new List<MethodDefinition>();
        private TypeDefinition m_td;

        public NetworkBehaviourProcessor(TypeDefinition td)
        {
            Weaver.DLog(td, "NetworkBehaviourProcessor", new object[0]);
            this.m_td = td;
        }

        private static void AddInvokeParameters(ICollection<ParameterDefinition> collection)
        {
            collection.Add(new ParameterDefinition("obj", ParameterAttributes.None, Weaver.NetworkBehaviourType2));
            collection.Add(new ParameterDefinition("reader", ParameterAttributes.None, Weaver.scriptDef.MainModule.ImportReference(Weaver.NetworkReaderType)));
        }

        private bool CheckForHookFunction(FieldDefinition syncVar, out MethodDefinition foundMethod)
        {
            foundMethod = null;
            foreach (CustomAttribute attribute in syncVar.CustomAttributes)
            {
                if (attribute.AttributeType.FullName == Weaver.SyncVarType.FullName)
                {
                    foreach (CustomAttributeNamedArgument argument in attribute.Fields)
                    {
                        if (argument.Name == "hook")
                        {
                            string str = argument.Argument.Value as string;
                            foreach (MethodDefinition definition in this.m_td.Methods)
                            {
                                if (definition.Name == str)
                                {
                                    if (definition.Parameters.Count == 1)
                                    {
                                        if (definition.Parameters[0].ParameterType != syncVar.FieldType)
                                        {
                                            Log.Error("SyncVar Hook function " + str + " has wrong type signature for " + this.m_td.Name);
                                            Weaver.fail = true;
                                            return false;
                                        }
                                        foundMethod = definition;
                                        return true;
                                    }
                                    Log.Error("SyncVar Hook function " + str + " must have one argument " + this.m_td.Name);
                                    Weaver.fail = true;
                                    return false;
                                }
                            }
                            Log.Error("SyncVar Hook function " + str + " not found for " + this.m_td.Name);
                            Weaver.fail = true;
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void GenerateCommandDelegate(ILProcessor awakeWorker, MethodReference registerMethod, MethodDefinition func, FieldReference field)
        {
            awakeWorker.Append(awakeWorker.Create(OpCodes.Ldtoken, this.m_td));
            awakeWorker.Append(awakeWorker.Create(OpCodes.Call, Weaver.getTypeFromHandleReference));
            awakeWorker.Append(awakeWorker.Create(OpCodes.Ldsfld, field));
            awakeWorker.Append(awakeWorker.Create(OpCodes.Ldnull));
            awakeWorker.Append(awakeWorker.Create(OpCodes.Ldftn, func));
            awakeWorker.Append(awakeWorker.Create(OpCodes.Newobj, Weaver.CmdDelegateConstructor));
            awakeWorker.Append(awakeWorker.Create(OpCodes.Call, registerMethod));
        }

        private void GenerateConstants()
        {
            if ((((this.m_Cmds.Count != 0) || (this.m_Rpcs.Count != 0)) || ((this.m_TargetRpcs.Count != 0) || (this.m_Events.Count != 0))) || (this.m_SyncLists.Count != 0))
            {
                Weaver.DLog(this.m_td, "  GenerateConstants ", new object[0]);
                MethodDefinition item = null;
                bool flag = false;
                foreach (MethodDefinition definition2 in this.m_td.Methods)
                {
                    if (definition2.Name == ".cctor")
                    {
                        item = definition2;
                        flag = true;
                    }
                }
                if (item != null)
                {
                    if (item.Body.Instructions.Count != 0)
                    {
                        Instruction instruction = item.Body.Instructions[item.Body.Instructions.Count - 1];
                        if (instruction.OpCode != OpCodes.Ret)
                        {
                            Log.Error("No cctor for " + this.m_td.Name);
                            Weaver.fail = true;
                            return;
                        }
                        item.Body.Instructions.RemoveAt(item.Body.Instructions.Count - 1);
                    }
                }
                else
                {
                    item = new MethodDefinition(".cctor", MethodAttributes.CompilerControlled | MethodAttributes.HideBySig | MethodAttributes.Private | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.Static, Weaver.voidType);
                }
                ILProcessor iLProcessor = item.Body.GetILProcessor();
                int num = 0;
                foreach (MethodDefinition definition3 in this.m_Cmds)
                {
                    FieldReference field = Weaver.ResolveField(this.m_td, "kCmd" + definition3.Name);
                    int hashCode = GetHashCode(this.m_td.Name + ":Cmd:" + definition3.Name);
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, hashCode));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Stsfld, field));
                    this.GenerateCommandDelegate(iLProcessor, Weaver.registerCommandDelegateReference, this.m_CmdInvocationFuncs[num], field);
                    num++;
                }
                int num3 = 0;
                foreach (MethodDefinition definition4 in this.m_Rpcs)
                {
                    FieldReference reference2 = Weaver.ResolveField(this.m_td, "kRpc" + definition4.Name);
                    int num4 = GetHashCode(this.m_td.Name + ":Rpc:" + definition4.Name);
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, num4));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Stsfld, reference2));
                    this.GenerateCommandDelegate(iLProcessor, Weaver.registerRpcDelegateReference, this.m_RpcInvocationFuncs[num3], reference2);
                    num3++;
                }
                int num5 = 0;
                foreach (MethodDefinition definition5 in this.m_TargetRpcs)
                {
                    FieldReference reference3 = Weaver.ResolveField(this.m_td, "kTargetRpc" + definition5.Name);
                    int num6 = GetHashCode(this.m_td.Name + ":TargetRpc:" + definition5.Name);
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, num6));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Stsfld, reference3));
                    this.GenerateCommandDelegate(iLProcessor, Weaver.registerRpcDelegateReference, this.m_TargetRpcInvocationFuncs[num5], reference3);
                    num5++;
                }
                int num7 = 0;
                foreach (EventDefinition definition6 in this.m_Events)
                {
                    FieldReference reference4 = Weaver.ResolveField(this.m_td, "kEvent" + definition6.Name);
                    int num8 = GetHashCode(this.m_td.Name + ":Event:" + definition6.Name);
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, num8));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Stsfld, reference4));
                    this.GenerateCommandDelegate(iLProcessor, Weaver.registerEventDelegateReference, this.m_EventInvocationFuncs[num7], reference4);
                    num7++;
                }
                int num9 = 0;
                foreach (FieldDefinition definition7 in this.m_SyncLists)
                {
                    FieldReference reference5 = Weaver.ResolveField(this.m_td, "kList" + definition7.Name);
                    int num10 = GetHashCode(this.m_td.Name + ":List:" + definition7.Name);
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, num10));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Stsfld, reference5));
                    this.GenerateCommandDelegate(iLProcessor, Weaver.registerSyncListDelegateReference, this.m_SyncListInvocationFuncs[num9], reference5);
                    num9++;
                }
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ldstr, this.m_td.Name));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, this.m_QosChannel));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.RegisterBehaviourReference));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
                if (!flag)
                {
                    this.m_td.Methods.Add(item);
                }
                this.m_td.Attributes &= ~(TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit);
                if (this.m_SyncLists.Count != 0)
                {
                    MethodDefinition definition8 = null;
                    bool flag2 = false;
                    foreach (MethodDefinition definition9 in this.m_td.Methods)
                    {
                        if (definition9.Name == "Awake")
                        {
                            definition8 = definition9;
                            flag2 = true;
                        }
                    }
                    if (definition8 != null)
                    {
                        if (definition8.Body.Instructions.Count != 0)
                        {
                            Instruction instruction2 = definition8.Body.Instructions[definition8.Body.Instructions.Count - 1];
                            if (instruction2.OpCode != OpCodes.Ret)
                            {
                                Log.Error("No ctor for " + this.m_td.Name);
                                Weaver.fail = true;
                                return;
                            }
                            definition8.Body.Instructions.RemoveAt(definition8.Body.Instructions.Count - 1);
                        }
                    }
                    else
                    {
                        definition8 = new MethodDefinition("Awake", MethodAttributes.CompilerControlled | MethodAttributes.Private, Weaver.voidType);
                    }
                    ILProcessor awakeWorker = definition8.Body.GetILProcessor();
                    int index = 0;
                    foreach (FieldDefinition definition10 in this.m_SyncLists)
                    {
                        this.GenerateSyncListInitializer(awakeWorker, definition10, index);
                        index++;
                    }
                    awakeWorker.Append(awakeWorker.Create(OpCodes.Ret));
                    if (!flag2)
                    {
                        this.m_td.Methods.Add(definition8);
                    }
                }
            }
        }

        private void GenerateDeSerialization()
        {
            Weaver.DLog(this.m_td, "  GenerateDeSerialization", new object[0]);
            this.m_NetIdFieldCounter = 0;
            foreach (MethodDefinition definition in this.m_td.Methods)
            {
                if (definition.Name == "OnDeserialize")
                {
                    return;
                }
            }
            MethodDefinition item = new MethodDefinition("OnDeserialize", MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual, Weaver.voidType) {
                Parameters = { 
                    new ParameterDefinition("reader", ParameterAttributes.None, Weaver.scriptDef.MainModule.ImportReference(Weaver.NetworkReaderType)),
                    new ParameterDefinition("initialState", ParameterAttributes.None, Weaver.boolType)
                }
            };
            ILProcessor iLProcessor = item.Body.GetILProcessor();
            if (this.m_td.BaseType.FullName != Weaver.NetworkBehaviourType.FullName)
            {
                MethodReference method = Weaver.ResolveMethod(this.m_td.BaseType, "OnDeserialize");
                if (method != null)
                {
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_2));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Call, method));
                }
            }
            if (this.m_SyncVars.Count == 0)
            {
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
                this.m_td.Methods.Add(item);
            }
            else
            {
                Instruction target = iLProcessor.Create(OpCodes.Nop);
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_2));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Brfalse, target));
                foreach (FieldDefinition definition3 in this.m_SyncVars)
                {
                    MethodReference readByReferenceFunc = Weaver.GetReadByReferenceFunc(definition3.FieldType);
                    if (readByReferenceFunc != null)
                    {
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, definition3));
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Call, readByReferenceFunc));
                    }
                    else
                    {
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
                        if (definition3.FieldType.FullName == Weaver.gameObjectType.FullName)
                        {
                            FieldDefinition field = this.m_SyncVarNetIds[this.m_NetIdFieldCounter];
                            this.m_NetIdFieldCounter++;
                            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkReaderReadNetworkInstanceId));
                            iLProcessor.Append(iLProcessor.Create(OpCodes.Stfld, field));
                        }
                        else
                        {
                            MethodReference readFunc = Weaver.GetReadFunc(definition3.FieldType);
                            if (readFunc != null)
                            {
                                iLProcessor.Append(iLProcessor.Create(OpCodes.Call, readFunc));
                            }
                            else
                            {
                                Log.Error(string.Concat(new object[] { "GenerateDeSerialization for ", this.m_td.Name, " unknown type [", definition3.FieldType, "]. UNet [SyncVar] member variables must be basic types." }));
                                Weaver.fail = true;
                                return;
                            }
                            iLProcessor.Append(iLProcessor.Create(OpCodes.Stfld, definition3));
                        }
                    }
                }
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
                iLProcessor.Append(target);
                item.Body.InitLocals = true;
                VariableDefinition definition5 = new VariableDefinition(Weaver.int32Type);
                item.Body.Variables.Add(definition5);
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkReaderReadPacked32));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_0));
                int syncVarStart = Weaver.GetSyncVarStart(this.m_td.BaseType.FullName);
                foreach (FieldDefinition definition6 in this.m_SyncVars)
                {
                    Instruction instruction2 = iLProcessor.Create(OpCodes.Nop);
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, (int) (((int) 1) << syncVarStart)));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.And));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Brfalse, instruction2));
                    MethodReference reference4 = Weaver.GetReadByReferenceFunc(definition6.FieldType);
                    if (reference4 != null)
                    {
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, definition6));
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Call, reference4));
                    }
                    else
                    {
                        MethodDefinition definition7;
                        MethodReference reference5 = Weaver.GetReadFunc(definition6.FieldType);
                        if (reference5 == null)
                        {
                            Log.Error(string.Concat(new object[] { "GenerateDeSerialization for ", this.m_td.Name, " unknown type [", definition6.FieldType, "]. UNet [SyncVar] member variables must be basic types." }));
                            Weaver.fail = true;
                            return;
                        }
                        if (!this.CheckForHookFunction(definition6, out definition7))
                        {
                            return;
                        }
                        if (definition7 == null)
                        {
                            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
                            iLProcessor.Append(iLProcessor.Create(OpCodes.Call, reference5));
                            iLProcessor.Append(iLProcessor.Create(OpCodes.Stfld, definition6));
                        }
                        else
                        {
                            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
                            iLProcessor.Append(iLProcessor.Create(OpCodes.Call, reference5));
                            iLProcessor.Append(iLProcessor.Create(OpCodes.Call, definition7));
                        }
                    }
                    iLProcessor.Append(instruction2);
                    syncVarStart++;
                }
                if (Weaver.generateLogErrors)
                {
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldstr, "Injected Deserialize " + this.m_td.Name));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.logErrorReference));
                }
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
                this.m_td.Methods.Add(item);
            }
        }

        private void GenerateNetworkChannelSetting(int channel)
        {
            MethodDefinition item = new MethodDefinition("GetNetworkChannel", MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual, Weaver.int32Type);
            ILProcessor iLProcessor = item.Body.GetILProcessor();
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, channel));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            this.m_td.Methods.Add(item);
        }

        private void GenerateNetworkIntervalSetting(float interval)
        {
            MethodDefinition item = new MethodDefinition("GetNetworkSendInterval", MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual, Weaver.singleType);
            ILProcessor iLProcessor = item.Body.GetILProcessor();
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_R4, interval));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            this.m_td.Methods.Add(item);
        }

        private void GenerateNetworkSettings()
        {
            foreach (CustomAttribute attribute in this.m_td.CustomAttributes)
            {
                if (attribute.AttributeType.FullName == Weaver.NetworkSettingsType.FullName)
                {
                    foreach (CustomAttributeNamedArgument argument in attribute.Fields)
                    {
                        if (argument.Name == "channel")
                        {
                            if (((int) argument.Argument.Value) == 0)
                            {
                                continue;
                            }
                            if (this.HasMethod("GetNetworkChannel"))
                            {
                                Log.Error("GetNetworkChannel, is already implemented, please make sure you either use NetworkSettings or GetNetworkChannel");
                                Weaver.fail = true;
                                break;
                            }
                            this.m_QosChannel = (int) argument.Argument.Value;
                            this.GenerateNetworkChannelSetting(this.m_QosChannel);
                        }
                        if ((argument.Name == "sendInterval") && (Math.Abs((float) (((float) argument.Argument.Value) - 0.1f)) > 1E-05f))
                        {
                            if (this.HasMethod("GetNetworkSendInterval"))
                            {
                                Log.Error("GetNetworkSendInterval, is already implemented, please make sure you either use NetworkSettings or GetNetworkSendInterval");
                                Weaver.fail = true;
                                break;
                            }
                            this.GenerateNetworkIntervalSetting((float) argument.Argument.Value);
                        }
                    }
                }
            }
        }

        private void GeneratePreStartClient()
        {
            this.m_NetIdFieldCounter = 0;
            MethodDefinition item = null;
            ILProcessor iLProcessor = null;
            foreach (MethodDefinition definition2 in this.m_td.Methods)
            {
                if (definition2.Name == "PreStartClient")
                {
                    return;
                }
            }
            foreach (FieldDefinition definition3 in this.m_SyncVars)
            {
                if (definition3.FieldType.FullName == Weaver.gameObjectType.FullName)
                {
                    if (item == null)
                    {
                        item = new MethodDefinition("PreStartClient", MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual, Weaver.voidType);
                        iLProcessor = item.Body.GetILProcessor();
                    }
                    FieldDefinition field = this.m_SyncVarNetIds[this.m_NetIdFieldCounter];
                    this.m_NetIdFieldCounter++;
                    Instruction target = iLProcessor.Create(OpCodes.Nop);
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldflda, field));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.NetworkInstanceIsEmpty));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Brtrue, target));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, field));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.FindLocalObjectReference));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Stfld, definition3));
                    iLProcessor.Append(target);
                }
            }
            if (item != null)
            {
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
                this.m_td.Methods.Add(item);
            }
        }

        private void GenerateSerialization()
        {
            Weaver.DLog(this.m_td, "  GenerateSerialization", new object[0]);
            foreach (MethodDefinition definition in this.m_td.Methods)
            {
                if (definition.Name == "OnSerialize")
                {
                    return;
                }
            }
            MethodDefinition item = new MethodDefinition("OnSerialize", MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual, Weaver.boolType) {
                Parameters = { 
                    new ParameterDefinition("writer", ParameterAttributes.None, Weaver.scriptDef.MainModule.ImportReference(Weaver.NetworkWriterType)),
                    new ParameterDefinition("forceAll", ParameterAttributes.None, Weaver.boolType)
                }
            };
            ILProcessor iLProcessor = item.Body.GetILProcessor();
            item.Body.InitLocals = true;
            VariableDefinition definition3 = new VariableDefinition(Weaver.boolType);
            item.Body.Variables.Add(definition3);
            bool flag = false;
            if (this.m_td.BaseType.FullName != Weaver.NetworkBehaviourType.FullName)
            {
                MethodReference method = Weaver.ResolveMethod(this.m_td.BaseType, "OnSerialize");
                if (method != null)
                {
                    VariableDefinition definition4 = new VariableDefinition(Weaver.boolType);
                    item.Body.Variables.Add(definition4);
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_2));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Call, method));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_1));
                    flag = true;
                }
            }
            if (this.m_SyncVars.Count == 0)
            {
                if (flag)
                {
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Or));
                }
                else
                {
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
                }
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
                this.m_td.Methods.Add(item);
            }
            else
            {
                Instruction target = iLProcessor.Create(OpCodes.Nop);
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_2));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Brfalse, target));
                foreach (FieldDefinition definition5 in this.m_SyncVars)
                {
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, definition5));
                    MethodReference writeFunc = Weaver.GetWriteFunc(definition5.FieldType);
                    if (writeFunc != null)
                    {
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Call, writeFunc));
                    }
                    else
                    {
                        Weaver.fail = true;
                        Log.Error(string.Concat(new object[] { "GenerateSerialization for ", this.m_td.Name, " unknown type [", definition5.FieldType, "]. UNet [SyncVar] member variables must be basic types." }));
                        return;
                    }
                }
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_1));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
                iLProcessor.Append(target);
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_0));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_0));
                int syncVarStart = Weaver.GetSyncVarStart(this.m_td.BaseType.FullName);
                foreach (FieldDefinition definition6 in this.m_SyncVars)
                {
                    Instruction instruction2 = iLProcessor.Create(OpCodes.Nop);
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.NetworkBehaviourDirtyBitsReference));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, (int) (((int) 1) << syncVarStart)));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.And));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Brfalse, instruction2));
                    WriteDirtyCheck(iLProcessor, true);
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, definition6));
                    MethodReference reference3 = Weaver.GetWriteFunc(definition6.FieldType);
                    if (reference3 != null)
                    {
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Call, reference3));
                    }
                    else
                    {
                        Log.Error(string.Concat(new object[] { "GenerateSerialization for ", this.m_td.Name, " unknown type [", definition6.FieldType, "]. UNet [SyncVar] member variables must be basic types." }));
                        Weaver.fail = true;
                        return;
                    }
                    iLProcessor.Append(instruction2);
                    syncVarStart++;
                }
                WriteDirtyCheck(iLProcessor, false);
                if (Weaver.generateLogErrors)
                {
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldstr, "Injected Serialize " + this.m_td.Name));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.logErrorReference));
                }
                if (flag)
                {
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Or));
                }
                else
                {
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
                }
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
                this.m_td.Methods.Add(item);
            }
        }

        private void GenerateSyncListInitializer(ILProcessor awakeWorker, FieldReference fd, int index)
        {
            awakeWorker.Append(awakeWorker.Create(OpCodes.Ldarg_0));
            awakeWorker.Append(awakeWorker.Create(OpCodes.Ldfld, fd));
            awakeWorker.Append(awakeWorker.Create(OpCodes.Ldarg_0));
            awakeWorker.Append(awakeWorker.Create(OpCodes.Ldsfld, this.m_SyncListStaticFields[index]));
            GenericInstanceType baseType = (GenericInstanceType) fd.FieldType.Resolve().BaseType;
            baseType = (GenericInstanceType) Weaver.scriptDef.MainModule.ImportReference(baseType);
            TypeReference reference = baseType.GenericArguments[0];
            TypeReference[] arguments = new TypeReference[] { reference };
            MethodReference method = Helpers.MakeHostInstanceGeneric(Weaver.SyncListInitBehaviourReference, arguments);
            awakeWorker.Append(awakeWorker.Create(OpCodes.Callvirt, method));
            Weaver.scriptDef.MainModule.ImportReference(method);
        }

        private static int GetChannelId(FieldDefinition field)
        {
            int num = 0;
            foreach (CustomAttribute attribute in field.CustomAttributes)
            {
                if (attribute.AttributeType.FullName == Weaver.SyncVarType.FullName)
                {
                    foreach (CustomAttributeNamedArgument argument in attribute.Fields)
                    {
                        if (argument.Name == "channel")
                        {
                            num = (int) argument.Argument.Value;
                            break;
                        }
                    }
                }
            }
            return num;
        }

        private static unsafe int GetHashCode(string s)
        {
            int length = s.Length;
            fixed (char* str = ((char*) s))
            {
                char* chPtr2 = str + RuntimeHelpers.OffsetToStringData;
                char* chPtr3 = (chPtr2 + length) - 1;
                int num2 = 0;
                while (chPtr2 < chPtr3)
                {
                    num2 = ((num2 << 5) - num2) + chPtr2[0];
                    num2 = ((num2 << 5) - num2) + chPtr2[1];
                    chPtr2 += 2;
                }
                chPtr3++;
                if (chPtr2 < chPtr3)
                {
                    num2 = ((num2 << 5) - num2) + chPtr2[0];
                }
                return num2;
            }
        }

        private bool HasMethod(string name)
        {
            foreach (MethodDefinition definition in this.m_td.Methods)
            {
                if (definition.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public void Process()
        {
            if (this.m_td.HasGenericParameters)
            {
                Weaver.fail = true;
                Log.Error("NetworkBehaviour " + this.m_td.Name + " cannot have generic parameters");
            }
            else
            {
                Weaver.DLog(this.m_td, "Process Start", new object[0]);
                this.ProcessVersion();
                this.ProcessSyncVars();
                Weaver.ResetRecursionCount();
                this.ProcessMethods();
                this.ProcessEvents();
                if (!Weaver.fail)
                {
                    this.GenerateNetworkSettings();
                    this.GenerateConstants();
                    Weaver.ResetRecursionCount();
                    this.GenerateSerialization();
                    if (!Weaver.fail)
                    {
                        this.GenerateDeSerialization();
                        this.GeneratePreStartClient();
                        Weaver.DLog(this.m_td, "Process Done", new object[0]);
                    }
                }
            }
        }

        private MethodDefinition ProcessCommandCall(MethodDefinition md, CustomAttribute ca)
        {
            MethodDefinition definition = new MethodDefinition("Call" + md.Name, MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig, Weaver.voidType);
            foreach (ParameterDefinition definition2 in md.Parameters)
            {
                definition.Parameters.Add(new ParameterDefinition(definition2.Name, ParameterAttributes.None, definition2.ParameterType));
            }
            ILProcessor iLProcessor = definition.Body.GetILProcessor();
            Instruction label = iLProcessor.Create(OpCodes.Nop);
            WriteSetupLocals(iLProcessor);
            if (Weaver.generateLogErrors)
            {
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ldstr, "Call Command function " + md.Name));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.logErrorReference));
            }
            WriteClientActiveCheck(iLProcessor, md.Name, label, "Command function");
            Instruction target = iLProcessor.Create(OpCodes.Nop);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.UBehaviourIsServer));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Brfalse, target));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            for (int i = 0; i < md.Parameters.Count; i++)
            {
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg, (int) (i + 1)));
            }
            iLProcessor.Append(iLProcessor.Create(OpCodes.Call, md));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            iLProcessor.Append(target);
            WriteCreateWriter(iLProcessor);
            WriteMessageSize(iLProcessor);
            WriteMessageId(iLProcessor, 5);
            FieldDefinition item = new FieldDefinition("kCmd" + md.Name, FieldAttributes.CompilerControlled | FieldAttributes.Private | FieldAttributes.Static, Weaver.int32Type);
            this.m_td.Fields.Add(item);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldsfld, item));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkWriterWritePacked32));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.getComponentReference));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.getUNetIdReference));
            MethodReference writeFunc = Weaver.GetWriteFunc(Weaver.NetworkInstanceIdType);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, writeFunc));
            if (!WriteArguments(iLProcessor, md, "Command", false))
            {
                return null;
            }
            int num2 = 0;
            foreach (CustomAttributeNamedArgument argument in ca.Fields)
            {
                if (argument.Name == "channel")
                {
                    num2 = (int) argument.Argument.Value;
                }
            }
            string name = md.Name;
            if (name.IndexOf("InvokeCmd") > -1)
            {
                name = name.Substring("InvokeCmd".Length);
            }
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, num2));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldstr, name));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.sendCommandInternal));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            return definition;
        }

        private MethodDefinition ProcessCommandInvoke(MethodDefinition md)
        {
            MethodDefinition definition = new MethodDefinition("InvokeCmd" + md.Name, MethodAttributes.CompilerControlled | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Static, Weaver.voidType);
            ILProcessor iLProcessor = definition.Body.GetILProcessor();
            Instruction label = iLProcessor.Create(OpCodes.Nop);
            WriteServerActiveCheck(iLProcessor, md.Name, label, "Command");
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Castclass, this.m_td));
            if (!this.ProcessNetworkReaderParameters(md, iLProcessor, false))
            {
                return null;
            }
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, md));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            AddInvokeParameters(definition.Parameters);
            return definition;
        }

        private MethodDefinition ProcessEventCall(EventDefinition ed, CustomAttribute ca)
        {
            MethodReference reference = Weaver.ResolveMethod(ed.EventType, "Invoke");
            MethodDefinition definition = new MethodDefinition("Call" + ed.Name, MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig, Weaver.voidType);
            foreach (ParameterDefinition definition2 in reference.Parameters)
            {
                definition.Parameters.Add(new ParameterDefinition(definition2.Name, ParameterAttributes.None, definition2.ParameterType));
            }
            ILProcessor iLProcessor = definition.Body.GetILProcessor();
            Instruction label = iLProcessor.Create(OpCodes.Nop);
            WriteSetupLocals(iLProcessor);
            WriteServerActiveCheck(iLProcessor, ed.Name, label, "Event");
            WriteCreateWriter(iLProcessor);
            WriteMessageSize(iLProcessor);
            WriteMessageId(iLProcessor, 7);
            FieldDefinition item = new FieldDefinition("kEvent" + ed.Name, FieldAttributes.CompilerControlled | FieldAttributes.Private | FieldAttributes.Static, Weaver.int32Type);
            this.m_td.Fields.Add(item);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldsfld, item));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkWriterWritePacked32));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.getComponentReference));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.getUNetIdReference));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkWriterWriteNetworkInstanceId));
            if (!WriteArguments(iLProcessor, reference.Resolve(), "SyncEvent", false))
            {
                return null;
            }
            int num = 0;
            foreach (CustomAttributeNamedArgument argument in ca.Fields)
            {
                if (argument.Name == "channel")
                {
                    num = (int) argument.Argument.Value;
                }
            }
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, num));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldstr, ed.Name));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.sendEventInternal));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            return definition;
        }

        private MethodDefinition ProcessEventInvoke(EventDefinition ed)
        {
            FieldDefinition field = null;
            foreach (FieldDefinition definition2 in this.m_td.Fields)
            {
                if (definition2.FullName == ed.FullName)
                {
                    field = definition2;
                    break;
                }
            }
            if (field == null)
            {
                Weaver.DLog(this.m_td, "ERROR: no event field?!", new object[0]);
                Weaver.fail = true;
                return null;
            }
            MethodDefinition definition4 = new MethodDefinition("InvokeSyncEvent" + ed.Name, MethodAttributes.CompilerControlled | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Static, Weaver.voidType);
            ILProcessor iLProcessor = definition4.Body.GetILProcessor();
            Instruction label = iLProcessor.Create(OpCodes.Nop);
            Instruction target = iLProcessor.Create(OpCodes.Nop);
            WriteClientActiveCheck(iLProcessor, ed.Name, label, "Event");
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Castclass, this.m_td));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, field));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Brtrue, target));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            iLProcessor.Append(target);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Castclass, this.m_td));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, field));
            MethodReference method = Weaver.ResolveMethod(field.FieldType, "Invoke");
            if (!this.ProcessNetworkReaderParameters(method.Resolve(), iLProcessor, false))
            {
                return null;
            }
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, method));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            AddInvokeParameters(definition4.Parameters);
            return definition4;
        }

        private void ProcessEvents()
        {
            foreach (EventDefinition definition in this.m_td.Events)
            {
                foreach (CustomAttribute attribute in definition.CustomAttributes)
                {
                    if (attribute.AttributeType.FullName == Weaver.SyncEventType.FullName)
                    {
                        if ((definition.Name.Length > 4) && (definition.Name.Substring(0, 5) != "Event"))
                        {
                            Log.Error("Event  [" + this.m_td.FullName + ":" + definition.FullName + "] doesnt have 'Event' prefix");
                            Weaver.fail = true;
                            break;
                        }
                        if (definition.EventType.Resolve().HasGenericParameters)
                        {
                            Log.Error("Event  [" + this.m_td.FullName + ":" + definition.FullName + "] cannot have generic parameters");
                            Weaver.fail = true;
                            break;
                        }
                        this.m_Events.Add(definition);
                        MethodDefinition item = this.ProcessEventInvoke(definition);
                        if (item != null)
                        {
                            this.m_td.Methods.Add(item);
                            this.m_EventInvocationFuncs.Add(item);
                            Weaver.DLog(this.m_td, "ProcessEvent " + definition, new object[0]);
                            MethodDefinition definition3 = this.ProcessEventCall(definition, attribute);
                            this.m_td.Methods.Add(definition3);
                            Weaver.lists.replacedEvents.Add(definition);
                            Weaver.lists.replacementEvents.Add(definition3);
                            Weaver.DLog(this.m_td, "  Event: " + definition.Name, new object[0]);
                        }
                        break;
                    }
                }
            }
        }

        private void ProcessMethods()
        {
            HashSet<string> set = new HashSet<string>();
            foreach (MethodDefinition definition in this.m_td.Methods)
            {
                Weaver.ResetRecursionCount();
                foreach (CustomAttribute attribute in definition.CustomAttributes)
                {
                    if (attribute.AttributeType.FullName == Weaver.CommandType.FullName)
                    {
                        if (!this.ProcessMethodsValidateCommand(definition, attribute))
                        {
                            return;
                        }
                        if (set.Contains(definition.Name))
                        {
                            Log.Error("Duplicate Command name [" + this.m_td.FullName + ":" + definition.Name + "]");
                            Weaver.fail = true;
                            return;
                        }
                        set.Add(definition.Name);
                        this.m_Cmds.Add(definition);
                        MethodDefinition item = this.ProcessCommandInvoke(definition);
                        if (item != null)
                        {
                            this.m_CmdInvocationFuncs.Add(item);
                        }
                        MethodDefinition definition3 = this.ProcessCommandCall(definition, attribute);
                        if (definition3 != null)
                        {
                            this.m_CmdCallFuncs.Add(definition3);
                            Weaver.lists.replacedMethods.Add(definition);
                            Weaver.lists.replacementMethods.Add(definition3);
                        }
                        break;
                    }
                    if (attribute.AttributeType.FullName == Weaver.TargetRpcType.FullName)
                    {
                        if (!this.ProcessMethodsValidateTargetRpc(definition, attribute))
                        {
                            return;
                        }
                        if (set.Contains(definition.Name))
                        {
                            Log.Error("Duplicate Target Rpc name [" + this.m_td.FullName + ":" + definition.Name + "]");
                            Weaver.fail = true;
                            return;
                        }
                        set.Add(definition.Name);
                        this.m_TargetRpcs.Add(definition);
                        MethodDefinition definition4 = this.ProcessTargetRpcInvoke(definition);
                        if (definition4 != null)
                        {
                            this.m_TargetRpcInvocationFuncs.Add(definition4);
                        }
                        MethodDefinition definition5 = this.ProcessTargetRpcCall(definition, attribute);
                        if (definition5 != null)
                        {
                            this.m_TargetRpcCallFuncs.Add(definition5);
                            Weaver.lists.replacedMethods.Add(definition);
                            Weaver.lists.replacementMethods.Add(definition5);
                        }
                        break;
                    }
                    if (attribute.AttributeType.FullName == Weaver.ClientRpcType.FullName)
                    {
                        if (!this.ProcessMethodsValidateRpc(definition, attribute))
                        {
                            return;
                        }
                        if (set.Contains(definition.Name))
                        {
                            Log.Error("Duplicate ClientRpc name [" + this.m_td.FullName + ":" + definition.Name + "]");
                            Weaver.fail = true;
                            return;
                        }
                        set.Add(definition.Name);
                        this.m_Rpcs.Add(definition);
                        MethodDefinition definition6 = this.ProcessRpcInvoke(definition);
                        if (definition6 != null)
                        {
                            this.m_RpcInvocationFuncs.Add(definition6);
                        }
                        MethodDefinition definition7 = this.ProcessRpcCall(definition, attribute);
                        if (definition7 != null)
                        {
                            this.m_RpcCallFuncs.Add(definition7);
                            Weaver.lists.replacedMethods.Add(definition);
                            Weaver.lists.replacementMethods.Add(definition7);
                        }
                        break;
                    }
                }
            }
            foreach (MethodDefinition definition8 in this.m_CmdInvocationFuncs)
            {
                this.m_td.Methods.Add(definition8);
            }
            foreach (MethodDefinition definition9 in this.m_CmdCallFuncs)
            {
                this.m_td.Methods.Add(definition9);
            }
            foreach (MethodDefinition definition10 in this.m_RpcInvocationFuncs)
            {
                this.m_td.Methods.Add(definition10);
            }
            foreach (MethodDefinition definition11 in this.m_TargetRpcInvocationFuncs)
            {
                this.m_td.Methods.Add(definition11);
            }
            foreach (MethodDefinition definition12 in this.m_RpcCallFuncs)
            {
                this.m_td.Methods.Add(definition12);
            }
            foreach (MethodDefinition definition13 in this.m_TargetRpcCallFuncs)
            {
                this.m_td.Methods.Add(definition13);
            }
        }

        private bool ProcessMethodsValidateCommand(MethodDefinition md, CustomAttribute ca)
        {
            if ((md.Name.Length > 2) && (md.Name.Substring(0, 3) != "Cmd"))
            {
                Log.Error("Command function [" + this.m_td.FullName + ":" + md.Name + "] doesnt have 'Cmd' prefix");
                Weaver.fail = true;
                return false;
            }
            if (md.IsStatic)
            {
                Log.Error("Command function [" + this.m_td.FullName + ":" + md.Name + "] cant be a static method");
                Weaver.fail = true;
                return false;
            }
            if (!this.ProcessMethodsValidateFunction(md, ca, "Command"))
            {
                return false;
            }
            if (!this.ProcessMethodsValidateParameters(md, ca, "Command"))
            {
                return false;
            }
            return true;
        }

        private bool ProcessMethodsValidateFunction(MethodReference md, CustomAttribute ca, string actionType)
        {
            if (md.ReturnType.FullName == Weaver.IEnumeratorType.FullName)
            {
                Log.Error(actionType + " function [" + this.m_td.FullName + ":" + md.Name + "] cannot be a coroutine");
                Weaver.fail = true;
                return false;
            }
            if (md.ReturnType.FullName != Weaver.voidType.FullName)
            {
                Log.Error(actionType + " function [" + this.m_td.FullName + ":" + md.Name + "] must have a void return type.");
                Weaver.fail = true;
                return false;
            }
            if (md.HasGenericParameters)
            {
                Log.Error(actionType + " [" + this.m_td.FullName + ":" + md.Name + "] cannot have generic parameters");
                Weaver.fail = true;
                return false;
            }
            return true;
        }

        private bool ProcessMethodsValidateParameters(MethodReference md, CustomAttribute ca, string actionType)
        {
            for (int i = 0; i < md.Parameters.Count; i++)
            {
                ParameterDefinition definition = md.Parameters[i];
                if (definition.IsOut)
                {
                    Log.Error(actionType + " function [" + this.m_td.FullName + ":" + md.Name + "] cannot have out parameters");
                    Weaver.fail = true;
                    return false;
                }
                if (definition.IsOptional)
                {
                    Log.Error(actionType + "function [" + this.m_td.FullName + ":" + md.Name + "] cannot have optional parameters");
                    Weaver.fail = true;
                    return false;
                }
                if (definition.ParameterType.Resolve().IsAbstract)
                {
                    Log.Error(actionType + " function [" + this.m_td.FullName + ":" + md.Name + "] cannot have abstract parameters");
                    Weaver.fail = true;
                    return false;
                }
                if (definition.ParameterType.IsByReference)
                {
                    Log.Error(actionType + " function [" + this.m_td.FullName + ":" + md.Name + "] cannot have ref parameters");
                    Weaver.fail = true;
                    return false;
                }
                if ((definition.ParameterType.FullName == Weaver.NetworkConnectionType.FullName) && ((ca.AttributeType.FullName != Weaver.TargetRpcType.FullName) || (i != 0)))
                {
                    Log.Error(actionType + " [" + this.m_td.FullName + ":" + md.Name + "] cannot use a NetworkConnection as a parameter. To access a player object's connection on the server use connectionToClient");
                    Log.Error("Name: " + ca.AttributeType.FullName + " parameter: " + md.Parameters[0].ParameterType.FullName);
                    Weaver.fail = true;
                    return false;
                }
                if (Weaver.IsDerivedFrom(definition.ParameterType.Resolve(), Weaver.ComponentType) && (definition.ParameterType.FullName != Weaver.NetworkIdentityType.FullName))
                {
                    Log.Error(actionType + " function [" + this.m_td.FullName + ":" + md.Name + "] parameter [" + definition.Name + "] is of the type [" + definition.ParameterType.Name + "] which is a Component. You cannot pass a Component to a remote call. Try passing data from within the component.");
                    Weaver.fail = true;
                    return false;
                }
            }
            return true;
        }

        private bool ProcessMethodsValidateRpc(MethodDefinition md, CustomAttribute ca)
        {
            if ((md.Name.Length > 2) && (md.Name.Substring(0, 3) != "Rpc"))
            {
                Log.Error("Rpc function [" + this.m_td.FullName + ":" + md.Name + "] doesnt have 'Rpc' prefix");
                Weaver.fail = true;
                return false;
            }
            if (md.IsStatic)
            {
                Log.Error("ClientRpc function [" + this.m_td.FullName + ":" + md.Name + "] cant be a static method");
                Weaver.fail = true;
                return false;
            }
            if (!this.ProcessMethodsValidateFunction(md, ca, "Rpc"))
            {
                return false;
            }
            if (!this.ProcessMethodsValidateParameters(md, ca, "Rpc"))
            {
                return false;
            }
            return true;
        }

        private bool ProcessMethodsValidateTargetRpc(MethodDefinition md, CustomAttribute ca)
        {
            int length = "Target".Length;
            if ((md.Name.Length > length) && (md.Name.Substring(0, length) != "Target"))
            {
                Log.Error("Target Rpc function [" + this.m_td.FullName + ":" + md.Name + "] doesnt have 'Target' prefix");
                Weaver.fail = true;
                return false;
            }
            if (md.IsStatic)
            {
                Log.Error("TargetRpc function [" + this.m_td.FullName + ":" + md.Name + "] cant be a static method");
                Weaver.fail = true;
                return false;
            }
            if (!this.ProcessMethodsValidateFunction(md, ca, "Target Rpc"))
            {
                return false;
            }
            if (md.Parameters.Count < 1)
            {
                Log.Error("Target Rpc function [" + this.m_td.FullName + ":" + md.Name + "] must have a NetworkConnection as the first parameter");
                Weaver.fail = true;
                return false;
            }
            if (md.Parameters[0].ParameterType.FullName != Weaver.NetworkConnectionType.FullName)
            {
                Log.Error("Target Rpc function [" + this.m_td.FullName + ":" + md.Name + "] first parameter must be a NetworkConnection");
                Weaver.fail = true;
                return false;
            }
            if (!this.ProcessMethodsValidateParameters(md, ca, "Target Rpc"))
            {
                return false;
            }
            return true;
        }

        private bool ProcessNetworkReaderParameters(MethodDefinition md, ILProcessor worker, bool skipFirst)
        {
            int num = 0;
            foreach (ParameterDefinition definition in md.Parameters)
            {
                if ((num++ != 0) || !skipFirst)
                {
                    MethodReference readFunc = Weaver.GetReadFunc(definition.ParameterType);
                    if (readFunc != null)
                    {
                        worker.Append(worker.Create(OpCodes.Ldarg_1));
                        worker.Append(worker.Create(OpCodes.Call, readFunc));
                        if (definition.ParameterType.FullName == Weaver.singleType.FullName)
                        {
                            worker.Append(worker.Create(OpCodes.Conv_R4));
                        }
                        else if (definition.ParameterType.FullName == Weaver.doubleType.FullName)
                        {
                            worker.Append(worker.Create(OpCodes.Conv_R8));
                        }
                    }
                    else
                    {
                        Log.Error(string.Concat(new object[] { "ProcessNetworkReaderParameters for ", this.m_td.Name, ":", md.Name, " type ", definition.ParameterType, " not supported" }));
                        Weaver.fail = true;
                        return false;
                    }
                }
            }
            return true;
        }

        private MethodDefinition ProcessRpcCall(MethodDefinition md, CustomAttribute ca)
        {
            MethodDefinition definition = new MethodDefinition("Call" + md.Name, MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig, Weaver.voidType);
            foreach (ParameterDefinition definition2 in md.Parameters)
            {
                definition.Parameters.Add(new ParameterDefinition(definition2.Name, ParameterAttributes.None, definition2.ParameterType));
            }
            ILProcessor iLProcessor = definition.Body.GetILProcessor();
            Instruction label = iLProcessor.Create(OpCodes.Nop);
            WriteSetupLocals(iLProcessor);
            WriteServerActiveCheck(iLProcessor, md.Name, label, "RPC Function");
            WriteCreateWriter(iLProcessor);
            WriteMessageSize(iLProcessor);
            WriteMessageId(iLProcessor, 2);
            FieldDefinition item = new FieldDefinition("kRpc" + md.Name, FieldAttributes.CompilerControlled | FieldAttributes.Private | FieldAttributes.Static, Weaver.int32Type);
            this.m_td.Fields.Add(item);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldsfld, item));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkWriterWritePacked32));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.getComponentReference));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.getUNetIdReference));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkWriterWriteNetworkInstanceId));
            if (!WriteArguments(iLProcessor, md, "RPC", false))
            {
                return null;
            }
            int num = 0;
            foreach (CustomAttributeNamedArgument argument in ca.Fields)
            {
                if (argument.Name == "channel")
                {
                    num = (int) argument.Argument.Value;
                }
            }
            string name = md.Name;
            if (name.IndexOf("InvokeRpc") > -1)
            {
                name = name.Substring("InvokeRpc".Length);
            }
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, num));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldstr, name));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.sendRpcInternal));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            return definition;
        }

        private MethodDefinition ProcessRpcInvoke(MethodDefinition md)
        {
            MethodDefinition definition = new MethodDefinition("InvokeRpc" + md.Name, MethodAttributes.CompilerControlled | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Static, Weaver.voidType);
            ILProcessor iLProcessor = definition.Body.GetILProcessor();
            Instruction label = iLProcessor.Create(OpCodes.Nop);
            WriteClientActiveCheck(iLProcessor, md.Name, label, "RPC");
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Castclass, this.m_td));
            if (!this.ProcessNetworkReaderParameters(md, iLProcessor, false))
            {
                return null;
            }
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, md));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            AddInvokeParameters(definition.Parameters);
            return definition;
        }

        private FieldDefinition ProcessSyncList(FieldDefinition fd, int dirtyBit)
        {
            MethodDefinition item = ProcessSyncListInvoke(fd);
            this.m_SyncListInvocationFuncs.Add(item);
            return new FieldDefinition("kList" + fd.Name, FieldAttributes.CompilerControlled | FieldAttributes.Private | FieldAttributes.Static, Weaver.int32Type);
        }

        private static MethodDefinition ProcessSyncListInvoke(FieldDefinition fd)
        {
            MethodDefinition definition = new MethodDefinition("InvokeSyncList" + fd.Name, MethodAttributes.CompilerControlled | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Static, Weaver.voidType);
            ILProcessor iLProcessor = definition.Body.GetILProcessor();
            Instruction label = iLProcessor.Create(OpCodes.Nop);
            WriteClientActiveCheck(iLProcessor, fd.Name, label, "SyncList");
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Castclass, fd.DeclaringType));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, fd));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
            GenericInstanceType baseType = (GenericInstanceType) fd.FieldType.Resolve().BaseType;
            baseType = (GenericInstanceType) Weaver.scriptDef.MainModule.ImportReference(baseType);
            TypeReference reference = baseType.GenericArguments[0];
            TypeReference[] arguments = new TypeReference[] { reference };
            MethodReference method = Helpers.MakeHostInstanceGeneric(Weaver.SyncListInitHandleMsg, arguments);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, method));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            AddInvokeParameters(definition.Parameters);
            return definition;
        }

        private void ProcessSyncVar(FieldDefinition fd, int dirtyBit)
        {
            string name = fd.Name;
            Weaver.lists.replacedFields.Add(fd);
            Weaver.DLog(this.m_td, string.Concat(new object[] { "Sync Var ", fd.Name, " ", fd.FieldType, " ", Weaver.gameObjectType }), new object[0]);
            FieldDefinition item = null;
            if (fd.FieldType.FullName == Weaver.gameObjectType.FullName)
            {
                item = new FieldDefinition("___" + fd.Name + "NetId", FieldAttributes.CompilerControlled | FieldAttributes.Private, Weaver.NetworkInstanceIdType);
                this.m_SyncVarNetIds.Add(item);
                Weaver.lists.netIdFields.Add(item);
            }
            MethodDefinition definition2 = ProcessSyncVarGet(fd, name);
            MethodDefinition definition3 = this.ProcessSyncVarSet(fd, name, dirtyBit, item);
            PropertyDefinition definition4 = new PropertyDefinition("Network" + name, PropertyAttributes.None, fd.FieldType) {
                GetMethod = definition2,
                SetMethod = definition3
            };
            this.m_td.Methods.Add(definition2);
            this.m_td.Methods.Add(definition3);
            this.m_td.Properties.Add(definition4);
            Weaver.lists.replacementProperties.Add(definition3);
        }

        private static MethodDefinition ProcessSyncVarGet(FieldDefinition fd, string originalName)
        {
            MethodDefinition definition = new MethodDefinition("get_Network" + originalName, MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.SpecialName, fd.FieldType);
            ILProcessor iLProcessor = definition.Body.GetILProcessor();
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, fd));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            definition.Body.Variables.Add(new VariableDefinition("V_0", fd.FieldType));
            definition.Body.InitLocals = true;
            definition.SemanticsAttributes = MethodSemanticsAttributes.Getter;
            return definition;
        }

        private void ProcessSyncVars()
        {
            int num = 0;
            int syncVarStart = Weaver.GetSyncVarStart(this.m_td.BaseType.FullName);
            this.m_SyncVarNetIds.Clear();
            List<FieldDefinition> list = new List<FieldDefinition>();
            foreach (FieldDefinition definition in this.m_td.Fields)
            {
                foreach (CustomAttribute attribute in definition.CustomAttributes)
                {
                    if (attribute.AttributeType.FullName == Weaver.SyncVarType.FullName)
                    {
                        TypeDefinition td = definition.FieldType.Resolve();
                        if (Weaver.IsDerivedFrom(td, Weaver.NetworkBehaviourType))
                        {
                            Log.Error("SyncVar [" + definition.FullName + "] cannot be derived from NetworkBehaviour. Use a GameObject or NetworkInstanceId.");
                            Weaver.fail = true;
                            return;
                        }
                        if (((ushort) (definition.Attributes & (FieldAttributes.CompilerControlled | FieldAttributes.Static))) != 0)
                        {
                            Log.Error("SyncVar [" + definition.FullName + "] cannot be static.");
                            Weaver.fail = true;
                            return;
                        }
                        if (td.HasGenericParameters)
                        {
                            Log.Error("SyncVar [" + definition.FullName + "] cannot have generic parameters.");
                            Weaver.fail = true;
                            return;
                        }
                        if (td.IsInterface)
                        {
                            Log.Error("SyncVar [" + definition.FullName + "] cannot be an interface.");
                            Weaver.fail = true;
                            return;
                        }
                        string name = td.Module.Name;
                        if ((((name != Weaver.scriptDef.MainModule.Name) && (name != Weaver.m_UnityAssemblyDefinition.MainModule.Name)) && ((name != Weaver.m_UNetAssemblyDefinition.MainModule.Name) && (name != Weaver.corLib.Name))) && (name != "System.Runtime.dll"))
                        {
                            Log.Error("SyncVar [" + definition.FullName + "] from " + td.Module.ToString() + " cannot be a different module.");
                            Weaver.fail = true;
                            return;
                        }
                        if (definition.FieldType.IsArray)
                        {
                            Log.Error("SyncVar [" + definition.FullName + "] cannot be an array. Use a SyncList instead.");
                            Weaver.fail = true;
                            return;
                        }
                        this.m_SyncVars.Add(definition);
                        this.ProcessSyncVar(definition, ((int) 1) << syncVarStart);
                        syncVarStart++;
                        num++;
                        if (syncVarStart == 0x20)
                        {
                            Log.Error(string.Concat(new object[] { "Script class [", this.m_td.FullName, "] has too many SyncVars (", 0x20, "). (This could include base classes)" }));
                            Weaver.fail = true;
                            return;
                        }
                        break;
                    }
                }
                if (definition.FieldType.FullName.Contains("UnityEngine.Networking.SyncListStruct"))
                {
                    Log.Error("SyncListStruct member variable [" + definition.FullName + "] must use a dervied class, like \"class MySyncList : SyncListStruct<MyStruct> {}\".");
                    Weaver.fail = true;
                    return;
                }
                if (Weaver.IsDerivedFrom(definition.FieldType.Resolve(), Weaver.SyncListType))
                {
                    this.m_SyncVars.Add(definition);
                    this.m_SyncLists.Add(definition);
                    list.Add(this.ProcessSyncList(definition, ((int) 1) << syncVarStart));
                    syncVarStart++;
                    num++;
                    if (syncVarStart == 0x20)
                    {
                        Log.Error(string.Concat(new object[] { "Script class [", this.m_td.FullName, "] has too many SyncVars (", 0x20, "). (This could include base classes)" }));
                        Weaver.fail = true;
                        return;
                    }
                }
            }
            foreach (FieldDefinition definition3 in list)
            {
                this.m_td.Fields.Add(definition3);
                this.m_SyncListStaticFields.Add(definition3);
            }
            foreach (FieldDefinition definition4 in this.m_SyncVarNetIds)
            {
                this.m_td.Fields.Add(definition4);
            }
            foreach (MethodDefinition definition5 in this.m_SyncListInvocationFuncs)
            {
                this.m_td.Methods.Add(definition5);
            }
            Weaver.SetNumSyncVars(this.m_td.FullName, num);
        }

        private MethodDefinition ProcessSyncVarSet(FieldDefinition fd, string originalName, int dirtyBit, FieldDefinition netFieldId)
        {
            MethodDefinition definition2;
            MethodDefinition definition = new MethodDefinition("set_Network" + originalName, MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.SpecialName, Weaver.voidType);
            ILProcessor iLProcessor = definition.Body.GetILProcessor();
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldflda, fd));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, dirtyBit));
            this.CheckForHookFunction(fd, out definition2);
            if (definition2 != null)
            {
                Instruction target = iLProcessor.Create(OpCodes.Nop);
                iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.NetworkServerGetLocalClientActive));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Brfalse, target));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.getSyncVarHookGuard));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Brtrue, target));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_1));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.setSyncVarHookGuard));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Call, definition2));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_0));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.setSyncVarHookGuard));
                iLProcessor.Append(target);
            }
            if (fd.FieldType.FullName == Weaver.gameObjectType.FullName)
            {
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ldflda, netFieldId));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.setSyncVarGameObjectReference));
            }
            else
            {
                GenericInstanceMethod method = new GenericInstanceMethod(Weaver.setSyncVarReference) {
                    GenericArguments = { fd.FieldType }
                };
                iLProcessor.Append(iLProcessor.Create(OpCodes.Call, method));
            }
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            definition.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.In, fd.FieldType));
            definition.SemanticsAttributes = MethodSemanticsAttributes.None | MethodSemanticsAttributes.Setter;
            return definition;
        }

        private MethodDefinition ProcessTargetRpcCall(MethodDefinition md, CustomAttribute ca)
        {
            MethodDefinition definition = new MethodDefinition("Call" + md.Name, MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig, Weaver.voidType);
            foreach (ParameterDefinition definition2 in md.Parameters)
            {
                definition.Parameters.Add(new ParameterDefinition(definition2.Name, ParameterAttributes.None, definition2.ParameterType));
            }
            ILProcessor iLProcessor = definition.Body.GetILProcessor();
            Instruction label = iLProcessor.Create(OpCodes.Nop);
            WriteSetupLocals(iLProcessor);
            WriteServerActiveCheck(iLProcessor, md.Name, label, "TargetRPC Function");
            WriteCreateWriter(iLProcessor);
            WriteMessageSize(iLProcessor);
            WriteMessageId(iLProcessor, 2);
            FieldDefinition item = new FieldDefinition("kTargetRpc" + md.Name, FieldAttributes.CompilerControlled | FieldAttributes.Private | FieldAttributes.Static, Weaver.int32Type);
            this.m_td.Fields.Add(item);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldsfld, item));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkWriterWritePacked32));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.getComponentReference));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.getUNetIdReference));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.NetworkWriterWriteNetworkInstanceId));
            if (!WriteArguments(iLProcessor, md, "TargetRPC", true))
            {
                return null;
            }
            int num = 0;
            foreach (CustomAttributeNamedArgument argument in ca.Fields)
            {
                if (argument.Name == "channel")
                {
                    num = (int) argument.Argument.Value;
                }
            }
            string name = md.Name;
            if (name.IndexOf("InvokeTargetRpc") > -1)
            {
                name = name.Substring("InvokeTargetRpc".Length);
            }
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4, num));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldstr, name));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, Weaver.sendTargetRpcInternal));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            return definition;
        }

        private MethodDefinition ProcessTargetRpcInvoke(MethodDefinition md)
        {
            MethodDefinition definition = new MethodDefinition("InvokeRpc" + md.Name, MethodAttributes.CompilerControlled | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Static, Weaver.voidType);
            ILProcessor iLProcessor = definition.Body.GetILProcessor();
            Instruction label = iLProcessor.Create(OpCodes.Nop);
            WriteClientActiveCheck(iLProcessor, md.Name, label, "TargetRPC");
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Castclass, this.m_td));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Call, Weaver.ReadyConnectionReference));
            if (!this.ProcessNetworkReaderParameters(md, iLProcessor, true))
            {
                return null;
            }
            iLProcessor.Append(iLProcessor.Create(OpCodes.Callvirt, md));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            AddInvokeParameters(definition.Parameters);
            return definition;
        }

        private void ProcessVersion()
        {
            foreach (MethodDefinition definition in this.m_td.Methods)
            {
                if (definition.Name == "UNetVersion")
                {
                    return;
                }
            }
            MethodDefinition item = new MethodDefinition("UNetVersion", MethodAttributes.CompilerControlled | MethodAttributes.Private, Weaver.voidType);
            ILProcessor iLProcessor = item.Body.GetILProcessor();
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            this.m_td.Methods.Add(item);
        }

        private static bool WriteArguments(ILProcessor worker, MethodDefinition md, string errString, bool skipFirst)
        {
            short num = 1;
            foreach (ParameterDefinition definition in md.Parameters)
            {
                if ((num == 1) && skipFirst)
                {
                    num = (short) (num + 1);
                }
                else
                {
                    MethodReference writeFunc = Weaver.GetWriteFunc(definition.ParameterType);
                    if (writeFunc == null)
                    {
                        Log.Error(string.Concat(new object[] { "WriteArguments for ", md.Name, " type ", definition.ParameterType, " not supported" }));
                        Weaver.fail = true;
                        return false;
                    }
                    worker.Append(worker.Create(OpCodes.Ldloc_0));
                    worker.Append(worker.Create(OpCodes.Ldarg, (int) num));
                    worker.Append(worker.Create(OpCodes.Call, writeFunc));
                    num = (short) (num + 1);
                }
            }
            return true;
        }

        private static void WriteClientActiveCheck(ILProcessor worker, string mdName, Instruction label, string errString)
        {
            worker.Append(worker.Create(OpCodes.Call, Weaver.NetworkClientGetActive));
            worker.Append(worker.Create(OpCodes.Brtrue, label));
            worker.Append(worker.Create(OpCodes.Ldstr, errString + " " + mdName + " called on server."));
            worker.Append(worker.Create(OpCodes.Call, Weaver.logErrorReference));
            worker.Append(worker.Create(OpCodes.Ret));
            worker.Append(label);
        }

        private static void WriteCreateWriter(ILProcessor worker)
        {
            worker.Append(worker.Create(OpCodes.Newobj, Weaver.NetworkWriterCtor));
            worker.Append(worker.Create(OpCodes.Stloc_0));
            worker.Append(worker.Create(OpCodes.Ldloc_0));
        }

        private static void WriteDirtyCheck(ILProcessor serWorker, bool reset)
        {
            Instruction target = serWorker.Create(OpCodes.Nop);
            serWorker.Append(serWorker.Create(OpCodes.Ldloc_0));
            serWorker.Append(serWorker.Create(OpCodes.Brtrue, target));
            serWorker.Append(serWorker.Create(OpCodes.Ldarg_1));
            serWorker.Append(serWorker.Create(OpCodes.Ldarg_0));
            serWorker.Append(serWorker.Create(OpCodes.Call, Weaver.NetworkBehaviourDirtyBitsReference));
            serWorker.Append(serWorker.Create(OpCodes.Callvirt, Weaver.NetworkWriterWritePacked32));
            if (reset)
            {
                serWorker.Append(serWorker.Create(OpCodes.Ldc_I4_1));
                serWorker.Append(serWorker.Create(OpCodes.Stloc_0));
            }
            serWorker.Append(target);
        }

        private static void WriteMessageId(ILProcessor worker, int msgId)
        {
            worker.Append(worker.Create(OpCodes.Ldloc_0));
            worker.Append(worker.Create(OpCodes.Ldc_I4, msgId));
            worker.Append(worker.Create(OpCodes.Conv_U2));
            worker.Append(worker.Create(OpCodes.Callvirt, Weaver.NetworkWriterWriteInt16));
        }

        private static void WriteMessageSize(ILProcessor worker)
        {
            worker.Append(worker.Create(OpCodes.Ldc_I4_0));
            worker.Append(worker.Create(OpCodes.Callvirt, Weaver.NetworkWriterWriteInt16));
        }

        private static void WriteServerActiveCheck(ILProcessor worker, string mdName, Instruction label, string errString)
        {
            worker.Append(worker.Create(OpCodes.Call, Weaver.NetworkServerGetActive));
            worker.Append(worker.Create(OpCodes.Brtrue, label));
            worker.Append(worker.Create(OpCodes.Ldstr, errString + " " + mdName + " called on client."));
            worker.Append(worker.Create(OpCodes.Call, Weaver.logErrorReference));
            worker.Append(worker.Create(OpCodes.Ret));
            worker.Append(label);
        }

        private static void WriteSetupLocals(ILProcessor worker)
        {
            worker.Body.InitLocals = true;
            worker.Body.Variables.Add(new VariableDefinition("V_0", Weaver.scriptDef.MainModule.ImportReference(Weaver.NetworkWriterType)));
        }
    }
}

