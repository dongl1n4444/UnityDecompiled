namespace UnityScript
{
    using Boo.Lang.Compiler;
    using System;
    using System.Runtime.CompilerServices;

    [CompilerGlobalScope]
    public sealed class UnityScriptCompilerModule
    {
        private UnityScriptCompilerModule()
        {
        }

        public static void ReplaceOptional(this CompilerPipeline pipeline, Type optionalPipelineStepType, ICompilerStep step)
        {
            int num = pipeline.Find(optionalPipelineStepType);
            if (num >= 0)
            {
                pipeline.RemoveAt(num);
                pipeline.Insert(num - 1, step);
            }
        }
    }
}

