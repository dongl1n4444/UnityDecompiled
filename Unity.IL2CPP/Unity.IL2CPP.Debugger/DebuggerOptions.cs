using System;
using Unity.Options;

namespace Unity.IL2CPP.Debugger
{
	[ProgramOptions(Group = "debugger")]
	public sealed class DebuggerOptions
	{
		[HideFromHelp]
		public static bool Enabled;
	}
}
