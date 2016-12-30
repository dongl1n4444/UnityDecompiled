namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.AnimatedValues;
    using UnityEditor.Rendering;
    using UnityEditor.SceneManagement;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Profiling;
    using UnityEngine.Rendering;

    [EditorWindowTitle(title="Scene", useTypeNameAsIconName=true)]
    public class SceneView : SearchableEditorWindow, IHasCustomMenu
    {
        [CompilerGenerated]
        private static EditorApplication.CallbackFunction <>f__mg$cache0;
        [CompilerGenerated]
        private static EditorApplication.CallbackFunction <>f__mg$cache1;
        [CompilerGenerated]
        private static ComponentUtility.IsDesiredComponent <>f__mg$cache2;
        [CompilerGenerated]
        private static ComponentUtility.IsDesiredComponent <>f__mg$cache3;
        [CompilerGenerated]
        private static SceneViewOverlay.WindowFunction <>f__mg$cache4;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map2;
        [SerializeField]
        private SceneViewGrid grid;
        private const double k_MaxDoubleKeypressTime = 0.5;
        private static readonly PrefKey k2DMode = new PrefKey("Tools/2D Mode", "2");
        private const CameraEvent kCommandBufferCameraEvent = CameraEvent.AfterImageEffectsOpaque;
        [NonSerialized]
        private static readonly Vector3 kDefaultPivot = Vector3.zero;
        [NonSerialized]
        private static readonly Quaternion kDefaultRotation = Quaternion.LookRotation(new Vector3(-1f, -0.7f, -1f));
        private const float kDefaultViewSize = 10f;
        private const float kOneOverSqrt2 = 0.7071068f;
        private const float kOrthoThresholdAngle = 3f;
        private const float kPerspectiveFov = 90f;
        private static readonly PrefColor kSceneViewBackground = new PrefColor("Scene/Background", 0.278431f, 0.278431f, 0.278431f, 0f);
        internal static Color kSceneViewDownLight = new Color(0.047f, 0.043f, 0.035f, 1f);
        internal static Color kSceneViewFrontLight = new Color(0.769f, 0.769f, 0.769f, 1f);
        internal static Color kSceneViewMidLight = new Color(0.114f, 0.125f, 0.133f, 1f);
        private static readonly PrefColor kSceneViewSelectedOutline = new PrefColor("Scene/Selected Outline", 1f, 0.4f, 0f, 0f);
        private static readonly PrefColor kSceneViewSelectedWire = new PrefColor("Scene/Wireframe Selected", 0.3686275f, 0.4666667f, 0.6078432f, 0.2509804f);
        internal static Color kSceneViewUpLight = new Color(0.212f, 0.227f, 0.259f, 1f);
        private static readonly PrefColor kSceneViewWire = new PrefColor("Scene/Wireframe", 0f, 0f, 0f, 0.5f);
        private static readonly PrefColor kSceneViewWireOverlay = new PrefColor("Scene/Wireframe Overlay", 0f, 0f, 0f, 0.25f);
        public double lastFramingTime = 0.0;
        private DrawCameraMode lastRenderMode = DrawCameraMode.Textured;
        [SerializeField]
        private bool m_2DMode;
        private GUIContent m_2DModeContent;
        private GUIStyle[] m_AlbedoSwatchColorStyles;
        private string[] m_AlbedoSwatchDescriptions;
        private GUIContent[] m_AlbedoSwatchGUIContent;
        private float m_AlbedoSwatchHueTolerance = 0.1f;
        private AlbedoSwatchInfo[] m_AlbedoSwatchInfos;
        private string[] m_AlbedoSwatchLuminanceStrings;
        private float m_AlbedoSwatchSaturationTolerance = 0.2f;
        public bool m_AudioPlay = false;
        private GUIContent m_AudioPlayContent;
        [NonSerialized]
        private Camera m_Camera;
        private EditorCache m_DragEditorCache;
        private DraggingLockedState m_DraggingLockedState;
        private GUIContent m_Fx;
        private GUIContent m_GizmosContent;
        [SerializeField]
        private bool m_isRotationLocked = false;
        private ColorSpace m_LastKnownColorSpace = ColorSpace.Uninitialized;
        [SerializeField]
        private Object m_LastLockedObject;
        private double m_lastRenderedTime;
        [SerializeField]
        private bool m_LastSceneViewOrtho;
        [SerializeField]
        private Quaternion m_LastSceneViewRotation;
        [NonSerialized]
        private Light[] m_Light = new Light[3];
        private GUIContent m_Lighting;
        private int m_MainViewControlID;
        internal Object m_OneClickDragObject;
        [SerializeField]
        internal AnimBool m_Ortho = new AnimBool();
        [SerializeField]
        private AnimVector3 m_Position = new AnimVector3(kDefaultPivot);
        private RectSelection m_RectSelection;
        private GUIContent m_RenderDocContent;
        public DrawCameraMode m_RenderMode = DrawCameraMode.Textured;
        [SerializeField]
        private Shader m_ReplacementShader;
        [SerializeField]
        private string m_ReplacementString;
        private bool m_RequestedSceneViewFiltering;
        [SerializeField]
        internal AnimQuaternion m_Rotation = new AnimQuaternion(kDefaultRotation);
        [SerializeField]
        public bool m_SceneLighting = true;
        private RenderTexture m_SceneTargetTexture;
        private SceneViewOverlay m_SceneViewOverlay;
        [SerializeField]
        internal SceneViewState m_SceneViewState;
        private int m_SelectedAlbedoSwatchIndex = 0;
        internal bool m_ShowSceneViewWindows = false;
        [SerializeField]
        private AnimFloat m_Size = new AnimFloat(10f);
        private double m_StartSearchFilterTime = -1.0;
        [NonSerialized]
        private ActiveEditorTracker m_Tracker;
        public bool m_ValidateTrueMetals = false;
        [SerializeField]
        private bool m_ViewIsLockedToObject;
        internal static OnSceneFunc onPreSceneGUIDelegate;
        public static OnSceneFunc onSceneGUIDelegate;
        private static Material s_AlphaOverlayMaterial;
        private static SceneView s_AudioSceneView;
        private static Shader s_AuraShader;
        private static SceneView s_CurrentDrawingSceneView;
        private static Tool s_CurrentTool;
        private static Material s_DeferredOverlayMaterial;
        private bool s_DraggingCursorIsCached;
        private static GUIStyle s_DropDownStyle;
        private static Shader s_GrayScaleShader;
        private static SceneView s_LastActiveSceneView;
        private static MouseCursor s_LastCursor = MouseCursor.Arrow;
        private static Texture2D s_MipColorsTexture;
        private static readonly List<CursorRect> s_MouseRects = new List<CursorRect>();
        private static ArrayList s_SceneViews = new ArrayList();
        private static Shader s_ShowLightmapsShader;
        private static Shader s_ShowMipsShader;
        private static Shader s_ShowOverdrawShader;
        [SerializeField]
        internal SceneViewRotation svRot;
        private static bool waitingFor2DModeKeyUp;

        public SceneView()
        {
            base.m_HierarchyType = HierarchyType.GameObjects;
        }

        [MenuItem("GameObject/Toggle Active State &#a")]
        internal static void ActivateSelection()
        {
            if (Selection.activeTransform != null)
            {
                GameObject[] gameObjects = Selection.gameObjects;
                Undo.RecordObjects(gameObjects, "Toggle Active State");
                bool flag = !Selection.activeGameObject.activeSelf;
                foreach (GameObject obj2 in gameObjects)
                {
                    obj2.SetActive(flag);
                }
            }
        }

        internal static void AddCursorRect(Rect rect, MouseCursor cursor)
        {
            if (Event.current.type == EventType.Repaint)
            {
                s_MouseRects.Add(new CursorRect(rect, cursor));
            }
        }

        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            if (RenderDoc.IsInstalled() && !RenderDoc.IsLoaded())
            {
                menu.AddItem(new GUIContent("Load RenderDoc"), false, new GenericMenu.MenuFunction(this.LoadRenderDoc));
            }
        }

        public void AlignViewToObject(Transform t)
        {
            this.FixNegativeSize();
            this.size = 10f;
            this.LookAt(t.position + ((Vector3) (t.forward * this.CalcCameraDist())), t.rotation);
        }

        public void AlignWithView()
        {
            float num;
            Vector3 vector3;
            this.FixNegativeSize();
            Vector3 position = this.camera.transform.position;
            Vector3 vector2 = position - Tools.handlePosition;
            (Quaternion.Inverse(Selection.activeTransform.rotation) * this.camera.transform.rotation).ToAngleAxis(out num, out vector3);
            vector3 = Selection.activeTransform.TransformDirection(vector3);
            Undo.RecordObjects(Selection.transforms, "Align with view");
            foreach (Transform transform in Selection.transforms)
            {
                transform.position += vector2;
                transform.RotateAround(position, vector3, num);
            }
        }

        internal void Awake()
        {
            if (this.m_SceneViewState == null)
            {
                this.m_SceneViewState = new SceneViewState();
            }
            if (this.m_2DMode || (EditorSettings.defaultBehaviorMode == EditorBehaviorMode.Mode2D))
            {
                this.m_LastSceneViewRotation = Quaternion.LookRotation(new Vector3(-1f, -0.7f, -1f));
                this.m_LastSceneViewOrtho = false;
                this.m_Rotation.value = Quaternion.identity;
                this.m_Ortho.value = true;
                this.m_2DMode = true;
                if (Tools.current == Tool.Move)
                {
                    Tools.current = Tool.Rect;
                }
            }
        }

        private float CalcCameraDist()
        {
            float num = this.m_Ortho.Fade(90f, 0f);
            if (num > 3f)
            {
                this.m_Camera.orthographic = false;
                return (this.size / Mathf.Tan((num * 0.5f) * 0.01745329f));
            }
            return 0f;
        }

        private void CallEditorDragFunctions()
        {
            Event current = Event.current;
            SpriteUtility.OnSceneDrag(this);
            if ((current.type != EventType.Used) && (DragAndDrop.objectReferences.Length != 0))
            {
                if (this.m_DragEditorCache == null)
                {
                    this.m_DragEditorCache = new EditorCache(EditorFeatures.OnSceneDrag);
                }
                foreach (Object obj2 in DragAndDrop.objectReferences)
                {
                    if (obj2 != null)
                    {
                        EditorWrapper wrapper = this.m_DragEditorCache[obj2];
                        if (wrapper != null)
                        {
                            wrapper.OnSceneDrag(this);
                        }
                        if (current.type == EventType.Used)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void CallOnPreSceneGUI()
        {
            foreach (Editor editor in this.GetActiveEditors())
            {
                Handles.ClearHandles();
                Component target = editor.target as Component;
                if ((target == null) || target.gameObject.activeInHierarchy)
                {
                    MethodInfo method = editor.GetType().GetMethod("OnPreSceneGUI", BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (method != null)
                    {
                        Editor.m_AllowMultiObjectAccess = true;
                        for (int i = 0; i < editor.targets.Length; i++)
                        {
                            editor.referenceTargetIndex = i;
                            Editor.m_AllowMultiObjectAccess = !editor.canEditMultipleObjects;
                            method.Invoke(editor, null);
                            Editor.m_AllowMultiObjectAccess = true;
                        }
                    }
                }
            }
            if (onPreSceneGUIDelegate != null)
            {
                onPreSceneGUIDelegate(this);
            }
            Handles.ClearHandles();
        }

        private void CallOnSceneGUI()
        {
            foreach (Editor editor in this.GetActiveEditors())
            {
                if (EditorGUIUtility.IsGizmosAllowedForObject(editor.target))
                {
                    MethodInfo method = editor.GetType().GetMethod("OnSceneGUI", BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (method != null)
                    {
                        Editor.m_AllowMultiObjectAccess = true;
                        for (int i = 0; i < editor.targets.Length; i++)
                        {
                            this.ResetOnSceneGUIState();
                            editor.referenceTargetIndex = i;
                            EditorGUI.BeginChangeCheck();
                            Editor.m_AllowMultiObjectAccess = !editor.canEditMultipleObjects;
                            method.Invoke(editor, null);
                            Editor.m_AllowMultiObjectAccess = true;
                            if (EditorGUI.EndChangeCheck())
                            {
                                editor.serializedObject.SetIsDifferentCacheDirty();
                            }
                        }
                        this.ResetOnSceneGUIState();
                    }
                }
            }
            if (onSceneGUIDelegate != null)
            {
                onSceneGUIDelegate(this);
                this.ResetOnSceneGUIState();
            }
        }

        internal bool CheckDrawModeForRenderingPath(DrawCameraMode mode)
        {
            RenderingPath actualRenderingPath = this.m_Camera.actualRenderingPath;
            if (((mode == DrawCameraMode.DeferredDiffuse) || (mode == DrawCameraMode.DeferredSpecular)) || ((mode == DrawCameraMode.DeferredSmoothness) || (mode == DrawCameraMode.DeferredNormal)))
            {
                return (actualRenderingPath == RenderingPath.DeferredShading);
            }
            return true;
        }

        private void CleanupCustomSceneLighting()
        {
            if (!this.m_SceneLighting && (Event.current.type == EventType.Repaint))
            {
                InternalEditorUtility.RemoveCustomLighting();
            }
        }

        private void CleanupEditorDragFunctions()
        {
            if (this.m_DragEditorCache != null)
            {
                this.m_DragEditorCache.Dispose();
            }
            this.m_DragEditorCache = null;
        }

        private void CommandsGUI()
        {
            bool flag = Event.current.type == EventType.ExecuteCommand;
            string commandName = Event.current.commandName;
            if (commandName != null)
            {
                int num;
                if (<>f__switch$map2 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(9) {
                        { 
                            "Find",
                            0
                        },
                        { 
                            "FrameSelected",
                            1
                        },
                        { 
                            "FrameSelectedWithLock",
                            2
                        },
                        { 
                            "SoftDelete",
                            3
                        },
                        { 
                            "Delete",
                            3
                        },
                        { 
                            "Duplicate",
                            4
                        },
                        { 
                            "Copy",
                            5
                        },
                        { 
                            "Paste",
                            6
                        },
                        { 
                            "SelectAll",
                            7
                        }
                    };
                    <>f__switch$map2 = dictionary;
                }
                if (<>f__switch$map2.TryGetValue(commandName, out num))
                {
                    switch (num)
                    {
                        case 0:
                            if (flag)
                            {
                                base.FocusSearchField();
                            }
                            Event.current.Use();
                            break;

                        case 1:
                            if (flag)
                            {
                                bool lockView = (EditorApplication.timeSinceStartup - this.lastFramingTime) < 0.5;
                                this.FrameSelected(lockView);
                                this.lastFramingTime = EditorApplication.timeSinceStartup;
                            }
                            Event.current.Use();
                            break;

                        case 2:
                            if (flag)
                            {
                                this.FrameSelected(true);
                            }
                            Event.current.Use();
                            break;

                        case 3:
                            if (flag)
                            {
                                Unsupported.DeleteGameObjectSelection();
                            }
                            Event.current.Use();
                            break;

                        case 4:
                            if (flag)
                            {
                                Unsupported.DuplicateGameObjectsUsingPasteboard();
                            }
                            Event.current.Use();
                            break;

                        case 5:
                            if (flag)
                            {
                                Unsupported.CopyGameObjectsToPasteboard();
                            }
                            Event.current.Use();
                            break;

                        case 6:
                            if (flag)
                            {
                                Unsupported.PasteGameObjectsFromPasteboard();
                            }
                            Event.current.Use();
                            break;

                        case 7:
                            if (flag)
                            {
                                Selection.objects = Object.FindObjectsOfType(typeof(GameObject));
                            }
                            Event.current.Use();
                            break;
                    }
                }
            }
        }

        private static bool ComponentHasImageEffectAttribute(Component c)
        {
            if (c == null)
            {
                return false;
            }
            return Attribute.IsDefined(c.GetType(), typeof(ImageEffectAllowedInSceneView));
        }

        private void CreateAlbedoSwatchData()
        {
            AlbedoSwatchInfo[] albedoSwatches = EditorGraphicsSettings.albedoSwatches;
            if (albedoSwatches.Length != 0)
            {
                this.m_AlbedoSwatchInfos = albedoSwatches;
            }
            else
            {
                AlbedoSwatchInfo[] infoArray1 = new AlbedoSwatchInfo[13];
                AlbedoSwatchInfo info = new AlbedoSwatchInfo {
                    name = "Black Acrylic Paint",
                    color = new Color(0.2196078f, 0.2196078f, 0.2196078f),
                    minLuminance = 0.03f,
                    maxLuminance = 0.07f
                };
                infoArray1[0] = info;
                AlbedoSwatchInfo info2 = new AlbedoSwatchInfo {
                    name = "Dark Soil",
                    color = new Color(0.3333333f, 0.2392157f, 0.1921569f),
                    minLuminance = 0.05f,
                    maxLuminance = 0.14f
                };
                infoArray1[1] = info2;
                AlbedoSwatchInfo info3 = new AlbedoSwatchInfo {
                    name = "Worn Asphalt",
                    color = new Color(0.3568628f, 0.3568628f, 0.3568628f),
                    minLuminance = 0.1f,
                    maxLuminance = 0.15f
                };
                infoArray1[2] = info3;
                AlbedoSwatchInfo info4 = new AlbedoSwatchInfo {
                    name = "Dry Clay Soil",
                    color = new Color(0.5372549f, 0.4705882f, 0.4f),
                    minLuminance = 0.15f,
                    maxLuminance = 0.35f
                };
                infoArray1[3] = info4;
                AlbedoSwatchInfo info5 = new AlbedoSwatchInfo {
                    name = "Green Grass",
                    color = new Color(0.4823529f, 0.5137255f, 0.2901961f),
                    minLuminance = 0.16f,
                    maxLuminance = 0.26f
                };
                infoArray1[4] = info5;
                AlbedoSwatchInfo info6 = new AlbedoSwatchInfo {
                    name = "Old Concrete",
                    color = new Color(0.5294118f, 0.5333334f, 0.5137255f),
                    minLuminance = 0.17f,
                    maxLuminance = 0.3f
                };
                infoArray1[5] = info6;
                AlbedoSwatchInfo info7 = new AlbedoSwatchInfo {
                    name = "Red Clay Tile",
                    color = new Color(0.772549f, 0.4901961f, 0.3921569f),
                    minLuminance = 0.23f,
                    maxLuminance = 0.33f
                };
                infoArray1[6] = info7;
                AlbedoSwatchInfo info8 = new AlbedoSwatchInfo {
                    name = "Dry Sand",
                    color = new Color(0.6941177f, 0.654902f, 0.5176471f),
                    minLuminance = 0.2f,
                    maxLuminance = 0.45f
                };
                infoArray1[7] = info8;
                AlbedoSwatchInfo info9 = new AlbedoSwatchInfo {
                    name = "New Concrete",
                    color = new Color(0.7254902f, 0.7137255f, 0.6862745f),
                    minLuminance = 0.32f,
                    maxLuminance = 0.55f
                };
                infoArray1[8] = info9;
                AlbedoSwatchInfo info10 = new AlbedoSwatchInfo {
                    name = "White Acrylic Paint",
                    color = new Color(0.8901961f, 0.8901961f, 0.8901961f),
                    minLuminance = 0.75f,
                    maxLuminance = 0.85f
                };
                infoArray1[9] = info10;
                AlbedoSwatchInfo info11 = new AlbedoSwatchInfo {
                    name = "Fresh Snow",
                    color = new Color(0.9529412f, 0.9529412f, 0.9529412f),
                    minLuminance = 0.85f,
                    maxLuminance = 0.95f
                };
                infoArray1[10] = info11;
                AlbedoSwatchInfo info12 = new AlbedoSwatchInfo {
                    name = "Blue Sky",
                    color = new Color(0.3647059f, 0.4823529f, 0.6156863f)
                };
                Color color = new Color(0.3647059f, 0.4823529f, 0.6156863f);
                info12.minLuminance = color.linear.maxColorComponent - 0.05f;
                Color color3 = new Color(0.3647059f, 0.4823529f, 0.6156863f);
                info12.maxLuminance = color3.linear.maxColorComponent + 0.05f;
                infoArray1[11] = info12;
                AlbedoSwatchInfo info13 = new AlbedoSwatchInfo {
                    name = "Foliage",
                    color = new Color(0.3568628f, 0.4235294f, 0.254902f)
                };
                Color color5 = new Color(0.3568628f, 0.4235294f, 0.254902f);
                info13.minLuminance = color5.linear.maxColorComponent - 0.05f;
                Color color7 = new Color(0.3568628f, 0.4235294f, 0.254902f);
                info13.maxLuminance = color7.linear.maxColorComponent + 0.05f;
                infoArray1[12] = info13;
                this.m_AlbedoSwatchInfos = infoArray1;
            }
            this.UpdateAlbedoSwatchGUI();
        }

        private void CreateCameraTargetTexture(Rect cameraRect, bool hdr)
        {
            bool flag = QualitySettings.activeColorSpace == ColorSpace.Linear;
            int num = Mathf.Max(1, QualitySettings.antiAliasing);
            if (this.IsSceneCameraDeferred())
            {
                num = 1;
            }
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal)
            {
                num = 1;
            }
            RenderTextureFormat format = (!hdr || !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf)) ? RenderTextureFormat.ARGB32 : RenderTextureFormat.ARGBHalf;
            if (this.m_SceneTargetTexture != null)
            {
                bool flag2 = (this.m_SceneTargetTexture != null) && (flag == this.m_SceneTargetTexture.sRGB);
                if (((this.m_SceneTargetTexture.format != format) || (this.m_SceneTargetTexture.antiAliasing != num)) || !flag2)
                {
                    Object.DestroyImmediate(this.m_SceneTargetTexture);
                    this.m_SceneTargetTexture = null;
                }
            }
            Rect rect = Handles.GetCameraRect(cameraRect);
            int width = (int) rect.width;
            int height = (int) rect.height;
            if (this.m_SceneTargetTexture == null)
            {
                this.m_SceneTargetTexture = new RenderTexture(0, 0, 0x18, format);
                this.m_SceneTargetTexture.name = "SceneView RT";
                this.m_SceneTargetTexture.antiAliasing = num;
                this.m_SceneTargetTexture.hideFlags = HideFlags.HideAndDontSave;
                EditorGUIUtility.SetGUITextureBlitColorspaceSettings(EditorGUIUtility.GUITextureBlitColorspaceMaterial);
            }
            if ((this.m_SceneTargetTexture.width != width) || (this.m_SceneTargetTexture.height != height))
            {
                this.m_SceneTargetTexture.Release();
                this.m_SceneTargetTexture.width = width;
                this.m_SceneTargetTexture.height = height;
            }
            this.m_SceneTargetTexture.Create();
        }

        private static void CreateMipColorsTexture()
        {
            if (s_MipColorsTexture == null)
            {
                s_MipColorsTexture = new Texture2D(0x20, 0x20, TextureFormat.RGBA32, true);
                s_MipColorsTexture.hideFlags = HideFlags.HideAndDontSave;
                Color[] colorArray = new Color[] { new Color(0f, 0f, 1f, 0.8f), new Color(0f, 0.5f, 1f, 0.4f), new Color(1f, 1f, 1f, 0f), new Color(1f, 0.7f, 0f, 0.2f), new Color(1f, 0.3f, 0f, 0.6f), new Color(1f, 0f, 0f, 0.8f) };
                int num = Mathf.Min(6, s_MipColorsTexture.mipmapCount);
                for (int i = 0; i < num; i++)
                {
                    int num3 = Mathf.Max(s_MipColorsTexture.width >> i, 1);
                    int num4 = Mathf.Max(s_MipColorsTexture.height >> i, 1);
                    Color[] colors = new Color[num3 * num4];
                    for (int j = 0; j < colors.Length; j++)
                    {
                        colors[j] = colorArray[i];
                    }
                    s_MipColorsTexture.SetPixels(colors, i);
                }
                s_MipColorsTexture.filterMode = FilterMode.Trilinear;
                s_MipColorsTexture.Apply(false);
                Shader.SetGlobalTexture("_SceneViewMipcolorsTexture", s_MipColorsTexture);
            }
        }

        private void CreateSceneCameraAndLights()
        {
            Type[] components = new Type[] { typeof(Camera) };
            GameObject obj2 = EditorUtility.CreateGameObjectWithHideFlags("SceneCamera", HideFlags.HideAndDontSave, components);
            obj2.AddComponentInternal("FlareLayer");
            obj2.AddComponentInternal("HaloLayer");
            this.m_Camera = obj2.GetComponent<Camera>();
            this.m_Camera.enabled = false;
            this.m_Camera.cameraType = CameraType.SceneView;
            for (int i = 0; i < 3; i++)
            {
                Type[] typeArray2 = new Type[] { typeof(Light) };
                this.m_Light[i] = EditorUtility.CreateGameObjectWithHideFlags("SceneLight", HideFlags.HideAndDontSave, typeArray2).GetComponent<Light>();
                this.m_Light[i].type = LightType.Directional;
                this.m_Light[i].intensity = 1f;
                this.m_Light[i].enabled = false;
            }
            this.m_Light[0].color = kSceneViewFrontLight;
            this.m_Light[1].color = kSceneViewUpLight - kSceneViewMidLight;
            this.m_Light[1].transform.LookAt(Vector3.down);
            this.m_Light[1].renderMode = LightRenderMode.ForceVertex;
            this.m_Light[2].color = kSceneViewDownLight - kSceneViewMidLight;
            this.m_Light[2].transform.LookAt(Vector3.up);
            this.m_Light[2].renderMode = LightRenderMode.ForceVertex;
            HandleUtility.handleMaterial.SetColor("_SkyColor", (Color) (kSceneViewUpLight * 1.5f));
            HandleUtility.handleMaterial.SetColor("_GroundColor", (Color) (kSceneViewDownLight * 1.5f));
            HandleUtility.handleMaterial.SetColor("_Color", (Color) (kSceneViewFrontLight * 1.5f));
            this.SetupPBRValidation();
        }

        private string CreateSwatchDescriptionForName(float minLum, float maxLum)
        {
            string[] textArray1 = new string[] { "Luminance (", minLum.ToString("F2"), " - ", maxLum.ToString("F2"), ")" };
            return string.Concat(textArray1);
        }

        private GUIStyle CreateSwatchStyleForColor(Color c)
        {
            Texture2D textured = new Texture2D(1, 1);
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
            {
                c = c.gamma;
            }
            textured.SetPixel(0, 0, c);
            textured.Apply();
            return new GUIStyle { normal = { background = textured } };
        }

        private void DefaultHandles()
        {
            EditorGUI.BeginChangeCheck();
            bool flag = Event.current.GetTypeForControl(GUIUtility.hotControl) == EventType.MouseDrag;
            bool flag2 = Event.current.GetTypeForControl(GUIUtility.hotControl) == EventType.MouseUp;
            if (GUIUtility.hotControl == 0)
            {
                s_CurrentTool = !Tools.viewToolActive ? Tools.current : Tool.View;
            }
            Tool tool = (Event.current.type != EventType.Repaint) ? s_CurrentTool : Tools.current;
            switch ((tool + 1))
            {
                case Tool.Rotate:
                    MoveTool.OnGUI(this);
                    break;

                case Tool.Scale:
                    RotateTool.OnGUI(this);
                    break;

                case Tool.Rect:
                    ScaleTool.OnGUI(this);
                    break;

                case (Tool.Rect | Tool.Move):
                    RectTool.OnGUI(this);
                    break;
            }
            if ((EditorGUI.EndChangeCheck() && EditorApplication.isPlaying) && flag)
            {
                Physics2D.SetEditorDragMovement(true, Selection.gameObjects);
            }
            if (EditorApplication.isPlaying && flag2)
            {
                Physics2D.SetEditorDragMovement(false, Selection.gameObjects);
            }
        }

        private void DoClearCamera(Rect cameraRect)
        {
            float verticalFOV = this.GetVerticalFOV(90f);
            float fieldOfView = this.m_Camera.fieldOfView;
            this.m_Camera.fieldOfView = verticalFOV;
            Handles.ClearCamera(cameraRect, this.m_Camera);
            this.m_Camera.fieldOfView = fieldOfView;
        }

        private void DoDrawCamera(Rect cameraRect, out bool pushedGUIClip)
        {
            pushedGUIClip = false;
            if (this.m_Camera.gameObject.activeInHierarchy)
            {
                DrawGridParameters gridParam = this.grid.PrepareGridRender(this.camera, this.pivot, this.m_Rotation.target, this.m_Size.value, this.m_Ortho.target, AnnotationUtility.showGrid);
                Event current = Event.current;
                if (this.UseSceneFiltering())
                {
                    if (current.type == EventType.Repaint)
                    {
                        Handles.EnableCameraFx(this.m_Camera, true);
                        Handles.SetCameraFilterMode(this.m_Camera, Handles.FilterMode.ShowRest);
                        float fade = Mathf.Clamp01((float) (EditorApplication.timeSinceStartup - this.m_StartSearchFilterTime));
                        Handles.DrawCamera(cameraRect, this.m_Camera, this.m_RenderMode);
                        Handles.DrawCameraFade(this.m_Camera, fade);
                        Handles.EnableCameraFx(this.m_Camera, false);
                        Handles.SetCameraFilterMode(this.m_Camera, Handles.FilterMode.ShowFiltered);
                        if (s_AuraShader == null)
                        {
                            s_AuraShader = EditorGUIUtility.LoadRequired("SceneView/SceneViewAura.shader") as Shader;
                        }
                        this.m_Camera.SetReplacementShader(s_AuraShader, "");
                        Handles.DrawCamera(cameraRect, this.m_Camera, this.m_RenderMode);
                        this.m_Camera.SetReplacementShader(this.m_ReplacementShader, this.m_ReplacementString);
                        Handles.DrawCamera(cameraRect, this.m_Camera, this.m_RenderMode, gridParam);
                        if (fade < 1f)
                        {
                            base.Repaint();
                        }
                    }
                    Rect screenRect = cameraRect;
                    if (current.type == EventType.Repaint)
                    {
                        RenderTexture.active = null;
                    }
                    GUI.EndGroup();
                    GUI.BeginGroup(new Rect(0f, 17f, base.position.width, base.position.height - 17f));
                    if (current.type == EventType.Repaint)
                    {
                        GL.sRGBWrite = QualitySettings.activeColorSpace == ColorSpace.Linear;
                        Graphics.DrawTexture(screenRect, this.m_SceneTargetTexture, new Rect(0f, 0f, 1f, 1f), 0, 0, 0, 0, GUI.color, EditorGUIUtility.GUITextureBlitColorspaceMaterial);
                        GL.sRGBWrite = false;
                    }
                    Handles.SetCamera(cameraRect, this.m_Camera);
                    this.HandleSelectionAndOnSceneGUI();
                }
                else
                {
                    if (this.SceneCameraRendersIntoRT())
                    {
                        GUIClip.Push(new Rect(0f, 0f, base.position.width, base.position.height), Vector2.zero, Vector2.zero, true);
                        pushedGUIClip = true;
                    }
                    Handles.DrawCameraStep1(cameraRect, this.m_Camera, this.m_RenderMode, gridParam);
                    this.DrawRenderModeOverlay(cameraRect);
                }
            }
        }

        internal static bool DoesCameraDrawModeSupportDeferred(DrawCameraMode mode) => 
            (((((((mode == DrawCameraMode.Normal) || (mode == DrawCameraMode.Textured)) || ((mode == DrawCameraMode.TexturedWire) || (mode == DrawCameraMode.ShadowCascades))) || (((mode == DrawCameraMode.RenderPaths) || (mode == DrawCameraMode.AlphaChannel)) || ((mode == DrawCameraMode.DeferredDiffuse) || (mode == DrawCameraMode.DeferredSpecular)))) || ((((mode == DrawCameraMode.DeferredSmoothness) || (mode == DrawCameraMode.DeferredNormal)) || ((mode == DrawCameraMode.Charting) || (mode == DrawCameraMode.Systems))) || (((mode == DrawCameraMode.Albedo) || (mode == DrawCameraMode.Emissive)) || ((mode == DrawCameraMode.Irradiance) || (mode == DrawCameraMode.Directionality))))) || (((mode == DrawCameraMode.Baked) || (mode == DrawCameraMode.Clustering)) || ((mode == DrawCameraMode.LitClustering) || (mode == DrawCameraMode.ValidateAlbedo)))) || (mode == DrawCameraMode.ValidateMetalSpecular));

        internal static bool DoesCameraDrawModeSupportHDR(DrawCameraMode mode) => 
            ((mode == DrawCameraMode.Textured) || (mode == DrawCameraMode.TexturedWire));

        private void DoOnPreSceneGUICallbacks(Rect cameraRect)
        {
            if (!this.UseSceneFiltering())
            {
                Handles.SetCamera(cameraRect, this.m_Camera);
                this.CallOnPreSceneGUI();
            }
        }

        private void DoToolbarGUI()
        {
            GUILayout.BeginHorizontal("toolbar", new GUILayoutOption[0]);
            GUIContent gUIContent = SceneRenderModeWindow.GetGUIContent(this.m_RenderMode);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(120f) };
            if (EditorGUI.ButtonMouseDown(GUILayoutUtility.GetRect(gUIContent, EditorStyles.toolbarDropDown, options), gUIContent, FocusType.Passive, EditorStyles.toolbarDropDown))
            {
                PopupWindow.Show(GUILayoutUtility.topLevel.GetLast(), new SceneRenderModeWindow(this));
                GUIUtility.ExitGUI();
            }
            EditorGUILayout.Space();
            this.in2DMode = GUILayout.Toggle(this.in2DMode, this.m_2DModeContent, "toolbarbutton", new GUILayoutOption[0]);
            EditorGUILayout.Space();
            this.m_SceneLighting = GUILayout.Toggle(this.m_SceneLighting, this.m_Lighting, "toolbarbutton", new GUILayoutOption[0]);
            if (this.renderMode == DrawCameraMode.ShadowCascades)
            {
                this.m_SceneLighting = true;
            }
            GUI.enabled = !Application.isPlaying;
            GUI.changed = false;
            this.m_AudioPlay = GUILayout.Toggle(this.m_AudioPlay, this.m_AudioPlayContent, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            if (GUI.changed)
            {
                this.RefreshAudioPlay();
            }
            GUI.enabled = true;
            Rect rect = GUILayoutUtility.GetRect(this.m_Fx, this.effectsDropDownStyle);
            Rect position = new Rect(rect.xMax - this.effectsDropDownStyle.border.right, rect.y, (float) this.effectsDropDownStyle.border.right, rect.height);
            if (EditorGUI.ButtonMouseDown(position, GUIContent.none, FocusType.Passive, GUIStyle.none))
            {
                PopupWindow.Show(GUILayoutUtility.topLevel.GetLast(), new SceneFXWindow(this));
                GUIUtility.ExitGUI();
            }
            bool flag = GUI.Toggle(rect, this.m_SceneViewState.IsAllOn(), this.m_Fx, this.effectsDropDownStyle);
            if (flag != this.m_SceneViewState.IsAllOn())
            {
                this.m_SceneViewState.Toggle(flag);
            }
            EditorGUILayout.Space();
            GUILayout.FlexibleSpace();
            if (((this.m_MainViewControlID != GUIUtility.keyboardControl) && (Event.current.type == EventType.KeyDown)) && !string.IsNullOrEmpty(base.m_SearchFilter))
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.UpArrow:
                    case KeyCode.DownArrow:
                        if (Event.current.keyCode == KeyCode.UpArrow)
                        {
                            base.SelectPreviousSearchResult();
                        }
                        else
                        {
                            base.SelectNextSearchResult();
                        }
                        this.FrameSelected(false);
                        Event.current.Use();
                        GUIUtility.ExitGUI();
                        return;
                }
            }
            if (RenderDoc.IsLoaded())
            {
                using (new EditorGUI.DisabledScope(!RenderDoc.IsSupported()))
                {
                    if (GUILayout.Button(this.m_RenderDocContent, EditorStyles.toolbarButton, new GUILayoutOption[0]))
                    {
                        base.m_Parent.CaptureRenderDoc();
                        GUIUtility.ExitGUI();
                    }
                }
            }
            if (EditorGUI.ButtonMouseDown(GUILayoutUtility.GetRect(this.m_GizmosContent, EditorStyles.toolbarDropDown), this.m_GizmosContent, FocusType.Passive, EditorStyles.toolbarDropDown) && AnnotationWindow.ShowAtPosition(GUILayoutUtility.topLevel.GetLast(), false))
            {
                GUIUtility.ExitGUI();
            }
            GUILayout.Space(6f);
            base.SearchFieldGUI(EditorGUILayout.kLabelFloatMaxW);
            GUILayout.EndHorizontal();
        }

        private static void DrawPBRSettings(Object target, SceneView sceneView)
        {
            sceneView.DrawTrueMetalCheckbox();
            sceneView.DrawPBRSettingsForScene();
        }

        internal void DrawPBRSettingsForScene()
        {
            if (this.m_RenderMode == DrawCameraMode.ValidateAlbedo)
            {
                EditorGUIUtility.labelWidth = 140f;
                this.m_SelectedAlbedoSwatchIndex = EditorGUILayout.Popup(new GUIContent("Luminance Validation:", "Select default luminance validation or validate against a configured albedo swatch"), this.m_SelectedAlbedoSwatchIndex, this.m_AlbedoSwatchGUIContent, new GUILayoutOption[0]);
                EditorGUI.indentLevel++;
                using (new EditorGUILayout.HorizontalScope(new GUILayoutOption[0]))
                {
                    EditorGUIUtility.labelWidth = 5f;
                    EditorGUILayout.LabelField(" ", this.m_AlbedoSwatchColorStyles[this.m_SelectedAlbedoSwatchIndex], new GUILayoutOption[0]);
                    EditorGUIUtility.labelWidth = 140f;
                    EditorGUILayout.LabelField(this.m_AlbedoSwatchLuminanceStrings[this.m_SelectedAlbedoSwatchIndex], new GUILayoutOption[0]);
                }
                this.UpdateAlbedoSwatch();
                EditorGUI.indentLevel--;
                using (new EditorGUI.DisabledScope(this.m_SelectedAlbedoSwatchIndex == 0))
                {
                    EditorGUI.BeginChangeCheck();
                    using (new EditorGUI.DisabledScope(this.m_SelectedAlbedoSwatchIndex == 0))
                    {
                        this.m_AlbedoSwatchHueTolerance = EditorGUILayout.Slider(new GUIContent("Hue Tolerance:", "Check that the hue of the albedo value of a material is within the tolerance of the hue of the albedo swatch being validated against"), this.m_AlbedoSwatchHueTolerance, 0f, 0.5f, new GUILayoutOption[0]);
                        this.m_AlbedoSwatchSaturationTolerance = EditorGUILayout.Slider(new GUIContent("Saturation Tolerance:", "Check that the saturation of the albedo value of a material is within the tolerance of the saturation of the albedo swatch being validated against"), this.m_AlbedoSwatchSaturationTolerance, 0f, 0.5f, new GUILayoutOption[0]);
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.UpdateAlbedoSwatch();
                    }
                }
            }
        }

        private void DrawRenderModeOverlay(Rect cameraRect)
        {
            if (this.m_RenderMode == DrawCameraMode.AlphaChannel)
            {
                if (s_AlphaOverlayMaterial == null)
                {
                    s_AlphaOverlayMaterial = EditorGUIUtility.LoadRequired("SceneView/SceneViewAlphaMaterial.mat") as Material;
                }
                Handles.BeginGUI();
                if (Event.current.type == EventType.Repaint)
                {
                    Graphics.DrawTexture(cameraRect, EditorGUIUtility.whiteTexture, s_AlphaOverlayMaterial);
                }
                Handles.EndGUI();
            }
            if (((this.m_RenderMode == DrawCameraMode.DeferredDiffuse) || (this.m_RenderMode == DrawCameraMode.DeferredSpecular)) || ((this.m_RenderMode == DrawCameraMode.DeferredSmoothness) || (this.m_RenderMode == DrawCameraMode.DeferredNormal)))
            {
                if (s_DeferredOverlayMaterial == null)
                {
                    s_DeferredOverlayMaterial = EditorGUIUtility.LoadRequired("SceneView/SceneViewDeferredMaterial.mat") as Material;
                }
                Handles.BeginGUI();
                if (Event.current.type == EventType.Repaint)
                {
                    s_DeferredOverlayMaterial.SetInt("_DisplayMode", ((int) this.m_RenderMode) - 8);
                    Graphics.DrawTexture(cameraRect, EditorGUIUtility.whiteTexture, s_DeferredOverlayMaterial);
                }
                Handles.EndGUI();
            }
        }

        internal void DrawTrueMetalCheckbox()
        {
            EditorGUI.BeginChangeCheck();
            this.m_ValidateTrueMetals = EditorGUILayout.ToggleLeft(new GUIContent("Check Pure Metals", "Check if albedo is black for materials with an average specular color above 0.45"), this.m_ValidateTrueMetals, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                Shader.SetGlobalInt("_CheckPureMetal", !this.m_ValidateTrueMetals ? 0 : 1);
            }
        }

        private void DrawValidateAlbedoSwatches(SceneView sceneView)
        {
            if (this.m_AlbedoSwatchInfos == null)
            {
                this.CreateAlbedoSwatchData();
            }
            if (PlayerSettings.colorSpace != this.m_LastKnownColorSpace)
            {
                this.UpdateAlbedoSwatchGUI();
                this.UpdateAlbedoSwatch();
            }
            if (<>f__mg$cache4 == null)
            {
                <>f__mg$cache4 = new SceneViewOverlay.WindowFunction(SceneView.DrawPBRSettings);
            }
            SceneViewOverlay.Window(new GUIContent("PBR Validation Settings"), <>f__mg$cache4, 350, SceneViewOverlay.WindowDisplayOption.OneWindowPerTitle);
        }

        public void FixNegativeSize()
        {
            float num = 90f;
            if (this.size < 0f)
            {
                float z = this.size / Mathf.Tan((num * 0.5f) * 0.01745329f);
                Vector3 vector = this.m_Position.value + (this.rotation * new Vector3(0f, 0f, -z));
                this.size = -this.size;
                z = this.size / Mathf.Tan((num * 0.5f) * 0.01745329f);
                this.m_Position.value = vector + (this.rotation * new Vector3(0f, 0f, z));
            }
        }

        internal bool Frame(Bounds bounds)
        {
            float num = bounds.extents.magnitude * 1.5f;
            switch (num)
            {
                case float.PositiveInfinity:
                    return false;

                case 0f:
                    num = 10f;
                    break;
            }
            this.LookAt(bounds.center, this.m_Rotation.target, num * 2.2f, this.m_Ortho.value, EditorApplication.isPlaying);
            return true;
        }

        public static bool FrameLastActiveSceneView() => 
            lastActiveSceneView?.SendEvent(EditorGUIUtility.CommandEvent("FrameSelected"));

        public static bool FrameLastActiveSceneViewWithLock() => 
            lastActiveSceneView?.SendEvent(EditorGUIUtility.CommandEvent("FrameSelectedWithLock"));

        public bool FrameSelected() => 
            this.FrameSelected(false);

        public bool FrameSelected(bool lockView)
        {
            this.viewIsLockedToObject = lockView;
            this.FixNegativeSize();
            Bounds bounds = InternalEditorUtility.CalculateSelectionBounds(false, Tools.pivotMode == PivotMode.Pivot);
            foreach (Editor editor in this.GetActiveEditors())
            {
                MethodInfo method = editor.GetType().GetMethod("HasFrameBounds", BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (method != null)
                {
                    object obj2 = method.Invoke(editor, null);
                    if ((obj2 is bool) && ((bool) obj2))
                    {
                        MethodInfo info2 = editor.GetType().GetMethod("OnGetFrameBounds", BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                        if (info2 != null)
                        {
                            object obj3 = info2.Invoke(editor, null);
                            if (obj3 is Bounds)
                            {
                                bounds = (Bounds) obj3;
                            }
                        }
                    }
                }
            }
            return this.Frame(bounds);
        }

        private Editor[] GetActiveEditors() => 
            this.m_Tracker?.activeEditors;

        public static Camera[] GetAllSceneCameras()
        {
            ArrayList list = new ArrayList();
            for (int i = 0; i < s_SceneViews.Count; i++)
            {
                Camera camera = ((SceneView) s_SceneViews[i]).m_Camera;
                if (camera != null)
                {
                    list.Add(camera);
                }
            }
            return (Camera[]) list.ToArray(typeof(Camera));
        }

        internal static Camera GetMainCamera()
        {
            Camera main = Camera.main;
            if (main != null)
            {
                return main;
            }
            Camera[] allCameras = Camera.allCameras;
            if ((allCameras != null) && (allCameras.Length == 1))
            {
                return allCameras[0];
            }
            return null;
        }

        internal static RenderingPath GetSceneViewRenderingPath()
        {
            Camera mainCamera = GetMainCamera();
            if (mainCamera != null)
            {
                return mainCamera.renderingPath;
            }
            return RenderingPath.UsePlayerSettings;
        }

        internal float GetVerticalFOV(float aspectNeutralFOV)
        {
            float f = (Mathf.Tan((aspectNeutralFOV * 0.5f) * 0.01745329f) * 0.7071068f) / Mathf.Sqrt(this.m_Camera.aspect);
            return ((Mathf.Atan(f) * 2f) * 57.29578f);
        }

        internal float GetVerticalOrthoSize() => 
            ((this.size * 0.7071068f) / Mathf.Sqrt(this.m_Camera.aspect));

        private void Handle2DModeSwitch()
        {
            Event current = Event.current;
            if (k2DMode.activated && !waitingFor2DModeKeyUp)
            {
                waitingFor2DModeKeyUp = true;
                this.in2DMode = !this.in2DMode;
                current.Use();
            }
            else if ((current.type == EventType.KeyUp) && (current.keyCode == k2DMode.KeyboardEvent.keyCode))
            {
                waitingFor2DModeKeyUp = false;
            }
        }

        private void HandleClickAndDragToFocus()
        {
            Event current = Event.current;
            if ((current.type == EventType.MouseDown) || (current.type == EventType.MouseDrag))
            {
                s_LastActiveSceneView = this;
            }
            else if (s_LastActiveSceneView == null)
            {
                s_LastActiveSceneView = this;
            }
            if (current.type == EventType.MouseDrag)
            {
                this.draggingLocked = DraggingLockedState.Dragging;
            }
            else if ((GUIUtility.hotControl == 0) && (this.draggingLocked == DraggingLockedState.Dragging))
            {
                this.draggingLocked = DraggingLockedState.LookAt;
            }
            if (current.type == EventType.MouseDown)
            {
                Tools.s_ButtonDown = current.button;
                if ((current.button == 1) && (Application.platform == RuntimePlatform.OSXEditor))
                {
                    base.Focus();
                }
            }
            else if ((current.type == EventType.MouseUp) && (Tools.s_ButtonDown == current.button))
            {
                Tools.s_ButtonDown = -1;
            }
        }

        private void HandleDragging()
        {
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.DragPerform:
                case EventType.DragUpdated:
                    this.CallEditorDragFunctions();
                    if (current.type != EventType.Used)
                    {
                        bool perform = current.type == EventType.DragPerform;
                        if (DragAndDrop.visualMode != DragAndDropVisualMode.Copy)
                        {
                            DragAndDrop.visualMode = InternalEditorUtility.SceneViewDrag(HandleUtility.PickGameObject(Event.current.mousePosition, true), this.pivot, Event.current.mousePosition, perform);
                        }
                        if (perform && (DragAndDrop.visualMode != DragAndDropVisualMode.None))
                        {
                            DragAndDrop.AcceptDrag();
                            current.Use();
                            GUIUtility.ExitGUI();
                        }
                        current.Use();
                    }
                    break;

                case EventType.DragExited:
                    this.CallEditorDragFunctions();
                    this.CleanupEditorDragFunctions();
                    break;
            }
        }

        private void HandleMouseCursor()
        {
            Event current = Event.current;
            if (GUIUtility.hotControl == 0)
            {
                this.s_DraggingCursorIsCached = false;
            }
            Rect position = new Rect(0f, 0f, base.position.width, base.position.height);
            if (!this.s_DraggingCursorIsCached)
            {
                MouseCursor arrow = MouseCursor.Arrow;
                if ((current.type == EventType.MouseMove) || (current.type == EventType.Repaint))
                {
                    foreach (CursorRect rect4 in s_MouseRects)
                    {
                        if (rect4.rect.Contains(current.mousePosition))
                        {
                            arrow = rect4.cursor;
                            position = rect4.rect;
                        }
                    }
                    if (GUIUtility.hotControl != 0)
                    {
                        this.s_DraggingCursorIsCached = true;
                    }
                    if (arrow != s_LastCursor)
                    {
                        s_LastCursor = arrow;
                        InternalEditorUtility.ResetCursor();
                        base.Repaint();
                    }
                }
            }
            if ((current.type == EventType.Repaint) && (s_LastCursor != MouseCursor.Arrow))
            {
                EditorGUIUtility.AddCursorRect(position, s_LastCursor);
            }
        }

        private void HandleSelectionAndOnSceneGUI()
        {
            this.m_RectSelection.OnGUI();
            this.CallOnSceneGUI();
        }

        private void HandleViewToolCursor()
        {
            if (Tools.viewToolActive && (Event.current.type == EventType.Repaint))
            {
                MouseCursor arrow = MouseCursor.Arrow;
                switch (Tools.viewTool)
                {
                    case ViewTool.Orbit:
                        arrow = MouseCursor.Orbit;
                        break;

                    case ViewTool.Pan:
                        arrow = MouseCursor.Pan;
                        break;

                    case ViewTool.Zoom:
                        arrow = MouseCursor.Zoom;
                        break;

                    case ViewTool.FPS:
                        arrow = MouseCursor.FPS;
                        break;
                }
                if (arrow != MouseCursor.Arrow)
                {
                    AddCursorRect(new Rect(0f, 17f, base.position.width, base.position.height - 17f), arrow);
                }
            }
        }

        private void InputForGizmosThatAreRenderedOnTopOfSceneView()
        {
            if (Event.current.type != EventType.Repaint)
            {
                this.svRot.OnGUI(this);
            }
        }

        internal bool IsSceneCameraDeferred() => 
            ((this.m_Camera?.actualRenderingPath == RenderingPath.DeferredLighting) || (this.m_Camera?.actualRenderingPath == RenderingPath.DeferredShading));

        internal static bool IsUsingDeferredRenderingPath()
        {
            RenderingPath sceneViewRenderingPath = GetSceneViewRenderingPath();
            return ((sceneViewRenderingPath == RenderingPath.DeferredShading) || ((sceneViewRenderingPath == RenderingPath.UsePlayerSettings) && (EditorGraphicsSettings.GetCurrentTierSettings().renderingPath == RenderingPath.DeferredShading)));
        }

        private void LoadRenderDoc()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                RenderDoc.Load();
                ShaderUtil.RecreateGfxDevice();
            }
        }

        public void LookAt(Vector3 pos)
        {
            this.FixNegativeSize();
            this.m_Position.target = pos;
        }

        public void LookAt(Vector3 pos, Quaternion rot)
        {
            this.FixNegativeSize();
            this.m_Position.target = pos;
            this.m_Rotation.target = rot;
            this.svRot.UpdateGizmoLabel(this, (Vector3) (rot * Vector3.forward), this.m_Ortho.target);
        }

        public void LookAt(Vector3 pos, Quaternion rot, float newSize)
        {
            this.FixNegativeSize();
            this.m_Position.target = pos;
            this.m_Rotation.target = rot;
            this.m_Size.target = Mathf.Abs(newSize);
            this.svRot.UpdateGizmoLabel(this, (Vector3) (rot * Vector3.forward), this.m_Ortho.target);
        }

        public void LookAt(Vector3 pos, Quaternion rot, float newSize, bool ortho)
        {
            this.LookAt(pos, rot, newSize, ortho, false);
        }

        public void LookAt(Vector3 pos, Quaternion rot, float newSize, bool ortho, bool instant)
        {
            this.FixNegativeSize();
            if (instant)
            {
                this.m_Position.value = pos;
                this.m_Rotation.value = rot;
                this.m_Size.value = Mathf.Abs(newSize);
                this.m_Ortho.value = ortho;
            }
            else
            {
                this.m_Position.target = pos;
                this.m_Rotation.target = rot;
                this.m_Size.target = Mathf.Abs(newSize);
                this.m_Ortho.target = ortho;
            }
            this.svRot.UpdateGizmoLabel(this, (Vector3) (rot * Vector3.forward), this.m_Ortho.target);
        }

        public void LookAtDirect(Vector3 pos, Quaternion rot)
        {
            this.FixNegativeSize();
            this.m_Position.value = pos;
            this.m_Rotation.value = rot;
            this.svRot.UpdateGizmoLabel(this, (Vector3) (rot * Vector3.forward), this.m_Ortho.target);
        }

        public void LookAtDirect(Vector3 pos, Quaternion rot, float newSize)
        {
            this.FixNegativeSize();
            this.m_Position.value = pos;
            this.m_Rotation.value = rot;
            this.m_Size.value = Mathf.Abs(newSize);
            this.svRot.UpdateGizmoLabel(this, (Vector3) (rot * Vector3.forward), this.m_Ortho.target);
        }

        [MenuItem("GameObject/Align View to Selected")]
        internal static void MenuAlignViewToSelected()
        {
            if (ValidateAlignViewToSelected())
            {
                s_LastActiveSceneView.AlignViewToObject(Selection.activeTransform);
            }
        }

        [MenuItem("GameObject/Align With View %#f")]
        internal static void MenuAlignWithView()
        {
            if (ValidateAlignWithView())
            {
                s_LastActiveSceneView.AlignWithView();
            }
        }

        [MenuItem("GameObject/Set as last sibling %-")]
        internal static void MenuMoveToBack()
        {
            foreach (Transform transform in Selection.transforms)
            {
                Undo.SetTransformParent(transform, transform.parent, "Set as last sibling");
                transform.SetAsLastSibling();
            }
        }

        [MenuItem("GameObject/Set as first sibling %=")]
        internal static void MenuMoveToFront()
        {
            foreach (Transform transform in Selection.transforms)
            {
                Undo.SetTransformParent(transform, transform.parent, "Set as first sibling");
                transform.SetAsFirstSibling();
            }
        }

        [MenuItem("GameObject/Move To View %&f")]
        internal static void MenuMoveToView()
        {
            if (ValidateMoveToView())
            {
                s_LastActiveSceneView.MoveToView();
            }
        }

        public void MoveToView()
        {
            this.FixNegativeSize();
            Vector3 vector = this.pivot - Tools.handlePosition;
            Undo.RecordObjects(Selection.transforms, "Move to view");
            foreach (Transform transform in Selection.transforms)
            {
                transform.position += vector;
            }
        }

        public void MoveToView(Transform target)
        {
            target.position = this.pivot;
        }

        private void On2DModeChange()
        {
            if (this.m_2DMode)
            {
                this.lastSceneViewRotation = this.rotation;
                this.m_LastSceneViewOrtho = this.orthographic;
                this.LookAt(this.pivot, Quaternion.identity, this.size, true);
                if (Tools.current == Tool.Move)
                {
                    Tools.current = Tool.Rect;
                }
            }
            else
            {
                this.LookAt(this.pivot, this.lastSceneViewRotation, this.size, this.m_LastSceneViewOrtho);
                if (Tools.current == Tool.Rect)
                {
                    Tools.current = Tool.Move;
                }
            }
            HandleUtility.ignoreRaySnapObjects = null;
            Tools.vertexDragging = false;
            Tools.handleOffset = Vector3.zero;
        }

        private void OnBecameInvisible()
        {
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateAnimatedMaterials));
        }

        private void OnBecameVisible()
        {
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateAnimatedMaterials));
        }

        public void OnDestroy()
        {
            if (this.m_AudioPlay)
            {
                this.m_AudioPlay = false;
                this.RefreshAudioPlay();
            }
        }

        public override void OnDisable()
        {
            if (<>f__mg$cache1 == null)
            {
                <>f__mg$cache1 = new EditorApplication.CallbackFunction(SceneView.RepaintAll);
            }
            EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.modifierKeysChanged, <>f__mg$cache1);
            if (this.m_Camera != null)
            {
                Object.DestroyImmediate(this.m_Camera.gameObject, true);
            }
            if (this.m_Light[0] != null)
            {
                Object.DestroyImmediate(this.m_Light[0].gameObject, true);
            }
            if (this.m_Light[1] != null)
            {
                Object.DestroyImmediate(this.m_Light[1].gameObject, true);
            }
            if (this.m_Light[2] != null)
            {
                Object.DestroyImmediate(this.m_Light[2].gameObject, true);
            }
            if (s_MipColorsTexture != null)
            {
                Object.DestroyImmediate(s_MipColorsTexture, true);
            }
            s_SceneViews.Remove(this);
            if (s_LastActiveSceneView == this)
            {
                if (s_SceneViews.Count > 0)
                {
                    s_LastActiveSceneView = s_SceneViews[0] as SceneView;
                }
                else
                {
                    s_LastActiveSceneView = null;
                }
            }
            this.CleanupEditorDragFunctions();
            base.OnDisable();
        }

        public override void OnEnable()
        {
            base.titleContent = base.GetLocalizedTitleContent();
            this.m_RectSelection = new RectSelection(this);
            if (this.grid == null)
            {
                this.grid = new SceneViewGrid();
            }
            this.grid.Register(this);
            if (this.svRot == null)
            {
                this.svRot = new SceneViewRotation();
            }
            this.svRot.Register(this);
            base.autoRepaintOnSceneChange = true;
            this.m_Rotation.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_Position.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_Size.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_Ortho.valueChanged.AddListener(new UnityAction(this.Repaint));
            base.wantsMouseMove = true;
            base.dontClearBackground = true;
            s_SceneViews.Add(this);
            this.m_Lighting = EditorGUIUtility.IconContent("SceneviewLighting", "Lighting|The scene lighting is used when toggled on. When toggled off a light attached to the scene view camera is used.");
            this.m_Fx = EditorGUIUtility.IconContent("SceneviewFx", "Fx|Toggles skybox, fog and lens flare effects.");
            this.m_AudioPlayContent = EditorGUIUtility.IconContent("SceneviewAudio", "AudioPlay|Toggles audio on or off.");
            this.m_GizmosContent = new GUIContent("Gizmos");
            this.m_2DModeContent = new GUIContent("2D");
            this.m_RenderDocContent = EditorGUIUtility.IconContent("renderdoc", "Capture|Capture the current view and open in RenderDoc");
            this.m_SceneViewOverlay = new SceneViewOverlay(this);
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new EditorApplication.CallbackFunction(SceneView.RepaintAll);
            }
            EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.modifierKeysChanged, <>f__mg$cache0);
            this.m_DraggingLockedState = DraggingLockedState.NotDragging;
            this.CreateSceneCameraAndLights();
            if (this.m_2DMode)
            {
                this.LookAt(this.pivot, Quaternion.identity, this.size, true, true);
            }
            base.OnEnable();
        }

        internal void OnFocus()
        {
            if ((!Application.isPlaying && this.m_AudioPlay) && (this.m_Camera != null))
            {
                this.RefreshAudioPlay();
            }
        }

        internal void OnGUI()
        {
            bool flag;
            float num;
            bool flag2;
            s_CurrentDrawingSceneView = this;
            Event current = Event.current;
            if (current.type == EventType.Repaint)
            {
                s_MouseRects.Clear();
                Profiler.BeginSample("SceneView.Repaint");
            }
            Color color = GUI.color;
            this.HandleClickAndDragToFocus();
            if (current.type == EventType.Layout)
            {
                this.m_ShowSceneViewWindows = lastActiveSceneView == this;
            }
            this.m_SceneViewOverlay.Begin();
            this.SetupFogAndShadowDistance(out flag, out num);
            this.DoToolbarGUI();
            GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
            GUI.color = Color.white;
            EditorGUIUtility.labelWidth = 100f;
            this.SetupCamera();
            RenderingPath renderingPath = this.m_Camera.renderingPath;
            this.SetupCustomSceneLighting();
            GUI.BeginGroup(new Rect(0f, 17f, base.position.width, base.position.height - 17f));
            Rect rect = new Rect(0f, 0f, base.position.width, base.position.height - 17f);
            Rect cameraRect = EditorGUIUtility.PointsToPixels(rect);
            this.HandleViewToolCursor();
            this.PrepareCameraTargetTexture(cameraRect);
            this.DoClearCamera(cameraRect);
            this.m_Camera.cullingMask = Tools.visibleLayers;
            this.InputForGizmosThatAreRenderedOnTopOfSceneView();
            this.DoOnPreSceneGUICallbacks(cameraRect);
            this.PrepareCameraReplacementShader();
            this.m_MainViewControlID = GUIUtility.GetControlID(FocusType.Keyboard);
            if (current.GetTypeForControl(this.m_MainViewControlID) == EventType.MouseDown)
            {
                GUIUtility.keyboardControl = this.m_MainViewControlID;
            }
            this.DoDrawCamera(rect, out flag2);
            this.CleanupCustomSceneLighting();
            if (!this.UseSceneFiltering())
            {
                Handles.DrawCameraStep2(this.m_Camera, this.m_RenderMode);
                this.HandleSelectionAndOnSceneGUI();
            }
            if ((current.type == EventType.ExecuteCommand) || (current.type == EventType.ValidateCommand))
            {
                this.CommandsGUI();
            }
            this.RestoreFogAndShadowDistance(flag, num);
            this.m_Camera.renderingPath = renderingPath;
            if (this.UseSceneFiltering())
            {
                Handles.SetCameraFilterMode(Camera.current, Handles.FilterMode.ShowFiltered);
            }
            else
            {
                Handles.SetCameraFilterMode(Camera.current, Handles.FilterMode.Off);
            }
            this.DefaultHandles();
            if (!this.UseSceneFiltering())
            {
                if (current.type == EventType.Repaint)
                {
                    Profiler.BeginSample("SceneView.BlitRT");
                    Graphics.SetRenderTarget((RenderTexture) null);
                }
                if (flag2)
                {
                    GUIClip.Pop();
                }
                if (current.type == EventType.Repaint)
                {
                    GL.sRGBWrite = QualitySettings.activeColorSpace == ColorSpace.Linear;
                    Graphics.DrawTexture(rect, this.m_SceneTargetTexture, new Rect(0f, 0f, 1f, 1f), 0, 0, 0, 0, GUI.color, EditorGUIUtility.GUITextureBlitColorspaceMaterial);
                    GL.sRGBWrite = false;
                    Profiler.EndSample();
                }
            }
            Handles.SetCameraFilterMode(Camera.current, Handles.FilterMode.Off);
            Handles.SetCameraFilterMode(this.m_Camera, Handles.FilterMode.Off);
            this.HandleDragging();
            this.RepaintGizmosThatAreRenderedOnTopOfSceneView();
            if (s_LastActiveSceneView == this)
            {
                SceneViewMotion.ArrowKeys(this);
                SceneViewMotion.DoViewTool(this);
            }
            this.Handle2DModeSwitch();
            GUI.EndGroup();
            GUI.color = color;
            this.m_SceneViewOverlay.End();
            this.HandleMouseCursor();
            if (current.type == EventType.Repaint)
            {
                Profiler.EndSample();
            }
            s_CurrentDrawingSceneView = null;
        }

        internal void OnLostFocus()
        {
            GameView view = (GameView) WindowLayout.FindEditorWindowOfType(typeof(GameView));
            if (((view != null) && (view.m_Parent != null)) && ((base.m_Parent != null) && (view.m_Parent == base.m_Parent)))
            {
                view.m_Parent.backgroundValid = false;
            }
            if (s_LastActiveSceneView == this)
            {
                SceneViewMotion.ResetMotion();
            }
        }

        internal void OnNewProjectLayoutWasCreated()
        {
            this.ResetToDefaults(EditorSettings.defaultBehaviorMode);
        }

        public void OnSelectionChange()
        {
            if ((Selection.activeObject != null) && (this.m_LastLockedObject != Selection.activeObject))
            {
                this.viewIsLockedToObject = false;
            }
            base.Repaint();
        }

        internal static void PlaceGameObjectInFrontOfSceneView(GameObject go)
        {
            if (s_SceneViews.Count >= 1)
            {
                SceneView view = s_LastActiveSceneView;
                if (view == null)
                {
                    view = s_SceneViews[0] as SceneView;
                }
                if (view != null)
                {
                    view.MoveToView(go.transform);
                }
            }
        }

        private void PrepareCameraReplacementShader()
        {
            if (Event.current.type == EventType.Repaint)
            {
                Handles.SetSceneViewColors((Color) kSceneViewWire, (Color) kSceneViewWireOverlay, (Color) kSceneViewSelectedOutline, (Color) kSceneViewSelectedWire);
                if (this.m_RenderMode == DrawCameraMode.Overdraw)
                {
                    if (s_ShowOverdrawShader == null)
                    {
                        s_ShowOverdrawShader = EditorGUIUtility.LoadRequired("SceneView/SceneViewShowOverdraw.shader") as Shader;
                    }
                    this.m_Camera.SetReplacementShader(s_ShowOverdrawShader, "RenderType");
                }
                else if (this.m_RenderMode == DrawCameraMode.Mipmaps)
                {
                    if (s_ShowMipsShader == null)
                    {
                        s_ShowMipsShader = EditorGUIUtility.LoadRequired("SceneView/SceneViewShowMips.shader") as Shader;
                    }
                    if ((s_ShowMipsShader != null) && s_ShowMipsShader.isSupported)
                    {
                        CreateMipColorsTexture();
                        this.m_Camera.SetReplacementShader(s_ShowMipsShader, "RenderType");
                    }
                    else
                    {
                        this.m_Camera.SetReplacementShader(this.m_ReplacementShader, this.m_ReplacementString);
                    }
                }
                else
                {
                    this.m_Camera.SetReplacementShader(this.m_ReplacementShader, this.m_ReplacementString);
                }
            }
        }

        private void PrepareCameraTargetTexture(Rect cameraRect)
        {
            bool hdr = this.SceneViewIsRenderingHDR();
            this.CreateCameraTargetTexture(cameraRect, hdr);
            this.m_Camera.targetTexture = this.m_SceneTargetTexture;
            if ((this.UseSceneFiltering() || !DoesCameraDrawModeSupportDeferred(this.m_RenderMode)) && this.IsSceneCameraDeferred())
            {
                this.m_Camera.renderingPath = RenderingPath.Forward;
            }
        }

        private void RefreshAudioPlay()
        {
            if (((s_AudioSceneView != null) && (s_AudioSceneView != this)) && s_AudioSceneView.m_AudioPlay)
            {
                s_AudioSceneView.m_AudioPlay = false;
                s_AudioSceneView.Repaint();
            }
            AudioSource[] sourceArray = (AudioSource[]) Object.FindObjectsOfType(typeof(AudioSource));
            foreach (AudioSource source in sourceArray)
            {
                if (source.playOnAwake)
                {
                    if (!this.m_AudioPlay)
                    {
                        source.Stop();
                    }
                    else if (!source.isPlaying)
                    {
                        source.Play();
                    }
                }
            }
            AudioUtil.SetListenerTransform(!this.m_AudioPlay ? null : this.m_Camera.transform);
            s_AudioSceneView = this;
        }

        public static void RepaintAll()
        {
            IEnumerator enumerator = s_SceneViews.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ((SceneView) enumerator.Current).Repaint();
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

        private void RepaintGizmosThatAreRenderedOnTopOfSceneView()
        {
            this.svRot.OnGUI(this);
        }

        internal static void Report2DAnalytics()
        {
            Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(SceneView));
            if (objArray.Length == 1)
            {
                SceneView view = objArray[0] as SceneView;
                if (view.in2DMode)
                {
                    UsabilityAnalytics.Event("2D", "SceneView", "Single 2D", 1);
                }
            }
        }

        private void ResetIfNaN()
        {
            if (float.IsInfinity(this.m_Position.value.x) || float.IsNaN(this.m_Position.value.x))
            {
                this.m_Position.value = Vector3.zero;
            }
            if (float.IsInfinity(this.m_Rotation.value.x) || float.IsNaN(this.m_Rotation.value.x))
            {
                this.m_Rotation.value = Quaternion.identity;
            }
        }

        private void ResetOnSceneGUIState()
        {
            Handles.ClearHandles();
            HandleUtility.s_CustomPickDistance = 5f;
            EditorGUIUtility.ResetGUIState();
            GUI.color = Color.white;
        }

        private void ResetToDefaults(EditorBehaviorMode behaviorMode)
        {
            if (behaviorMode != EditorBehaviorMode.Mode2D)
            {
                if (behaviorMode == EditorBehaviorMode.Mode3D)
                {
                }
            }
            else
            {
                this.m_2DMode = true;
                this.m_Rotation.value = Quaternion.identity;
                this.m_Position.value = kDefaultPivot;
                this.m_Size.value = 10f;
                this.m_Ortho.value = true;
                this.m_LastSceneViewRotation = kDefaultRotation;
                this.m_LastSceneViewOrtho = false;
                return;
            }
            this.m_2DMode = false;
            this.m_Rotation.value = kDefaultRotation;
            this.m_Position.value = kDefaultPivot;
            this.m_Size.value = 10f;
            this.m_Ortho.value = false;
        }

        private void RestoreFogAndShadowDistance(bool oldFog, float oldShadowDistance)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Unsupported.SetRenderSettingsUseFogNoDirty(oldFog);
                Unsupported.SetQualitySettingsShadowDistanceTemporarily(oldShadowDistance);
            }
        }

        private bool SceneCameraRendersIntoRT() => 
            (this.m_Camera.targetTexture != null);

        internal bool SceneViewIsRenderingHDR()
        {
            if (this.UseSceneFiltering())
            {
                return false;
            }
            return ((this.m_Camera != null) && this.m_Camera.allowHDR);
        }

        private void SetSceneCameraHDRAndDepthModes()
        {
            if (!this.m_SceneLighting || !DoesCameraDrawModeSupportHDR(this.m_RenderMode))
            {
                this.m_Camera.allowHDR = false;
                this.m_Camera.depthTextureMode = DepthTextureMode.None;
                this.m_Camera.clearStencilAfterLightingPass = false;
            }
            else
            {
                Camera mainCamera = GetMainCamera();
                if (mainCamera == null)
                {
                    this.m_Camera.allowHDR = false;
                    this.m_Camera.depthTextureMode = DepthTextureMode.None;
                    this.m_Camera.clearStencilAfterLightingPass = false;
                }
                else
                {
                    this.m_Camera.allowHDR = mainCamera.allowHDR;
                    this.m_Camera.depthTextureMode = mainCamera.depthTextureMode;
                    this.m_Camera.clearStencilAfterLightingPass = mainCamera.clearStencilAfterLightingPass;
                }
            }
        }

        public void SetSceneViewFiltering(bool enable)
        {
            this.m_RequestedSceneViewFiltering = enable;
        }

        public void SetSceneViewShaderReplace(Shader shader, string replaceString)
        {
            this.m_ReplacementShader = shader;
            this.m_ReplacementString = replaceString;
        }

        internal override void SetSearchFilter(string searchFilter, SearchableEditorWindow.SearchMode mode, bool setAll)
        {
            if ((base.m_SearchFilter == "") || (searchFilter == ""))
            {
                this.m_StartSearchFilterTime = EditorApplication.timeSinceStartup;
            }
            base.SetSearchFilter(searchFilter, mode, setAll);
        }

        private void SetupCamera()
        {
            if (this.m_RenderMode == DrawCameraMode.Overdraw)
            {
                this.m_Camera.backgroundColor = Color.black;
            }
            else
            {
                this.m_Camera.backgroundColor = (Color) kSceneViewBackground;
            }
            if (Event.current.type == EventType.Repaint)
            {
                this.UpdateImageEffects(!this.UseSceneFiltering() ? ((this.m_RenderMode == DrawCameraMode.Textured) && this.m_SceneViewState.showImageEffects) : false);
            }
            EditorUtility.SetCameraAnimateMaterials(this.m_Camera, this.m_SceneViewState.showMaterialUpdate);
            this.ResetIfNaN();
            this.m_Camera.transform.rotation = this.m_Rotation.value;
            float aspectNeutralFOV = this.m_Ortho.Fade(90f, 0f);
            if (aspectNeutralFOV > 3f)
            {
                this.m_Camera.orthographic = false;
                this.m_Camera.fieldOfView = this.GetVerticalFOV(aspectNeutralFOV);
            }
            else
            {
                this.m_Camera.orthographic = true;
                this.m_Camera.orthographicSize = this.GetVerticalOrthoSize();
            }
            this.m_Camera.transform.position = this.m_Position.value + (this.m_Camera.transform.rotation * new Vector3(0f, 0f, -this.cameraDistance));
            float num2 = Mathf.Max((float) 1000f, (float) (2000f * this.size));
            this.m_Camera.nearClipPlane = num2 * 5E-06f;
            this.m_Camera.farClipPlane = num2;
            this.m_Camera.renderingPath = GetSceneViewRenderingPath();
            if (!this.CheckDrawModeForRenderingPath(this.m_RenderMode))
            {
                this.m_RenderMode = DrawCameraMode.Textured;
            }
            this.SetSceneCameraHDRAndDepthModes();
            if ((this.m_RenderMode == DrawCameraMode.Textured) || (this.m_RenderMode == DrawCameraMode.TexturedWire))
            {
                Handles.EnableCameraFlares(this.m_Camera, this.m_SceneViewState.showFlares);
                Handles.EnableCameraSkybox(this.m_Camera, this.m_SceneViewState.showSkybox);
            }
            else
            {
                Handles.EnableCameraFlares(this.m_Camera, false);
                Handles.EnableCameraSkybox(this.m_Camera, false);
            }
            this.m_Light[0].transform.position = this.m_Camera.transform.position;
            this.m_Light[0].transform.rotation = this.m_Camera.transform.rotation;
            if (this.m_AudioPlay)
            {
                AudioUtil.SetListenerTransform(this.m_Camera.transform);
                AudioUtil.UpdateAudio();
            }
            if (this.m_ViewIsLockedToObject && (Selection.gameObjects.Length > 0))
            {
                switch (this.m_DraggingLockedState)
                {
                    case DraggingLockedState.LookAt:
                        if (!this.m_Position.value.Equals(Selection.activeGameObject.transform.position))
                        {
                            if (!EditorApplication.isPlaying)
                            {
                                this.m_Position.target = Selection.activeGameObject.transform.position;
                            }
                            else
                            {
                                this.m_Position.value = Selection.activeGameObject.transform.position;
                            }
                        }
                        else
                        {
                            this.m_DraggingLockedState = DraggingLockedState.NotDragging;
                        }
                        break;

                    case DraggingLockedState.NotDragging:
                        this.m_Position.value = Selection.activeGameObject.transform.position;
                        break;
                }
            }
        }

        private void SetupCustomSceneLighting()
        {
            if (!this.m_SceneLighting)
            {
                this.m_Light[0].transform.rotation = this.m_Camera.transform.rotation;
                if (Event.current.type == EventType.Repaint)
                {
                    InternalEditorUtility.SetCustomLighting(this.m_Light, kSceneViewMidLight);
                }
            }
        }

        private void SetupFogAndShadowDistance(out bool oldFog, out float oldShadowDistance)
        {
            oldFog = RenderSettings.fog;
            oldShadowDistance = QualitySettings.shadowDistance;
            if (Event.current.type == EventType.Repaint)
            {
                if (!this.m_SceneViewState.showFog)
                {
                    Unsupported.SetRenderSettingsUseFogNoDirty(false);
                }
                if (this.m_Camera.orthographic)
                {
                    Unsupported.SetQualitySettingsShadowDistanceTemporarily(QualitySettings.shadowDistance + (0.5f * this.cameraDistance));
                }
            }
        }

        private void SetupPBRValidation()
        {
            if (this.m_RenderMode == DrawCameraMode.ValidateAlbedo)
            {
                this.CreateAlbedoSwatchData();
                this.UpdateAlbedoSwatch();
            }
            if (((this.m_RenderMode == DrawCameraMode.ValidateAlbedo) || (this.m_RenderMode == DrawCameraMode.ValidateMetalSpecular)) && ((this.lastRenderMode != DrawCameraMode.ValidateAlbedo) && (this.lastRenderMode != DrawCameraMode.ValidateMetalSpecular)))
            {
                onSceneGUIDelegate = (OnSceneFunc) Delegate.Combine(onSceneGUIDelegate, new OnSceneFunc(this.DrawValidateAlbedoSwatches));
            }
            else if (((this.m_RenderMode != DrawCameraMode.ValidateAlbedo) && (this.m_RenderMode != DrawCameraMode.ValidateMetalSpecular)) && ((this.lastRenderMode == DrawCameraMode.ValidateAlbedo) || (this.lastRenderMode == DrawCameraMode.ValidateMetalSpecular)))
            {
                onSceneGUIDelegate = (OnSceneFunc) Delegate.Remove(onSceneGUIDelegate, new OnSceneFunc(this.DrawValidateAlbedoSwatches));
            }
            this.lastRenderMode = this.m_RenderMode;
        }

        public static void ShowCompileErrorNotification()
        {
            ShowNotification("All compiler errors have to be fixed before you can enter playmode!");
        }

        internal static void ShowNotification(string notificationText)
        {
            Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(SceneView));
            List<EditorWindow> list = new List<EditorWindow>();
            foreach (SceneView view in objArray)
            {
                if (view.m_Parent is DockArea)
                {
                    DockArea parent = (DockArea) view.m_Parent;
                    if ((parent != null) && (parent.actualView == view))
                    {
                        list.Add(view);
                    }
                }
            }
            if (list.Count > 0)
            {
                foreach (EditorWindow window in list)
                {
                    window.ShowNotification(GUIContent.Temp(notificationText));
                }
            }
            else
            {
                Debug.LogError(notificationText);
            }
        }

        internal static void ShowSceneViewPlayModeSaveWarning()
        {
            GameView view = (GameView) WindowLayout.FindEditorWindowOfType(typeof(GameView));
            if ((view != null) && view.hasFocus)
            {
                view.ShowNotification(new GUIContent("You must exit play mode to save the scene!"));
            }
            else
            {
                ShowNotification("You must exit play mode to save the scene!");
            }
        }

        private void UpdateAlbedoSwatch()
        {
            Color gray = Color.gray;
            if (this.m_SelectedAlbedoSwatchIndex != 0)
            {
                gray = this.m_AlbedoSwatchInfos[this.m_SelectedAlbedoSwatchIndex - 1].color;
                Shader.SetGlobalFloat("_AlbedoMinLuminance", this.m_AlbedoSwatchInfos[this.m_SelectedAlbedoSwatchIndex - 1].minLuminance);
                Shader.SetGlobalFloat("_AlbedoMaxLuminance", this.m_AlbedoSwatchInfos[this.m_SelectedAlbedoSwatchIndex - 1].maxLuminance);
                Shader.SetGlobalFloat("_AlbedoHueTolerance", this.m_AlbedoSwatchHueTolerance);
                Shader.SetGlobalFloat("_AlbedoSaturationTolerance", this.m_AlbedoSwatchSaturationTolerance);
            }
            Shader.SetGlobalColor("_AlbedoCompareColor", gray.linear);
            Shader.SetGlobalInt("_CheckAlbedo", (this.m_SelectedAlbedoSwatchIndex == 0) ? 0 : 1);
            Shader.SetGlobalInt("_CheckPureMetal", !this.m_ValidateTrueMetals ? 0 : 1);
        }

        private void UpdateAlbedoSwatchGUI()
        {
            this.m_LastKnownColorSpace = PlayerSettings.colorSpace;
            this.m_AlbedoSwatchColorStyles = new GUIStyle[this.m_AlbedoSwatchInfos.Length + 1];
            this.m_AlbedoSwatchGUIContent = new GUIContent[this.m_AlbedoSwatchInfos.Length + 1];
            this.m_AlbedoSwatchDescriptions = new string[this.m_AlbedoSwatchInfos.Length + 1];
            this.m_AlbedoSwatchLuminanceStrings = new string[this.m_AlbedoSwatchInfos.Length + 1];
            this.m_AlbedoSwatchColorStyles[0] = this.CreateSwatchStyleForColor(Color.gray);
            this.m_AlbedoSwatchDescriptions[0] = "Default Luminance";
            this.m_AlbedoSwatchGUIContent[0] = new GUIContent(this.m_AlbedoSwatchDescriptions[0]);
            this.m_AlbedoSwatchLuminanceStrings[0] = this.CreateSwatchDescriptionForName(0.012f, 0.9f);
            for (int i = 1; i < (this.m_AlbedoSwatchInfos.Length + 1); i++)
            {
                this.m_AlbedoSwatchColorStyles[i] = this.CreateSwatchStyleForColor(this.m_AlbedoSwatchInfos[i - 1].color);
                this.m_AlbedoSwatchDescriptions[i] = this.m_AlbedoSwatchInfos[i - 1].name;
                this.m_AlbedoSwatchGUIContent[i] = new GUIContent(this.m_AlbedoSwatchDescriptions[i]);
                this.m_AlbedoSwatchLuminanceStrings[i] = this.CreateSwatchDescriptionForName(this.m_AlbedoSwatchInfos[i - 1].minLuminance, this.m_AlbedoSwatchInfos[i - 1].maxLuminance);
            }
        }

        private void UpdateAnimatedMaterials()
        {
            if (this.m_SceneViewState.showMaterialUpdate && ((this.m_lastRenderedTime + 0.032999999821186066) < EditorApplication.timeSinceStartup))
            {
                this.m_lastRenderedTime = EditorApplication.timeSinceStartup;
                base.Repaint();
            }
        }

        private void UpdateImageEffects(bool enable)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Camera mainCamera = GetMainCamera();
                if (!enable || (mainCamera == null))
                {
                    if (<>f__mg$cache2 == null)
                    {
                        <>f__mg$cache2 = new ComponentUtility.IsDesiredComponent(SceneView.ComponentHasImageEffectAttribute);
                    }
                    ComponentUtility.DestroyComponentsMatching(this.m_Camera.gameObject, <>f__mg$cache2);
                }
                else
                {
                    if (<>f__mg$cache3 == null)
                    {
                        <>f__mg$cache3 = new ComponentUtility.IsDesiredComponent(SceneView.ComponentHasImageEffectAttribute);
                    }
                    ComponentUtility.ReplaceComponentsIfDifferent(mainCamera.gameObject, this.m_Camera.gameObject, <>f__mg$cache3);
                }
            }
        }

        private bool UseSceneFiltering() => 
            (!string.IsNullOrEmpty(base.m_SearchFilter) || this.m_RequestedSceneViewFiltering);

        [MenuItem("GameObject/Toggle Active State &#a", true)]
        internal static bool ValidateActivateSelection() => 
            (Selection.activeTransform != null);

        [MenuItem("GameObject/Align View to Selected", true)]
        internal static bool ValidateAlignViewToSelected() => 
            ((s_LastActiveSceneView != null) && (Selection.activeTransform != null));

        [MenuItem("GameObject/Align With View %#f", true)]
        internal static bool ValidateAlignWithView() => 
            ((s_LastActiveSceneView != null) && (Selection.activeTransform != null));

        [MenuItem("GameObject/Set as last sibling %-", true)]
        internal static bool ValidateMenuMoveToBack()
        {
            if (Selection.activeTransform != null)
            {
                Transform parent = Selection.activeTransform.parent;
                return ((parent != null) && (parent.GetChild(parent.childCount - 1) != Selection.activeTransform));
            }
            return false;
        }

        [MenuItem("GameObject/Set as first sibling %=", true)]
        internal static bool ValidateMenuMoveToFront()
        {
            if (Selection.activeTransform != null)
            {
                Transform parent = Selection.activeTransform.parent;
                return ((parent != null) && (parent.GetChild(0) != Selection.activeTransform));
            }
            return false;
        }

        [MenuItem("GameObject/Move To View %&f", true)]
        private static bool ValidateMoveToView() => 
            ((s_LastActiveSceneView != null) && (Selection.transforms.Length != 0));

        public Camera camera =>
            this.m_Camera;

        internal float cameraDistance
        {
            get
            {
                float num = this.m_Ortho.Fade(90f, 0f);
                if (!this.camera.orthographic)
                {
                    return (this.size / Mathf.Tan((num * 0.5f) * 0.01745329f));
                }
                return (this.size * 2f);
            }
        }

        internal Vector3 cameraTargetPosition =>
            (this.m_Position.target + (this.m_Rotation.target * new Vector3(0f, 0f, this.cameraDistance)));

        internal Quaternion cameraTargetRotation =>
            this.m_Rotation.target;

        public static SceneView currentDrawingSceneView =>
            s_CurrentDrawingSceneView;

        internal DraggingLockedState draggingLocked
        {
            get => 
                this.m_DraggingLockedState;
            set
            {
                this.m_DraggingLockedState = value;
            }
        }

        private GUIStyle effectsDropDownStyle
        {
            get
            {
                if (s_DropDownStyle == null)
                {
                    s_DropDownStyle = "GV Gizmo DropDown";
                }
                return s_DropDownStyle;
            }
        }

        public bool in2DMode
        {
            get => 
                this.m_2DMode;
            set
            {
                if (((this.m_2DMode != value) && (Tools.viewTool != ViewTool.FPS)) && (Tools.viewTool != ViewTool.Orbit))
                {
                    this.m_2DMode = value;
                    this.On2DModeChange();
                }
            }
        }

        public bool isRotationLocked
        {
            get => 
                this.m_isRotationLocked;
            set
            {
                if (this.m_isRotationLocked != value)
                {
                    this.m_isRotationLocked = value;
                }
            }
        }

        public static SceneView lastActiveSceneView =>
            s_LastActiveSceneView;

        public Quaternion lastSceneViewRotation
        {
            get
            {
                if (this.m_LastSceneViewRotation == new Quaternion(0f, 0f, 0f, 0f))
                {
                    this.m_LastSceneViewRotation = Quaternion.identity;
                }
                return this.m_LastSceneViewRotation;
            }
            set
            {
                this.m_LastSceneViewRotation = value;
            }
        }

        public bool orthographic
        {
            get => 
                this.m_Ortho.value;
            set
            {
                this.m_Ortho.value = value;
            }
        }

        public Vector3 pivot
        {
            get => 
                this.m_Position.value;
            set
            {
                this.m_Position.value = value;
            }
        }

        public DrawCameraMode renderMode
        {
            get => 
                this.m_RenderMode;
            set
            {
                this.m_RenderMode = value;
                this.SetupPBRValidation();
            }
        }

        public Quaternion rotation
        {
            get => 
                this.m_Rotation.value;
            set
            {
                this.m_Rotation.value = value;
            }
        }

        public static ArrayList sceneViews =>
            s_SceneViews;

        public float size
        {
            get => 
                this.m_Size.value;
            set
            {
                if (value > 40000f)
                {
                    value = 40000f;
                }
                this.m_Size.value = value;
            }
        }

        internal bool viewIsLockedToObject
        {
            get => 
                this.m_ViewIsLockedToObject;
            set
            {
                if (value)
                {
                    this.m_LastLockedObject = Selection.activeObject;
                }
                else
                {
                    this.m_LastLockedObject = null;
                }
                this.m_ViewIsLockedToObject = value;
                this.draggingLocked = DraggingLockedState.LookAt;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CursorRect
        {
            public Rect rect;
            public MouseCursor cursor;
            public CursorRect(Rect rect, MouseCursor cursor)
            {
                this.rect = rect;
                this.cursor = cursor;
            }
        }

        internal enum DraggingLockedState
        {
            NotDragging,
            Dragging,
            LookAt
        }

        public delegate void OnSceneFunc(SceneView sceneView);

        [Serializable]
        public class SceneViewState
        {
            public bool showFlares;
            public bool showFog;
            public bool showImageEffects;
            public bool showMaterialUpdate;
            public bool showSkybox;

            public SceneViewState()
            {
                this.showFog = true;
                this.showMaterialUpdate = false;
                this.showSkybox = true;
                this.showFlares = true;
                this.showImageEffects = true;
            }

            public SceneViewState(SceneView.SceneViewState other)
            {
                this.showFog = true;
                this.showMaterialUpdate = false;
                this.showSkybox = true;
                this.showFlares = true;
                this.showImageEffects = true;
                this.showFog = other.showFog;
                this.showMaterialUpdate = other.showMaterialUpdate;
                this.showSkybox = other.showSkybox;
                this.showFlares = other.showFlares;
                this.showImageEffects = other.showImageEffects;
            }

            public bool IsAllOn() => 
                (((this.showFog && this.showMaterialUpdate) && (this.showSkybox && this.showFlares)) && this.showImageEffects);

            public void Toggle(bool value)
            {
                this.showFog = value;
                this.showMaterialUpdate = value;
                this.showSkybox = value;
                this.showFlares = value;
                this.showImageEffects = value;
            }
        }
    }
}

