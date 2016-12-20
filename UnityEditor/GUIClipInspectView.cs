namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class GUIClipInspectView : BaseInspectView
    {
        private List<IMGUIClipInstruction> m_ClipList;
        private IMGUIClipInstruction m_Instruction;
        private Vector2 m_StacktraceScrollPos;

        public GUIClipInspectView(GUIViewDebuggerWindow guiViewDebuggerWindow) : base(guiViewDebuggerWindow)
        {
            this.m_ClipList = new List<IMGUIClipInstruction>();
            this.m_StacktraceScrollPos = new Vector2();
        }

        protected override void DoDrawInstruction(ListViewElement el, int id)
        {
            IMGUIClipInstruction instruction = this.m_ClipList[el.row];
            GUIContent content = GUIContent.Temp(this.GetInstructionListName(el.row));
            Rect position = el.position;
            position.xMin += instruction.level * 12;
            GUIViewDebuggerWindow.s_Styles.listItemBackground.Draw(el.position, false, false, base.m_ListViewState.row == el.row, false);
            GUIViewDebuggerWindow.s_Styles.listItem.Draw(position, content, id, base.m_ListViewState.row == el.row);
        }

        internal override void DoDrawSelectedInstructionDetails(int index)
        {
            IMGUIClipInstruction instruction = this.m_ClipList[index];
            GUILayout.Label("RenderOffset:", new GUILayoutOption[0]);
            GUILayout.Label(instruction.renderOffset.ToString(), new GUILayoutOption[0]);
            GUILayout.Label("ResetOffset:", new GUILayoutOption[0]);
            GUILayout.Label(instruction.resetOffset.ToString(), new GUILayoutOption[0]);
            GUILayout.Label("screenRect:", new GUILayoutOption[0]);
            GUILayout.Label(instruction.screenRect.ToString(), new GUILayoutOption[0]);
            GUILayout.Label("scrollOffset:", new GUILayoutOption[0]);
            GUILayout.Label(instruction.scrollOffset.ToString(), new GUILayoutOption[0]);
        }

        protected override void DrawInspectedStacktrace()
        {
            IMGUIClipInstruction instruction = this.m_ClipList[base.m_ListViewState.row];
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandHeight(false) };
            this.m_StacktraceScrollPos = EditorGUILayout.BeginScrollView(this.m_StacktraceScrollPos, GUIViewDebuggerWindow.s_Styles.stacktraceBackground, options);
            base.DrawStackFrameList(instruction.pushStacktrace);
            EditorGUILayout.EndScrollView();
        }

        protected override int GetInstructionCount()
        {
            return this.m_ClipList.Count;
        }

        internal override string GetInstructionListName(int index)
        {
            IMGUIClipInstruction instruction = this.m_ClipList[index];
            StackFrame[] pushStacktrace = instruction.pushStacktrace;
            if (pushStacktrace.Length == 0)
            {
                return "Empty";
            }
            int interestingFrameIndex = this.GetInterestingFrameIndex(pushStacktrace);
            StackFrame frame = pushStacktrace[interestingFrameIndex];
            return frame.methodName;
        }

        private int GetInterestingFrameIndex(StackFrame[] stacktrace)
        {
            string dataPath = Application.dataPath;
            int num = -1;
            for (int i = 0; i < stacktrace.Length; i++)
            {
                StackFrame frame = stacktrace[i];
                if (!string.IsNullOrEmpty(frame.sourceFile) && !frame.signature.StartsWith("UnityEngine.GUIClip"))
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
                IMGUIClipInstruction instruction = this.m_ClipList[base.m_ListViewState.row];
                base.m_GuiViewDebuggerWindow.HighlightInstruction(base.m_GuiViewDebuggerWindow.m_Inspected, instruction.unclippedScreenRect, GUIStyle.none);
            }
        }

        public override void UpdateInstructions()
        {
            this.m_ClipList.Clear();
            GUIViewDebuggerHelper.GetClipInstructions(this.m_ClipList);
        }
    }
}

