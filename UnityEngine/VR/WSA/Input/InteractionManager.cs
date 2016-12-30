namespace UnityEngine.VR.WSA.Input
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Provides access to user input from hands, controllers, and system voice commands.</para>
    /// </summary>
    public sealed class InteractionManager
    {
        [CompilerGenerated]
        private static InternalSourceEventHandler <>f__mg$cache0;
        private static InternalSourceEventHandler m_OnSourceEventHandler;

        public static  event SourceEventHandler SourceDetected;

        public static  event SourceEventHandler SourceLost;

        public static  event SourceEventHandler SourcePressed;

        public static  event SourceEventHandler SourceReleased;

        public static  event SourceEventHandler SourceUpdated;

        static InteractionManager()
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new InternalSourceEventHandler(InteractionManager.OnSourceEvent);
            }
            m_OnSourceEventHandler = <>f__mg$cache0;
            Initialize(Marshal.GetFunctionPointerForDelegate(m_OnSourceEventHandler));
        }

        /// <summary>
        /// <para>Get the current SourceState.</para>
        /// </summary>
        /// <returns>
        /// <para>An array of InteractionSourceState snapshots.</para>
        /// </returns>
        public static InteractionSourceState[] GetCurrentReading()
        {
            InteractionSourceState[] sourceStates = new InteractionSourceState[numSourceStates];
            if (sourceStates.Length > 0)
            {
                GetCurrentReading_Internal(sourceStates);
            }
            return sourceStates;
        }

        /// <summary>
        /// <para>Allows retrieving the current source states without allocating an array. The number of retrieved source states will be returned, up to a maximum of the size of the array.</para>
        /// </summary>
        /// <param name="sourceStates">An array for storing InteractionSourceState snapshots.</param>
        /// <returns>
        /// <para>The number of snapshots stored in the array, up to the size of the array.</para>
        /// </returns>
        public static int GetCurrentReading(InteractionSourceState[] sourceStates)
        {
            if (sourceStates == null)
            {
                throw new ArgumentNullException("sourceStates");
            }
            if (sourceStates.Length > 0)
            {
                return GetCurrentReading_Internal(sourceStates);
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int GetCurrentReading_Internal(InteractionSourceState[] sourceStates);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Initialize(IntPtr internalSourceEventHandler);
        private static void OnSourceEvent(EventType eventType, InteractionSourceState state)
        {
            switch (eventType)
            {
                case EventType.SourceDetected:
                {
                    SourceEventHandler sourceDetected = SourceDetected;
                    if (sourceDetected != null)
                    {
                        sourceDetected(state);
                    }
                    break;
                }
                case EventType.SourceLost:
                {
                    SourceEventHandler sourceLost = SourceLost;
                    if (sourceLost != null)
                    {
                        sourceLost(state);
                    }
                    break;
                }
                case EventType.SourceUpdated:
                {
                    SourceEventHandler sourceUpdated = SourceUpdated;
                    if (sourceUpdated != null)
                    {
                        sourceUpdated(state);
                    }
                    break;
                }
                case EventType.SourcePressed:
                {
                    SourceEventHandler sourcePressed = SourcePressed;
                    if (sourcePressed != null)
                    {
                        sourcePressed(state);
                    }
                    break;
                }
                case EventType.SourceReleased:
                {
                    SourceEventHandler sourceReleased = SourceReleased;
                    if (sourceReleased != null)
                    {
                        sourceReleased(state);
                    }
                    break;
                }
                default:
                    throw new ArgumentException("OnSourceEvent: Invalid EventType");
            }
        }

        /// <summary>
        /// <para>(Read Only) The number of InteractionSourceState snapshots available for reading with InteractionManager.GetCurrentReading.</para>
        /// </summary>
        public static int numSourceStates { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        private enum EventType
        {
            SourceDetected,
            SourceLost,
            SourceUpdated,
            SourcePressed,
            SourceReleased
        }

        private delegate void InternalSourceEventHandler(InteractionManager.EventType eventType, InteractionSourceState state);

        /// <summary>
        /// <para>Callback to handle InteractionManager events.</para>
        /// </summary>
        /// <param name="state"></param>
        public delegate void SourceEventHandler(InteractionSourceState state);
    }
}

