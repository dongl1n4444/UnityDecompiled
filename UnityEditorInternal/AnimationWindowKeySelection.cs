namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    internal class AnimationWindowKeySelection : ScriptableObject, ISerializationCallbackReceiver
    {
        private HashSet<int> m_SelectedKeyHashes;
        [SerializeField]
        private List<int> m_SelectedKeyHashesSerialized;

        public void OnAfterDeserialize()
        {
            this.m_SelectedKeyHashes = new HashSet<int>(this.m_SelectedKeyHashesSerialized);
        }

        public void OnBeforeSerialize()
        {
            this.m_SelectedKeyHashesSerialized = this.m_SelectedKeyHashes.ToList<int>();
        }

        public void SaveSelection(string undoLabel)
        {
            Undo.RegisterCompleteObjectUndo(this, undoLabel);
        }

        public HashSet<int> selectedKeyHashes
        {
            get
            {
                HashSet<int> selectedKeyHashes;
                if (this.m_SelectedKeyHashes != null)
                {
                    selectedKeyHashes = this.m_SelectedKeyHashes;
                }
                else
                {
                    selectedKeyHashes = this.m_SelectedKeyHashes = new HashSet<int>();
                }
                return selectedKeyHashes;
            }
            set
            {
                this.m_SelectedKeyHashes = value;
            }
        }
    }
}

