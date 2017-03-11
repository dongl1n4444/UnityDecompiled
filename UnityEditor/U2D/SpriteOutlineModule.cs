namespace UnityEditor.U2D
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Sprites;
    using UnityEditor.U2D.Interface;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.U2D.Interface;

    internal class SpriteOutlineModule : ISpriteEditorModule
    {
        [CompilerGenerated]
        private static Func<Vector3, Vector2> <>f__am$cache0;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IAssetDatabase <assetDatabase>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IEventSystem <eventSystem>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IGUIUtility <guiUtility>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <shapeEditorDirty>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IShapeEditorFactory <shapeEditorFactory>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ISpriteEditor <spriteEditorWindow>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IUndoSystem <undoSystem>k__BackingField;
        private readonly string k_DeleteCommandName = "Delete";
        private const float k_HandleSize = 5f;
        private readonly string k_SoftDeleteCommandName = "SoftDelete";
        private Matrix4x4 m_HandleMatrix;
        private Vector2 m_MousePosition;
        private ITexture2D m_OutlineTexture;
        private bool m_RequestRepaint;
        protected SpriteRect m_Selected;
        private Rect? m_SelectionRect;
        private ShapeEditor[] m_ShapeEditors;
        private ShapeEditorRectSelectionTool m_ShapeSelectionUI;
        private bool m_Snap;
        private Styles m_Styles;
        private bool m_WasRectSelecting = false;

        public SpriteOutlineModule(ISpriteEditor sem, IEventSystem es, IUndoSystem us, IAssetDatabase ad, IGUIUtility gu, IShapeEditorFactory sef, ITexture2D outlineTexture)
        {
            this.spriteEditorWindow = sem;
            this.undoSystem = us;
            this.eventSystem = es;
            this.assetDatabase = ad;
            this.guiUtility = gu;
            this.shapeEditorFactory = sef;
            this.m_OutlineTexture = outlineTexture;
            this.m_ShapeSelectionUI = new ShapeEditorRectSelectionTool(gu);
            this.m_ShapeSelectionUI.RectSelect += new Action<Rect, ShapeEditor.SelectionType>(this.RectSelect);
            this.m_ShapeSelectionUI.ClearSelection += new System.Action(this.ClearSelection);
        }

        public bool CanBeActivated() => 
            (UnityEditor.SpriteUtility.GetSpriteImportMode(this.assetDatabase, this.spriteEditorWindow.selectedTexture) != SpriteImportMode.None);

        private static Vector2 CapPointToRect(Vector2 so, Rect r)
        {
            so.x = Mathf.Min(r.xMax, so.x);
            so.x = Mathf.Max(r.xMin, so.x);
            so.y = Mathf.Min(r.yMax, so.y);
            so.y = Mathf.Max(r.yMin, so.y);
            return so;
        }

        private void CleanupShapeEditors()
        {
            if (this.m_ShapeEditors != null)
            {
                for (int i = 0; i < this.m_ShapeEditors.Length; i++)
                {
                    for (int j = 0; j < this.m_ShapeEditors.Length; j++)
                    {
                        if (i != j)
                        {
                            this.m_ShapeEditors[j].UnregisterFromShapeEditor(this.m_ShapeEditors[i]);
                        }
                    }
                    this.m_ShapeEditors[i].OnDisable();
                }
            }
            this.m_ShapeEditors = null;
        }

        private void ClearSelection()
        {
            this.m_RequestRepaint = true;
        }

        private Vector2 ConvertSpriteRectSpaceToTextureSpace(Vector2 value)
        {
            Vector2 vector = new Vector2((0.5f * this.m_Selected.rect.width) + this.m_Selected.rect.x, (0.5f * this.m_Selected.rect.height) + this.m_Selected.rect.y);
            value += vector;
            return value;
        }

        private Vector2 ConvertTextureSpaceToSpriteRectSpace(Vector2 value)
        {
            Vector2 vector = new Vector2((0.5f * this.m_Selected.rect.width) + this.m_Selected.rect.x, (0.5f * this.m_Selected.rect.height) + this.m_Selected.rect.y);
            value -= vector;
            return value;
        }

        public void CreateNewOutline(Rect rectOutline)
        {
            Rect r = this.m_Selected.rect;
            if (r.Contains(rectOutline.min) && r.Contains(rectOutline.max))
            {
                this.RecordUndo();
                SpriteOutline item = new SpriteOutline();
                Vector2 vector = new Vector2((0.5f * r.width) + r.x, (0.5f * r.height) + r.y);
                Rect rect2 = new Rect(rectOutline) {
                    min = this.SnapPoint((Vector3) rectOutline.min),
                    max = this.SnapPoint((Vector3) rectOutline.max)
                };
                item.Add(CapPointToRect(new Vector2(rect2.xMin, rect2.yMin), r) - vector);
                item.Add(CapPointToRect(new Vector2(rect2.xMin, rect2.yMax), r) - vector);
                item.Add(CapPointToRect(new Vector2(rect2.xMax, rect2.yMax), r) - vector);
                item.Add(CapPointToRect(new Vector2(rect2.xMax, rect2.yMin), r) - vector);
                this.selectedShapeOutline.Add(item);
                this.spriteEditorWindow.SetDataModified();
                this.shapeEditorDirty = true;
            }
        }

        public void DoTextureGUI()
        {
            IEvent current = this.eventSystem.current;
            this.m_RequestRepaint = false;
            this.m_HandleMatrix = Handles.matrix;
            this.m_MousePosition = Handles.inverseMatrix.MultiplyPoint((Vector3) this.eventSystem.current.mousePosition);
            if ((this.m_Selected == null) || ((!this.m_Selected.rect.Contains(this.m_MousePosition) && !this.IsMouseOverOutlinePoints()) && !current.shift))
            {
                this.spriteEditorWindow.HandleSpriteSelection();
            }
            this.HandleCreateNewOutline();
            this.m_WasRectSelecting = this.m_ShapeSelectionUI.isSelecting;
            this.UpdateShapeEditors();
            this.m_ShapeSelectionUI.OnGUI();
            this.DrawGizmos();
            if (this.m_RequestRepaint || (current.type == EventType.MouseMove))
            {
                this.spriteEditorWindow.RequestRepaint();
            }
        }

        private void DrawGizmos()
        {
            if ((this.eventSystem.current.type == EventType.Layout) || (this.eventSystem.current.type == EventType.Repaint))
            {
                SpriteRect selectedSpriteRect = this.spriteEditorWindow.selectedSpriteRect;
                if (selectedSpriteRect != null)
                {
                    SpriteEditorUtility.BeginLines(this.styles.spriteBorderColor);
                    SpriteEditorUtility.DrawBox(selectedSpriteRect.rect);
                    SpriteEditorUtility.EndLines();
                }
            }
        }

        public void DrawToolbarGUI(Rect drawArea)
        {
            Styles styles = this.styles;
            Rect position = new Rect(drawArea.x, drawArea.y, EditorStyles.toolbarButton.CalcSize(styles.snapButtonLabel).x, drawArea.height);
            this.m_Snap = GUI.Toggle(position, this.m_Snap, styles.snapButtonLabel, EditorStyles.toolbarButton);
            using (new EditorGUI.DisabledScope(this.editingDisabled || (this.m_Selected == null)))
            {
                float num = drawArea.width - position.width;
                drawArea.x = position.xMax;
                drawArea.width = EditorStyles.toolbarButton.CalcSize(styles.outlineTolerance).x;
                num -= drawArea.width;
                if (num < 0f)
                {
                    drawArea.width += num;
                }
                if (drawArea.width > 0f)
                {
                    GUI.Label(drawArea, styles.outlineTolerance, EditorStyles.miniLabel);
                }
                drawArea.x += drawArea.width;
                drawArea.width = 100f;
                num -= drawArea.width;
                if (num < 0f)
                {
                    drawArea.width += num;
                }
                if (drawArea.width > 0f)
                {
                    float num2 = (this.m_Selected == null) ? 0f : this.m_Selected.tessellationDetail;
                    EditorGUI.BeginChangeCheck();
                    float fieldWidth = EditorGUIUtility.fieldWidth;
                    float labelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.fieldWidth = 30f;
                    EditorGUIUtility.labelWidth = 1f;
                    num2 = EditorGUI.Slider(drawArea, Mathf.Clamp01(num2), 0f, 1f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.RecordUndo();
                        this.m_Selected.tessellationDetail = num2;
                    }
                    EditorGUIUtility.fieldWidth = fieldWidth;
                    EditorGUIUtility.labelWidth = labelWidth;
                }
                drawArea.x += drawArea.width;
                drawArea.width = EditorStyles.toolbarButton.CalcSize(styles.generateOutlineLabel).x;
                num -= drawArea.width;
                if (num < 0f)
                {
                    drawArea.width += num;
                }
                if ((drawArea.width > 0f) && GUI.Button(drawArea, styles.generateOutlineLabel, EditorStyles.toolbarButton))
                {
                    this.RecordUndo();
                    this.selectedShapeOutline.Clear();
                    this.SetupShapeEditorOutline(this.m_Selected);
                    this.selectedShapeOutline = this.m_Selected.outline;
                    this.spriteEditorWindow.SetDataModified();
                    this.shapeEditorDirty = true;
                }
            }
        }

        private void GenerateOutlineIfNotExist()
        {
            ISpriteRectCache spriteRects = this.spriteEditorWindow.spriteRects;
            if (spriteRects != null)
            {
                for (int i = 0; i < spriteRects.Count; i++)
                {
                    SpriteRect spriteRect = spriteRects.RectAt(i);
                    if ((spriteRect.outline == null) || (spriteRect.outline.Count == 0))
                    {
                        this.spriteEditorWindow.DisplayProgressBar(this.styles.generatingOutlineDialogTitle.text, string.Format(this.styles.generatingOutlineDialogContent.text, i + 1, spriteRects.Count), ((float) i) / ((float) spriteRects.Count));
                        this.SetupShapeEditorOutline(spriteRect);
                    }
                }
                this.spriteEditorWindow.ClearProgressBar();
            }
        }

        private static List<SpriteOutline> GenerateSpriteRectOutline(Rect rect, ITexture2D texture, float detail, byte alphaTolerance)
        {
            List<SpriteOutline> list = new List<SpriteOutline>();
            if (texture != null)
            {
                Vector2[][] vectorArray;
                int width = 0;
                int height = 0;
                (AssetImporter.GetAtPath(AssetDatabase.GetAssetPath((UnityEngine.Object) texture)) as UnityEditor.TextureImporter).GetWidthAndHeight(ref width, ref height);
                int num3 = texture.width;
                int num4 = texture.height;
                Vector2 vector = new Vector2(((float) num3) / ((float) width), ((float) num4) / ((float) height));
                Rect rect2 = rect;
                rect2.xMin *= vector.x;
                rect2.xMax *= vector.x;
                rect2.yMin *= vector.y;
                rect2.yMax *= vector.y;
                UnityEditor.Sprites.SpriteUtility.GenerateOutline((UnityEngine.Texture2D) texture, rect2, detail, alphaTolerance, true, out vectorArray);
                Rect r = new Rect {
                    size = rect.size,
                    center = Vector2.zero
                };
                for (int i = 0; i < vectorArray.Length; i++)
                {
                    SpriteOutline item = new SpriteOutline();
                    foreach (Vector2 vector2 in vectorArray[i])
                    {
                        item.Add(CapPointToRect(new Vector2(vector2.x / vector.x, vector2.y / vector.y), r));
                    }
                    list.Add(item);
                }
            }
            return list;
        }

        private float GetHandleSize() => 
            (5f / this.m_HandleMatrix.m00);

        public Vector3 GetPointPosition(int outlineIndex, int pointIndex)
        {
            if ((outlineIndex >= 0) && (outlineIndex < this.selectedShapeOutline.Count))
            {
                SpriteOutline outline = this.selectedShapeOutline[outlineIndex];
                if ((pointIndex >= 0) && (pointIndex < outline.Count))
                {
                    return (Vector3) this.ConvertSpriteRectSpaceToTextureSpace(outline[pointIndex]);
                }
            }
            return new Vector3(float.NaN, float.NaN, float.NaN);
        }

        public int GetPointsCount(int outlineIndex) => 
            this.selectedShapeOutline[outlineIndex].Count;

        private void HandleCreateNewOutline()
        {
            if ((this.m_WasRectSelecting && !this.m_ShapeSelectionUI.isSelecting) && (this.m_SelectionRect.HasValue && (this.m_Selected != null)))
            {
                bool flag = true;
                foreach (ShapeEditor editor in this.m_ShapeEditors)
                {
                    if (editor.selectedPoints.Count != 0)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    this.CreateNewOutline(this.m_SelectionRect.Value);
                }
            }
            this.m_SelectionRect = null;
        }

        public void InsertPointAt(int outlineIndex, int pointIndex, Vector3 position)
        {
            this.selectedShapeOutline[outlineIndex].Insert(pointIndex, this.ConvertTextureSpaceToSpriteRectSpace(CapPointToRect(position, this.m_Selected.rect)));
            this.spriteEditorWindow.SetDataModified();
        }

        private bool IsMouseOverOutlinePoints()
        {
            if (this.m_Selected != null)
            {
                Vector2 vector = new Vector2((0.5f * this.m_Selected.rect.width) + this.m_Selected.rect.x, (0.5f * this.m_Selected.rect.height) + this.m_Selected.rect.y);
                float handleSize = this.GetHandleSize();
                Rect rect5 = new Rect(0f, 0f, handleSize * 2f, handleSize * 2f);
                for (int i = 0; i < this.selectedShapeOutline.Count; i++)
                {
                    SpriteOutline outline = this.selectedShapeOutline[i];
                    for (int j = 0; j < outline.Count; j++)
                    {
                        rect5.center = outline[j] + vector;
                        if (rect5.Contains(this.m_MousePosition))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void OnModuleActivate()
        {
            this.GenerateOutlineIfNotExist();
            this.undoSystem.RegisterUndoCallback(new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            this.shapeEditorDirty = true;
            this.SetupShapeEditor();
            this.spriteEditorWindow.enableMouseMoveEvent = true;
        }

        public void OnModuleDeactivate()
        {
            this.undoSystem.UnregisterUndoCallback(new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            this.CleanupShapeEditors();
            this.m_Selected = null;
            this.spriteEditorWindow.enableMouseMoveEvent = false;
        }

        public void OnPostGUI()
        {
        }

        private void RecordUndo()
        {
            this.undoSystem.RegisterCompleteObjectUndo(this.spriteEditorWindow.spriteRects, "Outline changed");
        }

        private void RectSelect(Rect r, ShapeEditor.SelectionType selectionType)
        {
            Rect rect = EditorGUIExt.FromToRect(this.ScreenToLocal(r.min), this.ScreenToLocal(r.max));
            this.m_SelectionRect = new Rect?(rect);
        }

        public void RemovePointAt(int outlineIndex, int i)
        {
            this.selectedShapeOutline[outlineIndex].RemoveAt(i);
            this.spriteEditorWindow.SetDataModified();
        }

        private Vector3 ScreenToLocal(Vector2 point) => 
            Handles.inverseMatrix.MultiplyPoint((Vector3) point);

        public void SetPointPosition(int outlineIndex, int pointIndex, Vector3 position)
        {
            this.selectedShapeOutline[outlineIndex][pointIndex] = this.ConvertTextureSpaceToSpriteRectSpace(CapPointToRect(position, this.m_Selected.rect));
            this.spriteEditorWindow.SetDataModified();
        }

        public void SetupShapeEditor()
        {
            if (this.shapeEditorDirty || (this.m_Selected != this.spriteEditorWindow.selectedSpriteRect))
            {
                this.m_Selected = this.spriteEditorWindow.selectedSpriteRect;
                this.CleanupShapeEditors();
                if (this.m_Selected != null)
                {
                    this.SetupShapeEditorOutline(this.m_Selected);
                    this.selectedShapeOutline = this.m_Selected.outline;
                    this.m_ShapeEditors = new ShapeEditor[this.selectedShapeOutline.Count];
                    for (int i = 0; i < this.selectedShapeOutline.Count; i++)
                    {
                        <SetupShapeEditor>c__AnonStorey0 storey = new <SetupShapeEditor>c__AnonStorey0 {
                            $this = this,
                            outlineIndex = i
                        };
                        this.m_ShapeEditors[i] = this.shapeEditorFactory.CreateShapeEditor();
                        this.m_ShapeEditors[i].SetRectSelectionTool(this.m_ShapeSelectionUI);
                        this.m_ShapeEditors[i].LocalToWorldMatrix = new Func<Matrix4x4>(storey.<>m__0);
                        if (<>f__am$cache0 == null)
                        {
                            <>f__am$cache0 = point => Handles.matrix.MultiplyPoint(point);
                        }
                        this.m_ShapeEditors[i].LocalToScreen = <>f__am$cache0;
                        this.m_ShapeEditors[i].ScreenToLocal = new Func<Vector2, Vector3>(this.ScreenToLocal);
                        this.m_ShapeEditors[i].RecordUndo = new System.Action(this.RecordUndo);
                        this.m_ShapeEditors[i].GetHandleSize = new Func<float>(this.GetHandleSize);
                        this.m_ShapeEditors[i].lineTexture = (UnityEngine.Texture2D) this.m_OutlineTexture;
                        this.m_ShapeEditors[i].Snap = new Func<Vector3, Vector3>(this.SnapPoint);
                        this.m_ShapeEditors[i].GetPointPosition = new Func<int, Vector3>(storey.<>m__1);
                        this.m_ShapeEditors[i].SetPointPosition = new Action<int, Vector3>(storey.<>m__2);
                        this.m_ShapeEditors[i].InsertPointAt = new Action<int, Vector3>(storey.<>m__3);
                        this.m_ShapeEditors[i].RemovePointAt = new Action<int>(storey.<>m__4);
                        this.m_ShapeEditors[i].GetPointsCount = new Func<int>(storey.<>m__5);
                    }
                    for (int j = 0; j < this.selectedShapeOutline.Count; j++)
                    {
                        for (int k = 0; k < this.selectedShapeOutline.Count; k++)
                        {
                            if (j != k)
                            {
                                this.m_ShapeEditors[k].RegisterToShapeEditor(this.m_ShapeEditors[j]);
                            }
                        }
                    }
                }
                else
                {
                    this.m_ShapeEditors = new ShapeEditor[0];
                }
            }
            this.shapeEditorDirty = false;
        }

        protected virtual void SetupShapeEditorOutline(SpriteRect spriteRect)
        {
            if ((spriteRect.outline == null) || (spriteRect.outline.Count == 0))
            {
                spriteRect.outline = GenerateSpriteRectOutline(spriteRect.rect, this.spriteEditorWindow.selectedTexture, spriteRect.tessellationDetail, 0);
                if (spriteRect.outline.Count == 0)
                {
                    Vector2 vector = (Vector2) (spriteRect.rect.size * 0.5f);
                    List<SpriteOutline> list = new List<SpriteOutline>();
                    SpriteOutline item = new SpriteOutline();
                    List<Vector2> list2 = new List<Vector2> {
                        new Vector2(-vector.x, -vector.y),
                        new Vector2(-vector.x, vector.y),
                        new Vector2(vector.x, vector.y),
                        new Vector2(vector.x, -vector.y)
                    };
                    item.m_Path = list2;
                    list.Add(item);
                    spriteRect.outline = list;
                }
                this.spriteEditorWindow.SetDataModified();
            }
        }

        public Vector3 SnapPoint(Vector3 position)
        {
            if (this.m_Snap)
            {
                position.x = Mathf.RoundToInt(position.x);
                position.y = Mathf.RoundToInt(position.y);
            }
            return position;
        }

        private void UndoRedoPerformed()
        {
            this.shapeEditorDirty = true;
        }

        public void UpdateShapeEditors()
        {
            this.SetupShapeEditor();
            if (this.m_Selected != null)
            {
                IEvent current = this.eventSystem.current;
                bool flag = (current.type == EventType.ExecuteCommand) && ((current.commandName == this.k_SoftDeleteCommandName) || (current.commandName == this.k_DeleteCommandName));
                for (int i = 0; i < this.m_ShapeEditors.Length; i++)
                {
                    if (this.m_ShapeEditors[i].GetPointsCount() != 0)
                    {
                        this.m_ShapeEditors[i].inEditMode = true;
                        this.m_ShapeEditors[i].OnGUI();
                        if (this.shapeEditorDirty)
                        {
                            break;
                        }
                    }
                }
                if (flag)
                {
                    for (int j = this.selectedShapeOutline.Count - 1; j >= 0; j--)
                    {
                        if (this.selectedShapeOutline[j].Count < 3)
                        {
                            this.selectedShapeOutline.RemoveAt(j);
                            this.shapeEditorDirty = true;
                        }
                    }
                }
            }
        }

        private IAssetDatabase assetDatabase { get; set; }

        private bool editingDisabled =>
            this.spriteEditorWindow.editingDisabled;

        private IEventSystem eventSystem { get; set; }

        private IGUIUtility guiUtility { get; set; }

        public virtual string moduleName =>
            "Edit Outline";

        private List<SpriteOutline> selectedShapeOutline
        {
            get => 
                this.m_Selected.outline;
            set
            {
                this.m_Selected.outline = value;
            }
        }

        private bool shapeEditorDirty { get; set; }

        private IShapeEditorFactory shapeEditorFactory { get; set; }

        private ISpriteEditor spriteEditorWindow { get; set; }

        private Styles styles
        {
            get
            {
                if (this.m_Styles == null)
                {
                    this.m_Styles = new Styles();
                }
                return this.m_Styles;
            }
        }

        private IUndoSystem undoSystem { get; set; }

        [CompilerGenerated]
        private sealed class <SetupShapeEditor>c__AnonStorey0
        {
            internal SpriteOutlineModule $this;
            internal int outlineIndex;

            internal Matrix4x4 <>m__0() => 
                this.$this.m_HandleMatrix;

            internal Vector3 <>m__1(int index) => 
                this.$this.GetPointPosition(this.outlineIndex, index);

            internal void <>m__2(int index, Vector3 position)
            {
                this.$this.SetPointPosition(this.outlineIndex, index, position);
            }

            internal void <>m__3(int index, Vector3 position)
            {
                this.$this.InsertPointAt(this.outlineIndex, index, position);
            }

            internal void <>m__4(int index)
            {
                this.$this.RemovePointAt(this.outlineIndex, index);
            }

            internal int <>m__5() => 
                this.$this.GetPointsCount(this.outlineIndex);
        }

        private class Styles
        {
            public GUIContent generateOutlineLabel = EditorGUIUtility.TextContent("Update|Update new outline based on mesh detail value.");
            public GUIContent generatingOutlineDialogContent = EditorGUIUtility.TextContent("Generating outline {0}/{1}");
            public GUIContent generatingOutlineDialogTitle = EditorGUIUtility.TextContent("Outline");
            public GUIContent outlineTolerance = EditorGUIUtility.TextContent("Outline Tolerance|Sets how tight the outline should be from the sprite.");
            public GUIContent snapButtonLabel = EditorGUIUtility.TextContent("Snap|Snap points to nearest pixel");
            public Color spriteBorderColor = new Color(0.25f, 0.5f, 1f, 0.75f);
        }
    }
}

