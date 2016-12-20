namespace Unity.PackageManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Unity.DataContract;
    using Unity.PackageManager;
    using UnityEngine;

    [Serializable]
    internal class InfoView
    {
        [CompilerGenerated]
        private static Func<IvyModule, PackageInfo> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<KeyValuePair<string, PackageFileData>, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Predicate<DownloaderState> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<PackageInfo, PackageVersion> <>f__am$cache3;
        [NonSerialized]
        private List<DownloaderState> downloaderStates = new List<DownloaderState>();
        [NonSerialized]
        private PackageInfo m_Package;
        private Dictionary<string, HashSet<PackageInfo>> packageCache = new Dictionary<string, HashSet<PackageInfo>>();
        [SerializeField]
        private Vector2 scrollPosition;

        private void CacheDownloaderState()
        {
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = x => x.downloader.IsCompleted;
            }
            this.downloaderStates.RemoveAll(<>f__am$cache2);
        }

        private void DoDescription()
        {
            GUILayout.Label(this.package.description, Styles.infoText, new GUILayoutOption[0]);
        }

        private void DoHeader()
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label(string.Format("{0} ({1})", this.package.name, this.package.version), Styles.infoHeader, new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
        }

        private void DoReleaseNotes()
        {
            GUILayout.Label("Release Notes", Styles.infoHeader, new GUILayoutOption[0]);
            if (this.IsDownloadingNotes())
            {
                GUILayout.Label("Refreshing...", Styles.infoText, new GUILayoutOption[0]);
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = new Func<PackageInfo, PackageVersion>(null, (IntPtr) <DoReleaseNotes>m__4);
                }
                foreach (PackageInfo info in Enumerable.OrderByDescending<PackageInfo, PackageVersion>(this.packageCache[this.package.packageName], <>f__am$cache3))
                {
                    builder.AppendFormat("<b>{0}</b>:\n", info.version);
                    builder.AppendLine(info.releaseNotes);
                    builder.AppendLine(string.Empty);
                }
                GUILayout.Label(builder.ToString(), Styles.infoText, new GUILayoutOption[0]);
            }
        }

        private void DownloadReleaseNotes(PackageInfo version)
        {
            <DownloadReleaseNotes>c__AnonStorey0 storey = new <DownloadReleaseNotes>c__AnonStorey0 {
                version = version,
                $this = this
            };
            if (!Enumerable.Any<DownloaderState>(this.downloaderStates, new Func<DownloaderState, bool>(storey, (IntPtr) this.<>m__0)))
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = new Func<KeyValuePair<string, PackageFileData>, bool>(null, (IntPtr) <DownloadReleaseNotes>m__1);
                }
                KeyValuePair<string, PackageFileData> pair = Enumerable.FirstOrDefault<KeyValuePair<string, PackageFileData>>(this.m_Package.files, <>f__am$cache1);
                if (pair.Key != null)
                {
                    DownloaderTask task = new DownloaderTask(new Uri(pair.Value.url)) {
                        Name = "ReleaseNotes"
                    };
                    task.OnFinish += new TaskFinishedHandler(storey.<>m__1);
                    DownloaderState item = new DownloaderState {
                        downloader = task,
                        package = storey.version
                    };
                    this.downloaderStates.Add(item);
                    task.Run();
                }
            }
        }

        private bool IsDownloadingNotes()
        {
            return Enumerable.Any<DownloaderState>(this.downloaderStates, new Func<DownloaderState, bool>(this, (IntPtr) this.<IsDownloadingNotes>m__3));
        }

        public void OnGUI()
        {
            if (this.package != null)
            {
                if (Event.current.type == EventType.Layout)
                {
                    this.CacheDownloaderState();
                }
                GUILayout.BeginVertical(Styles.infoContainer, new GUILayoutOption[0]);
                this.DoHeader();
                GUILayout.Space(10f);
                this.DoDescription();
                GUILayout.EndVertical();
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) };
                this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, options);
                GUILayout.BeginVertical(Styles.infoContainer, new GUILayoutOption[0]);
                this.DoReleaseNotes();
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            }
        }

        private void UpdateReleaseNotes()
        {
            if (this.m_Package != null)
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<IvyModule, PackageInfo>(null, (IntPtr) <UpdateReleaseNotes>m__0);
                }
                foreach (PackageInfo info in Enumerable.Select<IvyModule, PackageInfo>(Unity.PackageManager.PackageManager.Database.AllVersionsOfPackage(this.m_Package.packageName), <>f__am$cache0))
                {
                    if (!this.packageCache.ContainsKey(info.packageName))
                    {
                        this.packageCache[info.packageName] = new HashSet<PackageInfo>();
                    }
                    this.packageCache[info.packageName].Add(info);
                    this.DownloadReleaseNotes(info);
                }
            }
        }

        public PackageInfo package
        {
            get
            {
                return PackageInfoExtensions.Refresh(this.m_Package);
            }
            set
            {
                if (this.m_Package != value)
                {
                    this.m_Package = PackageInfoExtensions.Refresh(value);
                    this.UpdateReleaseNotes();
                }
            }
        }

        [CompilerGenerated]
        private sealed class <DownloadReleaseNotes>c__AnonStorey0
        {
            internal InfoView $this;
            internal PackageInfo version;

            internal bool <>m__0(InfoView.DownloaderState i)
            {
                return (i.package == this.version);
            }

            internal void <>m__1(Task t, bool success)
            {
                <DownloadReleaseNotes>c__AnonStorey1 storey = new <DownloadReleaseNotes>c__AnonStorey1 {
                    <>f__ref$0 = this,
                    t = t
                };
                string result = storey.t.Result as string;
                if (File.Exists(result))
                {
                    File.Copy(result, Path.Combine(Settings.installLocation, Path.GetFileName(result)), true);
                    if (Enumerable.First<InfoView.DownloaderState>(this.$this.downloaderStates, new Func<InfoView.DownloaderState, bool>(storey, (IntPtr) this.<>m__0)).package == this.version)
                    {
                        PackageInfoExtensions.Refresh(this.version);
                    }
                }
            }

            private sealed class <DownloadReleaseNotes>c__AnonStorey1
            {
                internal InfoView.<DownloadReleaseNotes>c__AnonStorey0 <>f__ref$0;
                internal Task t;

                internal bool <>m__0(InfoView.DownloaderState x)
                {
                    return (x.downloader == this.t);
                }
            }
        }

        private class DownloaderState
        {
            public DownloaderTask downloader;
            public PackageInfo package;
        }
    }
}

