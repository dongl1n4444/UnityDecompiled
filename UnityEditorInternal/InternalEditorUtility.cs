namespace UnityEditorInternal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditor.Scripting;
    using UnityEditor.Utils;
    using UnityEngine;
    using UnityEngine.Internal;

    public sealed class InternalEditorUtility
    {
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map0;

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int AddScriptComponentUncheckedUndoable(GameObject gameObject, MonoScript script);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void AddSortingLayer();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void AddTag(string tag);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void AuxWindowManager_OnAssemblyReload();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern CanAppendBuild BuildCanBeAppended(BuildTarget target, string location);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void BumpMapSettingsFixingWindowReportResult(int result);
        internal static bool BumpMapTextureNeedsFixing(MaterialProperty prop)
        {
            if (prop.type == MaterialProperty.PropType.Texture)
            {
                bool flaggedAsNormal = (prop.flags & MaterialProperty.PropFlags.Normal) != MaterialProperty.PropFlags.None;
                foreach (Material material in prop.targets)
                {
                    if (BumpMapTextureNeedsFixingInternal(material, prop.name, flaggedAsNormal))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool BumpMapTextureNeedsFixingInternal(Material material, string propName, bool flaggedAsNormal);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void CalculateAmbientProbeFromSkybox();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string CalculateHashForObjectsAndDependencies(UnityEngine.Object[] objects);
        public static Bounds CalculateSelectionBounds(bool usePivotOnlyForParticles, bool onlyUseActiveSelection)
        {
            Bounds bounds;
            INTERNAL_CALL_CalculateSelectionBounds(usePivotOnlyForParticles, onlyUseActiveSelection, out bounds);
            return bounds;
        }

        internal static Bounds CalculateSelectionBoundsInSpace(Vector3 position, Quaternion rotation, bool rectBlueprintMode)
        {
            Quaternion quaternion = Quaternion.Inverse(rotation);
            Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3[] vectorArray = new Vector3[2];
            foreach (GameObject obj2 in Selection.gameObjects)
            {
                Bounds localBounds = GetLocalBounds(obj2);
                vectorArray[0] = localBounds.min;
                vectorArray[1] = localBounds.max;
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            Vector3 vector3 = new Vector3(vectorArray[i].x, vectorArray[j].y, vectorArray[k].z);
                            if (rectBlueprintMode && SupportsRectLayout(obj2.transform))
                            {
                                Vector3 localPosition = obj2.transform.localPosition;
                                localPosition.z = 0f;
                                vector3 = obj2.transform.parent.TransformPoint(vector3 + localPosition);
                            }
                            else
                            {
                                vector3 = obj2.transform.TransformPoint(vector3);
                            }
                            vector3 = (Vector3) (quaternion * (vector3 - position));
                            for (int m = 0; m < 3; m++)
                            {
                                vector[m] = Mathf.Min(vector[m], vector3[m]);
                                vector2[m] = Mathf.Max(vector2[m], vector3[m]);
                            }
                        }
                    }
                }
            }
            return new Bounds((Vector3) ((vector + vector2) * 0.5f), vector2 - vector);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool CanConnectToCacheServer();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void ClearSceneLighting();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int CreateScriptableObjectUnchecked(MonoScript script);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern DllType DetectDotNetDll(string path);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int DetermineDepthOrder(Transform lhs, Transform rhs);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void DrawSkyboxMaterial(Material mat, Camera cam);
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("use EditorSceneManager.EnsureUntitledSceneHasBeenSaved")]
        public static extern bool EnsureSceneHasBeenSaved(string operation);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void ExecuteCommandOnKeyWindow(string commandName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern ulong FixCacheServerIntegrityErrors();
        internal static void FixNormalmapTexture(MaterialProperty prop)
        {
            foreach (Material material in prop.targets)
            {
                FixNormalmapTextureInternal(material, prop.name);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void FixNormalmapTextureInternal(Material material, string propName);
        internal static IEnumerable<string> GetAllScriptGUIDs()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = asset => IsScriptOrAssembly(asset);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = asset => AssetDatabase.AssetPathToGUID(asset);
            }
            return Enumerable.Select<string, string>(Enumerable.Where<string>(AssetDatabase.GetAllAssetPaths(), <>f__am$cache0), <>f__am$cache1);
        }

        internal static string GetApplicationExtensionForRuntimePlatform(RuntimePlatform platform)
        {
            if (platform != RuntimePlatform.OSXEditor)
            {
                if (platform == RuntimePlatform.WindowsEditor)
                {
                    return "exe";
                }
            }
            else
            {
                return "app";
            }
            return string.Empty;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetAssetsFolder();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetAuthToken();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string[] GetAvailableDiffTools();
        public static Rect GetBoundsOfDesktopAtPoint(Vector2 pos)
        {
            Rect rect;
            INTERNAL_CALL_GetBoundsOfDesktopAtPoint(ref pos, out rect);
            return rect;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int GetClassIDWithoutLoadingObject(int instanceID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetCrashReportFolder();
        private static string GetDefaultStringEditorArgs()
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return "";
            }
            return "\"$(File)\"";
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Resolution GetDesktopResolution();
        public static string GetDisplayStringOfInvalidCharsOfFileName(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return "";
            }
            string str2 = new string(Path.GetInvalidFileNameChars());
            string str3 = "";
            foreach (char ch in filename)
            {
                if ((str2.IndexOf(ch) >= 0) && (str3.IndexOf(ch) == -1))
                {
                    if (str3.Length > 0)
                    {
                        str3 = str3 + " ";
                    }
                    str3 = str3 + ch;
                }
            }
            return str3;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetEditorAssemblyPath();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetEditorFolder();
        public static string[] GetEditorSettingsList(string prefix, int count)
        {
            ArrayList list = new ArrayList();
            for (int i = 1; i <= count; i++)
            {
                string str = EditorPrefs.GetString(prefix + i, "defaultValue");
                if (str == "defaultValue")
                {
                    break;
                }
                list.Add(str);
            }
            return (list.ToArray(typeof(string)) as string[]);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetEngineAssemblyPath();
        public static string GetExternalScriptEditor() => 
            EditorPrefs.GetString("kScriptsDefaultApp");

        public static string GetExternalScriptEditorArgs()
        {
            string externalScriptEditor = GetExternalScriptEditor();
            if (IsScriptEditorSpecial(externalScriptEditor))
            {
                return "";
            }
            return EditorPrefs.GetString(GetScriptEditorArgsKey(externalScriptEditor), GetDefaultStringEditorArgs());
        }

        private static bool GetFirstAndLastSelected(List<int> allInstanceIDs, List<int> selectedInstanceIDs, out int firstIndex, out int lastIndex)
        {
            firstIndex = -1;
            lastIndex = -1;
            for (int i = 0; i < allInstanceIDs.Count; i++)
            {
                if (selectedInstanceIDs.Contains(allInstanceIDs[i]))
                {
                    if (firstIndex == -1)
                    {
                        firstIndex = i;
                    }
                    lastIndex = i;
                }
            }
            return ((firstIndex != -1) && (lastIndex != -1));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetFullUnityVersion();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int GetGameObjectInstanceIDFromComponent(int instanceID);
        public static Texture2D GetIconForFile(string fileName)
        {
            int num = fileName.LastIndexOf('.');
            string key = (num != -1) ? fileName.Substring(num + 1).ToLower() : "";
            if (key != null)
            {
                int num2;
                if (<>f__switch$map0 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(0x81) {
                        { 
                            "boo",
                            0
                        },
                        { 
                            "cginc",
                            1
                        },
                        { 
                            "cs",
                            2
                        },
                        { 
                            "guiskin",
                            3
                        },
                        { 
                            "js",
                            4
                        },
                        { 
                            "mat",
                            5
                        },
                        { 
                            "physicmaterial",
                            6
                        },
                        { 
                            "prefab",
                            7
                        },
                        { 
                            "shader",
                            8
                        },
                        { 
                            "txt",
                            9
                        },
                        { 
                            "unity",
                            10
                        },
                        { 
                            "asset",
                            11
                        },
                        { 
                            "prefs",
                            11
                        },
                        { 
                            "anim",
                            12
                        },
                        { 
                            "meta",
                            13
                        },
                        { 
                            "mixer",
                            14
                        },
                        { 
                            "ttf",
                            15
                        },
                        { 
                            "otf",
                            15
                        },
                        { 
                            "fon",
                            15
                        },
                        { 
                            "fnt",
                            15
                        },
                        { 
                            "aac",
                            0x10
                        },
                        { 
                            "aif",
                            0x10
                        },
                        { 
                            "aiff",
                            0x10
                        },
                        { 
                            "au",
                            0x10
                        },
                        { 
                            "mid",
                            0x10
                        },
                        { 
                            "midi",
                            0x10
                        },
                        { 
                            "mp3",
                            0x10
                        },
                        { 
                            "mpa",
                            0x10
                        },
                        { 
                            "ra",
                            0x10
                        },
                        { 
                            "ram",
                            0x10
                        },
                        { 
                            "wma",
                            0x10
                        },
                        { 
                            "wav",
                            0x10
                        },
                        { 
                            "wave",
                            0x10
                        },
                        { 
                            "ogg",
                            0x10
                        },
                        { 
                            "ai",
                            0x11
                        },
                        { 
                            "apng",
                            0x11
                        },
                        { 
                            "png",
                            0x11
                        },
                        { 
                            "bmp",
                            0x11
                        },
                        { 
                            "cdr",
                            0x11
                        },
                        { 
                            "dib",
                            0x11
                        },
                        { 
                            "eps",
                            0x11
                        },
                        { 
                            "exif",
                            0x11
                        },
                        { 
                            "gif",
                            0x11
                        },
                        { 
                            "ico",
                            0x11
                        },
                        { 
                            "icon",
                            0x11
                        },
                        { 
                            "j",
                            0x11
                        },
                        { 
                            "j2c",
                            0x11
                        },
                        { 
                            "j2k",
                            0x11
                        },
                        { 
                            "jas",
                            0x11
                        },
                        { 
                            "jiff",
                            0x11
                        },
                        { 
                            "jng",
                            0x11
                        },
                        { 
                            "jp2",
                            0x11
                        },
                        { 
                            "jpc",
                            0x11
                        },
                        { 
                            "jpe",
                            0x11
                        },
                        { 
                            "jpeg",
                            0x11
                        },
                        { 
                            "jpf",
                            0x11
                        },
                        { 
                            "jpg",
                            0x11
                        },
                        { 
                            "jpw",
                            0x11
                        },
                        { 
                            "jpx",
                            0x11
                        },
                        { 
                            "jtf",
                            0x11
                        },
                        { 
                            "mac",
                            0x11
                        },
                        { 
                            "omf",
                            0x11
                        },
                        { 
                            "qif",
                            0x11
                        },
                        { 
                            "qti",
                            0x11
                        },
                        { 
                            "qtif",
                            0x11
                        },
                        { 
                            "tex",
                            0x11
                        },
                        { 
                            "tfw",
                            0x11
                        },
                        { 
                            "tga",
                            0x11
                        },
                        { 
                            "tif",
                            0x11
                        },
                        { 
                            "tiff",
                            0x11
                        },
                        { 
                            "wmf",
                            0x11
                        },
                        { 
                            "psd",
                            0x11
                        },
                        { 
                            "exr",
                            0x11
                        },
                        { 
                            "hdr",
                            0x11
                        },
                        { 
                            "3df",
                            0x12
                        },
                        { 
                            "3dm",
                            0x12
                        },
                        { 
                            "3dmf",
                            0x12
                        },
                        { 
                            "3ds",
                            0x12
                        },
                        { 
                            "3dv",
                            0x12
                        },
                        { 
                            "3dx",
                            0x12
                        },
                        { 
                            "blend",
                            0x12
                        },
                        { 
                            "c4d",
                            0x12
                        },
                        { 
                            "lwo",
                            0x12
                        },
                        { 
                            "lws",
                            0x12
                        },
                        { 
                            "ma",
                            0x12
                        },
                        { 
                            "max",
                            0x12
                        },
                        { 
                            "mb",
                            0x12
                        },
                        { 
                            "mesh",
                            0x12
                        },
                        { 
                            "obj",
                            0x12
                        },
                        { 
                            "vrl",
                            0x12
                        },
                        { 
                            "wrl",
                            0x12
                        },
                        { 
                            "wrz",
                            0x12
                        },
                        { 
                            "fbx",
                            0x12
                        },
                        { 
                            "asf",
                            0x13
                        },
                        { 
                            "asx",
                            0x13
                        },
                        { 
                            "avi",
                            0x13
                        },
                        { 
                            "dat",
                            0x13
                        },
                        { 
                            "divx",
                            0x13
                        },
                        { 
                            "dvx",
                            0x13
                        },
                        { 
                            "mlv",
                            0x13
                        },
                        { 
                            "m2l",
                            0x13
                        },
                        { 
                            "m2t",
                            0x13
                        },
                        { 
                            "m2ts",
                            0x13
                        },
                        { 
                            "m2v",
                            0x13
                        },
                        { 
                            "m4e",
                            0x13
                        },
                        { 
                            "m4v",
                            0x13
                        },
                        { 
                            "mjp",
                            0x13
                        },
                        { 
                            "mov",
                            0x13
                        },
                        { 
                            "movie",
                            0x13
                        },
                        { 
                            "mp21",
                            0x13
                        },
                        { 
                            "mp4",
                            0x13
                        },
                        { 
                            "mpe",
                            0x13
                        },
                        { 
                            "mpeg",
                            0x13
                        },
                        { 
                            "mpg",
                            0x13
                        },
                        { 
                            "mpv2",
                            0x13
                        },
                        { 
                            "ogm",
                            0x13
                        },
                        { 
                            "qt",
                            0x13
                        },
                        { 
                            "rm",
                            0x13
                        },
                        { 
                            "rmvb",
                            0x13
                        },
                        { 
                            "wmw",
                            0x13
                        },
                        { 
                            "xvid",
                            0x13
                        },
                        { 
                            "colors",
                            20
                        },
                        { 
                            "gradients",
                            20
                        },
                        { 
                            "curves",
                            20
                        },
                        { 
                            "curvesnormalized",
                            20
                        },
                        { 
                            "particlecurves",
                            20
                        },
                        { 
                            "particlecurvessigned",
                            20
                        },
                        { 
                            "particledoublecurves",
                            20
                        },
                        { 
                            "particledoublecurvessigned",
                            20
                        }
                    };
                    <>f__switch$map0 = dictionary;
                }
                if (<>f__switch$map0.TryGetValue(key, out num2))
                {
                    switch (num2)
                    {
                        case 0:
                            return EditorGUIUtility.FindTexture("boo Script Icon");

                        case 1:
                            return EditorGUIUtility.FindTexture("CGProgram Icon");

                        case 2:
                            return EditorGUIUtility.FindTexture("cs Script Icon");

                        case 3:
                            return EditorGUIUtility.FindTexture("GUISkin Icon");

                        case 4:
                            return EditorGUIUtility.FindTexture("Js Script Icon");

                        case 5:
                            return EditorGUIUtility.FindTexture("Material Icon");

                        case 6:
                            return EditorGUIUtility.FindTexture("PhysicMaterial Icon");

                        case 7:
                            return EditorGUIUtility.FindTexture("PrefabNormal Icon");

                        case 8:
                            return EditorGUIUtility.FindTexture("Shader Icon");

                        case 9:
                            return EditorGUIUtility.FindTexture("TextAsset Icon");

                        case 10:
                            return EditorGUIUtility.FindTexture("SceneAsset Icon");

                        case 11:
                            return EditorGUIUtility.FindTexture("GameManager Icon");

                        case 12:
                            return EditorGUIUtility.FindTexture("Animation Icon");

                        case 13:
                            return EditorGUIUtility.FindTexture("MetaFile Icon");

                        case 14:
                            return EditorGUIUtility.FindTexture("AudioMixerController Icon");

                        case 15:
                            return EditorGUIUtility.FindTexture("Font Icon");

                        case 0x10:
                            return EditorGUIUtility.FindTexture("AudioClip Icon");

                        case 0x11:
                            return EditorGUIUtility.FindTexture("Texture Icon");

                        case 0x12:
                            return EditorGUIUtility.FindTexture("Mesh Icon");

                        case 0x13:
                            return EditorGUIUtility.FindTexture("MovieTexture Icon");

                        case 20:
                            return EditorGUIUtility.FindTexture("ScriptableObject Icon");
                    }
                }
            }
            return EditorGUIUtility.FindTexture("DefaultAsset Icon");
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool GetIsInspectorExpanded(UnityEngine.Object obj);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetLayerName(int layer);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int[] GetLicenseFlags();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetLicenseInfo();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern UnityEngine.Object GetLoadedObjectFromInstanceID(int instanceID);
        private static Bounds GetLocalBounds(GameObject gameObject)
        {
            RectTransform component = gameObject.GetComponent<RectTransform>();
            if (component != null)
            {
                return new Bounds((Vector3) component.rect.center, (Vector3) component.rect.size);
            }
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer is MeshRenderer)
            {
                MeshFilter filter = renderer.GetComponent<MeshFilter>();
                if ((filter != null) && (filter.sharedMesh != null))
                {
                    return filter.sharedMesh.bounds;
                }
            }
            if (renderer is SpriteRenderer)
            {
                return ((SpriteRenderer) renderer).GetSpriteBounds();
            }
            return new Bounds(Vector3.zero, Vector3.zero);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern MonoIsland[] GetMonoIslands();
        public static List<int> GetNewSelection(int clickedInstanceID, List<int> allInstanceIDs, List<int> selectedInstanceIDs, int lastClickedInstanceID, bool keepMultiSelection, bool useShiftAsActionKey, bool allowMultiSelection)
        {
            List<int> list = new List<int>();
            bool flag = Event.current.shift || (EditorGUI.actionKey && useShiftAsActionKey);
            bool flag2 = EditorGUI.actionKey && !useShiftAsActionKey;
            if (!allowMultiSelection)
            {
                flag = flag2 = false;
            }
            if (flag2)
            {
                list.AddRange(selectedInstanceIDs);
                if (list.Contains(clickedInstanceID))
                {
                    list.Remove(clickedInstanceID);
                }
                else
                {
                    list.Add(clickedInstanceID);
                }
                return list;
            }
            if (flag)
            {
                int num;
                int num2;
                int num7;
                int num8;
                if (clickedInstanceID == lastClickedInstanceID)
                {
                    return selectedInstanceIDs;
                }
                if (!GetFirstAndLastSelected(allInstanceIDs, selectedInstanceIDs, out num, out num2))
                {
                    list.Add(clickedInstanceID);
                    return list;
                }
                int num3 = -1;
                int num4 = -1;
                for (int i = 0; i < allInstanceIDs.Count; i++)
                {
                    if (allInstanceIDs[i] == clickedInstanceID)
                    {
                        num3 = i;
                    }
                    if ((lastClickedInstanceID != 0) && (allInstanceIDs[i] == lastClickedInstanceID))
                    {
                        num4 = i;
                    }
                }
                int num6 = 0;
                if (num4 != -1)
                {
                    num6 = (num3 <= num4) ? -1 : 1;
                }
                if (num3 > num2)
                {
                    num7 = num;
                    num8 = num3;
                }
                else if ((num3 >= num) && (num3 < num2))
                {
                    if (num6 > 0)
                    {
                        num7 = num3;
                        num8 = num2;
                    }
                    else
                    {
                        num7 = num;
                        num8 = num3;
                    }
                }
                else
                {
                    num7 = num3;
                    num8 = num2;
                }
                for (int j = num7; j <= num8; j++)
                {
                    list.Add(allInstanceIDs[j]);
                }
                return list;
            }
            if (keepMultiSelection && selectedInstanceIDs.Contains(clickedInstanceID))
            {
                list.AddRange(selectedInstanceIDs);
                return list;
            }
            list.Add(clickedInstanceID);
            return list;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetNoDiffToolsDetectedMessage();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern UnityEngine.Object GetObjectFromInstanceID(int instanceID);
        public static Camera[] GetSceneViewCameras() => 
            SceneView.GetAllSceneCameras();

        private static string GetScriptEditorArgsKey(string path)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return ("kScriptEditorArgs_" + path);
            }
            return ("kScriptEditorArgs" + path);
        }

        public static ScriptEditor GetScriptEditorFromPath(string path)
        {
            string str = path.ToLower();
            if (str == "internal")
            {
                return ScriptEditor.Internal;
            }
            if ((str.Contains("monodevelop") || str.Contains("xamarinstudio")) || str.Contains("xamarin studio"))
            {
                return ScriptEditor.MonoDevelop;
            }
            if (str.EndsWith("devenv.exe"))
            {
                return ScriptEditor.VisualStudio;
            }
            if (str.EndsWith("vcsexpress.exe"))
            {
                return ScriptEditor.VisualStudioExpress;
            }
            switch (Path.GetFileName(Paths.UnifyDirectorySeparator(str)).Replace(" ", ""))
            {
                case "code.exe":
                case "visualstudiocode.app":
                case "vscode.app":
                case "code.app":
                case "code":
                    return ScriptEditor.VisualStudioCode;
            }
            return ScriptEditor.Other;
        }

        public static ScriptEditor GetScriptEditorFromPreferences() => 
            GetScriptEditorFromPath(GetExternalScriptEditor());

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int GetSortingLayerCount();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool GetSortingLayerLocked(int index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string GetSortingLayerName(int index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string GetSortingLayerNameFromUniqueID(int id);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int GetSortingLayerUniqueID(int index);
        public static Vector4 GetSpriteOuterUV(Sprite sprite, bool getAtlasData)
        {
            Vector4 vector;
            INTERNAL_CALL_GetSpriteOuterUV(sprite, getAtlasData, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetUnityBuildBranch();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetUnityCopyright();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int GetUnityRevision();
        public static Version GetUnityVersion()
        {
            Version version = new Version(GetUnityVersionDigits());
            return new Version(version.Major, version.Minor, version.Build, GetUnityRevision());
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int GetUnityVersionDate();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetUnityVersionDigits();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetUnityVersionFull();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool HasAdvancedLicenseOnBuildTarget(BuildTarget target);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern bool HasEduLicense();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern bool HasFreeLicense();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool HasFullscreenCamera();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern bool HasPro();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern bool HasTeamLicense();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern DragAndDropVisualMode HierarchyWindowDrag(HierarchyProperty property, bool perform, HierarchyDropMode dropMode);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern DragAndDropVisualMode InspectorWindowDrag(UnityEngine.Object[] targets, bool perform);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Material[] InstantiateMaterialsInEditMode(Renderer renderer);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_CalculateSelectionBounds(bool usePivotOnlyForParticles, bool onlyUseActiveSelection, out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetBoundsOfDesktopAtPoint(ref Vector2 pos, out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetSpriteOuterUV(Sprite sprite, bool getAtlasData, out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_PassAndReturnColor32(ref Color32 c, out Color32 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_PassAndReturnVector2(ref Vector2 v, out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Color[] INTERNAL_CALL_ReadScreenPixel(ref Vector2 pixelPos, int sizex, int sizey);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_SaveCursorToFile(string path, Texture2D image, ref Vector2 hotSpot);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern DragAndDropVisualMode INTERNAL_CALL_SceneViewDrag(UnityEngine.Object dropUpon, ref Vector3 worldPosition, ref Vector2 viewportPosition, bool perform);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetCustomLighting(Light[] lights, ref Color ambient);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetRectTransformTemporaryRect(RectTransform rectTransform, ref Rect rect);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_TransformBounds(ref Bounds b, Transform t, out Bounds value);
        public static bool IsDotNet4Dll(string path)
        {
            DllType type = DetectDotNetDll(path);
            switch (type)
            {
                case DllType.Unknown:
                case DllType.Native:
                case DllType.UnknownManaged:
                case DllType.ManagedNET35:
                    return false;

                case DllType.ManagedNET40:
                case DllType.WinMDNative:
                case DllType.WinMDNET40:
                    return true;
            }
            throw new Exception($"Unknown dll type: {type}");
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool IsInEditorFolder(string path);
        public static bool IsScriptEditorSpecial(string path) => 
            (GetScriptEditorFromPath(path) != ScriptEditor.Other);

        internal static bool IsScriptOrAssembly(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                switch (Path.GetExtension(filename).ToLower())
                {
                    case ".cs":
                    case ".js":
                    case ".boo":
                        return true;

                    case ".dll":
                    case ".exe":
                        return AssemblyHelper.IsManagedAssembly(filename);
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool IsSortingLayerDefault(int index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool IsUnityBeta();
        public static bool IsValidFileName(string filename)
        {
            string str = RemoveInvalidCharsFromFileName(filename, false);
            if ((str != filename) || string.IsNullOrEmpty(str))
            {
                return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Assembly LoadAssemblyWrapper(string dllName, string dllLocation);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void LoadDefaultLayout();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern UnityEngine.Object[] LoadSerializedFileAndForget(string path);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void OnGameViewFocus(bool focus);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void OpenEditorConsole();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool OpenFileAtLineExternal(string filename, int line);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void OpenPlayerConsole();
        internal static T ParentHasComponent<T>(Transform trans) where T: Component
        {
            if (trans != null)
            {
                T component = trans.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
                return ParentHasComponent<T>(trans.parent);
            }
            return null;
        }

        public static Color32 PassAndReturnColor32(Color32 c)
        {
            Color32 color;
            INTERNAL_CALL_PassAndReturnColor32(ref c, out color);
            return color;
        }

        public static Vector2 PassAndReturnVector2(Vector2 v)
        {
            Vector2 vector;
            INTERNAL_CALL_PassAndReturnVector2(ref v, out vector);
            return vector;
        }

        internal static void PrepareDragAndDropTesting(EditorWindow editorWindow)
        {
            if (editorWindow.m_Parent != null)
            {
                PrepareDragAndDropTestingInternal(editorWindow.m_Parent);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void PrepareDragAndDropTestingInternal(GUIView guiView);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern DragAndDropVisualMode ProjectWindowDrag(HierarchyProperty property, bool perform);
        public static Color[] ReadScreenPixel(Vector2 pixelPos, int sizex, int sizey) => 
            INTERNAL_CALL_ReadScreenPixel(ref pixelPos, sizex, sizey);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void RegisterExtensionDll(string dllLocation, string guid);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void ReloadWindowLayoutMenu();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void RemoveCustomLighting();
        public static string RemoveInvalidCharsFromFileName(string filename, bool logIfInvalidChars)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return filename;
            }
            filename = filename.Trim();
            if (string.IsNullOrEmpty(filename))
            {
                return filename;
            }
            string str2 = new string(Path.GetInvalidFileNameChars());
            string str3 = "";
            bool flag = false;
            foreach (char ch in filename)
            {
                if (str2.IndexOf(ch) == -1)
                {
                    str3 = str3 + ch;
                }
                else
                {
                    flag = true;
                }
            }
            if (flag && logIfInvalidChars)
            {
                string displayStringOfInvalidCharsOfFileName = GetDisplayStringOfInvalidCharsOfFileName(filename);
                if (displayStringOfInvalidCharsOfFileName.Length > 0)
                {
                    object[] args = new object[] { (displayStringOfInvalidCharsOfFileName.Length <= 1) ? "" : "s", displayStringOfInvalidCharsOfFileName };
                    Debug.LogWarningFormat("A filename cannot contain the following character{0}:  {1}", args);
                }
            }
            return str3;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void RemoveTag(string tag);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void RepaintAllViews();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void RequestScriptReload();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void ResetCursor();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void RevertFactoryLayoutSettings(bool quitOnCancel);
        [ExcludeFromDocs]
        internal static bool RunningUnderWindows8()
        {
            bool orHigher = true;
            return RunningUnderWindows8(orHigher);
        }

        internal static bool RunningUnderWindows8([DefaultValue("true")] bool orHigher)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                OperatingSystem oSVersion = Environment.OSVersion;
                int major = oSVersion.Version.Major;
                int minor = oSVersion.Version.Minor;
                if (orHigher)
                {
                    return ((major > 6) || ((major == 6) && (minor >= 2)));
                }
                return ((major == 6) && (minor == 2));
            }
            return false;
        }

        public static bool SaveCursorToFile(string path, Texture2D image, Vector2 hotSpot) => 
            INTERNAL_CALL_SaveCursorToFile(path, image, ref hotSpot);

        public static void SaveEditorSettingsList(string prefix, string[] aList, int count)
        {
            int num;
            for (num = 0; num < aList.Length; num++)
            {
                EditorPrefs.SetString(prefix + (num + 1), aList[num]);
            }
            for (num = aList.Length + 1; num <= count; num++)
            {
                EditorPrefs.DeleteKey(prefix + num);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SaveToSerializedFileAndForget(UnityEngine.Object[] obj, string path, bool allowTextSerialization);
        public static DragAndDropVisualMode SceneViewDrag(UnityEngine.Object dropUpon, Vector3 worldPosition, Vector2 viewportPosition, bool perform) => 
            INTERNAL_CALL_SceneViewDrag(dropUpon, ref worldPosition, ref viewportPosition, perform);

        public static void SetCustomLighting(Light[] lights, Color ambient)
        {
            INTERNAL_CALL_SetCustomLighting(lights, ref ambient);
        }

        public static void SetExternalScriptEditor(string path)
        {
            EditorPrefs.SetString("kScriptsDefaultApp", path);
        }

        public static void SetExternalScriptEditorArgs(string args)
        {
            EditorPrefs.SetString(GetScriptEditorArgsKey(GetExternalScriptEditor()), args);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetIsInspectorExpanded(UnityEngine.Object obj, bool isExpanded);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetPlatformPath(string path);
        public static void SetRectTransformTemporaryRect(RectTransform rectTransform, Rect rect)
        {
            INTERNAL_CALL_SetRectTransformTemporaryRect(rectTransform, ref rect);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetSortingLayerLocked(int index, bool locked);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetSortingLayerName(int index, string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetupCustomDll(string dllName, string dllLocation);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetupShaderMenu(Material material);
        public static void ShowGameView()
        {
            WindowLayout.ShowAppropriateViewOnEnterExitPlaymode(true);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void ShowPackageManagerWindow();
        internal static bool SupportsRectLayout(Transform tr)
        {
            if ((tr == null) || (tr.parent == null))
            {
                return false;
            }
            if ((tr.GetComponent<RectTransform>() == null) || (tr.parent.GetComponent<RectTransform>() == null))
            {
                return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SwitchSkinAndRepaintAllViews();
        public static string TextAreaForDocBrowser(Rect position, string text, GUIStyle style)
        {
            bool flag;
            int id = GUIUtility.GetControlID("TextAreaWithTabHandling".GetHashCode(), FocusType.Keyboard, position);
            EditorGUI.RecycledTextEditor editor = EditorGUI.s_RecycledEditor;
            Event current = Event.current;
            if (editor.IsEditingControl(id) && (current.type == EventType.KeyDown))
            {
                if (current.character == '\t')
                {
                    editor.Insert('\t');
                    current.Use();
                    GUI.changed = true;
                    text = editor.text;
                }
                if (current.character == '\n')
                {
                    editor.Insert('\n');
                    current.Use();
                    GUI.changed = true;
                    text = editor.text;
                }
            }
            text = EditorGUI.DoTextField(editor, id, EditorGUI.IndentedRect(position), text, style, null, out flag, false, true, false);
            return text;
        }

        public static string TextifyEvent(Event evt)
        {
            if (evt == null)
            {
                return "none";
            }
            string str2 = null;
            KeyCode keyCode = evt.keyCode;
            switch (keyCode)
            {
                case KeyCode.Keypad0:
                    str2 = "[0]";
                    break;

                case KeyCode.Keypad1:
                    str2 = "[1]";
                    break;

                case KeyCode.Keypad2:
                    str2 = "[2]";
                    break;

                case KeyCode.Keypad3:
                    str2 = "[3]";
                    break;

                case KeyCode.Keypad4:
                    str2 = "[4]";
                    break;

                case KeyCode.Keypad5:
                    str2 = "[5]";
                    break;

                case KeyCode.Keypad6:
                    str2 = "[6]";
                    break;

                case KeyCode.Keypad7:
                    str2 = "[7]";
                    break;

                case KeyCode.Keypad8:
                    str2 = "[8]";
                    break;

                case KeyCode.Keypad9:
                    str2 = "[9]";
                    break;

                case KeyCode.KeypadPeriod:
                    str2 = "[.]";
                    break;

                case KeyCode.KeypadDivide:
                    str2 = "[/]";
                    break;

                case KeyCode.KeypadMinus:
                    str2 = "[-]";
                    break;

                case KeyCode.KeypadPlus:
                    str2 = "[+]";
                    break;

                case KeyCode.KeypadEnter:
                    str2 = "enter";
                    break;

                case KeyCode.KeypadEquals:
                    str2 = "[=]";
                    break;

                case KeyCode.UpArrow:
                    str2 = "up";
                    break;

                case KeyCode.DownArrow:
                    str2 = "down";
                    break;

                case KeyCode.RightArrow:
                    str2 = "right";
                    break;

                case KeyCode.LeftArrow:
                    str2 = "left";
                    break;

                case KeyCode.Insert:
                    str2 = "insert";
                    break;

                case KeyCode.Home:
                    str2 = "home";
                    break;

                case KeyCode.End:
                    str2 = "end";
                    break;

                case KeyCode.PageUp:
                    str2 = "page up";
                    break;

                case KeyCode.PageDown:
                    str2 = "page down";
                    break;

                case KeyCode.F1:
                    str2 = "F1";
                    break;

                case KeyCode.F2:
                    str2 = "F2";
                    break;

                case KeyCode.F3:
                    str2 = "F3";
                    break;

                case KeyCode.F4:
                    str2 = "F4";
                    break;

                case KeyCode.F5:
                    str2 = "F5";
                    break;

                case KeyCode.F6:
                    str2 = "F6";
                    break;

                case KeyCode.F7:
                    str2 = "F7";
                    break;

                case KeyCode.F8:
                    str2 = "F8";
                    break;

                case KeyCode.F9:
                    str2 = "F9";
                    break;

                case KeyCode.F10:
                    str2 = "F10";
                    break;

                case KeyCode.F11:
                    str2 = "F11";
                    break;

                case KeyCode.F12:
                    str2 = "F12";
                    break;

                case KeyCode.F13:
                    str2 = "F13";
                    break;

                case KeyCode.F14:
                    str2 = "F14";
                    break;

                case KeyCode.F15:
                    str2 = "F15";
                    break;

                default:
                    if (keyCode == KeyCode.Backspace)
                    {
                        str2 = "backspace";
                    }
                    else if (keyCode == KeyCode.Return)
                    {
                        str2 = "return";
                    }
                    else if (keyCode == KeyCode.Escape)
                    {
                        str2 = "[esc]";
                    }
                    else if (keyCode == KeyCode.Delete)
                    {
                        str2 = "delete";
                    }
                    else
                    {
                        str2 = "" + evt.keyCode;
                    }
                    break;
            }
            string str3 = string.Empty;
            if (evt.alt)
            {
                str3 = str3 + "Alt+";
            }
            if (evt.command)
            {
                str3 = str3 + ((Application.platform != RuntimePlatform.OSXEditor) ? "Ctrl+" : "Cmd+");
            }
            if (evt.control)
            {
                str3 = str3 + "Ctrl+";
            }
            if (evt.shift)
            {
                str3 = str3 + "Shift+";
            }
            return (str3 + str2);
        }

        public static Bounds TransformBounds(Bounds b, Transform t)
        {
            Bounds bounds;
            INTERNAL_CALL_TransformBounds(ref b, t, out bounds);
            return bounds;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void UpdateSortingLayersOrder();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern ulong VerifyCacheServerIntegrity();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool WiiUSaveStartupScreenToFile(Texture2D image, string path, int outputWidth, int outputHeight);

        public static float defaultScreenHeight { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static float defaultScreenWidth { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static float defaultWebScreenHeight { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static float defaultWebScreenWidth { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static int[] expandedProjectWindowItems { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public static bool ignoreInspectorChanges { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static bool inBatchMode { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static bool isApplicationActive { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static bool isHumanControllingUs { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static string[] layers { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static float remoteScreenHeight { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static float remoteScreenWidth { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal static string[] sortingLayerNames { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal static int[] sortingLayerUniqueIDs { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static string[] tags { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static string unityPreferencesFolder { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public enum HierarchyDropMode
        {
            kHierarchyDragNormal = 0,
            kHierarchyDropAfterParent = 4,
            kHierarchyDropBetween = 2,
            kHierarchyDropUpon = 1
        }

        public enum ScriptEditor
        {
            Internal = 0,
            MonoDevelop = 1,
            Other = 0x20,
            VisualStudio = 2,
            VisualStudioCode = 4,
            VisualStudioExpress = 3
        }
    }
}

