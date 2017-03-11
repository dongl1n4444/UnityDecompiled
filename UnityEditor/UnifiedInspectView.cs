namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class UnifiedInspectView : BaseInspectView
    {
        private BaseInspectView m_InstructionClipView;
        private BaseInspectView m_InstructionLayoutView;
        private readonly List<IMGUIInstruction> m_Instructions;
        private BaseInspectView m_InstructionStyleView;
        private Vector2 m_StacktraceScrollPos;

        public UnifiedInspectView(GUIViewDebuggerWindow guiViewDebuggerWindow) : base(guiViewDebuggerWindow)
        {
            this.m_Instructions = new List<IMGUIInstruction>();
            this.m_StacktraceScrollPos = new Vector2();
            this.m_InstructionClipView = new GUIClipInspectView(guiViewDebuggerWindow);
            this.m_InstructionStyleView = new StyleDrawInspectView(guiViewDebuggerWindow);
            this.m_InstructionLayoutView = new GUILayoutInspectView(guiViewDebuggerWindow);
        }

        protected override void DoDrawInstruction(ListViewElement el, int controlId)
        {
            IMGUIInstruction instruction = this.m_Instructions[el.row];
            GUIContent content = GUIContent.Temp(this.GetInstructionListName(el.row));
            Rect position = el.position;
            position.xMin += instruction.level * 10;
            GUIViewDebuggerWindow.s_Styles.listItemBackground.Draw(position, false, false, base.m_ListViewState.row == el.row, false);
            GUIViewDebuggerWindow.s_Styles.listItem.Draw(position, content, controlId, base.m_ListViewState.row == el.row);
        }

        internal override void DoDrawSelectedInstructionDetails(int index)
        {
            IMGUIInstruction instruction = this.m_Instructions[index];
            BaseInspectView inspectViewForType = this.GetInspectViewForType(instruction.type);
            inspectViewForType.DoDrawSelectedInstructionDetails(instruction.typeInstructionIndex);
        }

        protected override void DrawInspectedStacktrace()
        {
            IMGUIInstruction instruction = this.m_Instructions[base.m_ListViewState.row];
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandHeight(false) };
            this.m_StacktraceScrollPos = EditorGUILayout.BeginScrollView(this.m_StacktraceScrollPos, GUIViewDebuggerWindow.s_Styles.stacktraceBackground, options);
            base.DrawStackFrameList(instruction.stack);
            EditorGUILayout.EndScrollView();
        }

        protected BaseInspectView GetInspectViewForType(UnityEditor.InstructionType type)
        {
            switch (type)
            {
                case UnityEditor.InstructionType.kStyleDraw:
                    return this.m_InstructionStyleView;

                case UnityEditor.InstructionType.kClipPush:
                case UnityEditor.InstructionType.kClipPop:
                    return this.m_InstructionClipView;

                case UnityEditor.InstructionType.kLayoutBeginGroup:
                case UnityEditor.InstructionType.kLayoutEndGroup:
                case UnityEditor.InstructionType.kLayoutEntry:
                    return this.m_InstructionLayoutView;
            }
            throw new NotImplementedException("Unhandled InstructionType");
        }

        protected override int GetInstructionCount() => 
            this.m_Instructions.Count;

        internal override string GetInstructionListName(int index)
        {
            IMGUIInstruction instruction = this.m_Instructions[index];
            BaseInspectView inspectViewForType = this.GetInspectViewForType(instruction.type);
            return inspectViewForType.GetInstructionListName(instruction.typeInstructionIndex);
        }

        internal override void OnDoubleClickInstruction(int index)
        {
            IMGUIInstruction instruction = this.m_Instructions[index];
            BaseInspectView inspectViewForType = this.GetInspectViewForType(instruction.type);
            inspectViewForType.OnDoubleClickInstruction(instruction.typeInstructionIndex);
        }

        internal override void OnSelectedInstructionChanged(int index)
        {
            base.m_ListViewState.row = index;
            IMGUIInstruction instruction = this.m_Instructions[base.m_ListViewState.row];
            BaseInspectView inspectViewForType = this.GetInspectViewForType(instruction.type);
            inspectViewForType.OnSelectedInstructionChanged(instruction.typeInstructionIndex);
            this.ShowOverlay();
        }

        public override void ShowOverlay()
        {
            if (this.HasSelectedinstruction())
            {
                IMGUIInstruction instruction = this.m_Instructions[base.m_ListViewState.row];
                this.GetInspectViewForType(instruction.type).ShowOverlay();
            }
        }

        public override void UpdateInstructions()
        {
            this.m_InstructionClipView.UpdateInstructions();
            this.m_InstructionStyleView.UpdateInstructions();
            this.m_InstructionLayoutView.UpdateInstructions();
            this.m_Instructions.Clear();
            GUIViewDebuggerHelper.GetUnifiedInstructions(this.m_Instructions);
        }
    }
}

