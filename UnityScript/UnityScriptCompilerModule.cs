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
            int index = pipeline.Find(optionalPipelineStepType);
            if (index >= 0)
            {
                pipeline.RemoveAt(index);
                pipeline.Insert(index - 1, step);
            }
        }
    }
}

