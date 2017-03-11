namespace UnityEditor.Build
{
    using System;
    using UnityEditor;

    public interface IActiveBuildTargetChanged : IOrderedCallback
    {
        /// <summary>
        /// <para>This function is called automatically when the active build platform has changed.</para>
        /// </summary>
        /// <param name="previousTarget">The build target before the change.</param>
        /// <param name="newTarget">The new active build target.</param>
        void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget);
    }
}

