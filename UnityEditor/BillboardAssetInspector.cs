namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(BillboardAsset)), CanEditMultipleObjects]
    internal class BillboardAssetInspector : Editor
    {
        [CompilerGenerated]
        private static Func<ushort, int> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<IEnumerable<Vector3>, IEnumerable<Vector3>> <>f__am$cache1;
        private SerializedProperty m_Bottom;
        private Material m_GeometryMaterial;
        private Mesh m_GeometryMesh;
        private SerializedProperty m_Height;
        private SerializedProperty m_Images;
        private SerializedProperty m_Indices;
        private SerializedProperty m_Material;
        private Vector2 m_PreviewDir = new Vector2(-120f, 20f);
        private bool m_PreviewShaded = true;
        private PreviewRenderUtility m_PreviewUtility;
        private MaterialPropertyBlock m_ShadedMaterialProperties;
        private Mesh m_ShadedMesh;
        private SerializedProperty m_Vertices;
        private SerializedProperty m_Width;
        private Material m_WireframeMaterial;
        private static GUIStyles s_Styles = null;

        private void DoRenderPreview(bool shaded)
        {
            BillboardAsset target = base.target as BillboardAsset;
            Bounds bounds = new Bounds(new Vector3(0f, (this.m_Height.floatValue + this.m_Bottom.floatValue) * 0.5f, 0f), new Vector3(this.m_Width.floatValue, this.m_Height.floatValue, this.m_Width.floatValue));
            float magnitude = bounds.extents.magnitude;
            float num2 = 8f * magnitude;
            Quaternion quaternion = Quaternion.Euler(-this.m_PreviewDir.y, -this.m_PreviewDir.x, 0f);
            this.m_PreviewUtility.m_Camera.transform.rotation = quaternion;
            this.m_PreviewUtility.m_Camera.transform.position = (Vector3) (quaternion * (-Vector3.forward * num2));
            this.m_PreviewUtility.m_Camera.nearClipPlane = num2 - (magnitude * 1.1f);
            this.m_PreviewUtility.m_Camera.farClipPlane = num2 + (magnitude * 1.1f);
            this.m_PreviewUtility.m_Light[0].intensity = 1.4f;
            this.m_PreviewUtility.m_Light[0].transform.rotation = quaternion * Quaternion.Euler(40f, 40f, 0f);
            this.m_PreviewUtility.m_Light[1].intensity = 1.4f;
            Color ambient = new Color(0.1f, 0.1f, 0.1f, 0f);
            InternalEditorUtility.SetCustomLighting(this.m_PreviewUtility.m_Light, ambient);
            if (shaded)
            {
                this.MakeRenderMesh(this.m_ShadedMesh, target);
                target.MakeMaterialProperties(this.m_ShadedMaterialProperties, this.m_PreviewUtility.m_Camera);
                ModelInspector.RenderMeshPreviewSkipCameraAndLighting(this.m_ShadedMesh, bounds, this.m_PreviewUtility, target.material, null, this.m_ShadedMaterialProperties, new Vector2(0f, 0f), -1);
            }
            else
            {
                this.MakePreviewMesh(this.m_GeometryMesh, target);
                ModelInspector.RenderMeshPreviewSkipCameraAndLighting(this.m_GeometryMesh, bounds, this.m_PreviewUtility, this.m_GeometryMaterial, this.m_WireframeMaterial, null, new Vector2(0f, 0f), -1);
            }
            InternalEditorUtility.RemoveCustomLighting();
        }

        public override string GetInfoString() => 
            $"{this.m_Vertices.arraySize} verts, {(this.m_Indices.arraySize / 3)} tris, {this.m_Images.arraySize} images";

        public override bool HasPreviewGUI() => 
            (base.target != null);

        private void InitPreview()
        {
            if (this.m_PreviewUtility == null)
            {
                this.m_PreviewUtility = new PreviewRenderUtility();
                this.m_ShadedMesh = new Mesh();
                this.m_ShadedMesh.hideFlags = HideFlags.HideAndDontSave;
                this.m_ShadedMesh.MarkDynamic();
                this.m_GeometryMesh = new Mesh();
                this.m_GeometryMesh.hideFlags = HideFlags.HideAndDontSave;
                this.m_GeometryMesh.MarkDynamic();
                this.m_ShadedMaterialProperties = new MaterialPropertyBlock();
                this.m_GeometryMaterial = EditorGUIUtility.GetBuiltinExtraResource(typeof(Material), "Default-Material.mat") as Material;
                this.m_WireframeMaterial = ModelInspector.CreateWireframeMaterial();
                EditorUtility.SetCameraAnimateMaterials(this.m_PreviewUtility.m_Camera, true);
            }
        }

        private void MakePreviewMesh(Mesh mesh, BillboardAsset billboard)
        {
            <MakePreviewMesh>c__AnonStorey0 storey = new <MakePreviewMesh>c__AnonStorey0 {
                width = billboard.width,
                height = billboard.height,
                bottom = billboard.bottom
            };
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = s => s;
            }
            mesh.SetVertices(Enumerable.SelectMany<IEnumerable<Vector3>, Vector3>(Enumerable.Repeat<IEnumerable<Vector3>>(Enumerable.Select<Vector2, Vector3>(billboard.GetVertices(), new Func<Vector2, Vector3>(storey.<>m__0)), 2), <>f__am$cache1).ToList<Vector3>());
            mesh.SetNormals(Enumerable.Repeat<Vector3>(Vector3.forward, billboard.vertexCount).Concat<Vector3>(Enumerable.Repeat<Vector3>(-Vector3.forward, billboard.vertexCount)).ToList<Vector3>());
            int[] triangles = new int[billboard.indexCount * 2];
            ushort[] indices = billboard.GetIndices();
            for (int i = 0; i < (billboard.indexCount / 3); i++)
            {
                triangles[i * 3] = indices[i * 3];
                triangles[(i * 3) + 1] = indices[(i * 3) + 1];
                triangles[(i * 3) + 2] = indices[(i * 3) + 2];
                triangles[(i * 3) + billboard.indexCount] = indices[(i * 3) + 2];
                triangles[((i * 3) + 1) + billboard.indexCount] = indices[(i * 3) + 1];
                triangles[((i * 3) + 2) + billboard.indexCount] = indices[i * 3];
            }
            mesh.SetTriangles(triangles, 0);
        }

        private void MakeRenderMesh(Mesh mesh, BillboardAsset billboard)
        {
            mesh.SetVertices(Enumerable.Repeat<Vector3>(Vector3.zero, billboard.vertexCount).ToList<Vector3>());
            mesh.SetColors(Enumerable.Repeat<Color>(Color.black, billboard.vertexCount).ToList<Color>());
            mesh.SetUVs(0, billboard.GetVertices().ToList<Vector2>());
            mesh.SetUVs(1, Enumerable.Repeat<Vector4>(new Vector4(1f, 1f, 0f, 0f), billboard.vertexCount).ToList<Vector4>());
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = v => v;
            }
            mesh.SetTriangles(Enumerable.Select<ushort, int>(billboard.GetIndices(), <>f__am$cache0).ToList<int>(), 0);
        }

        private void OnDisable()
        {
            if (this.m_PreviewUtility != null)
            {
                this.m_PreviewUtility.Cleanup();
                this.m_PreviewUtility = null;
                UnityEngine.Object.DestroyImmediate(this.m_ShadedMesh, true);
                UnityEngine.Object.DestroyImmediate(this.m_GeometryMesh, true);
                this.m_GeometryMaterial = null;
                if (this.m_WireframeMaterial != null)
                {
                    UnityEngine.Object.DestroyImmediate(this.m_WireframeMaterial, true);
                }
            }
        }

        private void OnEnable()
        {
            this.m_Width = base.serializedObject.FindProperty("width");
            this.m_Height = base.serializedObject.FindProperty("height");
            this.m_Bottom = base.serializedObject.FindProperty("bottom");
            this.m_Images = base.serializedObject.FindProperty("imageTexCoords");
            this.m_Vertices = base.serializedObject.FindProperty("vertices");
            this.m_Indices = base.serializedObject.FindProperty("indices");
            this.m_Material = base.serializedObject.FindProperty("material");
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_Width, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Height, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Bottom, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Material, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (!ShaderUtil.hardwareSupportsRectRenderTexture)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), "Preview requires\nrender texture support");
                }
            }
            else
            {
                this.InitPreview();
                this.m_PreviewDir = PreviewGUI.Drag2D(this.m_PreviewDir, r);
                if (Event.current.type == EventType.Repaint)
                {
                    this.m_PreviewUtility.BeginPreview(r, background);
                    this.DoRenderPreview(this.m_PreviewShaded);
                    this.m_PreviewUtility.EndAndDrawPreview(r);
                }
            }
        }

        public override void OnPreviewSettings()
        {
            if (ShaderUtil.hardwareSupportsRectRenderTexture)
            {
                bool flag = this.m_Material.objectReferenceValue != null;
                GUI.enabled = flag;
                if (!flag)
                {
                    this.m_PreviewShaded = false;
                }
                GUIContent content = !this.m_PreviewShaded ? Styles.m_Geometry : Styles.m_Shaded;
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(75f) };
                Rect position = GUILayoutUtility.GetRect(content, Styles.m_DropdownButton, options);
                if (EditorGUI.ButtonMouseDown(position, content, FocusType.Passive, Styles.m_DropdownButton))
                {
                    GUIUtility.hotControl = 0;
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(Styles.m_Shaded, this.m_PreviewShaded, () => this.m_PreviewShaded = true);
                    menu.AddItem(Styles.m_Geometry, !this.m_PreviewShaded, () => this.m_PreviewShaded = false);
                    menu.DropDown(position);
                }
            }
        }

        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
        {
            if (!ShaderUtil.hardwareSupportsRectRenderTexture)
            {
                return null;
            }
            this.InitPreview();
            this.m_PreviewUtility.BeginStaticPreview(new Rect(0f, 0f, (float) width, (float) height));
            this.DoRenderPreview(true);
            return this.m_PreviewUtility.EndStaticPreview();
        }

        private static GUIStyles Styles
        {
            get
            {
                if (s_Styles == null)
                {
                    s_Styles = new GUIStyles();
                }
                return s_Styles;
            }
        }

        [CompilerGenerated]
        private sealed class <MakePreviewMesh>c__AnonStorey0
        {
            internal float bottom;
            internal float height;
            internal float width;

            internal Vector3 <>m__0(Vector2 v) => 
                new Vector3((v.x - 0.5f) * this.width, (v.y * this.height) + this.bottom, 0f);
        }

        private class GUIStyles
        {
            public readonly GUIStyle m_DropdownButton = new GUIStyle("MiniPopup");
            public readonly GUIContent m_Geometry = new GUIContent("Geometry");
            public readonly GUIContent m_Shaded = new GUIContent("Shaded");
        }
    }
}

