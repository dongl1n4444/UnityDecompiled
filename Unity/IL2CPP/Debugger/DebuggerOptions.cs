namespace Unity.IL2CPP.Debugger
{
    using System;
    using Unity.Options;

    [ProgramOptions(Group="debugger")]
    public sealed class DebuggerOptions
    {
        [HideFromHelp]
        public static bool Enabled;
    }
}

