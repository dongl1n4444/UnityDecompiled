using Mono.Cecil;
using System;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
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
				string text = Emit.DivideByZeroCheck(stackInfo.Type, stackInfo.Expression);
				if (!string.IsNullOrEmpty(text))
				{
					this._writer.WriteLine("{0};", new object[]
					{
						text
					});
				}
			}
		}

		private void RecordDivideByZeroCheckEmitted()
		{
			DivideByZeroCheckSupport.StatsService.RecordDivideByZeroCheckEmitted(this._methodDefinition);
		}

		private bool ShouldEmitDivideByZeroChecksForMethod()
		{
			return CompilerServicesSupport.HasDivideByZeroChecksSupportEnabled(this._methodDefinition, this._divideByZeroChecksGloballyEnabled);
		}
	}
}
