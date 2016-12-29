namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Experimental.Director;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Interface to control the Mecanim animation system.</para>
    /// </summary>
    [UsedByNativeCode]
    public sealed class Animator : DirectorPlayer
    {
        /// <summary>
        /// <para>Apply the default Root Motion.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void ApplyBuiltinRootMotion();
        private void CheckIfInIKPass()
        {
            if (this.logWarnings && !this.CheckIfInIKPassInternal())
            {
                Debug.LogWarning("Setting and getting Body Position/Rotation, IK Goals, Lookat and BoneLocalRotation should only be done in OnAnimatorIK or OnStateIK");
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool CheckIfInIKPassInternal();
        internal static T[] ConvertStateMachineBehaviour<T>(ScriptableObject[] rawObjects) where T: StateMachineBehaviour
        {
            if (rawObjects == null)
            {
                return null;
            }
            T[] localArray2 = new T[rawObjects.Length];
            for (int i = 0; i < localArray2.Length; i++)
            {
                localArray2[i] = (T) rawObjects[i];
            }
            return localArray2;
        }

        [ExcludeFromDocs]
        public void CrossFade(int stateNameHash, float transitionDuration)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.CrossFade(stateNameHash, transitionDuration, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void CrossFade(string stateName, float transitionDuration)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.CrossFade(stateName, transitionDuration, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void CrossFade(int stateNameHash, float transitionDuration, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.CrossFade(stateNameHash, transitionDuration, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void CrossFade(string stateName, float transitionDuration, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.CrossFade(stateName, transitionDuration, layer, negativeInfinity);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.CrossFade.</para>
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="transitionDuration"></param>
        /// <param name="layer"></param>
        /// <param name="normalizedTime"></param>
        /// <param name="stateNameHash"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void CrossFade(int stateNameHash, float transitionDuration, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("float.NegativeInfinity")] float normalizedTime);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.CrossFade.</para>
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="transitionDuration"></param>
        /// <param name="layer"></param>
        /// <param name="normalizedTime"></param>
        /// <param name="stateNameHash"></param>
        public void CrossFade(string stateName, float transitionDuration, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("float.NegativeInfinity")] float normalizedTime)
        {
            this.CrossFade(StringToHash(stateName), transitionDuration, layer, normalizedTime);
        }

        [ExcludeFromDocs]
        public void CrossFadeInFixedTime(int stateNameHash, float transitionDuration)
        {
            float fixedTime = 0f;
            int layer = -1;
            this.CrossFadeInFixedTime(stateNameHash, transitionDuration, layer, fixedTime);
        }

        [ExcludeFromDocs]
        public void CrossFadeInFixedTime(string stateName, float transitionDuration)
        {
            float fixedTime = 0f;
            int layer = -1;
            this.CrossFadeInFixedTime(stateName, transitionDuration, layer, fixedTime);
        }

        [ExcludeFromDocs]
        public void CrossFadeInFixedTime(int stateNameHash, float transitionDuration, int layer)
        {
            float fixedTime = 0f;
            this.CrossFadeInFixedTime(stateNameHash, transitionDuration, layer, fixedTime);
        }

        [ExcludeFromDocs]
        public void CrossFadeInFixedTime(string stateName, float transitionDuration, int layer)
        {
            float fixedTime = 0f;
            this.CrossFadeInFixedTime(stateName, transitionDuration, layer, fixedTime);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.CrossFadeInFixedTime.</para>
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="transitionDuration"></param>
        /// <param name="layer"></param>
        /// <param name="fixedTime"></param>
        /// <param name="stateNameHash"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void CrossFadeInFixedTime(int stateNameHash, float transitionDuration, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("0.0f")] float fixedTime);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.CrossFadeInFixedTime.</para>
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="transitionDuration"></param>
        /// <param name="layer"></param>
        /// <param name="fixedTime"></param>
        /// <param name="stateNameHash"></param>
        public void CrossFadeInFixedTime(string stateName, float transitionDuration, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("0.0f")] float fixedTime)
        {
            this.CrossFadeInFixedTime(StringToHash(stateName), transitionDuration, layer, fixedTime);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void EvaluateController();
        [Obsolete("ForceStateNormalizedTime is deprecated. Please use Play or CrossFade instead.")]
        public void ForceStateNormalizedTime(float normalizedTime)
        {
            this.Play(0, 0, normalizedTime);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void GetAnimatorClipInfoInternal(int layerIndex, bool isCurrent, object clips);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetAnimatorTransitionInfo.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex);
        public T GetBehaviour<T>() where T: StateMachineBehaviour => 
            (this.GetBehaviour(typeof(T)) as T);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern ScriptableObject GetBehaviour(System.Type type);
        public T[] GetBehaviours<T>() where T: StateMachineBehaviour => 
            ConvertStateMachineBehaviour<T>(this.GetBehaviours(typeof(T)));

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern ScriptableObject[] GetBehaviours(System.Type type);
        internal Vector3 GetBodyPositionInternal()
        {
            Vector3 vector;
            INTERNAL_CALL_GetBodyPositionInternal(this, out vector);
            return vector;
        }

        internal Quaternion GetBodyRotationInternal()
        {
            Quaternion quaternion;
            INTERNAL_CALL_GetBodyRotationInternal(this, out quaternion);
            return quaternion;
        }

        /// <summary>
        /// <para>Returns transform mapped to this human bone id.</para>
        /// </summary>
        /// <param name="humanBoneId">The human bone that is queried, see enum HumanBodyBones for a list of possible values.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern Transform GetBoneTransform(HumanBodyBones humanBoneId);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetBool.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public bool GetBool(int id) => 
            this.GetBoolID(id);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetBool.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public bool GetBool(string name) => 
            this.GetBoolString(name);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool GetBoolID(int id);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool GetBoolString(string name);
        /// <summary>
        /// <para>Gets the list of AnimatorClipInfo currently played by the current state.</para>
        /// </summary>
        /// <param name="layerIndex">The layer's index.</param>
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("GetCurrentAnimationClipState is obsolete. Use GetCurrentAnimatorClipInfo instead (UnityUpgradable) -> GetCurrentAnimatorClipInfo(*)", true)]
        public AnimationInfo[] GetCurrentAnimationClipState(int layerIndex) => 
            null;

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetCurrentAnimatorClipInfo.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern AnimatorClipInfo[] GetCurrentAnimatorClipInfo(int layerIndex);
        public void GetCurrentAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips)
        {
            if (clips == null)
            {
                throw new ArgumentNullException("clips");
            }
            this.GetAnimatorClipInfoInternal(layerIndex, true, clips);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetCurrentAnimatorClipInfoCount.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int GetCurrentAnimatorClipInfoCount(int layerIndex);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetCurrentAnimatorStateInfo.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern string GetCurrentStateName(int layerIndex);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetFloat.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public float GetFloat(int id) => 
            this.GetFloatID(id);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetFloat.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public float GetFloat(string name) => 
            this.GetFloatString(name);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern float GetFloatID(int id);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern float GetFloatString(string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern float GetHintWeightPositionInternal(AvatarIKHint hint);
        /// <summary>
        /// <para>Gets the position of an IK hint.</para>
        /// </summary>
        /// <param name="hint">The AvatarIKHint that is queried.</param>
        /// <returns>
        /// <para>Return the current position of this IK hint in world space.</para>
        /// </returns>
        public Vector3 GetIKHintPosition(AvatarIKHint hint)
        {
            this.CheckIfInIKPass();
            return this.GetIKHintPositionInternal(hint);
        }

        internal Vector3 GetIKHintPositionInternal(AvatarIKHint hint)
        {
            Vector3 vector;
            INTERNAL_CALL_GetIKHintPositionInternal(this, hint, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Gets the translative weight of an IK Hint (0 = at the original animation before IK, 1 = at the hint).</para>
        /// </summary>
        /// <param name="hint">The AvatarIKHint that is queried.</param>
        /// <returns>
        /// <para>Return translative weight.</para>
        /// </returns>
        public float GetIKHintPositionWeight(AvatarIKHint hint)
        {
            this.CheckIfInIKPass();
            return this.GetHintWeightPositionInternal(hint);
        }

        /// <summary>
        /// <para>Gets the position of an IK goal.</para>
        /// </summary>
        /// <param name="goal">The AvatarIKGoal that is queried.</param>
        /// <returns>
        /// <para>Return the current position of this IK goal in world space.</para>
        /// </returns>
        public Vector3 GetIKPosition(AvatarIKGoal goal)
        {
            this.CheckIfInIKPass();
            return this.GetIKPositionInternal(goal);
        }

        internal Vector3 GetIKPositionInternal(AvatarIKGoal goal)
        {
            Vector3 vector;
            INTERNAL_CALL_GetIKPositionInternal(this, goal, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Gets the translative weight of an IK goal (0 = at the original animation before IK, 1 = at the goal).</para>
        /// </summary>
        /// <param name="goal">The AvatarIKGoal that is queried.</param>
        public float GetIKPositionWeight(AvatarIKGoal goal)
        {
            this.CheckIfInIKPass();
            return this.GetIKPositionWeightInternal(goal);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern float GetIKPositionWeightInternal(AvatarIKGoal goal);
        /// <summary>
        /// <para>Gets the rotation of an IK goal.</para>
        /// </summary>
        /// <param name="goal">The AvatarIKGoal that is is queried.</param>
        public Quaternion GetIKRotation(AvatarIKGoal goal)
        {
            this.CheckIfInIKPass();
            return this.GetIKRotationInternal(goal);
        }

        internal Quaternion GetIKRotationInternal(AvatarIKGoal goal)
        {
            Quaternion quaternion;
            INTERNAL_CALL_GetIKRotationInternal(this, goal, out quaternion);
            return quaternion;
        }

        /// <summary>
        /// <para>Gets the rotational weight of an IK goal (0 = rotation before IK, 1 = rotation at the IK goal).</para>
        /// </summary>
        /// <param name="goal">The AvatarIKGoal that is queried.</param>
        public float GetIKRotationWeight(AvatarIKGoal goal)
        {
            this.CheckIfInIKPass();
            return this.GetIKRotationWeightInternal(goal);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern float GetIKRotationWeightInternal(AvatarIKGoal goal);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetInteger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public int GetInteger(int id) => 
            this.GetIntegerID(id);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetInteger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public int GetInteger(string name) => 
            this.GetIntegerString(name);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int GetIntegerID(int id);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int GetIntegerString(string name);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetLayerIndex.</para>
        /// </summary>
        /// <param name="layerName"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int GetLayerIndex(string layerName);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetLayerName.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string GetLayerName(int layerIndex);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetLayerWeight.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern float GetLayerWeight(int layerIndex);
        /// <summary>
        /// <para>Gets the list of AnimatorClipInfo currently played by the next state.</para>
        /// </summary>
        /// <param name="layerIndex">The layer's index.</param>
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("GetNextAnimationClipState is obsolete. Use GetNextAnimatorClipInfo instead (UnityUpgradable) -> GetNextAnimatorClipInfo(*)", true)]
        public AnimationInfo[] GetNextAnimationClipState(int layerIndex) => 
            null;

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetNextAnimatorClipInfo.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern AnimatorClipInfo[] GetNextAnimatorClipInfo(int layerIndex);
        public void GetNextAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips)
        {
            if (clips == null)
            {
                throw new ArgumentNullException("clips");
            }
            this.GetAnimatorClipInfoInternal(layerIndex, false, clips);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetNextAnimatorClipInfoCount.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int GetNextAnimatorClipInfoCount(int layerIndex);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetNextAnimatorStateInfo.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern string GetNextStateName(int layerIndex);
        /// <summary>
        /// <para>See AnimatorController.GetParameter.</para>
        /// </summary>
        /// <param name="index"></param>
        public AnimatorControllerParameter GetParameter(int index)
        {
            AnimatorControllerParameter[] parameters = this.parameters;
            if ((index < 0) && (index >= this.parameters.Length))
            {
                throw new IndexOutOfRangeException("Index must be between 0 and " + this.parameters.Length);
            }
            return parameters[index];
        }

        /// <summary>
        /// <para>Gets the value of a quaternion parameter.</para>
        /// </summary>
        /// <param name="id">The id of the parameter. The id is generated using Animator::StringToHash.</param>
        [Obsolete("GetQuaternion is deprecated.")]
        public Quaternion GetQuaternion(int id) => 
            Quaternion.identity;

        /// <summary>
        /// <para>Gets the value of a quaternion parameter.</para>
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        [Obsolete("GetQuaternion is deprecated.")]
        public Quaternion GetQuaternion(string name) => 
            Quaternion.identity;

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern string GetStats();
        /// <summary>
        /// <para>Gets the value of a vector parameter.</para>
        /// </summary>
        /// <param name="id">The id of the parameter. The id is generated using Animator::StringToHash.</param>
        [Obsolete("GetVector is deprecated.")]
        public Vector3 GetVector(int id) => 
            Vector3.zero;

        /// <summary>
        /// <para>Gets the value of a vector parameter.</para>
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        [Obsolete("GetVector is deprecated.")]
        public Vector3 GetVector(string name) => 
            Vector3.zero;

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.HasState.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        /// <param name="stateID"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool HasState(int layerIndex, int stateID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetBodyPositionInternal(Animator self, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetBodyRotationInternal(Animator self, out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetIKHintPositionInternal(Animator self, AvatarIKHint hint, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetIKPositionInternal(Animator self, AvatarIKGoal goal, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetIKRotationInternal(Animator self, AvatarIKGoal goal, out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_MatchTarget(Animator self, ref Vector3 matchPosition, ref Quaternion matchRotation, AvatarTarget targetBodyPart, ref MatchTargetWeightMask weightMask, float startNormalizedTime, float targetNormalizedTime);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetBodyPositionInternal(Animator self, ref Vector3 bodyPosition);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetBodyRotationInternal(Animator self, ref Quaternion bodyRotation);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetBoneLocalRotationInternal(Animator self, HumanBodyBones humanBoneId, ref Quaternion rotation);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetIKHintPositionInternal(Animator self, AvatarIKHint hint, ref Vector3 hintPosition);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetIKPositionInternal(Animator self, AvatarIKGoal goal, ref Vector3 goalPosition);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetIKRotationInternal(Animator self, AvatarIKGoal goal, ref Quaternion goalRotation);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetLookAtPositionInternal(Animator self, ref Vector3 lookAtPosition);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_angularVelocity(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_deltaPosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_deltaRotation(out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_pivotPosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_rootPosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_rootRotation(out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_targetPosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_targetRotation(out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_velocity(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_rootPosition(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_rootRotation(ref Quaternion value);
        [ExcludeFromDocs]
        public void InterruptMatchTarget()
        {
            bool completeMatch = true;
            this.InterruptMatchTarget(completeMatch);
        }

        /// <summary>
        /// <para>Interrupts the automatic target matching.</para>
        /// </summary>
        /// <param name="completeMatch"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void InterruptMatchTarget([UnityEngine.Internal.DefaultValue("true")] bool completeMatch);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool IsBoneTransform(Transform transform);
        /// <summary>
        /// <para>Returns true if the transform is controlled by the Animator\.</para>
        /// </summary>
        /// <param name="transform">The transform that is queried.</param>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("use mask and layers to control subset of transfroms in a skeleton", true)]
        public extern bool IsControlled(Transform transform);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.IsInTransition.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool IsInTransition(int layerIndex);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.IsParameterControlledByCurve.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public bool IsParameterControlledByCurve(int id) => 
            this.IsParameterControlledByCurveID(id);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.IsParameterControlledByCurve.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public bool IsParameterControlledByCurve(string name) => 
            this.IsParameterControlledByCurveString(name);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool IsParameterControlledByCurveID(int id);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool IsParameterControlledByCurveString(string name);
        [ExcludeFromDocs]
        public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime)
        {
            float targetNormalizedTime = 1f;
            INTERNAL_CALL_MatchTarget(this, ref matchPosition, ref matchRotation, targetBodyPart, ref weightMask, startNormalizedTime, targetNormalizedTime);
        }

        /// <summary>
        /// <para>Automatically adjust the gameobject position and rotation so that the AvatarTarget reaches the matchPosition when the current state is at the specified progress.</para>
        /// </summary>
        /// <param name="matchPosition">The position we want the body part to reach.</param>
        /// <param name="matchRotation">The rotation in which we want the body part to be.</param>
        /// <param name="targetBodyPart">The body part that is involved in the match.</param>
        /// <param name="weightMask">Structure that contains weights for matching position and rotation.</param>
        /// <param name="startNormalizedTime">Start time within the animation clip (0 - beginning of clip, 1 - end of clip).</param>
        /// <param name="targetNormalizedTime">End time within the animation clip (0 - beginning of clip, 1 - end of clip), values greater than 1 can be set to trigger a match after a certain number of loops. Ex: 2.3 means at 30% of 2nd loop.</param>
        public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime, [UnityEngine.Internal.DefaultValue("1")] float targetNormalizedTime)
        {
            INTERNAL_CALL_MatchTarget(this, ref matchPosition, ref matchRotation, targetBodyPart, ref weightMask, startNormalizedTime, targetNormalizedTime);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void OnCullingModeChanged();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void OnUpdateModeChanged();
        [ExcludeFromDocs]
        public void Play(int stateNameHash)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.Play(stateNameHash, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void Play(string stateName)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.Play(stateName, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void Play(int stateNameHash, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.Play(stateNameHash, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void Play(string stateName, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.Play(stateName, layer, negativeInfinity);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.Play.</para>
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="layer"></param>
        /// <param name="normalizedTime"></param>
        /// <param name="stateNameHash"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Play(int stateNameHash, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("float.NegativeInfinity")] float normalizedTime);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.Play.</para>
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="layer"></param>
        /// <param name="normalizedTime"></param>
        /// <param name="stateNameHash"></param>
        public void Play(string stateName, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("float.NegativeInfinity")] float normalizedTime)
        {
            this.Play(StringToHash(stateName), layer, normalizedTime);
        }

        [ExcludeFromDocs]
        public void PlayInFixedTime(int stateNameHash)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.PlayInFixedTime(stateNameHash, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void PlayInFixedTime(string stateName)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.PlayInFixedTime(stateName, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void PlayInFixedTime(int stateNameHash, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.PlayInFixedTime(stateNameHash, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void PlayInFixedTime(string stateName, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.PlayInFixedTime(stateName, layer, negativeInfinity);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.PlayInFixedTime.</para>
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="layer"></param>
        /// <param name="fixedTime"></param>
        /// <param name="stateNameHash"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void PlayInFixedTime(int stateNameHash, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("float.NegativeInfinity")] float fixedTime);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.PlayInFixedTime.</para>
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="layer"></param>
        /// <param name="fixedTime"></param>
        /// <param name="stateNameHash"></param>
        public void PlayInFixedTime(string stateName, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("float.NegativeInfinity")] float fixedTime)
        {
            this.PlayInFixedTime(StringToHash(stateName), layer, fixedTime);
        }

        /// <summary>
        /// <para>Rebind all the animated properties and mesh data with the Animator.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Rebind();
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.ResetTrigger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public void ResetTrigger(int id)
        {
            this.ResetTriggerID(id);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.ResetTrigger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public void ResetTrigger(string name)
        {
            this.ResetTriggerString(name);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void ResetTriggerID(int id);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void ResetTriggerString(string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern string ResolveHash(int hash);
        internal void SetBodyPositionInternal(Vector3 bodyPosition)
        {
            INTERNAL_CALL_SetBodyPositionInternal(this, ref bodyPosition);
        }

        internal void SetBodyRotationInternal(Quaternion bodyRotation)
        {
            INTERNAL_CALL_SetBodyRotationInternal(this, ref bodyRotation);
        }

        /// <summary>
        /// <para>Sets local rotation of a human bone during a IK pass.</para>
        /// </summary>
        /// <param name="humanBoneId">The human bone Id.</param>
        /// <param name="rotation">The local rotation.</param>
        public void SetBoneLocalRotation(HumanBodyBones humanBoneId, Quaternion rotation)
        {
            this.CheckIfInIKPass();
            this.SetBoneLocalRotationInternal(humanBoneId, rotation);
        }

        internal void SetBoneLocalRotationInternal(HumanBodyBones humanBoneId, Quaternion rotation)
        {
            INTERNAL_CALL_SetBoneLocalRotationInternal(this, humanBoneId, ref rotation);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetBool.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        public void SetBool(int id, bool value)
        {
            this.SetBoolID(id, value);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetBool.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        public void SetBool(string name, bool value)
        {
            this.SetBoolString(name, value);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetBoolID(int id, bool value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetBoolString(string name, bool value);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetFloat.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="dampTime"></param>
        /// <param name="deltaTime"></param>
        /// <param name="id"></param>
        public void SetFloat(int id, float value)
        {
            this.SetFloatID(id, value);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetFloat.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="dampTime"></param>
        /// <param name="deltaTime"></param>
        /// <param name="id"></param>
        public void SetFloat(string name, float value)
        {
            this.SetFloatString(name, value);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetFloat.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="dampTime"></param>
        /// <param name="deltaTime"></param>
        /// <param name="id"></param>
        public void SetFloat(int id, float value, float dampTime, float deltaTime)
        {
            this.SetFloatIDDamp(id, value, dampTime, deltaTime);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetFloat.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="dampTime"></param>
        /// <param name="deltaTime"></param>
        /// <param name="id"></param>
        public void SetFloat(string name, float value, float dampTime, float deltaTime)
        {
            this.SetFloatStringDamp(name, value, dampTime, deltaTime);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetFloatID(int id, float value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetFloatIDDamp(int id, float value, float dampTime, float deltaTime);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetFloatString(string name, float value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetFloatStringDamp(string name, float value, float dampTime, float deltaTime);
        /// <summary>
        /// <para>Sets the position of an IK hint.</para>
        /// </summary>
        /// <param name="hint">The AvatarIKHint that is set.</param>
        /// <param name="hintPosition">The position in world space.</param>
        public void SetIKHintPosition(AvatarIKHint hint, Vector3 hintPosition)
        {
            this.CheckIfInIKPass();
            this.SetIKHintPositionInternal(hint, hintPosition);
        }

        internal void SetIKHintPositionInternal(AvatarIKHint hint, Vector3 hintPosition)
        {
            INTERNAL_CALL_SetIKHintPositionInternal(this, hint, ref hintPosition);
        }

        /// <summary>
        /// <para>Sets the translative weight of an IK hint (0 = at the original animation before IK, 1 = at the hint).</para>
        /// </summary>
        /// <param name="hint">The AvatarIKHint that is set.</param>
        /// <param name="value">The translative weight.</param>
        public void SetIKHintPositionWeight(AvatarIKHint hint, float value)
        {
            this.CheckIfInIKPass();
            this.SetIKHintPositionWeightInternal(hint, value);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void SetIKHintPositionWeightInternal(AvatarIKHint hint, float value);
        /// <summary>
        /// <para>Sets the position of an IK goal.</para>
        /// </summary>
        /// <param name="goal">The AvatarIKGoal that is set.</param>
        /// <param name="goalPosition">The position in world space.</param>
        public void SetIKPosition(AvatarIKGoal goal, Vector3 goalPosition)
        {
            this.CheckIfInIKPass();
            this.SetIKPositionInternal(goal, goalPosition);
        }

        internal void SetIKPositionInternal(AvatarIKGoal goal, Vector3 goalPosition)
        {
            INTERNAL_CALL_SetIKPositionInternal(this, goal, ref goalPosition);
        }

        /// <summary>
        /// <para>Sets the translative weight of an IK goal (0 = at the original animation before IK, 1 = at the goal).</para>
        /// </summary>
        /// <param name="goal">The AvatarIKGoal that is set.</param>
        /// <param name="value">The translative weight.</param>
        public void SetIKPositionWeight(AvatarIKGoal goal, float value)
        {
            this.CheckIfInIKPass();
            this.SetIKPositionWeightInternal(goal, value);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void SetIKPositionWeightInternal(AvatarIKGoal goal, float value);
        /// <summary>
        /// <para>Sets the rotation of an IK goal.</para>
        /// </summary>
        /// <param name="goal">The AvatarIKGoal that is set.</param>
        /// <param name="goalRotation">The rotation in world space.</param>
        public void SetIKRotation(AvatarIKGoal goal, Quaternion goalRotation)
        {
            this.CheckIfInIKPass();
            this.SetIKRotationInternal(goal, goalRotation);
        }

        internal void SetIKRotationInternal(AvatarIKGoal goal, Quaternion goalRotation)
        {
            INTERNAL_CALL_SetIKRotationInternal(this, goal, ref goalRotation);
        }

        /// <summary>
        /// <para>Sets the rotational weight of an IK goal (0 = rotation before IK, 1 = rotation at the IK goal).</para>
        /// </summary>
        /// <param name="goal">The AvatarIKGoal that is set.</param>
        /// <param name="value">The rotational weight.</param>
        public void SetIKRotationWeight(AvatarIKGoal goal, float value)
        {
            this.CheckIfInIKPass();
            this.SetIKRotationWeightInternal(goal, value);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void SetIKRotationWeightInternal(AvatarIKGoal goal, float value);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetInteger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        public void SetInteger(int id, int value)
        {
            this.SetIntegerID(id, value);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetInteger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        public void SetInteger(string name, int value)
        {
            this.SetIntegerString(name, value);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetIntegerID(int id, int value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetIntegerString(string name, int value);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetLayerWeight.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        /// <param name="weight"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetLayerWeight(int layerIndex, float weight);
        /// <summary>
        /// <para>Sets the look at position.</para>
        /// </summary>
        /// <param name="lookAtPosition">The position to lookAt.</param>
        public void SetLookAtPosition(Vector3 lookAtPosition)
        {
            this.CheckIfInIKPass();
            this.SetLookAtPositionInternal(lookAtPosition);
        }

        internal void SetLookAtPositionInternal(Vector3 lookAtPosition)
        {
            INTERNAL_CALL_SetLookAtPositionInternal(this, ref lookAtPosition);
        }

        /// <summary>
        /// <para>Set look at weights.</para>
        /// </summary>
        /// <param name="weight">(0-1) the global weight of the LookAt, multiplier for other parameters.</param>
        /// <param name="bodyWeight">(0-1) determines how much the body is involved in the LookAt.</param>
        /// <param name="headWeight">(0-1) determines how much the head is involved in the LookAt.</param>
        /// <param name="eyesWeight">(0-1) determines how much the eyes are involved in the LookAt.</param>
        /// <param name="clampWeight">(0-1) 0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).</param>
        [ExcludeFromDocs]
        public void SetLookAtWeight(float weight)
        {
            float clampWeight = 0.5f;
            float eyesWeight = 0f;
            float headWeight = 1f;
            float bodyWeight = 0f;
            this.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
        }

        /// <summary>
        /// <para>Set look at weights.</para>
        /// </summary>
        /// <param name="weight">(0-1) the global weight of the LookAt, multiplier for other parameters.</param>
        /// <param name="bodyWeight">(0-1) determines how much the body is involved in the LookAt.</param>
        /// <param name="headWeight">(0-1) determines how much the head is involved in the LookAt.</param>
        /// <param name="eyesWeight">(0-1) determines how much the eyes are involved in the LookAt.</param>
        /// <param name="clampWeight">(0-1) 0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).</param>
        [ExcludeFromDocs]
        public void SetLookAtWeight(float weight, float bodyWeight)
        {
            float clampWeight = 0.5f;
            float eyesWeight = 0f;
            float headWeight = 1f;
            this.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
        }

        /// <summary>
        /// <para>Set look at weights.</para>
        /// </summary>
        /// <param name="weight">(0-1) the global weight of the LookAt, multiplier for other parameters.</param>
        /// <param name="bodyWeight">(0-1) determines how much the body is involved in the LookAt.</param>
        /// <param name="headWeight">(0-1) determines how much the head is involved in the LookAt.</param>
        /// <param name="eyesWeight">(0-1) determines how much the eyes are involved in the LookAt.</param>
        /// <param name="clampWeight">(0-1) 0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).</param>
        [ExcludeFromDocs]
        public void SetLookAtWeight(float weight, float bodyWeight, float headWeight)
        {
            float clampWeight = 0.5f;
            float eyesWeight = 0f;
            this.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
        }

        /// <summary>
        /// <para>Set look at weights.</para>
        /// </summary>
        /// <param name="weight">(0-1) the global weight of the LookAt, multiplier for other parameters.</param>
        /// <param name="bodyWeight">(0-1) determines how much the body is involved in the LookAt.</param>
        /// <param name="headWeight">(0-1) determines how much the head is involved in the LookAt.</param>
        /// <param name="eyesWeight">(0-1) determines how much the eyes are involved in the LookAt.</param>
        /// <param name="clampWeight">(0-1) 0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).</param>
        [ExcludeFromDocs]
        public void SetLookAtWeight(float weight, float bodyWeight, float headWeight, float eyesWeight)
        {
            float clampWeight = 0.5f;
            this.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
        }

        /// <summary>
        /// <para>Set look at weights.</para>
        /// </summary>
        /// <param name="weight">(0-1) the global weight of the LookAt, multiplier for other parameters.</param>
        /// <param name="bodyWeight">(0-1) determines how much the body is involved in the LookAt.</param>
        /// <param name="headWeight">(0-1) determines how much the head is involved in the LookAt.</param>
        /// <param name="eyesWeight">(0-1) determines how much the eyes are involved in the LookAt.</param>
        /// <param name="clampWeight">(0-1) 0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).</param>
        public void SetLookAtWeight(float weight, [UnityEngine.Internal.DefaultValue("0.00f")] float bodyWeight, [UnityEngine.Internal.DefaultValue("1.00f")] float headWeight, [UnityEngine.Internal.DefaultValue("0.00f")] float eyesWeight, [UnityEngine.Internal.DefaultValue("0.50f")] float clampWeight)
        {
            this.CheckIfInIKPass();
            this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
        }

        [ExcludeFromDocs]
        internal void SetLookAtWeightInternal(float weight)
        {
            float clampWeight = 0.5f;
            float eyesWeight = 0f;
            float headWeight = 1f;
            float bodyWeight = 0f;
            this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
        }

        [ExcludeFromDocs]
        internal void SetLookAtWeightInternal(float weight, float bodyWeight)
        {
            float clampWeight = 0.5f;
            float eyesWeight = 0f;
            float headWeight = 1f;
            this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
        }

        [ExcludeFromDocs]
        internal void SetLookAtWeightInternal(float weight, float bodyWeight, float headWeight)
        {
            float clampWeight = 0.5f;
            float eyesWeight = 0f;
            this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
        }

        [ExcludeFromDocs]
        internal void SetLookAtWeightInternal(float weight, float bodyWeight, float headWeight, float eyesWeight)
        {
            float clampWeight = 0.5f;
            this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void SetLookAtWeightInternal(float weight, [UnityEngine.Internal.DefaultValue("0.00f")] float bodyWeight, [UnityEngine.Internal.DefaultValue("1.00f")] float headWeight, [UnityEngine.Internal.DefaultValue("0.00f")] float eyesWeight, [UnityEngine.Internal.DefaultValue("0.50f")] float clampWeight);
        /// <summary>
        /// <para>Sets the value of a quaternion parameter.</para>
        /// </summary>
        /// <param name="id">Of the parameter. The id is generated using Animator::StringToHash.</param>
        /// <param name="value">The new value for the parameter.</param>
        [Obsolete("SetQuaternion is deprecated.")]
        public void SetQuaternion(int id, Quaternion value)
        {
        }

        /// <summary>
        /// <para>Sets the value of a quaternion parameter.</para>
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The new value for the parameter.</param>
        [Obsolete("SetQuaternion is deprecated.")]
        public void SetQuaternion(string name, Quaternion value)
        {
        }

        /// <summary>
        /// <para>Sets an AvatarTarget and a targetNormalizedTime for the current state.</para>
        /// </summary>
        /// <param name="targetIndex">The avatar body part that is queried.</param>
        /// <param name="targetNormalizedTime">The current state Time that is queried.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetTarget(AvatarTarget targetIndex, float targetNormalizedTime);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetTrigger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public void SetTrigger(int id)
        {
            this.SetTriggerID(id);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetTrigger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public void SetTrigger(string name)
        {
            this.SetTriggerString(name);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetTriggerID(int id);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetTriggerString(string name);
        /// <summary>
        /// <para>Sets the value of a vector parameter.</para>
        /// </summary>
        /// <param name="id">The id of the parameter. The id is generated using Animator::StringToHash.</param>
        /// <param name="value">The new value for the parameter.</param>
        [Obsolete("SetVector is deprecated.")]
        public void SetVector(int id, Vector3 value)
        {
        }

        /// <summary>
        /// <para>Sets the value of a vector parameter.</para>
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The new value for the parameter.</param>
        [Obsolete("SetVector is deprecated.")]
        public void SetVector(string name, Vector3 value)
        {
        }

        /// <summary>
        /// <para>Sets the animator in playback mode.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void StartPlayback();
        /// <summary>
        /// <para>Sets the animator in recording mode, and allocates a circular buffer of size frameCount.</para>
        /// </summary>
        /// <param name="frameCount">The number of frames (updates) that will be recorded. If frameCount is 0, the recording will continue until the user calls StopRecording. The maximum value for frameCount is 10000.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void StartRecording(int frameCount);
        /// <summary>
        /// <para>Stops the animator playback mode. When playback stops, the avatar resumes getting control from game logic.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void StopPlayback();
        /// <summary>
        /// <para>Stops animator record mode.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void StopRecording();
        /// <summary>
        /// <para>Generates an parameter id from a string.</para>
        /// </summary>
        /// <param name="name">The string to convert to Id.</param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern int StringToHash(string name);
        /// <summary>
        /// <para>Evaluates the animator based on deltaTime.</para>
        /// </summary>
        /// <param name="deltaTime">The time delta.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Update(float deltaTime);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void WriteDefaultPose();

        internal bool allowConstantClipSamplingOptimization { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Gets the avatar angular velocity for the last evaluated frame.</para>
        /// </summary>
        public Vector3 angularVelocity
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_angularVelocity(out vector);
                return vector;
            }
        }

        /// <summary>
        /// <para>When turned on, animations will be executed in the physics loop. This is only useful in conjunction with kinematic rigidbodies.</para>
        /// </summary>
        [Obsolete("Use Animator.updateMode instead")]
        public bool animatePhysics
        {
            get => 
                (this.updateMode == AnimatorUpdateMode.AnimatePhysics);
            set
            {
                this.updateMode = !value ? AnimatorUpdateMode.Normal : AnimatorUpdateMode.AnimatePhysics;
            }
        }

        /// <summary>
        /// <para>Should root motion be applied?</para>
        /// </summary>
        public bool applyRootMotion { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Gets/Sets the current Avatar.</para>
        /// </summary>
        public Avatar avatar { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal Transform avatarRoot { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The position of the body center of mass.</para>
        /// </summary>
        public Vector3 bodyPosition
        {
            get
            {
                this.CheckIfInIKPass();
                return this.GetBodyPositionInternal();
            }
            set
            {
                this.CheckIfInIKPass();
                this.SetBodyPositionInternal(value);
            }
        }

        /// <summary>
        /// <para>The rotation of the body center of mass.</para>
        /// </summary>
        public Quaternion bodyRotation
        {
            get
            {
                this.CheckIfInIKPass();
                return this.GetBodyRotationInternal();
            }
            set
            {
                this.CheckIfInIKPass();
                this.SetBodyRotationInternal(value);
            }
        }

        /// <summary>
        /// <para>Controls culling of this Animator component.</para>
        /// </summary>
        public AnimatorCullingMode cullingMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Gets the avatar delta position for the last evaluated frame.</para>
        /// </summary>
        public Vector3 deltaPosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_deltaPosition(out vector);
                return vector;
            }
        }

        /// <summary>
        /// <para>Gets the avatar delta rotation for the last evaluated frame.</para>
        /// </summary>
        public Quaternion deltaRotation
        {
            get
            {
                Quaternion quaternion;
                this.INTERNAL_get_deltaRotation(out quaternion);
                return quaternion;
            }
        }

        /// <summary>
        /// <para>Blends pivot point between body center of mass and feet pivot. At 0%, the blending point is body center of mass. At 100%, the blending point is feet pivot.</para>
        /// </summary>
        public float feetPivotActive { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public bool fireEvents { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The current gravity weight based on current animations that are played.</para>
        /// </summary>
        public float gravityWeight { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns true if the current rig has root motion.</para>
        /// </summary>
        public bool hasRootMotion { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns true if the object has a transform hierarchy.</para>
        /// </summary>
        public bool hasTransformHierarchy { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns the scale of the current Avatar for a humanoid rig, (1 by default if the rig is generic).</para>
        /// </summary>
        public float humanScale { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns true if the current rig is humanoid, false if it is generic.</para>
        /// </summary>
        public bool isHuman { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns whether the animator is initialized successfully.</para>
        /// </summary>
        public bool isInitialized { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>If automatic matching is active.</para>
        /// </summary>
        public bool isMatchingTarget { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns true if the current rig is optimizable with AnimatorUtility.OptimizeTransformHierarchy.</para>
        /// </summary>
        public bool isOptimizable { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal bool isRootPositionOrRotationControlledByCurves { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.layerCount.</para>
        /// </summary>
        public int layerCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Additional layers affects the center of mass.</para>
        /// </summary>
        public bool layersAffectMassCenter { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Get left foot bottom height.</para>
        /// </summary>
        public float leftFeetBottomHeight { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>When linearVelocityBlending is set to true, the root motion velocity and angular velocity will be blended linearly.</para>
        /// </summary>
        public bool linearVelocityBlending { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public bool logWarnings { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.parameterCount.</para>
        /// </summary>
        public int parameterCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Read only acces to the AnimatorControllerParameters used by the animator.</para>
        /// </summary>
        public AnimatorControllerParameter[] parameters { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Get the current position of the pivot.</para>
        /// </summary>
        public Vector3 pivotPosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_pivotPosition(out vector);
                return vector;
            }
        }

        /// <summary>
        /// <para>Gets the pivot weight.</para>
        /// </summary>
        public float pivotWeight { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Sets the playback position in the recording buffer.</para>
        /// </summary>
        public float playbackTime { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Gets the mode of the Animator recorder.</para>
        /// </summary>
        public AnimatorRecorderMode recorderMode { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Start time of the first frame of the buffer relative to the frame at which StartRecording was called.</para>
        /// </summary>
        public float recorderStartTime { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>End time of the recorded clip relative to when StartRecording was called.</para>
        /// </summary>
        public float recorderStopTime { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Get right foot bottom height.</para>
        /// </summary>
        public float rightFeetBottomHeight { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The root position, the position of the game object.</para>
        /// </summary>
        public Vector3 rootPosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_rootPosition(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_rootPosition(ref value);
            }
        }

        /// <summary>
        /// <para>The root rotation, the rotation of the game object.</para>
        /// </summary>
        public Quaternion rootRotation
        {
            get
            {
                Quaternion quaternion;
                this.INTERNAL_get_rootRotation(out quaternion);
                return quaternion;
            }
            set
            {
                this.INTERNAL_set_rootRotation(ref value);
            }
        }

        /// <summary>
        /// <para>The runtime representation of AnimatorController that controls the Animator.</para>
        /// </summary>
        public RuntimeAnimatorController runtimeAnimatorController { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The playback speed of the Animator. 1 is normal playback speed.</para>
        /// </summary>
        public float speed { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Automatic stabilization of feet during transition and blending.</para>
        /// </summary>
        public bool stabilizeFeet { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal bool supportsOnAnimatorMove { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns the position of the target specified by SetTarget(AvatarTarget targetIndex, float targetNormalizedTime)).</para>
        /// </summary>
        public Vector3 targetPosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_targetPosition(out vector);
                return vector;
            }
        }

        /// <summary>
        /// <para>Returns the rotation of the target specified by SetTarget(AvatarTarget targetIndex, float targetNormalizedTime)).</para>
        /// </summary>
        public Quaternion targetRotation
        {
            get
            {
                Quaternion quaternion;
                this.INTERNAL_get_targetRotation(out quaternion);
                return quaternion;
            }
        }

        /// <summary>
        /// <para>Specifies the update mode of the Animator.</para>
        /// </summary>
        public AnimatorUpdateMode updateMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Gets the avatar velocity  for the last evaluated frame.</para>
        /// </summary>
        public Vector3 velocity
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_velocity(out vector);
                return vector;
            }
        }
    }
}

