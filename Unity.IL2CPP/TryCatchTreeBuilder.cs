using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IL2CPP.Common.CFG;

namespace Unity.IL2CPP
{
	internal class TryCatchTreeBuilder
	{
		internal enum ContextType
		{
			Root,
			Block,
			Try,
			Catch,
			Finally,
			Fault
		}

		internal class Context
		{
			public TryCatchTreeBuilder.ContextType Type;

			public InstructionBlock Block;

			public ExceptionHandler Handler;

			public List<TryCatchTreeBuilder.Context> Children = new List<TryCatchTreeBuilder.Context>();
		}

		private readonly MethodBody _methodBody;

		private readonly InstructionBlock[] _blocks;

		private readonly Stack<TryCatchTreeBuilder.Context> _contextStack = new Stack<TryCatchTreeBuilder.Context>();

		private readonly Dictionary<Instruction, TryCatchInfo> _tryCatchInfos;

		public TryCatchTreeBuilder(MethodBody methodBody, InstructionBlock[] blocks, Dictionary<Instruction, TryCatchInfo> tryCatchInfos)
		{
			this._methodBody = methodBody;
			this._blocks = blocks;
			this._tryCatchInfos = tryCatchInfos;
		}

		internal ExceptionSupport.Node Build()
		{
			return (!this._methodBody.HasExceptionHandlers) ? this.BuildTreeWithNoExceptionHandlers() : this.BuildTreeWithExceptionHandlers();
		}

		private ExceptionSupport.Node BuildTreeWithNoExceptionHandlers()
		{
			int num = 0;
			ExceptionSupport.Node[] array = new ExceptionSupport.Node[this._blocks.Length];
			InstructionBlock[] blocks = this._blocks;
			for (int i = 0; i < blocks.Length; i++)
			{
				InstructionBlock block = blocks[i];
				array[num++] = new ExceptionSupport.Node(ExceptionSupport.NodeType.Block, block);
			}
			return TryCatchTreeBuilder.MakeRoot(array);
		}

		private ExceptionSupport.Node BuildTreeWithExceptionHandlers()
		{
			this._contextStack.Push(new TryCatchTreeBuilder.Context
			{
				Type = TryCatchTreeBuilder.ContextType.Root
			});
			InstructionBlock[] blocks = this._blocks;
			for (int i = 0; i < blocks.Length; i++)
			{
				InstructionBlock instructionBlock = blocks[i];
				if (instructionBlock.Last.Next == null)
				{
					this._contextStack.Peek().Children.Add(new TryCatchTreeBuilder.Context
					{
						Type = TryCatchTreeBuilder.ContextType.Block,
						Block = instructionBlock
					});
					break;
				}
				Instruction firstInstr = instructionBlock.First;
				Instruction next = instructionBlock.Last.Next;
				TryCatchInfo tryCatchInfo = this._tryCatchInfos[firstInstr];
				TryCatchInfo tryCatchInfo2 = this._tryCatchInfos[next];
				if (tryCatchInfo.CatchStart != 0 && tryCatchInfo.FinallyStart != 0)
				{
					throw new NotSupportedException("An instruction cannot start both a catch and a finally block!");
				}
				for (int j = 0; j < tryCatchInfo.FinallyStart; j++)
				{
					this._contextStack.Push(new TryCatchTreeBuilder.Context
					{
						Type = TryCatchTreeBuilder.ContextType.Finally,
						Handler = this._methodBody.ExceptionHandlers.Single((ExceptionHandler h) => h.HandlerType == ExceptionHandlerType.Finally && h.HandlerStart == firstInstr)
					});
				}
				for (int k = 0; k < tryCatchInfo.FaultStart; k++)
				{
					this._contextStack.Push(new TryCatchTreeBuilder.Context
					{
						Type = TryCatchTreeBuilder.ContextType.Fault,
						Handler = this._methodBody.ExceptionHandlers.Single((ExceptionHandler h) => h.HandlerType == ExceptionHandlerType.Fault && h.HandlerStart == firstInstr)
					});
				}
				for (int l = 0; l < tryCatchInfo.CatchStart; l++)
				{
					this._contextStack.Push(new TryCatchTreeBuilder.Context
					{
						Type = TryCatchTreeBuilder.ContextType.Catch,
						Handler = this._methodBody.ExceptionHandlers.Single((ExceptionHandler h) => h.HandlerType == ExceptionHandlerType.Catch && h.HandlerStart == firstInstr)
					});
				}
				for (int m = 0; m < tryCatchInfo.TryStart; m++)
				{
					this._contextStack.Push(new TryCatchTreeBuilder.Context
					{
						Type = TryCatchTreeBuilder.ContextType.Try
					});
				}
				this._contextStack.Peek().Children.Add(new TryCatchTreeBuilder.Context
				{
					Type = TryCatchTreeBuilder.ContextType.Block,
					Block = instructionBlock
				});
				for (int n = 0; n < tryCatchInfo2.FinallyEnd; n++)
				{
					TryCatchTreeBuilder.Context context = this._contextStack.Pop();
					this._contextStack.Peek().Children.Add(new TryCatchTreeBuilder.Context
					{
						Type = TryCatchTreeBuilder.ContextType.Finally,
						Children = context.Children,
						Handler = context.Handler
					});
				}
				for (int num = 0; num < tryCatchInfo2.FaultEnd; num++)
				{
					TryCatchTreeBuilder.Context context2 = this._contextStack.Pop();
					this._contextStack.Peek().Children.Add(new TryCatchTreeBuilder.Context
					{
						Type = TryCatchTreeBuilder.ContextType.Fault,
						Children = context2.Children,
						Handler = context2.Handler
					});
				}
				for (int num2 = 0; num2 < tryCatchInfo2.CatchEnd; num2++)
				{
					TryCatchTreeBuilder.Context context3 = this._contextStack.Pop();
					this._contextStack.Peek().Children.Add(new TryCatchTreeBuilder.Context
					{
						Type = TryCatchTreeBuilder.ContextType.Catch,
						Children = context3.Children,
						Handler = context3.Handler
					});
				}
				for (int num3 = 0; num3 < tryCatchInfo2.TryEnd; num3++)
				{
					TryCatchTreeBuilder.Context context4 = this._contextStack.Pop();
					this._contextStack.Peek().Children.Add(new TryCatchTreeBuilder.Context
					{
						Type = TryCatchTreeBuilder.ContextType.Try,
						Children = context4.Children
					});
				}
			}
			if (this._contextStack.Count > 1)
			{
				throw new NotSupportedException("Mismatched context depth when building try/catch tree!");
			}
			return TryCatchTreeBuilder.MergeAndBuildRootNode(this._contextStack.Pop());
		}

