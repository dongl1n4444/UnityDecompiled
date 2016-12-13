using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IL2CPP.Common.CFG;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.StackAnalysis
{
	public class StackAnalysis
	{
		[Inject]
		public static ITypeProviderService TypeProvider;

		private readonly TypeResolver _typeResolver;

		private readonly ControlFlowGraph _cfg;

		private readonly MethodDefinition _methodDefinition;

		private readonly Dictionary<InstructionBlock, StackState> _ins = new Dictionary<InstructionBlock, StackState>();

		private readonly Dictionary<InstructionBlock, StackState> _outs = new Dictionary<InstructionBlock, StackState>();

		private GlobalVariable[] _globalGlobalVariables;

		public GlobalVariable[] Globals
		{
			get
			{
				GlobalVariable[] arg_1D_0;
				if ((arg_1D_0 = this._globalGlobalVariables) == null)
				{
					arg_1D_0 = (this._globalGlobalVariables = this.ComputeGlobalVariables());
				}
				return arg_1D_0;
			}
		}

		private StackAnalysis(MethodDefinition methodDefinition, ControlFlowGraph cfg, TypeResolver typeResolver)
		{
			this._methodDefinition = methodDefinition;
			this._cfg = cfg;
			this._typeResolver = typeResolver;
		}

		public static StackAnalysis Analyze(MethodDefinition methodDefinition, ControlFlowGraph cfg, TypeResolver typeResolver)
		{
			StackAnalysis stackAnalysis = new StackAnalysis(methodDefinition, cfg, typeResolver);
			stackAnalysis.Analyze();
			return stackAnalysis;
		}

		public GlobalVariable[] GlobalsForBlock(int blockIndex)
		{
			return (from v in this.Globals
			where v.BlockIndex == blockIndex
			select v).ToArray<GlobalVariable>();
		}

		private void Analyze()
		{
			InstructionBlock[] blocks = this._cfg.Blocks;
			for (int i = 0; i < blocks.Length; i++)
			{
				InstructionBlock instructionBlock = blocks[i];
				StackState initialState;
				if (!this._ins.TryGetValue(instructionBlock, out initialState))
				{
					initialState = new StackState();
				}
				StackState stackState = StackStateBuilder.StackStateFor(instructionBlock, initialState, this._methodDefinition, this._typeResolver);
				this._outs.Add(instructionBlock, stackState.Clone());
				foreach (InstructionBlock current in instructionBlock.Successors)
				{
					if (!this._ins.ContainsKey(current))
					{
						this._ins[current] = new StackState();
					}
					this._ins[current].Merge(stackState);
				}
			}
		}

		public StackState InputStackStateFor(InstructionBlock block)
		{
			StackState result;
			if (!this._ins.TryGetValue(block, out result))
			{
				result = new StackState();
			}
			return result;
		}

		public StackState OutputStackStateFor(InstructionBlock block)
		{
			StackState result;
			if (!this._outs.TryGetValue(block, out result))
			{
				result = new StackState();
			}
			return result;
		}

		public GlobalVariable GlobalInputVariableFor(Entry entry)
		{
			return this.GlobaVariableFor(this._ins, entry);
		}

		public GlobalVariable GlobalOutputVariableFor(Entry entry)
		{
			return this.GlobaVariableFor(this._outs, entry);
		}

		public GlobalVariable[] InputVariablesFor(InstructionBlock block)
		{
			return this.InputStackStateFor(block).Entries.Select(new Func<Entry, GlobalVariable>(this.GlobalInputVariableFor)).ToArray<GlobalVariable>();
		}

		public GlobalVariable[] OutputVariablesFor(InstructionBlock block)
		{
			return this.OutputStackStateFor(block).Entries.Select(new Func<Entry, GlobalVariable>(this.GlobalOutputVariableFor)).ToArray<GlobalVariable>();
		}

		private GlobalVariable[] ComputeGlobalVariables()
		{
			List<GlobalVariable> list = new List<GlobalVariable>();
			using (IEnumerator<KeyValuePair<InstructionBlock, StackState>> enumerator = (from e in this._ins
			where !e.Value.IsEmpty
			select e).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					StackAnalysis.<ComputeGlobalVariables>c__AnonStorey1 <ComputeGlobalVariables>c__AnonStorey = new StackAnalysis.<ComputeGlobalVariables>c__AnonStorey1();
					<ComputeGlobalVariables>c__AnonStorey.entry = enumerator.Current;
					<ComputeGlobalVariables>c__AnonStorey.$this = this;
					int index = 0;
					list.AddRange(from item in <ComputeGlobalVariables>c__AnonStorey.entry.Value.Entries
					select new GlobalVariable
					{
						BlockIndex = <ComputeGlobalVariables>c__AnonStorey.entry.Key.Index,
						Index = index++,
						Type = <ComputeGlobalVariables>c__AnonStorey.$this.ComputeType(item)
					});
				}
			}
			return list.ToArray();
		}

		private TypeReference ComputeType(Entry entry)
		{
			if (entry.Types.Any((TypeReference t) => t.ContainsGenericParameters()))
			{
				throw new NotImplementedException();
			}
			TypeReference result;
			if (entry.Types.Count == 1)
			{
				result = entry.Types.Single<TypeReference>();
			}
			else if (entry.Types.Any((TypeReference t) => t.IsValueType()))
			{
				TypeReference widestValueType = StackAnalysisUtils.GetWidestValueType(entry.Types);
				if (widestValueType != null)
				{
					result = widestValueType;
				}
				else
				{
					if (entry.Types.All((TypeReference t) => t.Resolve().IsEnum))
					{
						widestValueType = StackAnalysisUtils.GetWidestValueType(from t in entry.Types
						select t.Resolve().GetUnderlyingEnumType());
						if (widestValueType != null)
						{
							result = widestValueType;
							return result;
						}
					}
					if (entry.Types.All((TypeReference t) => t.IsValueType() || t.MetadataType == MetadataType.Var))
					{
						result = entry.Types.Single((TypeReference t) => t.MetadataType == MetadataType.Var);
					}
					else if (entry.Types.Any((TypeReference t) => t.IsSameType(StackAnalysis.TypeProvider.NativeUIntTypeReference)))
					{
						result = StackAnalysis.TypeProvider.NativeUIntTypeReference;
					}
					else
					{
						if (!entry.Types.Any((TypeReference t) => t.IsSameType(StackAnalysis.TypeProvider.NativeIntTypeReference)))
						{
							throw new NotImplementedException();
						}
						result = StackAnalysis.TypeProvider.NativeIntTypeReference;
					}
				}
			}
			else if (entry.Types.All((TypeReference t) => t.IsSameType(StackAnalysis.TypeProvider.NativeIntTypeReference) || t.IsPointer))
			{
				result = StackAnalysis.TypeProvider.NativeIntTypeReference;
			}
			else if (entry.Types.All((TypeReference t) => t.IsSameType(StackAnalysis.TypeProvider.NativeUIntTypeReference) || t.IsPointer))
			{
				result = StackAnalysis.TypeProvider.NativeUIntTypeReference;
			}
			else if ((from t in entry.Types
			select t.Resolve()).Any((TypeDefinition res) => res != null && res.IsInterface))
			{
				result = entry.Types.First((TypeReference t) => t.Resolve().IsInterface);
			}
			else if (entry.NullValue)
			{
				TypeReference typeReference = entry.Types.FirstOrDefault((TypeReference t) => t.MetadataType != MetadataType.Object);
				if (typeReference != null)
				{
					result = typeReference;
				}
				else
				{
					result = entry.Types.First<TypeReference>();
				}
			}
			else
			{
				TypeReference typeReference2 = entry.Types.FirstOrDefault((TypeReference t) => t.MetadataType == MetadataType.Object);
				if (typeReference2 != null)
				{
					result = typeReference2;
				}
				else
				{
					result = entry.Types.First<TypeReference>();
				}
			}
			return result;
		}

		private GlobalVariable GlobaVariableFor(Dictionary<InstructionBlock, StackState> stackStates, Entry entry)
		{
			InstructionBlock instructionBlock = StackAnalysis.BlockFor(stackStates, entry);
			return new GlobalVariable
			{
				BlockIndex = instructionBlock.Index,
				Index = StackAnalysis.StackIndexFor(stackStates, entry, instructionBlock),
				Type = this.ComputeType(entry)
			};
		}

		private static int StackIndexFor(IDictionary<InstructionBlock, StackState> stackStates, Entry entry, InstructionBlock block)
		{
			StackState stackState = stackStates[block];
			int num = 0;
			foreach (Entry current in stackState.Entries)
			{
				if (current == entry)
				{
					return num;
				}
				num++;
			}
			throw new ArgumentException("invalid Entry", "entry");
		}

		private static InstructionBlock BlockFor(Dictionary<InstructionBlock, StackState> stackStates, Entry entry)
		{
			InstructionBlock instructionBlock = (from s in stackStates
			where s.Value.Entries.Contains(entry)
			select s.Key).FirstOrDefault<InstructionBlock>();
			if (instructionBlock == null)
			{
				throw new ArgumentException("invalid Entry", "entry");
			}
			return instructionBlock;
		}
	}
}
