namespace UnityScript.Scripting.Pipeline
{
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.Steps;
    using System;
    using UnityScript.Scripting;

    [Serializable]
    public class IntroduceImports : AbstractCompilerStep
    {
        protected EvaluationContext _evaluationContext;

        public IntroduceImports(EvaluationContext context)
        {
            this._evaluationContext = context;
        }

        public void AddImports(string[] imports)
        {
            foreach (Module module in this.CompileUnit.Modules)
            {
                int index = 0;
                string[] strArray = imports;
                int length = strArray.Length;
                while (index < length)
                {
                    module.Imports.Add(new Import(strArray[index]));
                    index++;
                }
            }
        }

        public override void Run()
        {
            string[] imports = this._evaluationContext.ScriptContainer.GetImports();
            if (imports != null)
            {
                this.AddImports(imports);
            }
        }
    }
}

