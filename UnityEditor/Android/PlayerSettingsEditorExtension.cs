namespace UnityEditor.Android
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEngine;

    internal class PlayerSettingsEditorExtension : DefaultPlayerSettingsEditorExtension
    {
        private const int kIconSpacing = 6;
        private const int kMaxPreviewSize = 0x60;
        private const int kSlotSize = 0x40;
        private SerializedProperty m_AndroidBundleVersionCode;
        private SerializedProperty m_AndroidEnableBanner;
        private SerializedProperty m_AndroidGamepadSupportLevel;
        private SerializedProperty m_AndroidIsGame;
        private SerializedProperty m_AndroidKeyaliasName;
        private SerializedProperty m_AndroidKeystoreName;
        private SerializedProperty m_AndroidMinSdkVersion;
        private SerializedProperty m_AndroidPreferredInstallLocation;
        private SerializedProperty m_AndroidSplashScreen;
        private SerializedProperty m_AndroidSplashScreenScale;
        private SerializedProperty m_AndroidTargetDevice;
        private SerializedProperty m_AndroidTVCompatibility;
        private SerializedProperty m_APKExpansionFiles;
        private SerializedProperty m_CreateWallpaper;
        private SerializedProperty m_ForceInternetPermission;
        private SerializedProperty m_ForceSDCardPermission;
        private string[] m_KeystoreAvailableKeys = null;
        private string m_KeystoreConfirm = "";
        private bool m_KeystoreCreate = false;
        private PlayerSettingsEditor m_SettingsEditor;

        public override bool CanShowUnitySplashScreen()
        {
            return true;
        }

        public override void ConfigurationSectionGUI()
        {
            EditorGUILayout.PropertyField(this.m_AndroidTargetDevice, EditorGUIUtility.TextContent("Device Filter"), new GUILayoutOption[0]);
            GUIContent label = EditorGUIUtility.TextContent("Install Location");
            EditorGUILayout.PropertyField(this.m_AndroidPreferredInstallLocation, label, new GUILayoutOption[0]);
            EditorGUI.BeginChangeCheck();
            bool boolValue = this.m_ForceInternetPermission.boolValue;
            string[] displayedOptions = new string[] { "Auto", "Require" };
            boolValue = EditorGUILayout.Popup(EditorGUIUtility.TextContent("Internet Access").text, !boolValue ? 0 : 1, displayedOptions, new GUILayoutOption[0]) == 1;
            if (EditorGUI.EndChangeCheck())
            {
                this.m_ForceInternetPermission.boolValue = boolValue;
            }
            EditorGUI.BeginChangeCheck();
            bool flag2 = this.m_ForceSDCardPermission.boolValue;
            string[] texts = new string[] { "Internal", "External (SDCard)" };
            flag2 = EditorGUILayout.Popup(EditorGUIUtility.TextContent("Write Permission|Since KITKAT writing to external cache and persistent data paths doesn't require write external permission."), !flag2 ? 0 : 1, EditorGUIUtility.TempContent(texts), new GUILayoutOption[0]) == 1;
            if (EditorGUI.EndChangeCheck())
            {
                this.m_ForceSDCardPermission.boolValue = flag2;
            }
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.m_AndroidTVCompatibility, EditorGUIUtility.TextContent("Android TV Compatibility|Check the game for Android TV compatibility, update the manifest if needed"), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_AndroidIsGame, EditorGUIUtility.TextContent("Android Game|If enabled, built APK will be marked as a game rather than a regular app"), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_AndroidGamepadSupportLevel, EditorGUIUtility.TextContent("Android Gamepad Support Level"), new GUILayoutOption[0]);
            EditorGUILayout.Space();
        }

        public override bool HasIdentificationGUI()
        {
            return true;
        }

        public override void IconSectionGUI()
        {
            GUI.changed = false;
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.m_AndroidEnableBanner, EditorGUIUtility.TextContent("Enable Android Banner|If enabled, android banner will be added to the APK resources"), new GUILayoutOption[0]);
            bool boolValue = this.m_AndroidEnableBanner.boolValue;
            this.m_SettingsEditor.serializedObject.ApplyModifiedProperties();
            if (GUI.changed && !boolValue)
            {
                PlayerSettings.Android.SetAndroidBanners(new Texture2D[0]);
            }
            AndroidBanner[] androidBanners = PlayerSettings.Android.GetAndroidBanners();
            GUI.changed = false;
            GUI.enabled = boolValue;
            for (int i = 0; i < androidBanners.Length; i++)
            {
                int width = androidBanners[i].width;
                int height = androidBanners[i].height;
                int b = Mathf.Min(0x60, height);
                int num5 = (b * width) / height;
                Rect rect = GUILayoutUtility.GetRect((float) num5, (float) (Mathf.Max(0x40, b) + 6));
                float num6 = Mathf.Min(rect.width, (((EditorGUIUtility.labelWidth + 4f) + 64f) + 6f) + 96f);
                string text = width + "x" + height;
                GUI.Label(new Rect(rect.x, rect.y, ((num6 - 96f) - 64f) - 12f, 20f), text);
                androidBanners[i].banner = (Texture2D) EditorGUI.ObjectField(new Rect((((rect.x + num6) - 96f) - 64f) - 6f, rect.y, 64f, 64f), androidBanners[i].banner, typeof(Texture2D), false);
                Rect position = new Rect((rect.x + num6) - 96f, rect.y, (float) num5, (float) b);
                Texture2D androidBannerForHeight = PlayerSettings.Android.GetAndroidBannerForHeight(height);
                if (androidBannerForHeight != null)
                {
                    GUI.DrawTexture(position, androidBannerForHeight);
                }
                else
                {
                    GUI.Box(position, "");
                }
            }
            if (GUI.changed)
            {
                Undo.RecordObject(this.m_SettingsEditor.serializedObject.targetObject, "Change Android TV Banner");
                Texture2D[] banners = new Texture2D[androidBanners.Length];
                for (int j = 0; j < androidBanners.Length; j++)
                {
                    banners[j] = androidBanners[j].banner;
                }
                PlayerSettings.Android.SetAndroidBanners(banners);
            }
        }

        public override void IdentificationSectionGUI()
        {
            EditorGUILayout.PropertyField(this.m_AndroidBundleVersionCode, EditorGUIUtility.TextContent("Bundle Version Code"), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_AndroidMinSdkVersion, EditorGUIUtility.TextContent("Minimum API Level"), new GUILayoutOption[0]);
        }

        public override void OnEnable(PlayerSettingsEditor settingsEditor)
        {
            this.m_AndroidBundleVersionCode = settingsEditor.FindPropertyAssert("AndroidBundleVersionCode");
            this.m_AndroidKeystoreName = settingsEditor.FindPropertyAssert("AndroidKeystoreName");
            this.m_AndroidKeyaliasName = settingsEditor.FindPropertyAssert("AndroidKeyaliasName");
            this.m_AndroidMinSdkVersion = settingsEditor.FindPropertyAssert("AndroidMinSdkVersion");
            this.m_AndroidPreferredInstallLocation = settingsEditor.FindPropertyAssert("AndroidPreferredInstallLocation");
            this.m_AndroidSplashScreenScale = settingsEditor.FindPropertyAssert("AndroidSplashScreenScale");
            this.m_AndroidTargetDevice = settingsEditor.FindPropertyAssert("AndroidTargetDevice");
            this.m_AndroidTVCompatibility = settingsEditor.FindPropertyAssert("AndroidTVCompatibility");
            this.m_AndroidIsGame = settingsEditor.FindPropertyAssert("AndroidIsGame");
            this.m_AndroidEnableBanner = settingsEditor.FindPropertyAssert("androidEnableBanner");
            this.m_AndroidGamepadSupportLevel = settingsEditor.FindPropertyAssert("androidGamepadSupportLevel");
            this.m_APKExpansionFiles = settingsEditor.FindPropertyAssert("APKExpansionFiles");
            this.m_ForceInternetPermission = settingsEditor.FindPropertyAssert("ForceInternetPermission");
            this.m_ForceSDCardPermission = settingsEditor.FindPropertyAssert("ForceSDCardPermission");
            this.m_AndroidSplashScreen = settingsEditor.FindPropertyAssert("androidSplashScreen");
            this.m_SettingsEditor = settingsEditor;
        }

        public override void PublishSectionGUI(float h, float kLabelFloatMinW, float kLabelFloatMaxW)
        {
            Rect rect2;
            string keystorePass = PlayerSettings.keystorePass;
            string keyaliasPass = PlayerSettings.keyaliasPass;
            bool flag = false;
            GUILayout.Label(EditorGUIUtility.TextContent("Keystore"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            Rect position = GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, h + 5f, h + 5f, EditorStyles.layerMaskField, null);
            GUIContent[] contents = new GUIContent[] { EditorGUIUtility.TextContent("Use Existing Keystore"), EditorGUIUtility.TextContent("Create New Keystore") };
            bool flag2 = GUI.SelectionGrid(position, !this.m_KeystoreCreate ? 0 : 1, contents, 2, "toggle") == 1;
            if (flag2 != this.m_KeystoreCreate)
            {
                this.m_KeystoreCreate = flag2;
                this.m_AndroidKeystoreName.stringValue = "";
                this.m_AndroidKeyaliasName.stringValue = "";
                this.m_KeystoreAvailableKeys = null;
            }
            position = GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, h, h, EditorStyles.layerMaskField, null);
            GUIContent content = null;
            bool disabled = this.m_AndroidKeystoreName.stringValue.Length == 0;
            using (new EditorGUI.DisabledScope(disabled))
            {
                if (disabled)
                {
                    content = EditorGUIUtility.TextContent("Browse to select keystore name");
                }
                else
                {
                    content = EditorGUIUtility.TempContent(this.m_AndroidKeystoreName.stringValue);
                }
                float labelWidth = EditorGUIUtility.labelWidth;
                rect2 = new Rect(position.x + EditorGUI.indent, position.y, labelWidth - EditorGUI.indent, position.height);
                Rect rect3 = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, position.height);
                EditorGUI.TextArea(rect3, content.text, EditorStyles.label);
            }
            if (GUI.Button(rect2, EditorGUIUtility.TextContent("Browse Keystore")))
            {
                this.m_KeystoreAvailableKeys = null;
                string currentDirectory = Directory.GetCurrentDirectory();
                if (this.m_KeystoreCreate)
                {
                    this.m_AndroidKeystoreName.stringValue = EditorUtility.SaveFilePanel(EditorGUIUtility.TextContent("Create a new keystore...").text, currentDirectory, "user.keystore", "keystore");
                }
                else
                {
                    this.m_AndroidKeystoreName.stringValue = EditorUtility.OpenFilePanel(EditorGUIUtility.TextContent("Open existing keystore...").text, currentDirectory, "keystore");
                }
                if (this.m_KeystoreCreate && File.Exists(this.m_AndroidKeystoreName.stringValue))
                {
                    FileUtil.DeleteFileOrDirectory(this.m_AndroidKeystoreName.stringValue);
                }
                currentDirectory = currentDirectory + Path.DirectorySeparatorChar;
                if (this.m_AndroidKeystoreName.stringValue.StartsWith(currentDirectory))
                {
                    this.m_AndroidKeystoreName.stringValue = this.m_AndroidKeystoreName.stringValue.Substring(currentDirectory.Length);
                }
                this.m_SettingsEditor.serializedObject.ApplyModifiedProperties();
                GUIUtility.ExitGUI();
            }
            EditorGUI.BeginChangeCheck();
            keystorePass = EditorGUI.PasswordField(GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, h, h, EditorStyles.layerMaskField, null), EditorGUIUtility.TextContent("Keystore password"), keystorePass);
            if (EditorGUI.EndChangeCheck())
            {
                AndroidKeystoreWindow.GetAvailableKeys("", "");
                this.m_KeystoreAvailableKeys = null;
            }
            using (new EditorGUI.DisabledScope(!this.m_KeystoreCreate))
            {
                position = GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, h, h, EditorStyles.layerMaskField, null);
                this.m_KeystoreConfirm = EditorGUI.PasswordField(position, EditorGUIUtility.TextContent("Confirm password"), this.m_KeystoreConfirm);
            }
            GUIContent content2 = null;
            flag = false;
            if (keystorePass.Length == 0)
            {
                content2 = EditorGUIUtility.TextContent("Enter password.");
            }
            else if (keystorePass.Length < 6)
            {
                content2 = EditorGUIUtility.TextContent("Password must be at least 6 characters.");
            }
            else if (this.m_KeystoreCreate && (this.m_KeystoreConfirm.Length == 0))
            {
                content2 = EditorGUIUtility.TextContent("Confirm keystore password.");
            }
            else if (this.m_KeystoreCreate && !keystorePass.Equals(this.m_KeystoreConfirm))
            {
                content2 = EditorGUIUtility.TextContent("Passwords do not match.");
            }
            else
            {
                content2 = EditorGUIUtility.TempContent(" ");
                flag = true;
            }
            GUILayout.Label(content2, EditorStyles.miniLabel, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            GUILayout.Label(EditorGUIUtility.TextContent("Key"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            string[] array = new string[] { EditorGUIUtility.TextContent("Unsigned (debug)").text };
            position = GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, h, h, EditorStyles.layerMaskField, null);
            if (flag && !this.m_KeystoreCreate)
            {
                float num2 = EditorGUIUtility.labelWidth;
                Rect rect4 = new Rect(position.x + num2, position.y, position.width - num2, position.height);
                if (((Event.current.type == EventType.MouseDown) && (Event.current.button == 0)) && rect4.Contains(Event.current.mousePosition))
                {
                    string keystore = !Path.IsPathRooted(this.m_AndroidKeystoreName.stringValue) ? Path.Combine(Directory.GetCurrentDirectory(), this.m_AndroidKeystoreName.stringValue) : this.m_AndroidKeystoreName.stringValue;
                    this.m_KeystoreAvailableKeys = AndroidKeystoreWindow.GetAvailableKeys(keystore, keystorePass);
                }
            }
            else
            {
                AndroidKeystoreWindow.GetAvailableKeys("", "");
                this.m_KeystoreAvailableKeys = null;
            }
            int selectedIndex = 0;
            if ((this.m_KeystoreAvailableKeys != null) && (this.m_KeystoreAvailableKeys.Length != 0))
            {
                ArrayUtility.AddRange<string>(ref array, this.m_KeystoreAvailableKeys);
            }
            else if (this.m_AndroidKeyaliasName.stringValue.Length != 0)
            {
                ArrayUtility.Add<string>(ref array, this.m_AndroidKeyaliasName.stringValue);
            }
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(this.m_AndroidKeyaliasName.stringValue))
                {
                    selectedIndex = i;
                }
            }
            bool flag4 = flag && ((this.m_KeystoreCreate && (this.m_AndroidKeystoreName.stringValue.Length != 0)) || (this.m_KeystoreAvailableKeys != null));
            if (flag4)
            {
                ArrayUtility.Add<string>(ref array, "");
                ArrayUtility.Add<string>(ref array, EditorGUIUtility.TextContent("Create a new key").text);
            }
            int num5 = EditorGUI.Popup(position, EditorGUIUtility.TextContent("Alias"), selectedIndex, EditorGUIUtility.TempContent(array), EditorStyles.popup);
            if (flag4 && (num5 == (array.Length - 1)))
            {
                num5 = selectedIndex;
                this.m_KeystoreCreate = false;
                string str5 = !Path.IsPathRooted(this.m_AndroidKeystoreName.stringValue) ? Path.Combine(Directory.GetCurrentDirectory(), this.m_AndroidKeystoreName.stringValue) : this.m_AndroidKeystoreName.stringValue;
                AndroidKeystoreWindow.ShowAndroidKeystoreWindow(PlayerSettings.companyName, str5, PlayerSettings.keystorePass);
                GUIUtility.ExitGUI();
            }
            if (num5 != selectedIndex)
            {
                selectedIndex = num5;
                this.m_AndroidKeyaliasName.stringValue = (selectedIndex != 0) ? array[selectedIndex] : "";
            }
            using (new EditorGUI.DisabledScope(selectedIndex == 0))
            {
                keyaliasPass = EditorGUI.PasswordField(GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, h, h, EditorStyles.layerMaskField, null), EditorGUIUtility.TextContent("Password"), keyaliasPass);
            }
            GUIContent content3 = null;
            if (selectedIndex == 0)
            {
                content3 = EditorGUIUtility.TempContent(" ");
            }
            else if (keyaliasPass.Length == 0)
            {
                content3 = EditorGUIUtility.TextContent("Enter password.");
            }
            else if (keyaliasPass.Length < 6)
            {
                content3 = EditorGUIUtility.TextContent("Password must be at least 6 characters.");
            }
            else
            {
                content3 = EditorGUIUtility.TempContent(" ");
            }
            GUILayout.Label(content3, EditorStyles.miniLabel, new GUILayoutOption[0]);
            PlayerSettings.keystorePass = keystorePass;
            PlayerSettings.keyaliasPass = keyaliasPass;
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.m_APKExpansionFiles, EditorGUIUtility.TextContent("Split Application Binary|Split application binary into expansion files\n(use only with Google Play Store if larger than 50 MB)"), new GUILayoutOption[0]);
            EditorGUILayout.Space();
        }

        public override void SplashSectionGUI()
        {
            this.m_AndroidSplashScreen.objectReferenceValue = EditorGUILayout.ObjectField(EditorGUIUtility.TextContent("Android Splash Screen"), (Texture2D) this.m_AndroidSplashScreen.objectReferenceValue, typeof(Texture2D), false, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            using (new EditorGUI.DisabledScope(this.m_AndroidSplashScreen.objectReferenceValue == null))
            {
                EditorGUILayout.PropertyField(this.m_AndroidSplashScreenScale, EditorGUIUtility.TextContent("Splash scaling"), new GUILayoutOption[0]);
            }
        }

        public override bool SupportsOrientation()
        {
            return true;
        }
    }
}

