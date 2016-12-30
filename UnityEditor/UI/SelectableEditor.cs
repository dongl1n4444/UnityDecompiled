namespace UnityEditor.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEditor;
    using UnityEditor.AnimatedValues;
    using UnityEditor.Animations;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    /// <summary>
    /// <para>Custom Editor for the Selectable Component.</para>
    /// </summary>
    [CustomEditor(typeof(Selectable), true)]
    public class SelectableEditor : Editor
    {
        [CompilerGenerated]
        private static SceneView.OnSceneFunc <>f__mg$cache0;
        [CompilerGenerated]
        private static SceneView.OnSceneFunc <>f__mg$cache1;
        private const float kArrowHeadSize = 1.2f;
        private const float kArrowThickness = 2.5f;
        private SerializedProperty m_AnimTriggerProperty;
        private SerializedProperty m_ColorBlockProperty;
        private SerializedProperty m_InteractableProperty;
        private SerializedProperty m_NavigationProperty;
        private string[] m_PropertyPathToExcludeForChildClasses;
        private SerializedProperty m_Script;
        private AnimBool m_ShowAnimTransition = new AnimBool();
        private AnimBool m_ShowColorTint = new AnimBool();
        private AnimBool m_ShowSpriteTrasition = new AnimBool();
        private SerializedProperty m_SpriteStateProperty;
        private SerializedProperty m_TargetGraphicProperty;
        private SerializedProperty m_TransitionProperty;
        private GUIContent m_VisualizeNavigation = new GUIContent("Visualize", "Show navigation flows between selectable UI elements.");
        private static List<SelectableEditor> s_Editors = new List<SelectableEditor>();
        private static bool s_ShowNavigation = false;
        private static string s_ShowNavigationKey = "SelectableEditor.ShowNavigation";

        private static string BuildAnimationPath(Selectable target)
        {
            Graphic targetGraphic = target.targetGraphic;
            if (targetGraphic == null)
            {
                return string.Empty;
            }
            GameObject gameObject = targetGraphic.gameObject;
            GameObject obj3 = target.gameObject;
            Stack<string> stack = new Stack<string>();
            while (obj3 != gameObject)
            {
                stack.Push(gameObject.name);
                if (gameObject.transform.parent == null)
                {
                    return string.Empty;
                }
                gameObject = gameObject.transform.parent.gameObject;
            }
            StringBuilder builder = new StringBuilder();
            if (stack.Count > 0)
            {
                builder.Append(stack.Pop());
            }
            while (stack.Count > 0)
            {
                builder.Append("/").Append(stack.Pop());
            }
            return builder.ToString();
        }

        private void ChildClassPropertiesGUI()
        {
            if (!this.IsDerivedSelectableEditor())
            {
                Editor.DrawPropertiesExcluding(base.serializedObject, this.m_PropertyPathToExcludeForChildClasses);
            }
        }

        private static void DrawNavigationArrow(Vector2 direction, Selectable fromObj, Selectable toObj)
        {
            if ((fromObj != null) && (toObj != null))
            {
                Transform transform = fromObj.transform;
                Transform transform2 = toObj.transform;
                Vector2 vector = new Vector2(direction.y, -direction.x);
                Vector3 position = transform.TransformPoint(GetPointOnRectEdge(transform as RectTransform, direction));
                Vector3 vector3 = transform2.TransformPoint(GetPointOnRectEdge(transform2 as RectTransform, -direction));
                float num = HandleUtility.GetHandleSize(position) * 0.05f;
                float num2 = HandleUtility.GetHandleSize(vector3) * 0.05f;
                position += (Vector3) (transform.TransformDirection((Vector3) vector) * num);
                vector3 += (Vector3) (transform2.TransformDirection((Vector3) vector) * num2);
                float num3 = Vector3.Distance(position, vector3);
                Vector3 vector4 = (Vector3) (((transform.rotation * direction) * num3) * 0.3f);
                Vector3 vector5 = (Vector3) (((transform2.rotation * -direction) * num3) * 0.3f);
                Handles.DrawBezier(position, vector3, position + vector4, vector3 + vector5, Handles.color, null, 2.5f);
                Vector3[] points = new Vector3[] { vector3, vector3 + (((transform2.rotation * (-direction - vector)) * num2) * 1.2f) };
                Handles.DrawAAPolyLine((float) 2.5f, points);
                Vector3[] vectorArray2 = new Vector3[] { vector3, vector3 + ((Vector3) (((transform2.rotation * (-direction + vector)) * num2) * 1.2f)) };
                Handles.DrawAAPolyLine((float) 2.5f, vectorArray2);
            }
        }

        private static void DrawNavigationForSelectable(Selectable sel)
        {
            <DrawNavigationForSelectable>c__AnonStorey0 storey = new <DrawNavigationForSelectable>c__AnonStorey0();
            if (sel != null)
            {
                storey.transform = sel.transform;
                bool flag = Enumerable.Any<Transform>(Selection.transforms, new Func<Transform, bool>(storey.<>m__0));
                Handles.color = new Color(1f, 0.9f, 0.1f, !flag ? 0.4f : 1f);
                DrawNavigationArrow(-Vector2.right, sel, sel.FindSelectableOnLeft());
                DrawNavigationArrow(Vector2.right, sel, sel.FindSelectableOnRight());
                DrawNavigationArrow(Vector2.up, sel, sel.FindSelectableOnUp());
                DrawNavigationArrow(-Vector2.up, sel, sel.FindSelectableOnDown());
            }
        }

        private static AnimatorController GenerateSelectableAnimatorContoller(AnimationTriggers animationTriggers, Selectable target)
        {
            if (target == null)
            {
                return null;
            }
            string saveControllerPath = GetSaveControllerPath(target);
            if (string.IsNullOrEmpty(saveControllerPath))
            {
                return null;
            }
            string name = !string.IsNullOrEmpty(animationTriggers.normalTrigger) ? animationTriggers.normalTrigger : "Normal";
            string str3 = !string.IsNullOrEmpty(animationTriggers.highlightedTrigger) ? animationTriggers.highlightedTrigger : "Highlighted";
            string str4 = !string.IsNullOrEmpty(animationTriggers.pressedTrigger) ? animationTriggers.pressedTrigger : "Pressed";
            string str5 = !string.IsNullOrEmpty(animationTriggers.disabledTrigger) ? animationTriggers.disabledTrigger : "Disabled";
            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(saveControllerPath);
            GenerateTriggerableTransition(name, controller);
            GenerateTriggerableTransition(str3, controller);
            GenerateTriggerableTransition(str4, controller);
            GenerateTriggerableTransition(str5, controller);
            AssetDatabase.ImportAsset(saveControllerPath);
            return controller;
        }

        private static AnimationClip GenerateTriggerableTransition(string name, AnimatorController controller)
        {
            AnimationClip objectToAdd = AnimatorController.AllocateAnimatorClip(name);
            AssetDatabase.AddObjectToAsset(objectToAdd, controller);
            AnimatorState destinationState = controller.AddMotion(objectToAdd);
            controller.AddParameter(name, AnimatorControllerParameterType.Trigger);
            controller.layers[0].stateMachine.AddAnyStateTransition(destinationState).AddCondition(AnimatorConditionMode.If, 0f, name);
            return objectToAdd;
        }

        private static Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir)
        {
            if (rect == null)
            {
                return Vector3.zero;
            }
            if (dir != Vector2.zero)
            {
                dir = (Vector2) (dir / Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y)));
            }
            dir = rect.rect.center + Vector2.Scale(rect.rect.size, (Vector2) (dir * 0.5f));
            return (Vector3) dir;
        }

        private static string GetSaveControllerPath(Selectable target)
        {
            string name = target.gameObject.name;
            string message = $"Create a new animator for the game object '{name}':";
            return EditorUtility.SaveFilePanelInProject("New Animation Contoller", name, "controller", message);
        }

        private static Selectable.Transition GetTransition(SerializedProperty transition) => 
            ((Selectable.Transition) transition.enumValueIndex);

        private bool IsDerivedSelectableEditor() => 
            !(base.GetType() == typeof(SelectableEditor));

        /// <summary>
        /// <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected virtual void OnDisable()
        {
            this.m_ShowColorTint.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            this.m_ShowSpriteTrasition.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            s_Editors.Remove(this);
            this.RegisterStaticOnSceneGUI();
        }

        protected virtual void OnEnable()
        {
            this.m_Script = base.serializedObject.FindProperty("m_Script");
            this.m_InteractableProperty = base.serializedObject.FindProperty("m_Interactable");
            this.m_TargetGraphicProperty = base.serializedObject.FindProperty("m_TargetGraphic");
            this.m_TransitionProperty = base.serializedObject.FindProperty("m_Transition");
            this.m_ColorBlockProperty = base.serializedObject.FindProperty("m_Colors");
            this.m_SpriteStateProperty = base.serializedObject.FindProperty("m_SpriteState");
            this.m_AnimTriggerProperty = base.serializedObject.FindProperty("m_AnimationTriggers");
            this.m_NavigationProperty = base.serializedObject.FindProperty("m_Navigation");
            this.m_PropertyPathToExcludeForChildClasses = new string[] { this.m_Script.propertyPath, this.m_NavigationProperty.propertyPath, this.m_TransitionProperty.propertyPath, this.m_ColorBlockProperty.propertyPath, this.m_SpriteStateProperty.propertyPath, this.m_AnimTriggerProperty.propertyPath, this.m_InteractableProperty.propertyPath, this.m_TargetGraphicProperty.propertyPath };
            Selectable.Transition transition = GetTransition(this.m_TransitionProperty);
            this.m_ShowColorTint.value = transition == Selectable.Transition.ColorTint;
            this.m_ShowSpriteTrasition.value = transition == Selectable.Transition.SpriteSwap;
            this.m_ShowAnimTransition.value = transition == Selectable.Transition.Animation;
            this.m_ShowColorTint.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowSpriteTrasition.valueChanged.AddListener(new UnityAction(this.Repaint));
            s_Editors.Add(this);
            this.RegisterStaticOnSceneGUI();
            s_ShowNavigation = EditorPrefs.GetBool(s_ShowNavigationKey);
        }

        /// <summary>
        /// <para>See Editor.OnInspectorGUI.</para>
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_InteractableProperty, new GUILayoutOption[0]);
            Selectable.Transition transition = GetTransition(this.m_TransitionProperty);
            Graphic objectReferenceValue = this.m_TargetGraphicProperty.objectReferenceValue as Graphic;
            if (objectReferenceValue == null)
            {
                objectReferenceValue = (base.target as Selectable).GetComponent<Graphic>();
            }
            Animator component = (base.target as Selectable).GetComponent<Animator>();
            this.m_ShowColorTint.target = !this.m_TransitionProperty.hasMultipleDifferentValues && (transition == Selectable.Transition.ColorTint);
            this.m_ShowSpriteTrasition.target = !this.m_TransitionProperty.hasMultipleDifferentValues && (transition == Selectable.Transition.SpriteSwap);
            this.m_ShowAnimTransition.target = !this.m_TransitionProperty.hasMultipleDifferentValues && (transition == Selectable.Transition.Animation);
            EditorGUILayout.PropertyField(this.m_TransitionProperty, new GUILayoutOption[0]);
            EditorGUI.indentLevel++;
            switch (transition)
            {
                case Selectable.Transition.ColorTint:
                case Selectable.Transition.SpriteSwap:
                    EditorGUILayout.PropertyField(this.m_TargetGraphicProperty, new GUILayoutOption[0]);
                    break;
            }
            if (transition == Selectable.Transition.ColorTint)
            {
                if (objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox("You must have a Graphic target in order to use a color transition.", MessageType.Warning);
                }
            }
            else if ((transition == Selectable.Transition.SpriteSwap) && !(objectReferenceValue is Image))
            {
                EditorGUILayout.HelpBox("You must have a Image target in order to use a sprite swap transition.", MessageType.Warning);
            }
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowColorTint.faded))
            {
                EditorGUILayout.PropertyField(this.m_ColorBlockProperty, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndFadeGroup();
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowSpriteTrasition.faded))
            {
                EditorGUILayout.PropertyField(this.m_SpriteStateProperty, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndFadeGroup();
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowAnimTransition.faded))
            {
                EditorGUILayout.PropertyField(this.m_AnimTriggerProperty, new GUILayoutOption[0]);
                if ((component == null) || (component.runtimeAnimatorController == null))
                {
                    Rect position = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
                    position.xMin += EditorGUIUtility.labelWidth;
                    if (GUI.Button(position, "Auto Generate Animation", EditorStyles.miniButton))
                    {
                        AnimatorController controller = GenerateSelectableAnimatorContoller((base.target as Selectable).animationTriggers, base.target as Selectable);
                        if (controller != null)
                        {
                            if (component == null)
                            {
                                component = (base.target as Selectable).gameObject.AddComponent<Animator>();
                            }
                            AnimatorController.SetAnimatorController(component, controller);
                        }
                    }
                }
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.m_NavigationProperty, new GUILayoutOption[0]);
            EditorGUI.BeginChangeCheck();
            Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
            controlRect.xMin += EditorGUIUtility.labelWidth;
            s_ShowNavigation = GUI.Toggle(controlRect, s_ShowNavigation, this.m_VisualizeNavigation, EditorStyles.miniButton);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(s_ShowNavigationKey, s_ShowNavigation);
                SceneView.RepaintAll();
            }
            this.ChildClassPropertiesGUI();
            base.serializedObject.ApplyModifiedProperties();
        }

        private void RegisterStaticOnSceneGUI()
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new SceneView.OnSceneFunc(SelectableEditor.StaticOnSceneGUI);
            }
            SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc) Delegate.Remove(SceneView.onSceneGUIDelegate, <>f__mg$cache0);
            if (s_Editors.Count > 0)
            {
                if (<>f__mg$cache1 == null)
                {
                    <>f__mg$cache1 = new SceneView.OnSceneFunc(SelectableEditor.StaticOnSceneGUI);
                }
                SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc) Delegate.Combine(SceneView.onSceneGUIDelegate, <>f__mg$cache1);
            }
        }

        private static void SetUpCurves(AnimationClip highlightedClip, AnimationClip pressedClip, string animationPath)
        {
            string[] strArray = new string[] { "m_LocalScale.x", "m_LocalScale.y", "m_LocalScale.z" };
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 1f), new Keyframe(0.5f, 1.1f), new Keyframe(1f, 1f) };
            AnimationCurve curve = new AnimationCurve(keys);
            foreach (string str in strArray)
            {
                AnimationUtility.SetEditorCurve(highlightedClip, EditorCurveBinding.FloatCurve(animationPath, typeof(Transform), str), curve);
            }
            Keyframe[] keyframeArray2 = new Keyframe[] { new Keyframe(0f, 1.15f) };
            AnimationCurve curve2 = new AnimationCurve(keyframeArray2);
            foreach (string str2 in strArray)
            {
                AnimationUtility.SetEditorCurve(pressedClip, EditorCurveBinding.FloatCurve(animationPath, typeof(Transform), str2), curve2);
            }
        }

        private static void StaticOnSceneGUI(SceneView view)
        {
            if (s_ShowNavigation)
            {
                for (int i = 0; i < Selectable.allSelectables.Count; i++)
                {
                    DrawNavigationForSelectable(Selectable.allSelectables[i]);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <DrawNavigationForSelectable>c__AnonStorey0
        {
            internal Transform transform;

            internal bool <>m__0(Transform e) => 
                (e == this.transform);
        }
    }
}

