namespace UnityEditor
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class CurveSelection : IComparable
    {
        [SerializeField]
        public int curveID;
        [SerializeField]
        public int key;
        [SerializeField]
        public bool semiSelected;
        [SerializeField]
        public SelectionType type;

        internal CurveSelection(int curveID, int key)
        {
            this.curveID = 0;
            this.key = -1;
            this.semiSelected = false;
            this.curveID = curveID;
            this.key = key;
            this.type = SelectionType.Key;
        }

        internal CurveSelection(int curveID, int key, SelectionType type)
        {
            this.curveID = 0;
            this.key = -1;
            this.semiSelected = false;
            this.curveID = curveID;
            this.key = key;
            this.type = type;
        }

        public int CompareTo(object _other)
        {
            CurveSelection selection = (CurveSelection) _other;
            int num = this.curveID - selection.curveID;
            if (num != 0)
            {
                return num;
            }
            num = this.key - selection.key;
            if (num != 0)
            {
                return num;
            }
            return (int) (this.type - selection.type);
        }

        public override bool Equals(object _other)
        {
            CurveSelection selection = (CurveSelection) _other;
            return (((selection.curveID == this.curveID) && (selection.key == this.key)) && (selection.type == this.type));
        }

        public override int GetHashCode()
        {
            return (((this.curveID * 0x2d9) + (this.key * 0x1b)) + this.type);
        }

        internal enum SelectionType
        {
            Key,
            InTangent,
            OutTangent,
            Count
        }
    }
}

