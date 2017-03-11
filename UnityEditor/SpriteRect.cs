namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditorInternal;
    using UnityEngine;

    [Serializable]
    internal class SpriteRect
    {
        [SerializeField]
        private SpriteAlignment m_Alignment;
        [SerializeField]
        private Vector4 m_Border;
        [SerializeField]
        private string m_Name;
        [SerializeField]
        private string m_OriginalName;
        [SerializeField]
        private List<SpriteOutline> m_Outline = new List<SpriteOutline>();
        [SerializeField]
        private List<SpriteOutline> m_PhysicsShape = new List<SpriteOutline>();
        [SerializeField]
        private Vector2 m_Pivot;
        [SerializeField]
        private Rect m_Rect;
        [SerializeField]
        private float m_TessellationDetail;

        public static List<SpriteOutline> AcquireOutline(SerializedProperty outlineSP)
        {
            List<SpriteOutline> list = new List<SpriteOutline>();
            for (int i = 0; i < outlineSP.arraySize; i++)
            {
                SpriteOutline item = new SpriteOutline();
                SerializedProperty arrayElementAtIndex = outlineSP.GetArrayElementAtIndex(i);
                for (int j = 0; j < arrayElementAtIndex.arraySize; j++)
                {
                    Vector2 point = arrayElementAtIndex.GetArrayElementAtIndex(j).vector2Value;
                    item.Add(point);
                }
                list.Add(item);
            }
            return list;
        }

        public static void ApplyOutlineChanges(SerializedProperty outlineSP, List<SpriteOutline> outline)
        {
            outlineSP.ClearArray();
            for (int i = 0; i < outline.Count; i++)
            {
                outlineSP.InsertArrayElementAtIndex(i);
                SpriteOutline outline2 = outline[i];
                SerializedProperty arrayElementAtIndex = outlineSP.GetArrayElementAtIndex(i);
                arrayElementAtIndex.ClearArray();
                for (int j = 0; j < outline2.Count; j++)
                {
                    arrayElementAtIndex.InsertArrayElementAtIndex(j);
                    arrayElementAtIndex.GetArrayElementAtIndex(j).vector2Value = outline2[j];
                }
            }
        }

        public void ApplyToSerializedProperty(SerializedProperty sp)
        {
            sp.FindPropertyRelative("m_Rect").rectValue = this.rect;
            sp.FindPropertyRelative("m_Border").vector4Value = this.border;
            sp.FindPropertyRelative("m_Name").stringValue = this.name;
            sp.FindPropertyRelative("m_Alignment").intValue = (int) this.alignment;
            sp.FindPropertyRelative("m_Pivot").vector2Value = this.pivot;
            sp.FindPropertyRelative("m_TessellationDetail").floatValue = this.tessellationDetail;
            SerializedProperty outlineSP = sp.FindPropertyRelative("m_Outline");
            outlineSP.ClearArray();
            if (this.outline != null)
            {
                ApplyOutlineChanges(outlineSP, this.outline);
            }
            SerializedProperty property2 = sp.FindPropertyRelative("m_PhysicsShape");
            property2.ClearArray();
            if (this.physicsShape != null)
            {
                ApplyOutlineChanges(property2, this.physicsShape);
            }
        }

        public void LoadFromSerializedProperty(SerializedProperty sp)
        {
            this.rect = sp.FindPropertyRelative("m_Rect").rectValue;
            this.border = sp.FindPropertyRelative("m_Border").vector4Value;
            this.name = sp.FindPropertyRelative("m_Name").stringValue;
            this.alignment = (SpriteAlignment) sp.FindPropertyRelative("m_Alignment").intValue;
            this.pivot = SpriteEditorUtility.GetPivotValue(this.alignment, sp.FindPropertyRelative("m_Pivot").vector2Value);
            this.tessellationDetail = sp.FindPropertyRelative("m_TessellationDetail").floatValue;
            SerializedProperty outlineSP = sp.FindPropertyRelative("m_Outline");
            this.outline = AcquireOutline(outlineSP);
            outlineSP = sp.FindPropertyRelative("m_PhysicsShape");
            this.physicsShape = AcquireOutline(outlineSP);
        }

        public SpriteAlignment alignment
        {
            get => 
                this.m_Alignment;
            set
            {
                this.m_Alignment = value;
            }
        }

        public Vector4 border
        {
            get => 
                this.m_Border;
            set
            {
                this.m_Border = value;
            }
        }

        public string name
        {
            get => 
                this.m_Name;
            set
            {
                this.m_Name = value;
            }
        }

        public string originalName
        {
            get
            {
                if (this.m_OriginalName == null)
                {
                    this.m_OriginalName = this.name;
                }
                return this.m_OriginalName;
            }
            set
            {
                this.m_OriginalName = value;
            }
        }

        public List<SpriteOutline> outline
        {
            get => 
                this.m_Outline;
            set
            {
                this.m_Outline = value;
            }
        }

        public List<SpriteOutline> physicsShape
        {
            get => 
                this.m_PhysicsShape;
            set
            {
                this.m_PhysicsShape = value;
            }
        }

        public Vector2 pivot
        {
            get => 
                this.m_Pivot;
            set
            {
                this.m_Pivot = value;
            }
        }

        public Rect rect
        {
            get => 
                this.m_Rect;
            set
            {
                this.m_Rect = value;
            }
        }

        public float tessellationDetail
        {
            get => 
                this.m_TessellationDetail;
            set
            {
                this.m_TessellationDetail = value;
            }
        }
    }
}

