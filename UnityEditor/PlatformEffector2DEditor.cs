namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;

    [CustomEditor(typeof(PlatformEffector2D), true), CanEditMultipleObjects]
    internal class PlatformEffector2DEditor : Effector2DEditor
    {
        [CompilerGenerated]
        private static Func<Collider2D, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Collider2D, bool> <>f__am$cache1;
        private SerializedProperty m_RotationalOffset;
        private readonly AnimBool m_ShowOneWayRollout = new AnimBool();
        private static readonly AnimBool m_ShowSidesRollout = new AnimBool();
        private SerializedProperty m_SideArc;
        private SerializedProperty m_SurfaceArc;
        private SerializedProperty m_UseOneWay;
        private SerializedProperty m_UseOneWayGrouping;
        private SerializedProperty m_UseSideBounce;
        private SerializedProperty m_UseSideFriction;

        private static void DrawSideArc(PlatformEffector2D effector)
        {
            float f = -0.01745329f * (90f + effector.rotationalOffset);
            Vector3 normalized = effector.transform.TransformVector(new Vector3(Mathf.Sin(f), Mathf.Cos(f), 0f)).normalized;
            if (normalized.sqrMagnitude >= Mathf.Epsilon)
            {
                float num2 = Mathf.Atan2(normalized.x, normalized.y);
                float num3 = num2 + 3.141593f;
                float angle = Mathf.Clamp(effector.sideArc, 0.5f, 180f);
                float num5 = (angle * 0.5f) * 0.01745329f;
                Vector3 from = new Vector3(Mathf.Sin(num2 - num5), Mathf.Cos(num2 - num5), 0f);
                Vector3 vector4 = new Vector3(Mathf.Sin(num2 + num5), Mathf.Cos(num2 + num5), 0f);
                Vector3 vector5 = new Vector3(Mathf.Sin(num3 - num5), Mathf.Cos(num3 - num5), 0f);
                Vector3 vector6 = new Vector3(Mathf.Sin(num3 + num5), Mathf.Cos(num3 + num5), 0f);
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = collider => collider.enabled && collider.usedByEffector;
                }
                foreach (Collider2D colliderd in Enumerable.Where<Collider2D>(effector.gameObject.GetComponents<Collider2D>(), <>f__am$cache1))
                {
                    Vector3 center = colliderd.bounds.center;
                    float radius = HandleUtility.GetHandleSize(center) * 0.8f;
                    Handles.color = new Color(0f, 1f, 0.7f, 0.07f);
                    Handles.DrawSolidArc(center, Vector3.back, from, angle, radius);
                    Handles.DrawSolidArc(center, Vector3.back, vector5, angle, radius);
                    Handles.color = new Color(0f, 1f, 0.7f, 0.7f);
                    Handles.DrawWireArc(center, Vector3.back, from, angle, radius);
                    Handles.DrawWireArc(center, Vector3.back, vector5, angle, radius);
                    Handles.DrawDottedLine(center, center + ((Vector3) (from * radius)), 5f);
                    Handles.DrawDottedLine(center, center + ((Vector3) (vector4 * radius)), 5f);
                    Handles.DrawDottedLine(center, center + ((Vector3) (vector5 * radius)), 5f);
                    Handles.DrawDottedLine(center, center + ((Vector3) (vector6 * radius)), 5f);
                }
            }
        }

        private static void DrawSurfaceArc(PlatformEffector2D effector)
        {
            float f = -0.01745329f * effector.rotationalOffset;
            Vector3 normalized = effector.transform.TransformVector(new Vector3(Mathf.Sin(f), Mathf.Cos(f), 0f)).normalized;
            if (normalized.sqrMagnitude >= Mathf.Epsilon)
            {
                float num2 = Mathf.Atan2(normalized.x, normalized.y);
                float angle = Mathf.Clamp(effector.surfaceArc, 0.5f, 360f);
                float num4 = (angle * 0.5f) * 0.01745329f;
                Vector3 from = new Vector3(Mathf.Sin(num2 - num4), Mathf.Cos(num2 - num4), 0f);
                Vector3 vector4 = new Vector3(Mathf.Sin(num2 + num4), Mathf.Cos(num2 + num4), 0f);
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = collider => collider.enabled && collider.usedByEffector;
                }
                foreach (Collider2D colliderd in Enumerable.Where<Collider2D>(effector.gameObject.GetComponents<Collider2D>(), <>f__am$cache0))
                {
                    Vector3 center = colliderd.bounds.center;
                    float handleSize = HandleUtility.GetHandleSize(center);
                    Handles.color = new Color(0f, 1f, 1f, 0.07f);
                    Handles.DrawSolidArc(center, Vector3.back, from, angle, handleSize);
                    Handles.color = new Color(0f, 1f, 1f, 0.7f);
                    Handles.DrawWireArc(center, Vector3.back, from, angle, handleSize);
                    Handles.DrawDottedLine(center, center + ((Vector3) (from * handleSize)), 5f);
                    Handles.DrawDottedLine(center, center + ((Vector3) (vector4 * handleSize)), 5f);
                }
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            this.m_ShowOneWayRollout.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            m_ShowSidesRollout.valueChanged.RemoveListener(new UnityAction(this.Repaint));
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_RotationalOffset = base.serializedObject.FindProperty("m_RotationalOffset");
            this.m_ShowOneWayRollout.value = true;
            this.m_ShowOneWayRollout.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_UseOneWay = base.serializedObject.FindProperty("m_UseOneWay");
            this.m_UseOneWayGrouping = base.serializedObject.FindProperty("m_UseOneWayGrouping");
            this.m_SurfaceArc = base.serializedObject.FindProperty("m_SurfaceArc");
            m_ShowSidesRollout.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_UseSideFriction = base.serializedObject.FindProperty("m_UseSideFriction");
            this.m_UseSideBounce = base.serializedObject.FindProperty("m_UseSideBounce");
            this.m_SideArc = base.serializedObject.FindProperty("m_SideArc");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_RotationalOffset, new GUILayoutOption[0]);
            this.m_ShowOneWayRollout.target = EditorGUILayout.Foldout(this.m_ShowOneWayRollout.target, "One Way", true);
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowOneWayRollout.faded))
            {
                EditorGUILayout.PropertyField(this.m_UseOneWay, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_UseOneWayGrouping, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_SurfaceArc, new GUILayoutOption[0]);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
            m_ShowSidesRollout.target = EditorGUILayout.Foldout(m_ShowSidesRollout.target, "Sides", true);
            if (EditorGUILayout.BeginFadeGroup(m_ShowSidesRollout.faded))
            {
                EditorGUILayout.PropertyField(this.m_UseSideFriction, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_UseSideBounce, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_SideArc, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndFadeGroup();
            base.serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            PlatformEffector2D target = (PlatformEffector2D) base.target;
            if (target.enabled)
            {
                if (target.useOneWay)
                {
                    DrawSurfaceArc(target);
                }
                if (!target.useSideBounce || !target.useSideFriction)
                {
                    DrawSideArc(target);
                }
            }
        }
    }
}

