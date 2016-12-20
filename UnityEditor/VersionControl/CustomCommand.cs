namespace UnityEditor.VersionControl
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class CustomCommand
    {
        private IntPtr m_thisDummy;

        internal CustomCommand()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern Task StartTask();

        [ThreadAndSerializationSafe]
        public CommandContext context { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        [ThreadAndSerializationSafe]
        public string label { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        [ThreadAndSerializationSafe]
        public string name { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

