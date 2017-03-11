namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal class ShapeModuleUI : ModuleUI
    {
        private readonly ParticleSystemShapeType[] boxShapes;
        private readonly ParticleSystemShapeType[] coneShapes;
        private SerializedProperty m_AlignToDirection;
        private SerializedProperty m_Angle;
        private MultiModeParameter m_Arc;
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
        private MultiModeParameter m_Radius;
        private SerializedProperty m_RandomDirectionAmount;
        private SerializedProperty m_SkinnedMeshRenderer;
        private SerializedProperty m_SphericalDirectionAmount;
        private SerializedProperty m_Type;
        private readonly int[] m_TypeToGuiTypeIndex;
        private SerializedProperty m_UseMeshColors;
        private SerializedProperty m_UseMeshMaterialIndex;
        private static MultiModeTexts s_ArcTexts = new MultiModeTexts("Randomized Arc|New particles are spawned randomly around the arc.", "Looping Arc|New particles are spawned sequentially around the arc.", "Ping-Pong Arc|New particles are spawned sequentially around the arc, and alternate between clockwise and counter-clockwise.", "Distributed Arc|New particles are distributed evenly around the arc. (Use with Burst emission).", "Spread|Spawn particles only at specific angles around the arc (0 to disable).", "Arc Speed|Control the speed that the emission position moves around the arc.");
        private static int s_BoxHandleControlIDHint = typeof(ShapeModuleUI).Name.GetHashCode();
        private static MultiModeTexts s_RadiusTexts = new MultiModeTexts("Randomized Radius|New particles are spawned randomly along the radius.", "Looping Radius|New particles are spawned sequentially along the radius.", "Ping-Pong Radius|New particles are spawned sequentially along the radius, and alternate between clockwise and counter-clockwise.", "Distributed Radius|New particles are distributed evenly along the radius. (Use with Burst emission).", "Spread|Spawn particles only at specific positions along the radius (0 to disable).", "Radius Speed|Control the speed that the emission position moves along the radius.");
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
                this.m_Radius = MultiModeParameter.GetProperty(this, "radius", s_RadiusTexts.speed);
                this.m_Angle = base.GetProperty("angle");
                this.m_Length = base.GetProperty("length");
                this.m_BoxX = base.GetProperty("boxX");
                this.m_BoxY = base.GetProperty("boxY");
                this.m_BoxZ = base.GetProperty("boxZ");
                this.m_Arc = MultiModeParameter.GetProperty(this, "arc", s_ArcTexts.speed);
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
                    ModuleUI.GUIFloat(s_Texts.radius, this.m_Radius.m_Value, new GUILayoutOption[0]);
                    intValue = !ModuleUI.GUIToggle(s_Texts.emitFromShell, usesShell, new GUILayoutOption[0]) ? 0 : 1;
                    goto Label_0599;

                case ParticleSystemShapeType.Hemisphere:
                    ModuleUI.GUIFloat(s_Texts.radius, this.m_Radius.m_Value, new GUILayoutOption[0]);
                    intValue = !ModuleUI.GUIToggle(s_Texts.emitFromShell, usesShell, new GUILayoutOption[0]) ? 2 : 3;
                    goto Label_0599;

                case ParticleSystemShapeType.Cone:
                {
                    ModuleUI.GUIFloat(s_Texts.coneAngle, this.m_Angle, new GUILayoutOption[0]);
                    ModuleUI.GUIFloat(s_Texts.radius, this.m_Radius.m_Value, new GUILayoutOption[0]);
                    this.m_Arc.OnInspectorGUI(s_ArcTexts);
                    bool disabled = (intValue != 8) && (intValue != 9);
                    using (new EditorGUI.DisabledScope(disabled))
                    {
                        ModuleUI.GUIFloat(s_Texts.coneLength, this.m_Length, new GUILayoutOption[0]);
                    }
                    string[] options = new string[] { "Base", "Base Shell", "Volume", "Volume Shell" };
                    int num5 = this.ConvertConeTypeToConeEmitFrom((ParticleSystemShapeType) intValue);
                    num5 = ModuleUI.GUIPopup(s_Texts.emitFrom, num5, options, new GUILayoutOption[0]);
                    intValue = (int) this.ConvertConeEmitFromToConeType(num5);
                    goto Label_0599;
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
                    goto Label_0599;
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
                    ModuleUI.GUIFloat(s_Texts.radius, this.m_Radius.m_Value, new GUILayoutOption[0]);
                    this.m_Arc.OnInspectorGUI(s_ArcTexts);
                    intValue = !ModuleUI.GUIToggle(s_Texts.emitFromEdge, usesShell, new GUILayoutOption[0]) ? 10 : 11;
                    goto Label_0599;

                case ParticleSystemShapeType.SingleSidedEdge:
                    this.m_Radius.OnInspectorGUI(s_RadiusTexts);
                    goto Label_0599;

                default:
                    goto Label_0599;
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
        Label_0599:
            if (flag2 || !this.m_Type.hasMultipleDifferentValues)
            {
                this.m_Type.intValue = intValue;
            }
            ModuleUI.GUIToggle(s_Texts.alignToDirection, this.m_AlignToDirection, new GUILayoutOption[0]);
            ModuleUI.GUIFloat(s_Texts.randomDirectionAmount, this.m_RandomDirectionAmount, new GUILayoutOption[0]);
            ModuleUI.GUIFloat(s_Texts.sphericalDirectionAmount, this.m_SphericalDirectionAmount, new GUILayoutOption[0]);
        }

        public override void OnSceneViewGUI()
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
                    {
                        EditorGUI.BeginChangeCheck();
                        float num3 = Handles.DoSimpleRadiusHandle(Quaternion.identity, Vector3.zero, shape.radius, false);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(system, "Sphere Handle Change");
                            shape.radius = num3;
                        }
                        break;
                    }
                    case ParticleSystemShapeType.Circle:
                    case ParticleSystemShapeType.CircleEdge:
                    {
                        EditorGUI.BeginChangeCheck();
                        float radius = shape.radius;
                        float arc = shape.arc;
                        Handles.DoSimpleRadiusArcHandleXY(Quaternion.identity, Vector3.zero, ref radius, ref arc);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(system, "Circle Handle Change");
                            shape.radius = radius;
                            shape.arc = arc;
                        }
                        break;
                    }
                    case ParticleSystemShapeType.Hemisphere:
                    case ParticleSystemShapeType.HemisphereShell:
                    {
                        EditorGUI.BeginChangeCheck();
                        float num6 = Handles.DoSimpleRadiusHandle(Quaternion.identity, Vector3.zero, shape.radius, true);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(system, "Hemisphere Handle Change");
                            shape.radius = num6;
                        }
                        break;
                    }
                    case ParticleSystemShapeType.Cone:
                    case ParticleSystemShapeType.ConeShell:
                    {
                        EditorGUI.BeginChangeCheck();
                        Vector3 radiusAngleRange = new Vector3(shape.radius, shape.angle, main.startSpeedMultiplier);
                        radiusAngleRange = Handles.ConeFrustrumHandle(Quaternion.identity, Vector3.zero, radiusAngleRange);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(system, "Cone Handle Change");
                            shape.radius = radiusAngleRange.x;
                            shape.angle = radiusAngleRange.y;
                            main.startSpeedMultiplier = radiusAngleRange.z;
                        }
                        break;
                    }
                    case ParticleSystemShapeType.ConeVolume:
                    case ParticleSystemShapeType.ConeVolumeShell:
                    {
                        EditorGUI.BeginChangeCheck();
                        Vector3 vector2 = new Vector3(shape.radius, shape.angle, shape.length);
                        vector2 = Handles.ConeFrustrumHandle(Quaternion.identity, Vector3.zero, vector2);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(system, "Cone Volume Handle Change");
                            shape.radius = vector2.x;
                            shape.angle = vector2.y;
                            shape.length = vector2.z;
                        }
                        break;
                    }
                    case ParticleSystemShapeType.Box:
                    case ParticleSystemShapeType.BoxShell:
                    case ParticleSystemShapeType.BoxEdge:
                        EditorGUI.BeginChangeCheck();
                        this.m_BoxBoundsHandle.center = Vector3.zero;
                        this.m_BoxBoundsHandle.size = shape.box;
                        this.m_BoxBoundsHandle.SetColor(s_ShapeGizmoColor);
                        this.m_BoxBoundsHandle.DrawHandle();
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(system, "Box Handle Change");
                            shape.box = this.m_BoxBoundsHandle.size;
                        }
                        break;

                    case ParticleSystemShapeType.SingleSidedEdge:
                    {
                        EditorGUI.BeginChangeCheck();
                        float num7 = Handles.DoSimpleEdgeHandle(Quaternion.identity, Vector3.zero, shape.radius);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(system, "Edge Handle Change");
                            shape.radius = num7;
                        }
                        break;
                    }
                    case ParticleSystemShapeType.Mesh:
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
                        break;
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

        public override void UpdateCullingSupportedString(ref string text)
        {
            this.Init();
            if ((this.m_Arc.m_Mode.intValue != 0) || (this.m_Radius.m_Mode.intValue != 0))
            {
                text = text + "\n\tAnimated shape emission is enabled.";
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MultiModeParameter
        {
            public SerializedProperty m_Value;
            public SerializedProperty m_Mode;
            public SerializedProperty m_Spread;
            public SerializedMinMaxCurve m_Speed;
            [CompilerGenerated]
            private static GenericMenu.MenuFunction2 <>f__mg$cache0;
            public static ShapeModuleUI.MultiModeParameter GetProperty(ModuleUI ui, string name, GUIContent speed)
            {
                ShapeModuleUI.MultiModeParameter parameter = new ShapeModuleUI.MultiModeParameter {
                    m_Value = ui.GetProperty(name + ".value"),
                    m_Mode = ui.GetProperty(name + ".mode"),
                    m_Spread = ui.GetProperty(name + ".spread"),
                    m_Speed = new SerializedMinMaxCurve(ui, speed, name + ".speed", ModuleUI.kUseSignedRange)
                };
                parameter.m_Speed.m_AllowRandom = false;
                return parameter;
            }

            private static void SelectModeCallback(object obj)
            {
                ModeCallbackData data = (ModeCallbackData) obj;
                data.modeProp.intValue = (int) data.selectedState;
            }

            private static void GUIMMModePopUp(Rect rect, SerializedProperty modeProp)
            {
                if (EditorGUI.DropdownButton(rect, GUIContent.none, FocusType.Passive, ParticleSystemStyles.Get().minMaxCurveStateDropDown))
                {
                    GUIContent[] contentArray = new GUIContent[] { new GUIContent("Random"), new GUIContent("Loop"), new GUIContent("Ping-Pong"), new GUIContent("Burst Spread") };
                    ValueMode[] modeArray = new ValueMode[] { ValueMode.Random };
                    GenericMenu menu = new GenericMenu();
                    for (int i = 0; i < contentArray.Length; i++)
                    {
                        if (<>f__mg$cache0 == null)
                        {
                            <>f__mg$cache0 = new GenericMenu.MenuFunction2(ShapeModuleUI.MultiModeParameter.SelectModeCallback);
                        }
                        menu.AddItem(contentArray[i], modeProp.intValue == modeArray[i], <>f__mg$cache0, new ModeCallbackData(modeArray[i], modeProp));
                    }
                    menu.ShowAsContext();
                    Event.current.Use();
                }
            }

            public void OnInspectorGUI(ShapeModuleUI.MultiModeTexts text)
            {
                GUIContent[] contentArray = new GUIContent[] { text.modeRandom, text.modeLoop, text.modePingPong, text.modeDistributed };
                Rect position = GUILayoutUtility.GetRect((float) 0f, (float) 13f);
                Rect popupRect = ModuleUI.GetPopupRect(position);
                position = ModuleUI.SubtractPopupWidth(position);
                ModuleUI.PrefixLabel(position, contentArray[this.m_Mode.intValue]);
                float width = position.width;
                position.width /= 1.5f;
                ModuleUI.FloatDraggable(position, this.m_Value, 1f, EditorGUIUtility.labelWidth, "g7");
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 38f;
                position.x += position.width + 4f;
                position.width = (width - position.width) - 4f;
                ModuleUI.PrefixLabel(position, text.spread);
                ModuleUI.FloatDraggable(position, this.m_Spread, 1f, EditorGUIUtility.labelWidth, "g7");
                EditorGUIUtility.labelWidth = labelWidth;
                GUIMMModePopUp(popupRect, this.m_Mode);
                if ((this.m_Mode.intValue == 1) || (this.m_Mode.intValue == 2))
                {
                    ModuleUI.GUIMinMaxCurve(text.speed, this.m_Speed, new GUILayoutOption[0]);
                }
            }
            private class ModeCallbackData
            {
                public SerializedProperty modeProp;
                public ShapeModuleUI.MultiModeParameter.ValueMode selectedState;

                public ModeCallbackData(ShapeModuleUI.MultiModeParameter.ValueMode state, SerializedProperty p)
                {
                    this.modeProp = p;
                    this.selectedState = state;
                }
            }

            public enum ValueMode
            {
                Random,
                Loop,
                PingPong,
                BurstSpread
            }
        }

        private class MultiModeTexts
        {
            public GUIContent modeDistributed;
            public GUIContent modeLoop;
            public GUIContent modePingPong;
            public GUIContent modeRandom;
            public GUIContent speed;
            public GUIContent spread;

            public MultiModeTexts(string _modeRandom, string _modeLoop, string _modePingPong, string _modeDistributed, string _spread, string _speed)
            {
                this.modeRandom = EditorGUIUtility.TextContent(_modeRandom);
                this.modeLoop = EditorGUIUtility.TextContent(_modeLoop);
                this.modePingPong = EditorGUIUtility.TextContent(_modePingPong);
                this.modeDistributed = EditorGUIUtility.TextContent(_modeDistributed);
                this.spread = EditorGUIUtility.TextContent(_spread);
                this.speed = EditorGUIUtility.TextContent(_speed);
            }
        }

        private class Texts
        {
            public GUIContent alignToDirection = EditorGUIUtility.TextContent("Align To Direction|Automatically align particles based on their initial direction of travel.");
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
            public GUIContent useMeshColors = EditorGUIUtility.TextContent("Use Mesh Colors|Modulate particle color with mesh vertex colors, or if they don't exist, use the shader color property \"_Color\" or \"_TintColor\" from the material. Does not read texture colors.");
        }
    }
}

