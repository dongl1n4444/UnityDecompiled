namespace UnityEditor
{
    using System;
    using UnityEngine;
    using UnityEngine.Rendering;

    internal class LookDevResources
    {
        public static RenderTexture m_BrightestPointRT = null;
        public static Texture2D m_BrightestPointTexture = null;
        public static Cubemap m_DefaultHDRI = null;
        public static Material m_DeferredOverlayMaterial = null;
        public static Material m_DrawBallsMaterial = null;
        public static Material m_GBufferPatchMaterial = null;
        public static Material m_LookDevCompositing = null;
        public static Material m_LookDevCubeToLatlong = null;
        public static Mesh m_ScreenQuadMesh = null;
        public static RenderTexture m_SelectionTexture = null;
        public static Material m_SkyboxMaterial = null;
        public static SphericalHarmonicsL2 m_ZeroAmbientProbe;

        public static void Cleanup()
        {
            m_SkyboxMaterial = null;
            if (m_LookDevCompositing != null)
            {
                UnityEngine.Object.DestroyImmediate(m_LookDevCompositing);
                m_LookDevCompositing = null;
            }
        }

        public static void Initialize()
        {
            m_ZeroAmbientProbe.Clear();
            if (m_SkyboxMaterial == null)
            {
                m_SkyboxMaterial = new Material(Shader.Find("Skybox/Cubemap"));
            }
            if (m_ScreenQuadMesh == null)
            {
                m_ScreenQuadMesh = new Mesh();
                m_ScreenQuadMesh.vertices = new Vector3[] { new Vector3(-1f, -1f, 0f), new Vector3(1f, 1f, 0f), new Vector3(1f, -1f, 0f), new Vector3(-1f, 1f, 0f) };
                m_ScreenQuadMesh.triangles = new int[] { 0, 1, 2, 1, 0, 3 };
            }
            if (m_GBufferPatchMaterial == null)
            {
                m_GBufferPatchMaterial = new Material(EditorGUIUtility.LoadRequired("LookDevView/GBufferWhitePatch.shader") as Shader);
                m_DrawBallsMaterial = new Material(EditorGUIUtility.LoadRequired("LookDevView/GBufferBalls.shader") as Shader);
            }
            if (m_LookDevCompositing == null)
            {
                m_LookDevCompositing = new Material(EditorGUIUtility.LoadRequired("LookDevView/LookDevCompositing.shader") as Shader);
            }
            if (m_DeferredOverlayMaterial == null)
            {
                m_DeferredOverlayMaterial = EditorGUIUtility.LoadRequired("SceneView/SceneViewDeferredMaterial.mat") as Material;
            }
            if (m_DefaultHDRI == null)
            {
                m_DefaultHDRI = EditorGUIUtility.Load("LookDevView/DefaultHDRI.exr") as Cubemap;
                if (m_DefaultHDRI == null)
                {
                    m_DefaultHDRI = EditorGUIUtility.Load("LookDevView/DefaultHDRI.asset") as Cubemap;
                }
            }
            if (m_LookDevCubeToLatlong == null)
            {
                m_LookDevCubeToLatlong = new Material(EditorGUIUtility.LoadRequired("LookDevView/LookDevCubeToLatlong.shader") as Shader);
            }
            if (m_SelectionTexture == null)
            {
                m_SelectionTexture = new RenderTexture(250, 0x7d, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            }
            if (m_BrightestPointRT == null)
            {
                m_BrightestPointRT = new RenderTexture(250, 0x7d, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Default);
            }
            if (m_BrightestPointTexture == null)
            {
                m_BrightestPointTexture = new Texture2D(250, 0x7d, TextureFormat.RGBAHalf, false);
            }
        }

        public static void UpdateShadowInfoWithBrightestSpot(CubemapInfo cubemapInfo)
        {
            m_LookDevCubeToLatlong.SetTexture("_MainTex", cubemapInfo.cubemap);
            m_LookDevCubeToLatlong.SetVector("_WindowParams", new Vector4(10000f, -1000f, 2f, 0f));
            m_LookDevCubeToLatlong.SetVector("_CubeToLatLongParams", new Vector4(0.01745329f * cubemapInfo.angleOffset, 0.5f, 1f, 3f));
            m_LookDevCubeToLatlong.SetPass(0);
            int num = 250;
            int num2 = 0x7d;
            Graphics.Blit(cubemapInfo.cubemap, m_BrightestPointRT, m_LookDevCubeToLatlong);
            m_BrightestPointTexture.ReadPixels(new Rect(0f, 0f, (float) num, (float) num2), 0, 0, false);
            m_BrightestPointTexture.Apply();
            Color[] pixels = m_BrightestPointTexture.GetPixels();
            float num3 = 0f;
            for (int i = 0; i < num2; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    Vector3 vector = new Vector3(pixels[(i * num) + j].r, pixels[(i * num) + j].g, pixels[(i * num) + j].b);
                    float num6 = ((vector.x * 0.2126729f) + (vector.y * 0.7151522f)) + (vector.z * 0.072175f);
                    if (num3 < num6)
                    {
                        Vector2 vector2 = LookDevEnvironmentWindow.PositionToLatLong(new Vector2(((((float) j) / ((float) (num - 1))) * 2f) - 1f, ((((float) i) / ((float) (num2 - 1))) * 2f) - 1f));
                        cubemapInfo.shadowInfo.latitude = vector2.x;
                        cubemapInfo.shadowInfo.longitude = vector2.y - cubemapInfo.angleOffset;
                        num3 = num6;
                    }
                }
            }
        }
    }
}

