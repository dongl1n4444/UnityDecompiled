using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Unity.TinyProfiling;

namespace Unity.IL2CPP.Statistics
{
	public class ProfilerSnapshot
	{
		private readonly ReadOnlyCollection<TinyProfiler.ThreadContext> _profilerData;

		public ProfilerSnapshot(ReadOnlyCollection<TinyProfiler.ThreadContext> profilerData)
		{
			this._profilerData = profilerData;
		}

		public static ProfilerSnapshot Capture()
		{
			return new ProfilerSnapshot(TinyProfiler.CaptureSnapshot());
		}

		[DebuggerHidden]
		public IEnumerable<TinyProfiler.TimedSection> GetSectionsByLabel(string label)
		{
			ProfilerSnapshot.<GetSectionsByLabel>c__Iterator0 <GetSectionsByLabel>c__Iterator = new ProfilerSnapshot.<GetSectionsByLabel>c__Iterator0();
			<GetSectionsByLabel>c__Iterator.label = label;
			<GetSectionsByLabel>c__Iterator.$this = this;
			ProfilerSnapshot.<GetSectionsByLabel>c__Iterator0 expr_15 = <GetSectionsByLabel>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}
	}
}
