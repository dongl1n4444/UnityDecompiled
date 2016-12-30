namespace UnityEditor
{
    using System;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal class ShapeModuleUI : ModuleUI
    {
        private readonly ParticleSystemShapeType[] boxShapes;
        private readonly ParticleSystemShapeType[] coneShapes;
        private SerializedProperty m_AlignToDirection;
        private SerializedProperty m_Angle;
        private SerializedProperty m_Arc;
        private BoxBoundsHandle m_BoxBoundsHandle;
        private SerializedProperty m_BoxX;
        private SerializedProperty m_BoxY;
        private SerializedProperty m_BoxZ;
        private readonly string[] m_GuiNames;
        private readonly ParticleSystemShapeType[] m_GuiTypes;
        private SerializedProperty m_Length;
        private Material m_Material;
        private SerializedProperty m_Mesh;
        private SerializedProperty m_MeshMaterialIndex;
        private SerializedProperty m_MeshNormalOffset;
        private SerializedProperty m_MeshRenderer;
        private SerializedProperty m_MeshScale;
        private SerializedProperty m_PlacementMode;
        private SerializedProperty m_Radius;
        private SerializedProperty m_RandomDirectionAmount;
        private SerializedProperty m_SkinnedMeshRenderer;
        private SerializedProperty m_SphericalDirectionAmount;
        private SerializedProperty m_Type;
        private readonly int[] m_TypeToGuiTypeIndex;
        private SerializedProperty m_UseMeshColors;
        private SerializedProperty m_UseMeshMaterialIndex;
        private static int s_BoxHandleControlIDHint = typeof(ShapeModuleUI).Name.GetHashCode();
        private static Color s_ShapeGizmoColor = new Color(0.5803922f, 0.8980392f, 1f, 0.9f);
        private static Texts s_Texts = new Texts();
        private readonly ParticleSystemShapeType[] shellShapes;

        public ShapeModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "ShapeModule", displayName, ModuleUI.VisibilityState.VisibleAndFolded)
        {
            this.m_BoxBoundsHandle = new BoxBoundsHandle(s_BoxHandleControlIDHint);
            this.m_GuiNames = new string[] { "Sphere", "Hemisphere", "Cone", "Box", "Mesh", "Mesh Renderer", "Skinned Mesh Renderer", "Circle", "Edge" };
            this.m_GuiTypes = new ParticleSystemShapeType[] { ParticleSystemShapeType.Sphere };
            this.m_TypeToGuiTypeIndex = new int[] { 
                0, 0, 1, 1, 2, 3, 4, 2, 2, 2, 7, 7, 8, 5, 6, 3,
                3
            };
            this.boxShapes = new ParticleSystemShapeType[] { ParticleSystemShapeType.Box };
            this.coneShapes = new ParticleSystemShapeType[] { ParticleSystemShapeType.Cone };
            this.shellShapes = new ParticleSystemShapeType[] { ParticleSystemShapeType.BoxShell };
            base.m_ToolTip = "Shape of the emitter volume, which controls where particles are emitted and their initial direction.";
        }

        private ParticleSystemShapeType ConvertBoxEmitFromToConeType(int emitFrom) => 
            this.boxShapes[emitFrom];

        private int ConvertBoxTypeToConeEmitFrom(ParticleSystemShapeType shapeType) => 
            Array.IndexOf<ParticleSystemShapeType>(this.boxShapes, shapeType);

        private ParticleSystemShapeType ConvertConeEmitFromToConeType(int emitFrom) => 
            this.coneShapes[emitFrom];

        private int ConvertConeTypeToConeEmitFrom(ParticleSystemShapeType shapeType) => 
            Array.IndexOf<ParticleSystemShapeType>(this.coneShapes, shapeType);

        private bool GetUsesShell(ParticleSystemShapeType shapeType) => 
            (Array.IndexOf<ParticleSystemShapeType>(this.shellShapes, shapeType) != -1);

        public override float GetXAxisScalar() => 
            base.m_ParticleSystemUI.GetEmitterDuration();

        protected override void Init()
        {
            if (this.m_Type == null)
            {
                if (s_Texts == null)
                {
                    s_Texts = new Texts();
                }
                this.m_Type = base.GetProperty("type");
                this.m_Radius = base.GetProperty("radius");
                this.m_Angle = base.GetProperty("angle");
                this.m_Length = base.GetProperty("length");
                this.m_BoxX = base.GetProperty("boxX");
                this.m_BoxY = base.GetProperty("boxY");
                this.m_BoxZ = base.GetProperty("boxZ");
                this.m_Arc = base.GetProperty("arc");
                this.m_PlacementMode = base.GetProperty("placementMode");
                this.m_Mesh = base.GetProperty("m_Mesh");
                this.m_MeshRenderer = base.GetProperty("m_MeshRenderer");
                this.m_SkinnedMeshRenderer = base.GetProperty("m_SkinnedMeshRenderer");
                this.m_MeshMaterialIndex = base.GetProperty("m_MeshMaterialIndex");
                this.m_UseMeshMaterialIndex = base.GetProperty("m_UseMeshMaterialIndex");
                this.m_UseMeshColors = base.GetProperty("m_UseMeshColors");
                this.m_MeshNormalOffset = base.GetProperty("m_MeshNormalOffset");
                this.m_MeshScale = base.GetProperty("m_MeshScale");
                this.m_RandomDirectionAmount = base.GetProperty("randomDirectionAmount");
                this.m_SphericalDirectionAmount = base.GetProperty("sphericalDirectionAmount");
                this.m_AlignToDirection = base.GetProperty("alignToDirection");
                this.m_Material = EditorGUIUtility.GetBuiltinExtraResource(typeof(Material), "Default-Material.mat") as Material;
            }
        }

        public override void OnInspectorGUI(InitialModuleUI initial)
        {
            Material sharedMaterial;
            Mesh sharedMesh;
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            int intValue = this.m_Type.intValue;
            int num2 = this.m_TypeToGuiTypeIndex[intValue];
            bool usesShell = this.GetUsesShell((ParticleSystemShapeType) intValue);
            EditorGUI.BeginChangeCheck();
            int index = ModuleUI.GUIPopup(s_Texts.shape, num2, this.m_GuiNames, new GUILayoutOption[0]);
            bool flag2 = EditorGUI.EndChangeCheck();
            ParticleSystemShapeType type = this.m_GuiTypes[index];
            if (index != num2)
            {
                intValue = (int) type;
            }
            switch (type)
            {
                case ParticleSystemShapeType.Sphere:
                    ModuleUI.GUIFloat(s_Texts.radius, this.m_Radius, new GUILayoutOption[0]);
                    intValue = !ModuleUI.GUIToggle(s_Texts.emitFromShell, usesShell, new GUILayoutOption[0]) ? 0 : 1;
                    goto Label_058D;

                case ParticleSystemShapeType.Hemisphere:
                    ModuleUI.GUIFloat(s_Texts.radius, this.m_Radius, new GUILayoutOption[0]);
                    intValue = !ModuleUI.GUIToggle(s_Texts.emitFromShell, usesShell, new GUILayoutOption[0]) ? 2 : 3;
                    goto Label_058D;

                case ParticleSystemShapeType.Cone:
                {
                    ModuleUI.GUIFloat(s_Texts.coneAngle, this.m_Angle, new GUILayoutOption[0]);
                    ModuleUI.GUIFloat(s_Texts.radius, this.m_Radius, new GUILayoutOption[0]);
                    bool disabled = (intValue != 8) && (intValue != 9);
                    using (new EditorGUI.DisabledScope(disabled))
                    {
                        ModuleUI.GUIFloat(s_Texts.coneLength, this.m_Length, new GUILayoutOption[0]);
                    }
                    string[] options = new string[] { "Base", "Base Shell", "Volume", "Volume Shell" };
                    int num5 = this.ConvertConeTypeToConeEmitFrom((ParticleSystemShapeType) intValue);
                    num5 = ModuleUI.GUIPopup(s_Texts.emitFrom, num5, options, new GUILayoutOption[0]);
                    intValue = (int) this.ConvertConeEmitFromToConeType(num5);
                    goto Label_058D;
                }
                case ParticleSystemShapeType.Box:
                {
                    ModuleUI.GUIFloat(s_Texts.boxX, this.m_BoxX, new GUILayoutOption[0]);
                    ModuleUI.GUIFloat(s_Texts.boxY, this.m_BoxY, new GUILayoutOption[0]);
                    ModuleUI.GUIFloat(s_Texts.boxZ, this.m_BoxZ, new GUILayoutOption[0]);
                    string[] strArray = new string[] { "Volume", "Shell", "Edge" };
                    int num4 = this.ConvertBoxTypeToConeEmitFrom((ParticleSystemShapeType) intValue);
                    num4 = ModuleUI.GUIPopup(s_Texts.emitFrom, num4, strArray, new GUILayoutOption[0]);
                    intValue = (int) this.ConvertBoxEmitFromToConeType(num4);
                    goto Label_058D;
                }
                case ParticleSystemShapeType.Mesh:
                case ParticleSystemShapeType.MeshRenderer:
                case ParticleSystemShapeType.SkinnedMeshRenderer:
                {
                    string[] strArray3 = new string[] { "Vertex", "Edge", "Triangle" };
                    ModuleUI.GUIPopup("", this.m_PlacementMode, strArray3, new GUILayoutOption[0]);
                    sharedMaterial = null;
                    sharedMesh = null;
                    if (type != ParticleSystemShapeType.Mesh)
                    {
                        if (type == ParticleSystemShapeType.MeshRenderer)
                        {
                            ModuleUI.GUIObject(s_Texts.meshRenderer, this.m_MeshRenderer, new GUILayoutOption[0]);
                            MeshRenderer objectReferenceValue = (MeshRenderer) this.m_MeshRenderer.objectReferenceValue;
                            if (objectReferenceValue != null)
                            {
                                sharedMaterial = objectReferenceValue.sharedMaterial;
                                if (objectReferenceValue.GetComponent<MeshFilter>() != null)
                                {
                                    sharedMesh = objectReferenceValue.GetComponent<MeshFilter>().sharedMesh;
                                }
                            }
                        }
                        else
                        {
                            ModuleUI.GUIObject(s_Texts.skinnedMeshRenderer, this.m_SkinnedMeshRenderer, new GUILayoutOption[0]);
                            SkinnedMeshRenderer renderer2 = (SkinnedMeshRenderer) this.m_SkinnedMeshRenderer.objectReferenceValue;
                            if (renderer2 != null)
                            {
                                sharedMaterial = renderer2.sharedMaterial;
                                sharedMesh = renderer2.sharedMesh;
                            }
                        }
                        break;
                    }
                    ModuleUI.GUIObject(s_Texts.mesh, this.m_Mesh, new GUILayoutOption[0]);
                    break;
                }
                case ParticleSystemShapeType.Circle:
                    ModuleUI.GUIFloat(s_Texts.radius, this.m_Radius, new GUILayoutOption[0]);
                    ModuleUI.GUIFloat(s_Texts.arc, this.m_Arc, new GUILayoutOption[0]);
                    intValue = !ModuleUI.GUIToggle(s_Texts.emitFromEdge, usesShell, new GUILayoutOption[0]) ? 10 : 11;
                    goto Label_058D;

                case ParticleSystemShapeType.SingleSidedEdge:
                    ModuleUI.GUIFloat(s_Texts.radius, this.m_Radius, new GUILayoutOption[0]);
                    goto Label_058D;

                default:
                    goto Label_058D;
            }
            ModuleUI.GUIToggleWithIntField(s_Texts.meshMaterialIndex, this.m_UseMeshMaterialIndex, this.m_MeshMaterialIndex, false, new GUILayoutOption[0]);
            if (ModuleUI.GUIToggle(s_Texts.useMeshColors, this.m_UseMeshColors, new GUILayoutOption[0]) && ((sharedMaterial != null) && (sharedMesh != null)))
            {
                int nameID = Shader.PropertyToID("_Color");
                int num7 = Shader.PropertyToID("_TintColor");
                if ((!sharedMaterial.HasProperty(nameID) && !sharedMaterial.HasProperty(num7)) && !sharedMesh.HasChannel(Mesh.InternalShaderChannel.Color))
                {
                    EditorGUILayout.HelpBox(EditorGUIUtility.TextContent("To use mesh colors, your source mesh must either provide vertex colors, or its shader must contain a color property named \"_Color\" or \"_TintColor\".").text, MessageType.Warning, true);
                }
            }
            ModuleUI.GUIFloat(s_Texts.meshNormalOffset, this.m_MeshNormalOffset, new GUILayoutOption[0]);
            ModuleUI.GUIFloat(s_Texts.meshScale, this.m_MeshScale, new GUILayoutOption[0]);
        Label_058D:
            if (flag2 || !this.m_Type.hasMultipleDifferentValues)
            {
                this.m_Type.intValue = intValue;
            }
            ModuleUI.GUIToggle(s_Texts.alignToDirection, this.m_AlignToDirection, new GUILayoutOption[0]);
            ModuleUI.GUIFloat(s_Texts.randomDirectionAmount, this.m_RandomDirectionAmount, new GUILayoutOption[0]);
            ModuleUI.GUIFloat(s_Texts.sphericalDirectionAmount, this.m_SphericalDirectionAmount, new GUILayoutOption[0]);
        }

        public override void OnSceneGUI()
        {
            Color color = Handles.color;
            Handles.color = s_ShapeGizmoColor;
            Matrix4x4 matrix = Handles.matrix;
            EditorGUI.BeginChangeCheck();
            foreach (ParticleSystem system in base.m_ParticleSystemUI.m_ParticleSystems)
            {
                ParticleSystem.ShapeModule shape = system.shape;
                ParticleSystem.MainModule main = system.main;
                ParticleSystemShapeType shapeType = shape.shapeType;
                Matrix4x4 matrixx2 = new Matrix4x4();
                float x = (shapeType != ParticleSystemShapeType.Mesh) ? 1f : shape.meshScale;
                if (main.scalingMode == ParticleSystemScalingMode.Local)
                {
                    matrixx2.SetTRS(system.transform.position, system.transform.rotation, (Vector3) (system.transform.localScale * x));
                }
                else if (main.scalingMode == ParticleSystemScalingMode.Hierarchy)
                {
                    matrixx2 = system.transform.localToWorldMatrix * Matrix4x4.Scale(new Vector3(x, x, x));
                }
                else
                {
                    matrixx2.SetTRS(system.transform.position, system.transform.rotation, (Vector3) (system.transform.lossyScale * x));
                }
                Handles.matrix = matrixx2;
                switch (shapeType)
                {
                    case ParticleSystemShapeType.Sphere:
                    case ParticleSystemShapeType.SphereShell:
                        shape.radius = Handles.DoSimpleRadiusHandle(Quaternion.identity, Vector3.zero, shape.radius, false);
                        break;
                }
                if ((shapeType == ParticleSystemShapeType.Circle) || (shapeType == ParticleSystemShapeType.CircleEdge))
                {
                    float radius = shape.radius;
                    float arc = shape.arc;
                    Handles.DoSimpleRadiusArcHandleXY(Quaternion.identity, Vector3.zero, ref radius, ref arc);
                    shape.radius = radius;
                    shape.arc = arc;
                }
                else if ((shapeType == ParticleSystemShapeType.Hemisphere) || (shapeType == ParticleSystemShapeType.HemisphereShell))
                {
                    shape.radius = Handles.DoSimpleRadiusHandle(Quaternion.identity, Vector3.zero, shape.radius, true);
                }
                else if ((shapeType == ParticleSystemShapeType.Cone) || (shapeType == ParticleSystemShapeType.ConeShell))
                {
                    Vector3 radiusAngleRange = new Vector3(shape.radius, shape.angle, main.startSpeedMultiplier);
                    radiusAngleRange = Handles.ConeFrustrumHandle(Quaternion.identity, Vector3.zero, radiusAngleRange);
                    shape.radius = radiusAngleRange.x;
                    shape.angle = radiusAngleRange.y;
                    main.startSpeedMultiplier = radiusAngleRange.z;
                }
                else if ((shapeType == ParticleSystemShapeType.ConeVolume) || (shapeType == ParticleSystemShapeType.ConeVolumeShell))
                {
                    Vector3 vector2 = new Vector3(shape.radius, shape.angle, shape.length);
                    vector2 = Handles.ConeFrustrumHandle(Quaternion.identity, Vector3.zero, vector2);
                    shape.radius = vector2.x;
                    shape.angle = vector2.y;
                    shape.length = vector2.z;
                }
                else if (((shapeType == ParticleSystemShapeType.Box) || (shapeType == ParticleSystemShapeType.BoxShell)) || (shapeType == ParticleSystemShapeType.BoxEdge))
                {
                    this.m_BoxBoundsHandle.center = Vector3.zero;
                    this.m_BoxBoundsHandle.size = shape.box;
                    this.m_BoxBoundsHandle.SetColor(s_ShapeGizmoColor);
                    this.m_BoxBoundsHandle.DrawHandle();
                    shape.box = this.m_BoxBoundsHandle.size;
                }
                else if (shapeType == ParticleSystemShapeType.SingleSidedEdge)
                {
                    shape.radius = Handles.DoSimpleEdgeHandle(Quaternion.identity, Vector3.zero, shape.radius);
                }
                else if (shapeType == ParticleSystemShapeType.Mesh)
                {
                    Mesh mesh = shape.mesh;
                    if (mesh != null)
                    {
                        bool wireframe = GL.wireframe;
                        GL.wireframe = true;
                        this.m_Material.SetPass(0);
                        Graphics.DrawMeshNow(mesh, matrixx2);
                        GL.wireframe = wireframe;
                    }
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                base.m_ParticleSystemUI.m_ParticleEffectUI.m_Owner.Repaint();
            }
            Handles.color = color;
            Handles.matrix = matrix;
        }

        private class Texts
        {
            public GUIContent alignToDirection = EditorGUIUtility.TextContent("Align To Direction|Automatically align particles based on their initial direction of travel.");
            public GUIContent arc = EditorGUIUtility.TextContent("Arc|Circle arc angle.");
            public GUIContent boxX = EditorGUIUtility.TextContent("Box X|Scale of the box in X Axis.");
            public GUIContent boxY = EditorGUIUtility.TextContent("Box Y|Scale of the box in Y Axis.");
            public GUIContent boxZ = EditorGUIUtility.TextContent("Box Z|Scale of the box in Z Axis.");
            public GUIContent coneAngle = EditorGUIUtility.TextContent("Angle|Angle of the cone.");
            public GUIContent coneLength = EditorGUIUtility.TextContent("Length|Length of the cone.");
            public GUIContent emitFrom = EditorGUIUtility.TextContent("Emit from:|Specifies from where particles are emitted.");
            public GUIContent emitFromEdge = EditorGUIUtility.TextContent("Emit from Edge|Emit from edge of the shape. If disabled particles will be emitted from the volume of the shape.");
            public GUIContent emitFromShell = EditorGUIUtility.TextContent("Emit from Shell|Emit from shell of the sphere. If disabled particles will be emitted from the volume of the shape.");
            public GUIContent mesh = EditorGUIUtility.TextContent("Mesh|Mesh that the particle system will emit from.");
            public GUIContent meshMaterialIndex = EditorGUIUtility.TextContent("Single Material|Only emit from a specific material of the mesh.");
            public GUIContent meshNormalOffset = EditorGUIUtility.TextContent("Normal Offset|Offset particle spawn positions along the mesh normal.");
            public GUIContent meshRenderer = EditorGUIUtility.TextContent("Mesh|MeshRenderer that the particle system will emit from.");
            public GUIContent meshScale = EditorGUIUtility.TextContent("Mesh Scale|Adjust the size of the source mesh.");
            public GUIContent radius = EditorGUIUtility.TextContent("Radius|Radius of the shape.");
            public GUIContent randomDirectionAmount = EditorGUIUtility.TextContent("Randomize Direction|Randomize the emission direction.");
            public GUIContent shape = EditorGUIUtility.TextContent("Shape|Defines the shape of the volume from which particles can be emitted, and the direction of the start velocity.");
            public GUIContent skinnedMeshRenderer = EditorGUIUtility.TextContent("Mesh|SkinnedMeshRenderer that the particle system will emit from.");
            public GUIContent sphericalDirectionAmount = EditorGUIUtility.TextContent("Spherize Direction|Spherize the emission direction.");
            public GUIContent useMeshColors = EditorGUIUtility.TextContent("Use Mesh Colors|Modulate particle color with mesh vertex colors, or if they don't exist, use the shader color property \"_Color\" or \"_TintColor\" from the material.");
        }
    }
}

