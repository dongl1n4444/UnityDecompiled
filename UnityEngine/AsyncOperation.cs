namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Asynchronous operation coroutine.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public class AsyncOperation : YieldInstruction
    {
        internal IntPtr m_Ptr;
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
        private extern void InternalDestroy();
        ~AsyncOperation()
        {
            this.InternalDestroy();
        }

        /// <summary>
        /// <para>Has the operation finished? (Read Only)</para>
        /// </summary>
        public bool isDone { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
        /// <summary>
        /// <para>What's the operation's progress. (Read Only)</para>
        /// </summary>
        public float progress { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
        /// <summary>
        /// <para>Priority lets you tweak in which order async operation calls will be performed.</para>
        /// </summary>
        public int priority { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
        /// <summary>
        /// <para>Allow scenes to be activated as soon as it is ready.</para>
        /// </summary>
        public bool allowSceneActivation { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

