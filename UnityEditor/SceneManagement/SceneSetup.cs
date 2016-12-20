namespace UnityEditor.SceneManagement
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>The setup information for a scene in the SceneManager.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public class SceneSetup
    {
        [SerializeField]
        private string m_Path = null;
        [SerializeField]
        private bool m_IsLoaded = false;
        [SerializeField]
        private bool m_IsActive = false;
        /// <summary>
        /// <para>Path of the scene. Should be relative to the project folder. Like: "AssetsMyScenesMyScene.unity".</para>
        /// </summary>
        public string path
        {
            get
            {
                return this.m_Path;
            }
            set
            {
                this.m_Path = value;
            }
        }
        /// <summary>
        /// <para>If the scene is loaded.</para>
        /// </summary>
        public bool isLoaded
        {
            get
            {
                return this.m_IsLoaded;
            }
            set
            {
                this.m_IsLoaded = value;
            }
        }
        /// <summary>
        /// <para>If the scene is active.</para>
        /// </summary>
        public bool isActive
        {
            get
            {
                return this.m_IsActive;
            }
            set
            {
                this.m_IsActive = value;
            }
        }
    }
}

