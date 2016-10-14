using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
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
			if (this.ShouldEmitNullChecksForMethod())
			{
				if (!stackInfo.Type.IsValueType() && (!stackInfo.Type.IsByReference || !((ByReferenceType)stackInfo.Type).ElementType.IsValueType))
				{
					this.RecordNullCheckEmitted();
					this._writer.WriteLine("{0};", new object[]
					{
						Emit.NullCheck(stackInfo.Type, stackInfo.Expression)
					});
				}
			}
		}

		public void WriteNullCheckForInvocationIfNeeded(MethodReference methodReference, IList<string> args)
		{
			if (this.ShouldEmitNullChecksForMethod())
			{
				if (methodReference.HasThis)
				{
					if (!methodReference.DeclaringType.IsValueType())
					{
						if (!(args[0] == NullChecksSupport.Naming.ThisParameterName))
						{
							this.RecordNullCheckEmitted();
							this._writer.WriteLine("{0};", new object[]
							{
								Emit.NullCheck(args[0])
							});
						}
					}
				}
			}
		}

		private void RecordNullCheckEmitted()
		{
			NullChecksSupport.StatsService.RecordNullCheckEmitted(this._methodDefinition);
		}

		private bool ShouldEmitNullChecksForMethod()
		{
			return CompilerServicesSupport.HasNullChecksSupportEnabled(this._methodDefinition, this._nullChecksGloballyEnabled);
		}
	}
}
