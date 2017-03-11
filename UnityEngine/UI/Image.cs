namespace UnityEngine.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.Serialization;
    using UnityEngine.Sprites;

    /// <summary>
    /// <para>Displays a Sprite for the UI System.</para>
    /// </summary>
    [AddComponentMenu("UI/Image", 11)]
    public class Image : MaskableGraphic, ISerializationCallbackReceiver, ILayoutElement, ICanvasRaycastFilter
    {
        private float m_AlphaHitTestMinimumThreshold = 0f;
        [Range(0f, 1f), SerializeField]
        private float m_FillAmount = 1f;
        [SerializeField]
        private bool m_FillCenter = true;
        [SerializeField]
        private bool m_FillClockwise = true;
        [SerializeField]
        private FillMethod m_FillMethod = FillMethod.Radial360;
        [SerializeField]
        private int m_FillOrigin;
        [NonSerialized]
        private Sprite m_OverrideSprite;
        [SerializeField]
        private bool m_PreserveAspect = false;
        [FormerlySerializedAs("m_Frame"), SerializeField]
        private Sprite m_Sprite;
        [SerializeField]
        private Type m_Type = Type.Simple;
        protected static Material s_ETC1DefaultUI = null;
        private static readonly Vector3[] s_Uv = new Vector3[4];
        private static readonly Vector2[] s_UVScratch = new Vector2[4];
        private static readonly Vector2[] s_VertScratch = new Vector2[4];
        private static readonly Vector3[] s_Xy = new Vector3[4];

        protected Image()
        {
            base.useLegacyMeshGeneration = false;
        }

        private static void AddQuad(VertexHelper vertexHelper, Vector3[] quadPositions, Color32 color, Vector3[] quadUVs)
        {
            int currentVertCount = vertexHelper.currentVertCount;
            for (int i = 0; i < 4; i++)
            {
                vertexHelper.AddVert(quadPositions[i], color, quadUVs[i]);
            }
            vertexHelper.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
            vertexHelper.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
        }

        private static void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin, Vector2 uvMax)
        {
            int currentVertCount = vertexHelper.currentVertCount;
            vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0f), color, new Vector2(uvMin.x, uvMin.y));
            vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0f), color, new Vector2(uvMin.x, uvMax.y));
            vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0f), color, new Vector2(uvMax.x, uvMax.y));
            vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0f), color, new Vector2(uvMax.x, uvMin.y));
            vertexHelper.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
            vertexHelper.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
        }

        /// <summary>
        /// <para>See ILayoutElement.CalculateLayoutInputHorizontal.</para>
        /// </summary>
        public virtual void CalculateLayoutInputHorizontal()
        {
        }

        /// <summary>
        /// <para>See ILayoutElement.CalculateLayoutInputVertical.</para>
        /// </summary>
        public virtual void CalculateLayoutInputVertical()
        {
        }

        private void GenerateFilledSprite(VertexHelper toFill, bool preserveAspect)
        {
            toFill.Clear();
            if (this.m_FillAmount >= 0.001f)
            {
                Vector4 drawingDimensions = this.GetDrawingDimensions(preserveAspect);
                Vector4 vector2 = (this.activeSprite == null) ? Vector4.zero : DataUtility.GetOuterUV(this.activeSprite);
                UIVertex.simpleVert.color = this.color;
                float x = vector2.x;
                float y = vector2.y;
                float z = vector2.z;
                float w = vector2.w;
                if ((this.m_FillMethod == FillMethod.Horizontal) || (this.m_FillMethod == FillMethod.Vertical))
                {
                    if (this.fillMethod == FillMethod.Horizontal)
                    {
                        float num5 = (z - x) * this.m_FillAmount;
                        if (this.m_FillOrigin == 1)
                        {
                            drawingDimensions.x = drawingDimensions.z - ((drawingDimensions.z - drawingDimensions.x) * this.m_FillAmount);
                            x = z - num5;
                        }
                        else
                        {
                            drawingDimensions.z = drawingDimensions.x + ((drawingDimensions.z - drawingDimensions.x) * this.m_FillAmount);
                            z = x + num5;
                        }
                    }
                    else if (this.fillMethod == FillMethod.Vertical)
                    {
                        float num6 = (w - y) * this.m_FillAmount;
                        if (this.m_FillOrigin == 1)
                        {
                            drawingDimensions.y = drawingDimensions.w - ((drawingDimensions.w - drawingDimensions.y) * this.m_FillAmount);
                            y = w - num6;
                        }
                        else
                        {
                            drawingDimensions.w = drawingDimensions.y + ((drawingDimensions.w - drawingDimensions.y) * this.m_FillAmount);
                            w = y + num6;
                        }
                    }
                }
                s_Xy[0] = (Vector3) new Vector2(drawingDimensions.x, drawingDimensions.y);
                s_Xy[1] = (Vector3) new Vector2(drawingDimensions.x, drawingDimensions.w);
                s_Xy[2] = (Vector3) new Vector2(drawingDimensions.z, drawingDimensions.w);
                s_Xy[3] = (Vector3) new Vector2(drawingDimensions.z, drawingDimensions.y);
                s_Uv[0] = (Vector3) new Vector2(x, y);
                s_Uv[1] = (Vector3) new Vector2(x, w);
                s_Uv[2] = (Vector3) new Vector2(z, w);
                s_Uv[3] = (Vector3) new Vector2(z, y);
                if (((this.m_FillAmount < 1f) && (this.m_FillMethod != FillMethod.Horizontal)) && (this.m_FillMethod != FillMethod.Vertical))
                {
                    if (this.fillMethod == FillMethod.Radial90)
                    {
                        if (RadialCut(s_Xy, s_Uv, this.m_FillAmount, this.m_FillClockwise, this.m_FillOrigin))
                        {
                            AddQuad(toFill, s_Xy, this.color, s_Uv);
                        }
                    }
                    else if (this.fillMethod == FillMethod.Radial180)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            float num8;
                            float num9;
                            float num10;
                            float num11;
                            int num12 = (this.m_FillOrigin <= 1) ? 0 : 1;
                            if ((this.m_FillOrigin == 0) || (this.m_FillOrigin == 2))
                            {
                                num10 = 0f;
                                num11 = 1f;
                                if (i == num12)
                                {
                                    num8 = 0f;
                                    num9 = 0.5f;
                                }
                                else
                                {
                                    num8 = 0.5f;
                                    num9 = 1f;
                                }
                            }
                            else
                            {
                                num8 = 0f;
                                num9 = 1f;
                                if (i == num12)
                                {
                                    num10 = 0.5f;
                                    num11 = 1f;
                                }
                                else
                                {
                                    num10 = 0f;
                                    num11 = 0.5f;
                                }
                            }
                            s_Xy[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, num8);
                            s_Xy[1].x = s_Xy[0].x;
                            s_Xy[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, num9);
                            s_Xy[3].x = s_Xy[2].x;
                            s_Xy[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, num10);
                            s_Xy[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, num11);
                            s_Xy[2].y = s_Xy[1].y;
                            s_Xy[3].y = s_Xy[0].y;
                            s_Uv[0].x = Mathf.Lerp(x, z, num8);
                            s_Uv[1].x = s_Uv[0].x;
                            s_Uv[2].x = Mathf.Lerp(x, z, num9);
                            s_Uv[3].x = s_Uv[2].x;
                            s_Uv[0].y = Mathf.Lerp(y, w, num10);
                            s_Uv[1].y = Mathf.Lerp(y, w, num11);
                            s_Uv[2].y = s_Uv[1].y;
                            s_Uv[3].y = s_Uv[0].y;
                            float num13 = !this.m_FillClockwise ? ((this.m_FillAmount * 2f) - (1 - i)) : ((this.fillAmount * 2f) - i);
                            if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(num13), this.m_FillClockwise, ((i + this.m_FillOrigin) + 3) % 4))
                            {
                                AddQuad(toFill, s_Xy, this.color, s_Uv);
                            }
                        }
                    }
                    else if (this.fillMethod == FillMethod.Radial360)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            float num15;
                            float num16;
                            float num17;
                            float num18;
                            if (j < 2)
                            {
                                num15 = 0f;
                                num16 = 0.5f;
                            }
                            else
                            {
                                num15 = 0.5f;
                                num16 = 1f;
                            }
                            switch (j)
                            {
                                case 0:
                                case 3:
                                    num17 = 0f;
                                    num18 = 0.5f;
                                    break;

                                default:
                                    num17 = 0.5f;
                                    num18 = 1f;
                                    break;
                            }
                            s_Xy[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, num15);
                            s_Xy[1].x = s_Xy[0].x;
                            s_Xy[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, num16);
                            s_Xy[3].x = s_Xy[2].x;
                            s_Xy[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, num17);
                            s_Xy[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, num18);
                            s_Xy[2].y = s_Xy[1].y;
                            s_Xy[3].y = s_Xy[0].y;
                            s_Uv[0].x = Mathf.Lerp(x, z, num15);
                            s_Uv[1].x = s_Uv[0].x;
                            s_Uv[2].x = Mathf.Lerp(x, z, num16);
                            s_Uv[3].x = s_Uv[2].x;
                            s_Uv[0].y = Mathf.Lerp(y, w, num17);
                            s_Uv[1].y = Mathf.Lerp(y, w, num18);
                            s_Uv[2].y = s_Uv[1].y;
                            s_Uv[3].y = s_Uv[0].y;
                            float num19 = !this.m_FillClockwise ? ((this.m_FillAmount * 4f) - (3 - ((j + this.m_FillOrigin) % 4))) : ((this.m_FillAmount * 4f) - ((j + this.m_FillOrigin) % 4));
                            if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(num19), this.m_FillClockwise, (j + 2) % 4))
                            {
                                AddQuad(toFill, s_Xy, this.color, s_Uv);
                            }
                        }
                    }
                }
                else
                {
                    AddQuad(toFill, s_Xy, this.color, s_Uv);
                }
            }
        }

        private void GenerateSimpleSprite(VertexHelper vh, bool lPreserveAspect)
        {
            Vector4 drawingDimensions = this.GetDrawingDimensions(lPreserveAspect);
            Vector4 vector2 = (this.activeSprite == null) ? Vector4.zero : DataUtility.GetOuterUV(this.activeSprite);
            Color color = this.color;
            vh.Clear();
            vh.AddVert(new Vector3(drawingDimensions.x, drawingDimensions.y), color, new Vector2(vector2.x, vector2.y));
            vh.AddVert(new Vector3(drawingDimensions.x, drawingDimensions.w), color, new Vector2(vector2.x, vector2.w));
            vh.AddVert(new Vector3(drawingDimensions.z, drawingDimensions.w), color, new Vector2(vector2.z, vector2.w));
            vh.AddVert(new Vector3(drawingDimensions.z, drawingDimensions.y), color, new Vector2(vector2.z, vector2.y));
            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }

        private void GenerateSlicedSprite(VertexHelper toFill)
        {
            if (!this.hasBorder)
            {
                this.GenerateSimpleSprite(toFill, false);
            }
            else
            {
                Vector4 outerUV;
                Vector4 innerUV;
                Vector4 padding;
                Vector4 border;
                if (this.activeSprite != null)
                {
                    outerUV = DataUtility.GetOuterUV(this.activeSprite);
                    innerUV = DataUtility.GetInnerUV(this.activeSprite);
                    padding = DataUtility.GetPadding(this.activeSprite);
                    border = this.activeSprite.border;
                }
                else
                {
                    outerUV = Vector4.zero;
                    innerUV = Vector4.zero;
                    padding = Vector4.zero;
                    border = Vector4.zero;
                }
                Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
                Vector4 adjustedBorders = this.GetAdjustedBorders((Vector4) (border / this.pixelsPerUnit), pixelAdjustedRect);
                padding = (Vector4) (padding / this.pixelsPerUnit);
                s_VertScratch[0] = new Vector2(padding.x, padding.y);
                s_VertScratch[3] = new Vector2(pixelAdjustedRect.width - padding.z, pixelAdjustedRect.height - padding.w);
                s_VertScratch[1].x = adjustedBorders.x;
                s_VertScratch[1].y = adjustedBorders.y;
                s_VertScratch[2].x = pixelAdjustedRect.width - adjustedBorders.z;
                s_VertScratch[2].y = pixelAdjustedRect.height - adjustedBorders.w;
                for (int i = 0; i < 4; i++)
                {
                    s_VertScratch[i].x += pixelAdjustedRect.x;
                    s_VertScratch[i].y += pixelAdjustedRect.y;
                }
                s_UVScratch[0] = new Vector2(outerUV.x, outerUV.y);
                s_UVScratch[1] = new Vector2(innerUV.x, innerUV.y);
                s_UVScratch[2] = new Vector2(innerUV.z, innerUV.w);
                s_UVScratch[3] = new Vector2(outerUV.z, outerUV.w);
                toFill.Clear();
                for (int j = 0; j < 3; j++)
                {
                    int index = j + 1;
                    for (int k = 0; k < 3; k++)
                    {
                        if ((this.m_FillCenter || (j != 1)) || (k != 1))
                        {
                            int num5 = k + 1;
                            AddQuad(toFill, new Vector2(s_VertScratch[j].x, s_VertScratch[k].y), new Vector2(s_VertScratch[index].x, s_VertScratch[num5].y), this.color, new Vector2(s_UVScratch[j].x, s_UVScratch[k].y), new Vector2(s_UVScratch[index].x, s_UVScratch[num5].y));
                        }
                    }
                }
            }
        }

        private void GenerateTiledSprite(VertexHelper toFill)
        {
            Vector4 outerUV;
            Vector4 innerUV;
            Vector4 adjustedBorders;
            Vector2 size;
            if (this.activeSprite != null)
            {
                outerUV = DataUtility.GetOuterUV(this.activeSprite);
                innerUV = DataUtility.GetInnerUV(this.activeSprite);
                adjustedBorders = this.activeSprite.border;
                size = this.activeSprite.rect.size;
            }
            else
            {
                outerUV = Vector4.zero;
                innerUV = Vector4.zero;
                adjustedBorders = Vector4.zero;
                size = (Vector2) (Vector2.one * 100f);
            }
            Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
            float num = ((size.x - adjustedBorders.x) - adjustedBorders.z) / this.pixelsPerUnit;
            float num2 = ((size.y - adjustedBorders.y) - adjustedBorders.w) / this.pixelsPerUnit;
            adjustedBorders = this.GetAdjustedBorders((Vector4) (adjustedBorders / this.pixelsPerUnit), pixelAdjustedRect);
            Vector2 uvMin = new Vector2(innerUV.x, innerUV.y);
            Vector2 a = new Vector2(innerUV.z, innerUV.w);
            float x = adjustedBorders.x;
            float num4 = pixelAdjustedRect.width - adjustedBorders.z;
            float y = adjustedBorders.y;
            float num6 = pixelAdjustedRect.height - adjustedBorders.w;
            toFill.Clear();
            Vector2 uvMax = a;
            if (num <= 0f)
            {
                num = num4 - x;
            }
            if (num2 <= 0f)
            {
                num2 = num6 - y;
            }
            if ((this.activeSprite != null) && ((this.hasBorder || this.activeSprite.packed) || (this.activeSprite.texture.wrapMode != TextureWrapMode.Repeat)))
            {
                int num7 = 0;
                int num8 = 0;
                if (this.m_FillCenter)
                {
                    num7 = (int) Math.Ceiling((double) ((num4 - x) / num));
                    num8 = (int) Math.Ceiling((double) ((num6 - y) / num2));
                    int num9 = 0;
                    if (this.hasBorder)
                    {
                        num9 = ((num7 + 2) * (num8 + 2)) * 4;
                    }
                    else
                    {
                        num9 = (num7 * num8) * 4;
                    }
                    if (num9 > 0xfde8)
                    {
                        double num11;
                        Debug.LogError("Too many sprite tiles on Image \"" + base.name + "\". The tile size will be increased. To remove the limit on the number of tiles, convert the Sprite to an Advanced texture, remove the borders, clear the Packing tag and set the Wrap mode to Repeat.", this);
                        double num10 = 16250.0;
                        if (this.hasBorder)
                        {
                            num11 = (num7 + 2.0) / (num8 + 2.0);
                        }
                        else
                        {
                            num11 = ((double) num7) / ((double) num8);
                        }
                        double d = Math.Sqrt(num10 / num11);
                        double num13 = d * num11;
                        if (this.hasBorder)
                        {
                            d -= 2.0;
                            num13 -= 2.0;
                        }
                        num7 = (int) Math.Floor(d);
                        num8 = (int) Math.Floor(num13);
                        num = (num4 - x) / ((float) num7);
                        num2 = (num6 - y) / ((float) num8);
                    }
                }
                else if (this.hasBorder)
                {
                    num7 = (int) Math.Ceiling((double) ((num4 - x) / num));
                    num8 = (int) Math.Ceiling((double) ((num6 - y) / num2));
                    int num14 = (((num8 + num7) + 2) * 2) * 4;
                    if (num14 > 0xfde8)
                    {
                        Debug.LogError("Too many sprite tiles on Image \"" + base.name + "\". The tile size will be increased. To remove the limit on the number of tiles, convert the Sprite to an Advanced texture, remove the borders, clear the Packing tag and set the Wrap mode to Repeat.", this);
                        double num15 = 16250.0;
                        double num16 = ((double) num7) / ((double) num8);
                        double num17 = (num15 - 4.0) / (2.0 * (1.0 + num16));
                        double num18 = num17 * num16;
                        num7 = (int) Math.Floor(num17);
                        num8 = (int) Math.Floor(num18);
                        num = (num4 - x) / ((float) num7);
                        num2 = (num6 - y) / ((float) num8);
                    }
                }
                else
                {
                    num8 = num7 = 0;
                }
                if (this.m_FillCenter)
                {
                    for (int i = 0; i < num8; i++)
                    {
                        float num20 = y + (i * num2);
                        float num21 = y + ((i + 1) * num2);
                        if (num21 > num6)
                        {
                            uvMax.y = uvMin.y + (((a.y - uvMin.y) * (num6 - num20)) / (num21 - num20));
                            num21 = num6;
                        }
                        uvMax.x = a.x;
                        for (int j = 0; j < num7; j++)
                        {
                            float num23 = x + (j * num);
                            float num24 = x + ((j + 1) * num);
                            if (num24 > num4)
                            {
                                uvMax.x = uvMin.x + (((a.x - uvMin.x) * (num4 - num23)) / (num24 - num23));
                                num24 = num4;
                            }
                            AddQuad(toFill, new Vector2(num23, num20) + pixelAdjustedRect.position, new Vector2(num24, num21) + pixelAdjustedRect.position, this.color, uvMin, uvMax);
                        }
                    }
                }
                if (this.hasBorder)
                {
                    uvMax = a;
                    for (int k = 0; k < num8; k++)
                    {
                        float num26 = y + (k * num2);
                        float num27 = y + ((k + 1) * num2);
                        if (num27 > num6)
                        {
                            uvMax.y = uvMin.y + (((a.y - uvMin.y) * (num6 - num26)) / (num27 - num26));
                            num27 = num6;
                        }
                        AddQuad(toFill, new Vector2(0f, num26) + pixelAdjustedRect.position, new Vector2(x, num27) + pixelAdjustedRect.position, this.color, new Vector2(outerUV.x, uvMin.y), new Vector2(uvMin.x, uvMax.y));
                        AddQuad(toFill, new Vector2(num4, num26) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, num27) + pixelAdjustedRect.position, this.color, new Vector2(a.x, uvMin.y), new Vector2(outerUV.z, uvMax.y));
                    }
                    uvMax = a;
                    for (int m = 0; m < num7; m++)
                    {
                        float num29 = x + (m * num);
                        float num30 = x + ((m + 1) * num);
                        if (num30 > num4)
                        {
                            uvMax.x = uvMin.x + (((a.x - uvMin.x) * (num4 - num29)) / (num30 - num29));
                            num30 = num4;
                        }
                        AddQuad(toFill, new Vector2(num29, 0f) + pixelAdjustedRect.position, new Vector2(num30, y) + pixelAdjustedRect.position, this.color, new Vector2(uvMin.x, outerUV.y), new Vector2(uvMax.x, uvMin.y));
                        AddQuad(toFill, new Vector2(num29, num6) + pixelAdjustedRect.position, new Vector2(num30, pixelAdjustedRect.height) + pixelAdjustedRect.position, this.color, new Vector2(uvMin.x, a.y), new Vector2(uvMax.x, outerUV.w));
                    }
                    AddQuad(toFill, new Vector2(0f, 0f) + pixelAdjustedRect.position, new Vector2(x, y) + pixelAdjustedRect.position, this.color, new Vector2(outerUV.x, outerUV.y), new Vector2(uvMin.x, uvMin.y));
                    AddQuad(toFill, new Vector2(num4, 0f) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, y) + pixelAdjustedRect.position, this.color, new Vector2(a.x, outerUV.y), new Vector2(outerUV.z, uvMin.y));
                    AddQuad(toFill, new Vector2(0f, num6) + pixelAdjustedRect.position, new Vector2(x, pixelAdjustedRect.height) + pixelAdjustedRect.position, this.color, new Vector2(outerUV.x, a.y), new Vector2(uvMin.x, outerUV.w));
                    AddQuad(toFill, new Vector2(num4, num6) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, pixelAdjustedRect.height) + pixelAdjustedRect.position, this.color, new Vector2(a.x, a.y), new Vector2(outerUV.z, outerUV.w));
                }
            }
            else
            {
                Vector2 b = new Vector2((num4 - x) / num, (num6 - y) / num2);
                if (this.m_FillCenter)
                {
                    AddQuad(toFill, new Vector2(x, y) + pixelAdjustedRect.position, new Vector2(num4, num6) + pixelAdjustedRect.position, this.color, Vector2.Scale(uvMin, b), Vector2.Scale(a, b));
                }
            }
        }

        private unsafe Vector4 GetAdjustedBorders(Vector4 border, Rect adjustedRect)
        {
            Rect rect = base.rectTransform.rect;
            for (int i = 0; i <= 1; i++)
            {
                float num2;
                ref Vector4 vectorRef;
                if (rect.size[i] != 0f)
                {
                    int num3;
                    int num4;
                    num2 = adjustedRect.size[i] / rect.size[i];
                    (vectorRef = (Vector4) &border)[num3 = i] = vectorRef[num3] * num2;
                    (vectorRef = (Vector4) &border)[num4 = i + 2] = vectorRef[num4] * num2;
                }
                float num5 = border[i] + border[i + 2];
                if ((adjustedRect.size[i] < num5) && (num5 != 0f))
                {
                    int num6;
                    int num7;
                    num2 = adjustedRect.size[i] / num5;
                    (vectorRef = (Vector4) &border)[num6 = i] = vectorRef[num6] * num2;
                    (vectorRef = (Vector4) &border)[num7 = i + 2] = vectorRef[num7] * num2;
                }
            }
            return border;
        }

        private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
        {
            Vector4 vector = (this.activeSprite != null) ? DataUtility.GetPadding(this.activeSprite) : Vector4.zero;
            Vector2 vector2 = (this.activeSprite != null) ? new Vector2(this.activeSprite.rect.width, this.activeSprite.rect.height) : Vector2.zero;
            Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
            int num = Mathf.RoundToInt(vector2.x);
            int num2 = Mathf.RoundToInt(vector2.y);
            Vector4 vector3 = new Vector4(vector.x / ((float) num), vector.y / ((float) num2), (num - vector.z) / ((float) num), (num2 - vector.w) / ((float) num2));
            if (shouldPreserveAspect && (vector2.sqrMagnitude > 0f))
            {
                float num3 = vector2.x / vector2.y;
                float num4 = pixelAdjustedRect.width / pixelAdjustedRect.height;
                if (num3 > num4)
                {
                    float height = pixelAdjustedRect.height;
                    pixelAdjustedRect.height = pixelAdjustedRect.width * (1f / num3);
                    pixelAdjustedRect.y += (height - pixelAdjustedRect.height) * base.rectTransform.pivot.y;
                }
                else
                {
                    float width = pixelAdjustedRect.width;
                    pixelAdjustedRect.width = pixelAdjustedRect.height * num3;
                    pixelAdjustedRect.x += (width - pixelAdjustedRect.width) * base.rectTransform.pivot.x;
                }
            }
            return new Vector4(pixelAdjustedRect.x + (pixelAdjustedRect.width * vector3.x), pixelAdjustedRect.y + (pixelAdjustedRect.height * vector3.y), pixelAdjustedRect.x + (pixelAdjustedRect.width * vector3.z), pixelAdjustedRect.y + (pixelAdjustedRect.height * vector3.w));
        }

        /// <summary>
        /// <para>See:ICanvasRaycastFilter.</para>
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <param name="eventCamera"></param>
        public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            Vector2 vector;
            if (this.alphaHitTestMinimumThreshold <= 0f)
            {
                return true;
            }
            if (this.alphaHitTestMinimumThreshold > 1f)
            {
                return false;
            }
            if (this.activeSprite == null)
            {
                return true;
            }
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, screenPoint, eventCamera, out vector))
            {
                return false;
            }
            Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
            vector.x += base.rectTransform.pivot.x * pixelAdjustedRect.width;
            vector.y += base.rectTransform.pivot.y * pixelAdjustedRect.height;
            vector = this.MapCoordinate(vector, pixelAdjustedRect);
            Rect textureRect = this.activeSprite.textureRect;
            Vector2 vector4 = new Vector2(vector.x / textureRect.width, vector.y / textureRect.height);
            float u = Mathf.Lerp(textureRect.x, textureRect.xMax, vector4.x) / ((float) this.activeSprite.texture.width);
            float v = Mathf.Lerp(textureRect.y, textureRect.yMax, vector4.y) / ((float) this.activeSprite.texture.height);
            try
            {
                return (this.activeSprite.texture.GetPixelBilinear(u, v).a >= this.alphaHitTestMinimumThreshold);
            }
            catch (UnityException exception)
            {
                Debug.LogError("Using alphaHitTestMinimumThreshold greater than 0 on Image whose sprite texture cannot be read. " + exception.Message + " Also make sure to disable sprite packing for this sprite.", this);
                return true;
            }
        }

        private unsafe Vector2 MapCoordinate(Vector2 local, Rect rect)
        {
            Rect rect2 = this.activeSprite.rect;
            if ((this.type == Type.Simple) || (this.type == Type.Filled))
            {
                return new Vector2((local.x * rect2.width) / rect.width, (local.y * rect2.height) / rect.height);
            }
            Vector4 border = this.activeSprite.border;
            Vector4 adjustedBorders = this.GetAdjustedBorders((Vector4) (border / this.pixelsPerUnit), rect);
            for (int i = 0; i < 2; i++)
            {
                if (local[i] > adjustedBorders[i])
                {
                    ref Vector2 vectorRef;
                    if ((rect.size[i] - local[i]) <= adjustedBorders[i + 2])
                    {
                        int num2;
                        (vectorRef = (Vector2) &local)[num2 = i] = vectorRef[num2] - (rect.size[i] - rect2.size[i]);
                    }
                    else if (this.type == Type.Sliced)
                    {
                        float t = Mathf.InverseLerp(adjustedBorders[i], rect.size[i] - adjustedBorders[i + 2], local[i]);
                        local[i] = Mathf.Lerp(border[i], rect2.size[i] - border[i + 2], t);
                    }
                    else
                    {
                        int num4;
                        int num5;
                        (vectorRef = (Vector2) &local)[num4 = i] = vectorRef[num4] - adjustedBorders[i];
                        local[i] = Mathf.Repeat(local[i], (rect2.size[i] - border[i]) - border[i + 2]);
                        (vectorRef = (Vector2) &local)[num5 = i] = vectorRef[num5] + border[i];
                    }
                }
            }
            return local;
        }

        /// <summary>
        /// <para>Serialization Callback.</para>
        /// </summary>
        public virtual void OnAfterDeserialize()
        {
            if (this.m_FillOrigin < 0)
            {
                this.m_FillOrigin = 0;
            }
            else if ((this.m_FillMethod == FillMethod.Horizontal) && (this.m_FillOrigin > 1))
            {
                this.m_FillOrigin = 0;
            }
            else if ((this.m_FillMethod == FillMethod.Vertical) && (this.m_FillOrigin > 1))
            {
                this.m_FillOrigin = 0;
            }
            else if (this.m_FillOrigin > 3)
            {
                this.m_FillOrigin = 0;
            }
            this.m_FillAmount = Mathf.Clamp(this.m_FillAmount, 0f, 1f);
        }

        /// <summary>
        /// <para>Serialization Callback.</para>
        /// </summary>
        public virtual void OnBeforeSerialize()
        {
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (this.activeSprite == null)
            {
                base.OnPopulateMesh(toFill);
            }
            else
            {
                switch (this.type)
                {
                    case Type.Simple:
                        this.GenerateSimpleSprite(toFill, this.m_PreserveAspect);
                        break;

                    case Type.Sliced:
                        this.GenerateSlicedSprite(toFill);
                        break;

                    case Type.Tiled:
                        this.GenerateTiledSprite(toFill);
                        break;

                    case Type.Filled:
                        this.GenerateFilledSprite(toFill, this.m_PreserveAspect);
                        break;
                }
            }
        }

        private static bool RadialCut(Vector3[] xy, Vector3[] uv, float fill, bool invert, int corner)
        {
            if (fill < 0.001f)
            {
                return false;
            }
            if ((corner & 1) == 1)
            {
                invert = !invert;
            }
            if (invert || (fill <= 0.999f))
            {
                float f = Mathf.Clamp01(fill);
                if (invert)
                {
                    f = 1f - f;
                }
                f *= 1.570796f;
                float cos = Mathf.Cos(f);
                float sin = Mathf.Sin(f);
                RadialCut(xy, cos, sin, invert, corner);
                RadialCut(uv, cos, sin, invert, corner);
            }
            return true;
        }

        private static void RadialCut(Vector3[] xy, float cos, float sin, bool invert, int corner)
        {
            int index = corner;
            int num2 = (corner + 1) % 4;
            int num3 = (corner + 2) % 4;
            int num4 = (corner + 3) % 4;
            if ((corner & 1) == 1)
            {
                if (sin > cos)
                {
                    cos /= sin;
                    sin = 1f;
                    if (invert)
                    {
                        xy[num2].x = Mathf.Lerp(xy[index].x, xy[num3].x, cos);
                        xy[num3].x = xy[num2].x;
                    }
                }
                else if (cos > sin)
                {
                    sin /= cos;
                    cos = 1f;
                    if (!invert)
                    {
                        xy[num3].y = Mathf.Lerp(xy[index].y, xy[num3].y, sin);
                        xy[num4].y = xy[num3].y;
                    }
                }
                else
                {
                    cos = 1f;
                    sin = 1f;
                }
                if (!invert)
                {
                    xy[num4].x = Mathf.Lerp(xy[index].x, xy[num3].x, cos);
                }
                else
                {
                    xy[num2].y = Mathf.Lerp(xy[index].y, xy[num3].y, sin);
                }
            }
            else
            {
                if (cos > sin)
                {
                    sin /= cos;
                    cos = 1f;
                    if (!invert)
                    {
                        xy[num2].y = Mathf.Lerp(xy[index].y, xy[num3].y, sin);
                        xy[num3].y = xy[num2].y;
                    }
                }
                else if (sin > cos)
                {
                    cos /= sin;
                    sin = 1f;
                    if (invert)
                    {
                        xy[num3].x = Mathf.Lerp(xy[index].x, xy[num3].x, cos);
                        xy[num4].x = xy[num3].x;
                    }
                }
                else
                {
                    cos = 1f;
                    sin = 1f;
                }
                if (invert)
                {
                    xy[num4].y = Mathf.Lerp(xy[index].y, xy[num3].y, sin);
                }
                else
                {
                    xy[num2].x = Mathf.Lerp(xy[index].x, xy[num3].x, cos);
                }
            }
        }

        /// <summary>
        /// <para>Adjusts the image size to make it pixel-perfect.</para>
        /// </summary>
        public override void SetNativeSize()
        {
            if (this.activeSprite != null)
            {
                float x = this.activeSprite.rect.width / this.pixelsPerUnit;
                float y = this.activeSprite.rect.height / this.pixelsPerUnit;
                base.rectTransform.anchorMax = base.rectTransform.anchorMin;
                base.rectTransform.sizeDelta = new Vector2(x, y);
                this.SetAllDirty();
            }
        }

        protected override void UpdateMaterial()
        {
            base.UpdateMaterial();
            if (this.activeSprite == null)
            {
                base.canvasRenderer.SetAlphaTexture(null);
            }
            else
            {
                Texture2D associatedAlphaSplitTexture = this.activeSprite.associatedAlphaSplitTexture;
                if (associatedAlphaSplitTexture != null)
                {
                    base.canvasRenderer.SetAlphaTexture(associatedAlphaSplitTexture);
                }
            }
        }

        private Sprite activeSprite =>
            ((this.m_OverrideSprite == null) ? this.sprite : this.m_OverrideSprite);

        /// <summary>
        /// <para>The alpha threshold specifies the minimum alpha a pixel must have for the event to considered a "hit" on the Image.</para>
        /// </summary>
        public float alphaHitTestMinimumThreshold
        {
            get => 
                this.m_AlphaHitTestMinimumThreshold;
            set
            {
                this.m_AlphaHitTestMinimumThreshold = value;
            }
        }

        /// <summary>
        /// <para>Cache of the default Canvas Ericsson Texture Compression 1 (ETC1) and alpha Material.</para>
        /// </summary>
        public static Material defaultETC1GraphicMaterial
        {
            get
            {
                if (s_ETC1DefaultUI == null)
                {
                    s_ETC1DefaultUI = Canvas.GetETC1SupportedCanvasMaterial();
                }
                return s_ETC1DefaultUI;
            }
        }

        /// <summary>
        /// <para>The alpha threshold specifies the minimum alpha a pixel must have for the event to considered a "hit" on the Image.</para>
        /// </summary>
        [Obsolete("eventAlphaThreshold has been deprecated. Use eventMinimumAlphaThreshold instead (UnityUpgradable) -> alphaHitTestMinimumThreshold")]
        public float eventAlphaThreshold
        {
            get => 
                (1f - this.alphaHitTestMinimumThreshold);
            set
            {
                this.alphaHitTestMinimumThreshold = 1f - value;
            }
        }

        /// <summary>
        /// <para>Amount of the Image shown when the Image.type is set to Image.Type.Filled.</para>
        /// </summary>
        public float fillAmount
        {
            get => 
                this.m_FillAmount;
            set
            {
                if (SetPropertyUtility.SetStruct<float>(ref this.m_FillAmount, Mathf.Clamp01(value)))
                {
                    this.SetVerticesDirty();
                }
            }
        }

        /// <summary>
        /// <para>Whether or not to render the center of a Tiled or Sliced image.</para>
        /// </summary>
        public bool fillCenter
        {
            get => 
                this.m_FillCenter;
            set
            {
                if (SetPropertyUtility.SetStruct<bool>(ref this.m_FillCenter, value))
                {
                    this.SetVerticesDirty();
                }
            }
        }

        /// <summary>
        /// <para>Whether the Image should be filled clockwise (true) or counter-clockwise (false).</para>
        /// </summary>
        public bool fillClockwise
        {
            get => 
                this.m_FillClockwise;
            set
            {
                if (SetPropertyUtility.SetStruct<bool>(ref this.m_FillClockwise, value))
                {
                    this.SetVerticesDirty();
                }
            }
        }

        /// <summary>
        /// <para>What type of fill method to use.</para>
        /// </summary>
        public FillMethod fillMethod
        {
            get => 
                this.m_FillMethod;
            set
            {
                if (SetPropertyUtility.SetStruct<FillMethod>(ref this.m_FillMethod, value))
                {
                    this.SetVerticesDirty();
                    this.m_FillOrigin = 0;
                }
            }
        }

        /// <summary>
        /// <para>Controls the origin point of the Fill process. Value means different things with each fill method.</para>
        /// </summary>
        public int fillOrigin
        {
            get => 
                this.m_FillOrigin;
            set
            {
                if (SetPropertyUtility.SetStruct<int>(ref this.m_FillOrigin, value))
                {
                    this.SetVerticesDirty();
                }
            }
        }

        /// <summary>
        /// <para>See ILayoutElement.flexibleHeight.</para>
        /// </summary>
        public virtual float flexibleHeight =>
            -1f;

        /// <summary>
        /// <para>See ILayoutElement.flexibleWidth.</para>
        /// </summary>
        public virtual float flexibleWidth =>
            -1f;

        /// <summary>
        /// <para>True if the sprite used has borders.</para>
        /// </summary>
        public bool hasBorder =>
            ((this.activeSprite != null) && (this.activeSprite.border.sqrMagnitude > 0f));

        /// <summary>
        /// <para>See ILayoutElement.layoutPriority.</para>
        /// </summary>
        public virtual int layoutPriority =>
            0;

        /// <summary>
        /// <para>The image's texture. (ReadOnly).</para>
        /// </summary>
        public override Texture mainTexture =>
            this.activeSprite?.texture;

        /// <summary>
        /// <para>The specified Material used by this Image. The default Material is used instead if one wasn't specified.</para>
        /// </summary>
        public override Material material
        {
            get
            {
                if (base.m_Material != null)
                {
                    return base.m_Material;
                }
                if ((this.activeSprite != null) && (this.activeSprite.associatedAlphaSplitTexture != null))
                {
                    return defaultETC1GraphicMaterial;
                }
                return this.defaultMaterial;
            }
            set
            {
                base.material = value;
            }
        }

        /// <summary>
        /// <para>See ILayoutElement.minHeight.</para>
        /// </summary>
        public virtual float minHeight =>
            0f;

        /// <summary>
        /// <para>See ILayoutElement.minWidth.</para>
        /// </summary>
        public virtual float minWidth =>
            0f;

        /// <summary>
        /// <para>Set an override sprite to be used for rendering.</para>
        /// </summary>
        public Sprite overrideSprite
        {
            get => 
                this.activeSprite;
            set
            {
                if (SetPropertyUtility.SetClass<Sprite>(ref this.m_OverrideSprite, value))
                {
                    this.SetAllDirty();
                }
            }
        }

        public float pixelsPerUnit
        {
            get
            {
                float pixelsPerUnit = 100f;
                if (this.activeSprite != null)
                {
                    pixelsPerUnit = this.activeSprite.pixelsPerUnit;
                }
                float referencePixelsPerUnit = 100f;
                if (base.canvas != null)
                {
                    referencePixelsPerUnit = base.canvas.referencePixelsPerUnit;
                }
                return (pixelsPerUnit / referencePixelsPerUnit);
            }
        }

        /// <summary>
        /// <para>See ILayoutElement.preferredHeight.</para>
        /// </summary>
        public virtual float preferredHeight
        {
            get
            {
                if (this.activeSprite == null)
                {
                    return 0f;
                }
                if ((this.type == Type.Sliced) || (this.type == Type.Tiled))
                {
                    return (DataUtility.GetMinSize(this.activeSprite).y / this.pixelsPerUnit);
                }
                return (this.activeSprite.rect.size.y / this.pixelsPerUnit);
            }
        }

        /// <summary>
        /// <para>See ILayoutElement.preferredWidth.</para>
        /// </summary>
        public virtual float preferredWidth
        {
            get
            {
                if (this.activeSprite == null)
                {
                    return 0f;
                }
                if ((this.type == Type.Sliced) || (this.type == Type.Tiled))
                {
                    return (DataUtility.GetMinSize(this.activeSprite).x / this.pixelsPerUnit);
                }
                return (this.activeSprite.rect.size.x / this.pixelsPerUnit);
            }
        }

        /// <summary>
        /// <para>Whether this image should preserve its Sprite aspect ratio.</para>
        /// </summary>
        public bool preserveAspect
        {
            get => 
                this.m_PreserveAspect;
            set
            {
                if (SetPropertyUtility.SetStruct<bool>(ref this.m_PreserveAspect, value))
                {
                    this.SetVerticesDirty();
                }
            }
        }

        /// <summary>
        /// <para>The sprite that is used to render this image.</para>
        /// </summary>
        public Sprite sprite
        {
            get => 
                this.m_Sprite;
            set
            {
                if (SetPropertyUtility.SetClass<Sprite>(ref this.m_Sprite, value))
                {
                    this.SetAllDirty();
                }
            }
        }

        /// <summary>
        /// <para>How to display the image.</para>
        /// </summary>
        public Type type
        {
            get => 
                this.m_Type;
            set
            {
                if (SetPropertyUtility.SetStruct<Type>(ref this.m_Type, value))
                {
                    this.SetVerticesDirty();
                }
            }
        }

        /// <summary>
        /// <para>Fill method to be used by the Image.</para>
        /// </summary>
        public enum FillMethod
        {
            Horizontal,
            Vertical,
            Radial90,
            Radial180,
            Radial360
        }

        /// <summary>
        /// <para>Origin for the Image.FillMethod.Radial180.</para>
        /// </summary>
        public enum Origin180
        {
            Bottom,
            Left,
            Top,
            Right
        }

        /// <summary>
        /// <para>One of the points of the Arc for the Image.FillMethod.Radial360.</para>
        /// </summary>
        public enum Origin360
        {
            Bottom,
            Right,
            Top,
            Left
        }

        /// <summary>
        /// <para>Origin for the Image.FillMethod.Radial90.</para>
        /// </summary>
        public enum Origin90
        {
            BottomLeft,
            TopLeft,
            TopRight,
            BottomRight
        }

        /// <summary>
        /// <para>Origin for the Image.FillMethod.Horizontal.</para>
        /// </summary>
        public enum OriginHorizontal
        {
            Left,
            Right
        }

        /// <summary>
        /// <para>Origin for the Image.FillMethod.Vertical.</para>
        /// </summary>
        public enum OriginVertical
        {
            Bottom,
            Top
        }

        /// <summary>
        /// <para>Image Type.</para>
        /// </summary>
        public enum Type
        {
            Simple,
            Sliced,
            Tiled,
            Filled
        }
    }
}

