namespace UnityEditor.Build
{
    using System;
    using UnityEditor;

    public interface IPreprocessBuild : IOrderedCallback
    {
        /// <summary>
        /// <para>Implement this function to receive a callback before the build is started.</para>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="path"></param>
        void OnPreprocessBuild(BuildTarget target, string path);
    }
}

