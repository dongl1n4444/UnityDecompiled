namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.U2D.Interface;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.U2D.Interface;

    internal abstract class SpriteFrameModuleBase : ISpriteEditorModule
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private IAssetDatabase <assetDatabase>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private IEventSystem <eventSystem>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <moduleName>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ISpriteEditor <spriteEditor>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private SpriteImportMode <spriteImportMode>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private IUndoSystem <undoSystem>k__BackingField;
        private const float kInspectorHeight = 160f;
        private const float kInspectorWidth = 330f;
        private const float kInspectorWindowMargin = 8f;
        private const float kScrollbarMargin = 16f;
        private GizmoMode m_GizmoMode;
        protected ISpriteRectCache m_RectsCache;
        private float m_Zoom = 1f;
        private static Styles s_Styles;

        protected SpriteFrameModuleBase(string name, ISpriteEditor sw, IEventSystem es, IUndoSystem us, IAssetDatabase ad)
        {
            this.spriteEditor = sw;
            this.eventSystem = es;
            this.undoSystem = us;
            this.assetDatabase = ad;
            this.moduleName = name;
        }

        private static Vector2 ApplySpriteAlignmentToPivot(Vector2 pivot, Rect rect, SpriteAlignment alignment)
        {
            if (alignment != SpriteAlignment.Custom)
            {
                Vector2 texturePos = GetSnapPointsArray(rect)[(int) alignment];
                return ConvertFromTextureToNormalizedSpace(texturePos, rect);
            }
            return pivot;
        }

        public abstract bool CanBeActivated();
        protected static Vector4 ClampSpriteBorderToRect(Vector4 border, Rect rect)
        {
            Rect rect2 = FlipNegativeRect(rect);
            float width = rect2.width;
            float height = rect2.height;
            Vector4 vector = new Vector4 {
                x = Mathf.RoundToInt(Mathf.Clamp(border.x, 0f, Mathf.Min(Mathf.Abs((float) (width - border.z)), width)))
            };
            vector.z = Mathf.RoundToInt(Mathf.Clamp(border.z, 0f, Mathf.Min(Mathf.Abs((float) (width - vector.x)), width)));
            vector.y = Mathf.RoundToInt(Mathf.Clamp(border.y, 0f, Mathf.Min(Mathf.Abs((float) (height - border.w)), height)));
            vector.w = Mathf.RoundToInt(Mathf.Clamp(border.w, 0f, Mathf.Min(Mathf.Abs((float) (height - vector.y)), height)));
            return vector;
        }

        protected static Rect ClampSpriteRect(Rect rect, float maxX, float maxY)
        {
            Rect rect2 = new Rect {
                xMin = Mathf.Clamp(rect.xMin, 0f, maxX - 1f),
                yMin = Mathf.Clamp(rect.yMin, 0f, maxY - 1f),
                xMax = Mathf.Clamp(rect.xMax, 1f, maxX),
                yMax = Mathf.Clamp(rect.yMax, 1f, maxY)
            };
            if (Mathf.RoundToInt(rect2.width) == 0)
            {
                rect2.width = 1f;
            }
            if (Mathf.RoundToInt(rect2.height) == 0)
            {
                rect2.height = 1f;
            }
            return SpriteEditorUtility.RoundedRect(rect2);
        }

        private static Vector2 ConvertFromTextureToNormalizedSpace(Vector2 texturePos, Rect rect) => 
            new Vector2((texturePos.x - rect.xMin) / rect.width, (texturePos.y - rect.yMin) / rect.height);

        public int CurrentSelectedSpriteIndex()
        {
            for (int i = 0; i < this.m_RectsCache.Count; i++)
            {
                if (this.m_RectsCache.RectAt(i) == this.selected)
                {
                    return i;
                }
            }
            return -1;
        }

        private void DoBorderFields()
        {
            EditorGUI.BeginChangeCheck();
            Vector4 selectedSpriteBorder = this.selectedSpriteBorder;
            int x = Mathf.RoundToInt(selectedSpriteBorder.x);
            int w = Mathf.RoundToInt(selectedSpriteBorder.y);
            int z = Mathf.RoundToInt(selectedSpriteBorder.z);
            int y = Mathf.RoundToInt(selectedSpriteBorder.w);
            SpriteEditorUtility.FourIntFields(new Vector2(322f, 32f), styles.borderLabel, styles.lLabel, styles.tLabel, styles.rLabel, styles.bLabel, ref x, ref y, ref z, ref w);
            Vector4 vector2 = new Vector4((float) x, (float) w, (float) z, (float) y);
            if (EditorGUI.EndChangeCheck())
            {
                this.selectedSpriteBorder = vector2;
            }
        }

        private void DoNameField()
        {
            EditorGUI.BeginChangeCheck();
            string selectedSpriteName = this.selectedSpriteName;
            GUI.SetNextControlName("SpriteName");
            string str2 = EditorGUILayout.TextField(styles.nameLabel, selectedSpriteName, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.selectedSpriteName = str2;
            }
        }

        private void DoPivotFields()
        {
            EditorGUI.BeginChangeCheck();
            SpriteAlignment selectedSpriteAlignment = this.selectedSpriteAlignment;
            selectedSpriteAlignment = (SpriteAlignment) EditorGUILayout.Popup(styles.pivotLabel, (int) selectedSpriteAlignment, styles.spriteAlignmentOptions, new GUILayoutOption[0]);
            Vector2 selectedSpritePivot = this.selectedSpritePivot;
            Vector2 pivot = selectedSpritePivot;
            using (new EditorGUI.DisabledScope(selectedSpriteAlignment != SpriteAlignment.Custom))
            {
                Rect position = GUILayoutUtility.GetRect(322f, EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector2, styles.customPivotLabel));
                GUI.SetNextControlName("PivotField");
                pivot = EditorGUI.Vector2Field(position, styles.customPivotLabel, selectedSpritePivot);
            }
            if (EditorGUI.EndChangeCheck())
            {
                this.SetSpritePivotAndAlignment(pivot, selectedSpriteAlignment);
            }
        }

        private void DoPositionField()
        {
            EditorGUI.BeginChangeCheck();
            Rect selectedSpriteRect = this.selectedSpriteRect;
            int x = Mathf.RoundToInt(selectedSpriteRect.x);
            int y = Mathf.RoundToInt(selectedSpriteRect.y);
            int z = Mathf.RoundToInt(selectedSpriteRect.width);
            int w = Mathf.RoundToInt(selectedSpriteRect.height);
            SpriteEditorUtility.FourIntFields(new Vector2(322f, 32f), styles.positionLabel, styles.xLabel, styles.yLabel, styles.wLabel, styles.hLabel, ref x, ref y, ref z, ref w);
            if (EditorGUI.EndChangeCheck())
            {
                this.selectedSpriteRect = new Rect((float) x, (float) y, (float) z, (float) w);
            }
        }

        private void DoSelectedFrameInspector()
        {
            if (this.hasSelected)
            {
                EditorGUIUtility.wideMode = true;
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 135f;
                GUILayout.BeginArea(this.inspectorRect);
                GUILayout.BeginVertical(styles.spriteLabel, GUI.skin.window, new GUILayoutOption[0]);
                using (new EditorGUI.DisabledScope(!this.containsMultipleSprites))
                {
                    this.DoNameField();
                    this.DoPositionField();
                }
                this.DoBorderFields();
                this.DoPivotFields();
                GUILayout.EndVertical();
                GUILayout.EndArea();
                EditorGUIUtility.labelWidth = labelWidth;
            }
        }

        public virtual void DoTextureGUI()
        {
            this.m_Zoom = Handles.matrix.GetColumn(0).magnitude;
        }

        protected void DrawSpriteRectGizmos()
        {
            if (this.eventSystem.current.type == EventType.Repaint)
            {
                SpriteEditorUtility.BeginLines(new Color(0f, 1f, 0f, 0.7f));
                int num = this.CurrentSelectedSpriteIndex();
                for (int i = 0; i < this.spriteCount; i++)
                {
                    Vector4 spriteBorderAt = this.GetSpriteBorderAt(i);
                    if (((num == i) || (this.m_GizmoMode == GizmoMode.BorderEditing)) || !Mathf.Approximately(spriteBorderAt.sqrMagnitude, 0f))
                    {
                        Rect spriteRectAt = this.GetSpriteRectAt(i);
                        SpriteEditorUtility.DrawLine(new Vector3(spriteRectAt.xMin + spriteBorderAt.x, spriteRectAt.yMin), new Vector3(spriteRectAt.xMin + spriteBorderAt.x, spriteRectAt.yMax));
                        SpriteEditorUtility.DrawLine(new Vector3(spriteRectAt.xMax - spriteBorderAt.z, spriteRectAt.yMin), new Vector3(spriteRectAt.xMax - spriteBorderAt.z, spriteRectAt.yMax));
                        SpriteEditorUtility.DrawLine(new Vector3(spriteRectAt.xMin, spriteRectAt.yMin + spriteBorderAt.y), new Vector3(spriteRectAt.xMax, spriteRectAt.yMin + spriteBorderAt.y));
                        SpriteEditorUtility.DrawLine(new Vector3(spriteRectAt.xMin, spriteRectAt.yMax - spriteBorderAt.w), new Vector3(spriteRectAt.xMax, spriteRectAt.yMax - spriteBorderAt.w));
                    }
                }
                SpriteEditorUtility.EndLines();
                if (this.ShouldShowRectScaling())
                {
                    Rect selectedSpriteRect = this.selectedSpriteRect;
                    SpriteEditorUtility.BeginLines(new Color(0f, 0.1f, 0.3f, 0.25f));
                    SpriteEditorUtility.DrawBox(new Rect(selectedSpriteRect.xMin + (1f / this.m_Zoom), selectedSpriteRect.yMin + (1f / this.m_Zoom), selectedSpriteRect.width, selectedSpriteRect.height));
                    SpriteEditorUtility.EndLines();
                    SpriteEditorUtility.BeginLines(new Color(0.25f, 0.5f, 1f, 0.75f));
                    SpriteEditorUtility.DrawBox(selectedSpriteRect);
                    SpriteEditorUtility.EndLines();
                }
            }
        }

        public abstract void DrawToolbarGUI(Rect drawArea);
        protected static Rect FlipNegativeRect(Rect rect) => 
            new Rect { 
                xMin = Mathf.Min(rect.xMin, rect.xMax),
                yMin = Mathf.Min(rect.yMin, rect.yMax),
                xMax = Mathf.Max(rect.xMin, rect.xMax),
                yMax = Mathf.Max(rect.yMin, rect.yMax)
            };

        private static Vector2[] GetSnapPointsArray(Rect rect)
        {
            Vector2[] vectorArray = new Vector2[9];
            vectorArray[1] = new Vector2(rect.xMin, rect.yMax);
            vectorArray[2] = new Vector2(rect.center.x, rect.yMax);
            vectorArray[3] = new Vector2(rect.xMax, rect.yMax);
            vectorArray[4] = new Vector2(rect.xMin, rect.center.y);
            vectorArray[0] = new Vector2(rect.center.x, rect.center.y);
            vectorArray[5] = new Vector2(rect.xMax, rect.center.y);
            vectorArray[6] = new Vector2(rect.xMin, rect.yMin);
            vectorArray[7] = new Vector2(rect.center.x, rect.yMin);
            vectorArray[8] = new Vector2(rect.xMax, rect.yMin);
            return vectorArray;
        }

        public Vector4 GetSpriteBorderAt(int i) => 
            this.m_RectsCache.RectAt(i).border;

        public List<SpriteOutline> GetSpriteOutlineAt(int i) => 
            this.m_RectsCache.RectAt(i).outline;

        public Rect GetSpriteRectAt(int i) => 
            this.m_RectsCache.RectAt(i).rect;

        protected void HandleBorderCornerScalingHandles()
        {
            if (this.hasSelected)
            {
                GUIStyle dragBorderdot = styles.dragBorderdot;
                GUIStyle dragBorderDotActive = styles.dragBorderDotActive;
                Color color = new Color(0f, 1f, 0f);
                Rect selectedSpriteRect = this.selectedSpriteRect;
                Vector4 selectedSpriteBorder = this.selectedSpriteBorder;
                float x = selectedSpriteRect.xMin + selectedSpriteBorder.x;
                float num2 = selectedSpriteRect.xMax - selectedSpriteBorder.z;
                float y = selectedSpriteRect.yMax - selectedSpriteBorder.w;
                float num4 = selectedSpriteRect.yMin + selectedSpriteBorder.y;
                EditorGUI.BeginChangeCheck();
                this.HandleBorderPointSlider(ref x, ref y, MouseCursor.ResizeUpLeft, (selectedSpriteBorder.x < 1f) && (selectedSpriteBorder.w < 1f), dragBorderdot, dragBorderDotActive, color);
                this.HandleBorderPointSlider(ref num2, ref y, MouseCursor.ResizeUpRight, (selectedSpriteBorder.z < 1f) && (selectedSpriteBorder.w < 1f), dragBorderdot, dragBorderDotActive, color);
                this.HandleBorderPointSlider(ref x, ref num4, MouseCursor.ResizeUpRight, (selectedSpriteBorder.x < 1f) && (selectedSpriteBorder.y < 1f), dragBorderdot, dragBorderDotActive, color);
                this.HandleBorderPointSlider(ref num2, ref num4, MouseCursor.ResizeUpLeft, (selectedSpriteBorder.z < 1f) && (selectedSpriteBorder.y < 1f), dragBorderdot, dragBorderDotActive, color);
                if (EditorGUI.EndChangeCheck())
                {
                    selectedSpriteBorder.x = x - selectedSpriteRect.xMin;
                    selectedSpriteBorder.z = selectedSpriteRect.xMax - num2;
                    selectedSpriteBorder.w = selectedSpriteRect.yMax - y;
                    selectedSpriteBorder.y = num4 - selectedSpriteRect.yMin;
                    this.selectedSpriteBorder = selectedSpriteBorder;
                }
            }
        }

        protected void HandleBorderPointSlider(ref float x, ref float y, MouseCursor mouseCursor, bool isHidden, GUIStyle dragDot, GUIStyle dragDotActive, Color color)
        {
            Color color2 = GUI.color;
            if (isHidden)
            {
                GUI.color = new Color(0f, 0f, 0f, 0f);
            }
            else
            {
                GUI.color = color;
            }
            Vector2 vector = SpriteEditorHandles.PointSlider(new Vector2(x, y), mouseCursor, dragDot, dragDotActive);
            x = vector.x;
            y = vector.y;
            GUI.color = color2;
        }

        protected float HandleBorderScaleSlider(float x, float y, float width, float height, bool isHorizontal)
        {
            float num2;
            float fixedWidth = styles.dragBorderdot.fixedWidth;
            Vector2 pos = Handles.matrix.MultiplyPoint((Vector3) new Vector2(x, y));
            EditorGUI.BeginChangeCheck();
            if (isHorizontal)
            {
                Rect cursorRect = new Rect(pos.x - (fixedWidth * 0.5f), pos.y, fixedWidth, height);
                num2 = SpriteEditorHandles.ScaleSlider(pos, MouseCursor.ResizeHorizontal, cursorRect).x;
            }
            else
            {
                Rect rect2 = new Rect(pos.x, pos.y - (fixedWidth * 0.5f), width, fixedWidth);
                num2 = SpriteEditorHandles.ScaleSlider(pos, MouseCursor.ResizeVertical, rect2).y;
            }
            if (EditorGUI.EndChangeCheck())
            {
                return num2;
            }
            return (!isHorizontal ? y : x);
        }

        protected void HandleBorderSidePointScalingSliders()
        {
            if (this.hasSelected)
            {
                GUIStyle dragBorderdot = styles.dragBorderdot;
                GUIStyle dragBorderDotActive = styles.dragBorderDotActive;
                Color color = new Color(0f, 1f, 0f);
                Rect selectedSpriteRect = this.selectedSpriteRect;
                Vector4 selectedSpriteBorder = this.selectedSpriteBorder;
                float x = selectedSpriteRect.xMin + selectedSpriteBorder.x;
                float num2 = selectedSpriteRect.xMax - selectedSpriteBorder.z;
                float y = selectedSpriteRect.yMax - selectedSpriteBorder.w;
                float num4 = selectedSpriteRect.yMin + selectedSpriteBorder.y;
                EditorGUI.BeginChangeCheck();
                float num5 = num4 - ((num4 - y) / 2f);
                float num6 = x - ((x - num2) / 2f);
                float num7 = num5;
                this.HandleBorderPointSlider(ref x, ref num7, MouseCursor.ResizeHorizontal, false, dragBorderdot, dragBorderDotActive, color);
                num7 = num5;
                this.HandleBorderPointSlider(ref num2, ref num7, MouseCursor.ResizeHorizontal, false, dragBorderdot, dragBorderDotActive, color);
                num7 = num6;
                this.HandleBorderPointSlider(ref num7, ref y, MouseCursor.ResizeVertical, false, dragBorderdot, dragBorderDotActive, color);
                num7 = num6;
                this.HandleBorderPointSlider(ref num7, ref num4, MouseCursor.ResizeVertical, false, dragBorderdot, dragBorderDotActive, color);
                if (EditorGUI.EndChangeCheck())
                {
                    selectedSpriteBorder.x = x - selectedSpriteRect.xMin;
                    selectedSpriteBorder.z = selectedSpriteRect.xMax - num2;
                    selectedSpriteBorder.w = selectedSpriteRect.yMax - y;
                    selectedSpriteBorder.y = num4 - selectedSpriteRect.yMin;
                    this.selectedSpriteBorder = selectedSpriteBorder;
                }
            }
        }

        protected void HandleBorderSideScalingHandles()
        {
            if (this.hasSelected)
            {
                Rect rect = new Rect(this.selectedSpriteRect);
                Vector4 selectedSpriteBorder = this.selectedSpriteBorder;
                float x = rect.xMin + selectedSpriteBorder.x;
                float num2 = rect.xMax - selectedSpriteBorder.z;
                float y = rect.yMax - selectedSpriteBorder.w;
                float num4 = rect.yMin + selectedSpriteBorder.y;
                Vector2 vector2 = Handles.matrix.MultiplyPoint(new Vector3(rect.xMin, rect.yMin));
                Vector2 vector3 = Handles.matrix.MultiplyPoint(new Vector3(rect.xMax, rect.yMax));
                float width = Mathf.Abs((float) (vector3.x - vector2.x));
                float height = Mathf.Abs((float) (vector3.y - vector2.y));
                EditorGUI.BeginChangeCheck();
                x = this.HandleBorderScaleSlider(x, rect.yMax, width, height, true);
                num2 = this.HandleBorderScaleSlider(num2, rect.yMax, width, height, true);
                y = this.HandleBorderScaleSlider(rect.xMin, y, width, height, false);
                num4 = this.HandleBorderScaleSlider(rect.xMin, num4, width, height, false);
                if (EditorGUI.EndChangeCheck())
                {
                    selectedSpriteBorder.x = x - rect.xMin;
                    selectedSpriteBorder.z = rect.xMax - num2;
                    selectedSpriteBorder.w = rect.yMax - y;
                    selectedSpriteBorder.y = num4 - rect.yMin;
                    this.selectedSpriteBorder = selectedSpriteBorder;
                }
            }
        }

        protected void HandleGizmoMode()
        {
            GizmoMode gizmoMode = this.m_GizmoMode;
            IEvent current = this.eventSystem.current;
            if (current.control)
            {
                this.m_GizmoMode = GizmoMode.BorderEditing;
            }
            else
            {
                this.m_GizmoMode = GizmoMode.RectEditing;
            }
            if (((gizmoMode != this.m_GizmoMode) && ((current.type == EventType.KeyDown) || (current.type == EventType.KeyUp))) && (((current.keyCode == KeyCode.LeftControl) || (current.keyCode == KeyCode.RightControl)) || ((current.keyCode == KeyCode.LeftAlt) || (current.keyCode == KeyCode.RightAlt))))
            {
                this.Repaint();
            }
        }

        protected void HandlePivotHandle()
        {
            if (this.hasSelected)
            {
                EditorGUI.BeginChangeCheck();
                SpriteAlignment selectedSpriteAlignment = this.selectedSpriteAlignment;
                Vector2 selectedSpritePivot = this.selectedSpritePivot;
                Rect selectedSpriteRect = this.selectedSpriteRect;
                selectedSpritePivot = ApplySpriteAlignmentToPivot(selectedSpritePivot, selectedSpriteRect, selectedSpriteAlignment);
                Vector2 pivot = SpriteEditorHandles.PivotSlider(selectedSpriteRect, selectedSpritePivot, styles.pivotdot, styles.pivotdotactive);
                if (EditorGUI.EndChangeCheck())
                {
                    if (this.eventSystem.current.control)
                    {
                        this.SnapPivot(pivot, out selectedSpritePivot, out selectedSpriteAlignment);
                    }
                    else
                    {
                        selectedSpritePivot = pivot;
                        selectedSpriteAlignment = SpriteAlignment.Custom;
                    }
                    this.SetSpritePivotAndAlignment(selectedSpritePivot, selectedSpriteAlignment);
                }
            }
        }

        protected bool MouseOnTopOfInspector()
        {
            if (!this.hasSelected)
            {
                return false;
            }
            Vector2 point = GUIClip.Unclip(this.eventSystem.current.mousePosition) - (GUIClip.topmostRect.position - GUIClip.GetTopRect().position);
            return this.inspectorRect.Contains(point);
        }

        public virtual void OnModuleActivate()
        {
            this.spriteImportMode = SpriteUtility.GetSpriteImportMode(this.assetDatabase, this.spriteEditor.selectedTexture);
        }

        public abstract void OnModuleDeactivate();
        public virtual void OnPostGUI()
        {
            this.DoSelectedFrameInspector();
        }

        protected void Repaint()
        {
            this.spriteEditor.RequestRepaint();
        }

        public void SetSpritePivotAndAlignment(Vector2 pivot, SpriteAlignment alignment)
        {
            this.undoSystem.RegisterCompleteObjectUndo(this.m_RectsCache, "Change Sprite Pivot");
            this.spriteEditor.SetDataModified();
            this.selected.alignment = alignment;
            this.selected.pivot = SpriteEditorUtility.GetPivotValue(alignment, pivot);
        }

        private bool ShouldShowRectScaling() => 
            (this.hasSelected && (this.m_GizmoMode == GizmoMode.RectEditing));

        protected void SnapPivot(Vector2 pivot, out Vector2 outPivot, out SpriteAlignment outAlignment)
        {
            Rect selectedSpriteRect = this.selectedSpriteRect;
            Vector2 vector = new Vector2(selectedSpriteRect.xMin + (selectedSpriteRect.width * pivot.x), selectedSpriteRect.yMin + (selectedSpriteRect.height * pivot.y));
            Vector2[] snapPointsArray = GetSnapPointsArray(selectedSpriteRect);
            SpriteAlignment custom = SpriteAlignment.Custom;
            float maxValue = float.MaxValue;
            for (int i = 0; i < snapPointsArray.Length; i++)
            {
                Vector2 vector2 = vector - snapPointsArray[i];
                float num3 = vector2.magnitude * this.m_Zoom;
                if (num3 < maxValue)
                {
                    custom = (SpriteAlignment) i;
                    maxValue = num3;
                }
            }
            outAlignment = custom;
            outPivot = ConvertFromTextureToNormalizedSpace(snapPointsArray[(int) custom], selectedSpriteRect);
        }

        protected IAssetDatabase assetDatabase { get; private set; }

        public bool containsMultipleSprites =>
            (this.spriteImportMode == SpriteImportMode.Multiple);

        protected IEventSystem eventSystem { get; private set; }

        public bool hasSelected =>
            (this.spriteEditor.selectedSpriteRect != null);

        private Rect inspectorRect
        {
            get
            {
                Rect windowDimension = this.spriteEditor.windowDimension;
                return new Rect(((windowDimension.width - 330f) - 8f) - 16f, ((windowDimension.height - 160f) - 8f) - 16f, 330f, 160f);
            }
        }

        public string moduleName { get; private set; }

        protected ITexture2D previewTexture =>
            this.spriteEditor.previewTexture;

        protected SpriteRect selected
        {
            get => 
                this.spriteEditor.selectedSpriteRect;
            set
            {
                this.spriteEditor.selectedSpriteRect = value;
            }
        }

        public SpriteAlignment selectedSpriteAlignment =>
            this.selected.alignment;

        public Vector4 selectedSpriteBorder
        {
            get => 
                ClampSpriteBorderToRect(this.selected.border, this.selected.rect);
            set
            {
                this.undoSystem.RegisterCompleteObjectUndo(this.m_RectsCache, "Change Sprite Border");
                this.spriteEditor.SetDataModified();
                this.selected.border = ClampSpriteBorderToRect(value, this.selected.rect);
            }
        }

        public string selectedSpriteName
        {
            get => 
                this.selected.name;
            set
            {
                this.undoSystem.RegisterCompleteObjectUndo(this.m_RectsCache, "Change Sprite Name");
                this.spriteEditor.SetDataModified();
                string name = this.selected.name;
                string originalName = InternalEditorUtility.RemoveInvalidCharsFromFileName(value, true);
                if (string.IsNullOrEmpty(this.selected.originalName) && (originalName != name))
                {
                    this.selected.originalName = name;
                }
                if (string.IsNullOrEmpty(originalName))
                {
                    originalName = name;
                }
                for (int i = 0; i < this.m_RectsCache.Count; i++)
                {
                    if (this.m_RectsCache.RectAt(i).name == originalName)
                    {
                        originalName = this.selected.originalName;
                        break;
                    }
                }
                this.selected.name = originalName;
            }
        }

        public Vector2 selectedSpritePivot =>
            this.selected.pivot;

        public Rect selectedSpriteRect
        {
            get => 
                this.selected.rect;
            set
            {
                this.undoSystem.RegisterCompleteObjectUndo(this.m_RectsCache, "Change Sprite rect");
                this.spriteEditor.SetDataModified();
                this.selected.rect = ClampSpriteRect(value, (float) this.previewTexture.width, (float) this.previewTexture.height);
            }
        }

        protected string spriteAssetPath =>
            this.assetDatabase.GetAssetPath((UnityEngine.Object) this.spriteEditor.selectedTexture);

        public int spriteCount =>
            this.m_RectsCache.Count;

        protected ISpriteEditor spriteEditor { get; private set; }

        protected SpriteImportMode spriteImportMode { get; private set; }

        protected static Styles styles
        {
            get
            {
                if (s_Styles == null)
                {
                    s_Styles = new Styles();
                }
                return s_Styles;
            }
        }

        protected IUndoSystem undoSystem { get; private set; }

        protected enum GizmoMode
        {
            BorderEditing,
            RectEditing
        }

        protected class Styles
        {
            public readonly GUIContent bLabel = EditorGUIUtility.TextContent("B");
            public readonly GUIContent borderLabel = EditorGUIUtility.TextContent("Border");
            public readonly GUIStyle createRect = "U2D.createRect";
            public readonly GUIContent customPivotLabel = EditorGUIUtility.TextContent("Custom Pivot");
            public readonly GUIStyle dragBorderdot = new GUIStyle();
            public readonly GUIStyle dragBorderDotActive = new GUIStyle();
            public readonly GUIStyle dragdot = "U2D.dragDot";
            public readonly GUIStyle dragdotactive = "U2D.dragDotActive";
            public readonly GUIContent hLabel = EditorGUIUtility.TextContent("H");
            public readonly GUIContent lLabel = EditorGUIUtility.TextContent("L");
            public readonly GUIContent nameLabel = EditorGUIUtility.TextContent("Name");
            public readonly GUIStyle pivotdot = "U2D.pivotDot";
            public readonly GUIStyle pivotdotactive = "U2D.pivotDotActive";
            public readonly GUIContent pivotLabel = EditorGUIUtility.TextContent("Pivot");
            public readonly GUIContent positionLabel = EditorGUIUtility.TextContent("Position");
            public readonly GUIContent rLabel = EditorGUIUtility.TextContent("R");
            public readonly GUIContent[] spriteAlignmentOptions = new GUIContent[] { EditorGUIUtility.TextContent("Center"), EditorGUIUtility.TextContent("Top Left"), EditorGUIUtility.TextContent("Top"), EditorGUIUtility.TextContent("Top Right"), EditorGUIUtility.TextContent("Left"), EditorGUIUtility.TextContent("Right"), EditorGUIUtility.TextContent("Bottom Left"), EditorGUIUtility.TextContent("Bottom"), EditorGUIUtility.TextContent("Bottom Right"), EditorGUIUtility.TextContent("Custom") };
            public readonly GUIContent spriteLabel = EditorGUIUtility.TextContent("Sprite");
            public readonly GUIContent tLabel = EditorGUIUtility.TextContent("T");
            public readonly GUIStyle toolbar = new GUIStyle(EditorStyles.inspectorBig);
            public readonly GUIContent wLabel = EditorGUIUtility.TextContent("W");
            public readonly GUIContent xLabel = EditorGUIUtility.TextContent("X");
            public readonly GUIContent yLabel = EditorGUIUtility.TextContent("Y");

            public Styles()
            {
                this.toolbar.margin.top = 0;
                this.toolbar.margin.bottom = 0;
                this.createRect.border = new RectOffset(3, 3, 3, 3);
                this.dragBorderdot.fixedHeight = 5f;
                this.dragBorderdot.fixedWidth = 5f;
                this.dragBorderdot.normal.background = EditorGUIUtility.whiteTexture;
                this.dragBorderDotActive.fixedHeight = this.dragBorderdot.fixedHeight;
                this.dragBorderDotActive.fixedWidth = this.dragBorderdot.fixedWidth;
                this.dragBorderDotActive.normal.background = EditorGUIUtility.whiteTexture;
            }
        }
    }
}

