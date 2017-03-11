namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
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
        private static PlaneVizType m_PlaneVisualizationType = PlaneVizType.Solid;
        private string[] m_PlaneVizTypeNames;
        private SerializedProperty m_Quality;
        private SerializedProperty m_RadiusScale;
        private static float m_ScaleGrid = 1f;
        private List<Transform> m_ScenePlanes;
        private SerializedProperty[] m_ShownPlanes;
        private SerializedProperty m_Type;
        private SerializedProperty m_VoxelSize;
        private static Transform s_SelectedTransform;
        private static Texts s_Texts = new Texts();
        private static bool s_VisualizeBounds = false;

        public CollisionModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "CollisionModule", displayName)
        {
            this.m_PlaneVizTypeNames = new string[] { "Grid", "Solid" };
            this.m_Planes = new SerializedProperty[6];
            this.m_ScenePlanes = new List<Transform>();
            base.m_ToolTip = "Allows you to specify multiple collision planes that the particle can collide with.";
        }

        private void CollisionPlanesSceneGUI()
        {
            if (this.m_ScenePlanes.Count != 0)
            {
                Event current = Event.current;
                EventType rawType = current.type;
                if ((current.type == EventType.Ignore) && (current.rawType == EventType.MouseUp))
                {
                    rawType = current.rawType;
                }
                Color color = Handles.color;
                Color color2 = new Color(1f, 1f, 1f, 0.5f);
                for (int i = 0; i < this.m_ScenePlanes.Count; i++)
                {
                    if (this.m_ScenePlanes[i] != null)
                    {
                        Transform objB = this.m_ScenePlanes[i];
                        Vector3 position = objB.position;
                        Quaternion rotation = objB.rotation;
                        Vector3 vector2 = (Vector3) (rotation * Vector3.right);
                        Vector3 normal = (Vector3) (rotation * Vector3.up);
                        Vector3 vector4 = (Vector3) (rotation * Vector3.forward);
                        bool disabled = EditorApplication.isPlaying && objB.gameObject.isStatic;
                        if (this.editingPlanes)
                        {
                            if (object.ReferenceEquals(s_SelectedTransform, objB))
                            {
                                EditorGUI.BeginChangeCheck();
                                Vector3 vector5 = objB.position;
                                Quaternion quaternion2 = objB.rotation;
                                using (new EditorGUI.DisabledScope(disabled))
                                {
                                    if (disabled)
                                    {
                                        Handles.ShowStaticLabel(position);
                                    }
                                    if (UnityEditorInternal.EditMode.editMode == UnityEditorInternal.EditMode.SceneViewEditMode.ParticleSystemCollisionModulePlanesMove)
                                    {
                                        vector5 = Handles.PositionHandle(position, rotation);
                                    }
                                    else if (UnityEditorInternal.EditMode.editMode == UnityEditorInternal.EditMode.SceneViewEditMode.ParticleSystemCollisionModulePlanesRotate)
                                    {
                                        quaternion2 = Handles.RotationHandle(rotation, position);
                                    }
                                }
                                if (EditorGUI.EndChangeCheck())
                                {
                                    Undo.RecordObject(objB, "Modified Collision Plane Transform");
                                    objB.position = vector5;
                                    objB.rotation = quaternion2;
                                    ParticleSystemEditorUtils.PerformCompleteResimulation();
                                }
                            }
                            else
                            {
                                int keyboardControl = GUIUtility.keyboardControl;
                                float size = HandleUtility.GetHandleSize(position) * 0.6f;
                                if (<>f__mg$cache0 == null)
                                {
                                    <>f__mg$cache0 = new Handles.CapFunction(Handles.RectangleHandleCap);
                                }
                                Handles.FreeMoveHandle(position, Quaternion.identity, size, Vector3.zero, <>f__mg$cache0);
                                if (((rawType == EventType.MouseDown) && (current.type == EventType.Used)) && (keyboardControl != GUIUtility.keyboardControl))
                                {
                                    s_SelectedTransform = objB;
                                    rawType = EventType.Used;
                                    GUIUtility.hotControl = 0;
                                }
                            }
                        }
                        Handles.color = color2;
                        Color color3 = (Color) (Handles.s_ColliderHandleColor * 0.9f);
                        if (disabled)
                        {
                            color3.a *= 0.2f;
                        }
                        if (m_PlaneVisualizationType == PlaneVizType.Grid)
                        {
                            DrawGrid(position, vector2, vector4, normal, color3);
                        }
                        else
                        {
                            DrawSolidPlane(position, rotation, color3, Color.yellow);
                        }
                    }
                }
                Handles.color = color;
            }
        }

        private static GameObject CreateEmptyGameObject(string name, ParticleSystem parentOfGameObject)
        {
            GameObject objectToUndo = new GameObject(name);
            if (objectToUndo != null)
            {
                if (parentOfGameObject != null)
                {
                    objectToUndo.transform.parent = parentOfGameObject.transform;
                }
                Undo.RegisterCreatedObjectUndo(objectToUndo, "Created `" + name + "`");
                return objectToUndo;
            }
            return null;
        }

        private void DoListOfPlanesGUI()
        {
            if (!this.IsListOfPlanesValid())
            {
                EditorGUILayout.HelpBox("Plane list editing is only available when all selected systems contain the same number of planes", MessageType.Info, true);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                int index = base.GUIListOfFloatObjectToggleFields(s_Texts.planes, this.m_ShownPlanes, null, s_Texts.createPlane, !base.m_ParticleSystemUI.multiEdit, new GUILayoutOption[0]);
                bool flag = EditorGUI.EndChangeCheck();
                if ((index >= 0) && !base.m_ParticleSystemUI.multiEdit)
                {
                    GameObject obj2 = CreateEmptyGameObject("Plane Transform " + (index + 1), base.m_ParticleSystemUI.m_ParticleSystems[0]);
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
                    flag = true;
                }
                if ((this.m_ShownPlanes.Length < 6) && !base.m_ParticleSystemUI.multiEdit)
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
        }

        private static void DrawGrid(Vector3 pos, Vector3 axis1, Vector3 axis2, Vector3 normal, Color color)
        {
            if (Event.current.type == EventType.Repaint)
            {
                HandleUtility.ApplyWireMaterial();
                if (color.a > 0f)
                {
                    GL.Begin(1);
                    float num = 10f;
                    int num2 = 11;
                    num *= m_ScaleGrid;
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

        private static void DrawSolidPlane(Vector3 pos, Quaternion rot, Color faceColor, Color edgeColor)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Matrix4x4 matrix = Handles.matrix;
                float x = 10f * m_ScaleGrid;
                Handles.matrix = Matrix4x4.TRS(pos, rot, new Vector3(x, x, x)) * Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(90f, 0f, 0f), Vector3.one);
                Handles.DrawSolidRectangleWithOutline(new Rect(-0.5f, -0.5f, 1f, 1f), faceColor, edgeColor);
                Handles.DrawLine(Vector3.zero, (Vector3) (Vector3.back / x));
                Handles.matrix = matrix;
            }
        }

        private Bounds GetBounds()
        {
            Bounds bounds = new Bounds();
            foreach (ParticleSystem system in base.m_ParticleSystemUI.m_ParticleSystems)
            {
                ParticleSystemRenderer component = system.GetComponent<ParticleSystemRenderer>();
                bounds.Encapsulate(component.bounds);
            }
            return bounds;
        }

        protected override void Init()
        {
            if (this.m_Type == null)
            {
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
                m_PlaneVisualizationType = (PlaneVizType) EditorPrefs.GetInt("PlaneColisionVizType", 1);
                m_ScaleGrid = EditorPrefs.GetFloat("ScalePlaneColision", 1f);
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

        private bool IsListOfPlanesValid()
        {
            if (base.m_ParticleSystemUI.multiEdit)
            {
                for (int i = 0; i < 6; i++)
                {
                    int num2 = -1;
                    foreach (ParticleSystem system in base.m_ParticleSystemUI.m_ParticleSystems)
                    {
                        int num4 = (system.collision.GetPlane(i) == null) ? 0 : 1;
                        if (num2 == -1)
                        {
                            num2 = num4;
                        }
                        else if (num4 != num2)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public override void OnInspectorGUI(InitialModuleUI initial)
        {
            string[] options = new string[] { "Planes", "World" };
            EditorGUI.BeginChangeCheck();
            CollisionTypes types = (CollisionTypes) ModuleUI.GUIPopup("", this.m_Type, options, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.SyncVisualization();
            }
            CollisionModes modes = CollisionModes.Mode3D;
            if (types == CollisionTypes.Plane)
            {
                this.DoListOfPlanesGUI();
                EditorGUI.BeginChangeCheck();
                m_PlaneVisualizationType = (PlaneVizType) ModuleUI.GUIPopup(s_Texts.visualization, (int) m_PlaneVisualizationType, this.m_PlaneVizTypeNames, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetInt("PlaneColisionVizType", (int) m_PlaneVisualizationType);
                }
                EditorGUI.BeginChangeCheck();
                m_ScaleGrid = ModuleUI.GUIFloat(s_Texts.scalePlane, m_ScaleGrid, "f2", new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    m_ScaleGrid = Mathf.Max(0f, m_ScaleGrid);
                    EditorPrefs.SetFloat("ScalePlaneColision", m_ScaleGrid);
                }
                ModuleUI.GUIButtonGroup(s_Texts.sceneViewEditModes, s_Texts.toolContents, this.GetBounds(), base.m_ParticleSystemUI.m_ParticleEffectUI.m_Owner.customEditor);
            }
            else
            {
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
            this.editingPlanes = false;
        }

        protected override void OnModuleEnable()
        {
            base.OnModuleEnable();
            this.SyncVisualization();
        }

        public override void OnSceneViewGUI()
        {
            this.RenderCollisionBounds();
            this.CollisionPlanesSceneGUI();
        }

        private void RenderCollisionBounds()
        {
            if (s_VisualizeBounds)
            {
                Color color = Handles.color;
                Handles.color = Color.green;
                Matrix4x4 matrix = Handles.matrix;
                Vector3[] dest = new Vector3[20];
                Vector3[] vectorArray2 = new Vector3[20];
                Vector3[] vectorArray3 = new Vector3[20];
                Handles.SetDiscSectionPoints(dest, Vector3.zero, Vector3.forward, Vector3.right, 360f, 1f);
                Handles.SetDiscSectionPoints(vectorArray2, Vector3.zero, Vector3.up, -Vector3.right, 360f, 1f);
                Handles.SetDiscSectionPoints(vectorArray3, Vector3.zero, Vector3.right, Vector3.up, 360f, 1f);
                Vector3[] array = new Vector3[(dest.Length + vectorArray2.Length) + vectorArray3.Length];
                dest.CopyTo(array, 0);
                vectorArray2.CopyTo(array, 20);
                vectorArray3.CopyTo(array, 40);
                foreach (ParticleSystem system in base.m_ParticleSystemUI.m_ParticleSystems)
                {
                    ParticleSystem.Particle[] particles = new ParticleSystem.Particle[system.particleCount];
                    int num2 = system.GetParticles(particles);
                    Matrix4x4 identity = Matrix4x4.identity;
                    if (system.main.simulationSpace == ParticleSystemSimulationSpace.Local)
                    {
                        identity = system.GetLocalToWorldMatrix();
                    }
                    for (int i = 0; i < num2; i++)
                    {
                        ParticleSystem.Particle particle = particles[i];
                        Vector3 vector = particle.GetCurrentSize3D(system);
                        float x = (Math.Max(vector.x, Math.Max(vector.y, vector.z)) * 0.5f) * system.collision.radiusScale;
                        Handles.matrix = identity * Matrix4x4.TRS(particle.position, Quaternion.identity, new Vector3(x, x, x));
                        Handles.DrawPolyLine(array);
                    }
                }
                Handles.color = color;
                Handles.matrix = matrix;
            }
        }

        protected override void SetVisibilityState(ModuleUI.VisibilityState newState)
        {
            base.SetVisibilityState(newState);
            if (newState != ModuleUI.VisibilityState.VisibleAndFoldedOut)
            {
                s_SelectedTransform = null;
                this.editingPlanes = false;
            }
            else
            {
                this.SyncVisualization();
            }
        }

        private void SyncVisualization()
        {
            this.m_ScenePlanes.Clear();
            if (base.m_ParticleSystemUI.multiEdit)
            {
                foreach (ParticleSystem system in base.m_ParticleSystemUI.m_ParticleSystems)
                {
                    if (system.collision.type == ParticleSystemCollisionType.Planes)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            Transform plane = system.collision.GetPlane(i);
                            if ((plane != null) && !this.m_ScenePlanes.Contains(plane))
                            {
                                this.m_ScenePlanes.Add(plane);
                            }
                        }
                    }
                }
            }
            else if (this.m_Type.intValue != 0)
            {
                this.editingPlanes = false;
            }
            else
            {
                for (int j = 0; j < this.m_ShownPlanes.Length; j++)
                {
                    Transform objectReferenceValue = this.m_ShownPlanes[j].objectReferenceValue as Transform;
                    if ((objectReferenceValue != null) && !this.m_ScenePlanes.Contains(objectReferenceValue))
                    {
                        this.m_ScenePlanes.Add(objectReferenceValue);
                    }
                }
            }
        }

        public override void UndoRedoPerformed()
        {
            base.UndoRedoPerformed();
            this.SyncVisualization();
        }

        public override void UpdateCullingSupportedString(ref string text)
        {
            text = text + "\n\tCollision is enabled.";
        }

        private bool editingPlanes
        {
            get => 
                (((UnityEditorInternal.EditMode.editMode == UnityEditorInternal.EditMode.SceneViewEditMode.ParticleSystemCollisionModulePlanesMove) || (UnityEditorInternal.EditMode.editMode == UnityEditorInternal.EditMode.SceneViewEditMode.ParticleSystemCollisionModulePlanesRotate)) && UnityEditorInternal.EditMode.IsOwner(base.m_ParticleSystemUI.m_ParticleEffectUI.m_Owner.customEditor));
            set
            {
                if (!value && this.editingPlanes)
                {
                    UnityEditorInternal.EditMode.QuitEditMode();
                }
                SceneView.RepaintAll();
            }
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
            public UnityEditorInternal.EditMode.SceneViewEditMode[] sceneViewEditModes = new UnityEditorInternal.EditMode.SceneViewEditMode[] { UnityEditorInternal.EditMode.SceneViewEditMode.ParticleSystemCollisionModulePlanesMove, UnityEditorInternal.EditMode.SceneViewEditMode.ParticleSystemCollisionModulePlanesRotate };
            public GUIContent[] toolContents = new GUIContent[] { EditorGUIUtility.IconContent("MoveTool", "|Move plane editing mode."), EditorGUIUtility.IconContent("RotateTool", "|Rotate plane editing mode.") };
            public GUIContent visualization = EditorGUIUtility.TextContent("Visualization|Only used for visualizing the planes: Wireframe or Solid.");
            public GUIContent visualizeBounds = EditorGUIUtility.TextContent("Visualize Bounds|Render the collision bounds of the particles.");
            public GUIContent voxelSize = EditorGUIUtility.TextContent("Voxel Size|Size of voxels in the collision cache. Smaller values improve accuracy, but require higher memory usage and are less efficient.");
        }
    }
}

