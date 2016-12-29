namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct DivideByZeroCheckSupport
    {
        private readonly CppCodeWriter _writer;
        private readonly MethodDefinition _methodDefinition;
        private readonly bool _divideByZeroChecksGloballyEnabled;
        [Inject]
        public static IStatsService StatsService;
        public DivideByZeroCheckSupport(CppCodeWriter writer, MethodDefinition methodDefinition, bool divideByZeroChecksGloballyEnabled)
        {
            this._writer = writer;
            this._methodDefinition = methodDefinition;
            this._divideByZeroChecksGloballyEnabled = divideByZeroChecksGloballyEnabled;
        }

        public void WriteDivideByZeroCheckIfNeeded(StackInfo stackInfo)
        {
            if (this.ShouldEmitDivideByZeroChecksForMethod())
            {
                this.RecordDivideByZeroCheckEmitted();
                string str = Emit.DivideByZeroCheck(stackInfo.Type, stackInfo.Expression);
                if (!string.IsNullOrEmpty(str))
                {
                    object[] args = new object[] { str };
                    this._writer.WriteLine("{0};", args);
                }
            }
        }

        private void RecordDivideByZeroCheckEmitted()
        {
            StatsService.RecordDivideByZeroCheckEmitted(this._methodDefinition);
        }

        private bool ShouldEmitDivideByZeroChecksForMethod() => 
            CompilerServicesSupport.HasDivideByZeroChecksSupportEnabled(this._methodDefinition, this._divideByZeroChecksGloballyEnabled);
    }
}

