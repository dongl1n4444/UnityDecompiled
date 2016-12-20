namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [InitializeOnLoad]
    internal class DrivenRectTransformUndo
    {
        [CompilerGenerated]
        private static Undo.WillFlushUndoRecord <>f__mg$cache0;
        [CompilerGenerated]
        private static Undo.UndoRedoCallback <>f__mg$cache1;

        static DrivenRectTransformUndo()
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Undo.WillFlushUndoRecord(DrivenRectTransformUndo.ForceUpdateCanvases);
            }
            Undo.willFlushUndoRecord = (Undo.WillFlushUndoRecord) Delegate.Combine(Undo.willFlushUndoRecord, <>f__mg$cache0);
            if (<>f__mg$cache1 == null)
            {
                <>f__mg$cache1 = new Undo.UndoRedoCallback(DrivenRectTransformUndo.ForceUpdateCanvases);
            }
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, <>f__mg$cache1);
        }

        private static void ForceUpdateCanvases()
        {
            Canvas.ForceUpdateCanvases();
        }
    }
}

