using Mono.Cecil.Cil;
using System;

namespace Unity.IL2CPP.Debugger
{
	internal class SequencePointInfo
	{
		public Instruction Instruction;

		public SequencePoint SequencePoint;

		public SequencePointInfo NextSequencePoint;
	}
}
