namespace UnityEditor.Graphs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEditorInternal;
    using UnityEngine;

    [Serializable]
    internal class LayerControllerView : IAnimatorControllerSubEditor
    {
        private const float kElementHeight = 40f;
        private const int kSliderThickness = 1;
        private const float kSpacing = 4f;
        private bool m_HadKeyFocusAtMouseDown = false;
        private IAnimatorControllerEditor m_Host = null;
        private int m_LastSelectedIndex;
        private ReorderableList m_LayerList;
        private Vector2 m_LayerScroll;
        private RenameOverlay m_RenameOverlay;
        [SerializeField]
        private int m_SelectedLayerIndex = 0;
        private static Styles s_Styles;

        private void DeleteLayer()
        {
            this.OnRemoveLayer(this.m_LayerList.index);
        }

        public void GrabKeyboardFocus()
        {
            this.m_LayerList.GrabKeyboardFocus();
        }

        public bool HasKeyboardControl()
        {
            return this.m_LayerList.HasKeyboardControl();
        }

        public void Init(IAnimatorControllerEditor host)
        {
            this.m_Host = host;
            if (this.m_LayerList == null)
            {
                this.m_LayerList = new ReorderableList((this.m_Host.animatorController == null) ? ((IList) new UnityEditor.Animations.AnimatorControllerLayer[0]) : ((IList) this.m_Host.animatorController.layers), typeof(UnityEditor.Animations.AnimatorControllerLayer), true, false, false, false);
                this.m_LayerList.onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.OnReorderLayer);
                this.m_LayerList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.OnDrawLayer);
                this.m_LayerList.drawElementBackgroundCallback = new ReorderableList.ElementCallbackDelegate(this.OnDrawLayerBackground);
                this.m_LayerList.onMouseUpCallback = new ReorderableList.SelectCallbackDelegate(this.OnMouseUpLayer);
                this.m_LayerList.onSelectCallback = new ReorderableList.SelectCallbackDelegate(this.OnSelectLayer);
                this.m_LayerList.index = this.selectedLayerIndex;
                this.m_LayerList.headerHeight = 0f;
                this.m_LayerList.footerHeight = 0f;
                this.m_LayerList.elementHeight = 40f;
            }
        }

        private bool IsBlendTreeUsingHumanoid(UnityEditor.Animations.BlendTree blendTree)
        {
            ChildMotion[] children = blendTree.children;
            for (int i = 0; i < children.Length; i++)
            {
                Motion motion = children[i].motion;
                if (motion is AnimationClip)
                {
                    AnimationClip clip = motion as AnimationClip;
                    if (clip.humanMotion)
                    {
                        return true;
                    }
                }
                else if ((motion is UnityEditor.Animations.BlendTree) && this.IsBlendTreeUsingHumanoid(motion as UnityEditor.Animations.BlendTree))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsLayerUsingHumanoid(UnityEditor.Animations.AnimatorControllerLayer layer)
        {
            ChildAnimatorState[] states = layer.stateMachine.states;
            for (int i = 0; i < states.Length; i++)
            {
                AnimatorState state = states[i].state;
                if (state.motion is AnimationClip)
                {
                    AnimationClip motion = state.motion as AnimationClip;
                    if (motion.humanMotion)
                    {
                        return true;
                    }
                }
                else if ((state.motion is UnityEditor.Animations.BlendTree) && this.IsBlendTreeUsingHumanoid(state.motion as UnityEditor.Animations.BlendTree))
                {
                    return true;
                }
            }
            return false;
        }

        private void KeyboardHandling()
        {
            if (this.m_LayerList.HasKeyboardControl())
            {
                Event current = Event.current;
                switch (current.type)
                {
                    case EventType.ExecuteCommand:
                        if ((current.commandName == "SoftDelete") || (current.commandName == "Delete"))
                        {
                            current.Use();
                            this.OnRemoveLayer(this.m_LayerList.index);
                        }
                        break;

                    case EventType.KeyDown:
                    {
                        KeyCode keyCode = Event.current.keyCode;
                        if (keyCode != KeyCode.Home)
                        {
                            if (keyCode == KeyCode.End)
                            {
                                current.Use();
                                this.m_LayerList.index = this.m_SelectedLayerIndex = this.m_LayerList.count - 1;
                                this.m_Host.ResetUI();
                                break;
                            }
                            if (keyCode == KeyCode.Delete)
                            {
                                current.Use();
                                this.OnRemoveLayer(this.m_LayerList.index);
                                break;
                            }
                        }
                        else
                        {
                            current.Use();
                            this.m_LayerList.index = this.m_SelectedLayerIndex = 0;
                            this.m_Host.ResetUI();
                        }
                        break;
                    }
                }
            }
        }

        public void OnDestroy()
        {
        }

        public void OnDisable()
        {
            if (this.renameOverlay.IsRenaming())
            {
                this.RenameEnd();
            }
        }

        private void OnDrawLayer(Rect rect, int index, bool selected, bool focused)
        {
            Event current = Event.current;
            if (((current.type == EventType.MouseUp) && (current.button == 1)) && rect.Contains(current.mousePosition))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Delete"), false, new GenericMenu.MenuFunction(this.DeleteLayer));
                menu.ShowAsContext();
                Event.current.Use();
            }
            UnityEditor.Animations.AnimatorControllerLayer layer = this.m_LayerList.list[index] as UnityEditor.Animations.AnimatorControllerLayer;
            rect.yMin += 4f;
            rect.yMax -= 4f;
            Vector2 vector = s_Styles.invisibleButton.CalcSize(s_Styles.settingsIcon);
            Rect rect2 = new Rect((rect.xMax - vector.x) - 4f, rect.yMin + 1f, vector.x, rect.height - 16f);
            Rect position = rect2;
            if (GUI.Button(position, s_Styles.settingsIcon, s_Styles.invisibleButton))
            {
                Rect buttonRect = position;
                buttonRect.x += 15f;
                if (LayerSettingsWindow.ShowAtPosition(buttonRect, layer, index, this.m_Host.animatorController))
                {
                    GUIUtility.ExitGUI();
                }
            }
            if (layer.syncedLayerIndex != -1)
            {
                Vector2 vector2 = s_Styles.layerLabel.CalcSize(!layer.syncedLayerAffectsTiming ? s_Styles.sync : s_Styles.syncTime);
                position = new Rect((position.xMin - vector2.x) - 4f, rect.yMin, vector2.x, rect.height - 16f);
                GUI.Label(position, !layer.syncedLayerAffectsTiming ? s_Styles.sync : s_Styles.syncTime, s_Styles.layerLabel);
            }
            if (layer.iKPass)
            {
                Vector2 vector3 = s_Styles.layerLabel.CalcSize(s_Styles.ik);
                position = new Rect((position.xMin - vector3.x) - 4f, rect.yMin, vector3.x, rect.height - 16f);
                GUI.Label(position, s_Styles.ik, s_Styles.layerLabel);
            }
            if (layer.blendingMode == UnityEditor.Animations.AnimatorLayerBlendingMode.Additive)
            {
                Vector2 vector4 = s_Styles.layerLabel.CalcSize(s_Styles.additive);
                position = new Rect((position.xMin - vector4.x) - 4f, rect.yMin, vector4.x, rect.height - 16f);
                GUI.Label(position, s_Styles.additive, s_Styles.layerLabel);
            }
            if (layer.avatarMask != null)
            {
                Vector2 vector5 = s_Styles.layerLabel.CalcSize(s_Styles.mask);
                position = new Rect((position.xMin - vector5.x) - 4f, rect.yMin, vector5.x, rect.height - 16f);
                GUI.Label(position, s_Styles.mask, s_Styles.layerLabel);
                if ((index == 0) && this.IsLayerUsingHumanoid(layer))
                {
                    Vector2 vector6 = s_Styles.invisibleButton.CalcSize(s_Styles.maskWarningIcon);
                    float num = (position.height - vector6.y) * 0.5f;
                    position = new Rect((position.xMin - vector6.x) - 4f, rect.yMin + num, vector6.x, vector6.y);
                    GUI.Label(position, s_Styles.maskWarningIcon, s_Styles.invisibleButton);
                }
            }
            Rect rect5 = Rect.MinMaxRect(rect.xMin, rect.yMin, position.xMin - 4f, rect.yMax - 16f);
            float left = s_Styles.label.padding.left;
            Rect rect6 = Rect.MinMaxRect(rect.xMin + left, rect.yMax - 11f, rect2.xMax, rect.yMax - 9f);
            if ((this.renameOverlay.IsRenaming() && (this.renameOverlay.userData == index)) && !this.renameOverlay.isWaitingForDelay)
            {
                if ((rect5.width >= 0f) && (rect5.height >= 0f))
                {
                    rect5.x -= 2f;
                    this.renameOverlay.editFieldRect = rect5;
                }
                if (!this.renameOverlay.OnGUI())
                {
                    this.RenameEnd();
                }
            }
            else
            {
                GUI.Label(rect5, layer.name, s_Styles.label);
            }
            if (Event.current.type == EventType.Repaint)
            {
                float num3 = (index != 0) ? (!this.m_Host.liveLink ? layer.defaultWeight : this.m_Host.previewAnimator.GetLayerWeight(index)) : 1f;
                Rect rect7 = rect6;
                rect7.width *= num3;
                EditorGUI.DrawRect(rect6, s_Styles.progressBackground);
                EditorGUI.DrawRect(rect7, !this.m_Host.liveLink ? s_Styles.progressEdit : s_Styles.progressLiveLink);
            }
        }

        private void OnDrawLayerBackground(Rect rect, int index, bool selected, bool focused)
        {
            if (Event.current.type == EventType.Repaint)
            {
                GUI.Box(Rect.MinMaxRect(rect.xMin + 1f, rect.yMin, rect.xMax - 3f, rect.yMax), "");
                s_Styles.elementBackground.Draw(rect, false, selected, selected, focused);
            }
        }

        public void OnEnable()
        {
            this.ResetUI();
        }

        public void OnEvent()
        {
            this.renameOverlay.OnEvent();
        }

        public void OnFocus()
        {
        }

        public void OnGUI(Rect rect)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            this.KeyboardHandling();
            if (this.m_Host.animatorController != null)
            {
                this.m_LayerList.list = this.m_Host.animatorController.layers;
            }
            else if (this.m_LayerList.list.Count != 0)
            {
                this.m_LayerList.list = new UnityEditor.Animations.AnimatorControllerLayer[0];
            }
            Event current = Event.current;
            if ((current.type == EventType.MouseDown) && rect.Contains(current.mousePosition))
            {
                this.m_HadKeyFocusAtMouseDown = this.m_LayerList.HasKeyboardControl();
            }
            this.m_LayerList.draggable = !this.m_Host.liveLink;
            this.m_LayerScroll = GUILayout.BeginScrollView(this.m_LayerScroll, new GUILayoutOption[0]);
            this.m_LayerList.DoLayoutList();
            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
        }

        public void OnLostFocus()
        {
            if (this.renameOverlay.IsRenaming())
            {
                this.renameOverlay.EndRename(true);
                this.RenameEnd();
            }
        }

        private void OnMouseUpLayer(ReorderableList list)
        {
            if ((this.m_HadKeyFocusAtMouseDown && (list.index == this.m_LastSelectedIndex)) && (Event.current.button == 0))
            {
                UnityEditor.Animations.AnimatorControllerLayer layer = list.list[list.index] as UnityEditor.Animations.AnimatorControllerLayer;
                this.renameOverlay.BeginRename(layer.name, list.index, 0.1f);
            }
            else if ((AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine != null) && !Selection.Contains(AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine))
            {
                Selection.objects = new List<UnityEngine.Object> { AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine }.ToArray();
            }
            this.m_LastSelectedIndex = list.index;
        }

        protected void OnRemoveLayer(int layerIndex)
        {
            int count = this.m_LayerList.list.Count;
            List<int> layerIndexes = new List<int>();
            for (int i = count - 1; i >= 0; i--)
            {
                UnityEditor.Animations.AnimatorControllerLayer layer = this.m_LayerList.list[i] as UnityEditor.Animations.AnimatorControllerLayer;
                if ((layer.syncedLayerIndex == layerIndex) || (i == layerIndex))
                {
                    layerIndexes.Add(i);
                }
            }
            if (layerIndexes.Count > 0)
            {
                if (layerIndexes.Count == count)
                {
                    if (layerIndexes.Count == 1)
                    {
                        Debug.LogError("You cannot remove all layers from an AnimatorController.");
                    }
                    else if (layerIndexes.Count > 1)
                    {
                        Debug.LogError("Deleting this layer will also delete all the layers that are synchronized on it. This operation cannot be performed because it would remove all the layers on this controller.");
                    }
                }
                else if ((layerIndexes.Count == 1) || EditorUtility.DisplayDialog("Deleting synchronized layer", "Deleting this layer will also delete all the layers that are synchronized on it. Are you sure you want to delete it?", "Delete", "Cancel"))
                {
                    UnityEditor.Animations.AnimatorControllerLayer[] layers = this.m_Host.animatorController.layers;
                    this.m_Host.animatorController.RemoveLayers(layerIndexes);
                    UnityEditor.Animations.AnimatorControllerLayer[] newLayers = this.m_Host.animatorController.layers;
                    this.RemapIndices(layers, newLayers);
                    this.m_Host.animatorController.layers = newLayers;
                    this.ResetUI();
                    this.m_Host.ResetUI();
                }
            }
        }

        private void OnReorderLayer(ReorderableList reorderablelist)
        {
            <OnReorderLayer>c__AnonStorey0 storey = new <OnReorderLayer>c__AnonStorey0();
            UnityEditor.Animations.AnimatorControllerLayer[] layers = this.m_Host.animatorController.layers;
            UnityEditor.Animations.AnimatorControllerLayer[] list = reorderablelist.list as UnityEditor.Animations.AnimatorControllerLayer[];
            this.RemapIndices(layers, list);
            storey.layerName = layers[this.selectedLayerIndex].name;
            int num = Array.FindIndex<UnityEditor.Animations.AnimatorControllerLayer>(list, new Predicate<UnityEditor.Animations.AnimatorControllerLayer>(storey.<>m__0));
            if (num != -1)
            {
                this.selectedLayerIndex = num;
            }
            Undo.RegisterCompleteObjectUndo(this.m_Host.animatorController, "Layer reordering");
            this.m_Host.animatorController.layers = list;
            this.m_Host.Repaint();
        }

        private void OnSelectLayer(ReorderableList list)
        {
            if (this.selectedLayerIndex != list.index)
            {
                this.selectedLayerIndex = list.index;
                this.m_Host.ResetUI();
                Selection.objects = new List<UnityEngine.Object> { AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine }.ToArray();
            }
        }

        public void OnToolbarGUI()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            using (new EditorGUI.DisabledScope(this.m_Host.animatorController == null))
            {
                if (GUILayout.Button(s_Styles.addIcon, s_Styles.invisibleButton, new GUILayoutOption[0]))
                {
                    AnimatorControllerTool host = this.m_Host as AnimatorControllerTool;
                    if ((host != null) && (host.animatorController != null))
                    {
                        host.AddNewLayer();
                        this.m_LayerList.list = host.animatorController.layers;
                        this.m_LayerList.index = host.selectedLayerIndex;
                        this.selectedLayerIndex = this.m_LayerList.index;
                        if (this.renameOverlay.IsRenaming())
                        {
                            this.RenameEnd();
                        }
                        this.renameOverlay.BeginRename(host.animatorController.layers[this.selectedLayerIndex].name, this.selectedLayerIndex, 0.1f);
                    }
                }
            }
        }

        public void ReleaseKeyboardFocus()
        {
            this.m_LayerList.ReleaseKeyboardFocus();
        }

        private void RemapIndices(UnityEditor.Animations.AnimatorControllerLayer[] originalLayers, UnityEditor.Animations.AnimatorControllerLayer[] newLayers)
        {
            for (int i = 0; i < newLayers.Length; i++)
            {
                int syncedLayerIndex = newLayers[i].syncedLayerIndex;
                if (syncedLayerIndex != -1)
                {
                    for (int j = 0; j < newLayers.Length; j++)
                    {
                        if (originalLayers[syncedLayerIndex].name == newLayers[j].name)
                        {
                            newLayers[i].syncedLayerIndex = j;
                        }
                    }
                }
            }
        }

        private void RenameEnd()
        {
            if (this.renameOverlay.userAcceptedRename)
            {
                string name = !string.IsNullOrEmpty(this.renameOverlay.name) ? this.renameOverlay.name : this.renameOverlay.originalName;
                if (name != this.renameOverlay.originalName)
                {
                    int userData = this.renameOverlay.userData;
                    UnityEditor.Animations.AnimatorControllerLayer layer = this.m_LayerList.list[userData] as UnityEditor.Animations.AnimatorControllerLayer;
                    AnimatorStateMachine stateMachine = layer.stateMachine;
                    name = this.m_Host.animatorController.MakeUniqueLayerName(name);
                    if (stateMachine != null)
                    {
                        ObjectNames.SetNameSmart(stateMachine, name);
                    }
                    Undo.RegisterCompleteObjectUndo(this.m_Host.animatorController, "Layer renamed");
                    layer.name = name;
                    this.m_Host.animatorController.layers = this.m_LayerList.list as UnityEditor.Animations.AnimatorControllerLayer[];
                }
            }
            this.m_LayerList.GrabKeyboardFocus();
            this.renameOverlay.Clear();
        }

        public void ResetUI()
        {
            if (((AnimatorControllerTool.tool == null) || (AnimatorControllerTool.tool.animatorController == null)) || (AnimatorControllerTool.tool.animatorController.layers == null))
            {
                this.m_SelectedLayerIndex = 0;
                this.m_LastSelectedIndex = -1;
                this.m_LayerList.index = this.selectedLayerIndex;
            }
            else
            {
                if (this.selectedLayerIndex > AnimatorControllerTool.tool.animatorController.layers.Length)
                {
                    this.m_SelectedLayerIndex = AnimatorControllerTool.tool.animatorController.layers.Length - 1;
                }
                if (this.m_LastSelectedIndex > AnimatorControllerTool.tool.animatorController.layers.Length)
                {
                    this.m_LastSelectedIndex = AnimatorControllerTool.tool.animatorController.layers.Length - 1;
                }
                this.m_LayerList.index = this.m_SelectedLayerIndex;
                this.m_LayerScroll = Vector2.zero;
            }
        }

        public RenameOverlay renameOverlay
        {
            get
            {
                if (this.m_RenameOverlay == null)
                {
                    this.m_RenameOverlay = new RenameOverlay();
                }
                return this.m_RenameOverlay;
            }
        }

        public int selectedLayerIndex
        {
            get
            {
                if ((this.m_Host.animatorController != null) && (this.m_SelectedLayerIndex >= this.m_Host.animatorController.layers.Length))
                {
                    this.m_SelectedLayerIndex = this.m_Host.animatorController.layers.Length - 1;
                    this.m_LayerList.index = this.m_SelectedLayerIndex;
                    this.m_Host.ResetUI();
                }
                return this.m_SelectedLayerIndex;
            }
            set
            {
                this.m_SelectedLayerIndex = value;
                this.m_LayerList.index = this.m_SelectedLayerIndex;
                this.m_Host.ResetUI();
            }
        }

        [CompilerGenerated]
        private sealed class <OnReorderLayer>c__AnonStorey0
        {
            internal string layerName;

            internal bool <>m__0(UnityEditor.Animations.AnimatorControllerLayer layer)
            {
                return (layer.name == this.layerName);
            }
        }

        private class Styles
        {
            public readonly GUIContent addIcon = EditorGUIUtility.IconContent("Toolbar Plus");
            public readonly GUIContent additive = EditorGUIUtility.TextContent("A|Additive Layer.");
            public readonly GUIStyle elementBackground = "RL Element";
            public readonly GUIContent ik = EditorGUIUtility.TextContent("IK|When active, the layer will have an IK pass when evaluated. It will trigger an OnAnimatorIK callback.");
            public readonly GUIStyle invisibleButton = "InvisibleButton";
            public readonly GUIStyle label = "label";
            public readonly GUIStyle layerLabel = new GUIStyle("miniLabel");
            public readonly GUIContent mask = EditorGUIUtility.TextContent("M|Layer has an AvatarMask.");
            public readonly GUIContent maskWarning = EditorGUIUtility.TextContent("M|Character orientation may be erroneous for humanoid motion if there is an AvatarMask on the default layer.  Consider not using a mask for the default layer.");
            public readonly GUIContent maskWarningIcon = EditorGUIUtility.IconContent("console.warnicon.sml");
            public readonly UnityEngine.Color progressBackground = new UnityEngine.Color(0.24f, 0.24f, 0.24f);
            public readonly UnityEngine.Color progressEdit = new UnityEngine.Color(0.5450981f, 0.5450981f, 0.5450981f);
            public readonly UnityEngine.Color progressLiveLink = new UnityEngine.Color(0.2980392f, 0.6980392f, 1f);
            public readonly GUIContent settings = EditorGUIUtility.TextContent("Settings|Click to change layer settings.");
            public readonly GUIContent settingsIcon = EditorGUIUtility.IconContent("SettingsIcon");
            public readonly GUIContent sync = EditorGUIUtility.TextContent("S|Layer is a Synchronized layer.");
            public readonly GUIContent syncTime = EditorGUIUtility.TextContent("S+T|Layer is a Synchronized layer and will take control of the duration for this Synced Layer.");

            public Styles()
            {
                this.settingsIcon.tooltip = this.settings.tooltip;
                this.maskWarningIcon.tooltip = this.maskWarning.tooltip;
                this.layerLabel.alignment = TextAnchor.MiddleCenter;
                this.layerLabel.fontStyle = FontStyle.Bold;
                this.layerLabel.padding = new RectOffset(0, 0, 0, 0);
                this.layerLabel.margin = new RectOffset(0, 0, 0, 0);
            }
        }
    }
}

