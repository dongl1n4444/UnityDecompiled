namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;

    [CustomEditor(typeof(Rigidbody2D)), CanEditMultipleObjects]
    internal class Rigidbody2DEditor : Editor
    {
        [CompilerGenerated]
        private static Func<Object, bool> <>f__am$cache0;
        private const int k_ToggleOffset = 30;
        private SerializedProperty m_AngularDrag;
        private SerializedProperty m_BodyType;
        private SerializedProperty m_CollisionDetection;
        private SerializedProperty m_Constraints;
        private static readonly GUIContent m_FreezePositionLabel = new GUIContent("Freeze Position");
        private static readonly GUIContent m_FreezeRotationLabel = new GUIContent("Freeze Rotation");
        private SerializedProperty m_GravityScale;
        private SerializedProperty m_Interpolate;
        private SerializedProperty m_LinearDrag;
        private SerializedProperty m_Mass;
        private SerializedProperty m_Material;
        private readonly AnimBool m_ShowInfo = new AnimBool();
        private readonly AnimBool m_ShowIsKinematic = new AnimBool();
        private readonly AnimBool m_ShowIsStatic = new AnimBool();
        private SerializedProperty m_Simulated;
        private SerializedProperty m_SleepingMode;
        private SerializedProperty m_UseAutoMass;
        private SerializedProperty m_UseFullKinematicContacts;

        private void ConstraintToggle(Rect r, string label, RigidbodyConstraints2D value, int bit)
        {
            bool flag = (value & (((int) 1) << bit)) != RigidbodyConstraints2D.None;
            EditorGUI.showMixedValue = (this.m_Constraints.hasMultipleDifferentValuesBitwise & (((int) 1) << bit)) != 0;
            EditorGUI.BeginChangeCheck();
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            flag = EditorGUI.ToggleLeft(r, label, flag);
            EditorGUI.indentLevel = indentLevel;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(base.targets, "Edit Constraints2D");
                this.m_Constraints.SetBitAtIndexForAllTargetsImmediate(bit, flag);
            }
            EditorGUI.showMixedValue = false;
        }

        private static void FixedEndFadeGroup(float value)
        {
            if ((value != 0f) && (value != 1f))
            {
                EditorGUILayout.EndFadeGroup();
            }
        }

        public void OnDisable()
        {
            this.m_ShowIsStatic.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            this.m_ShowIsKinematic.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            this.m_ShowInfo.valueChanged.RemoveListener(new UnityAction(this.Repaint));
        }

        public void OnEnable()
        {
            Rigidbody2D target = base.target as Rigidbody2D;
            this.m_Simulated = base.serializedObject.FindProperty("m_Simulated");
            this.m_BodyType = base.serializedObject.FindProperty("m_BodyType");
            this.m_Material = base.serializedObject.FindProperty("m_Material");
            this.m_UseFullKinematicContacts = base.serializedObject.FindProperty("m_UseFullKinematicContacts");
            this.m_UseAutoMass = base.serializedObject.FindProperty("m_UseAutoMass");
            this.m_Mass = base.serializedObject.FindProperty("m_Mass");
            this.m_LinearDrag = base.serializedObject.FindProperty("m_LinearDrag");
            this.m_AngularDrag = base.serializedObject.FindProperty("m_AngularDrag");
            this.m_GravityScale = base.serializedObject.FindProperty("m_GravityScale");
            this.m_Interpolate = base.serializedObject.FindProperty("m_Interpolate");
            this.m_SleepingMode = base.serializedObject.FindProperty("m_SleepingMode");
            this.m_CollisionDetection = base.serializedObject.FindProperty("m_CollisionDetection");
            this.m_Constraints = base.serializedObject.FindProperty("m_Constraints");
            this.m_ShowIsStatic.value = target.bodyType != RigidbodyType2D.Static;
            this.m_ShowIsStatic.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowIsKinematic.value = target.bodyType != RigidbodyType2D.Kinematic;
            this.m_ShowIsKinematic.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowInfo.valueChanged.AddListener(new UnityAction(this.Repaint));
        }

        public override void OnInspectorGUI()
        {
            Rigidbody2D target = base.target as Rigidbody2D;
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_BodyType, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Material, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Simulated, new GUILayoutOption[0]);
            if (this.m_BodyType.hasMultipleDifferentValues)
            {
                EditorGUILayout.HelpBox("Cannot edit properties that are body type specific when the selection contains different body types.", MessageType.Info);
                goto Label_02A7;
            }
            this.m_ShowIsStatic.target = target.bodyType != RigidbodyType2D.Static;
            if (!EditorGUILayout.BeginFadeGroup(this.m_ShowIsStatic.faded))
            {
                goto Label_0296;
            }
            this.m_ShowIsKinematic.target = target.bodyType != RigidbodyType2D.Kinematic;
            if (!EditorGUILayout.BeginFadeGroup(this.m_ShowIsKinematic.faded))
            {
                goto Label_0199;
            }
            EditorGUILayout.PropertyField(this.m_UseAutoMass, new GUILayoutOption[0]);
            if (!this.m_UseAutoMass.hasMultipleDifferentValues)
            {
                if (this.m_UseAutoMass.boolValue)
                {
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = x => (PrefabUtility.GetPrefabType(x) == PrefabType.Prefab) || !(x as Rigidbody2D).gameObject.activeInHierarchy;
                    }
                    if (Enumerable.Any<Object>(base.targets, <>f__am$cache0))
                    {
                        EditorGUILayout.HelpBox("The auto mass value cannot be displayed for a prefab or if the object is not active.  The value will be calculated for a prefab instance and when the object is active.", MessageType.Info);
                        goto Label_0162;
                    }
                }
                EditorGUI.BeginDisabledGroup(target.useAutoMass);
                EditorGUILayout.PropertyField(this.m_Mass, new GUILayoutOption[0]);
                EditorGUI.EndDisabledGroup();
            }
        Label_0162:
            EditorGUILayout.PropertyField(this.m_LinearDrag, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_AngularDrag, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_GravityScale, new GUILayoutOption[0]);
        Label_0199:
            FixedEndFadeGroup(this.m_ShowIsKinematic.faded);
            if (!this.m_ShowIsKinematic.target)
            {
                EditorGUILayout.PropertyField(this.m_UseFullKinematicContacts, new GUILayoutOption[0]);
            }
            EditorGUILayout.PropertyField(this.m_CollisionDetection, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_SleepingMode, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Interpolate, new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            this.m_Constraints.isExpanded = EditorGUILayout.Foldout(this.m_Constraints.isExpanded, "Constraints", true);
            GUILayout.EndHorizontal();
            RigidbodyConstraints2D intValue = (RigidbodyConstraints2D) this.m_Constraints.intValue;
            if (this.m_Constraints.isExpanded)
            {
                EditorGUI.indentLevel++;
                this.ToggleFreezePosition(intValue, m_FreezePositionLabel, 0, 1);
                this.ToggleFreezeRotation(intValue, m_FreezeRotationLabel, 2);
                EditorGUI.indentLevel--;
            }
            if (intValue == RigidbodyConstraints2D.FreezeAll)
            {
                EditorGUILayout.HelpBox("Rather than turning on all constraints, you may want to consider removing the Rigidbody2D component which makes any colliders static.  This gives far better performance overall.", MessageType.Info);
            }
        Label_0296:
            FixedEndFadeGroup(this.m_ShowIsStatic.faded);
        Label_02A7:
            base.serializedObject.ApplyModifiedProperties();
            this.ShowBodyInfoProperties();
        }

        private void ShowBodyInfoProperties()
        {
            this.m_ShowInfo.target = EditorGUILayout.Foldout(this.m_ShowInfo.target, "Info", true);
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowInfo.faded))
            {
                if (base.targets.Length == 1)
                {
                    Rigidbody2D rigidbodyd = base.targets[0] as Rigidbody2D;
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.Vector2Field("Position", rigidbodyd.position, new GUILayoutOption[0]);
                    EditorGUILayout.FloatField("Rotation", rigidbodyd.rotation, new GUILayoutOption[0]);
                    EditorGUILayout.Vector2Field("Velocity", rigidbodyd.velocity, new GUILayoutOption[0]);
                    EditorGUILayout.FloatField("Angular Velocity", rigidbodyd.angularVelocity, new GUILayoutOption[0]);
                    EditorGUILayout.FloatField("Inertia", rigidbodyd.inertia, new GUILayoutOption[0]);
                    EditorGUILayout.Vector2Field("Local Center of Mass", rigidbodyd.centerOfMass, new GUILayoutOption[0]);
                    EditorGUILayout.Vector2Field("World Center of Mass", rigidbodyd.worldCenterOfMass, new GUILayoutOption[0]);
                    EditorGUILayout.LabelField("Sleep State", !rigidbodyd.IsSleeping() ? "Awake" : "Asleep", new GUILayoutOption[0]);
                    EditorGUI.EndDisabledGroup();
                    base.Repaint();
                }
                else
                {
                    EditorGUILayout.HelpBox("Cannot show Info properties when multiple bodies are selected.", MessageType.Info);
                }
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void ToggleFreezePosition(RigidbodyConstraints2D constraints, GUIContent label, int x, int y)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            Rect position = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, EditorGUILayout.kLabelFloatMaxW, 16f, 16f, EditorStyles.numberField);
            int id = GUIUtility.GetControlID(0x1c3f, FocusType.Keyboard, position);
            position = EditorGUI.PrefixLabel(position, id, label);
            position.width = 30f;
            this.ConstraintToggle(position, "X", constraints, x);
            position.x += 30f;
            this.ConstraintToggle(position, "Y", constraints, y);
            GUILayout.EndHorizontal();
        }

        private void ToggleFreezeRotation(RigidbodyConstraints2D constraints, GUIContent label, int z)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            Rect position = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, EditorGUILayout.kLabelFloatMaxW, 16f, 16f, EditorStyles.numberField);
            int id = GUIUtility.GetControlID(0x1c3f, FocusType.Keyboard, position);
            position = EditorGUI.PrefixLabel(position, id, label);
            position.width = 30f;
            this.ConstraintToggle(position, "Z", constraints, z);
            GUILayout.EndHorizontal();
        }
    }
}

