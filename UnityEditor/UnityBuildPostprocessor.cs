namespace UnityEditor
{
    using System;
    using UnityEditor.Build;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    internal class UnityBuildPostprocessor : IProcessScene, IOrderedCallback
    {
        public void OnProcessScene(Scene scene)
        {
            int num;
            int num2;
            PlayerSettings.GetBatchingForPlatform(EditorUserBuildSettings.activeBuildTarget, out num, out num2);
            if (num != 0)
            {
                InternalStaticBatchingUtility.Combine(null, true, true);
            }
        }

        public int callbackOrder =>
            0;
    }
}

