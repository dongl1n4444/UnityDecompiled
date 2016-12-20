namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    internal class CurveEditorSelection : ScriptableObject
    {
        [SerializeField]
        private List<CurveSelection> m_SelectedCurves;

        public List<CurveSelection> selectedCurves
        {
            get
            {
                List<CurveSelection> selectedCurves;
                if (this.m_SelectedCurves != null)
                {
                    selectedCurves = this.m_SelectedCurves;
                }
                else
                {
                    selectedCurves = this.m_SelectedCurves = new List<CurveSelection>();
                }
                return selectedCurves;
            }
            set
            {
                this.m_SelectedCurves = value;
            }
        }
    }
}

