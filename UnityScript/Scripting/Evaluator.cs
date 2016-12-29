namespace UnityScript.Scripting
{
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.IO;
    using Boo.Lang.Runtime;
    using System;
    using System.Reflection;
    using UnityScript;
    using UnityScript.Scripting.Pipeline;
    using UnityScript.Steps;

    [Serializable]
    public class Evaluator
    {
        protected EvaluationScriptCacheKey _cacheKey;
        protected string _code;
        protected CompilerContext _compilationResult;
        protected EvaluationContext _context;
        [NonSerialized]
        private static readonly object TaintedAnnotation = new object();

        public Evaluator(EvaluationContext context, string code)
        {
            if (code == null)
            {
                throw new ArgumentNullException("code");
            }
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            this._context = context;
            this._code = code;
            this._cacheKey = new EvaluationScriptCacheKey(context.GetType(), code);
        }

        public EvaluationScript ActivateScript(Type scriptType)
        {
            object[] args = new object[] { this._context };
            object obj1 = Activator.CreateInstance(scriptType, args);
            if (!(obj1 is EvaluationScript))
            {
            }
            EvaluationScript script = (EvaluationScript) RuntimeServices.Coerce(obj1, typeof(EvaluationScript));
            this._context.AddScript(script);
            return script;
        }

        private void AddEvaluationContextReferencesTo(UnityScriptCompiler compiler)
        {
            int index = 0;
            Assembly[] assemblyReferences = this._context.ScriptContainer.GetAssemblyReferences();
            int length = assemblyReferences.Length;
            while (index < length)
            {
                compiler.Parameters.get_References().Add(assemblyReferences[index]);
                index++;
            }
        }

        public static CompilerPipeline AdjustPipeline(EvaluationContext context, CompilerPipeline pipeline)
        {
            pipeline.InsertAfter(typeof(IntroduceUnityGlobalNamespaces), new IntroduceScriptingNamespace(context));
            pipeline.InsertAfter(typeof(IntroduceScriptingNamespace), new IntroduceImports(context));
            pipeline.InsertAfter(typeof(ApplySemantics), new IntroduceEvaluationContext(context));
            pipeline.Replace(typeof(ProcessUnityScriptMethods), new ProcessScriptingMethods(context));
            pipeline.InsertAfter(typeof(ProcessScriptingMethods), new IntroduceReturnValue());
            return pipeline;
        }

        private void CacheScript(Type type)
        {
            if (!IsTainted(this._compilationResult.get_CompileUnit()))
            {
                this.GetEvaluationDomain().CacheScript(this._cacheKey, type);
            }
        }

        private Type CompileScript()
        {
            Type cachedScript = this.GetCachedScript();
            cachedScript = this.DoCompile();
            this.CacheScript(cachedScript);
            return ((cachedScript == null) ? cachedScript : cachedScript);
        }

        private Type DoCompile()
        {
            UnityScriptCompiler compiler = new UnityScriptCompiler();
            compiler.Parameters.set_Pipeline(AdjustPipeline(this._context, UnityScriptCompiler.Pipelines.CompileToMemory()));
            compiler.Parameters.ScriptBaseType = typeof(EvaluationScript);
            compiler.Parameters.GlobalVariablesBecomeFields = false;
            compiler.Parameters.ScriptMainMethod = "Run";
            compiler.Parameters.get_Input().Add(new StringInput("script", this._code + ";"));
            compiler.Parameters.set_Debug(false);
            compiler.Parameters.set_GenerateInMemory(true);
            this.AddEvaluationContextReferencesTo(compiler);
            this._compilationResult = compiler.Run();
            if (this._compilationResult.get_Errors().Count != 0)
            {
                throw new CompilationErrorsException(this._compilationResult.get_Errors());
            }
            return this._compilationResult.get_GeneratedAssembly().GetType("script");
        }

        public static object Eval(EvaluationContext context, string code) => 
            new Evaluator(context, code).Run();

        private Type GetCachedScript() => 
            this.GetEvaluationDomain().GetCachedScript(this._cacheKey);

        private EvaluationDomain GetEvaluationDomain()
        {
            EvaluationDomain evaluationDomain = this._context.ScriptContainer.GetEvaluationDomain();
            if (evaluationDomain == null)
            {
                throw new AssertionFailedException("domain is not null");
            }
            return evaluationDomain;
        }

        public static bool IsTainted(CompileUnit cu) => 
            cu.ContainsAnnotation(TaintedAnnotation);

        public object Run()
        {
            Type scriptType = this.CompileScript();
            return this.ActivateScript(scriptType).Run();
        }

        public static void Taint(CompileUnit cu)
        {
            cu.set_Item(TaintedAnnotation, TaintedAnnotation);
        }
    }
}

