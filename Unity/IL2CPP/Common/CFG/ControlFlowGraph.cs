namespace Unity.IL2CPP.Common.CFG
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Collections.Generic;

    public class ControlFlowGraph
    {
        private InstructionBlock[] blocks;
        private Mono.Cecil.Cil.MethodBody body;
        private Dictionary<int, InstructionData> data;

        public ControlFlowGraph(Mono.Cecil.Cil.MethodBody body, InstructionBlock[] blocks, Dictionary<int, InstructionData> instructionData)
        {
            this.body = body;
            this.blocks = blocks;
            this.data = instructionData;
        }

        public static ControlFlowGraph Create(MethodDefinition method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            if (!method.HasBody)
            {
                throw new ArgumentException();
            }
            ControlFlowGraphBuilder builder = new ControlFlowGraphBuilder(method);
            return builder.CreateGraph();
        }

        public InstructionData GetData(Instruction instruction) => 
            this.data[instruction.Offset];

        public InstructionBlock[] Blocks =>
            this.blocks;

        public Mono.Cecil.Cil.MethodBody MethodBody =>
            this.body;
    }
}

