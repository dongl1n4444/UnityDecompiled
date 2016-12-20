namespace UnityEngine.Windows.Speech
{
    using System;

    /// <summary>
    /// <para>Represents the reason why dictation session has completed.</para>
    /// </summary>
    public enum DictationCompletionCause
    {
        Complete,
        AudioQualityFailure,
        Canceled,
        TimeoutExceeded,
        PauseLimitExceeded,
        NetworkFailure,
        MicrophoneUnavailable,
        UnknownError
    }
}

