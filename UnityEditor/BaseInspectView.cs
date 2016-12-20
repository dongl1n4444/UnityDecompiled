namespace UnityEditor
{
    using System;
    using System.Collections;
    using UnityEngine;

    internal abstract class BaseInspectView : IBaseInspectView
    {
        protected GUIViewDebuggerWindow m_GuiViewDebuggerWindow;
        protected Vector2 m_InstructionDetailsScrollPos = new Vector2();
        protected readonly SplitterState m_InstructionDetailStacktraceSplitter;
        [NonSerialized]
        public readonly ListViewState m_ListViewState = new ListViewState();
        private Styles m_Styles;

        public BaseInspectView(GUIViewDebuggerWindow guiViewDebuggerWindow)
        {
            float[] relativeSizes = new float[] { 80f, 20f };
            int[] minSizes = new int[] { 100, 100 };
            this.m_InstructionDetailStacktraceSplitter = new SplitterState(relativeSizes, minSizes, null);
            this.m_GuiViewDebuggerWindow = guiViewDebuggerWindow;
        }

        protected abstract void DoDrawInstruction(ListViewElement el, int controlId);
        protected virtual void DoDrawNothingSelected()
        {
            EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.Label("Select a Instruction on the left to see details", GUIViewDebuggerWindow.s_Styles.centeredText, new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }

        internal abstract void DoDrawSelectedInstructionDetails(int index);
        protected void DrawInspectedRect(Rect instructionRect)
        {
            Rect rect = GUILayoutUtility.GetRect((float) 0f, (float) 100f);
            int top = Mathf.CeilToInt(34f);
            int bottom = Mathf.CeilToInt(16f);
            int right = 100;
            RectOffset offset = new RectOffset(50, right, top, bottom);
            Rect position = offset.Remove(rect);
            float imageAspect = instructionRect.width / instructionRect.height;
            Rect outScreenRect = new Rect();
            Rect outSourceRect = new Rect();
            GUI.CalculateScaledTextureRects(position, ScaleMode.ScaleToFit, imageAspect, ref outScreenRect, ref outSourceRect);
            position = outScreenRect;
            position.width = Mathf.Max(80f, position.width);
            position.height = Mathf.Max(26f, position.height);
            Rect rect5 = new Rect {
                height = 16f,
                width = offset.left * 2,
                y = position.y - offset.top
            };
            rect5.x = position.x - (rect5.width / 2f);
            Rect rect7 = new Rect {
                height = 16f,
                width = offset.right * 2,
                y = position.yMax
            };
            Rect rect6 = rect7;
            rect6.x = position.xMax - (rect6.width / 2f);
            rect7 = new Rect {
                x = position.x,
                y = rect5.yMax + 2f,
                width = position.width,
                height = 16f
            };
            Rect rect8 = rect7;
            Rect rect9 = rect8;
            rect9.width = rect8.width / 3f;
            rect9.x = rect8.x + ((rect8.width - rect9.width) / 2f);
            Rect rect10 = position;
            rect10.x = position.xMax;
            rect10.width = 16f;
            Rect rect11 = rect10;
            rect11.height = 16f;
            rect11.width = offset.right;
            rect11.y += (rect10.height - rect11.height) / 2f;
            GUI.Label(rect5, string.Format("({0},{1})", instructionRect.x, instructionRect.y), this.styles.centeredLabel);
            Handles.color = new Color(1f, 1f, 1f, 0.5f);
            Vector3 vector = new Vector3(rect8.x, rect9.y);
            Vector3 vector2 = new Vector3(rect8.x, rect9.yMax);
            Handles.DrawLine(vector, vector2);
            vector.x = vector2.x = rect8.xMax;
            Handles.DrawLine(vector, vector2);
            vector.x = rect8.x;
            vector.y = vector2.y = Mathf.Lerp(vector.y, vector2.y, 0.5f);
            vector2.x = rect9.x;
            Handles.DrawLine(vector, vector2);
            vector.x = rect9.xMax;
            vector2.x = rect8.xMax;
            Handles.DrawLine(vector, vector2);
            GUI.Label(rect9, instructionRect.width.ToString(), this.styles.centeredLabel);
            vector = new Vector3(rect10.x, rect10.y);
            vector2 = new Vector3(rect10.xMax, rect10.y);
            Handles.DrawLine(vector, vector2);
            vector.y = vector2.y = rect10.yMax;
            Handles.DrawLine(vector, vector2);
            vector.x = vector2.x = Mathf.Lerp(vector.x, vector2.x, 0.5f);
            vector.y = rect10.y;
            vector2.y = rect11.y;
            Handles.DrawLine(vector, vector2);
            vector.y = rect11.yMax;
            vector2.y = rect10.yMax;
            Handles.DrawLine(vector, vector2);
            GUI.Label(rect11, instructionRect.height.ToString());
            GUI.Label(rect6, string.Format("({0},{1})", instructionRect.xMax, instructionRect.yMax), this.styles.centeredLabel);
            GUI.Box(position, GUIContent.none);
        }

        protected abstract void DrawInspectedStacktrace();
        public virtual void DrawInstructionList()
        {
            Event current = Event.current;
            this.m_ListViewState.totalRows = this.GetInstructionCount();
            EditorGUILayout.BeginVertical(GUIViewDebuggerWindow.s_Styles.listBackgroundStyle, new GUILayoutOption[0]);
            GUILayout.Label("Instructions", new GUILayoutOption[0]);
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
            IEnumerator enumerator = ListViewGUI.ListView(this.m_ListViewState, GUIViewDebuggerWindow.s_Styles.listBackgroundStyle, new GUILayoutOption[0]).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ListViewElement el = (ListViewElement) enumerator.Current;
                    if ((((current.type == EventType.MouseDown) && (current.button == 0)) && el.position.Contains(current.mousePosition)) && (current.clickCount == 2))
                    {
                        this.OnDoubleClickInstruction(el.row);
                    }
                    if (current.type == EventType.Repaint)
                    {
                        this.DoDrawInstruction(el, controlID);
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
            EditorGUILayout.EndVertical();
        }

        public virtual void DrawSelectedInstructionDetails()
        {
            if (this.m_ListViewState.selectionChanged)
            {
                this.OnSelectedInstructionChanged(this.m_ListViewState.row);
            }
            if (!this.HasSelectedinstruction())
            {
                this.DoDrawNothingSelected();
            }
            else
            {
                SplitterGUILayout.BeginVerticalSplit(this.m_InstructionDetailStacktraceSplitter, new GUILayoutOption[0]);
                this.m_InstructionDetailsScrollPos = EditorGUILayout.BeginScrollView(this.m_InstructionDetailsScrollPos, GUIViewDebuggerWindow.s_Styles.boxStyle, new GUILayoutOption[0]);
                this.DoDrawSelectedInstructionDetails(this.m_ListViewState.row);
                EditorGUILayout.EndScrollView();
                this.DrawInspectedStacktrace();
                SplitterGUILayout.EndVerticalSplit();
            }
        }

        protected void DrawStackFrameList(StackFrame[] stackframes)
        {
            if (stackframes != null)
            {
                foreach (StackFrame frame in stackframes)
                {
                    if (!string.IsNullOrEmpty(frame.sourceFile))
                    {
                        GUILayout.Label(string.Format("{0} [{1}:{2}]", frame.signature, frame.sourceFile, frame.lineNumber), GUIViewDebuggerWindow.s_Styles.stackframeStyle, new GUILayoutOption[0]);
                    }
                }
            }
        }

        protected abstract int GetInstructionCount();
        internal abstract string GetInstructionListName(int index);
        protected virtual bool HasSelectedinstruction()
        {
            return (this.m_ListViewState.row >= 0);
        }

        internal abstract void OnDoubleClickInstruction(int index);
        internal abstract void OnSelectedInstructionChanged(int newSelectionIndex);
        public virtual void SelectRow(int index)
        {
            this.m_ListViewState.row = index;
            this.m_ListViewState.selectionChanged = true;
        }

        public abstract void ShowOverlay();
        public virtual void Unselect()
        {
            this.m_ListViewState.row = -1;
            this.m_ListViewState.selectionChanged = true;
        }

        public abstract void UpdateInstructions();

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

        private class Styles
        {
            public GUIStyle centeredLabel = new GUIStyle("PR Label");

            public Styles()
            {
                this.centeredLabel.alignment = TextAnchor.MiddleCenter;
                this.centeredLabel.padding.right = 0;
                this.centeredLabel.padding.left = 0;
            }
        }
    }
}

