namespace Unity.IL2CPP.Common.CFG
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Collections.Generic;
    using System;
    using System.Collections.Generic;

    internal class ControlFlowGraphBuilder
    {
        private Dictionary<int, InstructionBlock> blocks = new Dictionary<int, InstructionBlock>();
        private MethodBody body;
        private Dictionary<int, InstructionData> data;
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

        private static InstructionBlock[] AddBlock(InstructionBlock block, InstructionBlock[] blocks)
        {
            InstructionBlock[] destinationArray = new InstructionBlock[blocks.Length + 1];
            Array.Copy(blocks, destinationArray, blocks.Length);
            destinationArray[destinationArray.Length - 1] = block;
            return destinationArray;
        }

        private void ComputeExceptionHandlerData()
        {
            Collection<ExceptionHandler> exceptionHandlers = this.body.ExceptionHandlers;
            if (exceptionHandlers.Count != 0)
            {
                Dictionary<int, ExceptionHandlerData> datas = new Dictionary<int, ExceptionHandlerData>();
                foreach (ExceptionHandler handler in exceptionHandlers)
                {
                    this.ComputeExceptionHandlerData(datas, handler);
                    this.ComputeExceptionSuccessor(handler);
                }
                this.exception_data = new List<ExceptionHandlerData>(datas.Values);
                this.exception_data.Sort();
            }
        }

        private void ComputeExceptionHandlerData(Dictionary<int, ExceptionHandlerData> datas, ExceptionHandler handler)
        {
            ExceptionHandlerData data;
            if (!datas.TryGetValue(handler.TryStart.Offset, out data))
            {
                data = new ExceptionHandlerData(this.ComputeRange(handler.TryStart, handler.TryEnd));
                datas.Add(handler.TryStart.Offset, data);
            }
            this.ComputeExceptionHandlerData(data, handler);
        }

        private void ComputeExceptionHandlerData(ExceptionHandlerData data, ExceptionHandler handler)
        {
            BlockRange range = this.ComputeRange(handler.HandlerStart, handler.HandlerEnd);
            switch (handler.HandlerType)
            {
                case ExceptionHandlerType.Catch:
                    data.Catches.Add(new CatchHandlerData(handler.CatchType, range));
                    break;

                case ExceptionHandlerType.Filter:
                    throw new NotImplementedException();

                case ExceptionHandlerType.Finally:
                    data.FinallyRange = range;
                    break;

                case ExceptionHandlerType.Fault:
                    data.FaultRange = range;
                    break;
            }
        }

        private void ComputeExceptionSuccessor(ExceptionHandler handler)
        {
            InstructionBlock block = this.GetBlock(handler.HandlerStart);
            this.GetBlock(handler.TryStart).AddExceptionSuccessor(block);
        }

        private static void ComputeIndexes(InstructionBlock[] blocks)
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i].Index = i;
            }
        }

        private void ComputeInstructionData()
        {
            this.data = new Dictionary<int, InstructionData>();
            HashSet<InstructionBlock> visited = new HashSet<InstructionBlock>();
            foreach (InstructionBlock block in this.blocks.Values)
            {
                this.ComputeInstructionData(visited, 0, block);
            }
        }

        private int ComputeInstructionData(int stackHeight, Instruction instruction)
        {
            if (this.IsCatchStart(instruction))
            {
                stackHeight++;
            }
            int before = stackHeight;
            int after = this.ComputeNewStackHeight(stackHeight, instruction);
            this.data.Add(instruction.Offset, new InstructionData(before, after));
            return after;
        }

        private void ComputeInstructionData(HashSet<InstructionBlock> visited, int stackHeight, InstructionBlock block)
        {
            if (!visited.Contains(block))
            {
                visited.Add(block);
                foreach (Instruction instruction in block)
                {
                    stackHeight = this.ComputeInstructionData(stackHeight, instruction);
                }
                foreach (InstructionBlock block2 in block.Successors)
                {
                    this.ComputeInstructionData(visited, stackHeight, block2);
                }
            }
        }

        private int ComputeNewStackHeight(int stackHeight, Instruction instruction) => 
            ((stackHeight + GetPushDelta(instruction)) - this.GetPopDelta(stackHeight, instruction));

        private BlockRange ComputeRange(Instruction start, Instruction end) => 
            new BlockRange(this.blocks[start.Offset], this.blocks[end.Offset]);

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
                    if (!HasMultipleBranches(last))
                    {
                        InstructionBlock branchTargetBlock = this.GetBranchTargetBlock(last);
                        branchTargetBlock.IsBranchTarget = true;
                        if ((last.OpCode.FlowControl == FlowControl.Cond_Branch) && (last.Next != null))
                        {
                            InstructionBlock[] blocks = new InstructionBlock[] { branchTargetBlock, this.GetBlock(last.Next) };
                            block.AddSuccessors(blocks);
                        }
                        else
                        {
                            InstructionBlock[] blockArray3 = new InstructionBlock[] { branchTargetBlock };
                            block.AddSuccessors(blockArray3);
                        }
                        return;
                    }
                    InstructionBlock[] branchTargetsBlocks = this.GetBranchTargetsBlocks(last);
                    foreach (InstructionBlock block2 in branchTargetsBlocks)
                    {
                        block2.IsBranchTarget = true;
                    }
                    if (last.Next != null)
                    {
                        branchTargetsBlocks = AddBlock(this.GetBlock(last.Next), branchTargetsBlocks);
                    }
                    block.AddSuccessors(branchTargetsBlocks);
                    return;
                }
                case FlowControl.Break:
                case FlowControl.Call:
                case FlowControl.Next:
                    if (last.Next != null)
                    {
                        InstructionBlock[] blockArray4 = new InstructionBlock[] { this.GetBlock(last.Next) };
                        block.AddSuccessors(blockArray4);
                    }
                    return;

                case FlowControl.Return:
                case FlowControl.Throw:
                    return;
            }
            throw new NotSupportedException($"Unhandled instruction flow behavior {last.OpCode.FlowControl}: {Formatter.FormatInstruction(last)}");
        }

        private void ConnectBlocks()
        {
            foreach (InstructionBlock block in this.blocks.Values)
            {
                this.ConnectBlock(block);
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

        private InstructionBlock GetBlock(Instruction firstInstruction)
        {
            InstructionBlock block;
            this.blocks.TryGetValue(firstInstruction.Offset, out block);
            return block;
        }

        private static Instruction GetBranchTarget(Instruction instruction) => 
            ((Instruction) instruction.Operand);

        private InstructionBlock GetBranchTargetBlock(Instruction instruction) => 
            this.GetBlock(GetBranchTarget(instruction));

        private static Instruction[] GetBranchTargets(Instruction instruction) => 
            ((Instruction[]) instruction.Operand);

        private InstructionBlock[] GetBranchTargetsBlocks(Instruction instruction)
        {
            Instruction[] branchTargets = GetBranchTargets(instruction);
            InstructionBlock[] blockArray = new InstructionBlock[branchTargets.Length];
            for (int i = 0; i < branchTargets.Length; i++)
            {
                blockArray[i] = this.GetBlock(branchTargets[i]);
            }
            return blockArray;
        }

        private int GetPopDelta(int stackHeight, Instruction instruction)
        {
            OpCode opCode = instruction.OpCode;
            switch (opCode.StackBehaviourPop)
            {
                case StackBehaviour.Pop0:
                    return 0;

                case StackBehaviour.Pop1:
                case StackBehaviour.Popi:
                case StackBehaviour.Popref:
                    return 1;

                case StackBehaviour.Pop1_pop1:
                case StackBehaviour.Popi_pop1:
                case StackBehaviour.Popi_popi:
                case StackBehaviour.Popi_popi8:
                case StackBehaviour.Popi_popr4:
                case StackBehaviour.Popi_popr8:
                case StackBehaviour.Popref_pop1:
                case StackBehaviour.Popref_popi:
                    return 2;

                case StackBehaviour.Popi_popi_popi:
                case StackBehaviour.Popref_popi_popi:
                case StackBehaviour.Popref_popi_popi8:
                case StackBehaviour.Popref_popi_popr4:
                case StackBehaviour.Popref_popi_popr8:
                case StackBehaviour.Popref_popi_popref:
                    return 3;

                case StackBehaviour.PopAll:
                    return stackHeight;

                case StackBehaviour.Varpop:
                {
                    if (opCode.FlowControl != FlowControl.Call)
                    {
                        if (opCode.Code == Code.Ret)
                        {
                            return (!this.IsVoidMethod() ? 1 : 0);
                        }
                        break;
                    }
                    IMethodSignature operand = (IMethodSignature) instruction.Operand;
                    int count = operand.Parameters.Count;
                    if (operand.HasThis && (OpCodes.Newobj.Value != opCode.Value))
                    {
                        count++;
                    }
                    return count;
                }
            }
            throw new ArgumentException(Formatter.FormatInstruction(instruction));
        }

        private static int GetPushDelta(Instruction instruction)
        {
            OpCode opCode = instruction.OpCode;
            switch (opCode.StackBehaviourPush)
            {
                case StackBehaviour.Push0:
                    return 0;

                case StackBehaviour.Push1:
                case StackBehaviour.Pushi:
                case StackBehaviour.Pushi8:
                case StackBehaviour.Pushr4:
                case StackBehaviour.Pushr8:
                case StackBehaviour.Pushref:
                    return 1;

                case StackBehaviour.Push1_push1:
                    return 2;

                case StackBehaviour.Varpush:
                {
                    if (opCode.FlowControl != FlowControl.Call)
                    {
                        break;
                    }
                    IMethodSignature operand = (IMethodSignature) instruction.Operand;
                    return (!IsVoid(operand.ReturnType) ? 1 : 0);
                }
            }
            throw new ArgumentException(Formatter.FormatInstruction(instruction));
        }

        private static bool HasMultipleBranches(Instruction instruction) => 
            (instruction.OpCode.Code == Code.Switch);

        private static bool IsBlockDelimiter(Instruction instruction)
        {
            switch (instruction.OpCode.FlowControl)
            {
                case FlowControl.Branch:
                case FlowControl.Break:
                case FlowControl.Cond_Branch:
                case FlowControl.Return:
                case FlowControl.Throw:
                    return true;
            }
            return false;
        }

        private bool IsCatchStart(Instruction instruction) => 
            this.exception_objects_offsets?.Contains(instruction.Offset);

        private static bool IsVoid(TypeReference type) => 
            (type.MetadataType == MetadataType.Void);

        private bool IsVoidMethod() => 
            IsVoid(this.body.Method.ReturnType);

        private void MarkBlockEnds(IList<Instruction> instructions)
        {
            InstructionBlock[] blockArray = this.ToArray();
            InstructionBlock block = blockArray[0];
            for (int i = 1; i < blockArray.Length; i++)
            {
                InstructionBlock block2 = blockArray[i];
                block.Last = block2.First.Previous;
                block = block2;
            }
            block.Last = instructions[instructions.Count - 1];
        }

        private void MarkBlockStart(Instruction instruction)
        {
            if (this.GetBlock(instruction) == null)
            {
                InstructionBlock block = new InstructionBlock(instruction);
                this.RegisterBlock(block);
            }
        }

        private void MarkBlockStarts(IList<ExceptionHandler> handlers)
        {
            for (int i = 0; i < handlers.Count; i++)
            {
                ExceptionHandler handler = handlers[i];
                this.MarkBlockStart(handler.TryStart);
                this.MarkBlockStart(handler.HandlerStart);
                if (handler.HandlerType == ExceptionHandlerType.Filter)
                {
                    this.MarkExceptionObjectPosition(handler.FilterStart);
                    this.MarkBlockStart(handler.FilterStart);
                }
                else if (handler.HandlerType == ExceptionHandlerType.Catch)
                {
                    this.MarkExceptionObjectPosition(handler.HandlerStart);
                    this.MarkBlockStart(handler.HandlerEnd);
                }
            }
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
                if (IsBlockDelimiter(instruction))
                {
                    if (HasMultipleBranches(instruction))
                    {
                        foreach (Instruction instruction2 in GetBranchTargets(instruction))
                        {
                            if (instruction2 != null)
                            {
                                this.MarkBlockStart(instruction2);
                            }
                        }
                    }
                    else
                    {
                        Instruction branchTarget = GetBranchTarget(instruction);
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

        private void MarkExceptionObjectPosition(Instruction instruction)
        {
            this.exception_objects_offsets.Add(instruction.Offset);
        }

        private void RegisterBlock(InstructionBlock block)
        {
            this.blocks.Add(block.First.Offset, block);
        }

        private InstructionBlock[] ToArray()
        {
            InstructionBlock[] array = new InstructionBlock[this.blocks.Count];
            this.blocks.Values.CopyTo(array, 0);
            Array.Sort<InstructionBlock>(array);
            ComputeIndexes(array);
            return array;
        }
    }
}

