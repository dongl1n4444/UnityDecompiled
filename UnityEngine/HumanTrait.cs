namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Details of all the human bone and muscle types defined by Mecanim.</para>
    /// </summary>
    public sealed class HumanTrait
    {
        /// <summary>
        /// <para>Return the bone to which a particular muscle is connected.</para>
        /// </summary>
        /// <param name="i">Muscle index.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern int BoneFromMuscle(int i);
        /// <summary>
        /// <para>Get the default maximum value of rotation for a muscle in degrees.</para>
        /// </summary>
        /// <param name="i">Muscle index.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern float GetMuscleDefaultMax(int i);
        /// <summary>
        /// <para>Get the default minimum value of rotation for a muscle in degrees.</para>
        /// </summary>
        /// <param name="i">Muscle index.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern float GetMuscleDefaultMin(int i);
        /// <summary>
        /// <para>Returns parent humanoid bone index of a bone.</para>
        /// </summary>
        /// <param name="i">Humanoid bone index to get parent from.</param>
        /// <returns>
        /// <para>Humanoid bone index of parent.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern int GetParentBone(int i);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool HasCollider(Avatar avatar, int i);
        /// <summary>
        /// <para>Obtain the muscle index for a particular bone index and "degree of freedom".</para>
        /// </summary>
        /// <param name="i">Bone index.</param>
        /// <param name="dofIndex">Number representing a "degree of freedom": 0 for X-Axis, 1 for Y-Axis, 2 for Z-Axis.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern int MuscleFromBone(int i, int dofIndex);
        /// <summary>
        /// <para>Is the bone a member of the minimal set of bones that Mecanim requires for a human model?</para>
        /// </summary>
        /// <param name="i">Index of the bone to test.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool RequiredBone(int i);

        /// <summary>
        /// <para>The number of human bone types defined by Mecanim.</para>
        /// </summary>
        public static int BoneCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Array of the names of all human bone types defined by Mecanim.</para>
        /// </summary>
        public static string[] BoneName { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The number of human muscle types defined by Mecanim.</para>
        /// </summary>
        public static int MuscleCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Array of the names of all human muscle types defined by Mecanim.</para>
        /// </summary>
        public static string[] MuscleName { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The number of bone types that are required by Mecanim for any human model.</para>
        /// </summary>
        public static int RequiredBoneCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

