namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>This class is used for entries in the Scenes list, as displayed in the window. This class contains the scene path of a scene and an enabled flag that indicates wether the scene is enabled in the BuildSettings window or not. 
    /// 
    /// You can use this class in combination with EditorBuildSettings.scenes to populate the list of Scenes included in the build via script. This is useful when creating custom editor scripts to automate your build pipeline.
    /// 
    /// See EditorBuildSettings.scenes for an example script.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class EditorBuildSettingsScene : IComparable
    {
        private int m_Enabled;
        private string m_Path;
        [CompilerGenerated]
        private static Func<EditorBuildSettingsScene, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<EditorBuildSettingsScene, string> <>f__am$cache1;
        public EditorBuildSettingsScene(string path, bool enable)
        {
            this.m_Path = path.Replace(@"\", "/");
            this.enabled = enable;
        }

        public EditorBuildSettingsScene()
        {
        }

        public int CompareTo(object obj)
        {
            if (!(obj is EditorBuildSettingsScene))
            {
                throw new ArgumentException("object is not a EditorBuildSettingsScene");
            }
            EditorBuildSettingsScene scene = (EditorBuildSettingsScene) obj;
            return scene.m_Path.CompareTo(this.m_Path);
        }

        /// <summary>
        /// <para>Whether this scene is enabled in the for an example of how to use this class.
        /// 
        /// See Also: EditorBuildSettingsScene, EditorBuildSettings.scenes.</para>
        /// </summary>
        public bool enabled
        {
            get => 
                (this.m_Enabled != 0);
            set
            {
                this.m_Enabled = !value ? 0 : 1;
            }
        }
        /// <summary>
        /// <para>The file path of the scene as listed in the Editor for an example of how to use this class.
        /// 
        /// See Also: EditorBuildSettingsScene, EditorBuildSettings.scenes.</para>
        /// </summary>
        public string path
        {
            get => 
                this.m_Path;
            set
            {
                this.m_Path = value.Replace(@"\", "/");
            }
        }
        public static string[] GetActiveSceneList(EditorBuildSettingsScene[] scenes)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = scene => scene.enabled;
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = scene => scene.path;
            }
            return Enumerable.Select<EditorBuildSettingsScene, string>(Enumerable.Where<EditorBuildSettingsScene>(scenes, <>f__am$cache0), <>f__am$cache1).ToArray<string>();
        }
    }
}

