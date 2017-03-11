namespace Unity.IL2CPP.Debugger
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using Unity.IL2CPP;

    internal class SequencePointsMappingVisitor
    {
        private readonly Action<Instruction, SequencePoint> _callback;

        public SequencePointsMappingVisitor(Action<Instruction, SequencePoint> callback)
        {
            this._callback = callback;
        }

        public void Process(MethodDefinition method)
        {
            foreach (Instruction instruction in method.Body.Instructions)
            {
                SequencePoint sequencePoint = instruction.GetSequencePoint(method);
                if (sequencePoint != null)
                {
                    this._callback(instruction, sequencePoint);
                }
            }
        }
    }
}

