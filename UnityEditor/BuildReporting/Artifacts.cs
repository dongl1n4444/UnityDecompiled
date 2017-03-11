namespace UnityEditor.BuildReporting
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class Artifacts : ScriptableObject
    {
        public bool isUploading = false;
        public string[] paths = new string[0];
        public Progress progress = new Progress();

        public void SetProgress(string progressJson)
        {
            this.progress = JsonUtility.FromJson<Progress>(progressJson);
            if (this.progress.success || !string.IsNullOrEmpty(this.progress.status))
            {
                this.isUploading = false;
                if (!this.progress.success)
                {
                    Debug.LogError("Uploading build to Facebook failed: " + this.progress.status);
                }
            }
            UnityEngine.Object[] objArray = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(BuildPlayerWindow));
            foreach (BuildPlayerWindow window in objArray)
            {
                window.Repaint();
            }
        }

        public void StartUpload()
        {
            this.isUploading = true;
            this.progress = new Progress();
        }

        [Serializable]
        public class Progress
        {
            public float progress = 0f;
            public string status = null;
            public bool success = false;
        }
    }
}

