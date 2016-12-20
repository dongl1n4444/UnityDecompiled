namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class TriggerModuleUI : ModuleUI
    {
        private const int k_MaxNumCollisionShapes = 6;
        private SerializedProperty[] m_CollisionShapes;
        private SerializedProperty m_Enter;
        private SerializedProperty m_Exit;
        private SerializedProperty m_Inside;
        private SerializedProperty m_Outside;
        private SerializedProperty m_RadiusScale;
        private SerializedProperty[] m_ShownCollisionShapes;
        private static Texts s_Texts;
        private static bool s_VisualizeBounds = false;

        public TriggerModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "TriggerModule", displayName)
        {
            this.m_CollisionShapes = new SerializedProperty[6];
            base.m_ToolTip = "Allows you to execute script code based on whether particles are inside or outside the collision shapes.";
        }

        private static GameObject CreateDefaultCollider(string name, ParticleSystem parentOfGameObject)
        {
            GameObject obj2 = new GameObject(name);
            if (obj2 != null)
            {
                if (parentOfGameObject != null)
                {
                    obj2.transform.parent = parentOfGameObject.transform;
                }
                obj2.AddComponent<SphereCollider>();
                return obj2;
            }
            return null;
        }

        private void DoListOfCollisionShapesGUI()
        {
            int index = base.GUIListOfFloatObjectToggleFields(s_Texts.collisionShapes, this.m_ShownCollisionShapes, null, s_Texts.createCollisionShape, true, new GUILayoutOption[0]);
            if (index >= 0)
            {
                GameObject obj2 = CreateDefaultCollider("Collider " + (index + 1), base.m_ParticleSystemUI.m_ParticleSystem);
                obj2.transform.localPosition = new Vector3(0f, 0f, (float) (10 + index));
                this.m_ShownCollisionShapes[index].objectReferenceValue = obj2;
            }
            Rect position = GUILayoutUtility.GetRect((float) 0f, (float) 16f);
            position.x = (position.xMax - 24f) - 5f;
            position.width = 12f;
            if ((this.m_ShownCollisionShapes.Length > 1) && ModuleUI.MinusButton(position))
            {
                this.m_ShownCollisionShapes[this.m_ShownCollisionShapes.Length - 1].objectReferenceValue = null;
                List<SerializedProperty> list = new List<SerializedProperty>(this.m_ShownCollisionShapes);
                list.RemoveAt(list.Count - 1);
                this.m_ShownCollisionShapes = list.ToArray();
            }
            if (this.m_ShownCollisionShapes.Length < 6)
            {
                position.x += 17f;
                if (ModuleUI.PlusButton(position))
                {
                    List<SerializedProperty> list2 = new List<SerializedProperty>(this.m_ShownCollisionShapes);
                    list2.Add(this.m_CollisionShapes[list2.Count]);
                    this.m_ShownCollisionShapes = list2.ToArray();
                }
            }
        }

        protected override void Init()
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            List<SerializedProperty> list = new List<SerializedProperty>();
            for (int i = 0; i < this.m_CollisionShapes.Length; i++)
            {
                this.m_CollisionShapes[i] = base.GetProperty("collisionShape" + i);
                if ((i == 0) || (this.m_CollisionShapes[i].objectReferenceValue != null))
                {
                    list.Add(this.m_CollisionShapes[i]);
                }
            }
            this.m_ShownCollisionShapes = list.ToArray();
            this.m_Inside = base.GetProperty("inside");
            this.m_Outside = base.GetProperty("outside");
            this.m_Enter = base.GetProperty("enter");
            this.m_Exit = base.GetProperty("exit");
            this.m_RadiusScale = base.GetProperty("radiusScale");
            s_VisualizeBounds = EditorPrefs.GetBool("VisualizeTriggerBounds", false);
        }

        public override void OnInspectorGUI(ParticleSystem s)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            this.DoListOfCollisionShapesGUI();
            ModuleUI.GUIPopup(s_Texts.inside, this.m_Inside, s_Texts.overlapOptions, new GUILayoutOption[0]);
            ModuleUI.GUIPopup(s_Texts.outside, this.m_Outside, s_Texts.overlapOptions, new GUILayoutOption[0]);
            ModuleUI.GUIPopup(s_Texts.enter, this.m_Enter, s_Texts.overlapOptions, new GUILayoutOption[0]);
            ModuleUI.GUIPopup(s_Texts.exit, this.m_Exit, s_Texts.overlapOptions, new GUILayoutOption[0]);
            ModuleUI.GUIFloat(s_Texts.radiusScale, this.m_RadiusScale, new GUILayoutOption[0]);
            EditorGUI.BeginChangeCheck();
            s_VisualizeBounds = ModuleUI.GUIToggle(s_Texts.visualizeBounds, s_VisualizeBounds, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool("VisualizeTriggerBounds", s_VisualizeBounds);
            }
        }

        [DrawGizmo(GizmoType.Active)]
        private static void RenderCollisionBounds(ParticleSystem system, GizmoType gizmoType)
        {
            if (system.trigger.enabled && s_VisualizeBounds)
            {
                ParticleSystem.Particle[] particles = new ParticleSystem.Particle[system.particleCount];
                int num = system.GetParticles(particles);
                Color color = Gizmos.color;
                Gizmos.color = Color.green;
                Matrix4x4 identity = Matrix4x4.identity;
                if (system.main.simulationSpace == ParticleSystemSimulationSpace.Local)
                {
                    identity = system.GetLocalToWorldMatrix();
                }
                Matrix4x4 matrix = Gizmos.matrix;
                Gizmos.matrix = identity;
                for (int i = 0; i < num; i++)
                {
                    ParticleSystem.Particle particle = particles[i];
                    Vector3 vector = particle.GetCurrentSize3D(system);
                    Gizmos.DrawWireSphere(particle.position, (Math.Max(vector.x, Math.Max(vector.y, vector.z)) * 0.5f) * system.trigger.radiusScale);
                }
                Gizmos.color = color;
                Gizmos.matrix = matrix;
            }
        }

        private enum OverlapOptions
        {
            Ignore,
            Kill,
            Callback
        }

        private class Texts
        {
            public GUIContent collisionShapes = EditorGUIUtility.TextContent("Colliders|The list of collision shapes to use for the trigger.");
            public GUIContent createCollisionShape = EditorGUIUtility.TextContent("|Create a GameObject containing a sphere collider and assigns it to the list.");
            public GUIContent enter = EditorGUIUtility.TextContent("Enter|Triggered once when particles enter the collison volume.");
            public GUIContent exit = EditorGUIUtility.TextContent("Exit|Triggered once when particles leave the collison volume.");
            public GUIContent inside = EditorGUIUtility.TextContent("Inside|What to do for particles that are inside the collision volume.");
            public GUIContent outside = EditorGUIUtility.TextContent("Outside|What to do for particles that are outside the collision volume.");
            public string[] overlapOptions = new string[] { "Ignore", "Kill", "Callback" };
            public GUIContent radiusScale = EditorGUIUtility.TextContent("Radius Scale|Scale particle bounds by this amount to get more precise collisions.");
            public GUIContent visualizeBounds = EditorGUIUtility.TextContent("Visualize Bounds|Render the collision bounds of the particles.");
        }
    }
}

