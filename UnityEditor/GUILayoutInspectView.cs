namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class GUILayoutInspectView : BaseInspectView
    {
        private GUIStyle m_FakeMargingStyleForOverlay;
        private readonly List<IMGUILayoutInstruction> m_LayoutInstructions;
        private Vector2 m_StacktraceScrollPos;

        public GUILayoutInspectView(GUIViewDebuggerWindow guiViewDebuggerWindow) : base(guiViewDebuggerWindow)
        {
            this.m_StacktraceScrollPos = new Vector2();
            this.m_LayoutInstructions = new List<IMGUILayoutInstruction>();
            this.m_FakeMargingStyleForOverlay = new GUIStyle();
        }

        protected override void DoDrawInstruction(ListViewElement el, int id)
        {
            IMGUILayoutInstruction instruction = this.m_LayoutInstructions[el.row];
            GUIContent content = GUIContent.Temp(this.GetInstructionListName(el.row));
            Rect position = el.position;
            position.xMin += instruction.level * 10;
            GUIViewDebuggerWindow.s_Styles.listItemBackground.Draw(position, false, false, base.m_ListViewState.row == el.row, false);
            GUIViewDebuggerWindow.s_Styles.listItem.Draw(position, content, id, base.m_ListViewState.row == el.row);
        }

        internal override void DoDrawSelectedInstructionDetails(int index)
        {
            IMGUILayoutInstruction instruction = this.m_LayoutInstructions[index];
            using (new EditorGUI.DisabledScope(true))
            {
                base.DrawInspectedRect(instruction.unclippedRect);
                EditorGUILayout.IntField("margin.left", instruction.marginLeft, new GUILayoutOption[0]);
                EditorGUILayout.IntField("margin.top", instruction.marginTop, new GUILayoutOption[0]);
                EditorGUILayout.IntField("margin.right", instruction.marginRight, new GUILayoutOption[0]);
                EditorGUILayout.IntField("margin.bottom", instruction.marginBottom, new GUILayoutOption[0]);
                if (instruction.style != null)
                {
                    EditorGUILayout.LabelField("Style Name", instruction.style.name, new GUILayoutOption[0]);
                }
                if (instruction.isGroup != 1)
                {
                    EditorGUILayout.Toggle("IsVertical", instruction.isVertical == 1, new GUILayoutOption[0]);
                }
            }
        }

        protected override void DrawInspectedStacktrace()
        {
            IMGUILayoutInstruction instruction = this.m_LayoutInstructions[base.m_ListViewState.row];
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandHeight(false) };
            this.m_StacktraceScrollPos = EditorGUILayout.BeginScrollView(this.m_StacktraceScrollPos, GUIViewDebuggerWindow.s_Styles.stacktraceBackground, options);
            base.DrawStackFrameList(instruction.stack);
            EditorGUILayout.EndScrollView();
        }

        protected override int GetInstructionCount() => 
            this.m_LayoutInstructions.Count;

        internal override string GetInstructionListName(int index)
        {
            IMGUILayoutInstruction instruction = this.m_LayoutInstructions[index];
            StackFrame[] stack = instruction.stack;
            int interestingFrameIndex = this.GetInterestingFrameIndex(stack);
            if (interestingFrameIndex > 0)
            {
                interestingFrameIndex--;
            }
            StackFrame frame = stack[interestingFrameIndex];
            return frame.methodName;
        }

        private int GetInterestingFrameIndex(StackFrame[] stacktrace)
        {
            string dataPath = Application.dataPath;
            int num = -1;
            for (int i = 0; i < stacktrace.Length; i++)
            {
                StackFrame frame = stacktrace[i];
                if ((!string.IsNullOrEmpty(frame.sourceFile) && !frame.signature.StartsWith("UnityEngine.GUIDebugger")) && !frame.signature.StartsWith("UnityEngine.GUILayoutUtility"))
                {
                    if (num == -1)
                    {
                        num = i;
                    }
                    if (frame.sourceFile.StartsWith(dataPath))
                    {
                        return i;
                    }
                }
            }
            if (num != -1)
            {
                return num;
            }
            return (stacktrace.Length - 1);
        }

        internal override void OnDoubleClickInstruction(int index)
        {
            throw new NotImplementedException();
        }

        internal override void OnSelectedInstructionChanged(int index)
        {
            base.m_ListViewState.row = index;
            this.ShowOverlay();
        }

        public override void ShowOverlay()
        {
            if (this.HasSelectedinstruction())
            {
                IMGUILayoutInstruction instruction = this.m_LayoutInstructions[base.m_ListViewState.row];
                RectOffset offset = new RectOffset {
                    left = instruction.marginLeft,
                    right = instruction.marginRight,
                    top = instruction.marginTop,
                    bottom = instruction.marginBottom
                };
                this.m_FakeMargingStyleForOverlay.padding = offset;
                Rect unclippedRect = instruction.unclippedRect;
                unclippedRect = offset.Add(unclippedRect);
                base.m_GuiViewDebuggerWindow.HighlightInstruction(base.m_GuiViewDebuggerWindow.m_Inspected, unclippedRect, this.m_FakeMargingStyleForOverlay);
            }
        }

        public override void UpdateInstructions()
        {
            this.m_LayoutInstructions.Clear();
            GUIViewDebuggerHelper.GetLayoutInstructions(this.m_LayoutInstructions);
        }
    }
}

