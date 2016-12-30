namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEditor.U2D.Interface;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.U2D.Interface;

    internal class SpriteFrameModule : SpriteFrameModuleBase
    {
        [CompilerGenerated]
        private static Comparison<Rect> <>f__am$cache0;
        internal static PrefKey k_SpriteEditorTrim = new PrefKey("Sprite Editor/Trim", "#t");
        private const int kDefaultColliderAlphaCutoff = 0xfe;
        private const float kDefaultColliderDetail = 0.25f;
        private bool[] m_AlphaPixelCache;

        public SpriteFrameModule(ISpriteEditor sw, IEventSystem es, IUndoSystem us, IAssetDatabase ad) : base("Sprite Editor", sw, es, us, ad)
        {
        }

        private void AddSprite(Rect frame, int alignment, Vector2 pivot, AutoSlicingMethod slicingMethod, ref int index)
        {
            if (slicingMethod != AutoSlicingMethod.DeleteAll)
            {
                SpriteRect existingOverlappingSprite = this.GetExistingOverlappingSprite(frame);
                if (existingOverlappingSprite != null)
                {
                    if (slicingMethod == AutoSlicingMethod.Smart)
                    {
                        existingOverlappingSprite.rect = frame;
                        existingOverlappingSprite.alignment = (SpriteAlignment) alignment;
                        existingOverlappingSprite.pivot = pivot;
                    }
                }
                else
                {
                    this.AddSpriteWithUniqueName(frame, alignment, pivot, 0xfe, 0.25f, index++);
                }
            }
            else
            {
                this.AddSprite(frame, alignment, pivot, 0xfe, 0.25f, this.GetSpriteNamePrefix() + "_" + ((int) index++));
            }
        }

        private SpriteRect AddSprite(Rect rect, int alignment, Vector2 pivot, int colliderAlphaCutoff, float colliderDetail, string name)
        {
            SpriteRect r = new SpriteRect {
                rect = rect,
                alignment = (SpriteAlignment) alignment,
                pivot = pivot,
                name = name
            };
            r.originalName = r.name;
            r.border = Vector4.zero;
            base.spriteEditor.SetDataModified();
            base.m_RectsCache.AddRect(r);
            base.spriteEditor.SetDataModified();
            return r;
        }

        public SpriteRect AddSpriteWithUniqueName(Rect rect, int alignment, Vector2 pivot, int colliderAlphaCutoff, float colliderDetail, int nameIndexingHint)
        {
            string uniqueName = this.GetUniqueName(this.GetSpriteNamePrefix(), nameIndexingHint);
            return this.AddSprite(rect, alignment, pivot, colliderAlphaCutoff, colliderDetail, uniqueName);
        }

        public override bool CanBeActivated() => 
            (base.spriteImportMode != SpriteImportMode.Polygon);

        public void CreateSprite(Rect rect)
        {
            rect = SpriteFrameModuleBase.ClampSpriteRect(rect, (float) base.previewTexture.width, (float) base.previewTexture.height);
            base.undoSystem.RegisterCompleteObjectUndo(base.m_RectsCache, "Create sprite");
            base.selected = this.AddSpriteWithUniqueName(rect, 0, Vector2.zero, 0xfe, 0.25f, 0);
        }

        public void DeleteSprite()
        {
            if (base.selected != null)
            {
                base.undoSystem.RegisterCompleteObjectUndo(base.m_RectsCache, "Delete sprite");
                base.m_RectsCache.RemoveRect(base.selected);
                base.selected = null;
                base.spriteEditor.SetDataModified();
            }
        }

        public void DoAutomaticSlicing(int minimumSpriteSize, int alignment, Vector2 pivot, AutoSlicingMethod slicingMethod)
        {
            base.undoSystem.RegisterCompleteObjectUndo(base.m_RectsCache, "Automatic Slicing");
            if (slicingMethod == AutoSlicingMethod.DeleteAll)
            {
                base.m_RectsCache.ClearAll();
            }
            List<Rect> rects = new List<Rect>(InternalSpriteUtility.GenerateAutomaticSpriteRectangles((Texture2D) base.spriteEditor.GetReadableTexture2D(), minimumSpriteSize, 0));
            rects = this.SortRects(rects);
            int index = 0;
            foreach (Rect rect in rects)
            {
                this.AddSprite(rect, alignment, pivot, slicingMethod, ref index);
            }
            base.selected = null;
            base.spriteEditor.SetDataModified();
            base.Repaint();
        }

        public void DoGridSlicing(Vector2 size, Vector2 offset, Vector2 padding, int alignment, Vector2 pivot)
        {
            Rect[] rectArray = InternalSpriteUtility.GenerateGridSpriteRectangles((Texture2D) base.spriteEditor.GetReadableTexture2D(), offset, size, padding);
            int num = 0;
            base.undoSystem.RegisterCompleteObjectUndo(base.m_RectsCache, "Grid Slicing");
            base.m_RectsCache.ClearAll();
            foreach (Rect rect in rectArray)
            {
                this.AddSprite(rect, alignment, pivot, 0xfe, 0.25f, this.GetSpriteNamePrefix() + "_" + num++);
            }
            base.selected = null;
            base.spriteEditor.SetDataModified();
            base.Repaint();
        }

        public override void DoTextureGUI()
        {
            base.DoTextureGUI();
            base.DrawSpriteRectGizmos();
            base.HandleGizmoMode();
            if (base.containsMultipleSprites)
            {
                this.HandleRectCornerScalingHandles();
            }
            base.HandleBorderCornerScalingHandles();
            base.HandleBorderSidePointScalingSliders();
            if (base.containsMultipleSprites)
            {
                this.HandleRectSideScalingHandles();
            }
            base.HandleBorderSideScalingHandles();
            base.HandlePivotHandle();
            if (base.containsMultipleSprites)
            {
                this.HandleDragging();
            }
            if (!base.MouseOnTopOfInspector())
            {
                base.spriteEditor.HandleSpriteSelection();
            }
            if (base.containsMultipleSprites)
            {
                this.HandleCreate();
                this.HandleDelete();
                this.HandleDuplicate();
            }
        }

        public override void DrawToolbarGUI(Rect toolbarRect)
        {
            using (new EditorGUI.DisabledScope(!base.containsMultipleSprites || base.spriteEditor.editingDisabled))
            {
                <DrawToolbarGUI>c__AnonStorey0 storey = new <DrawToolbarGUI>c__AnonStorey0 {
                    $this = this,
                    skin = EditorStyles.toolbarPopup
                };
                Rect drawRect = toolbarRect;
                drawRect.width = storey.skin.CalcSize(SpriteFrameModuleStyles.sliceButtonLabel).x;
                SpriteUtilityWindow.DrawToolBarWidget(ref drawRect, ref toolbarRect, new Action<Rect>(storey.<>m__0));
                using (new EditorGUI.DisabledScope(!base.hasSelected))
                {
                    drawRect.x += drawRect.width;
                    drawRect.width = storey.skin.CalcSize(SpriteFrameModuleStyles.trimButtonLabel).x;
                    SpriteUtilityWindow.DrawToolBarWidget(ref drawRect, ref toolbarRect, new Action<Rect>(storey.<>m__1));
                }
            }
        }

        public void DuplicateSprite()
        {
            if (base.selected != null)
            {
                base.undoSystem.RegisterCompleteObjectUndo(base.m_RectsCache, "Duplicate sprite");
                base.selected = this.AddSpriteWithUniqueName(base.selected.rect, (int) base.selected.alignment, base.selected.pivot, 0xfe, 0.25f, 0);
            }
        }

        private SpriteRect GetExistingOverlappingSprite(Rect rect)
        {
            for (int i = 0; i < base.m_RectsCache.Count; i++)
            {
                if (base.m_RectsCache.RectAt(i).rect.Overlaps(rect))
                {
                    return base.m_RectsCache.RectAt(i);
                }
            }
            return null;
        }

        private string GetSpriteNamePrefix() => 
            Path.GetFileNameWithoutExtension(base.spriteAssetPath);

        private string GetUniqueName(string prefix, int startIndex)
        {
            while (true)
            {
                string str = prefix + "_" + startIndex++;
                bool flag = false;
                for (int i = 0; i < base.m_RectsCache.Count; i++)
                {
                    if (base.m_RectsCache.RectAt(i).name == str)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    return str;
                }
            }
        }

        private void HandleCreate()
        {
            if (!base.MouseOnTopOfInspector() && !base.eventSystem.current.alt)
            {
                EditorGUI.BeginChangeCheck();
                Rect rect = SpriteEditorHandles.RectCreator((float) base.previewTexture.width, (float) base.previewTexture.height, SpriteFrameModuleBase.styles.createRect);
                if ((EditorGUI.EndChangeCheck() && (rect.width > 0f)) && (rect.height > 0f))
                {
                    this.CreateSprite(rect);
                    GUIUtility.keyboardControl = 0;
                }
            }
        }

        private void HandleDelete()
        {
            IEvent current = base.eventSystem.current;
            if (((current.type == EventType.ValidateCommand) || (current.type == EventType.ExecuteCommand)) && ((current.commandName == "SoftDelete") || (current.commandName == "Delete")))
            {
                if ((current.type == EventType.ExecuteCommand) && base.hasSelected)
                {
                    this.DeleteSprite();
                }
                current.Use();
            }
        }

        private void HandleDragging()
        {
            if (base.hasSelected && !base.MouseOnTopOfInspector())
            {
                Rect clamp = new Rect(0f, 0f, (float) base.previewTexture.width, (float) base.previewTexture.height);
                EditorGUI.BeginChangeCheck();
                Rect rect3 = SpriteEditorUtility.ClampedRect(SpriteEditorUtility.RoundedRect(SpriteEditorHandles.SliderRect(base.selectedSpriteRect)), clamp, true);
                if (EditorGUI.EndChangeCheck())
                {
                    base.selectedSpriteRect = rect3;
                }
            }
        }

        private void HandleDuplicate()
        {
            IEvent current = base.eventSystem.current;
            if (((current.type == EventType.ValidateCommand) || (current.type == EventType.ExecuteCommand)) && (current.commandName == "Duplicate"))
            {
                if (current.type == EventType.ExecuteCommand)
                {
                    this.DuplicateSprite();
                }
                current.Use();
            }
        }

        private void HandleRectCornerScalingHandles()
        {
            if (base.hasSelected)
            {
                GUIStyle dragdot = SpriteFrameModuleBase.styles.dragdot;
                GUIStyle dragdotactive = SpriteFrameModuleBase.styles.dragdotactive;
                Color white = Color.white;
                Rect r = new Rect(base.selectedSpriteRect);
                float xMin = r.xMin;
                float xMax = r.xMax;
                float yMax = r.yMax;
                float yMin = r.yMin;
                EditorGUI.BeginChangeCheck();
                base.HandleBorderPointSlider(ref xMin, ref yMax, MouseCursor.ResizeUpLeft, false, dragdot, dragdotactive, white);
                base.HandleBorderPointSlider(ref xMax, ref yMax, MouseCursor.ResizeUpRight, false, dragdot, dragdotactive, white);
                base.HandleBorderPointSlider(ref xMin, ref yMin, MouseCursor.ResizeUpRight, false, dragdot, dragdotactive, white);
                base.HandleBorderPointSlider(ref xMax, ref yMin, MouseCursor.ResizeUpLeft, false, dragdot, dragdotactive, white);
                if (EditorGUI.EndChangeCheck())
                {
                    r.xMin = xMin;
                    r.xMax = xMax;
                    r.yMax = yMax;
                    r.yMin = yMin;
                    this.ScaleSpriteRect(r);
                }
            }
        }

        private void HandleRectSideScalingHandles()
        {
            if (base.hasSelected)
            {
                Rect r = new Rect(base.selectedSpriteRect);
                float xMin = r.xMin;
                float xMax = r.xMax;
                float yMax = r.yMax;
                float yMin = r.yMin;
                Vector2 vector = Handles.matrix.MultiplyPoint(new Vector3(r.xMin, r.yMin));
                Vector2 vector2 = Handles.matrix.MultiplyPoint(new Vector3(r.xMax, r.yMax));
                float width = Mathf.Abs((float) (vector2.x - vector.x));
                float height = Mathf.Abs((float) (vector2.y - vector.y));
                EditorGUI.BeginChangeCheck();
                xMin = base.HandleBorderScaleSlider(xMin, r.yMax, width, height, true);
                xMax = base.HandleBorderScaleSlider(xMax, r.yMax, width, height, true);
                yMax = base.HandleBorderScaleSlider(r.xMin, yMax, width, height, false);
                yMin = base.HandleBorderScaleSlider(r.xMin, yMin, width, height, false);
                if (EditorGUI.EndChangeCheck())
                {
                    r.xMin = xMin;
                    r.xMax = xMax;
                    r.yMax = yMax;
                    r.yMin = yMin;
                    this.ScaleSpriteRect(r);
                }
            }
        }

        public override void OnModuleActivate()
        {
            base.m_RectsCache = base.spriteEditor.spriteRects;
            base.spriteEditor.enableMouseMoveEvent = true;
        }

        public override void OnModuleDeactivate()
        {
            base.m_RectsCache = null;
        }

        private bool PixelHasAlpha(int x, int y, ITexture2D texture)
        {
            if (this.m_AlphaPixelCache == null)
            {
                this.m_AlphaPixelCache = new bool[texture.width * texture.height];
                Color32[] colorArray = texture.GetPixels32();
                for (int i = 0; i < colorArray.Length; i++)
                {
                    this.m_AlphaPixelCache[i] = colorArray[i].a != 0;
                }
            }
            int index = (y * texture.width) + x;
            return this.m_AlphaPixelCache[index];
        }

        private List<Rect> RectSweep(List<Rect> rects, Rect sweepRect)
        {
            if ((rects == null) || (rects.Count == 0))
            {
                return new List<Rect>();
            }
            List<Rect> list2 = new List<Rect>();
            foreach (Rect rect in rects)
            {
                if (rect.Overlaps(sweepRect))
                {
                    list2.Add(rect);
                }
            }
            foreach (Rect rect2 in list2)
            {
                rects.Remove(rect2);
            }
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = (a, b) => a.x.CompareTo(b.x);
            }
            list2.Sort(<>f__am$cache0);
            return list2;
        }

        public void ScaleSpriteRect(Rect r)
        {
            if (base.selected != null)
            {
                base.undoSystem.RegisterCompleteObjectUndo(base.m_RectsCache, "Scale sprite");
                base.selected.rect = SpriteFrameModuleBase.ClampSpriteRect(r, (float) base.previewTexture.width, (float) base.previewTexture.height);
                base.selected.border = SpriteFrameModuleBase.ClampSpriteBorderToRect(base.selected.border, base.selected.rect);
                base.spriteEditor.SetDataModified();
            }
        }

        private List<Rect> SortRects(List<Rect> rects)
        {
            List<Rect> list = new List<Rect>();
            while (rects.Count > 0)
            {
                Rect rect = rects[rects.Count - 1];
                Rect sweepRect = new Rect(0f, rect.yMin, (float) base.previewTexture.width, rect.height);
                List<Rect> collection = this.RectSweep(rects, sweepRect);
                if (collection.Count > 0)
                {
                    list.AddRange(collection);
                }
                else
                {
                    list.AddRange(rects);
                    return list;
                }
            }
            return list;
        }

        public void TrimAlpha()
        {
            ITexture2D texture = base.spriteEditor.GetReadableTexture2D();
            if (texture != null)
            {
                Rect rect = base.selected.rect;
                int xMax = (int) rect.xMax;
                int xMin = (int) rect.xMin;
                int yMax = (int) rect.yMax;
                int yMin = (int) rect.yMin;
                for (int i = (int) rect.yMin; i < ((int) rect.yMax); i++)
                {
                    for (int j = (int) rect.xMin; j < ((int) rect.xMax); j++)
                    {
                        if (this.PixelHasAlpha(j, i, texture))
                        {
                            xMax = Mathf.Min(xMax, j);
                            xMin = Mathf.Max(xMin, j);
                            yMax = Mathf.Min(yMax, i);
                            yMin = Mathf.Max(yMin, i);
                        }
                    }
                }
                if ((xMax > xMin) || (yMax > yMin))
                {
                    rect = new Rect(0f, 0f, 0f, 0f);
                }
                else
                {
                    rect = new Rect((float) xMax, (float) yMax, (float) ((xMin - xMax) + 1), (float) ((yMin - yMax) + 1));
                }
                if ((rect.width <= 0f) && (rect.height <= 0f))
                {
                    base.m_RectsCache.RemoveRect(base.selected);
                    base.spriteEditor.SetDataModified();
                    base.selected = null;
                }
                else
                {
                    rect = SpriteFrameModuleBase.ClampSpriteRect(rect, (float) texture.width, (float) texture.height);
                    if (base.selected.rect != rect)
                    {
                        base.spriteEditor.SetDataModified();
                    }
                    base.selected.rect = rect;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <DrawToolbarGUI>c__AnonStorey0
        {
            internal SpriteFrameModule $this;
            internal GUIStyle skin;

            internal void <>m__0(Rect adjustedDrawArea)
            {
                if (GUI.Button(adjustedDrawArea, SpriteFrameModule.SpriteFrameModuleStyles.sliceButtonLabel, this.skin) && SpriteEditorMenu.ShowAtPosition(adjustedDrawArea, this.$this, this.$this.spriteEditor.previewTexture, this.$this.spriteEditor.selectedTexture))
                {
                    GUIUtility.ExitGUI();
                }
            }

            internal void <>m__1(Rect adjustedDrawArea)
            {
                if (GUI.Button(adjustedDrawArea, SpriteFrameModule.SpriteFrameModuleStyles.trimButtonLabel, EditorStyles.toolbarButton) || (string.IsNullOrEmpty(GUI.GetNameOfFocusedControl()) && SpriteFrameModule.k_SpriteEditorTrim.activated))
                {
                    this.$this.TrimAlpha();
                    this.$this.Repaint();
                }
            }
        }

        public enum AutoSlicingMethod
        {
            DeleteAll,
            Smart,
            Safe
        }

        private static class SpriteFrameModuleStyles
        {
            public static readonly GUIContent cancelButtonLabel = EditorGUIUtility.TextContent("Cancel");
            public static readonly GUIContent okButtonLabel = EditorGUIUtility.TextContent("Ok");
            public static readonly GUIContent sliceButtonLabel = EditorGUIUtility.TextContent("Slice");
            public static readonly GUIContent trimButtonLabel = EditorGUIUtility.TextContent("Trim|Trims selected rectangle (T)");
        }
    }
}

