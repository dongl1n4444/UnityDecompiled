namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Collections.Generic;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Unity.IL2CPP.Common.CFG;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class ExceptionSupport
    {
        private readonly Node _flowTree;
        private readonly Dictionary<Instruction, TryCatchInfo> _infos = new Dictionary<Instruction, TryCatchInfo>();
        private readonly Dictionary<Node, HashSet<Instruction>> _leaveTargets = new Dictionary<Node, HashSet<Instruction>>();
        private readonly MethodBody _methodBody;
        private readonly CppCodeWriter _writer;
        [CompilerGenerated]
        private static Func<ExceptionHandler, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<ExceptionHandler, int> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<ExceptionHandler, int> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<ExceptionHandler, int> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<ExceptionHandler, int> <>f__am$cache4;
        public const string LastUnhandledExceptionName = "__last_unhandled_exception";
        public const string LeaveTargetName = "__leave_target";
        public const string LocalExceptionName = "__exception_local";
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static ITypeProviderService TypeProvider;

        public ExceptionSupport(MethodDefinition methodDefinition, InstructionBlock[] blocks, CppCodeWriter writer)
        {
            this._writer = writer;
            this._methodBody = methodDefinition.Body;
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<ExceptionHandler, bool>(ExceptionSupport.<ExceptionSupport>m__0);
            }
            if (this._methodBody.ExceptionHandlers.Any<ExceptionHandler>(<>f__am$cache0))
            {
                throw new NotSupportedException("Filter exception handlers types are not supported yet!");
            }
            this.CollectTryCatchInfos(methodDefinition.Body);
            this._flowTree = new TryCatchTreeBuilder(this._methodBody, blocks, this._infos).Build();
        }

        [CompilerGenerated]
        private static bool <ExceptionSupport>m__0(ExceptionHandler h) => 
            (h.HandlerType == ExceptionHandlerType.Filter);

        internal void AddLeaveTarget(Node finallyNode, Instruction instruction)
        {
            HashSet<Instruction> set;
            if (!this._leaveTargets.TryGetValue(finallyNode, out set))
            {
                set = new HashSet<Instruction>();
                this._leaveTargets[finallyNode] = set;
            }
            set.Add((Instruction) instruction.Operand);
        }

        private void BuildTryCatchScopeRecursive(IList<Instruction> instructions, IList<ExceptionHandler> handlers)
        {
            <BuildTryCatchScopeRecursive>c__AnonStorey3 storey = new <BuildTryCatchScopeRecursive>c__AnonStorey3();
            if (handlers.Count != 0)
            {
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = h => h.TryStart.Offset;
                }
                storey.tryStart = handlers.Min<ExceptionHandler>(<>f__am$cache2);
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = eh => eh.TryEnd.Offset;
                }
                storey.tryEnd = handlers.Where<ExceptionHandler>(new Func<ExceptionHandler, bool>(storey.<>m__0)).Max<ExceptionHandler>(<>f__am$cache3);
                if (<>f__am$cache4 == null)
                {
                    <>f__am$cache4 = h => h.TryStart.Offset;
                }
                List<ExceptionHandler> second = handlers.Where<ExceptionHandler>(new Func<ExceptionHandler, bool>(storey.<>m__1)).OrderBy<ExceptionHandler, int>(<>f__am$cache4).ToList<ExceptionHandler>();
                HashSet<ExceptionHandler> source = new HashSet<ExceptionHandler>(handlers.Where<ExceptionHandler>(new Func<ExceptionHandler, bool>(storey.<>m__2)));
                int num = 0;
                while ((num < instructions.Count) && (instructions[num].Offset < storey.tryEnd))
                {
                    num++;
                }
                TryCatchInfo local1 = this._infos[instructions.Single<Instruction>(new Func<Instruction, bool>(storey.<>m__3))];
                local1.TryStart++;
                TryCatchInfo local2 = this._infos[instructions[num]];
                local2.TryEnd++;
                this.BuildTryCatchScopeRecursive(instructions, source.ToList<ExceptionHandler>());
                handlers = handlers.Except<ExceptionHandler>(source).ToArray<ExceptionHandler>();
                using (List<ExceptionHandler>.Enumerator enumerator = second.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        <BuildTryCatchScopeRecursive>c__AnonStorey4 storey2 = new <BuildTryCatchScopeRecursive>c__AnonStorey4 {
                            h = enumerator.Current
                        };
                        <BuildTryCatchScopeRecursive>c__AnonStorey5 storey3 = new <BuildTryCatchScopeRecursive>c__AnonStorey5 {
                            <>f__ref$4 = storey2,
                            blockNesterHandlers = storey2.h.HandlerEnd.Offset
                        };
                        int num2 = 0;
                        while ((num2 < instructions.Count) && (instructions[num2].Offset < storey2.h.HandlerStart.Offset))
                        {
                            num2++;
                        }
                        int num3 = 0;
                        while ((num3 < instructions.Count) && (instructions[num3].Offset < storey3.blockNesterHandlers))
                        {
                            num3++;
                        }
                        HashSet<ExceptionHandler> set2 = new HashSet<ExceptionHandler>(handlers.Where<ExceptionHandler>(new Func<ExceptionHandler, bool>(storey3.<>m__0)));
                        if (storey2.h.HandlerType == ExceptionHandlerType.Catch)
                        {
                            TryCatchInfo local3 = this._infos[instructions[num2]];
                            local3.CatchStart++;
                            TryCatchInfo local4 = this._infos[instructions[num3]];
                            local4.CatchEnd++;
                        }
                        else if (storey2.h.HandlerType == ExceptionHandlerType.Finally)
                        {
                            TryCatchInfo local5 = this._infos[instructions[num2]];
                            local5.FinallyStart++;
                            TryCatchInfo local6 = this._infos[instructions[num3]];
                            local6.FinallyEnd++;
                        }
                        else
                        {
                            TryCatchInfo local7 = this._infos[instructions[num2]];
                            local7.FaultStart++;
                            TryCatchInfo local8 = this._infos[instructions[num3]];
                            local8.FaultEnd++;
                        }
                        this.BuildTryCatchScopeRecursive(instructions, set2.ToList<ExceptionHandler>());
                        handlers = handlers.Except<ExceptionHandler>(set2).ToArray<ExceptionHandler>();
                    }
                }
                this.BuildTryCatchScopeRecursive(instructions, handlers.Except<ExceptionHandler>(second).ToArray<ExceptionHandler>());
            }
        }

        internal ExceptionHandler[] CatchHandlersForRange(Instruction start, Instruction end)
        {
            <CatchHandlersForRange>c__AnonStorey1 storey = new <CatchHandlersForRange>c__AnonStorey1 {
                start = start,
                end = end
            };
            return this._methodBody.ExceptionHandlers.Where<ExceptionHandler>(new Func<ExceptionHandler, bool>(storey.<>m__0)).ToArray<ExceptionHandler>();
        }

        private void CollectTryCatchInfos(MethodBody body)
        {
            Collection<Instruction> instructions = body.Instructions;
            foreach (Instruction instruction in instructions)
            {
                this._infos[instruction] = new TryCatchInfo();
            }
            this.BuildTryCatchScopeRecursive(instructions, body.ExceptionHandlers);
        }

        internal ExceptionHandler EnclosingFinallyHandlerForRange(Instruction start, Instruction end)
        {
            <EnclosingFinallyHandlerForRange>c__AnonStorey2 storey = new <EnclosingFinallyHandlerForRange>c__AnonStorey2 {
                start = start,
                end = end
            };
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = h => h.TryStart.Offset;
            }
            return this._methodBody.ExceptionHandlers.Where<ExceptionHandler>(new Func<ExceptionHandler, bool>(storey.<>m__0)).OrderByDescending<ExceptionHandler, int>(<>f__am$cache1).FirstOrDefault<ExceptionHandler>();
        }

        [DebuggerHidden]
        internal IEnumerable<Instruction> LeaveTargetsFor(Node finallyNode) => 
            new <LeaveTargetsFor>c__Iterator0 { 
                finallyNode = finallyNode,
                $this = this,
                $PC = -2
            };

        public void Prepare()
        {
            if (this._methodBody.HasExceptionHandlers)
            {
                object[] args = new object[] { Naming.ForVariable(TypeProvider.SystemException), "__last_unhandled_exception" };
                this._writer.WriteLine("{0} {1} = 0;", args);
                object[] objArray2 = new object[] { "__last_unhandled_exception" };
                this._writer.WriteLine("NO_UNUSED_WARNING ({0});", objArray2);
                object[] objArray3 = new object[] { Naming.ForVariable(TypeProvider.SystemException), "__exception_local" };
                this._writer.WriteLine("{0} {1} = 0;", objArray3);
                object[] objArray4 = new object[] { "__exception_local" };
                this._writer.WriteLine("NO_UNUSED_WARNING ({0});", objArray4);
                object[] objArray5 = new object[] { "__leave_target" };
                this._writer.WriteLine("int32_t {0} = 0;", objArray5);
                object[] objArray6 = new object[] { "__leave_target" };
                this._writer.WriteLine("NO_UNUSED_WARNING ({0});", objArray6);
            }
        }

        private static void PushExceptionOnStack(Stack<StackInfo> valueStack, TypeReference catchType)
        {
            valueStack.Push(new StackInfo($"(({Naming.ForVariable(catchType)}){"__exception_local"})", catchType));
        }

        internal void PushExceptionOnStackIfNeeded(Node node, Stack<StackInfo> valueStack, Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver)
        {
            if ((node.Type == NodeType.Catch) && (node.Block != null))
            {
                PushExceptionOnStack(valueStack, typeResolver.Resolve(node.Handler.CatchType));
            }
            else if ((node.Parent.Type == NodeType.Catch) && (node.Parent.Children[0] == node))
            {
                PushExceptionOnStack(valueStack, typeResolver.Resolve(node.Parent.Handler.CatchType));
            }
        }

        public Node FlowTree =>
            this._flowTree;

        [CompilerGenerated]
        private sealed class <BuildTryCatchScopeRecursive>c__AnonStorey3
        {
            internal int tryEnd;
            internal int tryStart;

            internal bool <>m__0(ExceptionHandler h) => 
                (h.TryStart.Offset == this.tryStart);

            internal bool <>m__1(ExceptionHandler h) => 
                ((h.TryStart.Offset == this.tryStart) && (h.TryEnd.Offset == this.tryEnd));

            internal bool <>m__2(ExceptionHandler h) => 
                (((this.tryStart <= h.TryStart.Offset) && (h.TryEnd.Offset < this.tryEnd)) || ((this.tryStart < h.TryStart.Offset) && (h.TryEnd.Offset <= this.tryEnd)));

            internal bool <>m__3(Instruction i) => 
                (i.Offset == this.tryStart);
        }

        [CompilerGenerated]
        private sealed class <BuildTryCatchScopeRecursive>c__AnonStorey4
        {
            internal ExceptionHandler h;
        }

        [CompilerGenerated]
        private sealed class <BuildTryCatchScopeRecursive>c__AnonStorey5
        {
            internal ExceptionSupport.<BuildTryCatchScopeRecursive>c__AnonStorey4 <>f__ref$4;
            internal int blockNesterHandlers;

            internal bool <>m__0(ExceptionHandler e) => 
                (((this.<>f__ref$4.h.HandlerStart.Offset <= e.TryStart.Offset) && (e.TryEnd.Offset < this.blockNesterHandlers)) || ((this.<>f__ref$4.h.HandlerStart.Offset < e.TryStart.Offset) && (e.TryEnd.Offset <= this.blockNesterHandlers)));
        }

        [CompilerGenerated]
        private sealed class <CatchHandlersForRange>c__AnonStorey1
        {
            internal Instruction end;
            internal Instruction start;

            internal bool <>m__0(ExceptionHandler h) => 
                (((h.HandlerType == ExceptionHandlerType.Catch) && (h.TryStart == this.start)) && (h.TryEnd == this.end));
        }

        [CompilerGenerated]
        private sealed class <EnclosingFinallyHandlerForRange>c__AnonStorey2
        {
            internal Instruction end;
            internal Instruction start;

            internal bool <>m__0(ExceptionHandler h) => 
                (((h.HandlerType == ExceptionHandlerType.Finally) && (h.TryStart.Offset <= this.start.Offset)) && (h.TryEnd.Offset >= this.end.Offset));
        }

        [CompilerGenerated]
        private sealed class <LeaveTargetsFor>c__Iterator0 : IEnumerable, IEnumerable<Instruction>, IEnumerator, IDisposable, IEnumerator<Instruction>
        {
            internal Instruction $current;
            internal bool $disposing;
            internal HashSet<Instruction>.Enumerator $locvar0;
            internal int $PC;
            internal ExceptionSupport $this;
            internal Instruction <instruction>__1;
            internal HashSet<Instruction> <targets>__0;
            internal ExceptionSupport.Node finallyNode;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                        try
                        {
                        }
                        finally
                        {
                            this.$locvar0.Dispose();
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        if (this.$this._leaveTargets.TryGetValue(this.finallyNode, out this.<targets>__0))
                        {
                            this.$locvar0 = this.<targets>__0.GetEnumerator();
                            num = 0xfffffffd;
                            break;
                        }
                        goto Label_00D6;

                    case 1:
                        break;

                    default:
                        goto Label_00D6;
                }
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<instruction>__1 = this.$locvar0.Current;
                        this.$current = this.<instruction>__1;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        flag = true;
                        return true;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    this.$locvar0.Dispose();
                }
                this.$PC = -1;
            Label_00D6:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<Instruction> IEnumerable<Instruction>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new ExceptionSupport.<LeaveTargetsFor>c__Iterator0 { 
                    $this = this.$this,
                    finallyNode = this.finallyNode
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<Mono.Cecil.Cil.Instruction>.GetEnumerator();

            Instruction IEnumerator<Instruction>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        public class Node
        {
            private readonly InstructionBlock _block;
            private readonly ExceptionSupport.Node[] _children;
            private readonly ExceptionHandler _handler;
            private ExceptionSupport.Node _parent;
            private readonly ExceptionSupport.NodeType _type;
            [CompilerGenerated]
            private static Func<ExceptionSupport.Node, ExceptionSupport.Node> <>f__am$cache0;
            [CompilerGenerated]
            private static Func<ExceptionSupport.Node, ExceptionSupport.Node> <>f__am$cache1;

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
                if ((this._block != null) && (type != ExceptionSupport.NodeType.Block))
                {
                    this._block.MarkIsAliveRecursive();
                }
                if ((this._parent != null) && (this._parent.Type != ExceptionSupport.NodeType.Root))
                {
                    this._block.MarkIsAliveRecursive();
                }
                bool flag = this._type != ExceptionSupport.NodeType.Root;
                foreach (ExceptionSupport.Node node in this._children)
                {
                    node._parent = this;
                    if (flag && (node.Block != null))
                    {
                        node._block.MarkIsAliveRecursive();
                    }
                }
            }

            internal ExceptionSupport.Node GetEnclosingFinallyOrFaultNode()
            {
                for (ExceptionSupport.Node node = this; node != null; node = node.Parent)
                {
                    if ((node.Type == ExceptionSupport.NodeType.Finally) || (node.Type == ExceptionSupport.NodeType.Fault))
                    {
                        return node;
                    }
                }
                return null;
            }

            internal IEnumerable<ExceptionSupport.Node> GetTargetFinallyAndFaultNodesForJump(int from, int to)
            {
                <GetTargetFinallyAndFaultNodesForJump>c__AnonStorey2 storey = new <GetTargetFinallyAndFaultNodesForJump>c__AnonStorey2 {
                    from = from,
                    to = to
                };
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = delegate (ExceptionSupport.Node n) {
                        ExceptionSupport.Node faultNode;
                        ExceptionSupport.Node finallyNode = n.FinallyNode;
                        if (finallyNode != null)
                        {
                            faultNode = finallyNode;
                        }
                        else
                        {
                            faultNode = n.FaultNode;
                        }
                        return faultNode;
                    };
                }
                return this.Root.Walk(new Func<ExceptionSupport.Node, bool>(storey.<>m__0)).Reverse<ExceptionSupport.Node>().Select<ExceptionSupport.Node, ExceptionSupport.Node>(<>f__am$cache1);
            }

            internal IEnumerable<ExceptionSupport.Node> GetTargetFinallyNodesForJump(int from, int to)
            {
                <GetTargetFinallyNodesForJump>c__AnonStorey1 storey = new <GetTargetFinallyNodesForJump>c__AnonStorey1 {
                    from = from,
                    to = to
                };
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = n => n.FinallyNode;
                }
                return this.Root.Walk(new Func<ExceptionSupport.Node, bool>(storey.<>m__0)).Reverse<ExceptionSupport.Node>().Select<ExceptionSupport.Node, ExceptionSupport.Node>(<>f__am$cache0);
            }

            private static bool IsTargetFaultNodeForJump(ExceptionSupport.Node node, int from, int to)
            {
                if (node.Type != ExceptionSupport.NodeType.Try)
                {
                    return false;
                }
                if (node.FaultNode == null)
                {
                    return false;
                }
                if (node.FaultNode.Handler.TryStart.Offset > from)
                {
                    return false;
                }
                if (node.FaultNode.Handler.TryEnd.Offset <= from)
                {
                    return false;
                }
                if (node.FaultNode.Handler.HandlerStart.Offset > to)
                {
                    return false;
                }
                return true;
            }

            private static bool IsTargetFinallyNodeForJump(ExceptionSupport.Node node, int from, int to)
            {
                if (node.Type != ExceptionSupport.NodeType.Try)
                {
                    return false;
                }
                if (node.FinallyNode == null)
                {
                    return false;
                }
                if (node.FinallyNode.Handler.TryStart.Offset > from)
                {
                    return false;
                }
                if (node.FinallyNode.Handler.TryEnd.Offset <= from)
                {
                    return false;
                }
                if (node.FinallyNode.Handler.HandlerStart.Offset > to)
                {
                    return false;
                }
                return true;
            }

            public override string ToString() => 
                $"{Enum.GetName(typeof(ExceptionSupport.NodeType), this._type)} children: {this._children.Length}, depth: {this.Depth}";

            [DebuggerHidden]
            private IEnumerable<ExceptionSupport.Node> Walk(Func<ExceptionSupport.Node, bool> filter) => 
                new <Walk>c__Iterator0 { 
                    filter = filter,
                    $this = this,
                    $PC = -2
                };

            internal InstructionBlock Block =>
                this._block;

            internal ExceptionSupport.Node[] CatchNodes
            {
                get
                {
                    if (this._type != ExceptionSupport.NodeType.Try)
                    {
                        throw new NotSupportedException("Cannot find the related finally handler for a non-try block");
                    }
                    List<ExceptionSupport.Node> list = new List<ExceptionSupport.Node>();
                    for (ExceptionSupport.Node node = this.NextSibling; (node != null) && (node.Type == ExceptionSupport.NodeType.Catch); node = node.NextSibling)
                    {
                        list.Add(node);
                    }
                    return list.ToArray();
                }
            }

            internal ExceptionSupport.Node[] Children =>
                this._children;

            internal int Depth
            {
                get
                {
                    int num = 0;
                    for (ExceptionSupport.Node node = this._parent; node != null; node = node.Parent)
                    {
                        num++;
                    }
                    return num;
                }
            }

            internal Instruction End
            {
                get
                {
                    if (this.Block != null)
                    {
                        return this.Block.Last;
                    }
                    if (this._children.Length == 0)
                    {
                        throw new NotSupportedException("Unsupported Node (" + this + ") with no children!");
                    }
                    return this._children[this._children.Length - 1].End;
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
                    if ((nextSibling == null) || (nextSibling.Type != ExceptionSupport.NodeType.Fault))
                    {
                        return null;
                    }
                    return nextSibling;
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
                    if ((nextSibling == null) || (nextSibling.Type != ExceptionSupport.NodeType.Finally))
                    {
                        return null;
                    }
                    return nextSibling;
                }
            }

            internal ExceptionHandler Handler =>
                this._handler;

            internal bool IsInCatchBlock
            {
                get
                {
                    ExceptionSupport.Node parent = this;
                    while ((parent != null) && (parent.Type != ExceptionSupport.NodeType.Root))
                    {
                        parent = parent.Parent;
                        if (parent.Type == ExceptionSupport.NodeType.Try)
                        {
                            return false;
                        }
                        if (parent.Type == ExceptionSupport.NodeType.Catch)
                        {
                            return true;
                        }
                        if (parent.Type == ExceptionSupport.NodeType.Finally)
                        {
                            return false;
                        }
                        if (parent.Type == ExceptionSupport.NodeType.Fault)
                        {
                            return false;
                        }
                    }
                    return false;
                }
            }

            internal bool IsInFaultBlock
            {
                get
                {
                    ExceptionSupport.Node parent = this;
                    while ((parent != null) && (parent.Type != ExceptionSupport.NodeType.Root))
                    {
                        parent = parent.Parent;
                        if (parent.Type == ExceptionSupport.NodeType.Try)
                        {
                            return false;
                        }
                        if (parent.Type == ExceptionSupport.NodeType.Catch)
                        {
                            return false;
                        }
                        if (parent.Type == ExceptionSupport.NodeType.Finally)
                        {
                            return false;
                        }
                        if (parent.Type == ExceptionSupport.NodeType.Fault)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }

            internal bool IsInFinallyBlock
            {
                get
                {
                    ExceptionSupport.Node parent = this;
                    while ((parent != null) && (parent.Type != ExceptionSupport.NodeType.Root))
                    {
                        parent = parent.Parent;
                        if (parent.Type == ExceptionSupport.NodeType.Try)
                        {
                            return false;
                        }
                        if (parent.Type == ExceptionSupport.NodeType.Catch)
                        {
                            return false;
                        }
                        if (parent.Type == ExceptionSupport.NodeType.Finally)
                        {
                            return true;
                        }
                        if (parent.Type == ExceptionSupport.NodeType.Fault)
                        {
                            return false;
                        }
                    }
                    return false;
                }
            }

            internal bool IsInTryBlock
            {
                get
                {
                    ExceptionSupport.Node parent = this;
                    while ((parent != null) && (parent.Type != ExceptionSupport.NodeType.Root))
                    {
                        parent = parent.Parent;
                        if (parent.Type == ExceptionSupport.NodeType.Try)
                        {
                            return true;
                        }
                        if (parent.Type == ExceptionSupport.NodeType.Catch)
                        {
                            return false;
                        }
                        if (parent.Type == ExceptionSupport.NodeType.Finally)
                        {
                            return false;
                        }
                        if (parent.Type == ExceptionSupport.NodeType.Fault)
                        {
                            return false;
                        }
                    }
                    return false;
                }
            }

            private ExceptionSupport.Node NextSibling
            {
                get
                {
                    if (this.Parent == null)
                    {
                        return null;
                    }
                    int index = Array.IndexOf<ExceptionSupport.Node>(this.Parent.Children, this);
                    if (index == (this.Parent.Children.Length - 1))
                    {
                        return null;
                    }
                    return this.Parent.Children[index + 1];
                }
            }

            internal ExceptionSupport.Node Parent =>
                this._parent;

            private ExceptionSupport.Node PrevSibling
            {
                get
                {
                    if (this.Parent == null)
                    {
                        return null;
                    }
                    int index = Array.IndexOf<ExceptionSupport.Node>(this.Parent.Children, this);
                    if (index == 0)
                    {
                        return null;
                    }
                    return this.Parent.Children[index - 1];
                }
            }

            private ExceptionSupport.Node Root
            {
                get
                {
                    ExceptionSupport.Node parent = this;
                    while ((parent != null) && (parent.Type != ExceptionSupport.NodeType.Root))
                    {
                        parent = parent.Parent;
                    }
                    return parent;
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

            internal ExceptionSupport.Node TryNode
            {
                get
                {
                    if (((this._type != ExceptionSupport.NodeType.Catch) && (this._type != ExceptionSupport.NodeType.Finally)) && (this._type != ExceptionSupport.NodeType.Fault))
                    {
                        throw new NotSupportedException("Cannot find the related try node for a non-handler block");
                    }
                    ExceptionSupport.Node prevSibling = this.PrevSibling;
                    while ((prevSibling != null) && (prevSibling.Type != ExceptionSupport.NodeType.Try))
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

            internal ExceptionSupport.NodeType Type =>
                this._type;

            [CompilerGenerated]
            private sealed class <GetTargetFinallyAndFaultNodesForJump>c__AnonStorey2
            {
                internal int from;
                internal int to;

                internal bool <>m__0(ExceptionSupport.Node node) => 
                    (ExceptionSupport.Node.IsTargetFinallyNodeForJump(node, this.from, this.to) || ExceptionSupport.Node.IsTargetFaultNodeForJump(node, this.from, this.to));
            }

            [CompilerGenerated]
            private sealed class <GetTargetFinallyNodesForJump>c__AnonStorey1
            {
                internal int from;
                internal int to;

                internal bool <>m__0(ExceptionSupport.Node node) => 
                    ExceptionSupport.Node.IsTargetFinallyNodeForJump(node, this.from, this.to);
            }

            [CompilerGenerated]
            private sealed class <Walk>c__Iterator0 : IEnumerable, IEnumerable<ExceptionSupport.Node>, IEnumerator, IDisposable, IEnumerator<ExceptionSupport.Node>
            {
                internal ExceptionSupport.Node $current;
                internal bool $disposing;
                internal ExceptionSupport.Node[] $locvar0;
                internal int $locvar1;
                internal int $PC;
                internal ExceptionSupport.Node $this;
                internal ExceptionSupport.Node <current>__1;
                internal Queue<ExceptionSupport.Node> <queue>__0;
                internal Func<ExceptionSupport.Node, bool> filter;

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
                            this.<queue>__0 = new Queue<ExceptionSupport.Node>();
                            this.<queue>__0.Enqueue(this.$this);
                            goto Label_00E5;

                        case 1:
                            break;

                        default:
                            goto Label_00FD;
                    }
                Label_008B:
                    this.$locvar0 = this.<current>__1.Children;
                    this.$locvar1 = 0;
                    while (this.$locvar1 < this.$locvar0.Length)
                    {
                        ExceptionSupport.Node item = this.$locvar0[this.$locvar1];
                        this.<queue>__0.Enqueue(item);
                        this.$locvar1++;
                    }
                Label_00E5:
                    if (this.<queue>__0.Count > 0)
                    {
                        this.<current>__1 = this.<queue>__0.Dequeue();
                        if (this.filter(this.<current>__1))
                        {
                            this.$current = this.<current>__1;
                            if (!this.$disposing)
                            {
                                this.$PC = 1;
                            }
                            return true;
                        }
                        goto Label_008B;
                    }
                    this.$PC = -1;
                Label_00FD:
                    return false;
                }

                [DebuggerHidden]
                public void Reset()
                {
                    throw new NotSupportedException();
                }

                [DebuggerHidden]
                IEnumerator<ExceptionSupport.Node> IEnumerable<ExceptionSupport.Node>.GetEnumerator()
                {
                    if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                    {
                        return this;
                    }
                    return new ExceptionSupport.Node.<Walk>c__Iterator0 { 
                        $this = this.$this,
                        filter = this.filter
                    };
                }

                [DebuggerHidden]
                IEnumerator IEnumerable.GetEnumerator() => 
                    this.System.Collections.Generic.IEnumerable<Unity.IL2CPP.ExceptionSupport.Node>.GetEnumerator();

                ExceptionSupport.Node IEnumerator<ExceptionSupport.Node>.Current =>
                    this.$current;

                object IEnumerator.Current =>
                    this.$current;
            }
        }

        internal enum NodeType
        {
            Try,
            Catch,
            Finally,
            Block,
            Root,
            Fault
        }
    }
}

