namespace UnityEditor.UI
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Sprites;

    internal class SpriteDrawUtility
    {
        private static Texture2D s_ContrastTex;

        private static Texture2D CreateCheckerTex(Color c0, Color c1)
        {
            Texture2D textured = new Texture2D(0x10, 0x10) {
                name = "[Generated] Checker Texture",
                hideFlags = HideFlags.DontSave
            };
            for (int i = 0; i < 8; i++)
            {
                for (int n = 0; n < 8; n++)
                {
                    textured.SetPixel(n, i, c1);
                }
            }
            for (int j = 8; j < 0x10; j++)
            {
                for (int num4 = 0; num4 < 8; num4++)
                {
                    textured.SetPixel(num4, j, c0);
                }
            }
            for (int k = 0; k < 8; k++)
            {
                for (int num6 = 8; num6 < 0x10; num6++)
                {
                    textured.SetPixel(num6, k, c0);
                }
            }
            for (int m = 8; m < 0x10; m++)
            {
                for (int num8 = 8; num8 < 0x10; num8++)
                {
                    textured.SetPixel(num8, m, c1);
                }
            }
            textured.Apply();
            textured.filterMode = UnityEngine.FilterMode.Point;
            return textured;
        }

        private static Texture2D CreateGradientTex()
        {
            Texture2D textured = new Texture2D(1, 0x10) {
                name = "[Generated] Gradient Texture",
                hideFlags = HideFlags.DontSave
            };
            Color a = new Color(1f, 1f, 1f, 0f);
            Color b = new Color(1f, 1f, 1f, 0.4f);
            for (int i = 0; i < 0x10; i++)
            {
                float t = Mathf.Abs((float) (((((float) i) / 15f) * 2f) - 1f));
                t *= t;
                textured.SetPixel(0, i, Color.Lerp(a, b, t));
            }
            textured.Apply();
            textured.filterMode = UnityEngine.FilterMode.Bilinear;
            return textured;
        }

        public static void DrawSprite(Sprite sprite, Rect drawArea, Color color)
        {
            if (sprite != null)
            {
                Texture2D texture = sprite.texture;
                if (texture != null)
                {
                    Rect outer = sprite.rect;
                    Rect inner = outer;
                    inner.xMin += sprite.border.x;
                    inner.yMin += sprite.border.y;
                    inner.xMax -= sprite.border.z;
                    inner.yMax -= sprite.border.w;
                    Vector4 outerUV = DataUtility.GetOuterUV(sprite);
                    Rect uv = new Rect(outerUV.x, outerUV.y, outerUV.z - outerUV.x, outerUV.w - outerUV.y);
                    Vector4 padding = DataUtility.GetPadding(sprite);
                    padding.x /= outer.width;
                    padding.y /= outer.height;
                    padding.z /= outer.width;
                    padding.w /= outer.height;
                    DrawSprite(texture, drawArea, padding, outer, inner, uv, color, null);
                }
            }
        }

        public static void DrawSprite(Texture tex, Rect drawArea, Rect outer, Rect uv, Color color)
        {
            DrawSprite(tex, drawArea, Vector4.zero, outer, outer, uv, color, null);
        }

        private static void DrawSprite(Texture tex, Rect drawArea, Vector4 padding, Rect outer, Rect inner, Rect uv, Color color, Material mat)
        {
            Rect position = drawArea;
            position.width = Mathf.Abs(outer.width);
            position.height = Mathf.Abs(outer.height);
            if (position.width > 0f)
            {
                float num = drawArea.width / position.width;
                position.width *= num;
                position.height *= num;
            }
            if (drawArea.height > position.height)
            {
                position.y += (drawArea.height - position.height) * 0.5f;
            }
            else if (position.height > drawArea.height)
            {
                float num2 = drawArea.height / position.height;
                position.width *= num2;
                position.height *= num2;
            }
            if (drawArea.width > position.width)
            {
                position.x += (drawArea.width - position.width) * 0.5f;
            }
            EditorGUI.DrawTextureTransparent(position, null, ScaleMode.ScaleToFit, outer.width / outer.height);
            GUI.color = color;
            Rect rect2 = new Rect(position.x + (position.width * padding.x), position.y + (position.height * padding.w), position.width - (position.width * (padding.z + padding.x)), position.height - (position.height * (padding.w + padding.y)));
            if (mat == null)
            {
                GL.sRGBWrite = QualitySettings.activeColorSpace == ColorSpace.Linear;
                GUI.DrawTextureWithTexCoords(rect2, tex, uv, true);
                GL.sRGBWrite = false;
            }
            else
            {
                EditorGUI.DrawPreviewTexture(rect2, tex, mat);
            }
            GUI.BeginGroup(position);
            tex = contrastTexture;
            GUI.color = Color.white;
            if (inner.xMin != outer.xMin)
            {
                float x = (((inner.xMin - outer.xMin) / outer.width) * position.width) - 1f;
                DrawTiledTexture(new Rect(x, 0f, 1f, position.height), tex);
            }
            if (inner.xMax != outer.xMax)
            {
                float num4 = (((inner.xMax - outer.xMin) / outer.width) * position.width) - 1f;
                DrawTiledTexture(new Rect(num4, 0f, 1f, position.height), tex);
            }
            if (inner.yMin != outer.yMin)
            {
                float num5 = (((inner.yMin - outer.yMin) / outer.height) * position.height) - 1f;
                DrawTiledTexture(new Rect(0f, position.height - num5, position.width, 1f), tex);
            }
            if (inner.yMax != outer.yMax)
            {
                float num6 = (((inner.yMax - outer.yMin) / outer.height) * position.height) - 1f;
                DrawTiledTexture(new Rect(0f, position.height - num6, position.width, 1f), tex);
            }
            GUI.EndGroup();
        }

        private static void DrawTiledTexture(Rect rect, Texture tex)
        {
            float width = rect.width / ((float) tex.width);
            float height = rect.height / ((float) tex.height);
            Rect texCoords = new Rect(0f, 0f, width, height);
            TextureWrapMode wrapMode = tex.wrapMode;
            tex.wrapMode = TextureWrapMode.Repeat;
            GUI.DrawTextureWithTexCoords(rect, tex, texCoords);
            tex.wrapMode = wrapMode;
        }

        private static Texture2D contrastTexture
        {
            get
            {
                if (s_ContrastTex == null)
                {
                    s_ContrastTex = CreateCheckerTex(new Color(0f, 0f, 0f, 0.5f), new Color(1f, 1f, 1f, 0.5f));
                }
                return s_ContrastTex;
            }
        }
    }
}

