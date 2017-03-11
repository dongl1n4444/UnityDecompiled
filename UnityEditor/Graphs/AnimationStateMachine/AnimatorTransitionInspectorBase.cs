namespace UnityEditor.Graphs.AnimationStateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEditor.Graphs;
    using UnityEditorInternal;
    using UnityEngine;

    internal abstract class AnimatorTransitionInspectorBase : Editor
    {
        [CompilerGenerated]
        private static ReorderableList.HeaderCallbackDelegate <>f__mg$cache0;
        protected ReorderableList m_ConditionList;
        protected SerializedProperty m_Conditions;
        protected UnityEditor.Animations.AnimatorController m_Controller;
        protected static List<AnimatorConditionMode> m_floatModes;
        protected static List<AnimatorConditionMode> m_intModes;
        protected int m_LayerIndex;
        protected SerializedProperty m_Name;
        protected Animator m_PreviewObject;
        protected SerializedObject m_SerializedTransition;
        private bool m_SingleTransitionHeader = false;
        private bool m_SyncTransitionContexts = false;
        protected TransitionEditionContext[] m_TransitionContexts;
        protected ReorderableList m_TransitionList;
        private static Styles s_Styles;
        public bool showTransitionList = true;
        private const int toggleColumnWidth = 30;

        protected AnimatorTransitionInspectorBase()
        {
        }

        private void AddConditionInList(ReorderableList list)
        {
            AnimatorTransitionBase targetObject = this.m_SerializedTransition.targetObject as AnimatorTransitionBase;
            string parameter = "";
            AnimatorConditionMode greater = AnimatorConditionMode.Greater;
            if (this.m_Controller != null)
            {
                UnityEngine.AnimatorControllerParameter[] parameters = this.m_Controller.parameters;
                if (parameters.Length > 0)
                {
                    parameter = parameters[0].name;
                    greater = ((parameters[0].type != UnityEngine.AnimatorControllerParameterType.Float) && (parameters[0].type != UnityEngine.AnimatorControllerParameterType.Int)) ? AnimatorConditionMode.If : AnimatorConditionMode.Greater;
                }
            }
            targetObject.AddCondition(greater, 0f, parameter);
        }

        protected void ComputeTransitionContexts()
        {
            this.m_TransitionContexts = new TransitionEditionContext[base.targets.Length];
            UnityEditor.Graphs.AnimationStateMachine.Graph graph = (AnimatorControllerTool.tool == null) ? null : AnimatorControllerTool.tool.stateMachineGraph;
            UnityEditor.Graphs.AnimationStateMachine.GraphGUI hgui = (AnimatorControllerTool.tool == null) ? null : AnimatorControllerTool.tool.stateMachineGraphGUI;
            for (int i = 0; i < base.targets.Length; i++)
            {
                AnimatorTransitionBase aTransition = base.targets[i] as AnimatorTransitionBase;
                this.m_TransitionContexts[i] = new TransitionEditionContext(aTransition, null, null, null);
                if ((graph != null) && (hgui != null))
                {
                    foreach (int num2 in hgui.edgeGUI.edgeSelection)
                    {
                        EdgeInfo edgeInfo = graph.GetEdgeInfo(graph.edges[num2]);
                        foreach (TransitionEditionContext context in edgeInfo.transitions)
                        {
                            if (context.transition == aTransition)
                            {
                                this.m_TransitionContexts[i] = context;
                            }
                        }
                    }
                }
            }
        }

        protected virtual void ControllerDirty()
        {
        }

        protected virtual void DoErrorAndWarning()
        {
        }

        protected virtual void DoPreview()
        {
        }

        private void DrawConditionsElement(Rect rect, int index, bool selected, bool focused)
        {
            SerializedProperty arrayElementAtIndex = this.m_Conditions.GetArrayElementAtIndex(index);
            AnimatorConditionMode intValue = (AnimatorConditionMode) arrayElementAtIndex.FindPropertyRelative("m_ConditionMode").intValue;
            int num = 3;
            Rect rect2 = new Rect(rect.x, rect.y + 2f, rect.width, rect.height - 5f);
            Rect position = rect2;
            position.xMax -= (rect2.width / 2f) + num;
            Rect rect4 = rect2;
            rect4.xMin += (rect2.width / 2f) + num;
            Rect rect5 = rect4;
            rect5.xMax -= (rect4.width / 2f) + num;
            Rect rect6 = rect4;
            rect6.xMin += (rect4.width / 2f) + num;
            string stringValue = arrayElementAtIndex.FindPropertyRelative("m_ConditionEvent").stringValue;
            int num2 = (this.m_Controller == null) ? -1 : this.m_Controller.IndexOfParameter(stringValue);
            bool flag = false;
            List<string> list = new List<string>();
            UnityEngine.AnimatorControllerParameter[] parameters = null;
            if (this.m_Controller != null)
            {
                parameters = this.m_Controller.parameters;
                for (int i = 0; i < parameters.Length; i++)
                {
                    list.Add(parameters[i].name);
                }
            }
            string name = EditorGUI.DelayedTextFieldDropDown(position, stringValue, list.ToArray());
            if (stringValue != name)
            {
                num2 = this.m_Controller.IndexOfParameter(name);
                arrayElementAtIndex.FindPropertyRelative("m_ConditionEvent").stringValue = name;
                intValue = AnimatorConditionMode.Greater;
                arrayElementAtIndex.FindPropertyRelative("m_ConditionMode").intValue = (int) intValue;
                flag = true;
            }
            UnityEngine.AnimatorControllerParameterType type = (num2 == -1) ? ((UnityEngine.AnimatorControllerParameterType) (-1)) : parameters[num2].type;
            if ((num2 != -1) && ((type == UnityEngine.AnimatorControllerParameterType.Float) || (type == UnityEngine.AnimatorControllerParameterType.Int)))
            {
                List<AnimatorConditionMode> list2 = (type != UnityEngine.AnimatorControllerParameterType.Float) ? m_intModes : m_floatModes;
                string[] displayedOptions = new string[list2.Count];
                for (int j = 0; j < displayedOptions.Length; j++)
                {
                    displayedOptions[j] = list2[j].ToString();
                }
                int selectedIndex = -1;
                for (int k = 0; k < displayedOptions.Length; k++)
                {
                    if (intValue.ToString() == displayedOptions[k])
                    {
                        selectedIndex = k;
                    }
                }
                if (selectedIndex == -1)
                {
                    Vector2 vector = GUI.skin.label.CalcSize(s_Styles.errorIcon);
                    Rect rect7 = rect5;
                    rect7.xMax = rect7.xMin + vector.x;
                    rect5.xMin += vector.x;
                    GUI.Label(rect7, s_Styles.errorIcon);
                }
                EditorGUI.BeginChangeCheck();
                selectedIndex = EditorGUI.Popup(rect5, selectedIndex, displayedOptions);
                if (EditorGUI.EndChangeCheck() || flag)
                {
                    arrayElementAtIndex.FindPropertyRelative("m_ConditionMode").intValue = list2[selectedIndex];
                }
                EditorGUI.BeginChangeCheck();
                float floatValue = arrayElementAtIndex.FindPropertyRelative("m_EventTreshold").floatValue;
                if (type == UnityEngine.AnimatorControllerParameterType.Float)
                {
                    floatValue = EditorGUI.FloatField(rect6, floatValue);
                }
                else
                {
                    floatValue = EditorGUI.IntField(rect6, Mathf.FloorToInt(floatValue));
                }
                if (EditorGUI.EndChangeCheck() || flag)
                {
                    arrayElementAtIndex.FindPropertyRelative("m_EventTreshold").floatValue = floatValue;
                }
            }
            else if ((num2 != -1) && (type == UnityEngine.AnimatorControllerParameterType.Bool))
            {
                string[] strArray2 = new string[] { "true", "false" };
                int num8 = (intValue != AnimatorConditionMode.IfNot) ? 0 : 1;
                EditorGUI.BeginChangeCheck();
                num8 = EditorGUI.Popup(rect4, num8, strArray2);
                if (EditorGUI.EndChangeCheck() || flag)
                {
                    arrayElementAtIndex.FindPropertyRelative("m_ConditionMode").intValue = (num8 != 0) ? 2 : 1;
                }
            }
            else if (type == UnityEngine.AnimatorControllerParameterType.Trigger)
            {
                if (flag)
                {
                    arrayElementAtIndex.FindPropertyRelative("m_ConditionMode").intValue = 1;
                }
            }
            else
            {
                EditorGUI.LabelField(rect4, "Parameter does not exist in Controller");
            }
        }

        private void DrawConditionsHeader(Rect headerRect)
        {
            GUI.Label(headerRect, EditorGUIUtility.TempContent("Conditions"));
        }

        internal override void DrawHeaderHelpAndSettingsGUI(Rect r)
        {
            if (!this.m_SingleTransitionHeader)
            {
                base.DrawHeaderHelpAndSettingsGUI(r);
            }
            else
            {
                if (Help.HasHelpForObject(this.m_SerializedTransition.targetObject) && GUI.Button(new Rect(r.width - 36f, r.y + 5f, 14f, 14f), EditorGUI.GUIContents.helpIcon, EditorStyles.inspectorTitlebarText))
                {
                    Help.ShowHelpForObject(this.m_SerializedTransition.targetObject);
                }
                Rect position = new Rect(r.width - 18f, r.y + 5f, 14f, 14f);
                if (EditorGUI.DropdownButton(position, EditorGUI.GUIContents.titleSettingsIcon, FocusType.Passive, EditorStyles.inspectorTitlebarText))
                {
                    EditorUtility.DisplayObjectContextMenu(position, this.m_SerializedTransition.targetObject, 0);
                }
            }
        }

        public override void DrawPreview(Rect previewPosition)
        {
            this.OnInteractivePreviewGUI(previewPosition, s_Styles.preBackground);
        }

        private void DrawTransitionElement(Rect rect, int index, bool selected, bool focused)
        {
            DrawTransitionElementCommon(rect, this.m_TransitionContexts[index], selected, focused);
        }

        public static void DrawTransitionElementCommon(Rect rect, TransitionEditionContext transitionContext, bool selected, bool focused)
        {
            rect.xMax -= 60f;
            if (transitionContext.transition == null)
            {
                GUI.Label(rect, new GUIContent("Not Found"));
            }
            else
            {
                bool solo = transitionContext.transition.solo;
                bool mute = transitionContext.transition.mute;
                GUI.Label(rect, new GUIContent(transitionContext.displayName, transitionContext.fullName));
                rect.xMin = rect.xMax;
                rect.width = 30f;
                solo = GUI.Toggle(rect, solo, "");
                rect.xMin = rect.xMax;
                rect.width = 30f;
                mute = GUI.Toggle(rect, mute, "");
                if (solo != transitionContext.transition.solo)
                {
                    Undo.RegisterCompleteObjectUndo(transitionContext.transition, "Solo changed");
                    transitionContext.transition.solo = solo;
                }
                if (mute != transitionContext.transition.mute)
                {
                    Undo.RegisterCompleteObjectUndo(transitionContext.transition, "Mute changed");
                    transitionContext.transition.mute = mute;
                }
            }
        }

        public static void DrawTransitionHeaderCommon(Rect rect)
        {
            rect.xMax -= 60f;
            GUI.Label(rect, "Transitions");
            rect.xMin = rect.xMax;
            rect.width = 30f;
            GUI.Label(rect, "Solo");
            rect.xMin = rect.xMax;
            rect.width = 30f;
            GUI.Label(rect, "Mute");
        }

        public override GUIContent GetPreviewTitle() => 
            s_Styles.previewTitle;

        protected virtual void InitSerializedProperties()
        {
            this.m_Conditions = this.m_SerializedTransition.FindProperty("m_Conditions");
            this.m_Name = this.m_SerializedTransition.FindProperty("m_Name");
        }

        private static void InitStyles()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
        }

        public virtual void OnDestroy()
        {
        }

        public virtual void OnDisable()
        {
            if (this.m_Controller != null)
            {
                this.m_Controller.OnAnimatorControllerDirty = (System.Action) Delegate.Remove(this.m_Controller.OnAnimatorControllerDirty, new System.Action(this.ControllerDirty));
            }
        }

        public virtual void OnEnable()
        {
            this.m_TransitionList = new ReorderableList(base.targets, typeof(AnimatorTransitionBase), false, true, false, true);
            this.m_TransitionList.onSelectCallback = new ReorderableList.SelectCallbackDelegate(this.OnSelectTransition);
            this.m_TransitionList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.OnRemoveTransition);
            this.m_TransitionList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawTransitionElement);
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new ReorderableList.HeaderCallbackDelegate(AnimatorTransitionInspectorBase.DrawTransitionHeaderCommon);
            }
            this.m_TransitionList.drawHeaderCallback = <>f__mg$cache0;
            this.m_TransitionList.index = 0;
            this.m_PreviewObject = AnimatorControllerTool.tool?.previewAnimator;
            this.m_Controller = AnimatorControllerTool.tool?.animatorController;
            if (this.m_Controller != null)
            {
                this.m_LayerIndex = AnimatorControllerTool.tool.selectedLayerIndex;
                this.m_Controller.OnAnimatorControllerDirty = (System.Action) Delegate.Combine(this.m_Controller.OnAnimatorControllerDirty, new System.Action(this.ControllerDirty));
            }
            if (m_intModes == null)
            {
                m_intModes = new List<AnimatorConditionMode>();
                m_intModes.Add(AnimatorConditionMode.Greater);
                m_intModes.Add(AnimatorConditionMode.Less);
                m_intModes.Add(AnimatorConditionMode.Equals);
                m_intModes.Add(AnimatorConditionMode.NotEqual);
            }
            if (m_floatModes == null)
            {
                m_floatModes = new List<AnimatorConditionMode>();
                m_floatModes.Add(AnimatorConditionMode.Greater);
                m_floatModes.Add(AnimatorConditionMode.Less);
            }
            this.m_SyncTransitionContexts = true;
        }

        internal override void OnHeaderControlsGUI()
        {
            if (!this.m_SingleTransitionHeader)
            {
                base.OnHeaderControlsGUI();
            }
            else
            {
                GUILayout.Label(this.m_TransitionContexts[this.m_TransitionList.index].displayName, new GUILayoutOption[0]);
            }
        }

        internal override void OnHeaderIconGUI(Rect iconRect)
        {
            Texture2D miniThumbnail = AssetPreview.GetMiniThumbnail(base.target);
            GUI.Label(iconRect, miniThumbnail);
        }

        internal override void OnHeaderTitleGUI(Rect titleRect, string header)
        {
            this.SyncTransitionContexts();
            if (!this.m_SingleTransitionHeader)
            {
                Rect position = titleRect;
                position.height = 16f;
                EditorGUI.LabelField(position, this.m_TransitionContexts[this.m_TransitionList.index].displayName);
                position.y += 18f;
                EditorGUI.LabelField(position, base.targets.Length + " " + ((base.targets.Length != 1) ? "Transitions" : "AnimatorTransitionBase"));
            }
            else
            {
                Rect rect2 = titleRect;
                rect2.height = 16f;
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = this.m_Name.hasMultipleDifferentValues;
                string name = EditorGUI.DelayedTextField(rect2, this.m_Name.stringValue, EditorStyles.textField);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    ObjectNames.SetNameSmart(this.m_SerializedTransition.targetObject, name);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            this.SyncTransitionContexts();
            InitStyles();
            if (this.showTransitionList)
            {
                this.m_TransitionList.DoLayoutList();
            }
            if (this.m_SerializedTransition != null)
            {
                this.m_SerializedTransition.Update();
                this.m_SingleTransitionHeader = true;
                Editor.DrawHeaderGUI(this, "");
                this.m_SingleTransitionHeader = false;
                this.DoPreview();
                EditorGUI.indentLevel = 0;
                GUILayout.Space(10f);
                if (this.m_ConditionList != null)
                {
                    this.m_ConditionList.DoLayoutList();
                }
                this.m_SerializedTransition.ApplyModifiedProperties();
                this.DoErrorAndWarning();
            }
        }

        protected void OnRemoveTransition(ReorderableList list)
        {
            int index = list.index;
            if (list.index >= (list.list.Count - 1))
            {
                list.index = list.list.Count - 1;
            }
            this.m_TransitionContexts[index].Remove(true);
            AnimatorControllerTool.tool.RebuildGraph();
            GUIUtility.ExitGUI();
        }

        private void OnSelectTransition(ReorderableList list)
        {
            this.SetTransitionToInspect(base.targets[list.index] as AnimatorTransitionBase);
        }

        public void SetTransitionContext(TransitionEditionContext context)
        {
            this.m_TransitionContexts = new TransitionEditionContext[] { context };
            this.m_SerializedTransition = null;
            this.SetTransitionToInspect(base.targets[0] as AnimatorTransitionBase);
            this.m_SyncTransitionContexts = false;
        }

        protected virtual void SetTransitionToInspect(AnimatorTransitionBase transition)
        {
            if (((this.m_SerializedTransition == null) || (this.m_SerializedTransition.targetObject != transition)) && (transition != null))
            {
                this.m_SerializedTransition = new SerializedObject(transition);
                if (this.m_SerializedTransition != null)
                {
                    this.InitSerializedProperties();
                    this.m_ConditionList = new ReorderableList(this.m_SerializedTransition, this.m_Conditions);
                    this.m_ConditionList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawConditionsElement);
                    this.m_ConditionList.onAddCallback = new ReorderableList.AddCallbackDelegate(this.AddConditionInList);
                    this.m_ConditionList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.DrawConditionsHeader);
                }
            }
        }

        protected void SyncTransitionContexts()
        {
            if (this.m_SyncTransitionContexts)
            {
                this.ComputeTransitionContexts();
                this.SetTransitionToInspect(base.targets[0] as AnimatorTransitionBase);
                this.m_SyncTransitionContexts = false;
            }
        }

        private class Styles
        {
            public readonly GUIStyle background = new GUIStyle("IN Label");
            public readonly GUIStyle boxBackground = "TE NodeBackground";
            public readonly GUIStyle draggingHandle = "WindowBottomResize";
            public GUIContent errorIcon = EditorGUIUtility.IconContent("console.erroricon.sml");
            public readonly GUIStyle footerBackground = "preLabel";
            public readonly GUIStyle headerBackground = "TE Toolbar";
            public GUIContent iconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus", "Remove selection from list");
            public GUIContent iconToolbarPlus = EditorGUIUtility.IconContent("Toolbar Plus", "Add to list");
            public GUIStyle preBackground = "preBackground";
            public readonly GUIStyle preButton = "preButton";
            public GUIContent previewTitle = new GUIContent("Preview");
        }
    }
}

