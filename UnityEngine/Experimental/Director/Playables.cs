namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Runtime.CompilerServices;

    internal sealed class Playables
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void BeginIgnoreAllocationTracker();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern object CastToInternal(Type castType, IntPtr handle, int version);
        internal static bool CheckInputBounds(Playable playable, int inputIndex) => 
            playable.CheckInputBounds(inputIndex);

        internal static bool CompareVersion(Playable lhs, Playable rhs) => 
            Playable.CompareVersion(lhs, rhs);

        internal static bool ConnectInternal(ref Playable source, ref Playable target, int sourceOutputPort, int targetInputPort) => 
            INTERNAL_CALL_ConnectInternal(ref source, ref target, sourceOutputPort, targetInputPort);

        internal static void DisconnectInternal(ref Playable target, int inputPort)
        {
            INTERNAL_CALL_DisconnectInternal(ref target, inputPort);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void EndIgnoreAllocationTracker();
        internal static bool Equals(Playable isAPlayable, object mightBeAnythingOrNull) => 
            ((mightBeAnythingOrNull != null) && isAPlayable.Equals(mightBeAnythingOrNull));

        internal static bool Equals(Playable lhs, Playable rhs) => 
            CompareVersion(lhs, rhs);

        internal static double GetDurationValidated(Playable playable, Type typeofPlayable) => 
            playable.duration;

        internal static int GetInputCountValidated(Playable playable, Type typeofPlayable) => 
            playable.inputCount;

        internal static Playable GetInputValidated(Playable playable, int inputPort, Type typeofPlayable) => 
            playable.GetInput(inputPort);

        internal static float GetInputWeightValidated(Playable playable, int index, Type typeofPlayable) => 
            playable.GetInputWeight(index);

        internal static int GetOutputCountValidated(Playable playable, Type typeofPlayable) => 
            playable.outputCount;

        internal static Playable GetOutputValidated(Playable playable, int outputPort, Type typeofPlayable) => 
            playable.GetOutput(outputPort);

        internal static PlayState GetPlayStateValidated(Playable playable, Type typeofPlayable) => 
            playable.state;

        internal static double GetTimeValidated(Playable playable, Type typeofPlayable) => 
            playable.time;

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Type GetTypeOfInternal(IntPtr handle, int version);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_ConnectInternal(ref Playable source, ref Playable target, int sourceOutputPort, int targetInputPort);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_DisconnectInternal(ref Playable target, int inputPort);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_InternalDestroy(ref Playable playable);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetPlayableDeleteOnDisconnect(ref Playable target, bool value);
        internal static void InternalDestroy(ref Playable playable)
        {
            INTERNAL_CALL_InternalDestroy(ref playable);
        }

        internal static bool IsValid(Playable playable) => 
            playable.IsValid();

        internal static void SetDurationValidated(Playable playable, double duration, Type typeofPlayable)
        {
            playable.duration = duration;
        }

        internal static void SetInputWeightValidated(Playable playable, int inputIndex, float weight, Type typeofPlayable)
        {
            playable.SetInputWeight(inputIndex, weight);
        }

        internal static void SetInputWeightValidated(Playable playable, Playable input, float weight, Type typeofPlayable)
        {
            playable.SetInputWeight(input, weight);
        }

        internal static void SetPlayableDeleteOnDisconnect(ref Playable target, bool value)
        {
            INTERNAL_CALL_SetPlayableDeleteOnDisconnect(ref target, value);
        }

        internal static void SetPlayStateValidated(Playable playable, PlayState playState, Type typeofPlayable)
        {
            playable.state = playState;
        }

        internal static void SetTimeValidated(Playable playable, double time, Type typeofPlayable)
        {
            playable.time = time;
        }
    }
}

