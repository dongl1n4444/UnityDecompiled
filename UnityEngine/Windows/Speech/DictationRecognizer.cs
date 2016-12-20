namespace UnityEngine.Windows.Speech
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>DictationRecognizer listens to speech input and attempts to determine what phrase was uttered.</para>
    /// </summary>
    public sealed class DictationRecognizer : IDisposable
    {
        private IntPtr m_Recognizer;

        public event DictationCompletedDelegate DictationComplete;

        public event DictationErrorHandler DictationError;

        public event DictationHypothesisDelegate DictationHypothesis;

        public event DictationResultDelegate DictationResult;

        /// <summary>
        /// <para>Create a DictationRecognizer with the specified minimum confidence and dictation topic constraint. Phrases under the specified minimum level will be ignored.</para>
        /// </summary>
        /// <param name="minimumConfidence">The confidence level at which the recognizer will begin accepting phrases.</param>
        /// <param name="topic">The dictation topic that this dictation recognizer should optimize its recognition for.</param>
        /// <param name="confidenceLevel"></param>
        public DictationRecognizer() : this(ConfidenceLevel.Medium, DictationTopicConstraint.Dictation)
        {
        }

        /// <summary>
        /// <para>Create a DictationRecognizer with the specified minimum confidence and dictation topic constraint. Phrases under the specified minimum level will be ignored.</para>
        /// </summary>
        /// <param name="minimumConfidence">The confidence level at which the recognizer will begin accepting phrases.</param>
        /// <param name="topic">The dictation topic that this dictation recognizer should optimize its recognition for.</param>
        /// <param name="confidenceLevel"></param>
        public DictationRecognizer(ConfidenceLevel confidenceLevel) : this(confidenceLevel, DictationTopicConstraint.Dictation)
        {
        }

        /// <summary>
        /// <para>Create a DictationRecognizer with the specified minimum confidence and dictation topic constraint. Phrases under the specified minimum level will be ignored.</para>
        /// </summary>
        /// <param name="minimumConfidence">The confidence level at which the recognizer will begin accepting phrases.</param>
        /// <param name="topic">The dictation topic that this dictation recognizer should optimize its recognition for.</param>
        /// <param name="confidenceLevel"></param>
        public DictationRecognizer(DictationTopicConstraint topic) : this(ConfidenceLevel.Medium, topic)
        {
        }

        /// <summary>
        /// <para>Create a DictationRecognizer with the specified minimum confidence and dictation topic constraint. Phrases under the specified minimum level will be ignored.</para>
        /// </summary>
        /// <param name="minimumConfidence">The confidence level at which the recognizer will begin accepting phrases.</param>
        /// <param name="topic">The dictation topic that this dictation recognizer should optimize its recognition for.</param>
        /// <param name="confidenceLevel"></param>
        public DictationRecognizer(ConfidenceLevel minimumConfidence, DictationTopicConstraint topic)
        {
            this.m_Recognizer = this.Create(minimumConfidence, topic);
        }

        private IntPtr Create(ConfidenceLevel minimumConfidence, DictationTopicConstraint topicConstraint)
        {
            IntPtr ptr;
            INTERNAL_CALL_Create(this, minimumConfidence, topicConstraint, out ptr);
            return ptr;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Destroy(IntPtr self);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private static extern void DestroyThreaded(IntPtr self);
        [RequiredByNativeCode]
        private void DictationRecognizer_InvokeCompletedEvent(DictationCompletionCause cause)
        {
            DictationCompletedDelegate dictationComplete = this.DictationComplete;
            if (dictationComplete != null)
            {
                dictationComplete(cause);
            }
        }

        [RequiredByNativeCode]
        private void DictationRecognizer_InvokeErrorEvent(string error, int hresult)
        {
            DictationErrorHandler dictationError = this.DictationError;
            if (dictationError != null)
            {
                dictationError(error, hresult);
            }
        }

        [RequiredByNativeCode]
        private void DictationRecognizer_InvokeHypothesisGeneratedEvent(string keyword)
        {
            DictationHypothesisDelegate dictationHypothesis = this.DictationHypothesis;
            if (dictationHypothesis != null)
            {
                dictationHypothesis(keyword);
            }
        }

        [RequiredByNativeCode]
        private void DictationRecognizer_InvokeResultGeneratedEvent(string keyword, ConfidenceLevel minimumConfidence)
        {
            DictationResultDelegate dictationResult = this.DictationResult;
            if (dictationResult != null)
            {
                dictationResult(keyword, minimumConfidence);
            }
        }

        /// <summary>
        /// <para>Disposes the resources this dictation recognizer uses.</para>
        /// </summary>
        public void Dispose()
        {
            if (this.m_Recognizer != IntPtr.Zero)
            {
                Destroy(this.m_Recognizer);
                this.m_Recognizer = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }

        ~DictationRecognizer()
        {
            if (this.m_Recognizer != IntPtr.Zero)
            {
                DestroyThreaded(this.m_Recognizer);
                this.m_Recognizer = IntPtr.Zero;
                GC.SuppressFinalize(this);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern float GetAutoSilenceTimeoutSeconds(IntPtr self);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern float GetInitialSilenceTimeoutSeconds(IntPtr self);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern SpeechSystemStatus GetStatus(IntPtr self);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Create(DictationRecognizer self, ConfidenceLevel minimumConfidence, DictationTopicConstraint topicConstraint, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetAutoSilenceTimeoutSeconds(IntPtr self, float value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetInitialSilenceTimeoutSeconds(IntPtr self, float value);
        /// <summary>
        /// <para>Starts the dictation recognization session. Dictation recognizer can only be started if PhraseRecognitionSystem is not running.</para>
        /// </summary>
        public void Start()
        {
            if (this.m_Recognizer != IntPtr.Zero)
            {
                Start(this.m_Recognizer);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Start(IntPtr self);
        /// <summary>
        /// <para>Stops the dictation recognization session.</para>
        /// </summary>
        public void Stop()
        {
            if (this.m_Recognizer != IntPtr.Zero)
            {
                Stop(this.m_Recognizer);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Stop(IntPtr self);

        /// <summary>
        /// <para>The time length in seconds before dictation recognizer session ends due to lack of audio input.</para>
        /// </summary>
        public float AutoSilenceTimeoutSeconds
        {
            get
            {
                if (this.m_Recognizer == IntPtr.Zero)
                {
                    return 0f;
                }
                return GetAutoSilenceTimeoutSeconds(this.m_Recognizer);
            }
            set
            {
                if (this.m_Recognizer != IntPtr.Zero)
                {
                    SetAutoSilenceTimeoutSeconds(this.m_Recognizer, value);
                }
            }
        }

        /// <summary>
        /// <para>The time length in seconds before dictation recognizer session ends due to lack of audio input in case there was no audio heard in the current session.</para>
        /// </summary>
        public float InitialSilenceTimeoutSeconds
        {
            get
            {
                if (this.m_Recognizer == IntPtr.Zero)
                {
                    return 0f;
                }
                return GetInitialSilenceTimeoutSeconds(this.m_Recognizer);
            }
            set
            {
                if (this.m_Recognizer != IntPtr.Zero)
                {
                    SetInitialSilenceTimeoutSeconds(this.m_Recognizer, value);
                }
            }
        }

        /// <summary>
        /// <para>Indicates the status of dictation recognizer.</para>
        /// </summary>
        public SpeechSystemStatus Status
        {
            get
            {
                return (!(this.m_Recognizer != IntPtr.Zero) ? SpeechSystemStatus.Stopped : GetStatus(this.m_Recognizer));
            }
        }

        /// <summary>
        /// <para>Delegate for DictationComplete event.</para>
        /// </summary>
        /// <param name="cause">The cause of dictation session completion.</param>
        public delegate void DictationCompletedDelegate(DictationCompletionCause cause);

        /// <summary>
        /// <para>Delegate for DictationError event.</para>
        /// </summary>
        /// <param name="error">The error mesage.</param>
        /// <param name="hresult">HRESULT code that corresponds to the error.</param>
        public delegate void DictationErrorHandler(string error, int hresult);

        /// <summary>
        /// <para>Callback indicating a hypothesis change event. You should register with DictationHypothesis event.</para>
        /// </summary>
        /// <param name="text">The text that the recognizer believes may have been recognized.</param>
        public delegate void DictationHypothesisDelegate(string text);

        /// <summary>
        /// <para>Callback indicating a phrase has been recognized with the specified confidence level. You should register with DictationResult event.</para>
        /// </summary>
        /// <param name="text">The recognized text.</param>
        /// <param name="confidence">The confidence level at which the text was recognized.</param>
        public delegate void DictationResultDelegate(string text, ConfidenceLevel confidence);
    }
}

