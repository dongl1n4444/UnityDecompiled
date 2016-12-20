namespace Unity.UNetWeaver
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Mdb;
    using Mono.Cecil.Pdb;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal class Weaver
    {
        public static TypeReference boolType;
        public static TypeReference byteType;
        public static TypeReference charType;
        public static TypeReference ClientRpcType;
        public static TypeReference ClientSceneType;
        public static MethodReference CmdDelegateConstructor;
        public static TypeReference CmdDelegateReference;
        public static TypeReference color32Type;
        public static TypeReference colorType;
        public static TypeReference CommandType;
        public static TypeReference ComponentType;
        public static ModuleDefinition corLib;
        public static TypeReference decimalType;
        public static TypeReference doubleType;
        public static bool fail;
        public static MethodReference FindLocalObjectReference;
        public static MethodReference gameObjectInequality;
        public static TypeReference gameObjectType;
        public static bool generateLogErrors = false;
        public static MethodReference getComponentReference;
        public static MethodReference getPlayerIdReference;
        public static MethodReference getSyncVarHookGuard;
        public static MethodReference getTypeFromHandleReference;
        public static MethodReference getTypeReference;
        public static MethodReference getUNetIdReference;
        public static TypeReference hashType;
        public static TypeReference IEnumeratorType;
        public static TypeReference int16Type;
        public static TypeReference int32Type;
        public static TypeReference int64Type;
        public static WeaverLists lists;
        public static MethodReference logErrorReference;
        public static MethodReference logWarningReference;
        private static bool m_DebugFlag = true;
        public static AssemblyDefinition m_UNetAssemblyDefinition;
        public static AssemblyDefinition m_UnityAssemblyDefinition;
        public static TypeReference matrixType;
        private const int MaxRecursionCount = 0x80;
        public static MethodReference MemoryStreamCtor;
        public static TypeReference MemoryStreamType;
        public static TypeReference MessageBaseType;
        public static TypeReference MonoBehaviourType;
        public static MethodReference NetworkBehaviourDirtyBitsReference;
        public static TypeReference NetworkBehaviourType;
        public static TypeReference NetworkBehaviourType2;
        public static MethodReference NetworkClientGetActive;
        public static TypeReference NetworkClientType;
        public static TypeReference NetworkConnectionType;
        public static TypeReference NetworkCRCType;
        public static TypeReference NetworkIdentityType;
        public static TypeReference NetworkInstanceIdType;
        public static MethodReference NetworkInstanceIsEmpty;
        public static MethodReference NetworkReaderCtor;
        public static TypeDefinition NetworkReaderDef;
        public static MethodReference NetworkReaderReadByte;
        public static MethodReference NetworkReaderReadInt32;
        public static MethodReference NetworkReaderReadNetworkInstanceId;
        public static MethodReference NetworkReaderReadNetworkSceneId;
        public static MethodReference NetworkReaderReadPacked32;
        public static MethodReference NetworkReaderReadPacked64;
        public static TypeReference NetworkReaderType;
        public static MethodReference NetworkReadUInt16;
        public static TypeReference NetworkSceneIdType;
        public static MethodReference NetworkServerGetActive;
        public static MethodReference NetworkServerGetLocalClientActive;
        public static TypeReference NetworkServerType;
        public static TypeReference NetworkSettingsType;
        public static MethodReference NetworkWriterCtor;
        public static TypeDefinition NetworkWriterDef;
        public static TypeReference NetworkWriterType;
        public static MethodReference NetworkWriterWriteInt16;
        public static MethodReference NetworkWriterWriteInt32;
        public static MethodReference NetworkWriterWriteNetworkInstanceId;
        public static MethodReference NetworkWriterWriteNetworkSceneId;
        public static MethodReference NetworkWriterWritePacked32;
        public static MethodReference NetworkWriterWritePacked64;
        public static MethodReference NetworkWriteUInt16;
        public static TypeReference objectType;
        public static TypeReference planeType;
        public static TypeReference quaternionType;
        public static TypeReference rayType;
        public static MethodReference ReadyConnectionReference;
        public static TypeReference rectType;
        public static MethodReference RegisterBehaviourReference;
        public static MethodReference registerCommandDelegateReference;
        public static MethodReference registerEventDelegateReference;
        public static MethodReference registerRpcDelegateReference;
        public static MethodReference registerSyncListDelegateReference;
        private static int s_RecursionCount;
        public static TypeReference sbyteType;
        public static AssemblyDefinition scriptDef;
        public static MethodReference sendCommandInternal;
        public static MethodReference sendEventInternal;
        public static MethodReference sendRpcInternal;
        public static MethodReference sendTargetRpcInternal;
        public static MethodReference setSyncVarGameObjectReference;
        public static MethodReference setSyncVarHookGuard;
        public static MethodReference setSyncVarReference;
        public static TypeReference singleType;
        public static TypeReference stringType;
        public static TypeReference SyncEventType;
        public static MethodReference SyncListBoolReadType;
        public static TypeReference SyncListBoolType;
        public static MethodReference SyncListBoolWriteType;
        public static MethodReference SyncListClear;
        public static MethodReference SyncListFloatReadType;
        public static TypeReference SyncListFloatType;
        public static MethodReference SyncListFloatWriteType;
        public static MethodReference SyncListInitBehaviourReference;
        public static MethodReference SyncListInitHandleMsg;
        public static MethodReference SyncListIntReadType;
        public static TypeReference SyncListIntType;
        public static MethodReference SyncListIntWriteType;
        public static MethodReference SyncListStringReadType;
        public static TypeReference SyncListStringType;
        public static MethodReference SyncListStringWriteType;
        public static TypeReference SyncListStructType;
        public static TypeReference SyncListType;
        public static MethodReference SyncListUIntReadType;
        public static TypeReference SyncListUIntType;
        public static MethodReference SyncListUIntWriteType;
        public static TypeReference SyncVarType;
        public static TypeReference TargetRpcType;
        public static TypeReference transformType;
        public static TypeReference typeType;
        public static MethodReference UBehaviourIsServer;
        public static TypeReference uint16Type;
        public static TypeReference uint32Type;
        public static TypeReference uint64Type;
        public static TypeReference unityObjectType;
        public static TypeReference vector2Type;
        public static TypeReference vector3Type;
        public static TypeReference vector4Type;
        public static TypeReference voidType;

        private static bool CheckMessageBase(TypeDefinition td)
        {
            if (!td.IsClass)
            {
                return false;
            }
            bool flag2 = false;
            TypeReference baseType = td.BaseType;
            while (baseType != null)
            {
                if (baseType.FullName == MessageBaseType.FullName)
                {
                    flag2 |= ProcessMessageType(td);
                    break;
                }
                try
                {
                    baseType = baseType.Resolve().BaseType;
                }
                catch (AssemblyResolutionException)
                {
                    break;
                }
            }
            foreach (TypeDefinition definition in td.NestedTypes)
            {
                flag2 |= CheckMessageBase(definition);
            }
            return flag2;
        }

        private static void CheckMonoBehaviour(TypeDefinition td)
        {
            if (IsDerivedFrom(td, MonoBehaviourType))
            {
                ProcessMonoBehaviourType(td);
            }
        }

        private static bool CheckNetworkBehaviour(TypeDefinition td)
        {
            if (!td.IsClass)
            {
                return false;
            }
            if (!IsNetworkBehaviour(td))
            {
                CheckMonoBehaviour(td);
                return false;
            }
            List<TypeDefinition> list = new List<TypeDefinition>();
            TypeDefinition item = td;
            while (item != null)
            {
                if (item.FullName == NetworkBehaviourType.FullName)
                {
                    break;
                }
                try
                {
                    list.Insert(0, item);
                    item = item.BaseType.Resolve();
                }
                catch (AssemblyResolutionException)
                {
                    break;
                }
            }
            bool flag2 = false;
            foreach (TypeDefinition definition2 in list)
            {
                flag2 |= ProcessNetworkBehaviourType(definition2);
            }
            return flag2;
        }

        private static bool CheckSyncListStruct(TypeDefinition td)
        {
            if (!td.IsClass)
            {
                return false;
            }
            bool flag2 = false;
            TypeReference baseType = td.BaseType;
            while (baseType != null)
            {
                if (baseType.FullName.Contains("SyncListStruct"))
                {
                    flag2 |= ProcessSyncListStructType(td);
                    break;
                }
                try
                {
                    baseType = baseType.Resolve().BaseType;
                }
                catch (AssemblyResolutionException)
                {
                    break;
                }
            }
            foreach (TypeDefinition definition in td.NestedTypes)
            {
                flag2 |= CheckSyncListStruct(definition);
            }
            return flag2;
        }

        private static void ConfirmGeneratedCodeClass(ModuleDefinition moduleDef)
        {
            if (lists.generateContainerClass == null)
            {
                lists.generateContainerClass = new TypeDefinition("Unity", "GeneratedNetworkCode", TypeAttributes.AnsiClass | TypeAttributes.AutoClass | TypeAttributes.BeforeFieldInit | TypeAttributes.Public, objectType);
                MethodDefinition item = new MethodDefinition(".ctor", MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName, voidType) {
                    Body = { Instructions = { 
                        Instruction.Create(OpCodes.Ldarg_0),
                        Instruction.Create(OpCodes.Call, ResolveMethod(objectType, ".ctor")),
                        Instruction.Create(OpCodes.Ret)
                    } }
                };
                lists.generateContainerClass.Methods.Add(item);
            }
        }

        public static void DLog(TypeDefinition td, string fmt, params object[] args)
        {
            if (m_DebugFlag)
            {
                Console.WriteLine("[" + td.Name + "] " + string.Format(fmt, args));
            }
        }

        private static MethodDefinition GenerateArrayReadFunc(TypeReference variable, MethodReference elementReadFunc)
        {
            string name = "_ReadArray" + variable.GetElementType().Name + "_";
            if (variable.DeclaringType != null)
            {
                name = name + variable.DeclaringType.Name;
            }
            else
            {
                name = name + "None";
            }
            MethodDefinition definition = new MethodDefinition(name, MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Static, variable) {
                Parameters = { new ParameterDefinition("reader", ParameterAttributes.None, scriptDef.MainModule.ImportReference(NetworkReaderType)) },
                Body = { 
                    Variables = { 
                        new VariableDefinition("v0", int32Type),
                        new VariableDefinition("v1", variable),
                        new VariableDefinition("v2", int32Type)
                    },
                    InitLocals = true
                }
            };
            ILProcessor iLProcessor = definition.Body.GetILProcessor();
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Call, NetworkReadUInt16));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
            Instruction target = iLProcessor.Create(OpCodes.Nop);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Brtrue, target));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Newarr, variable.GetElementType()));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            iLProcessor.Append(target);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Newarr, variable.GetElementType()));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_2));
            Instruction instruction2 = iLProcessor.Create(OpCodes.Nop);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Br, instruction2));
            Instruction instruction = iLProcessor.Create(OpCodes.Nop);
            iLProcessor.Append(instruction);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_2));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldelema, variable.GetElementType()));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Call, elementReadFunc));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Stobj, variable.GetElementType()));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_2));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Add));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_2));
            iLProcessor.Append(instruction2);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_2));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Blt, instruction));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            return definition;
        }

        private static MethodDefinition GenerateArrayWriteFunc(TypeReference variable, MethodReference elementWriteFunc)
        {
            string name = "_WriteArray" + variable.GetElementType().Name + "_";
            if (variable.DeclaringType != null)
            {
                name = name + variable.DeclaringType.Name;
            }
            else
            {
                name = name + "None";
            }
            MethodDefinition definition = new MethodDefinition(name, MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Static, voidType) {
                Parameters = { 
                    new ParameterDefinition("writer", ParameterAttributes.None, scriptDef.MainModule.ImportReference(NetworkWriterType)),
                    new ParameterDefinition("value", ParameterAttributes.None, scriptDef.MainModule.ImportReference(variable))
                },
                Body = { 
                    Variables = { 
                        new VariableDefinition("v0", uint16Type),
                        new VariableDefinition("v1", uint16Type)
                    },
                    InitLocals = true
                }
            };
            ILProcessor iLProcessor = definition.Body.GetILProcessor();
            Instruction target = iLProcessor.Create(OpCodes.Nop);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Brtrue, target));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Call, NetworkWriteUInt16));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            iLProcessor.Append(target);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldlen));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Conv_I4));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Conv_U2));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Call, NetworkWriteUInt16));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_1));
            Instruction instruction2 = iLProcessor.Create(OpCodes.Nop);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Br, instruction2));
            Instruction instruction = iLProcessor.Create(OpCodes.Nop);
            iLProcessor.Append(instruction);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldelema, variable.GetElementType()));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldobj, variable.GetElementType()));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Call, elementWriteFunc));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldc_I4_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Add));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Conv_U2));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_1));
            iLProcessor.Append(instruction2);
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldlen));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Conv_I4));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Blt, instruction));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            return definition;
        }

        private static MethodDefinition GenerateReadFunction(TypeReference variable)
        {
            if (!IsValidTypeToGenerate(variable.Resolve()))
            {
                return null;
            }
            string name = "_Read" + variable.Name + "_";
            if (variable.DeclaringType != null)
            {
                name = name + variable.DeclaringType.Name;
            }
            else
            {
                name = name + "None";
            }
            MethodDefinition definition2 = new MethodDefinition(name, MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Static, variable) {
                Body = { 
                    Variables = { new VariableDefinition("result", variable) },
                    InitLocals = true
                },
                Parameters = { new ParameterDefinition("reader", ParameterAttributes.None, scriptDef.MainModule.ImportReference(NetworkReaderType)) }
            };
            ILProcessor iLProcessor = definition2.Body.GetILProcessor();
            if (variable.IsValueType)
            {
                iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloca, 0));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Initobj, variable));
            }
            else
            {
                MethodDefinition method = ResolveDefaultPublicCtor(variable);
                if (method == null)
                {
                    Unity.UNetWeaver.Log.Error("The class " + variable.Name + " has no default constructor or it's private, aborting.");
                    return null;
                }
                iLProcessor.Append(iLProcessor.Create(OpCodes.Newobj, method));
                iLProcessor.Append(iLProcessor.Create(OpCodes.Stloc_0));
            }
            uint num = 0;
            foreach (FieldDefinition definition4 in variable.Resolve().Fields)
            {
                if (!definition4.IsStatic && !definition4.IsPrivate)
                {
                    if (variable.IsValueType)
                    {
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloca, 0));
                    }
                    else
                    {
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc, 0));
                    }
                    MethodReference readFunc = GetReadFunc(definition4.FieldType);
                    if (readFunc != null)
                    {
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Call, readFunc));
                    }
                    else
                    {
                        Unity.UNetWeaver.Log.Error(string.Concat(new object[] { "GetReadFunc for ", definition4.Name, " type ", definition4.FieldType, " no supported" }));
                        fail = true;
                        return null;
                    }
                    iLProcessor.Append(iLProcessor.Create(OpCodes.Stfld, definition4));
                    num++;
                }
            }
            if (num == 0)
            {
                Unity.UNetWeaver.Log.Warning("The class / struct " + variable.Name + " has no public or non-static fields to serialize");
            }
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ldloc_0));
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            return definition2;
        }

        private static MethodDefinition GenerateWriterFunction(TypeReference variable)
        {
            if (!IsValidTypeToGenerate(variable.Resolve()))
            {
                return null;
            }
            string name = "_Write" + variable.Name + "_";
            if (variable.DeclaringType != null)
            {
                name = name + variable.DeclaringType.Name;
            }
            else
            {
                name = name + "None";
            }
            MethodDefinition definition2 = new MethodDefinition(name, MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Static, voidType) {
                Parameters = { 
                    new ParameterDefinition("writer", ParameterAttributes.None, scriptDef.MainModule.ImportReference(NetworkWriterType)),
                    new ParameterDefinition("value", ParameterAttributes.None, scriptDef.MainModule.ImportReference(variable))
                }
            };
            ILProcessor iLProcessor = definition2.Body.GetILProcessor();
            uint num = 0;
            foreach (FieldDefinition definition3 in variable.Resolve().Fields)
            {
                if (!definition3.IsStatic && !definition3.IsPrivate)
                {
                    if (definition3.FieldType.Resolve().HasGenericParameters)
                    {
                        fail = true;
                        Unity.UNetWeaver.Log.Error(string.Concat(new object[] { "WriteReadFunc for ", definition3.Name, " [", definition3.FieldType, "/", definition3.FieldType.FullName, "]. Cannot have generic parameters." }));
                        return null;
                    }
                    if (definition3.FieldType.Resolve().IsInterface)
                    {
                        fail = true;
                        Unity.UNetWeaver.Log.Error(string.Concat(new object[] { "WriteReadFunc for ", definition3.Name, " [", definition3.FieldType, "/", definition3.FieldType.FullName, "]. Cannot be an interface." }));
                        return null;
                    }
                    MethodReference writeFunc = GetWriteFunc(definition3.FieldType);
                    if (writeFunc != null)
                    {
                        num++;
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_0));
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Ldarg_1));
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Ldfld, definition3));
                        iLProcessor.Append(iLProcessor.Create(OpCodes.Call, writeFunc));
                    }
                    else
                    {
                        Unity.UNetWeaver.Log.Error(string.Concat(new object[] { "WriteReadFunc for ", definition3.Name, " type ", definition3.FieldType, " no supported" }));
                        fail = true;
                        return null;
                    }
                }
            }
            if (num == 0)
            {
                Unity.UNetWeaver.Log.Warning("The class / struct " + variable.Name + " has no public or non-static fields to serialize");
            }
            iLProcessor.Append(iLProcessor.Create(OpCodes.Ret));
            return definition2;
        }

        private static Instruction GetEventLoadInstruction(ModuleDefinition moduleDef, TypeDefinition td, MethodDefinition md, int iCount, FieldReference foundEventField)
        {
            while (iCount > 0)
            {
                iCount--;
                Instruction instruction = md.Body.Instructions[iCount];
                if ((instruction.OpCode == OpCodes.Ldfld) && (instruction.Operand == foundEventField))
                {
                    DLog(td, "    " + instruction.Operand, new object[0]);
                    return instruction;
                }
            }
            return null;
        }

        public static MethodReference GetReadByReferenceFunc(TypeReference variable)
        {
            if (lists.readByReferenceFuncs.ContainsKey(variable.FullName))
            {
                return lists.readByReferenceFuncs[variable.FullName];
            }
            return null;
        }

        public static MethodReference GetReadFunc(TypeReference variable)
        {
            MethodDefinition definition2;
            if (lists.readFuncs.ContainsKey(variable.FullName))
            {
                MethodReference reference = lists.readFuncs[variable.FullName];
                if (reference.ReturnType.IsArray == variable.IsArray)
                {
                    return reference;
                }
            }
            TypeDefinition definition = variable.Resolve();
            if (definition == null)
            {
                Unity.UNetWeaver.Log.Error("GetReadFunc unsupported type " + variable.FullName);
                return null;
            }
            if (definition.IsEnum)
            {
                return NetworkReaderReadInt32;
            }
            if (variable.IsByReference)
            {
                Unity.UNetWeaver.Log.Error("GetReadFunc variable.IsByReference error.");
                return null;
            }
            if (variable.IsArray)
            {
                MethodReference readFunc = GetReadFunc(variable.GetElementType());
                if (readFunc == null)
                {
                    return null;
                }
                definition2 = GenerateArrayReadFunc(variable, readFunc);
            }
            else
            {
                definition2 = GenerateReadFunction(variable);
            }
            if (definition2 == null)
            {
                Unity.UNetWeaver.Log.Error("GetReadFunc unable to generate function for:" + variable.FullName);
                return null;
            }
            RegisterReadFunc(variable.FullName, definition2);
            return definition2;
        }

        public static int GetSyncVarStart(string className)
        {
            if (lists.numSyncVars.ContainsKey(className))
            {
                return lists.numSyncVars[className];
            }
            return 0;
        }

        public static MethodReference GetWriteFunc(TypeReference variable)
        {
            MethodDefinition definition;
            if (s_RecursionCount++ > 0x80)
            {
                Unity.UNetWeaver.Log.Error("GetWriteFunc recursion depth exceeded for " + variable.Name + ". Check for self-referencing member variables.");
                fail = true;
                return null;
            }
            if (lists.writeFuncs.ContainsKey(variable.FullName))
            {
                MethodReference reference2 = lists.writeFuncs[variable.FullName];
                if (reference2.Parameters[0].ParameterType.IsArray == variable.IsArray)
                {
                    return reference2;
                }
            }
            if (variable.Resolve().IsEnum)
            {
                return NetworkWriterWriteInt32;
            }
            if (variable.IsByReference)
            {
                Unity.UNetWeaver.Log.Error("GetWriteFunc variable.IsByReference error.");
                return null;
            }
            if (variable.IsArray)
            {
                MethodReference writeFunc = GetWriteFunc(variable.GetElementType());
                if (writeFunc == null)
                {
                    return null;
                }
                definition = GenerateArrayWriteFunc(variable, writeFunc);
            }
            else
            {
                definition = GenerateWriterFunction(variable);
            }
            if (definition == null)
            {
                return null;
            }
            RegisterWriteFunc(variable.FullName, definition);
            return definition;
        }

        private static TypeReference ImportCorLibType(string fullName)
        {
            TypeDefinition definition;
            <ImportCorLibType>c__AnonStorey0 storey = new <ImportCorLibType>c__AnonStorey0 {
                fullName = fullName
            };
            TypeDefinition type = corLib.GetType(storey.fullName);
            if (type != null)
            {
                definition = type;
            }
            else
            {
                definition = Enumerable.First<ExportedType>(corLib.ExportedTypes, new Func<ExportedType, bool>(storey, (IntPtr) this.<>m__0)).Resolve();
            }
            return scriptDef.MainModule.ImportReference(definition);
        }

        private static void InjectClientGuard(ModuleDefinition moduleDef, TypeDefinition td, MethodDefinition md, bool logWarning)
        {
            if (!IsNetworkBehaviour(td))
            {
                Unity.UNetWeaver.Log.Error("[Client] guard on non-NetworkBehaviour script at [" + md.FullName + "]");
            }
            else
            {
                ILProcessor iLProcessor = md.Body.GetILProcessor();
                Instruction target = md.Body.Instructions[0];
                iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Call, NetworkClientGetActive));
                iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Brtrue, target));
                if (logWarning)
                {
                    iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Ldstr, "[Client] function '" + md.FullName + "' called on server"));
                    iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Call, logWarningReference));
                }
                InjectGuardParameters(md, iLProcessor, target);
                InjectGuardReturnValue(md, iLProcessor, target);
                iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Ret));
            }
        }

        private static void InjectGuardParameters(MethodDefinition md, ILProcessor worker, Instruction top)
        {
            for (int i = 0; i < md.Parameters.Count; i++)
            {
                ParameterDefinition definition = md.Parameters[i];
                if (definition.IsOut)
                {
                    TypeReference elementType = definition.ParameterType.GetElementType();
                    if (elementType.IsPrimitive)
                    {
                        worker.InsertBefore(top, worker.Create(OpCodes.Ldarg, (int) (i + 1)));
                        worker.InsertBefore(top, worker.Create(OpCodes.Ldc_I4_0));
                        worker.InsertBefore(top, worker.Create(OpCodes.Stind_I4));
                    }
                    else
                    {
                        md.Body.Variables.Add(new VariableDefinition("__out" + i, elementType));
                        md.Body.InitLocals = true;
                        worker.InsertBefore(top, worker.Create(OpCodes.Ldarg, (int) (i + 1)));
                        worker.InsertBefore(top, worker.Create(OpCodes.Ldloca_S, (byte) (md.Body.Variables.Count - 1)));
                        worker.InsertBefore(top, worker.Create(OpCodes.Initobj, elementType));
                        worker.InsertBefore(top, worker.Create(OpCodes.Ldloc, (int) (md.Body.Variables.Count - 1)));
                        worker.InsertBefore(top, worker.Create(OpCodes.Stobj, elementType));
                    }
                }
            }
        }

        private static void InjectGuardReturnValue(MethodDefinition md, ILProcessor worker, Instruction top)
        {
            if (md.ReturnType.FullName != voidType.FullName)
            {
                if (md.ReturnType.IsPrimitive)
                {
                    worker.InsertBefore(top, worker.Create(OpCodes.Ldc_I4_0));
                }
                else
                {
                    md.Body.Variables.Add(new VariableDefinition("__ret", md.ReturnType));
                    md.Body.InitLocals = true;
                    worker.InsertBefore(top, worker.Create(OpCodes.Ldloca_S, (byte) (md.Body.Variables.Count - 1)));
                    worker.InsertBefore(top, worker.Create(OpCodes.Initobj, md.ReturnType));
                    worker.InsertBefore(top, worker.Create(OpCodes.Ldloc, (int) (md.Body.Variables.Count - 1)));
                }
            }
        }

        private static void InjectServerGuard(ModuleDefinition moduleDef, TypeDefinition td, MethodDefinition md, bool logWarning)
        {
            if (!IsNetworkBehaviour(td))
            {
                Unity.UNetWeaver.Log.Error("[Server] guard on non-NetworkBehaviour script at [" + md.FullName + "]");
            }
            else
            {
                ILProcessor iLProcessor = md.Body.GetILProcessor();
                Instruction target = md.Body.Instructions[0];
                iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Call, NetworkServerGetActive));
                iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Brtrue, target));
                if (logWarning)
                {
                    iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Ldstr, "[Server] function '" + md.FullName + "' called on client"));
                    iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Call, logWarningReference));
                }
                InjectGuardParameters(md, iLProcessor, target);
                InjectGuardReturnValue(md, iLProcessor, target);
                iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Ret));
            }
        }

        public static bool IsDerivedFrom(TypeDefinition td, TypeReference baseClass)
        {
            if (td.IsClass)
            {
                TypeReference baseType = td.BaseType;
                while (baseType != null)
                {
                    string fullName = baseType.FullName;
                    int index = fullName.IndexOf('<');
                    if (index != -1)
                    {
                        fullName = fullName.Substring(0, index);
                    }
                    if (fullName == baseClass.FullName)
                    {
                        return true;
                    }
                    try
                    {
                        baseType = baseType.Resolve().BaseType;
                    }
                    catch (AssemblyResolutionException)
                    {
                        break;
                    }
                }
            }
            return false;
        }

        private static bool IsNetworkBehaviour(TypeDefinition td)
        {
            if (td.IsClass)
            {
                TypeReference baseType = td.BaseType;
                while (baseType != null)
                {
                    if (baseType.FullName == NetworkBehaviourType.FullName)
                    {
                        return true;
                    }
                    try
                    {
                        baseType = baseType.Resolve().BaseType;
                    }
                    catch (AssemblyResolutionException)
                    {
                        break;
                    }
                }
            }
            return false;
        }

        public static bool IsValidTypeToGenerate(TypeDefinition variable)
        {
            string name = scriptDef.MainModule.Name;
            if (variable.Module.Name != name)
            {
                Unity.UNetWeaver.Log.Error("parameter [" + variable.Name + "] is of the type [" + variable.FullName + "] is not a valid type, please make sure to use a valid type.");
                fail = true;
                fail = true;
                return false;
            }
            return true;
        }

        private static void ProcessInstruction(ModuleDefinition moduleDef, TypeDefinition td, MethodDefinition md, Instruction i, int iCount)
        {
            if ((i.OpCode == OpCodes.Call) || (i.OpCode == OpCodes.Callvirt))
            {
                MethodReference operand = i.Operand as MethodReference;
                if (operand != null)
                {
                    ProcessInstructionMethod(moduleDef, td, md, i, operand, iCount);
                }
            }
            if (i.OpCode == OpCodes.Stfld)
            {
                FieldDefinition opField = i.Operand as FieldDefinition;
                if (opField != null)
                {
                    ProcessInstructionField(td, md, i, opField);
                }
            }
        }

        private static void ProcessInstructionField(TypeDefinition td, MethodDefinition md, Instruction i, FieldDefinition opField)
        {
            if ((md.Name != ".ctor") && (md.Name != "OnDeserialize"))
            {
                for (int j = 0; j < lists.replacedFields.Count; j++)
                {
                    FieldDefinition definition = lists.replacedFields[j];
                    if (opField == definition)
                    {
                        i.OpCode = OpCodes.Call;
                        i.Operand = lists.replacementProperties[j];
                        break;
                    }
                }
            }
        }

        private static void ProcessInstructionMethod(ModuleDefinition moduleDef, TypeDefinition td, MethodDefinition md, Instruction instr, MethodReference opMethodRef, int iCount)
        {
            if (opMethodRef.Name == "Invoke")
            {
                bool flag = false;
                while ((iCount > 0) && !flag)
                {
                    iCount--;
                    Instruction instruction = md.Body.Instructions[iCount];
                    if (instruction.OpCode == OpCodes.Ldfld)
                    {
                        FieldReference operand = instruction.Operand as FieldReference;
                        for (int i = 0; i < lists.replacedEvents.Count; i++)
                        {
                            EventDefinition definition = lists.replacedEvents[i];
                            if (definition.Name == operand.Name)
                            {
                                instr.Operand = lists.replacementEvents[i];
                                instruction.OpCode = OpCodes.Nop;
                                flag = true;
                                break;
                            }
                        }
                    }
                }
            }
            else if (lists.replacementMethodNames.Contains(opMethodRef.FullName))
            {
                for (int j = 0; j < lists.replacedMethods.Count; j++)
                {
                    MethodDefinition definition2 = lists.replacedMethods[j];
                    if (opMethodRef.FullName == definition2.FullName)
                    {
                        instr.Operand = lists.replacementMethods[j];
                        break;
                    }
                }
            }
        }

        private static bool ProcessMessageType(TypeDefinition td)
        {
            new MessageClassProcessor(td).Process();
            return true;
        }

        private static void ProcessMonoBehaviourType(TypeDefinition td)
        {
            new MonoBehaviourProcessor(td).Process();
        }

        private static bool ProcessNetworkBehaviourType(TypeDefinition td)
        {
            foreach (MethodDefinition definition in td.Resolve().Methods)
            {
                if (definition.Name == "UNetVersion")
                {
                    DLog(td, " Already processed", new object[0]);
                    return false;
                }
            }
            DLog(td, "Found NetworkBehaviour " + td.FullName, new object[0]);
            new NetworkBehaviourProcessor(td).Process();
            return true;
        }

        private static void ProcessPropertySites()
        {
            ProcessSitesModule(scriptDef.MainModule);
        }

        private static void ProcessSiteClass(ModuleDefinition moduleDef, TypeDefinition td)
        {
            foreach (MethodDefinition definition in td.Methods)
            {
                ProcessSiteMethod(moduleDef, td, definition);
            }
            foreach (TypeDefinition definition2 in td.NestedTypes)
            {
                ProcessSiteClass(moduleDef, definition2);
            }
        }

        private static void ProcessSiteMethod(ModuleDefinition moduleDef, TypeDefinition td, MethodDefinition md)
        {
            if (((md.Name != ".cctor") && (md.Name != "OnUnserializeVars")) && ((md.Name.Substring(0, Math.Min(md.Name.Length, 4)) != "UNet") && (md.Name.Substring(0, Math.Min(md.Name.Length, 7)) != "CallCmd")))
            {
                string str = md.Name.Substring(0, Math.Min(md.Name.Length, 9));
                if ((((str != "InvokeCmd") && (str != "InvokeRpc")) && (str != "InvokeSyn")) && ((md.Body != null) && (md.Body.Instructions != null)))
                {
                    foreach (CustomAttribute attribute in md.CustomAttributes)
                    {
                        if (attribute.Constructor.DeclaringType.ToString() == "UnityEngine.Networking.ServerAttribute")
                        {
                            InjectServerGuard(moduleDef, td, md, true);
                        }
                        else if (attribute.Constructor.DeclaringType.ToString() == "UnityEngine.Networking.ServerCallbackAttribute")
                        {
                            InjectServerGuard(moduleDef, td, md, false);
                        }
                        else if (attribute.Constructor.DeclaringType.ToString() == "UnityEngine.Networking.ClientAttribute")
                        {
                            InjectClientGuard(moduleDef, td, md, true);
                        }
                        else if (attribute.Constructor.DeclaringType.ToString() == "UnityEngine.Networking.ClientCallbackAttribute")
                        {
                            InjectClientGuard(moduleDef, td, md, false);
                        }
                    }
                    int iCount = 0;
                    foreach (Instruction instruction in md.Body.Instructions)
                    {
                        ProcessInstruction(moduleDef, td, md, instruction, iCount);
                        iCount++;
                    }
                }
            }
        }

        private static void ProcessSitesModule(ModuleDefinition moduleDef)
        {
            DateTime now = DateTime.Now;
            foreach (TypeDefinition definition in moduleDef.Types)
            {
                if (definition.IsClass)
                {
                    ProcessSiteClass(moduleDef, definition);
                }
            }
            if (lists.generateContainerClass != null)
            {
                moduleDef.Types.Add(lists.generateContainerClass);
                scriptDef.MainModule.ImportReference(lists.generateContainerClass);
                foreach (MethodDefinition definition2 in lists.generatedReadFunctions)
                {
                    scriptDef.MainModule.ImportReference(definition2);
                }
                foreach (MethodDefinition definition3 in lists.generatedWriteFunctions)
                {
                    scriptDef.MainModule.ImportReference(definition3);
                }
            }
            Console.WriteLine(string.Concat(new object[] { "  ProcessSitesModule ", moduleDef.Name, " elapsed time:", (TimeSpan) (DateTime.Now - now) }));
        }

        private static bool ProcessSyncListStructType(TypeDefinition td)
        {
            new SyncListStructProcessor(td).Process();
            return true;
        }

        public static void RegisterReadByReferenceFunc(string name, MethodDefinition newReaderFunc)
        {
            lists.readByReferenceFuncs[name] = newReaderFunc;
            lists.generatedReadFunctions.Add(newReaderFunc);
            ConfirmGeneratedCodeClass(scriptDef.MainModule);
            lists.generateContainerClass.Methods.Add(newReaderFunc);
        }

        public static void RegisterReadFunc(string name, MethodDefinition newReaderFunc)
        {
            lists.readFuncs[name] = newReaderFunc;
            lists.generatedReadFunctions.Add(newReaderFunc);
            ConfirmGeneratedCodeClass(scriptDef.MainModule);
            lists.generateContainerClass.Methods.Add(newReaderFunc);
        }

        public static void RegisterWriteFunc(string name, MethodDefinition newWriterFunc)
        {
            lists.writeFuncs[name] = newWriterFunc;
            lists.generatedWriteFunctions.Add(newWriterFunc);
            ConfirmGeneratedCodeClass(scriptDef.MainModule);
            lists.generateContainerClass.Methods.Add(newWriterFunc);
        }

        public static void ResetRecursionCount()
        {
            s_RecursionCount = 0;
        }

        private static MethodDefinition ResolveDefaultPublicCtor(TypeReference variable)
        {
            foreach (MethodDefinition definition in variable.Resolve().Methods)
            {
                if (((definition.Name == ".ctor") && definition.Resolve().IsPublic) && (definition.Parameters.Count == 0))
                {
                    return definition;
                }
            }
            return null;
        }

        public static FieldReference ResolveField(TypeReference t, string name)
        {
            foreach (FieldDefinition definition in t.Resolve().Fields)
            {
                if (definition.Name == name)
                {
                    return scriptDef.MainModule.ImportReference(definition);
                }
            }
            return null;
        }

        public static MethodReference ResolveMethod(TypeReference t, string name)
        {
            if (t == null)
            {
                Unity.UNetWeaver.Log.Error("Type missing for " + name);
                fail = true;
                return null;
            }
            foreach (MethodDefinition definition in t.Resolve().Methods)
            {
                if (definition.Name == name)
                {
                    return scriptDef.MainModule.ImportReference(definition);
                }
            }
            Unity.UNetWeaver.Log.Error(string.Concat(new object[] { "ResolveMethod failed ", t.Name, "::", name, " ", t.Resolve() }));
            foreach (MethodDefinition definition2 in t.Resolve().Methods)
            {
                Unity.UNetWeaver.Log.Error("Method " + definition2.Name);
            }
            fail = true;
            return null;
        }

        private static GenericInstanceMethod ResolveMethodGeneric(TypeReference t, string name, TypeReference genericType)
        {
            foreach (MethodDefinition definition in t.Resolve().Methods)
            {
                if (((definition.Name == name) && (definition.Parameters.Count == 0)) && (definition.GenericParameters.Count == 1))
                {
                    GenericInstanceMethod method = new GenericInstanceMethod(scriptDef.MainModule.ImportReference(definition)) {
                        GenericArguments = { genericType }
                    };
                    if (method.GenericArguments[0].FullName == genericType.FullName)
                    {
                        return method;
                    }
                }
            }
            Unity.UNetWeaver.Log.Error(string.Concat(new object[] { "ResolveMethodGeneric failed ", t.Name, "::", name, " ", genericType }));
            fail = true;
            return null;
        }

        private static MethodReference ResolveMethodWithArg(TypeReference t, string name, TypeReference argType)
        {
            foreach (MethodDefinition definition in t.Resolve().Methods)
            {
                if (((definition.Name == name) && (definition.Parameters.Count == 1)) && (definition.Parameters[0].ParameterType.FullName == argType.FullName))
                {
                    return scriptDef.MainModule.ImportReference(definition);
                }
            }
            Unity.UNetWeaver.Log.Error(string.Concat(new object[] { "ResolveMethodWithArg failed ", t.Name, "::", name, " ", argType }));
            fail = true;
            return null;
        }

        public static MethodReference ResolveProperty(TypeReference t, string name)
        {
            foreach (PropertyDefinition definition in t.Resolve().Properties)
            {
                if (definition.Name == name)
                {
                    return scriptDef.MainModule.ImportReference(definition.GetMethod);
                }
            }
            return null;
        }

        public static void SetNumSyncVars(string className, int num)
        {
            lists.numSyncVars[className] = num;
        }

        private static void SetupCorLib()
        {
            AssemblyNameReference name = AssemblyNameReference.Parse("mscorlib");
            ReaderParameters parameters = new ReaderParameters {
                AssemblyResolver = scriptDef.MainModule.AssemblyResolver
            };
            corLib = scriptDef.MainModule.AssemblyResolver.Resolve(name, parameters).MainModule;
        }

        private static void SetupReadFunctions()
        {
            Dictionary<string, MethodReference> dictionary = new Dictionary<string, MethodReference> {
                { 
                    singleType.FullName,
                    ResolveMethod(NetworkReaderType, "ReadSingle")
                },
                { 
                    doubleType.FullName,
                    ResolveMethod(NetworkReaderType, "ReadDouble")
                },
                { 
                    boolType.FullName,
                    ResolveMethod(NetworkReaderType, "ReadBoolean")
                },
                { 
                    stringType.FullName,
                    ResolveMethod(NetworkReaderType, "ReadString")
                },
                { 
                    int64Type.FullName,
                    NetworkReaderReadPacked64
                },
                { 
                    uint64Type.FullName,
                    NetworkReaderReadPacked64
                },
                { 
                    int32Type.FullName,
                    NetworkReaderReadPacked32
                },
                { 
                    uint32Type.FullName,
                    NetworkReaderReadPacked32
                },
                { 
                    int16Type.FullName,
                    NetworkReaderReadPacked32
                },
                { 
                    uint16Type.FullName,
                    NetworkReaderReadPacked32
                },
                { 
                    byteType.FullName,
                    NetworkReaderReadPacked32
                },
                { 
                    sbyteType.FullName,
                    NetworkReaderReadPacked32
                },
                { 
                    charType.FullName,
                    NetworkReaderReadPacked32
                },
                { 
                    decimalType.FullName,
                    ResolveMethod(NetworkReaderType, "ReadDecimal")
                },
                { 
                    vector2Type.FullName,
                    ResolveMethod(NetworkReaderType, "ReadVector2")
                },
                { 
                    vector3Type.FullName,
                    ResolveMethod(NetworkReaderType, "ReadVector3")
                },
                { 
                    vector4Type.FullName,
                    ResolveMethod(NetworkReaderType, "ReadVector4")
                },
                { 
                    colorType.FullName,
                    ResolveMethod(NetworkReaderType, "ReadColor")
                },
                { 
                    color32Type.FullName,
                    ResolveMethod(NetworkReaderType, "ReadColor32")
                },
                { 
                    quaternionType.FullName,
                    ResolveMethod(NetworkReaderType, "ReadQuaternion")
                },
                { 
                    rectType.FullName,
                    ResolveMethod(NetworkReaderType, "ReadRect")
                },
                { 
                    planeType.FullName,
                    ResolveMethod(NetworkReaderType, "ReadPlane")
                },
                { 
                    rayType.FullName,
                    ResolveMethod(NetworkReaderType, "ReadRay")
                },
                { 
                    matrixType.FullName,
                    ResolveMethod(NetworkReaderType, "ReadMatrix4x4")
                },
                { 
                    hashType.FullName,
                    ResolveMethod(NetworkReaderType, "ReadNetworkHash128")
                },
                { 
                    gameObjectType.FullName,
                    ResolveMethod(NetworkReaderType, "ReadGameObject")
                },
                { 
                    NetworkIdentityType.FullName,
                    ResolveMethod(NetworkReaderType, "ReadNetworkIdentity")
                },
                { 
                    NetworkInstanceIdType.FullName,
                    NetworkReaderReadNetworkInstanceId
                },
                { 
                    NetworkSceneIdType.FullName,
                    NetworkReaderReadNetworkSceneId
                },
                { 
                    transformType.FullName,
                    ResolveMethod(NetworkReaderType, "ReadTransform")
                },
                { 
                    "System.Byte[]",
                    ResolveMethod(NetworkReaderType, "ReadBytesAndSize")
                }
            };
            lists.readFuncs = dictionary;
            dictionary = new Dictionary<string, MethodReference> {
                { 
                    SyncListFloatType.FullName,
                    SyncListFloatReadType
                },
                { 
                    SyncListIntType.FullName,
                    SyncListIntReadType
                },
                { 
                    SyncListUIntType.FullName,
                    SyncListUIntReadType
                },
                { 
                    SyncListBoolType.FullName,
                    SyncListBoolReadType
                },
                { 
                    SyncListStringType.FullName,
                    SyncListStringReadType
                }
            };
            lists.readByReferenceFuncs = dictionary;
        }

        private static void SetupTargetTypes()
        {
            SetupCorLib();
            voidType = ImportCorLibType("System.Void");
            singleType = ImportCorLibType("System.Single");
            doubleType = ImportCorLibType("System.Double");
            decimalType = ImportCorLibType("System.Decimal");
            boolType = ImportCorLibType("System.Boolean");
            stringType = ImportCorLibType("System.String");
            int64Type = ImportCorLibType("System.Int64");
            uint64Type = ImportCorLibType("System.UInt64");
            int32Type = ImportCorLibType("System.Int32");
            uint32Type = ImportCorLibType("System.UInt32");
            int16Type = ImportCorLibType("System.Int16");
            uint16Type = ImportCorLibType("System.UInt16");
            byteType = ImportCorLibType("System.Byte");
            sbyteType = ImportCorLibType("System.SByte");
            charType = ImportCorLibType("System.Char");
            objectType = ImportCorLibType("System.Object");
            typeType = ImportCorLibType("System.Type");
            IEnumeratorType = ImportCorLibType("System.Collections.IEnumerator");
            MemoryStreamType = ImportCorLibType("System.IO.MemoryStream");
            NetworkReaderType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkReader");
            NetworkReaderDef = NetworkReaderType.Resolve();
            NetworkReaderCtor = ResolveMethod(NetworkReaderDef, ".ctor");
            NetworkWriterType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkWriter");
            NetworkWriterDef = NetworkWriterType.Resolve();
            NetworkWriterCtor = ResolveMethod(NetworkWriterDef, ".ctor");
            MemoryStreamCtor = ResolveMethod(MemoryStreamType, ".ctor");
            NetworkInstanceIdType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkInstanceId");
            NetworkSceneIdType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkSceneId");
            NetworkInstanceIdType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkInstanceId");
            NetworkSceneIdType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkSceneId");
            NetworkServerGetActive = ResolveMethod(NetworkServerType, "get_active");
            NetworkServerGetLocalClientActive = ResolveMethod(NetworkServerType, "get_localClientActive");
            NetworkClientGetActive = ResolveMethod(NetworkClientType, "get_active");
            NetworkReaderReadInt32 = ResolveMethod(NetworkReaderType, "ReadInt32");
            NetworkWriterWriteInt32 = ResolveMethodWithArg(NetworkWriterType, "Write", int32Type);
            NetworkWriterWriteInt16 = ResolveMethodWithArg(NetworkWriterType, "Write", int16Type);
            NetworkReaderReadPacked32 = ResolveMethod(NetworkReaderType, "ReadPackedUInt32");
            NetworkReaderReadPacked64 = ResolveMethod(NetworkReaderType, "ReadPackedUInt64");
            NetworkReaderReadByte = ResolveMethod(NetworkReaderType, "ReadByte");
            NetworkWriterWritePacked32 = ResolveMethod(NetworkWriterType, "WritePackedUInt32");
            NetworkWriterWritePacked64 = ResolveMethod(NetworkWriterType, "WritePackedUInt64");
            NetworkWriterWriteNetworkInstanceId = ResolveMethodWithArg(NetworkWriterType, "Write", NetworkInstanceIdType);
            NetworkWriterWriteNetworkSceneId = ResolveMethodWithArg(NetworkWriterType, "Write", NetworkSceneIdType);
            NetworkReaderReadNetworkInstanceId = ResolveMethod(NetworkReaderType, "ReadNetworkId");
            NetworkReaderReadNetworkSceneId = ResolveMethod(NetworkReaderType, "ReadSceneId");
            NetworkInstanceIsEmpty = ResolveMethod(NetworkInstanceIdType, "IsEmpty");
            NetworkReadUInt16 = ResolveMethod(NetworkReaderType, "ReadUInt16");
            NetworkWriteUInt16 = ResolveMethodWithArg(NetworkWriterType, "Write", uint16Type);
            CmdDelegateReference = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkBehaviour/CmdDelegate");
            CmdDelegateConstructor = ResolveMethod(CmdDelegateReference, ".ctor");
            scriptDef.MainModule.ImportReference(gameObjectType);
            scriptDef.MainModule.ImportReference(transformType);
            TypeReference type = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkIdentity");
            NetworkIdentityType = scriptDef.MainModule.ImportReference(type);
            NetworkInstanceIdType = scriptDef.MainModule.ImportReference(NetworkInstanceIdType);
            SyncListFloatReadType = ResolveMethod(SyncListFloatType, "ReadReference");
            SyncListIntReadType = ResolveMethod(SyncListIntType, "ReadReference");
            SyncListUIntReadType = ResolveMethod(SyncListUIntType, "ReadReference");
            SyncListBoolReadType = ResolveMethod(SyncListBoolType, "ReadReference");
            SyncListStringReadType = ResolveMethod(SyncListStringType, "ReadReference");
            SyncListFloatWriteType = ResolveMethod(SyncListFloatType, "WriteInstance");
            SyncListIntWriteType = ResolveMethod(SyncListIntType, "WriteInstance");
            SyncListUIntWriteType = ResolveMethod(SyncListUIntType, "WriteInstance");
            SyncListBoolWriteType = ResolveMethod(SyncListBoolType, "WriteInstance");
            SyncListStringWriteType = ResolveMethod(SyncListStringType, "WriteInstance");
            NetworkBehaviourType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkBehaviour");
            NetworkBehaviourType2 = scriptDef.MainModule.ImportReference(NetworkBehaviourType);
            NetworkConnectionType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkConnection");
            MonoBehaviourType = m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.MonoBehaviour");
            NetworkConnectionType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkConnection");
            NetworkConnectionType = scriptDef.MainModule.ImportReference(NetworkConnectionType);
            MessageBaseType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.MessageBase");
            SyncListStructType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.SyncListStruct`1");
            NetworkBehaviourDirtyBitsReference = ResolveProperty(NetworkBehaviourType, "syncVarDirtyBits");
            ComponentType = m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Component");
            ClientSceneType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.ClientScene");
            FindLocalObjectReference = ResolveMethod(ClientSceneType, "FindLocalObject");
            RegisterBehaviourReference = ResolveMethod(NetworkCRCType, "RegisterBehaviour");
            ReadyConnectionReference = ResolveMethod(ClientSceneType, "get_readyConnection");
            getComponentReference = ResolveMethodGeneric(ComponentType, "GetComponent", NetworkIdentityType);
            getUNetIdReference = ResolveMethod(type, "get_netId");
            gameObjectInequality = ResolveMethod(unityObjectType, "op_Inequality");
            UBehaviourIsServer = ResolveMethod(NetworkBehaviourType, "get_isServer");
            getPlayerIdReference = ResolveMethod(NetworkBehaviourType, "get_playerControllerId");
            setSyncVarReference = ResolveMethod(NetworkBehaviourType, "SetSyncVar");
            setSyncVarHookGuard = ResolveMethod(NetworkBehaviourType, "set_syncVarHookGuard");
            getSyncVarHookGuard = ResolveMethod(NetworkBehaviourType, "get_syncVarHookGuard");
            setSyncVarGameObjectReference = ResolveMethod(NetworkBehaviourType, "SetSyncVarGameObject");
            registerCommandDelegateReference = ResolveMethod(NetworkBehaviourType, "RegisterCommandDelegate");
            registerRpcDelegateReference = ResolveMethod(NetworkBehaviourType, "RegisterRpcDelegate");
            registerEventDelegateReference = ResolveMethod(NetworkBehaviourType, "RegisterEventDelegate");
            registerSyncListDelegateReference = ResolveMethod(NetworkBehaviourType, "RegisterSyncListDelegate");
            getTypeReference = ResolveMethod(objectType, "GetType");
            getTypeFromHandleReference = ResolveMethod(typeType, "GetTypeFromHandle");
            logErrorReference = ResolveMethod(m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Debug"), "LogError");
            logWarningReference = ResolveMethod(m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Debug"), "LogWarning");
            sendCommandInternal = ResolveMethod(NetworkBehaviourType, "SendCommandInternal");
            sendRpcInternal = ResolveMethod(NetworkBehaviourType, "SendRPCInternal");
            sendTargetRpcInternal = ResolveMethod(NetworkBehaviourType, "SendTargetRPCInternal");
            sendEventInternal = ResolveMethod(NetworkBehaviourType, "SendEventInternal");
            SyncListType = scriptDef.MainModule.ImportReference(SyncListType);
            SyncListInitBehaviourReference = ResolveMethod(SyncListType, "InitializeBehaviour");
            SyncListInitHandleMsg = ResolveMethod(SyncListType, "HandleMsg");
            SyncListClear = ResolveMethod(SyncListType, "Clear");
        }

        private static void SetupUnityTypes()
        {
            vector2Type = m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Vector2");
            vector3Type = m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Vector3");
            vector4Type = m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Vector4");
            colorType = m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Color");
            color32Type = m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Color32");
            quaternionType = m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Quaternion");
            rectType = m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Rect");
            planeType = m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Plane");
            rayType = m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Ray");
            matrixType = m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Matrix4x4");
            gameObjectType = m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.GameObject");
            transformType = m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Transform");
            unityObjectType = m_UnityAssemblyDefinition.MainModule.GetType("UnityEngine.Object");
            hashType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkHash128");
            NetworkClientType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkClient");
            NetworkServerType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkServer");
            NetworkCRCType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkCRC");
            SyncVarType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.SyncVarAttribute");
            CommandType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.CommandAttribute");
            ClientRpcType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.ClientRpcAttribute");
            TargetRpcType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.TargetRpcAttribute");
            SyncEventType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.SyncEventAttribute");
            SyncListType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.SyncList`1");
            NetworkSettingsType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.NetworkSettingsAttribute");
            SyncListFloatType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.SyncListFloat");
            SyncListIntType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.SyncListInt");
            SyncListUIntType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.SyncListUInt");
            SyncListBoolType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.SyncListBool");
            SyncListStringType = m_UNetAssemblyDefinition.MainModule.GetType("UnityEngine.Networking.SyncListString");
        }

        private static void SetupWriteFunctions()
        {
            Dictionary<string, MethodReference> dictionary = new Dictionary<string, MethodReference> {
                { 
                    singleType.FullName,
                    ResolveMethodWithArg(NetworkWriterType, "Write", singleType)
                },
                { 
                    doubleType.FullName,
                    ResolveMethodWithArg(NetworkWriterType, "Write", doubleType)
                },
                { 
                    boolType.FullName,
                    ResolveMethodWithArg(NetworkWriterType, "Write", boolType)
                },
                { 
                    stringType.FullName,
                    ResolveMethodWithArg(NetworkWriterType, "Write", stringType)
                },
                { 
                    int64Type.FullName,
                    NetworkWriterWritePacked64
                },
                { 
                    uint64Type.FullName,
                    NetworkWriterWritePacked64
                },
                { 
                    int32Type.FullName,
                    NetworkWriterWritePacked32
                },
                { 
                    uint32Type.FullName,
                    NetworkWriterWritePacked32
                },
                { 
                    int16Type.FullName,
                    NetworkWriterWritePacked32
                },
                { 
                    uint16Type.FullName,
                    NetworkWriterWritePacked32
                },
                { 
                    byteType.FullName,
                    NetworkWriterWritePacked32
                },
                { 
                    sbyteType.FullName,
                    NetworkWriterWritePacked32
                },
                { 
                    charType.FullName,
                    NetworkWriterWritePacked32
                },
                { 
                    decimalType.FullName,
                    ResolveMethodWithArg(NetworkWriterType, "Write", decimalType)
                },
                { 
                    vector2Type.FullName,
                    ResolveMethodWithArg(NetworkWriterType, "Write", vector2Type)
                },
                { 
                    vector3Type.FullName,
                    ResolveMethodWithArg(NetworkWriterType, "Write", vector3Type)
                },
                { 
                    vector4Type.FullName,
                    ResolveMethodWithArg(NetworkWriterType, "Write", vector4Type)
                },
                { 
                    colorType.FullName,
                    ResolveMethodWithArg(NetworkWriterType, "Write", colorType)
                },
                { 
                    color32Type.FullName,
                    ResolveMethodWithArg(NetworkWriterType, "Write", color32Type)
                },
                { 
                    quaternionType.FullName,
                    ResolveMethodWithArg(NetworkWriterType, "Write", quaternionType)
                },
                { 
                    rectType.FullName,
                    ResolveMethodWithArg(NetworkWriterType, "Write", rectType)
                },
                { 
                    planeType.FullName,
                    ResolveMethodWithArg(NetworkWriterType, "Write", planeType)
                },
                { 
                    rayType.FullName,
                    ResolveMethodWithArg(NetworkWriterType, "Write", rayType)
                },
                { 
                    matrixType.FullName,
                    ResolveMethodWithArg(NetworkWriterType, "Write", matrixType)
                },
                { 
                    hashType.FullName,
                    ResolveMethodWithArg(NetworkWriterType, "Write", hashType)
                },
                { 
                    gameObjectType.FullName,
                    ResolveMethodWithArg(NetworkWriterType, "Write", gameObjectType)
                },
                { 
                    NetworkIdentityType.FullName,
                    ResolveMethodWithArg(NetworkWriterType, "Write", NetworkIdentityType)
                },
                { 
                    NetworkInstanceIdType.FullName,
                    NetworkWriterWriteNetworkInstanceId
                },
                { 
                    NetworkSceneIdType.FullName,
                    NetworkWriterWriteNetworkSceneId
                },
                { 
                    transformType.FullName,
                    ResolveMethodWithArg(NetworkWriterType, "Write", transformType)
                },
                { 
                    "System.Byte[]",
                    ResolveMethod(NetworkWriterType, "WriteBytesFull")
                },
                { 
                    SyncListFloatType.FullName,
                    SyncListFloatWriteType
                },
                { 
                    SyncListIntType.FullName,
                    SyncListIntWriteType
                },
                { 
                    SyncListUIntType.FullName,
                    SyncListUIntWriteType
                },
                { 
                    SyncListBoolType.FullName,
                    SyncListBoolWriteType
                },
                { 
                    SyncListStringType.FullName,
                    SyncListStringWriteType
                }
            };
            lists.writeFuncs = dictionary;
        }

        private static bool Weave(string assName, IEnumerable<string> dependencies, IAssemblyResolver assemblyResolver, string unityEngineDLLPath, string unityUNetDLLPath, string outputDir)
        {
            if (assName.IndexOf("Assembly-") != -1)
            {
                if (assName.IndexOf("-Editor.") != -1)
                {
                    return true;
                }
                ReaderParameters parameters = Helpers.ReaderParameters(assName, dependencies, assemblyResolver, unityEngineDLLPath, unityUNetDLLPath);
                scriptDef = AssemblyDefinition.ReadAssembly(assName, parameters);
                SetupTargetTypes();
                SetupReadFunctions();
                SetupWriteFunctions();
                ModuleDefinition mainModule = scriptDef.MainModule;
                Console.WriteLine("Script Module: {0}", mainModule.Name);
                bool flag2 = false;
                for (int i = 0; i < 2; i++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    foreach (TypeDefinition definition2 in mainModule.Types)
                    {
                        if (definition2.IsClass)
                        {
                            try
                            {
                                if (i == 0)
                                {
                                    flag2 |= CheckSyncListStruct(definition2);
                                }
                                else
                                {
                                    flag2 |= CheckNetworkBehaviour(definition2);
                                    flag2 |= CheckMessageBase(definition2);
                                }
                            }
                            catch (Exception exception)
                            {
                                if (scriptDef.MainModule.SymbolReader != null)
                                {
                                    scriptDef.MainModule.SymbolReader.Dispose();
                                }
                                fail = true;
                                throw exception;
                            }
                        }
                        if (fail)
                        {
                            if (scriptDef.MainModule.SymbolReader != null)
                            {
                                scriptDef.MainModule.SymbolReader.Dispose();
                            }
                            return false;
                        }
                    }
                    stopwatch.Stop();
                    Console.WriteLine(string.Concat(new object[] { "Pass: ", i, " took ", stopwatch.ElapsedMilliseconds, " milliseconds" }));
                }
                if (flag2)
                {
                    foreach (MethodDefinition definition3 in lists.replacedMethods)
                    {
                        lists.replacementMethodNames.Add(definition3.FullName);
                    }
                    try
                    {
                        ProcessPropertySites();
                    }
                    catch (Exception exception2)
                    {
                        Unity.UNetWeaver.Log.Error("ProcessPropertySites exception: " + exception2);
                        if (scriptDef.MainModule.SymbolReader != null)
                        {
                            scriptDef.MainModule.SymbolReader.Dispose();
                        }
                        return false;
                    }
                    if (fail)
                    {
                        if (scriptDef.MainModule.SymbolReader != null)
                        {
                            scriptDef.MainModule.SymbolReader.Dispose();
                        }
                        return false;
                    }
                    string fileName = Helpers.DestinationFileFor(outputDir, assName);
                    WriterParameters writerParameters = Helpers.GetWriterParameters(parameters);
                    if (writerParameters.SymbolWriterProvider is PdbWriterProvider)
                    {
                        writerParameters.SymbolWriterProvider = new MdbWriterProvider();
                        File.Delete(Path.ChangeExtension(assName, ".pdb"));
                    }
                    scriptDef.Write(fileName, writerParameters);
                }
                if (scriptDef.MainModule.SymbolReader != null)
                {
                    scriptDef.MainModule.SymbolReader.Dispose();
                }
            }
            return true;
        }

        public static bool WeaveAssemblies(IEnumerable<string> assemblies, IEnumerable<string> dependencies, IAssemblyResolver assemblyResolver, string outputDir, string unityEngineDLLPath, string unityUNetDLLPath)
        {
            fail = false;
            lists = new WeaverLists();
            Console.WriteLine("WeaveAssemblies unityPath= " + unityEngineDLLPath);
            m_UnityAssemblyDefinition = AssemblyDefinition.ReadAssembly(unityEngineDLLPath);
            Console.WriteLine("WeaveAssemblies unetPath= " + unityUNetDLLPath);
            m_UNetAssemblyDefinition = AssemblyDefinition.ReadAssembly(unityUNetDLLPath);
            SetupUnityTypes();
            try
            {
                foreach (string str in assemblies)
                {
                    if (!Weave(str, dependencies, assemblyResolver, unityEngineDLLPath, unityUNetDLLPath, outputDir))
                    {
                        return false;
                    }
                }
            }
            catch (Exception exception)
            {
                Unity.UNetWeaver.Log.Error("Exception :" + exception);
                return false;
            }
            corLib = null;
            return true;
        }

        [CompilerGenerated]
        private sealed class <ImportCorLibType>c__AnonStorey0
        {
            internal string fullName;

            internal bool <>m__0(ExportedType t)
            {
                return (t.FullName == this.fullName);
            }
        }
    }
}

