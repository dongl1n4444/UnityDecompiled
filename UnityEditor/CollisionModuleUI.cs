namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class CollisionModuleUI : ModuleUI
    {
        [CompilerGenerated]
        private static Handles.CapFunction <>f__mg$cache0;
        private const int k_MaxNumPlanes = 6;
        private SerializedMinMaxCurve m_Bounce;
        private SerializedProperty m_CollidesWith;
        private SerializedProperty m_CollidesWithDynamic;
        private SerializedProperty m_CollisionMessages;
        private SerializedProperty m_CollisionMode;
        private SerializedMinMaxCurve m_Dampen;
        private SerializedProperty m_InteriorCollisions;
        private SerializedMinMaxCurve m_LifetimeLossOnCollision;
        private SerializedProperty m_MaxCollisionShapes;
        private SerializedProperty m_MaxKillSpeed;
        private SerializedProperty m_MinKillSpeed;
        private SerializedProperty[] m_Planes;
        private PlaneVizType m_PlaneVisualizationType;
        private string[] m_PlaneVizTypeNames;
        private SerializedProperty m_Quality;
        private SerializedProperty m_RadiusScale;
        private float m_ScaleGrid;
        private SerializedProperty[] m_ShownPlanes;
        private SerializedProperty m_Type;
        private SerializedProperty m_VoxelSize;
        private static Transform s_SelectedTransform;
        private static Texts s_Texts;
        private static bool s_VisualizeBounds = false;

        public CollisionModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "CollisionModule", displayName)
        {
            this.m_PlaneVizTypeNames = new string[] { "Grid", "Solid" };
            this.m_Planes = new SerializedProperty[6];
            this.m_PlaneVisualizationType = PlaneVizType.Solid;
            this.m_ScaleGrid = 1f;
            base.m_ToolTip = "Allows you to specify multiple collision planes that the particle can collide with.";
        }

        private static GameObject CreateEmptyGameObject(string name, ParticleSystem parentOfGameObject)
        {
            GameObject obj2 = new GameObject(name);
            if (obj2 != null)
            {
                if (parentOfGameObject != null)
                {
                    obj2.transform.parent = parentOfGameObject.transform;
                }
                return obj2;
            }
            return null;
        }

        private void DoListOfPlanesGUI()
        {
            EditorGUI.BeginChangeCheck();
            int index = base.GUIListOfFloatObjectToggleFields(s_Texts.planes, this.m_ShownPlanes, null, s_Texts.createPlane, true, new GUILayoutOption[0]);
            bool flag = EditorGUI.EndChangeCheck();
            if (index >= 0)
            {
                GameObject obj2 = CreateEmptyGameObject("Plane Transform " + (index + 1), base.m_ParticleSystemUI.m_ParticleSystem);
                obj2.transform.localPosition = new Vector3(0f, 0f, (float) (10 + index));
                obj2.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
                this.m_ShownPlanes[index].objectReferenceValue = obj2;
                flag = true;
            }
            Rect position = GUILayoutUtility.GetRect((float) 0f, (float) 16f);
            position.x = (position.xMax - 24f) - 5f;
            position.width = 12f;
            if ((this.m_ShownPlanes.Length > 1) && ModuleUI.MinusButton(position))
            {
                this.m_ShownPlanes[this.m_ShownPlanes.Length - 1].objectReferenceValue = null;
                List<SerializedProperty> list = new List<SerializedProperty>(this.m_ShownPlanes);
                list.RemoveAt(list.Count - 1);
                this.m_ShownPlanes = list.ToArray();
            }
            if (this.m_ShownPlanes.Length < 6)
            {
                position.x += 17f;
                if (ModuleUI.PlusButton(position))
                {
                    List<SerializedProperty> list2 = new List<SerializedProperty>(this.m_ShownPlanes);
                    list2.Add(this.m_Planes[list2.Count]);
                    this.m_ShownPlanes = list2.ToArray();
                }
            }
            if (flag)
            {
                this.SyncVisualization();
            }
        }

        private void DrawGrid(Vector3 pos, Vector3 axis1, Vector3 axis2, Vector3 normal, Color color, int planeIndex)
        {
            if (Event.current.type == EventType.Repaint)
            {
                HandleUtility.ApplyWireMaterial();
                if (color.a > 0f)
                {
                    GL.Begin(1);
                    float num = 10f;
                    int num2 = 11;
                    num *= this.m_ScaleGrid;
                    num2 = (int) num;
                    num2 = Mathf.Clamp(num2, 10, 40);
                    if ((num2 % 2) == 0)
                    {
                        num2++;
                    }
                    float num3 = num * 0.5f;
                    float num4 = num / ((float) (num2 - 1));
                    Vector3 vector = (Vector3) (axis1 * num);
                    Vector3 vector2 = (Vector3) (axis2 * num);
                    Vector3 vector3 = (Vector3) (axis1 * num4);
                    Vector3 vector4 = (Vector3) (axis2 * num4);
                    Vector3 vector5 = (Vector3) ((pos - (axis1 * num3)) - (axis2 * num3));
                    for (int i = 0; i < num2; i++)
                    {
                        if ((i % 2) == 0)
                        {
                            GL.Color((Color) (color * 0.7f));
                        }
                        else
                        {
                            GL.Color(color);
                        }
                        GL.Vertex(vector5 + ((Vector3) (i * vector3)));
                        GL.Vertex((vector5 + ((Vector3) (i * vector3))) + vector2);
                        GL.Vertex(vector5 + ((Vector3) (i * vector4)));
                        GL.Vertex((vector5 + ((Vector3) (i * vector4))) + vector);
                    }
                    GL.Color(color);
                    GL.Vertex(pos);
                    GL.Vertex(pos + normal);
                    GL.End();
                }
            }
        }

        private void DrawSolidPlane(Vector3 pos, Quaternion rot, int planeIndex)
        {
        }

        protected override void Init()
        {
            if (this.m_Type == null)
            {
                if (s_Texts == null)
                {
                    s_Texts = new Texts();
                }
                this.m_Type = base.GetProperty("type");
                List<SerializedProperty> list = new List<SerializedProperty>();
                for (int i = 0; i < this.m_Planes.Length; i++)
                {
                    this.m_Planes[i] = base.GetProperty("plane" + i);
                    if ((i == 0) || (this.m_Planes[i].objectReferenceValue != null))
                    {
                        list.Add(this.m_Planes[i]);
                    }
                }
                this.m_ShownPlanes = list.ToArray();
                this.m_Dampen = new SerializedMinMaxCurve(this, s_Texts.dampen, "m_Dampen");
                this.m_Dampen.m_AllowCurves = false;
                this.m_Bounce = new SerializedMinMaxCurve(this, s_Texts.bounce, "m_Bounce");
                this.m_Bounce.m_AllowCurves = false;
                this.m_LifetimeLossOnCollision = new SerializedMinMaxCurve(this, s_Texts.lifetimeLoss, "m_EnergyLossOnCollision");
                this.m_LifetimeLossOnCollision.m_AllowCurves = false;
                this.m_MinKillSpeed = base.GetProperty("minKillSpeed");
                this.m_MaxKillSpeed = base.GetProperty("maxKillSpeed");
                this.m_RadiusScale = base.GetProperty("radiusScale");
                this.m_PlaneVisualizationType = (PlaneVizType) EditorPrefs.GetInt("PlaneColisionVizType", 1);
                this.m_ScaleGrid = EditorPrefs.GetFloat("ScalePlaneColision", 1f);
                s_VisualizeBounds = EditorPrefs.GetBool("VisualizeBounds", false);
                this.m_CollidesWith = base.GetProperty("collidesWith");
                this.m_CollidesWithDynamic = base.GetProperty("collidesWithDynamic");
                this.m_InteriorCollisions = base.GetProperty("interiorCollisions");
                this.m_MaxCollisionShapes = base.GetProperty("maxCollisionShapes");
                this.m_Quality = base.GetProperty("quality");
                this.m_VoxelSize = base.GetProperty("voxelSize");
                this.m_CollisionMessages = base.GetProperty("collisionMessages");
                this.m_CollisionMode = base.GetProperty("collisionMode");
                this.SyncVisualization();
            }
        }

        public override void OnInspectorGUI(ParticleSystem s)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            string[] options = new string[] { "Planes", "World" };
            EditorGUI.BeginChangeCheck();
            CollisionTypes types = (CollisionTypes) ModuleUI.GUIPopup("", this.m_Type, options, new GUILayoutOption[0]);
            bool flag = EditorGUI.EndChangeCheck();
            CollisionModes modes = CollisionModes.Mode3D;
            if (types == CollisionTypes.Plane)
            {
                this.DoListOfPlanesGUI();
                EditorGUI.BeginChangeCheck();
                this.m_PlaneVisualizationType = (PlaneVizType) ModuleUI.GUIPopup(s_Texts.visualization, (int) this.m_PlaneVisualizationType, this.m_PlaneVizTypeNames, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck() || flag)
                {
                    EditorPrefs.SetInt("PlaneColisionVizType", (int) this.m_PlaneVisualizationType);
                    if (this.m_PlaneVisualizationType == PlaneVizType.Solid)
                    {
                        this.SyncVisualization();
                    }
                    else
                    {
                        ParticleEffectUtils.ClearPlanes();
                    }
                }
                EditorGUI.BeginChangeCheck();
                this.m_ScaleGrid = ModuleUI.GUIFloat(s_Texts.scalePlane, this.m_ScaleGrid, "f2", new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_ScaleGrid = Mathf.Max(0f, this.m_ScaleGrid);
                    EditorPrefs.SetFloat("ScalePlaneColision", this.m_ScaleGrid);
                    this.SyncVisualization();
                }
            }
            else
            {
                ParticleEffectUtils.ClearPlanes();
                string[] textArray2 = new string[] { "3D", "2D" };
                modes = (CollisionModes) ModuleUI.GUIPopup(s_Texts.collisionMode, this.m_CollisionMode, textArray2, new GUILayoutOption[0]);
            }
            EditorGUI.BeginChangeCheck();
            s_VisualizeBounds = ModuleUI.GUIToggle(s_Texts.visualizeBounds, s_VisualizeBounds, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool("VisualizeBounds", s_VisualizeBounds);
            }
            ModuleUI.GUIMinMaxCurve(s_Texts.dampen, this.m_Dampen, new GUILayoutOption[0]);
            ModuleUI.GUIMinMaxCurve(s_Texts.bounce, this.m_Bounce, new GUILayoutOption[0]);
            ModuleUI.GUIMinMaxCurve(s_Texts.lifetimeLoss, this.m_LifetimeLossOnCollision, new GUILayoutOption[0]);
            ModuleUI.GUIFloat(s_Texts.minKillSpeed, this.m_MinKillSpeed, new GUILayoutOption[0]);
            ModuleUI.GUIFloat(s_Texts.maxKillSpeed, this.m_MaxKillSpeed, new GUILayoutOption[0]);
            ModuleUI.GUIFloat(s_Texts.radiusScale, this.m_RadiusScale, new GUILayoutOption[0]);
            if (types == CollisionTypes.World)
            {
                ModuleUI.GUILayerMask(s_Texts.collidesWith, this.m_CollidesWith, new GUILayoutOption[0]);
                if (modes == CollisionModes.Mode3D)
                {
                    ModuleUI.GUIToggle(s_Texts.interiorCollisions, this.m_InteriorCollisions, new GUILayoutOption[0]);
                }
                ModuleUI.GUIInt(s_Texts.maxCollisionShapes, this.m_MaxCollisionShapes, new GUILayoutOption[0]);
                ModuleUI.GUIPopup(s_Texts.quality, this.m_Quality, s_Texts.qualitySettings, new GUILayoutOption[0]);
                if (this.m_Quality.intValue == 0)
                {
                    ModuleUI.GUIToggle(s_Texts.collidesWithDynamic, this.m_CollidesWithDynamic, new GUILayoutOption[0]);
                }
                else
                {
                    ModuleUI.GUIFloat(s_Texts.voxelSize, this.m_VoxelSize, new GUILayoutOption[0]);
                }
            }
            ModuleUI.GUIToggle(s_Texts.collisionMessages, this.m_CollisionMessages, new GUILayoutOption[0]);
        }

        protected override void OnModuleDisable()
        {
            base.OnModuleDisable();
            ParticleEffectUtils.ClearPlanes();
        }

        protected override void OnModuleEnable()
        {
            base.OnModuleEnable();
            this.SyncVisualization();
        }

        public override void OnSceneGUI(ParticleSystem s, InitialModuleUI initial)
        {
            Event current = Event.current;
            EventType rawType = current.type;
            if ((current.type == EventType.Ignore) && (current.rawType == EventType.MouseUp))
            {
                rawType = current.rawType;
            }
            Color color = Handles.color;
            Color color2 = new Color(1f, 1f, 1f, 0.5f);
            Handles.color = color2;
            if (this.m_Type.intValue == 0)
            {
                for (int i = 0; i < this.m_ShownPlanes.Length; i++)
                {
                    Object objectReferenceValue = this.m_ShownPlanes[i].objectReferenceValue;
                    if (objectReferenceValue != null)
                    {
                        Transform objB = objectReferenceValue as Transform;
                        if (objB != null)
                        {
                            Vector3 position = objB.position;
                            Quaternion rotation = objB.rotation;
                            Vector3 vector2 = (Vector3) (rotation * Vector3.right);
                            Vector3 normal = (Vector3) (rotation * Vector3.up);
                            Vector3 vector4 = (Vector3) (rotation * Vector3.forward);
                            if (object.ReferenceEquals(s_SelectedTransform, objB))
                            {
                                Tools.s_Hidden = true;
                                EditorGUI.BeginChangeCheck();
                                if (Tools.current == Tool.Move)
                                {
                                    objB.position = Handles.PositionHandle(position, rotation);
                                }
                                else if (Tools.current == Tool.Rotate)
                                {
                                    objB.rotation = Handles.RotationHandle(rotation, position);
                                }
                                if (EditorGUI.EndChangeCheck())
                                {
                                    if (this.m_PlaneVisualizationType == PlaneVizType.Solid)
                                    {
                                        GameObject plane = ParticleEffectUtils.GetPlane(i);
                                        plane.transform.position = position;
                                        plane.transform.rotation = rotation;
                                        plane.transform.localScale = new Vector3(this.m_ScaleGrid, this.m_ScaleGrid, this.m_ScaleGrid);
                                    }
                                    ParticleSystemEditorUtils.PerformCompleteResimulation();
                                }
                            }
                            else
                            {
                                int keyboardControl = GUIUtility.keyboardControl;
                                float size = HandleUtility.GetHandleSize(position) * 0.06f;
                                if (<>f__mg$cache0 == null)
                                {
                                    <>f__mg$cache0 = new Handles.CapFunction(Handles.RectangleHandleCap);
                                }
                                Handles.FreeMoveHandle(position, Quaternion.identity, size, Vector3.zero, <>f__mg$cache0);
                                if (((rawType == EventType.MouseDown) && (current.type == EventType.Used)) && (keyboardControl != GUIUtility.keyboardControl))
                                {
                                    s_SelectedTransform = objB;
                                    rawType = EventType.Used;
                                }
                            }
                            if (this.m_PlaneVisualizationType == PlaneVizType.Grid)
                            {
                                Color color3 = (Color) (Handles.s_ColliderHandleColor * 0.9f);
                                if (!base.enabled)
                                {
                                    color3 = new Color(0.7f, 0.7f, 0.7f, 0.7f);
                                }
                                this.DrawGrid(position, vector2, vector4, normal, color3, i);
                            }
                            else
                            {
                                this.DrawSolidPlane(position, rotation, i);
                            }
                        }
                        else
                        {
                            Debug.LogError("Not a transform: " + objectReferenceValue.GetType());
                        }
                    }
                }
            }
            Handles.color = color;
        }

        [DrawGizmo(GizmoType.Active)]
        private static void RenderCollisionBounds(ParticleSystem system, GizmoType gizmoType)
        {
            if (system.collision.enabled && s_VisualizeBounds)
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
                    Gizmos.DrawWireSphere(particle.position, (Math.Max(vector.x, Math.Max(vector.y, vector.z)) * 0.5f) * system.collision.radiusScale);
                }
                Gizmos.color = color;
                Gizmos.matrix = matrix;
            }
        }

        protected override void SetVisibilityState(ModuleUI.VisibilityState newState)
        {
            base.SetVisibilityState(newState);
            if (newState != ModuleUI.VisibilityState.VisibleAndFoldedOut)
            {
                Tools.s_Hidden = false;
                s_SelectedTransform = null;
                ParticleEffectUtils.ClearPlanes();
            }
            else
            {
                this.SyncVisualization();
            }
        }

        private void SyncVisualization()
        {
            if (base.enabled && (this.m_PlaneVisualizationType == PlaneVizType.Solid))
            {
                for (int i = 0; i < this.m_ShownPlanes.Length; i++)
                {
                    Object objectReferenceValue = this.m_ShownPlanes[i].objectReferenceValue;
                    if (objectReferenceValue == null)
                    {
                        ParticleEffectUtils.HidePlaneIfExists(i);
                    }
                    else
                    {
                        Transform transform = objectReferenceValue as Transform;
                        if (transform == null)
                        {
                            ParticleEffectUtils.HidePlaneIfExists(i);
                        }
                        else
                        {
                            GameObject plane = ParticleEffectUtils.GetPlane(i);
                            plane.transform.position = transform.position;
                            plane.transform.rotation = transform.rotation;
                            plane.transform.localScale = new Vector3(this.m_ScaleGrid, this.m_ScaleGrid, this.m_ScaleGrid);
                            Transform transform1 = plane.transform;
                            transform1.position += (Vector3) (transform.up.normalized * 0.002f);
                        }
                    }
                }
            }
        }

        public override void UpdateCullingSupportedString(ref string text)
        {
            text = text + "\n\tCollision is enabled.";
        }

        private enum CollisionModes
        {
            Mode3D,
            Mode2D
        }

        private enum CollisionTypes
        {
            Plane,
            World
        }

        private enum PlaneVizType
        {
            Grid,
            Solid
        }

        private class Texts
        {
            public GUIContent bounce = EditorGUIUtility.TextContent("Bounce|When particle collides, the bounce is scaled with this value. The bounce is the upwards motion in the plane normal direction.");
            public GUIContent collidesWith = EditorGUIUtility.TextContent("Collides With|Collides the particles with colliders included in the layermask.");
            public GUIContent collidesWithDynamic = EditorGUIUtility.TextContent("Enable Dynamic Colliders|Should particles collide with dynamic objects?");
            public GUIContent collisionMessages = EditorGUIUtility.TextContent("Send Collision Messages|Send collision callback messages.");
            public GUIContent collisionMode = EditorGUIUtility.TextContent("Collision Mode|Use 3D Physics or 2D Physics.");
            public GUIContent createPlane = EditorGUIUtility.TextContent("|Create an empty GameObject and assign it as a plane.");
            public GUIContent dampen = EditorGUIUtility.TextContent("Dampen|When particle collides, it will lose this fraction of its speed. Unless this is set to 0.0, particle will become slower after collision.");
            public GUIContent interiorCollisions = EditorGUIUtility.TextContent("Interior Collisions|Should particles collide with the insides of objects?");
            public GUIContent lifetimeLoss = EditorGUIUtility.TextContent("Lifetime Loss|When particle collides, it will lose this fraction of its Start Lifetime");
            public GUIContent maxCollisionShapes = EditorGUIUtility.TextContent("Max Collision Shapes|How many collision shapes can be considered for particle collisions. Excess shapes will be ignored. Terrains take priority.");
            public GUIContent maxKillSpeed = EditorGUIUtility.TextContent("Max Kill Speed|When particles collide and their speed is higher than this value, they are killed.");
            public GUIContent minKillSpeed = EditorGUIUtility.TextContent("Min Kill Speed|When particles collide and their speed is lower than this value, they are killed.");
            public GUIContent planes = EditorGUIUtility.TextContent("Planes|Planes are defined by assigning a reference to a transform. This transform can be any transform in the scene and can be animated. Multiple planes can be used. Note: the Y-axis is used as the plane normal.");
            public GUIContent quality = EditorGUIUtility.TextContent("Collision Quality|Quality of world collisions. Medium and low quality are approximate and may leak particles.");
            public string[] qualitySettings = new string[] { "High", "Medium (Static Colliders)", "Low (Static Colliders)" };
            public GUIContent radiusScale = EditorGUIUtility.TextContent("Radius Scale|Scale particle bounds by this amount to get more precise collisions.");
            public GUIContent scalePlane = EditorGUIUtility.TextContent("Scale Plane|Resizes the visualization planes.");
            public GUIContent visualization = EditorGUIUtility.TextContent("Visualization|Only used for visualizing the planes: Wireframe or Solid.");
            public GUIContent visualizeBounds = EditorGUIUtility.TextContent("Visualize Bounds|Render the collision bounds of the particles.");
            public GUIContent voxelSize = EditorGUIUtility.TextContent("Voxel Size|Size of voxels in the collision cache.");
        }
    }
}

