namespace UnityEditor.Animations
{
    using System;

    /// <summary>
    /// <para>Which AnimatorState transitions can interrupt the Transition.</para>
    /// </summary>
    public enum TransitionInterruptionSource
    {
        None,
        Source,
        Destination,
        SourceThenDestination,
        DestinationThenSource
    }
}

