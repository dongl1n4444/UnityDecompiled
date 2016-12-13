using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Unity.IL2CPP.Common.CFG
{
	public class InstructionBlock : IEnumerable<Instruction>, IComparable<InstructionBlock>, IEnumerable
	{
		private int index;

		private Instruction first;

		private Instruction last;

		private List<InstructionBlock> _successors = new List<InstructionBlock>();

		private List<InstructionBlock> _exceptionSuccessors = new List<InstructionBlock>();

		public int Index
		{
			get
			{
				return this.index;
			}
			internal set
			{
				this.index = value;
			}
		}

		public Instruction First
		{
			get
			{
				return this.first;
			}
			internal set
			{
				this.first = value;
			}
		}

		public Instruction Last
		{
			get
			{
				return this.last;
			}
			internal set
			{
				this.last = value;
			}
		}

		public IEnumerable<InstructionBlock> Successors
		{
			get
			{
				return this._successors;
			}
		}

		public IEnumerable<InstructionBlock> ExceptionSuccessors
		{
			get
			{
				return this._exceptionSuccessors;
			}
		}

		public bool IsBranchTarget
		{
			get;
			set;
		}

		public bool IsDead
		{
			get;
			private set;
		}

		internal InstructionBlock(Instruction first)
		{
			if (first == null)
			{
				throw new ArgumentNullException("first");
			}
			this.first = first;
		}

		public int CompareTo(InstructionBlock block)
		{
			return this.first.Offset - block.First.Offset;
		}

		[DebuggerHidden]
		public IEnumerator<Instruction> GetEnumerator()
		{
			InstructionBlock.<GetEnumerator>c__Iterator0 <GetEnumerator>c__Iterator = new InstructionBlock.<GetEnumerator>c__Iterator0();
			<GetEnumerator>c__Iterator.$this = this;
			return <GetEnumerator>c__Iterator;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void AddSuccessors(IEnumerable<InstructionBlock> blocks)
		{
			this._successors.AddRange(blocks);
		}

		public void AddExceptionSuccessor(InstructionBlock block)
		{
			this._exceptionSuccessors.Add(block);
		}

		public void MarkIsDead()
		{
			this.IsDead = true;
		}

		public void MarkIsAliveRecursive()
		{
			if (this.IsDead)
			{
				this.IsDead = false;
				foreach (InstructionBlock current in this.Successors)
				{
					current.MarkIsAliveRecursive();
				}
				foreach (InstructionBlock current2 in this.ExceptionSuccessors)
				{
					current2.MarkIsAliveRecursive();
				}
			}
		}
	}
}
