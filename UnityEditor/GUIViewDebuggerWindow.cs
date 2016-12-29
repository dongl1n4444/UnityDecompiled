namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class GUIViewDebuggerWindow : EditorWindow
    {
        public GUIView m_Inspected;
        private readonly SplitterState m_InstructionListDetailSplitter;
        private IBaseInspectView m_InstructionModeView;
        private UnityEditor.InstructionOverlayWindow m_InstructionOverlayWindow;
        private InstructionType m_InstructionType = InstructionType.Draw;
        [NonSerialized]
        private int m_LastSelectedRow;
        [NonSerialized]
        private Vector2 m_PointToInspect;
        [NonSerialized]
        private bool m_QueuedPointInspection = false;
        private bool m_ShowOverlay = true;
        private static GUIViewDebuggerWindow s_ActiveInspector;
        public static Styles s_Styles;

        public GUIViewDebuggerWindow()
        {
            float[] relativeSizes = new float[] { 30f, 70f };
            int[] minSizes = new int[] { 0x20, 0x20 };
            this.m_InstructionListDetailSplitter = new SplitterState(relativeSizes, minSizes, null);
            this.m_InstructionModeView = new StyleDrawInspectView(this);
        }

        private bool CanInspectView(GUIView view)
        {
            if (view == null)
            {
                return false;
            }
            EditorWindow editorWindow = GetEditorWindow(view);
            if ((editorWindow != null) && ((editorWindow == this) || (editorWindow == this.m_InstructionOverlayWindow)))
            {
                return false;
            }
            return true;
        }

        private void DoInspectTypePopup()
        {
            EditorGUI.BeginChangeCheck();
            InstructionType type = (InstructionType) EditorGUILayout.EnumPopup(this.m_InstructionType, EditorStyles.toolbarDropDown, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_InstructionType = type;
                switch (this.m_InstructionType)
                {
                    case InstructionType.Draw:
                        this.m_InstructionModeView = new StyleDrawInspectView(this);
                        break;

                    case InstructionType.Clip:
                        this.m_InstructionModeView = new GUIClipInspectView(this);
                        break;

                    case InstructionType.Layout:
                        this.m_InstructionModeView = new GUILayoutInspectView(this);
                        break;

                    case InstructionType.Unified:
                        this.m_InstructionModeView = new UnifiedInspectView(this);
                        break;
                }
                this.m_InstructionModeView.UpdateInstructions();
            }
        }

        private void DoInstructionOverlayToggle()
        {
            EditorGUI.BeginChangeCheck();
            this.m_ShowOverlay = GUILayout.Toggle(this.m_ShowOverlay, GUIContent.Temp("Show overlay"), EditorStyles.toolbarButton, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.OnShowOverlayChanged();
            }
        }

        private void DoToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
            this.DoWindowPopup();
            this.DoInspectTypePopup();
            this.DoInstructionOverlayToggle();
            GUILayout.EndHorizontal();
        }

        private void DoWindowPopup()
        {
            string t = "<Please Select>";
            if (this.m_Inspected != null)
            {
                t = GetViewName(this.m_Inspected);
            }
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            GUILayout.Label("Inspected Window: ", options);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            Rect position = GUILayoutUtility.GetRect(GUIContent.Temp(t), EditorStyles.toolbarDropDown, optionArray2);
            if (GUI.Button(position, GUIContent.Temp(t), EditorStyles.toolbarDropDown))
            {
                List<GUIView> views = new List<GUIView>();
                GUIViewDebuggerHelper.GetViews(views);
                List<GUIContent> list2 = new List<GUIContent>(views.Count + 1) {
                    new GUIContent("None")
                };
                int selected = 0;
                List<GUIView> userData = new List<GUIView>(views.Count + 1);
                for (int i = 0; i < views.Count; i++)
                {
                    GUIView view = views[i];
                    if (this.CanInspectView(view))
                    {
                        GUIContent item = new GUIContent(list2.Count + ". " + GetViewName(view));
                        list2.Add(item);
                        userData.Add(view);
                        if (view == this.m_Inspected)
                        {
                            selected = userData.Count;
                        }
                    }
                }
                EditorUtility.DisplayCustomMenu(position, list2.ToArray(), selected, new EditorUtility.SelectMenuItemFunction(this.OnWindowSelected), userData);
            }
        }

        private int FindInstructionUnderPoint(Vector2 point)
        {
            int instructionCount = GUIViewDebuggerHelper.GetInstructionCount();
            for (int i = 0; i < instructionCount; i++)
            {
                if (GUIViewDebuggerHelper.GetRectFromInstruction(i).Contains(point))
                {
                    return i;
                }
            }
            return -1;
        }

        private static EditorWindow GetEditorWindow(GUIView view)
        {
            HostView view2 = view as HostView;
            if (view2 != null)
            {
                return view2.actualView;
            }
            return null;
        }

        private static string GetViewName(GUIView view)
        {
            EditorWindow editorWindow = GetEditorWindow(view);
            if (editorWindow != null)
            {
                return editorWindow.titleContent.text;
            }
            return view.GetType().Name;
        }

        public void HighlightInstruction(GUIView view, Rect instructionRect, GUIStyle style)
        {
            if (this.m_ShowOverlay)
            {
                if (this.m_InstructionOverlayWindow == null)
                {
                    this.m_InstructionOverlayWindow = ScriptableObject.CreateInstance<UnityEditor.InstructionOverlayWindow>();
                }
                this.m_InstructionOverlayWindow.Show(view, instructionRect, style);
                base.Focus();
            }
        }

        private static void Init()
        {
            if (s_ActiveInspector == null)
            {
                GUIViewDebuggerWindow window = (GUIViewDebuggerWindow) EditorWindow.GetWindow(typeof(GUIViewDebuggerWindow));
                s_ActiveInspector = window;
            }
            s_ActiveInspector.Show();
        }

        private void InitializeStylesIfNeeded()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
        }

        private static void InspectPoint(Vector2 point)
        {
            Debug.Log("Inspecting " + point);
            s_ActiveInspector.InspectPointAt(point);
        }

        private void InspectPointAt(Vector2 point)
        {
            this.m_PointToInspect = point;
            this.m_QueuedPointInspection = true;
            this.m_Inspected.Repaint();
            base.Repaint();
        }

        private void OnDisable()
        {
            GUIViewDebuggerHelper.StopDebugging();
            if (this.m_InstructionOverlayWindow != null)
            {
                this.m_InstructionOverlayWindow.Close();
            }
        }

        private void OnEnable()
        {
            base.titleContent = new GUIContent("GUI Inspector");
        }

        private void OnGUI()
        {
            this.InitializeStylesIfNeeded();
            this.DoToolbar();
            this.ShowDrawInstructions();
        }

        private static void OnInspectedViewChanged()
        {
            if (s_ActiveInspector != null)
            {
                s_ActiveInspector.RefreshData();
                s_ActiveInspector.Repaint();
            }
        }

        private void OnShowOverlayChanged()
        {
            if (!this.m_ShowOverlay)
            {
                if (this.m_InstructionOverlayWindow != null)
                {
                    this.m_InstructionOverlayWindow.Close();
                }
            }
            else if (this.m_Inspected != null)
            {
                this.instructionModeView.ShowOverlay();
            }
        }

        private void OnWindowSelected(object userdata, string[] options, int selected)
        {
            GUIView view;
            selected--;
            if (selected >= 0)
            {
                List<GUIView> list = (List<GUIView>) userdata;
                view = list[selected];
            }
            else
            {
                view = null;
            }
            if (this.m_Inspected != view)
            {
                if (this.m_InstructionOverlayWindow != null)
                {
                    this.m_InstructionOverlayWindow.Close();
                }
                this.m_Inspected = view;
                if (this.m_Inspected != null)
                {
                    GUIViewDebuggerHelper.DebugWindow(this.m_Inspected);
                    this.m_Inspected.Repaint();
                }
                else
                {
                    GUIViewDebuggerHelper.StopDebugging();
                }
                this.instructionModeView.Unselect();
            }
            base.Repaint();
        }

        private void RefreshData()
        {
            this.instructionModeView.UpdateInstructions();
        }

        private void ShowDrawInstructions()
        {
            if (this.m_Inspected != null)
            {
                if (this.m_QueuedPointInspection)
                {
                    this.instructionModeView.Unselect();
                    this.instructionModeView.SelectRow(this.FindInstructionUnderPoint(this.m_PointToInspect));
                    this.m_QueuedPointInspection = false;
                }
                SplitterGUILayout.BeginHorizontalSplit(this.m_InstructionListDetailSplitter, new GUILayoutOption[0]);
                this.instructionModeView.DrawInstructionList();
                EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
                this.instructionModeView.DrawSelectedInstructionDetails();
                EditorGUILayout.EndVertical();
                SplitterGUILayout.EndHorizontalSplit();
            }
        }

        public IBaseInspectView instructionModeView =>
            this.m_InstructionModeView;

        public UnityEditor.InstructionOverlayWindow InstructionOverlayWindow
        {
            get => 
                this.m_InstructionOverlayWindow;
            set
            {
                this.m_InstructionOverlayWindow = value;
            }
        }

        private enum InstructionType
        {
            Draw,
            Clip,
            Layout,
            Unified
        }

        internal class Styles
        {
            public readonly GUIStyle boxStyle = new GUIStyle("CN Box");
            public readonly GUIStyle centeredText = new GUIStyle("PR Label");
            public readonly GUIStyle listBackgroundStyle = new GUIStyle("CN Box");
            public readonly GUIStyle listItem = new GUIStyle("PR Label");
            public readonly GUIStyle listItemBackground = new GUIStyle("CN EntryBackOdd");
            public readonly GUIStyle stackframeStyle = new GUIStyle(EditorStyles.label);
            public readonly GUIStyle stacktraceBackground = new GUIStyle("CN Box");

            public Styles()
            {
                this.stackframeStyle.margin = new RectOffset(0, 0, 0, 0);
                this.stackframeStyle.padding = new RectOffset(0, 0, 0, 0);
                this.stacktraceBackground.padding = new RectOffset(5, 5, 5, 5);
                this.centeredText.alignment = TextAnchor.MiddleCenter;
                this.centeredText.stretchHeight = true;
                this.centeredText.stretchWidth = true;
            }
        }
    }
}

