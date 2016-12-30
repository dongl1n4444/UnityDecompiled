namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class LightTableFilters
    {
        internal class LightModeEnum : SerializedPropertyFilters.SerializableFilter
        {
            [SerializeField]
            private Mask m_Mask = (Mask.Dynamic | Mask.Static | Mask.Stationary);

            public override bool Active() => 
                (this.m_Mask != ((Mask) (-1)));

            public override bool Filter(SerializedProperty prop) => 
                (((((int) 1) << prop.enumValueIndex) & this.m_Mask) != 0);

            public override void OnGUI(Rect r)
            {
                this.m_Mask = (Mask) EditorGUI.EnumMaskField(r, this.m_Mask);
            }

            private enum Mask
            {
                Dynamic = 4,
                Static = 2,
                Stationary = 1
            }
        }

        internal class LightTypeEnum : SerializedPropertyFilters.SerializableFilter
        {
            [SerializeField]
            private Mask m_Mask = (Mask.Area | Mask.Point | Mask.Directional | Mask.Spot);

            public override bool Active() => 
                (this.m_Mask != ((Mask) (-1)));

            public override bool Filter(SerializedProperty prop) => 
                (((((int) 1) << prop.enumValueIndex) & this.m_Mask) != 0);

            public override void OnGUI(Rect r)
            {
                this.m_Mask = (Mask) EditorGUI.EnumMaskField(r, this.m_Mask);
            }

            private enum Mask
            {
                Area = 8,
                Directional = 2,
                Point = 4,
                Spot = 1
            }
        }

        internal class ReflProbeModeEnum : SerializedPropertyFilters.SerializableFilter
        {
            [SerializeField]
            private Mask m_Mask = (Mask.Custom | Mask.Realtime | Mask.Baked);

            public override bool Active() => 
                (this.m_Mask != ((Mask) (-1)));

            public override bool Filter(SerializedProperty prop) => 
                (((((int) 1) << prop.intValue) & this.m_Mask) != 0);

            public override void OnGUI(Rect r)
            {
                this.m_Mask = (Mask) EditorGUI.EnumMaskField(r, this.m_Mask);
            }

            private enum Mask
            {
                Baked = 1,
                Custom = 4,
                Realtime = 2
            }
        }

        internal class ReflProbeProjEnum : SerializedPropertyFilters.SerializableFilter
        {
            [SerializeField]
            private Mask m_Mask = (Mask.Box | Mask.Infinite);

            public override bool Active() => 
                (this.m_Mask != ((Mask) (-1)));

            public override bool Filter(SerializedProperty prop)
            {
                int num = !prop.boolValue ? 0 : 1;
                return (((((int) 1) << num) & this.m_Mask) != 0);
            }

            public override void OnGUI(Rect r)
            {
                this.m_Mask = (Mask) EditorGUI.EnumMaskField(r, this.m_Mask);
            }

            private enum Mask
            {
                Box = 2,
                Infinite = 1
            }
        }

        internal class ShadowTypeEnum : SerializedPropertyFilters.SerializableFilter
        {
            [SerializeField]
            private Mask m_Mask = (Mask.SoftShadows | Mask.HardShadows | Mask.NoShadows);

            public override bool Active() => 
                (this.m_Mask != ((Mask) (-1)));

            public override bool Filter(SerializedProperty prop) => 
                (((((int) 1) << prop.enumValueIndex) & this.m_Mask) != 0);

            public override void OnGUI(Rect r)
            {
                this.m_Mask = (Mask) EditorGUI.EnumMaskField(r, this.m_Mask);
            }

            private enum Mask
            {
                HardShadows = 2,
                NoShadows = 1,
                SoftShadows = 4
            }
        }
    }
}

