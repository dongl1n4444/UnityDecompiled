namespace Unity.IL2CPP.StackAnalysis
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class StackStateBuilder
    {
        private readonly MethodDefinition _methodDefinition;
        private readonly Stack<Entry> _simulationStack;
        private readonly Unity.IL2CPP.ILPreProcessor.TypeResolver _typeResolver;
        [CompilerGenerated]
        private static StackAnalysisUtils.ResultTypeAnalysisMethod <>f__mg$cache0;
        [CompilerGenerated]
        private static StackAnalysisUtils.ResultTypeAnalysisMethod <>f__mg$cache1;
        [CompilerGenerated]
        private static StackAnalysisUtils.ResultTypeAnalysisMethod <>f__mg$cache2;
        [Inject]
        public static ITypeProviderService TypeProvider;

        private StackStateBuilder(MethodDefinition methodDefinition, StackState initialState, Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver)
        {
            this._methodDefinition = methodDefinition;
            this._typeResolver = typeResolver;
            this._simulationStack = new Stack<Entry>();
            foreach (Entry entry in initialState.Entries.Reverse<Entry>())
            {
                this._simulationStack.Push(entry.Clone());
            }
        }

        private StackState Build(IEnumerable<Instruction> instructions)
        {
            StackState state = new StackState();
            foreach (Instruction instruction in instructions)
            {
                this.SetupCatchBlockIfNeeded(instruction);
                switch (instruction.OpCode.Code)
                {
                    case Code.Ldarg_0:
                        this.LoadArg(0);
                        break;

                    case Code.Ldarg_1:
                        this.LoadArg(1);
                        break;

                    case Code.Ldarg_2:
                        this.LoadArg(2);
                        break;

                    case Code.Ldarg_3:
                        this.LoadArg(3);
                        break;

                    case Code.Ldloc_0:
                        this.LoadLocal(0);
                        break;

                    case Code.Ldloc_1:
                        this.LoadLocal(1);
                        break;

                    case Code.Ldloc_2:
                        this.LoadLocal(2);
                        break;

                    case Code.Ldloc_3:
                        this.LoadLocal(3);
                        break;

                    case Code.Stloc_0:
                        this.PopEntry();
                        break;

                    case Code.Stloc_1:
                        this.PopEntry();
                        break;

                    case Code.Stloc_2:
                        this.PopEntry();
                        break;

                    case Code.Stloc_3:
                        this.PopEntry();
                        break;

                    case Code.Ldarg_S:
                    {
                        ParameterReference operand = (ParameterReference) instruction.Operand;
                        int index = operand.Index;
                        if (this._methodDefinition.HasThis)
                        {
                            index++;
                        }
                        this.LoadArg(index);
                        break;
                    }
                    case Code.Ldarga_S:
                        this.LoadArgumentAddress((ParameterReference) instruction.Operand);
                        break;

                    case Code.Starg_S:
                        this.PopEntry();
                        break;

                    case Code.Ldloc_S:
                        this.LoadLocal(((VariableReference) instruction.Operand).Index);
                        break;

                    case Code.Ldloca_S:
                        this.LoadLocalAddress((VariableReference) instruction.Operand);
                        break;

                    case Code.Stloc_S:
                        this.PopEntry();
                        break;

                    case Code.Ldnull:
                        this.PushNullStackEntry();
                        break;

                    case Code.Ldc_I4_M1:
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Ldc_I4_0:
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Ldc_I4_1:
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Ldc_I4_2:
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Ldc_I4_3:
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Ldc_I4_4:
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Ldc_I4_5:
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Ldc_I4_6:
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Ldc_I4_7:
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Ldc_I4_8:
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Ldc_I4_S:
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Ldc_I4:
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Ldc_I8:
                        this.PushStackEntry(this.Int64TypeReference);
                        break;

                    case Code.Ldc_R4:
                        this.PushStackEntry(this.SingleTypeReference);
                        break;

                    case Code.Ldc_R8:
                        this.PushStackEntry(this.DoubleTypeReference);
                        break;

                    case Code.Dup:
                        this._simulationStack.Push(this._simulationStack.Peek().Clone());
                        break;

                    case Code.Pop:
                        this.PopEntry();
                        break;

                    case Code.Call:
                        this.CallMethod((MethodReference) instruction.Operand);
                        break;

                    case Code.Calli:
                        this.CallInternalMethod((MethodReference) instruction.Operand);
                        break;

                    case Code.Ret:
                        if (this.ReturnsValue())
                        {
                            this.PopEntry();
                        }
                        break;

                    case Code.Brfalse_S:
                        this.PopEntry();
                        break;

                    case Code.Brtrue_S:
                        this.PopEntry();
                        break;

                    case Code.Beq_S:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Bge_S:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Bgt_S:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Ble_S:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Blt_S:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Bne_Un_S:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Bge_Un_S:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Bgt_Un_S:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Ble_Un_S:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Blt_Un_S:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Brfalse:
                        this.PopEntry();
                        break;

                    case Code.Brtrue:
                        this.PopEntry();
                        break;

                    case Code.Beq:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Bge:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Bgt:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Ble:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Blt:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Bne_Un:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Bge_Un:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Bgt_Un:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Ble_Un:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Blt_Un:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Switch:
                        this.PopEntry();
                        break;

                    case Code.Ldind_I1:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Ldind_U1:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Ldind_I2:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Ldind_U2:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Ldind_I4:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Ldind_U4:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Ldind_I8:
                        this.PopEntry();
                        this.PushStackEntry(this.Int64TypeReference);
                        break;

                    case Code.Ldind_I:
                    {
                        TypeReference elementType = this.PopEntry().Types.First<TypeReference>().GetElementType();
                        if (elementType.IsIntegralPointerType())
                        {
                            this.PushStackEntry(elementType);
                        }
                        else
                        {
                            this.PushStackEntry(this.NativeIntTypeReference);
                        }
                        break;
                    }
                    case Code.Ldind_R4:
                        this.PopEntry();
                        this.PushStackEntry(this.SingleTypeReference);
                        break;

                    case Code.Ldind_R8:
                        this.PopEntry();
                        this.PushStackEntry(this.DoubleTypeReference);
                        break;

                    case Code.Ldind_Ref:
                    {
                        ByReferenceType type = (ByReferenceType) this.PopEntry().Types.First<TypeReference>();
                        this.PushStackEntry(type.ElementType);
                        break;
                    }
                    case Code.Stind_Ref:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Stind_I1:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Stind_I2:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Stind_I4:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Stind_I8:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Stind_R4:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Stind_R8:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Add:
                        if (<>f__mg$cache0 == null)
                        {
                            <>f__mg$cache0 = new StackAnalysisUtils.ResultTypeAnalysisMethod(StackAnalysisUtils.ResultTypeForAdd);
                        }
                        this._simulationStack.Push(this.GetResultEntryUsing(<>f__mg$cache0));
                        break;

                    case Code.Sub:
                        if (<>f__mg$cache1 == null)
                        {
                            <>f__mg$cache1 = new StackAnalysisUtils.ResultTypeAnalysisMethod(StackAnalysisUtils.ResultTypeForSub);
                        }
                        this._simulationStack.Push(this.GetResultEntryUsing(<>f__mg$cache1));
                        break;

                    case Code.Mul:
                        if (<>f__mg$cache2 == null)
                        {
                            <>f__mg$cache2 = new StackAnalysisUtils.ResultTypeAnalysisMethod(StackAnalysisUtils.ResultTypeForMul);
                        }
                        this._simulationStack.Push(this.GetResultEntryUsing(<>f__mg$cache2));
                        break;

                    case Code.Div:
                        this.PopEntry();
                        this._simulationStack.Push(this.PopEntry().Clone());
                        break;

                    case Code.Div_Un:
                        this.PopEntry();
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Rem:
                        this.PopEntry();
                        this._simulationStack.Push(this.PopEntry().Clone());
                        break;

                    case Code.Rem_Un:
                        this.PopEntry();
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.And:
                        this.PopEntry();
                        this._simulationStack.Push(this.PopEntry().Clone());
                        break;

                    case Code.Or:
                        this.PopEntry();
                        this._simulationStack.Push(this.PopEntry().Clone());
                        break;

                    case Code.Xor:
                        this.PopEntry();
                        this._simulationStack.Push(this.PopEntry().Clone());
                        break;

                    case Code.Shl:
                        this.PopEntry();
                        this._simulationStack.Push(this.PopEntry().Clone());
                        break;

                    case Code.Shr:
                        this.PopEntry();
                        this._simulationStack.Push(this.PopEntry().Clone());
                        break;

                    case Code.Shr_Un:
                        this.PopEntry();
                        this._simulationStack.Push(this.PopEntry().Clone());
                        break;

                    case Code.Conv_I1:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Conv_I2:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Conv_I4:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Conv_I8:
                        this.PopEntry();
                        this.PushStackEntry(this.Int64TypeReference);
                        break;

                    case Code.Conv_R4:
                        this.PopEntry();
                        this.PushStackEntry(this.SingleTypeReference);
                        break;

                    case Code.Conv_R8:
                        this.PopEntry();
                        this.PushStackEntry(this.DoubleTypeReference);
                        break;

                    case Code.Conv_U4:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Conv_U8:
                        this.PopEntry();
                        this.PushStackEntry(this.Int64TypeReference);
                        break;

                    case Code.Callvirt:
                        this.CallMethod((MethodReference) instruction.Operand);
                        break;

                    case Code.Cpobj:
                        throw new NotImplementedException();

                    case Code.Ldobj:
                        this.PopEntry();
                        this.PushStackEntry(this._typeResolver.Resolve((TypeReference) instruction.Operand));
                        break;

                    case Code.Ldstr:
                        this.PushStackEntry(this.StringTypeReference);
                        break;

                    case Code.Newobj:
                    {
                        MethodReference methodReference = this._typeResolver.Resolve((MethodReference) instruction.Operand);
                        this.CallConstructor(methodReference);
                        this.PushStackEntry(this._typeResolver.Resolve(methodReference.DeclaringType));
                        break;
                    }
                    case Code.Castclass:
                        this.PopEntry();
                        this.PushStackEntry(this._typeResolver.Resolve((TypeReference) instruction.Operand));
                        break;

                    case Code.Isinst:
                        this.PopEntry();
                        this.PushStackEntry(this._typeResolver.Resolve((TypeReference) instruction.Operand));
                        break;

                    case Code.Conv_R_Un:
                        this.PopEntry();
                        this.PushStackEntry(this.SingleTypeReference);
                        break;

                    case Code.Unbox:
                        this.HandleStackStateForUnbox(instruction);
                        break;

                    case Code.Throw:
                        this.PopEntry();
                        break;

                    case Code.Ldfld:
                        this.PopEntry();
                        this.PushStackEntry(this._typeResolver.ResolveFieldType((FieldReference) instruction.Operand));
                        break;

                    case Code.Ldflda:
                        this.PopEntry();
                        this.PushStackEntry(new ByReferenceType(this._typeResolver.ResolveFieldType((FieldReference) instruction.Operand)));
                        break;

                    case Code.Stfld:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Ldsfld:
                        this.PushStackEntry(this._typeResolver.ResolveFieldType((FieldReference) instruction.Operand));
                        break;

                    case Code.Ldsflda:
                        this.PushStackEntry(new ByReferenceType(this._typeResolver.ResolveFieldType((FieldReference) instruction.Operand)));
                        break;

                    case Code.Stsfld:
                        this.PopEntry();
                        break;

                    case Code.Stobj:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Conv_Ovf_I1_Un:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Conv_Ovf_I2_Un:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Conv_Ovf_I4_Un:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Conv_Ovf_I8_Un:
                        this.PopEntry();
                        this.PushStackEntry(this.Int64TypeReference);
                        break;

                    case Code.Conv_Ovf_U1_Un:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Conv_Ovf_U2_Un:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Conv_Ovf_U4_Un:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Conv_Ovf_U8_Un:
                        this.PopEntry();
                        this.PushStackEntry(this.Int64TypeReference);
                        break;

                    case Code.Conv_Ovf_I_Un:
                        this.PopEntry();
                        this.PushStackEntry(this.NativeIntTypeReference);
                        break;

                    case Code.Conv_Ovf_U_Un:
                        this.PopEntry();
                        this.PushStackEntry(this.NativeUIntTypeReference);
                        break;

                    case Code.Box:
                        this.PopEntry();
                        this.PushStackEntry(this.StackEntryForBoxedType((TypeReference) instruction.Operand));
                        break;

                    case Code.Newarr:
                        this.PopEntry();
                        this.PushStackEntry(new ArrayType(this._typeResolver.Resolve((TypeReference) instruction.Operand)));
                        break;

                    case Code.Ldlen:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Ldelema:
                        this.LoadElement(new ByReferenceType(this._typeResolver.Resolve((TypeReference) instruction.Operand)));
                        break;

                    case Code.Ldelem_I1:
                        this.LoadElement(this.Int32TypeReference);
                        break;

                    case Code.Ldelem_U1:
                        this.LoadElement(this.Int32TypeReference);
                        break;

                    case Code.Ldelem_I2:
                        this.LoadElement(this.Int32TypeReference);
                        break;

                    case Code.Ldelem_U2:
                        this.LoadElement(this.Int32TypeReference);
                        break;

                    case Code.Ldelem_I4:
                        this.LoadElement(this.Int32TypeReference);
                        break;

                    case Code.Ldelem_U4:
                        this.LoadElement(this.Int32TypeReference);
                        break;

                    case Code.Ldelem_I8:
                        this.LoadElement(this.Int64TypeReference);
                        break;

                    case Code.Ldelem_I:
                    {
                        this.PopEntry();
                        TypeReference typeReference = this.PopEntry().Types.First<TypeReference>().GetElementType();
                        if (typeReference.IsIntegralPointerType())
                        {
                            this.PushStackEntry(typeReference);
                        }
                        else
                        {
                            this.PushStackEntry(this.Int32TypeReference);
                        }
                        break;
                    }
                    case Code.Ldelem_R4:
                        this.LoadElement(this.SingleTypeReference);
                        break;

                    case Code.Ldelem_R8:
                        this.LoadElement(this.DoubleTypeReference);
                        break;

                    case Code.Ldelem_Ref:
                    {
                        TypeReference reference8;
                        this.PopEntry();
                        Entry entry11 = this.PopEntry();
                        TypeReference reference7 = entry11.Types.Single<TypeReference>();
                        if ((reference7 is ArrayType) || (reference7 is TypeSpecification))
                        {
                            reference8 = ArrayUtilities.ArrayElementTypeOf(entry11.Types.Single<TypeReference>());
                        }
                        else
                        {
                            reference8 = reference7.GetElementType();
                        }
                        this.PushStackEntry(reference8);
                        break;
                    }
                    case Code.Stelem_I:
                        this.PopEntry();
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Stelem_I1:
                        this.PopEntry();
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Stelem_I2:
                        this.PopEntry();
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Stelem_I4:
                        this.PopEntry();
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Stelem_I8:
                        this.PopEntry();
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Stelem_R4:
                        this.PopEntry();
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Stelem_R8:
                        this.PopEntry();
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Stelem_Ref:
                        this.PopEntry();
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Ldelem_Any:
                        this.PopEntry();
                        this.PopEntry();
                        this.PushStackEntry(this._typeResolver.Resolve((TypeReference) instruction.Operand));
                        break;

                    case Code.Stelem_Any:
                        this.PopEntry();
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Unbox_Any:
                        this.HandleStackStateForUnbox(instruction);
                        break;

                    case Code.Conv_Ovf_I1:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Conv_Ovf_U1:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Conv_Ovf_I2:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Conv_Ovf_U2:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Conv_Ovf_I4:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Conv_Ovf_U4:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Conv_Ovf_I8:
                        this.PopEntry();
                        this.PushStackEntry(this.Int64TypeReference);
                        break;

                    case Code.Conv_Ovf_U8:
                        this.PopEntry();
                        this.PushStackEntry(this.Int64TypeReference);
                        break;

                    case Code.Refanyval:
                        throw new NotImplementedException();

                    case Code.Ckfinite:
                        throw new NotImplementedException();

                    case Code.Mkrefany:
                        throw new NotImplementedException();

                    case Code.Ldtoken:
                        this.PushStackEntry(this.StackEntryForLdToken(instruction.Operand));
                        break;

                    case Code.Conv_U2:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Conv_U1:
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Conv_I:
                        this.PopEntry();
                        this.PushStackEntry(this.NativeIntTypeReference);
                        break;

                    case Code.Conv_Ovf_I:
                        this.PopEntry();
                        this.PushStackEntry(this.NativeIntTypeReference);
                        break;

                    case Code.Conv_Ovf_U:
                        this.PopEntry();
                        this.PushStackEntry(this.NativeUIntTypeReference);
                        break;

                    case Code.Add_Ovf:
                        this.PopEntry();
                        this._simulationStack.Push(this.PopEntry().Clone());
                        break;

                    case Code.Add_Ovf_Un:
                        this.PopEntry();
                        this._simulationStack.Push(this.PopEntry().Clone());
                        break;

                    case Code.Mul_Ovf:
                        this.PopEntry();
                        this._simulationStack.Push(this.PopEntry().Clone());
                        break;

                    case Code.Mul_Ovf_Un:
                        this.PopEntry();
                        this._simulationStack.Push(this.PopEntry().Clone());
                        break;

                    case Code.Sub_Ovf:
                        this.PopEntry();
                        this._simulationStack.Push(this.PopEntry().Clone());
                        break;

                    case Code.Sub_Ovf_Un:
                        this.PopEntry();
                        this._simulationStack.Push(this.PopEntry().Clone());
                        break;

                    case Code.Leave:
                        this.EmptyStack();
                        break;

                    case Code.Leave_S:
                        this.EmptyStack();
                        break;

                    case Code.Stind_I:
                        this.PopEntry();
                        this.PopEntry();
                        break;

                    case Code.Conv_U:
                        this.PopEntry();
                        this.PushStackEntry(this.NativeUIntTypeReference);
                        break;

                    case Code.Arglist:
                        this.PushStackEntry(this.NativeIntTypeReference);
                        break;

                    case Code.Ceq:
                        this.PopEntry();
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Cgt:
                        this.PopEntry();
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Cgt_Un:
                        this.PopEntry();
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Clt:
                        this.PopEntry();
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Clt_Un:
                        this.PopEntry();
                        this.PopEntry();
                        this.PushStackEntry(this.Int32TypeReference);
                        break;

                    case Code.Ldftn:
                        this.PushStackEntry(this.IntPtrTypeReference);
                        break;

                    case Code.Ldvirtftn:
                        this.PopEntry();
                        this.PushStackEntry(this.IntPtrTypeReference);
                        break;

                    case Code.Ldarg:
                        this.LoadArg(((ParameterReference) instruction.Operand).Index);
                        break;

                    case Code.Ldarga:
                        this.LoadArgumentAddress((ParameterReference) instruction.Operand);
                        break;

                    case Code.Starg:
                        this.PopEntry();
                        break;

                    case Code.Ldloc:
                        this.LoadLocal((VariableReference) instruction.Operand);
                        break;

                    case Code.Ldloca:
                        this.LoadLocalAddress((VariableReference) instruction.Operand);
                        break;

                    case Code.Stloc:
                        this.PopEntry();
                        break;

                    case Code.Localloc:
                        this.PopEntry();
                        this.PushStackEntry(new PointerType(this.SByteTypeReference));
                        break;

                    case Code.Endfilter:
                        throw new NotImplementedException();

                    case Code.Unaligned:
                        throw new NotImplementedException();

                    case Code.Initobj:
                        this.PopEntry();
                        break;

                    case Code.Cpblk:
                        throw new NotImplementedException();

                    case Code.Initblk:
                        throw new NotImplementedException();

                    case Code.No:
                        throw new NotImplementedException();

                    case Code.Sizeof:
                        this.PushStackEntry(this.UInt32TypeReference);
                        break;

                    case Code.Refanytype:
                        this.PopEntry();
                        this.PushStackEntry(TypeProvider.RuntimeTypeHandleTypeReference);
                        break;
                }
            }
            foreach (Entry entry18 in this._simulationStack.Reverse<Entry>())
            {
                state.Entries.Push(entry18.Clone());
            }
            return state;
        }

        private void CallConstructor(MethodReference methodReference)
        {
            for (int i = 0; i < methodReference.Parameters.Count; i++)
            {
                this.PopEntry();
            }
            TypeReference returnType = methodReference.ReturnType;
            if ((returnType != null) && (returnType.MetadataType != MetadataType.Void))
            {
                this.PushStackEntry(returnType);
            }
        }

        private void CallInternalMethod(MethodReference methodReference)
        {
            this.PopEntry();
            this.CallMethod(methodReference);
        }

        private void CallMethod(MethodReference methodReference)
        {
            for (int i = 0; i < methodReference.Parameters.Count; i++)
            {
                this.PopEntry();
            }
            if (methodReference.HasThis)
            {
                this.PopEntry();
            }
            TypeReference returnType = methodReference.ReturnType;
            if ((returnType != null) && (returnType.MetadataType != MetadataType.Void))
            {
                this.PushStackEntry(this._typeResolver.ResolveReturnType(methodReference));
            }
        }

        private bool CheckAnyPointerTypeInEntry(Entry entry)
        {
            foreach (TypeReference reference in entry.Types)
            {
                if (reference.IsSameType(this.NativeIntTypeReference))
                {
                    return true;
                }
                if (reference.IsSameType(this.NativeUIntTypeReference))
                {
                    return true;
                }
                if (reference.MetadataType == MetadataType.Pointer)
                {
                    return true;
                }
            }
            return false;
        }

        private void EmptyStack()
        {
            while (this._simulationStack.Count > 0)
            {
                this.PopEntry();
            }
        }

        private Entry GetResultEntryUsing(StackAnalysisUtils.ResultTypeAnalysisMethod getResultType)
        {
            Entry entry = this.PopEntry();
            Entry entry2 = this.PopEntry();
            return new Entry { Types = { getResultType(entry2.Types.First<TypeReference>(), entry.Types.First<TypeReference>(), TypeProvider) } };
        }

        private void HandleStackStateForUnbox(Instruction instruction)
        {
            this.PopEntry();
            this.PushStackEntry(this._typeResolver.Resolve((TypeReference) instruction.Operand));
        }

        private void LoadArg(int index)
        {
            if (this._methodDefinition.HasThis)
            {
                index--;
            }
            if (index < 0)
            {
                TypeDefinition declaringType = this._methodDefinition.DeclaringType;
                TypeReference typeReference = !declaringType.IsValueType ? ((TypeReference) declaringType) : ((TypeReference) new ByReferenceType(declaringType));
                this.PushStackEntry(typeReference);
            }
            else
            {
                this.PushStackEntry(this._typeResolver.ResolveParameterType(this._methodDefinition, this._methodDefinition.Parameters[index]));
            }
        }

        private void LoadArgumentAddress(ParameterReference parameter)
        {
            this.PushStackEntry(new ByReferenceType(this._typeResolver.ResolveParameterType(this._methodDefinition, parameter)));
        }

        private void LoadElement(TypeReference typeReference)
        {
            this.PopEntry();
            this.PopEntry();
            this.PushStackEntry(typeReference);
        }

        private void LoadLocal(VariableReference variable)
        {
            this.PushStackEntry(this._typeResolver.Resolve(variable.VariableType));
        }

        private void LoadLocal(int index)
        {
            this.PushStackEntry(this._typeResolver.Resolve(this._methodDefinition.Body.Variables[index].VariableType));
        }

        private void LoadLocalAddress(VariableReference variable)
        {
            this.PushStackEntry(new ByReferenceType(this._typeResolver.Resolve(variable.VariableType)));
        }

        private Entry PopEntry() => 
            this._simulationStack.Pop();

        private void PushNullStackEntry()
        {
            this._simulationStack.Push(Entry.ForNull(this.ObjectTypeReference));
        }

        private void PushStackEntry(TypeReference typeReference)
        {
            if (typeReference == null)
            {
                throw new ArgumentNullException("typeReference");
            }
            TypeDefinition definition = typeReference.Resolve();
            if (typeReference.ContainsGenericParameters() && ((definition == null) || !definition.IsEnum))
            {
                throw new NotImplementedException();
            }
            this._simulationStack.Push(Entry.For(typeReference));
        }

        private bool ReturnsValue() => 
            ((this._methodDefinition.ReturnType != null) && (this._methodDefinition.ReturnType.MetadataType != MetadataType.Void));

        private void SetupCatchBlockIfNeeded(Instruction instruction)
        {
            <SetupCatchBlockIfNeeded>c__AnonStorey0 storey = new <SetupCatchBlockIfNeeded>c__AnonStorey0 {
                instruction = instruction
            };
            MethodBody body = this._methodDefinition.Body;
            if (body.HasExceptionHandlers)
            {
                foreach (ExceptionHandler handler in body.ExceptionHandlers.Where<ExceptionHandler>(new Func<ExceptionHandler, bool>(storey.<>m__0)))
                {
                    if (handler.HandlerType == ExceptionHandlerType.Catch)
                    {
                        this.PushStackEntry(this._typeResolver.Resolve(handler.CatchType));
                    }
                }
            }
        }

        private TypeReference StackEntryForBoxedType(TypeReference operandType)
        {
            if (operandType == null)
            {
                return this.ObjectTypeReference;
            }
            GenericParameter typeReference = operandType as GenericParameter;
            if (typeReference == null)
            {
                return this.ObjectTypeReference;
            }
            if (typeReference.Constraints.Count == 0)
            {
                return this.ObjectTypeReference;
            }
            TypeReference reference2 = this._typeResolver.Resolve(typeReference);
            if (reference2.IsValueType())
            {
                return this.ObjectTypeReference;
            }
            return reference2;
        }

        private TypeReference StackEntryForLdToken(object operand)
        {
            if (operand is TypeReference)
            {
                return TypeProvider.RuntimeTypeHandleTypeReference;
            }
            if (operand is FieldReference)
            {
                return TypeProvider.RuntimeFieldHandleTypeReference;
            }
            if (!(operand is MethodReference))
            {
                throw new ArgumentException();
            }
            return TypeProvider.RuntimeMethodHandleTypeReference;
        }

        public static StackState StackStateFor(IEnumerable<Instruction> instructions, StackState initialState, MethodDefinition methodDefinition, Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver) => 
            new StackStateBuilder(methodDefinition, initialState, typeResolver).Build(instructions);

        private TypeReference DoubleTypeReference =>
            TypeProvider.DoubleTypeReference;

        private TypeReference Int32TypeReference =>
            TypeProvider.Int32TypeReference;

        private TypeReference Int64TypeReference =>
            TypeProvider.Int64TypeReference;

        private TypeReference IntPtrTypeReference =>
            TypeProvider.IntPtrTypeReference;

        private TypeReference NativeIntTypeReference =>
            TypeProvider.NativeIntTypeReference;

        private TypeReference NativeUIntTypeReference =>
            TypeProvider.NativeUIntTypeReference;

        private TypeReference ObjectTypeReference =>
            TypeProvider.ObjectTypeReference;

        private TypeReference SByteTypeReference =>
            TypeProvider.SByteTypeReference;

        private TypeReference SingleTypeReference =>
            TypeProvider.SingleTypeReference;

        private TypeReference StringTypeReference =>
            TypeProvider.StringTypeReference;

        private TypeReference UInt32TypeReference =>
            TypeProvider.UInt32TypeReference;

        private TypeReference UIntPtrTypeReference =>
            TypeProvider.UIntPtrTypeReference;

        [CompilerGenerated]
        private sealed class <SetupCatchBlockIfNeeded>c__AnonStorey0
        {
            internal Instruction instruction;

            internal bool <>m__0(ExceptionHandler h) => 
                (h.HandlerStart.Offset == this.instruction.Offset);
        }
    }
}

