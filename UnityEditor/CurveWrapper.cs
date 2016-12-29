namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class CurveWrapper
    {
        public EditorCurveBinding binding;
        public Color color;
        public GetAxisScalarsCallback getAxisUiScalarsCallback = null;
        public int groupId = -1;
        public bool hidden = false;
        public int id = 0;
        public int listIndex = -1;
        private bool m_Changed;
        private CurveRenderer m_Renderer;
        private ISelectionBinding m_SelectionBinding;
        public bool readOnly = false;
        public int regionId = -1;
        public SelectionMode selected;
        public SetAxisScalarsCallback setAxisUiScalarsCallback = null;
        public float vRangeMax = float.PositiveInfinity;
        public float vRangeMin = float.NegativeInfinity;
        public Color wrapColorMultiplier = Color.white;

        public AnimationClip animationClip =>
            this.m_SelectionBinding?.animationClip;

        public bool animationIsEditable =>
            ((this.m_SelectionBinding == null) || this.m_SelectionBinding.animationIsEditable);

        public bool changed
        {
            get => 
                this.m_Changed;
            set
            {
                this.m_Changed = value;
                if (value && (this.renderer != null))
                {
                    this.renderer.FlushCache();
                }
            }
        }

        public bool clipIsEditable =>
            ((this.m_SelectionBinding == null) || this.m_SelectionBinding.clipIsEditable);

        public AnimationCurve curve =>
            this.renderer.GetCurve();

        public CurveRenderer renderer
        {
            get => 
                this.m_Renderer;
            set
            {
                this.m_Renderer = value;
            }
        }

        public GameObject rootGameObjet =>
            this.m_SelectionBinding?.rootGameObject;

        public ISelectionBinding selectionBindingInterface
        {
            get => 
                this.m_SelectionBinding;
            set
            {
                this.m_SelectionBinding = value;
            }
        }

        public int selectionID =>
            ((this.m_SelectionBinding == null) ? 0 : this.m_SelectionBinding.id);

        public float timeOffset =>
            ((this.m_SelectionBinding == null) ? 0f : this.m_SelectionBinding.timeOffset);

        public delegate Vector2 GetAxisScalarsCallback();

        internal enum SelectionMode
        {
            None,
            Selected,
            SemiSelected
        }

        public delegate void SetAxisScalarsCallback(Vector2 newAxisScalars);
    }
}

