namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>An abstract base class for user-created scripting-driven DownloadHandler implementations.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class DownloadHandlerScript : DownloadHandler
    {
        /// <summary>
        /// <para>Create a DownloadHandlerScript which allocates new buffers when passing data to callbacks.</para>
        /// </summary>
        public DownloadHandlerScript()
        {
            base.InternalCreateScript();
        }

        /// <summary>
        /// <para>Create a DownloadHandlerScript which reuses a preallocated buffer to pass data to callbacks.</para>
        /// </summary>
        /// <param name="preallocatedBuffer">A byte buffer into which data will be copied, for use by DownloadHandler.ReceiveData.</param>
        public DownloadHandlerScript(byte[] preallocatedBuffer)
        {
            if ((preallocatedBuffer == null) || (preallocatedBuffer.Length < 1))
            {
                throw new ArgumentException("Cannot create a preallocated-buffer DownloadHandlerScript backed by a null or zero-length array");
            }
            base.InternalCreateScript();
            this.InternalSetPreallocatedBuffer(preallocatedBuffer);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void InternalSetPreallocatedBuffer(byte[] buffer);
    }
}

