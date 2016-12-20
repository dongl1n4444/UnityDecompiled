namespace UnityEditor.BuildReporting
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;

    internal static class BuildReportHelper
    {
        private static IBuildAnalyzer m_CachedAnalyzer;
        private static BuildTarget m_CachedAnalyzerTarget;

        private static IBuildAnalyzer GetAnalyzerForTarget(BuildTarget target)
        {
            if (m_CachedAnalyzerTarget != target)
            {
                m_CachedAnalyzer = ModuleManager.GetBuildAnalyzer(target);
                m_CachedAnalyzerTarget = target;
            }
            return m_CachedAnalyzer;
        }

        public static void OnAddedExecutable(BuildReport report, int fileIndex)
        {
            IBuildAnalyzer analyzerForTarget = GetAnalyzerForTarget(report.buildTarget);
            if (analyzerForTarget != null)
            {
                analyzerForTarget.OnAddedExecutable(report, fileIndex);
            }
        }
    }
}

