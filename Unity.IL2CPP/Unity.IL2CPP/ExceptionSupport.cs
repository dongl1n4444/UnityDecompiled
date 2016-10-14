using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.IL2CPP.Common.CFG;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	public class ExceptionSupport
	{
		internal enum NodeType
		{
			Try,
			Catch,
			Finally,
			Block,
			Root,
			Fault
		}

		public class Node
		{
			private ExceptionSupport.Node _parent;

			private readonly ExceptionSupport.NodeType _type;

			private readonly ExceptionSupport.Node[] _children;

			private readonly InstructionBlock _block;

			private readonly ExceptionHandler _handler;

			internal ExceptionSupport.NodeType Type
			{
				get
				{
					return this._type;
				}
			}

			internal InstructionBlock Block
			{
				get
				{
					return this._block;
				}
			}

			internal ExceptionSupport.Node[] Children
			{
				get
				{
					return this._children;
				}
			}

			internal ExceptionSupport.Node Parent
			{
				get
				{
					return this._parent;
				}
			}

			internal bool IsInTryBlock
			{
				get
				{
					ExceptionSupport.Node node = this;
					bool result;
					while (node != null && node.Type != ExceptionSupport.NodeType.Root)
					{
						node = node.Parent;
						if (node.Type == ExceptionSupport.NodeType.Try)
						{
							result = true;
						}
						else if (node.Type == ExceptionSupport.NodeType.Catch)
						{
							result = false;
						}
						else if (node.Type == ExceptionSupport.NodeType.Finally)
						{
							result = false;
						}
						else
						{
							if (node.Type != ExceptionSupport.NodeType.Fault)
							{
								continue;
							}
							result = false;
						}
						return result;
					}
					result = false;
					return result;
				}
			}

			internal bool IsInCatchBlock
			{
				get
				{
					ExceptionSupport.Node node = this;
					bool result;
					while (node != null && node.Type != ExceptionSupport.NodeType.Root)
					{
						node = node.Parent;
						if (node.Type == ExceptionSupport.NodeType.Try)
						{
							result = false;
						}
						else if (node.Type == ExceptionSupport.NodeType.Catch)
						{
							result = true;
						}
						else if (node.Type == ExceptionSupport.NodeType.Finally)
						{
							result = false;
						}
						else
						{
							if (node.Type != ExceptionSupport.NodeType.Fault)
							{
								continue;
							}
							result = false;
						}
						return result;
					}
					result = false;
					return result;
				}
			}

			internal bool IsInFinallyBlock
			{
				get
				{
					ExceptionSupport.Node node = this;
					bool result;
					while (node != null && node.Type != ExceptionSupport.NodeType.Root)
					{
						node = node.Parent;
						if (node.Type == ExceptionSupport.NodeType.Try)
						{
							result = false;
						}
						else if (node.Type == ExceptionSupport.NodeType.Catch)
						{
							result = false;
						}
						else if (node.Type == ExceptionSupport.NodeType.Finally)
						{
							result = true;
						}
						else
						{
							if (node.Type != ExceptionSupport.NodeType.Fault)
							{
								continue;
							}
							result = false;
						}
						return result;
					}
					result = false;
					return result;
				}
			}

			internal bool IsInFaultBlock
			{
				get
				{
					ExceptionSupport.Node node = this;
					bool result;
					while (node != null && node.Type != ExceptionSupport.NodeType.Root)
					{
						node = node.Parent;
						if (node.Type == ExceptionSupport.NodeType.Try)
						{
							result = false;
						}
						else if (node.Type == ExceptionSupport.NodeType.Catch)
						{
							result = false;
						}
						else if (node.Type == ExceptionSupport.NodeType.Finally)
						{
							result = false;
						}
						else
						{
							if (node.Type != ExceptionSupport.NodeType.Fault)
							{
								continue;
							}
							result = true;
						}
						return result;
					}
					result = false;
					return result;
				}
			}

			private ExceptionSupport.Node PrevSibling
			{
				get
				{
					ExceptionSupport.Node result;
					if (this.Parent == null)
					{
						result = null;
					}
					else
					{
						int num = Array.IndexOf<ExceptionSupport.Node>(this.Parent.Children, this);
						if (num == 0)
						{
							result = null;
						}
						else
						{
							result = this.Parent.Children[num - 1];
						}
					}
					return result;
				}
			}

			private ExceptionSupport.Node NextSibling
			{
				get
				{
					ExceptionSupport.Node result;
					if (this.Parent == null)
					{
						result = null;
					}
					else
					{
						int num = Array.IndexOf<ExceptionSupport.Node>(this.Parent.Children, this);
						if (num == this.Parent.Children.Length - 1)
						{
							result = null;
						}
						else
						{
							result = this.Parent.Children[num + 1];
						}
					}
					return result;
				}
			}

			internal ExceptionHandler Handler
			{
				get
				{
					return this._handler;
				}
			}

			private ExceptionSupport.Node Root
			{
				get
				{
					ExceptionSupport.Node node = this;
					while (node != null && node.Type != ExceptionSupport.NodeType.Root)
					{
						node = node.Parent;
					}
					return node;
				}
			}

			internal int Depth
			{
				get
				{
					int num = 0;
					for (ExceptionSupport.Node parent = this._parent; parent != null; parent = parent.Parent)
					{
						num++;
					}
					return num;
				}
			}

			internal ExceptionSupport.Node TryNode
			{
				get
				{
					if (this._type != ExceptionSupport.NodeType.Catch && this._type != ExceptionSupport.NodeType.Finally && this._type != ExceptionSupport.NodeType.Fault)
					{
						throw new NotSupportedException("Cannot find the related try node for a non-handler block");
					}
					ExceptionSupport.Node prevSibling = this.PrevSibling;
					while (prevSibling != null && prevSibling.Type != ExceptionSupport.NodeType.Try)
					{
						prevSibling = prevSibling.PrevSibling;
					}
					if (prevSibling == null)
					{
						throw new NotSupportedException("Handler block has not a corresponding try block!");
					}
					return prevSibling;
				}
			}

			internal ExceptionSupport.Node[] CatchNodes
			{
				get
				{
					if (this._type != ExceptionSupport.NodeType.Try)
					{
						throw new NotSupportedException("Cannot find the related finally handler for a non-try block");
					}
					List<ExceptionSupport.Node> list = new List<ExceptionSupport.Node>();
					ExceptionSupport.Node nextSibling = this.NextSibling;
					while (nextSibling != null && nextSibling.Type == ExceptionSupport.NodeType.Catch)
					{
						list.Add(nextSibling);
						nextSibling = nextSibling.NextSibling;
					}
					return list.ToArray();
				}
			}

			internal ExceptionSupport.Node FinallyNode
			{
				get
				{
					if (this._type != ExceptionSupport.NodeType.Try)
					{
						throw new NotSupportedException("Cannot find the related finally handler for a non-try block");
					}
					ExceptionSupport.Node nextSibling = this.NextSibling;
					ExceptionSupport.Node result;
					if (nextSibling == null || nextSibling.Type != ExceptionSupport.NodeType.Finally)
					{
						result = null;
					}
					else
					{
						result = nextSibling;
					}
					return result;
				}
			}

			internal ExceptionSupport.Node FaultNode
			{
				get
				{
					if (this._type != ExceptionSupport.NodeType.Try)
					{
						throw new NotSupportedException("Cannot find the related fault handler for a non-try block");
					}
					ExceptionSupport.Node nextSibling = this.NextSibling;
					ExceptionSupport.Node result;
					if (nextSibling == null || nextSibling.Type != ExceptionSupport.NodeType.Fault)
					{
						result = null;
					}
					else
					{
						result = nextSibling;
					}
					return result;
				}
			}

			internal Instruction Start
			{
				get
				{
					for (ExceptionSupport.Node node = this; node != null; node = node.Children[0])
					{
						if (node.Block != null)
						{
							return node.Block.First;
						}
					}
					throw new NotSupportedException("Unsupported Node (" + this + ") with no children!");
				}
			}

			internal Instruction End
			{
				get
				{
					Instruction result;
					if (this.Block != null)
					{
						result = this.Block.Last;
					}
					else
					{
						if (this._children.Length == 0)
						{
							throw new NotSupportedException("Unsupported Node (" + this + ") with no children!");
						}
						result = this._children[this._children.Length - 1].End;
					}
					return result;
				}
			}

			internal Node(ExceptionSupport.NodeType type, InstructionBlock block) : this(null, type, block, new ExceptionSupport.Node[0], null)
			{
			}

			internal Node(ExceptionSupport.Node parent, ExceptionSupport.NodeType type, InstructionBlock block, ExceptionSupport.Node[] children, ExceptionHandler handler)
			{
				this._parent = parent;
				this._type = type;
				this._block = block;
				this._children = children;
				this._handler = handler;
				if (this._block != null && type != ExceptionSupport.NodeType.Block)
				{
					this._block.MarkIsAliveRecursive();
				}
				if (this._parent != null && this._parent.Type != ExceptionSupport.NodeType.Root)
				{
					this._block.MarkIsAliveRecursive();
				}
				bool flag = this._type != ExceptionSupport.NodeType.Root;
				ExceptionSupport.Node[] children2 = this._children;
				for (int i = 0; i < children2.Length; i++)
				{
					ExceptionSupport.Node node = children2[i];
					node._parent = this;
					if (flag && node.Block != null)
					{
						node._block.MarkIsAliveRecursive();
					}
				}
			}

			internal IEnumerable<ExceptionSupport.Node> GetTargetFinallyNodesForJump(int from, int to)
			{
				return from n in this.Root.Walk((ExceptionSupport.Node node) => ExceptionSupport.Node.IsTargetFinallyNodeForJump(node, @from, to)).Reverse<ExceptionSupport.Node>()
				select n.FinallyNode;
			}

			internal IEnumerable<ExceptionSupport.Node> GetTargetFinallyAndFaultNodesForJump(int from, int to)
			{
				return from n in this.Root.Walk((ExceptionSupport.Node node) => ExceptionSupport.Node.IsTargetFinallyNodeForJump(node, @from, to) || ExceptionSupport.Node.IsTargetFaultNodeForJump(node, @from, to)).Reverse<ExceptionSupport.Node>()
				select n.FinallyNode ?? n.FaultNode;
			}

			private static bool IsTargetFinallyNodeForJump(ExceptionSupport.Node node, int from, int to)
			{
				return node.Type == ExceptionSupport.NodeType.Try && node.FinallyNode != null && node.FinallyNode.Handler.TryStart.Offset <= from && node.FinallyNode.Handler.TryEnd.Offset > from && node.FinallyNode.Handler.HandlerStart.Offset <= to;
			}

			private static bool IsTargetFaultNodeForJump(ExceptionSupport.Node node, int from, int to)
			{
				return node.Type == ExceptionSupport.NodeType.Try && node.FaultNode != null && node.FaultNode.Handler.TryStart.Offset <= from && node.FaultNode.Handler.TryEnd.Offset > from && node.FaultNode.Handler.HandlerStart.Offset <= to;
			}

			internal ExceptionSupport.Node GetEnclosingFinallyOrFaultNode()
			{
				ExceptionSupport.Node result;
				for (ExceptionSupport.Node node = this; node != null; node = node.Parent)
				{
					if (node.Type == ExceptionSupport.NodeType.Finally || node.Type == ExceptionSupport.NodeType.Fault)
					{
						result = node;
						return result;
					}
				}
				result = null;
				return result;
			}

			[DebuggerHidden]
			private IEnumerable<ExceptionSupport.Node> Walk(Func<ExceptionSupport.Node, bool> filter)
			{
				ExceptionSupport.Node.<Walk>c__Iterator0 <Walk>c__Iterator = new ExceptionSupport.Node.<Walk>c__Iterator0();
				<Walk>c__Iterator.filter = filter;
				<Walk>c__Iterator.$this = this;
				ExceptionSupport.Node.<Walk>c__Iterator0 expr_15 = <Walk>c__Iterator;
				expr_15.$PC = -2;
				return expr_15;
			}

			public override string ToString()
			{
				return string.Format("{0} children: {1}, depth: {2}", Enum.GetName(typeof(ExceptionSupport.NodeType), this._type), this._children.Length, this.Depth);
			}
		}

		[Inject]
		public static INamingService Naming;

		[Inject]
		public static ITypeProviderService TypeProvider;

		public const string LeaveTargetName = "__leave_target";

		public const string LocalExceptionName = "__exception_local";

		public const string LastUnhandledExceptionName = "__last_unhandled_exception";

		private readonly ExceptionSupport.Node _flowTree;

		private readonly CppCodeWriter _writer;

		private readonly MethodBody _methodBody;

		private readonly Dictionary<Instruction, TryCatchInfo> _infos = new Dictionary<Instruction, TryCatchInfo>();

		private readonly Dictionary<ExceptionSupport.Node, HashSet<Instruction>> _leaveTargets = new Dictionary<ExceptionSupport.Node, HashSet<Instruction>>();

		public ExceptionSupport.Node FlowTree
		{
			get
			{
				return this._flowTree;
			}
		}

		public ExceptionSupport(MethodDefinition methodDefinition, InstructionBlock[] blocks, CppCodeWriter writer)
		{
			this._writer = writer;
			this._methodBody = methodDefinition.Body;
			if (this._methodBody.ExceptionHandlers.Any((ExceptionHandler h) => h.HandlerType == ExceptionHandlerType.Filter))
			{
				throw new NotSupportedException("Filter exception handlers types are not supported yet!");
			}
			this.CollectTryCatchInfos(methodDefinition.Body);
			this._flowTree = new TryCatchTreeBuilder(this._methodBody, blocks, this._infos).Build();
		}

		public void Prepare()
		{
			if (this._methodBody.HasExceptionHandlers)
			{
				this._writer.WriteLine("{0} {1} = 0;", new object[]
				{
					ExceptionSupport.Naming.ForVariable(ExceptionSupport.TypeProvider.SystemException),
					"__last_unhandled_exception"
				});
				this._writer.WriteLine("NO_UNUSED_WARNING ({0});", new object[]
				{
					"__last_unhandled_exception"
				});
				this._writer.WriteLine("{0} {1} = 0;", new object[]
				{
					ExceptionSupport.Naming.ForVariable(ExceptionSupport.TypeProvider.SystemException),
					"__exception_local"
				});
				this._writer.WriteLine("NO_UNUSED_WARNING ({0});", new object[]
				{
					"__exception_local"
				});
				this._writer.WriteLine("int32_t {0} = 0;", new object[]
				{
					"__leave_target"
				});
				this._writer.WriteLine("NO_UNUSED_WARNING ({0});", new object[]
				{
					"__leave_target"
				});
			}
		}

		internal void PushExceptionOnStackIfNeeded(ExceptionSupport.Node node, Stack<StackInfo> valueStack, TypeResolver typeResolver)
		{
			if (node.Type == ExceptionSupport.NodeType.Catch && node.Block != null)
			{
				ExceptionSupport.PushExceptionOnStack(valueStack, typeResolver.Resolve(node.Handler.CatchType));
			}
			else if (node.Parent.Type == ExceptionSupport.NodeType.Catch)
			{
				if (node.Parent.Children[0] == node)
				{
					ExceptionSupport.PushExceptionOnStack(valueStack, typeResolver.Resolve(node.Parent.Handler.CatchType));
				}
			}
		}

		[DebuggerHidden]
		internal IEnumerable<Instruction> LeaveTargetsFor(ExceptionSupport.Node finallyNode)
		{
			ExceptionSupport.<LeaveTargetsFor>c__Iterator0 <LeaveTargetsFor>c__Iterator = new ExceptionSupport.<LeaveTargetsFor>c__Iterator0();
			<LeaveTargetsFor>c__Iterator.finallyNode = finallyNode;
			<LeaveTargetsFor>c__Iterator.$this = this;
			ExceptionSupport.<LeaveTargetsFor>c__Iterator0 expr_15 = <LeaveTargetsFor>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}

		internal void AddLeaveTarget(ExceptionSupport.Node finallyNode, Instruction instruction)
		{
			HashSet<Instruction> hashSet;
			if (!this._leaveTargets.TryGetValue(finallyNode, out hashSet))
			{
				hashSet = new HashSet<Instruction>();
				this._leaveTargets[finallyNode] = hashSet;
			}
			hashSet.Add((Instruction)instruction.Operand);
		}

		internal ExceptionHandler[] CatchHandlersForRange(Instruction start, Instruction end)
		{
			return (from h in this._methodBody.ExceptionHandlers
			where h.HandlerType == ExceptionHandlerType.Catch && h.TryStart == start && h.TryEnd == end
			select h).ToArray<ExceptionHandler>();
		}

		internal ExceptionHandler EnclosingFinallyHandlerForRange(Instruction start, Instruction end)
		{
			return (from h in this._methodBody.ExceptionHandlers
			where h.HandlerType == ExceptionHandlerType.Finally && h.TryStart.Offset <= start.Offset && h.TryEnd.Offset >= end.Offset
			orderby h.TryStart.Offset descending
			select h).FirstOrDefault<ExceptionHandler>();
		}

		private void CollectTryCatchInfos(MethodBody body)
		{
			Collection<Instruction> instructions = body.Instructions;
			foreach (Instruction current in instructions)
			{
				this._infos[current] = new TryCatchInfo();
			}
			this.BuildTryCatchScopeRecursive(instructions, body.ExceptionHandlers);
		}

		private void BuildTryCatchScopeRecursive(IList<Instruction> instructions, IList<ExceptionHandler> handlers)
		{
			if (handlers.Count != 0)
			{
				int tryStart = handlers.Min((ExceptionHandler h) => h.TryStart.Offset);
				int tryEnd = (from h in handlers
				where h.TryStart.Offset == tryStart
				select h).Max((ExceptionHandler eh) => eh.TryEnd.Offset);
				List<ExceptionHandler> list = (from h in handlers
				where h.TryStart.Offset == tryStart && h.TryEnd.Offset == tryEnd
				orderby h.TryStart.Offset
				select h).ToList<ExceptionHandler>();
				HashSet<ExceptionHandler> hashSet = new HashSet<ExceptionHandler>(from h in handlers
				where (tryStart <= h.TryStart.Offset && h.TryEnd.Offset < tryEnd) || (tryStart < h.TryStart.Offset && h.TryEnd.Offset <= tryEnd)
				select h);
				int num = 0;
				while (num < instructions.Count && instructions[num].Offset < tryEnd)
				{
					num++;
				}
				this._infos[instructions.Single((Instruction i) => i.Offset == tryStart)].TryStart++;
				this._infos[instructions[num]].TryEnd++;
				this.BuildTryCatchScopeRecursive(instructions, hashSet.ToList<ExceptionHandler>());
				handlers = handlers.Except(hashSet).ToArray<ExceptionHandler>();
				using (List<ExceptionHandler>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ExceptionSupport.<BuildTryCatchScopeRecursive>c__AnonStorey4 <BuildTryCatchScopeRecursive>c__AnonStorey2 = new ExceptionSupport.<BuildTryCatchScopeRecursive>c__AnonStorey4();
						<BuildTryCatchScopeRecursive>c__AnonStorey2.h = enumerator.Current;
						int blockNesterHandlers = <BuildTryCatchScopeRecursive>c__AnonStorey2.h.HandlerEnd.Offset;
						int num2 = 0;
						while (num2 < instructions.Count && instructions[num2].Offset < <BuildTryCatchScopeRecursive>c__AnonStorey2.h.HandlerStart.Offset)
						{
							num2++;
						}
						int num3 = 0;
						while (num3 < instructions.Count && instructions[num3].Offset < blockNesterHandlers)
						{
							num3++;
						}
						HashSet<ExceptionHandler> hashSet2 = new HashSet<ExceptionHandler>(from e in handlers
						where (<BuildTryCatchScopeRecursive>c__AnonStorey2.h.HandlerStart.Offset <= e.TryStart.Offset && e.TryEnd.Offset < blockNesterHandlers) || (<BuildTryCatchScopeRecursive>c__AnonStorey2.h.HandlerStart.Offset < e.TryStart.Offset && e.TryEnd.Offset <= blockNesterHandlers)
						select e);
						if (<BuildTryCatchScopeRecursive>c__AnonStorey2.h.HandlerType == ExceptionHandlerType.Catch)
						{
							this._infos[instructions[num2]].CatchStart++;
							this._infos[instructions[num3]].CatchEnd++;
						}
						else if (<BuildTryCatchScopeRecursive>c__AnonStorey2.h.HandlerType == ExceptionHandlerType.Finally)
						{
							this._infos[instructions[num2]].FinallyStart++;
							this._infos[instructions[num3]].FinallyEnd++;
						}
						else
						{
							this._infos[instructions[num2]].FaultStart++;
							this._infos[instructions[num3]].FaultEnd++;
						}
						this.BuildTryCatchScopeRecursive(instructions, hashSet2.ToList<ExceptionHandler>());
						handlers = handlers.Except(hashSet2).ToArray<ExceptionHandler>();
					}
				}
				this.BuildTryCatchScopeRecursive(instructions, handlers.Except(list).ToArray<ExceptionHandler>());
			}
		}

		private static void PushExceptionOnStack(Stack<StackInfo> valueStack, TypeReference catchType)
		{
			valueStack.Push(new StackInfo(string.Format("(({0}){1})", ExceptionSupport.Naming.ForVariable(catchType), "__exception_local"), catchType));
		}
	}
}
