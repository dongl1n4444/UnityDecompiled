namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(Joint2D))]
    internal class Joint2DEditor : Editor
    {
        [CompilerGenerated]
        private static Handles.CapFunction <>f__mg$cache0;
        [CompilerGenerated]
        private static Handles.CapFunction <>f__mg$cache1;
        private SerializedProperty m_BreakForce;
        private SerializedProperty m_BreakTorque;
        protected static Styles s_Styles;

        public static void AnchorHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            if (controlID == GUIUtility.keyboardControl)
            {
                HandleCap(controlID, position, s_Styles.anchorActive, eventType);
            }
            else
            {
                HandleCap(controlID, position, s_Styles.anchor, eventType);
            }
        }

        public static void ConnectedAnchorHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            if (controlID == GUIUtility.keyboardControl)
            {
                HandleCap(controlID, position, s_Styles.connectedAnchorActive, eventType);
            }
            else
            {
                HandleCap(controlID, position, s_Styles.connectedAnchor, eventType);
            }
        }

        public static void DrawAALine(Vector3 start, Vector3 end)
        {
            Vector3[] points = new Vector3[] { start, end };
            Handles.DrawAAPolyLine(points);
        }

        public static void DrawDistanceGizmo(Vector3 anchor, Vector3 connectedAnchor, float distance)
        {
            Vector3 vector2 = anchor - connectedAnchor;
            Vector3 normalized = vector2.normalized;
            Vector3 end = connectedAnchor + ((Vector3) (normalized * distance));
            Vector3 vector4 = (Vector3) (Vector3.Cross(normalized, Vector3.forward) * (HandleUtility.GetHandleSize(connectedAnchor) * 0.16f));
            Handles.color = Color.green;
            DrawAALine(anchor, end);
            DrawAALine(connectedAnchor + vector4, connectedAnchor - vector4);
            DrawAALine(end + vector4, end - vector4);
        }

        private static Matrix4x4 GetAnchorSpaceMatrix(Transform transform)
        {
            return Matrix4x4.TRS(transform.position, Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z), transform.lossyScale);
        }

        protected bool HandleAnchor(ref Vector3 position, bool isConnectedAnchor)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            if (isConnectedAnchor)
            {
            }
            if (<>f__mg$cache1 == null)
            {
                <>f__mg$cache1 = new Handles.CapFunction(Joint2DEditor.AnchorHandleCap);
            }
            Handles.CapFunction capFunction = (<>f__mg$cache0 != null) ? <>f__mg$cache1 : <>f__mg$cache0;
            int id = base.target.GetInstanceID() + (!isConnectedAnchor ? 0 : 1);
            EditorGUI.BeginChangeCheck();
            position = Handles.Slider2D(id, position, Vector3.back, Vector3.right, Vector3.up, 0f, capFunction, Vector2.zero);
            return EditorGUI.EndChangeCheck();
        }

        private static void HandleCap(int controlID, Vector3 position, GUIStyle guiStyle, EventType eventType)
        {
            if (eventType != EventType.Layout)
            {
                if (eventType == EventType.Repaint)
                {
                    Handles.BeginGUI();
                    position = (Vector3) HandleUtility.WorldToGUIPoint(position);
                    float fixedWidth = guiStyle.fixedWidth;
                    float fixedHeight = guiStyle.fixedHeight;
                    Rect rect = new Rect(position.x - (fixedWidth / 2f), position.y - (fixedHeight / 2f), fixedWidth, fixedHeight);
                    guiStyle.Draw(rect, GUIContent.none, controlID);
                    Handles.EndGUI();
                }
            }
            else
            {
                HandleUtility.AddControl(controlID, HandleUtility.DistanceToRectangleInternal(position, Quaternion.identity, Vector2.zero));
            }
        }

        protected static Vector3 InverseTransformPoint(Transform transform, Vector3 position)
        {
            return GetAnchorSpaceMatrix(transform).inverse.MultiplyPoint(position);
        }

        public void OnEnable()
        {
            this.m_BreakForce = base.serializedObject.FindProperty("m_BreakForce");
            this.m_BreakTorque = base.serializedObject.FindProperty("m_BreakTorque");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(this.m_BreakForce, new GUILayoutOption[0]);
            Type type = base.target.GetType();
            if (((type != typeof(DistanceJoint2D)) && (type != typeof(TargetJoint2D))) && (type != typeof(SpringJoint2D)))
            {
                EditorGUILayout.PropertyField(this.m_BreakTorque, new GUILayoutOption[0]);
            }
            base.serializedObject.ApplyModifiedProperties();
        }

        protected static Vector2 RotateVector2(Vector2 direction, float angle)
        {
            float f = 0.01745329f * -angle;
            float num2 = Mathf.Cos(f);
            float num3 = Mathf.Sin(f);
            float x = (direction.x * num2) - (direction.y * num3);
            return new Vector2(x, (direction.x * num3) + (direction.y * num2));
        }

        protected static Vector3 SnapToPoint(Vector3 position, Vector3 snapPosition, float snapDistance)
        {
            snapDistance = HandleUtility.GetHandleSize(position) * snapDistance;
            return ((Vector3.Distance(position, snapPosition) > snapDistance) ? position : snapPosition);
        }

        protected static Vector3 SnapToSprite(SpriteRenderer spriteRenderer, Vector3 position, float snapDistance)
        {
            if (spriteRenderer != null)
            {
                snapDistance = HandleUtility.GetHandleSize(position) * snapDistance;
                float x = spriteRenderer.sprite.bounds.size.x / 2f;
                float y = spriteRenderer.sprite.bounds.size.y / 2f;
                Vector2[] vectorArray = new Vector2[] { new Vector2(-x, -y), new Vector2(0f, -y), new Vector2(x, -y), new Vector2(-x, 0f), new Vector2(0f, 0f), new Vector2(x, 0f), new Vector2(-x, y), new Vector2(0f, y), new Vector2(x, y) };
                foreach (Vector2 vector4 in vectorArray)
                {
                    Vector3 b = spriteRenderer.transform.TransformPoint((Vector3) vector4);
                    if (Vector2.Distance(position, b) <= snapDistance)
                    {
                        return b;
                    }
                }
            }
            return position;
        }

        protected static Vector3 TransformPoint(Transform transform, Vector3 position)
        {
            return GetAnchorSpaceMatrix(transform).MultiplyPoint(position);
        }

        public class Styles
        {
            public readonly GUIStyle anchor = "U2D.pivotDot";
            public readonly GUIStyle anchorActive = "U2D.pivotDotActive";
            public readonly GUIStyle connectedAnchor = "U2D.dragDot";
            public readonly GUIStyle connectedAnchorActive = "U2D.dragDotActive";
        }
    }
}

