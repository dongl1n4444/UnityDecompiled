namespace UnityScript.Steps
{
    using Boo.Lang;
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Steps;
    using Boo.Lang.Runtime;
    using System;
    using System.Collections;

    [Serializable]
    public class SuppressWarnings : AbstractCompilerStep
    {
        protected List<string> _suppressed;

        public SuppressWarnings(List<string> suppressed)
        {
            this._suppressed = suppressed;
        }

        public override void Run()
        {
            this.Warnings.Adding += new EventHandler<CompilerWarningEventArgs>(this.Warnings_Adding);
        }

        public void Warnings_Adding(object sender, CompilerWarningEventArgs args)
        {
            if (RuntimeServices.op_Member(args.Warning.Code, (IList) this._suppressed))
            {
                args.Cancel();
            }
        }
    }
}

