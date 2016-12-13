using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Common.CFG
{
	public class ControlFlowGraph
	{
		private MethodBody body;

		private InstructionBlock[] blocks;

		private Dictionary<int, InstructionData> data;

		private List<ExceptionHandlerData> exception_data;

		private HashSet<int> exception_objects_offsets;

		public MethodBody MethodBody
		{
			get
			{
				return this.body;
			}
		}

		public InstructionBlock[] Blocks
		{
			get
			{
				return this.blocks;
			}
		}

		public ControlFlowGraph(MethodBody body, InstructionBlock[] blocks, Dictionary<int, InstructionData> instructionData, List<ExceptionHandlerData> exception_data, HashSet<int> exception_objects_offsets)
		{
			this.body = body;
			this.blocks = blocks;
			this.data = instructionData;
			this.exception_data = exception_data;
			this.exception_objects_offsets = exception_objects_offsets;
		}

		public InstructionData GetData(Instruction instruction)
		{
			return this.data[instruction.Offset];
		}

		public ExceptionHandlerData[] GetExceptionData()
		{
			return this.exception_data.ToArray();
		}

		public bool HasExceptionObject(int offset)
		{
			return this.exception_objects_offsets != null && this.exception_objects_offsets.Contains(offset);
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
			ControlFlowGraphBuilder controlFlowGraphBuilder = new ControlFlowGraphBuilder(method);
			return controlFlowGraphBuilder.CreateGraph();
		}
	}
}
