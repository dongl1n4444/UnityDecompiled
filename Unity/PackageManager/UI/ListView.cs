namespace Unity.PackageManager.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Unity.DataContract;
    using Unity.PackageManager;
    using Unity.PackageManager.Ivy;
    using UnityEditor;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    internal class ListView
    {
        [CompilerGenerated]
        private static Func<Unity.DataContract.PackageInfo, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Unity.DataContract.PackageInfo, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<IvyModule, Unity.DataContract.PackageInfo> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<Installer, InstallerState> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<Unity.DataContract.PackageInfo, PackageType> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<Unity.DataContract.PackageInfo, PackageType> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<IvyModule, Unity.DataContract.PackageInfo> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<IvyModule, bool> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<IvyModule, PackageVersion> <>f__am$cache8;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IEnumerable<InstallerState> <installerStates>k__BackingField;
        private Dictionary<string, AnimBool> faders = new Dictionary<string, AnimBool>();
        private static Dictionary<int, Unity.DataContract.PackageInfo> hashToPackageCache = new Dictionary<int, Unity.DataContract.PackageInfo>();
        private IEnumerable<Unity.DataContract.PackageInfo> m_AvailablePackages;
        [SerializeField]
        private bool m_ListViewShouldTakeKeyboardControl = true;
        private IEnumerable<Unity.DataContract.PackageInfo> m_LocalPackages;
        private Window m_Parent;
        [NonSerialized]
        private bool m_RefreshingRemotePackages = false;
        private static PackageNameComparer packageNameComparer;
        private static readonly Dictionary<PackageType, GUIContent> packageTypeContents;
        [SerializeField]
        private Vector2 scrollPosition;
        [SerializeField]
        private int selectedPackageHash = -1;

        static ListView()
        {
            Dictionary<PackageType, GUIContent> dictionary = new Dictionary<PackageType, GUIContent> {
                { 
                    PackageType.PackageManager,
                    new GUIContent("Module Manager")
                },
                { 
                    PackageType.PlaybackEngine,
                    new GUIContent("Playback Engines")
                },
                { 
                    PackageType.UnityExtension,
                    new GUIContent("Unity Extensions")
                },
                { 
                    PackageType.Unknown,
                    new GUIContent("Unknown")
                }
            };
            packageTypeContents = dictionary;
            packageNameComparer = new PackageNameComparer();
        }

        private static string BuildPackageText(Unity.DataContract.PackageInfo package, IEnumerable<IvyModule> allVersions)
        {
            <BuildPackageText>c__AnonStorey3 storey = new <BuildPackageText>c__AnonStorey3();
            StringBuilder builder = new StringBuilder(package.name);
            storey.loadedVersion = null;
            if (package.loaded)
            {
                storey.loadedVersion = package.version;
            }
            else
            {
                if (<>f__am$cache7 == null)
                {
                    <>f__am$cache7 = v => null != v.BasePath;
                }
                IvyModule module = Enumerable.FirstOrDefault<IvyModule>(allVersions, <>f__am$cache7);
                if (null != module)
                {
                    storey.loadedVersion = module.Version;
                }
            }
            if (null != storey.loadedVersion)
            {
                builder.AppendFormat(" {0}", storey.loadedVersion);
            }
            if ((null == storey.loadedVersion) || Enumerable.Any<IvyModule>(allVersions, new Func<IvyModule, bool>(storey.<>m__0)))
            {
                builder.AppendFormat(" (Update available)", new object[0]);
            }
            return builder.ToString();
        }

        private void CachePackageManagerState()
        {
            this.m_RefreshingRemotePackages = Unity.PackageManager.PackageManager.Instance.RemoteIndexer.IsRunning;
            this.RefreshLocalPackages();
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = p => p.ToPackageInfo().Refresh();
            }
            this.availablePackages = Enumerable.Select<IvyModule, Unity.DataContract.PackageInfo>(Unity.PackageManager.PackageManager.Database.NewOrUpdatedPackages, <>f__am$cache2);
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = i => new InstallerState { 
                    installer = i,
                    module = i.Package,
                    progress = i.Progress,
                    message = i.ProgressMessage,
                    successful = i.IsSuccessful,
                    running = i.IsRunning
                };
            }
            this.installerStates = Enumerable.Select<Installer, InstallerState>(Unity.PackageManager.PackageManager.Instance.Installers.Values, <>f__am$cache3);
            foreach (Unity.DataContract.PackageInfo info in from p in this.allListedPackages
                where !this.faders.ContainsKey(p.packageName)
                select p)
            {
                this.faders[info.packageName] = this.FaderForPackage(info);
            }
        }

        private void CancelPackage(IvyModule module)
        {
            <CancelPackage>c__AnonStorey2 storey = new <CancelPackage>c__AnonStorey2 {
                module = module
            };
            InstallerState state = Enumerable.FirstOrDefault<InstallerState>(this.installerStates, new Func<InstallerState, bool>(storey.<>m__0));
            if (state != null)
            {
                UnityEngine.Debug.Log($"Cancelling installation of {state.module.Name}");
                state.installer.Cancel(false);
            }
        }

        private void DoPackageElement(Unity.DataContract.PackageInfo package, int listsKeyboardID)
        {
            Event current = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = p => p.Version;
            }
            IEnumerable<IvyModule> allVersions = Enumerable.OrderByDescending<IvyModule, PackageVersion>(Unity.PackageManager.PackageManager.Database.AllVersionsOfPackage(package.packageName), <>f__am$cache8);
            GUIContent content = new GUIContent(BuildPackageText(package, allVersions));
            Rect position = GUILayoutUtility.GetRect(content, Unity.PackageManager.UI.Styles.listElement);
            bool on = this.SelectedPackage == package;
            AnimBool @bool = this.faders[package.packageName];
            @bool.target = on;
            if (current.GetTypeForControl(controlID) == EventType.Repaint)
            {
                Unity.PackageManager.UI.Styles.listElement.Draw(position, content, false, false, on, GUIUtility.keyboardControl == listsKeyboardID);
            }
            if (EditorGUILayout.BeginFadeGroup(@bool.faded))
            {
                int num2 = 30;
                using (IEnumerator<IvyModule> enumerator = allVersions.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        <DoPackageElement>c__AnonStorey4 storey = new <DoPackageElement>c__AnonStorey4 {
                            module = enumerator.Current
                        };
                        bool flag2 = storey.module.BasePath != null;
                        GUIContent content2 = new GUIContent($"{storey.module.Info.Version} {!flag2 ? string.Empty : "(downloaded)"}");
                        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                        GUILayout.Space((float) num2);
                        GUILayout.Label(content2, new GUILayoutOption[0]);
                        GUILayout.FlexibleSpace();
                        InstallerState state = Enumerable.FirstOrDefault<InstallerState>(this.installerStates, new Func<InstallerState, bool>(storey.<>m__0));
                        if (state != null)
                        {
                            if (!state.running && !state.successful)
                            {
                                GUILayout.Label(state.message, new GUILayoutOption[0]);
                                if (GUILayout.Button(Unity.PackageManager.UI.Styles.okButtonContent, new GUILayoutOption[0]))
                                {
                                    this.CancelPackage(storey.module);
                                }
                            }
                            else
                            {
                                GUILayout.Label($"{state.message}... ({state.progress.ToString("P")})", new GUILayoutOption[0]);
                                if (GUILayout.Button(Unity.PackageManager.UI.Styles.cancelButtonContent, new GUILayoutOption[0]))
                                {
                                    this.CancelPackage(storey.module);
                                }
                            }
                            HandleUtility.Repaint();
                        }
                        else if (!flag2 && GUILayout.Button(Unity.PackageManager.UI.Styles.downloadButtonContent, new GUILayoutOption[0]))
                        {
                            this.InstallPackage(storey.module);
                        }
                        else if (storey.module.Loaded)
                        {
                            EditorGUILayout.HelpBox(Unity.PackageManager.UI.Styles.loadedContent.text, MessageType.None);
                        }
                        else if (flag2)
                        {
                            if (storey.module == package)
                            {
                                if (GUILayout.Button(Unity.PackageManager.UI.Styles.restartUnityContent, new GUILayoutOption[0]))
                                {
                                    EditorApplication.OpenProject(Environment.CurrentDirectory, new string[0]);
                                }
                            }
                            else if (GUILayout.Button(Unity.PackageManager.UI.Styles.activateContent, new GUILayoutOption[0]))
                            {
                                Unity.PackageManager.PackageManager.Database.SelectPackage(storey.module);
                                Unity.PackageManager.PackageManager.Instance.LocalIndexer.CacheResult();
                                this.RefreshLocalPackages();
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndFadeGroup();
            if (((current.type == EventType.MouseDown) && (GUIUtility.hotControl == 0)) && position.Contains(current.mousePosition))
            {
                GUIUtility.keyboardControl = listsKeyboardID;
                this.SelectedPackage = package;
                current.Use();
            }
        }

        private AnimBool FaderForPackage(Unity.DataContract.PackageInfo package)
        {
            AnimBool @bool = new AnimBool {
                speed = 4f
            };
            @bool.valueChanged.AddListener(new UnityAction(this.m_Parent.Repaint));
            return @bool;
        }

        private void HandleKeyEvents(int listsKeyboardID)
        {
            Event current = Event.current;
            if ((current.type == EventType.KeyDown) && (GUIUtility.keyboardControl == listsKeyboardID))
            {
                switch (current.keyCode)
                {
                    case KeyCode.Escape:
                        this.SelectedPackage = null;
                        GUIUtility.keyboardControl = 0;
                        current.Use();
                        break;

                    case KeyCode.DownArrow:
                        this.SelectNext();
                        current.Use();
                        break;

                    case KeyCode.UpArrow:
                        this.SelectPrev();
                        current.Use();
                        break;
                }
            }
        }

        private void HandleMouseEvents(int listsKeyboardID)
        {
            Event current = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            EventType typeForControl = current.GetTypeForControl(controlID);
            if (typeForControl == EventType.MouseDown)
            {
                GUIUtility.hotControl = controlID;
                GUIUtility.keyboardControl = listsKeyboardID;
                this.SelectedPackage = null;
                current.Use();
            }
            else if (typeForControl == EventType.MouseDrag)
            {
                if (GUIUtility.hotControl == controlID)
                {
                    current.Use();
                }
            }
            else if ((typeForControl == EventType.MouseUp) && (GUIUtility.hotControl == controlID))
            {
                GUIUtility.hotControl = 0;
                current.Use();
            }
        }

        private void InstallPackage(IvyModule module)
        {
            <InstallPackage>c__AnonStorey1 storey = new <InstallPackage>c__AnonStorey1 {
                module = module
            };
            if (!Enumerable.Any<InstallerState>(this.installerStates, new Func<InstallerState, bool>(storey.<>m__0)))
            {
                Unity.PackageManager.PackageManager.Instance.SetupPackageInstall(storey.module).Run();
            }
        }

        private void ListPackages(IEnumerable<Unity.DataContract.PackageInfo> packages, int listsKeyboardID)
        {
            if (packages.Any<Unity.DataContract.PackageInfo>())
            {
                foreach (Unity.DataContract.PackageInfo info in packages)
                {
                    this.DoPackageElement(info, listsKeyboardID);
                }
            }
            else
            {
                GUILayout.Label("None", new GUILayoutOption[0]);
            }
        }

        public void OnGUI()
        {
            if (Event.current.type == EventType.Layout)
            {
                this.CachePackageManagerState();
            }
            this.Toolbar();
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) };
            this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, Unity.PackageManager.UI.Styles.listBackground, options);
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
            if (this.m_ListViewShouldTakeKeyboardControl)
            {
                GUIUtility.keyboardControl = controlID;
                this.m_ListViewShouldTakeKeyboardControl = false;
            }
            if (this.m_RefreshingRemotePackages)
            {
                GUILayout.Label("Refreshing...", new GUILayoutOption[0]);
            }
            else
            {
                IEnumerator enumerator = Enum.GetValues(typeof(PackageType)).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        object current = enumerator.Current;
                        <OnGUI>c__AnonStorey0 storey = new <OnGUI>c__AnonStorey0 {
                            packageType = (PackageType) current
                        };
                        IEnumerable<Unity.DataContract.PackageInfo> source = Enumerable.Where<Unity.DataContract.PackageInfo>(this.allListedPackages, new Func<Unity.DataContract.PackageInfo, bool>(storey.<>m__0));
                        if (source.Any<Unity.DataContract.PackageInfo>())
                        {
                            if (packageTypeContents.ContainsKey(storey.packageType))
                            {
                                SectionHeader(packageTypeContents[storey.packageType].text);
                            }
                            else
                            {
                                SectionHeader(packageTypeContents[PackageType.Unknown].text);
                            }
                            this.ListPackages(source, controlID);
                        }
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
            }
            this.HandleKeyEvents(controlID);
            this.HandleMouseEvents(controlID);
            GUILayout.EndScrollView();
        }

        private void RefreshLocalPackages()
        {
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = m => m.ToPackageInfo();
            }
            this.localPackages = Enumerable.Select<IvyModule, Unity.DataContract.PackageInfo>(Unity.PackageManager.PackageManager.Database.NewestLocalPackages, <>f__am$cache6);
        }

        private static void SectionHeader(string title)
        {
            GUILayout.Label(title, Unity.PackageManager.UI.Styles.listHeader, new GUILayoutOption[0]);
        }

        private void SelectNext()
        {
            bool flag = this.SelectedPackage == null;
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = p => p.type;
            }
            IEnumerable<Unity.DataContract.PackageInfo> source = Enumerable.OrderBy<Unity.DataContract.PackageInfo, PackageType>(this.allListedPackages, <>f__am$cache4);
            foreach (Unity.DataContract.PackageInfo info in source)
            {
                if (flag)
                {
                    this.SelectedPackage = info;
                    return;
                }
                if (this.SelectedPackage == info)
                {
                    flag = true;
                }
            }
            this.SelectedPackage = source.First<Unity.DataContract.PackageInfo>();
        }

        private void SelectPrev()
        {
            Unity.DataContract.PackageInfo info = null;
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = p => p.type;
            }
            IEnumerable<Unity.DataContract.PackageInfo> enumerable = Enumerable.OrderBy<Unity.DataContract.PackageInfo, PackageType>(this.allListedPackages, <>f__am$cache5);
            foreach (Unity.DataContract.PackageInfo info2 in enumerable)
            {
                if ((info != null) && (this.SelectedPackage == info2))
                {
                    this.SelectedPackage = info;
                    return;
                }
                info = info2;
            }
            this.SelectedPackage = info;
        }

        private void Toolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                Unity.PackageManager.PackageManager.Instance.CheckForRemoteUpdates();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private IEnumerable<Unity.DataContract.PackageInfo> allListedPackages =>
            this.availablePackages.Union<Unity.DataContract.PackageInfo>(this.localPackages, packageNameComparer);

        private IEnumerable<Unity.DataContract.PackageInfo> availablePackages
        {
            get
            {
                if (Unsupported.IsDeveloperBuild())
                {
                    return this.m_AvailablePackages;
                }
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = p => p.type != PackageType.PackageManager;
                }
                return Enumerable.Where<Unity.DataContract.PackageInfo>(this.m_AvailablePackages, <>f__am$cache1);
            }
            set
            {
                this.m_AvailablePackages = value;
            }
        }

        private IEnumerable<InstallerState> installerStates { get; set; }

        private IEnumerable<Unity.DataContract.PackageInfo> localPackages
        {
            get
            {
                if (Unsupported.IsDeveloperBuild())
                {
                    return this.m_LocalPackages;
                }
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = p => p.type != PackageType.PackageManager;
                }
                return Enumerable.Where<Unity.DataContract.PackageInfo>(this.m_LocalPackages, <>f__am$cache0);
            }
            set
            {
                this.m_LocalPackages = value;
            }
        }

        internal Window parent
        {
            get => 
                this.m_Parent;
            set
            {
                this.m_Parent = value;
            }
        }

        public Unity.DataContract.PackageInfo SelectedPackage
        {
            get
            {
                if ((this.selectedPackageHash == -1) && (this.allListedPackages.Count<Unity.DataContract.PackageInfo>() > 0))
                {
                    this.selectedPackageHash = this.allListedPackages.First<Unity.DataContract.PackageInfo>().GetHashCode();
                }
                if (hashToPackageCache.ContainsKey(this.selectedPackageHash))
                {
                    return hashToPackageCache[this.selectedPackageHash];
                }
                foreach (Unity.DataContract.PackageInfo info2 in this.allListedPackages)
                {
                    if (info2.GetHashCode() == this.selectedPackageHash)
                    {
                        hashToPackageCache.Add(this.selectedPackageHash, info2);
                        return info2;
                    }
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    this.selectedPackageHash = value.GetHashCode();
                }
                else
                {
                    this.selectedPackageHash = 0;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <BuildPackageText>c__AnonStorey3
        {
            internal PackageVersion loadedVersion;

            internal bool <>m__0(IvyModule v) => 
                ((v.BasePath == null) && (v.Version > this.loadedVersion));
        }

        [CompilerGenerated]
        private sealed class <CancelPackage>c__AnonStorey2
        {
            internal IvyModule module;

            internal bool <>m__0(ListView.InstallerState i) => 
                (i.module == this.module);
        }

        [CompilerGenerated]
        private sealed class <DoPackageElement>c__AnonStorey4
        {
            internal IvyModule module;

            internal bool <>m__0(ListView.InstallerState i) => 
                (i.module == this.module);
        }

        [CompilerGenerated]
        private sealed class <InstallPackage>c__AnonStorey1
        {
            internal IvyModule module;

            internal bool <>m__0(ListView.InstallerState i) => 
                (i.module == this.module);
        }

        [CompilerGenerated]
        private sealed class <OnGUI>c__AnonStorey0
        {
            internal PackageType packageType;

            internal bool <>m__0(Unity.DataContract.PackageInfo p) => 
                (p.type == this.packageType);
        }

        private class InstallerState
        {
            public Installer installer;
            public string message;
            public IvyModule module;
            public float progress;
            public bool running;
            public bool successful;
        }

        private class PackageNameComparer : IEqualityComparer<Unity.DataContract.PackageInfo>
        {
            public bool Equals(Unity.DataContract.PackageInfo x, Unity.DataContract.PackageInfo y) => 
                x.packageName.Equals(y.packageName);

            public int GetHashCode(Unity.DataContract.PackageInfo obj) => 
                obj.packageName.GetHashCode();
        }
    }
}

