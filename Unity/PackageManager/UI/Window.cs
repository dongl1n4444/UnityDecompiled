namespace Unity.PackageManager.UI
{
    using System;
    using Unity.PackageManager;
    using Unity.PackageManager.Ivy;
    using UnityEditor;
    using UnityEngine;

    internal class Window : EditorWindow
    {
        [SerializeField]
        private InfoView infoView;
        private const float kListViewHeight = 200f;
        private static readonly string[] kNames = new string[] { "External", "Internal", "Custom" };
        public const string kTaskName = "ReleaseNotes";
        private static readonly string[] kValues = new string[] { "external", "internal", "custom" };
        private static Vector2 kWindowSize = new Vector2(500f, 400f);
        [SerializeField]
        private ListView listView;
        private bool m_PrefsRead;
        private string m_Repo;
        private string m_RepoUrl;
        [NonSerialized]
        private bool m_RequestRepaint;

        private void ApplicationUpdate()
        {
            if (this.m_RequestRepaint)
            {
                base.Repaint();
                this.m_RequestRepaint = false;
            }
        }

        private void ChannelSelector()
        {
            if (!this.m_PrefsRead)
            {
                this.ReadPreferences();
                this.m_PrefsRead = true;
            }
            int index = Array.IndexOf<string>(kValues, this.m_Repo);
            if (index < 0)
            {
                index = 0;
            }
            EditorGUI.BeginChangeCheck();
            GUILayout.Space(5f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUIUtility.labelWidth = 70f;
            index = EditorGUILayout.Popup("Repository", index, kNames, new GUILayoutOption[0]);
            this.m_Repo = kValues[index];
            GUILayout.Space(10f);
            if (this.m_Repo == "custom")
            {
                if (GUI.changed)
                {
                    this.m_RepoUrl = string.Empty;
                }
                EditorGUIUtility.labelWidth = 30f;
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(200f) };
                this.m_RepoUrl = EditorGUILayout.TextField("URL", this.m_RepoUrl, options);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            if (EditorGUI.EndChangeCheck())
            {
                this.WritePreferences();
                if (this.m_Repo != "custom")
                {
                    Unity.PackageManager.PackageManager.Instance.CheckForRemoteUpdates();
                }
            }
        }

        private void HandleOnTask(Task task)
        {
            if (task.Name == "ReleaseNotes")
            {
                this.m_RequestRepaint = true;
            }
        }

        private void HandleOnUpdate(IvyModule[] modules)
        {
            this.m_RequestRepaint = true;
        }

        private void InitViews()
        {
            if (this.listView == null)
            {
                this.listView = new ListView();
                Unity.PackageManager.PackageManager.Instance.CheckForRemoteUpdates();
            }
            if (this.infoView == null)
            {
                this.infoView = new InfoView();
            }
        }

        public void OnDisable()
        {
            Unity.PackageManager.PackageManager.Database.OnUpdateAvailable -= new Action<IvyModule[]>(this.HandleOnUpdate);
            Unity.PackageManager.PackageManager.Database.OnUpdateLocal -= new Action<IvyModule[]>(this.HandleOnUpdate);
            Unity.PackageManager.PackageManager.Instance.OnTask -= new Action<Task>(this.HandleOnTask);
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.ApplicationUpdate));
        }

        public void OnEnable()
        {
            Unity.PackageManager.PackageManager.Database.OnUpdateAvailable += new Action<IvyModule[]>(this.HandleOnUpdate);
            Unity.PackageManager.PackageManager.Database.OnUpdateLocal += new Action<IvyModule[]>(this.HandleOnUpdate);
            Unity.PackageManager.PackageManager.Instance.OnTask += new Action<Task>(this.HandleOnTask);
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.ApplicationUpdate));
            this.InitViews();
        }

        public void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0f, 0f, kWindowSize.x, kWindowSize.y));
            if (Unity.PackageManager.PackageManager.Instance == null)
            {
                GUILayout.Box(new GUIContent("Package Manager Unavailable"), Styles.debugBox, new GUILayoutOption[0]);
            }
            else
            {
                if (Unsupported.IsDeveloperBuild())
                {
                    this.ChannelSelector();
                }
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(200f), GUILayout.ExpandWidth(true) };
                GUILayout.BeginVertical(options);
                this.listView.parent = this;
                this.listView.OnGUI();
                GUILayout.EndVertical();
                if (Event.current.type == EventType.Layout)
                {
                    this.infoView.package = this.listView.SelectedPackage;
                }
                this.infoView.OnGUI();
                GUILayout.EndArea();
            }
        }

        public void ReadPreferences()
        {
            this.m_Repo = Settings.repoType;
            this.m_RepoUrl = Settings.baseRepoUrl;
        }

        public static void ShowPackageManagerWindow()
        {
            Window window = EditorWindow.GetWindow<Window>(true, "Module Manager");
            Vector2 kWindowSize = Window.kWindowSize;
            window.maxSize = kWindowSize;
            window.minSize = kWindowSize;
        }

        public void WritePreferences()
        {
            this.m_RepoUrl = Settings.SelectRepo(this.m_Repo, this.m_RepoUrl);
            Settings.CacheAllSettings();
        }
    }
}

