namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.AnimatedValues;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Events;

    [CustomEditor(typeof(LODGroup))]
    internal class LODGroupEditor : Editor
    {
        [CompilerGenerated]
        private static Func<Vector3, Vector2> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<UnityEngine.Object, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<UnityEngine.Object, GameObject> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<UnityEngine.Object, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<UnityEngine.Object, Renderer> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<LODGroupGUI.LODInfo, bool> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<LODGroupGUI.LODInfo, int> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<LODGroupGUI.LODInfo, bool> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<LODGroupGUI.LODInfo, int> <>f__am$cache9;
        [CompilerGenerated]
        private static Func<UnityEngine.Object, bool> <>f__am$cacheA;
        [CompilerGenerated]
        private static Func<UnityEngine.Object, GameObject> <>f__am$cacheB;
        private const string kFadeTransitionWidthDataPath = "m_LODs.Array.data[{0}].fadeTransitionWidth";
        private const string kLODDataPath = "m_LODs.Array.data[{0}]";
        private const string kPixelHeightDataPath = "m_LODs.Array.data[{0}].screenRelativeHeight";
        private const string kRenderRootPath = "m_LODs.Array.data[{0}].renderers";
        private static readonly GUIContent[] kSLightIcons = new GUIContent[2];
        private SerializedProperty m_AnimateCrossFading;
        private readonly int m_CameraSliderId = "LODCameraIDHash".GetHashCode();
        private SerializedProperty m_FadeMode;
        private bool m_IsPrefab;
        private Vector3 m_LastCameraPos = Vector3.zero;
        private LODGroup m_LODGroup;
        private SerializedProperty m_LODs;
        private readonly int m_LODSliderId = "LODSliderIDHash".GetHashCode();
        private int m_NumberOfLODs;
        private Vector2 m_PreviewDir = new Vector2(0f, -20f);
        private PreviewRenderUtility m_PreviewUtility;
        private int m_SelectedLOD = -1;
        private int m_SelectedLODSlider = -1;
        private AnimBool m_ShowAnimateCrossFading = new AnimBool();
        private AnimBool m_ShowFadeTransitionWidth = new AnimBool();

        private void AddGameObjectRenderers(IEnumerable<Renderer> toAdd, bool add)
        {
            SerializedProperty property = base.serializedObject.FindProperty($"m_LODs.Array.data[{this.activeLOD}].renderers");
            if (!add)
            {
                property.ClearArray();
            }
            List<Renderer> list = new List<Renderer>();
            for (int i = 0; i < property.arraySize; i++)
            {
                Renderer objectReferenceValue = property.GetArrayElementAtIndex(i).FindPropertyRelative("renderer").objectReferenceValue as Renderer;
                if (objectReferenceValue != null)
                {
                    list.Add(objectReferenceValue);
                }
            }
            foreach (Renderer renderer2 in toAdd)
            {
                if (!list.Contains(renderer2))
                {
                    property.arraySize++;
                    property.GetArrayElementAtIndex(property.arraySize - 1).FindPropertyRelative("renderer").objectReferenceValue = renderer2;
                    list.Add(renderer2);
                }
            }
            base.serializedObject.ApplyModifiedProperties();
            this.m_LODGroup.RecalculateBounds();
        }

        private void BeginLODDrag(float desiredPercentage, LODGroup group)
        {
            if (((SceneView.lastActiveSceneView != null) && (SceneView.lastActiveSceneView.camera != null)) && !this.m_IsPrefab)
            {
                UpdateCamera(desiredPercentage, group);
                SceneView.lastActiveSceneView.ClearSearchFilter();
                SceneView.lastActiveSceneView.SetSceneViewFiltering(true);
                HierarchyProperty.FilterSingleSceneObject(group.gameObject.GetInstanceID(), false);
                SceneView.RepaintAll();
            }
        }

        private static Rect CalculateScreenRect(IEnumerable<Vector3> points)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = p => HandleUtility.WorldToGUIPoint(p);
            }
            List<Vector2> list = Enumerable.Select<Vector3, Vector2>(points, <>f__am$cache0).ToList<Vector2>();
            Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
            foreach (Vector2 vector3 in list)
            {
                vector.x = (vector3.x >= vector.x) ? vector.x : vector3.x;
                vector2.x = (vector3.x <= vector2.x) ? vector2.x : vector3.x;
                vector.y = (vector3.y >= vector.y) ? vector.y : vector3.y;
                vector2.y = (vector3.y <= vector2.y) ? vector2.y : vector3.y;
            }
            return new Rect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
        }

        private void DeletedLOD()
        {
            this.m_SelectedLOD--;
        }

        protected void DoRenderPreview()
        {
            if ((((this.m_PreviewUtility.m_RenderTexture.width > 0) && (this.m_PreviewUtility.m_RenderTexture.height > 0)) && (this.m_NumberOfLODs > 0)) && (this.activeLOD >= 0))
            {
                Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
                bool flag = false;
                List<MeshFilter> list = new List<MeshFilter>();
                SerializedProperty property = base.serializedObject.FindProperty($"m_LODs.Array.data[{this.activeLOD}].renderers");
                for (int i = 0; i < property.arraySize; i++)
                {
                    Renderer objectReferenceValue = property.GetArrayElementAtIndex(i).FindPropertyRelative("renderer").objectReferenceValue as Renderer;
                    if (objectReferenceValue != null)
                    {
                        MeshFilter component = objectReferenceValue.GetComponent<MeshFilter>();
                        if (((component != null) && (component.sharedMesh != null)) && (component.sharedMesh.subMeshCount > 0))
                        {
                            list.Add(component);
                        }
                        if (!flag)
                        {
                            bounds = objectReferenceValue.bounds;
                            flag = true;
                        }
                        else
                        {
                            bounds.Encapsulate(objectReferenceValue.bounds);
                        }
                    }
                }
                if (flag)
                {
                    float num3 = bounds.extents.magnitude * 10f;
                    Vector2 vector2 = (Vector2) -(this.m_PreviewDir / 100f);
                    float introduced18 = Mathf.Sin(vector2.x);
                    float y = Mathf.Sin(vector2.y);
                    float introduced20 = Mathf.Cos(vector2.x);
                    this.m_PreviewUtility.m_Camera.transform.position = bounds.center + ((Vector3) (new Vector3(introduced18 * Mathf.Cos(vector2.y), y, introduced20 * Mathf.Cos(vector2.y)) * num3));
                    this.m_PreviewUtility.m_Camera.transform.LookAt(bounds.center);
                    this.m_PreviewUtility.m_Camera.nearClipPlane = 0.05f;
                    this.m_PreviewUtility.m_Camera.farClipPlane = 1000f;
                    this.m_PreviewUtility.m_Light[0].intensity = 1f;
                    this.m_PreviewUtility.m_Light[0].transform.rotation = Quaternion.Euler(50f, 50f, 0f);
                    this.m_PreviewUtility.m_Light[1].intensity = 1f;
                    Color ambient = new Color(0.2f, 0.2f, 0.2f, 0f);
                    InternalEditorUtility.SetCustomLighting(this.m_PreviewUtility.m_Light, ambient);
                    foreach (MeshFilter filter2 in list)
                    {
                        for (int j = 0; j < filter2.sharedMesh.subMeshCount; j++)
                        {
                            if (j < filter2.GetComponent<Renderer>().sharedMaterials.Length)
                            {
                                Matrix4x4 matrix = Matrix4x4.TRS(filter2.transform.position, filter2.transform.rotation, filter2.transform.localScale);
                                this.m_PreviewUtility.DrawMesh(filter2.sharedMesh, matrix, filter2.GetComponent<Renderer>().sharedMaterials[j], j);
                            }
                        }
                    }
                    bool fog = RenderSettings.fog;
                    Unsupported.SetRenderSettingsUseFogNoDirty(false);
                    this.m_PreviewUtility.m_Camera.Render();
                    Unsupported.SetRenderSettingsUseFogNoDirty(fog);
                    InternalEditorUtility.RemoveCustomLighting();
                }
            }
        }

        private void DrawLODLevelSlider(Rect sliderPosition, List<LODGroupGUI.LODInfo> lods)
        {
            int controlID = GUIUtility.GetControlID(this.m_LODSliderId, FocusType.Passive);
            int num2 = GUIUtility.GetControlID(this.m_CameraSliderId, FocusType.Passive);
            Event current = Event.current;
            EventType typeForControl = current.GetTypeForControl(controlID);
            switch (typeForControl)
            {
                case EventType.MouseDown:
                {
                    if ((current.button != 1) || !sliderPosition.Contains(current.mousePosition))
                    {
                        Rect rect = sliderPosition;
                        rect.x -= 5f;
                        rect.width += 10f;
                        if (rect.Contains(current.mousePosition))
                        {
                            current.Use();
                            GUIUtility.hotControl = controlID;
                            bool flag3 = false;
                            if (<>f__am$cache6 == null)
                            {
                                <>f__am$cache6 = lod => lod.ScreenPercent > 0.5f;
                            }
                            if (<>f__am$cache7 == null)
                            {
                                <>f__am$cache7 = x => x.LODLevel;
                            }
                            IOrderedEnumerable<LODGroupGUI.LODInfo> collection = Enumerable.OrderByDescending<LODGroupGUI.LODInfo, int>(Enumerable.Where<LODGroupGUI.LODInfo>(lods, <>f__am$cache6), <>f__am$cache7);
                            if (<>f__am$cache8 == null)
                            {
                                <>f__am$cache8 = lod => lod.ScreenPercent <= 0.5f;
                            }
                            if (<>f__am$cache9 == null)
                            {
                                <>f__am$cache9 = x => x.LODLevel;
                            }
                            IOrderedEnumerable<LODGroupGUI.LODInfo> enumerable2 = Enumerable.OrderBy<LODGroupGUI.LODInfo, int>(Enumerable.Where<LODGroupGUI.LODInfo>(lods, <>f__am$cache8), <>f__am$cache9);
                            List<LODGroupGUI.LODInfo> list = new List<LODGroupGUI.LODInfo>();
                            list.AddRange(collection);
                            list.AddRange(enumerable2);
                            foreach (LODGroupGUI.LODInfo info2 in list)
                            {
                                if (info2.m_ButtonPosition.Contains(current.mousePosition))
                                {
                                    this.m_SelectedLODSlider = info2.LODLevel;
                                    flag3 = true;
                                    this.BeginLODDrag(info2.RawScreenPercent + 0.001f, this.m_LODGroup);
                                    break;
                                }
                            }
                            if (!flag3)
                            {
                                foreach (LODGroupGUI.LODInfo info3 in lods)
                                {
                                    if (info3.m_RangePosition.Contains(current.mousePosition))
                                    {
                                        this.m_SelectedLOD = info3.LODLevel;
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    }
                    float cameraPercent = LODGroupGUI.GetCameraPercent(current.mousePosition, sliderPosition);
                    GenericMenu menu = new GenericMenu();
                    if (lods.Count < 8)
                    {
                        menu.AddItem(EditorGUIUtility.TextContent("Insert Before"), false, new GenericMenu.MenuFunction(new LODAction(lods, cameraPercent, current.mousePosition, this.m_LODs, null).InsertLOD));
                    }
                    else
                    {
                        menu.AddDisabledItem(EditorGUIUtility.TextContent("Insert Before"));
                    }
                    bool flag = true;
                    if ((lods.Count > 0) && (lods[lods.Count - 1].RawScreenPercent < cameraPercent))
                    {
                        flag = false;
                    }
                    if (flag)
                    {
                        menu.AddDisabledItem(EditorGUIUtility.TextContent("Delete"));
                    }
                    else
                    {
                        menu.AddItem(EditorGUIUtility.TextContent("Delete"), false, new GenericMenu.MenuFunction(new LODAction(lods, cameraPercent, current.mousePosition, this.m_LODs, new LODAction.Callback(this.DeletedLOD)).DeleteLOD));
                    }
                    menu.ShowAsContext();
                    bool flag2 = false;
                    foreach (LODGroupGUI.LODInfo info in lods)
                    {
                        if (info.m_RangePosition.Contains(current.mousePosition))
                        {
                            this.m_SelectedLOD = info.LODLevel;
                            flag2 = true;
                            break;
                        }
                    }
                    if (!flag2)
                    {
                        this.m_SelectedLOD = -1;
                    }
                    current.Use();
                    break;
                }
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        this.m_SelectedLODSlider = -1;
                        this.EndLODDrag();
                        current.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (((GUIUtility.hotControl == controlID) && (this.m_SelectedLODSlider >= 0)) && (lods[this.m_SelectedLODSlider] != null))
                    {
                        current.Use();
                        float desiredPercentage = LODGroupGUI.GetCameraPercent(current.mousePosition, sliderPosition);
                        LODGroupGUI.SetSelectedLODLevelPercentage(desiredPercentage - 0.001f, this.m_SelectedLODSlider, lods);
                        base.serializedObject.FindProperty($"m_LODs.Array.data[{lods[this.m_SelectedLODSlider].LODLevel}].screenRelativeHeight").floatValue = lods[this.m_SelectedLODSlider].RawScreenPercent;
                        this.UpdateLODDrag(desiredPercentage, this.m_LODGroup);
                    }
                    break;

                case EventType.Repaint:
                    LODGroupGUI.DrawLODSlider(sliderPosition, lods, this.activeLOD);
                    break;

                case EventType.DragUpdated:
                case EventType.DragPerform:
                {
                    int lODLevel = -2;
                    foreach (LODGroupGUI.LODInfo info4 in lods)
                    {
                        if (info4.m_RangePosition.Contains(current.mousePosition))
                        {
                            lODLevel = info4.LODLevel;
                            break;
                        }
                    }
                    if ((lODLevel == -2) && LODGroupGUI.GetCulledBox(sliderPosition, (lods.Count <= 0) ? 1f : lods[lods.Count - 1].ScreenPercent).Contains(current.mousePosition))
                    {
                        lODLevel = -1;
                    }
                    if (lODLevel >= -1)
                    {
                        this.m_SelectedLOD = lODLevel;
                        if (DragAndDrop.objectReferences.Count<UnityEngine.Object>() > 0)
                        {
                            DragAndDrop.visualMode = !this.m_IsPrefab ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.None;
                            if (current.type == EventType.DragPerform)
                            {
                                if (<>f__am$cacheA == null)
                                {
                                    <>f__am$cacheA = go => go is GameObject;
                                }
                                if (<>f__am$cacheB == null)
                                {
                                    <>f__am$cacheB = go => go as GameObject;
                                }
                                IEnumerable<GameObject> selectedGameObjects = Enumerable.Select<UnityEngine.Object, GameObject>(Enumerable.Where<UnityEngine.Object>(DragAndDrop.objectReferences, <>f__am$cacheA), <>f__am$cacheB);
                                IEnumerable<Renderer> renderers = this.GetRenderers(selectedGameObjects, true);
                                if (lODLevel == -1)
                                {
                                    this.m_LODs.arraySize++;
                                    SerializedProperty property2 = base.serializedObject.FindProperty($"m_LODs.Array.data[{lods.Count}].screenRelativeHeight");
                                    if (lods.Count == 0)
                                    {
                                        property2.floatValue = 0.5f;
                                    }
                                    else
                                    {
                                        SerializedProperty property3 = base.serializedObject.FindProperty($"m_LODs.Array.data[{lods.Count - 1}].screenRelativeHeight");
                                        property2.floatValue = property3.floatValue / 2f;
                                    }
                                    this.m_SelectedLOD = lods.Count;
                                    this.AddGameObjectRenderers(renderers, false);
                                }
                                else
                                {
                                    this.AddGameObjectRenderers(renderers, true);
                                }
                                DragAndDrop.AcceptDrag();
                            }
                        }
                        current.Use();
                    }
                    break;
                }
                default:
                    if (typeForControl == EventType.DragExited)
                    {
                        current.Use();
                    }
                    break;
            }
            if (((SceneView.lastActiveSceneView != null) && (SceneView.lastActiveSceneView.camera != null)) && !this.m_IsPrefab)
            {
                Camera camera = SceneView.lastActiveSceneView.camera;
                float activeRelativeScreenSize = LODUtility.CalculateVisualizationData(camera, this.m_LODGroup, -1).activeRelativeScreenSize;
                float num7 = LODGroupGUI.DelinearizeScreenPercentage(activeRelativeScreenSize);
                Vector3 vector = LODUtility.CalculateWorldReferencePoint(this.m_LODGroup);
                Vector3 vector3 = SceneView.lastActiveSceneView.camera.transform.position - vector;
                Vector3 normalized = vector3.normalized;
                if (Vector3.Dot(camera.transform.forward, normalized) > 0f)
                {
                    num7 = 1f;
                }
                Rect rect3 = LODGroupGUI.CalcLODButton(sliderPosition, Mathf.Clamp01(num7));
                Rect position = new Rect(rect3.center.x - 15f, rect3.y - 25f, 32f, 32f);
                Rect rect5 = new Rect(rect3.center.x - 1f, rect3.y, 2f, rect3.height);
                Rect rect6 = new Rect(position.center.x - 5f, rect5.yMax, 35f, 20f);
                switch (current.GetTypeForControl(num2))
                {
                    case EventType.MouseDown:
                        if (position.Contains(current.mousePosition))
                        {
                            current.Use();
                            float num8 = LODGroupGUI.GetCameraPercent(current.mousePosition, sliderPosition);
                            this.UpdateSelectedLODFromCamera(lods, num8);
                            GUIUtility.hotControl = num2;
                            this.BeginLODDrag(num8, this.m_LODGroup);
                        }
                        break;

                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == num2)
                        {
                            this.EndLODDrag();
                            GUIUtility.hotControl = 0;
                            current.Use();
                        }
                        break;

                    case EventType.MouseDrag:
                        if (GUIUtility.hotControl == num2)
                        {
                            current.Use();
                            float num9 = LODGroupGUI.GetCameraPercent(current.mousePosition, sliderPosition);
                            this.UpdateSelectedLODFromCamera(lods, num9);
                            this.UpdateLODDrag(num9, this.m_LODGroup);
                        }
                        break;

                    case EventType.Repaint:
                    {
                        Color backgroundColor = GUI.backgroundColor;
                        GUI.backgroundColor = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 0.8f);
                        LODGroupGUI.Styles.m_LODCameraLine.Draw(rect5, false, false, false, false);
                        GUI.backgroundColor = backgroundColor;
                        GUI.Label(position, LODGroupGUI.Styles.m_CameraIcon, GUIStyle.none);
                        LODGroupGUI.Styles.m_LODSliderText.Draw(rect6, $"{Mathf.Clamp01(activeRelativeScreenSize) * 100f:0}%", false, false, false, false);
                        break;
                    }
                }
            }
        }

        private void DrawRendererButton(Rect position, int rendererIndex)
        {
            SerializedProperty property = base.serializedObject.FindProperty($"m_LODs.Array.data[{this.activeLOD}].renderers");
            Renderer objectReferenceValue = property.GetArrayElementAtIndex(rendererIndex).FindPropertyRelative("renderer").objectReferenceValue as Renderer;
            Rect rect = new Rect(position.xMax - 20f, position.yMax - 20f, 20f, 20f);
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.Repaint:
                    if (objectReferenceValue != null)
                    {
                        GUIContent content;
                        MeshFilter component = objectReferenceValue.GetComponent<MeshFilter>();
                        if ((component != null) && (component.sharedMesh != null))
                        {
                            content = new GUIContent(AssetPreview.GetAssetPreview(component.sharedMesh), objectReferenceValue.gameObject.name);
                        }
                        else if (objectReferenceValue is SkinnedMeshRenderer)
                        {
                            content = new GUIContent(AssetPreview.GetAssetPreview((objectReferenceValue as SkinnedMeshRenderer).sharedMesh), objectReferenceValue.gameObject.name);
                        }
                        else
                        {
                            content = new GUIContent(ObjectNames.NicifyVariableName(objectReferenceValue.GetType().Name), objectReferenceValue.gameObject.name);
                        }
                        LODGroupGUI.Styles.m_LODBlackBox.Draw(position, GUIContent.none, false, false, false, false);
                        LODGroupGUI.Styles.m_LODRendererButton.Draw(new Rect(position.x + 2f, position.y + 2f, position.width - 4f, position.height - 4f), content, false, false, false, false);
                    }
                    else
                    {
                        LODGroupGUI.Styles.m_LODBlackBox.Draw(position, GUIContent.none, false, false, false, false);
                        LODGroupGUI.Styles.m_LODRendererButton.Draw(position, "<Empty>", false, false, false, false);
                    }
                    if (!this.m_IsPrefab)
                    {
                        LODGroupGUI.Styles.m_LODBlackBox.Draw(rect, GUIContent.none, false, false, false, false);
                        LODGroupGUI.Styles.m_LODRendererRemove.Draw(rect, LODGroupGUI.Styles.m_IconRendererMinus, false, false, false, false);
                    }
                    break;

                case EventType.MouseDown:
                    if (!this.m_IsPrefab && rect.Contains(current.mousePosition))
                    {
                        property.DeleteArrayElementAtIndex(rendererIndex);
                        current.Use();
                        base.serializedObject.ApplyModifiedProperties();
                        this.m_LODGroup.RecalculateBounds();
                    }
                    else if (position.Contains(current.mousePosition))
                    {
                        EditorGUIUtility.PingObject(objectReferenceValue);
                        current.Use();
                    }
                    break;
            }
        }

        private void DrawRenderersInfo(float availableWidth)
        {
            int num = Mathf.FloorToInt(availableWidth / 60f);
            Rect position = GUILayoutUtility.GetRect(LODGroupGUI.Styles.m_RendersTitle, LODGroupGUI.Styles.m_LODSliderTextSelected);
            if (Event.current.type == EventType.Repaint)
            {
                EditorStyles.label.Draw(position, LODGroupGUI.Styles.m_RendersTitle, false, false, false, false);
            }
            SerializedProperty property = base.serializedObject.FindProperty($"m_LODs.Array.data[{this.activeLOD}].renderers");
            int num2 = property.arraySize + 1;
            int num3 = Mathf.CeilToInt(((float) num2) / ((float) num));
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            Rect rect2 = GUILayoutUtility.GetRect((float) 0f, (float) (num3 * 60), options);
            Rect rect3 = rect2;
            GUI.Box(rect2, GUIContent.none);
            rect3.width -= 6f;
            rect3.x += 3f;
            float num4 = rect3.width / ((float) num);
            List<Rect> alreadyDrawn = new List<Rect>();
            for (int i = 0; i < num3; i++)
            {
                for (int j = 0; (j < num) && (((i * num) + j) < property.arraySize); j++)
                {
                    Rect item = new Rect((2f + rect3.x) + (j * num4), (2f + rect3.y) + (i * 60), num4 - 4f, 56f);
                    alreadyDrawn.Add(item);
                    this.DrawRendererButton(item, (i * num) + j);
                }
            }
            if (!this.m_IsPrefab)
            {
                int num7 = (num2 - 1) % num;
                int num8 = num3 - 1;
                this.HandleAddRenderer(new Rect((2f + rect3.x) + (num7 * num4), (2f + rect3.y) + (num8 * 60), num4 - 4f, 56f), alreadyDrawn, rect2);
            }
        }

        private void EndLODDrag()
        {
            if (((SceneView.lastActiveSceneView != null) && (SceneView.lastActiveSceneView.camera != null)) && !this.m_IsPrefab)
            {
                SceneView.lastActiveSceneView.SetSceneViewFiltering(false);
                SceneView.lastActiveSceneView.ClearSearchFilter();
                HierarchyProperty.ClearSceneObjectsFilter();
            }
        }

        private ModelImporter GetImporter() => 
            (AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(base.target))) as ModelImporter);

        public override string GetInfoString()
        {
            if (((SceneView.lastActiveSceneView == null) || (SceneView.lastActiveSceneView.camera == null)) || ((this.m_NumberOfLODs <= 0) || (this.activeLOD < 0)))
            {
                return "";
            }
            List<Material> source = new List<Material>();
            SerializedProperty property = base.serializedObject.FindProperty($"m_LODs.Array.data[{this.activeLOD}].renderers");
            for (int i = 0; i < property.arraySize; i++)
            {
                Renderer objectReferenceValue = property.GetArrayElementAtIndex(i).FindPropertyRelative("renderer").objectReferenceValue as Renderer;
                if (objectReferenceValue != null)
                {
                    source.AddRange(objectReferenceValue.sharedMaterials);
                }
            }
            LODVisualizationInformation information = LODUtility.CalculateVisualizationData(SceneView.lastActiveSceneView.camera, this.m_LODGroup, this.activeLOD);
            return ((this.activeLOD == -1) ? "LOD: culled" : $"{property.arraySize} Renderer(s)
{information.triangleCount} Triangle(s)
{source.Distinct<Material>().Count<Material>()} Material(s)");
        }

        private IEnumerable<Renderer> GetRenderers(IEnumerable<GameObject> selectedGameObjects, bool searchChildren)
        {
            if (EditorUtility.IsPersistent(this.m_LODGroup))
            {
                return new List<Renderer>();
            }
            IEnumerable<GameObject> first = from go in selectedGameObjects
                where go.transform.IsChildOf(this.m_LODGroup.transform)
                select go;
            IEnumerable<GameObject> source = from go in selectedGameObjects
                where !go.transform.IsChildOf(this.m_LODGroup.transform)
                select go;
            List<GameObject> second = new List<GameObject>();
            if ((source.Count<GameObject>() > 0) && EditorUtility.DisplayDialog("Reparent GameObjects", "Some objects are not children of the LODGroup GameObject. Do you want to reparent them and add them to the LODGroup?", "Yes, Reparent", "No, Use Only Existing Children"))
            {
                foreach (GameObject obj2 in source)
                {
                    if (EditorUtility.IsPersistent(obj2))
                    {
                        GameObject item = UnityEngine.Object.Instantiate<GameObject>(obj2);
                        if (item != null)
                        {
                            item.transform.parent = this.m_LODGroup.transform;
                            item.transform.localPosition = Vector3.zero;
                            item.transform.localRotation = Quaternion.identity;
                            second.Add(item);
                        }
                    }
                    else
                    {
                        obj2.transform.parent = this.m_LODGroup.transform;
                        second.Add(obj2);
                    }
                }
                first = first.Union<GameObject>(second);
            }
            List<Renderer> list2 = new List<Renderer>();
            foreach (GameObject obj4 in first)
            {
                if (searchChildren)
                {
                    list2.AddRange(obj4.GetComponentsInChildren<Renderer>());
                }
                else
                {
                    list2.Add(obj4.GetComponent<Renderer>());
                }
            }
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = go => go is Renderer;
            }
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = go => go as Renderer;
            }
            IEnumerable<Renderer> collection = Enumerable.Select<UnityEngine.Object, Renderer>(Enumerable.Where<UnityEngine.Object>(DragAndDrop.objectReferences, <>f__am$cache4), <>f__am$cache5);
            list2.AddRange(collection);
            return list2;
        }

        private void HandleAddRenderer(Rect position, IEnumerable<Rect> alreadyDrawn, Rect drawArea)
        {
            <HandleAddRenderer>c__AnonStorey0 storey = new <HandleAddRenderer>c__AnonStorey0 {
                evt = Event.current
            };
            EventType type = storey.evt.type;
            switch (type)
            {
                case EventType.Repaint:
                    LODGroupGUI.Styles.m_LODStandardButton.Draw(position, GUIContent.none, false, false, false, false);
                    LODGroupGUI.Styles.m_LODRendererAddButton.Draw(new Rect(position.x - 2f, position.y, position.width, position.height), "Add", false, false, false, false);
                    return;

                case EventType.DragUpdated:
                case EventType.DragPerform:
                {
                    bool flag = false;
                    if (drawArea.Contains(storey.evt.mousePosition) && Enumerable.All<Rect>(alreadyDrawn, new Func<Rect, bool>(storey.<>m__0)))
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        if (DragAndDrop.objectReferences.Count<UnityEngine.Object>() > 0)
                        {
                            DragAndDrop.visualMode = !this.m_IsPrefab ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.None;
                            if (storey.evt.type == EventType.DragPerform)
                            {
                                if (<>f__am$cache2 == null)
                                {
                                    <>f__am$cache2 = go => go is GameObject;
                                }
                                if (<>f__am$cache3 == null)
                                {
                                    <>f__am$cache3 = go => go as GameObject;
                                }
                                IEnumerable<GameObject> selectedGameObjects = Enumerable.Select<UnityEngine.Object, GameObject>(Enumerable.Where<UnityEngine.Object>(DragAndDrop.objectReferences, <>f__am$cache2), <>f__am$cache3);
                                IEnumerable<Renderer> renderers = this.GetRenderers(selectedGameObjects, true);
                                this.AddGameObjectRenderers(renderers, true);
                                DragAndDrop.AcceptDrag();
                                storey.evt.Use();
                                return;
                            }
                        }
                        storey.evt.Use();
                    }
                    return;
                }
                case EventType.ExecuteCommand:
                    if ((storey.evt.commandName == "ObjectSelectorClosed") && (ObjectSelector.get.objectSelectorID == "LODGroupSelector".GetHashCode()))
                    {
                        GameObject currentObject = ObjectSelector.GetCurrentObject() as GameObject;
                        if (currentObject != null)
                        {
                            List<GameObject> list = new List<GameObject> {
                                currentObject
                            };
                            this.AddGameObjectRenderers(this.GetRenderers(list, true), true);
                        }
                        storey.evt.Use();
                        GUIUtility.ExitGUI();
                    }
                    return;
            }
            if ((type == EventType.MouseDown) && position.Contains(storey.evt.mousePosition))
            {
                storey.evt.Use();
                int hashCode = "LODGroupSelector".GetHashCode();
                ObjectSelector.get.Show(null, typeof(Renderer), null, true);
                ObjectSelector.get.objectSelectorID = hashCode;
                GUIUtility.ExitGUI();
            }
        }

        public override bool HasPreviewGUI() => 
            (base.target != null);

        private void InitPreview()
        {
            if (this.m_PreviewUtility == null)
            {
                this.m_PreviewUtility = new PreviewRenderUtility();
            }
            if (kSLightIcons[0] == null)
            {
                kSLightIcons[0] = EditorGUIUtility.IconContent("PreMatLight0");
                kSLightIcons[1] = EditorGUIUtility.IconContent("PreMatLight1");
            }
        }

        private bool IsLODUsingCrossFadeWidth(int lod)
        {
            if ((this.m_FadeMode.intValue != 0) && !this.m_AnimateCrossFading.boolValue)
            {
                if (this.m_FadeMode.intValue == 1)
                {
                    return true;
                }
                if ((this.m_NumberOfLODs > 0) && (this.m_SelectedLOD == (this.m_NumberOfLODs - 1)))
                {
                    return true;
                }
                if ((this.m_NumberOfLODs > 1) && (this.m_SelectedLOD == (this.m_NumberOfLODs - 2)))
                {
                    SerializedProperty property = base.serializedObject.FindProperty($"m_LODs.Array.data[{this.m_NumberOfLODs - 1}].renderers");
                    if ((property.arraySize == 1) && (property.GetArrayElementAtIndex(0).FindPropertyRelative("renderer").objectReferenceValue is BillboardRenderer))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void OnDisable()
        {
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
            this.m_ShowAnimateCrossFading.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            this.m_ShowFadeTransitionWidth.valueChanged.RemoveListener(new UnityAction(this.Repaint));
        }

        private void OnEnable()
        {
            this.m_FadeMode = base.serializedObject.FindProperty("m_FadeMode");
            this.m_AnimateCrossFading = base.serializedObject.FindProperty("m_AnimateCrossFading");
            this.m_LODs = base.serializedObject.FindProperty("m_LODs");
            this.m_ShowAnimateCrossFading.value = this.m_FadeMode.intValue != 0;
            this.m_ShowAnimateCrossFading.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowFadeTransitionWidth.value = false;
            this.m_ShowFadeTransitionWidth.valueChanged.AddListener(new UnityAction(this.Repaint));
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
            this.m_LODGroup = (LODGroup) base.target;
            PrefabType prefabType = PrefabUtility.GetPrefabType(this.m_LODGroup.gameObject);
            this.m_IsPrefab = (prefabType == PrefabType.Prefab) || (prefabType == PrefabType.ModelPrefab);
            base.Repaint();
        }

        public override void OnInspectorGUI()
        {
            bool enabled = GUI.enabled;
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_FadeMode, new GUILayoutOption[0]);
            this.m_ShowAnimateCrossFading.target = this.m_FadeMode.intValue != 0;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowAnimateCrossFading.faded))
            {
                EditorGUILayout.PropertyField(this.m_AnimateCrossFading, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndFadeGroup();
            this.m_NumberOfLODs = this.m_LODs.arraySize;
            if (this.m_SelectedLOD >= this.m_NumberOfLODs)
            {
                this.m_SelectedLOD = this.m_NumberOfLODs - 1;
            }
            if ((this.m_NumberOfLODs > 0) && (this.activeLOD >= 0))
            {
                SerializedProperty property = base.serializedObject.FindProperty($"m_LODs.Array.data[{this.activeLOD}].renderers");
                for (int j = property.arraySize - 1; j >= 0; j--)
                {
                    Renderer objectReferenceValue = property.GetArrayElementAtIndex(j).FindPropertyRelative("renderer").objectReferenceValue as Renderer;
                    if (objectReferenceValue == null)
                    {
                        property.DeleteArrayElementAtIndex(j);
                    }
                }
            }
            GUILayout.Space(18f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            Rect area = GUILayoutUtility.GetRect((float) 0f, (float) 30f, options);
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = i => $"LOD {i}";
            }
            List<LODGroupGUI.LODInfo> lods = LODGroupGUI.CreateLODInfos(this.m_NumberOfLODs, area, <>f__am$cache1, i => base.serializedObject.FindProperty($"m_LODs.Array.data[{i}].screenRelativeHeight").floatValue);
            this.DrawLODLevelSlider(area, lods);
            GUILayout.Space(16f);
            if (QualitySettings.lodBias != 1f)
            {
                EditorGUILayout.HelpBox($"Active LOD bias is {QualitySettings.lodBias:0.0#}. Distances are adjusted accordingly.", MessageType.Warning);
            }
            if (((this.m_NumberOfLODs > 0) && (this.activeLOD >= 0)) && (this.activeLOD < this.m_NumberOfLODs))
            {
                this.m_ShowFadeTransitionWidth.target = this.IsLODUsingCrossFadeWidth(this.activeLOD);
                if (EditorGUILayout.BeginFadeGroup(this.m_ShowFadeTransitionWidth.faded))
                {
                    EditorGUILayout.PropertyField(base.serializedObject.FindProperty($"m_LODs.Array.data[{this.activeLOD}].fadeTransitionWidth"), new GUILayoutOption[0]);
                }
                EditorGUILayout.EndFadeGroup();
                this.DrawRenderersInfo(EditorGUIUtility.currentViewWidth);
            }
            GUILayout.Space(8f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            bool flag2 = LODUtility.NeedUpdateLODGroupBoundingBox(this.m_LODGroup);
            using (new EditorGUI.DisabledScope(!flag2))
            {
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                if (GUILayout.Button(!flag2 ? LODGroupGUI.Styles.m_RecalculateBoundsDisabled : LODGroupGUI.Styles.m_RecalculateBounds, optionArray2))
                {
                    Undo.RecordObject(this.m_LODGroup, "Recalculate LODGroup Bounds");
                    this.m_LODGroup.RecalculateBounds();
                }
            }
            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            if (GUILayout.Button(LODGroupGUI.Styles.m_LightmapScale, optionArray3))
            {
                this.SendPercentagesToLightmapScale();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            ModelImporter importer = (PrefabUtility.GetPrefabType(base.target) != PrefabType.ModelPrefabInstance) ? null : this.GetImporter();
            if (importer != null)
            {
                SerializedObject obj2 = new SerializedObject(importer);
                SerializedProperty property3 = obj2.FindProperty("m_LODScreenPercentages");
                bool flag3 = property3.isArray && (property3.arraySize == lods.Count);
                bool flag4 = GUI.enabled;
                if (!flag3)
                {
                    GUI.enabled = false;
                }
                if ((importer != null) && GUILayout.Button(!flag3 ? LODGroupGUI.Styles.m_UploadToImporterDisabled : LODGroupGUI.Styles.m_UploadToImporter, new GUILayoutOption[0]))
                {
                    for (int k = 0; k < property3.arraySize; k++)
                    {
                        property3.GetArrayElementAtIndex(k).floatValue = lods[k].RawScreenPercent;
                    }
                    obj2.ApplyModifiedProperties();
                    AssetDatabase.ImportAsset(importer.assetPath);
                }
                GUI.enabled = flag4;
            }
            base.serializedObject.ApplyModifiedProperties();
            GUI.enabled = enabled;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (!ShaderUtil.hardwareSupportsRectRenderTexture)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), "LOD preview \nnot available");
                }
            }
            else
            {
                this.InitPreview();
                this.m_PreviewDir = PreviewGUI.Drag2D(this.m_PreviewDir, r);
                this.m_PreviewDir.y = Mathf.Clamp(this.m_PreviewDir.y, -89f, 89f);
                if (Event.current.type == EventType.Repaint)
                {
                    this.m_PreviewUtility.BeginPreview(r, background);
                    this.DoRenderPreview();
                    this.m_PreviewUtility.EndAndDrawPreview(r);
                }
            }
        }

        public void OnSceneGUI()
        {
            if (((Event.current.type == EventType.Repaint) && (Camera.current != null)) && (SceneView.lastActiveSceneView == SceneView.currentDrawingSceneView))
            {
                Camera camera = SceneView.lastActiveSceneView.camera;
                Vector3 position = LODUtility.CalculateWorldReferencePoint(this.m_LODGroup);
                Vector3 vector2 = camera.transform.position - position;
                if (Vector3.Dot(camera.transform.forward, vector2.normalized) <= 0f)
                {
                    LODVisualizationInformation information = LODUtility.CalculateVisualizationData(camera, this.m_LODGroup, -1);
                    float worldSpaceSize = information.worldSpaceSize;
                    Handles.color = (information.activeLODLevel == -1) ? LODGroupGUI.kCulledLODColor : LODGroupGUI.kLODColors[information.activeLODLevel];
                    Handles.SelectionFrame(0, position, camera.transform.rotation, worldSpaceSize / 2f);
                    Vector3 vector3 = (Vector3) ((camera.transform.right * worldSpaceSize) / 2f);
                    Vector3 vector4 = (Vector3) ((camera.transform.up * worldSpaceSize) / 2f);
                    Vector3[] points = new Vector3[] { (position - vector3) + vector4, (position - vector3) - vector4, (position + vector3) + vector4, (position + vector3) - vector4 };
                    Rect rect = CalculateScreenRect(points);
                    float num2 = rect.x + (rect.width / 2f);
                    rect = new Rect(num2 - 100f, rect.yMax, 200f, 45f);
                    if (rect.yMax > (Screen.height - 0x2d))
                    {
                        rect.y = (Screen.height - 0x2d) - 40;
                    }
                    Handles.BeginGUI();
                    GUI.Label(rect, GUIContent.none, EditorStyles.notificationBackground);
                    EditorGUI.DoDropShadowLabel(rect, GUIContent.Temp((information.activeLODLevel < 0) ? "Culled" : ("LOD " + information.activeLODLevel)), LODGroupGUI.Styles.m_LODLevelNotifyText, 0.3f);
                    Handles.EndGUI();
                }
            }
        }

        private void SendPercentagesToLightmapScale()
        {
            List<LODLightmapScale> list = new List<LODLightmapScale>();
            for (int i = 0; i < this.m_NumberOfLODs; i++)
            {
                SerializedProperty property = base.serializedObject.FindProperty($"m_LODs.Array.data[{i}].renderers");
                List<SerializedProperty> renderers = new List<SerializedProperty>();
                for (int k = 0; k < property.arraySize; k++)
                {
                    SerializedProperty item = property.GetArrayElementAtIndex(k).FindPropertyRelative("renderer");
                    if (item != null)
                    {
                        renderers.Add(item);
                    }
                }
                float scale = (i != 0) ? base.serializedObject.FindProperty($"m_LODs.Array.data[{i - 1}].screenRelativeHeight").floatValue : 1f;
                list.Add(new LODLightmapScale(scale, renderers));
            }
            for (int j = 0; j < this.m_NumberOfLODs; j++)
            {
                SetLODLightmapScale(list[j]);
            }
        }

        private static void SetLODLightmapScale(LODLightmapScale lodRenderer)
        {
            foreach (SerializedProperty property in lodRenderer.m_Renderers)
            {
                SerializedObject obj2 = new SerializedObject(property.objectReferenceValue);
                obj2.FindProperty("m_ScaleInLightmap").floatValue = Mathf.Max((float) 0f, (float) (lodRenderer.m_Scale * (1f / LightmapVisualization.GetLightmapLODLevelScale((Renderer) property.objectReferenceValue))));
                obj2.ApplyModifiedProperties();
            }
        }

        public void Update()
        {
            if (((SceneView.lastActiveSceneView != null) && (SceneView.lastActiveSceneView.camera != null)) && (SceneView.lastActiveSceneView.camera.transform.position != this.m_LastCameraPos))
            {
                this.m_LastCameraPos = SceneView.lastActiveSceneView.camera.transform.position;
                base.Repaint();
            }
        }

        private static void UpdateCamera(float desiredPercentage, LODGroup group)
        {
            Vector3 pos = LODUtility.CalculateWorldReferencePoint(group);
            float relativeScreenHeight = Mathf.Max((float) (desiredPercentage / QualitySettings.lodBias), (float) 1E-06f);
            float newSize = LODUtility.CalculateDistance(SceneView.lastActiveSceneView.camera, relativeScreenHeight, group);
            if (SceneView.lastActiveSceneView.camera.orthographic)
            {
                newSize *= Mathf.Sqrt(2f * SceneView.lastActiveSceneView.camera.aspect);
            }
            SceneView.lastActiveSceneView.LookAtDirect(pos, SceneView.lastActiveSceneView.camera.transform.rotation, newSize);
        }

        private void UpdateLODDrag(float desiredPercentage, LODGroup group)
        {
            if (((SceneView.lastActiveSceneView != null) && (SceneView.lastActiveSceneView.camera != null)) && !this.m_IsPrefab)
            {
                UpdateCamera(desiredPercentage, group);
                SceneView.RepaintAll();
            }
        }

        private void UpdateSelectedLODFromCamera(IEnumerable<LODGroupGUI.LODInfo> lods, float cameraPercent)
        {
            foreach (LODGroupGUI.LODInfo info in lods)
            {
                if (cameraPercent > info.RawScreenPercent)
                {
                    this.m_SelectedLOD = info.LODLevel;
                    break;
                }
            }
        }

        private int activeLOD =>
            this.m_SelectedLOD;

        [CompilerGenerated]
        private sealed class <HandleAddRenderer>c__AnonStorey0
        {
            internal Event evt;

            internal bool <>m__0(Rect x) => 
                !x.Contains(this.evt.mousePosition);
        }

        private class LODAction
        {
            private readonly Callback m_Callback;
            private readonly Vector2 m_ClickedPosition;
            private readonly List<LODGroupGUI.LODInfo> m_LODs;
            private readonly SerializedProperty m_LODsProperty;
            private readonly SerializedObject m_ObjectRef;
            private readonly float m_Percentage;

            public LODAction(List<LODGroupGUI.LODInfo> lods, float percentage, Vector2 clickedPosition, SerializedProperty propLODs, Callback callback)
            {
                this.m_LODs = lods;
                this.m_Percentage = percentage;
                this.m_ClickedPosition = clickedPosition;
                this.m_LODsProperty = propLODs;
                this.m_ObjectRef = propLODs.serializedObject;
                this.m_Callback = callback;
            }

            public void DeleteLOD()
            {
                if (this.m_LODs.Count > 0)
                {
                    foreach (LODGroupGUI.LODInfo info in this.m_LODs)
                    {
                        int arraySize = this.m_ObjectRef.FindProperty($"m_LODs.Array.data[{info.LODLevel}].renderers").arraySize;
                        if (info.m_RangePosition.Contains(this.m_ClickedPosition) && ((arraySize == 0) || EditorUtility.DisplayDialog("Delete LOD", "Are you sure you wish to delete this LOD?", "Yes", "No")))
                        {
                            this.m_ObjectRef.FindProperty($"m_LODs.Array.data[{info.LODLevel}]").DeleteCommand();
                            this.m_ObjectRef.ApplyModifiedProperties();
                            if (this.m_Callback != null)
                            {
                                this.m_Callback();
                            }
                            break;
                        }
                    }
                }
            }

            public void InsertLOD()
            {
                if (this.m_LODsProperty.isArray)
                {
                    int index = -1;
                    foreach (LODGroupGUI.LODInfo info in this.m_LODs)
                    {
                        if (this.m_Percentage > info.RawScreenPercent)
                        {
                            index = info.LODLevel;
                            break;
                        }
                    }
                    if (index < 0)
                    {
                        this.m_LODsProperty.InsertArrayElementAtIndex(this.m_LODs.Count);
                        index = this.m_LODs.Count;
                    }
                    else
                    {
                        this.m_LODsProperty.InsertArrayElementAtIndex(index);
                    }
                    this.m_ObjectRef.FindProperty($"m_LODs.Array.data[{index}].renderers").arraySize = 0;
                    this.m_LODsProperty.GetArrayElementAtIndex(index).FindPropertyRelative("screenRelativeHeight").floatValue = this.m_Percentage;
                    if (this.m_Callback != null)
                    {
                        this.m_Callback();
                    }
                    this.m_ObjectRef.ApplyModifiedProperties();
                }
            }

            public delegate void Callback();
        }

        private class LODLightmapScale
        {
            public readonly List<SerializedProperty> m_Renderers;
            public readonly float m_Scale;

            public LODLightmapScale(float scale, List<SerializedProperty> renderers)
            {
                this.m_Scale = scale;
                this.m_Renderers = renderers;
            }
        }
    }
}

