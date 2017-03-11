namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor.Sprites;
    using UnityEditor.U2D.Interface;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.U2D.Interface;

    internal class SpritePolygonModeModule : SpriteFrameModuleBase, ISpriteEditorModule
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <polygonSides>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private bool <showChangeShapeWindow>k__BackingField;
        private const int k_PolygonChangeShapeWindowHeight = 0x2d;
        private const int k_PolygonChangeShapeWindowMargin = 0x11;
        private const int k_PolygonChangeShapeWindowWarningHeight = 0x41;
        private const int k_PolygonChangeShapeWindowWidth = 150;
        private Rect m_PolygonChangeShapeWindowRect;

        public SpritePolygonModeModule(ISpriteEditor sw, IEventSystem es, IUndoSystem us, IAssetDatabase ad) : base("Sprite Polygon Mode Editor", sw, es, us, ad)
        {
            this.m_PolygonChangeShapeWindowRect = new Rect(0f, 17f, 150f, 45f);
        }

        public override bool CanBeActivated() => 
            (UnityEditor.SpriteUtility.GetSpriteImportMode(base.assetDatabase, base.spriteEditor.selectedTexture) == SpriteImportMode.Polygon);

        private void DeterminePolygonSides()
        {
            if (this.polygonSprite && (base.m_RectsCache.Count == 1))
            {
                SpriteRect rect = base.m_RectsCache.RectAt(0);
                if (rect.outline.Count == 1)
                {
                    this.polygonSides = rect.outline[0].Count;
                }
            }
            else
            {
                this.polygonSides = 0;
            }
        }

        private void DoPolygonChangeShapeWindow()
        {
            if (this.showChangeShapeWindow && !base.spriteEditor.editingDisabled)
            {
                bool flag = false;
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 45f;
                GUILayout.BeginArea(this.m_PolygonChangeShapeWindowRect);
                GUILayout.BeginVertical(GUI.skin.box, new GUILayoutOption[0]);
                IEvent current = base.eventSystem.current;
                if ((this.isSidesValid && (current.type == EventType.KeyDown)) && (current.keyCode == KeyCode.Return))
                {
                    flag = true;
                    current.Use();
                }
                EditorGUI.BeginChangeCheck();
                this.polygonSides = EditorGUILayout.IntField(SpritePolygonModeStyles.sidesLabel, this.polygonSides, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_PolygonChangeShapeWindowRect.height = !this.isSidesValid ? ((float) 0x41) : ((float) 0x2d);
                }
                GUILayout.FlexibleSpace();
                if (!this.isSidesValid)
                {
                    EditorGUILayout.HelpBox(SpritePolygonModeStyles.polygonChangeShapeHelpBoxContent.text, MessageType.Warning, true);
                }
                else
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.FlexibleSpace();
                    EditorGUI.BeginDisabledGroup(!this.isSidesValid);
                    if (GUILayout.Button(SpritePolygonModeStyles.changeButtonLabel, new GUILayoutOption[0]))
                    {
                        flag = true;
                    }
                    EditorGUI.EndDisabledGroup();
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                if (flag)
                {
                    if (this.isSidesValid)
                    {
                        this.GeneratePolygonOutline();
                    }
                    this.showChangeShapeWindow = false;
                    GUIUtility.hotControl = 0;
                    GUIUtility.keyboardControl = 0;
                }
                EditorGUIUtility.labelWidth = labelWidth;
                GUILayout.EndArea();
            }
        }

        public override void DoTextureGUI()
        {
            base.DoTextureGUI();
            this.DrawGizmos();
            base.HandleGizmoMode();
            base.HandleBorderCornerScalingHandles();
            base.HandleBorderSidePointScalingSliders();
            base.HandleBorderSideScalingHandles();
            base.HandlePivotHandle();
            if (!base.MouseOnTopOfInspector())
            {
                base.spriteEditor.HandleSpriteSelection();
            }
        }

        private void DrawGizmos()
        {
            if (base.eventSystem.current.type == EventType.Repaint)
            {
                for (int i = 0; i < base.spriteCount; i++)
                {
                    List<SpriteOutline> spriteOutlineAt = base.GetSpriteOutlineAt(i);
                    Vector2 vector = (Vector2) (base.GetSpriteRectAt(i).size * 0.5f);
                    if (spriteOutlineAt.Count > 0)
                    {
                        SpriteEditorUtility.BeginLines(new Color(0.75f, 0.75f, 0.75f, 0.75f));
                        for (int j = 0; j < spriteOutlineAt.Count; j++)
                        {
                            int num3 = 0;
                            int num4 = spriteOutlineAt[j].Count - 1;
                            while (num3 < spriteOutlineAt[j].Count)
                            {
                                SpriteEditorUtility.DrawLine((Vector3) (spriteOutlineAt[j][num4] + vector), (Vector3) (spriteOutlineAt[j][num3] + vector));
                                num4 = num3;
                                num3++;
                            }
                        }
                        SpriteEditorUtility.EndLines();
                    }
                }
                base.DrawSpriteRectGizmos();
            }
        }

        public override void DrawToolbarGUI(Rect toolbarRect)
        {
            using (new EditorGUI.DisabledScope(base.spriteEditor.editingDisabled))
            {
                GUIStyle toolbarPopup = EditorStyles.toolbarPopup;
                Rect drawRect = toolbarRect;
                drawRect.width = toolbarPopup.CalcSize(SpritePolygonModeStyles.changeShapeLabel).x;
                SpriteUtilityWindow.DrawToolBarWidget(ref drawRect, ref toolbarRect, adjustedDrawArea => this.showChangeShapeWindow = GUI.Toggle(adjustedDrawArea, this.showChangeShapeWindow, SpritePolygonModeStyles.changeShapeLabel, EditorStyles.toolbarButton));
            }
        }

        public void GeneratePolygonOutline()
        {
            for (int i = 0; i < base.m_RectsCache.Count; i++)
            {
                SpriteRect rect = base.m_RectsCache.RectAt(i);
                SpriteOutline item = new SpriteOutline();
                item.AddRange(UnityEditor.Sprites.SpriteUtility.GeneratePolygonOutlineVerticesOfSize(this.polygonSides, (int) rect.rect.width, (int) rect.rect.height));
                rect.outline.Clear();
                rect.outline.Add(item);
                base.spriteEditor.SetDataModified();
            }
            base.Repaint();
        }

        public int GetPolygonSideCount()
        {
            this.DeterminePolygonSides();
            return this.polygonSides;
        }

        public override void OnModuleActivate()
        {
            base.OnModuleActivate();
            base.m_RectsCache = base.spriteEditor.spriteRects;
            this.showChangeShapeWindow = this.polygonSprite;
            if (this.polygonSprite)
            {
                this.DeterminePolygonSides();
            }
        }

        public override void OnModuleDeactivate()
        {
            base.m_RectsCache = null;
        }

        public override void OnPostGUI()
        {
            this.DoPolygonChangeShapeWindow();
            base.OnPostGUI();
        }

        private bool isSidesValid =>
            ((this.polygonSides == 0) || ((this.polygonSides >= 3) && (this.polygonSides <= 0x80)));

        public int polygonSides { get; set; }

        private bool polygonSprite =>
            (base.spriteImportMode == SpriteImportMode.Polygon);

        public bool showChangeShapeWindow { get; set; }

        private static class SpritePolygonModeStyles
        {
            public static readonly GUIContent changeButtonLabel = EditorGUIUtility.TextContent("Change|Change to the new number of sides");
            public static readonly GUIContent changeShapeLabel = EditorGUIUtility.TextContent("Change Shape");
            public static readonly GUIContent polygonChangeShapeHelpBoxContent = EditorGUIUtility.TextContent("Sides can only be either 0 or anything between 3 and 128");
            public static readonly GUIContent sidesLabel = EditorGUIUtility.TextContent("Sides");
        }
    }
}

