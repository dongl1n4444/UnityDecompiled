namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor.Sprites;
    using UnityEditor.U2D;
    using UnityEditor.U2D.Interface;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.U2D.Interface;

    internal class SpriteEditorWindow : SpriteUtilityWindow, ISpriteEditor
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <textureIsDirty>k__BackingField;
        private const float k_MarginForFraming = 0.05f;
        private const float k_ModuleListWidth = 90f;
        private const float k_WarningMessageHeight = 40f;
        private const float k_WarningMessageWidth = 250f;
        private List<ISpriteEditorModule> m_AllRegisteredModules;
        private IAssetDatabase m_AssetDatabase;
        private ISpriteEditorModule m_CurrentModule = null;
        private int m_CurrentModuleIndex = 0;
        private IEventSystem m_EventSystem;
        private IGUIUtility m_GUIUtility;
        public bool m_IgnoreNextPostprocessEvent;
        public ITexture2D m_OriginalTexture;
        private UnityEngine.Texture2D m_OutlineTexture;
        private UnityEngine.Texture2D m_ReadableTexture;
        private SpriteRectCache m_RectsCache;
        private GUIContent[] m_RegisteredModuleNames;
        private List<ISpriteEditorModule> m_RegisteredModules;
        private bool m_RequestRepaint = false;
        public bool m_ResetOnNextRepaint;
        [SerializeField]
        private SpriteRect m_Selected;
        public string m_SelectedAssetPath;
        private SpriteEditorWindowStyles m_SpriteEditorWindowStyles;
        private UnityEditor.TextureImporter m_TextureImporter;
        private SerializedObject m_TextureImporterSO;
        private SerializedProperty m_TextureImporterSprites;
        private IUndoSystem m_UndoSystem;
        public static SpriteEditorWindow s_Instance;
        public static bool s_OneClickDragStarted = false;

        private void ApplyCacheSettingsToInspector(SerializedObject so)
        {
            if ((this.m_TextureImporterSO != null) && (this.m_TextureImporterSO.targetObject == so.targetObject))
            {
                if (so.FindProperty("m_SpriteMode").intValue == this.m_TextureImporterSO.FindProperty("m_SpriteMode").intValue)
                {
                    s_Instance.m_IgnoreNextPostprocessEvent = true;
                }
                else if (this.textureIsDirty && EditorUtility.DisplayDialog(SpriteEditorWindowStyles.spriteEditorWindowTitle.text, SpriteEditorWindowStyles.pendingChangesDialogContent.text, SpriteEditorWindowStyles.yesButtonLabel.text, SpriteEditorWindowStyles.noButtonLabel.text))
                {
                    this.DoApply(so);
                }
            }
        }

        public void ClearProgressBar()
        {
            EditorUtility.ClearProgressBar();
        }

        public void DisplayProgressBar(string title, string content, float progress)
        {
            EditorUtility.DisplayProgressBar(title, content, progress);
        }

        private void DoApply()
        {
            this.m_UndoSystem.ClearUndo(this.m_RectsCache);
            this.DoApply(this.m_TextureImporterSO);
            this.m_TextureImporterSO.ApplyModifiedPropertiesWithoutUndo();
            this.m_IgnoreNextPostprocessEvent = true;
            this.DoTextureReimport(this.m_TextureImporter.assetPath);
            this.textureIsDirty = false;
            this.selectedSpriteRect = null;
        }

        private void DoApply(SerializedObject so)
        {
            if (this.multipleSprites)
            {
                List<string> list = new List<string>();
                List<string> list2 = new List<string>();
                SerializedProperty property = so.FindProperty("m_SpriteSheet.m_Sprites");
                for (int i = 0; i < this.m_RectsCache.Count; i++)
                {
                    SpriteRect rect = this.m_RectsCache.RectAt(i);
                    if (string.IsNullOrEmpty(rect.name))
                    {
                        rect.name = "Empty";
                    }
                    if (!string.IsNullOrEmpty(rect.originalName))
                    {
                        list.Add(rect.originalName);
                        list2.Add(rect.name);
                    }
                    if (property.arraySize < this.m_RectsCache.Count)
                    {
                        property.InsertArrayElementAtIndex(property.arraySize);
                    }
                    SerializedProperty arrayElementAtIndex = property.GetArrayElementAtIndex(i);
                    rect.ApplyToSerializedProperty(arrayElementAtIndex);
                    EditorUtility.DisplayProgressBar(SpriteEditorWindowStyles.saveProgressTitle.text, string.Format(SpriteEditorWindowStyles.saveContentText.text, i, this.m_RectsCache.Count), ((float) i) / ((float) this.m_RectsCache.Count));
                }
                while (this.m_RectsCache.Count < property.arraySize)
                {
                    property.DeleteArrayElementAtIndex(this.m_RectsCache.Count);
                }
                if (list.Count > 0)
                {
                    PatchImportSettingRecycleID.PatchMultiple(so, 0xd5, list.ToArray(), list2.ToArray());
                }
            }
            else if (this.m_RectsCache.Count > 0)
            {
                SpriteRect rect2 = this.m_RectsCache.RectAt(0);
                so.FindProperty("m_Alignment").intValue = (int) rect2.alignment;
                so.FindProperty("m_SpriteBorder").vector4Value = rect2.border;
                so.FindProperty("m_SpritePivot").vector2Value = rect2.pivot;
                so.FindProperty("m_SpriteTessellationDetail").floatValue = rect2.tessellationDetail;
                SerializedProperty outlineSP = so.FindProperty("m_SpriteSheet.m_Outline");
                if (rect2.outline != null)
                {
                    SpriteRect.ApplyOutlineChanges(outlineSP, rect2.outline);
                }
                else
                {
                    outlineSP.ClearArray();
                }
                SerializedProperty property4 = so.FindProperty("m_SpriteSheet.m_PhysicsShape");
                if (rect2.physicsShape != null)
                {
                    SpriteRect.ApplyOutlineChanges(property4, rect2.physicsShape);
                }
                else
                {
                    property4.ClearArray();
                }
            }
            EditorUtility.ClearProgressBar();
        }

        private void DoEditingDisabledMessage()
        {
            if (this.IsEditingDisabled())
            {
                GUILayout.BeginArea(this.warningMessageRect);
                EditorGUILayout.HelpBox(SpriteEditorWindowStyles.editingDisableMessageLabel.text, MessageType.Warning);
                GUILayout.EndArea();
            }
        }

        private void DoRevert()
        {
            this.textureIsDirty = false;
            this.selectedSpriteRect = null;
            this.RefreshRects();
            GUI.FocusControl("");
        }

        protected override void DoTextureGUIExtras()
        {
            this.HandleFrameSelected();
            if (this.m_EventSystem.current.type == EventType.Repaint)
            {
                SpriteEditorUtility.BeginLines(new Color(1f, 1f, 1f, 0.5f));
                for (int i = 0; i < this.m_RectsCache.Count; i++)
                {
                    if (this.m_RectsCache.RectAt(i) != this.selectedSpriteRect)
                    {
                        SpriteEditorUtility.DrawBox(this.m_RectsCache.RectAt(i).rect);
                    }
                }
                SpriteEditorUtility.EndLines();
            }
            this.m_CurrentModule.DoTextureGUI();
        }

        public void DoTextureReimport(string path)
        {
            if (this.m_TextureImporterSO != null)
            {
                try
                {
                    AssetDatabase.StartAssetEditing();
                    AssetDatabase.ImportAsset(path);
                }
                finally
                {
                    AssetDatabase.StopAssetEditing();
                }
                this.textureIsDirty = false;
            }
        }

        private void DoToolbarGUI()
        {
            GUIStyle toolbar = EditorStyles.toolbar;
            Rect position = new Rect(0f, 0f, base.position.width, 17f);
            if (this.m_EventSystem.current.type == EventType.Repaint)
            {
                toolbar.Draw(position, false, false, false, false);
            }
            base.m_TextureViewRect = new Rect(0f, 17f, base.position.width - 16f, (base.position.height - 16f) - 17f);
            if (this.m_RegisteredModules.Count > 1)
            {
                int newModuleIndex = EditorGUI.Popup(new Rect(0f, 0f, 90f, 17f), this.m_CurrentModuleIndex, this.m_RegisteredModuleNames, EditorStyles.toolbarPopup);
                if (newModuleIndex != this.m_CurrentModuleIndex)
                {
                    if (this.textureIsDirty)
                    {
                        if (EditorUtility.DisplayDialog(SpriteEditorWindowStyles.applyRevertModuleDialogTitle.text, SpriteEditorWindowStyles.applyRevertModuleDialogContent.text, SpriteEditorWindowStyles.applyButtonLabel.text, SpriteEditorWindowStyles.revertButtonLabel.text))
                        {
                            this.DoApply();
                        }
                        else
                        {
                            this.DoRevert();
                        }
                    }
                    this.SetupModule(newModuleIndex);
                }
                position.x = 90f;
            }
            position = base.DoAlphaZoomToolbarGUI(position);
            Rect rect5 = position;
            rect5.x = rect5.width;
            using (new EditorGUI.DisabledScope(!this.textureIsDirty))
            {
                rect5.width = EditorStyles.toolbarButton.CalcSize(SpriteEditorWindowStyles.applyButtonLabel).x;
                rect5.x -= rect5.width;
                if (GUI.Button(rect5, SpriteEditorWindowStyles.applyButtonLabel, EditorStyles.toolbarButton))
                {
                    this.DoApply();
                    this.SetupModule(this.m_CurrentModuleIndex);
                }
                rect5.width = EditorStyles.toolbarButton.CalcSize(SpriteEditorWindowStyles.revertButtonLabel).x;
                rect5.x -= rect5.width;
                if (GUI.Button(rect5, SpriteEditorWindowStyles.revertButtonLabel, EditorStyles.toolbarButton))
                {
                    this.DoRevert();
                    this.SetupModule(this.m_CurrentModuleIndex);
                }
            }
            position.width = rect5.x - position.x;
            this.m_CurrentModule.DrawToolbarGUI(position);
        }

        public ITexture2D GetReadableTexture2D()
        {
            if (this.m_ReadableTexture == null)
            {
                ITextureImporter assetImporterFromPath = this.m_AssetDatabase.GetAssetImporterFromPath(this.m_SelectedAssetPath);
                int width = 0;
                int height = 0;
                assetImporterFromPath.GetWidthAndHeight(ref width, ref height);
                this.m_ReadableTexture = UnityEditor.SpriteUtility.CreateTemporaryDuplicate((UnityEngine.Texture2D) this.m_OriginalTexture, width, height);
                if (this.m_ReadableTexture != null)
                {
                    this.m_ReadableTexture.filterMode = UnityEngine.FilterMode.Point;
                }
            }
            return new UnityEngine.U2D.Interface.Texture2D(this.m_ReadableTexture);
        }

        private ITexture2D GetSelectedTexture2D()
        {
            UnityEngine.Texture2D o = null;
            if (Selection.activeObject is UnityEngine.Texture2D)
            {
                o = Selection.activeObject as UnityEngine.Texture2D;
            }
            else if (Selection.activeObject is Sprite)
            {
                o = UnityEditor.Sprites.SpriteUtility.GetSpriteTexture(Selection.activeObject as Sprite, false);
            }
            else if (((Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<SpriteRenderer>() != null)) && (Selection.activeGameObject.GetComponent<SpriteRenderer>().sprite != null))
            {
                o = UnityEditor.Sprites.SpriteUtility.GetSpriteTexture(Selection.activeGameObject.GetComponent<SpriteRenderer>().sprite, false);
            }
            if (o != null)
            {
                this.m_SelectedAssetPath = this.m_AssetDatabase.GetAssetPath(o);
            }
            return new UnityEngine.U2D.Interface.Texture2D(o);
        }

        public static void GetWindow()
        {
            EditorWindow.GetWindow<SpriteEditorWindow>();
        }

        private void HandleApplyRevertDialog()
        {
            if (this.textureIsDirty && (this.m_TextureImporter != null))
            {
                if (EditorUtility.DisplayDialog(SpriteEditorWindowStyles.applyRevertDialogTitle.text, string.Format(SpriteEditorWindowStyles.applyRevertDialogContent.text, this.m_TextureImporter.assetPath), SpriteEditorWindowStyles.applyButtonLabel.text, SpriteEditorWindowStyles.revertButtonLabel.text))
                {
                    this.DoApply();
                }
                else
                {
                    this.DoRevert();
                }
                this.SetupModule(this.m_CurrentModuleIndex);
            }
        }

        private void HandleFrameSelected()
        {
            IEvent current = this.m_EventSystem.current;
            if (((current.type == EventType.ValidateCommand) || (current.type == EventType.ExecuteCommand)) && (current.commandName == "FrameSelected"))
            {
                if (current.type == EventType.ExecuteCommand)
                {
                    if (this.selectedSpriteRect == null)
                    {
                        return;
                    }
                    Rect rect = this.selectedSpriteRect.rect;
                    float zoom = base.m_Zoom;
                    if (rect.width < rect.height)
                    {
                        zoom = this.m_TextureViewRect.height / (rect.height + (this.m_TextureViewRect.height * 0.05f));
                    }
                    else
                    {
                        zoom = this.m_TextureViewRect.width / (rect.width + (this.m_TextureViewRect.width * 0.05f));
                    }
                    base.m_Zoom = zoom;
                    this.m_ScrollPosition.x = (rect.center.x - (base.m_Texture.width * 0.5f)) * base.m_Zoom;
                    this.m_ScrollPosition.y = ((rect.center.y - (base.m_Texture.height * 0.5f)) * base.m_Zoom) * -1f;
                    base.Repaint();
                }
                current.Use();
            }
        }

        public void HandleSpriteSelection()
        {
            if (((this.m_EventSystem.current.type == EventType.MouseDown) && (this.m_EventSystem.current.button == 0)) && ((GUIUtility.hotControl == 0) && !this.m_EventSystem.current.alt))
            {
                SpriteRect selectedSpriteRect = this.selectedSpriteRect;
                this.selectedSpriteRect = this.TrySelect(this.m_EventSystem.current.mousePosition);
                if (this.selectedSpriteRect != null)
                {
                    s_OneClickDragStarted = true;
                }
                else
                {
                    this.RequestRepaint();
                }
                if ((selectedSpriteRect != this.selectedSpriteRect) && (this.selectedSpriteRect != null))
                {
                    this.m_EventSystem.current.Use();
                }
            }
        }

        private void InitModules()
        {
            this.m_AllRegisteredModules = new List<ISpriteEditorModule>();
            if (this.m_OutlineTexture == null)
            {
                this.m_OutlineTexture = new UnityEngine.Texture2D(1, 0x10, TextureFormat.RGBA32, false);
                Color[] colors = new Color[] { new Color(0.5f, 0.5f, 0.5f, 0.5f), new Color(0.5f, 0.5f, 0.5f, 0.5f), new Color(0.8f, 0.8f, 0.8f, 0.8f), new Color(0.8f, 0.8f, 0.8f, 0.8f), Color.white, Color.white, Color.white, Color.white, new Color(0.8f, 0.8f, 0.8f, 1f), new Color(0.5f, 0.5f, 0.5f, 0.8f), new Color(0.3f, 0.3f, 0.3f, 0.5f), new Color(0.3f, 0.3f, 0.3f, 0.5f), new Color(0.3f, 0.3f, 0.3f, 0.3f), new Color(0.3f, 0.3f, 0.3f, 0.3f), new Color(0.1f, 0.1f, 0.1f, 0.1f), new Color(0.1f, 0.1f, 0.1f, 0.1f) };
                this.m_OutlineTexture.SetPixels(colors);
                this.m_OutlineTexture.Apply();
                this.m_OutlineTexture.hideFlags = HideFlags.HideAndDontSave;
            }
            UnityEngine.U2D.Interface.Texture2D outlineTexture = new UnityEngine.U2D.Interface.Texture2D(this.m_OutlineTexture);
            this.m_AllRegisteredModules.Add(new SpriteFrameModule(this, this.m_EventSystem, this.m_UndoSystem, this.m_AssetDatabase));
            this.m_AllRegisteredModules.Add(new SpritePolygonModeModule(this, this.m_EventSystem, this.m_UndoSystem, this.m_AssetDatabase));
            this.m_AllRegisteredModules.Add(new SpriteOutlineModule(this, this.m_EventSystem, this.m_UndoSystem, this.m_AssetDatabase, this.m_GUIUtility, new ShapeEditorFactory(), outlineTexture));
            this.m_AllRegisteredModules.Add(new SpritePhysicsShapeModule(this, this.m_EventSystem, this.m_UndoSystem, this.m_AssetDatabase, this.m_GUIUtility, new ShapeEditorFactory(), outlineTexture));
            this.UpdateAvailableModules();
        }

        public void InvalidatePropertiesCache()
        {
            if (this.m_RectsCache != null)
            {
                this.m_RectsCache.ClearAll();
                UnityEngine.Object.DestroyImmediate(this.m_RectsCache);
            }
            if (this.m_ReadableTexture != null)
            {
                UnityEngine.Object.DestroyImmediate(this.m_ReadableTexture);
                this.m_ReadableTexture = null;
            }
            this.m_OriginalTexture = null;
            this.m_TextureImporter = null;
            this.m_TextureImporterSO = null;
            this.m_TextureImporterSprites = null;
        }

        public bool IsEditingDisabled() => 
            EditorApplication.isPlayingOrWillChangePlaymode;

        private void ModifierKeysChanged()
        {
            if (EditorWindow.focusedWindow == this)
            {
                base.Repaint();
            }
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            if (this.m_RectsCache != null)
            {
                Undo.ClearUndo(this.m_RectsCache);
            }
            this.HandleApplyRevertDialog();
            this.InvalidatePropertiesCache();
            EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.ModifierKeysChanged));
            s_Instance = null;
            if (this.m_OutlineTexture != null)
            {
                UnityEngine.Object.DestroyImmediate(this.m_OutlineTexture);
                this.m_OutlineTexture = null;
            }
            if (this.m_ReadableTexture != null)
            {
                UnityEngine.Object.DestroyImmediate(this.m_ReadableTexture);
                this.m_ReadableTexture = null;
            }
        }

        private void OnEnable()
        {
            this.m_EventSystem = new EventSystem();
            this.m_UndoSystem = new UndoSystem();
            this.m_AssetDatabase = new AssetDatabaseSystem();
            this.m_GUIUtility = new GUIUtilitySystem();
            base.minSize = new Vector2(360f, 200f);
            base.titleContent = SpriteEditorWindowStyles.spriteEditorWindowTitle;
            s_Instance = this;
            this.m_UndoSystem.RegisterUndoCallback(new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.ModifierKeysChanged));
            this.ResetWindow();
            this.RefreshPropertiesCache();
            this.RefreshRects();
            this.InitModules();
        }

        private void OnFocus()
        {
            if (this.selectedTextureChanged)
            {
                this.OnSelectionChange();
            }
        }

        private void OnGUI()
        {
            base.InitStyles();
            if (this.m_ResetOnNextRepaint || this.selectedTextureChanged)
            {
                this.ResetWindow();
                this.RefreshPropertiesCache();
                this.RefreshRects();
                this.UpdateAvailableModules();
                this.SetupModule(this.m_CurrentModuleIndex);
                this.m_ResetOnNextRepaint = false;
            }
            Matrix4x4 matrix = Handles.matrix;
            if (!this.activeTextureSelected)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    GUILayout.Label(SpriteEditorWindowStyles.noSelectionWarning, new GUILayoutOption[0]);
                }
            }
            else
            {
                this.DoToolbarGUI();
                base.DoTextureGUI();
                this.DoEditingDisabledMessage();
                this.m_CurrentModule.OnPostGUI();
                Handles.matrix = matrix;
                if (this.m_RequestRepaint)
                {
                    base.Repaint();
                }
            }
        }

        private void OnSelectionChange()
        {
            if ((this.GetSelectedTexture2D() == null) || this.selectedTextureChanged)
            {
                this.HandleApplyRevertDialog();
                this.ResetWindow();
                this.RefreshPropertiesCache();
                this.RefreshRects();
            }
            if (this.m_RectsCache != null)
            {
                if (Selection.activeObject is Sprite)
                {
                    this.UpdateSelectedSpriteRect(Selection.activeObject as Sprite);
                }
                else if ((Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<SpriteRenderer>() != null))
                {
                    Sprite sprite = Selection.activeGameObject.GetComponent<SpriteRenderer>().sprite;
                    this.UpdateSelectedSpriteRect(sprite);
                }
            }
            this.UpdateAvailableModules();
            base.Repaint();
        }

        public void RefreshPropertiesCache()
        {
            this.m_OriginalTexture = this.GetSelectedTexture2D();
            if (this.m_OriginalTexture != null)
            {
                this.m_TextureImporter = AssetImporter.GetAtPath(this.m_SelectedAssetPath) as UnityEditor.TextureImporter;
                if (this.m_TextureImporter != null)
                {
                    this.m_TextureImporterSO = new SerializedObject(this.m_TextureImporter);
                    this.m_TextureImporterSprites = this.m_TextureImporterSO.FindProperty("m_SpriteSheet.m_Sprites");
                    base.m_Texture = (this.m_OriginalTexture != null) ? new PreviewTexture2D((UnityEngine.Texture2D) this.m_OriginalTexture) : null;
                }
            }
        }

        private void RefreshRects()
        {
            if (this.m_TextureImporterSprites != null)
            {
                if (this.m_RectsCache != null)
                {
                    this.m_RectsCache.ClearAll();
                    Undo.ClearUndo(this.m_RectsCache);
                    UnityEngine.Object.DestroyImmediate(this.m_RectsCache);
                }
                this.m_RectsCache = ScriptableObject.CreateInstance<SpriteRectCache>();
                if (this.multipleSprites)
                {
                    for (int i = 0; i < this.m_TextureImporterSprites.arraySize; i++)
                    {
                        SpriteRect r = new SpriteRect();
                        r.LoadFromSerializedProperty(this.m_TextureImporterSprites.GetArrayElementAtIndex(i));
                        this.m_RectsCache.AddRect(r);
                        EditorUtility.DisplayProgressBar(SpriteEditorWindowStyles.loadProgressTitle.text, string.Format(SpriteEditorWindowStyles.loadContentText.text, i, this.m_TextureImporterSprites.arraySize), ((float) i) / ((float) this.m_TextureImporterSprites.arraySize));
                    }
                }
                else if (this.validSprite)
                {
                    SpriteRect rect2 = new SpriteRect {
                        rect = new Rect(0f, 0f, (float) base.m_Texture.width, (float) base.m_Texture.height),
                        name = this.m_OriginalTexture.name,
                        alignment = (SpriteAlignment) this.m_TextureImporterSO.FindProperty("m_Alignment").intValue,
                        border = this.m_TextureImporter.spriteBorder
                    };
                    rect2.pivot = SpriteEditorUtility.GetPivotValue(rect2.alignment, this.m_TextureImporter.spritePivot);
                    rect2.tessellationDetail = this.m_TextureImporterSO.FindProperty("m_SpriteTessellationDetail").floatValue;
                    SerializedProperty outlineSP = this.m_TextureImporterSO.FindProperty("m_SpriteSheet.m_Outline");
                    rect2.outline = SpriteRect.AcquireOutline(outlineSP);
                    SerializedProperty property2 = this.m_TextureImporterSO.FindProperty("m_SpriteSheet.m_PhysicsShape");
                    rect2.physicsShape = SpriteRect.AcquireOutline(property2);
                    this.m_RectsCache.AddRect(rect2);
                }
                EditorUtility.ClearProgressBar();
                if (this.m_RectsCache.Count > 0)
                {
                    this.selectedSpriteRect = this.m_RectsCache.RectAt(0);
                }
            }
        }

        public void RequestRepaint()
        {
            if (EditorWindow.focusedWindow != this)
            {
                base.Repaint();
            }
            else
            {
                this.m_RequestRepaint = true;
            }
        }

        public void ResetWindow()
        {
            this.InvalidatePropertiesCache();
            this.selectedSpriteRect = null;
            this.textureIsDirty = false;
            base.m_Zoom = -1f;
        }

        public void SetDataModified()
        {
            this.textureIsDirty = true;
        }

        private void SetupModule(int newModuleIndex)
        {
            if (s_Instance != null)
            {
                if (this.m_CurrentModule != null)
                {
                    this.m_CurrentModule.OnModuleDeactivate();
                }
                if (this.m_RegisteredModules.Count > newModuleIndex)
                {
                    this.m_CurrentModule = this.m_RegisteredModules[newModuleIndex];
                    this.m_CurrentModule.OnModuleActivate();
                    this.m_CurrentModuleIndex = newModuleIndex;
                }
            }
        }

        public static void TextureImporterApply(SerializedObject so)
        {
            if (s_Instance != null)
            {
                s_Instance.ApplyCacheSettingsToInspector(so);
            }
        }

        private SpriteRect TrySelect(Vector2 mousePosition)
        {
            float maxValue = float.MaxValue;
            SpriteRect rect = null;
            mousePosition = Handles.inverseMatrix.MultiplyPoint((Vector3) mousePosition);
            for (int i = 0; i < this.m_RectsCache.Count; i++)
            {
                SpriteRect rect2 = this.m_RectsCache.RectAt(i);
                if (rect2.rect.Contains(mousePosition))
                {
                    if (rect2 == this.selectedSpriteRect)
                    {
                        return rect2;
                    }
                    float width = rect2.rect.width;
                    float height = rect2.rect.height;
                    float num5 = width * height;
                    if (((width > 0f) && (height > 0f)) && (num5 < maxValue))
                    {
                        rect = rect2;
                        maxValue = num5;
                    }
                }
            }
            return rect;
        }

        private void UndoRedoPerformed()
        {
            ITexture2D textured = this.GetSelectedTexture2D();
            if ((textured != null) && (this.m_OriginalTexture != textured))
            {
                this.OnSelectionChange();
            }
            if ((this.m_RectsCache != null) && !this.m_RectsCache.Contains(this.selectedSpriteRect))
            {
                this.selectedSpriteRect = null;
            }
            base.Repaint();
        }

        private void UpdateAvailableModules()
        {
            if (this.m_AllRegisteredModules != null)
            {
                this.m_RegisteredModules = new List<ISpriteEditorModule>();
                foreach (ISpriteEditorModule module in this.m_AllRegisteredModules)
                {
                    if (module.CanBeActivated())
                    {
                        this.m_RegisteredModules.Add(module);
                    }
                }
                this.m_RegisteredModuleNames = new GUIContent[this.m_RegisteredModules.Count];
                for (int i = 0; i < this.m_RegisteredModules.Count; i++)
                {
                    this.m_RegisteredModuleNames[i] = new GUIContent(this.m_RegisteredModules[i].moduleName);
                }
                if (!this.m_RegisteredModules.Contains(this.m_CurrentModule))
                {
                    this.SetupModule(0);
                }
                else
                {
                    this.SetupModule(this.m_CurrentModuleIndex);
                }
            }
        }

        private void UpdateSelectedSpriteRect(Sprite sprite)
        {
            for (int i = 0; i < this.m_RectsCache.Count; i++)
            {
                if (sprite.rect == this.m_RectsCache.RectAt(i).rect)
                {
                    this.selectedSpriteRect = this.m_RectsCache.RectAt(i);
                    return;
                }
            }
            this.selectedSpriteRect = null;
        }

        private bool activeTextureSelected =>
            (((this.m_TextureImporter != null) && (base.m_Texture != null)) && (this.m_OriginalTexture != null));

        public bool editingDisabled =>
            EditorApplication.isPlayingOrWillChangePlaymode;

        public bool enableMouseMoveEvent
        {
            set
            {
                base.wantsMouseMove = value;
            }
        }

        private bool multipleSprites =>
            ((this.m_TextureImporter != null) && (this.m_TextureImporter.spriteImportMode == SpriteImportMode.Multiple));

        public ITexture2D previewTexture =>
            base.m_Texture;

        public SpriteRect selectedSpriteRect
        {
            get
            {
                if (this.editingDisabled)
                {
                    return null;
                }
                return this.m_Selected;
            }
            set
            {
                this.m_Selected = value;
            }
        }

        public ITexture2D selectedTexture =>
            this.m_OriginalTexture;

        public bool selectedTextureChanged
        {
            get
            {
                ITexture2D textured = this.GetSelectedTexture2D();
                return ((textured != null) && (this.m_OriginalTexture != textured));
            }
        }

        public ISpriteRectCache spriteRects =>
            this.m_RectsCache;

        public bool textureIsDirty { get; set; }

        private bool validSprite =>
            ((this.m_TextureImporter != null) && (this.m_TextureImporter.spriteImportMode != SpriteImportMode.None));

        private Rect warningMessageRect =>
            new Rect(((base.position.width - 250f) - 8f) - 16f, 24f, 250f, 40f);

        public Rect windowDimension =>
            base.position;

        internal class PreviewTexture2D : UnityEngine.U2D.Interface.Texture2D
        {
            private int m_ActualHeight;
            private int m_ActualWidth;

            public PreviewTexture2D(UnityEngine.Texture2D t) : base(t)
            {
                this.m_ActualWidth = 0;
                this.m_ActualHeight = 0;
                if (t != null)
                {
                    (AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(t)) as UnityEditor.TextureImporter).GetWidthAndHeight(ref this.m_ActualWidth, ref this.m_ActualHeight);
                }
            }

            public override int height =>
                this.m_ActualHeight;

            public override int width =>
                this.m_ActualWidth;
        }

        private class SpriteEditorWindowStyles
        {
            public static readonly GUIContent applyButtonLabel = EditorGUIUtility.TextContent("Apply");
            public static readonly GUIContent applyRevertDialogContent = EditorGUIUtility.TextContent("Unapplied import settings for '{0}'");
            public static readonly GUIContent applyRevertDialogTitle = EditorGUIUtility.TextContent("Unapplied import settings");
            public static readonly GUIContent applyRevertModuleDialogContent = EditorGUIUtility.TextContent("You have unapplied changes from the current module");
            public static readonly GUIContent applyRevertModuleDialogTitle = EditorGUIUtility.TextContent("Unapplied module changes");
            public static readonly GUIContent editingDisableMessageLabel = EditorGUIUtility.TextContent("Editing is disabled during play mode");
            public static readonly GUIContent loadContentText = EditorGUIUtility.TextContent("Loading Sprites {0}/{1}");
            public static readonly GUIContent loadProgressTitle = EditorGUIUtility.TextContent("Loading");
            public static readonly GUIContent noButtonLabel = EditorGUIUtility.TextContent("No");
            public static readonly GUIContent noSelectionWarning = EditorGUIUtility.TextContent("No texture or sprite selected");
            public static readonly GUIContent pendingChangesDialogContent = EditorGUIUtility.TextContent("You have pending changes in the Sprite Editor Window.\nDo you want to apply these changes?");
            public static readonly GUIContent revertButtonLabel = EditorGUIUtility.TextContent("Revert");
            public static readonly GUIContent saveContentText = EditorGUIUtility.TextContent("Saving Sprites {0}/{1}");
            public static readonly GUIContent saveProgressTitle = EditorGUIUtility.TextContent("Saving");
            public static readonly GUIContent spriteEditorWindowTitle = EditorGUIUtility.TextContent("Sprite Editor");
            public static readonly GUIContent yesButtonLabel = EditorGUIUtility.TextContent("Yes");
        }
    }
}

