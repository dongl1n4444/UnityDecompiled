namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.Common.CFG;
    using Unity.IL2CPP.Debugger;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Metadata;
    using Unity.IL2CPP.StackAnalysis;

    public class MethodBodyWriter
    {
        private readonly ArrayBoundsCheckSupport _arrayBoundsCheckSupport;
        private readonly ControlFlowGraph _cfg;
        private readonly HashSet<TypeReference> _classesAlreadyInitializedInBlock;
        private TypeReference _constrainedCallThisType;
        private readonly DivideByZeroCheckSupport _divideByZeroCheckSupport;
        private readonly HashSet<Instruction> _emittedLabels;
        private ExceptionSupport _exceptionSupport;
        private readonly Labeler _labeler;
        private readonly MethodDefinition _methodDefinition;
        private readonly MethodReference _methodReference;
        private NullChecksSupport _nullCheckSupport;
        private readonly MethodBodyWriterDebugOptions _options;
        private readonly HashSet<Instruction> _referencedLabels;
        private readonly IRuntimeMetadataAccess _runtimeMetadataAccess;
        private readonly SharingType _sharingType;
        private readonly Unity.IL2CPP.StackAnalysis.StackAnalysis _stackAnalysis;
        private int _tempIndex;
        private bool _thisInstructionIsVolatile;
        private readonly Unity.IL2CPP.ILPreProcessor.TypeResolver _typeResolver;
        private readonly Stack<StackInfo> _valueStack;
        private readonly VTableBuilder _vTableBuilder;
        private readonly CppCodeWriter _writer;
        [CompilerGenerated]
        private static Func<InstructionBlock, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<GlobalVariable, int> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache10;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache11;
        [CompilerGenerated]
        private static Func<InstructionBlock, string> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<GlobalVariable, int> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<ExceptionSupport.Node, ExceptionHandler> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<TypeReference, string> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<TypeReference, string> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache9;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cacheA;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cacheB;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cacheC;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cacheD;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cacheE;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cacheF;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__mg$cache0;
        [Inject]
        public static IGenericSharingAnalysisService GenericSharingAnalysis;
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static ISourceAnnotationWriter SourceAnnotationWriter;
        [Inject]
        public static IStatsService StatsService;
        [Inject]
        public static ITypeProviderService TypeProvider;
        private const string VariableNameForTypeInfoInConstrainedCall = "il2cpp_this_typeinfo";
        private const string VariableNameForVirtualInvokeDataInDirectVirtualCall = "il2cpp_virtual_invoke_data_";

        public MethodBodyWriter(CppCodeWriter writer, MethodReference methodReference, Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver, IRuntimeMetadataAccess metadataAccess, VTableBuilder vTableBuilder) : this(writer, methodReference, typeResolver, metadataAccess, vTableBuilder, new MethodBodyWriterDebugOptions())
        {
        }

        private MethodBodyWriter(CppCodeWriter writer, MethodReference methodReference, Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver, IRuntimeMetadataAccess metadataAccess, VTableBuilder vTableBuilder, MethodBodyWriterDebugOptions options)
        {
            this._valueStack = new Stack<StackInfo>();
            this._emittedLabels = new HashSet<Instruction>();
            this._referencedLabels = new HashSet<Instruction>();
            this._classesAlreadyInitializedInBlock = new HashSet<TypeReference>(new Unity.IL2CPP.Common.TypeReferenceEqualityComparer());
            this._thisInstructionIsVolatile = false;
            this._methodReference = methodReference;
            this._methodDefinition = methodReference.Resolve();
            this._nullCheckSupport = new NullChecksSupport(writer, this._methodDefinition, CodeGenOptions.EmitNullChecks);
            this._arrayBoundsCheckSupport = new ArrayBoundsCheckSupport(this._methodDefinition, CodeGenOptions.EnableArrayBoundsCheck);
            this._divideByZeroCheckSupport = new DivideByZeroCheckSupport(writer, this._methodDefinition, CodeGenOptions.EnableDivideByZeroCheck);
            this._cfg = ControlFlowGraph.Create(this._methodDefinition);
            this._writer = writer;
            this._typeResolver = typeResolver;
            this._vTableBuilder = vTableBuilder;
            this._options = options;
            this._stackAnalysis = Unity.IL2CPP.StackAnalysis.StackAnalysis.Analyze(this._methodDefinition, this._cfg, this._typeResolver);
            DeadBlockAnalysis.MarkBlocksDeadIfNeeded(this._cfg.Blocks);
            this._labeler = new Labeler(this._methodDefinition);
            this._runtimeMetadataAccess = metadataAccess;
            this._sharingType = !GenericSharingAnalysis.IsSharedMethod(methodReference) ? SharingType.NonShared : SharingType.Shared;
        }

        private void AddVolatileStackEntry()
        {
            this._thisInstructionIsVolatile = true;
        }

        private string ArrayIndexerForIntPtr(string indexExpression, TypeReference indexType) => 
            $"({Naming.ForArrayIndexType()})({(!indexType.IsSameType(this.IntPtrTypeReference) ? Naming.ForUIntPtrT : Naming.ForIntPtrT)}){this.FormatNativeIntGetterName(indexExpression, indexType)}";

        private StackInfo BoxThisForContraintedCallIntoNewTemp(StackInfo thisValue)
        {
            Local local = this.NewTemp(TypeProvider.ObjectTypeReference);
            object[] args = new object[] { local.IdentifierExpression, Emit.Call("Box", this._runtimeMetadataAccess.TypeInfoFor(this._constrainedCallThisType), thisValue.Expression) };
            this._writer.WriteLine("{0} = {1};", args);
            return new StackInfo(local.Expression, TypeProvider.ObjectTypeReference);
        }

        private TypeReference CalculateResultTypeForNegate(TypeReference type)
        {
            if (!type.IsUnsignedIntegralType())
            {
                return type;
            }
            if (((type.MetadataType == MetadataType.Byte) || (type.MetadataType == MetadataType.UInt16)) || (type.MetadataType == MetadataType.UInt32))
            {
                return TypeProvider.Int32TypeReference;
            }
            return TypeProvider.Int64TypeReference;
        }

        private string CallExpressionFor(MethodReference callingMethod, MethodReference unresolvedMethodToCall, MethodCallType callType, List<StackInfo> poppedValues, Func<string, string> addUniqueSuffix, bool emitNullCheckForInvocation = true)
        {
            MethodReference reference = this._typeResolver.Resolve(unresolvedMethodToCall);
            Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolverForMethodToCall = this._typeResolver;
            GenericInstanceMethod genericInstanceMethod = reference as GenericInstanceMethod;
            if (genericInstanceMethod != null)
            {
                typeResolverForMethodToCall = this._typeResolver.Nested(genericInstanceMethod);
            }
            List<TypeReference> parameterTypes = GetParameterTypes(reference, typeResolverForMethodToCall);
            if (reference.HasThis)
            {
                parameterTypes.Insert(0, !reference.DeclaringType.IsValueType() ? reference.DeclaringType : new ByReferenceType(reference.DeclaringType));
            }
            List<string> args = FormatArgumentsForMethodCall(parameterTypes, poppedValues, this._sharingType);
            if (NeedsNullArgForStaticMethod(reference, false))
            {
                args.Insert(0, !CodeGenOptions.EmitComments ? Naming.Null : "NULL /*static, unused*/");
            }
            if (MethodSignatureWriter.NeedsHiddenMethodInfo(reference, callType, false))
            {
                string str;
                if (CodeGenOptions.MonoRuntime)
                {
                    str = (callType != MethodCallType.DirectVirtual) ? this._runtimeMetadataAccess.HiddenMethodInfo(unresolvedMethodToCall) : $"il2cpp_codegen_vtable_slot_method({addUniqueSuffix("il2cpp_this_typeinfo")}, {Emit.MonoMethodMetadataGet(unresolvedMethodToCall)})";
                }
                else if (callType == MethodCallType.DirectVirtual)
                {
                    string str2 = addUniqueSuffix("il2cpp_virtual_invoke_data_");
                    str = $"{str2}.method";
                }
                else
                {
                    str = this._runtimeMetadataAccess.HiddenMethodInfo(unresolvedMethodToCall);
                }
                args.Add((!CodeGenOptions.EmitComments ? "" : "/*hidden argument*/") + str);
            }
            if (emitNullCheckForInvocation)
            {
                this._nullCheckSupport.WriteNullCheckForInvocationIfNeeded(reference, args);
            }
            if (GenericSharingAnalysis.ShouldTryToCallStaticConstructorBeforeMethodCall(reference, this._methodReference))
            {
                this.WriteCallToClassAndInitializerAndStaticConstructorIfNeeded(unresolvedMethodToCall.DeclaringType, this._methodDefinition, this._runtimeMetadataAccess);
            }
            return GetMethodCallExpression(callingMethod, reference, unresolvedMethodToCall, typeResolverForMethodToCall, callType, this._runtimeMetadataAccess, this._vTableBuilder, args, this._arrayBoundsCheckSupport.ShouldEmitBoundsChecksForMethod(), addUniqueSuffix);
        }

        private bool CanApplyValueTypeBoxBranchOptimizationToInstruction(Instruction ins, InstructionBlock block)
        {
            if ((ins != null) && (ins.OpCode.Code == Code.Box))
            {
                TypeReference typeReference = this._typeResolver.Resolve((TypeReference) ins.Operand);
                return (((typeReference.IsValueType() && !typeReference.IsNullable()) && ((ins != block.Last) && (ins.Next != null))) && ((((ins.Next.OpCode.Code == Code.Brtrue) || (ins.Next.OpCode.Code == Code.Brtrue_S)) || (ins.Next.OpCode.Code == Code.Brfalse)) || (ins.Next.OpCode.Code == Code.Brfalse_S)));
            }
            return false;
        }

        private string CastExpressionForBinaryOperator(StackInfo right)
        {
            if (right.Type.IsPointer)
            {
                return ("(" + Naming.ForVariable(StackTypeConverter.StackTypeForBinaryOperation(right.Type)) + ")");
            }
            try
            {
                return ("(" + StackTypeConverter.CppStackTypeFor(right.Type) + ")");
            }
            catch (ArgumentException)
            {
                return "";
            }
        }

        private string CastExpressionForOperandOfComparision(Signedness signedness, StackInfo left) => 
            ("(" + Naming.ForVariable(this.TypeForComparison(signedness, left.Type)) + ")");

        private static string CastIfPointerType(TypeReference type)
        {
            string str = string.Empty;
            if (type.IsPointer)
            {
                str = "(" + Naming.ForVariable(type) + ")";
            }
            return str;
        }

        private static string CastReferenceTypeOrNativeIntIfNeeded(StackInfo originalValue, TypeReference toType)
        {
            if (!toType.IsValueType())
            {
                return CastTypeIfNeeded(originalValue, toType);
            }
            if (originalValue.Type.IsNativeIntegralType())
            {
                return CastTypeIfNeeded(originalValue, new ByReferenceType(toType));
            }
            if (originalValue.Type.IsPointer)
            {
                return CastTypeIfNeeded(originalValue, new PointerType(toType));
            }
            return originalValue.Expression;
        }

        private static string CastTypeIfNeeded(StackInfo originalValue, TypeReference toType)
        {
            if (!Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(originalValue.Type, toType, TypeComparisonMode.Exact))
            {
                return $"({Emit.Cast(toType, originalValue.Expression)})";
            }
            return originalValue.Expression;
        }

        private void CollectUsedLabels()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = block => block.IsBranchTarget;
            }
            foreach (InstructionBlock block in this._cfg.Blocks.Where<InstructionBlock>(<>f__am$cache0))
            {
                this._referencedLabels.Add(block.First);
            }
            foreach (ExceptionSupport.Node node in this._exceptionSupport.FlowTree.Children)
            {
                this.RecursivelyAddLabelsForExceptionNodes(node);
            }
        }

        private string ConditionalExpressionFor(string cppOperator, Signedness signedness, bool negate)
        {
            StackInfo left = this._valueStack.Pop();
            StackInfo info2 = this._valueStack.Pop();
            if ((left.Expression == "0") && (signedness == Signedness.Unsigned))
            {
                if (cppOperator == "<")
                {
                    return (!negate ? "false" : "true");
                }
                if (cppOperator == ">=")
                {
                    return (!negate ? "true" : "false");
                }
            }
            string expression = this.CastExpressionForOperandOfComparision(signedness, info2);
            string str3 = this.CastExpressionForOperandOfComparision(signedness, left);
            if (IsNonPointerReferenceType(left) && IsNonPointerReferenceType(info2))
            {
                str3 = PrependCastToObject(str3);
                expression = PrependCastToObject(expression);
            }
            string expressionFor = this.GetExpressionFor(info2);
            string str5 = this.GetExpressionFor(left);
            string str6 = $"(({expression}{expressionFor}) {cppOperator} ({str3}{str5}))";
            return (!negate ? str6 : $"(!{str6})");
        }

        private string ConstrainedCallExpressionFor(MethodReference resolvedMethodToCall, ref MethodReference methodToCall, MethodCallType callType, List<StackInfo> poppedValues, Func<string, string> addUniqueSuffix, out string copyBackBoxedExpr)
        {
            StackInfo thisValue = poppedValues[0];
            ByReferenceType type = thisValue.Type as ByReferenceType;
            if (type == null)
            {
                throw new InvalidOperationException("Attempting to constrain an invalid type.");
            }
            TypeReference elementType = type.ElementType;
            TypeReference b = this._typeResolver.Resolve(this._constrainedCallThisType);
            if (!Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(elementType, b, TypeComparisonMode.Exact))
            {
                throw new InvalidOperationException($"Attempting to constrain a value of type '{elementType}' to type '{b}'.");
            }
            copyBackBoxedExpr = null;
            if (!elementType.IsValueType())
            {
                poppedValues[0] = new StackInfo(Emit.Dereference(thisValue.Expression), elementType);
                this._writer.AddIncludeForTypeDefinition(resolvedMethodToCall.DeclaringType);
                return this.CallExpressionFor(this._methodReference, methodToCall, callType, poppedValues, addUniqueSuffix, true);
            }
            if (resolvedMethodToCall.IsGenericInstance)
            {
                poppedValues[0] = this.BoxThisForContraintedCallIntoNewTemp(thisValue);
                return this.CallExpressionFor(this._methodReference, methodToCall, callType, poppedValues, addUniqueSuffix, true);
            }
            if (GenericSharingAnalysis.IsGenericSharingForValueTypesEnabled && (this._sharingType == SharingType.Shared))
            {
                string str7;
                string typeInfoVariable = addUniqueSuffix("il2cpp_this_typeinfo");
                object[] args = new object[] { "RuntimeClass", typeInfoVariable, this._runtimeMetadataAccess.TypeInfoFor(this._constrainedCallThisType) };
                this._writer.WriteLine("{0}* {1} = {2};", args);
                string str3 = this.FakeBox(elementType, typeInfoVariable, thisValue.Expression);
                List<StackInfo> list = new List<StackInfo>(poppedValues) {
                    [0] = new StackInfo(Emit.Call("Box", typeInfoVariable, thisValue.Expression), TypeProvider.ObjectTypeReference)
                };
                string str4 = this.CallExpressionFor(this._methodReference, methodToCall, callType, list, addUniqueSuffix, false);
                List<StackInfo> list2 = new List<StackInfo>(poppedValues) {
                    [0] = new StackInfo(Naming.AddressOf(str3), TypeProvider.ObjectTypeReference)
                };
                string str5 = this.CallExpressionFor(this._methodReference, methodToCall, MethodCallType.DirectVirtual, list2, addUniqueSuffix, false);
                string str6 = addUniqueSuffix("il2cpp_virtual_invoke_data_");
                if (!CodeGenOptions.MonoRuntime)
                {
                    if (resolvedMethodToCall.DeclaringType.IsInterface())
                    {
                        this._writer.WriteLine($"const VirtualInvokeData& {str6} = il2cpp_codegen_get_interface_invoke_data({this._vTableBuilder.IndexFor(methodToCall.Resolve())}, &{str3}, {this._runtimeMetadataAccess.TypeInfoFor(methodToCall.DeclaringType)});");
                    }
                    else
                    {
                        this._writer.WriteLine($"const VirtualInvokeData& {str6} = il2cpp_codegen_get_virtual_invoke_data({this._vTableBuilder.IndexFor(methodToCall.Resolve())}, &{str3});");
                    }
                    str7 = Emit.Call("il2cpp_codegen_type_implements_virtual_method", typeInfoVariable, $"{str6}.method");
                }
                else
                {
                    str7 = Emit.Call("il2cpp_codegen_type_implements_virtual_method", typeInfoVariable, Emit.MonoMethodMetadataGet(methodToCall.Resolve()));
                }
                return $"{str7} ? {str5} : {str4}";
            }
            MethodReference virtualMethodTargetMethodForConstrainedCallOnValueType = this._vTableBuilder.GetVirtualMethodTargetMethodForConstrainedCallOnValueType(b, resolvedMethodToCall);
            if (((virtualMethodTargetMethodForConstrainedCallOnValueType != null) && Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(virtualMethodTargetMethodForConstrainedCallOnValueType.DeclaringType, b, TypeComparisonMode.Exact)) && !CodeGenOptions.MonoRuntime)
            {
                if ((elementType.IsGenericInstance && elementType.IsValueType()) && (this._sharingType == SharingType.Shared))
                {
                    string str8 = this.FakeBox(elementType, this._runtimeMetadataAccess.TypeInfoFor(this._constrainedCallThisType), thisValue.Expression);
                    string str9 = addUniqueSuffix("il2cpp_virtual_invoke_data_");
                    if (resolvedMethodToCall.DeclaringType.IsInterface())
                    {
                        this._writer.WriteLine($"const VirtualInvokeData& {str9} = il2cpp_codegen_get_interface_invoke_data({this._vTableBuilder.IndexFor(methodToCall.Resolve())}, &{str8}, {this._runtimeMetadataAccess.TypeInfoFor(methodToCall.DeclaringType)});");
                    }
                    else
                    {
                        this._writer.WriteLine($"const VirtualInvokeData& {str9} = il2cpp_codegen_get_virtual_invoke_data({this._vTableBuilder.IndexFor(methodToCall.Resolve())}, &{str8});");
                    }
                    List<StackInfo> list3 = new List<StackInfo>(poppedValues) {
                        [0] = new StackInfo(Naming.AddressOf(str8), TypeProvider.ObjectTypeReference)
                    };
                    return this.CallExpressionFor(this._methodReference, methodToCall, MethodCallType.DirectVirtual, list3, addUniqueSuffix, false);
                }
                methodToCall = virtualMethodTargetMethodForConstrainedCallOnValueType;
                this._writer.AddIncludeForTypeDefinition(methodToCall.DeclaringType);
                this._writer.AddIncludeForTypeDefinition(this._typeResolver.ResolveReturnType(methodToCall));
                callType = MethodCallType.Normal;
                return this.CallExpressionFor(this._methodReference, methodToCall, callType, poppedValues, addUniqueSuffix, true);
            }
            poppedValues[0] = this.BoxThisForContraintedCallIntoNewTemp(thisValue);
            StackInfo info2 = poppedValues[0];
            copyBackBoxedExpr = Emit.Assign(Emit.Dereference(thisValue.Expression), Emit.Dereference(Emit.Cast(Naming.ForVariable(elementType) + "*", Emit.Call("UnBox", info2.Expression))));
            return this.CallExpressionFor(this._methodReference, methodToCall, callType, poppedValues, addUniqueSuffix, true);
        }

        private void ConvertToNaturalInt(TypeReference pointerType)
        {
            StackInfo info = this._valueStack.Pop();
            this.PushExpression(pointerType, $"(({Naming.ForVariable(pointerType)}){info.Expression})");
        }

        private void ConvertToNaturalIntWithOverflow<TMaxValueType>(TypeReference pointerType, bool treatInputAsUnsigned, TMaxValueType maxValue)
        {
            this.WriteCheckForOverflow<TMaxValueType>(treatInputAsUnsigned, maxValue, false);
            this.ConvertToNaturalInt(pointerType);
        }

        private bool DidAlreadyEmitLabelFor(Instruction ins) => 
            this._emittedLabels.Contains(ins);

        private void DumpInsFor(InstructionBlock block)
        {
            StackState state = this._stackAnalysis.InputStackStateFor(block);
            if (state.IsEmpty)
            {
                this.WriteComment("[in: -] empty", new object[0]);
            }
            else
            {
                List<Entry> list = new List<Entry>(state.Entries);
                for (int i = 0; i < list.Count; i++)
                {
                    Entry entry = list[i];
                    object[] args = new object[3];
                    args[0] = i;
                    if (<>f__am$cache5 == null)
                    {
                        <>f__am$cache5 = t => t.FullName;
                    }
                    args[1] = entry.Types.Select<TypeReference, string>(<>f__am$cache5).AggregateWithComma();
                    args[2] = entry.NullValue;
                    this.WriteComment("[in: {0}] {1} (null: {2})", args);
                }
            }
        }

        private void DumpOutsFor(InstructionBlock block)
        {
            StackState state = this._stackAnalysis.OutputStackStateFor(block);
            if (state.IsEmpty)
            {
                this.WriteComment("[out: -] empty", new object[0]);
            }
            else
            {
                List<Entry> list = new List<Entry>(state.Entries);
                for (int i = 0; i < list.Count; i++)
                {
                    Entry entry = list[i];
                    object[] args = new object[3];
                    args[0] = i;
                    if (<>f__am$cache6 == null)
                    {
                        <>f__am$cache6 = t => t.FullName;
                    }
                    args[1] = entry.Types.Select<TypeReference, string>(<>f__am$cache6).AggregateWithComma();
                    args[2] = entry.NullValue;
                    this.WriteComment("[out: {0}] {1} (null: {2})", args);
                }
            }
        }

        private string EmitArrayLoadElementAddress(StackInfo array, string indexExpression, TypeReference indexType)
        {
            this._arrayBoundsCheckSupport.RecordArrayBoundsCheckEmitted();
            if (indexType.IsIntegralPointerType() || indexType.IsNativeIntegralType())
            {
                return Emit.LoadArrayElementAddress(array.Expression, this.ArrayIndexerForIntPtr(indexExpression, indexType), this._arrayBoundsCheckSupport.ShouldEmitBoundsChecksForMethod());
            }
            return Emit.LoadArrayElementAddress(array.Expression, indexExpression, this._arrayBoundsCheckSupport.ShouldEmitBoundsChecksForMethod());
        }

        private void EmitCallExpressionAndStoreResult(Instruction instruction, TypeReference returnType, string callExpression, string copyBackBoxedExpr)
        {
            if (returnType.IsVoid())
            {
                this._writer.WriteStatement(callExpression);
            }
            else if ((instruction.Next != null) && (instruction.Next.OpCode.Code == Code.Pop))
            {
                this._writer.WriteStatement(callExpression);
                this._valueStack.Push(new StackInfo(Naming.Null, this.ObjectTypeReference));
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
            if (copyBackBoxedExpr != null)
            {
                this._writer.WriteStatement(copyBackBoxedExpr);
            }
        }

        private static string EmitCastRightCastToLeftType(TypeReference leftType, StackInfo right) => 
            $"({Naming.ForVariable(leftType)}){right.Expression}";

        private void EmitCodeForLeaveFromBlock(ExceptionSupport.Node node, Instruction ins)
        {
            int offset = ((Instruction) ins.Operand).Offset;
            if (!node.IsInTryBlock && !node.IsInCatchBlock)
            {
                this._writer.WriteLine(this._labeler.ForJump(offset));
            }
            else
            {
                ExceptionSupport.Node[] source = node.GetTargetFinallyNodesForJump(ins.Offset, offset).ToArray<ExceptionSupport.Node>();
                if (source.Length != 0)
                {
                    ExceptionSupport.Node node2 = source.First<ExceptionSupport.Node>();
                    foreach (ExceptionSupport.Node node3 in source)
                    {
                        this._exceptionSupport.AddLeaveTarget(node3, ins);
                    }
                    object[] args = new object[] { ((Instruction) ins.Operand).Offset, this._labeler.FormatOffset(node2.Start) };
                    this._writer.WriteLine("IL2CPP_LEAVE(0x{0:X}, {1});", args);
                }
                else
                {
                    this._writer.WriteLine(this._labeler.ForJump(offset));
                }
            }
        }

        private void EmitCodeForLeaveFromCatch(ExceptionSupport.Node node, Instruction ins)
        {
            int offset = ((Instruction) ins.Operand).Offset;
            ExceptionSupport.Node[] source = node.GetTargetFinallyNodesForJump(ins.Offset, offset).ToArray<ExceptionSupport.Node>();
            if (source.Length != 0)
            {
                ExceptionSupport.Node node2 = source.First<ExceptionSupport.Node>();
                foreach (ExceptionSupport.Node node3 in source)
                {
                    this._exceptionSupport.AddLeaveTarget(node3, ins);
                }
                object[] args = new object[] { ((Instruction) ins.Operand).Offset, this._labeler.FormatOffset(node2.Start) };
                this._writer.WriteLine("IL2CPP_LEAVE(0x{0:X}, {1});", args);
            }
            else
            {
                this._writer.WriteLine(this._labeler.ForJump(offset));
            }
        }

        private void EmitCodeForLeaveFromFinallyOrFault(Instruction ins)
        {
            this._writer.WriteLine(this._labeler.ForJump(((Instruction) ins.Operand).Offset));
        }

        private void EmitCodeForLeaveFromTry(ExceptionSupport.Node node, Instruction ins)
        {
            int offset = ((Instruction) ins.Operand).Offset;
            ExceptionSupport.Node[] source = node.GetTargetFinallyNodesForJump(ins.Offset, offset).ToArray<ExceptionSupport.Node>();
            if (source.Length != 0)
            {
                ExceptionSupport.Node node2 = source.First<ExceptionSupport.Node>();
                foreach (ExceptionSupport.Node node3 in source)
                {
                    this._exceptionSupport.AddLeaveTarget(node3, ins);
                }
                object[] args = new object[] { ((Instruction) ins.Operand).Offset, this._labeler.FormatOffset(node2.Start) };
                this._writer.WriteLine("IL2CPP_LEAVE(0x{0:X}, {1});", args);
            }
            else
            {
                this._writer.WriteLine(this._labeler.ForJump(offset));
            }
        }

        private Local EmitLocalIntPtrWithValue(string stringValue)
        {
            Local local = this.NewTemp(this.IntPtrTypeReference);
            object[] args = new object[] { local.IdentifierExpression };
            this._writer.WriteLine("{0};", args);
            if (<>f__am$cacheD == null)
            {
                <>f__am$cacheD = f => f.Name == Naming.IntPtrValueField;
            }
            string str = Naming.ForFieldSetter(this.IntPtrTypeReference.Resolve().Fields.First<FieldDefinition>(<>f__am$cacheD));
            object[] objArray2 = new object[] { local.Expression, str, stringValue };
            this._writer.WriteLine("{0}.{1}((void*){2});", objArray2);
            return local;
        }

        private Local EmitLocalUIntPtrWithValue(string stringValue)
        {
            Local local = this.NewTemp(this.UIntPtrTypeReference);
            object[] args = new object[] { local.IdentifierExpression };
            this._writer.WriteLine("{0};", args);
            if (<>f__am$cacheE == null)
            {
                <>f__am$cacheE = f => f.Name == Naming.UIntPtrPointerField;
            }
            string str = Naming.ForFieldSetter(this.UIntPtrTypeReference.Resolve().Fields.First<FieldDefinition>(<>f__am$cacheE));
            object[] objArray2 = new object[] { local.Expression, str, stringValue };
            this._writer.WriteLine("{0}.{1}((void*){2});", objArray2);
            return local;
        }

        private void EmitMemoryBarrierIfNecessary(FieldReference fieldReference = null)
        {
            if (this._thisInstructionIsVolatile || fieldReference.IsVolatile())
            {
                StatsService.RecordMemoryBarrierEmitted(this._methodDefinition);
                this._writer.WriteStatement(Emit.MemoryBarrier());
                this._thisInstructionIsVolatile = false;
            }
        }

        private void EnterCatch(ExceptionSupport.Node node)
        {
            this._writer.BeginBlock($"begin catch({(node.Handler.HandlerType != ExceptionHandlerType.Catch) ? "filter" : node.Handler.CatchType.FullName})");
        }

        private void EnterFault(ExceptionSupport.Node node)
        {
            this._writer.BeginBlock($"begin fault (depth: {node.Depth})");
        }

        private void EnterFilter(ExceptionSupport.Node node)
        {
            this._writer.BeginBlock($"begin filter(depth: {node.Depth})");
            this._writer.WriteLine($"bool {"__filter_local"} = false;");
            this._writer.WriteLine("try");
            this._writer.BeginBlock("begin implicit try block");
        }

        private void EnterFinally(ExceptionSupport.Node node)
        {
            this._writer.BeginBlock($"begin finally (depth: {node.Depth})");
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

                case ExceptionSupport.NodeType.Filter:
                    this.EnterFilter(node);
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
            this._writer.BeginBlock($"begin try (depth: {node.Depth})");
        }

        private void ExitCatch(ExceptionSupport.Node node)
        {
            this._writer.EndBlock($"end catch (depth: {node.Depth})", false);
        }

        private void ExitFault(ExceptionSupport.Node node)
        {
            this._writer.EndBlock("end fault", false);
            object[] args = new object[] { node.Start.Offset };
            this._writer.WriteLine("IL2CPP_CLEANUP({0})", args);
            this._writer.BeginBlock();
            foreach (Instruction instruction in this._exceptionSupport.LeaveTargetsFor(node))
            {
                ExceptionSupport.Node[] source = node.GetTargetFinallyAndFaultNodesForJump(node.End.Offset, instruction.Offset).ToArray<ExceptionSupport.Node>();
                if (source.Length > 0)
                {
                    object[] objArray2 = new object[] { instruction.Offset, this._labeler.FormatOffset(source.First<ExceptionSupport.Node>().Start) };
                    this._writer.WriteLine("IL2CPP_END_CLEANUP(0x{0:X}, {1});", objArray2);
                }
            }
            object[] objArray3 = new object[] { Naming.ForVariable(TypeProvider.SystemException) };
            this._writer.WriteLine("IL2CPP_RETHROW_IF_UNHANDLED({0})", objArray3);
            this._writer.EndBlock(false);
        }

        private void ExitFilter(ExceptionSupport.Node node)
        {
            this._writer.EndBlock("end implicit try block", false);
            this._writer.WriteLine("catch(Il2CppExceptionWrapper&)");
            this._writer.BeginBlock("begin implicit catch block");
            this._writer.WriteLine($"{"__filter_local"} = false;");
            this._writer.EndBlock("end implicit catch block", false);
            this._writer.WriteLine($"if ({"__filter_local"})");
            this._writer.BeginBlock();
            this._writer.WriteLine(this._labeler.ForJump(node.End.Next));
            this._writer.EndBlock(false);
            this._writer.WriteLine("else");
            this._writer.BeginBlock();
            this._writer.WriteStatement(Emit.RaiseManagedException("__exception_local"));
            this._writer.EndBlock(false);
            this._writer.EndBlock($"end filter (depth: {node.Depth})", false);
        }

        private void ExitFinally(ExceptionSupport.Node node)
        {
            this._writer.EndBlock($"end finally (depth: {node.Depth})", false);
            object[] args = new object[] { node.Start.Offset };
            this._writer.WriteLine("IL2CPP_CLEANUP({0})", args);
            this._writer.BeginBlock();
            foreach (Instruction instruction in this._exceptionSupport.LeaveTargetsFor(node))
            {
                ExceptionSupport.Node[] source = node.GetTargetFinallyAndFaultNodesForJump(node.End.Offset, instruction.Offset).ToArray<ExceptionSupport.Node>();
                if (source.Length > 0)
                {
                    object[] objArray2 = new object[] { instruction.Offset, this._labeler.FormatOffset(source.First<ExceptionSupport.Node>().Start) };
                    this._writer.WriteLine("IL2CPP_END_CLEANUP(0x{0:X}, {1});", objArray2);
                }
                else
                {
                    object[] objArray3 = new object[] { instruction.Offset, this._labeler.FormatOffset(instruction) };
                    this._writer.WriteLine("IL2CPP_JUMP_TBL(0x{0:X}, {1})", objArray3);
                }
            }
            object[] objArray4 = new object[] { Naming.ForVariable(TypeProvider.SystemException) };
            this._writer.WriteLine("IL2CPP_RETHROW_IF_UNHANDLED({0})", objArray4);
            this._writer.EndBlock(false);
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

                case ExceptionSupport.NodeType.Filter:
                    this.ExitFilter(node);
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
            this._writer.EndBlock($"end try (depth: {node.Depth})", false);
            ExceptionSupport.Node[] catchNodes = node.CatchNodes;
            ExceptionSupport.Node finallyNode = node.FinallyNode;
            ExceptionSupport.Node[] filterNodes = node.FilterNodes;
            ExceptionSupport.Node faultNode = node.FaultNode;
            this._writer.WriteLine("catch(Il2CppExceptionWrapper& e)");
            this._writer.BeginBlock();
            if (catchNodes.Length != 0)
            {
                object[] args = new object[] { "__exception_local", Naming.ForVariable(TypeProvider.SystemException) };
                this._writer.WriteLine("{0} = ({1})e.ex;", args);
                if (<>f__am$cache4 == null)
                {
                    <>f__am$cache4 = n => n.Handler;
                }
                foreach (ExceptionHandler handler in catchNodes.Select<ExceptionSupport.Node, ExceptionHandler>(<>f__am$cache4))
                {
                    object[] objArray2 = new object[] { this._runtimeMetadataAccess.TypeInfoFor(handler.CatchType) };
                    this._writer.WriteLine("if(il2cpp_codegen_class_is_assignable_from ({0}, il2cpp_codegen_exception_class(e.ex)))", objArray2);
                    this._writer.Indent(1);
                    this._writer.WriteLine(this._labeler.ForJump(handler.HandlerStart));
                    this._writer.Dedent(1);
                }
                if (finallyNode != null)
                {
                    object[] objArray3 = new object[] { "__last_unhandled_exception", Naming.ForVariable(TypeProvider.SystemException) };
                    this._writer.WriteLine("{0} = ({1})e.ex;", objArray3);
                    this._writer.WriteLine(this._labeler.ForJump(finallyNode.Handler.HandlerStart));
                }
                else if (faultNode != null)
                {
                    object[] objArray4 = new object[] { "__last_unhandled_exception", Naming.ForVariable(TypeProvider.SystemException) };
                    this._writer.WriteLine("{0} = ({1})e.ex;", objArray4);
                    this._writer.WriteLine(this._labeler.ForJump(faultNode.Handler.HandlerStart));
                }
                else
                {
                    this._writer.WriteLine("throw e;");
                }
            }
            else if (filterNodes.Length != 0)
            {
                object[] objArray5 = new object[] { "__exception_local", Naming.ForVariable(TypeProvider.SystemException) };
                this._writer.WriteLine("{0} = ({1})e.ex;", objArray5);
            }
            else
            {
                if ((finallyNode == null) && (faultNode == null))
                {
                    throw new NotSupportedException("Try block ends without any catch, finally, nor fault handler");
                }
                object[] objArray6 = new object[] { "__last_unhandled_exception", Naming.ForVariable(TypeProvider.SystemException) };
                this._writer.WriteLine("{0} = ({1})e.ex;", objArray6);
                if (finallyNode != null)
                {
                    this._writer.WriteLine(this._labeler.ForJump(finallyNode.Handler.HandlerStart));
                }
                if (faultNode != null)
                {
                    this._writer.WriteLine(this._labeler.ForJump(faultNode.Handler.HandlerStart));
                }
            }
            this._writer.EndBlock(false);
        }

        private string ExpressionForBinaryOperation(TypeReference type, string expression)
        {
            if ((type.MetadataType == MetadataType.IntPtr) || (type.MetadataType == MetadataType.UIntPtr))
            {
                return this.FormatNativeIntGetterName(expression, type);
            }
            return expression;
        }

        private string FakeBox(TypeReference thisType, string typeInfoVariable, string pointerToValue)
        {
            string str = this.NewTempName();
            this._writer.WriteLine($"Il2CppFakeBox<{Naming.ForVariable(thisType)}> {str}({typeInfoVariable}, {pointerToValue});");
            return str;
        }

        private static List<string> FormatArgumentsForMethodCall(List<TypeReference> parameterTypes, List<StackInfo> stackValues, SharingType sharingType)
        {
            int count = parameterTypes.Count;
            List<string> list = new List<string>();
            for (int i = 0; i < count; i++)
            {
                StringBuilder builder = new StringBuilder();
                StackInfo right = stackValues[i];
                TypeReference variableType = parameterTypes[i];
                if (variableType.IsPointer)
                {
                    builder.Append("(" + Naming.ForVariable(variableType) + ")");
                }
                else if (VarianceSupport.IsNeededForConversion(variableType, right.Type))
                {
                    builder.Append(VarianceSupport.Apply(variableType, right.Type));
                }
                builder.Append(WriteExpressionAndCastIfNeeded(variableType, right, sharingType));
                list.Add(builder.ToString());
            }
            return list;
        }

        private StackInfo FormatLoadTokenFor(Instruction ins)
        {
            object operand = ins.Operand;
            TypeReference type = operand as TypeReference;
            if (type != null)
            {
                return new StackInfo($"LoadTypeToken({this._runtimeMetadataAccess.Il2CppTypeFor(type)})", this.RuntimeTypeHandleTypeReference);
            }
            FieldReference field = operand as FieldReference;
            if (field != null)
            {
                this._writer.AddIncludeForTypeDefinition(this._typeResolver.Resolve(field.DeclaringType));
                return new StackInfo($"LoadFieldToken({this._runtimeMetadataAccess.FieldInfo(field)})", this.RuntimeFieldHandleTypeReference);
            }
            MethodReference method = operand as MethodReference;
            if (method == null)
            {
                throw new ArgumentException();
            }
            this._writer.AddIncludeForTypeDefinition(this._typeResolver.Resolve(method.DeclaringType));
            return new StackInfo($"LoadMethodToken({this._runtimeMetadataAccess.MethodInfo(method)})", this.RuntimeMethodHandleTypeReference);
        }

        private string FormatNativeIntGetterName(string variableName, TypeReference variableType)
        {
            if (variableType.IsSameType(this.IntPtrTypeReference))
            {
                if (<>f__am$cache8 == null)
                {
                    <>f__am$cache8 = f => f.Name == Naming.IntPtrValueField;
                }
                return $"{variableName}.{Naming.ForFieldGetter(TypeProvider.SystemIntPtr.Fields.Single<FieldDefinition>(<>f__am$cache8))}()";
            }
            if (variableType.IsSameType(this.UIntPtrTypeReference))
            {
                if (<>f__am$cache9 == null)
                {
                    <>f__am$cache9 = f => f.Name == Naming.UIntPtrPointerField;
                }
                return $"{variableName}.{Naming.ForFieldGetter(TypeProvider.SystemUIntPtr.Fields.Single<FieldDefinition>(<>f__am$cache9))}()";
            }
            if (!variableType.IsNativeIntegralType())
            {
                throw new ArgumentException("The variableType argument must be a TypeReference to an IntPtr or a an UIntPtr.", "variableType");
            }
            return variableName;
        }

        private string FormatNativeIntSetterInvocation(string variableName, TypeReference variableType, string value)
        {
            if (variableType.IsSameType(this.IntPtrTypeReference))
            {
                if (<>f__am$cacheA == null)
                {
                    <>f__am$cacheA = f => f.Name == Naming.IntPtrValueField;
                }
                return $"{variableName}.{Naming.ForFieldSetter(TypeProvider.SystemIntPtr.Fields.Single<FieldDefinition>(<>f__am$cacheA))}({value});";
            }
            if (variableType.IsSameType(this.UIntPtrTypeReference))
            {
                if (<>f__am$cacheB == null)
                {
                    <>f__am$cacheB = f => f.Name == Naming.UIntPtrPointerField;
                }
                return $"{variableName}.{Naming.ForFieldSetter(TypeProvider.SystemUIntPtr.Fields.Single<FieldDefinition>(<>f__am$cacheB))}({value});";
            }
            if (!variableType.IsNativeIntegralType())
            {
                throw new ArgumentException("The variableType argument must be a TypeReference to an IntPtr or a an UIntPtr.", "variableType");
            }
            return $"{variableName} = {value};";
        }

        public void Generate()
        {
            if (this._methodDefinition.HasBody)
            {
                if (GenericsUtilities.CheckForMaximumRecursion(this._methodReference.DeclaringType as GenericInstanceType) || GenericsUtilities.CheckForMaximumRecursion(this._methodReference as GenericInstanceMethod))
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
                    foreach (ExceptionHandler handler in this._methodDefinition.Body.ExceptionHandlers)
                    {
                        if (handler.CatchType != null)
                        {
                            this._writer.AddIncludeForTypeDefinition(this._typeResolver.Resolve(handler.CatchType));
                        }
                    }
                    foreach (GlobalVariable variable in this._stackAnalysis.Globals)
                    {
                        this._writer.WriteVariable(this._typeResolver.Resolve(variable.Type, true), variable.VariableName);
                    }
                    foreach (ExceptionSupport.Node node in this._exceptionSupport.FlowTree.Children)
                    {
                        this.GenerateCodeRecursive(node);
                    }
                }
            }
        }

        private void GenerateCodeRecursive(ExceptionSupport.Node node)
        {
            SequencePoint point;
            InstructionBlock block = node.Block;
            if (block == null)
            {
                if (node.Children.Length == 0)
                {
                    throw new NotSupportedException("Unexpected empty node!");
                }
                this.WriteLabelForBranchTarget(node.Start);
                this.EnterNode(node);
                foreach (ExceptionSupport.Node node2 in node.Children)
                {
                    this.GenerateCodeRecursive(node2);
                }
                this.ExitNode(node);
                return;
            }
            if (node.Children.Length > 0)
            {
                throw new NotSupportedException("Node with explicit Block should have no children!");
            }
            if (block.IsDead)
            {
                object[] args = new object[] { block.First.ToString() };
                this.WriteComment("Dead block : {0}", args);
                return;
            }
            this._valueStack.Clear();
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = v => v.Index;
            }
            foreach (GlobalVariable variable in this._stackAnalysis.InputVariablesFor(block).OrderBy<GlobalVariable, int>(<>f__am$cache1).Reverse<GlobalVariable>())
            {
                this._valueStack.Push(new StackInfo(variable.VariableName, this._typeResolver.Resolve(variable.Type, true)));
            }
            this._exceptionSupport.PushExceptionOnStackIfNeeded(node, this._valueStack, this._typeResolver, TypeProvider.SystemException);
            if (this._options.EmitBlockInfo)
            {
                object[] objArray2 = new object[] { block.Index };
                this.WriteComment("BLOCK: {0}", objArray2);
            }
            if (this._options.EmitInputAndOutputs)
            {
                this.DumpInsFor(block);
            }
            Instruction first = block.First;
            this.WriteLabelForBranchTarget(first);
            this.EnterNode(node);
            this._classesAlreadyInitializedInBlock.Clear();
        Label_0176:
            point = this.GetSequencePoint(first);
            if (point != null)
            {
                if ((point.StartLine != 0xfeefee) && this._options.EmitLineNumbers)
                {
                    object[] objArray3 = new object[] { point.StartLine, point.Document.Url.Replace(@"\", @"\\") };
                    this._writer.WriteUnindented("#line {0} \"{1}\"", objArray3);
                }
                if (first.OpCode != OpCodes.Nop)
                {
                    SourceAnnotationWriter.EmitAnnotation(this._writer, point);
                }
            }
            if (this._options.EmitIlCode)
            {
                object[] objArray4 = new object[] { first };
                this._writer.WriteUnindented("/* {0} */", objArray4);
            }
            this.ProcessInstruction(node, block, ref first);
            this.ProcessInstructionOperand(first.Operand);
            if ((first.Next != null) && (first != block.Last))
            {
                first = first.Next;
                goto Label_0176;
            }
            if (((first.OpCode.Code < Code.Br_S) || (first.OpCode.Code > Code.Blt_Un)) && (block.Successors.Any<InstructionBlock>() && (first.OpCode.Code != Code.Switch)))
            {
                this.SetupFallthroughVariables(block);
            }
            if (this._options.EmitInputAndOutputs)
            {
                this.DumpOutsFor(block);
            }
            if (this._options.EmitBlockInfo)
            {
                if (block.Successors.Any<InstructionBlock>())
                {
                    object[] objArray5 = new object[2];
                    objArray5[0] = block.Index;
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = b => b.Index.ToString();
                    }
                    objArray5[1] = block.Successors.Select<InstructionBlock, string>(<>f__am$cache2).AggregateWithComma();
                    this.WriteComment("END BLOCK {0} (succ: {1})", objArray5);
                }
                else
                {
                    object[] objArray6 = new object[] { block.Index };
                    this.WriteComment("END BLOCK {0} (succ: none)", objArray6);
                }
                this._writer.WriteLine();
                this._writer.WriteLine();
            }
            this.ExitNode(node);
        }

        private void GenerateConditional(string op, Signedness signedness, bool negate = false)
        {
            this.PushExpression(this.Int32TypeReference, this.ConditionalExpressionFor(op, signedness, negate) + "? 1 : 0");
        }

        private void GenerateConditionalJump(InstructionBlock block, Instruction ins, bool isTrue)
        {
            string expression;
            Instruction operand = (Instruction) ins.Operand;
            StackInfo info = this._valueStack.Pop();
            if ((info.Type.MetadataType == MetadataType.IntPtr) || (info.Type.MetadataType == MetadataType.UIntPtr))
            {
                expression = this.FormatNativeIntGetterName(info.Expression, info.Type);
            }
            else
            {
                expression = info.Expression;
            }
            string conditional = $"{!isTrue ? "!" : ""}{expression}";
            if (this._valueStack.Count == 0)
            {
                using (this.NewIfBlock(conditional))
                {
                    this.WriteJump(operand);
                }
            }
            else
            {
                this.WriteGlobalVariableAssignmentForLeftBranch(block, operand);
                using (this.NewIfBlock(conditional))
                {
                    this.WriteGlobalVariableAssignmentForRightBranch(block, operand);
                    this.WriteJump(operand);
                }
            }
        }

        private void GenerateConditionalJump(InstructionBlock block, Instruction ins, string cppOperator, Signedness signedness, bool negate = false)
        {
            <GenerateConditionalJump>c__AnonStorey6 storey = new <GenerateConditionalJump>c__AnonStorey6();
            string conditional = this.ConditionalExpressionFor(cppOperator, signedness, negate);
            storey.targetInstruction = (Instruction) ins.Operand;
            if (this._valueStack.Count == 0)
            {
                using (this.NewIfBlock(conditional))
                {
                    this.WriteJump(storey.targetInstruction);
                }
            }
            else
            {
                GlobalVariable[] globalVariables = this._stackAnalysis.InputVariablesFor(block.Successors.Single<InstructionBlock>(new Func<InstructionBlock, bool>(storey.<>m__0)));
                GlobalVariable[] variableArray2 = this._stackAnalysis.InputVariablesFor(block.Successors.Single<InstructionBlock>(new Func<InstructionBlock, bool>(storey.<>m__1)));
                this.WriteAssignGlobalVariables(globalVariables);
                using (this.NewIfBlock(conditional))
                {
                    this.WriteAssignGlobalVariables(variableArray2);
                    this.WriteJump(storey.targetInstruction);
                }
            }
        }

        private static string GetArrayAddressCall(MethodReference methodReference, string array, string arguments, bool useArrayBoundsCheck) => 
            Emit.Call($"({array})->{Naming.ForArrayItemAddressGetter(useArrayBoundsCheck)}", arguments);

        private static string GetArrayGetCall(MethodReference methodReference, string array, string arguments, bool useArrayBoundsCheck) => 
            Emit.Call($"({array})->{Naming.ForArrayItemGetter(useArrayBoundsCheck)}", arguments);

        private static string GetArraySetCall(MethodReference methodReference, string array, string arguments, bool useArrayBoundsCheck) => 
            Emit.Call($"({array})->{Naming.ForArrayItemSetter(useArrayBoundsCheck)}", arguments);

        public static string GetAssignment(string leftName, TypeReference leftType, StackInfo right, SharingType sharingType = 0) => 
            Emit.Assign(leftName, WriteExpressionAndCastIfNeeded(leftType, right, sharingType));

        private string GetCastclassOrIsInstCall(TypeReference targetType, StackInfo value, string operation, TypeReference resolvedTypeReference) => 
            Emit.Call(operation + GetOptimizedCastclassOrIsInstMethodSuffix(resolvedTypeReference, this._sharingType), value.Expression, this._runtimeMetadataAccess.TypeInfoFor(targetType));

        private MethodReference GetCreateStringMethod(MethodReference method)
        {
            if (method.DeclaringType.Name != "String")
            {
                throw new Exception("method.DeclaringType.Name != \"String\"");
            }
            if (<>f__am$cacheF == null)
            {
                <>f__am$cacheF = meth => meth.Name == "CreateString";
            }
            foreach (MethodDefinition definition in method.DeclaringType.Resolve().Methods.Where<MethodDefinition>(<>f__am$cacheF))
            {
                if (definition.Parameters.Count == method.Parameters.Count)
                {
                    bool flag = false;
                    for (int i = 0; i < definition.Parameters.Count; i++)
                    {
                        if (definition.Parameters[i].ParameterType.FullName != method.Parameters[i].ParameterType.FullName)
                        {
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        return definition;
                    }
                }
            }
            throw new Exception($"Can't find proper CreateString : {method.FullName}");
        }

        private string GetExpressionFor(StackInfo stackInfo)
        {
            if (stackInfo.Type.IsSameType(TypeProvider.SystemIntPtr))
            {
                if (<>f__am$cache10 == null)
                {
                    <>f__am$cache10 = f => f.Name == Naming.IntPtrValueField;
                }
                FieldDefinition field = TypeProvider.SystemIntPtr.Fields.Single<FieldDefinition>(<>f__am$cache10);
                return $"{stackInfo.Expression}.{Naming.ForFieldGetter(field)}()";
            }
            if (stackInfo.Type.IsSameType(TypeProvider.SystemUIntPtr))
            {
                if (<>f__am$cache11 == null)
                {
                    <>f__am$cache11 = f => f.Name == Naming.UIntPtrPointerField;
                }
                FieldDefinition definition2 = TypeProvider.SystemUIntPtr.Fields.Single<FieldDefinition>(<>f__am$cache11);
                return $"{stackInfo.Expression}.{Naming.ForFieldGetter(definition2)}()";
            }
            return stackInfo.Expression;
        }

        private static string GetLoadIndirectExpression(TypeReference castType, string expression) => 
            $"*(({Naming.ForVariable(castType)}){expression})";

        private static int GetMetadataTypeOrderFor(TypeReference type)
        {
            if (!type.IsSameType(TypeProvider.NativeIntTypeReference) && !type.IsSameType(TypeProvider.NativeUIntTypeReference))
            {
                MetadataType metadataType = type.MetadataType;
                switch (metadataType)
                {
                    case MetadataType.SByte:
                    case MetadataType.Byte:
                        return 0;

                    case MetadataType.Int16:
                    case MetadataType.UInt16:
                        return 1;

                    case MetadataType.Int32:
                    case MetadataType.UInt32:
                        return 2;

                    case MetadataType.Int64:
                    case MetadataType.UInt64:
                        return 4;

                    case MetadataType.Pointer:
                        break;

                    default:
                        if ((metadataType != MetadataType.IntPtr) && (metadataType != MetadataType.UIntPtr))
                        {
                            throw new Exception($"Invalid metadata type for typereference {type}");
                        }
                        break;
                }
            }
            return 3;
        }

        internal static string GetMethodCallExpression(MethodReference callingMethod, MethodReference methodToCall, MethodReference unresolvedMethodtoCall, Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolverForMethodToCall, MethodCallType callType, IRuntimeMetadataAccess runtimeMetadataAccess, VTableBuilder vTableBuilder, IEnumerable<string> argumentArray, bool useArrayBoundsCheck, Func<string, string> addUniqueSuffix = null)
        {
            if (methodToCall.DeclaringType.IsArray && (methodToCall.Name == "Set"))
            {
                return GetArraySetCall(methodToCall, argumentArray.First<string>(), argumentArray.Skip<string>(1).AggregateWithComma(), useArrayBoundsCheck);
            }
            if (methodToCall.DeclaringType.IsArray && (methodToCall.Name == "Get"))
            {
                return GetArrayGetCall(methodToCall, argumentArray.First<string>(), argumentArray.Skip<string>(1).AggregateWithComma(), useArrayBoundsCheck);
            }
            if (methodToCall.DeclaringType.IsArray && (methodToCall.Name == "Address"))
            {
                return GetArrayAddressCall(methodToCall, argumentArray.First<string>(), argumentArray.Skip<string>(1).AggregateWithComma(), useArrayBoundsCheck);
            }
            if (methodToCall.DeclaringType.IsSystemArray() && (methodToCall.Name == "GetGenericValueImpl"))
            {
                return Emit.Call("ArrayGetGenericValueImpl", argumentArray);
            }
            if (methodToCall.DeclaringType.IsSystemArray() && (methodToCall.Name == "SetGenericValueImpl"))
            {
                return Emit.Call("ArraySetGenericValueImpl", argumentArray);
            }
            if (GenericsUtilities.IsGenericInstanceOfCompareExchange(methodToCall))
            {
                GenericInstanceMethod method = (GenericInstanceMethod) methodToCall;
                string str2 = Naming.ForVariable(method.GenericArguments[0]);
                return Emit.Call($"InterlockedCompareExchangeImpl<{str2}>", argumentArray);
            }
            if (GenericsUtilities.IsGenericInstanceOfExchange(methodToCall))
            {
                GenericInstanceMethod method2 = (GenericInstanceMethod) methodToCall;
                string str3 = Naming.ForVariable(method2.GenericArguments[0]);
                return Emit.Call($"InterlockedExchangeImpl<{str3}>", argumentArray);
            }
            if (IntrinsicRemap.ShouldRemap(methodToCall))
            {
                return Emit.Call(IntrinsicRemap.MappedNameFor(methodToCall), !IntrinsicRemap.HasCustomArguments(methodToCall) ? argumentArray : IntrinsicRemap.GetCustomArguments(methodToCall, callingMethod, runtimeMetadataAccess, argumentArray));
            }
            if (methodToCall.Resolve().IsStatic)
            {
                return Emit.Call(runtimeMetadataAccess.Method(unresolvedMethodtoCall), argumentArray);
            }
            if (callType == MethodCallType.DirectVirtual)
            {
                string methodPointerForVTable = MethodSignatureWriter.GetMethodPointerForVTable(methodToCall);
                string str5 = addUniqueSuffix("il2cpp_virtual_invoke_data_");
                if (CodeGenOptions.MonoRuntime)
                {
                    return Emit.Call("(" + Emit.Cast(MethodSignatureWriter.GetMethodPointerForVTable(methodToCall), $"il2cpp_codegen_vtable_slot_method_pointer({addUniqueSuffix("il2cpp_this_typeinfo")}, {Emit.MonoMethodMetadataGet(unresolvedMethodtoCall)})") + ")", argumentArray);
                }
                return Emit.Call("(" + Emit.Cast(methodPointerForVTable, $"{str5}.methodPtr") + ")", argumentArray);
            }
            if ((callType != MethodCallType.Virtual) || MethodSignatureWriter.CanDevirtualizeMethodCall(methodToCall.Resolve()))
            {
                if (unresolvedMethodtoCall.DeclaringType.IsValueType())
                {
                    return Emit.Call(runtimeMetadataAccess.Method(methodToCall), argumentArray);
                }
                return Emit.Call(runtimeMetadataAccess.Method(unresolvedMethodtoCall), argumentArray);
            }
            return VirtualCallFor(methodToCall, unresolvedMethodtoCall, argumentArray, typeResolverForMethodToCall, runtimeMetadataAccess, vTableBuilder);
        }

        private static string GetOptimizedCastclassOrIsInstMethodSuffix(TypeReference resolvedTypeReference, SharingType sharingType)
        {
            if (((sharingType == SharingType.NonShared) && !resolvedTypeReference.IsInterface()) && (!resolvedTypeReference.IsArray && !resolvedTypeReference.IsNullable()))
            {
                return (!resolvedTypeReference.Resolve().IsSealed ? "Class" : "Sealed");
            }
            return string.Empty;
        }

        private static List<TypeReference> GetParameterTypes(MethodReference method, Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolverForMethodToCall)
        {
            <GetParameterTypes>c__AnonStorey8 storey = new <GetParameterTypes>c__AnonStorey8 {
                typeResolverForMethodToCall = typeResolverForMethodToCall,
                method = method
            };
            return new List<TypeReference>(storey.method.Parameters.Select<ParameterDefinition, TypeReference>(new Func<ParameterDefinition, TypeReference>(storey.<>m__0)));
        }

        private static TypeReference GetPointerOrByRefType(StackInfo address)
        {
            TypeReference typeReference = address.Type;
            typeReference = Naming.RemoveModifiers(typeReference);
            PointerType type = typeReference as PointerType;
            if (type != null)
            {
                return type.ElementType;
            }
            ByReferenceType type2 = typeReference as ByReferenceType;
            return type2?.ElementType;
        }

        private SequencePoint GetSequencePoint(Instruction ins) => 
            ins.GetSequencePoint(this._methodDefinition);

        private TypeReference GetSignedType(TypeReference type)
        {
            if (type.IsSameType(this.NativeIntTypeReference) || type.IsSameType(this.NativeUIntTypeReference))
            {
                return this.NativeIntTypeReference;
            }
            switch (type.MetadataType)
            {
                case MetadataType.SByte:
                case MetadataType.Byte:
                    return this.SByteTypeReference;

                case MetadataType.Int16:
                case MetadataType.UInt16:
                    return this.Int16TypeReference;

                case MetadataType.Int32:
                case MetadataType.UInt32:
                    return this.Int32TypeReference;

                case MetadataType.Int64:
                case MetadataType.UInt64:
                    return this.Int64TypeReference;

                case MetadataType.IntPtr:
                case MetadataType.UIntPtr:
                    return this.NativeIntTypeReference;
            }
            return type;
        }

        private TypeReference GetUnsignedType(TypeReference type)
        {
            if (type.IsSameType(this.NativeIntTypeReference) || type.IsSameType(this.NativeUIntTypeReference))
            {
                return this.NativeUIntTypeReference;
            }
            switch (type.MetadataType)
            {
                case MetadataType.SByte:
                case MetadataType.Byte:
                    return this.ByteTypeReference;

                case MetadataType.Int16:
                case MetadataType.UInt16:
                    return this.UInt16TypeReference;

                case MetadataType.Int32:
                case MetadataType.UInt32:
                    return this.UInt32TypeReference;

                case MetadataType.Int64:
                case MetadataType.UInt64:
                    return this.UInt64TypeReference;

                case MetadataType.IntPtr:
                case MetadataType.UIntPtr:
                    return this.NativeUIntTypeReference;
            }
            return type;
        }

        private static bool IsNonPointerReferenceType(StackInfo stackEntry) => 
            (!stackEntry.Type.IsValueType() && !stackEntry.Type.IsPointer);

        private void LoadArgumentAddress(ParameterReference parameter)
        {
            ParameterReference reference = parameter;
            int index = reference.Index;
            this._valueStack.Push(new StackInfo("(&" + ParameterNameFor(this._methodDefinition, index) + ")", new ByReferenceType(this._typeResolver.ResolveParameterType(this._methodReference, reference))));
        }

        private void LoadConstant(TypeReference type, string stringValue)
        {
            this.PushExpression(type, stringValue);
        }

        private void LoadElem(StackInfo array, TypeReference objectType, StackInfo index)
        {
            this._nullCheckSupport.WriteNullCheckIfNeeded(array);
            Local local = this.NewTemp(index.Type);
            object[] args = new object[] { local.IdentifierExpression, index.Expression };
            this._writer.WriteLine("{0} = {1};", args);
            string expression = local.Expression;
            if (local.Type.IsIntegralPointerType())
            {
                expression = this.ArrayIndexerForIntPtr(local.Expression, local.Type);
            }
            this._arrayBoundsCheckSupport.RecordArrayBoundsCheckEmitted();
            this.StoreLocalAndPush(objectType, Emit.LoadArrayElement(array.Expression, expression, this._arrayBoundsCheckSupport.ShouldEmitBoundsChecksForMethod()));
        }

        private void LoadElemAndPop(TypeReference typeReference)
        {
            StackInfo index = this._valueStack.Pop();
            StackInfo array = this._valueStack.Pop();
            this.LoadElem(array, typeReference, index);
        }

        private void LoadField(Instruction ins, bool loadAddress = false)
        {
            string str;
            StackInfo stackInfo = this._valueStack.Pop();
            FieldReference operand = (FieldReference) ins.Operand;
            TypeReference type = this._typeResolver.ResolveFieldType(operand);
            if (loadAddress)
            {
                type = new ByReferenceType(type);
                str = Naming.ForFieldAddressGetter(operand);
            }
            else
            {
                str = Naming.ForFieldGetter(operand);
            }
            if (stackInfo.Expression != Naming.ThisParameterName)
            {
                this._nullCheckSupport.WriteNullCheckIfNeeded(stackInfo);
            }
            Local local = this.NewTemp(type);
            this._valueStack.Push(new StackInfo(local));
            string str2 = Emit.Call((!stackInfo.Type.IsValueType() || stackInfo.Type.IsNativeIntegralType()) ? Emit.Arrow(CastReferenceTypeOrNativeIntIfNeeded(stackInfo, this._typeResolver.Resolve(operand.DeclaringType)), str) : Emit.Dot(stackInfo.Expression, str));
            if (this._sharingType == SharingType.Shared)
            {
                str2 = Emit.Cast(type, str2);
            }
            this._writer.WriteLine(Emit.Assign(local.IdentifierExpression, str2) + ";");
            this.EmitMemoryBarrierIfNecessary(operand);
        }

        private void LoadIndirect(TypeReference valueType, TypeReference storageType)
        {
            StackInfo info = this._valueStack.Pop();
            if (this._thisInstructionIsVolatile)
            {
                Local local = this.NewTemp(storageType);
                object[] args = new object[] { local.IdentifierExpression, GetLoadIndirectExpression(new PointerType(valueType), info.Expression) };
                this._writer.WriteLine("{0} = {1};", args);
                this.EmitMemoryBarrierIfNecessary(null);
                this._valueStack.Push(new StackInfo(local));
            }
            else
            {
                this.PushLoadIndirectExpression(storageType, new PointerType(valueType), info.Expression);
            }
        }

        private void LoadIndirectNativeInteger()
        {
            StackInfo address = this._valueStack.Pop();
            TypeReference pointerOrByRefType = GetPointerOrByRefType(address);
            if (pointerOrByRefType.IsIntegralPointerType())
            {
                this.PushExpression(pointerOrByRefType, $"(*({address.Expression}))");
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

        private void LoadIndirectReference()
        {
            StackInfo address = this._valueStack.Pop();
            TypeReference pointerOrByRefType = GetPointerOrByRefType(address);
            this.PushLoadIndirectExpression(pointerOrByRefType, address.Type, address.Expression);
        }

        private void LoadInt32Constant(int value)
        {
            this._valueStack.Push((value >= 0) ? new StackInfo(value.ToString(), this.Int32TypeReference) : new StackInfo($"({value})", this.Int32TypeReference));
        }

        private void LoadLocalAddress(VariableReference variableReference)
        {
            this._valueStack.Push(new StackInfo("(&" + Naming.ForVariableName(variableReference) + ")", new ByReferenceType(this._typeResolver.Resolve(variableReference.VariableType))));
        }

        private void LoadLong(Instruction ins, TypeReference type)
        {
            long operand = (long) ins.Operand;
            string str = operand + "LL";
            switch (operand)
            {
                case -9223372036854775808L:
                    str = "std::numeric_limits<int64_t>::min()";
                    break;

                case 0x7fffffffffffffffL:
                    str = "std::numeric_limits<int64_t>::max()";
                    break;
            }
            this.PushExpression(type, Emit.Cast(type, str));
        }

        private void LoadNull()
        {
            this._valueStack.Push(new StackInfo("NULL", this.ObjectTypeReference));
        }

        private void LoadPrimitiveTypeInt32(Instruction ins, TypeReference type)
        {
            int operand = (int) ins.Operand;
            string str = operand.ToString();
            long num2 = operand;
            if ((num2 <= -2147483648L) || (num2 >= 0x7fffffffL))
            {
                str = str + "LL";
            }
            this.PushExpression(type, Emit.Cast(type, str));
        }

        private void LoadPrimitiveTypeSByte(Instruction ins, TypeReference type)
        {
            sbyte operand = (sbyte) ins.Operand;
            this.PushExpression(type, Emit.Cast(type, operand.ToString()));
        }

        private void LoadVirtualFunction(Instruction ins)
        {
            MethodReference operand = (MethodReference) ins.Operand;
            MethodDefinition methodDefinition = operand.Resolve();
            StackInfo info = this._valueStack.Pop();
            if (methodDefinition.IsVirtual)
            {
                this.PushCallToLoadVirtualFunction(operand, methodDefinition, info.Expression);
            }
            else
            {
                this.PushCallToLoadFunction(operand);
            }
        }

        private static bool NeedsNullArgForStaticMethod(MethodReference method, bool isConstructor)
        {
            if (method.HasThis)
            {
                return false;
            }
            if (isConstructor)
            {
                return false;
            }
            if (IntrinsicRemap.ShouldRemap(method))
            {
                return false;
            }
            if (GenericsUtilities.IsGenericInstanceOfCompareExchange(method))
            {
                return false;
            }
            if (GenericsUtilities.IsGenericInstanceOfExchange(method))
            {
                return false;
            }
            return true;
        }

        private IDisposable NewBlock() => 
            new BlockWriter(this._writer, false);

        private IDisposable NewIfBlock(string conditional)
        {
            object[] args = new object[] { conditional };
            this._writer.WriteLine("if ({0})", args);
            return this.NewBlock();
        }

        private Local NewTemp(TypeReference type)
        {
            if (type.ContainsGenericParameters())
            {
                throw new InvalidOperationException("Callers should resolve the type prior to calling this method.");
            }
            return new Local(type, this.NewTempName());
        }

        private string NewTempName() => 
            ("L_" + this._tempIndex++);

        private static string ParameterNameFor(MethodDefinition method, int i)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            return Naming.ForParameterName(method.Parameters[i]);
        }

        private static List<StackInfo> PopItemsFromStack(int amount, Stack<StackInfo> valueStack)
        {
            if (amount > valueStack.Count)
            {
                throw new Exception($"Attempting to pop '{amount}' values from a stack of depth '{valueStack.Count}'.");
            }
            List<StackInfo> list = new List<StackInfo>();
            for (int i = 0; i != amount; i++)
            {
                list.Add(valueStack.Pop());
            }
            list.Reverse();
            return list;
        }

        private static string PrependCastToObject(string expression) => 
            $"({Naming.ForType(TypeProvider.SystemObject)}*){expression}";

        private void ProcessFieldReferenceOperand(FieldReference fieldReference)
        {
            this._writer.AddIncludeForTypeDefinition(this._typeResolver.Resolve(fieldReference.DeclaringType));
            this._writer.AddIncludeForTypeDefinition(this._typeResolver.Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveFieldTypeIfNeeded(fieldReference)));
        }

        private void ProcessInstruction(ExceptionSupport.Node node, InstructionBlock block, ref Instruction ins)
        {
            Local local;
            ErrorInformation.CurrentlyProcessing.Instruction = ins;
            if (this.GetSequencePoint(ins) != null)
            {
                DebuggerSupportFactory.GetDebuggerSupport().WriteSequencePoint(this._writer, ins, false);
            }
            switch (ins.OpCode.Code)
            {
                case Code.Nop:
                    DebuggerSupportFactory.GetDebuggerSupport().WriteSequencePoint(this._writer, ins, true);
                    return;

                case Code.Break:
                    DebuggerSupportFactory.GetDebuggerSupport().WriteDebugBreak(this._writer);
                    return;

                case Code.Ldarg_0:
                    this.WriteLdarg(0, block, ins);
                    return;

                case Code.Ldarg_1:
                    this.WriteLdarg(1, block, ins);
                    return;

                case Code.Ldarg_2:
                    this.WriteLdarg(2, block, ins);
                    return;

                case Code.Ldarg_3:
                    this.WriteLdarg(3, block, ins);
                    return;

                case Code.Ldloc_0:
                    this.WriteLdloc(0, block, ins);
                    return;

                case Code.Ldloc_1:
                    this.WriteLdloc(1, block, ins);
                    return;

                case Code.Ldloc_2:
                    this.WriteLdloc(2, block, ins);
                    return;

                case Code.Ldloc_3:
                    this.WriteLdloc(3, block, ins);
                    return;

                case Code.Stloc_0:
                    this.WriteStloc(0);
                    return;

                case Code.Stloc_1:
                    this.WriteStloc(1);
                    return;

                case Code.Stloc_2:
                    this.WriteStloc(2);
                    return;

                case Code.Stloc_3:
                    this.WriteStloc(3);
                    return;

                case Code.Ldarg_S:
                {
                    ParameterReference operand = (ParameterReference) ins.Operand;
                    int index = operand.Index;
                    if (this._methodDefinition.HasThis)
                    {
                        index++;
                    }
                    this.WriteLdarg(index, block, ins);
                    return;
                }
                case Code.Ldarga_S:
                    this.LoadArgumentAddress((ParameterReference) ins.Operand);
                    return;

                case Code.Starg_S:
                    this.StoreArg(ins);
                    return;

                case Code.Ldloc_S:
                case Code.Ldloc:
                {
                    VariableReference reference2 = (VariableReference) ins.Operand;
                    this.WriteLdloc(reference2.Index, block, ins);
                    return;
                }
                case Code.Ldloca_S:
                    this.LoadLocalAddress((VariableReference) ins.Operand);
                    return;

                case Code.Stloc_S:
                case Code.Stloc:
                {
                    VariableReference reference3 = (VariableReference) ins.Operand;
                    this.WriteStloc(reference3.Index);
                    return;
                }
                case Code.Ldnull:
                    this.LoadNull();
                    return;

                case Code.Ldc_I4_M1:
                    this.LoadInt32Constant(-1);
                    return;

                case Code.Ldc_I4_0:
                    this.LoadInt32Constant(0);
                    return;

                case Code.Ldc_I4_1:
                    this.LoadInt32Constant(1);
                    return;

                case Code.Ldc_I4_2:
                    this.LoadInt32Constant(2);
                    return;

                case Code.Ldc_I4_3:
                    this.LoadInt32Constant(3);
                    return;

                case Code.Ldc_I4_4:
                    this.LoadInt32Constant(4);
                    return;

                case Code.Ldc_I4_5:
                    this.LoadInt32Constant(5);
                    return;

                case Code.Ldc_I4_6:
                    this.LoadInt32Constant(6);
                    return;

                case Code.Ldc_I4_7:
                    this.LoadInt32Constant(7);
                    return;

                case Code.Ldc_I4_8:
                    this.LoadInt32Constant(8);
                    return;

                case Code.Ldc_I4_S:
                    this.LoadPrimitiveTypeSByte(ins, this.Int32TypeReference);
                    return;

                case Code.Ldc_I4:
                    this.LoadPrimitiveTypeInt32(ins, this.Int32TypeReference);
                    return;

                case Code.Ldc_I8:
                    this.LoadLong(ins, TypeProvider.Int64TypeReference);
                    return;

                case Code.Ldc_R4:
                    this.LoadConstant(this.SingleTypeReference, Unity.IL2CPP.Formatter.StringRepresentationFor((float) ins.Operand));
                    return;

                case Code.Ldc_R8:
                    this.LoadConstant(this.DoubleTypeReference, Unity.IL2CPP.Formatter.StringRepresentationFor((double) ins.Operand));
                    return;

                case Code.Dup:
                    this.WriteDup();
                    return;

                case Code.Pop:
                    this._valueStack.Pop();
                    return;

                case Code.Jmp:
                    throw new NotImplementedException();

                case Code.Call:
                {
                    <ProcessInstruction>c__AnonStorey1 storey2 = new <ProcessInstruction>c__AnonStorey1();
                    if (this._constrainedCallThisType != null)
                    {
                        throw new InvalidOperationException($"Constrained opcode was followed a Call rather than a Callvirt in method '{this._methodReference.FullName}' at instruction '{ins}'");
                    }
                    storey2.suffix = "_" + ins.Offset;
                    MethodReference unresolvedMethodToCall = (MethodReference) ins.Operand;
                    string callExpression = this.CallExpressionFor(this._methodReference, unresolvedMethodToCall, MethodCallType.Normal, PopItemsFromStack(unresolvedMethodToCall.Parameters.Count + (!unresolvedMethodToCall.HasThis ? 0 : 1), this._valueStack), new Func<string, string>(storey2.<>m__0), true);
                    this.EmitCallExpressionAndStoreResult(ins, this._typeResolver.ResolveReturnType(unresolvedMethodToCall), callExpression, null);
                    return;
                }
                case Code.Calli:
                    throw new NotImplementedException();

                case Code.Ret:
                    this.WriteReturnStatement();
                    return;

                case Code.Br_S:
                case Code.Br:
                    this.WriteUnconditionalJumpTo(block, (Instruction) ins.Operand);
                    return;

                case Code.Brfalse_S:
                case Code.Brfalse:
                    this.GenerateConditionalJump(block, ins, false);
                    return;

                case Code.Brtrue_S:
                case Code.Brtrue:
                    this.GenerateConditionalJump(block, ins, true);
                    return;

                case Code.Beq_S:
                case Code.Beq:
                    this.GenerateConditionalJump(block, ins, "==", Signedness.Signed, false);
                    return;

                case Code.Bge_S:
                case Code.Bge:
                    this.GenerateConditionalJump(block, ins, ">=", Signedness.Signed, false);
                    return;

                case Code.Bgt_S:
                case Code.Bgt:
                    this.GenerateConditionalJump(block, ins, ">", Signedness.Signed, false);
                    return;

                case Code.Ble_S:
                case Code.Ble:
                    this.GenerateConditionalJump(block, ins, "<=", Signedness.Signed, false);
                    return;

                case Code.Blt_S:
                case Code.Blt:
                    this.GenerateConditionalJump(block, ins, "<", Signedness.Signed, false);
                    return;

                case Code.Bne_Un_S:
                case Code.Bne_Un:
                    this.GenerateConditionalJump(block, ins, "==", Signedness.Unsigned, true);
                    return;

                case Code.Bge_Un_S:
                case Code.Bge_Un:
                    this.GenerateConditionalJump(block, ins, "<", Signedness.Unsigned, true);
                    return;

                case Code.Bgt_Un_S:
                case Code.Bgt_Un:
                    this.GenerateConditionalJump(block, ins, "<=", Signedness.Unsigned, true);
                    return;

                case Code.Ble_Un_S:
                case Code.Ble_Un:
                    this.GenerateConditionalJump(block, ins, ">", Signedness.Unsigned, true);
                    return;

                case Code.Blt_Un_S:
                case Code.Blt_Un:
                    this.GenerateConditionalJump(block, ins, ">=", Signedness.Unsigned, true);
                    return;

                case Code.Switch:
                {
                    <ProcessInstruction>c__AnonStorey2 storey3 = new <ProcessInstruction>c__AnonStorey2();
                    StackInfo info = this._valueStack.Pop();
                    storey3.targetInstructions = (Instruction[]) ins.Operand;
                    int num2 = 0;
                    List<InstructionBlock> source = new List<InstructionBlock>(block.Successors);
                    InstructionBlock item = source.SingleOrDefault<InstructionBlock>(new Func<InstructionBlock, bool>(storey3.<>m__0));
                    if (item != null)
                    {
                        source.Remove(item);
                        this.WriteAssignGlobalVariables(this._stackAnalysis.InputVariablesFor(item));
                    }
                    Instruction[] targetInstructions = storey3.targetInstructions;
                    for (int i = 0; i < targetInstructions.Length; i++)
                    {
                        <ProcessInstruction>c__AnonStorey3 storey4 = new <ProcessInstruction>c__AnonStorey3 {
                            targetInstruction = targetInstructions[i]
                        };
                        using (this.NewIfBlock($"{info} == {num2++}"))
                        {
                            InstructionBlock block3 = source.First<InstructionBlock>(new Func<InstructionBlock, bool>(storey4.<>m__0));
                            this.WriteAssignGlobalVariables(this._stackAnalysis.InputVariablesFor(block3));
                            this.WriteJump(storey4.targetInstruction);
                        }
                    }
                    return;
                }
                case Code.Ldind_I1:
                    this.LoadIndirect(this.SByteTypeReference, this.Int32TypeReference);
                    return;

                case Code.Ldind_U1:
                    this.LoadIndirect(this.ByteTypeReference, this.Int32TypeReference);
                    return;

                case Code.Ldind_I2:
                    this.LoadIndirect(this.Int16TypeReference, this.Int32TypeReference);
                    return;

                case Code.Ldind_U2:
                    this.LoadIndirect(this.UInt16TypeReference, this.Int32TypeReference);
                    return;

                case Code.Ldind_I4:
                    this.LoadIndirect(this.Int32TypeReference, this.Int32TypeReference);
                    return;

                case Code.Ldind_U4:
                    this.LoadIndirect(this.UInt32TypeReference, this.Int32TypeReference);
                    return;

                case Code.Ldind_I8:
                    this.LoadIndirect(this.Int64TypeReference, this.Int64TypeReference);
                    return;

                case Code.Ldind_I:
                    this.LoadIndirectNativeInteger();
                    return;

                case Code.Ldind_R4:
                    this.LoadIndirect(this.SingleTypeReference, this.SingleTypeReference);
                    return;

                case Code.Ldind_R8:
                    this.LoadIndirect(this.DoubleTypeReference, this.DoubleTypeReference);
                    return;

                case Code.Ldind_Ref:
                    this.LoadIndirectReference();
                    return;

                case Code.Stind_Ref:
                    this.StoreIndirect(this.ObjectTypeReference);
                    return;

                case Code.Stind_I1:
                    this.StoreIndirect(this.SByteTypeReference);
                    return;

                case Code.Stind_I2:
                    this.StoreIndirect(this.Int16TypeReference);
                    return;

                case Code.Stind_I4:
                    this.StoreIndirect(this.Int32TypeReference);
                    return;

                case Code.Stind_I8:
                    this.StoreIndirect(this.Int64TypeReference);
                    return;

                case Code.Stind_R4:
                    this.StoreIndirect(this.SingleTypeReference);
                    return;

                case Code.Stind_R8:
                    this.StoreIndirect(this.DoubleTypeReference);
                    return;

                case Code.Add:
                    this.WriteAdd(OverflowCheck.None);
                    return;

                case Code.Sub:
                    this.WriteSub(OverflowCheck.None);
                    return;

                case Code.Mul:
                    this.WriteMul(OverflowCheck.None);
                    return;

                case Code.Div:
                    this._divideByZeroCheckSupport.WriteDivideByZeroCheckIfNeeded(this._valueStack.Peek());
                    this.WriteBinaryOperationUsingLargestOperandTypeAsResultType("/");
                    return;

                case Code.Div_Un:
                    this._divideByZeroCheckSupport.WriteDivideByZeroCheckIfNeeded(this._valueStack.Peek());
                    this.WriteUnsignedArithmeticOperation("/");
                    return;

                case Code.Rem:
                    this.WriteRemainderOperation();
                    return;

                case Code.Rem_Un:
                    this.WriteUnsignedArithmeticOperation("%");
                    return;

                case Code.And:
                    this.WriteBinaryOperationUsingLeftOperandTypeAsResultType("&");
                    return;

                case Code.Or:
                    this.WriteBinaryOperationUsingLargestOperandTypeAsResultType("|");
                    return;

                case Code.Xor:
                    this.WriteBinaryOperationUsingLargestOperandTypeAsResultType("^");
                    return;

                case Code.Shl:
                    this.WriteBinaryOperationUsingLeftOperandTypeAsResultType("<<");
                    return;

                case Code.Shr:
                    this.WriteBinaryOperationUsingLeftOperandTypeAsResultType(">>");
                    return;

                case Code.Shr_Un:
                    this.WriteShrUn();
                    return;

                case Code.Neg:
                    this.WriteNegateOperation();
                    return;

                case Code.Not:
                    this.WriteNotOperation();
                    return;

                case Code.Conv_I1:
                    this.WriteNumericConversion(this.SByteTypeReference);
                    return;

                case Code.Conv_I2:
                    this.WriteNumericConversion(this.Int16TypeReference);
                    return;

                case Code.Conv_I4:
                    this.WriteNumericConversion(this.Int32TypeReference);
                    return;

                case Code.Conv_I8:
                    this.WriteNumericConversionI8();
                    return;

                case Code.Conv_R4:
                    this.WriteNumericConversionFloat(this.SingleTypeReference);
                    return;

                case Code.Conv_R8:
                    this.WriteNumericConversionFloat(this.DoubleTypeReference);
                    return;

                case Code.Conv_U4:
                    this.WriteNumericConversion(this.UInt32TypeReference, this.Int32TypeReference);
                    return;

                case Code.Conv_U8:
                    this.WriteNumericConversionU8();
                    return;

                case Code.Callvirt:
                {
                    string str;
                    <ProcessInstruction>c__AnonStorey0 storey = new <ProcessInstruction>c__AnonStorey0();
                    MethodReference methodReference = (MethodReference) ins.Operand;
                    if (methodReference.IsStatic())
                    {
                        throw new InvalidOperationException($"In method '{this._methodReference.FullName}', an attempt to call the static method '{methodReference.FullName}' with the callvirt opcode is not valid IL. Use the call opcode instead.");
                    }
                    List<StackInfo> poppedValues = PopItemsFromStack(methodReference.Parameters.Count + 1, this._valueStack);
                    storey.suffix = "_" + ins.Offset;
                    string copyBackBoxedExpr = null;
                    if (this._constrainedCallThisType != null)
                    {
                        MethodReference resolvedMethodToCall = this._typeResolver.Resolve(methodReference);
                        str = this.ConstrainedCallExpressionFor(resolvedMethodToCall, ref methodReference, MethodCallType.Virtual, poppedValues, new Func<string, string>(storey.<>m__0), out copyBackBoxedExpr);
                    }
                    else
                    {
                        str = this.CallExpressionFor(this._methodReference, methodReference, MethodCallType.Virtual, poppedValues, new Func<string, string>(storey.<>m__1), true);
                    }
                    this.EmitCallExpressionAndStoreResult(ins, this._typeResolver.ResolveReturnType(methodReference), str, copyBackBoxedExpr);
                    this._constrainedCallThisType = null;
                    return;
                }
                case Code.Cpobj:
                    throw new NotImplementedException();

                case Code.Ldobj:
                    this.WriteLoadObject(ins);
                    return;

                case Code.Ldstr:
                {
                    MetadataToken token;
                    string literal = (string) ins.Operand;
                    this._methodDefinition.Body.GetInstructionToken(ins, out token);
                    this._writer.AddIncludeForTypeDefinition(this.StringTypeReference);
                    this._valueStack.Push(new StackInfo(this._runtimeMetadataAccess.StringLiteral(literal, token, this._methodDefinition.Module.Assembly), this.StringTypeReference));
                    return;
                }
                case Code.Newobj:
                {
                    MethodReference method = (MethodReference) ins.Operand;
                    MethodReference reference7 = this._typeResolver.Resolve(method);
                    TypeReference reference8 = this._typeResolver.Resolve(reference7.DeclaringType);
                    local = this.NewTemp(reference8);
                    this._writer.AddIncludeForTypeDefinition(this._typeResolver.Resolve(method.DeclaringType));
                    GenericInstanceType declaringType = this._methodReference.DeclaringType as GenericInstanceType;
                    if ((declaringType != null) && !(reference7 is GenericInstanceMethod))
                    {
                        reference7 = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(declaringType).Resolve(reference7);
                    }
                    List<TypeReference> parameterTypes = GetParameterTypes(reference7, this._typeResolver);
                    List<string> arguments = FormatArgumentsForMethodCall(parameterTypes, PopItemsFromStack(parameterTypes.Count, this._valueStack), this._sharingType);
                    if (MethodSignatureWriter.NeedsHiddenMethodInfo(reference7, MethodCallType.Normal, true))
                    {
                        arguments.Add((!CodeGenOptions.EmitComments ? "" : "/*hidden argument*/") + this._runtimeMetadataAccess.HiddenMethodInfo(method));
                    }
                    if (!reference8.IsArray)
                    {
                        if (reference8.IsValueType())
                        {
                            string str6 = Naming.AddressOf(local.Expression);
                            arguments.Insert(0, str6);
                            this._writer.WriteVariable(reference8, local.Expression);
                            this._writer.WriteStatement(Emit.Call(this._runtimeMetadataAccess.Method(reference7), arguments));
                        }
                        else if ((reference7.Name == ".ctor") && (reference8.MetadataType == MetadataType.String))
                        {
                            arguments.Insert(0, Naming.Null);
                            this._writer.WriteStatement(Emit.Assign(local.IdentifierExpression, Emit.Call(this._runtimeMetadataAccess.Method(this.GetCreateStringMethod(reference7)), arguments)));
                        }
                        else
                        {
                            this._writer.WriteStatement(Emit.Assign(local.IdentifierExpression, Emit.Cast(reference8, Emit.Call("il2cpp_codegen_object_new", this._runtimeMetadataAccess.Newobj(method)))));
                            if (CodeGenOptions.MonoRuntime)
                            {
                                this.WriteCallToClassAndInitializerAndStaticConstructorIfNeeded(reference8, this._methodDefinition, this._runtimeMetadataAccess);
                            }
                            arguments.Insert(0, local.Expression);
                            this._writer.WriteStatement(Emit.Call(this._runtimeMetadataAccess.Method(method), arguments));
                        }
                        break;
                    }
                    ArrayType type = (ArrayType) reference8;
                    if (type.Rank < 2)
                    {
                        throw new NotImplementedException("Attempting to create a multidimensional array of rank lesser than 2");
                    }
                    string str5 = this.NewTempName();
                    object[] args = new object[] { Naming.ForArrayIndexType(), str5, Emit.CastEach(Naming.ForArrayIndexType(), arguments).AggregateWithComma() };
                    this._writer.WriteLine("{0} {1}[] = {{ {2} }};", args);
                    object[] objArray2 = new object[] { Emit.Assign(local.IdentifierExpression, Emit.Cast(type, Emit.Call("GenArrayNew", this._runtimeMetadataAccess.TypeInfoFor(method.DeclaringType), str5))) };
                    this._writer.WriteLine("{0};", objArray2);
                    break;
                }
                case Code.Castclass:
                    this.WriteCastclassOrIsInst((TypeReference) ins.Operand, this._valueStack.Pop(), "Castclass");
                    return;

                case Code.Isinst:
                    this.WriteCastclassOrIsInst((TypeReference) ins.Operand, this._valueStack.Pop(), "IsInst");
                    return;

                case Code.Conv_R_Un:
                    this.WriteNumericConversionToFloatFromUnsigned();
                    return;

                case Code.Unbox:
                    this.Unbox(ins);
                    return;

                case Code.Throw:
                {
                    StackInfo info2 = this._valueStack.Pop();
                    this._writer.WriteStatement(Emit.RaiseManagedException(info2.ToString()));
                    return;
                }
                case Code.Ldfld:
                    this.LoadField(ins, false);
                    return;

                case Code.Ldflda:
                    this.LoadField(ins, true);
                    return;

                case Code.Stfld:
                    this.StoreField(ins);
                    return;

                case Code.Ldsfld:
                case Code.Ldsflda:
                case Code.Stsfld:
                    this.StaticFieldAccess(ins);
                    return;

                case Code.Stobj:
                    this.WriteStoreObject((TypeReference) ins.Operand);
                    return;

                case Code.Conv_Ovf_I1_Un:
                    this.WriteNumericConversionWithOverflow<sbyte>(this.ByteTypeReference, true, 0x7f, true);
                    return;

                case Code.Conv_Ovf_I2_Un:
                    this.WriteNumericConversionWithOverflow<short>(this.Int16TypeReference, true, 0x7fff, true);
                    return;

                case Code.Conv_Ovf_I4_Un:
                    this.WriteNumericConversionWithOverflow<int>(this.Int32TypeReference, true, 0x7fffffff, true);
                    return;

                case Code.Conv_Ovf_I8_Un:
                    this.WriteNumericConversionWithOverflow<long>(this.Int64TypeReference, true, 0x7fffffffffffffffL, true);
                    return;

                case Code.Conv_Ovf_U1_Un:
                    this.WriteNumericConversionWithOverflow<byte>(this.ByteTypeReference, true, 0xff, true);
                    return;

                case Code.Conv_Ovf_U2_Un:
                    this.WriteNumericConversionWithOverflow<ushort>(this.UInt16TypeReference, true, 0xffff, true);
                    return;

                case Code.Conv_Ovf_U4_Un:
                    this.WriteNumericConversionWithOverflow<uint>(this.UInt32TypeReference, true, uint.MaxValue, true);
                    return;

                case Code.Conv_Ovf_U8_Un:
                    this.WriteNumericConversionWithOverflow<ulong>(this.UInt64TypeReference, true, ulong.MaxValue, true);
                    return;

                case Code.Conv_Ovf_I_Un:
                    this.ConvertToNaturalIntWithOverflow<string>(this.NativeIntTypeReference, true, "INTPTR_MAX");
                    return;

                case Code.Conv_Ovf_U_Un:
                    this.ConvertToNaturalIntWithOverflow<string>(this.NativeUIntTypeReference, true, "UINTPTR_MAX");
                    return;

                case Code.Box:
                {
                    TypeReference typeReference = this._typeResolver.Resolve((TypeReference) ins.Operand);
                    this._writer.AddIncludeForTypeDefinition(typeReference);
                    if (typeReference.IsValueType())
                    {
                        StackInfo originalValue = this._valueStack.Pop();
                        if (!this.CanApplyValueTypeBoxBranchOptimizationToInstruction(ins, block))
                        {
                            bool flag = (typeReference.MetadataType == MetadataType.IntPtr) || (typeReference.MetadataType == MetadataType.UIntPtr);
                            bool flag2 = originalValue.Type.IsSameType(this.NativeIntTypeReference) || originalValue.Type.IsSameType(this.NativeUIntTypeReference);
                            if (flag && flag2)
                            {
                                typeReference = originalValue.Type;
                            }
                            if (((originalValue.Type.MetadataType == MetadataType.IntPtr) && (typeReference.MetadataType == MetadataType.UIntPtr)) || ((originalValue.Type.MetadataType == MetadataType.UIntPtr) && (typeReference.MetadataType == MetadataType.IntPtr)))
                            {
                                this.StoreLocalAndPush(this.ObjectTypeReference, $"Box({this._runtimeMetadataAccess.TypeInfoFor((TypeReference) ins.Operand)}, &{originalValue.Expression})");
                            }
                            else
                            {
                                Local local2 = this.NewTemp(typeReference);
                                object[] objArray3 = new object[] { local2.IdentifierExpression, CastTypeIfNeeded(originalValue, typeReference) };
                                this._writer.WriteLine("{0} = {1};", objArray3);
                                this.StoreLocalAndPush(this.ObjectTypeReference, $"Box({this._runtimeMetadataAccess.TypeInfoFor((TypeReference) ins.Operand)}, &{local2.Expression})");
                            }
                            return;
                        }
                        Instruction next = ins.Next;
                        if ((next.OpCode.Code == Code.Brtrue) || (next.OpCode.Code == Code.Brtrue_S))
                        {
                            this.WriteGlobalVariableAssignmentForRightBranch(block, (Instruction) next.Operand);
                            this.WriteJump((Instruction) next.Operand);
                        }
                        this.WriteGlobalVariableAssignmentForLeftBranch(block, (Instruction) next.Operand);
                        ins = next;
                    }
                    return;
                }
                case Code.Newarr:
                {
                    StackInfo info4 = this._valueStack.Pop();
                    ArrayType type3 = new ArrayType(this._typeResolver.Resolve((TypeReference) ins.Operand));
                    this._writer.AddIncludeForTypeDefinition(type3);
                    string str7 = $"(uint32_t){info4.Expression}";
                    this.PushExpression(type3, Emit.Cast(type3, Emit.Call("SZArrayNew", this._runtimeMetadataAccess.ArrayInfo((TypeReference) ins.Operand), str7)));
                    return;
                }
                case Code.Ldlen:
                {
                    StackInfo stackInfo = this._valueStack.Pop();
                    this._nullCheckSupport.WriteNullCheckIfNeeded(stackInfo);
                    this.PushExpression(this.UInt32TypeReference, $"(({Naming.ForVariable(TypeProvider.SystemArray)}){stackInfo})->max_length");
                    return;
                }
                case Code.Ldelema:
                {
                    StackInfo info6 = this._valueStack.Pop();
                    StackInfo info7 = this._valueStack.Pop();
                    ByReferenceType type4 = new ByReferenceType(this._typeResolver.Resolve((TypeReference) ins.Operand));
                    this._nullCheckSupport.WriteNullCheckIfNeeded(info7);
                    this.PushExpression(type4, this.EmitArrayLoadElementAddress(info7, info6.Expression, info6.Type));
                    return;
                }
                case Code.Ldelem_I1:
                    this.LoadElemAndPop(this.SByteTypeReference);
                    return;

                case Code.Ldelem_U1:
                    this.LoadElemAndPop(this.ByteTypeReference);
                    return;

                case Code.Ldelem_I2:
                    this.LoadElemAndPop(this.Int16TypeReference);
                    return;

                case Code.Ldelem_U2:
                    this.LoadElemAndPop(this.UInt16TypeReference);
                    return;

                case Code.Ldelem_I4:
                    this.LoadElemAndPop(this.Int32TypeReference);
                    return;

                case Code.Ldelem_U4:
                    this.LoadElemAndPop(this.UInt32TypeReference);
                    return;

                case Code.Ldelem_I8:
                    this.LoadElemAndPop(this.Int64TypeReference);
                    return;

                case Code.Ldelem_I:
                    this.LoadElemAndPop(this.IntPtrTypeReference);
                    return;

                case Code.Ldelem_R4:
                    this.LoadElemAndPop(this.SingleTypeReference);
                    return;

                case Code.Ldelem_R8:
                    this.LoadElemAndPop(this.DoubleTypeReference);
                    return;

                case Code.Ldelem_Ref:
                {
                    StackInfo info8 = this._valueStack.Pop();
                    StackInfo array = this._valueStack.Pop();
                    this.LoadElem(array, ArrayUtilities.ArrayElementTypeOf(array.Type), info8);
                    return;
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
                    StackInfo info10 = this._valueStack.Pop();
                    StackInfo info11 = this._valueStack.Pop();
                    StackInfo info12 = this._valueStack.Pop();
                    this.StoreElement(info12, info11, info10, false);
                    return;
                }
                case Code.Stelem_Ref:
                {
                    StackInfo info13 = this._valueStack.Pop();
                    StackInfo info14 = this._valueStack.Pop();
                    StackInfo info15 = this._valueStack.Pop();
                    this.StoreElement(info15, info14, info13, true);
                    return;
                }
                case Code.Ldelem_Any:
                    this.LoadElemAndPop(this._typeResolver.Resolve((TypeReference) ins.Operand));
                    return;

                case Code.Unbox_Any:
                {
                    StackInfo info16 = this._valueStack.Pop();
                    TypeReference reference12 = this._typeResolver.Resolve((TypeReference) ins.Operand);
                    this._writer.AddIncludeForTypeDefinition(reference12);
                    if (!reference12.IsValueType())
                    {
                        this.WriteCastclass((TypeReference) ins.Operand, info16, ins);
                        return;
                    }
                    ByReferenceType type5 = new ByReferenceType(reference12);
                    this.PushExpression(type5, Emit.Cast(new PointerType(reference12), this.Unbox((TypeReference) ins.Operand, info16.Expression)));
                    StackInfo info17 = this._valueStack.Pop();
                    this.PushExpression(reference12, Emit.Dereference(Emit.Cast(type5, info17.Expression)));
                    return;
                }
                case Code.Conv_Ovf_I1:
                    this.WriteNumericConversionWithOverflow<sbyte>(this.SByteTypeReference, false, 0x7f, true);
                    return;

                case Code.Conv_Ovf_U1:
                    this.WriteNumericConversionWithOverflow<byte>(this.ByteTypeReference, false, 0xff, true);
                    return;

                case Code.Conv_Ovf_I2:
                    this.WriteNumericConversionWithOverflow<short>(this.Int16TypeReference, false, 0x7fff, true);
                    return;

                case Code.Conv_Ovf_U2:
                    this.WriteNumericConversionWithOverflow<ushort>(this.UInt16TypeReference, false, 0xffff, true);
                    return;

                case Code.Conv_Ovf_I4:
                    this.WriteNumericConversionWithOverflow<int>(this.Int32TypeReference, false, 0x7fffffff, true);
                    return;

                case Code.Conv_Ovf_U4:
                    this.WriteNumericConversionWithOverflow<uint>(this.UInt32TypeReference, false, uint.MaxValue, true);
                    return;

                case Code.Conv_Ovf_I8:
                    this.WriteNumericConversionWithOverflow<string>(this.Int64TypeReference, false, "std::numeric_limits<int64_t>::max()", false);
                    return;

                case Code.Conv_Ovf_U8:
                    this.WriteNumericConversionWithOverflow<string>(this.UInt64TypeReference, true, "std::numeric_limits<uint64_t>::max()", false);
                    return;

                case Code.Refanyval:
                    throw new NotImplementedException();

                case Code.Ckfinite:
                    throw new NotImplementedException();

                case Code.Mkrefany:
                    throw new NotImplementedException();

                case Code.Ldtoken:
                    this._valueStack.Push(this.FormatLoadTokenFor(ins));
                    return;

                case Code.Conv_U2:
                    this.WriteNumericConversion(this.UInt16TypeReference, this.Int32TypeReference);
                    return;

                case Code.Conv_U1:
                    this.WriteNumericConversion(this.ByteTypeReference, this.Int32TypeReference);
                    return;

                case Code.Conv_I:
                    this.ConvertToNaturalInt(this.NativeIntTypeReference);
                    return;

                case Code.Conv_Ovf_I:
                    this.ConvertToNaturalIntWithOverflow<string>(this.NativeIntTypeReference, false, "INTPTR_MAX");
                    return;

                case Code.Conv_Ovf_U:
                    this.ConvertToNaturalIntWithOverflow<string>(this.NativeUIntTypeReference, false, "UINTPTR_MAX");
                    return;

                case Code.Add_Ovf:
                    this.WriteAdd(OverflowCheck.Signed);
                    return;

                case Code.Add_Ovf_Un:
                    this.WriteAdd(OverflowCheck.Unsigned);
                    return;

                case Code.Mul_Ovf:
                    this.WriteMul(OverflowCheck.Signed);
                    return;

                case Code.Mul_Ovf_Un:
                    this.WriteMul(OverflowCheck.Unsigned);
                    return;

                case Code.Sub_Ovf:
                    this.WriteSub(OverflowCheck.Signed);
                    return;

                case Code.Sub_Ovf_Un:
                    this.WriteSub(OverflowCheck.Unsigned);
                    return;

                case Code.Endfinally:
                {
                    ExceptionSupport.Node enclosingFinallyOrFaultNode = node.GetEnclosingFinallyOrFaultNode();
                    object[] objArray4 = new object[] { enclosingFinallyOrFaultNode.Start.Offset };
                    this._writer.WriteLine("IL2CPP_END_FINALLY({0})", objArray4);
                    return;
                }
                case Code.Leave:
                case Code.Leave_S:
                    if (this.ShouldStripLeaveInstruction(block, ins))
                    {
                        object[] objArray5 = new object[] { ins };
                        this._writer.WriteLine("; // {0}", objArray5);
                        return;
                    }
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
                    return;

                case Code.Stind_I:
                    this.StoreIndirect(this.NativeIntTypeReference);
                    return;

                case Code.Conv_U:
                    this.ConvertToNaturalInt(TypeProvider.NativeUIntTypeReference);
                    return;

                case Code.Arglist:
                    this._writer.WriteLine("#pragma message(FIXME \"arglist is not supported\")");
                    this._writer.WriteLine("assert(false && \"arglist is not supported\");");
                    this._valueStack.Push(new StackInfo("LoadArgList()", this.NativeIntTypeReference));
                    return;

                case Code.Ceq:
                    this.GenerateConditional("==", Signedness.Signed, false);
                    return;

                case Code.Cgt:
                    this.GenerateConditional(">", Signedness.Signed, false);
                    return;

                case Code.Cgt_Un:
                    this.GenerateConditional("<=", Signedness.Unsigned, true);
                    return;

                case Code.Clt:
                    this.GenerateConditional("<", Signedness.Signed, false);
                    return;

                case Code.Clt_Un:
                    this.GenerateConditional(">=", Signedness.Unsigned, true);
                    return;

                case Code.Ldftn:
                    this.PushCallToLoadFunction((MethodReference) ins.Operand);
                    return;

                case Code.Ldvirtftn:
                    this.LoadVirtualFunction(ins);
                    return;

                case Code.Ldarg:
                {
                    ParameterReference reference13 = (ParameterReference) ins.Operand;
                    int num4 = reference13.Index;
                    if (this._methodDefinition.HasThis)
                    {
                        num4++;
                    }
                    this.WriteLdarg(num4, block, ins);
                    return;
                }
                case Code.Ldarga:
                    this.LoadArgumentAddress((ParameterReference) ins.Operand);
                    return;

                case Code.Starg:
                    this.StoreArg(ins);
                    return;

                case Code.Ldloca:
                    this.LoadLocalAddress((VariableReference) ins.Operand);
                    return;

                case Code.Localloc:
                {
                    StackInfo info18 = this._valueStack.Pop();
                    PointerType variableType = new PointerType(this.SByteTypeReference);
                    string str8 = this.NewTempName();
                    this._writer.WriteLine(string.Format("{0} {1} = ({0}) alloca({2});", Naming.ForVariable(variableType), str8, info18));
                    if (this._methodDefinition.Body.InitLocals)
                    {
                        object[] objArray6 = new object[] { str8, info18 };
                        this._writer.WriteLine("memset({0},0,{1});", objArray6);
                    }
                    this.PushExpression(variableType, str8);
                    return;
                }
                case Code.Endfilter:
                {
                    StackInfo info19 = this._valueStack.Pop();
                    this._writer.WriteLine($"{"__filter_local"} = ({info19}) ? true : false;");
                    return;
                }
                case Code.Unaligned:
                    throw new NotImplementedException();

                case Code.Volatile:
                    this.AddVolatileStackEntry();
                    return;

                case Code.Tail:
                    StatsService.RecordTailCall(this._methodDefinition);
                    return;

                case Code.Initobj:
                {
                    StackInfo info20 = this._valueStack.Pop();
                    ByReferenceType type8 = (ByReferenceType) this._typeResolver.Resolve(info20.Type);
                    TypeReference elementType = type8.ElementType;
                    this._writer.AddIncludeForTypeDefinition(this._typeResolver.Resolve(elementType));
                    object[] objArray7 = new object[] { this._runtimeMetadataAccess.TypeInfoFor(elementType), info20.Expression };
                    this._writer.WriteLine("Initobj ({0}, {1});", objArray7);
                    return;
                }
                case Code.Constrained:
                    this._constrainedCallThisType = (TypeReference) ins.Operand;
                    this._writer.AddIncludeForTypeDefinition(this._typeResolver.Resolve(this._constrainedCallThisType));
                    return;

                case Code.Cpblk:
                    throw new NotImplementedException();

                case Code.Initblk:
                    throw new NotImplementedException();

                case Code.No:
                    throw new NotImplementedException();

                case Code.Rethrow:
                    if (node.Type == ExceptionSupport.NodeType.Finally)
                    {
                        object[] objArray8 = new object[] { "__last_unhandled_exception", "__exception_local" };
                        this._writer.WriteLine("{0} = {1};", objArray8);
                        this.WriteJump(node.Handler.HandlerStart);
                    }
                    else
                    {
                        this._writer.WriteStatement(Emit.RaiseManagedException("__exception_local"));
                    }
                    return;

                case Code.Sizeof:
                    this.StoreLocalAndPush(this.UInt32TypeReference, this._runtimeMetadataAccess.SizeOf((TypeReference) ins.Operand));
                    return;

                case Code.Refanytype:
                {
                    StackInfo info21 = this._valueStack.Pop();
                    if ((info21.Type.FullName != "System.TypedReference") && (info21.Type.Resolve().Module.Name == "mscorlib"))
                    {
                        throw new InvalidOperationException();
                    }
                    FieldDefinition field = info21.Type.Resolve().Fields.Single<FieldDefinition>(f => Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(f.FieldType, this.RuntimeTypeHandleTypeReference, TypeComparisonMode.Exact));
                    string expression = $"{info21.Expression}.{Naming.ForFieldGetter(field)}()";
                    this._valueStack.Push(new StackInfo(expression, this.RuntimeTypeHandleTypeReference));
                    return;
                }
                case Code.Readonly:
                    return;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            this._valueStack.Push(new StackInfo(local));
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
            MethodDefinition definition = methodReference.Resolve();
            if ((definition != null) && definition.IsVirtual)
            {
                this._writer.AddIncludeForTypeDefinition(this._typeResolver.Resolve(methodReference.DeclaringType));
            }
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = this._typeResolver;
            MethodReference reference = methodReference;
            if (methodReference is GenericInstanceMethod)
            {
                reference = this._typeResolver.Resolve(methodReference);
                resolver = this._typeResolver.Nested(reference as GenericInstanceMethod);
            }
            this._writer.AddIncludeForTypeDefinition(resolver.Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveReturnTypeIfNeeded(reference)));
            if (reference.HasThis)
            {
                this._writer.AddIncludeForTypeDefinition(resolver.Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveThisTypeIfNeeded(reference)));
            }
            foreach (ParameterDefinition definition2 in reference.Parameters)
            {
                this._writer.AddIncludeForTypeDefinition(resolver.Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveParameterTypeIfNeeded(reference, definition2)));
            }
        }

        private void PushCallToLoadFunction(MethodReference merthodReference)
        {
            this._writer.AddIncludeForTypeDefinition(this._typeResolver.Resolve(merthodReference.DeclaringType));
            this.StoreLocalIntPtrAndPush($"(void*){this._runtimeMetadataAccess.MethodInfo(merthodReference)}");
        }

        private void PushCallToLoadVirtualFunction(MethodReference methodReference, MethodDefinition methodDefinition, string targetExpression)
        {
            string str;
            bool flag = methodReference.DeclaringType.IsInterface();
            if (methodReference.IsGenericInstance)
            {
                str = !flag ? Emit.Call("il2cpp_codegen_get_generic_virtual_method", this._runtimeMetadataAccess.MethodInfo(methodReference), targetExpression) : Emit.Call("il2cpp_codegen_get_generic_interface_method", this._runtimeMetadataAccess.MethodInfo(methodReference), targetExpression);
            }
            else if (flag)
            {
                if (CodeGenOptions.MonoRuntime)
                {
                    str = Emit.Call("GetInterfaceMethodInfo", targetExpression, "(MonoMethod*)" + this._runtimeMetadataAccess.MethodInfo(methodReference), this._runtimeMetadataAccess.TypeInfoFor(methodReference.DeclaringType));
                }
                else
                {
                    str = Emit.Call("GetInterfaceMethodInfo", targetExpression, this._vTableBuilder.IndexFor(methodDefinition).ToString(), this._runtimeMetadataAccess.TypeInfoFor(methodReference.DeclaringType));
                }
            }
            else if (CodeGenOptions.MonoRuntime)
            {
                str = Emit.Call("GetVirtualMethodInfo", targetExpression, Emit.MonoMethodMetadataGet(methodReference));
            }
            else
            {
                str = Emit.Call("GetVirtualMethodInfo", targetExpression, this._vTableBuilder.IndexFor(methodDefinition).ToString());
            }
            this._writer.AddIncludeForTypeDefinition(this._typeResolver.Resolve(methodReference.DeclaringType));
            this.StoreLocalIntPtrAndPush($"(void*){str}");
        }

        private void PushExpression(TypeReference typeReference, string expression)
        {
            this._valueStack.Push(new StackInfo($"({expression})", typeReference));
        }

        private void PushLoadIndirectExpression(TypeReference expressionType, TypeReference castType, string expression)
        {
            this.PushExpression(expressionType, GetLoadIndirectExpression(castType, expression));
        }

        private void RecursivelyAddLabelsForExceptionNodes(ExceptionSupport.Node node)
        {
            if (((node.Type == ExceptionSupport.NodeType.Try) || (node.Type == ExceptionSupport.NodeType.Catch)) || ((node.Type == ExceptionSupport.NodeType.Finally) || (node.Type == ExceptionSupport.NodeType.Fault)))
            {
                if (node.Block != null)
                {
                    this._referencedLabels.Add(node.Block.First);
                }
                foreach (ExceptionSupport.Node node2 in node.Children)
                {
                    if (node2.Block != null)
                    {
                        this._referencedLabels.Add(node2.Block.First);
                    }
                    this.RecursivelyAddLabelsForExceptionNodes(node2);
                }
            }
        }

        private static bool Requires64BitOverflowCheck(MetadataType metadataType) => 
            ((metadataType == MetadataType.UInt64) || (metadataType == MetadataType.Int64));

        private static bool Requires64BitOverflowCheck(MetadataType leftStackType, MetadataType rightStackType) => 
            (Requires64BitOverflowCheck(leftStackType) || Requires64BitOverflowCheck(rightStackType));

        private bool RequiresContravariantCastToStore(TypeReference destinationVariable, TypeReference sourceVariableType)
        {
            if (!destinationVariable.IsGenericInstance || !sourceVariableType.IsGenericInstance)
            {
                return false;
            }
            if (Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(destinationVariable, sourceVariableType, TypeComparisonMode.Exact))
            {
                return false;
            }
            return true;
        }

        private bool RequiresPointerOverflowCheck(TypeReference type) => 
            (type.IsSameType(TypeProvider.NativeIntTypeReference) || type.IsSameType(TypeProvider.NativeUIntTypeReference));

        private bool RequiresPointerOverflowCheck(TypeReference leftStackType, TypeReference rightStackType) => 
            (this.RequiresPointerOverflowCheck(leftStackType) || this.RequiresPointerOverflowCheck(rightStackType));

        private IEnumerable<KeyValuePair<string, TypeReference>> ResolveLocalVariableTypes() => 
            (from v in this._methodDefinition.Body.Variables select new KeyValuePair<string, TypeReference>(Naming.ForVariableName(v), this._typeResolver.Resolve(v.VariableType)));

        private void SetupFallthroughVariables(InstructionBlock block)
        {
            GlobalVariable[] globalVariables = this._stackAnalysis.InputVariablesFor(block.Successors.Single<InstructionBlock>());
            this.WriteAssignGlobalVariables(globalVariables);
            this._valueStack.Clear();
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = v => v.Index;
            }
            foreach (GlobalVariable variable in globalVariables.OrderBy<GlobalVariable, int>(<>f__am$cache3).Reverse<GlobalVariable>())
            {
                this._valueStack.Push(new StackInfo(variable.VariableName, this._typeResolver.Resolve(variable.Type)));
            }
        }

        private bool ShouldStripLeaveInstruction(InstructionBlock block, Instruction ins) => 
            (!this._labeler.NeedsLabel(ins) && (((block.First == block.Last) && (block.First.Previous != null)) && (block.First.Previous.OpCode.Code == Code.Leave)));

        private void StaticFieldAccess(Instruction ins)
        {
            TypeReference declaringType;
            FieldReference operand = (FieldReference) ins.Operand;
            if (operand.Resolve().IsLiteral)
            {
                throw new Exception("literal values should always be embedded rather than accessed via the field itself");
            }
            this.WriteCallToClassAndInitializerAndStaticConstructorIfNeeded(operand.DeclaringType, this._methodDefinition, this._runtimeMetadataAccess);
            TypeReference reference2 = this._typeResolver.ResolveFieldType(operand);
            string str = TypeStaticsExpressionFor(operand, this._typeResolver, this._runtimeMetadataAccess);
            if (operand.DeclaringType.IsGenericInstance)
            {
                declaringType = operand.DeclaringType;
            }
            else
            {
                declaringType = this._typeResolver.Resolve(operand.DeclaringType);
            }
            if (ins.OpCode.Code == Code.Stsfld)
            {
                StackInfo right = this._valueStack.Pop();
                this.EmitMemoryBarrierIfNecessary(null);
                if (CodeGenOptions.MonoRuntime)
                {
                    Local local = this.NewTemp(reference2);
                    object[] args = new object[] { local.IdentifierExpression, WriteExpressionAndCastIfNeeded(reference2, right, SharingType.NonShared) };
                    this._writer.WriteLine("{0} = {1};", args);
                    this._writer.WriteLine($"il2cpp_codegen_mono_set_static_field({this._runtimeMetadataAccess.StaticData(declaringType)}, {this._runtimeMetadataAccess.FieldInfo(operand)}, {!local.Type.IsValueType() ? string.Empty : "&"}{local.Expression});");
                }
                else
                {
                    this._writer.WriteLine(Statement.Expression(Emit.Call($"{str}{Naming.ForFieldSetter(operand)}", WriteExpressionAndCastIfNeeded(reference2, right, SharingType.NonShared))));
                }
            }
            else
            {
                if (ins.OpCode.Code == Code.Ldsflda)
                {
                    string str2;
                    ByReferenceType variableType = new ByReferenceType(reference2);
                    if (CodeGenOptions.MonoRuntime)
                    {
                        str2 = Emit.Cast(Naming.ForVariable(variableType), $"il2cpp_codegen_mono_get_static_field_address({this._runtimeMetadataAccess.StaticData(declaringType)}, {this._runtimeMetadataAccess.FieldInfo(operand)})");
                    }
                    else
                    {
                        str2 = Emit.Call($"{str}{Naming.ForFieldAddressGetter(operand)}");
                    }
                    this.PushExpression(variableType, str2);
                }
                else
                {
                    Local local2 = this.NewTemp(reference2);
                    if (CodeGenOptions.MonoRuntime)
                    {
                        object[] objArray3 = new object[] { local2.IdentifierExpression };
                        this._writer.WriteLine("{0};", objArray3);
                        this._writer.WriteLine($"il2cpp_codegen_mono_get_static_field({this._runtimeMetadataAccess.StaticData(declaringType)}, {this._runtimeMetadataAccess.FieldInfo(operand)}, &{local2.Expression});");
                    }
                    else
                    {
                        object[] objArray4 = new object[] { Emit.Assign(local2.IdentifierExpression, Emit.Call($"{str}{Naming.ForFieldGetter(operand)}")) };
                        this._writer.WriteLine("{0};", objArray4);
                    }
                    this._valueStack.Push(new StackInfo(local2));
                }
                this.EmitMemoryBarrierIfNecessary(null);
            }
        }

        private void StoreArg(Instruction ins)
        {
            StackInfo right = this._valueStack.Pop();
            ParameterReference operand = (ParameterReference) ins.Operand;
            if (operand.Index == -1)
            {
                this.WriteAssignment(Naming.ThisParameterName, this._typeResolver.ResolveParameterType(this._methodReference, operand), right);
            }
            else
            {
                this.WriteAssignment(Naming.ForParameterName(operand), this._typeResolver.ResolveParameterType(this._methodReference, operand), right);
            }
        }

        private void StoreElement(StackInfo array, StackInfo index, StackInfo value, bool emitElementTypeCheck)
        {
            TypeReference type = ArrayUtilities.ArrayElementTypeOf(array.Type);
            this._nullCheckSupport.WriteNullCheckIfNeeded(array);
            if (emitElementTypeCheck)
            {
                this._writer.WriteLine(Emit.ArrayElementTypeCheck(array.Expression, value.Expression));
            }
            string expression = index.Expression;
            if (index.Type.IsIntegralPointerType())
            {
                expression = this.ArrayIndexerForIntPtr(index.Expression, index.Type);
            }
            this._arrayBoundsCheckSupport.RecordArrayBoundsCheckEmitted();
            object[] args = new object[] { Emit.StoreArrayElement(array.Expression, expression, Emit.Cast(type, value.Expression), this._arrayBoundsCheckSupport.ShouldEmitBoundsChecksForMethod()) };
            this._writer.WriteLine("{0};", args);
        }

        private void StoreField(Instruction ins)
        {
            StackInfo right = this._valueStack.Pop();
            StackInfo stackInfo = this._valueStack.Pop();
            FieldReference operand = (FieldReference) ins.Operand;
            if (stackInfo.Expression != Naming.ThisParameterName)
            {
                this._nullCheckSupport.WriteNullCheckIfNeeded(stackInfo);
            }
            this.EmitMemoryBarrierIfNecessary(operand);
            object[] args = new object[] { CastReferenceTypeOrNativeIntIfNeeded(stackInfo, this._typeResolver.Resolve(operand.DeclaringType)), Naming.ForFieldSetter(operand), WriteExpressionAndCastIfNeeded(this._typeResolver.ResolveFieldType(operand), right, SharingType.NonShared) };
            this._writer.WriteLine("{0}->{1}({2});", args);
        }

        private void StoreIndirect(TypeReference type)
        {
            string expression;
            string str2;
            StackInfo info = this._valueStack.Pop();
            StackInfo info2 = this._valueStack.Pop();
            this.EmitMemoryBarrierIfNecessary(null);
            if (!Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(info.Type, this.IntPtrTypeReference, TypeComparisonMode.Exact) && !Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(info.Type, this.UIntPtrTypeReference, TypeComparisonMode.Exact))
            {
                PointerType variableType = new PointerType(type);
                expression = $"({Naming.ForVariable(variableType)})({info2.Expression})";
                str2 = $"({Naming.ForVariable(type)}){info.Expression}";
            }
            else
            {
                expression = info2.Expression;
                str2 = info.Expression;
            }
            object[] args = new object[] { expression, str2 };
            this._writer.WriteLine("*({0}) = {1};", args);
            this._writer.WriteWriteBarrierIfNeeded(type, expression, str2);
        }

        private void StoreLocalAndPush(TypeReference type, string stringValue)
        {
            Local local = this.NewTemp(type);
            object[] args = new object[] { local.IdentifierExpression, stringValue };
            this._writer.WriteLine("{0} = {1};", args);
            this._valueStack.Push(new StackInfo(local));
        }

        private void StoreLocalIntPtrAndPush(string stringValue)
        {
            this._valueStack.Push(new StackInfo(this.EmitLocalIntPtrWithValue(stringValue)));
        }

        private TypeReference TypeForComparison(Signedness signedness, TypeReference type)
        {
            TypeReference a = StackTypeConverter.StackTypeFor(type);
            if (a.IsSameType(TypeProvider.NativeIntTypeReference))
            {
                return ((signedness != Signedness.Signed) ? this.NativeUIntTypeReference : this.NativeIntTypeReference);
            }
            MetadataType metadataType = a.MetadataType;
            switch (metadataType)
            {
                case MetadataType.Int32:
                    return ((signedness != Signedness.Signed) ? this.UInt32TypeReference : this.Int32TypeReference);

                case MetadataType.Int64:
                    return ((signedness != Signedness.Signed) ? this.UInt64TypeReference : this.Int64TypeReference);

                case MetadataType.Pointer:
                case MetadataType.ByReference:
                    return ((signedness != Signedness.Signed) ? this.NativeUIntTypeReference : this.NativeIntTypeReference);
            }
            if ((metadataType != MetadataType.IntPtr) && (metadataType != MetadataType.UIntPtr))
            {
                return type;
            }
            return ((signedness != Signedness.Signed) ? this.UIntPtrTypeReference : this.IntPtrTypeReference);
        }

        internal static string TypeStaticsExpressionFor(FieldReference fieldReference, Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver, IRuntimeMetadataAccess runtimeMetadataAccess)
        {
            TypeReference type = typeResolver.Resolve(fieldReference.DeclaringType);
            string str = runtimeMetadataAccess.StaticData(fieldReference.DeclaringType);
            if (fieldReference.IsThreadStatic())
            {
                return $"(({Naming.ForThreadFieldsStruct(type)}*)il2cpp_codegen_get_thread_static_data({str}))->";
            }
            return $"(({Naming.ForStaticFieldsStruct(type)}*)il2cpp_codegen_static_fields_for({str}))->";
        }

        private void Unbox(Instruction ins)
        {
            StackInfo info = this._valueStack.Pop();
            TypeReference typeReference = this._typeResolver.Resolve((TypeReference) ins.Operand);
            this._writer.AddIncludeForTypeDefinition(typeReference);
            this.PushExpression(new ByReferenceType(typeReference), Emit.Cast(new PointerType(typeReference), this.Unbox(typeReference, info.Expression)));
        }

        private string Unbox(TypeReference type, string boxedExpression)
        {
            TypeReference reference = this._typeResolver.Resolve(type);
            if (reference.IsNullable())
            {
                TypeReference reference2 = ((GenericInstanceType) reference).GenericArguments[0];
                string str = this._runtimeMetadataAccess.TypeInfoFor(reference2);
                string str2 = this.NewTempName();
                this._writer.WriteLine($"void* {str2} = alloca(sizeof({Naming.ForVariable(reference)}));");
                this._writer.WriteLine($"UnBoxNullable({boxedExpression}, {str}, {str2});");
                return str2;
            }
            string str4 = this._runtimeMetadataAccess.TypeInfoFor(type);
            return string.Format($"UnBox({boxedExpression}, {str4})", new object[0]);
        }

        private static string VirtualCallFor(MethodReference method, MethodReference unresolvedMethod, IEnumerable<string> args, Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver, IRuntimeMetadataAccess runtimeMetadataAccess, VTableBuilder vTableBuilder)
        {
            bool isInterface = method.DeclaringType.Resolve().IsInterface;
            List<string> arguments = new List<string>();
            if (CodeGenOptions.MonoRuntime)
            {
                arguments.Add("(MonoMethod*)" + ((!method.IsGenericInstance && !isInterface) ? Emit.MonoMethodMetadataGet(method) : runtimeMetadataAccess.MethodInfo(unresolvedMethod)));
            }
            else
            {
                arguments.Add(!method.IsGenericInstance ? string.Concat(new object[] { vTableBuilder.IndexFor(method.Resolve()), " /* ", method.FullName, " */" }) : runtimeMetadataAccess.MethodInfo(unresolvedMethod));
            }
            if (isInterface && !method.IsGenericInstance)
            {
                arguments.Add(runtimeMetadataAccess.TypeInfoFor(unresolvedMethod.DeclaringType));
            }
            arguments.AddRange(args);
            return Emit.Call(Emit.VirtualCallInvokeMethod(method, typeResolver), arguments);
        }

        private void WriteAdd(OverflowCheck check)
        {
            StackInfo right = this._valueStack.Pop();
            StackInfo left = this._valueStack.Pop();
            if (check != OverflowCheck.None)
            {
                TypeReference leftStackType = StackTypeConverter.StackTypeFor(left.Type);
                TypeReference rightStackType = StackTypeConverter.StackTypeFor(right.Type);
                if (this.RequiresPointerOverflowCheck(leftStackType, rightStackType))
                {
                    this.WritePointerOverflowCheckUsing64Bits("+", check, left, right);
                }
                else if (Requires64BitOverflowCheck(leftStackType.MetadataType, rightStackType.MetadataType))
                {
                    if (check == OverflowCheck.Signed)
                    {
                        object[] args = new object[] { left.Expression, right.Expression };
                        this._writer.WriteLine("if (((int64_t){1} >= 0 && (int64_t){0} > kIl2CppInt64Max - (int64_t){1}) || ((int64_t){1} < 0 && (int64_t){0} < (int64_t)kIl2CppInt64Min - (int64_t){1}))", args);
                        object[] objArray2 = new object[] { Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()") };
                        this._writer.WriteLine("\t{0};", objArray2);
                    }
                    else
                    {
                        object[] objArray3 = new object[] { left.Expression, right.Expression };
                        this._writer.WriteLine("if ((uint64_t){0} > kIl2CppUInt64Max - (uint64_t){1})", objArray3);
                        object[] objArray4 = new object[] { Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()") };
                        this._writer.WriteLine("\t{0};", objArray4);
                    }
                }
                else
                {
                    this.WriteNarrowOverflowCheckUsing64Bits("+", check, left.Expression, right.Expression);
                }
            }
            TypeReference leftType = Naming.RemoveModifiers(left.Type);
            TypeReference rightType = Naming.RemoveModifiers(right.Type);
            TypeReference resultType = StackAnalysisUtils.ResultTypeForAdd(leftType, rightType, TypeProvider);
            this.WriteBinaryOperation("+", left, right, resultType);
        }

        private void WriteAssignGlobalVariables(GlobalVariable[] globalVariables)
        {
            <WriteAssignGlobalVariables>c__AnonStorey7 storey = new <WriteAssignGlobalVariables>c__AnonStorey7();
            if (globalVariables.Length != this._valueStack.Count)
            {
                throw new ArgumentException("Invalid global variables count", "globalVariables");
            }
            storey.stackIndex = 0;
            foreach (StackInfo info in this._valueStack)
            {
                GlobalVariable variable = globalVariables.Single<GlobalVariable>(new Func<GlobalVariable, bool>(storey.<>m__0));
                if (info.Type.FullName != variable.Type.FullName)
                {
                    object[] args = new object[] { variable.VariableName, Naming.ForVariable(this._typeResolver.Resolve(variable.Type)), (info.Type.MetadataType != MetadataType.Pointer) ? "" : "(intptr_t)", info.Expression };
                    this._writer.WriteLine("{0} = (({1}){2}({3}));", args);
                }
                else
                {
                    object[] objArray2 = new object[] { variable.VariableName, info.Expression };
                    this._writer.WriteLine("{0} = {1};", objArray2);
                }
                storey.stackIndex++;
            }
        }

        private void WriteAssignment(string leftName, TypeReference leftType, StackInfo right)
        {
            this._writer.WriteStatement(GetAssignment(leftName, leftType, right, this._sharingType));
        }

        private void WriteBinaryOperation(string op, StackInfo left, StackInfo right, TypeReference resultType)
        {
            TypeReference type = Naming.RemoveModifiers(left.Type);
            TypeReference reference2 = Naming.RemoveModifiers(right.Type);
            string rcast = this.CastExpressionForBinaryOperator(right);
            string lcast = this.CastExpressionForBinaryOperator(left);
            string str3 = this.ExpressionForBinaryOperation(type, left.Expression);
            string str4 = this.ExpressionForBinaryOperation(reference2, right.Expression);
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
            this.WriteBinaryOperation(resultType, lcast, str3, op, rcast, str4);
        }

        private void WriteBinaryOperation(TypeReference destType, string lcast, string left, string op, string rcast, string right)
        {
            this.PushExpression(destType, $"({Naming.ForVariable(destType)})({lcast}{left}{op}{rcast}{right})");
        }

        private void WriteBinaryOperationUsingLargestOperandTypeAsResultType(string op)
        {
            StackInfo right = this._valueStack.Pop();
            StackInfo left = this._valueStack.Pop();
            this.WriteBinaryOperation(op, left, right, StackAnalysisUtils.CorrectLargestTypeFor(left.Type, right.Type, TypeProvider));
        }

        private void WriteBinaryOperationUsingLeftOperandTypeAsResultType(string op)
        {
            StackInfo right = this._valueStack.Pop();
            StackInfo left = this._valueStack.Pop();
            this.WriteBinaryOperation(op, left, right, left.Type);
        }

        private void WriteCallToClassAndInitializerAndStaticConstructorIfNeeded(TypeReference type, MethodDefinition invokingMethod, IRuntimeMetadataAccess runtimeMetadataAccess)
        {
            if (type.HasStaticConstructor() && !this._classesAlreadyInitializedInBlock.Contains(type))
            {
                this._classesAlreadyInitializedInBlock.Add(type);
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new Func<MethodDefinition, bool>(Extensions.IsStaticConstructor);
                }
                MethodDefinition definition = type.Resolve().Methods.Single<MethodDefinition>(<>f__mg$cache0);
                if ((invokingMethod == null) || (definition != invokingMethod))
                {
                    this._writer.WriteLine(Statement.Expression(Emit.Call("IL2CPP_RUNTIME_CLASS_INIT", runtimeMetadataAccess.StaticData(type))));
                }
            }
        }

        private void WriteCastclass(TypeReference typeReference1, StackInfo value, Instruction ins)
        {
            TypeReference typeReference = this._typeResolver.Resolve(typeReference1);
            TypeReference type = !typeReference.IsValueType() ? typeReference : TypeProvider.ObjectTypeReference;
            string expression = Emit.Cast(type, Emit.Call("Castclass", value.Expression, this._runtimeMetadataAccess.TypeInfoFor(typeReference1)));
            this.PushExpression(type, expression);
        }

        private void WriteCastclassOrIsInst(TypeReference targetType, StackInfo value, string operation)
        {
            TypeReference typeReference = this._typeResolver.Resolve(targetType);
            TypeReference reference2 = !typeReference.IsValueType() ? typeReference : TypeProvider.ObjectTypeReference;
            this._writer.AddIncludeForTypeDefinition(typeReference);
            this.PushExpression(reference2, Emit.Cast(reference2, this.GetCastclassOrIsInstCall(targetType, value, operation, typeReference)));
        }

        private void WriteCheckForOverflow<TMaxValue>(bool treatInputAsUnsigned, TMaxValue maxValue, bool inputIsNumber)
        {
            StackInfo info = this._valueStack.Peek();
            if (info.Type.IsSameType(this.DoubleTypeReference) || info.Type.IsSameType(this.SingleTypeReference))
            {
                object[] args = new object[] { info.Expression, maxValue, Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()") };
                this._writer.WriteLine("if ({0} > (double)({1})) {2};", args);
            }
            else if (treatInputAsUnsigned)
            {
                object[] objArray2 = new object[] { info.Expression, maxValue, !inputIsNumber ? "" : "LL", Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()") };
                this._writer.WriteLine("if ((uint64_t)({0}) > {1}{2}) {3};", objArray2);
            }
            else
            {
                object[] objArray3 = new object[] { info.Expression, maxValue, !inputIsNumber ? "" : "LL", Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()") };
                this._writer.WriteLine("if ((int64_t)({0}) > {1}{2}) {3};", objArray3);
            }
        }

        private void WriteComment(string message, params object[] args)
        {
            object[] objArray1 = new object[] { string.Format(message, args) };
            this._writer.WriteLine("// {0}", objArray1);
        }

        private void WriteDup()
        {
            StackInfo right = this._valueStack.Pop();
            if (right.Expression == Naming.ThisParameterName)
            {
                this._valueStack.Push(new StackInfo(Naming.ThisParameterName, right.Type));
                this._valueStack.Push(new StackInfo(Naming.ThisParameterName, right.Type));
            }
            else if ((right.Expression == "NULL") && right.Type.IsSystemObject())
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

        private static string WriteExpressionAndCastIfNeeded(TypeReference leftType, StackInfo right, SharingType sharingType = 0)
        {
            if ((leftType.MetadataType == MetadataType.Boolean) && right.Type.IsIntegralType())
            {
                return EmitCastRightCastToLeftType(leftType, right);
            }
            if (leftType.IsPointer)
            {
                return EmitCastRightCastToLeftType(leftType, right);
            }
            if (leftType.IsGenericParameter())
            {
                return right.Expression;
            }
            if ((right.Type.MetadataType == MetadataType.Object) && (leftType.MetadataType != MetadataType.Object))
            {
                return EmitCastRightCastToLeftType(leftType, right);
            }
            if ((right.Type.IsArray && leftType.IsArray) && (right.Type.FullName != leftType.FullName))
            {
                return EmitCastRightCastToLeftType(leftType, right);
            }
            if ((right.Type.MetadataType == MetadataType.IntPtr) && (leftType.MetadataType == MetadataType.Int32))
            {
                if (<>f__am$cache7 == null)
                {
                    <>f__am$cache7 = field => field.Name == Naming.IntPtrValueField;
                }
                FieldDefinition definition = TypeProvider.SystemIntPtr.Fields.Single<FieldDefinition>(<>f__am$cache7);
                return $"({Naming.ForVariable(leftType)})({Naming.ForIntPtrT}){right.Expression}.{Naming.ForFieldGetter(definition)}()";
            }
            ByReferenceType type = leftType as ByReferenceType;
            if (((type != null) && (type.ElementType.IsIntegralPointerType() || type.ElementType.MetadataType.IsPrimitiveType())) && (right.Type == TypeProvider.NativeIntTypeReference))
            {
                return EmitCastRightCastToLeftType(leftType, right);
            }
            if ((leftType.IsIntegralType() && right.Type.IsIntegralType()) && (GetMetadataTypeOrderFor(leftType) < GetMetadataTypeOrderFor(right.Type)))
            {
                return EmitCastRightCastToLeftType(leftType, right);
            }
            if (sharingType == SharingType.Shared)
            {
                return EmitCastRightCastToLeftType(leftType, right);
            }
            if (!VarianceSupport.IsNeededForConversion(leftType, right.Type))
            {
                return right.Expression;
            }
            return $"{VarianceSupport.Apply(leftType, right.Type)}{right.Expression}";
        }

        private void WriteGlobalVariableAssignmentForLeftBranch(InstructionBlock block, Instruction targetInstruction)
        {
            <WriteGlobalVariableAssignmentForLeftBranch>c__AnonStorey5 storey = new <WriteGlobalVariableAssignmentForLeftBranch>c__AnonStorey5 {
                targetInstruction = targetInstruction
            };
            GlobalVariable[] globalVariables = this._stackAnalysis.InputVariablesFor(block.Successors.Single<InstructionBlock>(new Func<InstructionBlock, bool>(storey.<>m__0)));
            this.WriteAssignGlobalVariables(globalVariables);
        }

        private void WriteGlobalVariableAssignmentForRightBranch(InstructionBlock block, Instruction targetInstruction)
        {
            <WriteGlobalVariableAssignmentForRightBranch>c__AnonStorey4 storey = new <WriteGlobalVariableAssignmentForRightBranch>c__AnonStorey4 {
                targetInstruction = targetInstruction
            };
            GlobalVariable[] globalVariables = this._stackAnalysis.InputVariablesFor(block.Successors.Single<InstructionBlock>(new Func<InstructionBlock, bool>(storey.<>m__0)));
            this.WriteAssignGlobalVariables(globalVariables);
        }

        private void WriteJump(Instruction targetInstruction)
        {
            this._writer.WriteLine(this._labeler.ForJump(targetInstruction));
        }

        private void WriteLabelForBranchTarget(Instruction ins)
        {
            if (!this.DidAlreadyEmitLabelFor(ins))
            {
                this._emittedLabels.Add(ins);
                string str = "";
                if (this._referencedLabels.Contains(ins))
                {
                    this._writer.WriteLine();
                    this._writer.WriteUnindented($"{str}{this._labeler.ForLabel(ins)}", new object[0]);
                }
            }
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
                this._valueStack.Push(new StackInfo(Naming.ThisParameterName, typeReference));
            }
            else
            {
                TypeReference type = this._typeResolver.ResolveParameterType(this._methodReference, this._methodReference.Parameters[index]);
                Local local = this.NewTemp(type);
                string right = Naming.ForParameterName(this._methodDefinition.Parameters[index]);
                if (!this.CanApplyValueTypeBoxBranchOptimizationToInstruction(ins.Next, block) && ((ins.Next.OpCode.Code != Code.Ldobj) || !this.CanApplyValueTypeBoxBranchOptimizationToInstruction(ins.Next.Next, block)))
                {
                    object[] args = new object[] { Emit.Assign(local.IdentifierExpression, right) };
                    this._writer.WriteLine("{0};", args);
                }
                this._valueStack.Push(new StackInfo(local));
            }
        }

        private void WriteLdloc(int index, InstructionBlock block, Instruction ins)
        {
            VariableDefinition variable = this._methodDefinition.Body.Variables[index];
            TypeReference type = this._typeResolver.ResolveVariableType(this._methodReference, variable);
            Local local = this.NewTemp(type);
            this._valueStack.Push(new StackInfo(local));
            if (!this.CanApplyValueTypeBoxBranchOptimizationToInstruction(ins.Next, block))
            {
                object[] args = new object[] { Emit.Assign(local.IdentifierExpression, Naming.ForVariableName(variable)) };
                this._writer.WriteLine("{0};", args);
            }
        }

        private void WriteLoadObject(Instruction ins)
        {
            StackInfo info = this._valueStack.Pop();
            TypeReference reference = this._typeResolver.Resolve((TypeReference) ins.Operand);
            PointerType variableType = new PointerType(reference);
            this._valueStack.Push(new StackInfo($"(*({Naming.ForVariable(variableType)}){info.Expression})", reference));
            this.EmitMemoryBarrierIfNecessary(null);
        }

        private void WriteLocalVariables()
        {
            foreach (KeyValuePair<string, TypeReference> pair in this.ResolveLocalVariableTypes())
            {
                this._writer.AddIncludeForTypeDefinition(pair.Value);
                this._writer.WriteVariable(pair.Value, pair.Key);
            }
        }

        private void WriteMul(OverflowCheck check)
        {
            StackInfo right = this._valueStack.Pop();
            StackInfo left = this._valueStack.Pop();
            if (check != OverflowCheck.None)
            {
                TypeReference leftStackType = StackTypeConverter.StackTypeFor(left.Type);
                TypeReference rightStackType = StackTypeConverter.StackTypeFor(right.Type);
                if (this.RequiresPointerOverflowCheck(leftStackType, rightStackType))
                {
                    this.WritePointerOverflowCheckUsing64Bits("*", check, left, right);
                }
                else if (Requires64BitOverflowCheck(leftStackType.MetadataType, rightStackType.MetadataType))
                {
                    if (check == OverflowCheck.Signed)
                    {
                        object[] args = new object[] { left.Expression, right.Expression };
                        this._writer.WriteLine("if (il2cpp_codegen_check_mul_overflow_i64((int64_t){0}, (int64_t){1}, kIl2CppInt64Min, kIl2CppInt64Max))", args);
                        object[] objArray2 = new object[] { Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()") };
                        this._writer.WriteLine("\t{0};", objArray2);
                    }
                    else
                    {
                        object[] objArray3 = new object[] { left.Expression, right.Expression };
                        this._writer.WriteLine("if ((uint64_t){1} != 0 && (((uint64_t){0} * (uint64_t){1}) / (uint64_t){1} != (uint64_t){0}))", objArray3);
                        object[] objArray4 = new object[] { Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()") };
                        this._writer.WriteLine("\t{0};", objArray4);
                    }
                }
                else
                {
                    this.WriteNarrowOverflowCheckUsing64Bits("*", check, left.Expression, right.Expression);
                }
            }
            TypeReference leftType = Naming.RemoveModifiers(left.Type);
            TypeReference rightType = Naming.RemoveModifiers(right.Type);
            TypeReference resultType = StackAnalysisUtils.ResultTypeForMul(leftType, rightType, TypeProvider);
            this.WriteBinaryOperation("*", left, right, resultType);
        }

        private void WriteNarrowOverflowCheckUsing64Bits(string op, OverflowCheck check, string leftExpression, string rightExpression)
        {
            if (check == OverflowCheck.Signed)
            {
                object[] args = new object[] { op, leftExpression, rightExpression };
                this._writer.WriteLine("if (((int64_t){1} {0} (int64_t){2} < (int64_t)kIl2CppInt32Min) || ((int64_t){1} {0} (int64_t){2} > (int64_t)kIl2CppInt32Max))", args);
                object[] objArray2 = new object[] { Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()") };
                this._writer.WriteLine("\t{0};", objArray2);
            }
            else
            {
                object[] objArray3 = new object[] { op, leftExpression, rightExpression };
                this._writer.WriteLine("if ((uint64_t)(uint32_t){1} {0} (uint64_t)(uint32_t){2} > (uint64_t)(uint32_t)kIl2CppUInt32Max)", objArray3);
                object[] objArray4 = new object[] { Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()") };
                this._writer.WriteLine("\t{0};", objArray4);
            }
        }

        private void WriteNegateOperation()
        {
            StackInfo originalValue = this._valueStack.Pop();
            TypeReference a = this.CalculateResultTypeForNegate(originalValue.Type);
            if (a.IsSameType(TypeProvider.SystemIntPtr))
            {
                if (<>f__am$cacheC == null)
                {
                    <>f__am$cacheC = f => f.Name == Naming.IntPtrValueField;
                }
                FieldDefinition field = TypeProvider.SystemIntPtr.Fields.Single<FieldDefinition>(<>f__am$cacheC);
                string str = Naming.ForFieldGetter(field);
                string str2 = $"{originalValue.Expression}.{str}()";
                string stringValue = $"(-(intptr_t){str2})";
                Local local = this.EmitLocalIntPtrWithValue(stringValue);
                this.PushExpression(local.Type, local.Expression);
            }
            else
            {
                this.PushExpression(originalValue.Type, $"(-{CastTypeIfNeeded(originalValue, a)})");
            }
        }

        private void WriteNotOperation()
        {
            StackInfo info = this._valueStack.Pop();
            this.PushExpression(info.Type, $"(~{info.Expression})");
        }

        private void WriteNumericConversion(TypeReference typeReference)
        {
            this.WriteNumericConversion(typeReference, typeReference);
        }

        private void WriteNumericConversion(TypeReference inputType, TypeReference outputType)
        {
            StackInfo info = this._valueStack.Pop();
            string str = info.ToString();
            if ((info.Type.MetadataType == MetadataType.IntPtr) || (info.Type.MetadataType == MetadataType.UIntPtr))
            {
                str = this.FormatNativeIntGetterName(info.Expression, info.Type);
            }
            this.PushExpression(outputType, $"(({Naming.ForVariable(outputType)})(({Naming.ForVariable(inputType)}){(info.Type.MetadataType != MetadataType.Pointer) ? "" : "(intptr_t)"}{str}))");
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

        private void WriteNumericConversionI8()
        {
            if (this._valueStack.Peek().Type.IsSameType(this.UInt32TypeReference))
            {
                this.WriteNumericConversion(this.Int32TypeReference);
            }
            this.WriteNumericConversion(this.Int64TypeReference, this.Int64TypeReference);
        }

        private void WriteNumericConversionToFloatFromUnsigned()
        {
            StackInfo info = this._valueStack.Peek();
            if ((info.Type.MetadataType == MetadataType.Single) || (info.Type.MetadataType == MetadataType.Double))
            {
                this.WriteNumericConversion(info.Type, this.DoubleTypeReference);
            }
            else if ((info.Type.MetadataType == MetadataType.Int64) || (info.Type.MetadataType == MetadataType.UInt64))
            {
                this.WriteNumericConversion(this.UInt64TypeReference, this.DoubleTypeReference);
            }
            else
            {
                this.WriteNumericConversion(this.UInt32TypeReference, this.DoubleTypeReference);
            }
        }

        private void WriteNumericConversionU8()
        {
            if (this._valueStack.Peek().Type.IsSameType(this.Int32TypeReference))
            {
                this.WriteNumericConversion(this.UInt32TypeReference);
            }
            this.WriteNumericConversion(this.UInt64TypeReference, this.Int64TypeReference);
        }

        private void WriteNumericConversionWithOverflow<TMaxValue>(TypeReference typeReference, bool treatInputAsUnsigned, TMaxValue maxValue, bool inputIsValue = true)
        {
            this.WriteCheckForOverflow<TMaxValue>(treatInputAsUnsigned, maxValue, inputIsValue);
            this.WriteNumericConversion(typeReference);
        }

        private void WritePointerOverflowCheckUsing64Bits(string op, OverflowCheck check, string leftExpression, string rightExpression)
        {
            if (check == OverflowCheck.Signed)
            {
                object[] args = new object[] { op, leftExpression, rightExpression };
                this._writer.WriteLine("if (((intptr_t){1} {0} (intptr_t){2} < (intptr_t)kIl2CppIntPtrMin) || ((intptr_t){1} {0} (intptr_t){2} > (intptr_t)kIl2CppIntPtrMax))", args);
                object[] objArray2 = new object[] { Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()") };
                this._writer.WriteLine("\t{0};", objArray2);
            }
            else
            {
                object[] objArray3 = new object[] { op, leftExpression, rightExpression };
                this._writer.WriteLine("if ((uintptr_t){1} {0} (uintptr_t){2} > (uintptr_t)kIl2CppUIntPtrMax)", objArray3);
                object[] objArray4 = new object[] { Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()") };
                this._writer.WriteLine("\t{0};", objArray4);
            }
        }

        private void WritePointerOverflowCheckUsing64Bits(string op, OverflowCheck check, StackInfo left, StackInfo right)
        {
            string expressionFor = this.GetExpressionFor(left);
            string rightExpression = this.GetExpressionFor(right);
            this.WritePointerOverflowCheckUsing64Bits(op, check, expressionFor, rightExpression);
        }

        private void WriteRemainderOperation()
        {
            StackInfo right = this._valueStack.Pop();
            StackInfo left = this._valueStack.Pop();
            if ((right.Type.MetadataType == MetadataType.Single) || (left.Type.MetadataType == MetadataType.Single))
            {
                this.PushExpression(this.SingleTypeReference, $"fmodf({left.Expression}, {right.Expression})");
            }
            else if ((right.Type.MetadataType == MetadataType.Double) || (left.Type.MetadataType == MetadataType.Double))
            {
                this.PushExpression(this.DoubleTypeReference, $"fmod({left.Expression}, {right.Expression})");
            }
            else
            {
                this.WriteBinaryOperation("%", left, right, left.Type);
            }
        }

        private void WriteReturnStatement()
        {
            TypeReference type = this._typeResolver.ResolveReturnType(this._methodDefinition);
            if (((type.MetadataType == MetadataType.Void) && (this._valueStack.Count > 0)) || ((type.MetadataType != MetadataType.Void) && (this._valueStack.Count > 1)))
            {
                throw new InvalidOperationException($"Attempting to return a value from method '{this._methodDefinition.FullName}' when there is no value on the stack. Is this invalid IL code?");
            }
            if (type.MetadataType != MetadataType.Void)
            {
                StackInfo right = this._valueStack.Pop();
                string str = CastIfPointerType(type);
                bool flag = ((right.Type.IsSameType(this.NativeIntTypeReference) || right.Type.IsSameType(this.NativeUIntTypeReference)) || (right.Type is ByReferenceType)) || (right.Type is PointerType);
                if (((type.FullName != right.Type.FullName) && (type.Resolve() != null)) && type.Resolve().IsEnum)
                {
                    object[] args = new object[] { Naming.ForVariable(type), right.Expression };
                    this._writer.WriteLine("return ({0})({1});", args);
                }
                else if (!string.IsNullOrEmpty(str))
                {
                    object[] objArray2 = new object[] { str, right.Expression };
                    this._writer.WriteLine("return {0}({1});", objArray2);
                }
                else if ((type.MetadataType == MetadataType.IntPtr) && flag)
                {
                    Local local = this.EmitLocalIntPtrWithValue(right.Expression);
                    object[] objArray3 = new object[] { local.Expression };
                    this._writer.WriteLine("return {0};", objArray3);
                }
                else if ((type.MetadataType == MetadataType.UIntPtr) && flag)
                {
                    Local local2 = this.EmitLocalUIntPtrWithValue(right.Expression);
                    object[] objArray4 = new object[] { local2.Expression };
                    this._writer.WriteLine("return {0};", objArray4);
                }
                else
                {
                    object[] objArray5 = new object[] { WriteExpressionAndCastIfNeeded(type, right, SharingType.NonShared) };
                    this._writer.WriteLine("return {0};", objArray5);
                }
            }
            else
            {
                this._writer.WriteLine("return;");
            }
        }

        private void WriteShrUn()
        {
            StackInfo info = this._valueStack.Pop();
            StackInfo info2 = this._valueStack.Pop();
            string lcast = "";
            TypeReference destType = StackTypeConverter.StackTypeFor(info2.Type);
            if (destType.MetadataType == MetadataType.Int32)
            {
                lcast = "(uint32_t)";
            }
            if (destType.MetadataType == MetadataType.Int64)
            {
                lcast = "(uint64_t)";
            }
            this.WriteBinaryOperation(destType, lcast, info2.Expression, ">>", "", info.Expression);
        }

        private void WriteStloc(int index)
        {
            StackInfo info = this._valueStack.Pop();
            VariableDefinition variable = this._methodDefinition.Body.Variables[index];
            TypeReference destinationVariable = Naming.RemoveModifiers(variable.VariableType);
            TypeReference a = Naming.RemoveModifiers(info.Type);
            if ((destinationVariable.IsPointer || destinationVariable.IsByReference) || this.RequiresContravariantCastToStore(destinationVariable, info.Type))
            {
                object[] args = new object[] { Naming.ForVariableName(variable), Naming.ForVariable(this._typeResolver.Resolve(destinationVariable)), info };
                this._writer.WriteLine("{0} = ({1}){2};", args);
            }
            else if ((variable.VariableType.IsSameType(this.IntPtrTypeReference) && !a.IsSameType(this.IntPtrTypeReference)) || (variable.VariableType.IsSameType(this.UIntPtrTypeReference) && !a.IsSameType(this.UIntPtrTypeReference)))
            {
                this._writer.WriteLine(this.FormatNativeIntSetterInvocation(Naming.ForVariableName(variable), variable.VariableType, $"(void*){info}"));
            }
            else
            {
                this.WriteAssignment(Naming.ForVariableName(variable), this._typeResolver.Resolve(variable.VariableType), info);
            }
        }

        private void WriteStoreObject(TypeReference type)
        {
            TypeReference reference = this._typeResolver.Resolve(type);
            StackInfo info = this._valueStack.Pop();
            StackInfo info2 = this._valueStack.Pop();
            this.EmitMemoryBarrierIfNecessary(null);
            string str = Emit.Cast(new PointerType(reference), info2.Expression);
            this._writer.WriteStatement(Emit.Assign(Emit.Dereference(str), info.Expression));
            this._writer.WriteWriteBarrierIfNeeded(reference, str, info.Expression);
        }

        private void WriteSub(OverflowCheck check)
        {
            StackInfo right = this._valueStack.Pop();
            StackInfo left = this._valueStack.Pop();
            if (check != OverflowCheck.None)
            {
                TypeReference leftStackType = StackTypeConverter.StackTypeFor(left.Type);
                TypeReference rightStackType = StackTypeConverter.StackTypeFor(right.Type);
                if (this.RequiresPointerOverflowCheck(leftStackType, rightStackType))
                {
                    this.WritePointerOverflowCheckUsing64Bits("-", check, left, right);
                }
                else if (Requires64BitOverflowCheck(leftStackType.MetadataType, rightStackType.MetadataType))
                {
                    if (check == OverflowCheck.Signed)
                    {
                        object[] args = new object[] { left.Expression, right.Expression };
                        this._writer.WriteLine("if (((int64_t){1} >= 0 && (int64_t){0} < kIl2CppInt64Min + (int64_t){1}) || ((int64_t){1} < 0 && (int64_t){0} > kIl2CppInt64Max + (int64_t){1}))", args);
                        object[] objArray2 = new object[] { Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()") };
                        this._writer.WriteLine("\t{0};", objArray2);
                    }
                    else
                    {
                        object[] objArray3 = new object[] { left.Expression, right.Expression };
                        this._writer.WriteLine("if ((uint64_t){0} < (uint64_t){1})", objArray3);
                        object[] objArray4 = new object[] { Emit.RaiseManagedException("il2cpp_codegen_get_overflow_exception()") };
                        this._writer.WriteLine("\t{0};", objArray4);
                    }
                }
                else
                {
                    this.WriteNarrowOverflowCheckUsing64Bits("-", check, left.Expression, right.Expression);
                }
            }
            TypeReference leftType = Naming.RemoveModifiers(left.Type);
            TypeReference rightType = Naming.RemoveModifiers(right.Type);
            TypeReference resultType = StackAnalysisUtils.ResultTypeForSub(leftType, rightType, TypeProvider);
            this.WriteBinaryOperation("-", left, right, resultType);
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

        private void WriteUnsignedArithmeticOperation(string op)
        {
            StackInfo info = this._valueStack.Pop();
            StackInfo info2 = this._valueStack.Pop();
            TypeReference type = StackTypeConverter.StackTypeForBinaryOperation(info2.Type);
            TypeReference reference2 = StackTypeConverter.StackTypeForBinaryOperation(info.Type);
            TypeReference reference3 = (GetMetadataTypeOrderFor(type) >= GetMetadataTypeOrderFor(reference2)) ? this.GetUnsignedType(type) : this.GetUnsignedType(reference2);
            this.WriteBinaryOperation(this.GetSignedType(reference3), $"({Naming.ForVariable(reference3)})({Naming.ForVariable(type)})", this.ExpressionForBinaryOperation(info2.Type, info2.Expression), op, $"({Naming.ForVariable(reference3)})({Naming.ForVariable(reference2)})", this.ExpressionForBinaryOperation(info.Type, info.Expression));
        }

        private TypeReference ByteTypeReference =>
            TypeProvider.ByteTypeReference;

        private TypeReference DoubleTypeReference =>
            TypeProvider.DoubleTypeReference;

        private TypeReference Int16TypeReference =>
            TypeProvider.Int16TypeReference;

        private TypeReference Int32TypeReference =>
            TypeProvider.Int32TypeReference;

        private TypeReference Int64TypeReference =>
            TypeProvider.Int64TypeReference;

        private TypeReference IntPtrTypeReference =>
            TypeProvider.IntPtrTypeReference;

        private TypeReference NativeIntTypeReference =>
            TypeProvider.NativeIntTypeReference;

        private TypeReference NativeUIntTypeReference =>
            TypeProvider.NativeUIntTypeReference;

        private TypeReference ObjectTypeReference =>
            TypeProvider.ObjectTypeReference;

        private TypeReference RuntimeFieldHandleTypeReference =>
            TypeProvider.RuntimeFieldHandleTypeReference;

        private TypeReference RuntimeMethodHandleTypeReference =>
            TypeProvider.RuntimeMethodHandleTypeReference;

        private TypeReference RuntimeTypeHandleTypeReference =>
            TypeProvider.RuntimeTypeHandleTypeReference;

        private TypeReference SByteTypeReference =>
            TypeProvider.SByteTypeReference;

        private TypeReference SingleTypeReference =>
            TypeProvider.SingleTypeReference;

        private TypeReference StringTypeReference =>
            TypeProvider.StringTypeReference;

        private TypeReference UInt16TypeReference =>
            TypeProvider.UInt16TypeReference;

        private TypeReference UInt32TypeReference =>
            TypeProvider.UInt32TypeReference;

        private TypeReference UInt64TypeReference =>
            TypeProvider.UInt64TypeReference;

        private TypeReference UIntPtrTypeReference =>
            TypeProvider.UIntPtrTypeReference;

        [CompilerGenerated]
        private sealed class <GenerateConditionalJump>c__AnonStorey6
        {
            internal Instruction targetInstruction;

            internal bool <>m__0(InstructionBlock b) => 
                (b.First.Offset != this.targetInstruction.Offset);

            internal bool <>m__1(InstructionBlock b) => 
                (b.First.Offset == this.targetInstruction.Offset);
        }

        [CompilerGenerated]
        private sealed class <GetParameterTypes>c__AnonStorey8
        {
            internal MethodReference method;
            internal Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolverForMethodToCall;

            internal TypeReference <>m__0(ParameterDefinition parameter) => 
                this.typeResolverForMethodToCall.Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveParameterTypeIfNeeded(this.method, parameter));
        }

        [CompilerGenerated]
        private sealed class <ProcessInstruction>c__AnonStorey0
        {
            internal string suffix;

            internal string <>m__0(string s) => 
                (s + this.suffix);

            internal string <>m__1(string s) => 
                (s + this.suffix);
        }

        [CompilerGenerated]
        private sealed class <ProcessInstruction>c__AnonStorey1
        {
            internal string suffix;

            internal string <>m__0(string s) => 
                (s + this.suffix);
        }

        [CompilerGenerated]
        private sealed class <ProcessInstruction>c__AnonStorey2
        {
            private static Func<Instruction, int> <>f__am$cache0;
            internal Instruction[] targetInstructions;

            internal bool <>m__0(InstructionBlock b)
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = t => t.Offset;
                }
                return !this.targetInstructions.Select<Instruction, int>(<>f__am$cache0).Contains<int>(b.First.Offset);
            }

            private static int <>m__1(Instruction t) => 
                t.Offset;
        }

        [CompilerGenerated]
        private sealed class <ProcessInstruction>c__AnonStorey3
        {
            internal Instruction targetInstruction;

            internal bool <>m__0(InstructionBlock b) => 
                (b.First.Offset == this.targetInstruction.Offset);
        }

        [CompilerGenerated]
        private sealed class <WriteAssignGlobalVariables>c__AnonStorey7
        {
            internal int stackIndex;

            internal bool <>m__0(GlobalVariable v) => 
                (v.Index == this.stackIndex);
        }

        [CompilerGenerated]
        private sealed class <WriteGlobalVariableAssignmentForLeftBranch>c__AnonStorey5
        {
            internal Instruction targetInstruction;

            internal bool <>m__0(InstructionBlock b) => 
                (b.First.Offset != this.targetInstruction.Offset);
        }

        [CompilerGenerated]
        private sealed class <WriteGlobalVariableAssignmentForRightBranch>c__AnonStorey4
        {
            internal Instruction targetInstruction;

            internal bool <>m__0(InstructionBlock b) => 
                (b.First.Offset == this.targetInstruction.Offset);
        }

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
    }
}

