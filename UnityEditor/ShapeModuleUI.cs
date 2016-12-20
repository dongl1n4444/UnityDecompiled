namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class ShapeModuleUI : ModuleUI
    {
        private readonly ShapeTypes[] boxShapes;
        private readonly ShapeTypes[] coneShapes;
        private SerializedProperty m_AlignToDirection;
        private SerializedProperty m_Angle;
        private SerializedProperty m_Arc;
        private BoxEditor m_BoxEditor;
        private SerializedProperty m_BoxX;
        private SerializedProperty m_BoxY;
        private SerializedProperty m_BoxZ;
        private readonly string[] m_GuiNames;
        private readonly ShapeTypes[] m_GuiTypes;
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
        private static int s_BoxHash = "BoxColliderEditor".GetHashCode();
        private static Color s_ShapeGizmoColor = new Color(0.5803922f, 0.8980392f, 1f, 0.9f);
        private static Texts s_Texts = new Texts();
        private readonly ShapeTypes[] shellShapes;

        public ShapeModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "ShapeModule", displayName, ModuleUI.VisibilityState.VisibleAndFolded)
        {
            this.m_BoxEditor = new BoxEditor(true, s_BoxHash);
            this.m_GuiNames = new string[] { "Sphere", "Hemisphere", "Cone", "Box", "Mesh", "Mesh Renderer", "Skinned Mesh Renderer", "Circle", "Edge" };
            this.m_GuiTypes = new ShapeTypes[] { ShapeTypes.Sphere };
            this.m_TypeToGuiTypeIndex = new int[] { 
                0, 0, 1, 1, 2, 3, 4, 2, 2, 2, 7, 7, 8, 5, 6, 3,
                3
            };
            this.boxShapes = new ShapeTypes[] { ShapeTypes.Box };
            this.coneShapes = new ShapeTypes[] { ShapeTypes.Cone };
            this.shellShapes = new ShapeTypes[] { ShapeTypes.BoxShell };
            base.m_ToolTip = "Shape of the emitter volume, which controls where particles are emitted and their initial direction.";
        }

        private ShapeTypes ConvertBoxEmitFromToConeType(int emitFrom)
        {
            return this.boxShapes[emitFrom];
        }

        private int ConvertBoxTypeToConeEmitFrom(ShapeTypes shapeType)
        {
            return Array.IndexOf<ShapeTypes>(this.boxShapes, shapeType);
        }

        private ShapeTypes ConvertConeEmitFromToConeType(int emitFrom)
        {
            return this.coneShapes[emitFrom];
        }

        private int ConvertConeTypeToConeEmitFrom(ShapeTypes shapeType)
        {
            return Array.IndexOf<ShapeTypes>(this.coneShapes, shapeType);
        }

        private bool GetUsesShell(ShapeTypes shapeType)
        {
            return (Array.IndexOf<ShapeTypes>(this.shellShapes, shapeType) != -1);
        }

        public override float GetXAxisScalar()
        {
            return base.m_ParticleSystemUI.GetEmitterDuration();
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
                this.m_BoxEditor.SetAlwaysDisplayHandles(true);
            }
        }

        public override void OnInspectorGUI(ParticleSystem s)
        {
            Material sharedMaterial;
            Mesh sharedMesh;
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            int intValue = this.m_Type.intValue;
            int num2 = this.m_TypeToGuiTypeIndex[intValue];
            bool usesShell = this.GetUsesShell((ShapeTypes) intValue);
            int index = ModuleUI.GUIPopup(s_Texts.shape, num2, this.m_GuiNames, new GUILayoutOption[0]);
            ShapeTypes types = this.m_GuiTypes[index];
            if (index != num2)
            {
                intValue = (int) types;
            }
            switch (types)
            {
                case ShapeTypes.Sphere:
                    ModuleUI.GUIFloat(s_Texts.radius, this.m_Radius, new GUILayoutOption[0]);
                    intValue = !ModuleUI.GUIToggle(s_Texts.emitFromShell, usesShell, new GUILayoutOption[0]) ? 0 : 1;
                    goto Label_0581;

                case ShapeTypes.Hemisphere:
                    ModuleUI.GUIFloat(s_Texts.radius, this.m_Radius, new GUILayoutOption[0]);
                    intValue = !ModuleUI.GUIToggle(s_Texts.emitFromShell, usesShell, new GUILayoutOption[0]) ? 2 : 3;
                    goto Label_0581;

                case ShapeTypes.Cone:
                {
                    ModuleUI.GUIFloat(s_Texts.coneAngle, this.m_Angle, new GUILayoutOption[0]);
                    ModuleUI.GUIFloat(s_Texts.radius, this.m_Radius, new GUILayoutOption[0]);
                    bool disabled = (intValue != 8) && (intValue != 9);
                    using (new EditorGUI.DisabledScope(disabled))
                    {
                        ModuleUI.GUIFloat(s_Texts.coneLength, this.m_Length, new GUILayoutOption[0]);
                    }
                    string[] options = new string[] { "Base", "Base Shell", "Volume", "Volume Shell" };
                    int num5 = this.ConvertConeTypeToConeEmitFrom((ShapeTypes) intValue);
                    num5 = ModuleUI.GUIPopup(s_Texts.emitFrom, num5, options, new GUILayoutOption[0]);
                    intValue = (int) this.ConvertConeEmitFromToConeType(num5);
                    goto Label_0581;
                }
                case ShapeTypes.Box:
                {
                    ModuleUI.GUIFloat(s_Texts.boxX, this.m_BoxX, new GUILayoutOption[0]);
                    ModuleUI.GUIFloat(s_Texts.boxY, this.m_BoxY, new GUILayoutOption[0]);
                    ModuleUI.GUIFloat(s_Texts.boxZ, this.m_BoxZ, new GUILayoutOption[0]);
                    string[] strArray = new string[] { "Volume", "Shell", "Edge" };
                    int num4 = this.ConvertBoxTypeToConeEmitFrom((ShapeTypes) intValue);
                    num4 = ModuleUI.GUIPopup(s_Texts.emitFrom, num4, strArray, new GUILayoutOption[0]);
                    intValue = (int) this.ConvertBoxEmitFromToConeType(num4);
                    goto Label_0581;
                }
                case ShapeTypes.Mesh:
                case ShapeTypes.MeshRenderer:
                case ShapeTypes.SkinnedMeshRenderer:
                {
                    string[] strArray3 = new string[] { "Vertex", "Edge", "Triangle" };
                    ModuleUI.GUIPopup("", this.m_PlacementMode, strArray3, new GUILayoutOption[0]);
                    sharedMaterial = null;
                    sharedMesh = null;
                    if (types != ShapeTypes.Mesh)
                    {
                        if (types == ShapeTypes.MeshRenderer)
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
                case ShapeTypes.Circle:
                    ModuleUI.GUIFloat(s_Texts.radius, this.m_Radius, new GUILayoutOption[0]);
                    ModuleUI.GUIFloat(s_Texts.arc, this.m_Arc, new GUILayoutOption[0]);
                    intValue = !ModuleUI.GUIToggle(s_Texts.emitFromEdge, usesShell, new GUILayoutOption[0]) ? 10 : 11;
                    goto Label_0581;

                case ShapeTypes.SingleSidedEdge:
                    ModuleUI.GUIFloat(s_Texts.radius, this.m_Radius, new GUILayoutOption[0]);
                    goto Label_0581;

                default:
                    goto Label_0581;
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
        Label_0581:
            this.m_Type.intValue = intValue;
            ModuleUI.GUIToggle(s_Texts.alignToDirection, this.m_AlignToDirection, new GUILayoutOption[0]);
            ModuleUI.GUIFloat(s_Texts.randomDirectionAmount, this.m_RandomDirectionAmount, new GUILayoutOption[0]);
            ModuleUI.GUIFloat(s_Texts.sphericalDirectionAmount, this.m_SphericalDirectionAmount, new GUILayoutOption[0]);
        }

        public override void OnSceneGUI(ParticleSystem system, InitialModuleUI initial)
        {
            Color color = Handles.color;
            Handles.color = s_ShapeGizmoColor;
            Matrix4x4 matrix = Handles.matrix;
            EditorGUI.BeginChangeCheck();
            int intValue = this.m_Type.intValue;
            Matrix4x4 transform = new Matrix4x4();
            float num2 = (intValue != 6) ? 1f : this.m_MeshScale.floatValue;
            if (system.main.scalingMode == ParticleSystemScalingMode.Hierarchy)
            {
                transform.SetTRS(system.transform.position, system.transform.rotation, (Vector3) (system.transform.lossyScale * num2));
            }
            else
            {
                transform.SetTRS(system.transform.position, system.transform.rotation, (Vector3) (system.transform.localScale * num2));
            }
            Handles.matrix = transform;
            switch (intValue)
            {
                case 0:
                case 1:
                    this.m_Radius.floatValue = Handles.DoSimpleRadiusHandle(Quaternion.identity, Vector3.zero, this.m_Radius.floatValue, false);
                    break;
            }
            if ((intValue == 10) || (intValue == 11))
            {
                float floatValue = this.m_Radius.floatValue;
                float arc = this.m_Arc.floatValue;
                Handles.DoSimpleRadiusArcHandleXY(Quaternion.identity, Vector3.zero, ref floatValue, ref arc);
                this.m_Radius.floatValue = floatValue;
                this.m_Arc.floatValue = arc;
            }
            else if ((intValue == 2) || (intValue == 3))
            {
                this.m_Radius.floatValue = Handles.DoSimpleRadiusHandle(Quaternion.identity, Vector3.zero, this.m_Radius.floatValue, true);
            }
            else if ((intValue == 4) || (intValue == 7))
            {
                Vector3 radiusAngleRange = new Vector3(this.m_Radius.floatValue, this.m_Angle.floatValue, initial.m_Speed.scalar.floatValue);
                radiusAngleRange = Handles.ConeFrustrumHandle(Quaternion.identity, Vector3.zero, radiusAngleRange);
                this.m_Radius.floatValue = radiusAngleRange.x;
                this.m_Angle.floatValue = radiusAngleRange.y;
                initial.m_Speed.scalar.floatValue = radiusAngleRange.z;
            }
            else if ((intValue == 8) || (intValue == 9))
            {
                Vector3 vector2 = new Vector3(this.m_Radius.floatValue, this.m_Angle.floatValue, this.m_Length.floatValue);
                vector2 = Handles.ConeFrustrumHandle(Quaternion.identity, Vector3.zero, vector2);
                this.m_Radius.floatValue = vector2.x;
                this.m_Angle.floatValue = vector2.y;
                this.m_Length.floatValue = vector2.z;
            }
            else if (((intValue == 5) || (intValue == 15)) || (intValue == 0x10))
            {
                Vector3 zero = Vector3.zero;
                Vector3 size = new Vector3(this.m_BoxX.floatValue, this.m_BoxY.floatValue, this.m_BoxZ.floatValue);
                if (this.m_BoxEditor.OnSceneGUI(transform, s_ShapeGizmoColor, false, ref zero, ref size))
                {
                    this.m_BoxX.floatValue = size.x;
                    this.m_BoxY.floatValue = size.y;
                    this.m_BoxZ.floatValue = size.z;
                }
            }
            else if (intValue == 12)
            {
                this.m_Radius.floatValue = Handles.DoSimpleEdgeHandle(Quaternion.identity, Vector3.zero, this.m_Radius.floatValue);
            }
            else if (intValue == 6)
            {
                Mesh objectReferenceValue = (Mesh) this.m_Mesh.objectReferenceValue;
                if (objectReferenceValue != null)
                {
                    bool wireframe = GL.wireframe;
                    GL.wireframe = true;
                    this.m_Material.SetPass(0);
                    Graphics.DrawMeshNow(objectReferenceValue, transform);
                    GL.wireframe = wireframe;
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                base.m_ParticleSystemUI.m_ParticleEffectUI.m_Owner.Repaint();
            }
            Handles.color = color;
            Handles.matrix = matrix;
        }

        private enum ShapeTypes
        {
            Sphere,
            SphereShell,
            Hemisphere,
            HemisphereShell,
            Cone,
            Box,
            Mesh,
            ConeShell,
            ConeVolume,
            ConeVolumeShell,
            Circle,
            CircleEdge,
            SingleSidedEdge,
            MeshRenderer,
            SkinnedMeshRenderer,
            BoxShell,
            BoxEdge
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

