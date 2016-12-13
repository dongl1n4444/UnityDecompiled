using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Common.CFG
{
	internal class ControlFlowGraphBuilder
	{
		private MethodBody body;

		private Dictionary<int, InstructionData> data;

		private Dictionary<int, InstructionBlock> blocks = new Dictionary<int, InstructionBlock>();

		private List<ExceptionHandlerData> exception_data;

		private HashSet<int> exception_objects_offsets;

		internal ControlFlowGraphBuilder(MethodDefinition method)
		{
			this.body = method.Body;
			if (this.body.ExceptionHandlers.Count > 0)
			{
				this.exception_objects_offsets = new HashSet<int>();
			}
		}

		public ControlFlowGraph CreateGraph()
		{
			this.DelimitBlocks();
			this.ConnectBlocks();
			this.ComputeInstructionData();
			this.ComputeExceptionHandlerData();
			return new ControlFlowGraph(this.body, this.ToArray(), this.data, this.exception_data, this.exception_objects_offsets);
		}

		private void DelimitBlocks()
		{
			Collection<Instruction> instructions = this.body.Instructions;
			this.MarkBlockStarts(instructions);
			Collection<ExceptionHandler> exceptionHandlers = this.body.ExceptionHandlers;
			this.MarkBlockStarts(exceptionHandlers);
			this.MarkBlockEnds(instructions);
		}

		private void MarkBlockStarts(IList<ExceptionHandler> handlers)
		{
			for (int i = 0; i < handlers.Count; i++)
			{
				ExceptionHandler exceptionHandler = handlers[i];
				this.MarkBlockStart(exceptionHandler.TryStart);
				this.MarkBlockStart(exceptionHandler.HandlerStart);
				if (exceptionHandler.HandlerType == ExceptionHandlerType.Filter)
				{
					this.MarkExceptionObjectPosition(exceptionHandler.FilterStart);
					this.MarkBlockStart(exceptionHandler.FilterStart);
				}
				else if (exceptionHandler.HandlerType == ExceptionHandlerType.Catch)
				{
					this.MarkExceptionObjectPosition(exceptionHandler.HandlerStart);
					this.MarkBlockStart(exceptionHandler.HandlerEnd);
				}
			}
		}

		private void MarkExceptionObjectPosition(Instruction instruction)
		{
			this.exception_objects_offsets.Add(instruction.Offset);
		}

		private void MarkBlockStarts(IList<Instruction> instructions)
		{
			for (int i = 0; i < instructions.Count; i++)
			{
				Instruction instruction = instructions[i];
				if (i == 0)
				{
					this.MarkBlockStart(instruction);
				}
				if (ControlFlowGraphBuilder.IsBlockDelimiter(instruction))
				{
					if (ControlFlowGraphBuilder.HasMultipleBranches(instruction))
					{
						Instruction[] branchTargets = ControlFlowGraphBuilder.GetBranchTargets(instruction);
						for (int j = 0; j < branchTargets.Length; j++)
						{
							Instruction instruction2 = branchTargets[j];
							if (instruction2 != null)
							{
								this.MarkBlockStart(instruction2);
							}
						}
					}
					else
					{
						Instruction branchTarget = ControlFlowGraphBuilder.GetBranchTarget(instruction);
						if (branchTarget != null)
						{
							this.MarkBlockStart(branchTarget);
						}
					}
					if (instruction.Next != null)
					{
						this.MarkBlockStart(instruction.Next);
					}
				}
			}
		}

		private void MarkBlockEnds(IList<Instruction> instructions)
		{
			InstructionBlock[] array = this.ToArray();
			InstructionBlock instructionBlock = array[0];
			for (int i = 1; i < array.Length; i++)
			{
				InstructionBlock instructionBlock2 = array[i];
				instructionBlock.Last = instructionBlock2.First.Previous;
				instructionBlock = instructionBlock2;
			}
			instructionBlock.Last = instructions[instructions.Count - 1];
		}

		private static bool IsBlockDelimiter(Instruction instruction)
		{
			bool result;
			switch (instruction.OpCode.FlowControl)
			{
			case FlowControl.Branch:
			case FlowControl.Break:
			case FlowControl.Cond_Branch:
			case FlowControl.Return:
			case FlowControl.Throw:
				result = true;
				return result;
			}
			result = false;
			return result;
		}

		private void MarkBlockStart(Instruction instruction)
		{
			InstructionBlock instructionBlock = this.GetBlock(instruction);
			if (instructionBlock == null)
			{
				instructionBlock = new InstructionBlock(instruction);
				this.RegisterBlock(instructionBlock);
			}
		}

		private void ComputeInstructionData()
		{
			this.data = new Dictionary<int, InstructionData>();
			HashSet<InstructionBlock> visited = new HashSet<InstructionBlock>();
			foreach (InstructionBlock current in this.blocks.Values)
			{
				this.ComputeInstructionData(visited, 0, current);
			}
		}

		private void ComputeInstructionData(HashSet<InstructionBlock> visited, int stackHeight, InstructionBlock block)
		{
			if (!visited.Contains(block))
			{
				visited.Add(block);
				foreach (Instruction current in block)
				{
					stackHeight = this.ComputeInstructionData(stackHeight, current);
				}
				foreach (InstructionBlock current2 in block.Successors)
				{
					this.ComputeInstructionData(visited, stackHeight, current2);
				}
			}
		}

		private bool IsCatchStart(Instruction instruction)
		{
			return this.exception_objects_offsets != null && this.exception_objects_offsets.Contains(instruction.Offset);
		}

		private int ComputeInstructionData(int stackHeight, Instruction instruction)
		{
			if (this.IsCatchStart(instruction))
			{
				stackHeight++;
			}
			int before = stackHeight;
			int num = this.ComputeNewStackHeight(stackHeight, instruction);
			this.data.Add(instruction.Offset, new InstructionData(before, num));
			return num;
		}

		private int ComputeNewStackHeight(int stackHeight, Instruction instruction)
		{
			return stackHeight + ControlFlowGraphBuilder.GetPushDelta(instruction) - this.GetPopDelta(stackHeight, instruction);
		}

		private static int GetPushDelta(Instruction instruction)
		{
			OpCode opCode = instruction.OpCode;
			switch (opCode.StackBehaviourPush)
			{
			case StackBehaviour.Push0:
			{
				int result = 0;
				return result;
			}
			case StackBehaviour.Push1:
			case StackBehaviour.Pushi:
			case StackBehaviour.Pushi8:
			case StackBehaviour.Pushr4:
			case StackBehaviour.Pushr8:
			case StackBehaviour.Pushref:
			{
				int result = 1;
				return result;
			}
			case StackBehaviour.Push1_push1:
			{
				int result = 2;
				return result;
			}
			case StackBehaviour.Varpush:
				if (opCode.FlowControl == FlowControl.Call)
				{
					IMethodSignature methodSignature = (IMethodSignature)instruction.Operand;
					int result = (!ControlFlowGraphBuilder.IsVoid(methodSignature.ReturnType)) ? 1 : 0;
					return result;
				}
				break;
			}
			throw new ArgumentException(Formatter.FormatInstruction(instruction));
		}

		private int GetPopDelta(int stackHeight, Instruction instruction)
		{
			OpCode opCode = instruction.OpCode;
			switch (opCode.StackBehaviourPop)
			{
			case StackBehaviour.Pop0:
			{
				int result = 0;
				return result;
			}
			case StackBehaviour.Pop1:
			case StackBehaviour.Popi:
			case StackBehaviour.Popref:
			{
				int result = 1;
				return result;
			}
			case StackBehaviour.Pop1_pop1:
			case StackBehaviour.Popi_pop1:
			case StackBehaviour.Popi_popi:
			case StackBehaviour.Popi_popi8:
			case StackBehaviour.Popi_popr4:
			case StackBehaviour.Popi_popr8:
			case StackBehaviour.Popref_pop1:
			case StackBehaviour.Popref_popi:
			{
				int result = 2;
				return result;
			}
			case StackBehaviour.Popi_popi_popi:
			case StackBehaviour.Popref_popi_popi:
			case StackBehaviour.Popref_popi_popi8:
			case StackBehaviour.Popref_popi_popr4:
			case StackBehaviour.Popref_popi_popr8:
			case StackBehaviour.Popref_popi_popref:
			{
				int result = 3;
				return result;
			}
			case StackBehaviour.PopAll:
			{
				int result = stackHeight;
				return result;
			}
			case StackBehaviour.Varpop:
				if (opCode.FlowControl == FlowControl.Call)
				{
					IMethodSignature methodSignature = (IMethodSignature)instruction.Operand;
					int num = methodSignature.Parameters.Count;
					if (methodSignature.HasThis && OpCodes.Newobj.Value != opCode.Value)
					{
						num++;
					}
					int result = num;
					return result;
				}
				if (opCode.Code == Code.Ret)
				{
					int result = (!this.IsVoidMethod()) ? 1 : 0;
					return result;
				}
				break;
			}
			throw new ArgumentException(Formatter.FormatInstruction(instruction));
		}

		private bool IsVoidMethod()
		{
			return ControlFlowGraphBuilder.IsVoid(this.body.Method.ReturnType);
		}

		private static bool IsVoid(TypeReference type)
		{
			return type.MetadataType == MetadataType.Void;
		}

		private InstructionBlock[] ToArray()
		{
			InstructionBlock[] array = new InstructionBlock[this.blocks.Count];
			this.blocks.Values.CopyTo(array, 0);
			Array.Sort<InstructionBlock>(array);
			ControlFlowGraphBuilder.ComputeIndexes(array);
			return array;
		}

		private static void ComputeIndexes(InstructionBlock[] blocks)
		{
			for (int i = 0; i < blocks.Length; i++)
			{
				blocks[i].Index = i;
			}
		}

		private void ConnectBlocks()
		{
			foreach (InstructionBlock current in this.blocks.Values)
			{
				this.ConnectBlock(current);
			}
		}

		private void ConnectBlock(InstructionBlock block)
		{
			if (block.Last == null)
			{
				throw new ArgumentException("Undelimited block at offset " + block.First.Offset);
			}
			Instruction last = block.Last;
			switch (last.OpCode.FlowControl)
			{
			case FlowControl.Branch:
			case FlowControl.Cond_Branch:
			{
				if (ControlFlowGraphBuilder.HasMultipleBranches(last))
				{
					InstructionBlock[] array = this.GetBranchTargetsBlocks(last);
					InstructionBlock[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						InstructionBlock instructionBlock = array2[i];
						instructionBlock.IsBranchTarget = true;
					}
					if (last.Next != null)
					{
						array = ControlFlowGraphBuilder.AddBlock(this.GetBlock(last.Next), array);
					}
					block.AddSuccessors(array);
					return;
				}
				InstructionBlock branchTargetBlock = this.GetBranchTargetBlock(last);
				branchTargetBlock.IsBranchTarget = true;
				if (last.OpCode.FlowControl == FlowControl.Cond_Branch && last.Next != null)
				{
					block.AddSuccessors(new InstructionBlock[]
					{
						branchTargetBlock,
						this.GetBlock(last.Next)
					});
				}
				else
				{
					block.AddSuccessors(new InstructionBlock[]
					{
						branchTargetBlock
					});
				}
				return;
			}
			case FlowControl.Break:
			case FlowControl.Call:
			case FlowControl.Next:
				if (last.Next != null)
				{
					block.AddSuccessors(new InstructionBlock[]
					{
						this.GetBlock(last.Next)
					});
				}
				return;
			case FlowControl.Return:
			case FlowControl.Throw:
				return;
			}
			throw new NotSupportedException(string.Format("Unhandled instruction flow behavior {0}: {1}", last.OpCode.FlowControl, Formatter.FormatInstruction(last)));
		}

		private static InstructionBlock[] AddBlock(InstructionBlock block, InstructionBlock[] blocks)
		{
			InstructionBlock[] array = new InstructionBlock[blocks.Length + 1];
			Array.Copy(blocks, array, blocks.Length);
			array[array.Length - 1] = block;
			return array;
		}

		private static bool HasMultipleBranches(Instruction instruction)
		{
			return instruction.OpCode.Code == Code.Switch;
		}

		private InstructionBlock[] GetBranchTargetsBlocks(Instruction instruction)
		{
			Instruction[] branchTargets = ControlFlowGraphBuilder.GetBranchTargets(instruction);
			InstructionBlock[] array = new InstructionBlock[branchTargets.Length];
			for (int i = 0; i < branchTargets.Length; i++)
			{
				array[i] = this.GetBlock(branchTargets[i]);
			}
			return array;
		}

		private static Instruction[] GetBranchTargets(Instruction instruction)
		{
			return (Instruction[])instruction.Operand;
		}

		private InstructionBlock GetBranchTargetBlock(Instruction instruction)
		{
			return this.GetBlock(ControlFlowGraphBuilder.GetBranchTarget(instruction));
		}

		private static Instruction GetBranchTarget(Instruction instruction)
		{
			return (Instruction)instruction.Operand;
		}

		private void RegisterBlock(InstructionBlock block)
		{
			this.blocks.Add(block.First.Offset, block);
		}

		private InstructionBlock GetBlock(Instruction firstInstruction)
		{
			InstructionBlock result;
			this.blocks.TryGetValue(firstInstruction.Offset, out result);
			return result;
		}

		private void ComputeExceptionHandlerData()
		{
			Collection<ExceptionHandler> exceptionHandlers = this.body.ExceptionHandlers;
			if (exceptionHandlers.Count != 0)
			{
				Dictionary<int, ExceptionHandlerData> dictionary = new Dictionary<int, ExceptionHandlerData>();
				foreach (ExceptionHandler current in exceptionHandlers)
				{
					this.ComputeExceptionHandlerData(dictionary, current);
					this.ComputeExceptionSuccessor(current);
				}
				this.exception_data = new List<ExceptionHandlerData>(dictionary.Values);
				this.exception_data.Sort();
			}
		}

		private void ComputeExceptionHandlerData(Dictionary<int, ExceptionHandlerData> datas, ExceptionHandler handler)
		{
			ExceptionHandlerData value;
			if (!datas.TryGetValue(handler.TryStart.Offset, out value))
			{
				value = new ExceptionHandlerData(this.ComputeRange(handler.TryStart, handler.TryEnd));
				datas.Add(handler.TryStart.Offset, value);
			}
			this.ComputeExceptionHandlerData(value, handler);
		}

		private void ComputeExceptionHandlerData(ExceptionHandlerData data, ExceptionHandler handler)
		{
			BlockRange blockRange = this.ComputeRange(handler.HandlerStart, handler.HandlerEnd);
			switch (handler.HandlerType)
			{
			case ExceptionHandlerType.Catch:
				data.Catches.Add(new CatchHandlerData(handler.CatchType, blockRange));
				break;
			case ExceptionHandlerType.Filter:
				throw new NotImplementedException();
			case ExceptionHandlerType.Finally:
				data.FinallyRange = blockRange;
				break;
			case ExceptionHandlerType.Fault:
				data.FaultRange = blockRange;
				break;
			}
		}

		private BlockRange ComputeRange(Instruction start, Instruction end)
		{
			return new BlockRange(this.blocks[start.Offset], this.blocks[end.Offset]);
		}

		private void ComputeExceptionSuccessor(ExceptionHandler handler)
		{
			InstructionBlock block = this.GetBlock(handler.HandlerStart);
			InstructionBlock block2 = this.GetBlock(handler.TryStart);
			block2.AddExceptionSuccessor(block);
		}
	}
}
