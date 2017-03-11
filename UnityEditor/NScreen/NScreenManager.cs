namespace UnityEditor.NScreen
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class NScreenManager : ScriptableSingleton<NScreenManager>
    {
        [CompilerGenerated]
        private static Func<EditorWindow, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static EditorApplication.CallbackFunction <>f__mg$cache0;
        [SerializeField]
        private bool m_BuildOnPlay = true;
        [SerializeField]
        private int m_LatestId = 0;
        [SerializeField]
        private int m_SelectedSizeIndex;

        static NScreenManager()
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new EditorApplication.CallbackFunction(NScreenManager.PlayModeStateChanged);
            }
            EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.playmodeStateChanged, <>f__mg$cache0);
        }

        internal static void Build()
        {
            string[] array = new string[EditorBuildSettings.scenes.Length];
            int index = 0;
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                if (EditorBuildSettings.scenes[i].enabled)
                {
                    array[index] = EditorBuildSettings.scenes[i].path;
                    index++;
                }
            }
            Array.Resize<string>(ref array, index);
            Directory.CreateDirectory("Temp/NScreen");
            ResolutionDialogSetting displayResolutionDialog = PlayerSettings.displayResolutionDialog;
            bool runInBackground = PlayerSettings.runInBackground;
            bool defaultIsFullScreen = PlayerSettings.defaultIsFullScreen;
            PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
            PlayerSettings.runInBackground = true;
            PlayerSettings.defaultIsFullScreen = false;
            try
            {
                BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions {
                    scenes = array,
                    options = BuildOptions.CompressTextures,
                    locationPathName = "Temp/NScreen/NScreen.app"
                };
                if (IntPtr.Size == 4)
                {
                    buildPlayerOptions.target = BuildTarget.StandaloneOSXIntel;
                }
                else
                {
                    buildPlayerOptions.target = BuildTarget.StandaloneOSXIntel64;
                }
                BuildPipeline.BuildPlayer(buildPlayerOptions);
            }
            finally
            {
                PlayerSettings.displayResolutionDialog = displayResolutionDialog;
                PlayerSettings.runInBackground = runInBackground;
                PlayerSettings.defaultIsFullScreen = defaultIsFullScreen;
            }
        }

        internal int GetNewId() => 
            ++this.m_LatestId;

        internal static void Init()
        {
            RemoteGame window = (RemoteGame) EditorWindow.GetWindow(typeof(RemoteGame));
            if (EditorApplication.isPlaying && !window.IsRunning())
            {
                window.id = ScriptableSingleton<NScreenManager>.instance.GetNewId();
                window.StartGame();
            }
        }

        internal static void OpenAnotherWindow()
        {
            RemoteGame game = ScriptableObject.CreateInstance<RemoteGame>();
            foreach (ContainerWindow window in ContainerWindow.windows)
            {
                foreach (View view in window.rootView.allChildren)
                {
                    DockArea area = view as DockArea;
                    if (area != null)
                    {
                        if (<>f__am$cache0 == null)
                        {
                            <>f__am$cache0 = pane => pane.GetType() == typeof(RemoteGame);
                        }
                        if (Enumerable.Any<EditorWindow>(area.m_Panes, <>f__am$cache0))
                        {
                            area.AddTab(game);
                            break;
                        }
                    }
                }
            }
            game.Show();
            if (EditorApplication.isPlaying)
            {
                game.id = ScriptableSingleton<NScreenManager>.instance.GetNewId();
                game.StartGame();
            }
        }

        internal static void PlayModeStateChanged()
        {
            if (!EditorApplication.isPaused)
            {
                if ((!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode) && ((UnityEngine.Resources.FindObjectsOfTypeAll<RemoteGame>().Length > 0) && ScriptableSingleton<NScreenManager>.instance.BuildOnPlay))
                {
                    Build();
                }
                if (EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    StartAll();
                }
                else if (EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    StopAll();
                }
                else if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    RepaintAllGameViews();
                }
            }
        }

        internal static void RepaintAllGameViews()
        {
            foreach (RemoteGame game in UnityEngine.Resources.FindObjectsOfTypeAll<RemoteGame>())
            {
                game.Repaint();
                game.GameViewAspectWasChanged();
            }
        }

        internal void ResetIds()
        {
            this.m_LatestId = 0;
        }

        internal static void StartAll()
        {
            ScriptableSingleton<NScreenManager>.instance.ResetIds();
            foreach (RemoteGame game in UnityEngine.Resources.FindObjectsOfTypeAll<RemoteGame>())
            {
                game.id = ScriptableSingleton<NScreenManager>.instance.GetNewId();
                game.StartGame();
            }
        }

        internal static void StopAll()
        {
            foreach (RemoteGame game in UnityEngine.Resources.FindObjectsOfTypeAll<RemoteGame>())
            {
                game.StopGame();
            }
        }

        internal bool BuildOnPlay
        {
            get => 
                (this.m_BuildOnPlay || !this.HasBuild);
            set
            {
                this.m_BuildOnPlay = value;
            }
        }

        internal bool HasBuild =>
            Directory.Exists("Temp/NScreen/NScreen.app");

        internal int SelectedSizeIndex
        {
            get => 
                this.m_SelectedSizeIndex;
            set
            {
                this.m_SelectedSizeIndex = value;
            }
        }
    }
}

