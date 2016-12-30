namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    internal sealed class AnimationOffsetPlayable : AnimationPlayable
    {
        private static Vector3 GetPosition(ref PlayableHandle handle)
        {
            Vector3 vector;
            INTERNAL_CALL_GetPosition(ref handle, out vector);
            return vector;
        }

        private static Quaternion GetRotation(ref PlayableHandle handle)
        {
            Quaternion quaternion;
            INTERNAL_CALL_GetRotation(ref handle, out quaternion);
            return quaternion;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetPosition(ref PlayableHandle handle, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetRotation(ref PlayableHandle handle, out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetPosition(ref PlayableHandle handle, ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetRotation(ref PlayableHandle handle, ref Quaternion value);
        private static void SetPosition(ref PlayableHandle handle, Vector3 value)
        {
            INTERNAL_CALL_SetPosition(ref handle, ref value);
        }

        private static void SetRotation(ref PlayableHandle handle, Quaternion value)
        {
            INTERNAL_CALL_SetRotation(ref handle, ref value);
        }

        public Vector3 position
        {
            get => 
                GetPosition(ref this.handle);
            set
            {
                SetPosition(ref this.handle, value);
            }
        }

        public Quaternion rotation
        {
            get => 
                GetRotation(ref this.handle);
            set
            {
                SetRotation(ref this.handle, value);
            }
        }
    }
}

