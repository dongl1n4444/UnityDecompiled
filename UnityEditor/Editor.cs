namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Base class to derive custom Editors from. Use this to create your own custom inspectors and editors for your objects.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public class Editor : ScriptableObject, IPreviewable
    {
        private UnityEngine.Object[] m_Targets;
        private UnityEngine.Object m_Context;
        private int m_IsDirty;
        private int m_ReferenceTargetIndex = 0;
        private PropertyHandlerCache m_PropertyHandlerCache = new PropertyHandlerCache();
        private IPreviewable m_DummyPreview;
        internal SerializedObject m_SerializedObject = null;
        private OptimizedGUIBlock m_OptimizedBlock;
        internal InspectorMode m_InspectorMode = InspectorMode.Normal;
        internal const float kLineHeight = 16f;
        internal bool hideInspector = false;
        internal static bool m_AllowMultiObjectAccess = true;
        private static Styles s_Styles;
        private const float kImageSectionWidth = 44f;
        internal bool canEditMultipleObjects =>
            (base.GetType().GetCustomAttributes(typeof(CanEditMultipleObjects), false).Length > 0);
        /// <summary>
        /// <para>Make a custom editor for targetObject or targetObjects with a context object.</para>
        /// </summary>
        /// <param name="targetObjects"></param>
        /// <param name="context"></param>
        /// <param name="editorType"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern Editor CreateEditorWithContext(UnityEngine.Object[] targetObjects, UnityEngine.Object context, [DefaultValue("null")] System.Type editorType);
        [ExcludeFromDocs]
        public static Editor CreateEditorWithContext(UnityEngine.Object[] targetObjects, UnityEngine.Object context)
        {
            System.Type editorType = null;
            return CreateEditorWithContext(targetObjects, context, editorType);
        }

        public static void CreateCachedEditorWithContext(UnityEngine.Object targetObject, UnityEngine.Object context, System.Type editorType, ref Editor previousEditor)
        {
            UnityEngine.Object[] targetObjects = new UnityEngine.Object[] { targetObject };
            CreateCachedEditorWithContext(targetObjects, context, editorType, ref previousEditor);
        }

        public static void CreateCachedEditorWithContext(UnityEngine.Object[] targetObjects, UnityEngine.Object context, System.Type editorType, ref Editor previousEditor)
        {
            if (((previousEditor == null) || !ArrayUtility.ArrayEquals<UnityEngine.Object>(previousEditor.m_Targets, targetObjects)) || (previousEditor.m_Context != context))
            {
                if (previousEditor != null)
                {
                    UnityEngine.Object.DestroyImmediate(previousEditor);
                }
                previousEditor = CreateEditorWithContext(targetObjects, context, editorType);
            }
        }

        public static void CreateCachedEditor(UnityEngine.Object targetObject, System.Type editorType, ref Editor previousEditor)
        {
            UnityEngine.Object[] targetObjects = new UnityEngine.Object[] { targetObject };
            CreateCachedEditorWithContext(targetObjects, null, editorType, ref previousEditor);
        }

        public static void CreateCachedEditor(UnityEngine.Object[] targetObjects, System.Type editorType, ref Editor previousEditor)
        {
            CreateCachedEditorWithContext(targetObjects, null, editorType, ref previousEditor);
        }

        /// <summary>
        /// <para>Make a custom editor for targetObject or targetObjects.</para>
        /// </summary>
        /// <param name="objects">All objects must be of same exact type.</param>
        /// <param name="targetObject"></param>
        /// <param name="editorType"></param>
        /// <param name="targetObjects"></param>
        [ExcludeFromDocs]
        public static Editor CreateEditor(UnityEngine.Object targetObject)
        {
            System.Type editorType = null;
            return CreateEditor(targetObject, editorType);
        }

        /// <summary>
        /// <para>Make a custom editor for targetObject or targetObjects.</para>
        /// </summary>
        /// <param name="objects">All objects must be of same exact type.</param>
        /// <param name="targetObject"></param>
        /// <param name="editorType"></param>
        /// <param name="targetObjects"></param>
        public static Editor CreateEditor(UnityEngine.Object targetObject, [DefaultValue("null")] System.Type editorType)
        {
            UnityEngine.Object[] targetObjects = new UnityEngine.Object[] { targetObject };
            return CreateEditorWithContext(targetObjects, null, editorType);
        }

        /// <summary>
        /// <para>Make a custom editor for targetObject or targetObjects.</para>
        /// </summary>
        /// <param name="objects">All objects must be of same exact type.</param>
        /// <param name="targetObject"></param>
        /// <param name="editorType"></param>
        /// <param name="targetObjects"></param>
        [ExcludeFromDocs]
        public static Editor CreateEditor(UnityEngine.Object[] targetObjects)
        {
            System.Type editorType = null;
            return CreateEditor(targetObjects, editorType);
        }

        /// <summary>
        /// <para>Make a custom editor for targetObject or targetObjects.</para>
        /// </summary>
        /// <param name="objects">All objects must be of same exact type.</param>
        /// <param name="targetObject"></param>
        /// <param name="editorType"></param>
        /// <param name="targetObjects"></param>
        public static Editor CreateEditor(UnityEngine.Object[] targetObjects, [DefaultValue("null")] System.Type editorType) => 
            CreateEditorWithContext(targetObjects, null, editorType);

        /// <summary>
        /// <para>The object being inspected.</para>
        /// </summary>
        public UnityEngine.Object target
        {
            get => 
                this.m_Targets[this.referenceTargetIndex];
            set
            {
                throw new InvalidOperationException("You can't set the target on an editor.");
            }
        }
        /// <summary>
        /// <para>An array of all the object being inspected.</para>
        /// </summary>
        public UnityEngine.Object[] targets
        {
            get
            {
                if (!m_AllowMultiObjectAccess)
                {
                    Debug.LogError("The targets array should not be used inside OnSceneGUI or OnPreviewGUI. Use the single target property instead.");
                }
                return this.m_Targets;
            }
        }
        internal virtual int referenceTargetIndex
        {
            get => 
                Mathf.Clamp(this.m_ReferenceTargetIndex, 0, this.m_Targets.Length - 1);
            set
            {
                this.m_ReferenceTargetIndex = (Math.Abs((int) (value * this.m_Targets.Length)) + value) % this.m_Targets.Length;
            }
        }
        internal virtual string targetTitle
        {
            get
            {
                if ((this.m_Targets.Length == 1) || !m_AllowMultiObjectAccess)
                {
                    return this.target.name;
                }
                object[] objArray1 = new object[] { this.m_Targets.Length, " ", ObjectNames.NicifyVariableName(ObjectNames.GetClassName(this.target)), "s" };
                return string.Concat(objArray1);
            }
        }
        /// <summary>
        /// <para>A SerializedObject representing the object or objects being inspected.</para>
        /// </summary>
        public SerializedObject serializedObject
        {
            get
            {
                if (!m_AllowMultiObjectAccess)
                {
                    Debug.LogError("The serializedObject should not be used inside OnSceneGUI or OnPreviewGUI. Use the target property directly instead.");
                }
                return this.GetSerializedObjectInternal();
            }
        }
        internal virtual SerializedObject GetSerializedObjectInternal()
        {
            if (this.m_SerializedObject == null)
            {
                this.m_SerializedObject = new SerializedObject(this.targets, this.m_Context);
            }
            return this.m_SerializedObject;
        }

        private void CleanupPropertyEditor()
        {
            if (this.m_OptimizedBlock != null)
            {
                this.m_OptimizedBlock.Dispose();
                this.m_OptimizedBlock = null;
            }
            if (this.m_SerializedObject != null)
            {
                this.m_SerializedObject.Dispose();
                this.m_SerializedObject = null;
            }
        }

        private void OnDisableINTERNAL()
        {
            this.CleanupPropertyEditor();
        }

        internal virtual void OnForceReloadInspector()
        {
            if (this.m_SerializedObject != null)
            {
                this.m_SerializedObject.SetIsDifferentCacheDirty();
            }
        }

        internal bool GetOptimizedGUIBlockImplementation(bool isDirty, bool isVisible, out OptimizedGUIBlock block, out float height)
        {
            if (this.m_OptimizedBlock == null)
            {
                this.m_OptimizedBlock = new OptimizedGUIBlock();
            }
            block = this.m_OptimizedBlock;
            if (!isVisible)
            {
                height = 0f;
                return true;
            }
            if (this.m_SerializedObject == null)
            {
                this.m_SerializedObject = new SerializedObject(this.targets, this.m_Context);
            }
            else
            {
                this.m_SerializedObject.Update();
            }
            this.m_SerializedObject.inspectorMode = this.m_InspectorMode;
            SerializedProperty iterator = this.m_SerializedObject.GetIterator();
            height = 2f;
            for (bool flag2 = true; iterator.NextVisible(flag2); flag2 = false)
            {
                height += EditorGUI.GetPropertyHeight(iterator, null, true) + 2f;
            }
            if (height == 2f)
            {
                height = 0f;
            }
            return true;
        }

        internal bool OptimizedInspectorGUIImplementation(Rect contentRect)
        {
            SerializedProperty iterator = this.m_SerializedObject.GetIterator();
            bool enterChildren = true;
            bool enabled = GUI.enabled;
            contentRect.xMin += 14f;
            contentRect.xMax -= 4f;
            contentRect.y += 2f;
            while (iterator.NextVisible(enterChildren))
            {
                contentRect.height = EditorGUI.GetPropertyHeight(iterator, null, false);
                EditorGUI.indentLevel = iterator.depth;
                using (new EditorGUI.DisabledScope((this.m_InspectorMode == InspectorMode.Normal) && ("m_Script" == iterator.propertyPath)))
                {
                    enterChildren = EditorGUI.PropertyField(contentRect, iterator);
                }
                contentRect.y += contentRect.height + 2f;
            }
            GUI.enabled = enabled;
            return this.m_SerializedObject.ApplyModifiedProperties();
        }

        protected internal static void DrawPropertiesExcluding(SerializedObject obj, params string[] propertyToExclude)
        {
            SerializedProperty iterator = obj.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (!propertyToExclude.Contains<string>(iterator.name))
                {
                    EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
                }
            }
        }

        /// <summary>
        /// <para>Draw the built-in inspector.</para>
        /// </summary>
        public bool DrawDefaultInspector() => 
            this.DoDrawDefaultInspector();

        /// <summary>
        /// <para>Implement this function to make a custom inspector.</para>
        /// </summary>
        public virtual void OnInspectorGUI()
        {
            this.DrawDefaultInspector();
        }

        /// <summary>
        /// <para>Does this edit require to be repainted constantly in its current state?</para>
        /// </summary>
        public virtual bool RequiresConstantRepaint() => 
            false;

        internal void InternalSetTargets(UnityEngine.Object[] t)
        {
            this.m_Targets = t;
        }

        internal void InternalSetHidden(bool hidden)
        {
            this.hideInspector = hidden;
        }

        internal void InternalSetContextObject(UnityEngine.Object context)
        {
            this.m_Context = context;
        }

        internal virtual bool GetOptimizedGUIBlock(bool isDirty, bool isVisible, out OptimizedGUIBlock block, out float height)
        {
            block = null;
            height = -1f;
            return false;
        }

        internal virtual bool OnOptimizedInspectorGUI(Rect contentRect)
        {
            Debug.LogError("Not supported");
            return false;
        }

        internal bool isInspectorDirty
        {
            get => 
                (this.m_IsDirty != 0);
            set
            {
                this.m_IsDirty = !value ? 0 : 1;
            }
        }
        /// <summary>
        /// <para>Repaint any inspectors that shows this editor.</para>
        /// </summary>
        public void Repaint()
        {
            InspectorWindow.RepaintAllInspectors();
        }

        /// <summary>
        /// <para>Override this method in subclasses if you implement OnPreviewGUI.</para>
        /// </summary>
        /// <returns>
        /// <para>True if this component can be Previewed in its current state.</para>
        /// </returns>
        public virtual bool HasPreviewGUI() => 
            this.preview.HasPreviewGUI();

        /// <summary>
        /// <para>Override this method if you want to change the label of the Preview area.</para>
        /// </summary>
        public virtual GUIContent GetPreviewTitle() => 
            this.preview.GetPreviewTitle();

        /// <summary>
        /// <para>Override this method if you want to render a static preview that shows.</para>
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="subAssets"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public virtual Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height) => 
            null;

        /// <summary>
        /// <para>Implement to create your own custom preview for the preview area of the inspector, primary editor headers and the object selector.</para>
        /// </summary>
        /// <param name="r">Rectangle in which to draw the preview.</param>
        /// <param name="background">Background image.</param>
        public virtual void OnPreviewGUI(Rect r, GUIStyle background)
        {
            this.preview.OnPreviewGUI(r, background);
        }

        /// <summary>
        /// <para>Implement to create your own interactive custom preview. Interactive custom previews are used in the preview area of the inspector and the object selector.</para>
        /// </summary>
        /// <param name="r">Rectangle in which to draw the preview.</param>
        /// <param name="background">Background image.</param>
        public virtual void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            this.OnPreviewGUI(r, background);
        }

        /// <summary>
        /// <para>Override this method if you want to show custom controls in the preview header.</para>
        /// </summary>
        public virtual void OnPreviewSettings()
        {
            this.preview.OnPreviewSettings();
        }

        /// <summary>
        /// <para>Implement this method to show asset information on top of the asset preview.</para>
        /// </summary>
        public virtual string GetInfoString() => 
            this.preview.GetInfoString();

        internal virtual void OnAssetStoreInspectorGUI()
        {
        }

        public virtual void ReloadPreviewInstances()
        {
            this.preview.ReloadPreviewInstances();
        }

        internal virtual IPreviewable preview
        {
            get
            {
                if (this.m_DummyPreview == null)
                {
                    this.m_DummyPreview = new ObjectPreview();
                    this.m_DummyPreview.Initialize(this.targets);
                }
                return this.m_DummyPreview;
            }
        }
        internal PropertyHandlerCache propertyHandlerCache =>
            this.m_PropertyHandlerCache;
        internal static bool DoDrawDefaultInspector(SerializedObject obj)
        {
            EditorGUI.BeginChangeCheck();
            obj.Update();
            SerializedProperty iterator = obj.GetIterator();
            for (bool flag = true; iterator.NextVisible(flag); flag = false)
            {
                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                {
                    EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
                }
            }
            obj.ApplyModifiedProperties();
            return EditorGUI.EndChangeCheck();
        }

        internal bool DoDrawDefaultInspector() => 
            DoDrawDefaultInspector(this.serializedObject);

        /// <summary>
        /// <para>Call this function to draw the header of the editor.</para>
        /// </summary>
        public void DrawHeader()
        {
            if (EditorGUIUtility.hierarchyMode)
            {
                this.DrawHeaderFromInsideHierarchy();
            }
            else
            {
                this.OnHeaderGUI();
            }
        }

        protected virtual void OnHeaderGUI()
        {
            DrawHeaderGUI(this, this.targetTitle);
        }

        internal virtual void OnHeaderControlsGUI()
        {
            GUILayoutUtility.GetRect(10f, 10f, 16f, 16f, EditorStyles.layerMaskField);
            GUILayout.FlexibleSpace();
            bool flag = true;
            if (!(this is AssetImporterInspector))
            {
                if (!AssetDatabase.IsMainAsset(this.targets[0]))
                {
                    flag = false;
                }
                AssetImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(this.targets[0]));
                if ((atPath != null) && (atPath.GetType() != typeof(AssetImporter)))
                {
                    flag = false;
                }
            }
            if (flag && GUILayout.Button("Open", EditorStyles.miniButton, new GUILayoutOption[0]))
            {
                if (this is AssetImporterInspector)
                {
                    AssetDatabase.OpenAsset((this as AssetImporterInspector).assetEditor.targets);
                }
                else
                {
                    AssetDatabase.OpenAsset(this.targets);
                }
                GUIUtility.ExitGUI();
            }
        }

        internal virtual void OnHeaderIconGUI(Rect iconRect)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            Texture2D image = null;
            if (!this.HasPreviewGUI())
            {
                bool flag = AssetPreview.IsLoadingAssetPreview(this.target.GetInstanceID());
                image = AssetPreview.GetAssetPreview(this.target);
                if (image == null)
                {
                    if (flag)
                    {
                        this.Repaint();
                    }
                    image = AssetPreview.GetMiniThumbnail(this.target);
                }
            }
            if (this.HasPreviewGUI())
            {
                this.OnPreviewGUI(iconRect, s_Styles.inspectorBigInner);
            }
            else if (image != null)
            {
                GUI.Label(iconRect, image, s_Styles.centerStyle);
            }
        }

        internal virtual void OnHeaderTitleGUI(Rect titleRect, string header)
        {
            titleRect.yMin -= 2f;
            titleRect.yMax += 2f;
            GUI.Label(titleRect, header, EditorStyles.largeLabel);
        }

        internal virtual void DrawHeaderHelpAndSettingsGUI(Rect r)
        {
            UnityEngine.Object target = this.target;
            Vector2 vector = EditorStyles.iconButton.CalcSize(EditorGUI.GUIContents.titleSettingsIcon);
            float x = vector.x;
            Rect position = new Rect(r.xMax - x, r.y + 5f, vector.x, vector.y);
            if (EditorGUI.DropdownButton(position, EditorGUI.GUIContents.titleSettingsIcon, FocusType.Passive, EditorStyles.iconButton))
            {
                EditorUtility.DisplayObjectContextMenu(position, this.targets, 0);
            }
            x += vector.x;
            EditorGUI.HelpIconButton(new Rect(r.xMax - x, r.y + 5f, vector.x, vector.y), target);
        }

        private void DrawHeaderFromInsideHierarchy()
        {
            GUIStyle style = GUILayoutUtility.topLevel.style;
            EditorGUILayout.EndVertical();
            this.OnHeaderGUI();
            EditorGUILayout.BeginVertical(style, new GUILayoutOption[0]);
        }

        internal static Rect DrawHeaderGUI(Editor editor, string header) => 
            DrawHeaderGUI(editor, header, 0f);

        internal static Rect DrawHeaderGUI(Editor editor, string header, float leftMargin)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            GUILayout.BeginHorizontal(s_Styles.inspectorBig, new GUILayoutOption[0]);
            GUILayout.Space(38f);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Space(19f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (leftMargin > 0f)
            {
                GUILayout.Space(leftMargin);
            }
            if (editor != null)
            {
                editor.OnHeaderControlsGUI();
            }
            else
            {
                EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            Rect lastRect = GUILayoutUtility.GetLastRect();
            Rect r = new Rect(lastRect.x + leftMargin, lastRect.y, lastRect.width - leftMargin, lastRect.height);
            Rect iconRect = new Rect(r.x + 6f, r.y + 6f, 32f, 32f);
            if (editor != null)
            {
                editor.OnHeaderIconGUI(iconRect);
            }
            else
            {
                GUI.Label(iconRect, AssetPreview.GetMiniTypeThumbnail(typeof(UnityEngine.Object)), s_Styles.centerStyle);
            }
            Rect titleRect = new Rect(r.x + 44f, r.y + 6f, ((r.width - 44f) - 38f) - 4f, 16f);
            if (editor != null)
            {
                editor.OnHeaderTitleGUI(titleRect, header);
            }
            else
            {
                GUI.Label(titleRect, header, EditorStyles.largeLabel);
            }
            if (editor != null)
            {
                editor.DrawHeaderHelpAndSettingsGUI(r);
            }
            Event current = Event.current;
            if (((editor != null) && (current.type == EventType.MouseDown)) && ((current.button == 1) && r.Contains(current.mousePosition)))
            {
                EditorUtility.DisplayObjectContextMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), editor.targets, 0);
                current.Use();
            }
            return lastRect;
        }

        /// <summary>
        /// <para>The first entry point for Preview Drawing.</para>
        /// </summary>
        /// <param name="previewPosition">The available area to draw the preview.</param>
        /// <param name="previewArea"></param>
        public virtual void DrawPreview(Rect previewArea)
        {
            ObjectPreview.DrawPreview(this, previewArea, this.targets);
        }

        internal bool CanBeExpandedViaAFoldout()
        {
            if (this.m_SerializedObject == null)
            {
                this.m_SerializedObject = new SerializedObject(this.targets, this.m_Context);
            }
            else
            {
                this.m_SerializedObject.Update();
            }
            this.m_SerializedObject.inspectorMode = this.m_InspectorMode;
            SerializedProperty iterator = this.m_SerializedObject.GetIterator();
            for (bool flag = true; iterator.NextVisible(flag); flag = false)
            {
                if (EditorGUI.GetPropertyHeight(iterator, null, true) > 0f)
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsAppropriateFileOpenForEdit(UnityEngine.Object assetObject)
        {
            string str;
            return IsAppropriateFileOpenForEdit(assetObject, out str);
        }

        internal static bool IsAppropriateFileOpenForEdit(UnityEngine.Object assetObject, out string message)
        {
            message = string.Empty;
            if (AssetDatabase.IsNativeAsset(assetObject))
            {
                if (!AssetDatabase.IsOpenForEdit(assetObject, out message, StatusQueryOptions.UseCachedIfPossible))
                {
                    return false;
                }
            }
            else if (AssetDatabase.IsForeignAsset(assetObject) && !AssetDatabase.IsMetaFileOpenForEdit(assetObject, out message, StatusQueryOptions.UseCachedIfPossible))
            {
                return false;
            }
            return true;
        }

        internal virtual bool IsEnabled()
        {
            foreach (UnityEngine.Object obj2 in this.targets)
            {
                if ((obj2.hideFlags & HideFlags.NotEditable) != HideFlags.None)
                {
                    return false;
                }
                if (EditorUtility.IsPersistent(obj2) && !IsAppropriateFileOpenForEdit(obj2))
                {
                    return false;
                }
            }
            return true;
        }

        internal bool IsOpenForEdit()
        {
            string str;
            return this.IsOpenForEdit(out str);
        }

        internal bool IsOpenForEdit(out string message)
        {
            message = "";
            foreach (UnityEngine.Object obj2 in this.targets)
            {
                if (EditorUtility.IsPersistent(obj2) && !IsAppropriateFileOpenForEdit(obj2))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// <para>Override this method in subclasses to return false if you don't want default margins.</para>
        /// </summary>
        public virtual bool UseDefaultMargins() => 
            true;

        public void Initialize(UnityEngine.Object[] targets)
        {
            throw new InvalidOperationException("You shouldn't call Initialize for Editors");
        }

        public bool MoveNextTarget()
        {
            this.referenceTargetIndex++;
            return (this.referenceTargetIndex < this.targets.Length);
        }

        public void ResetTarget()
        {
            this.referenceTargetIndex = 0;
        }
        private class Styles
        {
            public GUIStyle centerStyle = new GUIStyle();
            public GUIStyle inspectorBig = new GUIStyle(EditorStyles.inspectorBig);
            public GUIStyle inspectorBigInner = new GUIStyle("IN BigTitle inner");

            public Styles()
            {
                this.centerStyle.alignment = TextAnchor.MiddleCenter;
                RectOffset padding = this.inspectorBig.padding;
                padding.bottom--;
            }
        }
    }
}

