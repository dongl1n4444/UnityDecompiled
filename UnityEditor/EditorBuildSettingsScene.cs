namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

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

        public bool enabled
        {
            get => 
                (this.m_Enabled != 0);
            set
            {
                this.m_Enabled = !value ? 0 : 1;
            }
        }
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

