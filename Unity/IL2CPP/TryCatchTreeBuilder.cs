namespace Unity.IL2CPP
{
    using Mono.Cecil.Cil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.Common.CFG;

    internal class TryCatchTreeBuilder
    {
        private readonly InstructionBlock[] _blocks;
        private readonly Stack<Context> _contextStack = new Stack<Context>();
        private readonly MethodBody _methodBody;
        private readonly Dictionary<Instruction, TryCatchInfo> _tryCatchInfos;

        public TryCatchTreeBuilder(MethodBody methodBody, InstructionBlock[] blocks, Dictionary<Instruction, TryCatchInfo> tryCatchInfos)
        {
            this._methodBody = methodBody;
            this._blocks = blocks;
            this._tryCatchInfos = tryCatchInfos;
        }

        private static InstructionBlock BlockFor(Context context) => 
            ((context.Type != ContextType.Block) ? null : context.Block);

        internal ExceptionSupport.Node Build() => 
            (!this._methodBody.HasExceptionHandlers ? this.BuildTreeWithNoExceptionHandlers() : this.BuildTreeWithExceptionHandlers());

        private ExceptionSupport.Node BuildTreeWithExceptionHandlers()
        {
            Context item = new Context {
                Type = ContextType.Root
            };
            this._contextStack.Push(item);
            foreach (InstructionBlock block in this._blocks)
            {
                <BuildTreeWithExceptionHandlers>c__AnonStorey0 storey = new <BuildTreeWithExceptionHandlers>c__AnonStorey0();
                if (block.Last.Next == null)
                {
                    item = new Context {
                        Type = ContextType.Block,
                        Block = block
                    };
                    this._contextStack.Peek().Children.Add(item);
                    break;
                }
                storey.firstInstr = block.First;
                Instruction next = block.Last.Next;
                TryCatchInfo info = this._tryCatchInfos[storey.firstInstr];
                TryCatchInfo info2 = this._tryCatchInfos[next];
                if ((info.CatchStart != 0) && (info.FinallyStart != 0))
                {
                    throw new NotSupportedException("An instruction cannot start both a catch and a finally block!");
                }
                for (int i = 0; i < info.FinallyStart; i++)
                {
                    item = new Context {
                        Type = ContextType.Finally,
                        Handler = this._methodBody.ExceptionHandlers.Single<ExceptionHandler>(new Func<ExceptionHandler, bool>(storey.<>m__0))
                    };
                    this._contextStack.Push(item);
                }
                for (int j = 0; j < info.FaultStart; j++)
                {
                    item = new Context {
                        Type = ContextType.Fault,
                        Handler = this._methodBody.ExceptionHandlers.Single<ExceptionHandler>(new Func<ExceptionHandler, bool>(storey.<>m__1))
                    };
                    this._contextStack.Push(item);
                }
                for (int k = 0; k < info.FilterStart; k++)
                {
                    item = new Context {
                        Type = ContextType.Filter,
                        Handler = this._methodBody.ExceptionHandlers.Single<ExceptionHandler>(new Func<ExceptionHandler, bool>(storey.<>m__2))
                    };
                    this._contextStack.Push(item);
                }
                for (int m = 0; m < info.CatchStart; m++)
                {
                    item = new Context {
                        Type = ContextType.Catch,
                        Handler = this._methodBody.ExceptionHandlers.Single<ExceptionHandler>(new Func<ExceptionHandler, bool>(storey.<>m__3))
                    };
                    this._contextStack.Push(item);
                }
                for (int n = 0; n < info.TryStart; n++)
                {
                    item = new Context {
                        Type = ContextType.Try
                    };
                    this._contextStack.Push(item);
                }
                item = new Context {
                    Type = ContextType.Block,
                    Block = block
                };
                this._contextStack.Peek().Children.Add(item);
                for (int num7 = 0; num7 < info2.FinallyEnd; num7++)
                {
                    Context context2 = this._contextStack.Pop();
                    item = new Context {
                        Type = ContextType.Finally,
                        Children = context2.Children,
                        Handler = context2.Handler
                    };
                    this._contextStack.Peek().Children.Add(item);
                }
                for (int num8 = 0; num8 < info2.FaultEnd; num8++)
                {
                    Context context3 = this._contextStack.Pop();
                    item = new Context {
                        Type = ContextType.Fault,
                        Children = context3.Children,
                        Handler = context3.Handler
                    };
                    this._contextStack.Peek().Children.Add(item);
                }
                for (int num9 = 0; num9 < info2.FilterEnd; num9++)
                {
                    Context context4 = this._contextStack.Pop();
                    item = new Context {
                        Type = ContextType.Filter,
                        Children = context4.Children,
                        Handler = context4.Handler
                    };
                    this._contextStack.Peek().Children.Add(item);
                }
                for (int num10 = 0; num10 < info2.CatchEnd; num10++)
                {
                    Context context5 = this._contextStack.Pop();
                    item = new Context {
                        Type = ContextType.Catch,
                        Children = context5.Children,
                        Handler = context5.Handler
                    };
                    this._contextStack.Peek().Children.Add(item);
                }
                for (int num11 = 0; num11 < info2.TryEnd; num11++)
                {
                    Context context6 = this._contextStack.Pop();
                    item = new Context {
                        Type = ContextType.Try,
                        Children = context6.Children
                    };
                    this._contextStack.Peek().Children.Add(item);
                }
            }
            if (this._contextStack.Count > 1)
            {
                throw new NotSupportedException("Mismatched context depth when building try/catch tree!");
            }
            return MergeAndBuildRootNode(this._contextStack.Pop());
        }

        private ExceptionSupport.Node BuildTreeWithNoExceptionHandlers()
        {
            int num = 0;
            ExceptionSupport.Node[] children = new ExceptionSupport.Node[this._blocks.Length];
            foreach (InstructionBlock block in this._blocks)
            {
                children[num++] = new ExceptionSupport.Node(ExceptionSupport.NodeType.Block, block);
            }
            return MakeRoot(children);
        }

        private static ExceptionHandler ExceptionHandlerFor(Context context)
        {
            switch (context.Type)
            {
                case ContextType.Filter:
                case ContextType.Catch:
                case ContextType.Finally:
                case ContextType.Fault:
                    return context.Handler;
            }
            return null;
        }

        private static ExceptionSupport.Node MakeRoot(ExceptionSupport.Node[] children) => 
            new ExceptionSupport.Node(null, ExceptionSupport.NodeType.Root, null, children, null);

        private static ExceptionSupport.Node MergeAndBuildRootNode(Context context)
        {
            int num = 0;
            ExceptionSupport.Node[] children = new ExceptionSupport.Node[context.Children.Count];
            foreach (Context context2 in context.Children)
            {
                children[num++] = MergeAndBuildRootNodeRecursive(context2);
            }
            return MakeRoot(children);
        }

        private static ExceptionSupport.Node MergeAndBuildRootNodeRecursive(Context context)
        {
            int num = 0;
            ExceptionSupport.Node[] children = new ExceptionSupport.Node[context.Children.Count];
            foreach (Context context2 in context.Children)
            {
                children[num++] = MergeAndBuildRootNodeRecursive(context2);
            }
            if (children.Length == 1)
            {
                ExceptionSupport.Node node = children[0];
                if (node.Type == ExceptionSupport.NodeType.Block)
                {
                    return new ExceptionSupport.Node(null, NodeTypeFor(context), node.Block, new ExceptionSupport.Node[0], ExceptionHandlerFor(context));
                }
            }
            return new ExceptionSupport.Node(null, NodeTypeFor(context), BlockFor(context), children, ExceptionHandlerFor(context));
        }

        private static ExceptionSupport.NodeType NodeTypeFor(Context context)
        {
            switch (context.Type)
            {
                case ContextType.Root:
                    return ExceptionSupport.NodeType.Root;

                case ContextType.Try:
                    return ExceptionSupport.NodeType.Try;

                case ContextType.Filter:
                    return ExceptionSupport.NodeType.Filter;

                case ContextType.Catch:
                    return ExceptionSupport.NodeType.Catch;

                case ContextType.Finally:
                    return ExceptionSupport.NodeType.Finally;

                case ContextType.Fault:
                    return ExceptionSupport.NodeType.Fault;
            }
            return ExceptionSupport.NodeType.Block;
        }

        [CompilerGenerated]
        private sealed class <BuildTreeWithExceptionHandlers>c__AnonStorey0
        {
            internal Instruction firstInstr;

            internal bool <>m__0(ExceptionHandler h) => 
                ((h.HandlerType == ExceptionHandlerType.Finally) && (h.HandlerStart == this.firstInstr));

            internal bool <>m__1(ExceptionHandler h) => 
                ((h.HandlerType == ExceptionHandlerType.Fault) && (h.HandlerStart == this.firstInstr));

            internal bool <>m__2(ExceptionHandler h) => 
                ((h.HandlerType == ExceptionHandlerType.Filter) && (h.FilterStart == this.firstInstr));

            internal bool <>m__3(ExceptionHandler h) => 
                (((h.HandlerType == ExceptionHandlerType.Catch) || (h.HandlerType == ExceptionHandlerType.Filter)) && (h.HandlerStart == this.firstInstr));
        }

        internal class Context
        {
            public InstructionBlock Block;
            public List<TryCatchTreeBuilder.Context> Children = new List<TryCatchTreeBuilder.Context>();
            public ExceptionHandler Handler;
            public TryCatchTreeBuilder.ContextType Type;
        }

        internal enum ContextType
        {
            Root,
            Block,
            Try,
            Filter,
            Catch,
            Finally,
            Fault
        }
    }
}

