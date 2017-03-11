﻿namespace UnityEngine.VR
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct VRNodeState
    {
        private VRNode m_Type;
        private AvailableTrackingData m_AvailableFields;
        private Vector3 m_Position;
        private Quaternion m_Rotation;
        private Vector3 m_Velocity;
        private Quaternion m_AngularVelocity;
        private Vector3 m_Acceleration;
        private Quaternion m_AngularAcceleration;
        private int m_Tracked;
        private ulong m_UniqueID;
        /// <summary>
        /// <para>The unique identifier of the tracked node.</para>
        /// </summary>
        public ulong uniqueID
        {
            get => 
                this.m_UniqueID;
            set
            {
                this.m_UniqueID = value;
            }
        }
        /// <summary>
        /// <para>The type of the tracked node as specified in VR.VRNode.</para>
        /// </summary>
        public VRNode nodeType
        {
            get => 
                this.m_Type;
            set
            {
                this.m_Type = value;
            }
        }
        /// <summary>
        /// <para>
        /// Set to true if the node is presently being tracked by the underlying VRAR system, 
        /// and false if the node is not presently being tracked by the underlying VRAR system.</para>
        /// </summary>
        public bool tracked
        {
            get => 
                (this.m_Tracked == 1);
            set
            {
                this.m_Tracked = !value ? 0 : 1;
            }
        }
        /// <summary>
        /// <para>Sets the vector representing the current position of the tracked node.</para>
        /// </summary>
        public Vector3 position
        {
            set
            {
                this.m_Position = value;
                this.m_AvailableFields |= AvailableTrackingData.PositionAvailable;
            }
        }
        /// <summary>
        /// <para>Sets the quaternion representing the current rotation of the tracked node.</para>
        /// </summary>
        public Quaternion rotation
        {
            set
            {
                this.m_Rotation = value;
                this.m_AvailableFields |= AvailableTrackingData.RotationAvailable;
            }
        }
        /// <summary>
        /// <para>Sets the vector representing the current velocity of the tracked node.</para>
        /// </summary>
        public Vector3 velocity
        {
            set
            {
                this.m_Velocity = value;
                this.m_AvailableFields |= AvailableTrackingData.VelocityAvailable;
            }
        }
        /// <summary>
        /// <para>Sets the quaternion representing the current angular velocity of the tracked node.</para>
        /// </summary>
        public Quaternion angularVelocity
        {
            set
            {
                this.m_AngularVelocity = value;
                this.m_AvailableFields |= AvailableTrackingData.AngularVelocityAvailable;
            }
        }
        /// <summary>
        /// <para>Sets the vector representing the current acceleration of the tracked node.</para>
        /// </summary>
        public Vector3 acceleration
        {
            set
            {
                this.m_Acceleration = value;
                this.m_AvailableFields |= AvailableTrackingData.AccelerationAvailable;
            }
        }
        /// <summary>
        /// <para>Sets the quaternion representing the current angular acceleration of the tracked node.</para>
        /// </summary>
        public Quaternion angularAcceleration
        {
            set
            {
                this.m_AngularAcceleration = value;
                this.m_AvailableFields |= AvailableTrackingData.AngularAccelerationAvailable;
            }
        }
        public bool TryGetPosition(out Vector3 position) => 
            this.TryGet<Vector3>(this.m_Position, AvailableTrackingData.PositionAvailable, out position);

        public bool TryGetRotation(out Quaternion rotation) => 
            this.TryGet<Quaternion>(this.m_Rotation, AvailableTrackingData.RotationAvailable, out rotation);

        public bool TryGetVelocity(out Vector3 velocity) => 
            this.TryGet<Vector3>(this.m_Velocity, AvailableTrackingData.VelocityAvailable, out velocity);

        public bool TryGetAngularVelocity(out Quaternion angularVelocity) => 
            this.TryGet<Quaternion>(this.m_AngularVelocity, AvailableTrackingData.AngularVelocityAvailable, out angularVelocity);

        public bool TryGetAcceleration(out Vector3 acceleration) => 
            this.TryGet<Vector3>(this.m_Acceleration, AvailableTrackingData.AccelerationAvailable, out acceleration);

        public bool TryGetAngularAcceleration(out Quaternion angularAcceleration) => 
            this.TryGet<Quaternion>(this.m_AngularAcceleration, AvailableTrackingData.AngularAccelerationAvailable, out angularAcceleration);

        private bool TryGet<T>(T inValue, AvailableTrackingData availabilityFlag, out T outValue) where T: new()
        {
            if ((this.m_Tracked == 1) && ((this.m_AvailableFields & availabilityFlag) > AvailableTrackingData.None))
            {
                outValue = inValue;
                return true;
            }
            outValue = Activator.CreateInstance<T>();
            return false;
        }
    }
}

