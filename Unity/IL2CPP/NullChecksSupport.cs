namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct NullChecksSupport
    {
        private readonly CppCodeWriter _writer;
        private readonly MethodDefinition _methodDefinition;
        private readonly bool _nullChecksGloballyEnabled;
        [Inject]
        public static IStatsService StatsService;
        [Inject]
        public static INamingService Naming;
        public NullChecksSupport(CppCodeWriter writer, MethodDefinition methodDefinition, bool nullChecksGloballyEnabled)
        {
            this._writer = writer;
            this._methodDefinition = methodDefinition;
            this._nullChecksGloballyEnabled = nullChecksGloballyEnabled;
        }

        public void WriteNullCheckIfNeeded(StackInfo stackInfo)
        {
            if ((this.ShouldEmitNullChecksForMethod() && !stackInfo.Type.IsValueType()) && (!stackInfo.Type.IsByReference || !((ByReferenceType) stackInfo.Type).ElementType.IsValueType))
            {
                this.RecordNullCheckEmitted();
                object[] args = new object[] { Emit.NullCheck(stackInfo.Type, stackInfo.Expression) };
                this._writer.WriteLine("{0};", args);
            }
        }

        public void WriteNullCheckForInvocationIfNeeded(MethodReference methodReference, IList<string> args)
        {
            if ((this.ShouldEmitNullChecksForMethod() && methodReference.HasThis) && (!methodReference.DeclaringType.IsValueType() && (args[0] != Naming.ThisParameterName)))
            {
                this.RecordNullCheckEmitted();
                object[] objArray1 = new object[] { Emit.NullCheck(args[0]) };
                this._writer.WriteLine("{0};", objArray1);
            }
        }

        private void RecordNullCheckEmitted()
        {
            StatsService.RecordNullCheckEmitted(this._methodDefinition);
        }

        private bool ShouldEmitNullChecksForMethod() => 
            CompilerServicesSupport.HasNullChecksSupportEnabled(this._methodDefinition, this._nullChecksGloballyEnabled);
    }
}

