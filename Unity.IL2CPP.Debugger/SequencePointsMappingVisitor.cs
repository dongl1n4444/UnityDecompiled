using Mono.Cecil.Cil;
using System;
using Unity.Cecil.Visitor;

namespace Unity.IL2CPP.Debugger
{
	internal class SequencePointsMappingVisitor : Visitor
	{
		private readonly Action<Instruction, SequencePoint> _callback;

		public SequencePointsMappingVisitor(Action<Instruction, SequencePoint> callback)
		{
			this._callback = callback;
		}

		protected override void Visit(Instruction instruction, Context context)
		{
			base.Visit(instruction, context);
			if (instruction.SequencePoint != null)
			{
				this._callback(instruction, instruction.SequencePoint);
			}
		}
	}
}
