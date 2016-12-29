namespace UnityEditor
{
    using System;
    using UnityEditor.Collaboration;
    using UnityEditorInternal;
    using UnityEngine;

    internal class CacheServerPreferences
    {
        private const string kDeprecatedEnabledKey = "CacheServerEnabled";
        private const string kIPAddressKey = "CacheServerIPAddress";
        private const string kModeKey = "CacheServerMode";
        private static string s_CachePath;
        private static string s_CacheServerIPAddress;
        private static CacheServerMode s_CacheServerMode;
        private static bool s_CollabCacheEnabled;
        private static string s_CollabCacheIPAddress;
        private static ConnectionState s_ConnectionState;
        private static Constants s_Constants = null;
        private static bool s_EnableCollabCacheConfiguration = false;
        private static bool s_EnableCustomPath;
        private static bool s_HasPendingChanges = false;
        private static int s_LocalCacheServerSize;
        private static long s_LocalCacheServerUsedSize = -1L;
        private static bool s_PrefsLoaded;

        private static bool IsCollabCacheEnabled() => 
            (s_EnableCollabCacheConfiguration || Application.HasARGV("enableCacheServer"));

        [PreferenceItem("Cache Server")]
        public static void OnGUI()
        {
            EventType type = Event.current.type;
            if (s_Constants == null)
            {
                s_Constants = new Constants();
            }
            GUILayout.Space(10f);
            if (!InternalEditorUtility.HasTeamLicense())
            {
                GUILayout.Label(EditorGUIUtility.TempContent("You need to have a Pro or Team license to use the cache server.", EditorGUIUtility.GetHelpIcon(MessageType.Warning)), EditorStyles.helpBox, new GUILayoutOption[0]);
            }
            using (new EditorGUI.DisabledScope(!InternalEditorUtility.HasTeamLicense()))
            {
                if (!s_PrefsLoaded)
                {
                    ReadPreferences();
                    if ((s_CacheServerMode != CacheServerMode.Disabled) && (s_ConnectionState == ConnectionState.Unknown))
                    {
                        if (InternalEditorUtility.CanConnectToCacheServer())
                        {
                            s_ConnectionState = ConnectionState.Success;
                        }
                        else
                        {
                            s_ConnectionState = ConnectionState.Failure;
                        }
                    }
                    s_PrefsLoaded = true;
                }
                EditorGUI.BeginChangeCheck();
                if (Collab.instance.collabInfo.whitelisted && IsCollabCacheEnabled())
                {
                    s_CollabCacheEnabled = EditorGUILayout.Toggle("Use Collab Cache", s_CollabCacheEnabled, new GUILayoutOption[0]);
                    using (new EditorGUI.DisabledScope(!s_CollabCacheEnabled))
                    {
                        s_CollabCacheIPAddress = EditorGUILayout.TextField("Collab Cache IP Address", s_CollabCacheIPAddress, new GUILayoutOption[0]);
                    }
                }
                s_CacheServerMode = (CacheServerMode) EditorGUILayout.EnumPopup("Cache Server Mode", s_CacheServerMode, new GUILayoutOption[0]);
                if (s_CacheServerMode != CacheServerMode.Remote)
                {
                    goto Label_0238;
                }
                s_CacheServerIPAddress = EditorGUILayout.DelayedTextField("IP Address", s_CacheServerIPAddress, new GUILayoutOption[0]);
                if (GUI.changed)
                {
                    s_ConnectionState = ConnectionState.Unknown;
                }
                GUILayout.Space(5f);
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(150f) };
                if (GUILayout.Button("Check Connection", options))
                {
                    if (InternalEditorUtility.CanConnectToCacheServer())
                    {
                        s_ConnectionState = ConnectionState.Success;
                    }
                    else
                    {
                        s_ConnectionState = ConnectionState.Failure;
                    }
                }
                GUILayout.Space(-25f);
                ConnectionState state = s_ConnectionState;
                if (state != ConnectionState.Success)
                {
                    if (state == ConnectionState.Failure)
                    {
                        goto Label_0212;
                    }
                    if (state == ConnectionState.Unknown)
                    {
                        goto Label_0223;
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Connection successful.", MessageType.Info, false);
                }
                goto Label_041C;
            Label_0212:
                EditorGUILayout.HelpBox("Connection failed.", MessageType.Warning, false);
                goto Label_041C;
            Label_0223:
                GUILayout.Space(44f);
                goto Label_041C;
            Label_0238:
                if (s_CacheServerMode == CacheServerMode.Local)
                {
                    s_LocalCacheServerSize = EditorGUILayout.IntSlider(Styles.maxCacheSize, s_LocalCacheServerSize, 1, 200, new GUILayoutOption[0]);
                    s_EnableCustomPath = EditorGUILayout.Toggle(Styles.customCacheLocation, s_EnableCustomPath, new GUILayoutOption[0]);
                    if (s_EnableCustomPath)
                    {
                        GUIStyle popup = EditorStyles.popup;
                        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                        EditorGUILayout.PrefixLabel(Styles.cacheFolderLocation, popup);
                        Rect position = GUILayoutUtility.GetRect(GUIContent.none, popup);
                        GUIContent content = !string.IsNullOrEmpty(s_CachePath) ? new GUIContent(s_CachePath) : Styles.browse;
                        if (EditorGUI.ButtonMouseDown(position, content, FocusType.Passive, popup))
                        {
                            string folder = s_CachePath;
                            string str2 = EditorUtility.OpenFolderPanel(Styles.browseCacheLocation.text, folder, "");
                            if (!string.IsNullOrEmpty(str2))
                            {
                                if (LocalCacheServer.CheckValidCacheLocation(str2))
                                {
                                    s_CachePath = str2;
                                    WritePreferences();
                                }
                                else
                                {
                                    EditorUtility.DisplayDialog("Invalid Cache Location", "The directory " + str2 + " contains some files which don't look like Unity Cache server files. Please delete the directory contents or choose another directory.", "OK");
                                }
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        s_CachePath = "";
                    }
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(120f) };
                    if (GUILayout.Button(Styles.cleanCache, optionArray2))
                    {
                        LocalCacheServer.Clear();
                        s_LocalCacheServerUsedSize = 0L;
                    }
                    if (s_LocalCacheServerUsedSize == -1L)
                    {
                        s_LocalCacheServerUsedSize = FileUtil.GetDirectorySize(LocalCacheServer.GetCacheLocation());
                    }
                    GUILayout.Label(Styles.cacheSizeIs.text + " " + EditorUtility.FormatBytes(s_LocalCacheServerUsedSize), new GUILayoutOption[0]);
                    GUILayout.Label(Styles.cacheFolderLocation.text + ":", new GUILayoutOption[0]);
                    GUILayout.Label(LocalCacheServer.GetCacheLocation(), s_Constants.cacheFolderLocation, new GUILayoutOption[0]);
                }
            Label_041C:
                if (EditorGUI.EndChangeCheck())
                {
                    s_HasPendingChanges = true;
                }
                if ((s_HasPendingChanges && (type == EventType.MouseUp)) || (type == EventType.KeyDown))
                {
                    WritePreferences();
                    ReadPreferences();
                    s_HasPendingChanges = false;
                }
            }
        }

        public static void ReadPreferences()
        {
            s_CacheServerIPAddress = EditorPrefs.GetString("CacheServerIPAddress", s_CacheServerIPAddress);
            s_CacheServerMode = (CacheServerMode) EditorPrefs.GetInt("CacheServerMode", !EditorPrefs.GetBool("CacheServerEnabled") ? 2 : 1);
            s_LocalCacheServerSize = EditorPrefs.GetInt("LocalCacheServerSize", 10);
            s_CachePath = EditorPrefs.GetString("LocalCacheServerPath");
            s_EnableCustomPath = EditorPrefs.GetBool("LocalCacheServerCustomPath");
            if (IsCollabCacheEnabled())
            {
                s_CollabCacheIPAddress = EditorPrefs.GetString("CollabCacheIPAddress", s_CollabCacheIPAddress);
                s_CollabCacheEnabled = EditorPrefs.GetBool("CollabCacheEnabled");
            }
        }

        public static void WritePreferences()
        {
            CacheServerMode @int = (CacheServerMode) EditorPrefs.GetInt("CacheServerMode");
            string str = EditorPrefs.GetString("LocalCacheServerPath");
            bool @bool = EditorPrefs.GetBool("LocalCacheServerCustomPath");
            bool flag2 = false;
            if ((@int != s_CacheServerMode) && (@int == CacheServerMode.Local))
            {
                flag2 = true;
            }
            if (s_EnableCustomPath && (str != s_CachePath))
            {
                flag2 = true;
            }
            if (((s_EnableCustomPath != @bool) && (s_CachePath != LocalCacheServer.GetCacheLocation())) && (s_CachePath != ""))
            {
                flag2 = true;
            }
            if (flag2)
            {
                s_LocalCacheServerUsedSize = -1L;
                string message = (s_CacheServerMode != CacheServerMode.Local) ? "You have disabled the local cache." : "You have changed the location of the local cache storage.";
                message = message + " Do you want to delete the old locally cached data at " + LocalCacheServer.GetCacheLocation() + "?";
                if (EditorUtility.DisplayDialog("Delete old Cache", message, "Delete", "Don't Delete"))
                {
                    LocalCacheServer.Clear();
                    s_LocalCacheServerUsedSize = 0L;
                }
            }
            EditorPrefs.SetString("CacheServerIPAddress", s_CacheServerIPAddress);
            EditorPrefs.SetInt("CacheServerMode", (int) s_CacheServerMode);
            EditorPrefs.SetInt("LocalCacheServerSize", s_LocalCacheServerSize);
            EditorPrefs.SetString("LocalCacheServerPath", s_CachePath);
            EditorPrefs.SetBool("LocalCacheServerCustomPath", s_EnableCustomPath);
            if (IsCollabCacheEnabled())
            {
                EditorPrefs.SetString("CollabCacheIPAddress", s_CollabCacheIPAddress);
                EditorPrefs.SetBool("CollabCacheEnabled", s_CollabCacheEnabled);
            }
            LocalCacheServer.Setup();
        }

        public enum CacheServerMode
        {
            Local,
            Remote,
            Disabled
        }

        private enum ConnectionState
        {
            Unknown,
            Success,
            Failure
        }

        internal class Constants
        {
            public GUIStyle cacheFolderLocation = new GUIStyle(GUI.skin.label);

            public Constants()
            {
                this.cacheFolderLocation.wordWrap = true;
            }
        }

        internal class Styles
        {
            public static readonly GUIContent browse = EditorGUIUtility.TextContent("Browse...");
            public static readonly GUIContent browseCacheLocation = EditorGUIUtility.TextContent("Browse for local asset cache server location");
            public static readonly GUIContent cacheFolderLocation = EditorGUIUtility.TextContent("Cache Folder Location|The local asset cache server folder is shared between all projects.");
            public static readonly GUIContent cacheSizeIs = EditorGUIUtility.TextContent("Cache size is");
            public static readonly GUIContent cleanCache = EditorGUIUtility.TextContent("Clean Cache");
            public static readonly GUIContent customCacheLocation = EditorGUIUtility.TextContent("Custom cache location|Specify the local asset cache server folder location.");
            public static readonly GUIContent maxCacheSize = EditorGUIUtility.TextContent("Maximum Cache Size (GB)|The size of the local asset cache server folder will be kept below this maximum value.");
        }
    }
}

