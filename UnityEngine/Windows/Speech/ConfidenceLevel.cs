namespace UnityEngine.Windows.Speech
{
    using System;

    /// <summary>
    /// <para>Used by KeywordRecognizer, GrammarRecognizer, DictationRecognizer. Phrases under the specified minimum level will be ignored.</para>
    /// </summary>
    public enum ConfidenceLevel
    {
        High,
        Medium,
        Low,
        Rejected
    }
}

