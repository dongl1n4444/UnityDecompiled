namespace UnityEditor.U2D.Interface
{
    using System;
    using UnityEditor;

    internal interface IUndoSystem
    {
        void ClearUndo(IUndoableObject obj);
        void RegisterCompleteObjectUndo(IUndoableObject obj, string undoText);
        void RegisterUndoCallback(Undo.UndoRedoCallback undoCallback);
        void UnregisterUndoCallback(Undo.UndoRedoCallback undoCallback);
    }
}

