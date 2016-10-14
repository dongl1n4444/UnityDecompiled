using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.Common.CFG;
using Unity.IL2CPP.Debugger;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Metadata;
using Unity.IL2CPP.StackAnalysis;

namespace Unity.IL2CPP
{
	public class MethodBodyWriter
	{
		private enum OverflowCheck
		{
			None,
			Signed,
			Unsigned
		}

		private enum Signedness
		{
			Signed,
			Unsigned
		}

		private const string VariableNameForIntefaceOffsetInDirectVirtualCall = "il2cpp_interface_offset_";

		private const string VariableNameForTypeInfoInConstrainedCall = "il2cpp_this_typeinfo";

		private int _tempIndex;

		private readonly SharingType _sharingType;

		private readonly Labeler _labeler;

		private readonly CppCodeWriter _writer;

		private readonly ControlFlowGraph _cfg;

		private readonly TypeResolver _typeResolver;

		private readonly MethodReference _methodReference;

		private readonly MethodDefinition _methodDefinition;

		private readonly MethodBodyWriterDebugOptions _options;

		private readonly StackAnalysis _stackAnalysis;

		private readonly IRuntimeMetadataAccess _runtimeMetadataAccess;

		private readonly ArrayBoundsCheckSupport _arrayBoundsCheckSupport;

		private readonly DivideByZeroCheckSupport _divideByZeroCheckSupport;

		private readonly VTableBuilder _vTableBuilder;

		private readonly Stack<StackInfo> _valueStack = new Stack<StackInfo>();

		private readonly HashSet<Instruction> _emittedLabels = new HashSet<Instruction>();

		private readonly HashSet<Instruction> _referencedLabels = new HashSet<Instruction>();

		private readonly HashSet<TypeReference> _classesAlreadyInitializedInBlock = new HashSet<TypeReference>(new TypeReferenceEqualityComparer());

		private bool _thisInstructionIsVolatile = false;

		private ExceptionSupport _exceptionSupport;

		private NullChecksSupport _nullCheckSupport;

		private TypeReference _constrainedCallThisType;

		[Inject]
		public static IStatsService StatsService;

		[Inject]
		public static ITypeProviderService TypeProvider;

		[Inject]
		public static IGenericSharingAnalysisService GenericSharingAnalysis;

		[Inject]
		public static INamingService Naming;

		[CompilerGenerated]
		private static Func<MethodDefinition, bool> <>f__mg$cache0;

		private TypeReference Int16TypeReference
		{
			get
			{
				return MethodBodyWriter.TypeProvider.Int16TypeReference;
			}
		}

		private TypeReference UInt16TypeReference
		{
			get
			{
				return MethodBodyWriter.TypeProvider.UInt16TypeReference;
			}
		}

		private TypeReference Int32TypeReference
		{
			get
			{
				return MethodBodyWriter.TypeProvider.Int32TypeReference;
			}
		}

		private TypeReference SByteTypeReference
		{
			get
			{
				return MethodBodyWriter.TypeProvider.SByteTypeReference;
			}
		}

		private TypeReference ByteTypeReference
		{
			get
			{
				return MethodBodyWriter.TypeProvider.ByteTypeReference;
			}
		}

		private TypeReference IntPtrTypeReference
		{
			get
			{
				return MethodBodyWriter.TypeProvider.IntPtrTypeReference;
			}
		}

		private TypeReference UIntPtrTypeReference
		{
			get
			{
				return MethodBodyWriter.TypeProvider.UIntPtrTypeReference;
			}
		}

		private TypeReference Int64TypeReference
		{
			get
			{
				return MethodBodyWriter.TypeProvider.Int64TypeReference;
			}
		}

		private TypeReference UInt32TypeReference
		{
			get
			{
				return MethodBodyWriter.TypeProvider.UInt32TypeReference;
			}
		}

		private TypeReference UInt64TypeReference
		{
			get
			{
				return MethodBodyWriter.TypeProvider.UInt64TypeReference;
			}
		}

		private TypeReference SingleTypeReference
		{
			get
			{
				return MethodBodyWriter.TypeProvider.SingleTypeReference;
			}
		}

		private TypeReference DoubleTypeReference
		{
			get
			{
				return MethodBodyWriter.TypeProvider.DoubleTypeReference;
			}
		}

		private TypeReference ObjectTypeReference
		{
			get
			{
				return MethodBodyWriter.TypeProvider.ObjectTypeReference;
			}
		}

		private TypeReference StringTypeReference
		{
			get
			{
				return MethodBodyWriter.TypeProvider.StringTypeReference;
			}
		}

		private TypeReference NativeIntTypeReference
		{
			get
			{
				return MethodBodyWriter.TypeProvider.NativeIntTypeReference;
			}
		}

		private TypeReference NativeUIntTypeReference
		{
			get
			{
				return MethodBodyWriter.TypeProvider.NativeUIntTypeReference;
			}
		}

		private TypeReference RuntimeTypeHandleTypeReference
		{
			get
			{
				return MethodBodyWriter.TypeProvider.RuntimeTypeHandleTypeReference;
			}
		}

		private TypeReference RuntimeMethodHandleTypeReference
		{
			get
			{
				return MethodBodyWriter.TypeProvider.RuntimeMethodHandleTypeReference;
			}
		}

		private TypeReference RuntimeFieldHandleTypeReference
		{
			get
			{
				return MethodBodyWriter.TypeProvider.RuntimeFieldHandleTypeReference;
			}
		}

		public MethodBodyWriter(CppCodeWriter writer, MethodReference methodReference, TypeResolver typeResolver, IRuntimeMetadataAccess metadataAccess, VTableBuilder vTableBuilder) : this(writer, methodReference, typeResolver, metadataAccess, vTableBuilder, new MethodBodyWriterDebugOptions())
		{
		}

		private MethodBodyWriter(CppCodeWriter writer, MethodReference methodReference, TypeResolver typeResolver, IRuntimeMetadataAccess metadataAccess, VTableBuilder vTableBuilder, MethodBodyWriterDebugOptions options)
		{
			this._methodReference = methodReference;
			this._methodDefinition = methodReference.Resolve();
			this._nullCheckSupport = new NullChecksSupport(writer, this._methodDefinition, CodeGenOptions.EmitNullChecks);
			this._arrayBoundsCheckSupport = new ArrayBoundsCheckSupport(writer, this._methodDefinition, CodeGenOptions.EnableArrayBoundsCheck);
			this._divideByZeroCheckSupport = new DivideByZeroCheckSupport(writer, this._methodDefinition, CodeGenOptions.EnableDivideByZeroCheck);
			this._cfg = ControlFlowGraph.Create(this._methodDefinition);
			this._writer = writer;
			this._typeResolver = typeResolver;
			this._vTableBuilder = vTableBuilder;
			this._options = options;
			this._stackAnalysis = StackAnalysis.Analyze(this._methodDefinition, this._cfg, this._typeResolver);
			DeadBlockAnalysis.MarkBlocksDeadIfNeeded(this._cfg.Blocks);
			this._labeler = new Labeler(this._methodDefinition);
			this._runtimeMetadataAccess = metadataAccess;
			this._sharingType = ((!MethodBodyWriter.GenericSharingAnalysis.IsSharedMethod(methodReference)) ? SharingType.NonShared : SharingType.Shared);
		}

		public void Generate()
		{
			if (this._methodDefinition.HasBody)
			{
				if (GenericsUtilities.CheckForMaximumRecursion(this._methodReference.DeclaringType as GenericInstanceType))
				{
					this._writer.WriteStatement(Emit.RaiseManagedException("il2cpp_codegen_get_maximum_nested_generics_exception()"));
				}
				else
				{
					this.WriteLocalVariables();
					DebuggerSupportFactory.GetDebuggerSupport().WriteCallStackInformation(this._writer, this._methodReference, this.ResolveLocalVariableTypes(), this._runtimeMetadataAccess);
					this._exceptionSupport = new ExceptionSupport(this._methodDefinition, this._cfg.Blocks, this._writer);
					this._exceptionSupport.Prepare();
					this.CollectUsedLabels();
					foreach (ExceptionHandler current in this._methodDefinition.Body.ExceptionHandlers)
					{
						if (current.CatchType != null)
						{
							this._writer.AddIncludeForTypeDefinition(this._typeResolver.Resolve(current.CatchType));
						}
					}
					GlobalVariable[] globals = this._stackAnalysis.Globals;
					for (int i = 0; i < globals.Length; i++)
					{
						GlobalVariable globalVariable = globals[i];
						this._writer.WriteVariable(this._typeResolver.Resolve(globalVariable.Type, true), globalVariable.VariableName);
					}
					ExceptionSupport.Node[] children = this._exceptionSupport.FlowTree.Children;
					for (int j = 0; j < children.Length; j++)
					{
						ExceptionSupport.Node node = children[j];
						this.GenerateCodeRecursive(node);
					}
				}
			}
		}

		private void WriteLocalVariables()
		{
			foreach (KeyValuePair<string, TypeReference> current in this.ResolveLocalVariableTypes())
			{
				this._writer.AddIncludeForTypeDefinition(current.Value);
				this._writer.WriteVariable(current.Value, current.Key);
			}
		}

		private IEnumerable<KeyValuePair<string, TypeReference>> ResolveLocalVariableTypes()
		{
			return from v in this._methodDefinition.Body.Variables
			select new KeyValuePair<string, TypeReference>(MethodBodyWriter.Naming.ForVariableName(v), this._typeResolver.Resolve(v.VariableType));
		}

		private void CollectUsedLabels()
		{
			foreach (InstructionBlock current in from block in this._cfg.Blocks
			where block.IsBranchTarget
			select block)
			{
				this._referencedLabels.Add(current.First);
			}
			ExceptionSupport.Node[] children = this._exceptionSupport.FlowTree.Children;
			for (int i = 0; i < children.Length; i++)
			{
				ExceptionSupport.Node node = children[i];
				this.RecursivelyAddLabelsForExceptionNodes(node);
			}
		}

		private void RecursivelyAddLabelsForExceptionNodes(ExceptionSupport.Node node)
		{
			if (node.Type == ExceptionSupport.NodeType.Try || node.Type == ExceptionSupport.NodeType.Catch || node.Type == ExceptionSupport.NodeType.Finally || node.Type == ExceptionSupport.NodeType.Fault)
			{
				if (node.Block != null)
				{
					this._referencedLabels.Add(node.Block.First);
				}
				ExceptionSupport.Node[] children = node.Children;
				for (int i = 0; i < children.Length; i++)
				{
					ExceptionSupport.Node node2 = children[i];
					if (node2.Block != null)
					{
						this._referencedLabels.Add(node2.Block.First);
					}
					this.RecursivelyAddLabelsForExceptionNodes(node2);
				}
			}
		}

		private void GenerateCodeRecursive(ExceptionSupport.Node node)
		{
			InstructionBlock block = node.Block;
			if (block != null)
			{
				if (node.Children.Length > 0)
				{
					throw new NotSupportedException("Node with explicit Block should have no children!");
				}
				if (block.IsDead)
				{
					this.WriteComment("Dead block : {0}", new object[]
					{
						block.First.ToString()
					});
				}
				else
				{
					this._valueStack.Clear();
					foreach (GlobalVariable current in (from v in this._stackAnalysis.InputVariablesFor(block)
					orderby v.Index
					select v).Reverse<GlobalVariable>())
					{
						this._valueStack.Push(new StackInfo(current.VariableName, this._typeResolver.Resolve(current.Type, true)));
					}
					this._exceptionSupport.PushExceptionOnStackIfNeeded(node, this._valueStack, this._typeResolver);
					if (this._options.EmitBlockInfo)
					{
						this.WriteComment("BLOCK: {0}", new object[]
						{
							block.Index
						});
					}
					if (this._options.EmitInputAndOutputs)
					{
						this.DumpInsFor(block);
					}
					Instruction instruction = block.First;
					this.WriteLabelForBranchTarget(instruction);
					this.EnterNode(node);
					this._classesAlreadyInitializedInBlock.Clear();
					while (true)
					{
						if (instruction.SequencePoint != null)
						{
							if (instruction.SequencePoint.StartLine != 16707566 && this._options.EmitLineNumbers)
							{
								this._writer.WriteUnindented("#line {0} \"{1}\"", new object[]
								{
									instruction.SequencePoint.StartLine,
									instruction.SequencePoint.Document.Url.Replace("\\", "\\\\")
								});
							}
						}
						if (this._options.EmitIlCode)
						{
							this._writer.WriteUnindented("/* {0} */", new object[]
							{
								instruction
							});
						}
						this.ProcessInstruction(node, block, ref instruction);
						this.ProcessInstructionOperand(instruction.Operand);
						if (instruction.Next == null || instruction == block.Last)
						{
							break;
						}
						instruction = instruction.Next;
					}
					if (instruction.OpCode.Code < Code.Br_S || instruction.OpCode.Code > Code.Blt_Un)
					{
						if (block.Successors.Any<InstructionBlock>() && instruction.OpCode.Code != Code.Switch)
						{
							this.SetupFallthroughVariables(block);
						}
					}
					if (this._options.EmitInputAndOutputs)
					{
						this.DumpOutsFor(block);
					}
					if (this._options.EmitBlockInfo)
					{
						if (block.Successors.Any<InstructionBlock>())
						{
							string arg_333_1 = "END BLOCK {0} (succ: {1})";
							object[] expr_2F5 = new object[2];
							expr_2F5[0] = block.Index;
							expr_2F5[1] = (from b in block.Successors
							select b.Index.ToString()).AggregateWithComma();
							this.WriteComment(arg_333_1, expr_2F5);
						}
						else
						{
							this.WriteComment("END BLOCK {0} (succ: none)", new object[]
							{
								block.Index
							});
						}
						this._writer.WriteLine();
						this._writer.WriteLine();
					}
					this.ExitNode(node);
				}
			}
			else
			{
				if (node.Children.Length == 0)
				{
					throw new NotSupportedException("Unexpected empty node!");
				}
				this.WriteLabelForBranchTarget(node.Start);
				this.EnterNode(node);
				ExceptionSupport.Node[] children = node.Children;
				for (int i = 0; i < children.Length; i++)
				{
					ExceptionSupport.Node node2 = children[i];
					this.GenerateCodeRecursive(node2);
				}
				this.ExitNode(node);
			}
		}

		private void ProcessInstructionOperand(object operand)
		{
			MethodReference methodReference = operand as MethodReference;
			if (methodReference != null)
			{
				this.ProcessMethodReferenceOperand(methodReference);
			}
			FieldReference fieldReference = operand as FieldReference;
			if (fieldReference != null)
			{
				this.ProcessFieldReferenceOperand(fieldReference);
			}
		}

		private void ProcessMethodReferenceOperand(MethodReference methodReference)
		{
			if (!MethodBodyWriter.Naming.IsSpecialArrayMethod(methodReference))
			{
				this._writer.AddIncludeForMethodDeclarations(this._typeResolver.Resolve(methodReference.DeclaringType));
				MethodDefinition methodDefinition = methodReference.Resolve();
				if (!methodDefinition.ContainsGenericParameter && !methodDefinition.DeclaringType.HasGenericParameters && !MethodReferenceComparer.AreEqual(methodReference, methodDefinition))
				{
					this._writer.AddIncludeForMethodDeclarations(methodDefinition.DeclaringType);
				}
				if (methodDefinition.IsVirtual)
				{
					this._writer.AddIncludeForTypeDefinition(this._typeResolver.Resolve(methodReference.DeclaringType));
				}
				TypeResolver typeResolver = this._typeResolver;
				MethodReference methodReference2 = methodReference;
				GenericInstanceMethod genericInstanceMethod = methodReference as GenericInstanceMethod;
				if (genericInstanceMethod != null)
				{
					methodReference2 = this._typeResolver.Resolve(methodReference);
					typeResolver = this._typeResolver.Nested(methodReference2 as GenericInstanceMethod);
				}
				this._writer.AddIncludeForTypeDefinition(typeResolver.Resolve(GenericParameterResolver.ResolveReturnTypeIfNeeded(methodReference2)));
				foreach (ParameterDefinition current in methodReference2.Parameters)
				{
					this._writer.AddIncludeForTypeDefinition(typeResolver.Resolve(GenericParameterResolver.ResolveParameterTypeIfNeeded(methodReference2, current)));
				}
				if (genericInstanceMethod != null)
				{
					this._writer.AddIncludesForMethodDeclaration((GenericInstanceMethod)this._typeResolver.Resolve(genericInstanceMethod));
				}
			}
		}

		private void ProcessFieldReferenceOperand(FieldReference fieldReference)
		{
			FieldDefinition fieldDefinition = fieldReference.Resolve();
			TypeReference typeReference = this._typeResolver.Resolve(fieldReference.DeclaringType);
			this._writer.AddIncludeForTypeDefinition(typeReference);
			if (fieldDefinition.IsStatic)
			{
				this._writer.AddIncludeForMethodDeclarations(typeReference);
			}
			this._writer.AddIncludeForTypeDefinition(this._typeResolver.Resolve(GenericParameterResolver.ResolveFieldTypeIfNeeded(fieldReference)));
		}

		private void SetupFallthroughVariables(InstructionBlock block)
		{
			GlobalVariable[] array = this._stackAnalysis.InputVariablesFor(block.Successors.Single<InstructionBlock>());
			this.WriteAssignGlobalVariables(array);
			this._valueStack.Clear();
			foreach (GlobalVariable current in (from v in array
			orderby v.Index
			select v).Reverse<GlobalVariable>())
			{
				this._valueStack.Push(new StackInfo(current.VariableName, this._typeResolver.Resolve(current.Type)));
			}
		}

		private void EnterNode(ExceptionSupport.Node node)
		{
			switch (node.Type)
			{
			case ExceptionSupport.NodeType.Try:
				this.EnterTry(node);
				return;
			case ExceptionSupport.NodeType.Catch:
				this.EnterCatch(node);
				return;
			case ExceptionSupport.NodeType.Finally:
				this.EnterFinally(node);
				return;
			case ExceptionSupport.NodeType.Block:
				this._writer.BeginBlock();
				return;
			case ExceptionSupport.NodeType.Fault:
				this.EnterFault(node);
				return;
			}
			throw new NotImplementedException("Unexpected node type " + node.Type);
		}

		private void EnterTry(ExceptionSupport.Node node)
		{
			this._writer.WriteLine("try");
			this._writer.BeginBlock(string.Format("begin try (depth: {0})", node.Depth));
		}

		private void EnterFinally(ExceptionSupport.Node node)
		{
			this._writer.BeginBlock(string.Format("begin finally (depth: {0})", node.Depth));
		}

		private void EnterFault(ExceptionSupport.Node node)
		{
			this._writer.BeginBlock(string.Format("begin fault (depth: {0})", node.Depth));
		}

		private void EnterCatch(ExceptionSupport.Node node)
		{
			this._writer.BeginBlock(string.Format("begin catch({0})", node.Handler.CatchType.FullName));
		}

		private void ExitNode(ExceptionSupport.Node node)
		{
			switch (node.Type)
			{
			case ExceptionSupport.NodeType.Try:
				this.ExitTry(node);
				return;
			case ExceptionSupport.NodeType.Catch:
				this.ExitCatch(node);
				return;
			case ExceptionSupport.NodeType.Finally:
				this.ExitFinally(node);
				return;
			case ExceptionSupport.NodeType.Block:
				this._writer.EndBlock(false);
				return;
			case ExceptionSupport.NodeType.Fault:
				this.ExitFault(node);
				return;
			}
			throw new NotImplementedException("Unexpected node type " + node.Type);
		}

