namespace UnityEditor.Facebook
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEngine;

    internal class FacebookSettingsExtension : DefaultPlayerSettingsEditorExtension
    {
        [CompilerGenerated]
        private static Func<Assembly, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Assembly, bool> <>f__am$cache1;
        private static GUIContent accessTokenLabel = new GUIContent("Upload access token");
        private static SerializedProperty AccessTokensProp;
        private static GUIContent appIDLabel = new GUIContent("AppID");
        private static SerializedProperty AppIdsProp;
        private static GUIContent cookieLabel = new GUIContent("Cookie [?]", "Sets a cookie which your server-side code can use to validate a user's Facebook session");
        private static SerializedProperty CookieProp;
        private static SerializedObject facebookSettings;
        private static ScriptableObject facebookSettingsScriptableObject;
        private static GUIContent frictionlessLabel = new GUIContent("Frictionless Requests [?]", "Use frictionless app requests, as described in their own documentation.");
        private static SerializedProperty FrictionlessRequestsProp;
        private static GUIContent loggingLabel = new GUIContent("Logging [?]", "(Web Player only) If true, outputs a verbose log to the Javascript console to facilitate debugging.");
        private static SerializedProperty LoggingProp;
        private int sdkVersion = 0;
        private bool showFacebookInitSettings = false;
        private static GUIContent statusLabel = new GUIContent("Status [?]", "If 'true', attempts to initialize the Facebook object with valid session data.");
        private static SerializedProperty StatusProp;
        private static GUIContent xfbmlLabel = new GUIContent("Xfbml [?]", "(Web Player only If true) Facebook will immediately parse any XFBML elements on the Facebook Canvas page hosting the app");
        private static SerializedProperty XfbmlProp;

        public override bool CanShowUnitySplashScreen() => 
            true;

        private void FBParamsInitGUI()
        {
            this.showFacebookInitSettings = EditorGUILayout.Foldout(this.showFacebookInitSettings, "FB.Init() Parameters");
            if (this.showFacebookInitSettings)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(CookieProp, cookieLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(LoggingProp, loggingLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(StatusProp, statusLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(XfbmlProp, xfbmlLabel, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(FrictionlessRequestsProp, frictionlessLabel, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    PersistChanges();
                }
            }
            EditorGUILayout.Space();
        }

        private static System.Type GetFacebookSettingsType()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = a => a.FullName == SDKManager.fbUnitySettingsName;
            }
            return Enumerable.FirstOrDefault<Assembly>(AppDomain.CurrentDomain.GetAssemblies(), <>f__am$cache0)?.GetType("Facebook.Unity.Settings.FacebookSettings");
        }

        private static string GetSettingsPath()
        {
            try
            {
                System.Type facebookSettingsType = GetFacebookSettingsType();
                string str = (string) facebookSettingsType.GetField("FacebookSettingsAssetName", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).GetValue(null);
                string str2 = (string) facebookSettingsType.GetField("FacebookSettingsAssetExtension", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).GetValue(null);
                string str3 = (string) facebookSettingsType.GetField("FacebookSettingsPath", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).GetValue(null);
                return Path.Combine(Path.Combine("Assets", str3), str + str2);
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        public override bool HasPublishSection() => 
            true;

        private static void InitFacebookSettings()
        {
            try
            {
                SDKManager.CheckSDKToUse();
                System.Type facebookSettingsType = GetFacebookSettingsType();
                facebookSettingsScriptableObject = AssetDatabase.LoadAssetAtPath(GetSettingsPath(), typeof(ScriptableObject)) as ScriptableObject;
                if (facebookSettingsScriptableObject == null)
                {
                    facebookSettingsScriptableObject = ScriptableObject.CreateInstance(facebookSettingsType);
                }
                facebookSettings = new SerializedObject(facebookSettingsScriptableObject);
                AppIdsProp = facebookSettings.FindProperty("appIds.Array.data[0]");
                CookieProp = facebookSettings.FindProperty("cookie");
                LoggingProp = facebookSettings.FindProperty("logging");
                StatusProp = facebookSettings.FindProperty("status");
                XfbmlProp = facebookSettings.FindProperty("xfbml");
                AccessTokensProp = facebookSettings.FindProperty("uploadAccessToken");
                FrictionlessRequestsProp = facebookSettings.FindProperty("frictionlessRequests");
            }
            catch (NullReferenceException)
            {
            }
        }

        public override void OnEnable(PlayerSettingsEditor settingsEditor)
        {
            string sdkVersion = PlayerSettings.Facebook.sdkVersion;
            this.sdkVersion = 0;
            while (this.sdkVersion < SDKManager.AvailableSDKs.Length)
            {
                if (SDKManager.AvailableSDKs[this.sdkVersion] == sdkVersion)
                {
                    break;
                }
                this.sdkVersion++;
            }
            InitFacebookSettings();
        }

        private static void PersistChanges()
        {
            facebookSettings.ApplyModifiedProperties();
            if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(facebookSettingsScriptableObject)))
            {
                string settingsPath = GetSettingsPath();
                if (settingsPath != null)
                {
                    string directoryName = Path.GetDirectoryName(settingsPath);
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                    AssetDatabase.CreateAsset(facebookSettingsScriptableObject, settingsPath);
                }
            }
            EditorUtility.SetDirty(facebookSettingsScriptableObject);
        }

        public override void PublishSectionGUI(float h, float kLabelFloatMinW, float kLabelFloatMaxW)
        {
            GUILayout.Label(EditorGUIUtility.TextContent("General"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            bool disabled = SDKManager.IsSDKOverriden();
            using (new EditorGUI.DisabledScope(disabled))
            {
                EditorGUI.BeginChangeCheck();
                int num = EditorGUILayout.Popup("Facebook SDK Version", this.sdkVersion, SDKManager.AvailableSDKs, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck() && (this.sdkVersion != num))
                {
                    this.sdkVersion = num;
                    EditorUtility.DisplayProgressBar("Progress...", "Changing Facebook SDK version...", 0.8f);
                    PlayerSettings.Facebook.sdkVersion = SDKManager.AvailableSDKs[this.sdkVersion];
                    SDKManager.RegisterAdditionalUnityExtensions();
                    EditorUtility.ClearProgressBar();
                }
            }
            if (disabled)
            {
                if (SDKManager.IsSDKOverridenIncompatibleVersion())
                {
                    EditorGUILayout.HelpBox("This project contains a custom Facebook SDK for unity, which is incompatible with the Facebook build target.", MessageType.Error);
                }
                else
                {
                    EditorGUILayout.HelpBox("This project contains a custom Facebook SDK for unity, which will override the SDK provided by the Facebook build target.", MessageType.Info);
                }
                GUILayout.Button("Open Facebook SDK settings", new GUILayoutOption[0]);
            }
            if (AppIdsProp == null)
            {
                EditorGUILayout.HelpBox("The selected Facebook SDK does not seem to be compatible with these settings.", MessageType.Error);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(AppIdsProp, appIDLabel, new GUILayoutOption[0]);
                EditorUserBuildSettings.facebookAccessToken = EditorGUILayout.TextField(accessTokenLabel, EditorUserBuildSettings.facebookAccessToken, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    PersistChanges();
                }
                GUILayout.Label("Windows", EditorStyles.boldLabel, new GUILayoutOption[0]);
                if (GUILayout.Button("Show Windows Player settings", new GUILayoutOption[0]))
                {
                    EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Standalone;
                }
                GUILayout.Label("WebGL", EditorStyles.boldLabel, new GUILayoutOption[0]);
                this.FBParamsInitGUI();
                if (GUILayout.Button("Show WebGL Player settings", new GUILayoutOption[0]))
                {
                    EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.WebGL;
                }
            }
        }

        public static string AccessToken
        {
            get
            {
                if (facebookSettings == null)
                {
                    InitFacebookSettings();
                }
                return AccessTokensProp.stringValue;
            }
        }

        public static string AppID
        {
            get
            {
                if (facebookSettings == null)
                {
                    InitFacebookSettings();
                }
                return AppIdsProp.stringValue;
            }
        }
    }
}

