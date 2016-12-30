namespace UnityEngine.Windows.Speech
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Phrase recognition system is responsible for managing phrase recognizers and dispatching recognition events to them.</para>
    /// </summary>
    public static class PhraseRecognitionSystem
    {
        public static  event ErrorDelegate OnError;

        public static  event StatusDelegate OnStatusChanged;

        [RequiredByNativeCode]
        private static void PhraseRecognitionSystem_InvokeErrorEvent(SpeechError errorCode)
        {
            ErrorDelegate onError = OnError;
            if (onError != null)
            {
                onError(errorCode);
            }
        }

        [RequiredByNativeCode]
        private static void PhraseRecognitionSystem_InvokeStatusChangedEvent(SpeechSystemStatus status)
        {
            StatusDelegate onStatusChanged = OnStatusChanged;
            if (onStatusChanged != null)
            {
                onStatusChanged(status);
            }
        }

        /// <summary>
        /// <para>Attempts to restart the phrase recognition system.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Restart();
        /// <summary>
        /// <para>Shuts phrase recognition system down.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Shutdown();

        /// <summary>
        /// <para>Returns whether speech recognition is supported on the machine that the application is running on.</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public static bool isSupported { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Returns the current status of the phrase recognition system.</para>
        /// </summary>
        public static SpeechSystemStatus Status { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Delegate for OnError event.</para>
        /// </summary>
        /// <param name="errorCode">Error code for the error that occurred.</param>
        public delegate void ErrorDelegate(SpeechError errorCode);

        /// <summary>
        /// <para>Delegate for OnStatusChanged event.</para>
        /// </summary>
        /// <param name="status">The new status of the phrase recognition system.</param>
        public delegate void StatusDelegate(SpeechSystemStatus status);
    }
}

