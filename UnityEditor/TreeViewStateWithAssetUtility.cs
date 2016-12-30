namespace UnityEditor
{
    using System;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    [Serializable]
    internal class TreeViewStateWithAssetUtility : TreeViewState
    {
        [SerializeField]
        private CreateAssetUtility m_CreateAssetUtility = new CreateAssetUtility();

        internal override void OnAwake()
        {
            base.OnAwake();
            this.m_CreateAssetUtility.Clear();
        }

        internal CreateAssetUtility createAssetUtility
        {
            get => 
                this.m_CreateAssetUtility;
            set
            {
                this.m_CreateAssetUtility = value;
            }
        }
    }
}

