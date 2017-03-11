namespace Unity.IL2CPP.StackAnalysis
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common.CFG;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class StackAnalysis
    {
        private readonly ControlFlowGraph _cfg;
        private GlobalVariable[] _globalGlobalVariables;
        private readonly Dictionary<InstructionBlock, StackState> _ins = new Dictionary<InstructionBlock, StackState>();
        private readonly MethodDefinition _methodDefinition;
        private readonly Dictionary<InstructionBlock, StackState> _outs = new Dictionary<InstructionBlock, StackState>();
        private readonly Unity.IL2CPP.ILPreProcessor.TypeResolver _typeResolver;
        [CompilerGenerated]
        private static Func<KeyValuePair<InstructionBlock, StackState>, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<TypeReference, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<KeyValuePair<InstructionBlock, StackState>, InstructionBlock> <>f__am$cache10;
        [CompilerGenerated]
        private static Func<TypeReference, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<TypeReference, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<TypeReference, TypeReference> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<TypeReference, bool> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<TypeReference, bool> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<TypeReference, bool> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<TypeReference, bool> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<TypeReference, bool> <>f__am$cache9;
        [CompilerGenerated]
        private static Func<TypeReference, bool> <>f__am$cacheA;
        [CompilerGenerated]
        private static Func<TypeReference, TypeDefinition> <>f__am$cacheB;
        [CompilerGenerated]
        private static Func<TypeDefinition, bool> <>f__am$cacheC;
        [CompilerGenerated]
        private static Func<TypeReference, bool> <>f__am$cacheD;
        [CompilerGenerated]
        private static Func<TypeReference, bool> <>f__am$cacheE;
        [CompilerGenerated]
        private static Func<TypeReference, bool> <>f__am$cacheF;
        [Inject]
        public static ITypeProviderService TypeProvider;

        private StackAnalysis(MethodDefinition methodDefinition, ControlFlowGraph cfg, Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver)
        {
            this._methodDefinition = methodDefinition;
            this._cfg = cfg;
            this._typeResolver = typeResolver;
        }

        private void Analyze()
        {
            foreach (InstructionBlock block in this._cfg.Blocks)
            {
                StackState state;
                if (!this._ins.TryGetValue(block, out state))
                {
                    state = new StackState();
                }
                StackState other = StackStateBuilder.StackStateFor(block, state, this._methodDefinition, this._typeResolver);
                this._outs.Add(block, other.Clone());
                foreach (InstructionBlock block2 in block.Successors)
                {
                    if (!this._ins.ContainsKey(block2))
                    {
                        this._ins[block2] = new StackState();
                    }
                    this._ins[block2].Merge(other);
                }
            }
        }

        public static Unity.IL2CPP.StackAnalysis.StackAnalysis Analyze(MethodDefinition methodDefinition, ControlFlowGraph cfg, Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver)
        {
            Unity.IL2CPP.StackAnalysis.StackAnalysis analysis = new Unity.IL2CPP.StackAnalysis.StackAnalysis(methodDefinition, cfg, typeResolver);
            analysis.Analyze();
            return analysis;
        }

        private static InstructionBlock BlockFor(Dictionary<InstructionBlock, StackState> stackStates, Entry entry)
        {
            <BlockFor>c__AnonStorey3 storey = new <BlockFor>c__AnonStorey3 {
                entry = entry
            };
            if (<>f__am$cache10 == null)
            {
                <>f__am$cache10 = s => s.Key;
            }
            InstructionBlock block = stackStates.Where<KeyValuePair<InstructionBlock, StackState>>(new Func<KeyValuePair<InstructionBlock, StackState>, bool>(storey.<>m__0)).Select<KeyValuePair<InstructionBlock, StackState>, InstructionBlock>(<>f__am$cache10).FirstOrDefault<InstructionBlock>();
            if (block == null)
            {
                throw new ArgumentException("invalid Entry", "entry");
            }
            return block;
        }

        private GlobalVariable[] ComputeGlobalVariables()
        {
            List<GlobalVariable> list = new List<GlobalVariable>();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = e => !e.Value.IsEmpty;
            }
            using (IEnumerator<KeyValuePair<InstructionBlock, StackState>> enumerator = this._ins.Where<KeyValuePair<InstructionBlock, StackState>>(<>f__am$cache0).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    <ComputeGlobalVariables>c__AnonStorey1 storey = new <ComputeGlobalVariables>c__AnonStorey1 {
                        entry = enumerator.Current,
                        $this = this
                    };
                    <ComputeGlobalVariables>c__AnonStorey2 storey2 = new <ComputeGlobalVariables>c__AnonStorey2 {
                        <>f__ref$1 = storey,
                        index = 0
                    };
                    list.AddRange(storey.entry.Value.Entries.Select<Entry, GlobalVariable>(new Func<Entry, GlobalVariable>(storey2.<>m__0)));
                }
            }
            return list.ToArray();
        }

        private TypeReference ComputeType(Entry entry)
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = t => t.ContainsGenericParameters();
            }
            if (entry.Types.Any<TypeReference>(<>f__am$cache1))
            {
                throw new NotImplementedException();
            }
            if (entry.Types.Count == 1)
            {
                return entry.Types.Single<TypeReference>();
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = t => t.IsValueType();
            }
            if (entry.Types.Any<TypeReference>(<>f__am$cache2))
            {
                TypeReference widestValueType = StackAnalysisUtils.GetWidestValueType(entry.Types);
                if (widestValueType != null)
                {
                    return widestValueType;
                }
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = t => t.Resolve().IsEnum;
                }
                if (entry.Types.All<TypeReference>(<>f__am$cache3))
                {
                    if (<>f__am$cache4 == null)
                    {
                        <>f__am$cache4 = t => t.Resolve().GetUnderlyingEnumType();
                    }
                    widestValueType = StackAnalysisUtils.GetWidestValueType(entry.Types.Select<TypeReference, TypeReference>(<>f__am$cache4));
                    if (widestValueType != null)
                    {
                        return widestValueType;
                    }
                }
                if (<>f__am$cache5 == null)
                {
                    <>f__am$cache5 = t => t.IsValueType() || (t.MetadataType == MetadataType.Var);
                }
                if (entry.Types.All<TypeReference>(<>f__am$cache5))
                {
                    if (<>f__am$cache6 == null)
                    {
                        <>f__am$cache6 = t => t.MetadataType == MetadataType.Var;
                    }
                    return entry.Types.Single<TypeReference>(<>f__am$cache6);
                }
                if (<>f__am$cache7 == null)
                {
                    <>f__am$cache7 = t => t.IsSameType(TypeProvider.NativeUIntTypeReference);
                }
                if (entry.Types.Any<TypeReference>(<>f__am$cache7))
                {
                    return TypeProvider.NativeUIntTypeReference;
                }
                if (<>f__am$cache8 == null)
                {
                    <>f__am$cache8 = t => t.IsSameType(TypeProvider.NativeIntTypeReference);
                }
                if (!entry.Types.Any<TypeReference>(<>f__am$cache8))
                {
                    throw new NotImplementedException();
                }
                return TypeProvider.NativeIntTypeReference;
            }
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = t => t.IsSameType(TypeProvider.NativeIntTypeReference) || t.IsPointer;
            }
            if (entry.Types.All<TypeReference>(<>f__am$cache9))
            {
                return TypeProvider.NativeIntTypeReference;
            }
            if (<>f__am$cacheA == null)
            {
                <>f__am$cacheA = t => t.IsSameType(TypeProvider.NativeUIntTypeReference) || t.IsPointer;
            }
            if (entry.Types.All<TypeReference>(<>f__am$cacheA))
            {
                return TypeProvider.NativeUIntTypeReference;
            }
            if (<>f__am$cacheB == null)
            {
                <>f__am$cacheB = t => t.Resolve();
            }
            if (<>f__am$cacheC == null)
            {
                <>f__am$cacheC = res => (res != null) && res.IsInterface;
            }
            if (entry.Types.Select<TypeReference, TypeDefinition>(<>f__am$cacheB).Any<TypeDefinition>(<>f__am$cacheC))
            {
                if (<>f__am$cacheD == null)
                {
                    <>f__am$cacheD = t => t.Resolve().IsInterface;
                }
                return entry.Types.First<TypeReference>(<>f__am$cacheD);
            }
            if (entry.NullValue)
            {
                if (<>f__am$cacheE == null)
                {
                    <>f__am$cacheE = t => t.MetadataType != MetadataType.Object;
                }
                TypeReference reference3 = entry.Types.FirstOrDefault<TypeReference>(<>f__am$cacheE);
                if (reference3 != null)
                {
                    return reference3;
                }
                return entry.Types.First<TypeReference>();
            }
            if (<>f__am$cacheF == null)
            {
                <>f__am$cacheF = t => t.MetadataType == MetadataType.Object;
            }
            TypeReference reference4 = entry.Types.FirstOrDefault<TypeReference>(<>f__am$cacheF);
            if (reference4 != null)
            {
                return reference4;
            }
            return entry.Types.First<TypeReference>();
        }

        public GlobalVariable GlobalInputVariableFor(Entry entry) => 
            this.GlobaVariableFor(this._ins, entry);

        public GlobalVariable GlobalOutputVariableFor(Entry entry) => 
            this.GlobaVariableFor(this._outs, entry);

        public GlobalVariable[] GlobalsForBlock(int blockIndex)
        {
            <GlobalsForBlock>c__AnonStorey0 storey = new <GlobalsForBlock>c__AnonStorey0 {
                blockIndex = blockIndex
            };
            return this.Globals.Where<GlobalVariable>(new Func<GlobalVariable, bool>(storey.<>m__0)).ToArray<GlobalVariable>();
        }

        private GlobalVariable GlobaVariableFor(Dictionary<InstructionBlock, StackState> stackStates, Entry entry)
        {
            InstructionBlock block = BlockFor(stackStates, entry);
            return new GlobalVariable { 
                BlockIndex = block.Index,
                Index = StackIndexFor(stackStates, entry, block),
                Type = this.ComputeType(entry)
            };
        }

        public StackState InputStackStateFor(InstructionBlock block)
        {
            StackState state;
            if (!this._ins.TryGetValue(block, out state))
            {
                state = new StackState();
            }
            return state;
        }

        public GlobalVariable[] InputVariablesFor(InstructionBlock block) => 
            this.InputStackStateFor(block).Entries.Select<Entry, GlobalVariable>(new Func<Entry, GlobalVariable>(this.GlobalInputVariableFor)).ToArray<GlobalVariable>();

        public StackState OutputStackStateFor(InstructionBlock block)
        {
            StackState state;
            if (!this._outs.TryGetValue(block, out state))
            {
                state = new StackState();
            }
            return state;
        }

        public GlobalVariable[] OutputVariablesFor(InstructionBlock block) => 
            this.OutputStackStateFor(block).Entries.Select<Entry, GlobalVariable>(new Func<Entry, GlobalVariable>(this.GlobalOutputVariableFor)).ToArray<GlobalVariable>();

        private static int StackIndexFor(IDictionary<InstructionBlock, StackState> stackStates, Entry entry, InstructionBlock block)
        {
            StackState state = stackStates[block];
            int num = 0;
            foreach (Entry entry2 in state.Entries)
            {
                if (entry2 == entry)
                {
                    return num;
                }
                num++;
            }
            throw new ArgumentException("invalid Entry", "entry");
        }

        public GlobalVariable[] Globals
        {
            get
            {
                GlobalVariable[] variableArray2;
                if (this._globalGlobalVariables != null)
                {
                    variableArray2 = this._globalGlobalVariables;
                }
                else
                {
                    variableArray2 = this._globalGlobalVariables = this.ComputeGlobalVariables();
                }
                return variableArray2;
            }
        }

        [CompilerGenerated]
        private sealed class <BlockFor>c__AnonStorey3
        {
            internal Entry entry;

            internal bool <>m__0(KeyValuePair<InstructionBlock, StackState> s) => 
                s.Value.Entries.Contains(this.entry);
        }

        [CompilerGenerated]
        private sealed class <ComputeGlobalVariables>c__AnonStorey1
        {
            internal Unity.IL2CPP.StackAnalysis.StackAnalysis $this;
            internal KeyValuePair<InstructionBlock, StackState> entry;
        }

        [CompilerGenerated]
        private sealed class <ComputeGlobalVariables>c__AnonStorey2
        {
            internal Unity.IL2CPP.StackAnalysis.StackAnalysis.<ComputeGlobalVariables>c__AnonStorey1 <>f__ref$1;
            internal int index;

            internal GlobalVariable <>m__0(Entry item) => 
                new GlobalVariable { 
                    BlockIndex = this.<>f__ref$1.entry.Key.Index,
                    Index = this.index++,
                    Type = this.<>f__ref$1.$this.ComputeType(item)
                };
        }

        [CompilerGenerated]
        private sealed class <GlobalsForBlock>c__AnonStorey0
        {
            internal int blockIndex;

            internal bool <>m__0(GlobalVariable v) => 
                (v.BlockIndex == this.blockIndex);
        }
    }
}

