using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.StackAnalysis
{
	public class StackStateBuilder
	{
		[Inject]
		public static ITypeProviderService TypeProvider;

		private readonly MethodDefinition _methodDefinition;

		private readonly TypeResolver _typeResolver;

		private readonly Stack<Entry> _simulationStack;

		[CompilerGenerated]
		private static StackAnalysisUtils.ResultTypeAnalysisMethod <>f__mg$cache0;

		[CompilerGenerated]
		private static StackAnalysisUtils.ResultTypeAnalysisMethod <>f__mg$cache1;

		[CompilerGenerated]
		private static StackAnalysisUtils.ResultTypeAnalysisMethod <>f__mg$cache2;

		private TypeReference Int32TypeReference
		{
			get
			{
				return StackStateBuilder.TypeProvider.Int32TypeReference;
			}
		}

		private TypeReference UInt32TypeReference
		{
			get
			{
				return StackStateBuilder.TypeProvider.UInt32TypeReference;
			}
		}

		private TypeReference SByteTypeReference
		{
			get
			{
				return StackStateBuilder.TypeProvider.SByteTypeReference;
			}
		}

		private TypeReference IntPtrTypeReference
		{
			get
			{
				return StackStateBuilder.TypeProvider.IntPtrTypeReference;
			}
		}

		private TypeReference UIntPtrTypeReference
		{
			get
			{
				return StackStateBuilder.TypeProvider.UIntPtrTypeReference;
			}
		}

		private TypeReference Int64TypeReference
		{
			get
			{
				return StackStateBuilder.TypeProvider.Int64TypeReference;
			}
		}

		private TypeReference SingleTypeReference
		{
			get
			{
				return StackStateBuilder.TypeProvider.SingleTypeReference;
			}
		}

		private TypeReference DoubleTypeReference
		{
			get
			{
				return StackStateBuilder.TypeProvider.DoubleTypeReference;
			}
		}

		private TypeReference ObjectTypeReference
		{
			get
			{
				return StackStateBuilder.TypeProvider.ObjectTypeReference;
			}
		}

		private TypeReference StringTypeReference
		{
			get
			{
				return StackStateBuilder.TypeProvider.StringTypeReference;
			}
		}

		private TypeReference NativeIntTypeReference
		{
			get
			{
				return StackStateBuilder.TypeProvider.NativeIntTypeReference;
			}
		}

		private TypeReference NativeUIntTypeReference
		{
			get
			{
				return StackStateBuilder.TypeProvider.NativeUIntTypeReference;
			}
		}

		private StackStateBuilder(MethodDefinition methodDefinition, StackState initialState, TypeResolver typeResolver)
		{
			this._methodDefinition = methodDefinition;
			this._typeResolver = typeResolver;
			this._simulationStack = new Stack<Entry>();
			foreach (Entry current in initialState.Entries.Reverse<Entry>())
			{
				this._simulationStack.Push(current.Clone());
			}
		}

		public static StackState StackStateFor(IEnumerable<Instruction> instructions, StackState initialState, MethodDefinition methodDefinition, TypeResolver typeResolver)
		{
			return new StackStateBuilder(methodDefinition, initialState, typeResolver).Build(instructions);
		}

		private StackState Build(IEnumerable<Instruction> instructions)
		{
			StackState stackState = new StackState();
			foreach (Instruction current in instructions)
			{
				this.SetupCatchBlockIfNeeded(current);
				switch (current.OpCode.Code)
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
					ParameterReference parameterReference = (ParameterReference)current.Operand;
					int num = parameterReference.Index;
					if (this._methodDefinition.HasThis)
					{
						num++;
					}
					this.LoadArg(num);
					break;
				}
				case Code.Ldarga_S:
					this.LoadArgumentAddress((ParameterReference)current.Operand);
					break;
				case Code.Starg_S:
					this.PopEntry();
					break;
				case Code.Ldloc_S:
					this.LoadLocal(((VariableReference)current.Operand).Index);
					break;
				case Code.Ldloca_S:
					this.LoadLocalAddress((VariableReference)current.Operand);
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
					this.CallMethod((MethodReference)current.Operand);
					break;
				case Code.Calli:
					this.CallInternalMethod((MethodReference)current.Operand);
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
					Entry entry = this.PopEntry();
					TypeReference typeReference = entry.Types.First<TypeReference>();
					TypeReference elementType = typeReference.GetElementType();
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
					ByReferenceType byReferenceType = (ByReferenceType)this.PopEntry().Types.First<TypeReference>();
					this.PushStackEntry(byReferenceType.ElementType);
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
				{
					Stack<Entry> arg_A1A_0 = this._simulationStack;
					if (StackStateBuilder.<>f__mg$cache0 == null)
					{
						StackStateBuilder.<>f__mg$cache0 = new StackAnalysisUtils.ResultTypeAnalysisMethod(StackAnalysisUtils.ResultTypeForAdd);
					}
					arg_A1A_0.Push(this.GetResultEntryUsing(StackStateBuilder.<>f__mg$cache0));
					break;
				}
				case Code.Sub:
				{
					Stack<Entry> arg_A4D_0 = this._simulationStack;
					if (StackStateBuilder.<>f__mg$cache1 == null)
					{
						StackStateBuilder.<>f__mg$cache1 = new StackAnalysisUtils.ResultTypeAnalysisMethod(StackAnalysisUtils.ResultTypeForSub);
					}
					arg_A4D_0.Push(this.GetResultEntryUsing(StackStateBuilder.<>f__mg$cache1));
					break;
				}
				case Code.Mul:
				{
					Stack<Entry> arg_A80_0 = this._simulationStack;
					if (StackStateBuilder.<>f__mg$cache2 == null)
					{
						StackStateBuilder.<>f__mg$cache2 = new StackAnalysisUtils.ResultTypeAnalysisMethod(StackAnalysisUtils.ResultTypeForMul);
					}
					arg_A80_0.Push(this.GetResultEntryUsing(StackStateBuilder.<>f__mg$cache2));
					break;
				}
				case Code.Div:
				{
					this.PopEntry();
					Entry entry2 = this.PopEntry();
					this._simulationStack.Push(entry2.Clone());
					break;
				}
				case Code.Div_Un:
					this.PopEntry();
					this.PopEntry();
					this.PushStackEntry(this.Int32TypeReference);
					break;
				case Code.Rem:
				{
					this.PopEntry();
					Entry entry3 = this.PopEntry();
					this._simulationStack.Push(entry3.Clone());
					break;
				}
				case Code.Rem_Un:
					this.PopEntry();
					this.PopEntry();
					this.PushStackEntry(this.Int32TypeReference);
					break;
				case Code.And:
				{
					this.PopEntry();
					Entry entry4 = this.PopEntry();
					this._simulationStack.Push(entry4.Clone());
					break;
				}
				case Code.Or:
				{
					this.PopEntry();
					Entry entry5 = this.PopEntry();
					this._simulationStack.Push(entry5.Clone());
					break;
				}
				case Code.Xor:
				{
					this.PopEntry();
					Entry entry6 = this.PopEntry();
					this._simulationStack.Push(entry6.Clone());
					break;
				}
				case Code.Shl:
				{
					this.PopEntry();
					Entry entry7 = this.PopEntry();
					this._simulationStack.Push(entry7.Clone());
					break;
				}
				case Code.Shr:
				{
					this.PopEntry();
					Entry entry8 = this.PopEntry();
					this._simulationStack.Push(entry8.Clone());
					break;
				}
				case Code.Shr_Un:
				{
					this.PopEntry();
					Entry entry9 = this.PopEntry();
					this._simulationStack.Push(entry9.Clone());
					break;
				}
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
					this.CallMethod((MethodReference)current.Operand);
					break;
				case Code.Cpobj:
					throw new NotImplementedException();
				case Code.Ldobj:
					this.PopEntry();
					this.PushStackEntry(this._typeResolver.Resolve((TypeReference)current.Operand));
					break;
				case Code.Ldstr:
					this.PushStackEntry(this.StringTypeReference);
					break;
				case Code.Newobj:
				{
					MethodReference methodReference = this._typeResolver.Resolve((MethodReference)current.Operand);
					this.CallConstructor(methodReference);
					this.PushStackEntry(this._typeResolver.Resolve(methodReference.DeclaringType));
					break;
				}
				case Code.Castclass:
					this.PopEntry();
					this.PushStackEntry(this._typeResolver.Resolve((TypeReference)current.Operand));
					break;
				case Code.Isinst:
					this.PopEntry();
					this.PushStackEntry(this._typeResolver.Resolve((TypeReference)current.Operand));
					break;
				case Code.Conv_R_Un:
					this.PopEntry();
					this.PushStackEntry(this.SingleTypeReference);
					break;
				case Code.Unbox:
					this.HandleStackStateForUnbox(current);
					break;
				case Code.Throw:
					this.PopEntry();
					break;
				case Code.Ldfld:
					this.PopEntry();
					this.PushStackEntry(this._typeResolver.ResolveFieldType((FieldReference)current.Operand));
					break;
				case Code.Ldflda:
					this.PopEntry();
					this.PushStackEntry(new ByReferenceType(this._typeResolver.ResolveFieldType((FieldReference)current.Operand)));
					break;
				case Code.Stfld:
					this.PopEntry();
					this.PopEntry();
					break;
				case Code.Ldsfld:
					this.PushStackEntry(this._typeResolver.ResolveFieldType((FieldReference)current.Operand));
					break;
				case Code.Ldsflda:
					this.PushStackEntry(new ByReferenceType(this._typeResolver.ResolveFieldType((FieldReference)current.Operand)));
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
					this.PushStackEntry(this.StackEntryForBoxedType((TypeReference)current.Operand));
					break;
				case Code.Newarr:
					this.PopEntry();
					this.PushStackEntry(new ArrayType(this._typeResolver.Resolve((TypeReference)current.Operand)));
					break;
				case Code.Ldlen:
					this.PopEntry();
					this.PushStackEntry(this.Int32TypeReference);
					break;
				case Code.Ldelema:
					this.LoadElement(new ByReferenceType(this._typeResolver.Resolve((TypeReference)current.Operand)));
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
					Entry entry10 = this.PopEntry();
					TypeReference typeReference2 = entry10.Types.First<TypeReference>();
					TypeReference elementType2 = typeReference2.GetElementType();
					if (elementType2.IsIntegralPointerType())
					{
						this.PushStackEntry(elementType2);
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
					this.PopEntry();
					Entry entry11 = this.PopEntry();
					TypeReference typeReference3 = entry11.Types.Single<TypeReference>();
					TypeReference typeReference4;
					if (typeReference3 is ArrayType || typeReference3 is TypeSpecification)
					{
						typeReference4 = ArrayUtilities.ArrayElementTypeOf(entry11.Types.Single<TypeReference>());
					}
					else
					{
						typeReference4 = typeReference3.GetElementType();
					}
					this.PushStackEntry(typeReference4);
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
					this.PushStackEntry(this._typeResolver.Resolve((TypeReference)current.Operand));
					break;
				case Code.Stelem_Any:
					this.PopEntry();
					this.PopEntry();
					this.PopEntry();
					break;
				case Code.Unbox_Any:
					this.HandleStackStateForUnbox(current);
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
					this.PushStackEntry(this.StackEntryForLdToken(current.Operand));
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
				{
					this.PopEntry();
					Entry entry12 = this.PopEntry();
					this._simulationStack.Push(entry12.Clone());
					break;
				}
				case Code.Add_Ovf_Un:
				{
					this.PopEntry();
					Entry entry13 = this.PopEntry();
					this._simulationStack.Push(entry13.Clone());
					break;
				}
				case Code.Mul_Ovf:
				{
					this.PopEntry();
					Entry entry14 = this.PopEntry();
					this._simulationStack.Push(entry14.Clone());
					break;
				}
				case Code.Mul_Ovf_Un:
				{
					this.PopEntry();
					Entry entry15 = this.PopEntry();
					this._simulationStack.Push(entry15.Clone());
					break;
				}
				case Code.Sub_Ovf:
				{
					this.PopEntry();
					Entry entry16 = this.PopEntry();
					this._simulationStack.Push(entry16.Clone());
					break;
				}
				case Code.Sub_Ovf_Un:
				{
					this.PopEntry();
					Entry entry17 = this.PopEntry();
					this._simulationStack.Push(entry17.Clone());
					break;
				}
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
					this.LoadArg(((ParameterReference)current.Operand).Index);
					break;
				case Code.Ldarga:
					this.LoadArgumentAddress((ParameterReference)current.Operand);
					break;
				case Code.Starg:
					this.PopEntry();
					break;
				case Code.Ldloc:
					this.LoadLocal((VariableReference)current.Operand);
					break;
				case Code.Ldloca:
					this.LoadLocalAddress((VariableReference)current.Operand);
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
					this.PushStackEntry(StackStateBuilder.TypeProvider.RuntimeTypeHandleTypeReference);
					break;
				}
			}
			foreach (Entry current2 in this._simulationStack.Reverse<Entry>())
			{
				stackState.Entries.Push(current2.Clone());
			}
			return stackState;
		}

		private void HandleStackStateForUnbox(Instruction instruction)
		{
			this.PopEntry();
			this.PushStackEntry(this._typeResolver.Resolve((TypeReference)instruction.Operand));
		}

		private Entry GetResultEntryUsing(StackAnalysisUtils.ResultTypeAnalysisMethod getResultType)
		{
			Entry entry = this.PopEntry();
			Entry entry2 = this.PopEntry();
			return new Entry
			{
				Types = 
				{
					getResultType(entry2.Types.First<TypeReference>(), entry.Types.First<TypeReference>(), StackStateBuilder.TypeProvider)
				}
			};
		}

		private bool CheckAnyPointerTypeInEntry(Entry entry)
		{
			bool result;
			foreach (TypeReference current in entry.Types)
			{
				if (current.IsSameType(this.NativeIntTypeReference))
				{
					result = true;
					return result;
				}
				if (current.IsSameType(this.NativeUIntTypeReference))
				{
					result = true;
					return result;
				}
				if (current.MetadataType == MetadataType.Pointer)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		private TypeReference StackEntryForBoxedType(TypeReference operandType)
		{
			TypeReference result;
			if (operandType == null)
			{
				result = this.ObjectTypeReference;
			}
			else
			{
				GenericParameter genericParameter = operandType as GenericParameter;
				if (genericParameter == null)
				{
					result = this.ObjectTypeReference;
				}
				else if (genericParameter.Constraints.Count == 0)
				{
					result = this.ObjectTypeReference;
				}
				else
				{
					TypeReference typeReference = this._typeResolver.Resolve(genericParameter);
					if (typeReference.IsValueType())
					{
						result = this.ObjectTypeReference;
					}
					else
					{
						result = typeReference;
					}
				}
			}
			return result;
		}

		private TypeReference StackEntryForLdToken(object operand)
		{
			TypeReference typeReference = operand as TypeReference;
			TypeReference result;
			if (typeReference != null)
			{
				result = StackStateBuilder.TypeProvider.RuntimeTypeHandleTypeReference;
			}
			else
			{
				FieldReference fieldReference = operand as FieldReference;
				if (fieldReference != null)
				{
					result = StackStateBuilder.TypeProvider.RuntimeFieldHandleTypeReference;
				}
				else
				{
					MethodReference methodReference = operand as MethodReference;
					if (methodReference == null)
					{
						throw new ArgumentException();
					}
					result = StackStateBuilder.TypeProvider.RuntimeMethodHandleTypeReference;
				}
			}
			return result;
		}

		private void LoadArgumentAddress(ParameterReference parameter)
		{
			this.PushStackEntry(new ByReferenceType(this._typeResolver.ResolveParameterType(this._methodDefinition, parameter)));
		}

		private void SetupCatchBlockIfNeeded(Instruction instruction)
		{
			MethodBody body = this._methodDefinition.Body;
			if (body.HasExceptionHandlers)
			{
				foreach (ExceptionHandler current in from h in body.ExceptionHandlers
				where h.HandlerStart.Offset == instruction.Offset
				select h)
				{
					if (current.HandlerType == ExceptionHandlerType.Catch)
					{
						this.PushStackEntry(this._typeResolver.Resolve(current.CatchType));
					}
				}
			}
		}

		private bool ReturnsValue()
		{
			return this._methodDefinition.ReturnType != null && this._methodDefinition.ReturnType.MetadataType != MetadataType.Void;
		}

		private void LoadElement(TypeReference typeReference)
		{
			this.PopEntry();
			this.PopEntry();
			this.PushStackEntry(typeReference);
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
				TypeReference typeReference = (!declaringType.IsValueType) ? declaringType : new ByReferenceType(declaringType);
				this.PushStackEntry(typeReference);
			}
			else
			{
				this.PushStackEntry(this._typeResolver.ResolveParameterType(this._methodDefinition, this._methodDefinition.Parameters[index]));
			}
		}

		private void LoadLocal(int index)
		{
			this.PushStackEntry(this._typeResolver.Resolve(this._methodDefinition.Body.Variables[index].VariableType));
		}

		private void LoadLocal(VariableReference variable)
		{
			this.PushStackEntry(this._typeResolver.Resolve(variable.VariableType));
		}

		private void LoadLocalAddress(VariableReference variable)
		{
			this.PushStackEntry(new ByReferenceType(this._typeResolver.Resolve(variable.VariableType)));
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
			if (returnType != null && returnType.MetadataType != MetadataType.Void)
			{
				this.PushStackEntry(this._typeResolver.ResolveReturnType(methodReference));
			}
		}

		private void CallConstructor(MethodReference methodReference)
		{
			for (int i = 0; i < methodReference.Parameters.Count; i++)
			{
				this.PopEntry();
			}
			TypeReference returnType = methodReference.ReturnType;
			if (returnType != null && returnType.MetadataType != MetadataType.Void)
			{
				this.PushStackEntry(returnType);
			}
		}

		private void CallInternalMethod(MethodReference methodReference)
		{
			this.PopEntry();
			this.CallMethod(methodReference);
		}

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
			TypeDefinition typeDefinition = typeReference.Resolve();
			if (typeReference.ContainsGenericParameters() && (typeDefinition == null || !typeDefinition.IsEnum))
			{
				throw new NotImplementedException();
			}
			this._simulationStack.Push(Entry.For(typeReference));
		}

		private Entry PopEntry()
		{
			return this._simulationStack.Pop();
		}

		private void EmptyStack()
		{
			while (this._simulationStack.Count > 0)
			{
				this.PopEntry();
			}
		}
	}
}
