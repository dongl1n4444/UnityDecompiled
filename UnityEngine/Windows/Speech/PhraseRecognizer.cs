namespace UnityEngine.Windows.Speech
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A common base class for both keyword recognizer and grammar recognizer.</para>
    /// </summary>
    public abstract class PhraseRecognizer : IDisposable
    {
        protected IntPtr m_Recognizer;

        public event PhraseRecognizedDelegate OnPhraseRecognized;

        internal PhraseRecognizer()
        {
        }

        protected IntPtr CreateFromGrammarFile(string grammarFilePath, ConfidenceLevel minimumConfidence)
        {
            IntPtr ptr;
            INTERNAL_CALL_CreateFromGrammarFile(this, grammarFilePath, minimumConfidence, out ptr);
            return ptr;
        }

        protected IntPtr CreateFromKeywords(string[] keywords, ConfidenceLevel minimumConfidence)
        {
            IntPtr ptr;
            INTERNAL_CALL_CreateFromKeywords(this, keywords, minimumConfidence, out ptr);
            return ptr;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Destroy(IntPtr recognizer);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private static extern void DestroyThreaded(IntPtr recognizer);
        /// <summary>
        /// <para>Disposes the resources used by phrase recognizer.</para>
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

        ~PhraseRecognizer()
        {
            if (this.m_Recognizer != IntPtr.Zero)
            {
                DestroyThreaded(this.m_Recognizer);
                this.m_Recognizer = IntPtr.Zero;
                GC.SuppressFinalize(this);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_CreateFromGrammarFile(PhraseRecognizer self, string grammarFilePath, ConfidenceLevel minimumConfidence, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_CreateFromKeywords(PhraseRecognizer self, string[] keywords, ConfidenceLevel minimumConfidence, out IntPtr value);
        [RequiredByNativeCode]
        private void InvokePhraseRecognizedEvent(string text, ConfidenceLevel confidence, SemanticMeaning[] semanticMeanings, long phraseStartFileTime, long phraseDurationTicks)
        {
            PhraseRecognizedDelegate onPhraseRecognized = this.OnPhraseRecognized;
            if (onPhraseRecognized != null)
            {
                onPhraseRecognized(new PhraseRecognizedEventArgs(text, confidence, semanticMeanings, DateTime.FromFileTime(phraseStartFileTime), TimeSpan.FromTicks(phraseDurationTicks)));
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool IsRunning_Internal(IntPtr recognizer);
        [RequiredByNativeCode]
        private static unsafe SemanticMeaning[] MarshalSemanticMeaning(IntPtr keys, IntPtr values, IntPtr valueSizes, int valueCount)
        {
            SemanticMeaning[] meaningArray = new SemanticMeaning[valueCount];
            int num = 0;
            for (int i = 0; i < valueCount; i++)
            {
                uint num3 = *((uint*) (((void*) valueSizes) + (i * 4)));
                SemanticMeaning meaning = new SemanticMeaning {
                    key = new string(*((char**) (((void*) keys) + (i * sizeof(char*))))),
                    values = new string[num3]
                };
                for (int j = 0; j < num3; j++)
                {
                    meaning.values[j] = new string(*((char**) (((void*) values) + ((num + j) * sizeof(char*)))));
                }
                meaningArray[i] = meaning;
                num += (int) num3;
            }
            return meaningArray;
        }

        /// <summary>
        /// <para>Makes the phrase recognizer start listening to phrases.</para>
        /// </summary>
        public void Start()
        {
            if (this.m_Recognizer != IntPtr.Zero)
            {
                Start_Internal(this.m_Recognizer);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Start_Internal(IntPtr recognizer);
        /// <summary>
        /// <para>Stops the phrase recognizer from listening to phrases.</para>
        /// </summary>
        public void Stop()
        {
            if (this.m_Recognizer != IntPtr.Zero)
            {
                Stop_Internal(this.m_Recognizer);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Stop_Internal(IntPtr recognizer);

        /// <summary>
        /// <para>Tells whether the phrase recognizer is listening for phrases.</para>
        /// </summary>
        public bool IsRunning =>
            ((this.m_Recognizer != IntPtr.Zero) && IsRunning_Internal(this.m_Recognizer));

        /// <summary>
        /// <para>Delegate for OnPhraseRecognized event.</para>
        /// </summary>
        /// <param name="args">Information about a phrase recognized event.</param>
        public delegate void PhraseRecognizedDelegate(PhraseRecognizedEventArgs args);
    }
}

