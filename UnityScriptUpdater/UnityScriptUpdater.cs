namespace UnityScriptUpdater
{
    using APIUpdater.Framework.Configuration;
    using APIUpdater.Framework.Log;
    using Boo.Lang;
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.TypeSystem.Services;
    using Boo.Lang.Environments;
    using BooUpdater;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityScript;

    public class UnityScriptUpdater : BooUpdater.BooUpdater
    {
        [CompilerGenerated]
        private static ObjectFactory <>f__am$cache0;

        public UnityScriptUpdater(IConfigurationProvider configurationProvider) : base(configurationProvider)
        {
        }

        protected override BooCompiler CreateCompiler() => 
            new MyUnityScriptCompiler().GetCompiler();

        private Type FindMonoBehaviour(IEnumerable<string> references)
        {
            Assembly[] assemblyArray = base.LoadAssembliesToReference(references);
            foreach (Assembly assembly in assemblyArray)
            {
                Type type = assembly.GetType("UnityEngine.MonoBehaviour");
                if (type != null)
                {
                    return type;
                }
            }
            throw new Exception("MonoBehaviour not found");
        }

        protected override BooBasedLanguageTraits GetLanguageTraits() => 
            UnityScriptLanguageTraits.Instance;

        protected override BooUpdater.FixParsedSourceLocations NewParsedSourceLocationFixer(IAPIUpdaterListener listener, Dictionary<string, SourceFile> sources) => 
            new UnityScriptUpdater.FixParsedSourceLocations(this.TabSize, sources, listener);

        protected override ReplacingAstVisitor[] PostProcessPipeline(ReplacingAstVisitor[] pipeline, BooUpdateContext context)
        {
            Replace(pipeline, typeof(DepricatedComponentPropertyGetterReplacer), new UnityScriptDepricatedComponentPropertyGetterReplacer(context));
            Replace(pipeline, typeof(MemberReferenceRemover), new UnityScriptMemberReferenceRemover(context));
            Replace(pipeline, typeof(BooUpdater.MethodSignatureChanger), new UnityScriptUpdater.MethodSignatureChanger(context));
            Replace(pipeline, typeof(BooUpdater.StringBasedAddComponentReplacer), new UnityScriptUpdater.StringBasedAddComponentReplacer(context));
            return pipeline;
        }

        private static void Replace(DepthFirstVisitor[] pipeline, Type type, ReplacingAstVisitor replacement)
        {
            for (int i = 0; i != pipeline.Length; i++)
            {
                if (type.IsInstanceOfType(pipeline[i]))
                {
                    pipeline[i] = replacement;
                    break;
                }
            }
        }

        protected override void SetupCompilerParameters(BooUpdateContext context, IEnumerable<string> defines, IEnumerable<string> references)
        {
            base.SetupCompilerParameters(context, defines, references);
            UnityScriptCompilerParameters parameters = (UnityScriptCompilerParameters) base._compiler.Parameters;
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = (ObjectFactory) (() => new CustomTypeInferenceRuleProvider("UnityEngineInternal.TypeInferenceRuleAttribute"));
            }
            parameters.AddToEnvironment(typeof(TypeInferenceRuleProvider), <>f__am$cache0);
            parameters.ScriptMainMethod = "MyMain";
            Boo.Lang.List<string> list = new Boo.Lang.List<string> { 
                "UnityEngine",
                "UnityEditor",
                "System.Collections"
            };
            parameters.Imports = list;
            parameters.ScriptBaseType = this.FindMonoBehaviour(references);
        }

        protected override void SetupCompilerPipeline()
        {
            base.SetupCompilerPipeline();
            CompilerPipeline pipeline = UnityScriptCompiler.Pipelines.AdjustBooPipeline(base._compiler.Parameters.Pipeline);
            base._compiler.Parameters.Pipeline = pipeline;
        }

        protected override bool FixExpressionStatements =>
            true;

        protected override int TabSize =>
            8;

        private class MyUnityScriptCompiler : UnityScriptCompiler
        {
            public BooCompiler GetCompiler() => 
                base._compiler;
        }
    }
}

