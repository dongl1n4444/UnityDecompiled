namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class ShapeEditorSelection : IEnumerable<int>, IEnumerable
    {
        [CompilerGenerated]
        private static Func<int, int> <>f__am$cache0;
        private HashSet<int> m_SelectedPoints = new HashSet<int>();
        private ShapeEditor m_ShapeEditor;

        public ShapeEditorSelection(ShapeEditor owner)
        {
            this.m_ShapeEditor = owner;
        }

        public void Clear()
        {
            this.m_SelectedPoints.Clear();
            if (this.m_ShapeEditor != null)
            {
                this.m_ShapeEditor.activePoint = -1;
            }
        }

        public bool Contains(int i) => 
            this.m_SelectedPoints.Contains(i);

        public void DeleteSelection()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = x => x;
            }
            IOrderedEnumerable<int> enumerable = Enumerable.OrderByDescending<int, int>(this.m_SelectedPoints, <>f__am$cache0);
            foreach (int num in enumerable)
            {
                this.m_ShapeEditor.RemovePointAt(num);
            }
            if (this.m_ShapeEditor.activePoint >= this.m_ShapeEditor.GetPointsCount())
            {
                this.m_ShapeEditor.activePoint = this.m_ShapeEditor.GetPointsCount() - 1;
            }
            this.m_SelectedPoints.Clear();
        }

        public IEnumerator<int> GetEnumerator() => 
            this.m_SelectedPoints.GetEnumerator();

        public void MoveSelection(Vector3 delta)
        {
            if (delta.sqrMagnitude >= float.Epsilon)
            {
                foreach (int num in this.m_SelectedPoints)
                {
                    this.m_ShapeEditor.SetPointPosition(num, this.m_ShapeEditor.GetPointPosition(num) + delta);
                }
            }
        }

        public void RectSelect(Rect rect, ShapeEditor.SelectionType type)
        {
            if (type == ShapeEditor.SelectionType.Normal)
            {
                this.m_SelectedPoints.Clear();
                this.m_ShapeEditor.activePoint = -1;
                type = ShapeEditor.SelectionType.Additive;
            }
            for (int i = 0; i < this.m_ShapeEditor.GetPointsCount(); i++)
            {
                Vector3 point = this.m_ShapeEditor.GetPointPosition(i);
                if (rect.Contains(point))
                {
                    this.SelectPoint(i, type);
                }
            }
            this.m_ShapeEditor.Repaint();
        }

        public void SelectPoint(int i, ShapeEditor.SelectionType type)
        {
            switch (type)
            {
                case ShapeEditor.SelectionType.Normal:
                    this.m_SelectedPoints.Clear();
                    this.m_ShapeEditor.activePoint = i;
                    this.m_SelectedPoints.Add(i);
                    break;

                case ShapeEditor.SelectionType.Additive:
                    this.m_ShapeEditor.activePoint = i;
                    this.m_SelectedPoints.Add(i);
                    break;

                case ShapeEditor.SelectionType.Subtractive:
                    this.m_ShapeEditor.activePoint = (i <= 0) ? 0 : (i - 1);
                    this.m_SelectedPoints.Remove(i);
                    break;

                default:
                    this.m_ShapeEditor.activePoint = i;
                    break;
            }
            this.m_ShapeEditor.Repaint();
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public int Count =>
            this.m_SelectedPoints.Count;

        public HashSet<int> indices =>
            this.m_SelectedPoints;
    }
}

