using System;

namespace Unity.IL2CPP.Debugger
{
	public sealed class DebuggerSupportFactory
	{
		private static IDebuggerSupport _debuggerSupport;

		public static IDebuggerSupport GetDebuggerSupport()
		{
			IDebuggerSupport result;
			if (DebuggerSupportFactory._debuggerSupport != null)
			{
				result = DebuggerSupportFactory._debuggerSupport;
			}
			else if (!DebuggerOptions.Enabled)
			{
				result = (DebuggerSupportFactory._debuggerSupport = new DisabledDebuggerSupport());
			}
			else
			{
				result = (DebuggerSupportFactory._debuggerSupport = new DebuggerSupport());
			}
			return result;
		}
	}
}
