namespace UnityEngine.WSA
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Provides essential methods related to Window Store application.</para>
    /// </summary>
    public sealed class Application
    {
        public static  event WindowActivated windowActivated;

        public static  event WindowSizeChanged windowSizeChanged;

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern string GetAdvertisingIdentifier();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern string GetAppArguments();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
        internal static extern bool InternalTryInvokeOnAppThread(AppCallbackItem item, bool waitUntilDone);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        internal static extern bool InternalTryInvokeOnUIThread(AppCallbackItem item, bool waitUntilDone);
        /// <summary>
        /// <para>Executes callback item on application thread.</para>
        /// </summary>
        /// <param name="item">Item to execute.</param>
        /// <param name="waitUntilDone">Wait until item is executed.</param>
        public static void InvokeOnAppThread(AppCallbackItem item, bool waitUntilDone)
        {
            item();
        }

        /// <summary>
        /// <para>Executes callback item on UI thread.</para>
        /// </summary>
        /// <param name="item">Item to execute.</param>
        /// <param name="waitUntilDone">Wait until item is executed.</param>
        public static void InvokeOnUIThread(AppCallbackItem item, bool waitUntilDone)
        {
            item();
        }

        internal static void InvokeWindowActivatedEvent(WindowActivationState state)
        {
            if (windowActivated != null)
            {
                windowActivated(state);
            }
        }

        internal static void InvokeWindowSizeChangedEvent(int width, int height)
        {
            if (windowSizeChanged != null)
            {
                windowSizeChanged(width, height);
            }
        }

        /// <summary>
        /// <para>Returns true if you're running on application thread.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        public static extern bool RunningOnAppThread();
        /// <summary>
        /// <para>Returns true if you're running on UI thread.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        public static extern bool RunningOnUIThread();
        /// <summary>
        /// <para>[OBSOLETE] Tries to execute callback item on application thread.</para>
        /// </summary>
        /// <param name="item">Item to execute.</param>
        /// <param name="waitUntilDone">Wait until item is executed.</param>
        [Obsolete("TryInvokeOnAppThread is deprecated, use InvokeOnAppThread")]
        public static bool TryInvokeOnAppThread(AppCallbackItem item, bool waitUntilDone)
        {
            item();
            return true;
        }

        /// <summary>
        /// <para>[OBSOLETE] Tries to execute callback item on UI thread.</para>
        /// </summary>
        /// <param name="item">Item to execute.</param>
        /// <param name="waitUntilDone">Wait until item is executed.</param>
        [Obsolete("TryInvokeOnUIThread is deprecated, use InvokeOnUIThread")]
        public static bool TryInvokeOnUIThread(AppCallbackItem item, bool waitUntilDone)
        {
            item();
            return true;
        }

        /// <summary>
        /// <para>Advertising ID.</para>
        /// </summary>
        public static string advertisingIdentifier
        {
            get
            {
                string advertisingIdentifier = GetAdvertisingIdentifier();
                UnityEngine.Application.InvokeOnAdvertisingIdentifierCallback(advertisingIdentifier, true);
                return advertisingIdentifier;
            }
        }

        /// <summary>
        /// <para>Arguments passed to application.</para>
        /// </summary>
        public static string arguments =>
            GetAppArguments();
    }
}

