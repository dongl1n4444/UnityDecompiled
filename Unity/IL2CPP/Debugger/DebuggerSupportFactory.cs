namespace Unity.IL2CPP.Debugger
{
    using System;

    public sealed class DebuggerSupportFactory
    {
        private static IDebuggerSupport _debuggerSupport;

        public static IDebuggerSupport GetDebuggerSupport()
        {
            if (_debuggerSupport != null)
            {
                return _debuggerSupport;
            }
            if (!DebuggerOptions.Enabled)
            {
                return (_debuggerSupport = new DisabledDebuggerSupport());
            }
            return (_debuggerSupport = new DebuggerSupport());
        }
    }
}

