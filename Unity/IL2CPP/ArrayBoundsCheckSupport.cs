namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class ArrayBoundsCheckSupport
    {
        private readonly bool _arrayBoundsChecksGloballyEnabled;
        private readonly MethodDefinition _methodDefinition;
        [Inject]
        public static IStatsService StatsService;

        public ArrayBoundsCheckSupport(MethodDefinition methodDefinition, bool arrayBoundsChecksGloballyEnabled)
        {
            this._methodDefinition = methodDefinition;
            this._arrayBoundsChecksGloballyEnabled = arrayBoundsChecksGloballyEnabled;
        }

        public void RecordArrayBoundsCheckEmitted()
        {
            if (this.ShouldEmitBoundsChecksForMethod())
            {
                StatsService.RecordArrayBoundsCheckEmitted(this._methodDefinition);
            }
        }

        public bool ShouldEmitBoundsChecksForMethod() => 
            CompilerServicesSupport.HasArrayBoundsChecksSupportEnabled(this._methodDefinition, this._arrayBoundsChecksGloballyEnabled);
    }
}

