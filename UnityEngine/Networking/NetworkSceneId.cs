namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>This is used to identify networked objects in a scene. These values are allocated in the editor and are persistent for the lifetime of the object in the scene.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct NetworkSceneId
    {
        [SerializeField]
        private uint m_Value;
        public NetworkSceneId(uint value)
        {
            this.m_Value = value;
        }

        /// <summary>
        /// <para>Returns true if the value is zero. Non-scene objects - ones which are spawned at runtime will have a sceneId of zero.</para>
        /// </summary>
        /// <returns>
        /// <para>True if zero.</para>
        /// </returns>
        public bool IsEmpty()
        {
            return (this.m_Value == 0);
        }

        public override int GetHashCode()
        {
            return (int) this.m_Value;
        }

        public override bool Equals(object obj)
        {
            return ((obj is NetworkSceneId) && (this == ((NetworkSceneId) obj)));
        }

        public static bool operator ==(NetworkSceneId c1, NetworkSceneId c2)
        {
            return (c1.m_Value == c2.m_Value);
        }

        public static bool operator !=(NetworkSceneId c1, NetworkSceneId c2)
        {
            return (c1.m_Value != c2.m_Value);
        }

        /// <summary>
        /// <para>Returns a string like SceneId:value.</para>
        /// </summary>
        /// <returns>
        /// <para>String representation of this object.</para>
        /// </returns>
        public override string ToString()
        {
            return this.m_Value.ToString();
        }

        /// <summary>
        /// <para>The internal value for this object.</para>
        /// </summary>
        public uint Value
        {
            get
            {
                return this.m_Value;
            }
        }
    }
}

