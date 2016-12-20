namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Details of the Transform name mapped to a model's skeleton bone and its default position and rotation in the T-pose.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct SkeletonBone
    {
        /// <summary>
        /// <para>The name of the Transform mapped to the bone.</para>
        /// </summary>
        public string name;
        internal string parentName;
        /// <summary>
        /// <para>The T-pose position of the bone in local space.</para>
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// <para>The T-pose rotation of the bone in local space.</para>
        /// </summary>
        public Quaternion rotation;
        /// <summary>
        /// <para>The T-pose scaling of the bone in local space.</para>
        /// </summary>
        public Vector3 scale;
        [Obsolete("transformModified is no longer used and has been deprecated.", true)]
        public int transformModified
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }
    }
}

