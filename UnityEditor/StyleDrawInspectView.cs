namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class StyleDrawInspectView : BaseInspectView
    {
        [NonSerialized]
        private CachedInstructionInfo m_CachedinstructionInfo;
        [NonSerialized]
        private GUIInstruction m_Instruction;
        private Vector2 m_StacktraceScrollPos;

        public StyleDrawInspectView(GUIViewDebuggerWindow guiViewDebuggerWindow) : base(guiViewDebuggerWindow)
        {
            this.m_StacktraceScrollPos = new Vector2();
        }

        protected override void DoDrawInstruction(ListViewElement el, int id)
        {
            GUIContent content = GUIContent.Temp(this.GetInstructionListName(el.row));
            GUIViewDebuggerWindow.s_Styles.listItemBackground.Draw(el.position, false, false, base.m_ListViewState.row == el.row, false);
            GUIViewDebuggerWindow.s_Styles.listItem.Draw(el.position, content, id, base.m_ListViewState.row == el.row);
        }

        internal override void DoDrawSelectedInstructionDetails(int index)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                base.DrawInspectedRect(this.m_Instruction.rect);
            }
            this.DrawInspectedStyle();
            using (new EditorGUI.DisabledScope(true))
            {
                this.DrawInspectedGUIContent();
            }
        }

        private void DrawInspectedGUIContent()
        {
            GUILayout.Label(GUIContent.Temp("GUIContent"), new GUILayoutOption[0]);
            EditorGUI.indentLevel++;
            EditorGUILayout.TextField(this.m_Instruction.usedGUIContent.text, new GUILayoutOption[0]);
            EditorGUILayout.ObjectField(this.m_Instruction.usedGUIContent.image, typeof(Texture2D), false, new GUILayoutOption[0]);
            EditorGUI.indentLevel--;
        }

        protected override void DrawInspectedStacktrace()
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandHeight(false) };
            this.m_StacktraceScrollPos = EditorGUILayout.BeginScrollView(this.m_StacktraceScrollPos, GUIViewDebuggerWindow.s_Styles.stacktraceBackground, options);
            base.DrawStackFrameList(this.m_Instruction.stackframes);
            EditorGUILayout.EndScrollView();
        }

        private void DrawInspectedStyle()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.m_CachedinstructionInfo.styleSerializedProperty, GUIContent.Temp("Style"), true, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_CachedinstructionInfo.styleContainerSerializedObject.ApplyModifiedPropertiesWithoutUndo();
                base.m_GuiViewDebuggerWindow.m_Inspected.Repaint();
            }
        }

        protected override int GetInstructionCount()
        {
            return GUIViewDebuggerHelper.GetInstructionCount();
        }

        internal override string GetInstructionListName(int index)
        {
            StackFrame[] managedStackTrace = GUIViewDebuggerHelper.GetManagedStackTrace(index);
            string instructionListName = this.GetInstructionListName(managedStackTrace);
            return string.Format("{0}. {1}", index, instructionListName);
        }

        protected string GetInstructionListName(StackFrame[] stacktrace)
        {
            int interestingFrameIndex = this.GetInterestingFrameIndex(stacktrace);
            if (interestingFrameIndex > 0)
            {
                interestingFrameIndex--;
            }
            StackFrame frame = stacktrace[interestingFrameIndex];
            return frame.methodName;
        }

        private int GetInterestingFrameIndex(StackFrame[] stacktrace)
        {
            string dataPath = Application.dataPath;
            int num = -1;
            for (int i = 0; i < stacktrace.Length; i++)
            {
                StackFrame frame = stacktrace[i];
                if ((!string.IsNullOrEmpty(frame.sourceFile) && !frame.signature.StartsWith("UnityEngine.GUI")) && !frame.signature.StartsWith("UnityEditor.EditorGUI"))
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

        public void GetSelectedStyleProperty(out SerializedObject serializedObject, out SerializedProperty styleProperty)
        {
            GUISkin skin = null;
            GUISkin current = GUISkin.current;
            GUIStyle style = current.FindStyle(this.m_Instruction.usedGUIStyle.name);
            if ((style != null) && (style == this.m_Instruction.usedGUIStyle))
            {
                skin = current;
            }
            styleProperty = null;
            if (skin != null)
            {
                serializedObject = new SerializedObject(skin);
                SerializedProperty iterator = serializedObject.GetIterator();
                bool enterChildren = true;
                while (iterator.NextVisible(enterChildren))
                {
                    if (iterator.type == "GUIStyle")
                    {
                        enterChildren = false;
                        if (iterator.FindPropertyRelative("m_Name").stringValue == this.m_Instruction.usedGUIStyle.name)
                        {
                            styleProperty = iterator;
                            return;
                        }
                    }
                    else
                    {
                        enterChildren = true;
                    }
                }
                Debug.Log(string.Format("Showing editable Style from GUISkin: {0}, IsPersistant: {1}", skin.name, EditorUtility.IsPersistent(skin)));
            }
            serializedObject = new SerializedObject(this.m_CachedinstructionInfo.styleContainer);
            styleProperty = serializedObject.FindProperty("inspectedStyle");
        }

        protected override bool HasSelectedinstruction()
        {
            return (this.m_Instruction != null);
        }

        internal override void OnDoubleClickInstruction(int index)
        {
            this.ShowInstructionInExternalEditor(GUIViewDebuggerHelper.GetManagedStackTrace(index));
        }

        internal override void OnSelectedInstructionChanged(int index)
        {
            base.m_ListViewState.row = index;
            if (base.m_ListViewState.row >= 0)
            {
                if (this.m_Instruction == null)
                {
                    this.m_Instruction = new GUIInstruction();
                }
                if (this.m_CachedinstructionInfo == null)
                {
                    this.m_CachedinstructionInfo = new CachedInstructionInfo();
                }
                this.m_Instruction.rect = GUIViewDebuggerHelper.GetRectFromInstruction(base.m_ListViewState.row);
                this.m_Instruction.usedGUIStyle = GUIViewDebuggerHelper.GetStyleFromInstruction(base.m_ListViewState.row);
                this.m_Instruction.usedGUIContent = GUIViewDebuggerHelper.GetContentFromInstruction(base.m_ListViewState.row);
                this.m_Instruction.stackframes = GUIViewDebuggerHelper.GetManagedStackTrace(base.m_ListViewState.row);
                this.m_CachedinstructionInfo.styleContainer.inspectedStyle = this.m_Instruction.usedGUIStyle;
                this.m_CachedinstructionInfo.styleContainerSerializedObject = null;
                this.m_CachedinstructionInfo.styleSerializedProperty = null;
                this.GetSelectedStyleProperty(out this.m_CachedinstructionInfo.styleContainerSerializedObject, out this.m_CachedinstructionInfo.styleSerializedProperty);
                base.m_GuiViewDebuggerWindow.HighlightInstruction(base.m_GuiViewDebuggerWindow.m_Inspected, this.m_Instruction.rect, this.m_Instruction.usedGUIStyle);
            }
            else
            {
                this.m_Instruction = null;
                this.m_CachedinstructionInfo = null;
                if (base.m_GuiViewDebuggerWindow.InstructionOverlayWindow != null)
                {
                    base.m_GuiViewDebuggerWindow.InstructionOverlayWindow.Close();
                }
            }
        }

        private void ShowInstructionInExternalEditor(StackFrame[] frames)
        {
            int interestingFrameIndex = this.GetInterestingFrameIndex(frames);
            StackFrame frame = frames[interestingFrameIndex];
            InternalEditorUtility.OpenFileAtLineExternal(frame.sourceFile, (int) frame.lineNumber);
        }

        public override void ShowOverlay()
        {
            if (this.m_Instruction != null)
            {
                base.m_GuiViewDebuggerWindow.HighlightInstruction(base.m_GuiViewDebuggerWindow.m_Inspected, this.m_Instruction.rect, this.m_Instruction.usedGUIStyle);
            }
        }

        public override void Unselect()
        {
            base.Unselect();
            this.m_Instruction = null;
        }

        public override void UpdateInstructions()
        {
        }

        [Serializable]
        private class CachedInstructionInfo
        {
            public readonly GUIStyleHolder styleContainer = ScriptableObject.CreateInstance<GUIStyleHolder>();
            public SerializedObject styleContainerSerializedObject = null;
            public SerializedProperty styleSerializedProperty = null;
        }

        private class GUIInstruction
        {
            public Rect rect;
            public StackFrame[] stackframes;
            public GUIContent usedGUIContent = GUIContent.none;
            public GUIStyle usedGUIStyle = GUIStyle.none;

            public void Reset()
            {
                this.rect = new Rect();
                this.usedGUIStyle = GUIStyle.none;
                this.usedGUIContent = GUIContent.none;
            }
        }
    }
}

