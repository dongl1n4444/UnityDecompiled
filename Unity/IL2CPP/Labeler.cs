namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Collections.Generic;

    internal class Labeler
    {
        private readonly Dictionary<Instruction, List<Instruction>> _jumpMap = new Dictionary<Instruction, List<Instruction>>();
        private readonly MethodDefinition _methodDefinition;

        public Labeler(MethodDefinition methodDefinition)
        {
            this._methodDefinition = methodDefinition;
            this.BuildLabelMap(methodDefinition);
        }

        private void AddJumpLabel(Instruction ins, Instruction targetInstruction)
        {
            List<Instruction> list;
            if (!this._jumpMap.TryGetValue(targetInstruction, out list))
            {
                this._jumpMap.Add(targetInstruction, list = new List<Instruction>());
            }
            list.Add(ins);
        }

        private void BuildLabelMap(MethodDefinition methodDefinition)
        {
            foreach (Instruction instruction in methodDefinition.Body.Instructions)
            {
                Instruction operand = instruction.Operand as Instruction;
                if (operand != null)
                {
                    this.AddJumpLabel(instruction, operand);
                }
                else
                {
                    Instruction[] instructionArray = instruction.Operand as Instruction[];
                    if (instructionArray != null)
                    {
                        foreach (Instruction instruction3 in instructionArray)
                        {
                            this.AddJumpLabel(instruction, instruction3);
                        }
                    }
                }
            }
            foreach (ExceptionHandler handler in methodDefinition.Body.ExceptionHandlers)
            {
                this.AddJumpLabel(null, handler.HandlerStart);
            }
        }

        public string ForJump(Instruction targetInstruction) => 
            $"goto {this.FormatOffset(targetInstruction)};";

        public string ForJump(int offset) => 
            $"goto {this.FormatOffset(offset)};";

        public string ForLabel(Instruction ins) => 
            (this.FormatOffset(ins) + ":");

        public string FormatOffset(Instruction ins) => 
            this.FormatOffset(ins.Offset);

        private string FormatOffset(int offset)
        {
            string str = "IL";
            foreach (ExceptionHandler handler in this._methodDefinition.Body.ExceptionHandlers)
            {
                if (handler.HandlerStart.Offset == offset)
                {
                    switch (handler.HandlerType)
                    {
                        case ExceptionHandlerType.Catch:
                            str = "CATCH";
                            break;

                        case ExceptionHandlerType.Filter:
                            str = "FILTER";
                            break;

                        case ExceptionHandlerType.Finally:
                            str = "FINALLY";
                            break;

                        case ExceptionHandlerType.Fault:
                            str = "FAULT";
                            break;
                    }
                }
            }
            return $"{str}_{offset:x4}";
        }

        public bool NeedsLabel(Instruction ins) => 
            this._jumpMap.ContainsKey(ins);
    }
}

