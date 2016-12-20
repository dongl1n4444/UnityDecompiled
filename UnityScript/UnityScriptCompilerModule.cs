namespace UnityScript
{
    using Boo.Lang.Compiler;
    using System;
    using System.Runtime.CompilerServices;

    [CompilerGlobalScope, Extension]
    public sealed class UnityScriptCompilerModule
    {
        private UnityScriptCompilerModule()
        {
        }

        [Extension]
        public static void ReplaceOptional(CompilerPipeline pipeline, Type optionalPipelineStepType, ICompilerStep step)
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

