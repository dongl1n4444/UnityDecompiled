namespace UnityEngine.VR.WSA.Input
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Manager class with API for recognizing user gestures.</para>
    /// </summary>
    public sealed class GestureRecognizer : IDisposable
    {
        private IntPtr m_Recognizer;

        public event GestureErrorDelegate GestureErrorEvent;

        public event HoldCanceledEventDelegate HoldCanceledEvent;

        public event HoldCompletedEventDelegate HoldCompletedEvent;

        public event HoldStartedEventDelegate HoldStartedEvent;

        public event ManipulationCanceledEventDelegate ManipulationCanceledEvent;

        public event ManipulationCompletedEventDelegate ManipulationCompletedEvent;

        public event ManipulationStartedEventDelegate ManipulationStartedEvent;

        public event ManipulationUpdatedEventDelegate ManipulationUpdatedEvent;

        public event NavigationCanceledEventDelegate NavigationCanceledEvent;

        public event NavigationCompletedEventDelegate NavigationCompletedEvent;

        public event NavigationStartedEventDelegate NavigationStartedEvent;

        public event NavigationUpdatedEventDelegate NavigationUpdatedEvent;

        public event RecognitionEndedEventDelegate RecognitionEndedEvent;

        public event RecognitionStartedEventDelegate RecognitionStartedEvent;

        public event TappedEventDelegate TappedEvent;

        /// <summary>
        /// <para>Create a GestureRecognizer.</para>
        /// </summary>
        public GestureRecognizer()
        {
            this.m_Recognizer = this.Internal_Create();
        }

        /// <summary>
        /// <para>Cancels any pending gesture events.  Additionally this will call StopCapturingGestures.</para>
        /// </summary>
        public void CancelGestures()
        {
            if (this.m_Recognizer != IntPtr.Zero)
            {
                this.Internal_CancelGestures(this.m_Recognizer);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Destroy(IntPtr recognizer);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private static extern void DestroyThreaded(IntPtr recognizer);
        /// <summary>
        /// <para>Disposes the resources used by gesture recognizer.</para>
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

        ~GestureRecognizer()
        {
            if (this.m_Recognizer != IntPtr.Zero)
            {
                DestroyThreaded(this.m_Recognizer);
                this.m_Recognizer = IntPtr.Zero;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// <para>Retrieve a mask of the currently enabled gestures.</para>
        /// </summary>
        /// <returns>
        /// <para>A mask indicating which Gestures are currently recognizable.</para>
        /// </returns>
        public GestureSettings GetRecognizableGestures()
        {
            if (this.m_Recognizer != IntPtr.Zero)
            {
                return (GestureSettings) this.Internal_GetRecognizableGestures(this.m_Recognizer);
            }
            return GestureSettings.None;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_Create(GestureRecognizer self, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Internal_CancelGestures(IntPtr recognizer);
        private IntPtr Internal_Create()
        {
            IntPtr ptr;
            INTERNAL_CALL_Internal_Create(this, out ptr);
            return ptr;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int Internal_GetRecognizableGestures(IntPtr recognizer);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool Internal_IsCapturingGestures(IntPtr recognizer);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int Internal_SetRecognizableGestures(IntPtr recognizer, int newMaskValue);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Internal_StartCapturingGestures(IntPtr recognizer);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Internal_StopCapturingGestures(IntPtr recognizer);
        [RequiredByNativeCode]
        private void InvokeErrorEvent(string error, int hresult)
        {
            GestureErrorDelegate gestureErrorEvent = this.GestureErrorEvent;
            if (gestureErrorEvent != null)
            {
                gestureErrorEvent(error, hresult);
            }
        }

        [RequiredByNativeCode]
        private void InvokeHoldEvent(GestureEventType eventType, InteractionSourceKind source, Ray headRay)
        {
            switch (eventType)
            {
                case GestureEventType.HoldCanceled:
                {
                    HoldCanceledEventDelegate holdCanceledEvent = this.HoldCanceledEvent;
                    if (holdCanceledEvent != null)
                    {
                        holdCanceledEvent(source, headRay);
                    }
                    break;
                }
                case GestureEventType.HoldCompleted:
                {
                    HoldCompletedEventDelegate holdCompletedEvent = this.HoldCompletedEvent;
                    if (holdCompletedEvent != null)
                    {
                        holdCompletedEvent(source, headRay);
                    }
                    break;
                }
                case GestureEventType.HoldStarted:
                {
                    HoldStartedEventDelegate holdStartedEvent = this.HoldStartedEvent;
                    if (holdStartedEvent != null)
                    {
                        holdStartedEvent(source, headRay);
                    }
                    break;
                }
                default:
                    throw new ArgumentException("InvokeHoldEvent: Invalid GestureEventType");
            }
        }

        [RequiredByNativeCode]
        private void InvokeManipulationEvent(GestureEventType eventType, InteractionSourceKind source, Vector3 position, Ray headRay)
        {
            switch (eventType)
            {
                case GestureEventType.ManipulationCanceled:
                {
                    ManipulationCanceledEventDelegate manipulationCanceledEvent = this.ManipulationCanceledEvent;
                    if (manipulationCanceledEvent != null)
                    {
                        manipulationCanceledEvent(source, position, headRay);
                    }
                    break;
                }
                case GestureEventType.ManipulationCompleted:
                {
                    ManipulationCompletedEventDelegate manipulationCompletedEvent = this.ManipulationCompletedEvent;
                    if (manipulationCompletedEvent != null)
                    {
                        manipulationCompletedEvent(source, position, headRay);
                    }
                    break;
                }
                case GestureEventType.ManipulationStarted:
                {
                    ManipulationStartedEventDelegate manipulationStartedEvent = this.ManipulationStartedEvent;
                    if (manipulationStartedEvent != null)
                    {
                        manipulationStartedEvent(source, position, headRay);
                    }
                    break;
                }
                case GestureEventType.ManipulationUpdated:
                {
                    ManipulationUpdatedEventDelegate manipulationUpdatedEvent = this.ManipulationUpdatedEvent;
                    if (manipulationUpdatedEvent != null)
                    {
                        manipulationUpdatedEvent(source, position, headRay);
                    }
                    break;
                }
                default:
                    throw new ArgumentException("InvokeManipulationEvent: Invalid GestureEventType");
            }
        }

        [RequiredByNativeCode]
        private void InvokeNavigationEvent(GestureEventType eventType, InteractionSourceKind source, Vector3 relativePosition, Ray headRay)
        {
            switch (eventType)
            {
                case GestureEventType.NavigationCanceled:
                {
                    NavigationCanceledEventDelegate navigationCanceledEvent = this.NavigationCanceledEvent;
                    if (navigationCanceledEvent != null)
                    {
                        navigationCanceledEvent(source, relativePosition, headRay);
                    }
                    break;
                }
                case GestureEventType.NavigationCompleted:
                {
                    NavigationCompletedEventDelegate navigationCompletedEvent = this.NavigationCompletedEvent;
                    if (navigationCompletedEvent != null)
                    {
                        navigationCompletedEvent(source, relativePosition, headRay);
                    }
                    break;
                }
                case GestureEventType.NavigationStarted:
                {
                    NavigationStartedEventDelegate navigationStartedEvent = this.NavigationStartedEvent;
                    if (navigationStartedEvent != null)
                    {
                        navigationStartedEvent(source, relativePosition, headRay);
                    }
                    break;
                }
                case GestureEventType.NavigationUpdated:
                {
                    NavigationUpdatedEventDelegate navigationUpdatedEvent = this.NavigationUpdatedEvent;
                    if (navigationUpdatedEvent != null)
                    {
                        navigationUpdatedEvent(source, relativePosition, headRay);
                    }
                    break;
                }
                default:
                    throw new ArgumentException("InvokeNavigationEvent: Invalid GestureEventType");
            }
        }

        [RequiredByNativeCode]
        private void InvokeRecognitionEvent(GestureEventType eventType, InteractionSourceKind source, Ray headRay)
        {
            if (eventType != GestureEventType.RecognitionEnded)
            {
                if (eventType != GestureEventType.RecognitionStarted)
                {
                    throw new ArgumentException("InvokeRecognitionEvent: Invalid GestureEventType");
                }
            }
            else
            {
                RecognitionEndedEventDelegate recognitionEndedEvent = this.RecognitionEndedEvent;
                if (recognitionEndedEvent != null)
                {
                    recognitionEndedEvent(source, headRay);
                }
                return;
            }
            RecognitionStartedEventDelegate recognitionStartedEvent = this.RecognitionStartedEvent;
            if (recognitionStartedEvent != null)
            {
                recognitionStartedEvent(source, headRay);
            }
        }

        [RequiredByNativeCode]
        private void InvokeTapEvent(InteractionSourceKind source, Ray headRay, int tapCount)
        {
            TappedEventDelegate tappedEvent = this.TappedEvent;
            if (tappedEvent != null)
            {
                tappedEvent(source, tapCount, headRay);
            }
        }

        /// <summary>
        /// <para>Used to query if the GestureRecognizer is currently receiving Gesture events.</para>
        /// </summary>
        /// <returns>
        /// <para>True if the GestureRecognizer is receiving events or false otherwise.</para>
        /// </returns>
        public bool IsCapturingGestures() => 
            ((this.m_Recognizer != IntPtr.Zero) && this.Internal_IsCapturingGestures(this.m_Recognizer));

        /// <summary>
        /// <para>Set the recognizable gestures to the ones specified in newMaskValues and return the old settings.</para>
        /// </summary>
        /// <param name="newMaskValue">A mask indicating which gestures are now recognizable.</param>
        /// <returns>
        /// <para>The previous value.</para>
        /// </returns>
        public GestureSettings SetRecognizableGestures(GestureSettings newMaskValue)
        {
            if (this.m_Recognizer != IntPtr.Zero)
            {
                return (GestureSettings) this.Internal_SetRecognizableGestures(this.m_Recognizer, (int) newMaskValue);
            }
            return GestureSettings.None;
        }

        /// <summary>
        /// <para>Call to begin receiving gesture events on this recognizer.  No events will be received until this method is called.</para>
        /// </summary>
        public void StartCapturingGestures()
        {
            if (this.m_Recognizer != IntPtr.Zero)
            {
                this.Internal_StartCapturingGestures(this.m_Recognizer);
            }
        }

        /// <summary>
        /// <para>Call to stop receiving gesture events on this recognizer.</para>
        /// </summary>
        public void StopCapturingGestures()
        {
            if (this.m_Recognizer != IntPtr.Zero)
            {
                this.Internal_StopCapturingGestures(this.m_Recognizer);
            }
        }

        /// <summary>
        /// <para>Callback indicating an error or warning occurred.</para>
        /// </summary>
        /// <param name="error">A readable error string (when possible).</param>
        /// <param name="hresult">The HRESULT code from the platform.</param>
        public delegate void GestureErrorDelegate([MarshalAs(UnmanagedType.LPStr)] string error, int hresult);

        private enum GestureEventType
        {
            InteractionDetected,
            HoldCanceled,
            HoldCompleted,
            HoldStarted,
            TapDetected,
            ManipulationCanceled,
            ManipulationCompleted,
            ManipulationStarted,
            ManipulationUpdated,
            NavigationCanceled,
            NavigationCompleted,
            NavigationStarted,
            NavigationUpdated,
            RecognitionStarted,
            RecognitionEnded
        }

        /// <summary>
        /// <para>Callback indicating a cancel event.</para>
        /// </summary>
        /// <param name="source">Indicates which input medium triggered this event.</param>
        /// <param name="headRay">Ray (with normalized direction) from user at the time this gesture began.</param>
        public delegate void HoldCanceledEventDelegate(InteractionSourceKind source, Ray headRay);

        /// <summary>
        /// <para>Callback indicating a hold completed event.</para>
        /// </summary>
        /// <param name="source">Indicates which input medium triggered this event.</param>
        /// <param name="headRay">Ray (with normalized direction) from user at the time this gesture began.</param>
        public delegate void HoldCompletedEventDelegate(InteractionSourceKind source, Ray headRay);

        /// <summary>
        /// <para>Callback indicating a hold started event.</para>
        /// </summary>
        /// <param name="source">Indicates which input medium triggered this event.</param>
        /// <param name="headRay">Ray (with normalized direction) from user at the time this gesture began.</param>
        public delegate void HoldStartedEventDelegate(InteractionSourceKind source, Ray headRay);

        /// <summary>
        /// <para>Callback indicating a cancel event.</para>
        /// </summary>
        /// <param name="source">Indicates which input medium triggered this event.</param>
        /// <param name="cumulativeDelta">Total distance moved since the beginning of the manipulation gesture.</param>
        /// <param name="headRay">Ray (with normalized direction) from user at the time this gesture began.</param>
        public delegate void ManipulationCanceledEventDelegate(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay);

        /// <summary>
        /// <para>Callback indicating a completed event.</para>
        /// </summary>
        /// <param name="source">Indicates which input medium triggered this event.</param>
        /// <param name="cumulativeDelta">Total distance moved since the beginning of the manipulation gesture.</param>
        /// <param name="headRay">Ray (with normalized direction) from user at the time this gesture began.</param>
        public delegate void ManipulationCompletedEventDelegate(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay);

        /// <summary>
        /// <para>Callback indicating a started event.</para>
        /// </summary>
        /// <param name="source">Indicates which input medium triggered this event.</param>
        /// <param name="cumulativeDelta">Total distance moved since the beginning of the manipulation gesture.</param>
        /// <param name="headRay">Ray (with normalized direction) from user at the time this gesture began.</param>
        public delegate void ManipulationStartedEventDelegate(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay);

        /// <summary>
        /// <para>Callback indicating a updated event.</para>
        /// </summary>
        /// <param name="source">Indicates which input medium triggered this event.</param>
        /// <param name="cumulativeDelta">Total distance moved since the beginning of the manipulation gesture.</param>
        /// <param name="headRay">Ray (with normalized direction) from user at the time this gesture began.</param>
        public delegate void ManipulationUpdatedEventDelegate(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay);

        /// <summary>
        /// <para>Callback indicating a cancel event.</para>
        /// </summary>
        /// <param name="source">Indicates which input medium triggered this event.</param>
        /// <param name="normalizedOffset">The last known normalized offset of the input within the unit cube for the navigation gesture.</param>
        /// <param name="headRay">Ray (with normalized direction) from user at the time this gesture began.</param>
        public delegate void NavigationCanceledEventDelegate(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay);

        /// <summary>
        /// <para>Callback indicating a completed event.</para>
        /// </summary>
        /// <param name="source">Indicates which input medium triggered this event.</param>
        /// <param name="normalizedOffset">The last known normalized offset, since the navigation gesture began, of the input within the unit cube for the navigation gesture.</param>
        /// <param name="headRay">Ray (with normalized direction) from user at the time this gesture began.</param>
        public delegate void NavigationCompletedEventDelegate(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay);

        /// <summary>
        /// <para>Callback indicating a started event.</para>
        /// </summary>
        /// <param name="source">Indicates which input medium triggered this event.</param>
        /// <param name="normalizedOffset">The normalized offset, since the navigation gesture began, of the input within the unit cube for the navigation gesture.</param>
        /// <param name="headRay">Ray (with normalized direction) from user at the time this gesture began.</param>
        public delegate void NavigationStartedEventDelegate(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay);

        /// <summary>
        /// <para>Callback indicating a update event.</para>
        /// </summary>
        /// <param name="source">Indicates which input medium triggered this event.</param>
        /// <param name="normalizedOffset">The last known normalized offset, since the navigation gesture began, of the input within the unit cube for the navigation gesture.</param>
        /// <param name="headRay">Ray (with normalized direction) from user at the time this gesture began.</param>
        public delegate void NavigationUpdatedEventDelegate(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay);

        /// <summary>
        /// <para>Callback indicating the gesture event has completed.</para>
        /// </summary>
        /// <param name="source">Indicates which input medium triggered this event.</param>
        /// <param name="headRay">Ray (with normalized direction) from user at the time a gesture began.</param>
        public delegate void RecognitionEndedEventDelegate(InteractionSourceKind source, Ray headRay);

        /// <summary>
        /// <para>Callback indicating the gesture event has started.</para>
        /// </summary>
        /// <param name="source">Indicates which input medium triggered this event.</param>
        /// <param name="headRay">Ray (with normalized direction) from user at the time a gesture began.</param>
        public delegate void RecognitionStartedEventDelegate(InteractionSourceKind source, Ray headRay);

        /// <summary>
        /// <para>Callback indicating a tap event.</para>
        /// </summary>
        /// <param name="source">Indicates which input medium triggered this event.</param>
        /// <param name="tapCount">The count of taps (1 for single tap, 2 for double tap).</param>
        /// <param name="headRay">Ray (with normalized direction) from user at the time this event interaction began.</param>
        public delegate void TappedEventDelegate(InteractionSourceKind source, int tapCount, Ray headRay);
    }
}

