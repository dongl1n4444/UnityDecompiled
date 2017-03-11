namespace UnityEditor
{
    using System;

    [CanEditMultipleObjects, CustomEditor(typeof(ModelImporter))]
    internal class ModelImporterEditor : AssetImporterTabbedEditor
    {
        public override bool HasPreviewGUI() => 
            (base.HasPreviewGUI() && (base.targets.Length < 2));

        internal override void OnEnable()
        {
            if (base.m_SubEditorTypes == null)
            {
                base.m_SubEditorTypes = new System.Type[] { typeof(ModelImporterModelEditor), typeof(ModelImporterRigEditor), typeof(ModelImporterClipEditor) };
                base.m_SubEditorNames = new string[] { "Model", "Rig", "Animations" };
            }
            base.OnEnable();
        }

        internal override bool showImportedObject =>
            (base.activeEditor is ModelImporterModelEditor);

        protected override bool useAssetDrawPreview =>
            false;
    }
}

