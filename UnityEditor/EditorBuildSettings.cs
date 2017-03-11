namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>This class allows you to modify the Editor for an example of how to use this class.
    /// 
    /// See Also: EditorBuildSettingsScene, EditorBuildSettings.scenes.</para>
    /// </summary>
    public sealed class EditorBuildSettings
    {
        /// <summary>
        /// <para>The list of Scenes that should be included in the build.
        /// This is the same list of Scenes that is shown in the window. You can modify this list to set up which Scenes should be included in the build.</para>
        /// </summary>
        public static EditorBuildSettingsScene[] scenes { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

