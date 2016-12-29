namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Playable that plays a RuntimeAnimatorController. Can be used as an input to an AnimationPlayable.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct AnimatorControllerPlayable
    {
        internal AnimationPlayable handle;
        internal Playable node =>
            this.handle.node;
        /// <summary>
        /// <para>Creates an AnimatorControllerPlayable.</para>
        /// </summary>
        /// <param name="controller"></param>
        public static AnimatorControllerPlayable Create(RuntimeAnimatorController controller)
        {
            AnimatorControllerPlayable that = new AnimatorControllerPlayable();
            InternalCreate(controller, ref that);
            return that;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void InternalCreate(RuntimeAnimatorController controller, ref AnimatorControllerPlayable that);
        /// <summary>
        /// <para>Call this method to release the resources allocated by the Playable.</para>
        /// </summary>
        public void Destroy()
        {
            this.node.Destroy();
        }

        public static implicit operator Playable(AnimatorControllerPlayable s) => 
            s.node;

        public static implicit operator AnimationPlayable(AnimatorControllerPlayable s) => 
            s.handle;

        /// <summary>
        /// <para>RuntimeAnimatorController played by this playable.</para>
        /// </summary>
        public RuntimeAnimatorController animatorController =>
            GetAnimatorControllerInternal(ref this);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern RuntimeAnimatorController GetAnimatorControllerInternal(ref AnimatorControllerPlayable that);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetFloat.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public float GetFloat(string name) => 
            GetFloatString(ref this, name);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetFloat.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public float GetFloat(int id) => 
            GetFloatID(ref this, id);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetFloat.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        public void SetFloat(string name, float value)
        {
            SetFloatString(ref this, name, value);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetFloat.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        public void SetFloat(int id, float value)
        {
            SetFloatID(ref this, id, value);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetBool.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public bool GetBool(string name) => 
            GetBoolString(ref this, name);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetBool.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public bool GetBool(int id) => 
            GetBoolID(ref this, id);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetBool.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        public void SetBool(string name, bool value)
        {
            SetBoolString(ref this, name, value);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetBool.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        public void SetBool(int id, bool value)
        {
            SetBoolID(ref this, id, value);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetInteger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public int GetInteger(string name) => 
            GetIntegerString(ref this, name);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetInteger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public int GetInteger(int id) => 
            GetIntegerID(ref this, id);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetInteger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        public void SetInteger(string name, int value)
        {
            SetIntegerString(ref this, name, value);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetInteger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        public void SetInteger(int id, int value)
        {
            SetIntegerID(ref this, id, value);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetTrigger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public void SetTrigger(string name)
        {
            SetTriggerString(ref this, name);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetTrigger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public void SetTrigger(int id)
        {
            SetTriggerID(ref this, id);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.ResetTrigger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public void ResetTrigger(string name)
        {
            ResetTriggerString(ref this, name);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.ResetTrigger.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public void ResetTrigger(int id)
        {
            ResetTriggerID(ref this, id);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.IsParameterControlledByCurve.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public bool IsParameterControlledByCurve(string name) => 
            IsParameterControlledByCurveString(ref this, name);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.IsParameterControlledByCurve.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public bool IsParameterControlledByCurve(int id) => 
            IsParameterControlledByCurveID(ref this, id);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.layerCount.</para>
        /// </summary>
        public int layerCount =>
            GetLayerCountInternal(ref this);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int GetLayerCountInternal(ref AnimatorControllerPlayable that);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string GetLayerNameInternal(ref AnimatorControllerPlayable that, int layerIndex);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetLayerName.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        public string GetLayerName(int layerIndex) => 
            GetLayerNameInternal(ref this, layerIndex);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int GetLayerIndexInternal(ref AnimatorControllerPlayable that, string layerName);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetLayerIndex.</para>
        /// </summary>
        /// <param name="layerName"></param>
        public int GetLayerIndex(string layerName) => 
            GetLayerIndexInternal(ref this, layerName);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern float GetLayerWeightInternal(ref AnimatorControllerPlayable that, int layerIndex);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetLayerWeight.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        public float GetLayerWeight(int layerIndex) => 
            GetLayerWeightInternal(ref this, layerIndex);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetLayerWeightInternal(ref AnimatorControllerPlayable that, int layerIndex, float weight);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.SetLayerWeight.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        /// <param name="weight"></param>
        public void SetLayerWeight(int layerIndex, float weight)
        {
            SetLayerWeightInternal(ref this, layerIndex, weight);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern AnimatorStateInfo GetCurrentAnimatorStateInfoInternal(ref AnimatorControllerPlayable that, int layerIndex);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetCurrentAnimatorStateInfo.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex) => 
            GetCurrentAnimatorStateInfoInternal(ref this, layerIndex);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern AnimatorStateInfo GetNextAnimatorStateInfoInternal(ref AnimatorControllerPlayable that, int layerIndex);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetNextAnimatorStateInfo.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        public AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex) => 
            GetNextAnimatorStateInfoInternal(ref this, layerIndex);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern AnimatorTransitionInfo GetAnimatorTransitionInfoInternal(ref AnimatorControllerPlayable that, int layerIndex);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetAnimatorTransitionInfo.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        public AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex) => 
            GetAnimatorTransitionInfoInternal(ref this, layerIndex);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern AnimatorClipInfo[] GetCurrentAnimatorClipInfoInternal(ref AnimatorControllerPlayable that, int layerIndex);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetCurrentAnimatorClipInfo.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        public AnimatorClipInfo[] GetCurrentAnimatorClipInfo(int layerIndex) => 
            GetCurrentAnimatorClipInfoInternal(ref this, layerIndex);

        public void GetCurrentAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips)
        {
            if (clips == null)
            {
                throw new ArgumentNullException("clips");
            }
            this.GetAnimatorClipInfoInternal(ref this, layerIndex, true, clips);
        }

        public void GetNextAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips)
        {
            if (clips == null)
            {
                throw new ArgumentNullException("clips");
            }
            this.GetAnimatorClipInfoInternal(ref this, layerIndex, false, clips);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void GetAnimatorClipInfoInternal(ref AnimatorControllerPlayable that, int layerIndex, bool isCurrent, object clips);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int GetAnimatorClipInfoCountInternal(ref AnimatorControllerPlayable that, int layerIndex, bool current);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetCurrentAnimatorClipInfoCount.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        public int GetCurrentAnimatorClipInfoCount(int layerIndex) => 
            this.GetAnimatorClipInfoCountInternal(ref this, layerIndex, true);

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetNextAnimatorClipInfoCount.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        public int GetNextAnimatorClipInfoCount(int layerIndex) => 
            this.GetAnimatorClipInfoCountInternal(ref this, layerIndex, false);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern AnimatorClipInfo[] GetNextAnimatorClipInfoInternal(ref AnimatorControllerPlayable that, int layerIndex);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.GetNextAnimatorClipInfo.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        public AnimatorClipInfo[] GetNextAnimatorClipInfo(int layerIndex) => 
            GetNextAnimatorClipInfoInternal(ref this, layerIndex);

        internal string ResolveHash(int hash) => 
            ResolveHashInternal(ref this, hash);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string ResolveHashInternal(ref AnimatorControllerPlayable that, int hash);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool IsInTransitionInternal(ref AnimatorControllerPlayable that, int layerIndex);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.IsInTransition.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        public bool IsInTransition(int layerIndex) => 
            IsInTransitionInternal(ref this, layerIndex);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int GetParameterCountInternal(ref AnimatorControllerPlayable that);
        /// <summary>
        /// <para>See IAnimatorControllerPlayable.parameterCount.</para>
        /// </summary>
        public int parameterCount =>
            GetParameterCountInternal(ref this);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern AnimatorControllerParameter[] GetParametersArrayInternal(ref AnimatorControllerPlayable that);
        /// <summary>
        /// <para>See AnimatorController.GetParameter.</para>
        /// </summary>
        /// <param name="index"></param>
        public AnimatorControllerParameter GetParameter(int index)
        {
            AnimatorControllerParameter[] parametersArrayInternal = GetParametersArrayInternal(ref this);
            if ((index < 0) && (index >= parametersArrayInternal.Length))
            {
                throw new IndexOutOfRangeException("index");
            }
            return parametersArrayInternal[index];
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private static extern int StringToHash(string name);
        [ExcludeFromDocs]
        public void CrossFadeInFixedTime(string stateName, float transitionDuration, int layer)
        {
            float fixedTime = 0f;
            this.CrossFadeInFixedTime(stateName, transitionDuration, layer, fixedTime);
        }

        [ExcludeFromDocs]
        public void CrossFadeInFixedTime(string stateName, float transitionDuration)
        {
            float fixedTime = 0f;
            int layer = -1;
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
        public void CrossFadeInFixedTime(string stateName, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTime)
        {
            CrossFadeInFixedTimeInternal(ref this, StringToHash(stateName), transitionDuration, layer, fixedTime);
        }

        [ExcludeFromDocs]
        public void CrossFadeInFixedTime(int stateNameHash, float transitionDuration, int layer)
        {
            float fixedTime = 0f;
            this.CrossFadeInFixedTime(stateNameHash, transitionDuration, layer, fixedTime);
        }

        [ExcludeFromDocs]
        public void CrossFadeInFixedTime(int stateNameHash, float transitionDuration)
        {
            float fixedTime = 0f;
            int layer = -1;
            this.CrossFadeInFixedTime(stateNameHash, transitionDuration, layer, fixedTime);
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
            CrossFadeInFixedTimeInternal(ref this, stateNameHash, transitionDuration, layer, fixedTime);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void CrossFadeInFixedTimeInternal(ref AnimatorControllerPlayable that, int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTime);
        [ExcludeFromDocs]
        private static void CrossFadeInFixedTimeInternal(ref AnimatorControllerPlayable that, int stateNameHash, float transitionDuration, int layer)
        {
            float fixedTime = 0f;
            CrossFadeInFixedTimeInternal(ref that, stateNameHash, transitionDuration, layer, fixedTime);
        }

        [ExcludeFromDocs]
        private static void CrossFadeInFixedTimeInternal(ref AnimatorControllerPlayable that, int stateNameHash, float transitionDuration)
        {
            float fixedTime = 0f;
            int layer = -1;
            CrossFadeInFixedTimeInternal(ref that, stateNameHash, transitionDuration, layer, fixedTime);
        }

        [ExcludeFromDocs]
        public void CrossFade(string stateName, float transitionDuration, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.CrossFade(stateName, transitionDuration, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void CrossFade(string stateName, float transitionDuration)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
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
        public void CrossFade(string stateName, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
        {
            CrossFadeInternal(ref this, StringToHash(stateName), transitionDuration, layer, normalizedTime);
        }

        [ExcludeFromDocs]
        public void CrossFade(int stateNameHash, float transitionDuration, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.CrossFade(stateNameHash, transitionDuration, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void CrossFade(int stateNameHash, float transitionDuration)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.CrossFade(stateNameHash, transitionDuration, layer, negativeInfinity);
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
            CrossFadeInternal(ref this, stateNameHash, transitionDuration, layer, normalizedTime);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void CrossFadeInternal(ref AnimatorControllerPlayable that, int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime);
        [ExcludeFromDocs]
        private static void CrossFadeInternal(ref AnimatorControllerPlayable that, int stateNameHash, float transitionDuration, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            CrossFadeInternal(ref that, stateNameHash, transitionDuration, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        private static void CrossFadeInternal(ref AnimatorControllerPlayable that, int stateNameHash, float transitionDuration)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            CrossFadeInternal(ref that, stateNameHash, transitionDuration, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void PlayInFixedTime(string stateName, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.PlayInFixedTime(stateName, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void PlayInFixedTime(string stateName)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.PlayInFixedTime(stateName, layer, negativeInfinity);
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
            PlayInFixedTimeInternal(ref this, StringToHash(stateName), layer, fixedTime);
        }

        [ExcludeFromDocs]
        public void PlayInFixedTime(int stateNameHash, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.PlayInFixedTime(stateNameHash, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void PlayInFixedTime(int stateNameHash)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.PlayInFixedTime(stateNameHash, layer, negativeInfinity);
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
            PlayInFixedTimeInternal(ref this, stateNameHash, layer, fixedTime);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void PlayInFixedTimeInternal(ref AnimatorControllerPlayable that, int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime);
        [ExcludeFromDocs]
        private static void PlayInFixedTimeInternal(ref AnimatorControllerPlayable that, int stateNameHash, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            PlayInFixedTimeInternal(ref that, stateNameHash, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        private static void PlayInFixedTimeInternal(ref AnimatorControllerPlayable that, int stateNameHash)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            PlayInFixedTimeInternal(ref that, stateNameHash, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void Play(string stateName, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.Play(stateName, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void Play(string stateName)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.Play(stateName, layer, negativeInfinity);
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
            this.PlayInternal(ref this, StringToHash(stateName), layer, normalizedTime);
        }

        [ExcludeFromDocs]
        public void Play(int stateNameHash, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.Play(stateNameHash, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void Play(int stateNameHash)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.Play(stateNameHash, layer, negativeInfinity);
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
            this.PlayInternal(ref this, stateNameHash, layer, normalizedTime);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void PlayInternal(ref AnimatorControllerPlayable that, int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime);
        [ExcludeFromDocs]
        private void PlayInternal(ref AnimatorControllerPlayable that, int stateNameHash, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.PlayInternal(ref that, stateNameHash, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        private void PlayInternal(ref AnimatorControllerPlayable that, int stateNameHash)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.PlayInternal(ref that, stateNameHash, layer, negativeInfinity);
        }

        /// <summary>
        /// <para>See IAnimatorControllerPlayable.HasState.</para>
        /// </summary>
        /// <param name="layerIndex"></param>
        /// <param name="stateID"></param>
        public bool HasState(int layerIndex, int stateID) => 
            this.HasStateInternal(ref this, layerIndex, stateID);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool HasStateInternal(ref AnimatorControllerPlayable that, int layerIndex, int stateID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetFloatString(ref AnimatorControllerPlayable that, string name, float value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetFloatID(ref AnimatorControllerPlayable that, int id, float value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern float GetFloatString(ref AnimatorControllerPlayable that, string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern float GetFloatID(ref AnimatorControllerPlayable that, int id);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetBoolString(ref AnimatorControllerPlayable that, string name, bool value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetBoolID(ref AnimatorControllerPlayable that, int id, bool value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool GetBoolString(ref AnimatorControllerPlayable that, string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool GetBoolID(ref AnimatorControllerPlayable that, int id);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetIntegerString(ref AnimatorControllerPlayable that, string name, int value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetIntegerID(ref AnimatorControllerPlayable that, int id, int value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int GetIntegerString(ref AnimatorControllerPlayable that, string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int GetIntegerID(ref AnimatorControllerPlayable that, int id);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetTriggerString(ref AnimatorControllerPlayable that, string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetTriggerID(ref AnimatorControllerPlayable that, int id);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void ResetTriggerString(ref AnimatorControllerPlayable that, string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void ResetTriggerID(ref AnimatorControllerPlayable that, int id);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool IsParameterControlledByCurveString(ref AnimatorControllerPlayable that, string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool IsParameterControlledByCurveID(ref AnimatorControllerPlayable that, int id);
        /// <summary>
        /// <para>Current Experimental.Director.PlayState of this playable. This indicates whether the Playable is currently playing or paused.</para>
        /// </summary>
        public PlayState state
        {
            get => 
                Playables.GetPlayStateValidated(*((Playable*) this), base.GetType());
            set
            {
                Playables.SetPlayStateValidated(*((Playable*) this), value, base.GetType());
            }
        }
        /// <summary>
        /// <para>Current time in seconds.</para>
        /// </summary>
        public double time
        {
            get => 
                Playables.GetTimeValidated(*((Playable*) this), base.GetType());
            set
            {
                Playables.SetTimeValidated(*((Playable*) this), value, base.GetType());
            }
        }
        /// <summary>
        /// <para>Duration in seconds.</para>
        /// </summary>
        public double duration
        {
            get => 
                Playables.GetDurationValidated(*((Playable*) this), base.GetType());
            set
            {
                Playables.SetDurationValidated(*((Playable*) this), value, base.GetType());
            }
        }
        /// <summary>
        /// <para>Returns true if the Playable is valid. A playable can be invalid if it was disposed. This is different from a Null playable.</para>
        /// </summary>
        public unsafe bool IsValid() => 
            Playables.IsValid(*((Playable*) this));

        public T CastTo<T>() where T: struct => 
            this.handle.CastTo<T>();

        public static bool operator ==(AnimatorControllerPlayable x, Playable y) => 
            Playables.Equals((Playable) x, y);

        public static bool operator !=(AnimatorControllerPlayable x, Playable y) => 
            !Playables.Equals((Playable) x, y);

        public override unsafe bool Equals(object p) => 
            Playables.Equals(*((Playable*) this), p);

        public override int GetHashCode() => 
            this.node.GetHashCode();
    }
}

