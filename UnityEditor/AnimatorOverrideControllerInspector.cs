namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.Animations;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(AnimatorOverrideController)), CanEditMultipleObjects]
    internal class AnimatorOverrideControllerInspector : Editor
    {
        private ReorderableList m_ClipList;
        private List<KeyValuePair<AnimationClip, AnimationClip>> m_Clips;
        private SerializedProperty m_Controller;
        private string m_Search;

        private void DrawClipElement(Rect rect, int index, bool selected, bool focused)
        {
            KeyValuePair<AnimationClip, AnimationClip> pair = this.m_Clips[index];
            AnimationClip key = pair.Key;
            KeyValuePair<AnimationClip, AnimationClip> pair2 = this.m_Clips[index];
            AnimationClip clip2 = pair2.Value;
            rect.xMax /= 2f;
            GUI.Label(rect, key.name, EditorStyles.label);
            rect.xMin = rect.xMax;
            rect.xMax *= 2f;
            EditorGUI.BeginChangeCheck();
            clip2 = EditorGUI.ObjectField(rect, "", clip2, typeof(AnimationClip), false) as AnimationClip;
            if (EditorGUI.EndChangeCheck())
            {
                this.m_Clips[index] = new KeyValuePair<AnimationClip, AnimationClip>(key, clip2);
            }
        }

        private void DrawClipHeader(Rect rect)
        {
            rect.xMax /= 2f;
            GUI.Label(rect, "Original", EditorStyles.label);
            rect.xMin = rect.xMax;
            rect.xMax *= 2f;
            GUI.Label(rect, "Override", EditorStyles.label);
        }

        private void FilterOverrides()
        {
            if (this.m_Search.Length != 0)
            {
                char[] separator = new char[] { ' ' };
                string[] strArray = this.m_Search.ToLower().Split(separator);
                List<KeyValuePair<AnimationClip, AnimationClip>> collection = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                List<KeyValuePair<AnimationClip, AnimationClip>> list2 = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                foreach (KeyValuePair<AnimationClip, AnimationClip> pair in this.m_Clips)
                {
                    string str = pair.Key.name.ToLower().Replace(" ", "");
                    bool flag = true;
                    bool flag2 = false;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        string str2 = strArray[i];
                        if (str.Contains(str2))
                        {
                            if ((i == 0) && str.StartsWith(str2))
                            {
                                flag2 = true;
                            }
                        }
                        else
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        if (flag2)
                        {
                            collection.Add(pair);
                        }
                        else
                        {
                            list2.Add(pair);
                        }
                    }
                }
                this.m_Clips.Clear();
                collection.Sort(new AnimationClipOverrideComparer());
                list2.Sort(new AnimationClipOverrideComparer());
                this.m_Clips.AddRange(collection);
                this.m_Clips.AddRange(list2);
            }
        }

        private void OnDisable()
        {
            AnimatorOverrideController target = base.target as AnimatorOverrideController;
            target.OnOverrideControllerDirty = (AnimatorOverrideController.OnOverrideControllerDirtyCallback) Delegate.Remove(target.OnOverrideControllerDirty, new AnimatorOverrideController.OnOverrideControllerDirtyCallback(this.Repaint));
        }

        private void OnEnable()
        {
            AnimatorOverrideController target = base.target as AnimatorOverrideController;
            this.m_Controller = base.serializedObject.FindProperty("m_Controller");
            this.m_Search = "";
            if (this.m_Clips == null)
            {
                this.m_Clips = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            }
            if (this.m_ClipList == null)
            {
                target.GetOverrides(this.m_Clips);
                this.m_Clips.Sort(new AnimationClipOverrideComparer());
                this.m_ClipList = new ReorderableList(this.m_Clips, typeof(KeyValuePair<AnimationClip, AnimationClip>), false, true, false, false);
                this.m_ClipList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawClipElement);
                this.m_ClipList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.DrawClipHeader);
                this.m_ClipList.onSelectCallback = new ReorderableList.SelectCallbackDelegate(this.SelectClip);
                this.m_ClipList.elementHeight = 16f;
            }
            target.OnOverrideControllerDirty = (AnimatorOverrideController.OnOverrideControllerDirtyCallback) Delegate.Combine(target.OnOverrideControllerDirty, new AnimatorOverrideController.OnOverrideControllerDirtyCallback(this.Repaint));
        }

        public override void OnInspectorGUI()
        {
            bool flag = base.targets.Length > 1;
            bool flag2 = false;
            base.serializedObject.UpdateIfRequiredOrScript();
            AnimatorOverrideController target = base.target as AnimatorOverrideController;
            RuntimeAnimatorController controller2 = !this.m_Controller.hasMultipleDifferentValues ? target.runtimeAnimatorController : null;
            EditorGUI.BeginChangeCheck();
            controller2 = EditorGUILayout.ObjectField("Controller", controller2, typeof(UnityEditor.Animations.AnimatorController), false, new GUILayoutOption[0]) as RuntimeAnimatorController;
            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < base.targets.Length; i++)
                {
                    AnimatorOverrideController controller3 = base.targets[i] as AnimatorOverrideController;
                    controller3.runtimeAnimatorController = controller2;
                }
                flag2 = true;
            }
            GUI.SetNextControlName("OverridesSearch");
            if (((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Escape)) && (GUI.GetNameOfFocusedControl() == "OverridesSearch"))
            {
                this.m_Search = "";
            }
            EditorGUI.BeginChangeCheck();
            string str = EditorGUILayout.ToolbarSearchField(this.m_Search, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_Search = str;
            }
            using (new EditorGUI.DisabledScope(((this.m_Controller == null) || (flag && this.m_Controller.hasMultipleDifferentValues)) || (controller2 == null)))
            {
                EditorGUI.BeginChangeCheck();
                target.GetOverrides(this.m_Clips);
                if (this.m_Search.Length > 0)
                {
                    this.FilterOverrides();
                }
                else
                {
                    this.m_Clips.Sort(new AnimationClipOverrideComparer());
                }
                this.m_ClipList.list = this.m_Clips;
                this.m_ClipList.DoLayoutList();
                if (EditorGUI.EndChangeCheck())
                {
                    for (int j = 0; j < base.targets.Length; j++)
                    {
                        (base.targets[j] as AnimatorOverrideController).ApplyOverrides(this.m_Clips);
                    }
                    flag2 = true;
                }
            }
            if (flag2)
            {
                target.PerformOverrideClipListCleanup();
            }
        }

        private void SelectClip(ReorderableList list)
        {
            if ((0 <= list.index) && (list.index < this.m_Clips.Count))
            {
                KeyValuePair<AnimationClip, AnimationClip> pair = this.m_Clips[list.index];
                EditorGUIUtility.PingObject(pair.Key);
            }
        }
    }
}

