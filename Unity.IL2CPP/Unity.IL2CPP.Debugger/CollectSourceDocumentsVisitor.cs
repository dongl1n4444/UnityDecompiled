using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Unity.Cecil.Visitor;

namespace Unity.IL2CPP.Debugger
{
	internal class CollectSourceDocumentsVisitor : Visitor
	{
		private readonly HashSet<string> _documents;

		public HashSet<string> Documents
		{
			get
			{
				return this._documents;
			}
		}

		public CollectSourceDocumentsVisitor()
		{
			this._documents = new HashSet<string>();
		}

		public CollectSourceDocumentsVisitor(HashSet<string> documents)
		{
			this._documents = documents;
		}

		protected override void Visit(Instruction instruction, Context context)
		{
			base.Visit(instruction, context);
			SequencePoint sequencePoint = instruction.SequencePoint;
			if (sequencePoint != null)
			{
				if (sequencePoint.Document != null)
				{
					this.Documents.Add(sequencePoint.Document.Url);
				}
			}
		}
	}
}
