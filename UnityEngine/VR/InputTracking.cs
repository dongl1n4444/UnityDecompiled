namespace UnityEngine.VR
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A collection of methods and properties for interacting with the VR tracking system.</para>
    /// </summary>
    [RequiredByNativeCode]
    public static class InputTracking
    {
        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public static  event Action<VRNodeState> nodeAdded;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public static  event Action<VRNodeState> nodeRemoved;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public static  event Action<VRNodeState> trackingAcquired;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public static  event Action<VRNodeState> trackingLost;

        /// <summary>
        /// <para>Gets the position of a specific node.</para>
        /// </summary>
        /// <param name="node">Specifies which node's position should be returned.</param>
        /// <returns>
        /// <para>The position of the node in its local tracking space.</para>
        /// </returns>
        public static Vector3 GetLocalPosition(VRNode node)
        {
            Vector3 vector;
            INTERNAL_CALL_GetLocalPosition(node, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Gets the rotation of a specific node.</para>
        /// </summary>
        /// <param name="node">Specifies which node's rotation should be returned.</param>
        /// <returns>
        /// <para>The rotation of the node in its local tracking space.</para>
        /// </returns>
        public static Quaternion GetLocalRotation(VRNode node)
        {
            Quaternion quaternion;
            INTERNAL_CALL_GetLocalRotation(node, out quaternion);
            return quaternion;
        }

        /// <summary>
        /// <para>Accepts the unique identifier for a tracked node and returns a friendly name for it.</para>
        /// </summary>
        /// <param name="uniqueID">The unique identifier for the Node index.</param>
        /// <returns>
        /// <para>The name of the tracked node if the given 64-bit identifier maps to a currently tracked node. Empty string otherwise.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string GetNodeName(ulong uniqueID);
        public static void GetNodeStates(List<VRNodeState> nodeStates)
        {
            if (nodeStates == null)
            {
                throw new ArgumentNullException("nodeStates");
            }
            nodeStates.Clear();
            GetNodeStatesInternal(nodeStates);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void GetNodeStatesInternal(object nodeStates);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetLocalPosition(VRNode node, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetLocalRotation(VRNode node, out Quaternion value);
        [RequiredByNativeCode]
        private static void InvokeTrackingEvent(TrackingStateEventType eventType, VRNode nodeType, long uniqueID, bool tracked)
        {
            Action<VRNodeState> nodeAdded = null;
            VRNodeState state = new VRNodeState {
                uniqueID = (ulong) uniqueID,
                nodeType = nodeType,
                tracked = tracked
            };
            switch (eventType)
            {
                case TrackingStateEventType.NodeAdded:
                    nodeAdded = InputTracking.nodeAdded;
                    break;

                case TrackingStateEventType.NodeRemoved:
                    nodeAdded = nodeRemoved;
                    break;

                case TrackingStateEventType.TrackingAcquired:
                    nodeAdded = trackingAcquired;
                    break;

                case TrackingStateEventType.TrackingLost:
                    nodeAdded = trackingLost;
                    break;

                default:
                    throw new ArgumentException("TrackingEventHandler - Invalid EventType: " + eventType);
            }
            if (nodeAdded != null)
            {
                nodeAdded(state);
            }
        }

        /// <summary>
        /// <para>Center tracking to the current position and orientation of the HMD.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Recenter();

        /// <summary>
        /// <para>Disables positional tracking in VR. This takes effect the next time the head pose is sampled.  If set to true the camera only tracks headset rotation state.</para>
        /// </summary>
        public static bool disablePositionalTracking { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        private enum TrackingStateEventType
        {
            NodeAdded,
            NodeRemoved,
            TrackingAcquired,
            TrackingLost
        }
    }
}