		private void ExitTry(ExceptionSupport.Node node)
		{
			this._writer.EndBlock(string.Format("end try (depth: {0})", node.Depth), false);
			ExceptionSupport.Node[] catchNodes = node.CatchNodes;
			ExceptionSupport.Node finallyNode = node.FinallyNode;
			ExceptionSupport.Node faultNode = node.FaultNode;
			this._writer.WriteLine("catch(Il2CppExceptionWrapper& e)");
			this._writer.BeginBlock();
			if (catchNodes.Length == 0)
			{
				if (finallyNode == null && faultNode == null)
				{
					throw new NotSupportedException("Try block ends without any catch, finally, nor fault handler");
				}
				this._writer.WriteLine("{0} = ({1})e.ex;", new object[]
				{
					"__last_unhandled_exception",
					MethodBodyWriter.Naming.ForVariable(MethodBodyWriter.TypeProvider.SystemException)
				});
				if (finallyNode != null)
				{
					this._writer.WriteLine(this._labeler.ForJump(finallyNode.Handler.HandlerStart));
				}
				if (faultNode != null)
				{
					this._writer.WriteLine(this._labeler.ForJump(faultNode.Handler.HandlerStart));
				}
			}
			else
			{
				this._writer.WriteLine("{0} = ({1})e.ex;", new object[]
				{
					"__exception_local",
					MethodBodyWriter.Naming.ForVariable(MethodBodyWriter.TypeProvider.SystemException)
				});
				foreach (ExceptionHandler current in from n in catchNodes
				select n.Handler)
				{
					this._writer.WriteLine("if(il2cpp_codegen_class_is_assignable_from ({0}, e.ex->object.klass))", new object[]
					{
						this._runtimeMetadataAccess.TypeInfoFor(current.CatchType)
					});
					this._writer.Indent();
					this._writer.WriteLine(this._labeler.ForJump(current.HandlerStart));
					this._writer.Dedent();
				}
				if (finallyNode != null)
				{
					this._writer.WriteLine("{0} = ({1})e.ex;", new object[]
					{
						"__last_unhandled_exception",
						MethodBodyWriter.Naming.ForVariable(MethodBodyWriter.TypeProvider.SystemException)
					});
					this._writer.WriteLine(this._labeler.ForJump(finallyNode.Handler.HandlerStart));
				}
				else if (faultNode != null)
				{
					this._writer.WriteLine("{0} = ({1})e.ex;", new object[]
					{
						"__last_unhandled_exception",
						MethodBodyWriter.Naming.ForVariable(MethodBodyWriter.TypeProvider.SystemException)
					});
					this._writer.WriteLine(this._labeler.ForJump(faultNode.Handler.HandlerStart));
				}
				else
				{
					this._writer.WriteLine("throw e;");
				}
			}
			this._writer.EndBlock(false);
		}

		private void ExitCatch(ExceptionSupport.Node node)
		{
			this._writer.EndBlock(string.Format("end catch (depth: {0})", node.Depth), false);
		}

		private void ExitFinally(ExceptionSupport.Node node)
		{
			this._writer.EndBlock(string.Format("end finally (depth: {0})", node.Depth), false);
			this._writer.WriteLine("IL2CPP_CLEANUP({0})", new object[]
			{
				node.Start.Offset
			});
			this._writer.BeginBlock();
			foreach (Instruction current in this._exceptionSupport.LeaveTargetsFor(node))
			{
				ExceptionSupport.Node[] array = node.GetTargetFinallyAndFaultNodesForJump(node.End.Offset, current.Offset).ToArray<ExceptionSupport.Node>();
				if (array.Length > 0)
				{
					this._writer.WriteLine("IL2CPP_END_CLEANUP(0x{0:X}, {1});", new object[]
					{
						current.Offset,
						this._labeler.FormatOffset(array.First<ExceptionSupport.Node>().Start)
					});
				}
				else
				{
					this._writer.WriteLine("IL2CPP_JUMP_TBL(0x{0:X}, {1})", new object[]
					{
						current.Offset,
						this._labeler.FormatOffset(current)
					});
				}
			}
			this._writer.WriteLine("IL2CPP_RETHROW_IF_UNHANDLED({0})", new object[]
			{
				MethodBodyWriter.Naming.ForVariable(MethodBodyWriter.TypeProvider.SystemException)
			});
			this._writer.EndBlock(false);
		}

		private void ExitFault(ExceptionSupport.Node node)
		{
			this._writer.EndBlock("end fault", false);
			this._writer.WriteLine("IL2CPP_CLEANUP({0})", new object[]
			{
				node.Start.Offset
			});
			this._writer.BeginBlock();
			foreach (Instruction current in this._exceptionSupport.LeaveTargetsFor(node))
			{
				ExceptionSupport.Node[] array = node.GetTargetFinallyAndFaultNodesForJump(node.End.Offset, current.Offset).ToArray<ExceptionSupport.Node>();
				if (array.Length > 0)
				{
					this._writer.WriteLine("IL2CPP_END_CLEANUP(0x{0:X}, {1});", new object[]
					{
						current.Offset,
						this._labeler.FormatOffset(array.First<ExceptionSupport.Node>().Start)
					});
				}
			}
			this._writer.WriteLine("IL2CPP_RETHROW_IF_UNHANDLED({0})", new object[]
			{
				MethodBodyWriter.Naming.ForVariable(MethodBodyWriter.TypeProvider.SystemException)
			});
			this._writer.EndBlock(false);
		}

		private void DumpInsFor(InstructionBlock block)
		{
			StackState stackState = this._stackAnalysis.InputStackStateFor(block);
			if (stackState.IsEmpty)
			{
				this.WriteComment("[in: -] empty", new object[0]);
			}
			else
			{
				List<Entry> list = new List<Entry>(stackState.Entries);
				for (int i = 0; i < list.Count; i++)
				{
					Entry entry = list[i];
					string arg_9F_1 = "[in: {0}] {1} (null: {2})";
					object[] expr_58 = new object[3];
					expr_58[0] = i;
					expr_58[1] = (from t in entry.Types
					select t.FullName).AggregateWithComma();
					expr_58[2] = entry.NullValue;
					this.WriteComment(arg_9F_1, expr_58);
				}
			}
		}

		private void DumpOutsFor(InstructionBlock block)
		{
			StackState stackState = this._stackAnalysis.OutputStackStateFor(block);
			if (stackState.IsEmpty)
			{
				this.WriteComment("[out: -] empty", new object[0]);
			}
			else
			{
				List<Entry> list = new List<Entry>(stackState.Entries);
				for (int i = 0; i < list.Count; i++)
				{
					Entry entry = list[i];
					string arg_9F_1 = "[out: {0}] {1} (null: {2})";
					object[] expr_58 = new object[3];
					expr_58[0] = i;
					expr_58[1] = (from t in entry.Types
					select t.FullName).AggregateWithComma();
					expr_58[2] = entry.NullValue;
					this.WriteComment(arg_9F_1, expr_58);
				}
			}
		}

		private void WriteComment(string message, params object[] args)
		{
			this._writer.WriteLine("// {0}", new object[]
			{
				string.Format(message, args)
			});
		}

		private void WriteAssignment(string leftName, TypeReference leftType, StackInfo right)
		{
			this._writer.WriteStatement(MethodBodyWriter.GetAssignment(leftName, leftType, right, this._sharingType));
		}

		public static string GetAssignment(string leftName, TypeReference leftType, StackInfo right, SharingType sharingType = SharingType.NonShared)
		{
			return Emit.Assign(leftName, MethodBodyWriter.WriteExpressionAndCastIfNeeded(leftType, right, sharingType));
		}

		private static string WriteExpressionAndCastIfNeeded(TypeReference leftType, StackInfo right, SharingType sharingType = SharingType.NonShared)
		{
			string result;
			if (leftType.MetadataType == MetadataType.Boolean && right.Type.IsIntegralType())
			{
				result = MethodBodyWriter.EmitCastRightCastToLeftType(leftType, right);
			}
			else if (leftType.IsPointer)
			{
				result = MethodBodyWriter.EmitCastRightCastToLeftType(leftType, right);
			}
			else if (leftType.IsGenericParameter())
			{
				result = right.Expression;
			}
			else if (right.Type.MetadataType == MetadataType.Object && leftType.MetadataType != MetadataType.Object)
			{
				result = MethodBodyWriter.EmitCastRightCastToLeftType(leftType, right);
			}
			else if (right.Type.IsArray && leftType.IsArray && right.Type.FullName != leftType.FullName)
			{
				result = MethodBodyWriter.EmitCastRightCastToLeftType(leftType, right);
			}
			else if (right.Type.MetadataType == MetadataType.IntPtr && leftType.MetadataType == MetadataType.Int32)
			{
				FieldDefinition field2 = MethodBodyWriter.TypeProvider.SystemIntPtr.Fields.Single((FieldDefinition field) => field.Name == MethodBodyWriter.Naming.IntPtrValueField);
				result = string.Format("({0})({1}){2}.{3}()", new object[]
				{
					MethodBodyWriter.Naming.ForVariable(leftType),
					MethodBodyWriter.Naming.ForIntPtrT,
					right.Expression,
					MethodBodyWriter.Naming.ForFieldGetter(field2)
				});
			}
			else if (sharingType == SharingType.Shared)
			{
				result = MethodBodyWriter.EmitCastRightCastToLeftType(leftType, right);
			}
			else if (!VarianceSupport.IsNeededForConversion(leftType, right.Type))
			{
				result = right.Expression;
			}
			else
			{
				result = string.Format("{0}{1}", VarianceSupport.Apply(leftType, right.Type), right.Expression);
			}
			return result;
		}

		private static string EmitCastRightCastToLeftType(TypeReference leftType, StackInfo right)
		{
			return string.Format("({0}){1}", MethodBodyWriter.Naming.ForVariable(leftType), right.Expression);
		}

