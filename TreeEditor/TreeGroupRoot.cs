namespace TreeEditor
{
    using System;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    public class TreeGroupRoot : TreeGroup
    {
        public float adaptiveLODQuality = 0.8f;
        public float aoDensity = 1f;
        public bool enableAmbientOcclusion = true;
        public bool enableMaterialOptimize = true;
        public bool enableWelding = true;
        public float groundOffset;
        public Matrix4x4 rootMatrix = Matrix4x4.identity;
        public float rootSpread = 5f;
        public int shadowTextureQuality = 3;

        public override bool CanHaveSubGroups() => 
            true;

        public void SetRootMatrix(Matrix4x4 m)
        {
            this.rootMatrix = m;
            this.rootMatrix.m03 = 0f;
            this.rootMatrix.m13 = 0f;
            this.rootMatrix.m23 = 0f;
            this.rootMatrix = MathUtils.OrthogonalizeMatrix(this.rootMatrix);
            base.nodes[0].matrix = this.rootMatrix;
        }

        public override void UpdateParameters()
        {
            base.nodes[0].size = this.rootSpread;
            base.nodes[0].matrix = this.rootMatrix;
            base.UpdateParameters();
        }

        internal override string DistributionModeString =>
            null;

        internal override string EdgeTurbulenceString =>
            null;

        internal override string FrequencyString =>
            null;

        internal override string GroupSeedString =>
            Styles.groupSeedString;

        internal override string GrowthAngleString =>
            null;

        internal override string GrowthScaleString =>
            null;

        internal override string MainTurbulenceString =>
            null;

        internal override string MainWindString =>
            null;

        internal override string TwirlString =>
            null;

        internal override string WhorledStepString =>
            null;

        private static class Styles
        {
            public static string groupSeedString = LocalizationDatabase.GetLocalizedString("Tree Seed|The global seed that affects the entire tree. Use it to randomize your tree, while keeping the general structure of it.");
        }
    }
}