		private static ExceptionSupport.Node MergeAndBuildRootNode(TryCatchTreeBuilder.Context context)
		{
			int num = 0;
			ExceptionSupport.Node[] array = new ExceptionSupport.Node[context.Children.Count];
			foreach (TryCatchTreeBuilder.Context current in context.Children)
			{
				array[num++] = TryCatchTreeBuilder.MergeAndBuildRootNodeRecursive(current);
			}
			return TryCatchTreeBuilder.MakeRoot(array);
		}

		private static ExceptionSupport.Node MergeAndBuildRootNodeRecursive(TryCatchTreeBuilder.Context context)
		{
			int num = 0;
			ExceptionSupport.Node[] array = new ExceptionSupport.Node[context.Children.Count];
			foreach (TryCatchTreeBuilder.Context current in context.Children)
			{
				array[num++] = TryCatchTreeBuilder.MergeAndBuildRootNodeRecursive(current);
			}
			ExceptionSupport.Node result;
			if (array.Length == 1)
			{
				ExceptionSupport.Node node = array[0];
				if (node.Type == ExceptionSupport.NodeType.Block)
				{
					result = new ExceptionSupport.Node(null, TryCatchTreeBuilder.NodeTypeFor(context), node.Block, new ExceptionSupport.Node[0], TryCatchTreeBuilder.ExceptionHandlerFor(context));
					return result;
				}
			}
			result = new ExceptionSupport.Node(null, TryCatchTreeBuilder.NodeTypeFor(context), TryCatchTreeBuilder.BlockFor(context), array, TryCatchTreeBuilder.ExceptionHandlerFor(context));
			return result;
		}

		private static ExceptionSupport.NodeType NodeTypeFor(TryCatchTreeBuilder.Context context)
		{
			ExceptionSupport.NodeType result;
			switch (context.Type)
			{
			case TryCatchTreeBuilder.ContextType.Root:
				result = ExceptionSupport.NodeType.Root;
				return result;
			case TryCatchTreeBuilder.ContextType.Try:
				result = ExceptionSupport.NodeType.Try;
				return result;
			case TryCatchTreeBuilder.ContextType.Catch:
				result = ExceptionSupport.NodeType.Catch;
				return result;
			case TryCatchTreeBuilder.ContextType.Finally:
				result = ExceptionSupport.NodeType.Finally;
				return result;
			case TryCatchTreeBuilder.ContextType.Fault:
				result = ExceptionSupport.NodeType.Fault;
				return result;
			}
			result = ExceptionSupport.NodeType.Block;
			return result;
		}

		private static InstructionBlock BlockFor(TryCatchTreeBuilder.Context context)
		{
			return (context.Type != TryCatchTreeBuilder.ContextType.Block) ? null : context.Block;
		}

		private static ExceptionHandler ExceptionHandlerFor(TryCatchTreeBuilder.Context context)
		{
			TryCatchTreeBuilder.ContextType type = context.Type;
			ExceptionHandler result;
			if (type != TryCatchTreeBuilder.ContextType.Catch && type != TryCatchTreeBuilder.ContextType.Finally && type != TryCatchTreeBuilder.ContextType.Fault)
			{
				result = null;
			}
			else
			{
				result = context.Handler;
			}
			return result;
		}

		private static ExceptionSupport.Node MakeRoot(ExceptionSupport.Node[] children)
		{
			return new ExceptionSupport.Node(null, ExceptionSupport.NodeType.Root, null, children, null);
		}
	}
}
