namespace UnityScript
{
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.Pipelines;
    using Boo.Lang.Compiler.Steps;
    using System;
    using UnityScript.Steps;

    [Serializable]
    public class UnityScriptCompiler
    {
        protected BooCompiler _compiler;
        protected UnityScriptCompilerParameters _parameters;

        public UnityScriptCompiler() : this(new UnityScriptCompilerParameters())
        {
        }

        public UnityScriptCompiler(UnityScriptCompilerParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }
            this._parameters = parameters;
            this._compiler = new BooCompiler(this._parameters);
        }

        public CompilerContext Run() => 
            this._compiler.Run();

        public CompilerContext Run(CompileUnit compileUnit) => 
            this._compiler.Run(compileUnit);

        public UnityScriptCompilerParameters Parameters =>
            this._parameters;

        public static class Pipelines
        {
            public static CompilerPipeline AdjustBooPipeline(CompilerPipeline pipeline)
            {
                pipeline.Insert(0, new PreProcess());
                pipeline.Replace(typeof(Parsing), new UnityScript.Steps.Parse());
                pipeline.Replace(typeof(IntroduceGlobalNamespaces), new IntroduceUnityGlobalNamespaces());
                pipeline.InsertAfter(typeof(PreErrorChecking), new ApplySemantics());
                pipeline.InsertAfter(typeof(ApplySemantics), new ApplyDefaultVisibility());
                pipeline.InsertBefore(typeof(ExpandDuckTypedExpressions), new ProcessAssignmentToDuckMembers());
                pipeline.Replace(typeof(ProcessMethodBodiesWithDuckTyping), new ProcessUnityScriptMethods());
                pipeline.InsertAfter(typeof(ProcessUnityScriptMethods), new AutoExplodeVarArgsInvocations());
                pipeline.InsertAfter(typeof(ProcessUnityScriptMethods), new ProcessEvalInvocations());
                pipeline.ReplaceOptional(typeof(ExpandDuckTypedExpressions), new ExpandUnityDuckTypedExpressions());
                pipeline.InsertBefore(typeof(EmitAssembly), new Lint());
                pipeline.InsertBefore(typeof(EmitAssembly), new EnableRawArrayIndexing());
                pipeline.InsertAfter(typeof(BindBaseTypes), new CheckBaseTypes());
                return pipeline;
            }

            public static CompilerPipeline Compile() => 
                AdjustBooPipeline(new Boo.Lang.Compiler.Pipelines.Compile());

            public static CompilerPipeline CompileToBoo() => 
                AdjustBooPipeline(new Boo.Lang.Compiler.Pipelines.CompileToBoo());

            public static CompilerPipeline CompileToFile() => 
                AdjustBooPipeline(new Boo.Lang.Compiler.Pipelines.CompileToFile());

            public static CompilerPipeline CompileToMemory() => 
                AdjustBooPipeline(new Boo.Lang.Compiler.Pipelines.CompileToMemory());

            public static CompilerPipeline Parse()
            {
                CompilerPipeline pipeline;
                CompilerPipeline pipeline1 = pipeline = RawParsing();
                pipeline.Add(new ApplySemantics());
                pipeline.Add(new ApplyDefaultVisibility());
                return pipeline;
            }

            public static CompilerPipeline RawParsing()
            {
                CompilerPipeline pipeline;
                CompilerPipeline pipeline1 = pipeline = new CompilerPipeline();
                pipeline.Add(new PreProcess());
                pipeline.Add(new UnityScript.Steps.Parse());
                return pipeline;
            }
        }
    }
}

