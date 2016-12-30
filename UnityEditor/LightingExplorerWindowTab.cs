namespace UnityEditor
{
    using System;

    internal class LightingExplorerWindowTab
    {
        private EmissionTable m_EmissionTable;
        private SerializedPropertyTable m_LightTable;

        public LightingExplorerWindowTab(EmissionTable emissionTable)
        {
            this.m_EmissionTable = emissionTable;
        }

        public LightingExplorerWindowTab(SerializedPropertyTable lightTable)
        {
            this.m_LightTable = lightTable;
        }

        public void OnDisable()
        {
            if (this.m_LightTable != null)
            {
                this.m_LightTable.OnDisable();
            }
            else if (this.m_EmissionTable != null)
            {
                this.m_EmissionTable.OnDisable();
            }
        }

        public void OnEnable()
        {
            if (this.m_LightTable != null)
            {
                this.m_LightTable.OnEnable();
            }
            else if (this.m_EmissionTable != null)
            {
                this.m_EmissionTable.OnEnable();
            }
        }

        public void OnGUI()
        {
            EditorGUI.indentLevel++;
            if (this.m_LightTable != null)
            {
                using (new TableScoper(0, () => this.m_LightTable.OnGUI()))
                {
                }
            }
            else if (this.m_EmissionTable != null)
            {
                using (new TableScoper(0, () => this.m_EmissionTable.OnGUI()))
                {
                }
            }
            EditorGUI.indentLevel--;
        }

        public void OnHierarchyChange()
        {
            if (this.m_LightTable != null)
            {
                this.m_LightTable.OnHierarchyChange();
            }
            else if (this.m_EmissionTable != null)
            {
                this.m_EmissionTable.OnHierarchyChange();
            }
        }

        public void OnInspectorUpdate()
        {
            if (this.m_LightTable != null)
            {
                this.m_LightTable.OnInspectorUpdate();
            }
            else if (this.m_EmissionTable != null)
            {
                this.m_EmissionTable.OnInspectorUpdate();
            }
        }

        public void OnSelectionChange()
        {
            if (this.m_LightTable != null)
            {
                this.m_LightTable.OnSelectionChange();
            }
            else if (this.m_EmissionTable != null)
            {
                this.m_EmissionTable.OnSelectionChange();
            }
        }

        public bool Refresh()
        {
            bool flag = false;
            return (flag || ((this.m_LightTable != null) && this.m_LightTable.Refresh()));
        }
    }
}

