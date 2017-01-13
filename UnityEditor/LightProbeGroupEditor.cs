namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Xml.Serialization;
    using UnityEngine;

    internal class LightProbeGroupEditor : IEditablePoint
    {
        [CompilerGenerated]
        private static Func<int, int> <>f__am$cache0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private bool <drawTetrahedra>k__BackingField;
        private static readonly Color kCloudColor = new Color(0.7843137f, 0.7843137f, 0.07843138f, 0.85f);
        private static readonly Color kSelectedCloudColor = new Color(0.3f, 0.6f, 1f, 1f);
        private bool m_Editing;
        private readonly LightProbeGroup m_Group;
        private LightProbeGroupInspector m_Inspector;
        private Vector3 m_LastPosition = Vector3.zero;
        private Quaternion m_LastRotation = Quaternion.identity;
        private Vector3 m_LastScale = Vector3.one;
        private List<int> m_Selection = new List<int>();
        private LightProbeGroupSelection m_SerializedSelectedProbes;
        private bool m_ShouldRecalculateTetrahedra;
        private List<Vector3> m_SourcePositions;

        public LightProbeGroupEditor(LightProbeGroup group, LightProbeGroupInspector inspector)
        {
            this.m_Group = group;
            this.MarkTetrahedraDirty();
            this.m_SerializedSelectedProbes = ScriptableObject.CreateInstance<LightProbeGroupSelection>();
            this.m_SerializedSelectedProbes.hideFlags = HideFlags.HideAndDontSave;
            this.m_Inspector = inspector;
            this.drawTetrahedra = true;
        }

        public void AddProbe(Vector3 position)
        {
            Object[] objectsToUndo = new Object[] { this.m_Group, this.m_SerializedSelectedProbes };
            Undo.RegisterCompleteObjectUndo(objectsToUndo, "Add Probe");
            this.m_SourcePositions.Add(position);
            this.SelectProbe(this.m_SourcePositions.Count - 1);
            this.MarkTetrahedraDirty();
        }

        private static bool CanPasteProbes()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Vector3[]));
                StringReader textReader = new StringReader(GUIUtility.systemCopyBuffer);
                serializer.Deserialize(textReader);
                textReader.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void CopySelectedProbes()
        {
            IEnumerable<Vector3> enumerable = this.SelectedProbePositions();
            XmlSerializer serializer = new XmlSerializer(typeof(Vector3[]));
            StringWriter writer = new StringWriter();
            serializer.Serialize((TextWriter) writer, (from pos in enumerable select this.m_Group.transform.TransformPoint(pos)).ToArray<Vector3>());
            writer.Close();
            GUIUtility.systemCopyBuffer = writer.ToString();
        }

        public void DeselectProbes()
        {
            this.m_Selection.Clear();
            this.m_SerializedSelectedProbes.m_Selection = this.m_Selection;
        }

        private void DrawTetrahedra()
        {
            if ((Event.current.type == EventType.Repaint) && (SceneView.lastActiveSceneView != null))
            {
                LightmapVisualization.DrawTetrahedra(this.m_ShouldRecalculateTetrahedra, SceneView.lastActiveSceneView.camera.transform.position);
                this.m_ShouldRecalculateTetrahedra = false;
            }
        }

        public void DuplicateSelectedProbes()
        {
            if (this.m_Selection.Count != 0)
            {
                Object[] objectsToUndo = new Object[] { this.m_Group, this.m_SerializedSelectedProbes };
                Undo.RegisterCompleteObjectUndo(objectsToUndo, "Duplicate Probes");
                foreach (Vector3 vector in this.SelectedProbePositions())
                {
                    this.m_SourcePositions.Add(vector);
                }
                this.MarkTetrahedraDirty();
            }
        }

        private Bounds GetBounds(List<Vector3> positions)
        {
            if (positions.Count == 0)
            {
                return new Bounds();
            }
            if (positions.Count == 1)
            {
                return new Bounds(this.m_Group.transform.TransformPoint(positions[0]), new Vector3(1f, 1f, 1f));
            }
            return GeometryUtility.CalculateBounds(positions.ToArray(), this.m_Group.transform.localToWorldMatrix);
        }

        public Color GetDefaultColor() => 
            kCloudColor;

        public float GetPointScale() => 
            (10f * AnnotationUtility.iconSize);

        public Vector3 GetPosition(int idx) => 
            this.m_SourcePositions[idx];

        public IEnumerable<Vector3> GetPositions() => 
            this.m_SourcePositions;

        public Color GetSelectedColor() => 
            kSelectedCloudColor;

        public Vector3[] GetSelectedPositions()
        {
            int selectedCount = this.SelectedCount;
            Vector3[] vectorArray = new Vector3[selectedCount];
            for (int i = 0; i < selectedCount; i++)
            {
                vectorArray[i] = this.m_SourcePositions[this.m_Selection[i]];
            }
            return vectorArray;
        }

        public Vector3[] GetUnselectedPositions()
        {
            int count = this.Count;
            int selectedCount = this.SelectedCount;
            if (selectedCount == count)
            {
                return new Vector3[0];
            }
            if (selectedCount == 0)
            {
                return this.m_SourcePositions.ToArray();
            }
            bool[] flagArray = new bool[count];
            for (int i = 0; i < count; i++)
            {
                flagArray[i] = false;
            }
            for (int j = 0; j < selectedCount; j++)
            {
                flagArray[this.m_Selection[j]] = true;
            }
            Vector3[] vectorArray2 = new Vector3[count - selectedCount];
            int num5 = 0;
            for (int k = 0; k < count; k++)
            {
                if (!flagArray[k])
                {
                    vectorArray2[num5++] = this.m_SourcePositions[k];
                }
            }
            return vectorArray2;
        }

        public Vector3 GetWorldPosition(int idx) => 
            this.m_Group.transform.TransformPoint(this.m_SourcePositions[idx]);

        public void HandleEditMenuHotKeyCommands()
        {
            if ((Event.current.type == EventType.ValidateCommand) || (Event.current.type == EventType.ExecuteCommand))
            {
                bool flag = Event.current.type == EventType.ExecuteCommand;
                switch (Event.current.commandName)
                {
                    case "SoftDelete":
                    case "Delete":
                        if (flag)
                        {
                            this.RemoveSelectedProbes();
                        }
                        Event.current.Use();
                        break;

                    case "Duplicate":
                        if (flag)
                        {
                            this.DuplicateSelectedProbes();
                        }
                        Event.current.Use();
                        break;

                    case "SelectAll":
                        if (flag)
                        {
                            this.SelectAllProbes();
                        }
                        Event.current.Use();
                        break;

                    case "Cut":
                        if (flag)
                        {
                            this.CopySelectedProbes();
                            this.RemoveSelectedProbes();
                        }
                        Event.current.Use();
                        break;

                    case "Copy":
                        if (flag)
                        {
                            this.CopySelectedProbes();
                        }
                        Event.current.Use();
                        break;
                }
            }
        }

        public void MarkTetrahedraDirty()
        {
            this.m_ShouldRecalculateTetrahedra = true;
        }

        public bool OnSceneGUI(Transform transform)
        {
            if (this.m_Group.enabled)
            {
                if (Event.current.type == EventType.Layout)
                {
                    if (((this.m_LastPosition != this.m_Group.transform.position) || (this.m_LastRotation != this.m_Group.transform.rotation)) || (this.m_LastScale != this.m_Group.transform.localScale))
                    {
                        this.MarkTetrahedraDirty();
                    }
                    this.m_LastPosition = this.m_Group.transform.position;
                    this.m_LastRotation = this.m_Group.transform.rotation;
                    this.m_LastScale = this.m_Group.transform.localScale;
                }
                bool firstSelect = false;
                if ((((Event.current.type == EventType.MouseDown) && (Event.current.button == 0)) && (this.SelectedCount == 0)) && ((PointEditor.FindNearest(Event.current.mousePosition, transform, this) != -1) && !this.m_Editing))
                {
                    this.m_Inspector.StartEditMode();
                    this.m_Editing = true;
                    firstSelect = true;
                }
                bool flag4 = Event.current.type == EventType.MouseUp;
                if (this.m_Editing && PointEditor.SelectPoints(this, transform, ref this.m_Selection, firstSelect))
                {
                    Object[] objectsToUndo = new Object[] { this.m_Group, this.m_SerializedSelectedProbes };
                    Undo.RegisterCompleteObjectUndo(objectsToUndo, "Select Probes");
                }
                if (((Event.current.type == EventType.ValidateCommand) || (Event.current.type == EventType.ExecuteCommand)) && (Event.current.commandName == "Paste"))
                {
                    if ((Event.current.type == EventType.ValidateCommand) && CanPasteProbes())
                    {
                        Event.current.Use();
                    }
                    if ((Event.current.type == EventType.ExecuteCommand) && this.PasteProbes())
                    {
                        Event.current.Use();
                        this.m_Editing = true;
                    }
                }
                if (this.drawTetrahedra)
                {
                    this.DrawTetrahedra();
                }
                PointEditor.Draw(this, transform, this.m_Selection, true);
                if (this.m_Editing)
                {
                    this.HandleEditMenuHotKeyCommands();
                    if (this.m_Editing && PointEditor.MovePoints(this, transform, this.m_Selection))
                    {
                        Object[] objArray2 = new Object[] { this.m_Group, this.m_SerializedSelectedProbes };
                        Undo.RegisterCompleteObjectUndo(objArray2, "Move Probes");
                        if (LightmapVisualization.dynamicUpdateLightProbes)
                        {
                            this.MarkTetrahedraDirty();
                        }
                    }
                    if ((this.m_Editing && flag4) && !LightmapVisualization.dynamicUpdateLightProbes)
                    {
                        this.MarkTetrahedraDirty();
                    }
                }
            }
            return this.m_Editing;
        }

        private bool PasteProbes()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Vector3[]));
                StringReader textReader = new StringReader(GUIUtility.systemCopyBuffer);
                Vector3[] vectorArray = (Vector3[]) serializer.Deserialize(textReader);
                textReader.Close();
                if (vectorArray.Length == 0)
                {
                    return false;
                }
                Object[] objectsToUndo = new Object[] { this.m_Group, this.m_SerializedSelectedProbes };
                Undo.RegisterCompleteObjectUndo(objectsToUndo, "Paste Probes");
                int count = this.m_SourcePositions.Count;
                foreach (Vector3 vector in vectorArray)
                {
                    this.m_SourcePositions.Add(this.m_Group.transform.InverseTransformPoint(vector));
                }
                this.DeselectProbes();
                for (int i = count; i < (count + vectorArray.Length); i++)
                {
                    this.SelectProbe(i);
                }
                this.MarkTetrahedraDirty();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void PullProbePositions()
        {
            this.m_SourcePositions = new List<Vector3>(this.m_Group.probePositions);
            this.m_Selection = new List<int>(this.m_SerializedSelectedProbes.m_Selection);
        }

        public void PushProbePositions()
        {
            this.m_Group.probePositions = this.m_SourcePositions.ToArray();
            this.m_SerializedSelectedProbes.m_Selection = this.m_Selection;
        }

        public void RemoveSelectedProbes()
        {
            if (this.m_Selection.Count != 0)
            {
                Object[] objectsToUndo = new Object[] { this.m_Group, this.m_SerializedSelectedProbes };
                Undo.RegisterCompleteObjectUndo(objectsToUndo, "Delete Probes");
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = x => x;
                }
                IOrderedEnumerable<int> enumerable = Enumerable.OrderByDescending<int, int>(this.m_Selection, <>f__am$cache0);
                foreach (int num2 in enumerable)
                {
                    this.m_SourcePositions.RemoveAt(num2);
                }
                this.DeselectProbes();
                this.MarkTetrahedraDirty();
            }
        }

        public void SelectAllProbes()
        {
            this.DeselectProbes();
            int count = this.m_SourcePositions.Count;
            for (int i = 0; i < count; i++)
            {
                this.m_Selection.Add(i);
            }
        }

        private IEnumerable<Vector3> SelectedProbePositions() => 
            (from t in this.m_Selection select this.m_SourcePositions[t]).ToList<Vector3>();

        private void SelectProbe(int i)
        {
            if (!this.m_Selection.Contains(i))
            {
                this.m_Selection.Add(i);
            }
        }

        public void SetEditing(bool editing)
        {
            this.m_Editing = editing;
        }

        public void SetPosition(int idx, Vector3 position)
        {
            if (this.m_SourcePositions[idx] != position)
            {
                this.m_SourcePositions[idx] = position;
            }
        }

        public static void TetrahedralizeSceneProbes(out Vector3[] positions, out int[] indices)
        {
            LightProbeGroup[] groupArray = Object.FindObjectsOfType(typeof(LightProbeGroup)) as LightProbeGroup[];
            if (groupArray == null)
            {
                positions = new Vector3[0];
                indices = new int[0];
            }
            else
            {
                List<Vector3> list = new List<Vector3>();
                foreach (LightProbeGroup group in groupArray)
                {
                    Vector3[] probePositions = group.probePositions;
                    foreach (Vector3 vector in probePositions)
                    {
                        Vector3 item = group.transform.TransformPoint(vector);
                        list.Add(item);
                    }
                }
                if (list.Count == 0)
                {
                    positions = new Vector3[0];
                    indices = new int[0];
                }
                else
                {
                    Lightmapping.Tetrahedralize(list.ToArray(), out indices, out positions);
                }
            }
        }

        public void UpdateSelectedPosition(int idx, Vector3 position)
        {
            if (idx <= (this.SelectedCount - 1))
            {
                this.m_SourcePositions[this.m_Selection[idx]] = position;
            }
        }

        public Bounds bounds =>
            this.GetBounds(this.m_SourcePositions);

        public int Count =>
            this.m_SourcePositions.Count;

        public bool drawTetrahedra { get; set; }

        public int SelectedCount =>
            this.m_Selection.Count;

        public Bounds selectedProbeBounds
        {
            get
            {
                List<Vector3> positions = new List<Vector3>();
                foreach (int num in this.m_Selection)
                {
                    positions.Add(this.m_SourcePositions[num]);
                }
                return this.GetBounds(positions);
            }
        }
    }
}

