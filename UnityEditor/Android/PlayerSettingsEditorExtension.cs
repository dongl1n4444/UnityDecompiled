namespace UnityEditor.Android
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEngine;

    internal class PlayerSettingsEditorExtension : DefaultPlayerSettingsEditorExtension
    {
        private static readonly Texts k_Texts = new Texts();
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
        private SerializedProperty m_AndroidTargetSdkVersion;
        private SerializedProperty m_AndroidTVCompatibility;
        private SerializedProperty m_APKExpansionFiles;
        private SerializedProperty m_ApplicationBundleVersion;
        private SerializedProperty m_CreateWallpaper;
        private SerializedProperty m_ForceInternetPermission;
        private SerializedProperty m_ForceSDCardPermission;
        private string[] m_KeystoreAvailableKeys = null;
        private string m_KeystoreConfirm = "";
        private bool m_KeystoreCreate = false;
        private PlayerSettingsEditor m_SettingsEditor;

        public override bool CanShowUnitySplashScreen() => 
            true;

        public override void ConfigurationSectionGUI()
        {
            EditorGUILayout.PropertyField(this.m_AndroidTargetDevice, k_Texts.deviceFilter, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_AndroidPreferredInstallLocation, k_Texts.installLocation, new GUILayoutOption[0]);
            EditorGUI.BeginChangeCheck();
            bool boolValue = this.m_ForceInternetPermission.boolValue;
            string[] displayedOptions = new string[] { "Auto", "Require" };
            boolValue = EditorGUILayout.Popup(k_Texts.internetAccess.text, !boolValue ? 0 : 1, displayedOptions, new GUILayoutOption[0]) == 1;
            if (EditorGUI.EndChangeCheck())
            {
                this.m_ForceInternetPermission.boolValue = boolValue;
            }
            EditorGUI.BeginChangeCheck();
            bool flag2 = this.m_ForceSDCardPermission.boolValue;
            string[] texts = new string[] { "Internal", "External (SDCard)" };
            flag2 = EditorGUILayout.Popup(k_Texts.writeExternal, !flag2 ? 0 : 1, EditorGUIUtility.TempContent(texts), new GUILayoutOption[0]) == 1;
            if (EditorGUI.EndChangeCheck())
            {
                this.m_ForceSDCardPermission.boolValue = flag2;
            }
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.m_AndroidTVCompatibility, k_Texts.tvCompatibility, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_AndroidIsGame, k_Texts.game, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_AndroidGamepadSupportLevel, k_Texts.gamepadSupport, new GUILayoutOption[0]);
            EditorGUILayout.Space();
        }

        public override bool HasIdentificationGUI() => 
            true;

        public override void IconSectionGUI()
        {
            GUI.changed = false;
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.m_AndroidEnableBanner, k_Texts.banner, new GUILayoutOption[0]);
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
            PlayerSettingsEditor.ShowApplicationIdentifierUI(this.m_SettingsEditor.serializedObject, BuildTargetGroup.Android, k_Texts.packageName.text, k_Texts.packageUndo.text);
            EditorGUILayout.PropertyField(this.m_ApplicationBundleVersion, k_Texts.version, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_AndroidBundleVersionCode, k_Texts.bundleVersion, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_AndroidMinSdkVersion, k_Texts.minimumLevel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_AndroidTargetSdkVersion, k_Texts.targetLevel, new GUILayoutOption[0]);
        }

        public override void OnEnable(PlayerSettingsEditor settingsEditor)
        {
            this.m_AndroidBundleVersionCode = settingsEditor.FindPropertyAssert("AndroidBundleVersionCode");
            this.m_AndroidKeystoreName = settingsEditor.FindPropertyAssert("AndroidKeystoreName");
            this.m_AndroidKeyaliasName = settingsEditor.FindPropertyAssert("AndroidKeyaliasName");
            this.m_AndroidMinSdkVersion = settingsEditor.FindPropertyAssert("AndroidMinSdkVersion");
            this.m_AndroidTargetSdkVersion = settingsEditor.FindPropertyAssert("AndroidTargetSdkVersion");
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
            this.m_ApplicationBundleVersion = settingsEditor.FindPropertyAssert("bundleVersion");
            this.m_SettingsEditor = settingsEditor;
        }

        public override void PublishSectionGUI(float h, float kLabelFloatMinW, float kLabelFloatMaxW)
        {
            Rect rect2;
            string keystorePass = PlayerSettings.keystorePass;
            string keyaliasPass = PlayerSettings.keyaliasPass;
            bool flag = false;
            GUILayout.Label(k_Texts.keystore, EditorStyles.boldLabel, new GUILayoutOption[0]);
            Rect position = GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, h + 5f, h + 5f, EditorStyles.layerMaskField, null);
            GUIContent[] contents = new GUIContent[] { k_Texts.useExistingKeystore, k_Texts.createKeystore };
            bool flag2 = GUI.SelectionGrid(position, !this.m_KeystoreCreate ? 0 : 1, contents, 2, "toggle") == 1;
            if (flag2 != this.m_KeystoreCreate)
            {
                this.m_KeystoreCreate = flag2;
                this.m_AndroidKeystoreName.stringValue = "";
                this.m_AndroidKeyaliasName.stringValue = "";
                this.m_KeystoreAvailableKeys = null;
            }
            position = GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, h, h, EditorStyles.layerMaskField, null);
            GUIContent browseSelectName = null;
            bool disabled = this.m_AndroidKeystoreName.stringValue.Length == 0;
            using (new EditorGUI.DisabledScope(disabled))
            {
                if (disabled)
                {
                    browseSelectName = k_Texts.browseSelectName;
                }
                else
                {
                    browseSelectName = EditorGUIUtility.TempContent(this.m_AndroidKeystoreName.stringValue);
                }
                float labelWidth = EditorGUIUtility.labelWidth;
                rect2 = new Rect(position.x + EditorGUI.indent, position.y, labelWidth - EditorGUI.indent, position.height);
                Rect rect3 = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, position.height);
                EditorGUI.TextArea(rect3, browseSelectName.text, EditorStyles.label);
            }
            if (GUI.Button(rect2, k_Texts.browseKeystore))
            {
                this.m_KeystoreAvailableKeys = null;
                string currentDirectory = Directory.GetCurrentDirectory();
                if (this.m_KeystoreCreate)
                {
                    this.m_AndroidKeystoreName.stringValue = EditorUtility.SaveFilePanel(k_Texts.createKeystore.text, currentDirectory, "user.keystore", "keystore");
                }
                else
                {
                    this.m_AndroidKeystoreName.stringValue = EditorUtility.OpenFilePanel(k_Texts.openKeystore.text, currentDirectory, "keystore");
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
            keystorePass = EditorGUI.PasswordField(GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, h, h, EditorStyles.layerMaskField, null), k_Texts.keystorePassword, keystorePass);
            if (EditorGUI.EndChangeCheck())
            {
                AndroidKeystoreWindow.GetAvailableKeys("", "");
                this.m_KeystoreAvailableKeys = null;
            }
            using (new EditorGUI.DisabledScope(!this.m_KeystoreCreate))
            {
                position = GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, h, h, EditorStyles.layerMaskField, null);
                this.m_KeystoreConfirm = EditorGUI.PasswordField(position, k_Texts.confirmPassword, this.m_KeystoreConfirm);
            }
            GUIContent enterPassword = null;
            flag = false;
            if (keystorePass.Length == 0)
            {
                enterPassword = k_Texts.enterPassword;
            }
            else if (keystorePass.Length < 6)
            {
                enterPassword = k_Texts.passwordLength;
            }
            else if (this.m_KeystoreCreate && (this.m_KeystoreConfirm.Length == 0))
            {
                enterPassword = k_Texts.confirmPassword;
            }
            else if (this.m_KeystoreCreate && !keystorePass.Equals(this.m_KeystoreConfirm))
            {
                enterPassword = k_Texts.passwordMatch;
            }
            else
            {
                enterPassword = EditorGUIUtility.TempContent(" ");
                flag = true;
            }
            GUILayout.Label(enterPassword, EditorStyles.miniLabel, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            GUILayout.Label(k_Texts.key, EditorStyles.boldLabel, new GUILayoutOption[0]);
            string[] array = new string[] { k_Texts.unsignedDebug.text };
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
                ArrayUtility.Add<string>(ref array, k_Texts.newKey.text);
            }
            int num5 = EditorGUI.Popup(position, k_Texts.alias, selectedIndex, EditorGUIUtility.TempContent(array), EditorStyles.popup);
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
                keyaliasPass = EditorGUI.PasswordField(GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, h, h, EditorStyles.layerMaskField, null), k_Texts.password, keyaliasPass);
            }
            GUIContent passwordLength = null;
            if (selectedIndex == 0)
            {
                passwordLength = EditorGUIUtility.TempContent(" ");
            }
            else if (keyaliasPass.Length == 0)
            {
                passwordLength = k_Texts.enterPassword;
            }
            else if (keyaliasPass.Length < 6)
            {
                passwordLength = k_Texts.passwordLength;
            }
            else
            {
                passwordLength = EditorGUIUtility.TempContent(" ");
            }
            GUILayout.Label(passwordLength, EditorStyles.miniLabel, new GUILayoutOption[0]);
            PlayerSettings.keystorePass = keystorePass;
            PlayerSettings.keyaliasPass = keyaliasPass;
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.m_APKExpansionFiles, k_Texts.splitApplicationBinary, new GUILayoutOption[0]);
            EditorGUILayout.Space();
        }

        public override void SplashSectionGUI()
        {
            GUILayout.Label(k_Texts.staticSplash, EditorStyles.boldLabel, new GUILayoutOption[0]);
            Rect totalPosition = EditorGUILayout.GetControlRect(true, 64f, EditorStyles.objectFieldThumb, new GUILayoutOption[0]);
            GUIContent label = EditorGUI.BeginProperty(totalPosition, k_Texts.image, this.m_AndroidSplashScreen);
            this.m_AndroidSplashScreen.objectReferenceValue = EditorGUI.ObjectField(totalPosition, label, (Texture2D) this.m_AndroidSplashScreen.objectReferenceValue, typeof(Texture2D), false);
            EditorGUI.EndProperty();
            EditorGUILayout.Space();
            using (new EditorGUI.DisabledScope(this.m_AndroidSplashScreen.objectReferenceValue == null))
            {
                EditorGUILayout.PropertyField(this.m_AndroidSplashScreenScale, k_Texts.scaling, new GUILayoutOption[0]);
            }
        }

        public override bool SupportsOrientation() => 
            true;

        private class Texts
        {
            public GUIContent alias = EditorGUIUtility.TextContent("Alias");
            public GUIContent banner = EditorGUIUtility.TextContent("Enable Android Banner|If enabled, android banner will be added to the APK resources");
            public GUIContent browseKeystore = EditorGUIUtility.TextContent("Browse Keystore");
            public GUIContent browseSelectName = EditorGUIUtility.TextContent("Browse to select keystore name");
            public GUIContent bundleVersion = EditorGUIUtility.TextContent("Bundle Version Code");
            public GUIContent confirmPassword = EditorGUIUtility.TextContent("Confirm keystore password.");
            public GUIContent createKeystore = EditorGUIUtility.TextContent("Create a new keystore...");
            public GUIContent deviceFilter = EditorGUIUtility.TextContent("Device Filter");
            public GUIContent enterPassword = EditorGUIUtility.TextContent("Enter password.");
            public GUIContent game = EditorGUIUtility.TextContent("Android Game|If enabled, built APK will be marked as a game rather than a regular app");
            public GUIContent gamepadSupport = EditorGUIUtility.TextContent("Android Gamepad Support Level");
            public GUIContent image = EditorGUIUtility.TextContent("Image");
            public GUIContent installLocation = EditorGUIUtility.TextContent("Install Location");
            public GUIContent internetAccess = EditorGUIUtility.TextContent("Internet Access");
            public GUIContent key = EditorGUIUtility.TextContent("Key");
            public GUIContent keystore = EditorGUIUtility.TextContent("Keystore");
            public GUIContent keystorePassword = EditorGUIUtility.TextContent("Keystore password");
            public GUIContent minimumLevel = EditorGUIUtility.TextContent("Minimum API Level");
            public GUIContent newKey = EditorGUIUtility.TextContent("Create a new key");
            public GUIContent openKeystore = EditorGUIUtility.TextContent("Open existing keystore...");
            public GUIContent packageName = EditorGUIUtility.TextContent("Package Name");
            public GUIContent packageUndo = EditorGUIUtility.TextContent("Changed Android package field");
            public GUIContent password = EditorGUIUtility.TextContent("Password");
            public GUIContent passwordLength = EditorGUIUtility.TextContent("Password must be at least 6 characters.");
            public GUIContent passwordMatch = EditorGUIUtility.TextContent("Passwords do not match.");
            public GUIContent scaling = EditorGUIUtility.TextContent("Scaling");
            public GUIContent splitApplicationBinary = EditorGUIUtility.TextContent("Split Application Binary|Split application binary into expansion files\n(use only with Google Play Store if larger than 50 MB)");
            public GUIContent staticSplash = EditorGUIUtility.TextContent("Static Splash Image|This image will be shown while the engine is loading");
            public GUIContent targetLevel = EditorGUIUtility.TextContent("Target API Level");
            public GUIContent tvCompatibility = EditorGUIUtility.TextContent("Android TV Compatibility|Check the game for Android TV compatibility, update the manifest if needed");
            public GUIContent unsignedDebug = EditorGUIUtility.TextContent("Unsigned (debug)");
            public GUIContent useExistingKeystore = EditorGUIUtility.TextContent("Use Existing Keystore");
            public GUIContent version = EditorGUIUtility.TextContent("Version*");
            public GUIContent writeExternal = EditorGUIUtility.TextContent("Write Permission|Since KITKAT writing to external cache and persistent data paths doesn't require write external permission.");
        }
    }
}

