namespace Unity.IL2CPP.Debugger
{
    using Mono.Cecil.Cil;
    using System;

    internal class SequencePointInfo
    {
        public Mono.Cecil.Cil.Instruction Instruction;
        public SequencePointInfo NextSequencePoint;
        public Mono.Cecil.Cil.SequencePoint SequencePoint;
    }
}

