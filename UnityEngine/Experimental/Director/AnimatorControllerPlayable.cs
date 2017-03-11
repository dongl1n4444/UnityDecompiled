namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Playable that plays a RuntimeAnimatorController. Can be used as an input to an AnimationPlayable.</para>
    /// </summary>
    [RequiredByNativeCode]
    public class AnimatorControllerPlayable : AnimationPlayable
    {
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
        public void CrossFade(int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
        {
            CrossFadeInternal(ref this.handle, stateNameHash, transitionDuration, layer, normalizedTime);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.CrossFade.</para>
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="transitionDuration"></param>
        /// <param name="layer"></param>
        /// <param name="normalizedTime"></param>
        /// <param name="stateNameHash"></param>
        public void CrossFade(string stateName, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
        {
            CrossFadeInternal(ref this.handle, StringToHash(stateName), transitionDuration, layer, normalizedTime);
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
        public void CrossFadeInFixedTime(int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTime)
        {
            CrossFadeInFixedTimeInternal(ref this.handle, stateNameHash, transitionDuration, layer, fixedTime);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.CrossFadeInFixedTime.</para>
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="transitionDuration"></param>
        /// <param name="layer"></param>
        /// <param name="fixedTime"></param>
        /// <param name="stateNameHash"></param>
        public void CrossFadeInFixedTime(string stateName, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTime)
        {
            CrossFadeInFixedTimeInternal(ref this.handle, StringToHash(stateName), transitionDuration, layer, fixedTime);
        }

        [ExcludeFromDocs]
        private static void CrossFadeInFixedTimeInternal(ref PlayableHandle handle, int stateNameHash, float transitionDuration)
        {
            float fixedTime = 0f;
            int layer = -1;
            INTERNAL_CALL_CrossFadeInFixedTimeInternal(ref handle, stateNameHash, transitionDuration, layer, fixedTime);
        }

        [ExcludeFromDocs]
        private static void CrossFadeInFixedTimeInternal(ref PlayableHandle handle, int stateNameHash, float transitionDuration, int layer)
        {
            float fixedTime = 0f;
            INTERNAL_CALL_CrossFadeInFixedTimeInternal(ref handle, stateNameHash, transitionDuration, layer, fixedTime);
        }

        private static void CrossFadeInFixedTimeInternal(ref PlayableHandle handle, int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTime)
        {
            INTERNAL_CALL_CrossFadeInFixedTimeInternal(ref handle, stateNameHash, transitionDuration, layer, fixedTime);
        }

        [ExcludeFromDocs]
        private static void CrossFadeInternal(ref PlayableHandle handle, int stateNameHash, float transitionDuration)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            INTERNAL_CALL_CrossFadeInternal(ref handle, stateNameHash, transitionDuration, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        private static void CrossFadeInternal(ref PlayableHandle handle, int stateNameHash, float transitionDuration, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            INTERNAL_CALL_CrossFadeInternal(ref handle, stateNameHash, transitionDuration, layer, negativeInfinity);
        }

        private static void CrossFadeInternal(ref PlayableHandle handle, int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
        {
            INTERNAL_CALL_CrossFadeInternal(ref handle, stateNameHash, transitionDuration, layer, normalizedTime);
        }

        private static int GetAnimatorClipInfoCountInternal(ref PlayableHandle handle, int layerIndex, bool current) => 
            INTERNAL_CALL_GetAnimatorClipInfoCountInternal(ref handle, layerIndex, current);

        private void GetAnimatorClipInfoInternal(ref PlayableHandle handle, int layerIndex, bool isCurrent, object clips)
        {
            INTERNAL_CALL_GetAnimatorClipInfoInternal(this, ref handle, layerIndex, isCurrent, clips);
        }

        private static RuntimeAnimatorController GetAnimatorControllerInternal(ref PlayableHandle handle) => 
            INTERNAL_CALL_GetAnimatorControllerInternal(ref handle);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetAnimatorTransitionInfo.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        public AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex) => 
            GetAnimatorTransitionInfoInternal(ref this.handle, layerIndex);

        private static AnimatorTransitionInfo GetAnimatorTransitionInfoInternal(ref PlayableHandle handle, int layerIndex) => 
            INTERNAL_CALL_GetAnimatorTransitionInfoInternal(ref handle, layerIndex);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetBool.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public bool GetBool(int id) => 
            GetBoolID(ref this.handle, id);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetBool.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public bool GetBool(string name) => 
            GetBoolString(ref this.handle, name);

        private static bool GetBoolID(ref PlayableHandle handle, int id) => 
            INTERNAL_CALL_GetBoolID(ref handle, id);

        private static bool GetBoolString(ref PlayableHandle handle, string name) => 
            INTERNAL_CALL_GetBoolString(ref handle, name);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetCurrentAnimatorClipInfo.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        public AnimatorClipInfo[] GetCurrentAnimatorClipInfo(int layerIndex) => 
            GetCurrentAnimatorClipInfoInternal(ref this.handle, layerIndex);

        public void GetCurrentAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips)
        {
            if (clips == null)
            {
                throw new ArgumentNullException("clips");
            }
            this.GetAnimatorClipInfoInternal(ref this.handle, layerIndex, true, clips);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetCurrentAnimatorClipInfoCount.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        public int GetCurrentAnimatorClipInfoCount(int layerIndex) => 
            GetAnimatorClipInfoCountInternal(ref this.handle, layerIndex, true);

        private static AnimatorClipInfo[] GetCurrentAnimatorClipInfoInternal(ref PlayableHandle handle, int layerIndex) => 
            INTERNAL_CALL_GetCurrentAnimatorClipInfoInternal(ref handle, layerIndex);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetCurrentAnimatorStateInfo.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex) => 
            GetCurrentAnimatorStateInfoInternal(ref this.handle, layerIndex);

        private static AnimatorStateInfo GetCurrentAnimatorStateInfoInternal(ref PlayableHandle handle, int layerIndex) => 
            INTERNAL_CALL_GetCurrentAnimatorStateInfoInternal(ref handle, layerIndex);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetFloat.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public float GetFloat(int id) => 
            GetFloatID(ref this.handle, id);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetFloat.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public float GetFloat(string name) => 
            GetFloatString(ref this.handle, name);

        private static float GetFloatID(ref PlayableHandle handle, int id) => 
            INTERNAL_CALL_GetFloatID(ref handle, id);

        private static float GetFloatString(ref PlayableHandle handle, string name) => 
            INTERNAL_CALL_GetFloatString(ref handle, name);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetInteger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public int GetInteger(int id) => 
            GetIntegerID(ref this.handle, id);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetInteger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public int GetInteger(string name) => 
            GetIntegerString(ref this.handle, name);

        private static int GetIntegerID(ref PlayableHandle handle, int id) => 
            INTERNAL_CALL_GetIntegerID(ref handle, id);

        private static int GetIntegerString(ref PlayableHandle handle, string name) => 
            INTERNAL_CALL_GetIntegerString(ref handle, name);

        private static int GetLayerCountInternal(ref PlayableHandle handle) => 
            INTERNAL_CALL_GetLayerCountInternal(ref handle);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetLayerIndex.</para>
        /// </summary>
        /// <param name="layerName"></param>
        public int GetLayerIndex(string layerName) => 
            GetLayerIndexInternal(ref this.handle, layerName);

        private static int GetLayerIndexInternal(ref PlayableHandle handle, string layerName) => 
            INTERNAL_CALL_GetLayerIndexInternal(ref handle, layerName);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetLayerName.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        public string GetLayerName(int layerIndex) => 
            GetLayerNameInternal(ref this.handle, layerIndex);

        private static string GetLayerNameInternal(ref PlayableHandle handle, int layerIndex) => 
            INTERNAL_CALL_GetLayerNameInternal(ref handle, layerIndex);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetLayerWeight.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        public float GetLayerWeight(int layerIndex) => 
            GetLayerWeightInternal(ref this.handle, layerIndex);

        private static float GetLayerWeightInternal(ref PlayableHandle handle, int layerIndex) => 
            INTERNAL_CALL_GetLayerWeightInternal(ref handle, layerIndex);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetNextAnimatorClipInfo.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        public AnimatorClipInfo[] GetNextAnimatorClipInfo(int layerIndex) => 
            GetNextAnimatorClipInfoInternal(ref this.handle, layerIndex);

        public void GetNextAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips)
        {
            if (clips == null)
            {
                throw new ArgumentNullException("clips");
            }
            this.GetAnimatorClipInfoInternal(ref this.handle, layerIndex, false, clips);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetNextAnimatorClipInfoCount.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        public int GetNextAnimatorClipInfoCount(int layerIndex) => 
            GetAnimatorClipInfoCountInternal(ref this.handle, layerIndex, false);

        private static AnimatorClipInfo[] GetNextAnimatorClipInfoInternal(ref PlayableHandle handle, int layerIndex) => 
            INTERNAL_CALL_GetNextAnimatorClipInfoInternal(ref handle, layerIndex);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetNextAnimatorStateInfo.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        public AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex) => 
            GetNextAnimatorStateInfoInternal(ref this.handle, layerIndex);

        private static AnimatorStateInfo GetNextAnimatorStateInfoInternal(ref PlayableHandle handle, int layerIndex) => 
            INTERNAL_CALL_GetNextAnimatorStateInfoInternal(ref handle, layerIndex);

        /// <summary>
        /// <para>See AnimatorController.GetParameter.</para>
        /// </summary>
        /// <param name="index"></param>
        public AnimatorControllerParameter GetParameter(int index)
        {
            AnimatorControllerParameter[] parametersArrayInternal = GetParametersArrayInternal(ref this.handle);
            if ((index < 0) && (index >= parametersArrayInternal.Length))
            {
                throw new IndexOutOfRangeException("index");
            }
            return parametersArrayInternal[index];
        }

        private static int GetParameterCountInternal(ref PlayableHandle handle) => 
            INTERNAL_CALL_GetParameterCountInternal(ref handle);

        private static AnimatorControllerParameter[] GetParametersArrayInternal(ref PlayableHandle handle) => 
            INTERNAL_CALL_GetParametersArrayInternal(ref handle);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.HasState.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        /// <param name="stateID"></param>
        public bool HasState(int layerIndex, int stateID) => 
            HasStateInternal(ref this.handle, layerIndex, stateID);

        private static bool HasStateInternal(ref PlayableHandle handle, int layerIndex, int stateID) => 
            INTERNAL_CALL_HasStateInternal(ref handle, layerIndex, stateID);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_CrossFadeInFixedTimeInternal(ref PlayableHandle handle, int stateNameHash, float transitionDuration, int layer, float fixedTime);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_CrossFadeInternal(ref PlayableHandle handle, int stateNameHash, float transitionDuration, int layer, float normalizedTime);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int INTERNAL_CALL_GetAnimatorClipInfoCountInternal(ref PlayableHandle handle, int layerIndex, bool current);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetAnimatorClipInfoInternal(AnimatorControllerPlayable self, ref PlayableHandle handle, int layerIndex, bool isCurrent, object clips);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern RuntimeAnimatorController INTERNAL_CALL_GetAnimatorControllerInternal(ref PlayableHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern AnimatorTransitionInfo INTERNAL_CALL_GetAnimatorTransitionInfoInternal(ref PlayableHandle handle, int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_GetBoolID(ref PlayableHandle handle, int id);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_GetBoolString(ref PlayableHandle handle, string name);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern AnimatorClipInfo[] INTERNAL_CALL_GetCurrentAnimatorClipInfoInternal(ref PlayableHandle handle, int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern AnimatorStateInfo INTERNAL_CALL_GetCurrentAnimatorStateInfoInternal(ref PlayableHandle handle, int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern float INTERNAL_CALL_GetFloatID(ref PlayableHandle handle, int id);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern float INTERNAL_CALL_GetFloatString(ref PlayableHandle handle, string name);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int INTERNAL_CALL_GetIntegerID(ref PlayableHandle handle, int id);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int INTERNAL_CALL_GetIntegerString(ref PlayableHandle handle, string name);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int INTERNAL_CALL_GetLayerCountInternal(ref PlayableHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int INTERNAL_CALL_GetLayerIndexInternal(ref PlayableHandle handle, string layerName);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern string INTERNAL_CALL_GetLayerNameInternal(ref PlayableHandle handle, int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern float INTERNAL_CALL_GetLayerWeightInternal(ref PlayableHandle handle, int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern AnimatorClipInfo[] INTERNAL_CALL_GetNextAnimatorClipInfoInternal(ref PlayableHandle handle, int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern AnimatorStateInfo INTERNAL_CALL_GetNextAnimatorStateInfoInternal(ref PlayableHandle handle, int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int INTERNAL_CALL_GetParameterCountInternal(ref PlayableHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern AnimatorControllerParameter[] INTERNAL_CALL_GetParametersArrayInternal(ref PlayableHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_HasStateInternal(ref PlayableHandle handle, int layerIndex, int stateID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_IsInTransitionInternal(ref PlayableHandle handle, int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_IsParameterControlledByCurveID(ref PlayableHandle handle, int id);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_IsParameterControlledByCurveString(ref PlayableHandle handle, string name);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_PlayInFixedTimeInternal(ref PlayableHandle handle, int stateNameHash, int layer, float fixedTime);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_PlayInternal(ref PlayableHandle handle, int stateNameHash, int layer, float normalizedTime);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_ResetTriggerID(ref PlayableHandle handle, int id);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_ResetTriggerString(ref PlayableHandle handle, string name);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern string INTERNAL_CALL_ResolveHashInternal(ref PlayableHandle handle, int hash);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetBoolID(ref PlayableHandle handle, int id, bool value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetBoolString(ref PlayableHandle handle, string name, bool value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetFloatID(ref PlayableHandle handle, int id, float value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetFloatString(ref PlayableHandle handle, string name, float value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetIntegerID(ref PlayableHandle handle, int id, int value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetIntegerString(ref PlayableHandle handle, string name, int value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetLayerWeightInternal(ref PlayableHandle handle, int layerIndex, float weight);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetTriggerID(ref PlayableHandle handle, int id);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetTriggerString(ref PlayableHandle handle, string name);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.IsInTransition.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        public bool IsInTransition(int layerIndex) => 
            IsInTransitionInternal(ref this.handle, layerIndex);

        private static bool IsInTransitionInternal(ref PlayableHandle handle, int layerIndex) => 
            INTERNAL_CALL_IsInTransitionInternal(ref handle, layerIndex);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.IsParameterControlledByCurve.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public bool IsParameterControlledByCurve(int id) => 
            IsParameterControlledByCurveID(ref this.handle, id);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.IsParameterControlledByCurve.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public bool IsParameterControlledByCurve(string name) => 
            IsParameterControlledByCurveString(ref this.handle, name);

        private static bool IsParameterControlledByCurveID(ref PlayableHandle handle, int id) => 
            INTERNAL_CALL_IsParameterControlledByCurveID(ref handle, id);

        private static bool IsParameterControlledByCurveString(ref PlayableHandle handle, string name) => 
            INTERNAL_CALL_IsParameterControlledByCurveString(ref handle, name);

        public static implicit operator PlayableHandle(AnimatorControllerPlayable b) => 
            b.handle;

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
        public void Play(int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
        {
            PlayInternal(ref this.handle, stateNameHash, layer, normalizedTime);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.Play.</para>
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="layer"></param>
        /// <param name="normalizedTime"></param>
        /// <param name="stateNameHash"></param>
        public void Play(string stateName, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
        {
            PlayInternal(ref this.handle, StringToHash(stateName), layer, normalizedTime);
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
        public void PlayInFixedTime(int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime)
        {
            PlayInFixedTimeInternal(ref this.handle, stateNameHash, layer, fixedTime);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.PlayInFixedTime.</para>
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="layer"></param>
        /// <param name="fixedTime"></param>
        /// <param name="stateNameHash"></param>
        public void PlayInFixedTime(string stateName, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime)
        {
            PlayInFixedTimeInternal(ref this.handle, StringToHash(stateName), layer, fixedTime);
        }

        [ExcludeFromDocs]
        private static void PlayInFixedTimeInternal(ref PlayableHandle handle, int stateNameHash)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            INTERNAL_CALL_PlayInFixedTimeInternal(ref handle, stateNameHash, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        private static void PlayInFixedTimeInternal(ref PlayableHandle handle, int stateNameHash, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            INTERNAL_CALL_PlayInFixedTimeInternal(ref handle, stateNameHash, layer, negativeInfinity);
        }

        private static void PlayInFixedTimeInternal(ref PlayableHandle handle, int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime)
        {
            INTERNAL_CALL_PlayInFixedTimeInternal(ref handle, stateNameHash, layer, fixedTime);
        }

        [ExcludeFromDocs]
        private static void PlayInternal(ref PlayableHandle handle, int stateNameHash)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            INTERNAL_CALL_PlayInternal(ref handle, stateNameHash, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        private static void PlayInternal(ref PlayableHandle handle, int stateNameHash, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            INTERNAL_CALL_PlayInternal(ref handle, stateNameHash, layer, negativeInfinity);
        }

        private static void PlayInternal(ref PlayableHandle handle, int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
        {
            INTERNAL_CALL_PlayInternal(ref handle, stateNameHash, layer, normalizedTime);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.ResetTrigger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public void ResetTrigger(int id)
        {
            ResetTriggerID(ref this.handle, id);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.ResetTrigger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public void ResetTrigger(string name)
        {
            ResetTriggerString(ref this.handle, name);
        }

        private static void ResetTriggerID(ref PlayableHandle handle, int id)
        {
            INTERNAL_CALL_ResetTriggerID(ref handle, id);
        }

        private static void ResetTriggerString(ref PlayableHandle handle, string name)
        {
            INTERNAL_CALL_ResetTriggerString(ref handle, name);
        }

        internal string ResolveHash(int hash) => 
            ResolveHashInternal(ref this.handle, hash);

        private static string ResolveHashInternal(ref PlayableHandle handle, int hash) => 
            INTERNAL_CALL_ResolveHashInternal(ref handle, hash);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetBool.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        public void SetBool(int id, bool value)
        {
            SetBoolID(ref this.handle, id, value);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetBool.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        public void SetBool(string name, bool value)
        {
            SetBoolString(ref this.handle, name, value);
        }

        private static void SetBoolID(ref PlayableHandle handle, int id, bool value)
        {
            INTERNAL_CALL_SetBoolID(ref handle, id, value);
        }

        private static void SetBoolString(ref PlayableHandle handle, string name, bool value)
        {
            INTERNAL_CALL_SetBoolString(ref handle, name, value);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetFloat.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        public void SetFloat(int id, float value)
        {
            SetFloatID(ref this.handle, id, value);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetFloat.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        public void SetFloat(string name, float value)
        {
            SetFloatString(ref this.handle, name, value);
        }

        private static void SetFloatID(ref PlayableHandle handle, int id, float value)
        {
            INTERNAL_CALL_SetFloatID(ref handle, id, value);
        }

        private static void SetFloatString(ref PlayableHandle handle, string name, float value)
        {
            INTERNAL_CALL_SetFloatString(ref handle, name, value);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetInteger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        public void SetInteger(int id, int value)
        {
            SetIntegerID(ref this.handle, id, value);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetInteger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        public void SetInteger(string name, int value)
        {
            SetIntegerString(ref this.handle, name, value);
        }

        private static void SetIntegerID(ref PlayableHandle handle, int id, int value)
        {
            INTERNAL_CALL_SetIntegerID(ref handle, id, value);
        }

        private static void SetIntegerString(ref PlayableHandle handle, string name, int value)
        {
            INTERNAL_CALL_SetIntegerString(ref handle, name, value);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetLayerWeight.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        /// <param name="weight"></param>
        public void SetLayerWeight(int layerIndex, float weight)
        {
            SetLayerWeightInternal(ref this.handle, layerIndex, weight);
        }

        private static void SetLayerWeightInternal(ref PlayableHandle handle, int layerIndex, float weight)
        {
            INTERNAL_CALL_SetLayerWeightInternal(ref handle, layerIndex, weight);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetTrigger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public void SetTrigger(int id)
        {
            SetTriggerID(ref this.handle, id);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetTrigger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public void SetTrigger(string name)
        {
            SetTriggerString(ref this.handle, name);
        }

        private static void SetTriggerID(ref PlayableHandle handle, int id)
        {
            INTERNAL_CALL_SetTriggerID(ref handle, id);
        }

        private static void SetTriggerString(ref PlayableHandle handle, string name)
        {
            INTERNAL_CALL_SetTriggerString(ref handle, name);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        private static extern int StringToHash(string name);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.layerCount.</para>
        /// </summary>
        public int layerCount =>
            GetLayerCountInternal(ref this.handle);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.parameterCount.</para>
        /// </summary>
        public int parameterCount =>
            GetParameterCountInternal(ref this.handle);
    }
}

