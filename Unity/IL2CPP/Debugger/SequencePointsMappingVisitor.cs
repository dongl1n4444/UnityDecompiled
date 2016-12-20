namespace Unity.IL2CPP.Debugger
{
    using Mono.Cecil.Cil;
    using System;
    using Unity.Cecil.Visitor;

    internal class SequencePointsMappingVisitor : Unity.Cecil.Visitor.Visitor
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
                this._callback.Invoke(instruction, instruction.SequencePoint);
            }
        }
    }
}

