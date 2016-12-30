namespace Unity.IL2CPP.Debugger
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Rocks;
    using System;
    using System.Collections.Generic;

    internal class CollectSourceDocumentsVisitor
    {
        private readonly HashSet<string> _documents;

        public CollectSourceDocumentsVisitor()
        {
            this._documents = new HashSet<string>();
        }

        public CollectSourceDocumentsVisitor(HashSet<string> documents)
        {
            this._documents = documents;
        }

        public void Process(AssemblyDefinition assembly)
        {
            foreach (TypeDefinition definition in assembly.MainModule.GetAllTypes())
            {
                this.Process(definition);
            }
        }

        public void Process(MethodDefinition method)
        {
            if (method.HasBody)
            {
                foreach (Instruction instruction in method.Body.Instructions)
                {
                    this.Visit(instruction);
                }
            }
        }

        public void Process(TypeDefinition type)
        {
            foreach (MethodDefinition definition in type.Methods)
            {
                this.Process(definition);
            }
        }

        private void Visit(Instruction instruction)
        {
            SequencePoint sequencePoint = instruction.SequencePoint;
            if ((sequencePoint != null) && (sequencePoint.Document != null))
            {
                this.Documents.Add(sequencePoint.Document.Url);
            }
        }

        public HashSet<string> Documents =>
            this._documents;
    }
}

