namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Retargetable humanoid pose.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HumanPose
    {
        /// <summary>
        /// <para>The human body position for that pose.</para>
        /// </summary>
        public Vector3 bodyPosition;
        /// <summary>
        /// <para>The human body orientation for that pose.</para>
        /// </summary>
        public Quaternion bodyRotation;
        /// <summary>
        /// <para>The array of muscle values for that pose.</para>
        /// </summary>
        public float[] muscles;
        internal void Init()
        {
            if ((this.muscles != null) && (this.muscles.Length != HumanTrait.MuscleCount))
            {
                throw new ArgumentException("Bad array size for HumanPose.muscles. Size must equal HumanTrait.MuscleCount");
            }
            if (this.muscles == null)
            {
                this.muscles = new float[HumanTrait.MuscleCount];
                if (((this.bodyRotation.x == 0f) && (this.bodyRotation.y == 0f)) && ((this.bodyRotation.z == 0f) && (this.bodyRotation.w == 0f)))
                {
                    this.bodyRotation.w = 1f;
                }
            }
        }
    }
}

