namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Editor drag &amp; drop operations.</para>
    /// </summary>
    public sealed class DragAndDrop
    {
        private static Hashtable ms_GenericData;

        /// <summary>
        /// <para>Accept a drag operation.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void AcceptDrag();
        /// <summary>
        /// <para>Get data associated with current drag and drop operation.</para>
        /// </summary>
        /// <param name="type"></param>
        public static object GetGenericData(string type)
        {
            if ((ms_GenericData != null) && ms_GenericData.Contains(type))
            {
                return ms_GenericData[type];
            }
            return null;
        }

        internal static bool HandleDelayedDrag(Rect position, int id, UnityEngine.Object objectToDrag)
        {
            Event current = Event.current;
            EventType typeForControl = current.GetTypeForControl(id);
            if (typeForControl != EventType.MouseDown)
            {
                if ((typeForControl == EventType.MouseDrag) && (GUIUtility.hotControl == id))
                {
                    DragAndDropDelay stateObject = (DragAndDropDelay) GUIUtility.GetStateObject(typeof(DragAndDropDelay), id);
                    if (stateObject.CanStartDrag())
                    {
                        GUIUtility.hotControl = 0;
                        PrepareStartDrag();
                        UnityEngine.Object[] objArray = new UnityEngine.Object[] { objectToDrag };
                        objectReferences = objArray;
                        StartDrag(ObjectNames.GetDragAndDropTitle(objectToDrag));
                        return true;
                    }
                }
            }
            else if ((position.Contains(current.mousePosition) && (current.clickCount == 1)) && ((current.button == 0) && ((Application.platform != RuntimePlatform.OSXEditor) || !current.control)))
            {
                GUIUtility.hotControl = id;
                DragAndDropDelay delay = (DragAndDropDelay) GUIUtility.GetStateObject(typeof(DragAndDropDelay), id);
                delay.mouseDownPosition = current.mousePosition;
                return true;
            }
            return false;
        }

        /// <summary>
        /// <para>Clears drag &amp; drop data.</para>
        /// </summary>
        public static void PrepareStartDrag()
        {
            ms_GenericData = null;
            PrepareStartDrag_Internal();
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void PrepareStartDrag_Internal();
        /// <summary>
        /// <para>Set data associated with current drag and drop operation.</para>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public static void SetGenericData(string type, object data)
        {
            if (ms_GenericData == null)
            {
                ms_GenericData = new Hashtable();
            }
            ms_GenericData[type] = data;
        }

        /// <summary>
        /// <para>Start a drag operation.</para>
        /// </summary>
        /// <param name="title"></param>
        public static void StartDrag(string title)
        {
            if ((Event.current.type == EventType.MouseDown) || (Event.current.type == EventType.MouseDrag))
            {
                StartDrag_Internal(title);
            }
            else
            {
                Debug.LogError("Drags can only be started from MouseDown or MouseDrag events");
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void StartDrag_Internal(string title);

        /// <summary>
        /// <para>Get or set ID of currently active drag and drop control.</para>
        /// </summary>
        public static int activeControlID { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>References to Object|objects being dragged.</para>
        /// </summary>
        public static UnityEngine.Object[] objectReferences { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The file names being dragged.</para>
        /// </summary>
        public static string[] paths { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The visual indication of the drag.</para>
        /// </summary>
        public static DragAndDropVisualMode visualMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

