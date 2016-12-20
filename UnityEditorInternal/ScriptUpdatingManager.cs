namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class ScriptUpdatingManager
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void ReportExpectedUpdateFailure();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void ReportGroupedAPIUpdaterFailure(string msg);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool WaitForVCSServerConnection(bool reportTimeout);

        public static int numberOfTimesAsked { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

