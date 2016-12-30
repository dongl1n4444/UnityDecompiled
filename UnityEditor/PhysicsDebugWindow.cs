namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    /// <summary>
    /// <para>Displays the Physics Debug Visualization options.
    /// 
    /// The Physics Debug Visualization is only displayed if this window is visible.
    /// 
    /// See Also: PhysicsVisualizationSettings.</para>
    /// </summary>
    public class PhysicsDebugWindow : EditorWindow
    {
        [CompilerGenerated]
        private static HandleUtility.PickClosestGameObjectFunc <>f__mg$cache0;
        [CompilerGenerated]
        private static HandleUtility.PickClosestGameObjectFunc <>f__mg$cache1;
        [SerializeField]
        private bool m_ColorFoldout = false;
        [SerializeField]
        private bool m_FilterColliderTypesFoldout = false;
        [SerializeField]
        private Vector2 m_MainScrollPos = Vector2.zero;
        private bool m_MouseLeaveListenerAdded = false;
        private bool m_PickAdded = false;
        [SerializeField]
        private bool m_RenderingFoldout = false;

        private void AddMouseLeaveListener()
        {
            if (!this.m_MouseLeaveListenerAdded)
            {
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.OnMouseLeaveCheck));
                this.m_MouseLeaveListenerAdded = true;
            }
        }

        private void AddPicker()
        {
            if (!this.m_PickAdded || (HandleUtility.pickClosestGameObjectDelegate == null))
            {
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new HandleUtility.PickClosestGameObjectFunc(PhysicsVisualizationSettings.PickClosestGameObject);
                }
                HandleUtility.pickClosestGameObjectDelegate = (HandleUtility.PickClosestGameObjectFunc) Delegate.Combine(HandleUtility.pickClosestGameObjectDelegate, <>f__mg$cache0);
            }
            this.m_PickAdded = true;
        }

        private void DisplayControls(Object o, SceneView view)
        {
            int dirtyCount = PhysicsVisualizationSettings.dirtyCount;
            PhysicsVisualizationSettings.showCollisionGeometry = EditorGUILayout.Toggle(Contents.showCollisionGeometry, PhysicsVisualizationSettings.showCollisionGeometry, new GUILayoutOption[0]);
            PhysicsVisualizationSettings.enableMouseSelect = EditorGUILayout.Toggle(Contents.enableMouseSelect, PhysicsVisualizationSettings.enableMouseSelect, new GUILayoutOption[0]);
            if (PhysicsVisualizationSettings.devOptions)
            {
                PhysicsVisualizationSettings.useSceneCam = EditorGUILayout.Toggle(Contents.useSceneCam, PhysicsVisualizationSettings.useSceneCam, new GUILayoutOption[0]);
            }
            Vector2 mousePosition = Event.current.mousePosition;
            bool flag = new Rect(0f, 17f, view.position.width, view.position.height - 17f).Contains(Event.current.mousePosition);
            if ((PhysicsVisualizationSettings.showCollisionGeometry && PhysicsVisualizationSettings.enableMouseSelect) && flag)
            {
                this.AddPicker();
                this.AddMouseLeaveListener();
                if (Event.current.type == EventType.MouseMove)
                {
                    PhysicsVisualizationSettings.UpdateMouseHighlight(HandleUtility.GUIPointToScreenPixelCoordinate(mousePosition));
                }
                if (Event.current.type == EventType.MouseDrag)
                {
                    PhysicsVisualizationSettings.ClearMouseHighlight();
                }
            }
            else
            {
                this.RemovePicker();
                PhysicsVisualizationSettings.ClearMouseHighlight();
            }
            if (dirtyCount != PhysicsVisualizationSettings.dirtyCount)
            {
                RepaintSceneAndGameViews();
            }
        }

        private void OnBecameInvisible()
        {
            this.RemovePicker();
            SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc) Delegate.Remove(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
            PhysicsVisualizationSettings.DeinitDebugDraw();
            RepaintSceneAndGameViews();
        }

        private void OnBecameVisible()
        {
            PhysicsVisualizationSettings.InitDebugDraw();
            SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc) Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
            RepaintSceneAndGameViews();
        }

        private void OnGUI()
        {
            int dirtyCount = PhysicsVisualizationSettings.dirtyCount;
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(130f) };
            PhysicsVisualizationSettings.filterWorkflow = (PhysicsVisualizationSettings.FilterWorkflow) EditorGUILayout.EnumPopup(PhysicsVisualizationSettings.filterWorkflow, EditorStyles.toolbarPopup, options);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Reset", EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                PhysicsVisualizationSettings.Reset();
            }
            EditorGUILayout.EndHorizontal();
            this.m_MainScrollPos = GUILayout.BeginScrollView(this.m_MainScrollPos, new GUILayoutOption[0]);
            PhysicsVisualizationSettings.FilterWorkflow filterWorkflow = PhysicsVisualizationSettings.filterWorkflow;
            string str = (filterWorkflow != PhysicsVisualizationSettings.FilterWorkflow.ShowSelectedItems) ? "Hide " : "Show ";
            int mask = InternalEditorUtility.LayerMaskToConcatenatedLayersMask(PhysicsVisualizationSettings.GetShowCollisionLayerMask(filterWorkflow));
            int concatenatedLayersMask = EditorGUILayout.MaskField(GUIContent.Temp(str + "Layers", str + "selected layers"), mask, InternalEditorUtility.layers, new GUILayoutOption[0]);
            PhysicsVisualizationSettings.SetShowCollisionLayerMask(filterWorkflow, (int) InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(concatenatedLayersMask));
            PhysicsVisualizationSettings.SetShowStaticColliders(filterWorkflow, EditorGUILayout.Toggle(GUIContent.Temp(str + "Static Colliders", str + "collision geometry from Colliders that do not have a Rigidbody"), PhysicsVisualizationSettings.GetShowStaticColliders(filterWorkflow), new GUILayoutOption[0]));
            PhysicsVisualizationSettings.SetShowTriggers(filterWorkflow, EditorGUILayout.Toggle(GUIContent.Temp(str + "Triggers", str + "collision geometry from Colliders that have 'isTrigger' enabled"), PhysicsVisualizationSettings.GetShowTriggers(filterWorkflow), new GUILayoutOption[0]));
            PhysicsVisualizationSettings.SetShowRigidbodies(filterWorkflow, EditorGUILayout.Toggle(GUIContent.Temp(str + "Rigidbodies", str + "collision geometry from Rigidbodies"), PhysicsVisualizationSettings.GetShowRigidbodies(filterWorkflow), new GUILayoutOption[0]));
            PhysicsVisualizationSettings.SetShowKinematicBodies(filterWorkflow, EditorGUILayout.Toggle(GUIContent.Temp(str + "Kinematic Bodies", str + "collision geometry from Kinematic Rigidbodies"), PhysicsVisualizationSettings.GetShowKinematicBodies(filterWorkflow), new GUILayoutOption[0]));
            PhysicsVisualizationSettings.SetShowSleepingBodies(filterWorkflow, EditorGUILayout.Toggle(GUIContent.Temp(str + "Sleeping Bodies", str + "collision geometry from Sleeping Rigidbodies"), PhysicsVisualizationSettings.GetShowSleepingBodies(filterWorkflow), new GUILayoutOption[0]));
            this.m_FilterColliderTypesFoldout = EditorGUILayout.Foldout(this.m_FilterColliderTypesFoldout, "Collider Types");
            if (this.m_FilterColliderTypesFoldout)
            {
                EditorGUI.indentLevel++;
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 200f;
                PhysicsVisualizationSettings.SetShowBoxColliders(filterWorkflow, EditorGUILayout.Toggle(GUIContent.Temp(str + "BoxColliders", str + "collision geometry from BoxColliders"), PhysicsVisualizationSettings.GetShowBoxColliders(filterWorkflow), new GUILayoutOption[0]));
                PhysicsVisualizationSettings.SetShowSphereColliders(filterWorkflow, EditorGUILayout.Toggle(GUIContent.Temp(str + "SphereColliders", str + "collision geometry from SphereColliders"), PhysicsVisualizationSettings.GetShowSphereColliders(filterWorkflow), new GUILayoutOption[0]));
                PhysicsVisualizationSettings.SetShowCapsuleColliders(filterWorkflow, EditorGUILayout.Toggle(GUIContent.Temp(str + "CapsuleColliders", str + "collision geometry from CapsuleColliders"), PhysicsVisualizationSettings.GetShowCapsuleColliders(filterWorkflow), new GUILayoutOption[0]));
                PhysicsVisualizationSettings.SetShowMeshColliders(filterWorkflow, PhysicsVisualizationSettings.MeshColliderType.Convex, EditorGUILayout.Toggle(GUIContent.Temp(str + "MeshColliders (convex)", str + "collision geometry from convex MeshColliders"), PhysicsVisualizationSettings.GetShowMeshColliders(filterWorkflow, PhysicsVisualizationSettings.MeshColliderType.Convex), new GUILayoutOption[0]));
                PhysicsVisualizationSettings.SetShowMeshColliders(filterWorkflow, PhysicsVisualizationSettings.MeshColliderType.NonConvex, EditorGUILayout.Toggle(GUIContent.Temp(str + "MeshColliders (concave)", str + "collision geometry from non-convex MeshColliders"), PhysicsVisualizationSettings.GetShowMeshColliders(filterWorkflow, PhysicsVisualizationSettings.MeshColliderType.NonConvex), new GUILayoutOption[0]));
                PhysicsVisualizationSettings.SetShowTerrainColliders(filterWorkflow, EditorGUILayout.Toggle(GUIContent.Temp(str + "TerrainColliders", str + "collision geometry from TerrainColliders"), PhysicsVisualizationSettings.GetShowTerrainColliders(filterWorkflow), new GUILayoutOption[0]));
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUI.indentLevel--;
            }
            GUILayout.Space(4f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            bool flag = GUILayout.Button(str + "None", "MiniButton", new GUILayoutOption[0]);
            bool selected = GUILayout.Button(str + "All", "MiniButton", new GUILayoutOption[0]);
            if (flag || selected)
            {
                PhysicsVisualizationSettings.SetShowForAllFilters(filterWorkflow, selected);
            }
            GUILayout.EndHorizontal();
            this.m_ColorFoldout = EditorGUILayout.Foldout(this.m_ColorFoldout, "Colors");
            if (this.m_ColorFoldout)
            {
                EditorGUI.indentLevel++;
                PhysicsVisualizationSettings.staticColor = EditorGUILayout.ColorField(Contents.staticColor, PhysicsVisualizationSettings.staticColor, false, true, false, Contents.pickerConfig, new GUILayoutOption[0]);
                PhysicsVisualizationSettings.triggerColor = EditorGUILayout.ColorField(Contents.triggerColor, PhysicsVisualizationSettings.triggerColor, false, true, false, Contents.pickerConfig, new GUILayoutOption[0]);
                PhysicsVisualizationSettings.rigidbodyColor = EditorGUILayout.ColorField(Contents.rigidbodyColor, PhysicsVisualizationSettings.rigidbodyColor, false, true, false, Contents.pickerConfig, new GUILayoutOption[0]);
                PhysicsVisualizationSettings.kinematicColor = EditorGUILayout.ColorField(Contents.kinematicColor, PhysicsVisualizationSettings.kinematicColor, false, true, false, Contents.pickerConfig, new GUILayoutOption[0]);
                PhysicsVisualizationSettings.sleepingBodyColor = EditorGUILayout.ColorField(Contents.sleepingBodyColor, PhysicsVisualizationSettings.sleepingBodyColor, false, true, false, Contents.pickerConfig, new GUILayoutOption[0]);
                PhysicsVisualizationSettings.colorVariance = EditorGUILayout.Slider("Variation", PhysicsVisualizationSettings.colorVariance, 0f, 1f, new GUILayoutOption[0]);
                EditorGUI.indentLevel--;
            }
            this.m_RenderingFoldout = EditorGUILayout.Foldout(this.m_RenderingFoldout, "Rendering");
            if (this.m_RenderingFoldout)
            {
                EditorGUI.indentLevel++;
                PhysicsVisualizationSettings.baseAlpha = 1f - EditorGUILayout.Slider("Transparency", 1f - PhysicsVisualizationSettings.baseAlpha, 0f, 1f, new GUILayoutOption[0]);
                PhysicsVisualizationSettings.forceOverdraw = EditorGUILayout.Toggle(Contents.forceOverdraw, PhysicsVisualizationSettings.forceOverdraw, new GUILayoutOption[0]);
                PhysicsVisualizationSettings.viewDistance = EditorGUILayout.FloatField(Contents.viewDistance, PhysicsVisualizationSettings.viewDistance, new GUILayoutOption[0]);
                PhysicsVisualizationSettings.terrainTilesMax = EditorGUILayout.IntField(Contents.terrainTilesMax, PhysicsVisualizationSettings.terrainTilesMax, new GUILayoutOption[0]);
                EditorGUI.indentLevel--;
            }
            if (Unsupported.IsDeveloperBuild() || PhysicsVisualizationSettings.devOptions)
            {
                PhysicsVisualizationSettings.devOptions = EditorGUILayout.Toggle(Contents.devOptions, PhysicsVisualizationSettings.devOptions, new GUILayoutOption[0]);
            }
            if (PhysicsVisualizationSettings.devOptions)
            {
                PhysicsVisualizationSettings.dotAlpha = EditorGUILayout.Slider("dotAlpha", PhysicsVisualizationSettings.dotAlpha, -1f, 1f, new GUILayoutOption[0]);
                PhysicsVisualizationSettings.forceDot = EditorGUILayout.Toggle(Contents.forceDot, PhysicsVisualizationSettings.forceDot, new GUILayoutOption[0]);
                Tools.hidden = EditorGUILayout.Toggle(Contents.toolsHidden, Tools.hidden, new GUILayoutOption[0]);
            }
            GUILayout.EndScrollView();
            if (dirtyCount != PhysicsVisualizationSettings.dirtyCount)
            {
                RepaintSceneAndGameViews();
            }
        }

        private void OnMouseLeaveCheck()
        {
            if (this.m_MouseLeaveListenerAdded && !(EditorWindow.mouseOverWindow is SceneView))
            {
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.OnMouseLeaveCheck));
                this.m_MouseLeaveListenerAdded = false;
                if (PhysicsVisualizationSettings.HasMouseHighlight())
                {
                    PhysicsVisualizationSettings.ClearMouseHighlight();
                }
            }
        }

        private void OnSceneViewGUI(SceneView view)
        {
            SceneViewOverlay.Window(Contents.physicsDebug, new SceneViewOverlay.WindowFunction(this.DisplayControls), 350, SceneViewOverlay.WindowDisplayOption.OneWindowPerTarget);
        }

        private void RemovePicker()
        {
            if (this.m_PickAdded && (HandleUtility.pickClosestGameObjectDelegate != null))
            {
                if (<>f__mg$cache1 == null)
                {
                    <>f__mg$cache1 = new HandleUtility.PickClosestGameObjectFunc(PhysicsVisualizationSettings.PickClosestGameObject);
                }
                HandleUtility.pickClosestGameObjectDelegate = (HandleUtility.PickClosestGameObjectFunc) Delegate.Remove(HandleUtility.pickClosestGameObjectDelegate, <>f__mg$cache1);
            }
            this.m_PickAdded = false;
        }

        private static void RepaintSceneAndGameViews()
        {
            SceneView.RepaintAll();
        }

        [MenuItem("Window/Physics Debugger", false, 0x835)]
        public static PhysicsDebugWindow ShowWindow()
        {
            PhysicsDebugWindow window = EditorWindow.GetWindow(typeof(PhysicsDebugWindow)) as PhysicsDebugWindow;
            if (window != null)
            {
                window.titleContent.text = "Physics Debug";
            }
            return window;
        }

        private static class Contents
        {
            public static readonly GUIContent devOptions = EditorGUIUtility.TextContent("devOptions");
            public static readonly GUIContent enableMouseSelect = EditorGUIUtility.TextContent("Mouse Select");
            public static readonly GUIContent forceDot = EditorGUIUtility.TextContent("Force Dot");
            public static readonly GUIContent forceOverdraw = EditorGUIUtility.TextContent("Force Overdraw|Draws Collider geometry on top of render geometry");
            public static readonly GUIContent kinematicColor = new GUIContent("Kinematic Bodies");
            public static readonly GUIContent physicsDebug = new GUIContent("Physics Debug");
            public static readonly ColorPickerHDRConfig pickerConfig = new ColorPickerHDRConfig(0f, 99f, 0.01010101f, 3f);
            public static readonly GUIContent rigidbodyColor = new GUIContent("Rigidbodies");
            public static readonly GUIContent showCollisionGeometry = EditorGUIUtility.TextContent("Collision Geometry");
            public static readonly GUIContent sleepingBodyColor = new GUIContent("Sleeping Bodies");
            public static readonly GUIContent staticColor = new GUIContent("Static Colliders");
            public static readonly GUIContent terrainTilesMax = EditorGUIUtility.TextContent("Terrain Tiles Max|Number of mesh tiles to drawn.");
            public static readonly GUIContent toolsHidden = EditorGUIUtility.TextContent("Hide tools");
            public static readonly GUIContent triggerColor = new GUIContent("Triggers");
            public static readonly GUIContent useSceneCam = EditorGUIUtility.TextContent("Use Scene Cam");
            public static readonly GUIContent viewDistance = EditorGUIUtility.TextContent("View Distance|Lower bound on distance from camera to physics geometry.");
            public static readonly GUIContent workflow = new GUIContent("Workflow", "The \"Hide\" mode is useful for fast discovery while the \"Show\" mode is useful for finding specific items.");
        }
    }
}

