namespace UnityEditor
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEngine;

    internal class ShapeEditorRectSelectionTool
    {
        [CompilerGenerated]
        private static Action<Rect, ShapeEditor.SelectionType> <>f__am$cache0;
        [CompilerGenerated]
        private static System.Action <>f__am$cache1;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IGUIUtility <guiUtility>k__BackingField;
        private const float k_MinSelectionSize = 6f;
        private bool m_RectSelecting;
        private int m_RectSelectionID;
        private Vector2 m_SelectMousePoint;
        private Vector2 m_SelectStartPoint;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event System.Action ClearSelection;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event Action<Rect, ShapeEditor.SelectionType> RectSelect;

        public ShapeEditorRectSelectionTool(IGUIUtility gu)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Action<Rect, ShapeEditor.SelectionType>(ShapeEditorRectSelectionTool.<RectSelect>m__0);
            }
            this.RectSelect = <>f__am$cache0;
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new System.Action(ShapeEditorRectSelectionTool.<ClearSelection>m__1);
            }
            this.ClearSelection = <>f__am$cache1;
            this.guiUtility = gu;
            this.m_RectSelectionID = this.guiUtility.GetPermanentControlID();
        }

        [CompilerGenerated]
        private static void <ClearSelection>m__1()
        {
        }

        [CompilerGenerated]
        private static void <RectSelect>m__0(Rect i, ShapeEditor.SelectionType p)
        {
        }

        public void OnGUI()
        {
            ShapeEditor.SelectionType normal;
            Event current = Event.current;
            Handles.BeginGUI();
            Vector2 mousePosition = current.mousePosition;
            int rectSelectionID = this.m_RectSelectionID;
            switch (current.GetTypeForControl(rectSelectionID))
            {
                case EventType.MouseDown:
                    if ((HandleUtility.nearestControl == rectSelectionID) && (current.button == 0))
                    {
                        this.guiUtility.hotControl = rectSelectionID;
                        this.m_SelectStartPoint = mousePosition;
                    }
                    goto Label_0256;

                case EventType.MouseUp:
                    if ((this.guiUtility.hotControl != rectSelectionID) || (current.button != 0))
                    {
                        goto Label_0256;
                    }
                    this.guiUtility.hotControl = 0;
                    this.guiUtility.keyboardControl = 0;
                    if (!this.m_RectSelecting)
                    {
                        this.ClearSelection();
                        goto Label_024A;
                    }
                    this.m_SelectMousePoint = new Vector2(mousePosition.x, mousePosition.y);
                    normal = ShapeEditor.SelectionType.Normal;
                    if (!Event.current.control)
                    {
                        if (Event.current.shift)
                        {
                            normal = ShapeEditor.SelectionType.Additive;
                        }
                        break;
                    }
                    normal = ShapeEditor.SelectionType.Subtractive;
                    break;

                case EventType.MouseDrag:
                    if (this.guiUtility.hotControl == rectSelectionID)
                    {
                        if (!this.m_RectSelecting)
                        {
                            Vector2 vector2 = mousePosition - this.m_SelectStartPoint;
                            if (vector2.magnitude > 6f)
                            {
                                this.m_RectSelecting = true;
                            }
                        }
                        if (this.m_RectSelecting)
                        {
                            this.m_SelectMousePoint = mousePosition;
                            ShapeEditor.SelectionType subtractive = ShapeEditor.SelectionType.Normal;
                            if (Event.current.control)
                            {
                                subtractive = ShapeEditor.SelectionType.Subtractive;
                            }
                            else if (Event.current.shift)
                            {
                                subtractive = ShapeEditor.SelectionType.Additive;
                            }
                            this.RectSelect(EditorGUIExt.FromToRect(this.m_SelectStartPoint, this.m_SelectMousePoint), subtractive);
                        }
                        current.Use();
                    }
                    goto Label_0256;

                case EventType.Repaint:
                    if ((this.guiUtility.hotControl == rectSelectionID) && this.m_RectSelecting)
                    {
                        EditorStyles.selectionRect.Draw(EditorGUIExt.FromToRect(this.m_SelectStartPoint, this.m_SelectMousePoint), GUIContent.none, false, false, false, false);
                    }
                    goto Label_0256;

                case EventType.Layout:
                    if (!Tools.viewToolActive)
                    {
                        HandleUtility.AddDefaultControl(rectSelectionID);
                    }
                    goto Label_0256;

                default:
                    goto Label_0256;
            }
            this.RectSelect(EditorGUIExt.FromToRect(this.m_SelectStartPoint, this.m_SelectMousePoint), normal);
            this.m_RectSelecting = false;
        Label_024A:
            current.Use();
        Label_0256:
            Handles.EndGUI();
        }

        private IGUIUtility guiUtility { get; set; }

        public bool isSelecting =>
            (this.guiUtility.hotControl == this.m_RectSelectionID);
    }
}

