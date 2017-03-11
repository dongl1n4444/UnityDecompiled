namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEditor.AnimatedValues;
    using UnityEditor.Modules;
    using UnityEditor.Rendering;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    [CanEditMultipleObjects, CustomEditor(typeof(Camera))]
    internal class CameraEditor : Editor
    {
        [CompilerGenerated]
        private static Handles.CapFunction <>f__mg$cache0;
        private static readonly GUIContent[] kCameraRenderPaths = new GUIContent[] { new GUIContent("Use Graphics Settings"), new GUIContent("Forward"), new GUIContent("Deferred"), new GUIContent("Legacy Vertex Lit"), new GUIContent("Legacy Deferred (light prepass)") };
        private static readonly int[] kCameraRenderPathValues = new int[] { -1, 1, 3, 0, 2 };
        private static readonly Color kGizmoCamera = new Color(0.9137255f, 0.9137255f, 0.9137255f, 0.5019608f);
        private const float kPreviewNormalizedSize = 0.2f;
        private const float kPreviewWindowOffset = 10f;
        private static readonly GUIContent[] kTargetEyes = new GUIContent[] { new GUIContent("Both"), new GUIContent("Left"), new GUIContent("Right"), new GUIContent("None (Main Display)") };
        private static readonly int[] kTargetEyeValues = new int[] { 3, 1, 2, 0 };
        private SerializedProperty m_BackgroundColor;
        private SerializedProperty m_ClearFlags;
        private bool m_CommandBuffersShown = true;
        private SerializedProperty m_CullingMask;
        private SerializedProperty m_Depth;
        private SerializedProperty m_FieldOfView;
        private SerializedProperty m_HDR;
        private SerializedProperty[] m_NearAndFarClippingPlanes;
        private SerializedProperty m_NormalizedViewPortRect;
        private SerializedProperty m_OcclusionCulling;
        private SerializedProperty m_Orthographic;
        private SerializedProperty m_OrthographicSize;
        private Camera m_PreviewCamera;
        private SerializedProperty m_RenderingPath;
        private readonly AnimBool m_ShowBGColorOptions = new AnimBool();
        private readonly AnimBool m_ShowOrthoOptions = new AnimBool();
        private readonly AnimBool m_ShowTargetEyeOption = new AnimBool();
        private SerializedProperty m_StereoConvergence;
        private SerializedProperty m_StereoSeparation;
        private SerializedProperty m_TargetDisplay;
        private SerializedProperty m_TargetEye;
        private SerializedProperty m_TargetTexture;
        private readonly GUIContent m_ViewportLabel = new GUIContent("Viewport Rect");

        private void CommandBufferGUI()
        {
            if (base.targets.Length == 1)
            {
                Camera target = base.target as Camera;
                if (target != null)
                {
                    int commandBufferCount = target.commandBufferCount;
                    if (commandBufferCount != 0)
                    {
                        this.m_CommandBuffersShown = GUILayout.Toggle(this.m_CommandBuffersShown, GUIContent.Temp(commandBufferCount + " command buffers"), EditorStyles.foldout, new GUILayoutOption[0]);
                        if (this.m_CommandBuffersShown)
                        {
                            EditorGUI.indentLevel++;
                            foreach (CameraEvent event2 in (CameraEvent[]) Enum.GetValues(typeof(CameraEvent)))
                            {
                                CommandBuffer[] commandBuffers = target.GetCommandBuffers(event2);
                                foreach (CommandBuffer buffer in commandBuffers)
                                {
                                    using (new GUILayout.HorizontalScope(new GUILayoutOption[0]))
                                    {
                                        Rect r = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.miniLabel);
                                        r.xMin += EditorGUI.indent;
                                        Rect removeButtonRect = GetRemoveButtonRect(r);
                                        r.xMax = removeButtonRect.x;
                                        GUI.Label(r, $"{event2}: {buffer.name} ({EditorUtility.FormatBytes(buffer.sizeInBytes)})", EditorStyles.miniLabel);
                                        if (GUI.Button(removeButtonRect, Styles.iconRemove, Styles.invisibleButton))
                                        {
                                            target.RemoveCommandBuffer(event2, buffer);
                                            SceneView.RepaintAll();
                                            GameView.RepaintAll();
                                            GUIUtility.ExitGUI();
                                        }
                                    }
                                }
                            }
                            using (new GUILayout.HorizontalScope(new GUILayoutOption[0]))
                            {
                                GUILayout.FlexibleSpace();
                                if (GUILayout.Button("Remove all", EditorStyles.miniButton, new GUILayoutOption[0]))
                                {
                                    target.RemoveAllCommandBuffers();
                                    SceneView.RepaintAll();
                                    GameView.RepaintAll();
                                }
                            }
                            EditorGUI.indentLevel--;
                        }
                    }
                }
            }
        }

        private void DepthTextureModeGUI()
        {
            if (base.targets.Length == 1)
            {
                Camera target = base.target as Camera;
                if ((target != null) && (target.depthTextureMode != DepthTextureMode.None))
                {
                    List<string> list = new List<string>();
                    if ((target.depthTextureMode & DepthTextureMode.Depth) != DepthTextureMode.None)
                    {
                        list.Add("Depth");
                    }
                    if ((target.depthTextureMode & DepthTextureMode.DepthNormals) != DepthTextureMode.None)
                    {
                        list.Add("DepthNormals");
                    }
                    if ((target.depthTextureMode & DepthTextureMode.MotionVectors) != DepthTextureMode.None)
                    {
                        list.Add("MotionVectors");
                    }
                    if (list.Count != 0)
                    {
                        StringBuilder builder = new StringBuilder("Info: renders ");
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (i != 0)
                            {
                                builder.Append(" & ");
                            }
                            builder.Append(list[i]);
                        }
                        builder.Append((list.Count <= 1) ? " texture" : " textures");
                        EditorGUILayout.HelpBox(builder.ToString(), MessageType.None, true);
                    }
                }
            }
        }

        private void DisplayHDRWarnings()
        {
            Camera target = base.target as Camera;
            if (target != null)
            {
                string[] hDRWarnings = target.GetHDRWarnings();
                if (hDRWarnings.Length > 0)
                {
                    EditorGUILayout.HelpBox(string.Join("\n\n", hDRWarnings), MessageType.Warning, true);
                }
            }
        }

        [DrawGizmo(GizmoType.NonSelected)]
        private static void DrawCameraBound(Camera camera, GizmoType gizmoType)
        {
            SceneView currentDrawingSceneView = SceneView.currentDrawingSceneView;
            if (((currentDrawingSceneView != null) && currentDrawingSceneView.in2DMode) && ((camera == Camera.main) && camera.orthographic))
            {
                RenderGizmo(camera);
            }
        }

        private static bool GetFrustum(Camera camera, Vector3[] near, Vector3[] far, out float frustumAspect)
        {
            frustumAspect = GetFrustumAspectRatio(camera);
            if (frustumAspect < 0f)
            {
                return false;
            }
            if (far != null)
            {
                far[0] = new Vector3(0f, 0f, camera.farClipPlane);
                far[1] = new Vector3(0f, 1f, camera.farClipPlane);
                far[2] = new Vector3(1f, 1f, camera.farClipPlane);
                far[3] = new Vector3(1f, 0f, camera.farClipPlane);
                for (int i = 0; i < 4; i++)
                {
                    far[i] = camera.ViewportToWorldPoint(far[i]);
                }
            }
            if (near != null)
            {
                near[0] = new Vector3(0f, 0f, camera.nearClipPlane);
                near[1] = new Vector3(0f, 1f, camera.nearClipPlane);
                near[2] = new Vector3(1f, 1f, camera.nearClipPlane);
                near[3] = new Vector3(1f, 0f, camera.nearClipPlane);
                for (int j = 0; j < 4; j++)
                {
                    near[j] = camera.ViewportToWorldPoint(near[j]);
                }
            }
            return true;
        }

        private static float GetFrustumAspectRatio(Camera camera)
        {
            Rect rect = camera.rect;
            if ((rect.width <= 0f) || (rect.height <= 0f))
            {
                return -1f;
            }
            float num2 = rect.width / rect.height;
            return (GetGameViewAspectRatio() * num2);
        }

        [RequiredByNativeCode]
        private static float GetGameViewAspectRatio()
        {
            Vector2 mainGameViewTargetSize = GameView.GetMainGameViewTargetSize();
            if (mainGameViewTargetSize.x < 0f)
            {
                mainGameViewTargetSize.x = Screen.width;
                mainGameViewTargetSize.y = Screen.height;
            }
            return (mainGameViewTargetSize.x / mainGameViewTargetSize.y);
        }

        private static Rect GetRemoveButtonRect(Rect r)
        {
            Vector2 vector = Styles.invisibleButton.CalcSize(Styles.iconRemove);
            return new Rect(r.xMax - vector.x, r.y + ((int) ((r.height / 2f) - (vector.y / 2f))), vector.x, vector.y);
        }

        private static bool IsDeferredRenderingPath(RenderingPath rp) => 
            ((rp == RenderingPath.DeferredLighting) || (rp == RenderingPath.DeferredShading));

        private static bool IsViewPortRectValidToRender(Rect normalizedViewPortRect)
        {
            if ((normalizedViewPortRect.width <= 0f) || (normalizedViewPortRect.height <= 0f))
            {
                return false;
            }
            if ((normalizedViewPortRect.x >= 1f) || (normalizedViewPortRect.xMax <= 0f))
            {
                return false;
            }
            if ((normalizedViewPortRect.y >= 1f) || (normalizedViewPortRect.yMax <= 0f))
            {
                return false;
            }
            return true;
        }

        private static Vector3 MidPointPositionSlider(Vector3 position1, Vector3 position2, Vector3 direction)
        {
            Vector3 position = Vector3.Lerp(position1, position2, 0.5f);
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Handles.CapFunction(Handles.DotHandleCap);
            }
            return Handles.Slider(position, direction, HandleUtility.GetHandleSize(position) * 0.03f, <>f__mg$cache0, 0f);
        }

        public void OnDestroy()
        {
            if (this.m_PreviewCamera != null)
            {
                UnityEngine.Object.DestroyImmediate(this.m_PreviewCamera.gameObject, true);
            }
        }

        internal void OnDisable()
        {
            this.m_ShowBGColorOptions.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            this.m_ShowOrthoOptions.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            this.m_ShowTargetEyeOption.valueChanged.RemoveListener(new UnityAction(this.Repaint));
        }

        public void OnEnable()
        {
            this.m_ClearFlags = base.serializedObject.FindProperty("m_ClearFlags");
            this.m_BackgroundColor = base.serializedObject.FindProperty("m_BackGroundColor");
            this.m_NormalizedViewPortRect = base.serializedObject.FindProperty("m_NormalizedViewPortRect");
            this.m_NearAndFarClippingPlanes = new SerializedProperty[] { base.serializedObject.FindProperty("near clip plane"), base.serializedObject.FindProperty("far clip plane") };
            this.m_FieldOfView = base.serializedObject.FindProperty("field of view");
            this.m_Orthographic = base.serializedObject.FindProperty("orthographic");
            this.m_OrthographicSize = base.serializedObject.FindProperty("orthographic size");
            this.m_Depth = base.serializedObject.FindProperty("m_Depth");
            this.m_CullingMask = base.serializedObject.FindProperty("m_CullingMask");
            this.m_RenderingPath = base.serializedObject.FindProperty("m_RenderingPath");
            this.m_OcclusionCulling = base.serializedObject.FindProperty("m_OcclusionCulling");
            this.m_TargetTexture = base.serializedObject.FindProperty("m_TargetTexture");
            this.m_HDR = base.serializedObject.FindProperty("m_HDR");
            this.m_StereoConvergence = base.serializedObject.FindProperty("m_StereoConvergence");
            this.m_StereoSeparation = base.serializedObject.FindProperty("m_StereoSeparation");
            this.m_TargetDisplay = base.serializedObject.FindProperty("m_TargetDisplay");
            this.m_TargetEye = base.serializedObject.FindProperty("m_TargetEye");
            Camera target = (Camera) base.target;
            this.m_ShowBGColorOptions.value = !this.m_ClearFlags.hasMultipleDifferentValues && ((target.clearFlags == CameraClearFlags.Color) || (target.clearFlags == CameraClearFlags.Skybox));
            this.m_ShowOrthoOptions.value = target.orthographic;
            this.m_ShowTargetEyeOption.value = (this.m_TargetEye.intValue != 3) || PlayerSettings.virtualRealitySupported;
            this.m_ShowBGColorOptions.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowOrthoOptions.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowTargetEyeOption.valueChanged.AddListener(new UnityAction(this.Repaint));
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            Camera target = (Camera) base.target;
            this.m_ShowBGColorOptions.target = !this.m_ClearFlags.hasMultipleDifferentValues && ((target.clearFlags == CameraClearFlags.Color) || (target.clearFlags == CameraClearFlags.Skybox));
            this.m_ShowOrthoOptions.target = !this.m_Orthographic.hasMultipleDifferentValues && target.orthographic;
            this.m_ShowTargetEyeOption.target = (this.m_TargetEye.intValue != 3) || PlayerSettings.virtualRealitySupported;
            EditorGUILayout.PropertyField(this.m_ClearFlags, new GUILayoutOption[0]);
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowBGColorOptions.faded))
            {
                EditorGUILayout.PropertyField(this.m_BackgroundColor, new GUIContent("Background", "Camera clears the screen to this color before rendering."), new GUILayoutOption[0]);
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.PropertyField(this.m_CullingMask, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            ProjectionType selected = !this.m_Orthographic.boolValue ? ProjectionType.Perspective : ProjectionType.Orthographic;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = this.m_Orthographic.hasMultipleDifferentValues;
            selected = (ProjectionType) EditorGUILayout.EnumPopup("Projection", selected, new GUILayoutOption[0]);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.m_Orthographic.boolValue = selected == ProjectionType.Orthographic;
            }
            if (!this.m_Orthographic.hasMultipleDifferentValues)
            {
                if (EditorGUILayout.BeginFadeGroup(this.m_ShowOrthoOptions.faded))
                {
                    EditorGUILayout.PropertyField(this.m_OrthographicSize, new GUIContent("Size"), new GUILayoutOption[0]);
                }
                EditorGUILayout.EndFadeGroup();
                if (EditorGUILayout.BeginFadeGroup(1f - this.m_ShowOrthoOptions.faded))
                {
                    EditorGUILayout.Slider(this.m_FieldOfView, 1f, 179f, new GUIContent("Field of View"), new GUILayoutOption[0]);
                }
                EditorGUILayout.EndFadeGroup();
            }
            EditorGUILayout.PropertiesField(EditorGUI.s_ClipingPlanesLabel, this.m_NearAndFarClippingPlanes, EditorGUI.s_NearAndFarLabels, 35f, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_NormalizedViewPortRect, this.m_ViewportLabel, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.m_Depth, new GUILayoutOption[0]);
            EditorGUILayout.IntPopup(this.m_RenderingPath, kCameraRenderPaths, kCameraRenderPathValues, EditorGUIUtility.TempContent("Rendering Path"), new GUILayoutOption[0]);
            if (this.m_ShowOrthoOptions.target && this.wantDeferredRendering)
            {
                EditorGUILayout.HelpBox("Deferred rendering does not work with Orthographic camera, will use Forward.", MessageType.Warning, true);
            }
            EditorGUILayout.PropertyField(this.m_TargetTexture, new GUILayoutOption[0]);
            if (!this.m_TargetTexture.hasMultipleDifferentValues)
            {
                RenderTexture objectReferenceValue = this.m_TargetTexture.objectReferenceValue as RenderTexture;
                if (((objectReferenceValue != null) && (objectReferenceValue.antiAliasing > 1)) && this.wantDeferredRendering)
                {
                    EditorGUILayout.HelpBox("Manual MSAA target set with deferred rendering. This will lead to undefined behavior.", MessageType.Warning, true);
                }
            }
            EditorGUILayout.PropertyField(this.m_OcclusionCulling, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_HDR, new GUILayoutOption[0]);
            if (this.m_HDR.boolValue)
            {
                this.DisplayHDRWarnings();
            }
            if (PlayerSettings.virtualRealitySupported)
            {
                EditorGUILayout.PropertyField(this.m_StereoSeparation, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_StereoConvergence, new GUILayoutOption[0]);
            }
            if (this.ShouldShowTargetDisplayProperty())
            {
                int intValue = this.m_TargetDisplay.intValue;
                EditorGUILayout.Space();
                EditorGUILayout.IntPopup(this.m_TargetDisplay, DisplayUtility.GetDisplayNames(), DisplayUtility.GetDisplayIndices(), EditorGUIUtility.TempContent("Target Display"), new GUILayoutOption[0]);
                if (intValue != this.m_TargetDisplay.intValue)
                {
                    GameView.RepaintAll();
                }
            }
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowTargetEyeOption.faded))
            {
                EditorGUILayout.IntPopup(this.m_TargetEye, kTargetEyes, kTargetEyeValues, EditorGUIUtility.TempContent("Target Eye"), new GUILayoutOption[0]);
            }
            EditorGUILayout.EndFadeGroup();
            this.DepthTextureModeGUI();
            this.CommandBufferGUI();
            base.serializedObject.ApplyModifiedProperties();
        }

        public void OnOverlayGUI(UnityEngine.Object target, SceneView sceneView)
        {
            if (target != null)
            {
                Camera other = (Camera) target;
                Vector2 mainGameViewTargetSize = GameView.GetMainGameViewTargetSize();
                if (mainGameViewTargetSize.x < 0f)
                {
                    mainGameViewTargetSize.x = sceneView.position.width;
                    mainGameViewTargetSize.y = sceneView.position.height;
                }
                Rect rect = other.rect;
                mainGameViewTargetSize.x *= Mathf.Max(rect.width, 0f);
                mainGameViewTargetSize.y *= Mathf.Max(rect.height, 0f);
                if ((mainGameViewTargetSize.x > 0f) && (mainGameViewTargetSize.y > 0f))
                {
                    float num = mainGameViewTargetSize.x / mainGameViewTargetSize.y;
                    mainGameViewTargetSize.y = 0.2f * sceneView.position.height;
                    mainGameViewTargetSize.x = mainGameViewTargetSize.y * num;
                    if (mainGameViewTargetSize.y > (sceneView.position.height * 0.5f))
                    {
                        mainGameViewTargetSize.y = sceneView.position.height * 0.5f;
                        mainGameViewTargetSize.x = mainGameViewTargetSize.y * num;
                    }
                    if (mainGameViewTargetSize.x > (sceneView.position.width * 0.5f))
                    {
                        mainGameViewTargetSize.x = sceneView.position.width * 0.5f;
                        mainGameViewTargetSize.y = mainGameViewTargetSize.x / num;
                    }
                    Rect rect9 = EditorGUIUtility.PointsToPixels(GUILayoutUtility.GetRect(mainGameViewTargetSize.x, mainGameViewTargetSize.y));
                    rect9.y = (((sceneView.position.height + 1f) * EditorGUIUtility.pixelsPerPoint) - rect9.y) - rect9.height;
                    if (Event.current.type == EventType.Repaint)
                    {
                        this.previewCamera.CopyFrom(other);
                        Skybox component = this.previewCamera.GetComponent<Skybox>();
                        if (component != null)
                        {
                            Skybox skybox2 = other.GetComponent<Skybox>();
                            if ((skybox2 != null) && skybox2.enabled)
                            {
                                component.enabled = true;
                                component.material = skybox2.material;
                            }
                            else
                            {
                                component.enabled = false;
                            }
                        }
                        this.previewCamera.targetTexture = null;
                        this.previewCamera.pixelRect = rect9;
                        Handles.EmitGUIGeometryForCamera(other, this.previewCamera);
                        this.previewCamera.Render();
                    }
                }
            }
        }

        public void OnSceneGUI()
        {
            Camera target = (Camera) base.target;
            if (IsViewPortRectValidToRender(target.rect))
            {
                float num;
                SceneViewOverlay.Window(new GUIContent("Camera Preview"), new SceneViewOverlay.WindowFunction(this.OnOverlayGUI), -100, base.target, SceneViewOverlay.WindowDisplayOption.OneWindowPerTarget);
                Color color = Handles.color;
                Color kGizmoCamera = CameraEditor.kGizmoCamera;
                kGizmoCamera.a *= 2f;
                Handles.color = kGizmoCamera;
                Vector3[] far = new Vector3[4];
                if (GetFrustum(target, null, far, out num))
                {
                    Vector3 a = far[0];
                    Vector3 vector2 = far[1];
                    Vector3 b = far[2];
                    Vector3 vector4 = far[3];
                    bool changed = GUI.changed;
                    Vector3 vector5 = Vector3.Lerp(a, b, 0.5f);
                    float magnitude = -1f;
                    Vector3 vector6 = MidPointPositionSlider(vector2, b, target.transform.up);
                    if (!GUI.changed)
                    {
                        vector6 = MidPointPositionSlider(a, vector4, -target.transform.up);
                    }
                    if (GUI.changed)
                    {
                        Vector3 vector7 = vector6 - vector5;
                        magnitude = vector7.magnitude;
                    }
                    GUI.changed = false;
                    vector6 = MidPointPositionSlider(vector4, b, target.transform.right);
                    if (!GUI.changed)
                    {
                        vector6 = MidPointPositionSlider(a, vector2, -target.transform.right);
                    }
                    if (GUI.changed)
                    {
                        Vector3 vector8 = vector6 - vector5;
                        magnitude = vector8.magnitude / num;
                    }
                    if (magnitude >= 0f)
                    {
                        Undo.RecordObject(target, "Adjust Camera");
                        if (target.orthographic)
                        {
                            target.orthographicSize = magnitude;
                        }
                        else
                        {
                            Vector3 vector9 = vector5 + ((Vector3) (target.transform.up * magnitude));
                            target.fieldOfView = Vector3.Angle(target.transform.forward, vector9 - target.transform.position) * 2f;
                        }
                        changed = true;
                    }
                    GUI.changed = changed;
                    Handles.color = color;
                }
            }
        }

        internal static void RenderGizmo(Camera camera)
        {
            float num;
            Vector3[] near = new Vector3[4];
            Vector3[] far = new Vector3[4];
            if (GetFrustum(camera, near, far, out num))
            {
                Color color = Handles.color;
                Handles.color = kGizmoCamera;
                for (int i = 0; i < 4; i++)
                {
                    Handles.DrawLine(near[i], near[(i + 1) % 4]);
                    Handles.DrawLine(far[i], far[(i + 1) % 4]);
                    Handles.DrawLine(near[i], far[i]);
                }
                Handles.color = color;
            }
        }

        private bool ShouldShowTargetDisplayProperty()
        {
            GUIContent[] displayNames = ModuleManager.GetDisplayNames(EditorUserBuildSettings.activeBuildTarget.ToString());
            return ((BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget) == BuildTargetGroup.Standalone) || (displayNames != null));
        }

        private Camera camera =>
            (base.target as Camera);

        private Camera previewCamera
        {
            get
            {
                if (this.m_PreviewCamera == null)
                {
                    System.Type[] components = new System.Type[] { typeof(Camera), typeof(Skybox) };
                    this.m_PreviewCamera = EditorUtility.CreateGameObjectWithHideFlags("Preview Camera", HideFlags.HideAndDontSave, components).GetComponent<Camera>();
                }
                this.m_PreviewCamera.enabled = false;
                return this.m_PreviewCamera;
            }
        }

        private bool wantDeferredRendering
        {
            get
            {
                bool flag = IsDeferredRenderingPath(this.camera.renderingPath);
                bool flag2 = IsDeferredRenderingPath(EditorGraphicsSettings.GetCurrentTierSettings().renderingPath);
                return (flag || ((this.camera.renderingPath == RenderingPath.UsePlayerSettings) && flag2));
            }
        }

        private enum ProjectionType
        {
            Perspective,
            Orthographic
        }

        private class Styles
        {
            public static GUIContent iconRemove = EditorGUIUtility.IconContent("Toolbar Minus", "|Remove command buffer");
            public static GUIStyle invisibleButton = "InvisibleButton";
        }
    }
}

