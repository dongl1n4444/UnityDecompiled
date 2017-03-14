namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor.SceneManagement;
    using UnityEditor.VersionControl;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(GameObject)), CanEditMultipleObjects]
    internal class GameObjectInspector : Editor
    {
        public static GameObject dragObject;
        private const float kIconSize = 24f;
        private bool m_AllOfSamePrefabType = true;
        private bool m_HasInstance = false;
        private SerializedProperty m_Icon;
        private SerializedProperty m_IsActive;
        private SerializedProperty m_Layer;
        private SerializedProperty m_Name;
        private List<GameObject> m_PreviewInstances;
        private PreviewRenderUtility m_PreviewUtility;
        private SerializedProperty m_StaticEditorFlags;
        private SerializedProperty m_Tag;
        private Vector2 previewDir;
        private static Styles s_Styles;

        private void CalculatePrefabStatus()
        {
            this.m_HasInstance = false;
            this.m_AllOfSamePrefabType = true;
            PrefabType prefabType = PrefabUtility.GetPrefabType(base.targets[0] as GameObject);
            foreach (GameObject obj2 in base.targets)
            {
                PrefabType type2 = PrefabUtility.GetPrefabType(obj2);
                if (type2 != prefabType)
                {
                    this.m_AllOfSamePrefabType = false;
                }
                if (((type2 != PrefabType.None) && (type2 != PrefabType.Prefab)) && (type2 != PrefabType.ModelPrefab))
                {
                    this.m_HasInstance = true;
                }
            }
        }

        private void CreatePreviewInstances()
        {
            this.DestroyPreviewInstances();
            if (this.m_PreviewInstances == null)
            {
                this.m_PreviewInstances = new List<GameObject>(base.targets.Length);
            }
            for (int i = 0; i < base.targets.Length; i++)
            {
                GameObject go = EditorUtility.InstantiateForAnimatorPreview(base.targets[i]);
                SetEnabledRecursive(go, false);
                this.m_PreviewInstances.Add(go);
            }
        }

        private void DestroyPreviewInstances()
        {
            if ((this.m_PreviewInstances != null) && (this.m_PreviewInstances.Count != 0))
            {
                foreach (GameObject obj2 in this.m_PreviewInstances)
                {
                    UnityEngine.Object.DestroyImmediate(obj2);
                }
                this.m_PreviewInstances.Clear();
            }
        }

        private void DoLayerField(GameObject go)
        {
            EditorGUIUtility.labelWidth = s_Styles.layerFieldWidth;
            Rect totalPosition = GUILayoutUtility.GetRect(GUIContent.none, s_Styles.layerPopup);
            EditorGUI.BeginProperty(totalPosition, GUIContent.none, this.m_Layer);
            EditorGUI.BeginChangeCheck();
            int layer = EditorGUI.LayerField(totalPosition, EditorGUIUtility.TempContent("Layer"), go.layer, s_Styles.layerPopup);
            if (EditorGUI.EndChangeCheck())
            {
                GameObjectUtility.ShouldIncludeChildren children = GameObjectUtility.DisplayUpdateChildrenDialogIfNeeded(base.targets.OfType<GameObject>(), "Change Layer", "Do you want to set layer to " + InternalEditorUtility.GetLayerName(layer) + " for all child objects as well?");
                if (children != GameObjectUtility.ShouldIncludeChildren.Cancel)
                {
                    this.m_Layer.intValue = layer;
                    this.SetLayer(layer, children == GameObjectUtility.ShouldIncludeChildren.IncludeChildren);
                }
            }
            EditorGUI.EndProperty();
        }

        private void DoPrefabButtons(PrefabType prefabType, GameObject go)
        {
            if (this.m_HasInstance)
            {
                using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode))
                {
                    EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUIContent content = (base.targets.Length <= 1) ? s_Styles.goTypeLabel[(int) prefabType] : s_Styles.goTypeLabelMultiple;
                    if (content != null)
                    {
                        GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(24f + s_Styles.tagFieldWidth) };
                        EditorGUILayout.BeginHorizontal(options);
                        GUILayout.FlexibleSpace();
                        if (((prefabType == PrefabType.DisconnectedModelPrefabInstance) || (prefabType == PrefabType.MissingPrefabInstance)) || (prefabType == PrefabType.DisconnectedPrefabInstance))
                        {
                            GUI.contentColor = GUI.skin.GetStyle("CN StatusWarn").normal.textColor;
                            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                            GUILayout.Label(content, EditorStyles.whiteLabel, optionArray2);
                            GUI.contentColor = Color.white;
                        }
                        else
                        {
                            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                            GUILayout.Label(content, optionArray3);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    if (base.targets.Length > 1)
                    {
                        GUILayout.Label("Instance Management Disabled", s_Styles.instanceManagementInfo, new GUILayoutOption[0]);
                    }
                    else
                    {
                        if ((prefabType != PrefabType.MissingPrefabInstance) && GUILayout.Button("Select", "MiniButtonLeft", new GUILayoutOption[0]))
                        {
                            Selection.activeObject = PrefabUtility.GetPrefabParent(base.target);
                            EditorGUIUtility.PingObject(Selection.activeObject);
                        }
                        if (((prefabType == PrefabType.DisconnectedModelPrefabInstance) || (prefabType == PrefabType.DisconnectedPrefabInstance)) && GUILayout.Button("Revert", "MiniButtonMid", new GUILayoutOption[0]))
                        {
                            List<UnityEngine.Object> hierarchy = new List<UnityEngine.Object>();
                            this.GetObjectListFromHierarchy(hierarchy, go);
                            Undo.RegisterFullObjectHierarchyUndo(go, "Revert to prefab");
                            PrefabUtility.ReconnectToLastPrefab(go);
                            Undo.RegisterCreatedObjectUndo(PrefabUtility.GetPrefabObject(go), "Revert to prefab");
                            PrefabUtility.RevertPrefabInstance(go);
                            this.CalculatePrefabStatus();
                            List<UnityEngine.Object> list2 = new List<UnityEngine.Object>();
                            this.GetObjectListFromHierarchy(list2, go);
                            this.RegisterNewComponents(list2, hierarchy);
                        }
                        using (new EditorGUI.DisabledScope(UnityEditor.AnimationMode.InAnimationMode()))
                        {
                            if (((prefabType == PrefabType.ModelPrefabInstance) || (prefabType == PrefabType.PrefabInstance)) && GUILayout.Button("Revert", "MiniButtonMid", new GUILayoutOption[0]))
                            {
                                this.RevertAndCheckForNewComponents(go);
                            }
                            if ((prefabType == PrefabType.PrefabInstance) || (prefabType == PrefabType.DisconnectedPrefabInstance))
                            {
                                GameObject source = PrefabUtility.FindValidUploadPrefabInstanceRoot(go);
                                GUI.enabled = (source != null) && !UnityEditor.AnimationMode.InAnimationMode();
                                if (GUILayout.Button("Apply", "MiniButtonRight", new GUILayoutOption[0]))
                                {
                                    UnityEngine.Object prefabParent = PrefabUtility.GetPrefabParent(source);
                                    string assetPath = AssetDatabase.GetAssetPath(prefabParent);
                                    string[] assets = new string[] { assetPath };
                                    if (Provider.PromptAndCheckoutIfNeeded(assets, "The version control requires you to check out the prefab before applying changes."))
                                    {
                                        PrefabUtility.ReplacePrefab(source, prefabParent, ReplacePrefabOptions.ConnectToPrefab);
                                        this.CalculatePrefabStatus();
                                        EditorSceneManager.MarkSceneDirty(source.scene);
                                        GUIUtility.ExitGUI();
                                    }
                                }
                            }
                        }
                        if (((prefabType == PrefabType.DisconnectedModelPrefabInstance) || (prefabType == PrefabType.ModelPrefabInstance)) && GUILayout.Button("Open", "MiniButtonRight", new GUILayoutOption[0]))
                        {
                            AssetDatabase.OpenAsset(PrefabUtility.GetPrefabParent(base.target));
                            GUIUtility.ExitGUI();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void DoRenderPreview()
        {
            GameObject go = this.m_PreviewInstances[this.referenceTargetIndex];
            Bounds bounds = new Bounds(go.transform.position, Vector3.zero);
            GetRenderableBoundsRecurse(ref bounds, go);
            float num = Mathf.Max(bounds.extents.magnitude, 0.0001f);
            float num2 = num * 3.8f;
            Quaternion quaternion = Quaternion.Euler(-this.previewDir.y, -this.previewDir.x, 0f);
            Vector3 vector2 = bounds.center - ((Vector3) (quaternion * (Vector3.forward * num2)));
            this.m_PreviewUtility.m_Camera.transform.position = vector2;
            this.m_PreviewUtility.m_Camera.transform.rotation = quaternion;
            this.m_PreviewUtility.m_Camera.nearClipPlane = num2 - (num * 1.1f);
            this.m_PreviewUtility.m_Camera.farClipPlane = num2 + (num * 1.1f);
            this.m_PreviewUtility.m_Light[0].intensity = 0.7f;
            this.m_PreviewUtility.m_Light[0].transform.rotation = quaternion * Quaternion.Euler(40f, 40f, 0f);
            this.m_PreviewUtility.m_Light[1].intensity = 0.7f;
            this.m_PreviewUtility.m_Light[1].transform.rotation = quaternion * Quaternion.Euler(340f, 218f, 177f);
            Color ambient = new Color(0.1f, 0.1f, 0.1f, 0f);
            InternalEditorUtility.SetCustomLighting(this.m_PreviewUtility.m_Light, ambient);
            bool fog = RenderSettings.fog;
            Unsupported.SetRenderSettingsUseFogNoDirty(false);
            SetEnabledRecursive(go, true);
            this.m_PreviewUtility.m_Camera.Render();
            SetEnabledRecursive(go, false);
            Unsupported.SetRenderSettingsUseFogNoDirty(fog);
            InternalEditorUtility.RemoveCustomLighting();
        }

        private void DoStaticFlagsDropDown(GameObject go)
        {
            int num;
            bool flag;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = this.m_StaticEditorFlags.hasMultipleDifferentValues;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            EditorGUI.EnumMaskField(GUILayoutUtility.GetRect(GUIContent.none, s_Styles.staticDropdown, options), GameObjectUtility.GetStaticEditorFlags(go), s_Styles.staticDropdown, out num, out flag);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                SceneModeUtility.SetStaticFlags(base.targets, num, flag);
                base.serializedObject.SetIsDifferentCacheDirty();
            }
        }

        private void DoStaticToggleField(GameObject go)
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            Rect totalPosition = GUILayoutUtility.GetRect(s_Styles.staticContent, EditorStyles.toggle, options);
            EditorGUI.BeginProperty(totalPosition, GUIContent.none, this.m_StaticEditorFlags);
            EditorGUI.BeginChangeCheck();
            Rect position = totalPosition;
            EditorGUI.showMixedValue |= ShowMixedStaticEditorFlags((StaticEditorFlags) this.m_StaticEditorFlags.intValue);
            Event current = Event.current;
            EventType type = current.type;
            bool flag = (current.type == EventType.MouseDown) && (current.button != 0);
            if (flag)
            {
                current.type = EventType.Ignore;
            }
            bool flagValue = EditorGUI.ToggleLeft(position, s_Styles.staticContent, go.isStatic);
            if (flag)
            {
                current.type = type;
            }
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                SceneModeUtility.SetStaticFlags(base.targets, -1, flagValue);
                base.serializedObject.SetIsDifferentCacheDirty();
            }
            EditorGUI.EndProperty();
        }

        private void DoTagsField(GameObject go)
        {
            string tag = null;
            try
            {
                tag = go.tag;
            }
            catch (Exception)
            {
                tag = "Undefined";
            }
            EditorGUIUtility.labelWidth = s_Styles.tagFieldWidth;
            Rect totalPosition = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.popup);
            EditorGUI.BeginProperty(totalPosition, GUIContent.none, this.m_Tag);
            EditorGUI.BeginChangeCheck();
            string str2 = EditorGUI.TagField(totalPosition, EditorGUIUtility.TempContent("Tag"), tag);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_Tag.stringValue = str2;
                Undo.RecordObjects(base.targets, "Change Tag of " + this.targetTitle);
                foreach (UnityEngine.Object obj2 in base.targets)
                {
                    (obj2 as GameObject).tag = str2;
                }
            }
            EditorGUI.EndProperty();
        }

        internal bool DrawInspector()
        {
            base.serializedObject.Update();
            GameObject target = base.target as GameObject;
            GUIContent goIcon = null;
            PrefabType none = PrefabType.None;
            if (this.m_AllOfSamePrefabType)
            {
                none = PrefabUtility.GetPrefabType(target);
                switch (none)
                {
                    case PrefabType.None:
                        goIcon = s_Styles.goIcon;
                        break;

                    case PrefabType.Prefab:
                    case PrefabType.PrefabInstance:
                    case PrefabType.MissingPrefabInstance:
                    case PrefabType.DisconnectedPrefabInstance:
                        goIcon = s_Styles.prefabIcon;
                        break;

                    case PrefabType.ModelPrefab:
                    case PrefabType.ModelPrefabInstance:
                    case PrefabType.DisconnectedModelPrefabInstance:
                        goIcon = s_Styles.modelIcon;
                        break;
                }
            }
            else
            {
                goIcon = s_Styles.typelessIcon;
            }
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            EditorGUI.ObjectIconDropDown(GUILayoutUtility.GetRect((float) 24f, (float) 24f, options), base.targets, true, goIcon.image as Texture2D, this.m_Icon);
            using (new EditorGUI.DisabledScope(none == PrefabType.ModelPrefab))
            {
                EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
                EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(s_Styles.tagFieldWidth) };
                EditorGUILayout.BeginHorizontal(optionArray2);
                GUILayout.FlexibleSpace();
                GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                EditorGUI.PropertyField(GUILayoutUtility.GetRect((float) EditorStyles.toggle.padding.left, EditorGUIUtility.singleLineHeight, EditorStyles.toggle, optionArray3), this.m_IsActive, GUIContent.none);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.DelayedTextField(this.m_Name, GUIContent.none, new GUILayoutOption[0]);
                this.DoStaticToggleField(target);
                this.DoStaticFlagsDropDown(target);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                this.DoTagsField(target);
                this.DoLayerField(target);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2f);
            using (new EditorGUI.DisabledScope(none == PrefabType.ModelPrefab))
            {
                this.DoPrefabButtons(none, target);
            }
            base.serializedObject.ApplyModifiedProperties();
            return true;
        }

        private void GetObjectListFromHierarchy(List<UnityEngine.Object> hierarchy, GameObject gameObject)
        {
            Transform transform = null;
            List<Component> results = new List<Component>();
            gameObject.GetComponents<Component>(results);
            foreach (Component component in results)
            {
                if (component is Transform)
                {
                    transform = component as Transform;
                }
                else
                {
                    hierarchy.Add(component);
                }
            }
            if (transform != null)
            {
                int childCount = transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    this.GetObjectListFromHierarchy(hierarchy, transform.GetChild(i).gameObject);
                }
            }
        }

        private UnityEngine.Object[] GetObjects(bool includeChildren) => 
            SceneModeUtility.GetObjects(base.targets, includeChildren);

        public static void GetRenderableBoundsRecurse(ref Bounds bounds, GameObject go)
        {
            MeshRenderer component = go.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            MeshFilter filter = go.GetComponent(typeof(MeshFilter)) as MeshFilter;
            if (((component != null) && (filter != null)) && (filter.sharedMesh != null))
            {
                if (bounds.extents == Vector3.zero)
                {
                    bounds = component.bounds;
                }
                else
                {
                    bounds.Encapsulate(component.bounds);
                }
            }
            SkinnedMeshRenderer renderer2 = go.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
            if ((renderer2 != null) && (renderer2.sharedMesh != null))
            {
                if (bounds.extents == Vector3.zero)
                {
                    bounds = renderer2.bounds;
                }
                else
                {
                    bounds.Encapsulate(renderer2.bounds);
                }
            }
            SpriteRenderer renderer3 = go.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            if ((renderer3 != null) && (renderer3.sprite != null))
            {
                if (bounds.extents == Vector3.zero)
                {
                    bounds = renderer3.bounds;
                }
                else
                {
                    bounds.Encapsulate(renderer3.bounds);
                }
            }
            IEnumerator enumerator = go.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    GetRenderableBoundsRecurse(ref bounds, current.gameObject);
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

        public static Vector3 GetRenderableCenterRecurse(GameObject go, int minDepth, int maxDepth)
        {
            Vector3 zero = Vector3.zero;
            float num = GetRenderableCenterRecurse(ref zero, go, 0, minDepth, maxDepth);
            if (num > 0f)
            {
                return (Vector3) (zero / num);
            }
            return go.transform.position;
        }

        private static float GetRenderableCenterRecurse(ref Vector3 center, GameObject go, int depth, int minDepth, int maxDepth)
        {
            if (depth > maxDepth)
            {
                return 0f;
            }
            float num2 = 0f;
            if (depth > minDepth)
            {
                MeshRenderer component = go.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
                MeshFilter filter = go.GetComponent(typeof(MeshFilter)) as MeshFilter;
                SkinnedMeshRenderer renderer2 = go.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
                SpriteRenderer renderer3 = go.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
                if (((component == null) && (filter == null)) && ((renderer2 == null) && (renderer3 == null)))
                {
                    num2 = 1f;
                    center += go.transform.position;
                }
                else if ((component != null) && (filter != null))
                {
                    if (Vector3.Distance(component.bounds.center, go.transform.position) < 0.01f)
                    {
                        num2 = 1f;
                        center += go.transform.position;
                    }
                }
                else if (renderer2 != null)
                {
                    if (Vector3.Distance(renderer2.bounds.center, go.transform.position) < 0.01f)
                    {
                        num2 = 1f;
                        center += go.transform.position;
                    }
                }
                else if ((renderer3 != null) && (Vector3.Distance(renderer3.bounds.center, go.transform.position) < 0.01f))
                {
                    num2 = 1f;
                    center += go.transform.position;
                }
            }
            depth++;
            IEnumerator enumerator = go.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    num2 += GetRenderableCenterRecurse(ref center, current.gameObject, depth, minDepth, maxDepth);
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
            return num2;
        }

        public override bool HasPreviewGUI()
        {
            if (!EditorUtility.IsPersistent(base.target))
            {
                return false;
            }
            return this.HasStaticPreview();
        }

        public static bool HasRenderableParts(GameObject go)
        {
            MeshRenderer[] componentsInChildren = go.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in componentsInChildren)
            {
                MeshFilter component = renderer.gameObject.GetComponent<MeshFilter>();
                if ((component != null) && (component.sharedMesh != null))
                {
                    return true;
                }
            }
            SkinnedMeshRenderer[] rendererArray3 = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer renderer2 in rendererArray3)
            {
                if (renderer2.sharedMesh != null)
                {
                    return true;
                }
            }
            SpriteRenderer[] rendererArray5 = go.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer renderer3 in rendererArray5)
            {
                if (renderer3.sprite != null)
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasStaticPreview()
        {
            if (base.targets.Length > 1)
            {
                return true;
            }
            if (base.target == null)
            {
                return false;
            }
            GameObject target = base.target as GameObject;
            Camera component = target.GetComponent(typeof(Camera)) as Camera;
            return ((component != null) || HasRenderableParts(target));
        }

        private void InitPreview()
        {
            if (this.m_PreviewUtility == null)
            {
                this.m_PreviewUtility = new PreviewRenderUtility(true);
                this.m_PreviewUtility.m_CameraFieldOfView = 30f;
                this.m_PreviewUtility.m_Camera.cullingMask = ((int) 1) << Camera.PreviewCullingLayer;
                this.CreatePreviewInstances();
            }
        }

        public void OnDestroy()
        {
            this.DestroyPreviewInstances();
            if (this.m_PreviewUtility != null)
            {
                this.m_PreviewUtility.Cleanup();
                this.m_PreviewUtility = null;
            }
        }

        private void OnDisable()
        {
        }

        public void OnEnable()
        {
            if (EditorSettings.defaultBehaviorMode == EditorBehaviorMode.Mode2D)
            {
                this.previewDir = new Vector2(0f, 0f);
            }
            else
            {
                this.previewDir = new Vector2(120f, -20f);
            }
            this.m_Name = base.serializedObject.FindProperty("m_Name");
            this.m_IsActive = base.serializedObject.FindProperty("m_IsActive");
            this.m_Layer = base.serializedObject.FindProperty("m_Layer");
            this.m_Tag = base.serializedObject.FindProperty("m_TagString");
            this.m_StaticEditorFlags = base.serializedObject.FindProperty("m_StaticEditorFlags");
            this.m_Icon = base.serializedObject.FindProperty("m_Icon");
            this.CalculatePrefabStatus();
        }

        protected override void OnHeaderGUI()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            bool enabled = GUI.enabled;
            GUI.enabled = true;
            EditorGUILayout.BeginVertical(s_Styles.header, new GUILayoutOption[0]);
            GUI.enabled = enabled;
            this.DrawInspector();
            EditorGUILayout.EndVertical();
        }

        public override void OnInspectorGUI()
        {
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (!ShaderUtil.hardwareSupportsRectRenderTexture)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), "Preview requires\nrender texture support");
                }
            }
            else
            {
                this.InitPreview();
                this.previewDir = PreviewGUI.Drag2D(this.previewDir, r);
                if (Event.current.type == EventType.Repaint)
                {
                    this.m_PreviewUtility.BeginPreview(r, background);
                    this.DoRenderPreview();
                    this.m_PreviewUtility.EndAndDrawPreview(r);
                }
            }
        }

        public override void OnPreviewSettings()
        {
            if (ShaderUtil.hardwareSupportsRectRenderTexture)
            {
                GUI.enabled = true;
                this.InitPreview();
            }
        }

        public void OnSceneDrag(SceneView sceneView)
        {
            GameObject target = base.target as GameObject;
            switch (PrefabUtility.GetPrefabType(target))
            {
                case PrefabType.Prefab:
                case PrefabType.ModelPrefab:
                {
                    Event current = Event.current;
                    EventType type = current.type;
                    if (type != EventType.DragUpdated)
                    {
                        if (type == EventType.DragPerform)
                        {
                            string uniqueNameForSibling = GameObjectUtility.GetUniqueNameForSibling(null, dragObject.name);
                            dragObject.hideFlags = HideFlags.None;
                            Undo.RegisterCreatedObjectUndo(dragObject, "Place " + dragObject.name);
                            EditorUtility.SetDirty(dragObject);
                            DragAndDrop.AcceptDrag();
                            Selection.activeObject = dragObject;
                            HandleUtility.ignoreRaySnapObjects = null;
                            EditorWindow.mouseOverWindow.Focus();
                            dragObject.name = uniqueNameForSibling;
                            dragObject = null;
                            current.Use();
                            break;
                        }
                        if (type == EventType.DragExited)
                        {
                            if (dragObject != null)
                            {
                                UnityEngine.Object.DestroyImmediate(dragObject, false);
                                HandleUtility.ignoreRaySnapObjects = null;
                                dragObject = null;
                                current.Use();
                            }
                            break;
                        }
                    }
                    else
                    {
                        if (dragObject == null)
                        {
                            dragObject = (GameObject) PrefabUtility.InstantiatePrefab(PrefabUtility.FindPrefabRoot(target));
                            dragObject.hideFlags = HideFlags.HideInHierarchy;
                            dragObject.name = target.name;
                        }
                        if (HandleUtility.ignoreRaySnapObjects == null)
                        {
                            HandleUtility.ignoreRaySnapObjects = dragObject.GetComponentsInChildren<Transform>();
                        }
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        object obj3 = HandleUtility.RaySnap(HandleUtility.GUIPointToWorldRay(current.mousePosition));
                        if (obj3 != null)
                        {
                            RaycastHit hit = (RaycastHit) obj3;
                            float num = 0f;
                            if (Tools.pivotMode == PivotMode.Center)
                            {
                                float num2 = HandleUtility.CalcRayPlaceOffset(HandleUtility.ignoreRaySnapObjects, hit.normal);
                                if (num2 != float.PositiveInfinity)
                                {
                                    num = Vector3.Dot(dragObject.transform.position, hit.normal) - num2;
                                }
                            }
                            dragObject.transform.position = Matrix4x4.identity.MultiplyPoint(hit.point + ((Vector3) (hit.normal * num)));
                        }
                        else
                        {
                            dragObject.transform.position = HandleUtility.GUIPointToWorldRay(current.mousePosition).GetPoint(10f);
                        }
                        if (sceneView.in2DMode)
                        {
                            Vector3 position = dragObject.transform.position;
                            position.z = PrefabUtility.FindPrefabRoot(target).transform.position.z;
                            dragObject.transform.position = position;
                        }
                        current.Use();
                    }
                    break;
                }
            }
        }

        private void RegisterNewComponents(List<UnityEngine.Object> newHierarchy, List<UnityEngine.Object> hierarchy)
        {
            for (int i = 0; i < newHierarchy.Count; i++)
            {
                bool flag = false;
                UnityEngine.Object obj2 = newHierarchy[i];
                for (int j = 0; j < hierarchy.Count; j++)
                {
                    if (hierarchy[j].GetInstanceID() == obj2.GetInstanceID())
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    Undo.RegisterCreatedObjectUndo(newHierarchy[i], "Dangly component");
                }
            }
        }

        public override void ReloadPreviewInstances()
        {
            this.CreatePreviewInstances();
        }

        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
        {
            if (!this.HasStaticPreview() || !ShaderUtil.hardwareSupportsRectRenderTexture)
            {
                return null;
            }
            this.InitPreview();
            this.m_PreviewUtility.BeginStaticPreview(new Rect(0f, 0f, (float) width, (float) height));
            this.DoRenderPreview();
            return this.m_PreviewUtility.EndStaticPreview();
        }

        public void RevertAndCheckForNewComponents(GameObject gameObject)
        {
            List<UnityEngine.Object> hierarchy = new List<UnityEngine.Object>();
            this.GetObjectListFromHierarchy(hierarchy, gameObject);
            Undo.RegisterFullObjectHierarchyUndo(gameObject, "Revert Prefab Instance");
            PrefabUtility.RevertPrefabInstance(gameObject);
            this.CalculatePrefabStatus();
            List<UnityEngine.Object> list2 = new List<UnityEngine.Object>();
            this.GetObjectListFromHierarchy(list2, gameObject);
            this.RegisterNewComponents(list2, hierarchy);
        }

        public static void SetEnabledRecursive(GameObject go, bool enabled)
        {
            foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = enabled;
            }
        }

        private void SetLayer(int layer, bool includeChildren)
        {
            UnityEngine.Object[] objects = this.GetObjects(includeChildren);
            Undo.RecordObjects(objects, "Change Layer of " + this.targetTitle);
            foreach (GameObject obj2 in objects)
            {
                obj2.layer = layer;
            }
        }

        private static bool ShowMixedStaticEditorFlags(StaticEditorFlags mask)
        {
            uint num = 0;
            uint num2 = 0;
            IEnumerator enumerator = Enum.GetValues(typeof(StaticEditorFlags)).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    num2++;
                    if ((mask & ((StaticEditorFlags) current)) > 0)
                    {
                        num++;
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
            return ((num > 0) && (num != num2));
        }

        private class Styles
        {
            public GUIContent goIcon = EditorGUIUtility.IconContent("GameObject Icon");
            public GUIContent[] goTypeLabel;
            public GUIContent goTypeLabelMultiple = new GUIContent("Multiple");
            public GUIStyle header = new GUIStyle("IN GameObjectHeader");
            public GUIStyle instanceManagementInfo = new GUIStyle(EditorStyles.helpBox);
            public float layerFieldWidth = EditorStyles.boldLabel.CalcSize(EditorGUIUtility.TempContent("Layer")).x;
            public GUIStyle layerPopup = new GUIStyle(EditorStyles.popup);
            public GUIContent modelIcon = EditorGUIUtility.IconContent("PrefabModel Icon");
            public GUIContent prefabIcon = EditorGUIUtility.IconContent("PrefabNormal Icon");
            public GUIContent staticContent = EditorGUIUtility.TextContent("Static");
            public GUIStyle staticDropdown = "StaticDropdown";
            public float tagFieldWidth = EditorStyles.boldLabel.CalcSize(EditorGUIUtility.TempContent("Tag")).x;
            public GUIContent typelessIcon = EditorGUIUtility.IconContent("Prefab Icon");

            public Styles()
            {
                GUIContent[] contentArray1 = new GUIContent[8];
                contentArray1[1] = EditorGUIUtility.TextContent("Prefab");
                contentArray1[2] = EditorGUIUtility.TextContent("Model");
                contentArray1[3] = EditorGUIUtility.TextContent("Prefab");
                contentArray1[4] = EditorGUIUtility.TextContent("Model");
                contentArray1[5] = EditorGUIUtility.TextContent("Missing|The source Prefab or Model has been deleted.");
                contentArray1[6] = EditorGUIUtility.TextContent("Prefab|You have broken the prefab connection. Changes to the prefab will not be applied to this object before you Apply or Revert.");
                contentArray1[7] = EditorGUIUtility.TextContent("Model|You have broken the prefab connection. Changes to the model will not be applied to this object before you Revert.");
                this.goTypeLabel = contentArray1;
                GUIStyle style = "MiniButtonMid";
                this.instanceManagementInfo.padding = style.padding;
                this.instanceManagementInfo.alignment = style.alignment;
                this.layerPopup.margin.right = 0;
                RectOffset padding = this.header.padding;
                padding.bottom -= 3;
            }
        }
    }
}

