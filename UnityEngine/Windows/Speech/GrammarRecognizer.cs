namespace UnityEngine.Windows.Speech
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>The GrammarRecognizer is a complement to the KeywordRecognizer. In many cases developers will find the KeywordRecognizer fills all their development needs. However, in some cases, more complex grammars will be better expressed in the form of an xml file on disk.
    /// The GrammarRecognizer uses Extensible Markup Language (XML) elements and attributes, as specified in the World Wide Web Consortium (W3C) Speech Recognition Grammar Specification (SRGS) Version 1.0. These XML elements and attributes represent the rule structures that define the words or phrases (commands) recognized by speech recognition engines.</para>
    /// </summary>
    public sealed class GrammarRecognizer : PhraseRecognizer
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <GrammarFilePath>k__BackingField;

        /// <summary>
        /// <para>Creates a grammar recognizer using specified file path and minimum confidence.</para>
        /// </summary>
        /// <param name="grammarFilePath">Path of the grammar file.</param>
        /// <param name="minimumConfidence">The confidence level at which the recognizer will begin accepting phrases.</param>
        public GrammarRecognizer(string grammarFilePath) : this(grammarFilePath, ConfidenceLevel.Medium)
        {
        }

        /// <summary>
        /// <para>Creates a grammar recognizer using specified file path and minimum confidence.</para>
        /// </summary>
        /// <param name="grammarFilePath">Path of the grammar file.</param>
        /// <param name="minimumConfidence">The confidence level at which the recognizer will begin accepting phrases.</param>
        public GrammarRecognizer(string grammarFilePath, ConfidenceLevel minimumConfidence)
        {
            if (grammarFilePath == null)
            {
                throw new ArgumentNullException("grammarFilePath");
            }
            if (grammarFilePath.Length == 0)
            {
                throw new ArgumentException("Grammar file path cannot be empty.");
            }
            this.GrammarFilePath = grammarFilePath;
            base.m_Recognizer = base.CreateFromGrammarFile(grammarFilePath, minimumConfidence);
        }

        /// <summary>
        /// <para>Returns the grammar file path which was supplied when the grammar recognizer was created.</para>
        /// </summary>
        public string GrammarFilePath { get; private set; }
    }
}

