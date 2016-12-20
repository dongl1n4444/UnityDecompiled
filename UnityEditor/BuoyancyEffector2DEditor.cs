namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;

    [CustomEditor(typeof(BuoyancyEffector2D), true), CanEditMultipleObjects]
    internal class BuoyancyEffector2DEditor : Effector2DEditor
    {
        [CompilerGenerated]
        private static Func<Collider2D, bool> <>f__am$cache0;
        private SerializedProperty m_AngularDrag;
        private SerializedProperty m_Density;
        private SerializedProperty m_FlowAngle;
        private SerializedProperty m_FlowMagnitude;
        private SerializedProperty m_FlowVariation;
        private SerializedProperty m_LinearDrag;
        private static readonly AnimBool m_ShowDampingRollout = new AnimBool();
        private static readonly AnimBool m_ShowFlowRollout = new AnimBool();
        private SerializedProperty m_SurfaceLevel;

        public override void OnDisable()
        {
            base.OnDisable();
            m_ShowDampingRollout.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            m_ShowFlowRollout.valueChanged.RemoveListener(new UnityAction(this.Repaint));
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_Density = base.serializedObject.FindProperty("m_Density");
            this.m_SurfaceLevel = base.serializedObject.FindProperty("m_SurfaceLevel");
            m_ShowDampingRollout.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_LinearDrag = base.serializedObject.FindProperty("m_LinearDrag");
            this.m_AngularDrag = base.serializedObject.FindProperty("m_AngularDrag");
            m_ShowFlowRollout.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_FlowAngle = base.serializedObject.FindProperty("m_FlowAngle");
            this.m_FlowMagnitude = base.serializedObject.FindProperty("m_FlowMagnitude");
            this.m_FlowVariation = base.serializedObject.FindProperty("m_FlowVariation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_Density, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_SurfaceLevel, new GUILayoutOption[0]);
            m_ShowDampingRollout.target = EditorGUILayout.Foldout(m_ShowDampingRollout.target, "Damping", true);
            if (EditorGUILayout.BeginFadeGroup(m_ShowDampingRollout.faded))
            {
                EditorGUILayout.PropertyField(this.m_LinearDrag, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_AngularDrag, new GUILayoutOption[0]);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
            m_ShowFlowRollout.target = EditorGUILayout.Foldout(m_ShowFlowRollout.target, "Flow", true);
            if (EditorGUILayout.BeginFadeGroup(m_ShowFlowRollout.faded))
            {
                EditorGUILayout.PropertyField(this.m_FlowAngle, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_FlowMagnitude, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_FlowVariation, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndFadeGroup();
            base.serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            BuoyancyEffector2D target = (BuoyancyEffector2D) base.target;
            if (target.enabled)
            {
                float y = target.transform.position.y + (target.transform.lossyScale.y * target.surfaceLevel);
                List<Vector3> list = new List<Vector3>();
                float negativeInfinity = float.NegativeInfinity;
                float x = negativeInfinity;
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<Collider2D, bool>(null, (IntPtr) <OnSceneGUI>m__0);
                }
                foreach (Collider2D colliderd in Enumerable.Where<Collider2D>(target.gameObject.GetComponents<Collider2D>(), <>f__am$cache0))
                {
                    Bounds bounds = colliderd.bounds;
                    float num4 = bounds.min.x;
                    float num5 = bounds.max.x;
                    if (float.IsNegativeInfinity(negativeInfinity))
                    {
                        negativeInfinity = num4;
                        x = num5;
                    }
                    else
                    {
                        if (num4 < negativeInfinity)
                        {
                            negativeInfinity = num4;
                        }
                        if (num5 > x)
                        {
                            x = num5;
                        }
                    }
                    Vector3 item = new Vector3(num4, y, 0f);
                    Vector3 vector6 = new Vector3(num5, y, 0f);
                    list.Add(item);
                    list.Add(vector6);
                }
                Handles.color = Color.red;
                Vector3[] points = new Vector3[] { new Vector3(negativeInfinity, y, 0f), new Vector3(x, y, 0f) };
                Handles.DrawAAPolyLine(points);
                Handles.color = Color.cyan;
                for (int i = 0; i < (list.Count - 1); i += 2)
                {
                    Vector3[] vectorArray2 = new Vector3[] { list[i], list[i + 1] };
                    Handles.DrawAAPolyLine(vectorArray2);
                }
            }
        }
    }
}

