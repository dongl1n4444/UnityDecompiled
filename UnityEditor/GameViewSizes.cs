namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.VR;

    [FilePath("GameViewSizes.asset", FilePathAttribute.Location.PreferencesFolder)]
    internal class GameViewSizes : ScriptableSingleton<GameViewSizes>
    {
        [CompilerGenerated]
        private static Action <>f__am$cache0;
        [SerializeField]
        private GameViewSizeGroup m_Android = new GameViewSizeGroup();
        [NonSerialized]
        private int m_ChangeID = 0;
        [SerializeField]
        private GameViewSizeGroup m_HMD = new GameViewSizeGroup();
        [SerializeField]
        private GameViewSizeGroup m_iOS = new GameViewSizeGroup();
        [NonSerialized]
        private Vector2 m_LastRemoteScreenSize = new Vector2(-1f, -1f);
        [NonSerialized]
        private Vector2 m_LastStandaloneScreenSize = new Vector2(-1f, -1f);
        [SerializeField]
        private GameViewSizeGroup m_N3DS = new GameViewSizeGroup();
        [NonSerialized]
        private GameViewSize m_Remote = null;
        [SerializeField]
        private GameViewSizeGroup m_Standalone = new GameViewSizeGroup();
        [SerializeField]
        private GameViewSizeGroup m_Tizen = new GameViewSizeGroup();
        [SerializeField]
        private GameViewSizeGroup m_WiiU = new GameViewSizeGroup();
        [NonSerialized]
        private static GameViewSizeGroupType s_GameViewSizeGroupType;

        public static GameViewSizeGroupType BuildTargetGroupToGameViewSizeGroup(BuildTargetGroup buildTargetGroup)
        {
            if (VRSettings.enabled && VRSettings.showDeviceView)
            {
                return GameViewSizeGroupType.HMD;
            }
            switch (buildTargetGroup)
            {
                case BuildTargetGroup.Standalone:
                    return GameViewSizeGroupType.Standalone;

                case BuildTargetGroup.iPhone:
                    return GameViewSizeGroupType.iOS;

                case BuildTargetGroup.N3DS:
                    return GameViewSizeGroupType.N3DS;
            }
            if (buildTargetGroup != BuildTargetGroup.WiiU)
            {
                if (buildTargetGroup == BuildTargetGroup.Android)
                {
                    return GameViewSizeGroupType.Android;
                }
                if (buildTargetGroup == BuildTargetGroup.Tizen)
                {
                    return GameViewSizeGroupType.Tizen;
                }
            }
            else
            {
                return GameViewSizeGroupType.WiiU;
            }
            return GameViewSizeGroupType.Standalone;
        }

        public void Changed()
        {
            this.m_ChangeID++;
        }

        internal static bool DefaultLowResolutionSettingForSizeGroupType(GameViewSizeGroupType sizeGroupType)
        {
            switch (sizeGroupType)
            {
                case GameViewSizeGroupType.Standalone:
                case GameViewSizeGroupType.WiiU:
                case GameViewSizeGroupType.N3DS:
                    return true;

                case GameViewSizeGroupType.iOS:
                case GameViewSizeGroupType.Android:
                case GameViewSizeGroupType.Tizen:
                    return false;
            }
            return false;
        }

        public int GetChangeID() => 
            this.m_ChangeID;

        public static Rect GetConstrainedRect(Rect startRect, GameViewSizeGroupType groupType, int gameViewSizeIndex, out bool fitsInsideRect)
        {
            bool flag;
            fitsInsideRect = true;
            Rect rect = startRect;
            GameViewSize gameViewSize = ScriptableSingleton<GameViewSizes>.instance.GetGroup(groupType).GetGameViewSize(gameViewSizeIndex);
            RefreshDerivedGameViewSize(groupType, gameViewSizeIndex, gameViewSize);
            if (gameViewSize.isFreeAspectRatio)
            {
                return startRect;
            }
            float aspectRatio = 0f;
            GameViewSizeType sizeType = gameViewSize.sizeType;
            if (sizeType != GameViewSizeType.AspectRatio)
            {
                if (sizeType != GameViewSizeType.FixedResolution)
                {
                    throw new ArgumentException("Unrecognized size type");
                }
            }
            else
            {
                aspectRatio = gameViewSize.aspectRatio;
                flag = true;
                goto Label_00D3;
            }
            if ((gameViewSize.height > startRect.height) || (gameViewSize.width > startRect.width))
            {
                aspectRatio = gameViewSize.aspectRatio;
                flag = true;
                fitsInsideRect = false;
            }
            else
            {
                rect.height = gameViewSize.height;
                rect.width = gameViewSize.width;
                flag = false;
            }
        Label_00D3:
            if (flag)
            {
                rect.height = ((rect.width / aspectRatio) <= startRect.height) ? (rect.width / aspectRatio) : startRect.height;
                rect.width = rect.height * aspectRatio;
            }
            rect.height = Mathf.Clamp(rect.height, 0f, startRect.height);
            rect.width = Mathf.Clamp(rect.width, 0f, startRect.width);
            rect.y = ((startRect.height * 0.5f) - (rect.height * 0.5f)) + startRect.y;
            rect.x = ((startRect.width * 0.5f) - (rect.width * 0.5f)) + startRect.x;
            rect.width = Mathf.Floor(rect.width + 0.5f);
            rect.height = Mathf.Floor(rect.height + 0.5f);
            rect.x = Mathf.Floor(rect.x + 0.5f);
            rect.y = Mathf.Floor(rect.y + 0.5f);
            return rect;
        }

        public int GetDefaultStandaloneIndex() => 
            (this.m_Standalone.GetBuiltinCount() - 1);

        public GameViewSizeGroup GetGroup(GameViewSizeGroupType gameViewSizeGroupType)
        {
            this.InitBuiltinGroups();
            switch (gameViewSizeGroupType)
            {
                case GameViewSizeGroupType.Standalone:
                case GameViewSizeGroupType.WebPlayer:
                case GameViewSizeGroupType.PS3:
                case GameViewSizeGroupType.WP8:
                    return this.m_Standalone;

                case GameViewSizeGroupType.iOS:
                    return this.m_iOS;

                case GameViewSizeGroupType.Android:
                    return this.m_Android;

                case GameViewSizeGroupType.WiiU:
                    return this.m_WiiU;

                case GameViewSizeGroupType.Tizen:
                    return this.m_Tizen;

                case GameViewSizeGroupType.N3DS:
                    return this.m_N3DS;

                case GameViewSizeGroupType.HMD:
                    return this.m_HMD;
            }
            Debug.LogError("Unhandled group enum! " + gameViewSizeGroupType);
            return this.m_Standalone;
        }

        public static Vector2 GetRenderTargetSize(Rect startRect, GameViewSizeGroupType groupType, int gameViewSizeIndex, out bool clamped)
        {
            Vector2 size;
            float num2;
            GameViewSize gameViewSize = ScriptableSingleton<GameViewSizes>.instance.GetGroup(groupType).GetGameViewSize(gameViewSizeIndex);
            RefreshDerivedGameViewSize(groupType, gameViewSizeIndex, gameViewSize);
            clamped = false;
            if (gameViewSize.isFreeAspectRatio)
            {
                size = startRect.size;
            }
            else
            {
                GameViewSizeType sizeType = gameViewSize.sizeType;
                if (sizeType != GameViewSizeType.AspectRatio)
                {
                    if (sizeType != GameViewSizeType.FixedResolution)
                    {
                        throw new ArgumentException("Unrecognized size type");
                    }
                }
                else
                {
                    if ((startRect.height == 0f) || (gameViewSize.aspectRatio == 0f))
                    {
                        size = Vector2.zero;
                    }
                    else
                    {
                        float num = startRect.width / startRect.height;
                        if (num < gameViewSize.aspectRatio)
                        {
                            size = new Vector2(startRect.width, startRect.width / gameViewSize.aspectRatio);
                        }
                        else
                        {
                            size = new Vector2(startRect.height * gameViewSize.aspectRatio, startRect.height);
                        }
                    }
                    goto Label_010B;
                }
                size = new Vector2((float) gameViewSize.width, (float) gameViewSize.height);
            }
        Label_010B:
            num2 = (((SystemInfo.graphicsMemorySize * 0.2f) / 12f) * 1024f) * 1024f;
            float num3 = size.x * size.y;
            if (num3 > num2)
            {
                float num4 = size.x / size.y;
                size.x = Mathf.Sqrt(num2 * num4);
                size.y = num4 * size.x;
                clamped = true;
            }
            float b = 8192f;
            float num6 = Mathf.Min((float) SystemInfo.maxRenderTextureSize, b);
            if ((size.x > num6) || (size.y > num6))
            {
                if (size.x > size.y)
                {
                    size = (Vector2) (size * (num6 / size.x));
                }
                else
                {
                    size = (Vector2) (size * (num6 / size.y));
                }
                clamped = true;
            }
            return size;
        }

        private void InitBuiltinGroups()
        {
            if (this.m_Standalone.GetBuiltinCount() <= 0)
            {
                this.m_Remote = new GameViewSize(GameViewSizeType.FixedResolution, 0, 0, "Remote (Not Connected)");
                GameViewSize size = new GameViewSize(GameViewSizeType.AspectRatio, 0, 0, "Free Aspect");
                GameViewSize size2 = new GameViewSize(GameViewSizeType.AspectRatio, 5, 4, "");
                GameViewSize size3 = new GameViewSize(GameViewSizeType.AspectRatio, 4, 3, "");
                GameViewSize size4 = new GameViewSize(GameViewSizeType.AspectRatio, 3, 2, "");
                GameViewSize size5 = new GameViewSize(GameViewSizeType.AspectRatio, 0x10, 10, "");
                GameViewSize size6 = new GameViewSize(GameViewSizeType.AspectRatio, 0x10, 9, "");
                GameViewSize size7 = new GameViewSize(GameViewSizeType.FixedResolution, 0, 0, "Standalone");
                GameViewSize size8 = new GameViewSize(GameViewSizeType.FixedResolution, 320, 480, "iPhone Tall");
                GameViewSize size9 = new GameViewSize(GameViewSizeType.FixedResolution, 480, 320, "iPhone Wide");
                GameViewSize size10 = new GameViewSize(GameViewSizeType.FixedResolution, 640, 960, "iPhone 4 Tall");
                GameViewSize size11 = new GameViewSize(GameViewSizeType.FixedResolution, 960, 640, "iPhone 4 Wide");
                GameViewSize size12 = new GameViewSize(GameViewSizeType.FixedResolution, 0x300, 0x400, "iPad Tall");
                GameViewSize size13 = new GameViewSize(GameViewSizeType.FixedResolution, 0x400, 0x300, "iPad Wide");
                GameViewSize size14 = new GameViewSize(GameViewSizeType.AspectRatio, 9, 0x10, "iPhone 5 Tall");
                GameViewSize size15 = new GameViewSize(GameViewSizeType.AspectRatio, 0x10, 9, "iPhone 5 Wide");
                GameViewSize size16 = new GameViewSize(GameViewSizeType.AspectRatio, 2, 3, "iPhone Tall");
                GameViewSize size17 = new GameViewSize(GameViewSizeType.AspectRatio, 3, 2, "iPhone Wide");
                GameViewSize size18 = new GameViewSize(GameViewSizeType.AspectRatio, 3, 4, "iPad Tall");
                GameViewSize size19 = new GameViewSize(GameViewSizeType.AspectRatio, 4, 3, "iPad Wide");
                GameViewSize size20 = new GameViewSize(GameViewSizeType.FixedResolution, 320, 480, "HVGA Portrait");
                GameViewSize size21 = new GameViewSize(GameViewSizeType.FixedResolution, 480, 320, "HVGA Landscape");
                GameViewSize size22 = new GameViewSize(GameViewSizeType.FixedResolution, 480, 800, "WVGA Portrait");
                GameViewSize size23 = new GameViewSize(GameViewSizeType.FixedResolution, 800, 480, "WVGA Landscape");
                GameViewSize size24 = new GameViewSize(GameViewSizeType.FixedResolution, 480, 0x356, "FWVGA Portrait");
                GameViewSize size25 = new GameViewSize(GameViewSizeType.FixedResolution, 0x356, 480, "FWVGA Landscape");
                GameViewSize size26 = new GameViewSize(GameViewSizeType.FixedResolution, 600, 0x400, "WSVGA Portrait");
                GameViewSize size27 = new GameViewSize(GameViewSizeType.FixedResolution, 0x400, 600, "WSVGA Landscape");
                GameViewSize size28 = new GameViewSize(GameViewSizeType.FixedResolution, 800, 0x500, "WXGA Portrait");
                GameViewSize size29 = new GameViewSize(GameViewSizeType.FixedResolution, 0x500, 800, "WXGA Landscape");
                GameViewSize size30 = new GameViewSize(GameViewSizeType.AspectRatio, 2, 3, "3:2 Portrait");
                GameViewSize size31 = new GameViewSize(GameViewSizeType.AspectRatio, 3, 2, "3:2 Landscape");
                GameViewSize size32 = new GameViewSize(GameViewSizeType.AspectRatio, 10, 0x10, "16:10 Portrait");
                GameViewSize size33 = new GameViewSize(GameViewSizeType.AspectRatio, 0x10, 10, "16:10 Landscape");
                GameViewSize size34 = new GameViewSize(GameViewSizeType.FixedResolution, 0x780, 0x438, "1080p (16:9)");
                GameViewSize size35 = new GameViewSize(GameViewSizeType.FixedResolution, 0x500, 720, "720p (16:9)");
                GameViewSize size36 = new GameViewSize(GameViewSizeType.FixedResolution, 0x356, 480, "GamePad 480p (16:9)");
                GameViewSize size37 = new GameViewSize(GameViewSizeType.FixedResolution, 0x500, 720, "16:9 Landscape");
                GameViewSize size38 = new GameViewSize(GameViewSizeType.FixedResolution, 720, 0x500, "9:16 Portrait");
                GameViewSize size39 = new GameViewSize(GameViewSizeType.FixedResolution, 400, 240, "Top Screen");
                GameViewSize size40 = new GameViewSize(GameViewSizeType.FixedResolution, 320, 240, "Bottom Screen");
                GameViewSize[] sizes = new GameViewSize[] { size, size2, size3, size4, size5, size6, size7 };
                this.m_Standalone.AddBuiltinSizes(sizes);
                GameViewSize[] sizeArray2 = new GameViewSize[] { size, size3, size6, size34, size35, size36 };
                this.m_WiiU.AddBuiltinSizes(sizeArray2);
                GameViewSize[] sizeArray3 = new GameViewSize[] { size, size8, size9, size10, size11, size12, size13, size14, size15, size16, size17, size18, size19 };
                this.m_iOS.AddBuiltinSizes(sizeArray3);
                GameViewSize[] sizeArray4 = new GameViewSize[] { size, this.m_Remote, size20, size21, size22, size23, size24, size25, size26, size27, size28, size29, size30, size31, size32, size33 };
                this.m_Android.AddBuiltinSizes(sizeArray4);
                GameViewSize[] sizeArray5 = new GameViewSize[] { size, size37, size38 };
                this.m_Tizen.AddBuiltinSizes(sizeArray5);
                GameViewSize[] sizeArray6 = new GameViewSize[] { size, size39, size40 };
                this.m_N3DS.AddBuiltinSizes(sizeArray6);
                GameViewSize[] sizeArray7 = new GameViewSize[] { size, this.m_Remote };
                this.m_HMD.AddBuiltinSizes(sizeArray7);
            }
        }

        public bool IsDefaultStandaloneScreenSize(GameViewSizeGroupType gameViewSizeGroupType, int index) => 
            ((gameViewSizeGroupType == GameViewSizeGroupType.Standalone) && (this.GetDefaultStandaloneIndex() == index));

        public bool IsRemoteScreenSize(GameViewSizeGroupType gameViewSizeGroupType, int index) => 
            (this.GetGroup(gameViewSizeGroupType).IndexOf(this.m_Remote) == index);

        private void OnEnable()
        {
            RefreshGameViewSizeGroupType();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = () => RefreshGameViewSizeGroupType();
            }
            EditorUserBuildSettings.activeBuildTargetChanged = (Action) Delegate.Combine(EditorUserBuildSettings.activeBuildTargetChanged, <>f__am$cache0);
        }

        private static void RefreshDerivedGameViewSize(GameViewSizeGroupType groupType, int gameViewSizeIndex, GameViewSize gameViewSize)
        {
            if (ScriptableSingleton<GameViewSizes>.instance.IsDefaultStandaloneScreenSize(groupType, gameViewSizeIndex))
            {
                gameViewSize.width = (int) InternalEditorUtility.defaultScreenWidth;
                gameViewSize.height = (int) InternalEditorUtility.defaultScreenHeight;
            }
            else if (ScriptableSingleton<GameViewSizes>.instance.IsRemoteScreenSize(groupType, gameViewSizeIndex))
            {
                int eyeTextureWidth = 0;
                int eyeTextureHeight = 0;
                if (VRSettings.isDeviceActive)
                {
                    eyeTextureWidth = VRSettings.eyeTextureWidth;
                    eyeTextureHeight = VRSettings.eyeTextureHeight;
                }
                else
                {
                    eyeTextureWidth = (int) InternalEditorUtility.remoteScreenWidth;
                    eyeTextureHeight = (int) InternalEditorUtility.remoteScreenHeight;
                }
                if ((eyeTextureWidth > 0) && (eyeTextureHeight > 0))
                {
                    gameViewSize.sizeType = GameViewSizeType.FixedResolution;
                    gameViewSize.width = eyeTextureWidth;
                    gameViewSize.height = eyeTextureHeight;
                }
                else
                {
                    gameViewSize.sizeType = GameViewSizeType.AspectRatio;
                    int num3 = 0;
                    gameViewSize.height = num3;
                    gameViewSize.width = num3;
                }
            }
        }

        private static void RefreshGameViewSizeGroupType()
        {
            s_GameViewSizeGroupType = BuildTargetGroupToGameViewSizeGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));
        }

        public void RefreshRemoteScreenSize(int width, int height)
        {
            this.m_Remote.width = width;
            this.m_Remote.height = height;
            if ((width > 0) && (height > 0))
            {
                this.m_Remote.baseText = "Remote";
            }
            else
            {
                this.m_Remote.baseText = "Remote (Not Connected)";
            }
            this.Changed();
        }

        public void RefreshStandaloneAndRemoteDefaultSizes()
        {
            if ((InternalEditorUtility.defaultScreenWidth != this.m_LastStandaloneScreenSize.x) || (InternalEditorUtility.defaultScreenHeight != this.m_LastStandaloneScreenSize.y))
            {
                this.m_LastStandaloneScreenSize = new Vector2(InternalEditorUtility.defaultScreenWidth, InternalEditorUtility.defaultScreenHeight);
                this.RefreshStandaloneDefaultScreenSize((int) this.m_LastStandaloneScreenSize.x, (int) this.m_LastStandaloneScreenSize.y);
            }
            if ((InternalEditorUtility.remoteScreenWidth != this.m_LastRemoteScreenSize.x) || (InternalEditorUtility.remoteScreenHeight != this.m_LastRemoteScreenSize.y))
            {
                this.m_LastRemoteScreenSize = new Vector2(InternalEditorUtility.remoteScreenWidth, InternalEditorUtility.remoteScreenHeight);
                this.RefreshRemoteScreenSize((int) this.m_LastRemoteScreenSize.x, (int) this.m_LastRemoteScreenSize.y);
            }
            if ((VRSettings.isDeviceActive && (this.m_Remote.width != VRSettings.eyeTextureWidth)) && (this.m_Remote.height != VRSettings.eyeTextureHeight))
            {
                this.RefreshRemoteScreenSize(VRSettings.eyeTextureWidth, VRSettings.eyeTextureHeight);
            }
        }

        public void RefreshStandaloneDefaultScreenSize(int width, int height)
        {
            GameViewSize gameViewSize = this.m_Standalone.GetGameViewSize(this.GetDefaultStandaloneIndex());
            gameViewSize.height = height;
            gameViewSize.width = width;
            this.Changed();
        }

        public void SaveToHDD()
        {
            bool saveAsText = true;
            this.Save(saveAsText);
        }

        public GameViewSizeGroup currentGroup =>
            this.GetGroup(s_GameViewSizeGroupType);

        public GameViewSizeGroupType currentGroupType =>
            s_GameViewSizeGroupType;
    }
}

