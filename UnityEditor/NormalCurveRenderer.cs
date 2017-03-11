namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    internal class NormalCurveRenderer : CurveRenderer
    {
        private const string kCurveRendererMeshName = "NormalCurveRendererMesh";
        private const int kMaximumLoops = 100;
        private const int kMaximumSampleCount = 50;
        private const float kSegmentWindowResolution = 1000f;
        private Bounds? m_Bounds;
        private AnimationCurve m_Curve;
        private Mesh m_CurveMesh;
        private float m_CustomRangeEnd = 0f;
        private float m_CustomRangeStart = 0f;
        private WrapMode postWrapMode = WrapMode.Once;
        private WrapMode preWrapMode = WrapMode.Once;
        private static Material s_CurveMaterial;

        public NormalCurveRenderer(AnimationCurve curve)
        {
            this.m_Curve = curve;
            if (this.m_Curve == null)
            {
                this.m_Curve = new AnimationCurve();
            }
        }

        private void AddPoints(ref List<Vector3> points, float minTime, float maxTime, float visibleMinTime, float visibleMaxTime)
        {
            Keyframe keyframe = this.m_Curve[0];
            if (keyframe.time >= minTime)
            {
                Keyframe keyframe2 = this.m_Curve[0];
                points.Add(new Vector3(this.rangeStart, keyframe2.value));
                Keyframe keyframe3 = this.m_Curve[0];
                Keyframe keyframe4 = this.m_Curve[0];
                points.Add(new Vector3(keyframe3.time, keyframe4.value));
            }
            for (int i = 0; i < (this.m_Curve.length - 1); i++)
            {
                Keyframe keyframe5 = this.m_Curve[i];
                Keyframe keyframe6 = this.m_Curve[i + 1];
                if ((keyframe6.time >= minTime) && (keyframe5.time <= maxTime))
                {
                    points.Add(new Vector3(keyframe5.time, keyframe5.value));
                    int num2 = GetSegmentResolution(visibleMinTime, visibleMaxTime, keyframe5.time, keyframe6.time);
                    float x = Mathf.Lerp(keyframe5.time, keyframe6.time, 0.001f / ((float) num2));
                    points.Add(new Vector3(x, this.m_Curve.Evaluate(x)));
                    for (float j = 1f; j < num2; j++)
                    {
                        x = Mathf.Lerp(keyframe5.time, keyframe6.time, j / ((float) num2));
                        points.Add(new Vector3(x, this.m_Curve.Evaluate(x)));
                    }
                    x = Mathf.Lerp(keyframe5.time, keyframe6.time, 1f - (0.001f / ((float) num2)));
                    points.Add(new Vector3(x, this.m_Curve.Evaluate(x)));
                    x = keyframe6.time;
                    points.Add(new Vector3(x, keyframe6.value));
                }
            }
            Keyframe keyframe7 = this.m_Curve[this.m_Curve.length - 1];
            if (keyframe7.time <= maxTime)
            {
                Keyframe keyframe8 = this.m_Curve[this.m_Curve.length - 1];
                Keyframe keyframe9 = this.m_Curve[this.m_Curve.length - 1];
                points.Add(new Vector3(keyframe8.time, keyframe9.value));
                Keyframe keyframe10 = this.m_Curve[this.m_Curve.length - 1];
                points.Add(new Vector3(this.rangeEnd, keyframe10.value));
            }
        }

        private void BuildCurveMesh()
        {
            if (this.m_CurveMesh == null)
            {
                Vector3[] points = this.GetPoints();
                this.m_CurveMesh = new Mesh();
                this.m_CurveMesh.name = "NormalCurveRendererMesh";
                this.m_CurveMesh.hideFlags |= HideFlags.DontSave;
                this.m_CurveMesh.vertices = points;
                if (points.Length > 0)
                {
                    int num = points.Length - 1;
                    int item = 0;
                    List<int> list = new List<int>(num * 2);
                    while (item < num)
                    {
                        list.Add(item);
                        list.Add(++item);
                    }
                    this.m_CurveMesh.SetIndices(list.ToArray(), MeshTopology.Lines, 0);
                }
            }
        }

        public static float[,] CalculateRanges(float minTime, float maxTime, float rangeStart, float rangeEnd, WrapMode preWrapMode, WrapMode postWrapMode)
        {
            WrapMode mode = preWrapMode;
            if (postWrapMode != mode)
            {
                return new float[,] { { rangeStart, rangeEnd } };
            }
            if (mode == WrapMode.Loop)
            {
                if ((maxTime - minTime) > (rangeEnd - rangeStart))
                {
                    return new float[,] { { rangeStart, rangeEnd } };
                }
                minTime = Mathf.Repeat(minTime - rangeStart, rangeEnd - rangeStart) + rangeStart;
                maxTime = Mathf.Repeat(maxTime - rangeStart, rangeEnd - rangeStart) + rangeStart;
                if (minTime < maxTime)
                {
                    return new float[,] { { minTime, maxTime } };
                }
                return new float[,] { { rangeStart, maxTime }, { minTime, rangeEnd } };
            }
            if (mode == WrapMode.PingPong)
            {
                return new float[,] { { rangeStart, rangeEnd } };
            }
            return new float[,] { { minTime, maxTime } };
        }

        public void DrawCurve(float minTime, float maxTime, Color color, Matrix4x4 transform, Color wrapColor)
        {
            this.BuildCurveMesh();
            Keyframe[] keys = this.m_Curve.keys;
            if (keys.Length > 0)
            {
                Vector3 firstPoint = new Vector3(this.rangeStart, keys.First<Keyframe>().value);
                Vector3 lastPoint = new Vector3(this.rangeEnd, keys.Last<Keyframe>().value);
                DrawCurveWrapped(minTime, maxTime, this.rangeStart, this.rangeEnd, this.preWrapMode, this.postWrapMode, this.m_CurveMesh, firstPoint, lastPoint, transform, color, wrapColor);
            }
        }

        public static void DrawCurveWrapped(float minTime, float maxTime, float rangeStart, float rangeEnd, WrapMode preWrap, WrapMode postWrap, Color color, Matrix4x4 transform, Vector3[] points, Color wrapColor)
        {
            if (points.Length != 0)
            {
                int num;
                int num2;
                if ((rangeEnd - rangeStart) != 0f)
                {
                    num = Mathf.FloorToInt((minTime - rangeStart) / (rangeEnd - rangeStart));
                    num2 = Mathf.CeilToInt((maxTime - rangeEnd) / (rangeEnd - rangeStart));
                    if (num < -100)
                    {
                        preWrap = WrapMode.Once;
                    }
                    if (num2 > 100)
                    {
                        postWrap = WrapMode.Once;
                    }
                }
                else
                {
                    preWrap = WrapMode.Once;
                    postWrap = WrapMode.Once;
                    num = (minTime >= rangeStart) ? 0 : -1;
                    num2 = (maxTime <= rangeEnd) ? 0 : 1;
                }
                int index = points.Length - 1;
                Handles.color = color;
                List<Vector3> list = new List<Vector3>();
                if ((num <= 0) && (num2 >= 0))
                {
                    DrawPolyLine(transform, 2f, points);
                }
                else
                {
                    Handles.DrawPolyLine(points);
                }
                Handles.color = new Color(wrapColor.r, wrapColor.g, wrapColor.b, wrapColor.a * color.a);
                if (preWrap == WrapMode.Loop)
                {
                    list = new List<Vector3>();
                    for (int i = num; i < 0; i++)
                    {
                        for (int j = 0; j < points.Length; j++)
                        {
                            Vector3 point = points[j];
                            point.x += i * (rangeEnd - rangeStart);
                            point = transform.MultiplyPoint(point);
                            list.Add(point);
                        }
                    }
                    list.Add(transform.MultiplyPoint(points[0]));
                    Handles.DrawPolyLine(list.ToArray());
                }
                else if (preWrap == WrapMode.PingPong)
                {
                    list = new List<Vector3>();
                    for (int k = num; k < 0; k++)
                    {
                        for (int m = 0; m < points.Length; m++)
                        {
                            if ((k / 2) == (((float) k) / 2f))
                            {
                                Vector3 vector2 = points[m];
                                vector2.x += k * (rangeEnd - rangeStart);
                                vector2 = transform.MultiplyPoint(vector2);
                                list.Add(vector2);
                            }
                            else
                            {
                                Vector3 vector3 = points[index - m];
                                vector3.x = (-vector3.x + ((k + 1) * (rangeEnd - rangeStart))) + (rangeStart * 2f);
                                vector3 = transform.MultiplyPoint(vector3);
                                list.Add(vector3);
                            }
                        }
                    }
                    Handles.DrawPolyLine(list.ToArray());
                }
                else if (num < 0)
                {
                    Vector3[] vectorArray1 = new Vector3[] { transform.MultiplyPoint(new Vector3(minTime, points[0].y, 0f)), transform.MultiplyPoint(new Vector3(Mathf.Min(maxTime, points[0].x), points[0].y, 0f)) };
                    Handles.DrawPolyLine(vectorArray1);
                }
                if (postWrap == WrapMode.Loop)
                {
                    list = new List<Vector3> {
                        transform.MultiplyPoint(points[index])
                    };
                    for (int n = 1; n <= num2; n++)
                    {
                        for (int num9 = 0; num9 < points.Length; num9++)
                        {
                            Vector3 vector4 = points[num9];
                            vector4.x += n * (rangeEnd - rangeStart);
                            vector4 = transform.MultiplyPoint(vector4);
                            list.Add(vector4);
                        }
                    }
                    Handles.DrawPolyLine(list.ToArray());
                }
                else if (postWrap == WrapMode.PingPong)
                {
                    list = new List<Vector3>();
                    for (int num10 = 1; num10 <= num2; num10++)
                    {
                        for (int num11 = 0; num11 < points.Length; num11++)
                        {
                            if ((num10 / 2) == (((float) num10) / 2f))
                            {
                                Vector3 vector5 = points[num11];
                                vector5.x += num10 * (rangeEnd - rangeStart);
                                vector5 = transform.MultiplyPoint(vector5);
                                list.Add(vector5);
                            }
                            else
                            {
                                Vector3 vector6 = points[index - num11];
                                vector6.x = (-vector6.x + ((num10 + 1) * (rangeEnd - rangeStart))) + (rangeStart * 2f);
                                vector6 = transform.MultiplyPoint(vector6);
                                list.Add(vector6);
                            }
                        }
                    }
                    Handles.DrawPolyLine(list.ToArray());
                }
                else if (num2 > 0)
                {
                    Vector3[] vectorArray2 = new Vector3[] { transform.MultiplyPoint(new Vector3(Mathf.Max(minTime, points[index].x), points[index].y, 0f)), transform.MultiplyPoint(new Vector3(maxTime, points[index].y, 0f)) };
                    Handles.DrawPolyLine(vectorArray2);
                }
            }
        }

        public static void DrawCurveWrapped(float minTime, float maxTime, float rangeStart, float rangeEnd, WrapMode preWrap, WrapMode postWrap, Mesh mesh, Vector3 firstPoint, Vector3 lastPoint, Matrix4x4 transform, Color color, Color wrapColor)
        {
            if ((mesh.vertexCount != 0) && (Event.current.type == EventType.Repaint))
            {
                int num;
                int num2;
                if ((rangeEnd - rangeStart) != 0f)
                {
                    num = Mathf.FloorToInt((minTime - rangeStart) / (rangeEnd - rangeStart));
                    num2 = Mathf.CeilToInt((maxTime - rangeEnd) / (rangeEnd - rangeStart));
                }
                else
                {
                    preWrap = WrapMode.Once;
                    postWrap = WrapMode.Once;
                    num = (minTime >= rangeStart) ? 0 : -1;
                    num2 = (maxTime <= rangeEnd) ? 0 : 1;
                }
                Material curveMaterial = NormalCurveRenderer.curveMaterial;
                curveMaterial.SetColor("_Color", color);
                Handles.color = color;
                curveMaterial.SetPass(0);
                Graphics.DrawMeshNow(mesh, Handles.matrix * transform);
                curveMaterial.SetColor("_Color", new Color(wrapColor.r, wrapColor.g, wrapColor.b, wrapColor.a * color.a));
                Handles.color = new Color(wrapColor.r, wrapColor.g, wrapColor.b, wrapColor.a * color.a);
                if (preWrap == WrapMode.Loop)
                {
                    Matrix4x4 matrixx = (Handles.matrix * transform) * Matrix4x4.TRS(new Vector3(num * (rangeEnd - rangeStart), 0f, 0f), Quaternion.identity, Vector3.one);
                    Matrix4x4 matrixx2 = Matrix4x4.TRS(new Vector3(rangeEnd - rangeStart, 0f, 0f), Quaternion.identity, Vector3.one);
                    curveMaterial.SetPass(0);
                    Matrix4x4 matrix = matrixx;
                    for (int i = num; i < 0; i++)
                    {
                        Graphics.DrawMeshNow(mesh, matrix);
                        matrix *= matrixx2;
                    }
                    matrix = matrixx;
                    for (int j = num; j < 0; j++)
                    {
                        Matrix4x4 matrixx4 = matrix * matrixx2;
                        Handles.DrawLine(matrix.MultiplyPoint(lastPoint), matrixx4.MultiplyPoint(firstPoint));
                        matrix = matrixx4;
                    }
                }
                else if (preWrap == WrapMode.PingPong)
                {
                    curveMaterial.SetPass(0);
                    for (int k = num; k < 0; k++)
                    {
                        if ((k % 2) == 0)
                        {
                            Matrix4x4 matrixx5 = Matrix4x4.TRS(new Vector3(k * (rangeEnd - rangeStart), 0f, 0f), Quaternion.identity, Vector3.one);
                            Graphics.DrawMeshNow(mesh, (Handles.matrix * transform) * matrixx5);
                        }
                        else
                        {
                            Matrix4x4 matrixx6 = Matrix4x4.TRS(new Vector3(((k + 1) * (rangeEnd - rangeStart)) + (rangeStart * 2f), 0f, 0f), Quaternion.identity, new Vector3(-1f, 1f, 1f));
                            Graphics.DrawMeshNow(mesh, (Handles.matrix * transform) * matrixx6);
                        }
                    }
                }
                else if (num < 0)
                {
                    Handles.DrawLine(transform.MultiplyPoint(new Vector3(minTime, firstPoint.y, 0f)), transform.MultiplyPoint(new Vector3(Mathf.Min(maxTime, firstPoint.x), firstPoint.y, 0f)));
                }
                if (postWrap == WrapMode.Loop)
                {
                    Matrix4x4 matrixx7 = Handles.matrix * transform;
                    Matrix4x4 matrixx8 = Matrix4x4.TRS(new Vector3(rangeEnd - rangeStart, 0f, 0f), Quaternion.identity, Vector3.one);
                    Matrix4x4 matrixx9 = matrixx7;
                    for (int m = 1; m <= num2; m++)
                    {
                        Matrix4x4 matrixx10 = matrixx9 * matrixx8;
                        Handles.DrawLine(matrixx9.MultiplyPoint(lastPoint), matrixx10.MultiplyPoint(firstPoint));
                        matrixx9 = matrixx10;
                    }
                    curveMaterial.SetPass(0);
                    matrixx9 = matrixx7;
                    for (int n = 1; n <= num2; n++)
                    {
                        Matrix4x4 matrixx11 = matrixx9 * matrixx8;
                        Graphics.DrawMeshNow(mesh, matrixx11);
                        matrixx9 = matrixx11;
                    }
                }
                else if (postWrap == WrapMode.PingPong)
                {
                    curveMaterial.SetPass(0);
                    for (int num8 = 1; num8 <= num2; num8++)
                    {
                        if ((num8 % 2) == 0)
                        {
                            Matrix4x4 matrixx12 = Matrix4x4.TRS(new Vector3(num8 * (rangeEnd - rangeStart), 0f, 0f), Quaternion.identity, Vector3.one);
                            Graphics.DrawMeshNow(mesh, (Handles.matrix * transform) * matrixx12);
                        }
                        else
                        {
                            Matrix4x4 matrixx13 = Matrix4x4.TRS(new Vector3(((num8 + 1) * (rangeEnd - rangeStart)) + (rangeStart * 2f), 0f, 0f), Quaternion.identity, new Vector3(-1f, 1f, 1f));
                            Graphics.DrawMeshNow(mesh, (Handles.matrix * transform) * matrixx13);
                        }
                    }
                }
                else if (num2 > 0)
                {
                    Handles.DrawLine(transform.MultiplyPoint(new Vector3(Mathf.Max(minTime, lastPoint.x), lastPoint.y, 0f)), transform.MultiplyPoint(new Vector3(maxTime, lastPoint.y, 0f)));
                }
            }
        }

        public static void DrawPolyLine(Matrix4x4 transform, float minDistance, params Vector3[] points)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Color c = Handles.color * new Color(1f, 1f, 1f, 0.75f);
                HandleUtility.ApplyWireMaterial();
                GL.PushMatrix();
                GL.MultMatrix(Handles.matrix);
                GL.Begin(1);
                GL.Color(c);
                Vector3 v = transform.MultiplyPoint(points[0]);
                for (int i = 1; i < points.Length; i++)
                {
                    Vector3 vector2 = transform.MultiplyPoint(points[i]);
                    Vector3 vector3 = v - vector2;
                    if (vector3.magnitude > minDistance)
                    {
                        GL.Vertex(v);
                        GL.Vertex(vector2);
                        v = vector2;
                    }
                }
                GL.End();
                GL.PopMatrix();
            }
        }

        public float EvaluateCurveDeltaSlow(float time)
        {
            float num = 0.0001f;
            return ((this.m_Curve.Evaluate(time + num) - this.m_Curve.Evaluate(time - num)) / (num * 2f));
        }

        public float EvaluateCurveSlow(float time) => 
            this.m_Curve.Evaluate(time);

        public void FlushCache()
        {
            UnityEngine.Object.DestroyImmediate(this.m_CurveMesh);
        }

        public Bounds GetBounds()
        {
            this.BuildCurveMesh();
            if (!this.m_Bounds.HasValue)
            {
                this.m_Bounds = new Bounds?(this.m_CurveMesh.bounds);
            }
            return this.m_Bounds.Value;
        }

        public Bounds GetBounds(float minTime, float maxTime)
        {
            Vector3[] points = this.GetPoints(minTime, maxTime);
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 vector = points[i];
                if (vector.y > negativeInfinity)
                {
                    negativeInfinity = vector.y;
                }
                if (vector.y < positiveInfinity)
                {
                    positiveInfinity = vector.y;
                }
            }
            if (positiveInfinity == float.PositiveInfinity)
            {
                positiveInfinity = 0f;
                negativeInfinity = 0f;
            }
            return new Bounds(new Vector3((maxTime + minTime) * 0.5f, (negativeInfinity + positiveInfinity) * 0.5f, 0f), new Vector3(maxTime - minTime, negativeInfinity - positiveInfinity, 0f));
        }

        public AnimationCurve GetCurve() => 
            this.m_Curve;

        private Vector3[] GetPoints() => 
            this.GetPoints(this.rangeStart, this.rangeEnd);

        private Vector3[] GetPoints(float minTime, float maxTime)
        {
            List<Vector3> points = new List<Vector3>();
            if (this.m_Curve.length != 0)
            {
                points.Capacity = 0x3e8 + this.m_Curve.length;
                float[,] numArray = CalculateRanges(minTime, maxTime, this.rangeStart, this.rangeEnd, this.preWrapMode, this.postWrapMode);
                for (int i = 0; i < numArray.GetLength(0); i++)
                {
                    this.AddPoints(ref points, numArray[i, 0], numArray[i, 1], minTime, maxTime);
                }
                if (points.Count > 0)
                {
                    for (int j = 1; j < points.Count; j++)
                    {
                        Vector3 vector = points[j];
                        Vector3 vector2 = points[j - 1];
                        if (vector.x < vector2.x)
                        {
                            points.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }
            return points.ToArray();
        }

        private static int GetSegmentResolution(float minTime, float maxTime, float keyTime, float nextKeyTime)
        {
            float num = maxTime - minTime;
            float num2 = nextKeyTime - keyTime;
            return Mathf.Clamp(Mathf.RoundToInt(1000f * (num2 / num)), 1, 50);
        }

        public float RangeEnd() => 
            this.rangeEnd;

        public float RangeStart() => 
            this.rangeStart;

        public void SetCustomRange(float start, float end)
        {
            this.m_CustomRangeStart = start;
            this.m_CustomRangeEnd = end;
        }

        public void SetWrap(WrapMode wrap)
        {
            this.preWrapMode = wrap;
            this.postWrapMode = wrap;
        }

        public void SetWrap(WrapMode preWrap, WrapMode postWrap)
        {
            this.preWrapMode = preWrap;
            this.postWrapMode = postWrap;
        }

        public static Material curveMaterial
        {
            get
            {
                if (s_CurveMaterial == null)
                {
                    Shader shader = (Shader) EditorGUIUtility.LoadRequired("Editors/AnimationWindow/Curve.shader");
                    s_CurveMaterial = new Material(shader);
                }
                return s_CurveMaterial;
            }
        }

        private float rangeEnd =>
            ((((this.m_CustomRangeStart != 0f) || (this.m_CustomRangeEnd != 0f)) || (this.m_Curve.length <= 0)) ? this.m_CustomRangeEnd : this.m_Curve.keys[this.m_Curve.length - 1].time);

        private float rangeStart =>
            ((((this.m_CustomRangeStart != 0f) || (this.m_CustomRangeEnd != 0f)) || (this.m_Curve.length <= 0)) ? this.m_CustomRangeStart : this.m_Curve.keys[0].time);
    }
}

