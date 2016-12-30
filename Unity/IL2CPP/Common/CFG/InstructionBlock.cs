namespace Unity.IL2CPP.Common.CFG
{
    using Mono.Cecil.Cil;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class InstructionBlock : IEnumerable<Instruction>, IComparable<InstructionBlock>, IEnumerable
    {
        private List<InstructionBlock> _exceptionSuccessors = new List<InstructionBlock>();
        private List<InstructionBlock> _successors = new List<InstructionBlock>();
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <IsBranchTarget>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <IsDead>k__BackingField;
        private Instruction first;
        private int index;
        private Instruction last;

        internal InstructionBlock(Instruction first)
        {
            if (first == null)
            {
                throw new ArgumentNullException("first");
            }
            this.first = first;
        }

        public void AddExceptionSuccessor(InstructionBlock block)
        {
            this._exceptionSuccessors.Add(block);
        }

        public void AddSuccessors(IEnumerable<InstructionBlock> blocks)
        {
            this._successors.AddRange(blocks);
        }

        public int CompareTo(InstructionBlock block) => 
            (this.first.Offset - block.First.Offset);

        [DebuggerHidden]
        public IEnumerator<Instruction> GetEnumerator() => 
            new <GetEnumerator>c__Iterator0 { $this = this };

        public void MarkIsAliveRecursive()
        {
            if (this.IsDead)
            {
                this.IsDead = false;
                foreach (InstructionBlock block in this.Successors)
                {
                    block.MarkIsAliveRecursive();
                }
                foreach (InstructionBlock block2 in this.ExceptionSuccessors)
                {
                    block2.MarkIsAliveRecursive();
                }
            }
        }

        public void MarkIsDead()
        {
            this.IsDead = true;
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public IEnumerable<InstructionBlock> ExceptionSuccessors =>
            this._exceptionSuccessors;

        public Instruction First
        {
            get => 
                this.first;
            internal set
            {
                this.first = value;
            }
        }

        public int Index
        {
            get => 
                this.index;
            internal set
            {
                this.index = value;
            }
        }

        public bool IsBranchTarget { get; set; }

        public bool IsDead { get; private set; }

        public Instruction Last
        {
            get => 
                this.last;
            internal set
            {
                this.last = value;
            }
        }

        public IEnumerable<InstructionBlock> Successors =>
            this._successors;

        [CompilerGenerated]
        private sealed class <GetEnumerator>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<Instruction>
        {
            internal Instruction $current;
            internal bool $disposing;
            internal int $PC;
            internal InstructionBlock $this;
            internal Instruction <instruction>__1;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<instruction>__1 = this.$this.first;
                        break;

                    case 1:
                        if (this.<instruction>__1 != this.$this.last)
                        {
                            this.<instruction>__1 = this.<instruction>__1.Next;
                            break;
                        }
                        goto Label_008E;

                    default:
                        goto Label_008E;
                }
                this.$current = this.<instruction>__1;
                if (!this.$disposing)
                {
                    this.$PC = 1;
                }
                return true;
                this.$PC = -1;
            Label_008E:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            Instruction IEnumerator<Instruction>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

