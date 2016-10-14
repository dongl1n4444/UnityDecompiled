using Mono.Cecil;
using System;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	public class ArrayBoundsCheckSupport
	{
		private readonly CppCodeWriter _writer;

		private readonly MethodDefinition _methodDefinition;

		private readonly bool _arrayBoundsChecksGloballyEnabled;

		[Inject]
		public static IStatsService StatsService;

		public ArrayBoundsCheckSupport(CppCodeWriter writer, MethodDefinition methodDefinition, bool arrayBoundsChecksGloballyEnabled)
		{
			this._writer = writer;
			this._methodDefinition = methodDefinition;
			this._arrayBoundsChecksGloballyEnabled = arrayBoundsChecksGloballyEnabled;
		}

		public void WriteArrayBoundsCheckIfNeeded(string array, string index)
		{
			if (this.ShouldEmitBoundsChecksForMethod())
			{
				this.RecordArrayBoundsCheckEmitted();
				this._writer.WriteLine(Emit.ArrayBoundsCheck(array, index));
			}
		}

		private void RecordArrayBoundsCheckEmitted()
		{
			ArrayBoundsCheckSupport.StatsService.RecordArrayBoundsCheckEmitted(this._methodDefinition);
		}

		private bool ShouldEmitBoundsChecksForMethod()
		{
			return CompilerServicesSupport.HasArrayBoundsChecksSupportEnabled(this._methodDefinition, this._arrayBoundsChecksGloballyEnabled);
		}
	}
}
