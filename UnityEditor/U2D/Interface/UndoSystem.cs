namespace UnityEditor.U2D.Interface
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class UndoSystem : IUndoSystem
    {
        private ScriptableObject CheckUndoObjectType(IUndoableObject obj)
        {
            ScriptableObject obj2 = obj as ScriptableObject;
            if (obj2 == null)
            {
                Debug.LogError("Register Undo object is not a ScriptableObject");
            }
            return obj2;
        }

        public void ClearUndo(IUndoableObject obj)
        {
            ScriptableObject identifier = this.CheckUndoObjectType(obj);
            if (identifier != null)
            {
                Undo.ClearUndo(identifier);
            }
        }

        public void RegisterCompleteObjectUndo(IUndoableObject obj, string undoText)
        {
            ScriptableObject objectToUndo = this.CheckUndoObjectType(obj);
            if (objectToUndo != null)
            {
                Undo.RegisterCompleteObjectUndo(objectToUndo, undoText);
            }
        }

        public void RegisterUndoCallback(Undo.UndoRedoCallback undoCallback)
        {
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, undoCallback);
        }

        public void UnregisterUndoCallback(Undo.UndoRedoCallback undoCallback)
        {
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, undoCallback);
        }
    }
}

