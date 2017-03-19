namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(Texture3D)), CanEditMultipleObjects]
    internal class Texture3DInspector : TextureInspector
    {
        private Material m_Material;
        private Mesh m_Mesh;
        public Vector2 m_PreviewDir = new Vector2(0f, 0f);
        private PreviewRenderUtility m_PreviewUtility;

        public override string GetInfoString()
        {
            Texture3D target = base.target as Texture3D;
            return $"{target.width}x{target.height}x{target.depth} {TextureUtil.GetTextureFormatString(target.format)} {EditorUtility.FormatBytes(TextureUtil.GetRuntimeMemorySizeLong(target))}";
        }

        private void InitPreview()
        {
            if (this.m_PreviewUtility == null)
            {
                this.m_PreviewUtility = new PreviewRenderUtility();
                this.m_PreviewUtility.m_CameraFieldOfView = 30f;
            }
            if (this.m_Mesh == null)
            {
                this.m_Mesh = PreviewRenderUtility.GetPreviewSphere();
            }
            if (this.m_Material == null)
            {
                this.m_Material = EditorGUIUtility.LoadRequired("Previews/Preview3DTextureMaterial.mat") as Material;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (this.m_PreviewUtility != null)
            {
                this.m_PreviewUtility.Cleanup();
                this.m_PreviewUtility = null;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (!ShaderUtil.hardwareSupportsRectRenderTexture || !SystemInfo.supports3DTextures)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), "3D texture preview not supported");
                }
            }
            else
            {
                this.m_PreviewDir = PreviewGUI.Drag2D(this.m_PreviewDir, r);
                if (Event.current.type == EventType.Repaint)
                {
                    this.InitPreview();
                    this.m_Material.mainTexture = base.target as Texture;
                    this.m_PreviewUtility.BeginPreview(r, background);
                    bool fog = RenderSettings.fog;
                    Unsupported.SetRenderSettingsUseFogNoDirty(false);
                    this.m_PreviewUtility.m_Camera.transform.position = (Vector3) (-Vector3.forward * 3f);
                    this.m_PreviewUtility.m_Camera.transform.rotation = Quaternion.identity;
                    Quaternion rot = Quaternion.Euler(this.m_PreviewDir.y, 0f, 0f) * Quaternion.Euler(0f, this.m_PreviewDir.x, 0f);
                    this.m_PreviewUtility.DrawMesh(this.m_Mesh, Vector3.zero, rot, this.m_Material, 0);
                    this.m_PreviewUtility.m_Camera.Render();
                    Unsupported.SetRenderSettingsUseFogNoDirty(fog);
                    this.m_PreviewUtility.EndAndDrawPreview(r);
                }
            }
        }

        public override void OnPreviewSettings()
        {
            if (ShaderUtil.hardwareSupportsRectRenderTexture && SystemInfo.supports3DTextures)
            {
                GUI.enabled = true;
            }
        }
    }
}

