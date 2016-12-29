namespace UnityEngine.VR.WSA
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The WorldAnchor component allows a GameObject's position to be locked in physical space.</para>
    /// </summary>
    public sealed class WorldAnchor : Component
    {
        public event OnTrackingChangedDelegate OnTrackingChanged;

        private WorldAnchor()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_IsLocated_Internal(WorldAnchor self);
        [RequiredByNativeCode]
        private static void Internal_TriggerEventOnTrackingLost(WorldAnchor self, bool located)
        {
            if ((self != null) && (self.OnTrackingChanged != null))
            {
                self.OnTrackingChanged(self, located);
            }
        }

        private bool IsLocated_Internal() => 
            INTERNAL_CALL_IsLocated_Internal(this);

        /// <summary>
        /// <para>Returns true if this WorldAnchor is located (read only).  A return of false typically indicates a loss of tracking.</para>
        /// </summary>
        public bool isLocated =>
            this.IsLocated_Internal();

        /// <summary>
        /// <para>Event that is fired when this object's tracking state changes.</para>
        /// </summary>
        /// <param name="located">Set to true if the object is locatable.</param>
        /// <param name="self">The WorldAnchor reporting the tracking state change.</param>
        public delegate void OnTrackingChangedDelegate(WorldAnchor self, bool located);
    }
}

