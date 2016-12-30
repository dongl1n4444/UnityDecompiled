namespace UnityEditor.Build
{
    using System;
    using UnityEngine.SceneManagement;

    public interface IProcessScene : IOrderedCallback
    {
        /// <summary>
        /// <para>Implement this function to receive a callback for each Scene during the build.</para>
        /// </summary>
        /// <param name="scene">The current Scene being processed.</param>
        void OnProcessScene(Scene scene);
    }
}

