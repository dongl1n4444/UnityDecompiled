namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    internal struct AnimationLayerMixerPlayable
    {
        internal AnimationPlayable handle;
        internal Playable node =>
            this.handle.node;
        public static AnimationLayerMixerPlayable Create()
        {
            AnimationLayerMixerPlayable that = new AnimationLayerMixerPlayable();
            InternalCreate(ref that);
            return that;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void InternalCreate(ref AnimationLayerMixerPlayable that);
        public void Destroy()
        {
            this.node.Destroy();
        }

        public static bool operator ==(AnimationLayerMixerPlayable x, Playable y) => 
            Playables.Equals((Playable) x, y);

        public static bool operator !=(AnimationLayerMixerPlayable x, Playable y) => 
            !Playables.Equals((Playable) x, y);

        public override unsafe bool Equals(object p) => 
            Playables.Equals(*((Playable*) this), p);

        public override int GetHashCode() => 
            this.node.GetHashCode();

        public static implicit operator Playable(AnimationLayerMixerPlayable b) => 
            b.node;

        public static implicit operator AnimationPlayable(AnimationLayerMixerPlayable b) => 
            b.handle;

        public unsafe bool IsValid() => 
            Playables.IsValid(*((Playable*) this));

        public int inputCount =>
            Playables.GetInputCountValidated(*((Playable*) this), base.GetType());
        public unsafe Playable GetInput(int inputPort) => 
            Playables.GetInputValidated(*((Playable*) this), inputPort, base.GetType());

        public int outputCount =>
            Playables.GetOutputCountValidated(*((Playable*) this), base.GetType());
        public unsafe Playable GetOutput(int outputPort) => 
            Playables.GetOutputValidated(*((Playable*) this), outputPort, base.GetType());

        public unsafe float GetInputWeight(int index) => 
            Playables.GetInputWeightValidated(*((Playable*) this), index, base.GetType());

        public unsafe void SetInputWeight(int inputIndex, float weight)
        {
            Playables.SetInputWeightValidated(*((Playable*) this), inputIndex, weight, base.GetType());
        }

        public PlayState state
        {
            get => 
                Playables.GetPlayStateValidated(*((Playable*) this), base.GetType());
            set
            {
                Playables.SetPlayStateValidated(*((Playable*) this), value, base.GetType());
            }
        }
        public double time
        {
            get => 
                Playables.GetTimeValidated(*((Playable*) this), base.GetType());
            set
            {
                Playables.SetTimeValidated(*((Playable*) this), value, base.GetType());
            }
        }
        public double duration
        {
            get => 
                Playables.GetDurationValidated(*((Playable*) this), base.GetType());
            set
            {
                Playables.SetDurationValidated(*((Playable*) this), value, base.GetType());
            }
        }
        public T CastTo<T>() where T: struct => 
            this.handle.CastTo<T>();

        public unsafe int AddInput(Playable input) => 
            AnimationPlayableUtilities.AddInputValidated(*((AnimationPlayable*) this), input, base.GetType());

        public unsafe bool SetInput(Playable source, int index) => 
            AnimationPlayableUtilities.SetInputValidated(*((AnimationPlayable*) this), source, index, base.GetType());

        public unsafe bool SetInputs(IEnumerable<Playable> sources) => 
            AnimationPlayableUtilities.SetInputsValidated(*((AnimationPlayable*) this), sources, base.GetType());

        public unsafe bool RemoveInput(int index) => 
            AnimationPlayableUtilities.RemoveInputValidated(*((AnimationPlayable*) this), index, base.GetType());

        public unsafe bool RemoveAllInputs() => 
            AnimationPlayableUtilities.RemoveAllInputsValidated(*((AnimationPlayable*) this), base.GetType());
    }
}

