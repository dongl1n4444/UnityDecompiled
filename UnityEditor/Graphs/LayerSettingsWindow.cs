namespace UnityEditor.Graphs
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEngine;

    [InitializeOnLoad]
    internal class LayerSettingsWindow : EditorWindow
    {
        private AnimatorController m_Controller = null;
        private AnimatorControllerLayer m_Layer = null;
        private int m_LayerIndex = 0;
        private static long s_LastClosedTime;
        public static LayerSettingsWindow s_LayerSettingsWindow;
        private static Styles s_Styles = null;

        private void Init(Rect buttonRect)
        {
            buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
            PopupLocationHelper.PopupLocation[] locationPriorityOrder = new PopupLocationHelper.PopupLocation[] { PopupLocationHelper.PopupLocation.Right };
            base.ShowAsDropDown(buttonRect, this.windowSize, locationPriorityOrder);
            base.Focus();
            base.m_Parent.AddToAuxWindowList();
            base.wantsMouseMove = true;
        }

        private void OnDisable()
        {
            s_LayerSettingsWindow = null;
            s_LastClosedTime = DateTime.Now.Ticks / 0x2710L;
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }

        private void OnEditorUpdate()
        {
        }

        private void OnEnable()
        {
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }

        internal void OnGUI()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            if (this.m_Layer != null)
            {
                bool flag = false;
                GUI.Box(new Rect(0f, 0f, base.position.width, base.position.height), GUIContent.none, new GUIStyle("grey_border"));
                AnimatorControllerLayer[] layers = this.m_Controller.layers;
                EditorGUIUtility.labelWidth = 100f;
                using (new EditorGUI.DisabledScope(this.m_LayerIndex == 0))
                {
                    if (AnimatorControllerTool.tool.liveLink)
                    {
                        float num = (this.m_LayerIndex != 0) ? AnimatorControllerTool.tool.previewAnimator.GetLayerWeight(this.m_LayerIndex) : 1f;
                        AnimatorControllerTool.tool.previewAnimator.SetLayerWeight(this.m_LayerIndex, EditorGUILayout.Slider(s_Styles.weight, num, 0f, 1f, new GUILayoutOption[0]));
                    }
                    else
                    {
                        EditorGUI.BeginChangeCheck();
                        float num2 = (this.m_LayerIndex != 0) ? this.m_Layer.defaultWeight : 1f;
                        num2 = EditorGUILayout.Slider(s_Styles.weight, num2, 0f, 1f, new GUILayoutOption[0]);
                        if (EditorGUI.EndChangeCheck() && (this.m_LayerIndex != 0))
                        {
                            this.m_Layer.defaultWeight = num2;
                            flag = true;
                        }
                    }
                }
                EditorGUI.BeginChangeCheck();
                this.m_Layer.avatarMask = EditorGUILayout.ObjectField(s_Styles.mask, this.m_Layer.avatarMask, typeof(AvatarMask), false, new GUILayoutOption[0]) as AvatarMask;
                this.m_Layer.blendingMode = (AnimatorLayerBlendingMode) EditorGUILayout.EnumPopup(s_Styles.blending, this.m_Layer.blendingMode, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    flag = true;
                }
                int selectedIndex = 0;
                List<GUIContent> list = new List<GUIContent>();
                List<int> list2 = new List<int>();
                for (int i = 0; i < layers.Length; i++)
                {
                    AnimatorControllerLayer layer = layers[i];
                    if ((this.m_LayerIndex != i) && (layer.syncedLayerIndex == -1))
                    {
                        list.Add(new GUIContent(layer.name));
                        list2.Add(i);
                        if (i == this.m_Layer.syncedLayerIndex)
                        {
                            selectedIndex = list.Count - 1;
                        }
                    }
                }
                using (new EditorGUI.DisabledScope(list.Count == 0))
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    bool flag2 = EditorGUILayout.Toggle(s_Styles.sync, this.m_Layer.syncedLayerIndex > -1, new GUILayoutOption[0]);
                    GUILayout.Space(10f);
                    GUI.enabled = flag2 && (this.m_Layer.blendingMode == AnimatorLayerBlendingMode.Override);
                    EditorGUI.BeginChangeCheck();
                    this.m_Layer.syncedLayerAffectsTiming = EditorGUILayout.Toggle(s_Styles.timing, this.m_Layer.syncedLayerAffectsTiming, new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        flag = true;
                    }
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                    if (flag2)
                    {
                        int num5 = EditorGUILayout.Popup(s_Styles.sourceLayer, selectedIndex, list.ToArray(), new GUILayoutOption[0]);
                        if ((num5 < list2.Count) && (list2[num5] != this.m_Layer.syncedLayerIndex))
                        {
                            this.m_Layer.syncedLayerIndex = list2[num5];
                            flag = true;
                        }
                    }
                    else if (this.m_Layer.syncedLayerIndex != -1)
                    {
                        this.m_Layer.syncedLayerIndex = -1;
                        flag = true;
                    }
                }
                EditorGUI.BeginChangeCheck();
                this.m_Layer.iKPass = EditorGUILayout.Toggle(s_Styles.ik, this.m_Layer.iKPass, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    flag = true;
                }
                if (flag)
                {
                    Undo.RegisterCompleteObjectUndo(this.m_Controller, "Layer settings Changed");
                    layers[this.m_LayerIndex] = this.m_Layer;
                    this.m_Controller.layers = layers;
                    this.m_Layer = this.m_Controller.layers[this.m_LayerIndex];
                    AnimatorControllerTool.tool.ResetUI();
                }
            }
        }

        internal static bool ShowAtPosition(Rect buttonRect, AnimatorControllerLayer layer, int layerIndex, AnimatorController controller)
        {
            long num = DateTime.Now.Ticks / 0x2710L;
            if (num >= (s_LastClosedTime + 50L))
            {
                Event.current.Use();
                if (s_LayerSettingsWindow == null)
                {
                    s_LayerSettingsWindow = ScriptableObject.CreateInstance<LayerSettingsWindow>();
                }
                s_LayerSettingsWindow.m_Layer = layer;
                s_LayerSettingsWindow.m_LayerIndex = layerIndex;
                s_LayerSettingsWindow.m_Controller = controller;
                s_LayerSettingsWindow.Init(buttonRect);
                return true;
            }
            return false;
        }

        private void UndoRedoPerformed()
        {
            AnimatorControllerLayer[] layers = this.m_Controller.layers;
            if (this.m_LayerIndex < layers.Length)
            {
                this.m_Layer = layers[this.m_LayerIndex];
                base.Repaint();
            }
            else
            {
                this.m_Layer = null;
                Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
                base.Close();
            }
        }

        public AnimatorControllerLayer layer
        {
            get
            {
                return this.m_Layer;
            }
        }

        public int layerIndex
        {
            get
            {
                return this.m_LayerIndex;
            }
        }

        private Vector2 windowSize
        {
            get
            {
                return new Vector2(250f, EditorGUIUtility.singleLineHeight * 7f);
            }
        }

        private class Styles
        {
            public readonly GUIContent blending = EditorGUIUtility.TextContent("Blending|Choose between Override and Additive layer.");
            public readonly GUIContent ik = EditorGUIUtility.TextContent("IK Pass|When active, the layer will have an IK pass when evaluated. It will trigger an OnAnimatorIK callback.");
            public readonly GUIContent mask = EditorGUIUtility.TextContent("Mask|The AvatarMask that is used to mask the animation on the given layer.");
            public readonly GUIContent sourceLayer = EditorGUIUtility.TextContent("Source Layer|Specifies the source of the Synced Layer.");
            public readonly GUIContent sync = EditorGUIUtility.TextContent("Sync|Synchronize this layer with another layer.");
            public readonly GUIContent timing = EditorGUIUtility.TextContent("Timing|When active, the layer will take control of the duration of the Synced Layer.");
            public readonly GUIContent weight = EditorGUIUtility.TextContent("Weight|Change layer default weight.");
        }
    }
}

