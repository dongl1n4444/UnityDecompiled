namespace UnityEditor.Build
{
    using System;
    using UnityEditor;

    public interface IPostprocessBuild : IOrderedCallback
    {
        /// <summary>
        /// <para>Implement this function to receive a callback after the build is complete.</para>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="path"></param>
        void OnPostprocessBuild(BuildTarget target, string path);
    }
}

