namespace UnityEngine.Advertisements.Editor
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Advertisements;

    internal sealed class Placeholder : MonoBehaviour
    {
        private bool m_AllowSkip;
        private Texture2D m_LandscapeTexture;
        private string m_PlacementId;
        private Texture2D m_PortraitTexture;
        private bool m_Showing;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event EventHandler<FinishEventArgs> OnFinish;

        public void Load(string extensionPath)
        {
            this.m_LandscapeTexture = TextureFromFile(Path.Combine(extensionPath, "Editor/Resources/Editor/landscape.jpg"));
            this.m_PortraitTexture = TextureFromFile(Path.Combine(extensionPath, "Editor/Resources/Editor/portrait.jpg"));
        }

        private void ModalWindowFunction(int id)
        {
            GUI.DrawTexture(new Rect(0f, 0f, (float) Screen.width, (float) Screen.height), (Screen.width <= Screen.height) ? this.m_PortraitTexture : this.m_LandscapeTexture, ScaleMode.ScaleAndCrop);
            if (this.m_AllowSkip && GUI.Button(new Rect(20f, 20f, 150f, 50f), "Skip"))
            {
                this.m_Showing = false;
                EventHandler<FinishEventArgs> onFinish = this.OnFinish;
                if (onFinish != null)
                {
                    onFinish(this, new FinishEventArgs(this.m_PlacementId, ShowResult.Skipped));
                }
            }
            if (GUI.Button(new Rect((float) (Screen.width - 170), 20f, 150f, 50f), "Close"))
            {
                this.m_Showing = false;
                EventHandler<FinishEventArgs> handler2 = this.OnFinish;
                if (handler2 != null)
                {
                    handler2(this, new FinishEventArgs(this.m_PlacementId, ShowResult.Finished));
                }
            }
        }

        public void OnGUI()
        {
            if (this.m_Showing)
            {
                GUI.ModalWindow(0, new Rect(0f, 0f, (float) Screen.width, (float) Screen.height), new GUI.WindowFunction(this.ModalWindowFunction), "");
            }
        }

        public void Show(string placementId, bool allowSkip)
        {
            this.m_PlacementId = placementId;
            this.m_AllowSkip = allowSkip;
            this.m_Showing = true;
        }

        private static Texture2D TextureFromFile(string filePath)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(File.ReadAllBytes(filePath));
            return tex;
        }
    }
}

