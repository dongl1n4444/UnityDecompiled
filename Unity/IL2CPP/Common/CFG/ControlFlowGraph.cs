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
        private List<ExceptionHandlerData> exception_data;
        private HashSet<int> exception_objects_offsets;

        public ControlFlowGraph(Mono.Cecil.Cil.MethodBody body, InstructionBlock[] blocks, Dictionary<int, InstructionData> instructionData, List<ExceptionHandlerData> exception_data, HashSet<int> exception_objects_offsets)
        {
            this.body = body;
            this.blocks = blocks;
            this.data = instructionData;
            this.exception_data = exception_data;
            this.exception_objects_offsets = exception_objects_offsets;
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

        public ExceptionHandlerData[] GetExceptionData() => 
            this.exception_data.ToArray();

        public bool HasExceptionObject(int offset) => 
            this.exception_objects_offsets?.Contains(offset);

        public InstructionBlock[] Blocks =>
            this.blocks;

        public Mono.Cecil.Cil.MethodBody MethodBody =>
            this.body;
    }
}

