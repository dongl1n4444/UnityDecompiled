namespace UnityEngine.Windows.Speech
{
    using System;

    /// <summary>
    /// <para>Represents an error in a speech recognition system.</para>
    /// </summary>
    public enum SpeechError
    {
        NoError,
        TopicLanguageNotSupported,
        GrammarLanguageMismatch,
        GrammarCompilationFailure,
        AudioQualityFailure,
        PauseLimitExceeded,
        TimeoutExceeded,
        NetworkFailure,
        MicrophoneUnavailable,
        UnknownError
    }
}

