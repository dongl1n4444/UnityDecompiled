namespace UnityEditor.IMGUI.Controls
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    /// <summary>
    /// <para>Base class for a compound handle to edit a bounding volume in the Scene view.</para>
    /// </summary>
    public abstract class PrimitiveBoundsHandle
    {
        [CompilerGenerated]
        private static Handles.CapFunction <>f__mg$cache0;
        [CompilerGenerated]
        private static Handles.SizeFunction <>f__mg$cache1;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private Axes <axes>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private Color <handleColor>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Handles.CapFunction <midpointHandleDrawFunction>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Handles.SizeFunction <midpointHandleSizeFunction>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private Color <wireframeColor>k__BackingField;
        private Bounds m_Bounds;
        private Bounds m_BoundsOnClick;
        private int m_ControlIDHint;
        private int[] m_ControlIDs = new int[6];
        private static readonly float s_DefaultMidpointHandleSize = 0.03f;
        private static GUIContent s_EditModeButton;
        private static readonly int[] s_NextAxis = new int[] { 1, 2, 0 };

        /// <summary>
        /// <para>Create a new instance of the PrimitiveBoundsHandle class.</para>
        /// </summary>
        /// <param name="controlIDHint">An integer value used to generate consistent control IDs for each control handle on this instance. You may use any value you like, but should avoid using the same value for all of your PrimitiveBoundsHandle instances.</param>
        public PrimitiveBoundsHandle(int controlIDHint)
        {
            this.m_ControlIDHint = controlIDHint;
            this.handleColor = Color.white;
            this.wireframeColor = Color.white;
            this.axes = Axes.All;
        }

        private void AdjustMidpointHandleColor(Vector3 localPos, Vector3 localTangent, Vector3 localBinormal, bool isCameraInsideBox)
        {
            float a = 1f;
            if (!isCameraInsideBox && (this.axes == Axes.All))
            {
                float num2;
                Vector3 lhs = Handles.matrix.MultiplyVector(localTangent);
                Vector3 rhs = Handles.matrix.MultiplyVector(localBinormal);
                Vector3 normalized = Vector3.Cross(lhs, rhs).normalized;
                if (Camera.current.orthographic)
                {
                    num2 = Vector3.Dot(-Camera.current.transform.forward, normalized);
                }
                else
                {
                    Vector3 vector5 = Camera.current.transform.position - Handles.matrix.MultiplyPoint(localPos);
                    num2 = Vector3.Dot(vector5.normalized, normalized);
                }
                if (num2 < -0.0001f)
                {
                    a *= Handles.backfaceAlphaMultiplier;
                }
            }
            Handles.color *= new Color(1f, 1f, 1f, a);
        }

        private static float DefaultMidpointHandleSizeFunction(Vector3 position) => 
            (HandleUtility.GetHandleSize(position) * s_DefaultMidpointHandleSize);

        /// <summary>
        /// <para>A function to display this instance in the current handle camera using its current configuration.</para>
        /// </summary>
        public void DrawHandle()
        {
            for (int i = 0; i < this.m_ControlIDs.Length; i++)
            {
                this.m_ControlIDs[i] = GUIUtility.GetControlID(this.m_ControlIDHint, FocusType.Keyboard);
            }
            if (!Tools.viewToolActive)
            {
                Color color = Handles.color;
                Handles.color *= this.wireframeColor;
                if (Handles.color.a > 0f)
                {
                    this.DrawWireframe();
                }
                Vector3 min = this.m_Bounds.min;
                Vector3 max = this.m_Bounds.max;
                int hotControl = GUIUtility.hotControl;
                Handles.color = color * this.handleColor;
                Vector3 point = Handles.inverseMatrix.MultiplyPoint(Camera.current.transform.position);
                bool isCameraInsideBox = this.m_Bounds.Contains(point);
                EditorGUI.BeginChangeCheck();
                this.MidpointHandles(ref min, ref max, isCameraInsideBox);
                bool flag2 = EditorGUI.EndChangeCheck();
                if ((hotControl != GUIUtility.hotControl) && (GUIUtility.hotControl != 0))
                {
                    this.m_BoundsOnClick = this.m_Bounds;
                }
                if (flag2)
                {
                    this.m_Bounds.center = (Vector3) ((max + min) * 0.5f);
                    this.m_Bounds.size = max - min;
                    int index = 0;
                    int length = this.m_ControlIDs.Length;
                    while (index < length)
                    {
                        if (GUIUtility.hotControl == this.m_ControlIDs[index])
                        {
                            this.m_Bounds = this.OnHandleChanged((HandleDirection) index, this.m_BoundsOnClick, this.m_Bounds);
                        }
                        index++;
                    }
                    if (Event.current.shift)
                    {
                        int num5 = GUIUtility.hotControl;
                        Vector3 size = this.m_Bounds.size;
                        int num6 = 0;
                        if ((num5 == this.m_ControlIDs[2]) || (num5 == this.m_ControlIDs[3]))
                        {
                            num6 = 1;
                        }
                        if ((num5 == this.m_ControlIDs[4]) || (num5 == this.m_ControlIDs[5]))
                        {
                            num6 = 2;
                        }
                        float num7 = !Mathf.Approximately(this.m_BoundsOnClick.size[num6], 0f) ? (size[num6] / this.m_BoundsOnClick.size[num6]) : 1f;
                        int num8 = s_NextAxis[num6];
                        size[num8] = num7 * this.m_BoundsOnClick.size[num8];
                        num8 = s_NextAxis[num8];
                        size[num8] = num7 * this.m_BoundsOnClick.size[num8];
                        this.m_Bounds.size = size;
                    }
                    if (Event.current.alt)
                    {
                        this.m_Bounds.center = this.m_BoundsOnClick.center;
                    }
                }
                Handles.color = color;
            }
        }

        /// <summary>
        /// <para>Draw a wireframe shape for this instance. Subclasses must implement this method.</para>
        /// </summary>
        protected abstract void DrawWireframe();
        /// <summary>
        /// <para>Gets the current size of the bounding volume for this instance.</para>
        /// </summary>
        /// <returns>
        /// <para>The current size of the bounding volume for this instance.</para>
        /// </returns>
        protected Vector3 GetSize()
        {
            Vector3 size = this.m_Bounds.size;
            for (int i = 0; i < 3; i++)
            {
                if (!this.IsAxisEnabled(i))
                {
                    size[i] = 0f;
                }
            }
            return size;
        }

        /// <summary>
        /// <para>Gets a value indicating whether the specified axis is enabled for the current instance.</para>
        /// </summary>
        /// <param name="axis">An Axes.</param>
        /// <param name="vector3Axis">An integer corresponding to an axis on a Vector3. For example, 0 is x, 1 is y, and 2 is z.</param>
        /// <returns>
        /// <para>true if the specified axis is enabled; otherwise, false.</para>
        /// </returns>
        protected bool IsAxisEnabled(int vector3Axis)
        {
            switch (vector3Axis)
            {
                case 0:
                    return this.IsAxisEnabled(Axes.X);

                case 1:
                    return this.IsAxisEnabled(Axes.Y);

                case 2:
                    return this.IsAxisEnabled(Axes.Z);
            }
            throw new ArgumentOutOfRangeException("vector3Axis", "Must be 0, 1, or 2");
        }

        protected bool IsAxisEnabled(Axes axis) => 
            ((this.axes & axis) == axis);

        private Vector3 MidpointHandle(int id, Vector3 localPos, Vector3 localTangent, Vector3 localBinormal, bool isCameraInsideBox)
        {
            Color color = Handles.color;
            this.AdjustMidpointHandleColor(localPos, localTangent, localBinormal, isCameraInsideBox);
            if (Handles.color.a > 0f)
            {
                Handles.CapFunction function;
                Handles.SizeFunction function2;
                Vector3 normalized = Vector3.Cross(localTangent, localBinormal).normalized;
                Handles.CapFunction midpointHandleDrawFunction = this.midpointHandleDrawFunction;
                if (midpointHandleDrawFunction != null)
                {
                    function = midpointHandleDrawFunction;
                }
                else
                {
                    if (<>f__mg$cache0 == null)
                    {
                        <>f__mg$cache0 = new Handles.CapFunction(Handles.DotHandleCap);
                    }
                    function = <>f__mg$cache0;
                }
                Handles.SizeFunction midpointHandleSizeFunction = this.midpointHandleSizeFunction;
                if (midpointHandleSizeFunction != null)
                {
                    function2 = midpointHandleSizeFunction;
                }
                else
                {
                    if (<>f__mg$cache1 == null)
                    {
                        <>f__mg$cache1 = new Handles.SizeFunction(PrimitiveBoundsHandle.DefaultMidpointHandleSizeFunction);
                    }
                    function2 = <>f__mg$cache1;
                }
                localPos = Slider1D.Do(id, localPos, normalized, function2(localPos), function, SnapSettings.scale);
            }
            Handles.color = color;
            return localPos;
        }

        private void MidpointHandles(ref Vector3 minPos, ref Vector3 maxPos, bool isCameraInsideBox)
        {
            Vector3 vector5;
            Vector3 vector6;
            Vector3 right = Vector3.right;
            Vector3 up = Vector3.up;
            Vector3 forward = Vector3.forward;
            Vector3 vector4 = (Vector3) ((maxPos + minPos) * 0.5f);
            if (this.IsAxisEnabled(Axes.X))
            {
                vector5 = new Vector3(maxPos.x, vector4.y, vector4.z);
                vector6 = this.MidpointHandle(this.m_ControlIDs[0], vector5, up, forward, isCameraInsideBox);
                maxPos.x = Mathf.Max(vector6.x, minPos.x);
                vector5 = new Vector3(minPos.x, vector4.y, vector4.z);
                vector6 = this.MidpointHandle(this.m_ControlIDs[1], vector5, up, -forward, isCameraInsideBox);
                minPos.x = Mathf.Min(vector6.x, maxPos.x);
            }
            if (this.IsAxisEnabled(Axes.Y))
            {
                vector5 = new Vector3(vector4.x, maxPos.y, vector4.z);
                vector6 = this.MidpointHandle(this.m_ControlIDs[2], vector5, right, -forward, isCameraInsideBox);
                maxPos.y = Mathf.Max(vector6.y, minPos.y);
                vector5 = new Vector3(vector4.x, minPos.y, vector4.z);
                vector6 = this.MidpointHandle(this.m_ControlIDs[3], vector5, right, forward, isCameraInsideBox);
                minPos.y = Mathf.Min(vector6.y, maxPos.y);
            }
            if (this.IsAxisEnabled(Axes.Z))
            {
                vector5 = new Vector3(vector4.x, vector4.y, maxPos.z);
                vector6 = this.MidpointHandle(this.m_ControlIDs[4], vector5, up, -right, isCameraInsideBox);
                maxPos.z = Mathf.Max(vector6.z, minPos.z);
                vector5 = new Vector3(vector4.x, vector4.y, minPos.z);
                vector6 = this.MidpointHandle(this.m_ControlIDs[5], vector5, up, right, isCameraInsideBox);
                minPos.z = Mathf.Min(vector6.z, maxPos.z);
            }
        }

        protected virtual Bounds OnHandleChanged(HandleDirection handle, Bounds boundsOnClick, Bounds newBounds) => 
            newBounds;

        /// <summary>
        /// <para>Set handleColor and wireframeColor to the same value.</para>
        /// </summary>
        /// <param name="color">The color to use for the control handles and the wireframe shape.</param>
        public void SetColor(Color color)
        {
            this.handleColor = color;
            this.wireframeColor = color;
        }

        /// <summary>
        /// <para>Sets the current size of the bounding volume for this instance.</para>
        /// </summary>
        /// <param name="size">A Vector3 specifying how large the bounding volume is along all of its axes.</param>
        protected void SetSize(Vector3 size)
        {
            this.m_Bounds.size = new Vector3(Mathf.Abs(size.x), Mathf.Abs(size.y), Mathf.Abs(size.z));
        }

        /// <summary>
        /// <para>Flags specifying which axes should display control handles.</para>
        /// </summary>
        public Axes axes { get; set; }

        /// <summary>
        /// <para>Gets or sets the center of the bounding volume for the handle.</para>
        /// </summary>
        public Vector3 center
        {
            get => 
                this.m_Bounds.center;
            set
            {
                this.m_Bounds.center = value;
            }
        }

        internal static GUIContent editModeButton
        {
            get
            {
                if (s_EditModeButton == null)
                {
                    s_EditModeButton = new GUIContent(EditorGUIUtility.IconContent("EditCollider").image, EditorGUIUtility.TextContent("Edit bounding volume.\n\n - Hold Alt after clicking control handle to pin center in place.\n - Hold Shift after clicking control handle to scale uniformly.").text);
                }
                return s_EditModeButton;
            }
        }

        /// <summary>
        /// <para>Specifies the color of the control handles.</para>
        /// </summary>
        public Color handleColor { get; set; }

        /// <summary>
        /// <para>An optional Handles.CapFunction to use when displaying the control handles. Defaults to Handles.DotHandleCap if no value is specified.</para>
        /// </summary>
        public Handles.CapFunction midpointHandleDrawFunction { get; set; }

        /// <summary>
        /// <para>An optional Handles.HandleSizeFunction to specify how large the control handles should be in the space of Handles.matrix. Defaults to a fixed screen-space size.</para>
        /// </summary>
        public Handles.SizeFunction midpointHandleSizeFunction { get; set; }

        /// <summary>
        /// <para>Specifies the color of the wireframe shape.</para>
        /// </summary>
        public Color wireframeColor { get; set; }

        /// <summary>
        /// <para>A flag enumeration for specifying which axes on a PrimitiveBoundsHandle object should be enabled.</para>
        /// </summary>
        [Flags]
        public enum Axes
        {
            /// <summary>
            /// <para>All axes.</para>
            /// </summary>
            All = 7,
            /// <summary>
            /// <para>No axes.</para>
            /// </summary>
            None = 0,
            /// <summary>
            /// <para>X-axis (bit 0).</para>
            /// </summary>
            X = 1,
            /// <summary>
            /// <para>Y-axis (bit 1).</para>
            /// </summary>
            Y = 2,
            /// <summary>
            /// <para>Z-axis (bit 2).</para>
            /// </summary>
            Z = 4
        }

        protected enum HandleDirection
        {
            PositiveX,
            NegativeX,
            PositiveY,
            NegativeY,
            PositiveZ,
            NegativeZ
        }
    }
}

