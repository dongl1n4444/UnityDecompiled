namespace UnityEditor.WSA
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEngine;

    internal class MetroSettingsEditorExtension : DefaultPlayerSettingsEditorExtension
    {
        private Vector2 capScrollViewPosition = Vector2.zero;
        private SerializedProperty m_CompanyName;
        private SerializedProperty m_DefaultIsFullScreen;
        private GUIContent m_GUIDefaultIsFullScreen;
        private SerializedProperty m_HolographicPauseOnTrackingLoss;
        private SerializedProperty m_HolographicSplashScreen;
        private SerializedProperty m_HolographicTrackingLossScreen;
        private MetroScaledImageGroup m_LargeTiles;
        private MetroScaledImageGroup m_MediumTiles;
        private SerializedProperty m_MetroApplicationDescription;
        private SerializedProperty m_MetroFTAFileTypes;
        private SerializedProperty m_MetroFTAName;
        private SerializedProperty m_MetroPackageName;
        private SerializedProperty m_MetroPackageVersion;
        private SerializedProperty m_MetroProtocolName;
        private SerializedProperty m_MetroTileBackgroundColor;
        private SerializedProperty m_MetroTileShortName;
        private MetroScaledImageGroup m_PackageLogos;
        private MetroScaledImageGroup m_PhoneAppIcons;
        private MetroScaledImageGroup m_PhoneMediumTiles;
        private MetroScaledImageGroup m_PhoneSmallTiles;
        private MetroScaledImage[] m_PhoneSplashScreens;
        private MetroScaledImageGroup m_PhoneWideTiles;
        private SerializedProperty m_ProductName;
        private SerializedProperty m_RunInBackground;
        private PlayerSettingsEditor m_SettingsEditor;
        private MetroScaledImageGroup m_SmallTiles;
        private MetroScaledImageGroup m_StoreSmallLogos;
        private MetroScaledImageGroup m_UWPSquare150x150Logos;
        private MetroScaledImageGroup m_UWPSquare310x310Logos;
        private MetroScaledImageGroup m_UWPSquare44x44Logos;
        private MetroScaledImageGroup m_UWPSquare71x71Logos;
        private MetroScaledImageGroup m_UWPWide310x150Logos;
        private MetroScaledImageGroup m_WideTiles;
        private MetroScaledImage[] m_WindowsSplashScreens;
        private Vector2 metroFTAFileTypesScrollViewPosition = Vector2.zero;

        public override bool CanShowUnitySplashScreen() => 
            true;

        private static MetroScaledImageGroup CreateScaledImageGroup(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale[] scales, string labelFormat, int[] widths, int[] heights) => 
            new MetroScaledImageGroup(CreateScaledImages(type, scales, labelFormat, widths, heights));

        private static MetroScaledImage[] CreateScaledImages(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale[] scales, string labelFormat, int[] widths, int[] heights)
        {
            if (((scales.Length == 0) || (scales.Length != widths.Length)) || (scales.Length != heights.Length))
            {
                throw new Exception("If you see this, it's a bug. Scales, widths and heights must all be equal and non-zero");
            }
            MetroScaledImage[] imageArray = new MetroScaledImage[scales.Length];
            for (int i = 0; i < scales.Length; i++)
            {
                imageArray[i] = new MetroScaledImage(type, scales[i], string.Format(labelFormat, (int) scales[i], widths[i], heights[i]), widths[i], heights[i]);
            }
            return imageArray;
        }

        private static MetroScaledImageGroup CreateScaledSquareImageGroup(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale[] scales, string labelFormat, int[] sizes) => 
            CreateScaledImageGroup(type, scales, labelFormat, sizes, sizes);

        private static MetroScaledImage[] CreateScaledSquareImages(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale[] scales, string labelFormat, int[] sizes) => 
            CreateScaledImages(type, scales, labelFormat, sizes, sizes);

        public override bool HasResolutionSection() => 
            true;

        public override void IconSectionGUI()
        {
            this.MetroLogoSection();
        }

        private void ImageField(MetroScaledImage image, GUIContent label)
        {
            this.ImageField(image.type, image.scale, label, image.width, image.height, ref image.imagePath, ref image.image);
        }

        private void ImageField(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale, GUIContent label, int imageWidth, int imageHeight, ref string imagePath, ref Texture2D image)
        {
            if (imagePath == null)
            {
                imagePath = PlayerSettings.WSA.GetVisualAssetsImage(type, scale);
            }
            if ((image == null) && !string.IsNullOrEmpty(imagePath))
            {
                image = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath);
            }
            Texture2D assetObject = (Texture2D) EditorGUILayout.ObjectField(label, image, typeof(Texture2D), false, new GUILayoutOption[0]);
            string b = (assetObject == null) ? "" : AssetDatabase.GetAssetPath(assetObject);
            if (!string.Equals(imagePath, b) && (!string.IsNullOrEmpty(b) ? ValidateImage(b, imageWidth, imageHeight) : true))
            {
                Undo.RecordObject(this.m_SettingsEditor.serializedObject.targetObject, "Change WSA Visual Asset");
                PlayerSettings.WSA.SetVisualAssetsImage(b, type, scale);
                imagePath = b;
                image = assetObject;
            }
        }

        private void MetroLogoSection()
        {
            this.m_PackageLogos.unfolded = this.ScaledImageGroup("Store Logo", this.m_PackageLogos);
            EditorGUILayout.Space();
            GUILayout.Label(EditorGUIUtility.TextContent("Tile"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_MetroTileShortName, EditorGUIUtility.TextContent("Short name|Specifies an abbreviated name for the app."), new GUILayoutOption[0]);
            this.m_MetroTileShortName.stringValue = this.ValidateMetroTileShortName(this.m_MetroTileShortName.stringValue);
            switch (PlayerSettings.WSA.tileShowName)
            {
                case PlayerSettings.WSAApplicationShowName.NotSet:
                    goto Label_00D5;

                case PlayerSettings.WSAApplicationShowName.AllLogos:
                    PlayerSettings.WSA.mediumTileShowName = true;
                    PlayerSettings.WSA.largeTileShowName = true;
                    PlayerSettings.WSA.wideTileShowName = true;
                    break;

                case PlayerSettings.WSAApplicationShowName.StandardLogoOnly:
                    PlayerSettings.WSA.mediumTileShowName = true;
                    PlayerSettings.WSA.largeTileShowName = true;
                    break;

                case PlayerSettings.WSAApplicationShowName.WideLogoOnly:
                    PlayerSettings.WSA.wideTileShowName = true;
                    break;
            }
            PlayerSettings.WSA.tileShowName = PlayerSettings.WSAApplicationShowName.NotSet;
        Label_00D5:
            GUILayout.Label(EditorGUIUtility.TextContent("Show name on|Specifies which logos the app name appears on."), new GUILayoutOption[0]);
            PlayerSettings.WSA.mediumTileShowName = GUILayout.Toggle(PlayerSettings.WSA.mediumTileShowName, EditorGUIUtility.TextContent("Medium Tile"), new GUILayoutOption[0]);
            PlayerSettings.WSA.largeTileShowName = GUILayout.Toggle(PlayerSettings.WSA.largeTileShowName, EditorGUIUtility.TextContent("Large Tile"), new GUILayoutOption[0]);
            PlayerSettings.WSA.wideTileShowName = GUILayout.Toggle(PlayerSettings.WSA.wideTileShowName, EditorGUIUtility.TextContent("Wide Tile"), new GUILayoutOption[0]);
            PlayerSettings.WSA.tileForegroundText = (PlayerSettings.WSAApplicationForegroundText) EditorGUILayout.EnumPopup(EditorGUIUtility.TextContent("Foreground text|Sets the value of the text color relative to the background color on the app's tile in Windows."), PlayerSettings.WSA.tileForegroundText, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_MetroTileBackgroundColor, EditorGUIUtility.TextContent("Background color|Sets the background color for the app's tile in Windows."), new GUILayoutOption[0]);
            PlayerSettings.WSA.defaultTileSize = (PlayerSettings.WSADefaultTileSize) EditorGUILayout.EnumPopup(EditorGUIUtility.TextContent("Default Size"), PlayerSettings.WSA.defaultTileSize, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            GUILayout.Label(EditorGUIUtility.TextContent("Windows Tiles and Logos"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            this.m_StoreSmallLogos.unfolded = this.ScaledImageGroup("Small Logo", this.m_StoreSmallLogos);
            this.m_MediumTiles.unfolded = this.ScaledImageGroup("Medium Tile", this.m_MediumTiles);
            this.m_WideTiles.unfolded = this.ScaledImageGroup("Wide Tile", this.m_WideTiles);
            this.m_SmallTiles.unfolded = this.ScaledImageGroup("Small Tile", this.m_SmallTiles);
            this.m_LargeTiles.unfolded = this.ScaledImageGroup("Large Tile", this.m_LargeTiles);
            EditorGUILayout.Space();
            GUILayout.Label(EditorGUIUtility.TextContent("Windows Phone Tiles and Logos"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            this.m_PhoneAppIcons.unfolded = this.ScaledImageGroup("Application Icon", this.m_PhoneAppIcons);
            this.m_PhoneSmallTiles.unfolded = this.ScaledImageGroup("Small Tile", this.m_PhoneSmallTiles);
            this.m_PhoneMediumTiles.unfolded = this.ScaledImageGroup("Medium Tile", this.m_PhoneMediumTiles);
            this.m_PhoneWideTiles.unfolded = this.ScaledImageGroup("Wide Tile", this.m_PhoneWideTiles);
            EditorGUILayout.Space();
            GUILayout.Label(EditorGUIUtility.TextContent("Universal 10 Tiles and Logos"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            this.m_UWPSquare44x44Logos.unfolded = this.ScaledImageGroup("Square 44x44 Logo", this.m_UWPSquare44x44Logos);
            this.m_UWPSquare71x71Logos.unfolded = this.ScaledImageGroup("Square 71x71 Logo", this.m_UWPSquare71x71Logos);
            this.m_UWPSquare150x150Logos.unfolded = this.ScaledImageGroup("Square 150x150 Logo", this.m_UWPSquare150x150Logos);
            this.m_UWPSquare310x310Logos.unfolded = this.ScaledImageGroup("Square 310x310 Logo", this.m_UWPSquare310x310Logos);
            this.m_UWPWide310x150Logos.unfolded = this.ScaledImageGroup("Wide 310x150 Logo", this.m_UWPWide310x150Logos);
            EditorGUILayout.Space();
        }

        private void MetroSplashScreenSection(float kLabelMinWidth, float kLabelMaxWidth, float kLabelMinHeight, float kLabelMaxHeight)
        {
            EditorGUILayout.Space();
            GUILayout.Label(EditorGUIUtility.TextContent("Windows"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            foreach (MetroScaledImage image in this.m_WindowsSplashScreens)
            {
                this.ImageField(image, EditorGUIUtility.TextContent(image.label));
            }
            EditorGUILayout.Space();
            GUILayout.Label(EditorGUIUtility.TextContent("Windows Phone"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            foreach (MetroScaledImage image2 in this.m_PhoneSplashScreens)
            {
                this.ImageField(image2, EditorGUIUtility.TextContent(image2.label));
            }
            EditorGUILayout.Space();
            GUILayout.Label(EditorGUIUtility.TextContent("Windows Holographic"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.m_HolographicSplashScreen.objectReferenceValue = EditorGUILayout.ObjectField(EditorGUIUtility.TextContent("Holographic Splash Image"), (Texture2D) this.m_HolographicSplashScreen.objectReferenceValue, typeof(Texture2D), false, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_HolographicPauseOnTrackingLoss, EditorGUIUtility.TextContent("On Tracking Loss Pause and Show Image"), new GUILayoutOption[0]);
            this.m_HolographicTrackingLossScreen.objectReferenceValue = EditorGUILayout.ObjectField(EditorGUIUtility.TextContent("Tracking Loss Image"), (Texture2D) this.m_HolographicTrackingLossScreen.objectReferenceValue, typeof(Texture2D), false, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            bool hasValue = PlayerSettings.WSA.splashScreenBackgroundColor.HasValue;
            bool flag2 = EditorGUILayout.BeginToggleGroup(EditorGUIUtility.TextContent("Overwrite background color|Uses default background color otherwise."), hasValue);
            Rect rect = GUILayoutUtility.GetRect(kLabelMinWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, EditorStyles.layerMaskField);
            Rect position = new Rect(rect.x - EditorGUI.indent, rect.y, EditorGUIUtility.labelWidth - EditorGUI.indent, rect.height);
            Rect rect3 = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, rect.width - EditorGUIUtility.labelWidth, rect.height);
            EditorGUI.LabelField(position, EditorGUIUtility.TextContent("Background color|Sets the background color for the app's splash screen."));
            if (flag2 != hasValue)
            {
                if (flag2)
                {
                    PlayerSettings.WSA.splashScreenBackgroundColor = new Color?(PlayerSettings.WSA.tileBackgroundColor);
                }
                else
                {
                    PlayerSettings.WSA.splashScreenBackgroundColor = null;
                }
            }
            if (flag2)
            {
                PlayerSettings.WSA.splashScreenBackgroundColor = new Color?(EditorGUI.ColorField(rect3, PlayerSettings.WSA.splashScreenBackgroundColor.Value));
            }
            else
            {
                EditorGUI.ColorField(rect3, PlayerSettings.WSA.tileBackgroundColor);
            }
            EditorGUILayout.EndToggleGroup();
        }

        public override void OnEnable(PlayerSettingsEditor settingsEditor)
        {
            this.m_SettingsEditor = settingsEditor;
            this.m_GUIDefaultIsFullScreen = EditorGUIUtility.TextContent("Default Is Full Screen*");
            this.m_CompanyName = settingsEditor.FindPropertyAssert("companyName");
            this.m_ProductName = settingsEditor.FindPropertyAssert("productName");
            this.m_DefaultIsFullScreen = settingsEditor.FindPropertyAssert("defaultIsFullScreen");
            this.m_RunInBackground = settingsEditor.FindPropertyAssert("runInBackground");
            this.m_MetroPackageName = settingsEditor.FindPropertyAssert("metroPackageName");
            this.m_MetroPackageName.stringValue = Utility.TryValidatePackageName(this.m_MetroPackageName.stringValue);
            this.m_MetroPackageVersion = settingsEditor.FindPropertyAssert("metroPackageVersion");
            this.m_MetroPackageVersion.stringValue = PlayerSettings.WSA.ValidatePackageVersion(this.m_MetroPackageVersion.stringValue);
            this.m_MetroApplicationDescription = settingsEditor.FindPropertyAssert("metroApplicationDescription");
            this.m_MetroApplicationDescription.stringValue = this.ValidateMetroApplicationDescription(this.m_MetroApplicationDescription.stringValue);
            this.m_MetroTileShortName = settingsEditor.FindPropertyAssert("metroTileShortName");
            this.m_MetroTileShortName.stringValue = this.ValidateMetroTileShortName(this.m_MetroTileShortName.stringValue);
            this.m_MetroTileBackgroundColor = settingsEditor.FindPropertyAssert("metroTileBackgroundColor");
            this.m_MetroFTAName = settingsEditor.FindPropertyAssert("metroFTAName");
            this.m_MetroFTAFileTypes = settingsEditor.FindPropertyAssert("metroFTAFileTypes");
            this.m_MetroProtocolName = settingsEditor.FindPropertyAssert("metroProtocolName");
            PlayerSettings.WSAImageScale[] scales = new PlayerSettings.WSAImageScale[] { PlayerSettings.WSAImageScale._80 };
            PlayerSettings.WSAImageScale[] scaleArray2 = new PlayerSettings.WSAImageScale[] { PlayerSettings.WSAImageScale._100 };
            PlayerSettings.WSAImageScale[] scaleArray3 = new PlayerSettings.WSAImageScale[] { PlayerSettings.WSAImageScale._100 };
            this.m_PackageLogos = CreateScaledSquareImageGroup(PlayerSettings.WSAImageType.PackageLogo, new PlayerSettings.WSAImageScale[] { PlayerSettings.WSAImageScale._100 }, "Scale {0}% ({1}x{2} pixels)|Specifies the image that appears on the Store description page for the product.", new int[] { 50, 0x3f, 70, 0x4b, 90, 100, 120, 200 });
            MetroScaledImage[] sourceArray = CreateScaledSquareImages(PlayerSettings.WSAImageType.StoreTileSmallLogo, scales, "Scale {0}% ({1}x{2} pixels)|Specifies the image that is shown when viewing all installed applications.", new int[] { 0x18, 30, 0x2a, 0x36 });
            MetroScaledImage[] imageArray2 = CreateScaledSquareImages(PlayerSettings.WSAImageType.StoreTileSmallLogo, new PlayerSettings.WSAImageScale[] { PlayerSettings.WSAImageScale.Target16 }, "Target size {0} ({1}x{2} pixels)|Specifies the image that is shown when viewing all installed applications.", new int[] { 0x10, 0x20, 0x30, 0x100 });
            MetroScaledImage[] destinationArray = new MetroScaledImage[sourceArray.Length + imageArray2.Length];
            Array.Copy(sourceArray, destinationArray, sourceArray.Length);
            Array.Copy(imageArray2, 0, destinationArray, sourceArray.Length, imageArray2.Length);
            this.m_StoreSmallLogos = new MetroScaledImageGroup(destinationArray);
            this.m_MediumTiles = CreateScaledSquareImageGroup(PlayerSettings.WSAImageType.StoreTileLogo, scales, "Scale {0}% ({1}x{2} pixels)|Specifies the image to display on the app's tile in Windows.", new int[] { 120, 150, 210, 270 });
            this.m_WideTiles = CreateScaledImageGroup(PlayerSettings.WSAImageType.StoreTileWideLogo, scales, "Scale {0}% ({1}x{2} pixels)|Specifies the image to display on the app's wide tile in Windows if supported.", new int[] { 0xf8, 310, 0x1b2, 0x22e }, new int[] { 120, 150, 210, 270 });
            this.m_SmallTiles = CreateScaledSquareImageGroup(PlayerSettings.WSAImageType.StoreSmallTile, scales, "Scale {0}% ({1}x{2} pixels)|Specifies the image to display on the app's small tile in Windows if supported.", new int[] { 0x38, 70, 0x62, 0x7e });
            this.m_LargeTiles = CreateScaledSquareImageGroup(PlayerSettings.WSAImageType.StoreLargeTile, scales, "Scale {0}% ({1}x{2} pixels)|Specifies the image to display on the app's large tile in Windows if supported.", new int[] { 0xf8, 310, 0x1b2, 0x22e });
            this.m_PhoneAppIcons = CreateScaledSquareImageGroup(PlayerSettings.WSAImageType.PhoneAppIcon, scaleArray2, "Scale {0}% ({1}x{2} pixels)|Specifies the image to display on the app's small tile.", new int[] { 0x2c, 0x3e, 0x6a });
            this.m_PhoneSmallTiles = CreateScaledSquareImageGroup(PlayerSettings.WSAImageType.PhoneSmallTile, scaleArray2, "Scale {0}% ({1}x{2} pixels)|Specifies the image to display on the app's small tile", new int[] { 0x47, 0x63, 170 });
            this.m_PhoneMediumTiles = CreateScaledSquareImageGroup(PlayerSettings.WSAImageType.PhoneMediumTile, scaleArray2, "Scale {0}% ({1}x{2} pixels)|Specifies the image to display on the app's tile.", new int[] { 150, 210, 360 });
            this.m_PhoneWideTiles = CreateScaledImageGroup(PlayerSettings.WSAImageType.PhoneWideTile, scaleArray2, "Scale {0}% ({1}x{2} pixels)|Specifies the image to display on the app's wide tile.", new int[] { 310, 0x1b2, 0x2e8 }, new int[] { 150, 210, 360 });
            MetroScaledImage[] imageArray4 = CreateScaledSquareImages(PlayerSettings.WSAImageType.UWPSquare44x44Logo, scaleArray3, "Scale {0}% ({1}x{2} pixels)|Specifies the image to display on the app's tile.", new int[] { 0x2c, 0x37, 0x42, 0x58, 0xb0 });
            MetroScaledImage[] imageArray5 = CreateScaledSquareImages(PlayerSettings.WSAImageType.UWPSquare44x44Logo, new PlayerSettings.WSAImageScale[] { PlayerSettings.WSAImageScale.Target16 }, "Target size {0} ({1}x{2} pixels)|Specifies the image to display on the app's tile.", new int[] { 0x10, 0x18, 0x30, 0x100 });
            MetroScaledImage[] imageArray6 = new MetroScaledImage[imageArray4.Length + imageArray5.Length];
            Array.Copy(imageArray4, imageArray6, imageArray4.Length);
            Array.Copy(imageArray5, 0, imageArray6, imageArray4.Length, imageArray5.Length);
            this.m_UWPSquare44x44Logos = new MetroScaledImageGroup(imageArray6);
            this.m_UWPSquare71x71Logos = CreateScaledSquareImageGroup(PlayerSettings.WSAImageType.UWPSquare71x71Logo, scaleArray3, "Scale {0}% ({1}x{2} pixels)|Specifies the image to display on the app's tile.", new int[] { 0x47, 0x59, 0x6b, 0x8e, 0x11c });
            this.m_UWPSquare150x150Logos = CreateScaledSquareImageGroup(PlayerSettings.WSAImageType.UWPSquare150x150Logo, scaleArray3, "Scale {0}% ({1}x{2} pixels)|Specifies the image to display on the app's tile.", new int[] { 150, 0xbc, 0xe1, 300, 600 });
            this.m_UWPSquare310x310Logos = CreateScaledSquareImageGroup(PlayerSettings.WSAImageType.UWPSquare310x310Logo, scaleArray3, "Scale {0}% ({1}x{2} pixels)|Specifies the image to display on the app's tile.", new int[] { 310, 0x184, 0x1d1, 620, 0x4d8 });
            this.m_UWPWide310x150Logos = CreateScaledImageGroup(PlayerSettings.WSAImageType.UWPWide310x150Logo, scaleArray3, "Scale {0}% ({1}x{2} pixels)|Specifies the image to display on the app's tile.", new int[] { 310, 0x184, 0x1d1, 620, 0x4d8 }, new int[] { 150, 0xbc, 0xe1, 300, 600 });
            this.m_WindowsSplashScreens = CreateScaledImages(PlayerSettings.WSAImageType.SplashScreenImage, new PlayerSettings.WSAImageScale[] { PlayerSettings.WSAImageScale._100 }, "Scale {0}% ({1}x{2} pixels)|Sets the foreground image for the app's splash screen.", new int[] { 620, 0x307, 0x364, 930, 0x45c, 0x4d8, 0x9b0 }, new int[] { 300, 0x177, 420, 450, 540, 600, 0x4b0 });
            this.m_PhoneSplashScreens = CreateScaledImages(PlayerSettings.WSAImageType.PhoneSplashScreen, scaleArray2, "Scale {0}% ({1}x{2} pixels)|Sets the foreground image for the app's splash screen.", new int[] { 480, 0x2a0, 0x480 }, new int[] { 800, 0x460, 0x780 });
            this.m_HolographicSplashScreen = settingsEditor.FindPropertyAssert("m_VirtualRealitySplashScreen");
            this.m_HolographicPauseOnTrackingLoss = settingsEditor.FindPropertyAssert("m_HolographicPauseOnTrackingLoss");
            this.m_HolographicTrackingLossScreen = settingsEditor.FindPropertyAssert("m_HolographicTrackingLossScreen");
        }

        public override void PublishSectionGUI(float h, float kLabelMinWidth, float kLabelMaxWidth)
        {
            GUIContent content;
            float minHeight = h;
            float maxHeight = h;
            GUILayout.Label(EditorGUIUtility.TextContent("Packaging"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_MetroPackageName, EditorGUIUtility.TextContent("Package name|Specifies the unique name that identifies the package on the system."), new GUILayoutOption[0]);
            this.m_MetroPackageName.stringValue = Utility.TryValidatePackageName(this.m_MetroPackageName.stringValue);
            EditorGUILayout.LabelField(EditorGUIUtility.TextContent("Package display name|Specifies the app name that appears in the Store. Same as Product Name."), new GUIContent(this.m_ProductName.stringValue), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_MetroPackageVersion, EditorGUIUtility.TextContent("Version|The version number of the package. A version string in quad notation \"Major.Minor.Build.Revision\"."), new GUILayoutOption[0]);
            this.m_MetroPackageVersion.stringValue = PlayerSettings.WSA.ValidatePackageVersion(this.m_MetroPackageVersion.stringValue);
            EditorGUILayout.LabelField(EditorGUIUtility.TextContent("Publisher display name|A friendly name for the publisher that can be displayed to users. Same as Company Name."), new GUIContent(this.m_CompanyName.stringValue), new GUILayoutOption[0]);
            EditorGUILayout.Space();
            GUILayout.Label(EditorGUIUtility.TextContent("Certificate"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.LabelField(EditorGUIUtility.TextContent("Publisher"), new GUIContent(PlayerSettings.WSA.certificateSubject), new GUILayoutOption[0]);
            EditorGUILayout.LabelField(EditorGUIUtility.TextContent("Issued by"), new GUIContent(PlayerSettings.WSA.certificateIssuer), new GUILayoutOption[0]);
            EditorGUILayout.LabelField(EditorGUIUtility.TextContent("Expiration date"), new GUIContent(!PlayerSettings.WSA.certificateNotAfter.HasValue ? null : PlayerSettings.WSA.certificateNotAfter.Value.ToShortDateString()), new GUILayoutOption[0]);
            Rect rect = GUILayoutUtility.GetRect(kLabelMinWidth, kLabelMaxWidth, minHeight, maxHeight, EditorStyles.layerMaskField);
            Rect position = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, rect.width - EditorGUIUtility.labelWidth, rect.height);
            string certificatePath = PlayerSettings.WSA.certificatePath;
            if (string.IsNullOrEmpty(certificatePath))
            {
                content = EditorGUIUtility.TextContent("Select...|Browse for certificate.");
            }
            else
            {
                content = new GUIContent(FileUtil.GetLastPathNameComponent(certificatePath), certificatePath);
            }
            if (GUI.Button(position, content))
            {
                certificatePath = EditorUtility.OpenFilePanel(null, Application.dataPath, "pfx").Replace('\\', '/');
                string projectRelativePath = FileUtil.GetProjectRelativePath(certificatePath);
                if (string.IsNullOrEmpty(projectRelativePath) && !string.IsNullOrEmpty(certificatePath))
                {
                    Debug.LogError("Certificate path '" + Path.GetFullPath(certificatePath) + "' has to be relative to " + Path.GetFullPath(Application.dataPath + @"\.."));
                }
                else
                {
                    try
                    {
                        if (!PlayerSettings.WSA.SetCertificate(projectRelativePath, null))
                        {
                            MetroCertificatePasswordWindow.Show(projectRelativePath);
                        }
                    }
                    catch (UnityException exception)
                    {
                        Debug.LogError(exception.Message);
                    }
                }
            }
            rect = GUILayoutUtility.GetRect(kLabelMinWidth, kLabelMaxWidth, minHeight, maxHeight, EditorStyles.layerMaskField);
            position = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, rect.width - EditorGUIUtility.labelWidth, rect.height);
            if (GUI.Button(position, EditorGUIUtility.TextContent("Create...|Create test certificate.")))
            {
                MetroCreateTestCertificateWindow.Show(this.m_CompanyName.stringValue);
            }
            EditorGUILayout.Space();
            GUILayout.Label(EditorGUIUtility.TextContent("Application UI"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.LabelField(EditorGUIUtility.TextContent("Display name|Specifies the full name of the app."), new GUIContent(this.m_ProductName.stringValue), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_MetroApplicationDescription, EditorGUIUtility.TextContent("Description|Specifies the text that describes the app on its tile in Windows."), new GUILayoutOption[0]);
            this.m_MetroApplicationDescription.stringValue = this.ValidateMetroApplicationDescription(this.m_MetroApplicationDescription.stringValue);
            EditorGUILayout.Space();
            GUILayout.Label("File Type Associations", EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("Name:", new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(150f) };
            this.m_MetroFTAName.stringValue = GUILayout.TextField(this.m_MetroFTAName.stringValue, options);
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            GUILayout.Label("File Types", EditorStyles.boldLabel, new GUILayoutOption[0]);
            bool flag = !string.IsNullOrEmpty(this.m_MetroFTAName.stringValue);
            if (flag)
            {
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MinHeight(100f) };
                this.metroFTAFileTypesScrollViewPosition = GUILayout.BeginScrollView(this.metroFTAFileTypesScrollViewPosition, EditorStyles.helpBox, optionArray2);
                int index = -1;
                int num4 = 0;
                IEnumerator enumerator = this.m_MetroFTAFileTypes.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        SerializedProperty current = (SerializedProperty) enumerator.Current;
                        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                        GUILayout.BeginVertical(new GUILayoutOption[0]);
                        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                        GUILayout.Label("Content Type:", new GUILayoutOption[0]);
                        SerializedProperty property2 = current.FindPropertyRelative("contentType");
                        if (property2 != null)
                        {
                            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.MaxWidth(150f) };
                            property2.stringValue = GUILayout.TextField(property2.stringValue, optionArray3);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                        GUILayout.Label("File Type:", new GUILayoutOption[0]);
                        SerializedProperty property3 = current.FindPropertyRelative("fileType");
                        if (property3 != null)
                        {
                            GUILayoutOption[] optionArray4 = new GUILayoutOption[] { GUILayout.MaxWidth(150f) };
                            property3.stringValue = GUILayout.TextField(property3.stringValue, optionArray4).ToLower();
                        }
                        GUILayout.EndHorizontal();
                        GUILayoutOption[] optionArray5 = new GUILayoutOption[] { GUILayout.MaxWidth(150f) };
                        if (GUILayout.Button("Remove", optionArray5))
                        {
                            index = num4;
                        }
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                        num4++;
                        if (num4 < this.m_MetroFTAFileTypes.arraySize)
                        {
                            GUILayoutOption[] optionArray6 = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1f) };
                            GUILayout.Box(GUIContent.none, EditorStyles.helpBox, optionArray6);
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
                if ((index >= 0) && (index < this.m_MetroFTAFileTypes.arraySize))
                {
                    this.m_MetroFTAFileTypes.DeleteArrayElementAtIndex(index);
                }
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("Please specify Name first.", new GUILayoutOption[0]);
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            using (new EditorGUI.DisabledScope(!flag))
            {
                if (GUILayout.Button("Add New", new GUILayoutOption[0]))
                {
                    this.m_MetroFTAFileTypes.InsertArrayElementAtIndex(this.m_MetroFTAFileTypes.arraySize);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            GUILayout.Label("Protocol", EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("Name:", new GUILayoutOption[0]);
            GUILayoutOption[] optionArray7 = new GUILayoutOption[] { GUILayout.MaxWidth(150f) };
            this.m_MetroProtocolName.stringValue = GUILayout.TextField(this.m_MetroProtocolName.stringValue, optionArray7).ToLower();
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            GUILayout.Label("Compilation", EditorStyles.boldLabel, new GUILayoutOption[0]);
            PlayerSettings.WSA.compilationOverrides = (PlayerSettings.WSACompilationOverrides) EditorGUILayout.EnumPopup(EditorGUIUtility.TextContent("Compilation Overrides"), PlayerSettings.WSA.compilationOverrides, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            GUILayout.Label("Misc", EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            PlayerSettings.WSA.inputSource = (PlayerSettings.WSAInputSource) EditorGUILayout.EnumPopup(EditorGUIUtility.TextContent("Input Source"), PlayerSettings.WSA.inputSource, new GUILayoutOption[0]);
            GUILayout.Label("Capabilities", EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUILayoutOption[] optionArray8 = new GUILayoutOption[] { GUILayout.MinHeight(200f) };
            this.capScrollViewPosition = GUILayout.BeginScrollView(this.capScrollViewPosition, EditorStyles.helpBox, optionArray8);
            IEnumerator enumerator2 = Enum.GetValues(typeof(PlayerSettings.WSACapability)).GetEnumerator();
            try
            {
                while (enumerator2.MoveNext())
                {
                    PlayerSettings.WSACapability capability = (PlayerSettings.WSACapability) enumerator2.Current;
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    bool introduced33 = PlayerSettings.WSA.GetCapability(capability);
                    GUILayoutOption[] optionArray9 = new GUILayoutOption[] { GUILayout.MinWidth(150f) };
                    bool flag2 = GUILayout.Toggle(introduced33, capability.ToString(), optionArray9);
                    PlayerSettings.WSA.SetCapability(capability, flag2);
                    GUILayout.EndHorizontal();
                }
            }
            finally
            {
                IDisposable disposable2 = enumerator2 as IDisposable;
                if (disposable2 != null)
                {
                    disposable2.Dispose();
                }
            }
            GUILayout.EndScrollView();
        }

        public override void ResolutionSectionGUI(float h, float midWidth, float maxWidth)
        {
            EditorGUILayout.PropertyField(this.m_DefaultIsFullScreen, this.m_GUIDefaultIsFullScreen, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_RunInBackground, EditorGUIUtility.TextContent("Run In Background*"), new GUILayoutOption[0]);
        }

        private bool ScaledImageGroup(string groupLabel, MetroScaledImageGroup images)
        {
            string text = EditorGUIUtility.TextContent(groupLabel).text;
            if (!images.unfolded)
            {
                string str2 = " (";
                foreach (MetroScaledImage image in images.images)
                {
                    if (!string.IsNullOrEmpty(PlayerSettings.WSA.GetVisualAssetsImage(image.type, image.scale)))
                    {
                        str2 = str2 + ((int) image.scale) + ",";
                    }
                }
                if (str2 != " (")
                {
                    str2 = str2.Substring(0, str2.Length - 1) + ")";
                    text = text + str2;
                }
            }
            bool flag = EditorGUILayout.Foldout(images.unfolded, text);
            if (flag)
            {
                foreach (MetroScaledImage image2 in images.images)
                {
                    this.ImageField(image2, EditorGUIUtility.TextContent(image2.label));
                }
            }
            return flag;
        }

        public override void SplashSectionGUI()
        {
            float kLabelMinHeight = 16f;
            float kLabelMinWidth = (80f + EditorGUIUtility.fieldWidth) + 5f;
            this.MetroSplashScreenSection(kLabelMinWidth, kLabelMinWidth, kLabelMinHeight, kLabelMinHeight);
        }

        public override bool SupportsOrientation() => 
            true;

        private static bool ValidateImage(string imageFile, int width, int height)
        {
            string str = Path.GetExtension(imageFile).ToLowerInvariant();
            if ((str == null) || (((str != ".jpeg") && (str != ".jpg")) && (str != ".png")))
            {
                Debug.Log("Only JPEG and PNG files are supported");
                return false;
            }
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(File.ReadAllBytes(imageFile));
            int num = tex.width;
            int num2 = tex.height;
            UnityEngine.Object.DestroyImmediate(tex);
            if ((num != width) || (num2 != height))
            {
                Debug.LogError($"Invalid image size ({num}x{num2}), should be {width}x{height}");
                return false;
            }
            return true;
        }

        private string ValidateMetroApplicationDescription(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return this.m_ProductName.stringValue;
            }
            return value;
        }

        private string ValidateMetroTileShortName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = this.m_ProductName.stringValue;
            }
            if ((value != null) && (value.Length > 40))
            {
                char[] trimChars = new char[] { ' ' };
                return value.Substring(0, 40).TrimEnd(trimChars);
            }
            return value;
        }

        private class MetroScaledImage
        {
            public int height;
            public Texture2D image;
            public string imagePath;
            public string label;
            public PlayerSettings.WSAImageScale scale;
            public PlayerSettings.WSAImageType type;
            public int width;

            public MetroScaledImage(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale, string label, int width, int height)
            {
                this.type = type;
                this.scale = scale;
                this.label = label;
                this.width = width;
                this.height = height;
                this.imagePath = null;
                this.image = null;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MetroScaledImageGroup
        {
            public MetroSettingsEditorExtension.MetroScaledImage[] images;
            public bool unfolded;
            public MetroScaledImageGroup(MetroSettingsEditorExtension.MetroScaledImage[] images)
            {
                this.images = images;
                this.unfolded = false;
            }
        }
    }
}

