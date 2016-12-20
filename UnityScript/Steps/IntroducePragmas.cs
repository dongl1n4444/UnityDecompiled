namespace UnityScript.Steps
{
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.Steps;
    using System;
    using System.Collections.Generic;
    using UnityScript.Core;

    [Serializable]
    public class IntroducePragmas : AbstractCompilerStep
    {
        protected IEnumerable<string> _pragmas;

        public IntroducePragmas(IEnumerable<string> pragmas)
        {
            this._pragmas = pragmas;
        }

        public override void Run()
        {
            foreach (Module module in this.get_CompileUnit().get_Modules())
            {
                foreach (string str in this._pragmas)
                {
                    Pragmas.TryToEnableOn(module, str);
                }
            }
        }
    }
}

