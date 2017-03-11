namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.AI;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.AI;

    [EditorWindowTitle(title="Navigation", icon="Navigation")]
    internal class NavMeshEditorWindow : EditorWindow, IHasCustomMenu
    {
        [CompilerGenerated]
        private static Func<GameObject, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static SceneViewOverlay.WindowFunction <>f__mg$cache0;
        [CompilerGenerated]
        private static SceneViewOverlay.WindowFunction <>f__mg$cache1;
        [CompilerGenerated]
        private static SceneViewOverlay.WindowFunction <>f__mg$cache2;
        private const string kRootPath = "m_BuildSettings.";
        private SerializedProperty m_AccuratePlacement;
        private bool m_Advanced;
        private SerializedProperty m_AgentClimb;
        private SerializedProperty m_AgentHeight;
        private SerializedProperty m_AgentRadius;
        private SerializedProperty m_Agents;
        private ReorderableList m_AgentsList = null;
        private SerializedProperty m_AgentSlope;
        private SerializedProperty m_Areas;
        private ReorderableList m_AreasList = null;
        private bool m_BecameVisibleCalled = false;
        private SerializedProperty m_CellSize;
        private bool m_HasPendingAgentDebugInfo = false;
        private bool m_HasRepaintedForPendingAgentDebugInfo = false;
        private SerializedProperty m_LedgeDropHeight;
        private SerializedProperty m_ManualCellSize;
        private SerializedProperty m_MaxJumpAcrossDistance;
        private SerializedProperty m_MinRegionArea;
        private Mode m_Mode = Mode.ObjectSettings;
        private SerializedObject m_NavMeshProjectSettingsObject;
        private Vector2 m_ScrollPos = Vector2.zero;
        private int m_SelectedNavMeshAgentCount = 0;
        private int m_SelectedNavMeshObstacleCount = 0;
        private SerializedProperty m_SettingNames;
        private SerializedObject m_SettingsObject;
        private static NavMeshEditorWindow s_NavMeshEditorWindow;
        private static Styles s_Styles;

        private void AddAgent(ReorderableList list)
        {
            NavMesh.CreateSettings();
            list.index = NavMesh.GetSettingsCount() - 1;
        }

        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Reset Legacy Bake Settings"), false, new GenericMenu.MenuFunction(this.ResetBakeSettings));
        }

        private void AgentSettings()
        {
            if (this.m_Agents == null)
            {
                this.InitAgents();
            }
            this.m_NavMeshProjectSettingsObject.Update();
            if (this.m_AgentsList.index < 0)
            {
                this.m_AgentsList.index = 0;
            }
            this.m_AgentsList.DoLayoutList();
            if ((this.m_AgentsList.index >= 0) && (this.m_AgentsList.index < this.m_Agents.arraySize))
            {
                SerializedProperty arrayElementAtIndex = this.m_SettingNames.GetArrayElementAtIndex(this.m_AgentsList.index);
                SerializedProperty property2 = this.m_Agents.GetArrayElementAtIndex(this.m_AgentsList.index);
                SerializedProperty property = property2.FindPropertyRelative("agentRadius");
                SerializedProperty property4 = property2.FindPropertyRelative("agentHeight");
                SerializedProperty property5 = property2.FindPropertyRelative("agentClimb");
                SerializedProperty property6 = property2.FindPropertyRelative("agentSlope");
                NavMeshEditorHelpers.DrawAgentDiagram(EditorGUILayout.GetControlRect(false, 120f, new GUILayoutOption[0]), property.floatValue, property4.floatValue, property5.floatValue, property6.floatValue);
                EditorGUILayout.PropertyField(arrayElementAtIndex, EditorGUIUtility.TempContent("Name"), new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(property, EditorGUIUtility.TempContent("Radius"), new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(property4, EditorGUIUtility.TempContent("Height"), new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(property5, EditorGUIUtility.TempContent("Step Height"), new GUILayoutOption[0]);
                EditorGUILayout.Slider(property6, 0f, 60f, EditorGUIUtility.TempContent("Max Slope"), new GUILayoutOption[0]);
            }
            EditorGUILayout.Space();
            this.m_NavMeshProjectSettingsObject.ApplyModifiedProperties();
        }

        private void AreaSettings()
        {
            if (this.m_Areas == null)
            {
                this.InitAreas();
            }
            this.m_NavMeshProjectSettingsObject.Update();
            this.m_AreasList.DoLayoutList();
            this.m_NavMeshProjectSettingsObject.ApplyModifiedProperties();
        }

        public static void BackgroundTaskStatusChanged()
        {
            if (s_NavMeshEditorWindow != null)
            {
                s_NavMeshEditorWindow.Repaint();
            }
        }

        private static void BakeButtons()
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            bool enabled = GUI.enabled;
            GUI.enabled &= !Application.isPlaying;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(95f) };
            if (GUILayout.Button("Clear", options))
            {
                UnityEditor.AI.NavMeshBuilder.ClearAllNavMeshes();
            }
            GUI.enabled = enabled;
            if (UnityEditor.AI.NavMeshBuilder.isRunning)
            {
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(95f) };
                if (GUILayout.Button("Cancel", optionArray2))
                {
                    UnityEditor.AI.NavMeshBuilder.Cancel();
                }
            }
            else
            {
                enabled = GUI.enabled;
                GUI.enabled &= !Application.isPlaying;
                GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(95f) };
                if (GUILayout.Button("Bake", optionArray3))
                {
                    UnityEditor.AI.NavMeshBuilder.BuildNavMeshAsync();
                }
                GUI.enabled = enabled;
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private int Bit(int a, int b) => 
            ((a & (1 << (b & 0x1f))) >> b);

        private static void DisplayAgentControls(UnityEngine.Object target, SceneView sceneView)
        {
            EditorGUIUtility.labelWidth = 150f;
            bool flag = false;
            if (Event.current.type == EventType.Layout)
            {
                s_NavMeshEditorWindow.m_HasPendingAgentDebugInfo = NavMeshVisualizationSettings.hasPendingAgentDebugInfo;
            }
            bool showAgentPath = NavMeshVisualizationSettings.showAgentPath;
            if (showAgentPath != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Show Path Polygons|Shows the polygons leading to goal."), showAgentPath, new GUILayoutOption[0]))
            {
                NavMeshVisualizationSettings.showAgentPath = !showAgentPath;
                flag = true;
            }
            bool showAgentPathInfo = NavMeshVisualizationSettings.showAgentPathInfo;
            if (showAgentPathInfo != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Show Path Query Nodes|Shows the nodes expanded during last path query."), showAgentPathInfo, new GUILayoutOption[0]))
            {
                NavMeshVisualizationSettings.showAgentPathInfo = !showAgentPathInfo;
                flag = true;
            }
            bool showAgentNeighbours = NavMeshVisualizationSettings.showAgentNeighbours;
            if (showAgentNeighbours != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Show Neighbours|Show the agent neighbours cosidered during simulation."), showAgentNeighbours, new GUILayoutOption[0]))
            {
                NavMeshVisualizationSettings.showAgentNeighbours = !showAgentNeighbours;
                flag = true;
            }
            bool showAgentWalls = NavMeshVisualizationSettings.showAgentWalls;
            if (showAgentWalls != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Show Walls|Shows the wall segments handled during simulation."), showAgentWalls, new GUILayoutOption[0]))
            {
                NavMeshVisualizationSettings.showAgentWalls = !showAgentWalls;
                flag = true;
            }
            bool showAgentAvoidance = NavMeshVisualizationSettings.showAgentAvoidance;
            if (showAgentAvoidance != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Show Avoidance|Shows the processed avoidance geometry from simulation."), showAgentAvoidance, new GUILayoutOption[0]))
            {
                NavMeshVisualizationSettings.showAgentAvoidance = !showAgentAvoidance;
                flag = true;
            }
            if (showAgentAvoidance)
            {
                if (s_NavMeshEditorWindow.m_HasPendingAgentDebugInfo)
                {
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(165f) };
                    EditorGUILayout.BeginVertical(options);
                    EditorGUILayout.HelpBox("Avoidance display is not valid until after next game update.", MessageType.Warning);
                    EditorGUILayout.EndVertical();
                }
                if (s_NavMeshEditorWindow.m_SelectedNavMeshAgentCount > 10)
                {
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MaxWidth(165f) };
                    EditorGUILayout.BeginVertical(optionArray2);
                    EditorGUILayout.HelpBox(string.Concat(new object[] { "Avoidance visualization can be drawn for ", 10, " agents (", s_NavMeshEditorWindow.m_SelectedNavMeshAgentCount, " selected)." }), MessageType.Warning);
                    EditorGUILayout.EndVertical();
                }
            }
            if (flag)
            {
                RepaintSceneAndGameViews();
            }
        }

        private static void DisplayControls(UnityEngine.Object target, SceneView sceneView)
        {
            EditorGUIUtility.labelWidth = 150f;
            bool flag = false;
            bool showNavMesh = NavMeshVisualizationSettings.showNavMesh;
            if (showNavMesh != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Show NavMesh"), showNavMesh, new GUILayoutOption[0]))
            {
                NavMeshVisualizationSettings.showNavMesh = !showNavMesh;
                flag = true;
            }
            using (new EditorGUI.DisabledScope(!NavMeshVisualizationSettings.hasHeightMesh))
            {
                bool showHeightMesh = NavMeshVisualizationSettings.showHeightMesh;
                if (showHeightMesh != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Show HeightMesh"), showHeightMesh, new GUILayoutOption[0]))
                {
                    NavMeshVisualizationSettings.showHeightMesh = !showHeightMesh;
                    flag = true;
                }
            }
            if (Unsupported.IsDeveloperBuild())
            {
                GUILayout.Label("Internal", new GUILayoutOption[0]);
                bool showNavMeshPortals = NavMeshVisualizationSettings.showNavMeshPortals;
                if (showNavMeshPortals != EditorGUILayout.Toggle(new GUIContent("Show NavMesh Portals"), showNavMeshPortals, new GUILayoutOption[0]))
                {
                    NavMeshVisualizationSettings.showNavMeshPortals = !showNavMeshPortals;
                    flag = true;
                }
                bool showNavMeshLinks = NavMeshVisualizationSettings.showNavMeshLinks;
                if (showNavMeshLinks != EditorGUILayout.Toggle(new GUIContent("Show NavMesh Links"), showNavMeshLinks, new GUILayoutOption[0]))
                {
                    NavMeshVisualizationSettings.showNavMeshLinks = !showNavMeshLinks;
                    flag = true;
                }
                bool showProximityGrid = NavMeshVisualizationSettings.showProximityGrid;
                if (showProximityGrid != EditorGUILayout.Toggle(new GUIContent("Show Proximity Grid"), showProximityGrid, new GUILayoutOption[0]))
                {
                    NavMeshVisualizationSettings.showProximityGrid = !showProximityGrid;
                    flag = true;
                }
                bool showHeightMeshBVTree = NavMeshVisualizationSettings.showHeightMeshBVTree;
                if (showHeightMeshBVTree != EditorGUILayout.Toggle(new GUIContent("Show HeightMesh BV-Tree"), showHeightMeshBVTree, new GUILayoutOption[0]))
                {
                    NavMeshVisualizationSettings.showHeightMeshBVTree = !showHeightMeshBVTree;
                    flag = true;
                }
            }
            if (flag)
            {
                RepaintSceneAndGameViews();
            }
        }

        private static void DisplayObstacleControls(UnityEngine.Object target, SceneView sceneView)
        {
            EditorGUIUtility.labelWidth = 150f;
            bool flag = false;
            bool showObstacleCarveHull = NavMeshVisualizationSettings.showObstacleCarveHull;
            if (showObstacleCarveHull != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Show Carve Hull|Shows the hull used to carve the obstacle from navmesh."), showObstacleCarveHull, new GUILayoutOption[0]))
            {
                NavMeshVisualizationSettings.showObstacleCarveHull = !showObstacleCarveHull;
                flag = true;
            }
            if (flag)
            {
                RepaintSceneAndGameViews();
            }
        }

        private void DrawAgentListElement(Rect rect, int index, bool selected, bool focused)
        {
            SerializedProperty arrayElementAtIndex = this.m_Agents.GetArrayElementAtIndex(index);
            if (arrayElementAtIndex != null)
            {
                SerializedProperty property2 = arrayElementAtIndex.FindPropertyRelative("agentTypeID");
                if (property2 != null)
                {
                    rect.height -= 2f;
                    bool disabled = property2.intValue == 0;
                    using (new EditorGUI.DisabledScope(disabled))
                    {
                        string settingsNameFromID = NavMesh.GetSettingsNameFromID(property2.intValue);
                        GUI.Label(rect, EditorGUIUtility.TempContent(settingsNameFromID));
                    }
                }
            }
        }

        private void DrawAgentListHeader(Rect rect)
        {
            GUI.Label(rect, s_Styles.m_AgentTypesHeader);
        }

        private void DrawAreaListElement(Rect rect, int index, bool selected, bool focused)
        {
            SerializedProperty arrayElementAtIndex = this.m_Areas.GetArrayElementAtIndex(index);
            if (arrayElementAtIndex != null)
            {
                SerializedProperty property = arrayElementAtIndex.FindPropertyRelative("name");
                SerializedProperty property3 = arrayElementAtIndex.FindPropertyRelative("cost");
                if ((property != null) && (property3 != null))
                {
                    Rect rect2;
                    Rect rect3;
                    Rect rect4;
                    Rect rect5;
                    rect.height -= 2f;
                    bool flag = false;
                    bool flag2 = true;
                    bool flag3 = true;
                    switch (index)
                    {
                        case 0:
                            flag = true;
                            flag2 = false;
                            flag3 = true;
                            break;

                        case 1:
                            flag = true;
                            flag2 = false;
                            flag3 = false;
                            break;

                        case 2:
                            flag = true;
                            flag2 = false;
                            flag3 = true;
                            break;

                        default:
                            flag = false;
                            flag2 = true;
                            flag3 = true;
                            break;
                    }
                    this.GetAreaListRects(rect, out rect2, out rect3, out rect4, out rect5);
                    bool enabled = GUI.enabled;
                    Color areaColor = this.GetAreaColor(index);
                    Color color = new Color(areaColor.r * 0.1f, areaColor.g * 0.1f, areaColor.b * 0.1f, 0.6f);
                    EditorGUI.DrawRect(rect2, areaColor);
                    EditorGUI.DrawRect(new Rect(rect2.x, rect2.y, 1f, rect2.height), color);
                    EditorGUI.DrawRect(new Rect((rect2.x + rect2.width) - 1f, rect2.y, 1f, rect2.height), color);
                    EditorGUI.DrawRect(new Rect(rect2.x + 1f, rect2.y, rect2.width - 2f, 1f), color);
                    EditorGUI.DrawRect(new Rect(rect2.x + 1f, (rect2.y + rect2.height) - 1f, rect2.width - 2f, 1f), color);
                    if (flag)
                    {
                        GUI.Label(rect3, EditorGUIUtility.TempContent("Built-in " + index));
                    }
                    else
                    {
                        GUI.Label(rect3, EditorGUIUtility.TempContent("User " + index));
                    }
                    int indentLevel = EditorGUI.indentLevel;
                    EditorGUI.indentLevel = 0;
                    GUI.enabled = enabled && flag2;
                    EditorGUI.PropertyField(rect4, property, GUIContent.none);
                    GUI.enabled = enabled && flag3;
                    EditorGUI.PropertyField(rect5, property3, GUIContent.none);
                    GUI.enabled = enabled;
                    EditorGUI.indentLevel = indentLevel;
                }
            }
        }

        private void DrawAreaListHeader(Rect rect)
        {
            Rect rect2;
            Rect rect3;
            Rect rect4;
            Rect rect5;
            this.GetAreaListRects(rect, out rect2, out rect3, out rect4, out rect5);
            GUI.Label(rect4, s_Styles.m_NameLabel);
            GUI.Label(rect5, s_Styles.m_CostLabel);
        }

        private Color GetAreaColor(int i)
        {
            if (i == 0)
            {
                return new Color(0f, 0.75f, 1f, 0.5f);
            }
            int num = ((this.Bit(i, 4) + (this.Bit(i, 1) * 2)) + 1) * 0x3f;
            int num2 = ((this.Bit(i, 3) + (this.Bit(i, 2) * 2)) + 1) * 0x3f;
            int num3 = ((this.Bit(i, 5) + (this.Bit(i, 0) * 2)) + 1) * 0x3f;
            return new Color(((float) num) / 255f, ((float) num2) / 255f, ((float) num3) / 255f, 0.5f);
        }

        private void GetAreaListRects(Rect rect, out Rect stripeRect, out Rect labelRect, out Rect nameRect, out Rect costRect)
        {
            float num = EditorGUIUtility.singleLineHeight * 0.8f;
            float num2 = EditorGUIUtility.singleLineHeight * 5f;
            float width = EditorGUIUtility.singleLineHeight * 4f;
            float num4 = ((rect.width - num) - num2) - width;
            float x = rect.x;
            stripeRect = new Rect(x, rect.y, num - 4f, rect.height);
            x += num;
            labelRect = new Rect(x, rect.y, num2 - 4f, rect.height);
            x += num2;
            nameRect = new Rect(x, rect.y, num4 - 4f, rect.height);
            x += num4;
            costRect = new Rect(x, rect.y, width, rect.height);
        }

        private static List<GameObject> GetObjects(bool includeChildren)
        {
            if (includeChildren)
            {
                List<GameObject> list = new List<GameObject>();
                foreach (GameObject obj2 in Selection.gameObjects)
                {
                    list.AddRange(GetObjectsRecurse(obj2));
                }
                return list;
            }
            return new List<GameObject>(Selection.gameObjects);
        }

        private static IEnumerable<GameObject> GetObjectsRecurse(GameObject root)
        {
            List<GameObject> list = new List<GameObject> {
                root
            };
            IEnumerator enumerator = root.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    list.AddRange(GetObjectsRecurse(current.gameObject));
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
            return list;
        }

        private void InitAgents()
        {
            if (this.m_Agents == null)
            {
                this.m_Agents = this.m_NavMeshProjectSettingsObject.FindProperty("m_Settings");
                this.m_SettingNames = this.m_NavMeshProjectSettingsObject.FindProperty("m_SettingNames");
            }
            if (this.m_AgentsList == null)
            {
                this.m_AgentsList = new ReorderableList(this.m_NavMeshProjectSettingsObject, this.m_Agents, false, false, true, true);
                this.m_AgentsList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawAgentListElement);
                this.m_AgentsList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.DrawAgentListHeader);
                this.m_AgentsList.onAddCallback = new ReorderableList.AddCallbackDelegate(this.AddAgent);
                this.m_AgentsList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.RemoveAgent);
                this.m_AgentsList.elementHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        private void InitAreas()
        {
            if (this.m_Areas == null)
            {
                this.m_Areas = this.m_NavMeshProjectSettingsObject.FindProperty("areas");
            }
            if (this.m_AreasList == null)
            {
                this.m_AreasList = new ReorderableList(this.m_NavMeshProjectSettingsObject, this.m_Areas, false, false, false, false);
                this.m_AreasList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawAreaListElement);
                this.m_AreasList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.DrawAreaListHeader);
                this.m_AreasList.elementHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        private void InitProjectSettings()
        {
            if (this.m_NavMeshProjectSettingsObject == null)
            {
                UnityEngine.Object serializedAssetInterfaceSingleton = Unsupported.GetSerializedAssetInterfaceSingleton("NavMeshProjectSettings");
                this.m_NavMeshProjectSettingsObject = new SerializedObject(serializedAssetInterfaceSingleton);
            }
        }

        private void InitSceneBakeSettings()
        {
            this.m_SettingsObject = new SerializedObject(UnityEditor.AI.NavMeshBuilder.navMeshSettingsObject);
            this.m_AgentRadius = this.m_SettingsObject.FindProperty("m_BuildSettings.agentRadius");
            this.m_AgentHeight = this.m_SettingsObject.FindProperty("m_BuildSettings.agentHeight");
            this.m_AgentSlope = this.m_SettingsObject.FindProperty("m_BuildSettings.agentSlope");
            this.m_LedgeDropHeight = this.m_SettingsObject.FindProperty("m_BuildSettings.ledgeDropHeight");
            this.m_AgentClimb = this.m_SettingsObject.FindProperty("m_BuildSettings.agentClimb");
            this.m_MaxJumpAcrossDistance = this.m_SettingsObject.FindProperty("m_BuildSettings.maxJumpAcrossDistance");
            this.m_MinRegionArea = this.m_SettingsObject.FindProperty("m_BuildSettings.minRegionArea");
            this.m_ManualCellSize = this.m_SettingsObject.FindProperty("m_BuildSettings.manualCellSize");
            this.m_CellSize = this.m_SettingsObject.FindProperty("m_BuildSettings.cellSize");
            this.m_AccuratePlacement = this.m_SettingsObject.FindProperty("m_BuildSettings.accuratePlacement");
        }

        private void ModeToggle()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            this.m_Mode = (Mode) GUILayout.Toolbar((int) this.m_Mode, s_Styles.m_ModeToggles, "LargeButton", new GUILayoutOption[0]);
        }

        private static void ObjectSettings()
        {
            GameObject[] objArray;
            bool flag = true;
            System.Type[] types = new System.Type[] { typeof(MeshRenderer), typeof(Terrain) };
            SceneModeUtility.SearchBar(types);
            EditorGUILayout.Space();
            MeshRenderer[] selectedObjectsOfType = SceneModeUtility.GetSelectedObjectsOfType<MeshRenderer>(out objArray, new System.Type[0]);
            if (objArray.Length > 0)
            {
                flag = false;
                ObjectSettings(selectedObjectsOfType, objArray);
            }
            Terrain[] components = SceneModeUtility.GetSelectedObjectsOfType<Terrain>(out objArray, new System.Type[0]);
            if (objArray.Length > 0)
            {
                flag = false;
                ObjectSettings(components, objArray);
            }
            if (flag)
            {
                GUILayout.Label("Select a MeshRenderer or a Terrain from the scene.", EditorStyles.helpBox, new GUILayoutOption[0]);
            }
        }

        private static void ObjectSettings(UnityEngine.Object[] components, GameObject[] gos)
        {
            EditorGUILayout.HelpBox("Legacy Feature - Applies only to statically baked per-scene NavMesh", MessageType.Warning);
            EditorGUILayout.MultiSelectionObjectTitleBar(components);
            SerializedObject obj2 = new SerializedObject(gos);
            using (new EditorGUI.DisabledScope(!SceneModeUtility.StaticFlagField("Navigation Static", obj2.FindProperty("m_StaticEditorFlags"), 8)))
            {
                SceneModeUtility.StaticFlagField("Generate OffMeshLinks", obj2.FindProperty("m_StaticEditorFlags"), 0x20);
                SerializedProperty property = obj2.FindProperty("m_NavMeshLayer");
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
                string[] navMeshAreaNames = GameObjectUtility.GetNavMeshAreaNames();
                int navMeshArea = GameObjectUtility.GetNavMeshArea(gos[0]);
                int selectedIndex = -1;
                for (int i = 0; i < navMeshAreaNames.Length; i++)
                {
                    if (GameObjectUtility.GetNavMeshAreaFromName(navMeshAreaNames[i]) == navMeshArea)
                    {
                        selectedIndex = i;
                        break;
                    }
                }
                int index = EditorGUILayout.Popup("Navigation Area", selectedIndex, navMeshAreaNames, new GUILayoutOption[0]);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    int navMeshAreaFromName = GameObjectUtility.GetNavMeshAreaFromName(navMeshAreaNames[index]);
                    GameObjectUtility.ShouldIncludeChildren children = GameObjectUtility.DisplayUpdateChildrenDialogIfNeeded(Selection.gameObjects, "Change Navigation Area", "Do you want change the navigation area to " + navMeshAreaNames[index] + " for all the child objects as well?");
                    if (children != GameObjectUtility.ShouldIncludeChildren.Cancel)
                    {
                        property.intValue = navMeshAreaFromName;
                        SetNavMeshArea(navMeshAreaFromName, children == GameObjectUtility.ShouldIncludeChildren.IncludeChildren);
                    }
                }
            }
            obj2.ApplyModifiedProperties();
        }

        public void OnBecameInvisible()
        {
            if (this.m_BecameVisibleCalled)
            {
                NavMeshVisualizationSettings.showNavigation--;
                RepaintSceneAndGameViews();
                this.m_BecameVisibleCalled = false;
            }
        }

        public void OnBecameVisible()
        {
            if (!this.m_BecameVisibleCalled)
            {
                bool flag = NavMeshVisualizationSettings.showNavigation == 0;
                NavMeshVisualizationSettings.showNavigation++;
                if (flag)
                {
                    RepaintSceneAndGameViews();
                }
                this.m_BecameVisibleCalled = true;
            }
        }

        public void OnDisable()
        {
            s_NavMeshEditorWindow = null;
            EditorApplication.searchChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(this.Repaint));
            SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc) Delegate.Remove(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
        }

        public void OnEnable()
        {
            base.titleContent = base.GetLocalizedTitleContent();
            s_NavMeshEditorWindow = this;
            EditorApplication.searchChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(this.Repaint));
            SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc) Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
            this.UpdateSelectedAgentAndObstacleState();
            base.Repaint();
        }

        public void OnGUI()
        {
            EditorGUILayout.Space();
            this.ModeToggle();
            EditorGUILayout.Space();
            this.InitProjectSettings();
            this.m_ScrollPos = EditorGUILayout.BeginScrollView(this.m_ScrollPos, new GUILayoutOption[0]);
            switch (this.m_Mode)
            {
                case Mode.AgentSettings:
                    this.AgentSettings();
                    break;

                case Mode.AreaSettings:
                    this.AreaSettings();
                    break;

                case Mode.SceneBakeSettings:
                    this.SceneBakeSettings();
                    break;

                case Mode.ObjectSettings:
                    ObjectSettings();
                    break;
            }
            EditorGUILayout.EndScrollView();
        }

        private void OnInspectorUpdate()
        {
            if (this.m_SelectedNavMeshAgentCount > 0)
            {
                if (this.m_HasPendingAgentDebugInfo != NavMeshVisualizationSettings.hasPendingAgentDebugInfo)
                {
                    if (!this.m_HasRepaintedForPendingAgentDebugInfo)
                    {
                        this.m_HasRepaintedForPendingAgentDebugInfo = true;
                        RepaintSceneAndGameViews();
                    }
                }
                else
                {
                    this.m_HasRepaintedForPendingAgentDebugInfo = false;
                }
            }
        }

        public void OnSceneViewGUI(SceneView sceneView)
        {
            if (NavMeshVisualizationSettings.showNavigation != 0)
            {
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new SceneViewOverlay.WindowFunction(NavMeshEditorWindow.DisplayControls);
                }
                SceneViewOverlay.Window(new GUIContent("Navmesh Display"), <>f__mg$cache0, 300, SceneViewOverlay.WindowDisplayOption.OneWindowPerTarget);
                if (this.m_SelectedNavMeshAgentCount > 0)
                {
                    if (<>f__mg$cache1 == null)
                    {
                        <>f__mg$cache1 = new SceneViewOverlay.WindowFunction(NavMeshEditorWindow.DisplayAgentControls);
                    }
                    SceneViewOverlay.Window(new GUIContent("Agent Display"), <>f__mg$cache1, 300, SceneViewOverlay.WindowDisplayOption.OneWindowPerTarget);
                }
                if (this.m_SelectedNavMeshObstacleCount > 0)
                {
                    if (<>f__mg$cache2 == null)
                    {
                        <>f__mg$cache2 = new SceneViewOverlay.WindowFunction(NavMeshEditorWindow.DisplayObstacleControls);
                    }
                    SceneViewOverlay.Window(new GUIContent("Obstacle Display"), <>f__mg$cache2, 300, SceneViewOverlay.WindowDisplayOption.OneWindowPerTarget);
                }
            }
        }

        private void OnSelectionChange()
        {
            this.UpdateSelectedAgentAndObstacleState();
            this.m_ScrollPos = Vector2.zero;
            if (this.m_Mode == Mode.ObjectSettings)
            {
                base.Repaint();
            }
        }

        public static void OpenAgentSettings(int agentTypeID)
        {
            SetupWindow();
            if (s_NavMeshEditorWindow != null)
            {
                s_NavMeshEditorWindow.m_Mode = Mode.AgentSettings;
                s_NavMeshEditorWindow.InitProjectSettings();
                s_NavMeshEditorWindow.InitAgents();
                s_NavMeshEditorWindow.m_AgentsList.index = -1;
                for (int i = 0; i < s_NavMeshEditorWindow.m_Agents.arraySize; i++)
                {
                    if (s_NavMeshEditorWindow.m_Agents.GetArrayElementAtIndex(i).FindPropertyRelative("agentTypeID").intValue == agentTypeID)
                    {
                        s_NavMeshEditorWindow.m_AgentsList.index = i;
                        break;
                    }
                }
            }
        }

        public static void OpenAreaSettings()
        {
            SetupWindow();
            if (s_NavMeshEditorWindow != null)
            {
                s_NavMeshEditorWindow.m_Mode = Mode.AreaSettings;
                s_NavMeshEditorWindow.InitProjectSettings();
                s_NavMeshEditorWindow.InitAgents();
            }
        }

        private void RemoveAgent(ReorderableList list)
        {
            SerializedProperty arrayElementAtIndex = this.m_Agents.GetArrayElementAtIndex(list.index);
            if (arrayElementAtIndex != null)
            {
                SerializedProperty property2 = arrayElementAtIndex.FindPropertyRelative("agentTypeID");
                if ((property2 != null) && (property2.intValue != 0))
                {
                    this.m_SettingNames.DeleteArrayElementAtIndex(list.index);
                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                }
            }
        }

        private static void RepaintSceneAndGameViews()
        {
            SceneView.RepaintAll();
            foreach (GameView view in UnityEngine.Resources.FindObjectsOfTypeAll(typeof(GameView)))
            {
                view.Repaint();
            }
        }

        private void ResetBakeSettings()
        {
            Unsupported.SmartReset(UnityEditor.AI.NavMeshBuilder.navMeshSettingsObject);
        }

        private void SceneBakeSettings()
        {
            EditorGUILayout.HelpBox("Legacy Feature - Applies only to statically baked per-scene NavMesh", MessageType.Warning);
            if ((this.m_SettingsObject == null) || (this.m_SettingsObject.targetObject == null))
            {
                this.InitSceneBakeSettings();
            }
            this.m_SettingsObject.Update();
            EditorGUILayout.LabelField(s_Styles.m_AgentSizeHeader, EditorStyles.boldLabel, new GUILayoutOption[0]);
            NavMeshEditorHelpers.DrawAgentDiagram(EditorGUILayout.GetControlRect(false, 120f, new GUILayoutOption[0]), this.m_AgentRadius.floatValue, this.m_AgentHeight.floatValue, this.m_AgentClimb.floatValue, this.m_AgentSlope.floatValue);
            float num = EditorGUILayout.FloatField(s_Styles.m_AgentRadiusContent, this.m_AgentRadius.floatValue, new GUILayoutOption[0]);
            if ((num >= 0.001f) && !Mathf.Approximately(num - this.m_AgentRadius.floatValue, 0f))
            {
                this.m_AgentRadius.floatValue = num;
                if (!this.m_ManualCellSize.boolValue)
                {
                    this.m_CellSize.floatValue = (2f * this.m_AgentRadius.floatValue) / 6f;
                }
            }
            if ((this.m_AgentRadius.floatValue < 0.05f) && !this.m_ManualCellSize.boolValue)
            {
                EditorGUILayout.HelpBox("The agent radius you've set is really small, this can slow down the build.\nIf you intended to allow the agent to move close to the borders and walls, please adjust voxel size in advaced settings to ensure correct bake.", MessageType.Warning);
            }
            float num2 = EditorGUILayout.FloatField(s_Styles.m_AgentHeightContent, this.m_AgentHeight.floatValue, new GUILayoutOption[0]);
            if ((num2 >= 0.001f) && !Mathf.Approximately(num2 - this.m_AgentHeight.floatValue, 0f))
            {
                this.m_AgentHeight.floatValue = num2;
            }
            EditorGUILayout.Slider(this.m_AgentSlope, 0f, 60f, s_Styles.m_AgentSlopeContent, new GUILayoutOption[0]);
            if (this.m_AgentSlope.floatValue > 60f)
            {
                EditorGUILayout.HelpBox("The maximum slope should be set to less than " + 60f + " degrees to prevent NavMesh build artifacts on slopes. ", MessageType.Warning);
            }
            float num3 = EditorGUILayout.FloatField(s_Styles.m_AgentClimbContent, this.m_AgentClimb.floatValue, new GUILayoutOption[0]);
            if ((num3 >= 0f) && !Mathf.Approximately(this.m_AgentClimb.floatValue - num3, 0f))
            {
                this.m_AgentClimb.floatValue = num3;
            }
            if (this.m_AgentClimb.floatValue > this.m_AgentHeight.floatValue)
            {
                EditorGUILayout.HelpBox("Step height should be less than agent height.\nClamping step height to " + this.m_AgentHeight.floatValue + " internally when baking.", MessageType.Warning);
            }
            float floatValue = this.m_CellSize.floatValue;
            float num5 = floatValue * 0.5f;
            int num6 = (int) Mathf.Ceil(this.m_AgentClimb.floatValue / num5);
            float num7 = Mathf.Tan((this.m_AgentSlope.floatValue / 180f) * 3.141593f) * floatValue;
            int num8 = (int) Mathf.Ceil((num7 * 2f) / num5);
            if (num8 > num6)
            {
                float f = (num6 * num5) / (floatValue * 2f);
                float num10 = (Mathf.Atan(f) / 3.141593f) * 180f;
                float num11 = (num8 - 1) * num5;
                EditorGUILayout.HelpBox("Step Height conflicts with Max Slope. This makes some slopes unwalkable.\nConsider decreasing Max Slope to < " + num10.ToString("0.0") + " degrees.\nOr, increase Step Height to > " + num11.ToString("0.00") + ".", MessageType.Warning);
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(s_Styles.m_OffmeshHeader, EditorStyles.boldLabel, new GUILayoutOption[0]);
            float num12 = EditorGUILayout.FloatField(s_Styles.m_AgentDropContent, this.m_LedgeDropHeight.floatValue, new GUILayoutOption[0]);
            if ((num12 >= 0f) && !Mathf.Approximately(num12 - this.m_LedgeDropHeight.floatValue, 0f))
            {
                this.m_LedgeDropHeight.floatValue = num12;
            }
            float num13 = EditorGUILayout.FloatField(s_Styles.m_AgentJumpContent, this.m_MaxJumpAcrossDistance.floatValue, new GUILayoutOption[0]);
            if ((num13 >= 0f) && !Mathf.Approximately(num13 - this.m_MaxJumpAcrossDistance.floatValue, 0f))
            {
                this.m_MaxJumpAcrossDistance.floatValue = num13;
            }
            EditorGUILayout.Space();
            this.m_Advanced = GUILayout.Toggle(this.m_Advanced, s_Styles.m_AdvancedHeader, EditorStyles.foldout, new GUILayoutOption[0]);
            if (this.m_Advanced)
            {
                EditorGUI.indentLevel++;
                bool flag = EditorGUILayout.Toggle(s_Styles.m_ManualCellSizeContent, this.m_ManualCellSize.boolValue, new GUILayoutOption[0]);
                if (flag != this.m_ManualCellSize.boolValue)
                {
                    this.m_ManualCellSize.boolValue = flag;
                    if (!flag)
                    {
                        this.m_CellSize.floatValue = (2f * this.m_AgentRadius.floatValue) / 6f;
                    }
                }
                EditorGUI.indentLevel++;
                using (new EditorGUI.DisabledScope(!this.m_ManualCellSize.boolValue))
                {
                    float num14 = EditorGUILayout.FloatField(s_Styles.m_CellSizeContent, this.m_CellSize.floatValue, new GUILayoutOption[0]);
                    if ((num14 > 0f) && !Mathf.Approximately(num14 - this.m_CellSize.floatValue, 0f))
                    {
                        this.m_CellSize.floatValue = Math.Max(0.01f, num14);
                    }
                    if (num14 < 0.01f)
                    {
                        EditorGUILayout.HelpBox("The voxel size should be larger than 0.01.", MessageType.Warning);
                    }
                    float num15 = (this.m_CellSize.floatValue <= 0f) ? 0f : (this.m_AgentRadius.floatValue / this.m_CellSize.floatValue);
                    EditorGUILayout.LabelField(" ", num15.ToString("0.00") + " voxels per agent radius", EditorStyles.miniLabel, new GUILayoutOption[0]);
                    if (this.m_ManualCellSize.boolValue)
                    {
                        float num16 = this.m_CellSize.floatValue * 0.5f;
                        if (((int) Mathf.Floor(this.m_AgentHeight.floatValue / num16)) > 250)
                        {
                            EditorGUILayout.HelpBox("The number of voxels per agent height is too high. This will reduce the accuracy of the navmesh. Consider using voxel size of at least " + (((this.m_AgentHeight.floatValue / 250f) / 0.5f)).ToString("0.000") + ".", MessageType.Warning);
                        }
                        if (num15 < 1f)
                        {
                            EditorGUILayout.HelpBox("The number of voxels per agent radius is too small. The agent may not avoid walls and ledges properly. Consider using a voxel size less than " + ((this.m_AgentRadius.floatValue / 2f)).ToString("0.000") + " (2 voxels per agent radius).", MessageType.Warning);
                        }
                        else if (num15 > 8f)
                        {
                            EditorGUILayout.HelpBox("The number of voxels per agent radius is too high. It can cause excessive build times. Consider using voxel size closer to " + ((this.m_AgentRadius.floatValue / 8f)).ToString("0.000") + " (8 voxels per radius).", MessageType.Warning);
                        }
                    }
                    if (this.m_ManualCellSize.boolValue)
                    {
                        EditorGUILayout.HelpBox("Voxel size controls how accurately the navigation mesh is generated from the level geometry. A good voxel size is 2-4 voxels per agent radius. Making voxel size smaller will increase build time.", MessageType.None);
                    }
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
                float num20 = EditorGUILayout.FloatField(s_Styles.m_MinRegionAreaContent, this.m_MinRegionArea.floatValue, new GUILayoutOption[0]);
                if ((num20 >= 0f) && (num20 != this.m_MinRegionArea.floatValue))
                {
                    this.m_MinRegionArea.floatValue = num20;
                }
                EditorGUILayout.Space();
                bool flag2 = EditorGUILayout.Toggle(s_Styles.m_AgentPlacementContent, this.m_AccuratePlacement.boolValue, new GUILayoutOption[0]);
                if (flag2 != this.m_AccuratePlacement.boolValue)
                {
                    this.m_AccuratePlacement.boolValue = flag2;
                }
                EditorGUI.indentLevel--;
            }
            this.m_SettingsObject.ApplyModifiedProperties();
            BakeButtons();
        }

        private static bool SelectionHasChildren()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = obj => obj.transform.childCount > 0;
            }
            return Enumerable.Any<GameObject>(Selection.gameObjects, <>f__am$cache0);
        }

        private static void SetNavMeshArea(int area, bool includeChildren)
        {
            List<GameObject> objects = GetObjects(includeChildren);
            if (objects.Count > 0)
            {
                Undo.RecordObjects(objects.ToArray(), "Change NavMesh area");
                foreach (GameObject obj2 in objects)
                {
                    GameObjectUtility.SetNavMeshArea(obj2, area);
                }
            }
        }

        [UnityEditor.MenuItem("Window/Navigation", false, 0x834)]
        public static void SetupWindow()
        {
            System.Type[] desiredDockNextTo = new System.Type[] { typeof(InspectorWindow) };
            EditorWindow.GetWindow<NavMeshEditorWindow>(desiredDockNextTo).minSize = new Vector2(300f, 360f);
        }

        private void UpdateSelectedAgentAndObstacleState()
        {
            UnityEngine.Object[] filtered = Selection.GetFiltered(typeof(NavMeshAgent), UnityEditor.SelectionMode.Editable | UnityEditor.SelectionMode.ExcludePrefab);
            UnityEngine.Object[] objArray2 = Selection.GetFiltered(typeof(NavMeshObstacle), UnityEditor.SelectionMode.Editable | UnityEditor.SelectionMode.ExcludePrefab);
            this.m_SelectedNavMeshAgentCount = filtered.Length;
            this.m_SelectedNavMeshObstacleCount = objArray2.Length;
        }

        private enum Mode
        {
            AgentSettings,
            AreaSettings,
            SceneBakeSettings,
            ObjectSettings
        }

        private class Styles
        {
            public readonly GUIContent m_AdvancedHeader = new GUIContent("Advanced");
            public readonly GUIContent m_AgentClimbContent = EditorGUIUtility.TextContent("Step Height|The height of discontinuities in the level the agent can climb over (i.e. steps and stairs).");
            public readonly GUIContent m_AgentDropContent = EditorGUIUtility.TextContent("Drop Height|Maximum agent drop height.");
            public readonly GUIContent m_AgentHeightContent = EditorGUIUtility.TextContent("Agent Height|How much vertical clearance space must exist.");
            public readonly GUIContent m_AgentJumpContent = EditorGUIUtility.TextContent("Jump Distance|Maximum agent jump distance.");
            public readonly GUIContent m_AgentPlacementContent = EditorGUIUtility.TextContent("Height Mesh|Generate an accurate height mesh for precise agent placement (slower).");
            public readonly GUIContent m_AgentRadiusContent = EditorGUIUtility.TextContent("Agent Radius|How close to the walls navigation mesh exist.");
            public readonly GUIContent m_AgentSizeHeader = new GUIContent("Baked Agent Size");
            public readonly GUIContent m_AgentSlopeContent = EditorGUIUtility.TextContent("Max Slope|Maximum slope the agent can walk up.");
            public readonly GUIContent m_AgentTypesHeader = new GUIContent("Agent Types");
            public readonly GUIContent m_CellSizeContent = EditorGUIUtility.TextContent("Voxel Size|Specifies at the voxelization resolution at which the NavMesh is build.");
            public readonly GUIContent m_CostLabel = new GUIContent("Cost");
            public readonly GUIContent m_ManualCellSizeContent = EditorGUIUtility.TextContent("Manual Voxel Size|Enable to set voxel size manually.");
            public readonly GUIContent m_MinRegionAreaContent = EditorGUIUtility.TextContent("Min Region Area|Minimum area that a navmesh region can be.");
            public readonly GUIContent[] m_ModeToggles = new GUIContent[] { EditorGUIUtility.TextContent("Agents|Navmesh agent settings."), EditorGUIUtility.TextContent("Areas|Navmesh area settings."), EditorGUIUtility.TextContent("Bake|Navmesh bake settings."), EditorGUIUtility.TextContent("Object|Bake settings for the currently selected object.") };
            public readonly GUIContent m_NameLabel = new GUIContent("Name");
            public readonly GUIContent m_OffmeshHeader = new GUIContent("Generated Off Mesh Links");
        }
    }
}