		private void ProcessInstruction(ExceptionSupport.Node node, InstructionBlock block, ref Instruction ins)
		{
			ErrorInformation.CurrentlyProcessing.Instruction = ins;
			if (ins.SequencePoint != null)
			{
				DebuggerSupportFactory.GetDebuggerSupport().WriteSequencePoint(this._writer, ins, false);
			}
			switch (ins.OpCode.Code)
			{
			case Code.Nop:
				DebuggerSupportFactory.GetDebuggerSupport().WriteSequencePoint(this._writer, ins, true);
				break;
			case Code.Break:
				DebuggerSupportFactory.GetDebuggerSupport().WriteDebugBreak(this._writer);
				break;
			case Code.Ldarg_0:
				this.WriteLdarg(0, block, ins);
				break;
			case Code.Ldarg_1:
				this.WriteLdarg(1, block, ins);
				break;
			case Code.Ldarg_2:
				this.WriteLdarg(2, block, ins);
				break;
			case Code.Ldarg_3:
				this.WriteLdarg(3, block, ins);
				break;
			case Code.Ldloc_0:
				this.WriteLdloc(0, block, ins);
				break;
			case Code.Ldloc_1:
				this.WriteLdloc(1, block, ins);
				break;
			case Code.Ldloc_2:
				this.WriteLdloc(2, block, ins);
				break;
			case Code.Ldloc_3:
				this.WriteLdloc(3, block, ins);
				break;
			case Code.Stloc_0:
				this.WriteStloc(0);
				break;
			case Code.Stloc_1:
				this.WriteStloc(1);
				break;
			case Code.Stloc_2:
				this.WriteStloc(2);
				break;
			case Code.Stloc_3:
				this.WriteStloc(3);
				break;
			case Code.Ldarg_S:
			{
				ParameterReference parameterReference = (ParameterReference)ins.Operand;
				int num = parameterReference.Index;
				if (this._methodDefinition.HasThis)
				{
					num++;
				}
				this.WriteLdarg(num, block, ins);
				break;
			}
			case Code.Ldarga_S:
				this.LoadArgumentAddress((ParameterReference)ins.Operand);
				break;
			case Code.Starg_S:
				this.StoreArg(ins);
				break;
			case Code.Ldloc_S:
			case Code.Ldloc:
			{
				VariableReference variableReference = (VariableReference)ins.Operand;
				this.WriteLdloc(variableReference.Index, block, ins);
				break;
			}
			case Code.Ldloca_S:
				this.LoadLocalAddress((VariableReference)ins.Operand);
				break;
			case Code.Stloc_S:
			case Code.Stloc:
			{
				VariableReference variableReference2 = (VariableReference)ins.Operand;
				this.WriteStloc(variableReference2.Index);
				break;
			}
			case Code.Ldnull:
				this.LoadNull();
				break;
			case Code.Ldc_I4_M1:
				this.LoadInt32Constant(-1);
				break;
			case Code.Ldc_I4_0:
				this.LoadInt32Constant(0);
				break;
			case Code.Ldc_I4_1:
				this.LoadInt32Constant(1);
				break;
			case Code.Ldc_I4_2:
				this.LoadInt32Constant(2);
				break;
			case Code.Ldc_I4_3:
				this.LoadInt32Constant(3);
				break;
			case Code.Ldc_I4_4:
				this.LoadInt32Constant(4);
				break;
			case Code.Ldc_I4_5:
				this.LoadInt32Constant(5);
				break;
			case Code.Ldc_I4_6:
				this.LoadInt32Constant(6);
				break;
			case Code.Ldc_I4_7:
				this.LoadInt32Constant(7);
				break;
			case Code.Ldc_I4_8:
				this.LoadInt32Constant(8);
				break;
			case Code.Ldc_I4_S:
				this.LoadPrimitiveTypeSByte(ins, this.Int32TypeReference);
				break;
			case Code.Ldc_I4:
				this.LoadPrimitiveTypeInt32(ins, this.Int32TypeReference);
				break;
			case Code.Ldc_I8:
				this.LoadLong(ins, MethodBodyWriter.TypeProvider.Int64TypeReference);
				break;
			case Code.Ldc_R4:
				this.LoadConstant(this.SingleTypeReference, Formatter.StringRepresentationFor((float)ins.Operand));
				break;
			case Code.Ldc_R8:
				this.LoadConstant(this.DoubleTypeReference, Formatter.StringRepresentationFor((double)ins.Operand));
				break;
			case Code.Dup:
				this.WriteDup();
				break;
			case Code.Pop:
				this._valueStack.Pop();
				break;
			case Code.Jmp:
				throw new NotImplementedException();
			case Code.Call:
			{
				if (this._constrainedCallThisType != null)
				{
					throw new InvalidOperationException(string.Format("Constrained opcode was followed a Call rather than a Callvirt in method '{0}' at instruction '{1}'", this._methodReference.FullName, ins));
				}
				string suffix = "_" + ins.Offset;
				MethodReference methodReference = (MethodReference)ins.Operand;
				string callExpression = this.CallExpressionFor(this._methodReference, methodReference, MethodCallType.Normal, MethodBodyWriter.PopItemsFromStack(methodReference.Parameters.Count + ((!methodReference.HasThis) ? 0 : 1), this._valueStack), (string s) => s + suffix, true);
				this.EmitCallExpressionAndStoreResult(ins, this._typeResolver.ResolveReturnType(methodReference), callExpression);
				break;
			}
			case Code.Calli:
				throw new NotImplementedException();
			case Code.Ret:
				this.WriteReturnStatement();
				break;
			case Code.Br_S:
			case Code.Br:
				this.WriteUnconditionalJumpTo(block, (Instruction)ins.Operand);
				break;
			case Code.Brfalse_S:
			case Code.Brfalse:
				this.GenerateConditionalJump(block, ins, false);
				break;
			case Code.Brtrue_S:
			case Code.Brtrue:
				this.GenerateConditionalJump(block, ins, true);
				break;
			case Code.Beq_S:
			case Code.Beq:
				this.GenerateConditionalJump(block, ins, "==", MethodBodyWriter.Signedness.Signed, false);
				break;
			case Code.Bge_S:
			case Code.Bge:
				this.GenerateConditionalJump(block, ins, ">=", MethodBodyWriter.Signedness.Signed, false);
				break;
			case Code.Bgt_S:
			case Code.Bgt:
				this.GenerateConditionalJump(block, ins, ">", MethodBodyWriter.Signedness.Signed, false);
				break;
			case Code.Ble_S:
			case Code.Ble:
				this.GenerateConditionalJump(block, ins, "<=", MethodBodyWriter.Signedness.Signed, false);
				break;
			case Code.Blt_S:
			case Code.Blt:
				this.GenerateConditionalJump(block, ins, "<", MethodBodyWriter.Signedness.Signed, false);
				break;
			case Code.Bne_Un_S:
			case Code.Bne_Un:
				this.GenerateConditionalJump(block, ins, "==", MethodBodyWriter.Signedness.Unsigned, true);
				break;
			case Code.Bge_Un_S:
			case Code.Bge_Un:
				this.GenerateConditionalJump(block, ins, "<", MethodBodyWriter.Signedness.Unsigned, true);
				break;
			case Code.Bgt_Un_S:
			case Code.Bgt_Un:
				this.GenerateConditionalJump(block, ins, "<=", MethodBodyWriter.Signedness.Unsigned, true);
				break;
			case Code.Ble_Un_S:
			case Code.Ble_Un:
				this.GenerateConditionalJump(block, ins, ">", MethodBodyWriter.Signedness.Unsigned, true);
				break;
			case Code.Blt_Un_S:
			case Code.Blt_Un:
				this.GenerateConditionalJump(block, ins, ">=", MethodBodyWriter.Signedness.Unsigned, true);
				break;
			case Code.Switch:
			{
				StackInfo stackInfo = this._valueStack.Pop();
				Instruction[] targetInstructions = (Instruction[])ins.Operand;
				int num2 = 0;
				List<InstructionBlock> list = new List<InstructionBlock>(block.Successors);
				InstructionBlock instructionBlock = list.SingleOrDefault((InstructionBlock b) => !(from t in targetInstructions
				select t.Offset).Contains(b.First.Offset));
				if (instructionBlock != null)
				{
					list.Remove(instructionBlock);
					this.WriteAssignGlobalVariables(this._stackAnalysis.InputVariablesFor(instructionBlock));
				}
				Instruction[] targetInstructions2 = targetInstructions;
				for (int i = 0; i < targetInstructions2.Length; i++)
				{
					Instruction targetInstruction = targetInstructions2[i];
					using (this.NewIfBlock(string.Format("{0} == {1}", stackInfo, num2++)))
					{
						InstructionBlock block2 = list.First((InstructionBlock b) => b.First.Offset == targetInstruction.Offset);
						this.WriteAssignGlobalVariables(this._stackAnalysis.InputVariablesFor(block2));
						this.WriteJump(targetInstruction);
					}
				}
				break;
			}
			case Code.Ldind_I1:
				this.LoadIndirect(this.SByteTypeReference, this.Int32TypeReference);
				break;
			case Code.Ldind_U1:
				this.LoadIndirect(this.ByteTypeReference, this.Int32TypeReference);
				break;
			case Code.Ldind_I2:
				this.LoadIndirect(this.Int16TypeReference, this.Int32TypeReference);
				break;
			case Code.Ldind_U2:
				this.LoadIndirect(this.UInt16TypeReference, this.Int32TypeReference);
				break;
			case Code.Ldind_I4:
				this.LoadIndirect(this.Int32TypeReference, this.Int32TypeReference);
				break;
			case Code.Ldind_U4:
				this.LoadIndirect(this.UInt32TypeReference, this.Int32TypeReference);
				break;
			case Code.Ldind_I8:
				this.LoadIndirect(this.Int64TypeReference, this.Int64TypeReference);
				break;
			case Code.Ldind_I:
				this.LoadIndirectNativeInteger();
				break;
			case Code.Ldind_R4:
				this.LoadIndirect(this.SingleTypeReference, this.SingleTypeReference);
				break;
			case Code.Ldind_R8:
				this.LoadIndirect(this.DoubleTypeReference, this.DoubleTypeReference);
				break;
			case Code.Ldind_Ref:
				this.LoadIndirectReference();
				break;
			case Code.Stind_Ref:
				this.StoreIndirect(this.ObjectTypeReference);
				break;
			case Code.Stind_I1:
				this.StoreIndirect(this.SByteTypeReference);
				break;
			case Code.Stind_I2:
				this.StoreIndirect(this.Int16TypeReference);
				break;
			case Code.Stind_I4:
				this.StoreIndirect(this.Int32TypeReference);
				break;
			case Code.Stind_I8:
				this.StoreIndirect(this.Int64TypeReference);
				break;
			case Code.Stind_R4:
				this.StoreIndirect(this.SingleTypeReference);
				break;
			case Code.Stind_R8:
				this.StoreIndirect(this.DoubleTypeReference);
				break;
			case Code.Add:
				this.WriteAdd(MethodBodyWriter.OverflowCheck.None);
				break;
			case Code.Sub:
				this.WriteSub(MethodBodyWriter.OverflowCheck.None);
				break;
			case Code.Mul:
				this.WriteMul(MethodBodyWriter.OverflowCheck.None);
				break;
			case Code.Div:
				this._divideByZeroCheckSupport.WriteDivideByZeroCheckIfNeeded(this._valueStack.Peek());
				this.WriteBinaryOperationUsingLargestOperandTypeAsResultType("/");
				break;
			case Code.Div_Un:
				this._divideByZeroCheckSupport.WriteDivideByZeroCheckIfNeeded(this._valueStack.Peek());
				this.WriteUnsignedArithmeticOperation("/");
				break;
			case Code.Rem:
				this.WriteRemainderOperation();
				break;
			case Code.Rem_Un:
				this.WriteUnsignedArithmeticOperation("%");
				break;
			case Code.And:
				this.WriteBinaryOperationUsingLeftOperandTypeAsResultType("&");
				break;
			case Code.Or:
				this.WriteBinaryOperationUsingLargestOperandTypeAsResultType("|");
				break;
			case Code.Xor:
				this.WriteBinaryOperationUsingLargestOperandTypeAsResultType("^");
				break;
			case Code.Shl:
				this.WriteBinaryOperationUsingLeftOperandTypeAsResultType("<<");
				break;
			case Code.Shr:
				this.WriteBinaryOperationUsingLeftOperandTypeAsResultType(">>");
				break;
			case Code.Shr_Un:
				this.WriteShrUn();
				break;
			case Code.Neg:
				this.WriteNegateOperation();
				break;
			case Code.Not:
				this.WriteNotOperation();
				break;
			case Code.Conv_I1:
				this.WriteNumericConversion(this.SByteTypeReference);
				break;
			case Code.Conv_I2:
				this.WriteNumericConversion(this.Int16TypeReference);
				break;
			case Code.Conv_I4:
				this.WriteNumericConversion(this.Int32TypeReference);
				break;
			case Code.Conv_I8:
				this.WriteNumericConversionI8();
				break;
			case Code.Conv_R4:
				this.WriteNumericConversionFloat(this.SingleTypeReference);
				break;
			case Code.Conv_R8:
				this.WriteNumericConversionFloat(this.DoubleTypeReference);
				break;
			case Code.Conv_U4:
				this.WriteNumericConversion(this.UInt32TypeReference, this.Int32TypeReference);
				break;
			case Code.Conv_U8:
				this.WriteNumericConversionU8();
				break;
			case Code.Callvirt:
			{
				MethodReference methodReference2 = (MethodReference)ins.Operand;
				List<StackInfo> poppedValues = MethodBodyWriter.PopItemsFromStack(methodReference2.Parameters.Count + 1, this._valueStack);
				string suffix = "_" + ins.Offset;
				string callExpression2;
				if (this._constrainedCallThisType != null)
				{
					callExpression2 = this.ConstrainedCallExpressionFor(this._typeResolver.Resolve(methodReference2), ref methodReference2, MethodCallType.Virtual, poppedValues, (string s) => s + suffix);
				}
				else
				{
					callExpression2 = this.CallExpressionFor(this._methodReference, methodReference2, MethodCallType.Virtual, poppedValues, (string s) => s + suffix, true);
				}
				this.EmitCallExpressionAndStoreResult(ins, this._typeResolver.ResolveReturnType(methodReference2), callExpression2);
				this._constrainedCallThisType = null;
				break;
			}
			case Code.Cpobj:
				throw new NotImplementedException();
			case Code.Ldobj:
				this.WriteLoadObject(ins);
				break;
			case Code.Ldstr:
			{
				string literal = (string)ins.Operand;
				this._writer.AddIncludeForTypeDefinition(this.StringTypeReference);
				this._valueStack.Push(new StackInfo(this._runtimeMetadataAccess.StringLiteral(literal), this.StringTypeReference));
				break;
			}
			case Code.Newobj:
			{
				MethodReference methodReference3 = (MethodReference)ins.Operand;
				MethodReference methodReference4 = this._typeResolver.Resolve(methodReference3);
				TypeReference typeReference = this._typeResolver.Resolve(methodReference4.DeclaringType);
				Local local = this.NewTemp(typeReference);
				this._writer.AddIncludeForTypeDefinition(this._typeResolver.Resolve(methodReference3.DeclaringType));
				GenericInstanceType genericInstanceType = this._methodReference.DeclaringType as GenericInstanceType;
				if (genericInstanceType != null && !(methodReference4 is GenericInstanceMethod))
				{
					methodReference4 = TypeResolver.For(genericInstanceType).Resolve(methodReference4);
				}
				List<TypeReference> parameterTypes = MethodBodyWriter.GetParameterTypes(methodReference4, this._typeResolver);
				List<string> list2 = MethodBodyWriter.FormatArgumentsForMethodCall(parameterTypes, MethodBodyWriter.PopItemsFromStack(parameterTypes.Count, this._valueStack), this._sharingType);
				if (MethodSignatureWriter.NeedsHiddenMethodInfo(methodReference4, MethodCallType.Normal, true))
				{
					list2.Add(((!CodeGenOptions.EmitComments) ? "" : "/*hidden argument*/") + this._runtimeMetadataAccess.HiddenMethodInfo(methodReference3));
				}
				if (typeReference.IsArray)
				{
					ArrayType arrayType = (ArrayType)typeReference;
					if (arrayType.Rank < 2)
					{
						throw new NotImplementedException("Attempting to create a multidimensional array of rank lesser than 2");
					}
					string text = this.NewTempName();
					this._writer.WriteLine("{0} {1}[] = {{ {2} }};", new object[]
					{
						MethodBodyWriter.Naming.ForArrayIndexType(),
						text,
						Emit.CastEach(MethodBodyWriter.Naming.ForArrayIndexType(), list2).AggregateWithComma()
					});
					this._writer.WriteLine("{0};", new object[]
					{
						Emit.Assign(local.IdentifierExpression, Emit.Cast(arrayType, Emit.Call("GenArrayNew", this._runtimeMetadataAccess.TypeInfoFor(methodReference3.DeclaringType), text)))
					});
				}
				else if (typeReference.IsValueType())
				{
					string item = MethodBodyWriter.Naming.AddressOf(local.Expression);
					list2.Insert(0, item);
					this._writer.WriteVariable(typeReference, local.Expression);
					this._writer.WriteStatement(Emit.Call(MethodBodyWriter.Naming.ForMethod(methodReference4), list2));
				}
				else if (methodReference4.Name == ".ctor" && typeReference.MetadataType == MetadataType.String)
				{
					list2.Insert(0, MethodBodyWriter.Naming.Null);
					this._writer.WriteStatement(Emit.Assign(local.IdentifierExpression, Emit.Call(MethodBodyWriter.Naming.ForCreateStringMethod(methodReference4), list2)));
				}
				else
				{
					this._writer.WriteStatement(Emit.Assign(local.IdentifierExpression, Emit.Cast(typeReference, Emit.Call("il2cpp_codegen_object_new", this._runtimeMetadataAccess.Newobj(methodReference3)))));
					list2.Insert(0, local.Expression);
					this._writer.WriteStatement(Emit.Call(this._runtimeMetadataAccess.Method(methodReference3), list2));
				}
				this._valueStack.Push(new StackInfo(local));
				break;
			}
			case Code.Castclass:
				this.WriteCastclassOrIsInst((TypeReference)ins.Operand, this._valueStack.Pop(), "Castclass");
				break;
			case Code.Isinst:
				this.WriteCastclassOrIsInst((TypeReference)ins.Operand, this._valueStack.Pop(), "IsInst");
				break;
			case Code.Conv_R_Un:
				this.WriteNumericConversionToFloatFromUnsigned();
				break;
			case Code.Unbox:
				this.Unbox(ins);
				break;
			case Code.Throw:
			{
				StackInfo stackInfo2 = this._valueStack.Pop();
				this._writer.WriteStatement(Emit.RaiseManagedException(stackInfo2.ToString()));
				break;
			}
			case Code.Ldfld:
				this.LoadField(ins, false);
				break;
			case Code.Ldflda:
				this.LoadField(ins, true);
				break;
			case Code.Stfld:
				this.StoreField(ins);
				break;
			case Code.Ldsfld:
			case Code.Ldsflda:
			case Code.Stsfld:
				this.StaticFieldAccess(ins);
				break;
			case Code.Stobj:
				this.WriteStoreObject((TypeReference)ins.Operand);
				break;
			case Code.Conv_Ovf_I1_Un:
				this.WriteNumericConversionWithOverflow<sbyte>(this.ByteTypeReference, true, 127, true);
				break;
			case Code.Conv_Ovf_I2_Un:
				this.WriteNumericConversionWithOverflow<short>(this.Int16TypeReference, true, 32767, true);
				break;
			case Code.Conv_Ovf_I4_Un:
				this.WriteNumericConversionWithOverflow<int>(this.Int32TypeReference, true, 2147483647, true);
				break;
			case Code.Conv_Ovf_I8_Un:
				this.WriteNumericConversionWithOverflow<long>(this.Int64TypeReference, true, 9223372036854775807L, true);
				break;
			case Code.Conv_Ovf_U1_Un:
				this.WriteNumericConversionWithOverflow<byte>(this.ByteTypeReference, true, 255, true);
				break;
			case Code.Conv_Ovf_U2_Un:
				this.WriteNumericConversionWithOverflow<ushort>(this.UInt16TypeReference, true, 65535, true);
				break;
			case Code.Conv_Ovf_U4_Un:
				this.WriteNumericConversionWithOverflow<uint>(this.UInt32TypeReference, true, 4294967295u, true);
				break;
			case Code.Conv_Ovf_U8_Un:
				this.WriteNumericConversionWithOverflow<ulong>(this.UInt64TypeReference, true, 18446744073709551615uL, true);
				break;
			case Code.Conv_Ovf_I_Un:
				this.ConvertToNaturalIntWithOverflow<string>(this.NativeIntTypeReference, true, "INTPTR_MAX");
				break;
			case Code.Conv_Ovf_U_Un:
				this.ConvertToNaturalIntWithOverflow<string>(this.NativeUIntTypeReference, true, "UINTPTR_MAX");
				break;
			case Code.Box:
			{
				TypeReference typeReference2 = this._typeResolver.Resolve((TypeReference)ins.Operand);
				this._writer.AddIncludeForTypeDefinition(typeReference2);
				if (typeReference2.IsValueType())
				{
					StackInfo originalValue = this._valueStack.Pop();
					if (this.CanApplyValueTypeBoxBranchOptimizationToInstruction(ins, block))
					{
						Instruction next = ins.Next;
						if (next.OpCode.Code == Code.Brtrue || next.OpCode.Code == Code.Brtrue_S)
						{
							this.WriteGlobalVariableAssignmentForRightBranch(block, (Instruction)next.Operand);
							this.WriteJump((Instruction)next.Operand);
						}
						this.WriteGlobalVariableAssignmentForLeftBranch(block, (Instruction)next.Operand);
						ins = next;
					}
					else
					{
						bool flag = typeReference2.MetadataType == MetadataType.IntPtr || typeReference2.MetadataType == MetadataType.UIntPtr;
						bool flag2 = originalValue.Type.IsSameType(this.NativeIntTypeReference) || originalValue.Type.IsSameType(this.NativeUIntTypeReference);
						if (flag && flag2)
						{
							typeReference2 = originalValue.Type;
						}
						if ((originalValue.Type.MetadataType == MetadataType.IntPtr && typeReference2.MetadataType == MetadataType.UIntPtr) || (originalValue.Type.MetadataType == MetadataType.UIntPtr && typeReference2.MetadataType == MetadataType.IntPtr))
						{
							this.StoreLocalAndPush(this.ObjectTypeReference, string.Format("Box({0}, &{1})", this._runtimeMetadataAccess.TypeInfoFor((TypeReference)ins.Operand), originalValue.Expression));
						}
						else
						{
							Local local2 = this.NewTemp(typeReference2);
							this._writer.WriteLine("{0} = {1};", new object[]
							{
								local2.IdentifierExpression,
								MethodBodyWriter.CastTypeIfNeeded(originalValue, typeReference2)
							});
							this.StoreLocalAndPush(this.ObjectTypeReference, string.Format("Box({0}, &{1})", this._runtimeMetadataAccess.TypeInfoFor((TypeReference)ins.Operand), local2.Expression));
						}
					}
				}
				break;
			}
			case Code.Newarr:
			{
				StackInfo stackInfo3 = this._valueStack.Pop();
				TypeReference type = this._typeResolver.Resolve((TypeReference)ins.Operand);
				ArrayType arrayType2 = new ArrayType(type);
				this._writer.AddIncludeForTypeDefinition(arrayType2);
				string argument = string.Format("(uint32_t){0}", stackInfo3.Expression);
				this.PushExpression(arrayType2, Emit.Cast(arrayType2, Emit.Call("SZArrayNew", this._runtimeMetadataAccess.ArrayInfo((TypeReference)ins.Operand), argument)));
				break;
			}
			case Code.Ldlen:
			{
				StackInfo stackInfo4 = this._valueStack.Pop();
				this._nullCheckSupport.WriteNullCheckIfNeeded(stackInfo4);
				this.PushExpression(this.UInt32TypeReference, string.Format("(({0}){1})->max_length", MethodBodyWriter.Naming.ForVariable(MethodBodyWriter.TypeProvider.SystemArray), stackInfo4));
				break;
			}
			case Code.Ldelema:
			{
				StackInfo index = this._valueStack.Pop();
				StackInfo stackInfo5 = this._valueStack.Pop();
				TypeReference type2 = this._typeResolver.Resolve((TypeReference)ins.Operand);
				ByReferenceType typeReference3 = new ByReferenceType(type2);
				this._nullCheckSupport.WriteNullCheckIfNeeded(stackInfo5);
				this.WriteArrayBoundsCheckIfNeeded(stackInfo5, index);
				this.PushExpression(typeReference3, this.EmitArrayLoadElementAddress(stackInfo5, index.Expression, index.Type));
				break;
			}
			case Code.Ldelem_I1:
				this.LoadElemAndPop(this.SByteTypeReference);
				break;
			case Code.Ldelem_U1:
				this.LoadElemAndPop(this.ByteTypeReference);
				break;
			case Code.Ldelem_I2:
				this.LoadElemAndPop(this.Int16TypeReference);
				break;
			case Code.Ldelem_U2:
				this.LoadElemAndPop(this.UInt16TypeReference);
				break;
			case Code.Ldelem_I4:
				this.LoadElemAndPop(this.Int32TypeReference);
				break;
			case Code.Ldelem_U4:
				this.LoadElemAndPop(this.UInt32TypeReference);
				break;
			case Code.Ldelem_I8:
				this.LoadElemAndPop(this.Int64TypeReference);
				break;
			case Code.Ldelem_I:
				this.LoadElemAndPop(this.IntPtrTypeReference);
				break;
			case Code.Ldelem_R4:
				this.LoadElemAndPop(this.SingleTypeReference);
				break;
			case Code.Ldelem_R8:
				this.LoadElemAndPop(this.DoubleTypeReference);
				break;
			case Code.Ldelem_Ref:
			{
				StackInfo index2 = this._valueStack.Pop();
				StackInfo array = this._valueStack.Pop();
				this.LoadElem(array, ArrayUtilities.ArrayElementTypeOf(array.Type), index2);
				break;
			}
			case Code.Stelem_I:
			case Code.Stelem_I1:
			case Code.Stelem_I2:
			case Code.Stelem_I4:
			case Code.Stelem_I8:
			case Code.Stelem_R4:
			case Code.Stelem_R8:
			case Code.Stelem_Any:
			{
				StackInfo value = this._valueStack.Pop();
				StackInfo index3 = this._valueStack.Pop();
				StackInfo array2 = this._valueStack.Pop();
				this.StoreElement(array2, index3, value, false);
				break;
			}
			case Code.Stelem_Ref:
			{
				StackInfo value2 = this._valueStack.Pop();
				StackInfo index4 = this._valueStack.Pop();
				StackInfo array3 = this._valueStack.Pop();
				this.StoreElement(array3, index4, value2, true);
				break;
			}
			case Code.Ldelem_Any:
				this.LoadElemAndPop(this._typeResolver.Resolve((TypeReference)ins.Operand));
				break;
			case Code.Unbox_Any:
			{
				StackInfo value3 = this._valueStack.Pop();
				TypeReference typeReference4 = this._typeResolver.Resolve((TypeReference)ins.Operand);
				this._writer.AddIncludeForTypeDefinition(typeReference4);
				if (typeReference4.IsValueType())
				{
					ByReferenceType byReferenceType = new ByReferenceType(typeReference4);
					this.PushExpression(byReferenceType, Emit.Cast(new PointerType(typeReference4), this.Unbox((TypeReference)ins.Operand, value3.Expression)));
					this.PushExpression(typeReference4, Emit.Dereference(Emit.Cast(byReferenceType, this._valueStack.Pop().Expression)));
				}
				else
				{
					this.WriteCastclass((TypeReference)ins.Operand, value3, ins);
				}
				break;
			}
			case Code.Conv_Ovf_I1:
				this.WriteNumericConversionWithOverflow<sbyte>(this.SByteTypeReference, false, 127, true);
				break;
			case Code.Conv_Ovf_U1:
				this.WriteNumericConversionWithOverflow<byte>(this.ByteTypeReference, false, 255, true);
				break;
			case Code.Conv_Ovf_I2:
				this.WriteNumericConversionWithOverflow<short>(this.Int16TypeReference, false, 32767, true);
				break;
			case Code.Conv_Ovf_U2:
				this.WriteNumericConversionWithOverflow<ushort>(this.UInt16TypeReference, false, 65535, true);
				break;
			case Code.Conv_Ovf_I4:
				this.WriteNumericConversionWithOverflow<int>(this.Int32TypeReference, false, 2147483647, true);
				break;
			case Code.Conv_Ovf_U4:
				this.WriteNumericConversionWithOverflow<uint>(this.UInt32TypeReference, false, 4294967295u, true);
				break;
			case Code.Conv_Ovf_I8:
				this.WriteNumericConversionWithOverflow<string>(this.Int64TypeReference, false, "std::numeric_limits<int64_t>::max()", false);
				break;
			case Code.Conv_Ovf_U8:
				this.WriteNumericConversionWithOverflow<string>(this.UInt64TypeReference, true, "std::numeric_limits<uint64_t>::max()", false);
				break;
			case Code.Refanyval:
				throw new NotImplementedException();
			case Code.Ckfinite:
				throw new NotImplementedException();
			case Code.Mkrefany:
				throw new NotImplementedException();
			case Code.Ldtoken:
				this._valueStack.Push(this.FormatLoadTokenFor(ins));
				break;
			case Code.Conv_U2:
				this.WriteNumericConversion(this.UInt16TypeReference, this.Int32TypeReference);
				break;
			case Code.Conv_U1:
				this.WriteNumericConversion(this.ByteTypeReference, this.Int32TypeReference);
				break;
			case Code.Conv_I:
				this.ConvertToNaturalInt(this.NativeIntTypeReference);
				break;
			case Code.Conv_Ovf_I:
				this.ConvertToNaturalIntWithOverflow<string>(this.NativeIntTypeReference, false, "INTPTR_MAX");
				break;
			case Code.Conv_Ovf_U:
				this.ConvertToNaturalIntWithOverflow<string>(this.NativeUIntTypeReference, false, "UINTPTR_MAX");
				break;
			case Code.Add_Ovf:
				this.WriteAdd(MethodBodyWriter.OverflowCheck.Signed);
				break;
			case Code.Add_Ovf_Un:
				this.WriteAdd(MethodBodyWriter.OverflowCheck.Unsigned);
				break;
			case Code.Mul_Ovf:
				this.WriteMul(MethodBodyWriter.OverflowCheck.Signed);
				break;
			case Code.Mul_Ovf_Un:
				this.WriteMul(MethodBodyWriter.OverflowCheck.Unsigned);
				break;
			case Code.Sub_Ovf:
				this.WriteSub(MethodBodyWriter.OverflowCheck.Signed);
				break;
			case Code.Sub_Ovf_Un:
				this.WriteSub(MethodBodyWriter.OverflowCheck.Unsigned);
				break;
			case Code.Endfinally:
			{
				ExceptionSupport.Node enclosingFinallyOrFaultNode = node.GetEnclosingFinallyOrFaultNode();
				this._writer.WriteLine("IL2CPP_END_FINALLY({0})", new object[]
				{
					enclosingFinallyOrFaultNode.Start.Offset
				});
				break;
			}
			case Code.Leave:
			case Code.Leave_S:
				if (this.ShouldStripLeaveInstruction(block, ins))
				{
					this._writer.WriteLine("; // {0}", new object[]
					{
						ins
					});
				}
				else
				{
					switch (node.Type)
					{
					case ExceptionSupport.NodeType.Try:
						this.EmitCodeForLeaveFromTry(node, ins);
						break;
					case ExceptionSupport.NodeType.Catch:
						this.EmitCodeForLeaveFromCatch(node, ins);
						break;
					case ExceptionSupport.NodeType.Finally:
					case ExceptionSupport.NodeType.Fault:
						this.EmitCodeForLeaveFromFinallyOrFault(ins);
						break;
					case ExceptionSupport.NodeType.Block:
					case ExceptionSupport.NodeType.Root:
						this.EmitCodeForLeaveFromBlock(node, ins);
						break;
					}
					this._valueStack.Clear();
				}
				break;
			case Code.Stind_I:
				this.StoreIndirect(this.NativeIntTypeReference);
				break;
			case Code.Conv_U:
				this.ConvertToNaturalInt(MethodBodyWriter.TypeProvider.NativeUIntTypeReference);
				break;
			case Code.Arglist:
				this._writer.WriteLine("#pragma message(FIXME \"arglist is not supported\")");
				this._writer.WriteLine("assert(false && \"arglist is not supported\");");
				this._valueStack.Push(new StackInfo("LoadArgList()", this.NativeIntTypeReference));
				break;
			case Code.Ceq:
				this.GenerateConditional("==", MethodBodyWriter.Signedness.Signed, false);
				break;
			case Code.Cgt:
				this.GenerateConditional(">", MethodBodyWriter.Signedness.Signed, false);
				break;
			case Code.Cgt_Un:
				this.GenerateConditional("<=", MethodBodyWriter.Signedness.Unsigned, true);
				break;
			case Code.Clt:
				this.GenerateConditional("<", MethodBodyWriter.Signedness.Signed, false);
				break;
			case Code.Clt_Un:
				this.GenerateConditional(">=", MethodBodyWriter.Signedness.Unsigned, true);
				break;
			case Code.Ldftn:
			{
				MethodReference methodReference5 = (MethodReference)ins.Operand;
				this._writer.AddIncludeForTypeDefinition(this._typeResolver.Resolve(methodReference5.DeclaringType));
				this.StoreLocalIntPtrAndPush(string.Format("(void*){0}", this._runtimeMetadataAccess.MethodInfo(methodReference5)));
				break;
			}
			case Code.Ldvirtftn:
				this.PushCallToLoadVirtualFunction(ins);
				break;
			case Code.Ldarg:
			{
				ParameterReference parameterReference2 = (ParameterReference)ins.Operand;
				int num3 = parameterReference2.Index;
				if (this._methodDefinition.HasThis)
				{
					num3++;
				}
				this.WriteLdarg(num3, block, ins);
				break;
			}
			case Code.Ldarga:
				this.LoadArgumentAddress((ParameterReference)ins.Operand);
				break;
			case Code.Starg:
				this.StoreArg(ins);
				break;
			case Code.Ldloca:
				this.LoadLocalAddress((VariableReference)ins.Operand);
				break;
			case Code.Localloc:
			{
				StackInfo stackInfo6 = this._valueStack.Pop();
				PointerType pointerType = new PointerType(this.SByteTypeReference);
				string text2 = this.NewTempName();
				this._writer.WriteLine(string.Format("{0} {1} = ({0}) alloca({2});", MethodBodyWriter.Naming.ForVariable(pointerType), text2, stackInfo6));
				if (this._methodDefinition.Body.InitLocals)
				{
					this._writer.WriteLine("memset({0},0,{1});", new object[]
					{
						text2,
						stackInfo6
					});
				}
				this.PushExpression(pointerType, text2);
				break;
			}
			case Code.Endfilter:
				throw new NotImplementedException();
			case Code.Unaligned:
				throw new NotImplementedException();
			case Code.Volatile:
				this.AddVolatileStackEntry();
				break;
			case Code.Tail:
				break;
			case Code.Initobj:
			{
				StackInfo stackInfo7 = this._valueStack.Pop();
				ByReferenceType byReferenceType2 = (ByReferenceType)this._typeResolver.Resolve(stackInfo7.Type);
				TypeReference elementType = byReferenceType2.ElementType;
				this._writer.AddIncludeForTypeDefinition(this._typeResolver.Resolve(elementType));
				this._writer.WriteLine("Initobj ({0}, {1});", new object[]
				{
					this._runtimeMetadataAccess.TypeInfoFor(elementType),
					stackInfo7.Expression
				});
				break;
			}
			case Code.Constrained:
				this._constrainedCallThisType = (TypeReference)ins.Operand;
				this._writer.AddIncludeForTypeDefinition(this._typeResolver.Resolve(this._constrainedCallThisType));
				break;
			case Code.Cpblk:
				throw new NotImplementedException();
			case Code.Initblk:
				throw new NotImplementedException();
			case Code.No:
				throw new NotImplementedException();
			case Code.Rethrow:
				if (node.Type == ExceptionSupport.NodeType.Finally)
				{
					this._writer.WriteLine("{0} = {1};", new object[]
					{
						"__last_unhandled_exception",
						"__exception_local"
					});
					this.WriteJump(node.Handler.HandlerStart);
				}
				else
				{
					this._writer.WriteStatement(Emit.RaiseManagedException("__exception_local"));
				}
				break;
			case Code.Sizeof:
				this.StoreLocalAndPush(this.UInt32TypeReference, this._runtimeMetadataAccess.SizeOf((TypeReference)ins.Operand));
				break;
			case Code.Refanytype:
			{
				StackInfo stackInfo8 = this._valueStack.Pop();
				if (stackInfo8.Type.FullName != "System.TypedReference" && stackInfo8.Type.Resolve().Module.Name == "mscorlib")
				{
					throw new InvalidOperationException();
				}
				FieldDefinition field = stackInfo8.Type.Resolve().Fields.Single((FieldDefinition f) => TypeReferenceEqualityComparer.AreEqual(f.FieldType, this.RuntimeTypeHandleTypeReference, TypeComparisonMode.Exact));
				string expression = string.Format("{0}.{1}()", stackInfo8.Expression, MethodBodyWriter.Naming.ForFieldGetter(field));
				this._valueStack.Push(new StackInfo(expression, this.RuntimeTypeHandleTypeReference));
				break;
			}
			case Code.Readonly:
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void WriteNumericConversionToFloatFromUnsigned()
		{
			StackInfo stackInfo = this._valueStack.Peek();
			if (stackInfo.Type.MetadataType == MetadataType.Single || stackInfo.Type.MetadataType == MetadataType.Double)
			{
				this.WriteNumericConversion(stackInfo.Type, this.DoubleTypeReference);
			}
			else if (stackInfo.Type.MetadataType == MetadataType.Int64 || stackInfo.Type.MetadataType == MetadataType.UInt64)
			{
				this.WriteNumericConversion(this.UInt64TypeReference, this.DoubleTypeReference);
			}
			else
			{
				this.WriteNumericConversion(this.UInt32TypeReference, this.DoubleTypeReference);
			}
		}

		private void WriteReturnStatement()
		{
			TypeReference typeReference = this._typeResolver.ResolveReturnType(this._methodDefinition);
			if ((typeReference.MetadataType == MetadataType.Void && this._valueStack.Count > 0) || (typeReference.MetadataType != MetadataType.Void && this._valueStack.Count > 1))
			{
				throw new InvalidOperationException(string.Format("Attempting to return a value from method '{0}' when there is no value on the stack. Is this invalid IL code?", this._methodDefinition.FullName));
			}
			if (typeReference.MetadataType != MetadataType.Void)
			{
				StackInfo right = this._valueStack.Pop();
				string text = MethodBodyWriter.CastIfPointerType(typeReference);
				if (typeReference.FullName != right.Type.FullName && typeReference.Resolve() != null && typeReference.Resolve().IsEnum)
				{
					this._writer.WriteLine("return ({0})({1});", new object[]
					{
						MethodBodyWriter.Naming.ForVariable(typeReference),
						right.Expression
					});
				}
				else if (!string.IsNullOrEmpty(text))
				{
					this._writer.WriteLine("return {0}({1});", new object[]
					{
						text,
						right.Expression
					});
				}
				else if (typeReference.MetadataType == MetadataType.IntPtr && right.Type.IsSameType(this.NativeIntTypeReference))
				{
					Local local = this.EmitLocalIntPtrWithValue(right.Expression);
					this._writer.WriteLine("return {0};", new object[]
					{
						local.Expression
					});
				}
				else if (typeReference.MetadataType == MetadataType.UIntPtr && right.Type.IsSameType(this.NativeIntTypeReference))
				{
					Local local2 = this.EmitLocalUIntPtrWithValue(right.Expression);
					this._writer.WriteLine("return {0};", new object[]
					{
						local2.Expression
					});
				}
				else
				{
					this._writer.WriteLine("return {0};", new object[]
					{
						MethodBodyWriter.WriteExpressionAndCastIfNeeded(typeReference, right, SharingType.NonShared)
					});
				}
			}
			else
			{
				this._writer.WriteLine("return;");
			}
		}

		private bool CanApplyValueTypeBoxBranchOptimizationToInstruction(Instruction ins, InstructionBlock block)
		{
			bool result;
			if (ins != null && ins.OpCode.Code == Code.Box)
			{
				TypeReference typeReference = this._typeResolver.Resolve((TypeReference)ins.Operand);
				result = (typeReference.IsValueType() && !typeReference.IsNullable() && ins != block.Last && ins.Next != null && (ins.Next.OpCode.Code == Code.Brtrue || ins.Next.OpCode.Code == Code.Brtrue_S || ins.Next.OpCode.Code == Code.Brfalse || ins.Next.OpCode.Code == Code.Brfalse_S));
			}
			else
			{
				result = false;
			}
			return result;
		}

		private string ConstrainedCallExpressionFor(MethodReference resolvedMethodToCall, ref MethodReference methodToCall, MethodCallType callType, List<StackInfo> poppedValues, Func<string, string> addUniqueSuffix)
		{
			StackInfo thisValue = poppedValues[0];
			ByReferenceType byReferenceType = thisValue.Type as ByReferenceType;
			if (byReferenceType == null)
			{
				throw new InvalidOperationException("Attempting to constrain an invalid type.");
			}
			TypeReference elementType = byReferenceType.ElementType;
			TypeReference typeReference = this._typeResolver.Resolve(this._constrainedCallThisType);
			if (!TypeReferenceEqualityComparer.AreEqual(elementType, typeReference, TypeComparisonMode.Exact))
			{
				throw new InvalidOperationException(string.Format("Attempting to constrain a value of type '{0}' to type '{1}'.", elementType, typeReference));
			}
			string result;
			if (!elementType.IsValueType())
			{
				poppedValues[0] = new StackInfo(Emit.Dereference(thisValue.Expression), elementType);
				this._writer.AddIncludeForTypeDefinition(resolvedMethodToCall.DeclaringType);
				result = this.CallExpressionFor(this._methodReference, methodToCall, callType, poppedValues, addUniqueSuffix, true);
			}
			else if (resolvedMethodToCall.IsGenericInstance)
			{
				poppedValues[0] = this.BoxThisForContraintedCallIntoNewTemp(thisValue);
				result = this.CallExpressionFor(this._methodReference, methodToCall, callType, poppedValues, addUniqueSuffix, true);
			}
			else if (MethodBodyWriter.GenericSharingAnalysis.IsGenericSharingForValueTypesEnabled && this._sharingType == SharingType.Shared)
			{
				string text = addUniqueSuffix("il2cpp_this_typeinfo");
				this._writer.WriteLine("Il2CppClass* {0} = {1};", new object[]
				{
					text,
					this._runtimeMetadataAccess.TypeInfoFor(this._constrainedCallThisType)
				});
				if (resolvedMethodToCall.DeclaringType.IsInterface())
				{
					this._writer.WriteLine("int32_t {0} = il2cpp_codegen_class_interface_offset({1}, {2});", new object[]
					{
						addUniqueSuffix("il2cpp_interface_offset_"),
						text,
						this._runtimeMetadataAccess.TypeInfoFor(resolvedMethodToCall.DeclaringType)
					});
				}
				string arg = MethodBodyWriter.InterfaceOffsetExpressionForDirectVirtualCall(resolvedMethodToCall, addUniqueSuffix);
				List<StackInfo> list = new List<StackInfo>(poppedValues);
				list[0] = new StackInfo(Emit.Call("Box", text, thisValue.Expression), MethodBodyWriter.TypeProvider.ObjectTypeReference);
				string arg2 = this.CallExpressionFor(this._methodReference, methodToCall, callType, list, addUniqueSuffix, false);
				List<StackInfo> list2 = new List<StackInfo>(poppedValues);
				list2[0] = new StackInfo(string.Format("il2cpp_codegen_fake_box({0})", list2[0].Expression), MethodBodyWriter.TypeProvider.ObjectTypeReference);
				string arg3 = this.CallExpressionFor(this._methodReference, methodToCall, MethodCallType.DirectVirtual, list2, addUniqueSuffix, false);
				string arg4 = Emit.Call("il2cpp_codegen_type_implements_virtual_method", text, this._vTableBuilder.IndexFor(methodToCall.Resolve()) + arg);
				result = string.Format("{0} ? {1} : {2}", arg4, arg3, arg2);
			}
			else
			{
				MethodReference virtualMethodTargetMethodForConstrainedCallOnValueType = this._vTableBuilder.GetVirtualMethodTargetMethodForConstrainedCallOnValueType(typeReference, resolvedMethodToCall);
				if (virtualMethodTargetMethodForConstrainedCallOnValueType != null && TypeReferenceEqualityComparer.AreEqual(virtualMethodTargetMethodForConstrainedCallOnValueType.DeclaringType, typeReference, TypeComparisonMode.Exact))
				{
					if (elementType.IsGenericInstance && elementType.IsValueType() && this._sharingType == SharingType.Shared)
					{
						string text2 = addUniqueSuffix("il2cpp_this_typeinfo");
						this._writer.WriteLine("Il2CppClass* {0} = {1};", new object[]
						{
							text2,
							this._runtimeMetadataAccess.TypeInfoFor(this._constrainedCallThisType)
						});
						if (resolvedMethodToCall.DeclaringType.IsInterface())
						{
							this._writer.WriteLine("int32_t {0} = il2cpp_codegen_class_interface_offset({1}, {2});", new object[]
							{
								addUniqueSuffix("il2cpp_interface_offset_"),
								text2,
								this._runtimeMetadataAccess.TypeInfoFor(resolvedMethodToCall.DeclaringType)
							});
						}
						List<StackInfo> list3 = new List<StackInfo>(poppedValues);
						list3[0] = new StackInfo(string.Format("il2cpp_codegen_fake_box({0})", list3[0].Expression), MethodBodyWriter.TypeProvider.ObjectTypeReference);
						result = this.CallExpressionFor(this._methodReference, methodToCall, MethodCallType.DirectVirtual, list3, addUniqueSuffix, false);
					}
					else
					{
						methodToCall = virtualMethodTargetMethodForConstrainedCallOnValueType;
						this._writer.AddIncludeForTypeDefinition(methodToCall.DeclaringType);
						this._writer.AddIncludeForTypeDefinition(this._typeResolver.ResolveReturnType(methodToCall));
						callType = MethodCallType.Normal;
						result = this.CallExpressionFor(this._methodReference, methodToCall, callType, poppedValues, addUniqueSuffix, true);
					}
				}
				else
				{
					poppedValues[0] = this.BoxThisForContraintedCallIntoNewTemp(thisValue);
					result = this.CallExpressionFor(this._methodReference, methodToCall, callType, poppedValues, addUniqueSuffix, true);
				}
			}
			return result;
		}

		private StackInfo BoxThisForContraintedCallIntoNewTemp(StackInfo thisValue)
		{
			Local local = this.NewTemp(MethodBodyWriter.TypeProvider.ObjectTypeReference);
			this._writer.WriteLine("{0} = {1};", new object[]
			{
				local.IdentifierExpression,
				Emit.Call("Box", this._runtimeMetadataAccess.TypeInfoFor(this._constrainedCallThisType), thisValue.Expression)
			});
			return new StackInfo(local.Expression, MethodBodyWriter.TypeProvider.ObjectTypeReference);
		}

		private void EmitCodeForLeaveFromTry(ExceptionSupport.Node node, Instruction ins)
		{
			int offset = ((Instruction)ins.Operand).Offset;
			ExceptionSupport.Node[] array = node.GetTargetFinallyNodesForJump(ins.Offset, offset).ToArray<ExceptionSupport.Node>();
			if (array.Length != 0)
			{
				ExceptionSupport.Node node2 = array.First<ExceptionSupport.Node>();
				ExceptionSupport.Node[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					ExceptionSupport.Node finallyNode = array2[i];
					this._exceptionSupport.AddLeaveTarget(finallyNode, ins);
				}
				this._writer.WriteLine("IL2CPP_LEAVE(0x{0:X}, {1});", new object[]
				{
					((Instruction)ins.Operand).Offset,
					this._labeler.FormatOffset(node2.Start)
				});
			}
			else
			{
				this._writer.WriteLine(this._labeler.ForJump(offset));
			}
		}

		private void EmitCodeForLeaveFromCatch(ExceptionSupport.Node node, Instruction ins)
		{
			int offset = ((Instruction)ins.Operand).Offset;
			ExceptionSupport.Node[] array = node.GetTargetFinallyNodesForJump(ins.Offset, offset).ToArray<ExceptionSupport.Node>();
			if (array.Length != 0)
			{
				ExceptionSupport.Node node2 = array.First<ExceptionSupport.Node>();
				ExceptionSupport.Node[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					ExceptionSupport.Node finallyNode = array2[i];
					this._exceptionSupport.AddLeaveTarget(finallyNode, ins);
				}
				this._writer.WriteLine("IL2CPP_LEAVE(0x{0:X}, {1});", new object[]
				{
					((Instruction)ins.Operand).Offset,
					this._labeler.FormatOffset(node2.Start)
				});
			}
			else
			{
				this._writer.WriteLine(this._labeler.ForJump(offset));
			}
		}

		private void EmitCodeForLeaveFromFinallyOrFault(Instruction ins)
		{
			this._writer.WriteLine(this._labeler.ForJump(((Instruction)ins.Operand).Offset));
		}

		private void EmitCodeForLeaveFromBlock(ExceptionSupport.Node node, Instruction ins)
		{
			int offset = ((Instruction)ins.Operand).Offset;
			if (!node.IsInTryBlock && !node.IsInCatchBlock)
			{
				this._writer.WriteLine(this._labeler.ForJump(offset));
			}
			else
			{
				ExceptionSupport.Node[] array = node.GetTargetFinallyNodesForJump(ins.Offset, offset).ToArray<ExceptionSupport.Node>();
				if (array.Length != 0)
				{
					ExceptionSupport.Node node2 = array.First<ExceptionSupport.Node>();
					ExceptionSupport.Node[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						ExceptionSupport.Node finallyNode = array2[i];
						this._exceptionSupport.AddLeaveTarget(finallyNode, ins);
					}
					this._writer.WriteLine("IL2CPP_LEAVE(0x{0:X}, {1});", new object[]
					{
						((Instruction)ins.Operand).Offset,
						this._labeler.FormatOffset(node2.Start)
					});
				}
				else
				{
					this._writer.WriteLine(this._labeler.ForJump(offset));
				}
			}
		}

		private bool ShouldStripLeaveInstruction(InstructionBlock block, Instruction ins)
		{
			return !this._labeler.NeedsLabel(ins) && (block.First == block.Last && block.First.Previous != null) && block.First.Previous.OpCode.Code == Code.Leave;
		}

		private void PushExpression(TypeReference typeReference, string expression)
		{
			this._valueStack.Push(new StackInfo(string.Format("({0})", expression), typeReference));
		}

		private string EmitArrayLoadElementAddress(StackInfo array, string indexExpression, TypeReference indexType)
		{
			string result;
			if (indexType.IsIntegralPointerType() || indexType.IsNativeIntegralType())
			{
				result = Emit.LoadArrayElementAddress(array.Expression, this.ArrayIndexerForIntPtr(indexExpression, indexType));
			}
			else
			{
				result = Emit.LoadArrayElementAddress(array.Expression, indexExpression);
			}
			return result;
		}

		private void WriteArrayBoundsCheckIfNeeded(StackInfo array, StackInfo index)
		{
			if (index.Type.IsIntegralPointerType() || index.Type.IsNativeIntegralType())
			{
				this._arrayBoundsCheckSupport.WriteArrayBoundsCheckIfNeeded(array.Expression, this.ArrayIndexerForIntPtr(index.Expression, index.Type));
			}
			else
			{
				this._arrayBoundsCheckSupport.WriteArrayBoundsCheckIfNeeded(array.Expression, index.Expression);
			}
		}

		private string ArrayIndexerForIntPtr(string indexExpression, TypeReference indexType)
		{
			return string.Format("({0})({1}){2}", MethodBodyWriter.Naming.ForArrayIndexType(), (!indexType.IsSameType(this.IntPtrTypeReference)) ? MethodBodyWriter.Naming.ForUIntPtrT : MethodBodyWriter.Naming.ForIntPtrT, this.FormatNativeIntGetterName(indexExpression, indexType));
		}

		private string FormatNativeIntGetterName(string variableName, TypeReference variableType)
		{
			string result;
			if (variableType.IsSameType(this.IntPtrTypeReference))
			{
				result = string.Format("{0}.{1}()", variableName, MethodBodyWriter.Naming.ForFieldGetter(MethodBodyWriter.TypeProvider.SystemIntPtr.Fields.Single((FieldDefinition f) => f.Name == MethodBodyWriter.Naming.IntPtrValueField)));
			}
			else if (variableType.IsSameType(this.UIntPtrTypeReference))
			{
				result = string.Format("{0}.{1}()", variableName, MethodBodyWriter.Naming.ForFieldGetter(MethodBodyWriter.TypeProvider.SystemUIntPtr.Fields.Single((FieldDefinition f) => f.Name == MethodBodyWriter.Naming.UIntPtrPointerField)));
			}
			else
			{
				if (!variableType.IsNativeIntegralType())
				{
					throw new ArgumentException("The variableType argument must be a TypeReference to an IntPtr or a an UIntPtr.", "variableType");
				}
				result = variableName;
			}
			return result;
		}

		private string FormatNativeIntSetterInvocation(string variableName, TypeReference variableType, string value)
		{
			string result;
			if (variableType.IsSameType(this.IntPtrTypeReference))
			{
				result = string.Format("{0}.{1}({2});", variableName, MethodBodyWriter.Naming.ForFieldSetter(MethodBodyWriter.TypeProvider.SystemIntPtr.Fields.Single((FieldDefinition f) => f.Name == MethodBodyWriter.Naming.IntPtrValueField)), value);
			}
			else if (variableType.IsSameType(this.UIntPtrTypeReference))
			{
				result = string.Format("{0}.{1}({2});", variableName, MethodBodyWriter.Naming.ForFieldSetter(MethodBodyWriter.TypeProvider.SystemUIntPtr.Fields.Single((FieldDefinition f) => f.Name == MethodBodyWriter.Naming.UIntPtrPointerField)), value);
			}
			else
			{
				if (!variableType.IsNativeIntegralType())
				{
					throw new ArgumentException("The variableType argument must be a TypeReference to an IntPtr or a an UIntPtr.", "variableType");
				}
				result = string.Format("{0} = {1};", variableName, value);
			}
			return result;
		}

		private void WriteCastclass(TypeReference typeReference1, StackInfo value, Instruction ins)
		{
			TypeReference typeReference2 = this._typeResolver.Resolve(typeReference1);
			TypeReference typeReference3 = (!typeReference2.IsValueType()) ? typeReference2 : MethodBodyWriter.TypeProvider.ObjectTypeReference;
			string expression = Emit.Cast(typeReference3, Emit.Call("Castclass", value.Expression, this._runtimeMetadataAccess.TypeInfoFor(typeReference1)));
			this.PushExpression(typeReference3, expression);
		}

		private void LoadArgumentAddress(ParameterReference parameter)
		{
			int index = parameter.Index;
			this._valueStack.Push(new StackInfo("(&" + MethodBodyWriter.ParameterNameFor(this._methodDefinition, index) + ")", new ByReferenceType(this._typeResolver.ResolveParameterType(this._methodReference, parameter))));
		}

		private void WriteLabelForBranchTarget(Instruction ins)
		{
			if (!this.DidAlreadyEmitLabelFor(ins))
			{
				this._emittedLabels.Add(ins);
				string arg = "";
				if (this._referencedLabels.Contains(ins))
				{
					this._writer.WriteLine();
					this._writer.WriteUnindented(string.Format("{0}{1}", arg, this._labeler.ForLabel(ins)), new object[0]);
				}
			}
		}

		private bool DidAlreadyEmitLabelFor(Instruction ins)
		{
			return this._emittedLabels.Contains(ins);
		}

		private void WriteJump(Instruction targetInstruction)
		{
			this._writer.WriteLine(this._labeler.ForJump(targetInstruction));
		}

		private void LoadLocalAddress(VariableReference variableReference)
		{
			this._valueStack.Push(new StackInfo("(&" + MethodBodyWriter.Naming.ForVariableName(variableReference) + ")", new ByReferenceType(this._typeResolver.Resolve(variableReference.VariableType))));
		}

		private void WriteDup()
		{
			StackInfo right = this._valueStack.Pop();
			if (right.Expression == MethodBodyWriter.Naming.ThisParameterName)
			{
				this._valueStack.Push(new StackInfo(MethodBodyWriter.Naming.ThisParameterName, right.Type));
				this._valueStack.Push(new StackInfo(MethodBodyWriter.Naming.ThisParameterName, right.Type));
			}
			else if (right.Expression == "NULL" && right.Type.IsSystemObject())
			{
				this._valueStack.Push(new StackInfo("NULL", this.ObjectTypeReference));
				this._valueStack.Push(new StackInfo("NULL", this.ObjectTypeReference));
			}
			else
			{
				Local local = this.NewTemp(right.Type);
				this.WriteAssignment(local.IdentifierExpression, right.Type, right);
				this._valueStack.Push(new StackInfo(local));
				this._valueStack.Push(new StackInfo(local));
			}
		}

		private void WriteNotOperation()
		{
			StackInfo stackInfo = this._valueStack.Pop();
			this.PushExpression(stackInfo.Type, string.Format("(~{0})", stackInfo.Expression));
		}

		private void WriteNegateOperation()
		{
			StackInfo originalValue = this._valueStack.Pop();
			TypeReference toType = this.CalculateResultTypeForNegate(originalValue.Type);
			this.PushExpression(originalValue.Type, string.Format("(-{0})", MethodBodyWriter.CastTypeIfNeeded(originalValue, toType)));
		}

		private TypeReference CalculateResultTypeForNegate(TypeReference type)
		{
			TypeReference result;
			if (type.IsUnsignedIntegralType())
			{
				if (type.MetadataType == MetadataType.Byte || type.MetadataType == MetadataType.UInt16 || type.MetadataType == MetadataType.UInt32)
				{
					result = MethodBodyWriter.TypeProvider.Int32TypeReference;
				}
				else
				{
					result = MethodBodyWriter.TypeProvider.Int64TypeReference;
				}
			}
			else
			{
				result = type;
			}
			return result;
		}

		private void LoadConstant(TypeReference type, string stringValue)
		{
			this.PushExpression(type, stringValue);
		}

		private void StoreLocalAndPush(TypeReference type, string stringValue)
		{
			Local local = this.NewTemp(type);
			this._writer.WriteLine("{0} = {1};", new object[]
			{
				local.IdentifierExpression,
				stringValue
			});
			this._valueStack.Push(new StackInfo(local));
		}

		private void StoreLocalIntPtrAndPush(string stringValue)
		{
			this._valueStack.Push(new StackInfo(this.EmitLocalIntPtrWithValue(stringValue)));
		}

		private Local EmitLocalIntPtrWithValue(string stringValue)
		{
			Local local = this.NewTemp(this.IntPtrTypeReference);
			this._writer.WriteLine("{0};", new object[]
			{
				local.IdentifierExpression
			});
			string text = MethodBodyWriter.Naming.ForFieldSetter(this.IntPtrTypeReference.Resolve().Fields.First((FieldDefinition f) => f.Name == MethodBodyWriter.Naming.IntPtrValueField));
			this._writer.WriteLine("{0}.{1}((void*){2});", new object[]
			{
				local.Expression,
				text,
				stringValue
			});
			return local;
		}

		private Local EmitLocalUIntPtrWithValue(string stringValue)
		{
			Local local = this.NewTemp(this.UIntPtrTypeReference);
			this._writer.WriteLine("{0};", new object[]
			{
				local.IdentifierExpression
			});
			string text = MethodBodyWriter.Naming.ForFieldSetter(this.UIntPtrTypeReference.Resolve().Fields.First((FieldDefinition f) => f.Name == MethodBodyWriter.Naming.UIntPtrPointerField));
			this._writer.WriteLine("{0}.{1}((void*){2});", new object[]
			{
				local.Expression,
				text,
				stringValue
			});
			return local;
		}

		private string CallExpressionFor(MethodReference callingMethod, MethodReference unresolvedMethodToCall, MethodCallType callType, List<StackInfo> poppedValues, Func<string, string> addUniqueSuffix, bool emitNullCheckForInvocation = true)
		{
			MethodReference methodReference = this._typeResolver.Resolve(unresolvedMethodToCall);
			TypeResolver typeResolverForMethodToCall = this._typeResolver;
			GenericInstanceMethod genericInstanceMethod = methodReference as GenericInstanceMethod;
			if (genericInstanceMethod != null)
			{
				typeResolverForMethodToCall = this._typeResolver.Nested(genericInstanceMethod);
			}
			List<TypeReference> parameterTypes = MethodBodyWriter.GetParameterTypes(methodReference, typeResolverForMethodToCall);
			if (methodReference.HasThis)
			{
				parameterTypes.Insert(0, (!methodReference.DeclaringType.IsValueType()) ? methodReference.DeclaringType : new ByReferenceType(methodReference.DeclaringType));
			}
			List<string> list = MethodBodyWriter.FormatArgumentsForMethodCall(parameterTypes, poppedValues, this._sharingType);
			if (MethodBodyWriter.NeedsNullArgForStaticMethod(methodReference, false))
			{
				list.Insert(0, (!CodeGenOptions.EmitComments) ? MethodBodyWriter.Naming.Null : "NULL /*static, unused*/");
			}
			if (MethodSignatureWriter.NeedsHiddenMethodInfo(methodReference, callType, false))
			{
				string str = (callType != MethodCallType.DirectVirtual) ? this._runtimeMetadataAccess.HiddenMethodInfo(unresolvedMethodToCall) : string.Format("{0}->vtable[{1}].method", addUniqueSuffix("il2cpp_this_typeinfo"), this._vTableBuilder.IndexFor(unresolvedMethodToCall.Resolve()) + MethodBodyWriter.InterfaceOffsetExpressionForDirectVirtualCall(unresolvedMethodToCall, addUniqueSuffix));
				list.Add(((!CodeGenOptions.EmitComments) ? "" : "/*hidden argument*/") + str);
			}
			if (emitNullCheckForInvocation)
			{
				this._nullCheckSupport.WriteNullCheckForInvocationIfNeeded(methodReference, list);
			}
			if (MethodBodyWriter.GenericSharingAnalysis.ShouldTryToCallStaticConstructorBeforeMethodCall(methodReference, this._methodReference))
			{
				this.WriteCallToClassAndInitializerAndStaticConstructorIfNeeded(unresolvedMethodToCall.DeclaringType, this._methodDefinition, this._runtimeMetadataAccess);
			}
			string methodCallExpression = MethodBodyWriter.GetMethodCallExpression(callingMethod, methodReference, unresolvedMethodToCall, typeResolverForMethodToCall, callType, this._runtimeMetadataAccess, this._vTableBuilder, list, addUniqueSuffix);
			if (callType != MethodCallType.Virtual || (MethodSignatureWriter.CanDevirtualizeMethodCall(methodReference.Resolve()) && unresolvedMethodToCall.DeclaringType.IsValueType()))
			{
				this._writer.AddIncludeForMethodDeclarations(methodReference.DeclaringType);
			}
			return methodCallExpression;
		}

		private void EmitCallExpressionAndStoreResult(Instruction instruction, TypeReference returnType, string callExpression)
		{
			if (returnType.IsVoid())
			{
				this._writer.WriteStatement(callExpression);
			}
			else if (instruction.Next != null && instruction.Next.OpCode.Code == Code.Pop)
			{
				this._writer.WriteStatement(callExpression);
				this._valueStack.Push(new StackInfo(MethodBodyWriter.Naming.Null, this.ObjectTypeReference));
			}
			else if (DebuggerOptions.Enabled)
			{
				Local local = this.NewTemp(returnType);
				this._writer.WriteStatement(Emit.Assign(local.IdentifierExpression, callExpression));
				DebuggerSupportFactory.GetDebuggerSupport().WriteSequencePoint(this._writer, instruction, true);
				Local local2 = this.NewTemp(returnType);
				this._valueStack.Push(new StackInfo(local2.Expression, returnType));
				this._writer.WriteLine(Emit.Assign(local2.IdentifierExpression, local.Expression));
			}
			else
			{
				Local local3 = this.NewTemp(returnType);
				this._valueStack.Push(new StackInfo(local3.Expression, returnType));
				this._writer.WriteStatement(Emit.Assign(local3.IdentifierExpression, callExpression));
			}
		}

		internal static string GetMethodCallExpression(MethodReference callingMethod, MethodReference methodToCall, MethodReference unresolvedMethodtoCall, TypeResolver typeResolverForMethodToCall, MethodCallType callType, IRuntimeMetadataAccess runtimeMetadataAccess, VTableBuilder vTableBuilder, IEnumerable<string> argumentArray, Func<string, string> addUniqueSuffix = null)
		{
			string result;
			if (methodToCall.DeclaringType.IsArray && methodToCall.Name == "Set")
			{
				result = MethodBodyWriter.GetArraySetCall(methodToCall, argumentArray.First<string>(), argumentArray.Skip(1).AggregateWithComma());
			}
			else if (methodToCall.DeclaringType.IsArray && methodToCall.Name == "Get")
			{
				result = MethodBodyWriter.GetArrayGetCall(methodToCall, argumentArray.First<string>(), argumentArray.Skip(1).AggregateWithComma());
			}
			else if (methodToCall.DeclaringType.IsArray && methodToCall.Name == "Address")
			{
				result = MethodBodyWriter.GetArrayAddressCall(methodToCall, argumentArray.First<string>(), argumentArray.Skip(1).AggregateWithComma());
			}
			else if (methodToCall.DeclaringType.IsSystemArray() && methodToCall.Name == "GetGenericValueImpl")
			{
				result = Emit.Call("ArrayGetGenericValueImpl", argumentArray);
			}
			else if (methodToCall.DeclaringType.IsSystemArray() && methodToCall.Name == "SetGenericValueImpl")
			{
				result = Emit.Call("ArraySetGenericValueImpl", argumentArray);
			}
			else if (GenericsUtilities.IsGenericInstanceOfCompareExchange(methodToCall))
			{
				GenericInstanceMethod genericInstanceMethod = (GenericInstanceMethod)methodToCall;
				string arg = MethodBodyWriter.Naming.ForVariable(genericInstanceMethod.GenericArguments[0]);
				result = Emit.Call(string.Format("InterlockedCompareExchangeImpl<{0}>", arg), argumentArray);
			}
			else if (GenericsUtilities.IsGenericInstanceOfExchange(methodToCall))
			{
				GenericInstanceMethod genericInstanceMethod2 = (GenericInstanceMethod)methodToCall;
				string arg2 = MethodBodyWriter.Naming.ForVariable(genericInstanceMethod2.GenericArguments[0]);
				result = Emit.Call(string.Format("InterlockedExchangeImpl<{0}>", arg2), argumentArray);
			}
			else if (IntrinsicRemap.ShouldRemap(methodToCall))
			{
				result = Emit.Call(IntrinsicRemap.MappedNameFor(methodToCall), (!IntrinsicRemap.HasCustomArguments(methodToCall)) ? argumentArray : IntrinsicRemap.GetCustomArguments(methodToCall, callingMethod, runtimeMetadataAccess, argumentArray));
			}
			else if (methodToCall.Resolve().IsStatic)
			{
				result = Emit.Call(runtimeMetadataAccess.Method(unresolvedMethodtoCall), argumentArray);
			}
			else if (callType == MethodCallType.DirectVirtual)
			{
				result = Emit.Call("(" + Emit.Cast(MethodSignatureWriter.GetMethodPointerForVTable(methodToCall), string.Format("{0}->vtable[{1}].methodPtr", addUniqueSuffix("il2cpp_this_typeinfo"), vTableBuilder.IndexFor(unresolvedMethodtoCall.Resolve()) + MethodBodyWriter.InterfaceOffsetExpressionForDirectVirtualCall(unresolvedMethodtoCall, addUniqueSuffix))) + ")", argumentArray);
			}
			else if (callType != MethodCallType.Virtual || MethodSignatureWriter.CanDevirtualizeMethodCall(methodToCall.Resolve()))
			{
				if (unresolvedMethodtoCall.DeclaringType.IsValueType())
				{
					result = Emit.Call(MethodBodyWriter.Naming.ForMethod(methodToCall), argumentArray);
				}
				else
				{
					result = Emit.Call(runtimeMetadataAccess.Method(unresolvedMethodtoCall), argumentArray);
				}
			}
			else
			{
				result = MethodBodyWriter.VirtualCallFor(methodToCall, unresolvedMethodtoCall, argumentArray, typeResolverForMethodToCall, runtimeMetadataAccess, vTableBuilder);
			}
			return result;
		}

		private static string InterfaceOffsetExpressionForDirectVirtualCall(MethodReference unresolvedMethodtoCall, Func<string, string> addUniqueSuffix)
		{
			string result;
			if (unresolvedMethodtoCall.DeclaringType.IsInterface())
			{
				result = string.Format(" + {0}", addUniqueSuffix("il2cpp_interface_offset_"));
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}

		private static string VirtualCallFor(MethodReference method, MethodReference unresolvedMethod, IEnumerable<string> args, TypeResolver typeResolver, IRuntimeMetadataAccess runtimeMetadataAccess, VTableBuilder vTableBuilder)
		{
			bool flag = method.ReturnType.MetadataType != MetadataType.Void;
			List<TypeReference> list = new List<TypeReference>();
			string empty = string.Empty;
			if (flag)
			{
				list.Add(typeResolver.ResolveReturnType(method));
			}
			list.AddRange(from p in method.Parameters
			select typeResolver.Resolve(GenericParameterResolver.ResolveParameterTypeIfNeeded(method, p)));
			string text = "";
			if (list.Count > 0)
			{
				text = "< " + list.Select(new Func<TypeReference, string>(MethodBodyWriter.Naming.ForVariable)).AggregateWithComma() + " >";
			}
			bool isInterface = method.DeclaringType.Resolve().IsInterface;
			string text2 = (!isInterface) ? "Virt" : "Interface";
			string method2 = string.Format("{0}{1}{2}{3}Invoker{4}{5}::Invoke", new object[]
			{
				empty,
				(!method.IsGenericInstance) ? ((!method.IsComOrWindowsRuntimeInterface()) ? string.Empty : "Com") : "Generic",
				text2,
				(!flag) ? "Action" : "Func",
				method.Parameters.Count,
				text
			});
			List<string> list2 = new List<string>();
			list2.Add((!method.IsGenericInstance) ? string.Concat(new object[]
			{
				vTableBuilder.IndexFor(method.Resolve()),
				" /* ",
				method.FullName,
				" */"
			}) : runtimeMetadataAccess.MethodInfo(unresolvedMethod));
			if (isInterface && !method.IsGenericInstance)
			{
				list2.Add(runtimeMetadataAccess.TypeInfoFor(unresolvedMethod.DeclaringType));
			}
			list2.AddRange(args);
			return Emit.Call(method2, list2);
		}

		private static string GetArrayAddressCall(MethodReference methodReference, string array, string arguments)
		{
			ArrayType arrayType = (ArrayType)methodReference.DeclaringType;
			return Emit.Call(string.Format("({0})->{1}", array, MethodBodyWriter.Naming.ForArrayItemAddressGetter()), arguments);
		}

		private static string GetArrayGetCall(MethodReference methodReference, string array, string arguments)
		{
			ArrayType arrayType = (ArrayType)methodReference.DeclaringType;
			return Emit.Call(string.Format("({0})->{1}", array, MethodBodyWriter.Naming.ForArrayItemGetter()), arguments);
		}

		private static string GetArraySetCall(MethodReference methodReference, string array, string arguments)
		{
			ArrayType arrayType = (ArrayType)methodReference.DeclaringType;
			return Emit.Call(string.Format("({0})->{1}", array, MethodBodyWriter.Naming.ForArrayItemSetter()), arguments);
		}

		private void WriteUnconditionalJumpTo(InstructionBlock block, Instruction target)
		{
			if (block.Successors.Count<InstructionBlock>() != 1)
			{
				throw new ArgumentException("Expected only one successor for the current block", "target");
			}
			this.WriteAssignGlobalVariables(this._stackAnalysis.InputVariablesFor(block.Successors.Single<InstructionBlock>()));
			this.WriteJump(target);
		}

		private void WriteCastclassOrIsInst(TypeReference targetType, StackInfo value, string operation)
		{
			TypeReference typeReference = this._typeResolver.Resolve(targetType);
			TypeReference typeReference2 = (!typeReference.IsValueType()) ? typeReference : MethodBodyWriter.TypeProvider.ObjectTypeReference;
			this._writer.AddIncludeForTypeDefinition(typeReference);
			this.PushExpression(typeReference2, Emit.Cast(typeReference2, this.GetCastclassOrIsInstCall(targetType, value, operation, typeReference)));
		}

		private string GetCastclassOrIsInstCall(TypeReference targetType, StackInfo value, string operation, TypeReference resolvedTypeReference)
		{
			return Emit.Call(operation + MethodBodyWriter.GetOptimizedCastclassOrIsInstMethodSuffix(resolvedTypeReference, this._sharingType), value.Expression, this._runtimeMetadataAccess.TypeInfoFor(targetType));
		}

		private static string GetOptimizedCastclassOrIsInstMethodSuffix(TypeReference resolvedTypeReference, SharingType sharingType)
		{
			string result;
			if (sharingType == SharingType.NonShared && !resolvedTypeReference.IsInterface() && !resolvedTypeReference.IsArray && !resolvedTypeReference.IsNullable())
			{
				bool isSealed = resolvedTypeReference.Resolve().IsSealed;
				result = ((!isSealed) ? "Class" : "Sealed");
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}

		private void Unbox(Instruction ins)
		{
			StackInfo stackInfo = this._valueStack.Pop();
			TypeReference typeReference = this._typeResolver.Resolve((TypeReference)ins.Operand);
			this._writer.AddIncludeForTypeDefinition(typeReference);
			this.PushExpression(new ByReferenceType(typeReference), Emit.Cast(new PointerType(typeReference), this.Unbox(typeReference, stackInfo.Expression)));
		}

		private string Unbox(TypeReference type, string boxedExpression)
		{
			string arg = this._runtimeMetadataAccess.TypeInfoFor(type);
			TypeReference typeReference = this._typeResolver.Resolve(type);
			string result;
			if (typeReference.IsNullable())
			{
				string text = this.NewTempName();
				this._writer.WriteLine(string.Format("void* {0} = alloca(sizeof({1}));", text, MethodBodyWriter.Naming.ForVariable(typeReference)));
				this._writer.WriteLine(string.Format("UnBoxNullable({0}, {1}, {2});", boxedExpression, arg, text));
				result = text;
			}
			else
			{
				result = string.Format("UnBox ({0}, {1})", boxedExpression, arg);
			}
			return result;
		}

		private void WriteUnsignedArithmeticOperation(string op)
		{
			StackInfo stackInfo = this._valueStack.Pop();
			StackInfo stackInfo2 = this._valueStack.Pop();
			TypeReference typeReference = StackTypeConverter.StackTypeForBinaryOperation(stackInfo2.Type);
			TypeReference typeReference2 = StackTypeConverter.StackTypeForBinaryOperation(stackInfo.Type);
			TypeReference typeReference3 = (this.GetMetadataTypeOrderFor(typeReference) >= this.GetMetadataTypeOrderFor(typeReference2)) ? this.GetUnsignedType(typeReference) : this.GetUnsignedType(typeReference2);
			this.WriteBinaryOperation(this.GetSignedType(typeReference3), string.Format("({0})({1})", MethodBodyWriter.Naming.ForVariable(typeReference3), MethodBodyWriter.Naming.ForVariable(typeReference)), this.ExpressionForBinaryOperation(stackInfo2.Type, stackInfo2.Expression), op, string.Format("({0})({1})", MethodBodyWriter.Naming.ForVariable(typeReference3), MethodBodyWriter.Naming.ForVariable(typeReference2)), this.ExpressionForBinaryOperation(stackInfo.Type, stackInfo.Expression));
		}

		private TypeReference GetUnsignedType(TypeReference type)
		{
			TypeReference result;
			if (type.IsSameType(this.NativeIntTypeReference) || type.IsSameType(this.NativeUIntTypeReference))
			{
				result = this.NativeUIntTypeReference;
			}
			else
			{
				MetadataType metadataType = type.MetadataType;
				switch (metadataType)
				{
				case MetadataType.SByte:
				case MetadataType.Byte:
					result = this.ByteTypeReference;
					break;
				case MetadataType.Int16:
				case MetadataType.UInt16:
					result = this.UInt16TypeReference;
					break;
				case MetadataType.Int32:
				case MetadataType.UInt32:
					result = this.UInt32TypeReference;
					break;
				case MetadataType.Int64:
				case MetadataType.UInt64:
					result = this.UInt64TypeReference;
					break;
				default:
					if (metadataType != MetadataType.IntPtr && metadataType != MetadataType.UIntPtr)
					{
						result = type;
					}
					else
					{
						result = this.NativeUIntTypeReference;
					}
					break;
				}
			}
			return result;
		}

		private TypeReference GetSignedType(TypeReference type)
		{
			TypeReference result;
			if (type.IsSameType(this.NativeIntTypeReference) || type.IsSameType(this.NativeUIntTypeReference))
			{
				result = this.NativeIntTypeReference;
			}
			else
			{
				MetadataType metadataType = type.MetadataType;
				switch (metadataType)
				{
				case MetadataType.SByte:
				case MetadataType.Byte:
					result = this.SByteTypeReference;
					break;
				case MetadataType.Int16:
				case MetadataType.UInt16:
					result = this.Int16TypeReference;
					break;
				case MetadataType.Int32:
				case MetadataType.UInt32:
					result = this.Int32TypeReference;
					break;
				case MetadataType.Int64:
				case MetadataType.UInt64:
					result = this.Int64TypeReference;
					break;
				default:
					if (metadataType != MetadataType.IntPtr && metadataType != MetadataType.UIntPtr)
					{
						result = type;
					}
					else
					{
						result = this.NativeIntTypeReference;
					}
					break;
				}
			}
			return result;
		}

		private int GetMetadataTypeOrderFor(TypeReference type)
		{
			int result;
			if (type.IsSameType(this.NativeIntTypeReference) || type.IsSameType(this.NativeUIntTypeReference))
			{
				result = 3;
			}
			else
			{
				MetadataType metadataType = type.MetadataType;
				switch (metadataType)
				{
				case MetadataType.SByte:
				case MetadataType.Byte:
					result = 0;
					return result;
				case MetadataType.Int16:
				case MetadataType.UInt16:
					result = 1;
					return result;
				case MetadataType.Int32:
				case MetadataType.UInt32:
					result = 2;
					return result;
				case MetadataType.Int64:
				case MetadataType.UInt64:
					result = 4;
					return result;
				case MetadataType.Single:
				case MetadataType.Double:
				case MetadataType.String:
					IL_69:
					if (metadataType != MetadataType.IntPtr && metadataType != MetadataType.UIntPtr)
					{
						throw new Exception(string.Format("Invalid metadata type for typereference {0}", type));
					}
					goto IL_93;
				case MetadataType.Pointer:
					goto IL_93;
				}
				goto IL_69;
				IL_93:
				result = 3;
			}
			return result;
		}

		private void StoreField(Instruction ins)
		{
			StackInfo right = this._valueStack.Pop();
			StackInfo stackInfo = this._valueStack.Pop();
			FieldReference fieldReference = (FieldReference)ins.Operand;
			if (stackInfo.Expression != MethodBodyWriter.Naming.ThisParameterName)
			{
				this._nullCheckSupport.WriteNullCheckIfNeeded(stackInfo);
			}
			this.EmitMemoryBarrierIfNecessary(fieldReference);
			this._writer.WriteLine("{0}->{1}({2});", new object[]
			{
				MethodBodyWriter.CastReferenceTypeOrNativeIntIfNeeded(stackInfo, this._typeResolver.Resolve(fieldReference.DeclaringType)),
				MethodBodyWriter.Naming.ForFieldSetter(fieldReference),
				MethodBodyWriter.WriteExpressionAndCastIfNeeded(this._typeResolver.ResolveFieldType(fieldReference), right, SharingType.NonShared)
			});
		}

		private static string CastReferenceTypeOrNativeIntIfNeeded(StackInfo originalValue, TypeReference toType)
		{
			string result;
			if (!toType.IsValueType())
			{
				result = MethodBodyWriter.CastTypeIfNeeded(originalValue, toType);
			}
			else if (originalValue.Type.IsNativeIntegralType())
			{
				result = MethodBodyWriter.CastTypeIfNeeded(originalValue, new ByReferenceType(toType));
			}
			else
			{
				result = originalValue.Expression;
			}
			return result;
		}

		private static string CastTypeIfNeeded(StackInfo originalValue, TypeReference toType)
		{
			string result;
			if (!TypeReferenceEqualityComparer.AreEqual(originalValue.Type, toType, TypeComparisonMode.Exact))
			{
				result = string.Format("({0})", Emit.Cast(toType, originalValue.Expression));
			}
			else
			{
				result = originalValue.Expression;
			}
			return result;
		}

		private static string CastIfPointerType(TypeReference type)
		{
			string result = string.Empty;
			if (type.IsPointer)
			{
				result = "(" + MethodBodyWriter.Naming.ForVariable(type) + ")";
			}
			return result;
		}

		private void WriteAdd(MethodBodyWriter.OverflowCheck check)
		{
			StackInfo right = this._valueStack.Pop();
			StackInfo left = this._valueStack.Pop();
			if (check != MethodBodyWriter.OverflowCheck.None)
			{
				TypeReference typeReference = StackTypeConverter.StackTypeFor(left.Type);
				TypeReference typeReference2 = StackTypeConverter.StackTypeFor(right.Type);
				if (this.RequiresPointerOverflowCheck(typeReference, typeReference2))
				{
					this.WritePointerOverflowCheckUsing64Bits("+", check, left.Expression, right.Expression);
				}
				else if (MethodBodyWriter.Requires64BitOverflowCheck(typeReference.MetadataType, typeReference2.MetadataType))
				{
					if (check == MethodBodyWriter.OverflowCheck.Signed)
					{
						this._writer.WriteLine("if (((int64_t){1} >= 0 && (int64_t){0} > kIl2CppInt64Max - (int64_t){1}) || ((int64_t){1} < 0 && (int64_t){0} < (int64_t)kIl2CppInt64Min - (int64_t){1}))", new object[]
						{
							left.Expression,
							right.Expression
						});
						this._writer.WriteLine("\t{0};", new object[]
						{
							Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()")
						});
					}
					else
					{
						this._writer.WriteLine("if ((uint64_t){0} > kIl2CppUInt64Max - (uint64_t){1})", new object[]
						{
							left.Expression,
							right.Expression
						});
						this._writer.WriteLine("\t{0};", new object[]
						{
							Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()")
						});
					}
				}
				else
				{
					this.WriteNarrowOverflowCheckUsing64Bits("+", check, left.Expression, right.Expression);
				}
			}
			TypeReference leftType = MethodBodyWriter.Naming.RemoveModifiers(left.Type);
			TypeReference rightType = MethodBodyWriter.Naming.RemoveModifiers(right.Type);
			TypeReference resultType = StackAnalysisUtils.ResultTypeForAdd(leftType, rightType, MethodBodyWriter.TypeProvider);
			this.WriteBinaryOperation("+", left, right, resultType);
		}

		private void WriteSub(MethodBodyWriter.OverflowCheck check)
		{
			StackInfo right = this._valueStack.Pop();
			StackInfo left = this._valueStack.Pop();
			if (check != MethodBodyWriter.OverflowCheck.None)
			{
				TypeReference typeReference = StackTypeConverter.StackTypeFor(left.Type);
				TypeReference typeReference2 = StackTypeConverter.StackTypeFor(right.Type);
				if (this.RequiresPointerOverflowCheck(typeReference, typeReference2))
				{
					this.WritePointerOverflowCheckUsing64Bits("-", check, left.Expression, right.Expression);
				}
				else if (MethodBodyWriter.Requires64BitOverflowCheck(typeReference.MetadataType, typeReference2.MetadataType))
				{
					if (check == MethodBodyWriter.OverflowCheck.Signed)
					{
						this._writer.WriteLine("if (((int64_t){1} >= 0 && (int64_t){0} < kIl2CppInt64Min + (int64_t){1}) || ((int64_t){1} < 0 && (int64_t){0} > kIl2CppInt64Max + (int64_t){1}))", new object[]
						{
							left.Expression,
							right.Expression
						});
						this._writer.WriteLine("\t{0};", new object[]
						{
							Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()")
						});
					}
					else
					{
						this._writer.WriteLine("if ((uint64_t){0} < (uint64_t){1})", new object[]
						{
							left.Expression,
							right.Expression
						});
						this._writer.WriteLine("\t{0};", new object[]
						{
							Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()")
						});
					}
				}
				else
				{
					this.WriteNarrowOverflowCheckUsing64Bits("-", check, left.Expression, right.Expression);
				}
			}
			TypeReference leftType = MethodBodyWriter.Naming.RemoveModifiers(left.Type);
			TypeReference rightType = MethodBodyWriter.Naming.RemoveModifiers(right.Type);
			TypeReference resultType = StackAnalysisUtils.ResultTypeForSub(leftType, rightType, MethodBodyWriter.TypeProvider);
			this.WriteBinaryOperation("-", left, right, resultType);
		}

		private void WriteMul(MethodBodyWriter.OverflowCheck check)
		{
			StackInfo right = this._valueStack.Pop();
			StackInfo left = this._valueStack.Pop();
			if (check != MethodBodyWriter.OverflowCheck.None)
			{
				TypeReference typeReference = StackTypeConverter.StackTypeFor(left.Type);
				TypeReference typeReference2 = StackTypeConverter.StackTypeFor(right.Type);
				if (this.RequiresPointerOverflowCheck(typeReference, typeReference2))
				{
					this.WritePointerOverflowCheckUsing64Bits("*", check, left.Expression, right.Expression);
				}
				else if (MethodBodyWriter.Requires64BitOverflowCheck(typeReference.MetadataType, typeReference2.MetadataType))
				{
					if (check == MethodBodyWriter.OverflowCheck.Signed)
					{
						this._writer.WriteLine("if (il2cpp_codegen_check_mul_overflow_i64((int64_t){0}, (int64_t){1}, kIl2CppInt64Min, kIl2CppInt64Max))", new object[]
						{
							left.Expression,
							right.Expression
						});
						this._writer.WriteLine("\t{0};", new object[]
						{
							Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()")
						});
					}
					else
					{
						this._writer.WriteLine("if ((uint64_t){1} != 0 && (((uint64_t){0} * (uint64_t){1}) / (uint64_t){1} != (uint64_t){0}))", new object[]
						{
							left.Expression,
							right.Expression
						});
						this._writer.WriteLine("\t{0};", new object[]
						{
							Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()")
						});
					}
				}
				else
				{
					this.WriteNarrowOverflowCheckUsing64Bits("*", check, left.Expression, right.Expression);
				}
			}
			TypeReference leftType = MethodBodyWriter.Naming.RemoveModifiers(left.Type);
			TypeReference rightType = MethodBodyWriter.Naming.RemoveModifiers(right.Type);
			TypeReference resultType = StackAnalysisUtils.ResultTypeForMul(leftType, rightType, MethodBodyWriter.TypeProvider);
			this.WriteBinaryOperation("*", left, right, resultType);
		}

		private void WritePointerOverflowCheckUsing64Bits(string op, MethodBodyWriter.OverflowCheck check, string leftExpression, string rightExpression)
		{
			if (check == MethodBodyWriter.OverflowCheck.Signed)
			{
				this._writer.WriteLine("if (((intptr_t){1} {0} (intptr_t){2} < (intptr_t)kIl2CppIntPtrMin) || ((intptr_t){1} {0} (intptr_t){2} > (intptr_t)kIl2CppIntPtrMax))", new object[]
				{
					op,
					leftExpression,
					rightExpression
				});
				this._writer.WriteLine("\t{0};", new object[]
				{
					Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()")
				});
			}
			else
			{
				this._writer.WriteLine("if ((uintptr_t){1} {0} (uintptr_t){2} > (uintptr_t)kIl2CppUIntPtrMax)", new object[]
				{
					op,
					leftExpression,
					rightExpression
				});
				this._writer.WriteLine("\t{0};", new object[]
				{
					Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()")
				});
			}
		}

		private void WriteNarrowOverflowCheckUsing64Bits(string op, MethodBodyWriter.OverflowCheck check, string leftExpression, string rightExpression)
		{
			if (check == MethodBodyWriter.OverflowCheck.Signed)
			{
				this._writer.WriteLine("if (((int64_t){1} {0} (int64_t){2} < (int64_t)kIl2CppInt32Min) || ((int64_t){1} {0} (int64_t){2} > (int64_t)kIl2CppInt32Max))", new object[]
				{
					op,
					leftExpression,
					rightExpression
				});
				this._writer.WriteLine("\t{0};", new object[]
				{
					Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()")
				});
			}
			else
			{
				this._writer.WriteLine("if ((uint64_t)(uint32_t){1} {0} (uint64_t)(uint32_t){2} > (uint64_t)(uint32_t)kIl2CppUInt32Max)", new object[]
				{
					op,
					leftExpression,
					rightExpression
				});
				this._writer.WriteLine("\t{0};", new object[]
				{
					Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()")
				});
			}
		}

		private static bool Requires64BitOverflowCheck(MetadataType leftStackType, MetadataType rightStackType)
		{
			return MethodBodyWriter.Requires64BitOverflowCheck(leftStackType) || MethodBodyWriter.Requires64BitOverflowCheck(rightStackType);
		}

		private static bool Requires64BitOverflowCheck(MetadataType metadataType)
		{
			return metadataType == MetadataType.UInt64 || metadataType == MetadataType.Int64;
		}

		private bool RequiresPointerOverflowCheck(TypeReference leftStackType, TypeReference rightStackType)
		{
			return this.RequiresPointerOverflowCheck(leftStackType) || this.RequiresPointerOverflowCheck(rightStackType);
		}

		private bool RequiresPointerOverflowCheck(TypeReference type)
		{
			return type.IsSameType(MethodBodyWriter.TypeProvider.NativeIntTypeReference) || type.IsSameType(MethodBodyWriter.TypeProvider.NativeUIntTypeReference);
		}

		private StackInfo FormatLoadTokenFor(Instruction ins)
		{
			object operand = ins.Operand;
			TypeReference typeReference = operand as TypeReference;
			StackInfo result;
			if (typeReference != null)
			{
				string expression = string.Format("LoadTypeToken({0})", this._runtimeMetadataAccess.Il2CppTypeFor(typeReference));
				result = new StackInfo(expression, this.RuntimeTypeHandleTypeReference);
			}
			else
			{
				FieldReference fieldReference = operand as FieldReference;
				if (fieldReference != null)
				{
					this._writer.AddIncludeForTypeDefinition(this._typeResolver.Resolve(fieldReference.DeclaringType));
					string expression = string.Format("LoadFieldToken({0})", this._runtimeMetadataAccess.FieldInfo(fieldReference));
					result = new StackInfo(expression, this.RuntimeFieldHandleTypeReference);
				}
				else
				{
					MethodReference methodReference = operand as MethodReference;
					if (methodReference == null)
					{
						throw new ArgumentException();
					}
					this._writer.AddIncludeForTypeDefinition(this._typeResolver.Resolve(methodReference.DeclaringType));
					string expression = string.Format("LoadMethodToken({0})", this._runtimeMetadataAccess.MethodInfo(methodReference));
					result = new StackInfo(expression, this.RuntimeMethodHandleTypeReference);
				}
			}
			return result;
		}

		private void LoadField(Instruction ins, bool loadAddress = false)
		{
			StackInfo stackInfo = this._valueStack.Pop();
			FieldReference fieldReference = (FieldReference)ins.Operand;
			TypeReference type = this._typeResolver.ResolveFieldType(fieldReference);
			string right;
			if (loadAddress)
			{
				type = new ByReferenceType(type);
				right = MethodBodyWriter.Naming.ForFieldAddressGetter(fieldReference);
			}
			else
			{
				right = MethodBodyWriter.Naming.ForFieldGetter(fieldReference);
			}
			if (stackInfo.Expression != MethodBodyWriter.Naming.ThisParameterName)
			{
				this._nullCheckSupport.WriteNullCheckIfNeeded(stackInfo);
			}
			Local local = this.NewTemp(type);
			this._valueStack.Push(new StackInfo(local));
			string text = Emit.Call((!stackInfo.Type.IsValueType() || stackInfo.Type.IsNativeIntegralType()) ? Emit.Arrow(MethodBodyWriter.CastReferenceTypeOrNativeIntIfNeeded(stackInfo, this._typeResolver.Resolve(fieldReference.DeclaringType)), right) : Emit.Dot(stackInfo.Expression, right));
			if (this._sharingType == SharingType.Shared)
			{
				text = Emit.Cast(type, text);
			}
			string str = Emit.Assign(local.IdentifierExpression, text);
			this._writer.WriteLine(str + ";");
			this.EmitMemoryBarrierIfNecessary(fieldReference);
		}

		private void StaticFieldAccess(Instruction ins)
		{
			FieldReference fieldReference = (FieldReference)ins.Operand;
			if (fieldReference.Resolve().IsLiteral)
			{
				throw new Exception("literal values should always be embedded rather than accessed via the field itself");
			}
			this.WriteCallToClassAndInitializerAndStaticConstructorIfNeeded(fieldReference.DeclaringType, this._methodDefinition, this._runtimeMetadataAccess);
			TypeReference typeReference = this._typeResolver.ResolveFieldType(fieldReference);
			string arg = MethodBodyWriter.TypeStaticsExpressionFor(fieldReference, this._typeResolver, this._runtimeMetadataAccess);
			if (ins.OpCode.Code == Code.Stsfld)
			{
				StackInfo right = this._valueStack.Pop();
				this.EmitMemoryBarrierIfNecessary(null);
				this._writer.WriteLine(Statement.Expression(Emit.Call(string.Format("{0}{1}", arg, MethodBodyWriter.Naming.ForFieldSetter(fieldReference)), MethodBodyWriter.WriteExpressionAndCastIfNeeded(typeReference, right, SharingType.NonShared))));
			}
			else
			{
				if (ins.OpCode.Code == Code.Ldsflda)
				{
					ByReferenceType typeReference2 = new ByReferenceType(typeReference);
					string expression = Emit.Call(string.Format("{0}{1}", arg, MethodBodyWriter.Naming.ForFieldAddressGetter(fieldReference)));
					this.PushExpression(typeReference2, expression);
				}
				else
				{
					Local local = this.NewTemp(typeReference);
					this._writer.WriteLine("{0};", new object[]
					{
						Emit.Assign(local.IdentifierExpression, Emit.Call(string.Format("{0}{1}", arg, MethodBodyWriter.Naming.ForFieldGetter(fieldReference))))
					});
					this._valueStack.Push(new StackInfo(local));
				}
				this.EmitMemoryBarrierIfNecessary(null);
			}
		}

		private void WriteCallToClassAndInitializerAndStaticConstructorIfNeeded(TypeReference type, MethodDefinition invokingMethod, IRuntimeMetadataAccess runtimeMetadataAccess)
		{
			if (type.HasStaticConstructor())
			{
				if (!this._classesAlreadyInitializedInBlock.Contains(type))
				{
					this._classesAlreadyInitializedInBlock.Add(type);
					string argument = runtimeMetadataAccess.StaticData(type);
					IEnumerable<MethodDefinition> arg_64_0 = type.Resolve().Methods;
					if (MethodBodyWriter.<>f__mg$cache0 == null)
					{
						MethodBodyWriter.<>f__mg$cache0 = new Func<MethodDefinition, bool>(Extensions.IsStaticConstructor);
					}
					MethodDefinition methodDefinition = arg_64_0.Single(MethodBodyWriter.<>f__mg$cache0);
					if (invokingMethod == null || methodDefinition != invokingMethod)
					{
						this._writer.WriteLine(Statement.Expression(Emit.Call("IL2CPP_RUNTIME_CLASS_INIT", argument)));
					}
				}
			}
		}

		internal static string TypeStaticsExpressionFor(FieldReference fieldReference, TypeResolver typeResolver, IRuntimeMetadataAccess runtimeMetadataAccess)
		{
			TypeReference type = typeResolver.Resolve(fieldReference.DeclaringType);
			string arg = runtimeMetadataAccess.StaticData(fieldReference.DeclaringType);
			string result;
			if (fieldReference.IsThreadStatic())
			{
				result = string.Format("(({0}*)il2cpp_codegen_get_thread_static_data({1}))->", MethodBodyWriter.Naming.ForThreadFieldsStruct(type), arg);
			}
			else
			{
				result = string.Format("(({0}*){1}->static_fields)->", MethodBodyWriter.Naming.ForStaticFieldsStruct(type), arg);
			}
			return result;
		}

		private void LoadIndirect(TypeReference valueType, TypeReference storageType)
		{
			StackInfo stackInfo = this._valueStack.Pop();
			if (this._thisInstructionIsVolatile)
			{
				Local local = this.NewTemp(storageType);
				this._writer.WriteLine("{0} = {1};", new object[]
				{
					local.IdentifierExpression,
					MethodBodyWriter.GetLoadIndirectExpression(new PointerType(valueType), stackInfo.Expression)
				});
				this.EmitMemoryBarrierIfNecessary(null);
				this._valueStack.Push(new StackInfo(local));
			}
			else
			{
				this.PushLoadIndirectExpression(storageType, new PointerType(valueType), stackInfo.Expression);
			}
		}

		private void LoadIndirectReference()
		{
			StackInfo address = this._valueStack.Pop();
			this.PushLoadIndirectExpression(MethodBodyWriter.GetPointerOrByRefType(address), address.Type, address.Expression);
		}

		private void LoadIndirectNativeInteger()
		{
			StackInfo address = this._valueStack.Pop();
			TypeReference pointerOrByRefType = MethodBodyWriter.GetPointerOrByRefType(address);
			if (pointerOrByRefType.IsIntegralPointerType())
			{
				this.PushExpression(pointerOrByRefType, string.Format("(*({0}))", address.Expression));
			}
			else if (pointerOrByRefType.IsVoid() || address.Type.IsByReference)
			{
				this.PushLoadIndirectExpression(this.NativeIntTypeReference, new PointerType(this.NativeIntTypeReference), address.Expression);
			}
			else
			{
				this.PushLoadIndirectExpression(pointerOrByRefType, new PointerType(pointerOrByRefType), address.Expression);
			}
		}

		private void PushLoadIndirectExpression(TypeReference expressionType, TypeReference castType, string expression)
		{
			this.PushExpression(expressionType, MethodBodyWriter.GetLoadIndirectExpression(castType, expression));
		}

		private static string GetLoadIndirectExpression(TypeReference castType, string expression)
		{
			return string.Format("*(({0}){1})", MethodBodyWriter.Naming.ForVariable(castType), expression);
		}

		private static TypeReference GetPointerOrByRefType(StackInfo address)
		{
			TypeReference typeReference = address.Type;
			typeReference = MethodBodyWriter.Naming.RemoveModifiers(typeReference);
			PointerType pointerType = typeReference as PointerType;
			TypeReference elementType;
			if (pointerType != null)
			{
				elementType = pointerType.ElementType;
			}
			else
			{
				ByReferenceType byReferenceType = typeReference as ByReferenceType;
				if (byReferenceType == null)
				{
					throw new Exception();
				}
				elementType = byReferenceType.ElementType;
			}
			return elementType;
		}

		private void ConvertToNaturalInt(TypeReference pointerType)
		{
			StackInfo stackInfo = this._valueStack.Pop();
			this.PushExpression(pointerType, string.Format("(({0}){1})", MethodBodyWriter.Naming.ForVariable(pointerType), stackInfo.Expression));
		}

		private void ConvertToNaturalIntWithOverflow<TMaxValueType>(TypeReference pointerType, bool treatInputAsUnsigned, TMaxValueType maxValue)
		{
			this.WriteCheckForOverflow<TMaxValueType>(treatInputAsUnsigned, maxValue, false);
			this.ConvertToNaturalInt(pointerType);
		}

		private void WriteCheckForOverflow<TMaxValue>(bool treatInputAsUnsigned, TMaxValue maxValue, bool inputIsNumber)
		{
			StackInfo stackInfo = this._valueStack.Peek();
			if (stackInfo.Type.IsSameType(this.DoubleTypeReference) || stackInfo.Type.IsSameType(this.SingleTypeReference))
			{
				this._writer.WriteLine("if ({0} > (double)({1})) {2};", new object[]
				{
					stackInfo.Expression,
					maxValue,
					Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()")
				});
			}
			else if (treatInputAsUnsigned)
			{
				this._writer.WriteLine("if ((uint64_t)({0}) > {1}{2}) {3};", new object[]
				{
					stackInfo.Expression,
					maxValue,
					(!inputIsNumber) ? "" : "LL",
					Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()")
				});
			}
			else
			{
				this._writer.WriteLine("if ((int64_t)({0}) > {1}{2}) {3};", new object[]
				{
					stackInfo.Expression,
					maxValue,
					(!inputIsNumber) ? "" : "LL",
					Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()")
				});
			}
		}

		private void LoadElemAndPop(TypeReference typeReference)
		{
			StackInfo index = this._valueStack.Pop();
			StackInfo array = this._valueStack.Pop();
			this.LoadElem(array, typeReference, index);
		}

		private void StoreArg(Instruction ins)
		{
			StackInfo right = this._valueStack.Pop();
			ParameterReference parameterReference = (ParameterReference)ins.Operand;
			this.WriteAssignment(MethodBodyWriter.Naming.ForParameterName(parameterReference), this._typeResolver.ResolveParameterType(this._methodReference, parameterReference), right);
		}

		private void LoadElem(StackInfo array, TypeReference objectType, StackInfo index)
		{
			this._nullCheckSupport.WriteNullCheckIfNeeded(array);
			this.WriteArrayBoundsCheckIfNeeded(array, index);
			Local local = this.NewTemp(index.Type);
			this._writer.WriteLine("{0} = {1};", new object[]
			{
				local.IdentifierExpression,
				index.Expression
			});
			string index2 = local.Expression;
			if (local.Type.IsIntegralPointerType())
			{
				index2 = this.ArrayIndexerForIntPtr(local.Expression, local.Type);
			}
			this.StoreLocalAndPush(objectType, Emit.LoadArrayElement(array.Expression, index2));
		}

		private void StoreIndirect(TypeReference type)
		{
			StackInfo stackInfo = this._valueStack.Pop();
			StackInfo stackInfo2 = this._valueStack.Pop();
			this.EmitMemoryBarrierIfNecessary(null);
			string text;
			string text2;
			if (!TypeReferenceEqualityComparer.AreEqual(stackInfo.Type, this.IntPtrTypeReference, TypeComparisonMode.Exact) && !TypeReferenceEqualityComparer.AreEqual(stackInfo.Type, this.UIntPtrTypeReference, TypeComparisonMode.Exact))
			{
				PointerType variableType = new PointerType(type);
				text = string.Format("({0})({1})", MethodBodyWriter.Naming.ForVariable(variableType), stackInfo2.Expression);
				text2 = string.Format("({0}){1}", MethodBodyWriter.Naming.ForVariable(type), stackInfo.Expression);
			}
			else
			{
				text = stackInfo2.Expression;
				text2 = stackInfo.Expression;
			}
			this._writer.WriteLine("*({0}) = {1};", new object[]
			{
				text,
				text2
			});
			this._writer.WriteWriteBarrierIfNeeded(type, text, text2);
		}

		private void StoreElement(StackInfo array, StackInfo index, StackInfo value, bool emitElementTypeCheck)
		{
			TypeReference type = ArrayUtilities.ArrayElementTypeOf(array.Type);
			this._nullCheckSupport.WriteNullCheckIfNeeded(array);
			this.WriteArrayBoundsCheckIfNeeded(array, index);
			if (emitElementTypeCheck)
			{
				this._writer.WriteLine(Emit.ArrayElementTypeCheck(array.Expression, value.Expression));
			}
			string index2 = index.Expression;
			if (index.Type.IsIntegralPointerType())
			{
				index2 = this.ArrayIndexerForIntPtr(index.Expression, index.Type);
			}
			this._writer.WriteLine("{0};", new object[]
			{
				Emit.StoreArrayElement(array.Expression, index2, Emit.Cast(type, value.Expression))
			});
		}

		private void LoadNull()
		{
			this._valueStack.Push(new StackInfo("NULL", this.ObjectTypeReference));
		}

		private void WriteLdarg(int index, InstructionBlock block, Instruction ins)
		{
			if (this._methodDefinition.HasThis)
			{
				index--;
			}
			if (index < 0)
			{
				TypeReference typeReference = this._typeResolver.Resolve(this._methodReference.DeclaringType);
				if (typeReference.IsValueType())
				{
					typeReference = new ByReferenceType(typeReference);
				}
				this._valueStack.Push(new StackInfo(MethodBodyWriter.Naming.ThisParameterName, typeReference));
			}
			else
			{
				TypeReference type = this._typeResolver.ResolveParameterType(this._methodReference, this._methodReference.Parameters[index]);
				Local local = this.NewTemp(type);
				string right = MethodBodyWriter.Naming.ForParameterName(this._methodDefinition.Parameters[index]);
				if (!this.CanApplyValueTypeBoxBranchOptimizationToInstruction(ins.Next, block) && (ins.Next.OpCode.Code != Code.Ldobj || !this.CanApplyValueTypeBoxBranchOptimizationToInstruction(ins.Next.Next, block)))
				{
					this._writer.WriteLine("{0};", new object[]
					{
						Emit.Assign(local.IdentifierExpression, right)
					});
				}
				this._valueStack.Push(new StackInfo(local));
			}
		}

		private void WriteNumericConversion(TypeReference inputType, TypeReference outputType)
		{
			StackInfo stackInfo = this._valueStack.Pop();
			string text = stackInfo.ToString();
			if (stackInfo.Type.MetadataType == MetadataType.IntPtr || stackInfo.Type.MetadataType == MetadataType.UIntPtr)
			{
				text = this.FormatNativeIntGetterName(stackInfo.Expression, stackInfo.Type);
			}
			this.PushExpression(outputType, string.Format("(({0})(({1}){2}{3}))", new object[]
			{
				MethodBodyWriter.Naming.ForVariable(outputType),
				MethodBodyWriter.Naming.ForVariable(inputType),
				(stackInfo.Type.MetadataType != MetadataType.Pointer) ? "" : "(intptr_t)",
				text
			}));
		}

		private void WriteNumericConversion(TypeReference typeReference)
		{
			this.WriteNumericConversion(typeReference, typeReference);
		}

		private void WriteNumericConversionWithOverflow<TMaxValue>(TypeReference typeReference, bool treatInputAsUnsigned, TMaxValue maxValue, bool inputIsValue = true)
		{
			this.WriteCheckForOverflow<TMaxValue>(treatInputAsUnsigned, maxValue, inputIsValue);
			this.WriteNumericConversion(typeReference);
		}

		private void WriteNumericConversionI8()
		{
			if (this._valueStack.Peek().Type.IsSameType(this.UInt32TypeReference))
			{
				this.WriteNumericConversion(this.Int32TypeReference);
			}
			this.WriteNumericConversion(this.Int64TypeReference, this.Int64TypeReference);
		}

		private void WriteNumericConversionU8()
		{
			if (this._valueStack.Peek().Type.IsSameType(this.Int32TypeReference))
			{
				this.WriteNumericConversion(this.UInt32TypeReference);
			}
			this.WriteNumericConversion(this.UInt64TypeReference, this.Int64TypeReference);
		}

		private void WriteNumericConversionFloat(TypeReference outputType)
		{
			if (this._valueStack.Peek().Type.IsSameType(this.UInt32TypeReference))
			{
				this.WriteNumericConversion(this.Int32TypeReference, outputType);
			}
			else if (this._valueStack.Peek().Type.IsSameType(this.UInt64TypeReference))
			{
				this.WriteNumericConversion(this.Int64TypeReference, outputType);
			}
			this.WriteNumericConversion(outputType);
		}

		private void WriteLdloc(int index, InstructionBlock block, Instruction ins)
		{
			VariableDefinition variable = this._methodDefinition.Body.Variables[index];
			TypeReference type = this._typeResolver.ResolveVariableType(this._methodReference, variable);
			Local local = this.NewTemp(type);
			this._valueStack.Push(new StackInfo(local));
			if (!this.CanApplyValueTypeBoxBranchOptimizationToInstruction(ins.Next, block))
			{
				this._writer.WriteLine("{0};", new object[]
				{
					Emit.Assign(local.IdentifierExpression, MethodBodyWriter.Naming.ForVariableName(variable))
				});
			}
		}

		private void WriteStloc(int index)
		{
			StackInfo stackInfo = this._valueStack.Pop();
			VariableDefinition variableDefinition = this._methodDefinition.Body.Variables[index];
			TypeReference typeReference = MethodBodyWriter.Naming.RemoveModifiers(variableDefinition.VariableType);
			if (typeReference.IsPointer || typeReference.IsByReference)
			{
				this._writer.WriteLine("{0} = ({1}){2};", new object[]
				{
					MethodBodyWriter.Naming.ForVariableName(variableDefinition),
					MethodBodyWriter.Naming.ForVariable(this._typeResolver.Resolve(typeReference)),
					stackInfo
				});
			}
			else if ((variableDefinition.VariableType.IsSameType(this.IntPtrTypeReference) && !stackInfo.Type.IsSameType(this.IntPtrTypeReference)) || (variableDefinition.VariableType.IsSameType(this.UIntPtrTypeReference) && !stackInfo.Type.IsSameType(this.UIntPtrTypeReference)))
			{
				this._writer.WriteLine(this.FormatNativeIntSetterInvocation(MethodBodyWriter.Naming.ForVariableName(variableDefinition), variableDefinition.VariableType, string.Format("(void*){0}", stackInfo)));
			}
			else
			{
				this.WriteAssignment(MethodBodyWriter.Naming.ForVariableName(variableDefinition), this._typeResolver.Resolve(variableDefinition.VariableType), stackInfo);
			}
		}

		private void GenerateConditional(string op, MethodBodyWriter.Signedness signedness, bool negate = false)
		{
			this.PushExpression(this.Int32TypeReference, this.ConditionalExpressionFor(op, signedness, negate) + "? 1 : 0");
		}

		private string ConditionalExpressionFor(string cppOperator, MethodBodyWriter.Signedness signedness, bool negate)
		{
			StackInfo stackInfo = this._valueStack.Pop();
			StackInfo stackInfo2 = this._valueStack.Pop();
			string result;
			if (stackInfo.Expression == "0" && signedness == MethodBodyWriter.Signedness.Unsigned)
			{
				if (cppOperator == "<")
				{
					result = ((!negate) ? "false" : "true");
					return result;
				}
				if (cppOperator == ">=")
				{
					result = ((!negate) ? "true" : "false");
					return result;
				}
			}
			string text = this.CastExpressionForOperandOfComparision(signedness, stackInfo2);
			string text2 = this.CastExpressionForOperandOfComparision(signedness, stackInfo);
			if (MethodBodyWriter.IsNonPointerReferenceType(stackInfo) && MethodBodyWriter.IsNonPointerReferenceType(stackInfo2))
			{
				text2 = MethodBodyWriter.PrependCastToObject(text2);
				text = MethodBodyWriter.PrependCastToObject(text);
			}
			string text3 = string.Format("(({0}{1}) {2} ({3}{4}))", new object[]
			{
				text,
				stackInfo2,
				cppOperator,
				text2,
				stackInfo
			});
			result = ((!negate) ? text3 : string.Format("(!{0})", text3));
			return result;
		}

		private static bool IsNonPointerReferenceType(StackInfo stackEntry)
		{
			return !stackEntry.Type.IsValueType() && !stackEntry.Type.IsPointer;
		}

		private static string PrependCastToObject(string expression)
		{
			return string.Format("({0}*){1}", MethodBodyWriter.Naming.ForType(MethodBodyWriter.TypeProvider.SystemObject), expression);
		}

		private string CastExpressionForOperandOfComparision(MethodBodyWriter.Signedness signedness, StackInfo left)
		{
			return "(" + MethodBodyWriter.Naming.ForVariable(this.TypeForComparison(signedness, left.Type)) + ")";
		}

		private TypeReference TypeForComparison(MethodBodyWriter.Signedness signedness, TypeReference type)
		{
			TypeReference typeReference = StackTypeConverter.StackTypeFor(type);
			TypeReference result;
			if (!typeReference.IsSameType(MethodBodyWriter.TypeProvider.NativeIntTypeReference))
			{
				MetadataType metadataType = typeReference.MetadataType;
				switch (metadataType)
				{
				case MetadataType.Int32:
					result = ((signedness != MethodBodyWriter.Signedness.Signed) ? this.UInt32TypeReference : this.Int32TypeReference);
					return result;
				case MetadataType.UInt32:
					IL_56:
					if (metadataType == MetadataType.IntPtr || metadataType == MetadataType.UIntPtr)
					{
						result = ((signedness != MethodBodyWriter.Signedness.Signed) ? this.UIntPtrTypeReference : this.IntPtrTypeReference);
						return result;
					}
					if (metadataType != MetadataType.Pointer)
					{
						result = type;
						return result;
					}
					result = ((signedness != MethodBodyWriter.Signedness.Signed) ? this.NativeUIntTypeReference : this.NativeIntTypeReference);
					return result;
				case MetadataType.Int64:
					result = ((signedness != MethodBodyWriter.Signedness.Signed) ? this.UInt64TypeReference : this.Int64TypeReference);
					return result;
				}
				goto IL_56;
			}
			result = ((signedness != MethodBodyWriter.Signedness.Signed) ? this.NativeUIntTypeReference : this.NativeIntTypeReference);
			return result;
		}

		private void GenerateConditionalJump(InstructionBlock block, Instruction ins, bool isTrue)
		{
			Instruction targetInstruction = (Instruction)ins.Operand;
			StackInfo stackInfo = this._valueStack.Pop();
			string arg;
			if (stackInfo.Type.MetadataType == MetadataType.IntPtr || stackInfo.Type.MetadataType == MetadataType.UIntPtr)
			{
				arg = this.FormatNativeIntGetterName(stackInfo.Expression, stackInfo.Type);
			}
			else
			{
				arg = stackInfo.Expression;
			}
			string conditional = string.Format("{0}{1}", (!isTrue) ? "!" : "", arg);
			if (this._valueStack.Count == 0)
			{
				using (this.NewIfBlock(conditional))
				{
					this.WriteJump(targetInstruction);
				}
			}
			else
			{
				this.WriteGlobalVariableAssignmentForLeftBranch(block, targetInstruction);
				using (this.NewIfBlock(conditional))
				{
					this.WriteGlobalVariableAssignmentForRightBranch(block, targetInstruction);
					this.WriteJump(targetInstruction);
				}
			}
		}

		private void WriteGlobalVariableAssignmentForRightBranch(InstructionBlock block, Instruction targetInstruction)
		{
			GlobalVariable[] globalVariables = this._stackAnalysis.InputVariablesFor(block.Successors.Single((InstructionBlock b) => b.First.Offset == targetInstruction.Offset));
			this.WriteAssignGlobalVariables(globalVariables);
		}

		private void WriteGlobalVariableAssignmentForLeftBranch(InstructionBlock block, Instruction targetInstruction)
		{
			GlobalVariable[] globalVariables = this._stackAnalysis.InputVariablesFor(block.Successors.Single((InstructionBlock b) => b.First.Offset != targetInstruction.Offset));
			this.WriteAssignGlobalVariables(globalVariables);
		}

		private void GenerateConditionalJump(InstructionBlock block, Instruction ins, string cppOperator, MethodBodyWriter.Signedness signedness, bool negate = false)
		{
			string conditional = this.ConditionalExpressionFor(cppOperator, signedness, negate);
			Instruction targetInstruction = (Instruction)ins.Operand;
			if (this._valueStack.Count == 0)
			{
				using (this.NewIfBlock(conditional))
				{
					this.WriteJump(targetInstruction);
				}
			}
			else
			{
				GlobalVariable[] globalVariables = this._stackAnalysis.InputVariablesFor(block.Successors.Single((InstructionBlock b) => b.First.Offset != targetInstruction.Offset));
				GlobalVariable[] globalVariables2 = this._stackAnalysis.InputVariablesFor(block.Successors.Single((InstructionBlock b) => b.First.Offset == targetInstruction.Offset));
				this.WriteAssignGlobalVariables(globalVariables);
				using (this.NewIfBlock(conditional))
				{
					this.WriteAssignGlobalVariables(globalVariables2);
					this.WriteJump(targetInstruction);
				}
			}
		}

		private void WriteAssignGlobalVariables(GlobalVariable[] globalVariables)
		{
			if (globalVariables.Length != this._valueStack.Count)
			{
				throw new ArgumentException("Invalid global variables count", "globalVariables");
			}
			int stackIndex = 0;
			foreach (StackInfo current in this._valueStack)
			{
				GlobalVariable globalVariable = globalVariables.Single((GlobalVariable v) => v.Index == stackIndex);
				if (current.Type.FullName != globalVariable.Type.FullName)
				{
					this._writer.WriteLine("{0} = (({1}){2}({3}));", new object[]
					{
						globalVariable.VariableName,
						MethodBodyWriter.Naming.ForVariable(this._typeResolver.Resolve(globalVariable.Type)),
						(current.Type.MetadataType != MetadataType.Pointer) ? "" : "(intptr_t)",
						current.Expression
					});
				}
				else
				{
					this._writer.WriteLine("{0} = {1};", new object[]
					{
						globalVariable.VariableName,
						current.Expression
					});
				}
				stackIndex++;
			}
		}

		private string ExpressionForBinaryOperation(TypeReference type, string expression)
		{
			string result;
			if (type.MetadataType == MetadataType.IntPtr || type.MetadataType == MetadataType.UIntPtr)
			{
				result = this.FormatNativeIntGetterName(expression, type);
			}
			else
			{
				result = expression;
			}
			return result;
		}

		private void WriteBinaryOperation(TypeReference destType, string lcast, string left, string op, string rcast, string right)
		{
			this.PushExpression(destType, string.Format("({0})({1}{2}{3}{4}{5})", new object[]
			{
				MethodBodyWriter.Naming.ForVariable(destType),
				lcast,
				left,
				op,
				rcast,
				right
			}));
		}

		private void WriteRemainderOperation()
		{
			StackInfo right = this._valueStack.Pop();
			StackInfo left = this._valueStack.Pop();
			if (right.Type.MetadataType == MetadataType.Single || left.Type.MetadataType == MetadataType.Single)
			{
				this.PushExpression(this.SingleTypeReference, string.Format("fmodf({0}, {1})", left.Expression, right.Expression));
			}
			else if (right.Type.MetadataType == MetadataType.Double || left.Type.MetadataType == MetadataType.Double)
			{
				this.PushExpression(this.DoubleTypeReference, string.Format("fmod({0}, {1})", left.Expression, right.Expression));
			}
			else
			{
				this.WriteBinaryOperation("%", left, right, left.Type);
			}
		}

		private void WriteBinaryOperationUsingLargestOperandTypeAsResultType(string op)
		{
			StackInfo right = this._valueStack.Pop();
			StackInfo left = this._valueStack.Pop();
			this.WriteBinaryOperation(op, left, right, StackAnalysisUtils.CorrectLargestTypeFor(left.Type, right.Type, MethodBodyWriter.TypeProvider));
		}

		private void WriteBinaryOperationUsingLeftOperandTypeAsResultType(string op)
		{
			StackInfo right = this._valueStack.Pop();
			StackInfo left = this._valueStack.Pop();
			this.WriteBinaryOperation(op, left, right, left.Type);
		}

		private void WriteBinaryOperation(string op, StackInfo left, StackInfo right, TypeReference resultType)
		{
			TypeReference type = MethodBodyWriter.Naming.RemoveModifiers(left.Type);
			TypeReference type2 = MethodBodyWriter.Naming.RemoveModifiers(right.Type);
			string rcast = this.CastExpressionForBinaryOperator(right);
			string lcast = this.CastExpressionForBinaryOperator(left);
			string left2 = this.ExpressionForBinaryOperation(type, left.Expression);
			string right2 = this.ExpressionForBinaryOperation(type2, right.Expression);
			if (!resultType.IsPointer)
			{
				try
				{
					resultType = StackTypeConverter.StackTypeFor(resultType);
				}
				catch (ArgumentException)
				{
				}
			}
			this.WriteBinaryOperation(resultType, lcast, left2, op, rcast, right2);
		}

		private string CastExpressionForBinaryOperator(StackInfo right)
		{
			string result;
			if (right.Type.IsPointer)
			{
				result = "(" + MethodBodyWriter.Naming.ForVariable(StackTypeConverter.StackTypeForBinaryOperation(right.Type)) + ")";
			}
			else
			{
				try
				{
					result = "(" + StackTypeConverter.CppStackTypeFor(right.Type) + ")";
				}
				catch (ArgumentException)
				{
					result = "";
				}
			}
			return result;
		}

		private void WriteShrUn()
		{
			StackInfo stackInfo = this._valueStack.Pop();
			StackInfo stackInfo2 = this._valueStack.Pop();
			string lcast = "";
			TypeReference typeReference = StackTypeConverter.StackTypeFor(stackInfo2.Type);
			if (typeReference.MetadataType == MetadataType.Int32)
			{
				lcast = "(uint32_t)";
			}
			if (typeReference.MetadataType == MetadataType.Int64)
			{
				lcast = "(uint64_t)";
			}
			this.WriteBinaryOperation(typeReference, lcast, stackInfo2.Expression, ">>", "", stackInfo.Expression);
		}

		private string NewTempName()
		{
			return "L_" + this._tempIndex++;
		}

		private Local NewTemp(TypeReference type)
		{
			if (type.ContainsGenericParameters())
			{
				throw new InvalidOperationException("Callers should resolve the type prior to calling this method.");
			}
			string expression = this.NewTempName();
			return new Local(type, expression);
		}

		private void LoadPrimitiveTypeSByte(Instruction ins, TypeReference type)
		{
			this.PushExpression(type, Emit.Cast(type, ((sbyte)ins.Operand).ToString()));
		}

		private void LoadPrimitiveTypeInt32(Instruction ins, TypeReference type)
		{
			int num = (int)ins.Operand;
			string text = num.ToString();
			long num2 = (long)num;
			if (num2 <= -2147483648L || num2 >= 2147483647L)
			{
				text += "LL";
			}
			this.PushExpression(type, Emit.Cast(type, text));
		}

		private void LoadLong(Instruction ins, TypeReference type)
		{
			long num = (long)ins.Operand;
			string value = num + "LL";
			if (num == -9223372036854775808L)
			{
				value = "std::numeric_limits<int64_t>::min()";
			}
			if (num == 9223372036854775807L)
			{
				value = "std::numeric_limits<int64_t>::max()";
			}
			this.PushExpression(type, Emit.Cast(type, value));
		}

		private void LoadInt32Constant(int value)
		{
			this._valueStack.Push((value >= 0) ? new StackInfo(value.ToString(), this.Int32TypeReference) : new StackInfo(string.Format("({0})", value), this.Int32TypeReference));
		}

		private static List<string> FormatArgumentsForMethodCall(List<TypeReference> parameterTypes, List<StackInfo> stackValues, SharingType sharingType)
		{
			int count = parameterTypes.Count;
			List<string> list = new List<string>();
			for (int i = 0; i < count; i++)
			{
				StringBuilder stringBuilder = new StringBuilder();
				StackInfo right = stackValues[i];
				TypeReference typeReference = parameterTypes[i];
				if (typeReference.IsPointer)
				{
					stringBuilder.Append("(" + MethodBodyWriter.Naming.ForVariable(typeReference) + ")");
				}
				else if (VarianceSupport.IsNeededForConversion(typeReference, right.Type))
				{
					stringBuilder.Append(VarianceSupport.Apply(typeReference, right.Type));
				}
				stringBuilder.Append(MethodBodyWriter.WriteExpressionAndCastIfNeeded(typeReference, right, sharingType));
				list.Add(stringBuilder.ToString());
			}
			return list;
		}

		private static List<TypeReference> GetParameterTypes(MethodReference method, TypeResolver typeResolverForMethodToCall)
		{
			return new List<TypeReference>(from parameter in method.Parameters
			select typeResolverForMethodToCall.Resolve(GenericParameterResolver.ResolveParameterTypeIfNeeded(method, parameter)));
		}

		private static bool NeedsNullArgForStaticMethod(MethodReference method, bool isConstructor)
		{
			return !method.HasThis && !isConstructor && !IntrinsicRemap.ShouldRemap(method) && !GenericsUtilities.IsGenericInstanceOfCompareExchange(method) && !GenericsUtilities.IsGenericInstanceOfExchange(method);
		}

		private static List<StackInfo> PopItemsFromStack(int amount, Stack<StackInfo> valueStack)
		{
			if (amount > valueStack.Count)
			{
				throw new Exception(string.Format("Attempting to pop '{0}' values from a stack of depth '{1}'.", amount, valueStack.Count));
			}
			List<StackInfo> list = new List<StackInfo>();
			for (int num = 0; num != amount; num++)
			{
				list.Add(valueStack.Pop());
			}
			list.Reverse();
			return list;
		}

		private static string ParameterNameFor(MethodDefinition method, int i)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			return MethodBodyWriter.Naming.ForParameterName(method.Parameters[i]);
		}

		private IDisposable NewIfBlock(string conditional)
		{
			this._writer.WriteLine("if ({0})", new object[]
			{
				conditional
			});
			return this.NewBlock();
		}

		private IDisposable NewBlock()
		{
			return new BlockWriter(this._writer, false);
		}

		private void EmitMemoryBarrierIfNecessary(FieldReference fieldReference = null)
		{
			if (this._thisInstructionIsVolatile || fieldReference.IsVolatile())
			{
				MethodBodyWriter.StatsService.RecordMemoryBarrierEmitted(this._methodDefinition);
				this._writer.WriteStatement(Emit.MemoryBarrier());
				this._thisInstructionIsVolatile = false;
			}
		}

		private void AddVolatileStackEntry()
		{
			this._thisInstructionIsVolatile = true;
		}

		private void WriteLoadObject(Instruction ins)
		{
			StackInfo stackInfo = this._valueStack.Pop();
			TypeReference type = this._typeResolver.Resolve((TypeReference)ins.Operand);
			PointerType variableType = new PointerType(type);
			this._valueStack.Push(new StackInfo(string.Format("(*({0}){1})", MethodBodyWriter.Naming.ForVariable(variableType), stackInfo.Expression), type));
			this.EmitMemoryBarrierIfNecessary(null);
		}

		private void WriteStoreObject(TypeReference type)
		{
			TypeReference typeReference = this._typeResolver.Resolve(type);
			StackInfo stackInfo = this._valueStack.Pop();
			StackInfo stackInfo2 = this._valueStack.Pop();
			this.EmitMemoryBarrierIfNecessary(null);
			string text = Emit.Cast(new PointerType(typeReference), stackInfo2.Expression);
			this._writer.WriteStatement(Emit.Assign(Emit.Dereference(text), stackInfo.Expression));
			this._writer.WriteWriteBarrierIfNeeded(typeReference, text, stackInfo.Expression);
		}

		private void PushCallToLoadVirtualFunction(Instruction ins)
		{
			StackInfo stackInfo = this._valueStack.Pop();
			MethodReference methodReference = (MethodReference)ins.Operand;
			bool flag = methodReference.DeclaringType.IsInterface();
			MethodDefinition methodDefinition = methodReference.Resolve();
			string arg;
			if (flag)
			{
				arg = Emit.Call("GetInterfaceMethodInfo", stackInfo.Expression, this._vTableBuilder.IndexFor(methodDefinition).ToString(), this._runtimeMetadataAccess.TypeInfoFor(methodReference.DeclaringType));
			}
			else if (methodDefinition.IsVirtual)
			{
				arg = Emit.Call("GetVirtualMethodInfo", stackInfo.Expression, this._vTableBuilder.IndexFor(methodDefinition).ToString());
			}
			else
			{
				arg = this._runtimeMetadataAccess.MethodInfo(methodReference);
			}
			this._writer.AddIncludeForTypeDefinition(this._typeResolver.Resolve(methodReference.DeclaringType));
			this.StoreLocalIntPtrAndPush(string.Format("(void*){0}", arg));
		}
	}
}
